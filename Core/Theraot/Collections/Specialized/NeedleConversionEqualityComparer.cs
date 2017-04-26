// Needed for NET35 (ConditionalWeakTable)

using System.Collections.Generic;
using Theraot.Core.Theraot.Threading.Needles;

namespace Theraot.Core.Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class NeedleConversionEqualityComparer<TNeedle, T> : ConversionEqualityComparer<TNeedle, T>, IEqualityComparer<TNeedle>
        where TNeedle : INeedle<T>
    {
        public NeedleConversionEqualityComparer(IEqualityComparer<T> comparer)
            : base(comparer, Conversion)
        {
            // Empty
        }

        private static T Conversion(TNeedle needle)
        {
            if (ReferenceEquals(needle, null))
            {
                return default(T);
            }
            return needle.Value;
        }
    }
}