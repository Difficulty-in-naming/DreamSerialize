// Needed for NET30

using System;
using System.Collections.Generic;
using Theraot.Core.System;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class ConditionalExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>
    {
        private readonly Func<bool> _enumerateAppend;
        private readonly Func<bool> _enumerateTarget;

        public ConditionalExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> append, Func<bool> enumerateTarget, Func<bool> enumerateAppend)
            : base(target, append)
        {
            if (enumerateTarget == null)
            {
                throw new ArgumentNullException("enumerateTarget");
            }
            _enumerateTarget = enumerateTarget;
            _enumerateAppend = enumerateAppend ?? (null == append ? FuncHelper.GetFallacyFunc() : FuncHelper.GetTautologyFunc());
        }

        public override IEnumerator<T> GetEnumerator()
        {
            if (_enumerateTarget.Invoke())
            {
                foreach (var item in Target)
                {
                    yield return item;
                }
            }
            if (_enumerateAppend.Invoke())
            {
                foreach (var item in Append)
                {
                    yield return item;
                }
            }
        }
    }
}