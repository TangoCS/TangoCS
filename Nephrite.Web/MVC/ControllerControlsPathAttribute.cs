using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
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
