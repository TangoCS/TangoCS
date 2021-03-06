//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    [Serializable]
    public class ActionNotSupportedException : CommunicationException
    {
        public ActionNotSupportedException() { }
        public ActionNotSupportedException(string message) : base(message) { }
        public ActionNotSupportedException(string message, Exception innerException) : base(message, innerException) { }

        internal Message ProvideFault(MessageVersion messageVersion)
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.ActionNotSupported, messageVersion.Addressing.Namespace);
            string reason = this.Message;
            return System.ServiceModel.Channels.Message.CreateMessage(
               messageVersion, code, reason, messageVersion.Addressing.FaultAction);
        }
    }
}
