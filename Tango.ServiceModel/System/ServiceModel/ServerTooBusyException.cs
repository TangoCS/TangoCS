//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class ServerTooBusyException : CommunicationException
    {
        public ServerTooBusyException() { }
        public ServerTooBusyException(string message) : base(message) { }
        public ServerTooBusyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
