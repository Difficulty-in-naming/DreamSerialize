﻿// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Core.System;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Collections.Specialized
{
    public class EnumerableFromDelegate<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerator<T>> _getEnumerator;

        public EnumerableFromDelegate(Func<IEnumerator> getEnumerator)
        {
            // Specify the type arguments explicitly
            _getEnumerator = getEnumerator.ChainConversion<IEnumerator, IEnumerator<T>>(ConvertEnumerator);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }

        private static IEnumerator<T> ConvertEnumerator(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                return null;
            }
            var genericEnumerator = enumerator as IEnumerator<T>;
            if (genericEnumerator != null)
            {
                return genericEnumerator;
            }
            return ConvertEnumeratorExtracted(enumerator);
        }

        private static IEnumerator<T> ConvertEnumeratorExtracted(IEnumerator enumerator)
        {
            try
            {
                while (enumerator.MoveNext())
                {
                    var element = enumerator.Current;
                    if (element is T)
                    {
                        yield return (T)element;
                    }
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}