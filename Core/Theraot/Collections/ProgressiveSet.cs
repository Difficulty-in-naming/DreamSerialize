// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Core.System.Collections.Generic;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Collections
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class ProgressiveSet<T> : ProgressiveCollection<T>, ISet<T>
    {
        // Note: these constructors uses ExtendedSet because HashSet is not an ISet<T> in .NET 3.5 and base class needs an ISet<T>
        public ProgressiveSet(IEnumerable<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(Progressor<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        public ProgressiveSet(Progressor<T> wrapped, IEqualityComparer<T> comparer)
           : this(wrapped, new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        protected ProgressiveSet(IEnumerable<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : this((IEnumerator<T>) Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), cache, comparer)
        {
            // Empty
        }

        protected ProgressiveSet(Progressor<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : base
            (
                (out T value) =>
                {
                    again:
                    if (wrapped.TryTake(out value))
                    {
                        if (cache.Contains(value))
                        {
                            goto again;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                },
                cache,
                comparer
            )
        {
            // Empty
        }

        private ProgressiveSet(IEnumerator<T> enumerator, ISet<T> cache, IEqualityComparer<T> comparer)
            : base
            (
                (out T value) =>
                {
                    again:
                    if (enumerator.MoveNext())
                    {
                        value = enumerator.Current;
                        if (cache.Contains(value))
                        {
                            goto again;
                        }
                        return true;
                    }
                    else
                    {
                        enumerator.Dispose();
                        value = default(T);
                        return false;
                    }
                },
                cache,
                comparer
            )
        {
            // Empty
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }
    }
}