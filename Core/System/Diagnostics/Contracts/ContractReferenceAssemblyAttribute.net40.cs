﻿using System;

#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Theraot.Core.System.Diagnostics.Contracts
{
    /// <summary>
    /// Attribute that specifies that an assembly is a reference assembly with contracts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ContractReferenceAssemblyAttribute : Attribute
    {
        // Empty
    }
}

#endif