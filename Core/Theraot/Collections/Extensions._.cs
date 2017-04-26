﻿// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Core.System;
using Theraot.Core.System.Linq;
using Theraot.Core.Theraot.Collections.Specialized;
using Theraot.Core.Theraot.Collections.ThreadSafe;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class Extensions
    {
        public static void Add<T>(this Stack<T> stack, T item)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }
            stack.Push(item);
        }

        public static void Add<T>(this Queue<T> queue, T item)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            queue.Enqueue(item);
        }

        public static T[] AddFirst<T>(this IList<T> list, T item)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var res = new T[list.Count + 1];
            res[0] = item;
            list.CopyTo(res, 1);
            return res;
        }

        public static void CanCopyTo(int count, Array array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

        public static void CanCopyTo(int count, Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

        public static void CanCopyTo<T>(int count, T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

        public static void CanCopyTo<T>(int count, T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

        public static void CanCopyTo<T>(T[] array, int arrayIndex, int countLimit)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
            }
            if (countLimit > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> target)
        {
            return new List<T>(target);
        }

        public static void Consume<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (var element in source)
            {
                GC.KeepAlive(element);
            }
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            var localCollection = AsCollection(source);
            foreach (var item in items)
            {
                if (localCollection.Contains(item, comparer))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            comparer = comparer ?? EqualityComparer<T>.Default;
            var localCollection = AsCollection(source);
            foreach (var item in items)
            {
                if (localCollection.Contains(item, comparer))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var result = new List<TOutput>();
            foreach (var item in source)
            {
                result.Add(converter(item));
            }
            return result;
        }

        public static TList ConvertAll<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter)
            where TList : ICollection<TOutput>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var result = new TList();
            foreach (var item in source)
            {
                result.Add(converter(item));
            }
            return result;
        }

        public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var result = new List<TOutput>();
            foreach (var item in source)
            {
                if (filter(item))
                {
                    result.Add(converter(item));
                }
            }
            return result;
        }

        public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new List<TOutput>();
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    result.Add(converter(item));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertFiltered<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
            where TList : ICollection<TOutput>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var result = new TList();
            foreach (var item in source)
            {
                if (filter(item))
                {
                    result.Add(converter(item));
                }
            }
            return result;
        }

        public static TList ConvertFiltered<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
            where TList : ICollection<TOutput>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new TList();
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    result.Add(converter(item));
                }
                index++;
            }
            return result;
        }

        public static List<KeyValuePair<int, TOutput>> ConvertIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new List<KeyValuePair<int, TOutput>>();
            foreach (var item in source)
            {
                if (filter(item))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, converter(item)));
                }
                index++;
            }
            return result;
        }

        public static List<KeyValuePair<int, TOutput>> ConvertIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new List<KeyValuePair<int, TOutput>>();
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, converter(item)));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertIndexed<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
            where TList : ICollection<KeyValuePair<int, TOutput>>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new TList();
            foreach (var item in source)
            {
                if (filter(item))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, converter(item)));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertIndexed<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
            where TList : ICollection<KeyValuePair<int, TOutput>>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            var result = new TList();
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, converter(item)));
                }
                index++;
            }
            return result;
        }

        public static IEnumerable<TOutput> ConvertProgressive<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            foreach (var item in source)
            {
                yield return converter(item);
            }
        }

        public static IEnumerable<TOutput> ConvertProgressiveFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            foreach (var item in source)
            {
                if (filter(item))
                {
                    yield return converter(item);
                }
            }
        }

        public static IEnumerable<TOutput> ConvertProgressiveFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    yield return converter(item);
                }
                index++;
            }
        }

        public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            foreach (var item in source)
            {
                if (filter(item))
                {
                    yield return new KeyValuePair<int, TOutput>(index, converter(item));
                }
                index++;
            }
        }

        public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            var index = 0;
            foreach (var item in source)
            {
                if (filter(item, index))
                {
                    yield return new KeyValuePair<int, TOutput>(index, converter(item));
                }
                index++;
            }
        }

        public static T[] Copy<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            return copy;
        }

        public static void CopyTo<T>(this IEnumerable<T> source, T[] array)
        {
            CopyTo(source, array, 0);
        }

        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            try
            {
                var index = arrayIndex;
                foreach (var item in source)
                {
                    array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, "array");
            }
        }

        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex, int countLimit)
        {
            CopyTo(source.TakeItems(countLimit), array, arrayIndex);
        }

        public static int CountContiguousItems<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var result = 0;
            var equalityComparer = EqualityComparer<T>.Default;
            foreach (var value in source)
            {
                if (equalityComparer.Equals(value, item))
                {
                    result++;
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public static int CountContiguousItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var result = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result++;
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public static int CountItems<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var result = 0;
            foreach (var value in source)
            {
                result++;
                GC.KeepAlive(value);
            }
            return result;
        }

        public static int CountItems<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var result = 0;
            var equalityComparer = EqualityComparer<T>.Default;
            foreach (var value in source)
            {
                if (equalityComparer.Equals(value, item))
                {
                    result++;
                }
            }
            return result;
        }

        public static int CountItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var result = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result++;
                }
            }
            return result;
        }

        public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            var index = 0;
            foreach (var item in source)
            {
                array.SetValue(item, index++);
            }
        }

        public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            foreach (var item in source)
            {
                array.SetValue(item, index++);
            }
        }

        public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty);
        }

        public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
                onNotEmpty();
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty, onNotEmpty);
        }

        public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onUnknownSize, Action<int> onKnownSize)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
                onKnownSize(sourceCollection.Count);
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty, onUnknownSize);
        }

        public static int ExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            var count = 0;
            foreach (var item in other)
            {
                while (source.Remove(item))
                {
                    count++;
                }
            }
            return count;
        }

        public static IEnumerable<T> ExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            foreach (var item in other)
            {
                while (source.Remove(item))
                {
                    yield return item;
                }
            }
        }

        public static bool Exists<T>(this IEnumerable<T> source, T value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            foreach (var local in source)
            {
                if (comparer.Equals(local, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var local in source)
            {
                if (comparer.Equals(local, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
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

        public static T Find<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var limit = index + count;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                    currentIndex++;
                }
                return default(T);
            }
        }

        public static T Find<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                    currentIndex++;
                }
                return default(T);
            }
        }

        public static T Find<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                }
                return default(T);
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var limit = index + count;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static T FindLast<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var limit = index + count;
            var result = default(T);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static T FindLast<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var result = default(T);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static T FindLast<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var result = default(T);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                }
                return result;
            }
        }

        public static int FindLastIndex<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var limit = index + count;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int FindLastIndex<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int FindLastIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var currentIndex = 0;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static List<T> FindWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var result = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static TList FindWhere<T, TList>(this IEnumerable<T> source, Predicate<T> predicate)
            where TList : ICollection<T>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var result = new TList();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return FlattenExtracted(source);
        }

        public static void For<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var index = 0;
            foreach (var item in source)
            {
                action.Invoke(index, item);
                index++;
            }
        }

        public static void For<T>(this IEnumerable<T> source, Action<int, T> action, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var index = 0;
            foreach (var item in source)
            {
                if (predicate.Invoke(item))
                {
                    action.Invoke(index, item);
                }
                index++;
            }
        }

        public static void For<T>(this IEnumerable<T> source, Action<int, T> action, Func<T, int, bool> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var index = 0;
            foreach (var item in source)
            {
                if (filter.Invoke(item, index))
                {
                    action.Invoke(index, item);
                }
                index++;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (var item in source)
            {
                action.Invoke(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (var item in source)
            {
                if (predicate.Invoke(item))
                {
                    action.Invoke(item);
                }
            }
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            var newValue = TypeHelper.CreateOrDefault<TValue>();
            dictionary.Add(key, newValue);
            return newValue;
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            dictionary.Add(key, newValue);
            return newValue;
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> create)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            var newValue = create == null ? default(TValue) : create();
            dictionary.Add(key, newValue);
            return newValue;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            var comparer = EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, int count, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            comparer = comparer ?? EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var comparer = EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var comparer = EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static IEnumerable<T> InterleaveMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                var ok = true;
                while (ok)
                {
                    ok = false;
                    foreach (var enumerator in enumerators)
                    {
                        if (enumerator.MoveNext())
                        {
                            yield return enumerator.Current;
                            ok = true;
                        }
                    }
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    enumerator.Dispose();
                }
            }
        }

        public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            var otherAsCollection = AsCollection(other);
            return source.RemoveWhere(input => !otherAsCollection.Contains(input));
        }

        public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            comparer = comparer ?? EqualityComparer<T>.Default;
            var otherASCollection = AsCollection(other);
            return source.RemoveWhere(input => !otherASCollection.Contains(input, comparer));
        }

        public static IEnumerable<T> IntersectWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            var otherAsCollection = AsCollection(other);
            return source.RemoveWhereEnumerable(input => !otherAsCollection.Contains(input));
        }

        public static IEnumerable<T> IntersectWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            comparer = comparer ?? EqualityComparer<T>.Default;
            var otherAsCollection = AsCollection(other);
            return source.RemoveWhereEnumerable(input => !otherAsCollection.Contains(input, comparer));
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool IsProperSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return IsSubsetOf(source, other, true);
        }

        public static bool IsProperSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return IsSupersetOf(source, other, true);
        }

        public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return IsSubsetOf(source, other, false);
        }

        public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return IsSupersetOf(source, other, false);
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var comparer = EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var comparer = EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, int count, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            comparer = comparer ?? EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            var comparer = EqualityComparer<T>.Default;
            var result = -1;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static bool ListEquals<T>(this ICollection<T> first, ICollection<T> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            if (first.Count != second.Count)
            {
                return false;
            }
            var cmp = EqualityComparer<T>.Default;
            var f = first.GetEnumerator();
            var s = second.GetEnumerator();
            try
            {
                while (f.MoveNext())
                {
                    s.MoveNext();

                    if (!cmp.Equals(f.Current, s.Current))
                    {
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                f.Dispose();
                s.Dispose();
            }
        }

        // Name needs to be different so it doesn't conflict with Enumerable.Select
        public static TOutput[] Map<TInput, TOutput>(this ICollection<TInput> source, Func<TInput, TOutput> select)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var count = source.Count;
            var result = new TOutput[count];
            count = 0;
            foreach (var t in source)
            {
                result[count++] = select(t);
            }
            return result;
        }

        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex)
            {
                newIndex--;
            }
            list.Insert(newIndex, item);
        }

        public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty)
        {
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            if (source == null)
            {
                onEmpty();
                return ArrayReservoir<T>.EmptyArray;
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty);
        }

        public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
        {
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            if (source == null)
            {
                onEmpty();
                return ArrayReservoir<T>.EmptyArray;
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
                onNotEmpty();
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty, onNotEmpty);
        }

        public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onUnknownSize, Action<int> onKnownSize)
        {
            if (onEmpty == null)
            {
                throw new ArgumentException("onEmpty");
            }
            if (source == null)
            {
                onEmpty();
                return ArrayReservoir<T>.EmptyArray;
            }
            var sourceCollection = source as ICollection<T>;
            if (sourceCollection != null)
            {
                if (sourceCollection.Count == 0)
                {
                    onEmpty();
                    return ArrayReservoir<T>.EmptyArray;
                }
                onKnownSize(sourceCollection.Count);
            }
            return NullOrEmptyCheckedExtracted(source, onEmpty, onUnknownSize);
        }

        public static bool Overlaps<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            return ContainsAny(source, items);
        }

        public static IEnumerable<TPackage> Pack<T, TPackage>(this IEnumerable<T> source, int size)
            where TPackage : ICollection<T>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var count = 0;
            var currentPackage = new TPackage();
            foreach (var item in source)
            {
                currentPackage.Add(item);
                count++;
                if (count == size)
                {
                    yield return currentPackage;
                    currentPackage = new TPackage();
                    count = 0;
                }
            }
            if (count > 0)
            {
                yield return currentPackage;
            }
        }

        public static IEnumerable<T[]> Pack<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var index = 0;
            var currentPackage = new T[size];
            foreach (var item in source)
            {
                currentPackage[index] = item;
                index++;
                if (index == size)
                {
                    yield return currentPackage;
                    currentPackage = new T[size];
                    index = 0;
                }
            }
            if (index > 0)
            {
                Array.Resize(ref currentPackage, index);
                yield return currentPackage;
            }
        }

        public static bool Remove<T>(this ICollection<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            comparer = comparer ?? EqualityComparer<T>.Default;
            using (var enumerator = source.RemoveWhereEnumerable(input => comparer.Equals(input, item)).GetEnumerator())
            {
                return enumerator.MoveNext();
            }
        }

        public static T[] RemoveFirst<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var result = new T[array.Length - 1];
            Array.Copy(array, 1, result, 0, result.Length);
            return result;
        }

        public static T[] RemoveLast<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var result = new T[array.Length - 1];
            Array.Copy(array, 0, result, 0, result.Length);
            return result;
        }

        public static int RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return RemoveWhere(source, items => Where(items, predicate));
        }

        public static int RemoveWhere<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            return ExceptWith
                   (
                       source,
                       new List<T>(converter.Invoke(source))
                   );
        }

        public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Predicate<T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return RemoveWhereEnumerable(source, items => Where(items, predicate));
        }

        public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            return ExceptWithEnumerable
                   (
                       source,
                       new List<T>(converter.Invoke(source))
                   );
        }

        public static void Reverse<T>(this IList<T> list, int index, int count)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
            }
            var listCount = list.Count;
            if (count > listCount - index)
            {
                throw new ArgumentException("The list does not contain the number of elements.", "list");
            }
            var end = index + count;
            for (; index < end; index++, end++)
            {
                SwapExtracted(list, index, end);
            }
        }

        public static bool SetEquals<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            var thatAsCollection = AsCollection(other);
            foreach (var item in thatAsCollection.Where(input => !source.Contains(input)))
            {
                GC.KeepAlive(item);
                return false;
            }
            foreach (var item in source.Where(input => !thatAsCollection.Contains(input)))
            {
                GC.KeepAlive(item);
                return false;
            }
            return true;
        }

        public static void Sort<T>(this IList<T> list, int index, int count, IComparer<T> comparer)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            comparer = comparer ?? Comparer<T>.Default;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
            }
            var listCount = list.Count;
            if (count > listCount - index)
            {
                throw new ArgumentException("The list does not contain the number of elements.", "list");
            }
            SortExtracted(list, index, count + index, comparer);
        }

        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (indexA < 0)
            {
                throw new ArgumentOutOfRangeException("indexA", "Non-negative number is required.");
            }
            if (indexB < 0)
            {
                throw new ArgumentOutOfRangeException("indexB", "Non-negative number is required.");
            }
            var listCount = list.Count;
            if (indexA >= listCount || indexB >= listCount)
            {
                throw new ArgumentException("The list does not contain the number of elements.", "list");
            }
            if (indexA != indexB)
            {
                SwapExtracted(list, indexA, indexB);
            }
        }

        public static int SymmetricExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRange(Where(other.Distinct(), input => !source.Remove(input)));
        }

        public static IEnumerable<T> SymmetricExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRangeEnumerable(Where(other.Distinct(), input => !source.Remove(input)));
        }

        public static T[] ToArray<T>(this ICollection<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return (new List<T>(source)).ToArray();
        }

        public static ReadOnlyCollection<TSource> ToReadOnly<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return new ReadOnlyCollection<TSource>(ArrayReservoir<TSource>.EmptyArray);
            }
            var sourceAsReadOnlyCollection = source as ReadOnlyCollection<TSource>;
            if (sourceAsReadOnlyCollection != null)
            {
                return sourceAsReadOnlyCollection;
            }
            return new ReadOnlyCollection<TSource>(source.ToArray());
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
                return false;
            }
        }

        public static bool TryFind<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate, out T founT)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        founT = enumerator.Current;
                        return true;
                    }
                    currentIndex++;
                }
                founT = default(T);
                return false;
            }
        }

        public static bool TryFind<T>(this IEnumerable<T> source, int index, Predicate<T> predicate, out T founT)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        founT = enumerator.Current;
                        return true;
                    }
                    currentIndex++;
                }
                founT = default(T);
                return false;
            }
        }

        public static bool TryFind<T>(this IEnumerable<T> source, Predicate<T> predicate, out T founT)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        founT = enumerator.Current;
                        return true;
                    }
                }
                founT = default(T);
                return false;
            }
        }

        public static bool TryFindLast<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate, out T foundItem)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            var limit = index + count;
            foundItem = default(T);
            var found = false;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                    currentIndex++;
                }
                return found;
            }
        }

        public static bool TryFindLast<T>(this IEnumerable<T> source, int index, Predicate<T> predicate, out T foundItem)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var currentIndex = 0;
            foundItem = default(T);
            var found = false;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                    currentIndex++;
                }
                return found;
            }
        }

        public static bool TryFindLast<T>(this IEnumerable<T> source, Predicate<T> predicate, out T foundItem)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foundItem = default(T);
            var found = false;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                }
                return found;
            }
        }

        public static bool TryTake<T>(this Stack<T> stack, out T item)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }
            try
            {
                item = stack.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

        public static bool TryTake<T>(this Queue<T> queue, out T item)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            try
            {
                item = queue.Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

        public static int UnionWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRange(other.Where(input => !source.Contains(input)));
        }

        public static IEnumerable<T> UnionWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRangeEnumerable(other.Where(input => !source.Contains(input)));
        }

        public static IEnumerable<T> Where<T>(IEnumerable<T> source, Predicate<T> predicate)
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

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
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

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action whereNot)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (whereNot == null)
            {
                return WhereExtracted(source, predicate);
            }
            return WhereExtracted(source, predicate, whereNot);
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T> whereNot)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (whereNot == null)
            {
                return WhereExtracted(source, predicate);
            }
            return WhereExtracted(source, predicate, whereNot);
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, Action whereNot)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (whereNot == null)
            {
                return WhereExtracted(source, predicate);
            }
            return WhereExtracted(source, predicate, whereNot);
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, Action<T> whereNot)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (whereNot == null)
            {
                return WhereExtracted(source, predicate);
            }
            return WhereExtracted(source, predicate, whereNot);
        }

        public static IEnumerable<T> WhereType<T>(IEnumerable enumerable)
        {
            return new EnumerableFromDelegate<T>(enumerable.GetEnumerator);
        }

        public static IEnumerable<TResult> ZipMany<T, TResult>(this IEnumerable<IEnumerable<T>> source, Func<IEnumerable<T>, TResult> func)
        {
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(enumerator => enumerator.MoveNext()))
                {
                    yield return func(enumerators.Select(enumerator => enumerator.Current));
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    enumerator.Dispose();
                }
            }
        }

        private static IEnumerable<T> FlattenExtracted<T>(IEnumerable<IEnumerable<T>> source)
        {
            foreach (var key in source)
            {
                foreach (var item in key)
                {
                    yield return item;
                }
            }
        }

        private static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = AsDistinctCollection(source);
            var that = AsDistinctCollection(other);
            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (@this.Contains(item))
                {
                    matchCount++;
                }
            }
            if (proper)
            {
                return matchCount == @this.Count && elementCount > @this.Count;
            }
            return matchCount == @this.Count;
        }

        private static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = AsDistinctCollection(source);
            var that = AsDistinctCollection(other);
            var elementCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (!@this.Contains(item))
                {
                    return false;
                }
            }
            if (proper)
            {
                return elementCount < @this.Count;
            }
            return true;
        }

        private static IEnumerable<T> NullOrEmptyCheckedExtracted<T>(IEnumerable<T> source, Action onEmpty)
        {
            var enumerator = source.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                else
                {
                    onEmpty();
                    yield break;
                }
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> NullOrEmptyCheckedExtracted<T>(IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
        {
            var enumerator = source.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    onNotEmpty();
                    yield return enumerator.Current;
                }
                else
                {
                    onEmpty();
                    yield break;
                }
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static void SortExtracted<T>(IList<T> list, int indexStart, int indexEnd, IComparer<T> comparer)
        {
            var low = indexStart;
            var high = indexEnd;
            var pivot = list[low + ((high - low) / 2)];
            while (low <= high)
            {
                while (low < indexEnd && comparer.Compare(list[low], pivot) < 0)
                {
                    low++;
                }
                while (high > indexStart && comparer.Compare(pivot, list[high]) < 0)
                {
                    high--;
                }
                if (low == high)
                {
                    low++;
                    high--;
                }
                else if (low < high)
                {
                    SwapExtracted(list, low, high);
                    low++;
                    high--;
                }
            }
            if (indexStart < high)
            {
                SortExtracted(list, indexStart, high, comparer);
            }
            if (low < indexEnd)
            {
                SortExtracted(list, low, indexEnd, comparer);
            }
        }

        private static void SwapExtracted<T>(IList<T> list, int indexA, int indexB)
        {
            var itemA = list[indexA];
            var itemB = list[indexB];
            list[indexA] = itemB;
            list[indexB] = itemA;
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Predicate<T> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                index++;
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate, Action whereNot)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    whereNot();
                }
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate, Action whereNot)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                else
                {
                    whereNot();
                }
                index++;
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate, Action<T> whereNot)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    whereNot(item);
                }
            }
        }

        private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate, Action<T> whereNot)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                else
                {
                    whereNot(item);
                }
                index++;
            }
        }
    }

    public static partial class Extensions
    {
#if NET35

        public static bool Contains<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            var localComparer = EqualityComparer<T>.Default;
            var localCollection = AsCollection(source);
            foreach (var item in items)
            {
                if (!localCollection.Contains(item, localComparer))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Contains<T>(this IEnumerable<T> source, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            var localComparer = comparer ?? EqualityComparer<T>.Default;
            var localCollection = AsCollection(source);
            foreach (var item in items)
            {
                if (!localCollection.Contains(item, localComparer))
                {
                    return false;
                }
            }
            return true;
        }

#endif
    }
}