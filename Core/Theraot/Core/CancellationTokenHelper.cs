using System;
using Theraot.Core.System.Threading;

namespace Theraot.Core.Theraot.Core
{
    public static class CancellationTokenHelper
    {
        public static void ThrowIfSourceDisposed(this CancellationToken cancellationToken)
        {
            //CancellationToken.WaitHandle is documented to throw ObjectDispodesException if the source of the CancellationToken is disposed.
            GC.KeepAlive(cancellationToken.WaitHandle);
        }
    }
}