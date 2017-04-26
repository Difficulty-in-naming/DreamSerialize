// Needed for Workaround

using System;
using System.Reflection;
using Theraot.Core.System.Reflection;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class WeakDelegateNeedle : WeakNeedle<Delegate>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
    {
        public WeakDelegateNeedle(Delegate handler)
            : base((Delegate) Check.NotNullArgument(handler, "handler"))
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var temp = handler;
        }

        public WeakDelegateNeedle(MethodInfo methodInfo, object target)
            : base(BuildDelegate(methodInfo, target))
        {
            // Empty
        }

        public MethodInfo Method
        {
            get
            {
                var value = Value;
                if (IsAlive)
                {
                    return value.GetMethodInfo();
                }
                return null;
            }
        }

        public bool Equals(Delegate other)
        {
            var method = other.GetMethodInfo();
            return !ReferenceEquals(null, other) && Equals(method, other.Target);
        }

        public bool Equals(MethodInfo method, object target)
        {
            var value = Value;
            if (IsAlive)
            {
                return value.GetMethodInfo().Equals(method) && ReferenceEquals(value.Target, target);
            }
            return false;
        }

        public bool Equals(WeakDelegateNeedle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            var value = Value;
            if (IsAlive)
            {
                var otherValue = other.Value;
                if (other.IsAlive)
                {
                    var method = otherValue.GetMethodInfo();
                    return value.GetMethodInfo().Equals(method) && ReferenceEquals(value.Target, otherValue.Target);
                }
                return false;
            }
            return !other.IsAlive;
        }

        public void Invoke(object[] args)
        {
            TryInvoke(args);
        }

        public bool TryInvoke(object[] args)
        {
            var value = Value;
            if (IsAlive)
            {
                value.DynamicInvoke(args); // Throws TargetInvocationException
                return true;
            }
            return false;
        }

        public bool TryInvoke(object[] args, out object result)
        {
            var value = Value;
            if (IsAlive)
            {
                result = value.DynamicInvoke(args);
                return true;
            }
            result = null;
            return false;
        }

        public bool TryInvoke<TResult>(object[] args, out TResult result)
        {
            var value = Value;
            if (IsAlive)
            {
                result = (TResult)value.DynamicInvoke(args);
                return true;
            }
            result = default(TResult);
            return false;
        }

        private static Delegate BuildDelegate(MethodInfo methodInfo, object target)
        {
            if (ReferenceEquals(methodInfo, null))
            {
                throw new ArgumentNullException("methodInfo");
            }
            if (methodInfo.IsStatic != ReferenceEquals(null, target))
            {
                if (ReferenceEquals(target, null))
                {
                    throw new ArgumentNullException("target", "target is null and the method is not static.");
                }
                throw new ArgumentException("target is not null and the method is static", "target");
            }
            var type = methodInfo.DeclaringType;
            if (ReferenceEquals(type, null))
            {
                throw new ArgumentException("methodInfo.DeclaringType is null", "methodInfo");
            }
            return methodInfo.CreateDelegate(type, target);
        }
    }
}