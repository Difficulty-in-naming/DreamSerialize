﻿#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Theraot.Core.System.Diagnostics.Contracts;
using Theraot.Core.System.Linq;
using Theraot.Core.System.Runtime.ExceptionServices;
using Theraot.Core.Theraot.Threading;

namespace Theraot.Core.System.Threading.Tasks
{
    /// <summary>
    /// An exception holder manages a list of exceptions for one particular task.
    /// It offers the ability to aggregate, but more importantly, also offers intrinsic
    /// support for propagating unhandled exceptions that are never observed. It does
    /// this by aggregating and throwing if the holder is ever GC'd without the holder's
    /// contents ever having been requested (e.g. by a Task.Wait, Task.get_Exception, etc).
    /// This behavior is prominent in .NET 4 but is suppressed by default beyond that release.
    /// </summary>
    internal class TaskExceptionHolder
    {
        /// <summary>Whether we should propagate exceptions on the finalizer.</summary>
        private static readonly bool _failFastOnUnobservedException = ShouldFailFastOnUnobservedException();

        /// <summary>An event handler used to notify of domain unload.</summary>
        private static EventHandler _adUnloadEventHandler;

        /// <summary>Whether the AppDomain has started to unload.</summary>
        private static volatile bool _domainUnloadStarted;

        /// <summary>The task with which this holder is associated.</summary>
        private readonly Task _task;

        /// <summary>An exception that triggered the task to cancel.</summary>
        private ExceptionDispatchInfo _cancellationException;

        /// <summary>
        /// The lazily-initialized list of faulting exceptions.  Volatile
        /// so that it may be read to determine whether any exceptions were stored.
        /// </summary>
        private volatile List<ExceptionDispatchInfo> _faultExceptions;

        /// <summary>Whether the holder was "observed" and thus doesn't cause finalization behavior.</summary>
        private volatile bool _isHandled;

        /// <summary>
        /// Creates a new holder; it will be registered for finalization.
        /// </summary>
        /// <param name="task">The task this holder belongs to.</param>
        internal TaskExceptionHolder(Task task)
        {
            Contract.Requires(task != null, "Expected a non-null task.");
            _task = task;
            EnsureAppDomainUnloadCallbackRegistered();
        }

        /// <summary>
        /// A finalizer that repropagates unhandled exceptions.
        /// </summary>
        ~TaskExceptionHolder()
        {
            // Raise unhandled exceptions only when we know that neither the process or nor the appdomain is being torn down.
            // We need to do this filtering because all TaskExceptionHolders will be finalized during shutdown or unload
            // regardles of reachability of the task (i.e. even if the user code was about to observe the task's exception),
            // which can otherwise lead to spurious crashes during shutdown.
            if (_faultExceptions != null && !_isHandled &&
                !Environment.HasShutdownStarted && !GCMonitor.FinalizingForUnload && !_domainUnloadStarted)
            {
                // We don't want to crash the finalizer thread if any ThreadAbortExceptions
                // occur in the list or in any nested AggregateExceptions.
                // (Don't rethrow ThreadAbortExceptions.)
                foreach (var edi in _faultExceptions)
                {
                    var exp = edi.SourceException;
                    var aggExp = exp as AggregateException;
                    if (aggExp != null)
                    {
                        var flattenedAggExp = aggExp.Flatten();
                        foreach (var innerExp in flattenedAggExp.InnerExceptions)
                        {
                            if (innerExp is ThreadAbortException)
                            {
                                return;
                            }
                        }
                    }
                    else if (exp is ThreadAbortException)
                    {
                        return;
                    }
                }
                var exceptionToThrow = CreateAggregateException();
                var ueea = new UnobservedTaskExceptionEventArgs(exceptionToThrow);
                TaskScheduler.PublishUnobservedTaskException(_task, ueea);

                // Now, if we are still unobserved and we're configured to crash on unobserved, throw the exception.
                // We need to publish the event above even if we're not going to crash, hence
                // why this check doesn't come at the beginning of the method.
                if (_failFastOnUnobservedException && !ueea.Observed)
                {
                    throw exceptionToThrow;
                }
            }
        }

        /// <summary>Gets whether the exception holder is currently storing any exceptions for faults.</summary>
        internal bool ContainsFaultList
        {
            get { return _faultExceptions != null; }
        }

        /// <summary>
        /// Add an exception to the holder.  This will ensure the holder is
        /// in the proper state (handled/unhandled) depending on the list's contents.
        /// </summary>
        /// <param name="representsCancellation">
        /// Whether the exception represents a cancellation request (true) or a fault (false).
        /// </param>
        /// <param name="exceptionObject">
        /// An exception object (either an Exception, an ExceptionDispatchInfo,
        /// an IEnumerable{Exception}, or an IEnumerable{ExceptionDispatchInfo})
        /// to add to the list.
        /// </param>
        /// <remarks>
        /// Must be called under lock.
        /// </remarks>
        internal void Add(object exceptionObject, bool representsCancellation)
        {
            Contract.Requires(exceptionObject != null, "TaskExceptionHolder.Add(): Expected a non-null exceptionObject");
            Contract.Requires(
                exceptionObject is Exception || exceptionObject is IEnumerable<Exception> ||
                exceptionObject is ExceptionDispatchInfo || exceptionObject is IEnumerable<ExceptionDispatchInfo>,
                "TaskExceptionHolder.Add(): Expected Exception, IEnumerable<Exception>, ExceptionDispatchInfo, or IEnumerable<ExceptionDispatchInfo>");

            if (representsCancellation)
            {
                SetCancellationException(exceptionObject);
            }
            else
            {
                AddFaultException(exceptionObject);
            }
        }

        /// <summary>
        /// Allocates a new aggregate exception and adds the contents of the list to
        /// it. By calling this method, the holder assumes exceptions to have been
        /// "observed", such that the finalization check will be subsequently skipped.
        /// </summary>
        /// <param name="calledFromFinalizer">Whether this is being called from a finalizer.</param>
        /// <param name="includeThisException">An extra exception to be included (optionally).</param>
        /// <returns>The aggregate exception to throw.</returns>
        internal AggregateException CreateExceptionObject(bool calledFromFinalizer, Exception includeThisException)
        {
            var exceptions = _faultExceptions;
            Debug.Assert(exceptions != null, "Expected an initialized list.");
            Debug.Assert(exceptions.Count > 0, "Expected at least one exception.");

            // Mark as handled and aggregate the exceptions.
            MarkAsHandled(calledFromFinalizer);

            // If we're only including the previously captured exceptions,
            // return them immediately in an aggregate.
            if (includeThisException == null)
            {
                return new AggregateException(exceptions.Select(exceptionDispatchInfo => exceptionDispatchInfo.SourceException));
            }

            // Otherwise, the caller wants a specific exception to be included,
            // so return an aggregate containing that exception and the rest.
            var combinedExceptions = new Exception[exceptions.Count + 1];
            for (var i = 0; i < combinedExceptions.Length - 1; i++)
            {
                combinedExceptions[i] = exceptions[i].SourceException;
            }
            combinedExceptions[combinedExceptions.Length - 1] = includeThisException;
            return new AggregateException(combinedExceptions);
        }

        /// <summary>
        /// Gets the ExceptionDispatchInfo representing the singular exception
        /// that was the cause of the task's cancellation.
        /// </summary>
        /// <returns>
        /// The ExceptionDispatchInfo for the cancellation exception.  May be null.
        /// </returns>
        internal ExceptionDispatchInfo GetCancellationExceptionDispatchInfo()
        {
            var edi = _cancellationException;
            Debug.Assert(edi == null || edi.SourceException is OperationCanceledException,
                "Expected the EDI to be for an OperationCanceledException");
            return edi;
        }

        /// <summary>
        /// Wraps the exception dispatch infos into a new read-only collection. By calling this method,
        /// the holder assumes exceptions to have been "observed", such that the finalization
        /// check will be subsequently skipped.
        /// </summary>
        internal ReadOnlyCollection<ExceptionDispatchInfo> GetExceptionDispatchInfos()
        {
            var exceptions = _faultExceptions;
            Debug.Assert(exceptions != null, "Expected an initialized list.");
            Debug.Assert(exceptions.Count > 0, "Expected at least one exception.");
            MarkAsHandled(false);
            return new ReadOnlyCollection<ExceptionDispatchInfo>(exceptions);
        }

        /// <summary>
        /// A private helper method that ensures the holder is considered
        /// handled, i.e. it is not registered for finalization.
        /// </summary>
        /// <param name="calledFromFinalizer">Whether this is called from the finalizer thread.</param>
        internal void MarkAsHandled(bool calledFromFinalizer)
        {
            if (!_isHandled)
            {
                if (!calledFromFinalizer)
                {
                    GC.SuppressFinalize(this);
                }

                _isHandled = true;
            }
        }

        private static void AppDomainUnloadCallback(object sender, EventArgs e)
        {
            _domainUnloadStarted = true;
        }

        private static void EnsureAppDomainUnloadCallbackRegistered()
        {
            if (Volatile.Read(ref _adUnloadEventHandler) == null)
            {
                EventHandler handler = AppDomainUnloadCallback;
                if (Interlocked.CompareExchange(ref _adUnloadEventHandler, handler, null) == null)
                {
                    AppDomain.CurrentDomain.DomainUnload += handler;
                }
            }
        }

        private static bool ShouldFailFastOnUnobservedException()
        {
            return false;
        }

        /// <summary>Adds the exception to the fault list.</summary>
        /// <param name="exceptionObject">The exception to store.</param>
        /// <remarks>
        /// Must be called under lock.
        /// </remarks>
        private void AddFaultException(object exceptionObject)
        {
            Contract.Requires(exceptionObject != null, "AddFaultException(): Expected a non-null exceptionObject");

            // Initialize the exceptions list if necessary.  The list should be non-null iff it contains exceptions.
            var exceptions = _faultExceptions;
            if (exceptions == null)
            {
                _faultExceptions = exceptions = new List<ExceptionDispatchInfo>(1);
            }
            else
            {
                Debug.Assert(exceptions.Count > 0, "Expected existing exceptions list to have > 0 exceptions.");
            }

            // Handle Exception by capturing it into an ExceptionDispatchInfo and storing that
            var exception = exceptionObject as Exception;
            if (exception != null)
            {
                exceptions.Add(ExceptionDispatchInfo.Capture(exception));
            }
            else
            {
                // Handle ExceptionDispatchInfo by storing it into the list
                var edi = exceptionObject as ExceptionDispatchInfo;
                if (edi != null)
                {
                    exceptions.Add(edi);
                }
                else
                {
                    // Handle enumerables of exceptions by capturing each of the contained exceptions into an EDI and storing it
                    var exColl = exceptionObject as IEnumerable<Exception>;
                    if (exColl != null)
                    {
#if DEBUG
                        var numExceptions = 0;
#endif
                        foreach (var exc in exColl)
                        {
#if DEBUG
                            Debug.Assert(exc != null, "No exceptions should be null");
                            numExceptions++;
#endif
                            exceptions.Add(ExceptionDispatchInfo.Capture(exc));
                        }
#if DEBUG
                        Debug.Assert(numExceptions > 0, "Collection should contain at least one exception.");
#endif
                    }
                    else
                    {
                        // Handle enumerables of EDIs by storing them directly
                        var ediColl = exceptionObject as IEnumerable<ExceptionDispatchInfo>;
                        if (ediColl != null)
                        {
                            exceptions.AddRange(ediColl);
#if DEBUG
                            Debug.Assert(exceptions.Count > 0, "There should be at least one dispatch info.");
                            foreach (var tmp in exceptions)
                            {
                                Debug.Assert(tmp != null, "No dispatch infos should be null");
                            }
#endif
                        }
                        // Anything else is a programming error
                        else
                        {
                            throw new ArgumentException("(Internal)Expected an Exception or an IEnumerable<Exception>", "exceptionObject");
                        }
                    }
                }
            }

            // If all of the exceptions are ThreadAbortExceptions and/or
            // AppDomainUnloadExceptions, we do not want the finalization
            // probe to propagate them, so we consider the holder to be
            // handled.  If a subsequent exception comes in of a different
            // kind, we will reactivate the holder.
            for (var i = 0; i < exceptions.Count; i++)
            {
                var t = exceptions[i].SourceException.GetType();
                if (t != typeof(ThreadAbortException) && t != typeof(AppDomainUnloadedException))
                {
                    MarkAsUnhandled();
                    break;
                }
                else if (i == exceptions.Count - 1)
                {
                    MarkAsHandled(false);
                }
            }
        }

        private AggregateException CreateAggregateException()
        {
            // We will only propagate if this is truly unhandled. The reason this could
            // ever occur is somewhat subtle: if a Task's exceptions are observed in some
            // other finalizer, and the Task was finalized before the holder, the holder
            // will have been marked as handled before even getting here.

            // Give users a chance to keep this exception from crashing the process

            // First, publish the unobserved exception and allow users to observe it
            return new AggregateException(
                "A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread.",
                _faultExceptions.Select(exceptionDispatchInfo => exceptionDispatchInfo.SourceException));
        }

        /// <summary>
        /// A private helper method that ensures the holder is considered
        /// unhandled, i.e. it is registered for finalization.
        /// </summary>
        private void MarkAsUnhandled()
        {
            // If a thread partially observed this thread's exceptions, we
            // should revert back to "not handled" so that subsequent exceptions
            // must also be seen. Otherwise, some could go missing. We also need
            // to reregister for finalization.
            if (_isHandled)
            {
                GC.ReRegisterForFinalize(this);
                _isHandled = false;
            }
        }

        /// <summary>Sets the cancellation exception.</summary>
        /// <param name="exceptionObject">The cancellation exception.</param>
        /// <remarks>
        /// Must be called under lock.
        /// </remarks>
        private void SetCancellationException(object exceptionObject)
        {
            Contract.Requires(exceptionObject != null, "Expected exceptionObject to be non-null.");

            Debug.Assert(_cancellationException == null,
                "Expected SetCancellationException to be called only once.");
            // Breaking this assumption will overwrite a previously OCE,
            // and implies something may be wrong elsewhere, since there should only ever be one.

            Debug.Assert(_faultExceptions == null,
                "Expected SetCancellationException to be called before any faults were added.");
            // Breaking this assumption shouldn't hurt anything here, but it implies something may be wrong elsewhere.
            // If this changes, make sure to only conditionally mark as handled below.

            // Store the cancellation exception
            var oce = exceptionObject as OperationCanceledException;
            if (oce != null)
            {
                _cancellationException = ExceptionDispatchInfo.Capture(oce);
            }
            else
            {
                var edi = exceptionObject as ExceptionDispatchInfo;
                Debug.Assert(edi != null && edi.SourceException is OperationCanceledException,
                    "Expected an OCE or an EDI that contained an OCE");
                _cancellationException = edi;
            }

            // This is just cancellation, and there are no faults, so mark the holder as handled.
            MarkAsHandled(false);
        }
    }
}

#endif