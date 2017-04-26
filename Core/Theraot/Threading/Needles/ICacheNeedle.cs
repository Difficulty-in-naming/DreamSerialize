// Needed for NET40

namespace Theraot.Core.Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IPromise
    {
        bool TryGetValue(out T value);
    }
}