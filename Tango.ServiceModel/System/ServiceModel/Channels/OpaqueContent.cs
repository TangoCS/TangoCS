// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime;
    using System.Threading.Tasks;

    class OpaqueContent : HttpContent
    {
        MessageEncoder messageEncoder;
        Message message;
        string mtomBoundary;

        public OpaqueContent(MessageEncoder encoder, Message message, string mtomBoundary)
        {
            this.messageEncoder = encoder;
            this.message = message;
            this.mtomBoundary = mtomBoundary;
        }

        public bool IsEmpty
        {
            get 
            {
                return this.message.IsEmpty;
            }
        }

        public void WriteToStream(Stream stream)
        {
            MtomMessageEncoder mtomMessageEncoder = this.messageEncoder as MtomMessageEncoder;
            if (mtomMessageEncoder == null)
            {
                this.messageEncoder.WriteMessage(this.message, stream);
            }
            else
            {
                mtomMessageEncoder.WriteMessage(this.message, stream, this.mtomBoundary);
            }
        }

		//public IAsyncResult BeginWriteToStream(Stream stream, AsyncCallback callback, object state)
		//{
		//    MtomMessageEncoder mtomMessageEncoder = this.messageEncoder as MtomMessageEncoder;
		//    if (mtomMessageEncoder == null)
		//    {
		//        return this.messageEncoder.BeginWriteMessage(this.message, stream, callback, state);
		//    }
		//    else
		//    {
		//        return mtomMessageEncoder.BeginWriteMessage(this.message, stream, this.mtomBoundary, callback, state);
		//    }
		//}

		//public void EndWriteToStream(IAsyncResult result)
		//{
		//    this.messageEncoder.EndWriteMessage(result);
		//}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
				Res.GetString(Res.S("WebSocketOpaqueStreamContentNotSupportError"))));
		}

		protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
