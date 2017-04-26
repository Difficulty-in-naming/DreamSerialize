// Needed for NET35 (ConditionalWeakTable)

using System;
using System.Collections.Generic;
using Theraot.Core.System;

namespace Theraot.Core.Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ConversionEqualityComparer<TInput, TOutput> : IEqualityComparer<TInput>
    {
        private readonly IEqualityComparer<TOutput> _comparer;
        private readonly Func<TInput, TOutput> _converter;

        public ConversionEqualityComparer(IEqualityComparer<TOutput> comparer, Func<TInput, TOutput> converter)
        {
            _comparer = comparer ?? EqualityComparer<TOutput>.Default;
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            _converter = converter;
        }

        public bool Equals(TInput x, TInput y)
        {
            return _comparer.Equals(_converter.Invoke(x), _converter.Invoke(y));
        }

        public int GetHashCode(TInput obj)
        {
            return _comparer.GetHashCode(_converter.Invoke(obj));
        }
    }
}