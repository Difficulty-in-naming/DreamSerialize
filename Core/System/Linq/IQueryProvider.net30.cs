#if NET20 || NET30

using Theraot.Core.System.Linq.Expressions;

namespace Theraot.Core.System.Linq
{
    public interface IQueryProvider
    {
        IQueryable CreateQuery(Expression expression);

        IQueryable<TElement> CreateQuery<TElement>(Expression expression);

        object Execute(Expression expression);

        TResult Execute<TResult>(Expression expression);
    }
}

#endif