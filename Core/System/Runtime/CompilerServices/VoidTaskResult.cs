#if NET20 || NET30 || NET35 || NET40

using System.Runtime.InteropServices;

namespace Theraot.Core.System.Runtime.CompilerServices
{
    /// <summary>
    /// Used with Task(of void)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct VoidTaskResult
    {
    }
}

#endif