using System.IO;
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
            XmlWriterSettings settings = new XmlWriterSettings
            {
                
                Encoding = Encoding.GetEncoding("windows-1251"),
                Indent = true,
                NewLineHandling = NewLineHandling.None
            };
            return settings;
		}

		/// <summary>
		/// Преобразовывает xml в соответствии с xsl-шаблоном
		/// </summary>
		/// <param name="xml">Преобразуемый xml</param>
		/// <param name="assembly">Сборка с ресурсами (xslt)</param>
		/// <param name="xslResource">Имя ресурса xslt</param>
		/// <param name="output">Куда выводится результат</param>
		public static void Transform(string xml, Assembly assembly, string xslResource, TextWriter output)
		{
			var xslStream = assembly.GetManifestResourceStream(xslResource);
			XslCompiledTransform xct = new XslCompiledTransform();
			XmlReader styleSheet = XmlReader.Create(xslStream);
			XsltSettings settings = new XsltSettings(true, true);
			XmlResolver res = new XmlResolver(assembly);
			xct.Load(styleSheet, settings, res);
			XmlReader xr = XmlReader.Create(new StringReader(xml));
			xct.Transform(xr, null, output);
		}
	}
}
