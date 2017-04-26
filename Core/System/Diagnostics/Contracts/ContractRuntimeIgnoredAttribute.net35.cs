using System;
using System.Diagnostics;

#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Theraot.Core.System.Diagnostics.Contracts
{
    /// <summary>
    /// Methods (and properties) marked with this attribute can be used within calls to Contract methods, but have no runtime behavior associated with them.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ContractRuntimeIgnoredAttribute : Attribute
    {
        // Empty
    }
}

#endif