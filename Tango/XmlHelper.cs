using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Tango
{
	public class XmlHelper
	{
		//public static MemoryStream SerializeSOAP(object request)
		//{
		//	SoapFormatter serializer = new SoapFormatter();
		//	MemoryStream memStream = new MemoryStream();
		//	serializer.Serialize(memStream, request);
		//	return memStream;
		//}

		//public static object DeserializeSOAP(MemoryStream memStream)
		//{
		//	object sr;
		//	SoapFormatter deserializer = new SoapFormatter();
		//	memStream.Position = 0;
		//	sr = deserializer.Deserialize(memStream);
		//	return sr;
		//}

		public static XElement Serialize<T>(T request)
			where T : class
		{
			XmlSerializer xs = new XmlSerializer(request.GetType());
			XDocument d = new XDocument();

            using (XmlWriter w = d.CreateWriter())                
                xs.Serialize(w, request);

			XElement e = d.Root;
			e.Remove();            

			return e;
		}

		public static MemoryStream SerializeWin1251<T>(T obj)
		{
			return Serialize(obj, new XmlWriterSettings {
				Encoding = Encoding.GetEncoding("windows-1251"),
				Indent = true,
				NewLineHandling = NewLineHandling.None
			});
		}

		public static MemoryStream Serialize<T>(T obj, XmlWriterSettings settings)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (XmlWriter writer = XmlWriter.Create(stream, settings))
				{
					var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
					serializer.Serialize(writer, obj);
				}
				return stream;
			}
		}

		public static T Deserialize<T>(string xml)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			XDocument doc = XDocument.Parse(xml);

			using (var reader = doc.Root.CreateReader())
				return (T)xmlSerializer.Deserialize(reader);
		}

		public static T Deserialize<T>(XElement xe)
			where T : class, new()
		{
			T o = new T();
			XmlSerializer xs = new XmlSerializer(o.GetType());
			return xs.Deserialize(xe.CreateReader()) as T;
		}

        public static XmlWriterSettings GetSettings()
        {
           return new XmlWriterSettings
            {
                Encoding = Encoding.GetEncoding("windows-1251"),
                Indent = true,
                NewLineHandling = NewLineHandling.None
            };
		}

		/// <summary>
		/// Преобразовывает xml в соответствии с xsl-шаблоном
		/// </summary>
		/// <param name="xml">Преобразуемый xml</param>
		/// <param name="assembly">Сборка с ресурсами (xslt)</param>
		/// <param name="xslResource">Имя ресурса xslt</param>
		/// <param name="output">Куда выводится результат</param>
		public static void Transform(string xml, string xslPath, TextWriter output)
		{
			AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
			XslCompiledTransform xct = new XslCompiledTransform();
			XmlReader styleSheet = XmlReader.Create(xslPath);
			XsltSettings settings = new XsltSettings(true, true);
			xct.Load(styleSheet, settings, new XmlUrlResolver());
			XmlReader xr = XmlReader.Create(new StringReader(xml));
			xct.Transform(xr, null, output);
		}
	}
}
