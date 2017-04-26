#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Theraot.Core.System.Dynamic.Utils;
using Theraot.Core.Theraot.Collections;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.System.Linq.Expressions
{
    /// <summary>
    /// Represents creating a new array and possibly initializing the elements of the new array.
    /// </summary>
    [DebuggerTypeProxy(typeof(NewArrayExpressionProxy))]
    public class NewArrayExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _expressions;
        private readonly Type _type;

        internal NewArrayExpression(Type type, ReadOnlyCollection<Expression> expressions)
        {
            _expressions = expressions;
            _type = type;
        }

        internal static NewArrayExpression Make(ExpressionType nodeType, Type type, ReadOnlyCollection<Expression> expressions)
        {
            if (nodeType == ExpressionType.NewArrayInit)
            {
                return new NewArrayInitExpression(type, expressions);
            }
            else
            {
                return new NewArrayBoundsExpression(type, expressions);
            }
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the bounds of the array if the value of the <see cref="P:NodeType"/> property is NewArrayBounds, or the values to initialize the elements of the new array if the value of the <see cref="P:NodeType"/> property is NewArrayInit.
        /// </summary>
        public ReadOnlyCollection<Expression> Expressions
        {
            get { return _expressions; }
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitNewArray(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expressions">The <see cref="Expressions" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public NewArrayExpression Update(IEnumerable<Expression> expressions)
        {
            if (expressions == Expressions)
            {
                return this;
            }
            if (NodeType == ExpressionType.NewArrayInit)
            {
                return NewArrayInit(Type.GetElementType(), expressions);
            }
            return NewArrayBounds(Type.GetElementType(), expressions);
        }
    }

    internal sealed class NewArrayInitExpression : NewArrayExpression
    {
        internal NewArrayInitExpression(Type type, ReadOnlyCollection<Expression> expressions)
            : base(type, expressions)
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public override ExpressionType NodeType
        {
            get { return ExpressionType.NewArrayInit; }
        }
    }

    internal sealed class NewArrayBoundsExpression : NewArrayExpression
    {
        internal NewArrayBoundsExpression(Type type, ReadOnlyCollection<Expression> expressions)
            : base(type, expressions)
        {
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public override ExpressionType NodeType
        {
            get { return ExpressionType.NewArrayBounds; }
        }
    }

    public partial class Expression
    {
        #region NewArrayInit

        /// <summary>
        /// Creates a new array expression of the specified type from the provided initializers.
        /// </summary>
        /// <param name="type">A Type that represents the element type of the array.</param>
        /// <param name="initializers">The expressions used to create the array elements.</param>
        /// <returns>An instance of the <see cref="NewArrayExpression"/>.</returns>
        public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
        {
            return NewArrayInit(type, (IEnumerable<Expression>)initializers);
        }

        /// <summary>
        /// Creates a new array expression of the specified type from the provided initializers.
        /// </summary>
        /// <param name="type">A Type that represents the element type of the array.</param>
        /// <param name="initializers">The expressions used to create the array elements.</param>
        /// <returns>An instance of the <see cref="NewArrayExpression"/>.</returns>
        public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
        {
            ContractUtils.RequiresNotNull(type, "type");
            ContractUtils.RequiresNotNull(initializers, "initializers");
            if (type == typeof(void))
            {
                throw Error.ArgumentCannotBeOfTypeVoid();
            }

            var initializerList = initializers.ToReadOnly();

            Expression[] newList = null;
            var n = initializerList.Count;
            for (var i = 0; i < n; i++)
            {
                var expr = initializerList[i];
                RequiresCanRead(expr, "initializers");

                if (!TypeHelper.AreReferenceAssignable(type, expr.Type))
                {
                    if (!TryQuote(type, ref expr))
                    {
                        throw Error.ExpressionTypeCannotInitializeArrayType(expr.Type, type);
                    }
                    if (newList == null)
                    {
                        newList = new Expression[n];
                        for (var j = 0; j < i; j++)
                        {
                            newList[j] = initializerList[j];
                        }
                    }
                }
                if (newList != null)
                {
                    newList[i] = expr;
                }
            }
            if (newList != null)
            {
                initializerList = new TrueReadOnlyCollection<Expression>(newList);
            }

            return NewArrayExpression.Make(ExpressionType.NewArrayInit, type.MakeArrayType(), initializerList);
        }

        #endregion NewArrayInit

        #region NewArrayBounds

        /// <summary>
        /// Creates a <see cref="NewArrayExpression"/> that represents creating an array that has a specified rank.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the element type of the array.</param>
        /// <param name="bounds">An array that contains Expression objects to use to populate the Expressions collection.</param>
        /// <returns>A <see cref="NewArrayExpression"/> that has the <see cref="P:NodeType"/> property equal to type and the <see cref="P:Expressions"/> property set to the specified value.</returns>
        public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
        {
            return NewArrayBounds(type, (IEnumerable<Expression>)bounds);
        }

        /// <summary>
        /// Creates a <see cref="NewArrayExpression"/> that represents creating an array that has a specified rank.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the element type of the array.</param>
        /// <param name="bounds">An IEnumerable{T} that contains Expression objects to use to populate the Expressions collection.</param>
        /// <returns>A <see cref="NewArrayExpression"/> that has the <see cref="P:NodeType"/> property equal to type and the <see cref="P:Expressions"/> property set to the specified value.</returns>
        public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
        {
            ContractUtils.RequiresNotNull(type, "type");
            ContractUtils.RequiresNotNull(bounds, "bounds");

            if (type == typeof(void))
            {
                throw Error.ArgumentCannotBeOfTypeVoid();
            }

            var boundsList = bounds.ToReadOnly();

            var dimensions = boundsList.Count;
            if (dimensions <= 0)
            {
                throw Error.BoundsCannotBeLessThanOne();
            }

            for (var i = 0; i < dimensions; i++)
            {
                var expr = boundsList[i];
                RequiresCanRead(expr, "bounds");
                if (!expr.Type.IsInteger())
                {
                    throw Error.ArgumentMustBeInteger();
                }
            }

            Type arrayType;
            if (dimensions == 1)
            {
                //To get a vector, need call Type.MakeArrayType().
                //Type.MakeArrayType(1) gives a non-vector array, which will cause type check error.
                arrayType = type.MakeArrayType();
            }
            else
            {
                arrayType = type.MakeArrayType(dimensions);
            }

            return NewArrayExpression.Make(ExpressionType.NewArrayBounds, arrayType, bounds.ToReadOnly());
        }

        #endregion NewArrayBounds
    }
}

#endif