//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Security
{
	using System.ServiceModel.Channels;
	using System.ServiceModel;

	[Serializable]
    public class MessageSecurityException : CommunicationException
    {
        MessageFault fault;
        bool isReplay = false;

        public MessageSecurityException()
            : base()
        {
        }

        public MessageSecurityException(String message)
            : base(message)
        {
        }

        public MessageSecurityException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal MessageSecurityException(string message, Exception innerException, MessageFault fault)
            : base(message, innerException)
        {
            this.fault = fault;
        }

        internal MessageSecurityException(String message, bool isReplay)
            : base(message)
        {
            this.isReplay = isReplay;
        }

        internal bool ReplayDetected
        {
            get
            {
                return this.isReplay;
            }
        }

        internal MessageFault Fault
        {
            get { return this.fault; }
        }
    }
}
