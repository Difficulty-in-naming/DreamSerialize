﻿#if NET20 || NET30

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Core.System.Collections.Generic;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    var folded = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        folded = func(folded, enumerator.Current);
                    }
                    return folded;
                }
                else
                {
                    throw new InvalidOperationException("No elements in source list");
                }
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var folded = seed;
            foreach (var item in source)
            {
                folded = func(folded, item);
            }
            return folded;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var result = seed;
            foreach (var item in source)
            {
                result = func(result, item);
            }
            return resultSelector(result);
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var collection = source as ICollection<TSource>;
            if (collection == null)
            {
                using (var enumerator = source.GetEnumerator())
                {
                    return enumerator.MoveNext();
                }
            }
            else
            {
                return collection.Count > 0;
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            return source;
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var enumerable = source as IEnumerable<TResult>;
            if (enumerable != null)
            {
                return enumerable;
            }
            else
            {
                return CastExtracted<TResult>(source);
            }
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return ConcatExtracted(first, second);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return Contains(source, value, null);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            foreach (var item in source)
            {
                if (comparer.Equals(item, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var collection = source as ICollection<TSource>;
            if (collection == null)
            {
                var result = 0;
                using (var item = source.GetEnumerator())
                {
                    while (item.MoveNext())
                    {
                        checked
                        {
                            result++;
                        }
                    }
                }
                return result;
            }
            else
            {
                return collection.Count;
            }
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Count(source.Where(predicate));
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            var item = default(TSource);
            return DefaultIfEmpty(source, item);
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return DefaultIfEmptyExtracted(source, defaultValue);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return Distinct(source, null);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return DistinctExtracted(source, comparer);
        }

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "index < 0");
            }
            else
            {
                var list = source as IList<TSource>;
                if (list != null)
                {
                    return list[index];
                }
                var readOnlyList = source as IReadOnlyList<TSource>;
                if (readOnlyList != null)
                {
                    return readOnlyList[index];
                }
                var count = 0L;
                foreach (var item in source)
                {
                    if (index == count)
                    {
                        return item;
                    }
                    count++;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (index < 0)
            {
                return default(TSource);
            }
            else
            {
                var list = source as IList<TSource>;
                if (list != null)
                {
                    if (index < list.Count)
                    {
                        return list[index];
                    }
                    else
                    {
                        return default(TSource);
                    }
                }
                var readOnlyList = source as IReadOnlyList<TSource>;
                if (readOnlyList != null)
                {
                    if (index < readOnlyList.Count)
                    {
                        return readOnlyList[index];
                    }
                    else
                    {
                        return default(TSource);
                    }
                }
                var count = 0L;
                foreach (var item in source)
                {
                    if (index == count)
                    {
                        return item;
                    }
                    count++;
                }
                return default(TSource);
            }
        }

        public static IEnumerable<TResult> Empty<TResult>()
        {
            yield break;
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return ExceptExtracted(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return ExceptExtracted(first, second, comparer);
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var list = source as IList<TSource>;
            if (list == null)
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
            }
            else
            {
                if (list.Count != 0)
                {
                    return list[0];
                }
            }

            throw new InvalidOperationException("The source sequence is empty");
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException();
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (var item in source)
            {
                return item;
            }
            return default(TSource);
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return IntersectExtracted(first, second, EqualityComparer<TSource>.Default);
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return IntersectExtracted(first, second, comparer ?? EqualityComparer<TSource>.Default);
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var collection = source as ICollection<TSource>;
            if (collection != null && collection.Count == 0)
            {
                throw new InvalidOperationException();
            }
            else
            {
                var list = source as IList<TSource>;
                if (list == null)
                {
                    var found = false;
                    var result = default(TSource);
                    foreach (var item in source)
                    {
                        result = item;
                        found = true;
                    }
                    if (found)
                    {
                        return result;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    return list[list.Count - 1];
                }
            }
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var found = false;
            var result = default(TSource);
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                result = item;
                found = true;
            }
            if (found)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var list = source as IList<TSource>;
            if (list == null)
            {
                var found = false;
                var result = default(TSource);
                foreach (var item in source)
                {
                    result = item;
                    found = true;
                }
                if (found)
                {
                    return result;
                }
                else
                {
                    return default(TSource);
                }
            }
            else
            {
                return list.Count > 0 ? list[list.Count - 1] : default(TSource);
            }
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var result = default(TSource);
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                result = item;
            }
            return result;
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var array = source as TSource[];
            if (array == null)
            {
                long count = 0;
                using (var item = source.GetEnumerator())
                {
                    while (item.MoveNext())
                    {
                        count++;
                    }
                }
                return count;
            }
            else
            {
                return array.LongLength;
            }
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return LongCount(source.Where(predicate));
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return OfTypeExtracted<TResult>(source);
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderBy(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);
            return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Ascending);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderByDescending(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);
            return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Descending);
        }

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "count < 0");
            }
            else
            {
                return RepeatExtracted(element, count);
            }
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return ReverseExtracted(source);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            else
            {
                return Select(source, (item, i) => selector(item));
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            return SelectExtracted(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            return SelectManyExtracted(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            return SelectManyExtracted(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException("collectionSelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return SelectManyExtracted(source, collectionSelector, resultSelector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException("collectionSelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return SelectManyExtracted(source, collectionSelector, resultSelector);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            return SequenceEqualExtracted(first, second, comparer ?? EqualityComparer<TSource>.Default);
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var found = false;
            var result = default(TSource);
            foreach (var item in source)
            {
                if (found)
                {
                    throw new InvalidOperationException();
                }
                found = true;
                result = item;
            }
            if (found)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var found = false;
            var result = default(TSource);
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                if (found)
                {
                    throw new InvalidOperationException();
                }
                found = true;
                result = item;
            }
            if (found)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var found = false;
            var result = default(TSource);
            foreach (var item in source)
            {
                if (found)
                {
                    throw new InvalidOperationException();
                }
                found = true;
                result = item;
            }
            return result;
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var found = false;
            var result = default(TSource);
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                if (found)
                {
                    throw new InvalidOperationException();
                }
                found = true;
                result = item;
            }
            return result;
        }

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            return SkipWhile(source, (item, i) => i < count);
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return SkipWhile(source, (item, i) => predicate(item));
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return SkipWhileExtracted(source, predicate);
        }

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.TakeWhileExtracted(count);
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return TakeWhile(source, (item, i) => predicate(item));
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.TakeWhileExtracted(predicate);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenBy(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);
            var oe = source as OrderedEnumerable<TSource>;
            if (oe != null)
            {
                return oe.CreateOrderedEnumerable(keySelector, comparer, false);
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenByDescending(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);
            var oe = source as OrderedEnumerable<TSource>;
            if (oe != null)
            {
                return oe.CreateOrderedEnumerable(keySelector, comparer, true);
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, true);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            return ToList(source).ToArray();
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToDictionary(source, keySelector, elementSelector, null);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            var result = new Dictionary<TKey, TElement>(comparer);
            foreach (var item in source)
            {
                result.Add(keySelector(item), elementSelector(item));
            }
            return result;
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ToDictionary(source, keySelector, null);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return ToDictionary(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new List<TSource>(source);
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Lookup<TKey, TSource>.Create(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), null);
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return Lookup<TKey, TSource>.Create(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return Lookup<TKey, TElement>.Create(source, keySelector, elementSelector, null);
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return Lookup<TKey, TElement>.Create(source, keySelector, elementSelector, comparer);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return Union(first, second, null);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return Distinct(Concat(first, second), comparer);
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return Where(source, (item, i) => predicate(item));
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return WhereExtracted(source, predicate);
        }

        private static IEnumerable<TResult> CastExtracted<TResult>(IEnumerable source)
        {
            foreach (var obj in source)
            {
                yield return (TResult)obj;
            }
        }

        private static IEnumerable<TSource> ConcatExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            foreach (var item in first)
            {
                yield return item;
            }
            var enumerator = second.GetEnumerator();
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    yield return current;
                }
            }
        }

        private static IEnumerable<TSource> DefaultIfEmptyExtracted<TSource>(IEnumerable<TSource> source, TSource defaultValue)
        {
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    while (true)
                    {
                        yield return enumerator.Current;
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                    }
                }
                else
                {
                    yield return defaultValue;
                }
            }
        }

        private static IEnumerable<TSource> DistinctExtracted<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            var found = new Dictionary<TSource, object>(comparer);
            var foundNull = false;
            foreach (var item in source)
            {
                if (ReferenceEquals(item, null))
                {
                    if (foundNull)
                    {
                        continue;
                    }
                    foundNull = true;
                }
                else
                {
                    if (found.ContainsKey(item))
                    {
                        continue;
                    }
                    found.Add(item, null);
                }
                yield return item;
            }
        }

        private static IEnumerable<TSource> ExceptExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            var items = new HashSet<TSource>(second, comparer);
            foreach (var item in first)
            {
                if (items.Add(item))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<TSource> IntersectExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            var items = new HashSet<TSource>(second, comparer);
            foreach (var element in first)
            {
                if (items.Remove(element))
                {
                    yield return element;
                }
            }
        }

        private static IEnumerable<TResult> OfTypeExtracted<TResult>(IEnumerable source)
        {
            foreach (var item in source)
            {
                if (item is TResult)
                {
                    yield return (TResult)item;
                }
            }
        }

        private static IEnumerable<TResult> RepeatExtracted<TResult>(TResult element, int count)
        {
            for (var index = 0; index < count; index++)
            {
                yield return element;
            }
        }

        private static IEnumerable<TSource> ReverseExtracted<TSource>(IEnumerable<TSource> source)
        {
            var stack = new Stack<TSource>();
            foreach (var item in source)
            {
                stack.Push(item);
            }
            foreach (var item in stack)
            {
                yield return item;
            }
        }

        private static IEnumerable<TResult> SelectExtracted<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var item in source)
            {
                yield return selector(item, count);
                count++;
            }
        }

        private static IEnumerable<TResult> SelectManyExtracted<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            // NOTICE this method has no null check
            foreach (var key in source)
            {
                foreach (var item in selector(key))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<TResult> SelectManyExtracted<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var key in source)
            {
                foreach (var item in selector(key, count))
                {
                    yield return item;
                }
                count++;
            }
        }

        private static IEnumerable<TResult> SelectManyExtracted<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            // NOTICE this method has no null check
            foreach (var element in source)
            {
                foreach (var collection in collectionSelector(element))
                {
                    yield return resultSelector(element, collection);
                }
            }
        }

        private static IEnumerable<TResult> SelectManyExtracted<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var element in source)
            {
                foreach (var collection in collectionSelector(element, count))
                {
                    yield return resultSelector(element, collection);
                }
                count++;
            }
        }

        private static bool SequenceEqualExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            using (IEnumerator<TSource> firstEnumerator = first.GetEnumerator(), secondEnumerator = second.GetEnumerator())
            {
                while (firstEnumerator.MoveNext())
                {
                    if (!secondEnumerator.MoveNext())
                    {
                        return false;
                    }
                    if (!comparer.Equals(firstEnumerator.Current, secondEnumerator.Current))
                    {
                        return false;
                    }
                }
                return !secondEnumerator.MoveNext();
            }
        }

        private static IEnumerable<TSource> SkipWhileExtracted<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            // NOTICE this method has no null check
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                var count = 0;
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current, count))
                    {
                        while (true)
                        {
                            yield return enumerator.Current;
                            if (!enumerator.MoveNext())
                            {
                                yield break;
                            }
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
            }
        }

        private static IEnumerable<TSource> TakeWhileExtracted<TSource>(this IEnumerable<TSource> source, int maxCount)
        {
            if (maxCount > 0)
            {
                var count = 0;
                foreach (var item in source)
                {
                    yield return item;
                    count++;
                    if (count == maxCount)
                    {
                        break;
                    }
                }
            }
        }

        private static IEnumerable<TSource> TakeWhileExtracted<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var item in source)
            {
                if (!predicate(item, count))
                {
                    break;
                }
                yield return item;
                count++;
            }
        }

        private static IEnumerable<TSource> WhereExtracted<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var item in source)
            {
                if (!predicate(item, count))
                {
                    continue;
                }
                yield return item;
                count++;
            }
        }

        public static IEnumerable<TReturn> Zip<T1, T2, TReturn>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, TReturn> resultSelector)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            using (var enumeratorFirst = first.GetEnumerator())
            using (var enumeratorSecond = second.GetEnumerator())
            {
                while
                (
                    enumeratorFirst.MoveNext()
                    && enumeratorSecond.MoveNext()
                )
                {
                    yield return resultSelector
                    (
                        enumeratorFirst.Current,
                        enumeratorSecond.Current
                    );
                }
            }
        }
    }
}

#endif