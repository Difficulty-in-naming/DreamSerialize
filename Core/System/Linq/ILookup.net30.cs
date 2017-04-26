﻿#if NET20 || NET30

using System.Collections.Generic;

namespace Theraot.Core.System.Linq
{
    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        int Count { get; }

        IEnumerable<TElement> this[TKey key] { get; }

        bool Contains(TKey key);
    }
}

#endif