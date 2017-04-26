﻿#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Core.System.Threading.Tasks
{
    /// <summary>
    /// Represents an abstract scheduler for tasks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see> acts as the extension point for all
    /// pluggable scheduling logic.  This includes mechanisms such as how to schedule a task for execution, and
    /// how scheduled tasks should be exposed to debuggers.
    /// </para>
    /// <para>
    /// All members of the abstract <see cref="TaskScheduler"/> type are thread-safe
    /// and may be used from multiple threads concurrently.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("Id={Id}")]
    [DebuggerTypeProxy(typeof(SystemThreadingTasksTaskSchedulerDebugView))]
    public abstract partial class TaskScheduler
    {
        /// <summary>
        /// Indicates the maximum concurrency level this
        /// <see cref="TaskScheduler"/>  is able to support.
        /// </summary>
        public virtual int MaximumConcurrencyLevel
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Notifies the scheduler that a work item has made progress.
        /// </summary>
        internal virtual void NotifyWorkItemProgress()
        {
        }

        // An AppDomain-wide default manager.
        private static readonly TaskScheduler _defaultTaskScheduler = new ThreadPoolTaskScheduler();

        /// <summary>
        /// Gets the <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see>
        /// associated with the currently executing task.
        /// </summary>
        /// <remarks>
        /// When not called from within a task, <see cref="InternalCurrent"/> will return null.
        /// </remarks>
        internal static TaskScheduler InternalCurrent
        {
            get
            {
                var currentTask = Task.InternalCurrent;
                return ((currentTask != null)
                    && ((currentTask.CreationOptions & TaskCreationOptions.HideScheduler) == 0)
                    ) ? currentTask.ExecutingTaskScheduler : null;
            }
        }

        private static readonly object _unobservedTaskExceptionLockObject = new object();
        private static EventHandler<UnobservedTaskExceptionEventArgs> _unobservedTaskException;

        /// <summary>
        /// Occurs when a faulted <see cref="System.Threading.Tasks.Task"/>'s unobserved exception is about to trigger exception escalation
        /// policy, which, by default, would terminate the process.
        /// </summary>
        /// <remarks>
        /// This AppDomain-wide event provides a mechanism to prevent exception
        /// escalation policy (which, by default, terminates the process) from triggering.
        /// Each handler is passed a <see cref="T:Theraot.Core.System.Threading.Tasks.UnobservedTaskExceptionEventArgs"/>
        /// instance, which may be used to examine the exception and to mark it as observed.
        /// </remarks>
        public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException
        {
            [global::System.Security.SecurityCritical]
            add
            {
                if (value != null)
                {
                    lock (_unobservedTaskExceptionLockObject)
                    {
                        _unobservedTaskException += value;
                    }
                }
            }
            [global::System.Security.SecurityCritical]
            remove
            {
                lock (_unobservedTaskExceptionLockObject)
                {
                    _unobservedTaskException -= value;
                }
            }
        }

        internal static void PublishUnobservedTaskException(Task sender, UnobservedTaskExceptionEventArgs ueea)
        {
            // Lock this logic to prevent just-unregistered handlers from being called.
            lock (_unobservedTaskExceptionLockObject)
            {
                // Since we are under lock, it is technically no longer necessary
                // to make a copy.  It is done here for convenience.
                var handler = _unobservedTaskException;
                if (handler != null)
                {
                    handler(sender, ueea);
                }
            }
        }

        /// <summary>
        /// Provides an array of all queued <see cref="System.Threading.Tasks.Task">Task</see> instances
        /// for the debugger.
        /// </summary>
        /// <remarks>
        /// The returned array is populated through a call to <see cref="GetScheduledTasks"/>.
        /// Note that this function is only meant to be invoked by a debugger remotely.
        /// It should not be called by any other codepaths.
        /// </remarks>
        /// <returns>An array of <see cref="System.Threading.Tasks.Task">Task</see> instances.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// This scheduler is unable to generate a list of queued tasks at this time.
        /// </exception>
        internal Task[] GetScheduledTasksForDebugger()
        {
            // this can throw InvalidOperationException indicating that they are unable to provide the info
            // at the moment. We should let the debugger receive that exception so that it can indicate it in the UI
            var activeTasksSource = GetScheduledTasks();

            if (activeTasksSource == null)
            {
                return null;
            }

            // If it can be cast to an array, use it directly
            var activeTasksArray = activeTasksSource as Task[] ?? (new List<Task>(activeTasksSource)).ToArray();

            // touch all Task.Id fields so that the debugger doesn't need to do a lot of cross-proc calls to generate them
            foreach (var t in activeTasksArray)
            {
                GC.KeepAlive(t.Id);
            }

            return activeTasksArray;
        }

        /// <summary>
        /// Nested class that provides debugger view for TaskScheduler
        /// </summary>
        internal sealed class SystemThreadingTasksTaskSchedulerDebugView
        {
            private readonly TaskScheduler _taskScheduler;

            public SystemThreadingTasksTaskSchedulerDebugView(TaskScheduler scheduler)
            {
                _taskScheduler = scheduler;
            }

            // returns the scheduler's Id
            public int Id
            {
                get { return _taskScheduler.Id; }
            }

            // returns the scheduler's GetScheduledTasks
            public IEnumerable<Task> ScheduledTasks
            {
                get { return _taskScheduler.GetScheduledTasks(); }
            }
        }
    }
}

#endif