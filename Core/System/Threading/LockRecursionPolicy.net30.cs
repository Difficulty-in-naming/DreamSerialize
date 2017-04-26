using System;

#if NET20 || NET30

namespace Theraot.Core.System.Threading
{
    [Serializable]
    public enum LockRecursionPolicy
    {
        NoRecursion,
        SupportsRecursion
    }
}

#endif