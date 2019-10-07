//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class ChannelTerminatedException : CommunicationException
    {
        public ChannelTerminatedException() { }
        public ChannelTerminatedException(string message) : base(message) { }
        public ChannelTerminatedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
