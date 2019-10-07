//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Channels;

    internal sealed class MessageOperationFormatter : IClientMessageFormatter, IDispatchMessageFormatter
    {
        static MessageOperationFormatter instance;

        internal static MessageOperationFormatter Instance
        {
            get
            {
                if (MessageOperationFormatter.instance == null)
                    MessageOperationFormatter.instance = new MessageOperationFormatter();
                return MessageOperationFormatter.instance;
            }
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
            if (parameters != null && parameters.Length > 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(Res.GetString(Res.S("SFxParametersMustBeEmpty"))));

            return message;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
            if (parameters == null)
                throw TraceUtility.ThrowHelperError(new ArgumentNullException("parameters"), message);
            if (parameters.Length != 1)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(Res.GetString(Res.S("SFxParameterMustBeArrayOfOneElement"))));

            parameters[0] = message;
        }

        public bool IsFault(string operation, Exception error)
        {
            return false;
        }

        public MessageFault SerializeFault(Exception error)
        {
            
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("SFxMessageOperationFormatterCannotSerializeFault"))));
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            if (!(result is Message))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(Res.GetString(Res.S("SFxResultMustBeMessage"))));
            if (parameters != null && parameters.Length > 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(Res.GetString(Res.S("SFxParametersMustBeEmpty"))));

            return (Message)result;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parameters"));
            if (parameters.Length != 1 || !(parameters[0] is Message))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(Res.GetString(Res.S("SFxParameterMustBeMessage"))));

            return (Message)parameters[0];
        }
    }
}
