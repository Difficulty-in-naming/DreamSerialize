#if NET20 || NET30 || NET35

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Core.System.Linq.Expressions;

namespace Theraot.Core.System.Linq
{
    public class EnumerableQuery<T> : EnumerableQuery, IOrderedQueryable<T>, IQueryProvider
    {
        private readonly QueryableEnumerable<T> _queryable;

        public EnumerableQuery(Expression expression)
        {
            _queryable = new QueryableEnumerable<T>(expression);
        }

        public EnumerableQuery(IEnumerable<T> enumerable)
        {
            _queryable = new QueryableEnumerable<T>(enumerable);
        }

        Type IQueryable.ElementType
        {
            get { return _queryable.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _queryable.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _queryable; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _queryable.CreateQuery(expression);
        }

        IQueryable<TElem> IQueryProvider.CreateQuery<TElem>(Expression expression)
        {
            return new EnumerableQuery<TElem>(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _queryable.Execute(expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _queryable.Execute<TResult>(expression);
        }

        public override string ToString()
        {
            return _queryable.ToString();
        }
    }
}

#endif