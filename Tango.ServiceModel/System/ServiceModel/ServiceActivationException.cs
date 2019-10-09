//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class ServiceActivationException : CommunicationException
    {
        public ServiceActivationException() { }
        public ServiceActivationException(string message) : base(message) { }
        public ServiceActivationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
