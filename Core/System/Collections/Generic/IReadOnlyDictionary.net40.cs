﻿using System.Collections.Generic;

#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Collections.Generic
{
    public partial interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }
    }

    public partial interface IReadOnlyDictionary<TKey, TValue>
    {
        TValue this[TKey key] { get; }
    }

    public partial interface IReadOnlyDictionary<TKey, TValue>
    {
        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }
}

#endif