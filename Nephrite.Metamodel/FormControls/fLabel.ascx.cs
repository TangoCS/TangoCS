using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel.FormControls
{
	public partial class fLabel : System.Web.UI.UserControl, IXMLFormControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void SetValue(XElement element)
		{
			
		}

		public XElement GetValue(XElement primaryElement)
		{
			return primaryElement;
		}

		public bool HasValue
		{
			get { return true; }
		}
	}
}