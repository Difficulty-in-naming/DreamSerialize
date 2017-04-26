#if NET20 || NET30 || NET35

using System;
using System.Collections.Generic;

namespace Theraot.Core.System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class DynamicAttribute : Attribute
    {
        private static readonly IList<bool> _empty = Array.AsReadOnly(new[] { true });

        private readonly IList<bool> _transformFlags;

        public DynamicAttribute()
        {
            _transformFlags = _empty;
        }

        public DynamicAttribute(bool[] transformFlags)
        {
            if (transformFlags == null)
            {
                throw new ArgumentNullException("transformFlags");
            }
            _transformFlags = transformFlags;
        }

        public IList<bool> TransformFlags
        {
            get { return _transformFlags; }
        }
    }
}

#endif