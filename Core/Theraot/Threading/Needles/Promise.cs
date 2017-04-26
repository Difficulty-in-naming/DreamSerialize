// Needed for Workaround

using System;
using Theraot.Core.System;
using Theraot.Core.System.Threading;

namespace Theraot.Core.Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class Promise : IWaitablePromise
    {
        private readonly int _hashCode;
        private Exception _exception;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public Promise(bool done)
        {
            _exception = null;
            _hashCode = base.GetHashCode();
            if (!done)
            {
                _waitHandle = new ManualResetEventSlim(false);
            }
        }

        public Promise(Exception exception)
        {
            _exception = exception;
            _hashCode = exception.GetHashCode();
            _waitHandle = new ManualResetEventSlim(true);
        }

        ~Promise()
        {
            ReleaseWaitHandle(false);
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        bool IPromise.IsCanceled
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get
            {
                var waitHandle = _waitHandle.Value;
                return waitHandle == null || waitHandle.IsSet;
            }
        }

        public bool IsFaulted
        {
            get { return _exception != null; }
        }

        protected IRecyclableNeedle<ManualResetEventSlim> WaitHandle
        {
            get { return _waitHandle; }
        }

        public virtual void Free()
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle == null)
            {
                _waitHandle.Value = new ManualResetEventSlim(false);
            }
            else
            {
                waitHandle.Reset();
            }
            _exception = null;
        }

        public virtual void Free(Action beforeFree)
        {
            if (beforeFree == null)
            {
                throw new ArgumentNullException("beforeFree");
            }
            var waitHandle = _waitHandle.Value;
            if (waitHandle == null || waitHandle.IsSet)
            {
                try
                {
                    beforeFree();
                }
                finally
                {
                    if (waitHandle == null)
                    {
                        _waitHandle.Value = new ManualResetEventSlim(false);
                    }
                    else
                    {
                        waitHandle.Reset();
                    }
                    _exception = null;
                }
            }
            else
            {
                waitHandle.Reset();
                _exception = null;
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void SetCompleted()
        {
            _exception = null;
            ReleaseWaitHandle(true);
        }

        public void SetError(Exception error)
        {
            _exception = error;
            ReleaseWaitHandle(true);
        }

        public override string ToString()
        {
            return IsCompleted
                ? (_exception == null
                    ? "[Done]"
                    : _exception.ToString())
                : "[Not Created]";
        }

        public virtual void Wait()
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait();
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        public virtual void Wait(CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait(cancellationToken);
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        public virtual void Wait(int milliseconds)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait(milliseconds);
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        public virtual void Wait(TimeSpan timeout)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait(timeout);
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        public virtual void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait(milliseconds, cancellationToken);
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait(timeout, cancellationToken);
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        protected void ReleaseWaitHandle(bool done)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                if (done)
                {
                    waitHandle.Set();
                }
                waitHandle.Dispose();
            }
            _waitHandle.Value = null;
        }
    }
}