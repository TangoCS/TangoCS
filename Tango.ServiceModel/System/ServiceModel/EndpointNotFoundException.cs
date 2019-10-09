//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    [Serializable]
    public class EndpointNotFoundException : CommunicationException
    {
        public EndpointNotFoundException() { }
        public EndpointNotFoundException(string message) : base(message) { }
        public EndpointNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
