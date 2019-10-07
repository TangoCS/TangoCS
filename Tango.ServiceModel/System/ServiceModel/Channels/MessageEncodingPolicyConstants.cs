using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    static class MessageEncodingPolicyConstants
    {
        public const string BinaryEncodingName = "BinaryEncoding";
        public const string BinaryEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netbinary1";
        public const string BinaryEncodingPrefix = "msb";
        public const string OptimizedMimeSerializationNamespace = "http://schemas.xmlsoap.org/ws/2004/09/policy/optimizedmimeserialization";
        public const string OptimizedMimeSerializationPrefix = "wsoma";
        public const string MtomEncodingName = "OptimizedMimeSerialization";
    }
}
