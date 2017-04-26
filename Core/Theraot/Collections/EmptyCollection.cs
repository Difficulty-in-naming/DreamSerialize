// Needed for NET30

using System.Collections.ObjectModel;
using Theraot.Core.System.Collections.Generic;
using Theraot.Core.Theraot.Collections.ThreadSafe;

namespace Theraot.Core.Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptyCollection<T> : ReadOnlyCollection<T>, IReadOnlyCollection<T>
    {
        private static readonly EmptyCollection<T> _instance = new EmptyCollection<T>();

        private EmptyCollection()
            : base(ArrayReservoir<T>.EmptyArray)
        {
        }

        public static EmptyCollection<T> Instance
        {
            get { return _instance; }
        }
    }
}