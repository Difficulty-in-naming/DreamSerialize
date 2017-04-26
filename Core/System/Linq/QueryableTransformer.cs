// QueryableTransformer.cs
//
// Authors:
//    Roei Erez (roeie@mainsoft.com)
//    Jb Evain (jbevain@novell.com)
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Needed for NET35 (LINQ)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Core.System.Linq.Expressions;
using Theraot.Core.System.Runtime.CompilerServices;
using Theraot.Core.Theraot.Core;

namespace Theraot.Core.System.Linq
{
    internal class QueryableTransformer : ExpressionTransformer
    {
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            var qe = constant.Value as IQueryableEnumerable;
            if (qe == null)
            {
                return constant;
            }
            var enumerable = qe.GetEnumerable();
            if (enumerable != null)
            {
                return Expression.Constant(enumerable);
            }
            return Visit(qe.Expression);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            if (IsQueryableExtension(methodCall.Method))
            {
                return ReplaceQueryableMethod(methodCall);
            }
            return base.VisitMethodCall(methodCall);
        }

        private static Type GetComparableType(Type type)
        {
            if (type.IsGenericInstanceOf(typeof(IQueryable<>)))
            {
                type = typeof(IEnumerable<>).MakeGenericTypeFrom(type);
            }
            else if (type.IsGenericInstanceOf(typeof(IOrderedQueryable<>)))
            {
                type = typeof(IOrderedEnumerable<>).MakeGenericTypeFrom(type);
            }
            else if (type.IsGenericInstanceOf(typeof(Expression<>)))
            {
                type = type.GetFirstGenericArgument();
            }
            else if (type == typeof(IQueryable))
            {
                type = typeof(IEnumerable);
            }
            return type;
        }

        private static MethodInfo GetMatchingMethod(MethodInfo method, Type declaring)
        {
            foreach (var candidate in declaring.GetMethods())
            {
                if (MethodMatch(candidate, method))
                {
                    if (method.IsGenericMethod)
                    {
                        return candidate.MakeGenericMethodFrom(method);
                    }
                    return candidate;
                }
            }
            return null;
        }

        private static Type GetTargetDeclaringType(MethodInfo method)
        {
            return method.DeclaringType == typeof(Queryable) ? typeof(Enumerable) : method.DeclaringType;
        }

        private static bool HasExtensionAttribute(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(ExtensionAttribute), false).Length > 0;
        }

        private static bool IsQueryableExtension(MethodInfo method)
        {
            return HasExtensionAttribute(method) && method.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IQueryable));
        }

        private static bool MethodMatch(MethodInfo candidate, MethodInfo method)
        {
            if (candidate.Name != method.Name || !HasExtensionAttribute(candidate))
            {
                return false;
            }
            var parameters = method.GetParameterTypes();
            if (parameters.Length != candidate.GetParameters().Length)
            {
                return false;
            }
            else if (method.IsGenericMethod)
            {
                if (!candidate.IsGenericMethod || candidate.GetGenericArguments().Length != method.GetGenericArguments().Length)
                {
                    return false;
                }
                candidate = candidate.MakeGenericMethodFrom(method);
            }
            if (!TypeMatch(candidate.ReturnType, method.ReturnType))
            {
                return false;
            }
            var candidateParameters = candidate.GetParameterTypes();
            if (candidateParameters[0] != GetComparableType(parameters[0]))
            {
                return false;
            }
            for (var index = 1; index < candidateParameters.Length; ++index)
            {
                if (!TypeMatch(candidateParameters[index], parameters[index]))
                {
                    return false;
                }
            }
            return true;
        }

        private static MethodInfo ReplaceQueryableMethod(MethodInfo method)
        {
            var targetType = GetTargetDeclaringType(method);
            var result = GetMatchingMethod(method, targetType);
            if (result != null)
            {
                return result;
            }
            throw new InvalidOperationException
                (
                    string.Format
                    (
                        "There is no method {0} on type {1} that matches the specified arguments",
                        method.Name,
                        targetType.FullName
                    )
                );
        }

        private static bool TypeMatch(Type candidate, Type type)
        {
            if (candidate == type)
            {
                return true;
            }
            return candidate == GetComparableType(type);
        }

        private static Expression UnquoteIfNeeded(Expression expression, Type delegateType)
        {
            if (expression.NodeType != ExpressionType.Quote)
            {
                return expression;
            }
            var lambda = (LambdaExpression)((UnaryExpression)expression).Operand;
            if (lambda.Type == delegateType)
            {
                return lambda;
            }
            return expression;
        }

        private MethodCallExpression ReplaceQueryableMethod(MethodCallExpression old)
        {
            Expression target = null;
            if (old.Object != null)
            {
                target = Visit(old.Object);
            }
            var method = ReplaceQueryableMethod(old.Method);
            var parameters = method.GetParameters();
            var arguments = new Expression[old.Arguments.Count];
            for (var index = 0; index < arguments.Length; index++)
            {
                arguments[index] = UnquoteIfNeeded
                    (
                        Visit(old.Arguments[index]),
                        parameters[index].ParameterType
                    );
            }
            return Expression.Call(target, method, arguments);
        }
    }
}