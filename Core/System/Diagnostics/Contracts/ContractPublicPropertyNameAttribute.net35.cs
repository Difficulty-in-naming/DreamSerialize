using System;
using System.Diagnostics;

#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Theraot.Core.System.Diagnostics.Contracts
{
    /// <summary>
    /// Allows a field f to be used in the method contracts for a method m when f has less visibility than m.
    /// For instance, if the method is public, but the field is private.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ContractPublicPropertyNameAttribute : Attribute
    {
        private readonly string _publicName;

        public ContractPublicPropertyNameAttribute(string name)
        {
            _publicName = name;
        }

        public string Name
        {
            get { return _publicName; }
        }
    }
}

#endif