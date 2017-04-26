#if NET20 || NET30 || NET35

namespace Theraot.Core.System
{
    public enum LazyThreadSafetyMode
    {
        None,
        PublicationOnly,
        ExecutionAndPublication
    }
}

#endif