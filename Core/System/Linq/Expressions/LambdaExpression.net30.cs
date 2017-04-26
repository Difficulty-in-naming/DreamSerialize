#define FEATURE_CORECLR
#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Theraot.Core.System.Dynamic.Utils;
using Theraot.Core.Theraot.Collections;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.System.Linq.Expressions
{
    /// <summary>
    /// Creates a <see cref="LambdaExpression"/> node.
    /// This captures a block of code that is similar to a .NET method body.
    /// </summary>
    /// <remarks>
    /// Lambda expressions take input through parameters and are expected to be fully bound.
    /// </remarks>
    [DebuggerTypeProxy(typeof(LambdaExpressionProxy))]
    public abstract class LambdaExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _body;
        private readonly ReadOnlyCollection<ParameterExpression> _parameters;
        private readonly Type _delegateType;
        private readonly bool _tailCall;

        internal LambdaExpression(
            Type delegateType,
            string name,
            Expression body,
            bool tailCall,
            ReadOnlyCollection<ParameterExpression> parameters
        )
        {
            Debug.Assert(delegateType != null);

            _name = name;
            _body = body;
            _parameters = parameters;
            _delegateType = delegateType;
            _tailCall = tailCall;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type
        {
            get { return _delegateType; }
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.Lambda; }
        }

        /// <summary>
        /// Gets the parameters of the lambda expression.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Gets the name of the lambda expression.
        /// </summary>
        /// <remarks>Used for debugging purposes.</remarks>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the body of the lambda expression.
        /// </summary>
        public Expression Body
        {
            get { return _body; }
        }

        /// <summary>
        /// Gets the return type of the lambda expression.
        /// </summary>
        public Type ReturnType
        {
            get { return Type.GetMethod("Invoke").ReturnType; }
        }

        /// <summary>
        /// Gets the value that indicates if the lambda expression will be compiled with
        /// tail call optimization.
        /// </summary>
        public bool TailCall
        {
            get { return _tailCall; }
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public Delegate Compile()
        {
#if FEATURE_CORECLR
            return Compiler.LambdaCompiler.Compile(this);
#else
            return new System.Linq.Expressions.Interpreter.LightCompiler().CompileTop(this).CreateDelegate();
#endif
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLambda(this);
        }

        public virtual LambdaExpression Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == Body && parameters == Parameters)
            {
                return this;
            }
            return Lambda(Type, body, Name, TailCall, parameters);
        }

#if FEATURE_CORECLR

        internal abstract LambdaExpression Accept(Compiler.StackSpiller spiller);

#endif
    }

    /// <summary>
    /// Defines a <see cref="Expression{TDelegate}"/> node.
    /// This captures a block of code that is similar to a .NET method body.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <remarks>
    /// Lambda expressions take input through parameters and are expected to be fully bound.
    /// </remarks>
    public sealed class Expression<TDelegate> : LambdaExpression
    {
        public Expression(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
            : base(typeof(TDelegate), name, body, tailCall, parameters)
        {
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public new TDelegate Compile()
        {
#if FEATURE_CORECLR
            return (TDelegate)(object)Compiler.LambdaCompiler.Compile(this);
#else
            return (TDelegate)(object)new System.Linq.Expressions.Interpreter.LightCompiler().CompileTop(this).CreateDelegate();
#endif
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="body">The <see cref="LambdaExpression.Body">Body</see> property of the result.</param>
        /// <param name="parameters">The <see cref="LambdaExpression.Parameters">Parameters</see> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public override LambdaExpression Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == Body && parameters == Parameters)
            {
                return this;
            }
            return Lambda<TDelegate>(body, Name, TailCall, parameters);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLambda(this);
        }

#if FEATURE_CORECLR

        internal override LambdaExpression Accept(Compiler.StackSpiller spiller)
        {
            return spiller.Rewrite(this);
        }

        internal static LambdaExpression Create(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return new Expression<TDelegate>(body, name, tailCall, parameters);
        }

#endif
    }

#if !FEATURE_CORECLR
// Seperate expression creation class to hide the CreateExpressionFunc function from users reflecting on Expression<T>
public class ExpressionCreator<TDelegate>
    {
        public static LambdaExpression CreateExpressionFunc(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return new Expression<TDelegate>(body, name, tailCall, parameters);
        }
    }
#endif

    public partial class Expression
    {
        internal static LambdaExpression CreateLambda(Type delegateType, Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            // Get or create a delegate to the public Expression.Lambda<T>
            // method and call that will be used for creating instances of this
            // delegate type
            Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression> fastPath;
            var factories = _lambdaFactories;
            if (factories == null)
            {
                _lambdaFactories = factories = new CacheDict<Type, Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>>(50);
            }

            MethodInfo create = null;
            if (!factories.TryGetValue(delegateType, out fastPath))
            {
#if FEATURE_CORECLR
                create = typeof(Expression<>).MakeGenericType(delegateType).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
#else
                create = typeof(ExpressionCreator<>).MakeGenericType(delegateType).GetMethod("CreateExpressionFunc", BindingFlags.Static | BindingFlags.Public);
#endif
                if (delegateType.CanCache())
                {
                    factories[delegateType] = fastPath = (Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>)create.CreateDelegate(typeof(Func<Expression, string, bool, ReadOnlyCollection<ParameterExpression>, LambdaExpression>));
                }
            }

            if (fastPath != null)
            {
                return fastPath(body, name, tailCall, parameters);
            }

            Debug.Assert(create != null);
            return (LambdaExpression)create.Invoke(null, new object[] { body, name, tailCall, parameters });
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda<TDelegate>(body, false, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda<TDelegate>(body, tailCall, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, null, false, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="name">The name of the lambda. Used for generating debugging info.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, name, false, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type. </typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="name">The name of the lambda. Used for generating debugging info.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="P:NodeType"/> property equal to <see cref="P:Lambda"/> and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = parameters.ToReadOnly();
            ValidateLambdaArgs(typeof(TDelegate), ref body, parameterList);
            return new Expression<TDelegate>(body, name, tailCall, parameterList);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(body, false, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda(body, tailCall, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, null, false, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(delegateType, body, null, false, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda(delegateType, body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(delegateType, body, null, false, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(delegateType, body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, name, false, parameters);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(body, "body");

            var parameterList = parameters.ToReadOnly();

            var paramCount = parameterList.Count;
            var typeArgs = new Type[paramCount + 1];
            if (paramCount > 0)
            {
                var set = new Set<ParameterExpression>(parameterList.Count);
                for (var i = 0; i < paramCount; i++)
                {
                    var param = parameterList[i];
                    ContractUtils.RequiresNotNull(param, "parameter");
                    typeArgs[i] = param.IsByRef ? param.Type.MakeByRefType() : param.Type;
                    if (set.Contains(param))
                    {
                        throw Error.DuplicateVariable(param);
                    }
                    set.Add(param);
                }
            }
            typeArgs[paramCount] = body.Type;

            var delegateType = Compiler.DelegateHelpers.MakeDelegateType(typeArgs);

            return CreateLambda(delegateType, body, name, tailCall, parameterList);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            var paramList = parameters.ToReadOnly();
            ValidateLambdaArgs(delegateType, ref body, paramList);

            return CreateLambda(delegateType, body, name, false, paramList);
        }

        /// <summary>
        /// Creates a LambdaExpression by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="P:Body"/> property equal to. </param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression. </param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="P:Parameters"/> collection. </param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="P:NodeType"/> property equal to Lambda and the <see cref="P:Body"/> and <see cref="P:Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            var paramList = parameters.ToReadOnly();
            ValidateLambdaArgs(delegateType, ref body, paramList);

            return CreateLambda(delegateType, body, name, tailCall, paramList);
        }

        private static void ValidateLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(delegateType, "delegateType");
            RequiresCanRead(body, "body");

            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
            {
                throw Error.LambdaTypeMustBeDerivedFromSystemDelegate();
            }

            MethodInfo mi;
            var ldc = _lambdaDelegateCache;
            if (!ldc.TryGetValue(delegateType, out mi))
            {
                mi = delegateType.GetMethod("Invoke");
                if (delegateType.CanCache())
                {
                    ldc[delegateType] = mi;
                }
            }

            var pis = mi.GetParameters();

            if (pis.Length > 0)
            {
                if (pis.Length != parameters.Count)
                {
                    throw Error.IncorrectNumberOfLambdaDeclarationParameters();
                }
                var set = new Set<ParameterExpression>(pis.Length);
                var n = pis.Length;
                for (int i = 0; i < n; i++)
                {
                    var pex = parameters[i];
                    var pi = pis[i];
                    RequiresCanRead(pex, "parameters");
                    var pType = pi.ParameterType;
                    if (pex.IsByRef)
                    {
                        if (!pType.IsByRef)
                        {
                            //We cannot pass a parameter of T& to a delegate that takes T or any non-ByRef type.
                            throw Error.ParameterExpressionNotValidAsDelegate(pex.Type.MakeByRefType(), pType);
                        }
                        pType = pType.GetElementType();
                    }
                    if (!TypeHelper.AreReferenceAssignable(pex.Type, pType))
                    {
                        throw Error.ParameterExpressionNotValidAsDelegate(pex.Type, pType);
                    }
                    if (set.Contains(pex))
                    {
                        throw Error.DuplicateVariable(pex);
                    }
                    set.Add(pex);
                }
            }
            else if (parameters.Count > 0)
            {
                throw Error.IncorrectNumberOfLambdaDeclarationParameters();
            }
            if (mi.ReturnType != typeof(void) && !TypeHelper.AreReferenceAssignable(mi.ReturnType, body.Type))
            {
                if (!TryQuote(mi.ReturnType, ref body))
                {
                    throw Error.ExpressionTypeDoesNotMatchReturn(body.Type, mi.ReturnType);
                }
            }
        }

        private static bool ValidateTryGetFuncActionArgs(Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                throw new ArgumentNullException("typeArgs");
            }
            var n = typeArgs.Length;
            for (int i = 0; i < n; i++)
            {
                var a = typeArgs[i];
                if (a == null)
                {
                    throw new ArgumentNullException("typeArgs");
                }
                if (a.IsByRef)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a <see cref="Type"/> object that represents a generic System.Func delegate type that has specific type arguments.
        /// The last type argument specifies the return type of the created delegate.
        /// </summary>
        /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Func delegate type.</param>
        /// <returns>The type of a System.Func delegate that has the specified type arguments.</returns>
        public static Type GetFuncType(params Type[] typeArgs)
        {
            if (!ValidateTryGetFuncActionArgs(typeArgs))
            {
                throw Error.TypeMustNotBeByRef();
            }

            var result = Compiler.DelegateHelpers.GetFuncType(typeArgs);
            if (result == null)
            {
                throw Error.IncorrectNumberOfTypeArgsForFunc();
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="Type"/> object that represents a generic System.Func delegate type that has specific type arguments.
        /// The last type argument specifies the return type of the created delegate.
        /// </summary>
        /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Func delegate type.</param>
        /// <param name="funcType">When this method returns, contains the generic System.Func delegate type that has specific type arguments. Contains null if there is no generic System.Func delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
        /// <returns>true if generic System.Func delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.</returns>
        public static bool TryGetFuncType(Type[] typeArgs, out Type funcType)
        {
            if (ValidateTryGetFuncActionArgs(typeArgs))
            {
                return (funcType = Compiler.DelegateHelpers.GetFuncType(typeArgs)) != null;
            }
            funcType = null;
            return false;
        }

        /// <summary>
        /// Creates a <see cref="Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
        /// </summary>
        /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Action delegate type.</param>
        /// <returns>The type of a System.Action delegate that has the specified type arguments.</returns>
        public static Type GetActionType(params Type[] typeArgs)
        {
            if (!ValidateTryGetFuncActionArgs(typeArgs))
            {
                throw Error.TypeMustNotBeByRef();
            }

            var result = Compiler.DelegateHelpers.GetActionType(typeArgs);
            if (result == null)
            {
                throw Error.IncorrectNumberOfTypeArgsForAction();
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
        /// </summary>
        /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Action delegate type.</param>
        /// <param name="actionType">When this method returns, contains the generic System.Action delegate type that has specific type arguments. Contains null if there is no generic System.Action delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
        /// <returns>true if generic System.Action delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.</returns>
        public static bool TryGetActionType(Type[] typeArgs, out Type actionType)
        {
            if (ValidateTryGetFuncActionArgs(typeArgs))
            {
                return (actionType = Compiler.DelegateHelpers.GetActionType(typeArgs)) != null;
            }
            actionType = null;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="Type"/> object that represents a generic System.Func or System.Action delegate type that has specific type arguments.
        /// The last type argument determines the return type of the delegate. If no Func or Action is large enough, it will generate a custom
        /// delegate type.
        /// </summary>
        /// <param name="typeArgs">The type arguments of the delegate.</param>
        /// <returns>The delegate type.</returns>
        /// <remarks>
        /// As with Func, the last argument is the return type. It can be set
        /// to System.Void to produce an Action.</remarks>
        public static Type GetDelegateType(params Type[] typeArgs)
        {
            ContractUtils.RequiresNotEmpty(typeArgs, "typeArgs");
            ContractUtils.RequiresNotNullItems(typeArgs, "typeArgs");
            return Compiler.DelegateHelpers.MakeDelegateType(typeArgs);
        }
    }
}

#endif