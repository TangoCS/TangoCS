//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
	using System.ServiceModel;

	public abstract class MessageEncodingBindingElement : BindingElement
    {
        protected MessageEncodingBindingElement()
        {
        }

        protected MessageEncodingBindingElement(MessageEncodingBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
        }

        public abstract MessageVersion MessageVersion { get; set; }


        internal virtual bool IsWsdlExportable
        {
            get { return true; }
        }

        internal IChannelFactory<TChannel> InternalBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        internal bool InternalCanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            }

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        //internal IChannelListener<TChannel> InternalBuildChannelListener<TChannel>(BindingContext context)
        //    where TChannel : class, IChannel
        //{
        //    if (context == null)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
        //    }

        //    context.BindingParameters.Add(this);
        //    return context.BuildInnerChannelListener<TChannel>();
        //}

        //internal bool InternalCanBuildChannelListener<TChannel>(BindingContext context)
        //    where TChannel : class, IChannel
        //{
        //    if (context == null)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
        //    }

        //    context.BindingParameters.Add(this);
        //    return context.CanBuildInnerChannelListener<TChannel>();
        //}

        public abstract MessageEncoderFactory CreateMessageEncoderFactory();

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)this.MessageVersion;
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal virtual bool CheckEncodingVersion(EnvelopeVersion version)
        {
            return false;
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
                return false;

            MessageEncodingBindingElement encoding = b as MessageEncodingBindingElement;
            if (encoding == null)
                return false;

            return true;
        }
    }
}
