// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Theraot.Core.System.Dynamic.Utils;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.System.Linq.Expressions
{
#if NET20 || NET30 || NET35

    /// <summary>
    /// Used to denote the target of a <see cref="GotoExpression"/>.
    /// </summary>
    public sealed class LabelTarget
    {
        private readonly Type _type;
        private readonly string _name;

        internal LabelTarget(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        /// <summary>
        /// Gets the name of the label.
        /// </summary>
        /// <remarks>The label's name is provided for information purposes only.</remarks>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The type of value that is passed when jumping to the label
        /// (or System.Void if no value should be passed).
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="object"/>. </returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? "UnamedLabel" : Name;
        }
    }

#endif
#if NET20 || NET30

    public partial class Expression
    {
        /// <summary>
        /// Creates a <see cref="LabelTarget"/> representing a label with void type and no name.
        /// </summary>
        /// <returns>The new <see cref="LabelTarget"/>.</returns>
        public static LabelTarget Label()
        {
            return Label(typeof(void), null);
        }

        /// <summary>
        /// Creates a <see cref="LabelTarget"/> representing a label with void type and the given name.
        /// </summary>
        /// <param name="name">The name of the label.</param>
        /// <returns>The new <see cref="LabelTarget"/>.</returns>
        public static LabelTarget Label(string name)
        {
            return Label(typeof(void), name);
        }

        /// <summary>
        /// Creates a <see cref="LabelTarget"/> representing a label with the given type.
        /// </summary>
        /// <param name="type">The type of value that is passed when jumping to the label.</param>
        /// <returns>The new <see cref="LabelTarget"/>.</returns>
        public static LabelTarget Label(Type type)
        {
            return Label(type, null);
        }

        /// <summary>
        /// Creates a <see cref="LabelTarget"/> representing a label with the given type and name.
        /// </summary>
        /// <param name="type">The type of value that is passed when jumping to the label.</param>
        /// <param name="name">The name of the label.</param>
        /// <returns>The new <see cref="LabelTarget"/>.</returns>
        public static LabelTarget Label(Type type, string name)
        {
            ContractUtils.RequiresNotNull(type, "type");
            TypeHelper.ValidateType(type);
            return new LabelTarget(type, name);
        }
    }

#endif
}