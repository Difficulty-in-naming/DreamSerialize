// Needed for Workaround

using System.Collections.Generic;
using Theraot.Core.System;
using Theraot.Core.Theraot.Collections.Specialized;

namespace Theraot.Core.Theraot.Core
{
    public static class EqualityComparerHelper
    {
        public static IEqualityComparer<T> ToComparer<T>(this Func<T, T, bool> equalityComparison, Func<T, int> getHashCode)
        {
            return new CustomEqualityComparer<T>(equalityComparison, getHashCode);
        }
    }
}

#if FAT

namespace Theraot.Core
{
    public static class EqualityComparerHelper<T>
    {
        private static readonly IEqualityComparer<T> _default;

        static EqualityComparerHelper()
        {
            var type = typeof(T);
            Type tmp;
            PropertyInfo property;
            if (type.IsImplementationOf(typeof(IEquatable<>).MakeGenericType(type)))
            {
                property = GetPropertyDelegated(type, typeof(EqualityComparer<>));
            }
            else if (type.IsAssignableTo(typeof(Delegate)))
            {
                property = typeof(DelegateEqualityComparer).GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            }
            else if (type.IsAssignableTo(typeof(Thread)))
            {
                property = typeof(ReferenceEqualityComparer).GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            }
            else if (type.IsGenericImplementationOf(out tmp, typeof(INeedle<>)))
            {
                var types = tmp.GetGenericArguments();
                var conversionType = typeof(NeedleConversionEqualityComparer<,>).MakeGenericType(tmp, types[0]);
                _default = (IEqualityComparer<T>)conversionType.Create
                           (
                               GetPropertyDelegated
                               (
                                   types[0],
                                   typeof(EqualityComparerHelper<>)
                               ).GetValue
                               (
                                   null,
                                   null
                               )
                           );
                return;
            }
            else if (type.IsGenericInstanceOf(typeof(KeyValuePair<,>)))
            {
                property = GetProperty(type, typeof(KeyValuePairEqualityComparer<,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,,,>));
            }
            else
            {
                property = GetPropertyDelegated(type, typeof(EqualityComparer<>));
            }
            _default = (IEqualityComparer<T>)property.GetValue(null, null);
        }

        public static IEqualityComparer<T> Default
        {
            get { return _default; }
        }

        private static PropertyInfo GetProperty(Type type, Type equalityComparerType)
        {
            var genericTypeArguments = type.GetGenericArguments();
            var generticType = equalityComparerType.MakeGenericType(genericTypeArguments);
            return generticType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }

        private static PropertyInfo GetPropertyDelegated(Type type, Type equalityComparerType)
        {
            var generticType = equalityComparerType.MakeGenericType(type);
            return generticType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }
    }
}

#endif