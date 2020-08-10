using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;

namespace Tango
{
	public class XmlResolver : System.Xml.XmlResolver
	{
		Assembly assembly;

		public XmlResolver(Assembly a)
		{
			assembly = a;
		}

		public override ICredentials Credentials { set { } }

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			var names = assembly.GetManifestResourceNames();
			var fileName = System.IO.Path.GetFileName(absoluteUri.LocalPath);
			var resName = names.FirstOrDefault(o => o.Contains(fileName));
			if (!names.Any(o => o.Contains(fileName)))
				throw new Exception(fileName + " отсутствует в списке ресурсов " + assembly.FullName + ":\n" + String.Join("\n", names));
			return assembly.GetManifestResourceStream(resName);
		}
	}
}
