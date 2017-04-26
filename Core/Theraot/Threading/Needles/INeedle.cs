// Needed for NET40

namespace Theraot.Core.Theraot.Threading.Needles
{
    public interface INeedle<T> : IReadOnlyNeedle<T>
    {
        new T Value { get; set; }
    }
}