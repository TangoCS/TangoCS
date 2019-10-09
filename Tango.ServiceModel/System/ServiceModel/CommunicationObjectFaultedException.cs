//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class CommunicationObjectFaultedException : CommunicationException
    {
        public CommunicationObjectFaultedException() { }
        public CommunicationObjectFaultedException(string message) : base(message) { }
        public CommunicationObjectFaultedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
