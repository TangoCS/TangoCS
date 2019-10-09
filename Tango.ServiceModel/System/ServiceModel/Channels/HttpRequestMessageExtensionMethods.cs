// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;

    public static class HttpRequestMessageExtensionMethods
    {
        const string MessageHeadersPropertyKey = "System.ServiceModel.Channels.MessageHeaders";
        const string PrincipalKey = "MS_UserPrincipal";

        public static void SetUserPrincipal(this HttpRequestMessage httpRequestMessage, IPrincipal user)
        {
            if (httpRequestMessage == null)
            {
                throw FxTrace.Exception.AsError(new ArgumentNullException("httpRequestMessage"));
            }

            httpRequestMessage.Properties[PrincipalKey] = user;
        }

        public static IPrincipal GetUserPrincipal(this HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage == null)
            {
                throw FxTrace.Exception.AsError(new ArgumentNullException("httpRequestMessage"));
            }

            object user;
            if (httpRequestMessage.Properties.TryGetValue(PrincipalKey, out user))
            {
                return user as IPrincipal;
            }

            return null;
        }
        
        internal static void AddHeader(this HttpRequestMessage httpRequestMessage, string header, string value)
        {
            HttpHeaderInfo headerInfo = HttpHeaderInfo.Create(header);
            EnsureNotResponseHeader(headerInfo);
            AddHeader(httpRequestMessage, headerInfo, value);
        }

        internal static void SetHeader(this HttpRequestMessage httpRequestMessage, string header, string value)
        {
            HttpHeaderInfo headerInfo = HttpHeaderInfo.Create(header);
            EnsureNotResponseHeader(headerInfo);
            RemoveHeader(httpRequestMessage, headerInfo);
            AddHeader(httpRequestMessage, headerInfo, value);
        }

        internal static IEnumerable<string> GetHeader(this HttpRequestMessage httpRequestMessage, string header)
        {
            HttpHeaderInfo headerInfo = HttpHeaderInfo.Create(header);
            EnsureNotResponseHeader(headerInfo);
            return GetHeader(httpRequestMessage, headerInfo);
        }

        internal static void RemoveHeader(this HttpRequestMessage httpRequestMessage, string header)
        {
            HttpHeaderInfo headerInfo = HttpHeaderInfo.Create(header);
            EnsureNotResponseHeader(headerInfo);
            RemoveHeader(httpRequestMessage, headerInfo);
        }

        internal static HttpRequestMessage CreateBufferedCopy(this HttpRequestMessage httpRequestMessage)
        {
            HttpRequestMessage bufferedHttpRequestMessage = new HttpRequestMessage();
            bufferedHttpRequestMessage.RequestUri = httpRequestMessage.RequestUri != null ? new Uri(httpRequestMessage.RequestUri, string.Empty) : null;
            bufferedHttpRequestMessage.Method = httpRequestMessage.Method != null ? new HttpMethod(httpRequestMessage.Method.Method) : null;
            bufferedHttpRequestMessage.Version = (Version)(httpRequestMessage.Version != null ? httpRequestMessage.Version.Clone() : null);

            foreach (KeyValuePair<string, IEnumerable<string>> header in httpRequestMessage.Headers)
            {
                bufferedHttpRequestMessage.Headers.AddHeaderWithoutValidation(header);
            }

            foreach (KeyValuePair<string, object> header in httpRequestMessage.Properties)
            {
                IMessageProperty messageProperty = header.Value as IMessageProperty;
                object value = messageProperty != null ?
                    messageProperty.CreateCopy() :
                    header.Value;

                bufferedHttpRequestMessage.Properties.Add(header.Key, value); 
            }

            bufferedHttpRequestMessage.Content = CreateBufferedCopyOfContent(httpRequestMessage.Content);

            return bufferedHttpRequestMessage;
        }

        internal static HttpContent CreateBufferedCopyOfContent(HttpContent content)
        {
            if (content != null)
            {
                SharedByteArrayContent shareableContent = content as SharedByteArrayContent;
                byte[] contentBytes = shareableContent == null ?
                    content.ReadAsByteArrayAsync().Result :
                    shareableContent.ContentBytes;

                HttpContent bufferedContent = new SharedByteArrayContent(contentBytes);

                foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
                {
                    bufferedContent.Headers.AddHeaderWithoutValidation(header);
                }

                return bufferedContent;
            }

            return null;
        }

        internal static void CopyPropertiesFromMessage(this HttpRequestMessage httpRequestMessage, Message message)
        {           
            IDictionary<string, object> properties = httpRequestMessage.Properties;
            CopyProperties(message.Properties, properties);
            properties[MessageHeadersPropertyKey] = message.Headers;
        }

        internal static void AddHeaderWithoutValidation(this HttpHeaders httpHeaders, KeyValuePair<string, IEnumerable<string>> header)
        {
            if (!httpHeaders.TryAddWithoutValidation(header.Key, header.Value))
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(Res.GetString(
                                Res.S("CopyHttpHeaderFailed"),
                                header.Key,
                                header.Value,
                                httpHeaders.GetType().Name)));
            }
        }

        private static void CopyProperties(MessageProperties messageProperties, IDictionary<string, object> properties)
        {
            foreach (KeyValuePair<string, object> property in messageProperties)
            {
                object value = property.Value;
                string key = property.Key;

                if ((value is HttpRequestMessageProperty && string.Equals(key, HttpRequestMessageProperty.Name, StringComparison.OrdinalIgnoreCase)) ||
                    (value is HttpResponseMessageProperty && string.Equals(key, HttpResponseMessageProperty.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                properties[key] = value;
            }
        }

        private static void EnsureNotResponseHeader(HttpHeaderInfo headerInfo)
        {
            if (!headerInfo.IsRequestHeader && !headerInfo.IsContentHeader && headerInfo.IsResponseHeader)
            {
                throw FxTrace.Exception.AsError(
                    new InvalidOperationException(
                        Res.GetString(Res.S("ResponseHeaderWithRequestHeadersCollection"), headerInfo.Name)));
            }
        }

        private static IEnumerable<string> GetHeader(HttpRequestMessage httpRequestMessage, HttpHeaderInfo headerInfo)
        {
            IEnumerable<string> values = null;

            if (headerInfo.IsRequestHeader)
            {
                values = headerInfo.TryGetHeader(httpRequestMessage.Headers);
            }

            if (values == null && 
                headerInfo.IsContentHeader && 
                httpRequestMessage.Content != null)
            {
                values = headerInfo.TryGetHeader(httpRequestMessage.Content.Headers);
            }

            return values;
        }

        private static void RemoveHeader(HttpRequestMessage httpRequestMessage, HttpHeaderInfo headerInfo)
        {
            if (headerInfo.IsRequestHeader)
            {
                headerInfo.TryRemoveHeader(httpRequestMessage.Headers);
            }

            if (headerInfo.IsContentHeader && httpRequestMessage.Content != null)
            {
                headerInfo.TryRemoveHeader(httpRequestMessage.Content.Headers);
            }
        }

        private static void AddHeader(HttpRequestMessage httpRequestMessage, HttpHeaderInfo headerInfo, string value)
        {
            if (headerInfo.IsRequestHeader)
            {
                if (headerInfo.TryAddHeader(httpRequestMessage.Headers, value))
                {
                    return;
                }
            }
            
            if (headerInfo.IsContentHeader)
            {
                CreateContentIfNull(httpRequestMessage);
                headerInfo.TryAddHeader(httpRequestMessage.Content.Headers, value);
            }  
        }

        private static bool CreateContentIfNull(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage.Content == null)
            {
                httpRequestMessage.Content = new ByteArrayContent(EmptyArray<byte>.Instance);
                return true;
            }

            return false;
        }

        class SharedByteArrayContent : ByteArrayContent
        {
            public SharedByteArrayContent(byte[] content)
                : base(content)
            {
                this.ContentBytes = content;
            }

            public byte[] ContentBytes { get; private set; }
        }
    }
}
