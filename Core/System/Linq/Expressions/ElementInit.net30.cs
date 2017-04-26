#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Theraot.Core.System.Dynamic.Utils;
using Theraot.Core.Theraot.Collections;

namespace Theraot.Core.System.Linq.Expressions
{
    /// <summary>
    /// Represents the initialization of a list.
    /// </summary>
    public sealed class ElementInit : IArgumentProvider
    {
        private readonly MethodInfo _addMethod;
        private readonly ReadOnlyCollection<Expression> _arguments;

        internal ElementInit(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            _addMethod = addMethod;
            _arguments = arguments;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> used to add elements to the object.
        /// </summary>
        public MethodInfo AddMethod
        {
            get { return _addMethod; }
        }

        /// <summary>
        /// Gets the list of elements to be added to the object.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments
        {
            get { return _arguments; }
        }

        public Expression GetArgument(int index)
        {
            return _arguments[index];
        }

        public int ArgumentCount
        {
            get { return _arguments.Count; }
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of the node.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the node.</returns>
        public override string ToString()
        {
            return ExpressionStringBuilder.ElementInitBindingToString(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ElementInit Update(IEnumerable<Expression> arguments)
        {
            if (arguments == Arguments)
            {
                return this;
            }
            return Expression.ElementInit(AddMethod, arguments);
        }
    }

    public partial class Expression
    {
        /// <summary>
        /// Creates an <see cref="T:ElementInit">ElementInit</see> expression that represents the initialization of a list.
        /// </summary>
        /// <param name="addMethod">The <see cref="MethodInfo"/> for the list's Add method.</param>
        /// <param name="arguments">An array containing the Expressions to be used to initialize the list.</param>
        /// <returns>The created <see cref="T:ElementInit">ElementInit</see> expression.</returns>
        public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
        {
            return ElementInit(addMethod, arguments as IEnumerable<Expression>);
        }

        /// <summary>
        /// Creates an <see cref="T:ElementInit">ElementInit</see> expression that represents the initialization of a list.
        /// </summary>
        /// <param name="addMethod">The <see cref="MethodInfo"/> for the list's Add method.</param>
        /// <param name="arguments">An <see cref="IEnumerable{T}"/> containing <see cref="Expression"/> elements to initialize the list.</param>
        /// <returns>The created <see cref="T:ElementInit">ElementInit</see> expression.</returns>
        public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            ContractUtils.RequiresNotNull(addMethod, "addMethod");
            ContractUtils.RequiresNotNull(arguments, "arguments");

            var argumentsReadOnly = arguments.ToReadOnly();

            RequiresCanRead(argumentsReadOnly, "arguments");
            ValidateElementInitAddMethodInfo(addMethod);
            ValidateArgumentTypes(addMethod, ExpressionType.Call, ref argumentsReadOnly);
            return new ElementInit(addMethod, argumentsReadOnly);
        }

        private static void ValidateElementInitAddMethodInfo(MethodInfo addMethod)
        {
            ValidateMethodInfo(addMethod);
            var pis = addMethod.GetParameters();
            if (pis.Length == 0)
            {
                throw Error.ElementInitializerMethodWithZeroArgs();
            }
            if (!addMethod.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                throw Error.ElementInitializerMethodNotAdd();
            }
            if (addMethod.IsStatic)
            {
                throw Error.ElementInitializerMethodStatic();
            }
            foreach (var pi in pis)
            {
                if (pi.ParameterType.IsByRef)
                {
                    throw Error.ElementInitializerMethodNoRefOutParam(pi.Name, addMethod.Name);
                }
            }
        }
    }
}

#endif