#if NET20 || NET30

using System;
using System.Collections;
using Theraot.Core.System.Linq.Expressions;

namespace Theraot.Core.System.Linq
{
    public interface IQueryable : IEnumerable
    {
        Type ElementType { get; }

        Expression Expression { get; }

        IQueryProvider Provider { get; }
    }
}

#endif