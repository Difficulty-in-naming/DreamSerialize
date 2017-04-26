#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Runtime.CompilerServices
{
    /// <summary>
    /// Represents an asynchronous method builder.
    /// </summary>
    internal interface IAsyncMethodBuilder
    {
        void PreBoxInitialization();
    }
}

#endif