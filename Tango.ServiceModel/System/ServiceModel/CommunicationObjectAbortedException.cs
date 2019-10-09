//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class CommunicationObjectAbortedException : CommunicationException
    {
        public CommunicationObjectAbortedException() { }
        public CommunicationObjectAbortedException(string message) : base(message) { }
        public CommunicationObjectAbortedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
