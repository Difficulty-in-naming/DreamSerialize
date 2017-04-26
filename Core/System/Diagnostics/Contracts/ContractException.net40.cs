#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Theraot.Core.System.Runtime.CompilerServices;

namespace Theraot.Core.System.Diagnostics.Contracts
{
    [Serializable]
    internal sealed class ContractException : Exception
    {
        private readonly ContractFailureKind _kind;
        private readonly string _userMessage;
        private readonly string _condition;

        public ContractFailureKind Kind
        {
            get { return _kind; }
        }

        public string Failure
        {
            get { return Message; }
        }

        public string UserMessage
        {
            get { return _userMessage; }
        }

        public string Condition
        {
            get { return _condition; }
        }

        // Called by COM Interop, if we see Cor_E_Codecontractfailed as an HRESULT.
        private ContractException()
        {
            HResult = ContractHelper.Cor_E_Codecontractfailed;
        }

        public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException)
            : base(failure, innerException)
        {
            HResult = ContractHelper.Cor_E_Codecontractfailed;
            _kind = kind;
            _userMessage = userMessage;
            _condition = condition;
        }

        private ContractException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _kind = (ContractFailureKind)info.GetInt32("Kind");
            _userMessage = info.GetString("UserMessage");
            _condition = info.GetString("Condition");
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Kind", _kind);
            info.AddValue("UserMessage", _userMessage);
            info.AddValue("Condition", _condition);
        }
    }
}

#endif