//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime;
    using System.ServiceModel;
    using System.Text;
    using System.Xml;

    class BinaryMessageEncoderFactory : MessageEncoderFactory
    {
        const int maxPooledXmlReaderPerMessage = 2;

        BinaryMessageEncoder messageEncoder;
        MessageVersion messageVersion;
        int maxReadPoolSize;
        int maxWritePoolSize;
        CompressionFormat compressionFormat;

        // Double-checked locking pattern requires volatile for read/write synchronization
        volatile SynchronizedPool<XmlDictionaryWriter> streamedWriterPool;
        volatile SynchronizedPool<XmlDictionaryReader> streamedReaderPool;
        volatile SynchronizedPool<BinaryBufferedMessageData> bufferedDataPool;
        volatile SynchronizedPool<BinaryBufferedMessageWriter> bufferedWriterPool;
        volatile SynchronizedPool<RecycledMessageState> recycledStatePool;

        object thisLock;
        int maxSessionSize;
        OnXmlDictionaryReaderClose onStreamedReaderClose;
        XmlDictionaryReaderQuotas readerQuotas;
        XmlDictionaryReaderQuotas bufferedReadReaderQuotas;
        BinaryVersion binaryVersion;

        public BinaryMessageEncoderFactory(MessageVersion messageVersion, int maxReadPoolSize, int maxWritePoolSize, int maxSessionSize,
            XmlDictionaryReaderQuotas readerQuotas, long maxReceivedMessageSize, BinaryVersion version, CompressionFormat compressionFormat)
        {
            this.messageVersion = messageVersion;
            this.maxReadPoolSize = maxReadPoolSize;
            this.maxWritePoolSize = maxWritePoolSize;
            this.maxSessionSize = maxSessionSize;
            this.thisLock = new object();
            this.onStreamedReaderClose = new OnXmlDictionaryReaderClose(ReturnStreamedReader);
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            if (readerQuotas != null)
            {
                readerQuotas.CopyTo(this.readerQuotas);
            }

            this.bufferedReadReaderQuotas = EncoderHelpers.GetBufferedReadQuotas(this.readerQuotas);
            this.MaxReceivedMessageSize = maxReceivedMessageSize;

            this.binaryVersion = version;
            this.compressionFormat = compressionFormat;
            this.messageEncoder = new BinaryMessageEncoder(this, false, 0);
        }

        public static IXmlDictionary XmlDictionary
        {
            get { return XD.Dictionary; }
        }

        public override MessageEncoder Encoder
        {
            get
            {
                return messageEncoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get { return messageVersion; }
        }

        public int MaxWritePoolSize
        {
            get { return maxWritePoolSize; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return readerQuotas;
            }
        }

        public int MaxReadPoolSize
        {
            get { return maxReadPoolSize; }
        }

        public int MaxSessionSize
        {
            get { return maxSessionSize; }
        }

        public CompressionFormat CompressionFormat
        {
            get { return this.compressionFormat; }
        }

        long MaxReceivedMessageSize
        {
            get;
            set;
        }

        object ThisLock
        {
            get { return thisLock; }
        }

        SynchronizedPool<RecycledMessageState> RecycledStatePool
        {
            get
            {
                if (recycledStatePool == null)
                {
                    lock (ThisLock)
                    {
                        if (recycledStatePool == null)
                        {
                            //running = true;
                            recycledStatePool = new SynchronizedPool<RecycledMessageState>(maxReadPoolSize);
                        }
                    }
                }
                return recycledStatePool;
            }
        }

        public override MessageEncoder CreateSessionEncoder()
        {
            return new BinaryMessageEncoder(this, true, maxSessionSize);
        }

        XmlDictionaryWriter TakeStreamedWriter(Stream stream)
        {
            if (streamedWriterPool == null)
            {
                lock (ThisLock)
                {
                    if (streamedWriterPool == null)
                    {
                        //running = true;
                        streamedWriterPool = new SynchronizedPool<XmlDictionaryWriter>(maxWritePoolSize);
                    }
                }
            }
            XmlDictionaryWriter xmlWriter = streamedWriterPool.Take();
            if (xmlWriter == null)
            {
                xmlWriter = XmlDictionaryWriter.CreateBinaryWriter(stream, binaryVersion.Dictionary, null, false);
            }
            else
            {
                ((IXmlBinaryWriterInitializer)xmlWriter).SetOutput(stream, binaryVersion.Dictionary, null, false);
            }
            return xmlWriter;
        }

        void ReturnStreamedWriter(XmlDictionaryWriter xmlWriter)
        {
            xmlWriter.Close();
            streamedWriterPool.Return(xmlWriter);
        }

        BinaryBufferedMessageWriter TakeBufferedWriter()
        {
            if (bufferedWriterPool == null)
            {
                lock (ThisLock)
                {
                    if (bufferedWriterPool == null)
                    {
                        //running = true;
                        bufferedWriterPool = new SynchronizedPool<BinaryBufferedMessageWriter>(maxWritePoolSize);
                    }
                }
            }

            BinaryBufferedMessageWriter messageWriter = bufferedWriterPool.Take();
            if (messageWriter == null)
            {
                messageWriter = new BinaryBufferedMessageWriter(binaryVersion.Dictionary);
            }
            return messageWriter;
        }

        void ReturnMessageWriter(BinaryBufferedMessageWriter messageWriter)
        {
            bufferedWriterPool.Return(messageWriter);
        }

        XmlDictionaryReader TakeStreamedReader(Stream stream)
        {
            if (streamedReaderPool == null)
            {
                lock (ThisLock)
                {
                    if (streamedReaderPool == null)
                    {
                        //running = true;
                        streamedReaderPool = new SynchronizedPool<XmlDictionaryReader>(maxReadPoolSize);
                    }
                }
            }

            XmlDictionaryReader xmlReader = streamedReaderPool.Take();
            if (xmlReader == null)
            {
                xmlReader = XmlDictionaryReader.CreateBinaryReader(stream,
                    binaryVersion.Dictionary,
                    readerQuotas,
                    null,
                    onStreamedReaderClose);
            }
            else
            {
                ((IXmlBinaryReaderInitializer)xmlReader).SetInput(stream,
                    binaryVersion.Dictionary,
                    readerQuotas,
                    null,
                    onStreamedReaderClose);
            }

            return xmlReader;
        }

        void ReturnStreamedReader(XmlDictionaryReader xmlReader)
        {
            streamedReaderPool.Return(xmlReader);
        }

        BinaryBufferedMessageData TakeBufferedData(BinaryMessageEncoder messageEncoder)
        {
            if (bufferedDataPool == null)
            {
                lock (ThisLock)
                {
                    if (bufferedDataPool == null)
                    {
                        //running = true;
                        bufferedDataPool = new SynchronizedPool<BinaryBufferedMessageData>(maxReadPoolSize);
                    }
                }
            }
            BinaryBufferedMessageData messageData = bufferedDataPool.Take();
            if (messageData == null)
            {
                messageData = new BinaryBufferedMessageData(this, maxPooledXmlReaderPerMessage);
            }
            messageData.SetMessageEncoder(messageEncoder);
            return messageData;
        }

        void ReturnBufferedData(BinaryBufferedMessageData messageData)
        {
            messageData.SetMessageEncoder(null);
            bufferedDataPool.Return(messageData);
        }

        class BinaryBufferedMessageData : BufferedMessageData
        {
            BinaryMessageEncoderFactory factory;
            BinaryMessageEncoder messageEncoder;
            Pool<XmlDictionaryReader> readerPool;
            OnXmlDictionaryReaderClose onClose;

            public BinaryBufferedMessageData(BinaryMessageEncoderFactory factory, int maxPoolSize)
                : base(factory.RecycledStatePool)
            {
                this.factory = factory;
                readerPool = new Pool<XmlDictionaryReader>(maxPoolSize);
                onClose = new OnXmlDictionaryReaderClose(OnXmlReaderClosed);
            }

            public override MessageEncoder MessageEncoder
            {
                get { return messageEncoder; }
            }

            public override XmlDictionaryReaderQuotas Quotas
            {
                get { return factory.readerQuotas; }
            }

            public void SetMessageEncoder(BinaryMessageEncoder messageEncoder)
            {
                this.messageEncoder = messageEncoder;
            }

            protected override XmlDictionaryReader TakeXmlReader()
            {
                ArraySegment<byte> buffer = this.Buffer;
                XmlDictionaryReader xmlReader = readerPool.Take();

                if (xmlReader != null)
                {
                    ((IXmlBinaryReaderInitializer)xmlReader).SetInput(buffer.Array, buffer.Offset, buffer.Count,
                        factory.binaryVersion.Dictionary,
                        factory.bufferedReadReaderQuotas,
                        messageEncoder.ReaderSession,
                        onClose);
                }
                else
                {
                    xmlReader = XmlDictionaryReader.CreateBinaryReader(buffer.Array, buffer.Offset, buffer.Count,
                        factory.binaryVersion.Dictionary,
                        factory.bufferedReadReaderQuotas,
                        messageEncoder.ReaderSession,
                        onClose);
                }

                return xmlReader;
            }

            protected override void ReturnXmlReader(XmlDictionaryReader reader)
            {
                readerPool.Return(reader);
            }

            protected override void OnClosed()
            {
                factory.ReturnBufferedData(this);
            }
        }

        class BinaryBufferedMessageWriter : BufferedMessageWriter
        {
            XmlDictionaryWriter writer;
            IXmlDictionary dictionary;
            XmlBinaryWriterSession session;

            public BinaryBufferedMessageWriter(IXmlDictionary dictionary)
            {
                this.dictionary = dictionary;
            }

            public BinaryBufferedMessageWriter(IXmlDictionary dictionary, XmlBinaryWriterSession session)
            {
                this.dictionary = dictionary;
                this.session = session;
            }

            protected override XmlDictionaryWriter TakeXmlWriter(Stream stream)
            {
                XmlDictionaryWriter returnedWriter = writer;
                if (returnedWriter == null)
                {
                    returnedWriter = XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, session, false);
                }
                else
                {
                    writer = null;
                    ((IXmlBinaryWriterInitializer)returnedWriter).SetOutput(stream, dictionary, session, false);
                }
                return returnedWriter;
            }

            protected override void ReturnXmlWriter(XmlDictionaryWriter writer)
            {
                writer.Close();

                if (this.writer == null)
                {
                    this.writer = writer;
                }
            }
        }

        class BinaryMessageEncoder : MessageEncoder, ICompressedMessageEncoder //, ITraceSourceStringProvider
        {
            const string SupportedCompressionTypesMessageProperty = "BinaryMessageEncoder.SupportedCompressionTypes";

            BinaryMessageEncoderFactory factory;
            bool isSession;
            XmlBinaryWriterSessionWithQuota writerSession;
            BinaryBufferedMessageWriter sessionMessageWriter;
            XmlBinaryReaderSession readerSession;
            //XmlBinaryReaderSession readerSessionForLogging;
            //bool readerSessionForLoggingIsInvalid = false;
            //int writeIdCounter;
            int idCounter;
            int maxSessionSize;
            int remainingReaderSessionSize;
            bool isReaderSessionInvalid;
            MessagePatterns messagePatterns;
            string contentType;
            string normalContentType;
            string gzipCompressedContentType;
            string deflateCompressedContentType;
            CompressionFormat sessionCompressionFormat;
            readonly long maxReceivedMessageSize; 

            public BinaryMessageEncoder(BinaryMessageEncoderFactory factory, bool isSession, int maxSessionSize)
            {
                this.factory = factory;
                this.isSession = isSession;
                this.maxSessionSize = maxSessionSize;
                this.remainingReaderSessionSize = maxSessionSize;
                this.normalContentType = isSession ? factory.binaryVersion.SessionContentType : factory.binaryVersion.ContentType;
                this.gzipCompressedContentType = isSession ? BinaryVersion.GZipVersion1.SessionContentType : BinaryVersion.GZipVersion1.ContentType;
                this.deflateCompressedContentType = isSession ? BinaryVersion.DeflateVersion1.SessionContentType : BinaryVersion.DeflateVersion1.ContentType;
                this.sessionCompressionFormat = this.factory.CompressionFormat;
                this.maxReceivedMessageSize = this.factory.MaxReceivedMessageSize;

                switch (this.factory.CompressionFormat)
                {
                    case CompressionFormat.Deflate:
                        this.contentType = this.deflateCompressedContentType;
                        break;
                    case CompressionFormat.GZip:
                        this.contentType = this.gzipCompressedContentType;
                        break;
                    default:
                        this.contentType = this.normalContentType;
                        break;
                }
            }

            public override string ContentType
            {
                get
                {
                    return this.contentType;
                }
            }

            public override MessageVersion MessageVersion
            {
                get { return factory.messageVersion; }
            }

            public override string MediaType
            {
                get { return this.contentType; }
            }

            public XmlBinaryReaderSession ReaderSession
            {
                get { return readerSession; }
            }

            public bool CompressionEnabled
            {
                get { return this.factory.CompressionFormat != CompressionFormat.None; }
            }

            ArraySegment<byte> AddSessionInformationToMessage(ArraySegment<byte> messageData, BufferManager bufferManager, int maxMessageSize)
            {
                int dictionarySize = 0;
                byte[] buffer = messageData.Array;

                if (writerSession.HasNewStrings)
                {
                    IList<XmlDictionaryString> newStrings = writerSession.GetNewStrings();
                    for (int i = 0; i < newStrings.Count; i++)
                    {
                        int utf8ValueSize = Encoding.UTF8.GetByteCount(newStrings[i].Value);
                        dictionarySize += IntEncoder.GetEncodedSize(utf8ValueSize) + utf8ValueSize;
                    }

                    int messageSize = messageData.Offset + messageData.Count;
                    int remainingMessageSize = maxMessageSize - messageSize;
                    if (remainingMessageSize - dictionarySize < 0)
                    {
                        string excMsg = Res.GetString(Res.S("MaxSentMessageSizeExceeded"), maxMessageSize);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(excMsg));
                    }

                    int requiredBufferSize = messageData.Offset + messageData.Count + dictionarySize;
                    if (buffer.Length < requiredBufferSize)
                    {
                        byte[] newBuffer = bufferManager.TakeBuffer(requiredBufferSize);
                        Buffer.BlockCopy(buffer, messageData.Offset, newBuffer, messageData.Offset, messageData.Count);
                        bufferManager.ReturnBuffer(buffer);
                        buffer = newBuffer;
                    }

                    Buffer.BlockCopy(buffer, messageData.Offset, buffer, messageData.Offset + dictionarySize, messageData.Count);

                    int offset = messageData.Offset;
                    for (int i = 0; i < newStrings.Count; i++)
                    {
                        string newString = newStrings[i].Value;
                        int utf8ValueSize = Encoding.UTF8.GetByteCount(newString);
                        offset += IntEncoder.Encode(utf8ValueSize, buffer, offset);
                        offset += Encoding.UTF8.GetBytes(newString, 0, newString.Length, buffer, offset);
                    }

                    writerSession.ClearNewStrings();
                }

                int headerSize = IntEncoder.GetEncodedSize(dictionarySize);
                int newOffset = messageData.Offset - headerSize;
                int newSize = headerSize + messageData.Count + dictionarySize;
                IntEncoder.Encode(dictionarySize, buffer, newOffset);
                return new ArraySegment<byte>(buffer, newOffset, newSize);
            }

            ArraySegment<byte> ExtractSessionInformationFromMessage(ArraySegment<byte> messageData)
            {
                if (isReaderSessionInvalid)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataException(Res.GetString(Res.S("BinaryEncoderSessionInvalid"))));
                }

                byte[] buffer = messageData.Array;
                int dictionarySize;
                int headerSize;
                int newOffset;
                int newSize;
                bool throwing = true;
                try
                {
                    IntDecoder decoder = new IntDecoder();
                    headerSize = decoder.Decode(buffer, messageData.Offset, messageData.Count);
                    dictionarySize = decoder.Value;
                    if (dictionarySize > messageData.Count)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataException(Res.GetString(Res.S("BinaryEncoderSessionMalformed"))));
                    }
                    newOffset = messageData.Offset + headerSize + dictionarySize;
                    newSize = messageData.Count - headerSize - dictionarySize;
                    if (newSize < 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataException(Res.GetString(Res.S("BinaryEncoderSessionMalformed"))));
                    }
                    if (dictionarySize > 0)
                    {
                        if (dictionarySize > remainingReaderSessionSize)
                        {
                            string message = Res.GetString(Res.S("BinaryEncoderSessionTooLarge"), this.maxSessionSize);
                            Exception inner = new QuotaExceededException(message);
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(message, inner));
                        }
                        else
                        {
                            remainingReaderSessionSize -= dictionarySize;
                        }

                        int size = dictionarySize;
                        int offset = messageData.Offset + headerSize;

                        while (size > 0)
                        {
                            decoder.Reset();
                            int bytesDecoded = decoder.Decode(buffer, offset, size);
                            int utf8ValueSize = decoder.Value;
                            offset += bytesDecoded;
                            size -= bytesDecoded;
                            if (utf8ValueSize > size)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataException(Res.GetString(Res.S("BinaryEncoderSessionMalformed"))));
                            }
                            string value = Encoding.UTF8.GetString(buffer, offset, utf8ValueSize);
                            offset += utf8ValueSize;
                            size -= utf8ValueSize;
                            readerSession.Add(idCounter, value);
                            idCounter++;
                        }
                    }
                    throwing = false;
                }
                finally
                {
                    if (throwing)
                    {
                        isReaderSessionInvalid = true;
                    }
                }

                return new ArraySegment<byte>(buffer, newOffset, newSize);
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bufferManager");
                }

                CompressionFormat compressionFormat = this.CheckContentType(contentType);

                if (compressionFormat != CompressionFormat.None)
                {
                    MessageEncoderCompressionHandler.DecompressBuffer(ref buffer, bufferManager, compressionFormat, this.maxReceivedMessageSize);
                }

                if (isSession)
                {
                    if (readerSession == null)
                    {
                        readerSession = new XmlBinaryReaderSession();
                        messagePatterns = new MessagePatterns(factory.binaryVersion.Dictionary, readerSession, this.MessageVersion);
                    }
                    try
                    {
                        buffer = ExtractSessionInformationFromMessage(buffer);
                    }
                    catch (InvalidDataException)
                    {
                        throw;
                    }
                }
                BinaryBufferedMessageData messageData = factory.TakeBufferedData(this);
                Message message;
                if (messagePatterns != null)
                {
                    message = messagePatterns.TryCreateMessage(buffer.Array, buffer.Offset, buffer.Count, bufferManager, messageData);
                }
                else
                {
                    message = null;
                }
                if (message == null)
                {
                    messageData.Open(buffer, bufferManager);
                    RecycledMessageState messageState = messageData.TakeMessageState();
                    if (messageState == null)
                    {
                        messageState = new RecycledMessageState();
                    }
                    message = new BufferedMessage(messageData, messageState);
                }
                message.Properties.Encoder = this;

                return message;
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
                }

                CompressionFormat compressionFormat = this.CheckContentType(contentType);

                if (compressionFormat != CompressionFormat.None)
                {
                    stream = new MaxMessageSizeStream(
                        MessageEncoderCompressionHandler.GetDecompressStream(stream, compressionFormat), this.maxReceivedMessageSize);
                }

                XmlDictionaryReader reader = factory.TakeStreamedReader(stream);
                Message message = Message.CreateMessage(reader, maxSizeOfHeaders, factory.messageVersion);
                message.Properties.Encoder = this;

                return message;
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
                }

                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bufferManager");
                }

                if (maxMessageSize < 0)
                {

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxMessageSize", maxMessageSize,
                        Res.GetString(Res.S("ValueMustBeNonNegative"))));
                }


                message.Properties.Encoder = this;

                if (isSession)
                {
                    if (writerSession == null)
                    {
                        writerSession = new XmlBinaryWriterSessionWithQuota(maxSessionSize);
                        sessionMessageWriter = new BinaryBufferedMessageWriter(factory.binaryVersion.Dictionary, writerSession);
                    }
                    messageOffset += IntEncoder.MaxEncodedSize;
                }

                if (messageOffset < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("messageOffset", messageOffset,
                        Res.GetString(Res.S("ValueMustBeNonNegative"))));
                }

                if (messageOffset > maxMessageSize)
                {
                    string excMsg = Res.GetString(Res.S("MaxSentMessageSizeExceeded"), maxMessageSize);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(excMsg));
                }

                ThrowIfMismatchedMessageVersion(message);
                BinaryBufferedMessageWriter messageWriter;
                if (isSession)
                {
                    messageWriter = sessionMessageWriter;
                }
                else
                {
                    messageWriter = factory.TakeBufferedWriter();
                }
                ArraySegment<byte> messageData = messageWriter.WriteMessage(message, bufferManager, messageOffset, maxMessageSize);

                //this.readerSessionForLoggingIsInvalid = true;

				if (isSession)
                {
                    messageData = AddSessionInformationToMessage(messageData, bufferManager, maxMessageSize);
                }
                else
                {
                    factory.ReturnMessageWriter(messageWriter);
                }

                CompressionFormat compressionFormat = this.CheckCompressedWrite(message);
                if (compressionFormat != CompressionFormat.None)
                {
                    MessageEncoderCompressionHandler.CompressBuffer(ref messageData, bufferManager, compressionFormat);
                }

                return messageData;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
                }
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));
                }

                CompressionFormat compressionFormat = this.CheckCompressedWrite(message);
                if (compressionFormat != CompressionFormat.None)
                {
                    stream = MessageEncoderCompressionHandler.GetCompressStream(stream, compressionFormat);
                }

                ThrowIfMismatchedMessageVersion(message);
                message.Properties.Encoder = this;
                XmlDictionaryWriter xmlWriter = factory.TakeStreamedWriter(stream);
                message.WriteMessage(xmlWriter);
                xmlWriter.Flush();



                factory.ReturnStreamedWriter(xmlWriter);

                if (compressionFormat != CompressionFormat.None)
                {
                    stream.Close();
                }
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                bool supported = true;
                if (!base.IsContentTypeSupported(contentType))
                {
                    if (this.CompressionEnabled)
                    {
                        supported = (this.factory.CompressionFormat == CompressionFormat.GZip &&
                            base.IsContentTypeSupported(contentType, this.gzipCompressedContentType, this.gzipCompressedContentType)) ||
                            (this.factory.CompressionFormat == CompressionFormat.Deflate &&
                            base.IsContentTypeSupported(contentType, this.deflateCompressedContentType, this.deflateCompressedContentType)) ||
                            base.IsContentTypeSupported(contentType, this.normalContentType, this.normalContentType);
                    }
                    else
                    {
                        supported = false;
                    }
                }
                return supported;
            }

            public void SetSessionContentType(string contentType)
            {
                if (base.IsContentTypeSupported(contentType, this.gzipCompressedContentType, this.gzipCompressedContentType))
                {
                    this.sessionCompressionFormat = CompressionFormat.GZip;
                }
                else if (base.IsContentTypeSupported(contentType, this.deflateCompressedContentType, this.deflateCompressedContentType))
                {
                    this.sessionCompressionFormat = CompressionFormat.Deflate;
                }
                else
                {
                    this.sessionCompressionFormat = CompressionFormat.None;
                }
            }

            public void AddCompressedMessageProperties(Message message, string supportedCompressionTypes)
            {
                message.Properties.Add(SupportedCompressionTypesMessageProperty, supportedCompressionTypes);
            }

            static bool ContentTypeEqualsOrStartsWith(string contentType, string supportedContentType)
            {
                return contentType == supportedContentType || contentType.StartsWith(supportedContentType, StringComparison.OrdinalIgnoreCase);
            }

            CompressionFormat CheckContentType(string contentType)
            {
                CompressionFormat compressionFormat = CompressionFormat.None;
                if (contentType == null)
                {
                    compressionFormat = this.sessionCompressionFormat;
                }
                else
                {
                    if (!this.CompressionEnabled)
                    {
                        if (!ContentTypeEqualsOrStartsWith(contentType, this.ContentType))
                        {
                            throw FxTrace.Exception.AsError(new ProtocolException(Res.GetString(Res.S("EncoderUnrecognizedContentType"), contentType, this.ContentType)));
                        }
                    }
                    else
                    {
                        if (this.factory.CompressionFormat == CompressionFormat.GZip && ContentTypeEqualsOrStartsWith(contentType, this.gzipCompressedContentType))
                        {
                            compressionFormat = CompressionFormat.GZip;
                        }
                        else if (this.factory.CompressionFormat == CompressionFormat.Deflate && ContentTypeEqualsOrStartsWith(contentType, this.deflateCompressedContentType))
                        {
                            compressionFormat = CompressionFormat.Deflate;
                        }
                        else if (ContentTypeEqualsOrStartsWith(contentType, this.normalContentType))
                        {
                            compressionFormat = CompressionFormat.None;
                        }
                        else
                        {
                            throw FxTrace.Exception.AsError(new ProtocolException(Res.GetString(Res.S("EncoderUnrecognizedContentType"), contentType, this.ContentType)));
                        }
                    }
                }

                return compressionFormat;
            }

            CompressionFormat CheckCompressedWrite(Message message)
            {
                CompressionFormat compressionFormat = this.sessionCompressionFormat;
                if (compressionFormat != CompressionFormat.None && !this.isSession)
                {
                    string acceptEncoding;
                    if (message.Properties.TryGetValue<string>(SupportedCompressionTypesMessageProperty, out acceptEncoding) &&
                        acceptEncoding != null)
                    {
                        acceptEncoding = acceptEncoding.ToLowerInvariant();
                        if ((compressionFormat == CompressionFormat.GZip &&
                            !acceptEncoding.Contains(MessageEncoderCompressionHandler.GZipContentEncoding)) ||
                            (compressionFormat == CompressionFormat.Deflate &&
                            !acceptEncoding.Contains(MessageEncoderCompressionHandler.DeflateContentEncoding)))
                        {
                            compressionFormat = CompressionFormat.None;
                        }
                    }
                }
                return compressionFormat;
            }


        }

        class XmlBinaryWriterSessionWithQuota : XmlBinaryWriterSession
        {
            int bytesRemaining;
            List<XmlDictionaryString> newStrings;

            public XmlBinaryWriterSessionWithQuota(int maxSessionSize)
            {
                bytesRemaining = maxSessionSize;
            }

            public bool HasNewStrings
            {
                get { return newStrings != null; }
            }

            public override bool TryAdd(XmlDictionaryString s, out int key)
            {
                if (bytesRemaining == 0)
                {
                    key = -1;
                    return false;
                }

                int bytesRequired = Encoding.UTF8.GetByteCount(s.Value);
                bytesRequired += IntEncoder.GetEncodedSize(bytesRequired);

                if (bytesRequired > bytesRemaining)
                {
                    key = -1;
                    bytesRemaining = 0;
                    return false;
                }

                if (base.TryAdd(s, out key))
                {
                    if (newStrings == null)
                    {
                        newStrings = new List<XmlDictionaryString>();
                    }
                    newStrings.Add(s);
                    bytesRemaining -= bytesRequired;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public IList<XmlDictionaryString> GetNewStrings()
            {
                return newStrings;
            }

            public void ClearNewStrings()
            {
                newStrings = null;
            }
        }
    }

    class BinaryFormatBuilder
    {
        List<byte> bytes;

        public BinaryFormatBuilder()
        {
            this.bytes = new List<byte>();
        }

        public int Count
        {
            get { return bytes.Count; }
        }

        public void AppendPrefixDictionaryElement(char prefix, int key)
        {
            this.AppendNode(XmlBinaryNodeType.PrefixDictionaryElementA + GetPrefixOffset(prefix));
            this.AppendKey(key);
        }

        public void AppendDictionaryXmlnsAttribute(char prefix, int key)
        {
            this.AppendNode(XmlBinaryNodeType.DictionaryXmlnsAttribute);
            this.AppendUtf8(prefix);
            this.AppendKey(key);
        }

        public void AppendPrefixDictionaryAttribute(char prefix, int key, char value)
        {
            this.AppendNode(XmlBinaryNodeType.PrefixDictionaryAttributeA + GetPrefixOffset(prefix));
            this.AppendKey(key);
            if (value == '1')
            {
                this.AppendNode(XmlBinaryNodeType.OneText);
            }
            else
            {
                this.AppendNode(XmlBinaryNodeType.Chars8Text);
                this.AppendUtf8(value);
            }
        }

        public void AppendDictionaryAttribute(char prefix, int key, char value)
        {
            this.AppendNode(XmlBinaryNodeType.DictionaryAttribute);
            this.AppendUtf8(prefix);
            this.AppendKey(key);
            this.AppendNode(XmlBinaryNodeType.Chars8Text);
            this.AppendUtf8(value);
        }

        public void AppendDictionaryTextWithEndElement(int key)
        {
            this.AppendNode(XmlBinaryNodeType.DictionaryTextWithEndElement);
            this.AppendKey(key);
        }

        public void AppendDictionaryTextWithEndElement()
        {
            this.AppendNode(XmlBinaryNodeType.DictionaryTextWithEndElement);
        }

        public void AppendUniqueIDWithEndElement()
        {
            this.AppendNode(XmlBinaryNodeType.UniqueIdTextWithEndElement);
        }

        public void AppendEndElement()
        {
            this.AppendNode(XmlBinaryNodeType.EndElement);
        }

        void AppendKey(int key)
        {
            if (key < 0 || key >= 0x4000)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("key", key,
                    Res.GetString(Res.S("ValueMustBeInRange"), 0, 0x4000)));
            }
            if (key >= 0x80)
            {
                this.AppendByte((key & 0x7f) | 0x80);
                this.AppendByte(key >> 7);
            }
            else
            {
                this.AppendByte(key);
            }
        }

        void AppendNode(XmlBinaryNodeType value)
        {
            this.AppendByte((int)value);
        }

        void AppendByte(int value)
        {
            if (value < 0 || value > 0xFF)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                    Res.GetString(Res.S("ValueMustBeInRange"), 0, 0xFF)));
            }
            this.bytes.Add((byte)value);
        }

        void AppendUtf8(char value)
        {
            AppendByte(1);
            AppendByte((int)value);
        }

        public int GetStaticKey(int value)
        {
            return value * 2;
        }

        public int GetSessionKey(int value)
        {
            return value * 2 + 1;
        }

        int GetPrefixOffset(char prefix)
        {
            if (prefix < 'a' && prefix > 'z')
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("prefix", prefix,
                    Res.GetString(Res.S("ValueMustBeInRange"), 'a', 'z')));
            }
            return prefix - 'a';
        }

        public byte[] ToByteArray()
        {
            byte[] array = this.bytes.ToArray();
            this.bytes.Clear();
            return array;
        }
    }

    static class BinaryFormatParser
    {
        public static bool IsSessionKey(int value)
        {
            return (value & 1) != 0;
        }

        public static int GetSessionKey(int value)
        {
            return value / 2;
        }

        public static int GetStaticKey(int value)
        {
            return value / 2;
        }

        public static int ParseInt32(byte[] buffer, int offset, int size)
        {
            switch (size)
            {
                case 1:
                    return buffer[offset];
                case 2:
                    return (buffer[offset] & 0x7f) + (buffer[offset + 1] << 7);
                case 3:
                    return (buffer[offset] & 0x7f) + ((buffer[offset + 1] & 0x7f) << 7) + (buffer[offset + 2] << 14);
                case 4:
                    return (buffer[offset] & 0x7f) + ((buffer[offset + 1] & 0x7f) << 7) + ((buffer[offset + 2] & 0x7f) << 14) + (buffer[offset + 3] << 21);
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size,
                        Res.GetString(Res.S("ValueMustBeInRange"), 1, 4)));
            }
        }

        public static int ParseKey(byte[] buffer, int offset, int size)
        {
            return ParseInt32(buffer, offset, size);
        }

        public /*unsafe*/ static UniqueId ParseUniqueID(byte[] buffer, int offset, int size)
        {
            return new UniqueId(buffer, offset);
        }

        public static int MatchBytes(byte[] buffer, int offset, int size, byte[] buffer2)
        {
            if (size < buffer2.Length)
            {
                return 0;
            }
            int j = offset;
            for (int i = 0; i < buffer2.Length; i++, j++)
            {
                if (buffer2[i] != buffer[j])
                {
                    return 0;
                }
            }
            return buffer2.Length;
        }


        public static bool MatchAttributeNode(byte[] buffer, int offset, int size)
        {
            const XmlBinaryNodeType minAttribute = XmlBinaryNodeType.ShortAttribute;
            const XmlBinaryNodeType maxAttribute = XmlBinaryNodeType.DictionaryAttribute;
            if (size < 1)
            {
                return false;
            }
            XmlBinaryNodeType nodeType = (XmlBinaryNodeType)buffer[offset];
            return nodeType >= minAttribute && nodeType <= maxAttribute;
        }

        public static int MatchKey(byte[] buffer, int offset, int size)
        {
            return MatchInt32(buffer, offset, size);
        }

        public static int MatchInt32(byte[] buffer, int offset, int size)
        {
            if (size > 0)
            {
                if ((buffer[offset] & 0x80) == 0)
                {
                    return 1;
                }
            }
            if (size > 1)
            {
                if ((buffer[offset + 1] & 0x80) == 0)
                {
                    return 2;
                }
            }
            if (size > 2)
            {
                if ((buffer[offset + 2] & 0x80) == 0)
                {
                    return 3;
                }
            }
            if (size > 3)
            {
                if ((buffer[offset + 3] & 0x80) == 0)
                {
                    return 4;
                }
            }

            return 0;
        }

        public static int MatchUniqueID(byte[] buffer, int offset, int size)
        {
            if (size < 16)
            {
                return 0;
            }
            return 16;
        }
    }

    class MessagePatterns
    {
        static readonly byte[] commonFragment; // <Envelope><Headers><Action>
        static readonly byte[] requestFragment1; // </Action><MessageID>
        static readonly byte[] requestFragment2; // </MessageID><ReplyTo>...</ReplyTo><To>session-to-key</To></Headers><Body>
        static readonly byte[] responseFragment1; // </Action><RelatesTo>
        static readonly byte[] responseFragment2; // </RelatesTo><To>static-anonymous-key</To></Headers><Body>
        static readonly byte[] bodyFragment; // <Envelope><Body>
        const int ToValueSessionKey = 1;

        IXmlDictionary dictionary;
        XmlBinaryReaderSession readerSession;
        ToHeader toHeader;
        MessageVersion messageVersion;

        static MessagePatterns()
        {
            BinaryFormatBuilder builder = new BinaryFormatBuilder();

            MessageDictionary messageDictionary = XD.MessageDictionary;
            Message12Dictionary message12Dictionary = XD.Message12Dictionary;
            AddressingDictionary addressingDictionary = XD.AddressingDictionary;
            Addressing10Dictionary addressing10Dictionary = XD.Addressing10Dictionary;

            char messagePrefix = MessageStrings.Prefix[0];
            char addressingPrefix = AddressingStrings.Prefix[0];

            // <s:Envelope xmlns:s="soap-ns" xmlns="addressing-ns">
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Envelope.Key));
            builder.AppendDictionaryXmlnsAttribute(messagePrefix, builder.GetStaticKey(message12Dictionary.Namespace.Key));
            builder.AppendDictionaryXmlnsAttribute(addressingPrefix, builder.GetStaticKey(addressing10Dictionary.Namespace.Key));

            // <s:Header>
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Header.Key));

            // <a:Action>...
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.Action.Key));
            builder.AppendPrefixDictionaryAttribute(messagePrefix, builder.GetStaticKey(messageDictionary.MustUnderstand.Key), '1');
            builder.AppendDictionaryTextWithEndElement();
            commonFragment = builder.ToByteArray();

            // <a:MessageID>...
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.MessageId.Key));
            builder.AppendUniqueIDWithEndElement();
            requestFragment1 = builder.ToByteArray();

            // <a:ReplyTo><a:Address>static-anonymous-key</a:Address></a:ReplyTo>
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.ReplyTo.Key));
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.Address.Key));
            builder.AppendDictionaryTextWithEndElement(builder.GetStaticKey(addressing10Dictionary.Anonymous.Key));
            builder.AppendEndElement();

            // <a:To>session-to-key</a:To>
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.To.Key));
            builder.AppendPrefixDictionaryAttribute(messagePrefix, builder.GetStaticKey(messageDictionary.MustUnderstand.Key), '1');
            builder.AppendDictionaryTextWithEndElement(builder.GetSessionKey(ToValueSessionKey));

            // </s:Header>
            builder.AppendEndElement();

            // <s:Body>
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Body.Key));
            requestFragment2 = builder.ToByteArray();

            // <a:RelatesTo>...
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.RelatesTo.Key));
            builder.AppendUniqueIDWithEndElement();
            responseFragment1 = builder.ToByteArray();

            // <a:To>static-anonymous-key</a:To>
            builder.AppendPrefixDictionaryElement(addressingPrefix, builder.GetStaticKey(addressingDictionary.To.Key));
            builder.AppendPrefixDictionaryAttribute(messagePrefix, builder.GetStaticKey(messageDictionary.MustUnderstand.Key), '1');
            builder.AppendDictionaryTextWithEndElement(builder.GetStaticKey(addressing10Dictionary.Anonymous.Key));

            // </s:Header>
            builder.AppendEndElement();

            // <s:Body>
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Body.Key));
            responseFragment2 = builder.ToByteArray();

            // <s:Envelope xmlns:s="soap-ns" xmlns="addressing-ns">
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Envelope.Key));
            builder.AppendDictionaryXmlnsAttribute(messagePrefix, builder.GetStaticKey(message12Dictionary.Namespace.Key));
            builder.AppendDictionaryXmlnsAttribute(addressingPrefix, builder.GetStaticKey(addressing10Dictionary.Namespace.Key));

            // <s:Body>
            builder.AppendPrefixDictionaryElement(messagePrefix, builder.GetStaticKey(messageDictionary.Body.Key));
            bodyFragment = builder.ToByteArray();
        }

        public MessagePatterns(IXmlDictionary dictionary, XmlBinaryReaderSession readerSession, MessageVersion messageVersion)
        {
            this.dictionary = dictionary;
            this.readerSession = readerSession;
            this.messageVersion = messageVersion;
        }

        public Message TryCreateMessage(byte[] buffer, int offset, int size, BufferManager bufferManager, BufferedMessageData messageData)
        {
            RelatesToHeader relatesToHeader;
            MessageIDHeader messageIDHeader;
            XmlDictionaryString toString;

            int currentOffset = offset;
            int remainingSize = size;

            int bytesMatched = BinaryFormatParser.MatchBytes(buffer, currentOffset, remainingSize, commonFragment);
            if (bytesMatched == 0)
            {
                return null;
            }
            currentOffset += bytesMatched;
            remainingSize -= bytesMatched;

            bytesMatched = BinaryFormatParser.MatchKey(buffer, currentOffset, remainingSize);
            if (bytesMatched == 0)
            {
                return null;
            }
            int actionOffset = currentOffset;
            int actionSize = bytesMatched;
            currentOffset += bytesMatched;
            remainingSize -= bytesMatched;

            int totalBytesMatched;

            bytesMatched = BinaryFormatParser.MatchBytes(buffer, currentOffset, remainingSize, requestFragment1);
            if (bytesMatched != 0)
            {
                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                bytesMatched = BinaryFormatParser.MatchUniqueID(buffer, currentOffset, remainingSize);
                if (bytesMatched == 0)
                {
                    return null;
                }
                int messageIDOffset = currentOffset;
                int messageIDSize = bytesMatched;
                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                bytesMatched = BinaryFormatParser.MatchBytes(buffer, currentOffset, remainingSize, requestFragment2);
                if (bytesMatched == 0)
                {
                    return null;
                }
                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                if (BinaryFormatParser.MatchAttributeNode(buffer, currentOffset, remainingSize))
                {
                    return null;
                }

                UniqueId messageId = BinaryFormatParser.ParseUniqueID(buffer, messageIDOffset, messageIDSize);
                messageIDHeader = MessageIDHeader.Create(messageId, messageVersion.Addressing);
                relatesToHeader = null;

                if (!readerSession.TryLookup(ToValueSessionKey, out toString))
                {
                    return null;
                }

                totalBytesMatched = requestFragment1.Length + messageIDSize + requestFragment2.Length;
            }
            else
            {
                bytesMatched = BinaryFormatParser.MatchBytes(buffer, currentOffset, remainingSize, responseFragment1);

                if (bytesMatched == 0)
                {
                    return null;
                }

                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                bytesMatched = BinaryFormatParser.MatchUniqueID(buffer, currentOffset, remainingSize);
                if (bytesMatched == 0)
                {
                    return null;
                }
                int messageIDOffset = currentOffset;
                int messageIDSize = bytesMatched;
                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                bytesMatched = BinaryFormatParser.MatchBytes(buffer, currentOffset, remainingSize, responseFragment2);
                if (bytesMatched == 0)
                {
                    return null;
                }
                currentOffset += bytesMatched;
                remainingSize -= bytesMatched;

                if (BinaryFormatParser.MatchAttributeNode(buffer, currentOffset, remainingSize))
                {
                    return null;
                }

                UniqueId messageId = BinaryFormatParser.ParseUniqueID(buffer, messageIDOffset, messageIDSize);
                relatesToHeader = RelatesToHeader.Create(messageId, messageVersion.Addressing);
                messageIDHeader = null;
                toString = XD.Addressing10Dictionary.Anonymous;

                totalBytesMatched = responseFragment1.Length + messageIDSize + responseFragment2.Length;
            }

            totalBytesMatched += commonFragment.Length + actionSize;

            int actionKey = BinaryFormatParser.ParseKey(buffer, actionOffset, actionSize);

            XmlDictionaryString actionString;
            if (!TryLookupKey(actionKey, out actionString))
            {
                return null;
            }

            ActionHeader actionHeader = ActionHeader.Create(actionString, messageVersion.Addressing);

            if (toHeader == null)
            {
                toHeader = ToHeader.Create(new Uri(toString.Value), messageVersion.Addressing);
            }

            int abandonedSize = totalBytesMatched - bodyFragment.Length;

            offset += abandonedSize;
            size -= abandonedSize;

            Buffer.BlockCopy(bodyFragment, 0, buffer, offset, bodyFragment.Length);

            messageData.Open(new ArraySegment<byte>(buffer, offset, size), bufferManager);

            PatternMessage patternMessage = new PatternMessage(messageData, this.messageVersion);

            MessageHeaders headers = patternMessage.Headers;
            headers.AddActionHeader(actionHeader);
            if (messageIDHeader != null)
            {
                headers.AddMessageIDHeader(messageIDHeader);
                headers.AddReplyToHeader(ReplyToHeader.AnonymousReplyTo10);
            }
            else
            {
                headers.AddRelatesToHeader(relatesToHeader);
            }
            headers.AddToHeader(toHeader);

            return patternMessage;
        }

        bool TryLookupKey(int key, out XmlDictionaryString result)
        {
            if (BinaryFormatParser.IsSessionKey(key))
            {
                return readerSession.TryLookup(BinaryFormatParser.GetSessionKey(key), out result);
            }
            else
            {
                return dictionary.TryLookup(BinaryFormatParser.GetStaticKey(key), out result);
            }
        }

        sealed class PatternMessage : ReceivedMessage
        {
            IBufferedMessageData messageData;
            MessageHeaders headers;
            RecycledMessageState recycledMessageState;
            MessageProperties properties;
            XmlDictionaryReader reader;

            public PatternMessage(IBufferedMessageData messageData, MessageVersion messageVersion)
            {
                this.messageData = messageData;
                recycledMessageState = messageData.TakeMessageState();
                if (recycledMessageState == null)
                {
                    recycledMessageState = new RecycledMessageState();
                }
                properties = recycledMessageState.TakeProperties();
                if (properties == null)
                {
                    this.properties = new MessageProperties();
                }
                headers = recycledMessageState.TakeHeaders();
                if (headers == null)
                {
                    headers = new MessageHeaders(messageVersion);
                }
                else
                {
                    headers.Init(messageVersion);
                }
                XmlDictionaryReader reader = messageData.GetMessageReader();
                reader.ReadStartElement();
                VerifyStartBody(reader, messageVersion.Envelope);
                ReadStartBody(reader);
                this.reader = reader;
            }

            public PatternMessage(IBufferedMessageData messageData, MessageVersion messageVersion,
                KeyValuePair<string, object>[] properties, MessageHeaders headers)
            {
                this.messageData = messageData;
                this.messageData.Open();
                this.recycledMessageState = this.messageData.TakeMessageState();
                if (this.recycledMessageState == null)
                {
                    this.recycledMessageState = new RecycledMessageState();
                }

                this.properties = recycledMessageState.TakeProperties();
                if (this.properties == null)
                {
                    this.properties = new MessageProperties();
                }
                if (properties != null)
                {
                    this.properties.CopyProperties(properties);
                }

                this.headers = recycledMessageState.TakeHeaders();
                if (this.headers == null)
                {
                    this.headers = new MessageHeaders(messageVersion);
                }
                if (headers != null)
                {
                    this.headers.CopyHeadersFrom(headers);
                }

                XmlDictionaryReader reader = messageData.GetMessageReader();
                reader.ReadStartElement();
                VerifyStartBody(reader, messageVersion.Envelope);
                ReadStartBody(reader);
                this.reader = reader;
            }


            public override MessageHeaders Headers
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateMessageDisposedException());
                    }
                    return headers;
                }
            }

            public override MessageProperties Properties
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateMessageDisposedException());
                    }
                    return properties;
                }
            }

            public override MessageVersion Version
            {
                get
                {
                    if (IsDisposed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateMessageDisposedException());
                    }
                    return headers.MessageVersion;
                }
            }

            internal override RecycledMessageState RecycledMessageState
            {
                get { return recycledMessageState; }
            }

            XmlDictionaryReader GetBufferedReaderAtBody()
            {
                XmlDictionaryReader reader = messageData.GetMessageReader();
                reader.ReadStartElement();
                reader.ReadStartElement();
                return reader;
            }

            protected override void OnBodyToString(XmlDictionaryWriter writer)
            {
                using (XmlDictionaryReader reader = GetBufferedReaderAtBody())
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        writer.WriteNode(reader, false);
                    }
                }
            }

            protected override void OnClose()
            {
                Exception ex = null;
                try
                {
                    base.OnClose();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    ex = e;
                }

                try
                {
                    properties.Dispose();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ex == null)
                    {
                        ex = e;
                    }
                }

                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ex == null)
                    {
                        ex = e;
                    }
                }

                try
                {
                    recycledMessageState.ReturnHeaders(headers);
                    recycledMessageState.ReturnProperties(properties);
                    messageData.ReturnMessageState(recycledMessageState);
                    recycledMessageState = null;
                    messageData.Close();
                    messageData = null;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ex == null)
                    {
                        ex = e;
                    }
                }

                if (ex != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ex);
                }
            }

            protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
            {
                KeyValuePair<string, object>[] properties = new KeyValuePair<string, object>[Properties.Count];
                ((ICollection<KeyValuePair<string, object>>)Properties).CopyTo(properties, 0);
                messageData.EnableMultipleUsers();
                return new PatternMessageBuffer(this.messageData, this.Version, properties, this.headers);
            }

            protected override XmlDictionaryReader OnGetReaderAtBodyContents()
            {
                XmlDictionaryReader reader = this.reader;
                this.reader = null;
                return reader;
            }

            protected override string OnGetBodyAttribute(string localName, string ns)
            {
                return null;
            }
        }

        class PatternMessageBuffer : MessageBuffer
        {
            bool closed;
            MessageHeaders headers;
            IBufferedMessageData messageDataAtBody;
            MessageVersion messageVersion;
            KeyValuePair<string, object>[] properties;
            object thisLock = new object();
            RecycledMessageState recycledMessageState;

            public PatternMessageBuffer(IBufferedMessageData messageDataAtBody, MessageVersion messageVersion,
                KeyValuePair<string, object>[] properties, MessageHeaders headers)
            {
                this.messageDataAtBody = messageDataAtBody;
                this.messageDataAtBody.Open();

                this.recycledMessageState = this.messageDataAtBody.TakeMessageState();
                if (this.recycledMessageState == null)
                {
                    this.recycledMessageState = new RecycledMessageState();
                }

                this.headers = this.recycledMessageState.TakeHeaders();
                if (this.headers == null)
                {
                    this.headers = new MessageHeaders(messageVersion);
                }
                this.headers.CopyHeadersFrom(headers);
                this.properties = properties;
                this.messageVersion = messageVersion;
            }

            public override int BufferSize
            {
                get
                {
                    lock (this.ThisLock)
                    {
                        if (this.closed)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                        }

                        return messageDataAtBody.Buffer.Count;
                    }
                }
            }

            object ThisLock
            {
                get
                {
                    return this.thisLock;
                }
            }

            public override void Close()
            {
                lock (this.thisLock)
                {
                    if (!this.closed)
                    {
                        this.closed = true;
                        this.recycledMessageState.ReturnHeaders(this.headers);
                        this.messageDataAtBody.ReturnMessageState(this.recycledMessageState);
                        this.messageDataAtBody.Close();
                        this.recycledMessageState = null;
                        this.messageDataAtBody = null;
                        this.properties = null;
                        this.messageVersion = null;
                        this.headers = null;
                    }
                }
            }

            public override Message CreateMessage()
            {
                lock (this.ThisLock)
                {
                    if (this.closed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                    }

                    return new PatternMessage(this.messageDataAtBody, this.messageVersion, this.properties,
                        this.headers);
                }
            }
        }
    }

	enum XmlBinaryNodeType
	{
		// ProcessingInstruction = 0, // Reserved (Not supported)
		EndElement = 1,
		Comment = 2,
		Array = 3,

		MinAttribute = Array + 1,
		ShortAttribute = MinAttribute + 0,
		Attribute = MinAttribute + 1,
		ShortDictionaryAttribute = MinAttribute + 2,
		DictionaryAttribute = MinAttribute + 3,
		ShortXmlnsAttribute = MinAttribute + 4,
		XmlnsAttribute = MinAttribute + 5,
		ShortDictionaryXmlnsAttribute = MinAttribute + 6,
		DictionaryXmlnsAttribute = MinAttribute + 7,
		PrefixDictionaryAttributeA = MinAttribute + 8,
		PrefixDictionaryAttributeB = PrefixDictionaryAttributeA + 1,
		PrefixDictionaryAttributeC = PrefixDictionaryAttributeB + 1,
		PrefixDictionaryAttributeD = PrefixDictionaryAttributeC + 1,
		PrefixDictionaryAttributeE = PrefixDictionaryAttributeD + 1,
		PrefixDictionaryAttributeF = PrefixDictionaryAttributeE + 1,
		PrefixDictionaryAttributeG = PrefixDictionaryAttributeF + 1,
		PrefixDictionaryAttributeH = PrefixDictionaryAttributeG + 1,
		PrefixDictionaryAttributeI = PrefixDictionaryAttributeH + 1,
		PrefixDictionaryAttributeJ = PrefixDictionaryAttributeI + 1,
		PrefixDictionaryAttributeK = PrefixDictionaryAttributeJ + 1,
		PrefixDictionaryAttributeL = PrefixDictionaryAttributeK + 1,
		PrefixDictionaryAttributeM = PrefixDictionaryAttributeL + 1,
		PrefixDictionaryAttributeN = PrefixDictionaryAttributeM + 1,
		PrefixDictionaryAttributeO = PrefixDictionaryAttributeN + 1,
		PrefixDictionaryAttributeP = PrefixDictionaryAttributeO + 1,
		PrefixDictionaryAttributeQ = PrefixDictionaryAttributeP + 1,
		PrefixDictionaryAttributeR = PrefixDictionaryAttributeQ + 1,
		PrefixDictionaryAttributeS = PrefixDictionaryAttributeR + 1,
		PrefixDictionaryAttributeT = PrefixDictionaryAttributeS + 1,
		PrefixDictionaryAttributeU = PrefixDictionaryAttributeT + 1,
		PrefixDictionaryAttributeV = PrefixDictionaryAttributeU + 1,
		PrefixDictionaryAttributeW = PrefixDictionaryAttributeV + 1,
		PrefixDictionaryAttributeX = PrefixDictionaryAttributeW + 1,
		PrefixDictionaryAttributeY = PrefixDictionaryAttributeX + 1,
		PrefixDictionaryAttributeZ = PrefixDictionaryAttributeY + 1,
		PrefixAttributeA = PrefixDictionaryAttributeZ + 1,
		PrefixAttributeB = PrefixAttributeA + 1,
		PrefixAttributeC = PrefixAttributeB + 1,
		PrefixAttributeD = PrefixAttributeC + 1,
		PrefixAttributeE = PrefixAttributeD + 1,
		PrefixAttributeF = PrefixAttributeE + 1,
		PrefixAttributeG = PrefixAttributeF + 1,
		PrefixAttributeH = PrefixAttributeG + 1,
		PrefixAttributeI = PrefixAttributeH + 1,
		PrefixAttributeJ = PrefixAttributeI + 1,
		PrefixAttributeK = PrefixAttributeJ + 1,
		PrefixAttributeL = PrefixAttributeK + 1,
		PrefixAttributeM = PrefixAttributeL + 1,
		PrefixAttributeN = PrefixAttributeM + 1,
		PrefixAttributeO = PrefixAttributeN + 1,
		PrefixAttributeP = PrefixAttributeO + 1,
		PrefixAttributeQ = PrefixAttributeP + 1,
		PrefixAttributeR = PrefixAttributeQ + 1,
		PrefixAttributeS = PrefixAttributeR + 1,
		PrefixAttributeT = PrefixAttributeS + 1,
		PrefixAttributeU = PrefixAttributeT + 1,
		PrefixAttributeV = PrefixAttributeU + 1,
		PrefixAttributeW = PrefixAttributeV + 1,
		PrefixAttributeX = PrefixAttributeW + 1,
		PrefixAttributeY = PrefixAttributeX + 1,
		PrefixAttributeZ = PrefixAttributeY + 1,
		MaxAttribute = PrefixAttributeZ,

		MinElement = MaxAttribute + 1,
		ShortElement = MinElement,
		Element = MinElement + 1,
		ShortDictionaryElement = MinElement + 2,
		DictionaryElement = MinElement + 3,
		PrefixDictionaryElementA = MinElement + 4,
		PrefixDictionaryElementB = PrefixDictionaryElementA + 1,
		PrefixDictionaryElementC = PrefixDictionaryElementB + 1,
		PrefixDictionaryElementD = PrefixDictionaryElementC + 1,
		PrefixDictionaryElementE = PrefixDictionaryElementD + 1,
		PrefixDictionaryElementF = PrefixDictionaryElementE + 1,
		PrefixDictionaryElementG = PrefixDictionaryElementF + 1,
		PrefixDictionaryElementH = PrefixDictionaryElementG + 1,
		PrefixDictionaryElementI = PrefixDictionaryElementH + 1,
		PrefixDictionaryElementJ = PrefixDictionaryElementI + 1,
		PrefixDictionaryElementK = PrefixDictionaryElementJ + 1,
		PrefixDictionaryElementL = PrefixDictionaryElementK + 1,
		PrefixDictionaryElementM = PrefixDictionaryElementL + 1,
		PrefixDictionaryElementN = PrefixDictionaryElementM + 1,
		PrefixDictionaryElementO = PrefixDictionaryElementN + 1,
		PrefixDictionaryElementP = PrefixDictionaryElementO + 1,
		PrefixDictionaryElementQ = PrefixDictionaryElementP + 1,
		PrefixDictionaryElementR = PrefixDictionaryElementQ + 1,
		PrefixDictionaryElementS = PrefixDictionaryElementR + 1,
		PrefixDictionaryElementT = PrefixDictionaryElementS + 1,
		PrefixDictionaryElementU = PrefixDictionaryElementT + 1,
		PrefixDictionaryElementV = PrefixDictionaryElementU + 1,
		PrefixDictionaryElementW = PrefixDictionaryElementV + 1,
		PrefixDictionaryElementX = PrefixDictionaryElementW + 1,
		PrefixDictionaryElementY = PrefixDictionaryElementX + 1,
		PrefixDictionaryElementZ = PrefixDictionaryElementY + 1,
		PrefixElementA = PrefixDictionaryElementZ + 1,
		PrefixElementB = PrefixElementA + 1,
		PrefixElementC = PrefixElementB + 1,
		PrefixElementD = PrefixElementC + 1,
		PrefixElementE = PrefixElementD + 1,
		PrefixElementF = PrefixElementE + 1,
		PrefixElementG = PrefixElementF + 1,
		PrefixElementH = PrefixElementG + 1,
		PrefixElementI = PrefixElementH + 1,
		PrefixElementJ = PrefixElementI + 1,
		PrefixElementK = PrefixElementJ + 1,
		PrefixElementL = PrefixElementK + 1,
		PrefixElementM = PrefixElementL + 1,
		PrefixElementN = PrefixElementM + 1,
		PrefixElementO = PrefixElementN + 1,
		PrefixElementP = PrefixElementO + 1,
		PrefixElementQ = PrefixElementP + 1,
		PrefixElementR = PrefixElementQ + 1,
		PrefixElementS = PrefixElementR + 1,
		PrefixElementT = PrefixElementS + 1,
		PrefixElementU = PrefixElementT + 1,
		PrefixElementV = PrefixElementU + 1,
		PrefixElementW = PrefixElementV + 1,
		PrefixElementX = PrefixElementW + 1,
		PrefixElementY = PrefixElementX + 1,
		PrefixElementZ = PrefixElementY + 1,
		MaxElement = PrefixElementZ,

		// MinorVersion = MaxElement + 1, // Reserved (Not supported)

		MinText = 0x80, // Must be even 
		ZeroText = MinText,
		OneText = MinText + 1 * 2,
		FalseText = MinText + 2 * 2,
		TrueText = MinText + 3 * 2,
		Int8Text = MinText + 4 * 2,
		Int16Text = MinText + 5 * 2,
		Int32Text = MinText + 6 * 2,
		Int64Text = MinText + 7 * 2,
		FloatText = MinText + 8 * 2,
		DoubleText = MinText + 9 * 2,
		DecimalText = MinText + 10 * 2,
		DateTimeText = MinText + 11 * 2,
		Chars8Text = MinText + 12 * 2,
		Chars16Text = MinText + 13 * 2,
		Chars32Text = MinText + 14 * 2,
		Bytes8Text = MinText + 15 * 2,
		Bytes16Text = MinText + 16 * 2,
		Bytes32Text = MinText + 17 * 2,
		StartListText = MinText + 18 * 2,
		EndListText = MinText + 19 * 2,
		EmptyText = MinText + 20 * 2,
		DictionaryText = MinText + 21 * 2,
		UniqueIdText = MinText + 22 * 2,
		TimeSpanText = MinText + 23 * 2,
		GuidText = MinText + 24 * 2,
		UInt64Text = MinText + 25 * 2,
		BoolText = MinText + 26 * 2,
		UnicodeChars8Text = MinText + 27 * 2,
		UnicodeChars16Text = MinText + 28 * 2,
		UnicodeChars32Text = MinText + 29 * 2,
		QNameDictionaryText = MinText + 30 * 2,

		ZeroTextWithEndElement = ZeroText + 1,
		OneTextWithEndElement = OneText + 1,
		FalseTextWithEndElement = FalseText + 1,
		TrueTextWithEndElement = TrueText + 1,
		Int8TextWithEndElement = Int8Text + 1,
		Int16TextWithEndElement = Int16Text + 1,
		Int32TextWithEndElement = Int32Text + 1,
		Int64TextWithEndElement = Int64Text + 1,
		FloatTextWithEndElement = FloatText + 1,
		DoubleTextWithEndElement = DoubleText + 1,
		DecimalTextWithEndElement = DecimalText + 1,
		DateTimeTextWithEndElement = DateTimeText + 1,
		Chars8TextWithEndElement = Chars8Text + 1,
		Chars16TextWithEndElement = Chars16Text + 1,
		Chars32TextWithEndElement = Chars32Text + 1,
		Bytes8TextWithEndElement = Bytes8Text + 1,
		Bytes16TextWithEndElement = Bytes16Text + 1,
		Bytes32TextWithEndElement = Bytes32Text + 1,
		StartListTextWithEndElement = StartListText + 1,
		EndListTextWithEndElement = EndListText + 1,
		EmptyTextWithEndElement = EmptyText + 1,
		DictionaryTextWithEndElement = DictionaryText + 1,
		UniqueIdTextWithEndElement = UniqueIdText + 1,
		TimeSpanTextWithEndElement = TimeSpanText + 1,
		GuidTextWithEndElement = GuidText + 1,
		UInt64TextWithEndElement = UInt64Text + 1,
		BoolTextWithEndElement = BoolText + 1,
		UnicodeChars8TextWithEndElement = UnicodeChars8Text + 1,
		UnicodeChars16TextWithEndElement = UnicodeChars16Text + 1,
		UnicodeChars32TextWithEndElement = UnicodeChars32Text + 1,
		QNameDictionaryTextWithEndElement = QNameDictionaryText + 1,
		MaxText = QNameDictionaryTextWithEndElement
	}
}
