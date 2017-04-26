// Needed for NET40

namespace Theraot.Core.Theraot.Threading.Needles
{
    public interface IRecyclableNeedle<T> : INeedle<T>
    {
        void Free();
    }
}