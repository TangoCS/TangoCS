//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System;
    using System.ComponentModel;
    using System.ServiceModel;

    //[Obsolete(HttpChannelUtilities.ObsoleteDescriptionStrings.TypeObsoleteUseAllowCookies, false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class HttpCookieContainerBindingElement : BindingElement
    {
        //[Obsolete(HttpChannelUtilities.ObsoleteDescriptionStrings.TypeObsoleteUseAllowCookies, false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HttpCookieContainerBindingElement() 
        { 
        }

        //[Obsolete(HttpChannelUtilities.ObsoleteDescriptionStrings.TypeObsoleteUseAllowCookies, false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected HttpCookieContainerBindingElement(HttpCookieContainerBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override BindingElement Clone()
        { 
            return new HttpCookieContainerBindingElement(this); 
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            }

            if (!context.Binding.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                !context.Binding.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(
                        Res.GetString(Res.S("CookieContainerBindingElementNeedsHttp"), typeof(HttpCookieContainerBindingElement))));
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            return context.GetInnerProperty<T>();
        }
    }
}
