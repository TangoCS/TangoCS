//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
	using System.Globalization;
	using System.ServiceModel.Channels;

	[Serializable]
    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, Exception innerException) : base(message, innerException) { }

        internal static ProtocolException ReceiveShutdownReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = Res.GetString(Res.S("ReceiveShutdownReturnedFault"), reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = Res.GetString(Res.S("ReceiveShutdownReturnedLargeFault"), message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = Res.GetString(Res.S("ReceiveShutdownReturnedMessage"), message.Headers.Action);
                return new ProtocolException(text);
            }
        }

        internal static ProtocolException OneWayOperationReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = Res.GetString(Res.S("OneWayOperationReturnedFault"), reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = Res.GetString(Res.S("OneWayOperationReturnedLargeFault"), message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = Res.GetString(Res.S("OneWayOperationReturnedMessage"), message.Headers.Action);
                return new ProtocolException(text);
            }
        }
    }
}
