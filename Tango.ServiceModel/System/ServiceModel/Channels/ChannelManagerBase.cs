//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
namespace System.ServiceModel.Channels
{
	using System;
	using System.ServiceModel;

	public abstract class ChannelManagerBase : CommunicationObject, IDefaultCommunicationTimeouts
    {
        protected ChannelManagerBase()
        {
        }

        protected abstract TimeSpan DefaultReceiveTimeout { get; }
        protected abstract TimeSpan DefaultSendTimeout { get; }

        internal TimeSpan InternalReceiveTimeout
        {
            get { return this.DefaultReceiveTimeout; }
        }

        internal TimeSpan InternalSendTimeout
        {
            get { return this.DefaultSendTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
        {
            get { return this.DefaultCloseTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
        {
            get { return this.DefaultOpenTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
        {
            get { return this.DefaultReceiveTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.SendTimeout
        {
            get { return this.DefaultSendTimeout; }
        }

        internal Exception CreateChannelTypeNotSupportedException(Type type)
        {
            return new ArgumentException(Res.GetString(Res.S("ChannelTypeNotSupported"), type), "TChannel");
        }
    }
}
