﻿// Needed for NET40

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Theraot.Core.System;

namespace Theraot.Core.Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class StringHelper
    {
        public static string Append(this string text, string value)
        {
            return string.Concat(text, value);
        }

        public static string Append(this string text, string value1, string value2)
        {
            return string.Concat(text, value1, value2);
        }

        public static string Append(this string text, string value1, string value2, string value3)
        {
            return string.Concat(text, value1, value2, value3);
        }

        public static string Append(this string text, params string[] values)
        {
            return string.Concat(text, values);
        }

        public static string Concat(params string[] value)
        {
            return string.Concat(value);
        }

        public static string Concat(string[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            return ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Concat(string[] array, int arrayIndex, int countLimit)
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
                throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            return ConcatExtracted(array, arrayIndex, countLimit);
        }

        public static string Concat(params object[] values)
        {
            return string.Concat(values);
        }

        public static string Concat(object[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            return ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Concat(object[] array, int arrayIndex, int countLimit)
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
                throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            return ConcatExtracted(array, arrayIndex, countLimit);
        }

        public static string Concat<T>(IEnumerable<T> values, Func<T, string> converter)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                var itemToString = converter.Invoke(item);
                stringList.Add(itemToString);
                length += itemToString.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
        }

        public static string End(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            var length = text.Length;
            if (length < characterCount)
            {
                return text;
            }
            return text.Substring(length - characterCount);
        }

        public static string EnsureEnd(this string text, string end, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.EndsWith(end, comparisonType))
            {
                return text.Append(end);
            }
            return text;
        }

        public static string EnsureStart(this string text, string start)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.StartsWith(start, StringComparison.CurrentCulture))
            {
                return start.Append(text);
            }
            return text;
        }

        public static string EnsureStart(this string text, string start, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.StartsWith(start, comparisonType))
            {
                return start.Append(text);
            }
            return text;
        }

        public static string ExceptEnd(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            var length = text.Length;
            if (length < characterCount)
            {
                return string.Empty;
            }
            return text.Substring(0, length - characterCount);
        }

        public static string ExceptStart(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            var length = text.Length;
            if (length < characterCount)
            {
                return string.Empty;
            }
            return text.Substring(characterCount);
        }

        public static string Implode(string separator, params object[] values)
        {
            if (separator == null)
            {
                return string.Concat(values);
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var array = new string[values.Length];
            var index = 0;
            foreach (var item in values)
            {
                array[index++] = item.ToString();
            }
            return ImplodeExtracted(separator, array, 0, array.Length);
        }

        public static string Implode(string separator, object[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }
            return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Implode(string separator, object[] array, int arrayIndex, int countLimit)
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
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }
            return ImplodeExtracted(separator, array, arrayIndex, countLimit);
        }

        public static string Implode(string separator, params string[] value)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }
            return ImplodeExtracted(separator, value, 0, value.Length);
        }

        public static string Implode(string separator, string[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }
            return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Implode(string separator, string[] array, int arrayIndex, int countLimit)
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
            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }
            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }
            return ImplodeExtracted(separator, array, arrayIndex, countLimit);
        }

        public static string Implode(string separator, IEnumerable<string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item);
            }
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode<T>(string separator, IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item.ToString());
            }
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values, converter);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item.ToString());
            }
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode(string separator, IEnumerable<string> values, string start, string end)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item);
            }
            if (stringList.Count > 0)
            {
                start = start ?? string.Empty;
                end = end ?? string.Empty;
                return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
            }
            return string.Empty;
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, string start, string end)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item.ToString());
            }
            if (stringList.Count > 0)
            {
                start = start ?? string.Empty;
                end = end ?? string.Empty;
                return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
            }
            return string.Empty;
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter, string start, string end)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                return Concat(values, converter);
            }
            var stringList = new List<string>();
            foreach (var item in values)
            {
                stringList.Add(item.ToString());
            }
            if (stringList.Count > 0)
            {
                start = start ?? string.Empty;
                end = end ?? string.Empty;
                return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
            }
            return string.Empty;
        }

        public static bool Like(this string text, Regex regex, int startAt)
        {
            return regex.IsMatch(text, startAt);
        }

        public static bool Like(this string text, Regex regex)
        {
            return text.Like(regex, 0);
        }

        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.IsMatch(text, startAt);
        }

        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Like(regexPattern, regexOptions, 0);
        }

        public static bool Like(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static bool Like(this string text, string regexPattern)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static bool Like(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        public static bool Like(this string text, string regexPattern, int startAt)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        public static Match Match(this string text, Regex regex, int startAt, int length)
        {
            return regex.Match(text, startAt, length);
        }

        public static Match Match(this string text, Regex regex, int startAt)
        {
            return regex.Match(text, startAt);
        }

        public static Match Match(this string text, Regex regex)
        {
            return text.Match(regex, 0);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt, int length)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Match(regexPattern, regexOptions, 0);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt, int length)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static Match Match(this string text, string regexPattern, int startAt, int length)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, int startAt)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        public static Match Match(this string text, string regexPattern)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static MatchCollection Matches(this string text, Regex regex, int startAt)
        {
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, Regex regex)
        {
            return text.Matches(regex, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Matches(regexPattern, regexOptions, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            var regex = new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Matches(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, int startAt)
        {
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern)
        {
            return text.Matches(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static string NeglectEnd(this string text, string end, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }
            if (text.EndsWith(end, comparisonType))
            {
                return text.ExceptEnd(end.Length);
            }
            return text;
        }

        public static string NeglectStart(this string text, string start)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (text.StartsWith(start, StringComparison.CurrentCulture))
            {
                return text.ExceptStart(start.Length);
            }
            return text;
        }

        public static string NeglectStart(this string text, string start, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (text.StartsWith(start, comparisonType))
            {
                return text.ExceptStart(start.Length);
            }
            return text;
        }

        public static string Safe(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return text;
        }

        public static string Start(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            var length = text.Length;
            if (length < characterCount)
            {
                return text;
            }
            return text.Substring(0, characterCount);
        }

        private static string ConcatExtracted(object[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            var newArray = new string[count];
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!ReferenceEquals(item, null))
                {
                    var itemToString = item.ToString();
                    newArray[index - startIndex] = itemToString;
                    length += itemToString.Length;
                }
            }
            return ConcatExtractedExtracted(newArray, 0, count, length);
        }

        private static string ConcatExtracted(string[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!ReferenceEquals(item, null))
                {
                    length += item.Length;
                }
            }
            return ConcatExtractedExtracted(array, startIndex, maxIndex, length);
        }

        private static string ConcatExtractedExtracted(string[] array, int startIndex, int maxIndex, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }
            var result = new StringBuilder(length);
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                result.Append(item);
            }
            return result.ToString();
        }

        private static string ImplodeExtracted(string separator, object[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            var newArray = new string[count];
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!ReferenceEquals(item, null))
                {
                    var itemToString = item.ToString();
                    newArray[index - startIndex] = itemToString;
                    length += itemToString.Length;
                }
            }
            length += separator.Length * (count - 1);
            return ImplodeExtractedExtracted(separator, newArray, 0, count, length);
        }

        private static string ImplodeExtracted(string separator, string[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!ReferenceEquals(item, null))
                {
                    length += item.Length;
                }
            }
            length += separator.Length * (count - 1);
            return ImplodeExtractedExtracted(separator, array, startIndex, maxIndex, length);
        }

        private static string ImplodeExtractedExtracted(string separator, string[] array, int startIndex, int maxIndex, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }
            var result = new StringBuilder(length);
            var first = true;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append(separator);
                }
                result.Append(item);
            }
            return result.ToString();
        }
    }

    public static partial class StringHelper
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
#if NET20 || NET30 || NET35
            //Added in .NET 4.0
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            foreach (var character in value)
            {
                if (!char.IsWhiteSpace(character))
                {
                    return false;
                }
            }
            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }

        public static string Concat(IEnumerable<string> values)
        {
#if NET20 || NET30 || NET35
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                stringList.Add(item);
                length += item.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Concat(values);
#endif
        }

        public static string Concat<T>(IEnumerable<T> values)
        {
#if NET20 || NET30 || NET35
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Concat(values);
#endif
        }
    }

    public static partial class StringHelper
    {
        public static string Join(string separator, IEnumerable<string> values)
        {
#if NET20 || NET30 || NET35
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator.Length;
            foreach (var item in values)
            {
                if (length != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }
                stringList.Add(item);
                length += item.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if NET20 || NET30 || NET35
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator.Length;
            foreach (var item in values)
            {
                if (length != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }
                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        public static string Join(string separator, params object[] values)
        {
#if NET20 || NET30 || NET35
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator.Length;
            foreach (var item in values)
            {
                if (length != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }
                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }
            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        public static string Join(string separator, params string[] values)
        {
            return string.Join(separator, values);
        }

        public static string Join(string separator, string[] values, int startIndex, int count)
        {
            return string.Join(separator, values, startIndex, count);
        }
    }

    public static partial class StringHelper
    {
#if !NETCOREAPP1_1

        public static string EnsureEnd(this string text, string end)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.EndsWith(end, false, CultureInfo.CurrentCulture))
            {
                return text.Append(end);
            }
            return text;
        }

        public static string EnsureEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.EndsWith(end, ignoreCase, culture))
            {
                return text.Append(end);
            }
            return text;
        }

        public static string EnsureStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!text.StartsWith(start, ignoreCase, culture))
            {
                return start.Append(text);
            }
            return text;
        }

        public static string NeglectEnd(this string text, string end)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }
            if (text.EndsWith(end, false, CultureInfo.CurrentCulture))
            {
                return text.ExceptEnd(end.Length);
            }
            return text;
        }

        public static string NeglectEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }
            if (text.EndsWith(end, ignoreCase, culture))
            {
                return text.ExceptEnd(end.Length);
            }
            return text;
        }

        public static string NeglectStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (text.StartsWith(start, ignoreCase, culture))
            {
                return text.ExceptStart(start.Length);
            }
            return text;
        }

#endif
    }
}