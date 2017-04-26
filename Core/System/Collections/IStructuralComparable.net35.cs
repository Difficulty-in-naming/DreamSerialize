using System.Collections;

#if NET20 || NET30 || NET35

namespace Theraot.Core.System.Collections
{
    public interface IStructuralComparable
    {
        int CompareTo(object other, IComparer comparer);
    }
}

#endif