#if NET20 || NET30

namespace Theraot.Core.System.Runtime.CompilerServices
{
    public interface IStrongBox
    {
        object Value { get; set; }
    }
}

#endif