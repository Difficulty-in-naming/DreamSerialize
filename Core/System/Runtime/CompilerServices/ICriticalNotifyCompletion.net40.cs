﻿#if NET20 || NET30 || NET35 || NET40

using System.Security;

namespace Theraot.Core.System.Runtime.CompilerServices
{
    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }
}

#endif