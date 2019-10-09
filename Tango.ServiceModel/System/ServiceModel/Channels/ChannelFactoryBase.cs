//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System.Runtime;
    using System.ServiceModel;

    public abstract class ChannelFactoryBase : ChannelManagerBase, IChannelFactory
    {
        TimeSpan closeTimeout = ServiceDefaults.CloseTimeout;
        TimeSpan openTimeout = ServiceDefaults.OpenTimeout;
        TimeSpan receiveTimeout = ServiceDefaults.ReceiveTimeout;
        TimeSpan sendTimeout = ServiceDefaults.SendTimeout;

        protected ChannelFactoryBase()
        {
        }

        protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts)
        {
            this.InitializeTimeouts(timeouts);
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return this.closeTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return this.openTimeout; }
        }

        protected override TimeSpan DefaultReceiveTimeout
        {
            get { return this.receiveTimeout; }
        }

        protected override TimeSpan DefaultSendTimeout
        {
            get { return this.sendTimeout; }
        }

        public virtual T GetProperty<T>()
            where T : class
        {
            if (typeof(T) == typeof(IChannelFactory))
            {
                return (T)(object)this;
            }

            return default(T);
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }


        void InitializeTimeouts(IDefaultCommunicationTimeouts timeouts)
        {
            if (timeouts != null)
            {
                this.closeTimeout = timeouts.CloseTimeout;
                this.openTimeout = timeouts.OpenTimeout;
                this.receiveTimeout = timeouts.ReceiveTimeout;
                this.sendTimeout = timeouts.SendTimeout;
            }
        }
    }

    public abstract class ChannelFactoryBase<TChannel> : ChannelFactoryBase, IChannelFactory<TChannel>
    {
        CommunicationObjectManager<IChannel> channels;

        protected ChannelFactoryBase()
            : this(null)
        {
        }

        protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts)
            : base(timeouts)
        {
            this.channels = new CommunicationObjectManager<IChannel>(this.ThisLock);
        }

        public TChannel CreateChannel(EndpointAddress address)
        {
            if (address == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");

            return this.InternalCreateChannel(address, address.Uri);
        }

        public TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");

            if (via == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("via");

            return this.InternalCreateChannel(address, via);
        }

        TChannel InternalCreateChannel(EndpointAddress address, Uri via)
        {
            this.ValidateCreateChannel();
            TChannel channel = this.OnCreateChannel(address, via);

            bool success = false;

            try
            {
                this.channels.Add((IChannel)(object)channel);
                success = true;
            }
            finally
            {
                if (!success)
                    ((IChannel)(object)channel).Abort();
            }

            return channel;
        }

        protected abstract TChannel OnCreateChannel(EndpointAddress address, Uri via);

        protected void ValidateCreateChannel()
        {
            ThrowIfDisposed();
            if (this.State != CommunicationState.Opened)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("ChannelFactoryCannotBeUsedToCreateChannels"), this.GetType().ToString())));
            }
        }

        protected override void OnAbort()
        {
            IChannel[] currentChannels = this.channels.ToArray();
            foreach (IChannel channel in currentChannels)
                channel.Abort();

            this.channels.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            IChannel[] currentChannels = this.channels.ToArray();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            foreach (IChannel channel in currentChannels)
                channel.Close(timeoutHelper.RemainingTime());

            this.channels.Close(timeoutHelper.RemainingTime());
        }
    }
}
