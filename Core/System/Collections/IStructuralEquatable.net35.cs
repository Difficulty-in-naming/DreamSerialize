using System.Collections;

#if NET20 || NET30 || NET35

namespace Theraot.Core.System.Collections
{
    public interface IStructuralEquatable
    {
        bool Equals(object other, IEqualityComparer comparer);

        int GetHashCode(IEqualityComparer comparer);
    }
}

#endif