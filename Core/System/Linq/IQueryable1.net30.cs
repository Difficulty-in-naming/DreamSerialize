#if NET20 || NET30

using System.Collections.Generic;

namespace Theraot.Core.System.Linq
{
    public interface IQueryable<T> : IQueryable, IEnumerable<T>
    {
        //Empty
    }
}

#endif