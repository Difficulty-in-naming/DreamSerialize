// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Theraot.Core.System.Dynamic.Utils
{
    internal static class Strings
    {
        /// <summary>
        /// A string like "Invalid argument value"
        /// </summary>
        internal static string InvalidArgumentValue
        {
            get { return SR.InvalidArgumentValue; }
        }

        /// <summary>
        /// A string like "Non-empty collection required"
        /// </summary>
        internal static string NonEmptyCollectionRequired
        {
            get { return SR.NonEmptyCollectionRequired; }
        }

        internal static string InvalidNullValue(object p0)
        {
            return SR.Format(SR.InvalidNullValue, p0);
        }

        internal static string InvalidObjectType(object p0, object p1)
        {
            return SR.Format(SR.InvalidObjectType, p0, p1);
        }

        internal static string TypeContainsGenericParameters(object p0)
        {
            return SR.Format(SR.TypeContainsGenericParameters, p0);
        }

        internal static string TypeIsGeneric(object p0)
        {
            return SR.Format(SR.TypeIsGeneric, p0);
        }

        /// <summary>
        /// A string like "Collection was modified; enumeration operation may not execute."
        /// </summary>
        internal static string CollectionModifiedWhileEnumerating
        {
            get { return SR.CollectionModifiedWhileEnumerating; }
        }

        /// <summary>
        /// A string like "Enumeration has either not started or has already finished."
        /// </summary>
        internal static string EnumerationIsDone
        {
            get { return SR.EnumerationIsDone; }
        }

        internal static string ExpressionMustBeReadable
        {
            get { return SR.ExpressionMustBeReadable; }
        }

        internal static string ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchMethodParameter, p0, p1, p2);
        }

        internal static string ExpressionTypeDoesNotMatchParameter(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchParameter, p0, p1);
        }

        internal static string ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchConstructorParameter, p0, p1);
        }

        internal static string IncorrectNumberOfMethodCallArguments(object p0)
        {
            return SR.Format(SR.IncorrectNumberOfMethodCallArguments, p0);
        }

        /// <summary>
        /// A string like "Incorrect number of arguments supplied for lambda invocation"
        /// </summary>
        internal static string IncorrectNumberOfLambdaArguments
        {
            get { return SR.IncorrectNumberOfLambdaArguments; }
        }

        /// <summary>
        /// A string like "Incorrect number of arguments for constructor"
        /// </summary>
        internal static string IncorrectNumberOfConstructorArguments
        {
            get { return SR.IncorrectNumberOfConstructorArguments; }
        }
    }
}