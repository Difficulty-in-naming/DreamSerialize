﻿// Needed for Workaround

using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Core.System.Threading;

namespace Theraot.Core.Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int _maxProbingHint = 128;
        private const int _statusFinished = 1;
        private const int _statusNotReady = -2;
        private const int _statusPending = -1;
        private const int _statusReady = 0;
        private static int _status = _statusNotReady;

#if !NETCOREAPP1_1
        static GCMonitor()
        {
            var currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += ReportApplicationDomainExit;
            currentAppDomain.DomainUnload += ReportApplicationDomainExit;
        }
#endif

        public static event EventHandler Collected
        {
            add
            {
                try
                {
                    Initialize();
                    Internal.CollectedEventHandlers.Add(value);
                }
                catch
                {
                    if (ReferenceEquals(value, null))
                    {
                        return;
                    }
                    throw;
                }
            }
            remove
            {
                if (Volatile.Read(ref _status) == _statusReady)
                {
                    try
                    {
                        Internal.CollectedEventHandlers.Remove(value);
                    }
                    catch
                    {
                        if (ReferenceEquals(value, null))
                        {
                            return;
                        }
                        throw;
                    }
                }
            }
        }

        public static bool FinalizingForUnload
        {
            get
            {
                // If you need to get rid of this, just set this property to return false
#if !NETCOREAPP1_1
                return AppDomain.CurrentDomain.IsFinalizingForUnload();
#else
                return false;
#endif
            }
        }

        private static void Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, _statusPending, _statusNotReady);
            switch (check)
            {
                case _statusNotReady:
                    GC.KeepAlive(new GCProbe());
                    Volatile.Write(ref _status, _statusReady);
                    break;

                case _statusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, _statusReady);
                    break;
            }
        }

#if !NETCOREAPP1_1
        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Volatile.Write(ref _status, _statusFinished);
        }
#endif

        [global::System.Diagnostics.DebuggerNonUserCode]
        private sealed class GCProbe
#if !NETCOREAPP1_1
            : CriticalFinalizerObject
#endif
        {
            ~GCProbe()
            {
                try
                {
                    // Empty
                }
                finally
                {
                    try
                    {
                        var check = Volatile.Read(ref _status);
                        if (check == _statusReady)
                        {
                            GC.ReRegisterForFinalize(this);
                            Internal.Invoke();
                        }
                    }
                    catch (Exception exception)
                    {
                        // Catch'em all - there shouldn't be exceptions here, yet we really don't want them
                        GC.KeepAlive(exception);
                    }
                }
            }
        }
    }
}