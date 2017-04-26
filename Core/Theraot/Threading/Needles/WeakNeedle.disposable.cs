// Needed for Workaround

using System;
using Theraot.Core.System;

namespace Theraot.Core.Theraot.Threading.Needles
{
    public partial class WeakNeedle<T>
    {
        private int _status;

        [global::System.Diagnostics.DebuggerNonUserCode]
        ~WeakNeedle()
        {
            try
            {
                // Empty
            }
            finally
            {
                Dispose(false);
            }
        }

        public bool IsDisposed
        {
            [global::System.Diagnostics.DebuggerNonUserCode]
            get { return _status == -1; }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_status == -1)
            {
                if (whenDisposed != null)
                {
                    whenDisposed.Invoke();
                }
            }
            else
            {
                if (whenNotDisposed != null)
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            global::System.Threading.Interlocked.Decrement(ref _status);
                        }
                    }
                    else
                    {
                        if (whenDisposed != null)
                        {
                            whenDisposed.Invoke();
                        }
                    }
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_status == -1)
            {
                if (whenDisposed == null)
                {
                    return default(TReturn);
                }
                return whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default(TReturn);
            }
            if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                try
                {
                    return whenNotDisposed.Invoke();
                }
                finally
                {
                    global::System.Threading.Interlocked.Decrement(ref _status);
                }
            }
            if (whenDisposed == null)
            {
                return default(TReturn);
            }
            return whenDisposed.Invoke();
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        protected virtual void Dispose(bool disposeManagedResources)
        {
            try
            {
                if (TakeDisposalExecution())
                {
                    try
                    {
                        if (disposeManagedResources)
                        {
                            ReportManagedDisposal();
                        }
                    }
                    finally
                    {
                        ReleaseExtracted();
                    }
                }
            }
            catch (Exception exception)
            {
                // Catch'em all - fields may be partially collected.
                GC.KeepAlive(exception);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        protected void ProtectedCheckDisposed(string exceptionMessegeWhenDisposed)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(exceptionMessegeWhenDisposed);
            }
        }

        protected bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        protected void ThrowDisposedexception()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        protected TReturn ThrowDisposedexception<TReturn>()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        protected bool UnDispose()
        {
            if (global::Theraot.Core.System.Threading.Volatile.Read(ref _status) == -1)
            {
                global::Theraot.Core.System.Threading.Volatile.Write(ref _status, 0);
                return true;
            }
            return false;
        }
    }
}