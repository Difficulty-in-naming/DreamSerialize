using System;
using System.Diagnostics;

#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Theraot.Core.System.Diagnostics.Contracts
{
    /// <summary>
    /// This attribute is used to mark a method as being the invariant
    /// method for a class. The method can have any name, but it must
    /// return "void" and take no parameters. The body of the method
    /// must consist solely of one or more calls to the method
    /// Contract.Invariant. A suggested name for the method is
    /// "ObjectInvariant".
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ContractInvariantMethodAttribute : Attribute
    {
        // Empty
    }
}

#endif