// Needed for NET30

using System.Collections.Generic;
using Theraot.Core.Theraot.Collections.ThreadSafe;

namespace Theraot.Core.Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public abstract class ExtendedEnumerableBase<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _append;
        private readonly IEnumerable<T> _target;

        protected ExtendedEnumerableBase(IEnumerable<T> target, IEnumerable<T> append)
        {
            _target = target ?? ArrayReservoir<T>.EmptyArray;
            _append = append ?? ArrayReservoir<T>.EmptyArray;
        }

        protected IEnumerable<T> Append
        {
            get { return _append; }
        }

        protected IEnumerable<T> Target
        {
            get { return _target; }
        }

        public abstract IEnumerator<T> GetEnumerator();

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}