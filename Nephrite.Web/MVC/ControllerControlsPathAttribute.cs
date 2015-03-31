using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Nephrite.Web
{
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ControllerControlsPathAttribute : Attribute
	{
		readonly string path;

		public ControllerControlsPathAttribute(string path)
		{
			this.path = path;
		}

		public string Path
		{
			get { return path;}
		}
	}
}
