#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Theraot.Core.System.Linq.Expressions
{
    /// <summary>
    /// Describes the binding types that are used in MemberInitExpression objects.
    /// </summary>
    public enum MemberBindingType
    {
        /// <summary>
        /// A binding that represents initializing a member with the value of an expression.
        /// </summary>
        Assignment,

        /// <summary>
        /// A binding that represents recursively initializing members of a member.
        /// </summary>
        MemberBinding,

        /// <summary>
        /// A binding that represents initializing a member of type <see cref="IList"/> or <see cref="ICollection{T}"/> from a list of elements.
        /// </summary>
        ListBinding
    }

    /// <summary>
    /// Provides the base class from which the classes that represent bindings that are used to initialize members of a newly created object derive.
    /// </summary>
    public abstract class MemberBinding
    {
        private readonly MemberBindingType _type;
        private readonly MemberInfo _member;

        /// <summary>
        /// Initializes an instance of <see cref="MemberBinding"/> class.
        /// </summary>
        /// <param name="type">The type of member binding.</param>
        /// <param name="member">The field or property to be initialized.</param>
        [Obsolete("Do not use this constructor. It will be removed in future releases.")]
        protected MemberBinding(MemberBindingType type, MemberInfo member)
        {
            _type = type;
            _member = member;
        }

        /// <summary>
        /// Gets the type of binding that is represented.
        /// </summary>
        public MemberBindingType BindingType
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the field or property to be initialized.
        /// </summary>
        public MemberInfo Member
        {
            get { return _member; }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="object"/>. </returns>
        public override string ToString()
        {
            return ExpressionStringBuilder.MemberBindingToString(this);
        }
    }
}

#endif