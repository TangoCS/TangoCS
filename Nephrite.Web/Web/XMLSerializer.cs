using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Nephrite.Web
{
	public class XMLSerializer
	{
		public static MemoryStream SerializeSOAP(object request)
		{
			SoapFormatter serializer = new SoapFormatter();
			MemoryStream memStream = new MemoryStream();
			serializer.Serialize(memStream, request);
			return memStream;
		}

		public static object DeserializeSOAP(MemoryStream memStream)
		{
			object sr;
			SoapFormatter deserializer = new SoapFormatter();
			memStream.Position = 0;
			sr = deserializer.Deserialize(memStream);
			return sr;
		}

		public static XElement Serialize<T>(T request)
			where T: class
		{
			XmlSerializer xs = new XmlSerializer(request.GetType());
			XDocument d = new XDocument();
			
			using (XmlWriter w = d.CreateWriter()) xs.Serialize(w, request);
			
			XElement e = d.Root;
			e.Remove();

			return e;
		}

		public static T Deserialize<T>(XElement xe)
			where T: class, new()
		{
			T o = new T();
			XmlSerializer xs = new XmlSerializer(o.GetType());

			return xs.Deserialize(xe.CreateReader()) as T;
		}

	}
}
