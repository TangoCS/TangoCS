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
	public partial class fTextBox : System.Web.UI.UserControl, IXMLFormControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void SetValue(XElement element)
		{
			tb.Text = element.Value;
		}

		public XElement GetValue(XElement primaryElement)
		{
			return new XElement(primaryElement.Name, HttpUtility.HtmlEncode(tb.Text));
		}

		public bool HasValue
		{
			get { return !String.IsNullOrEmpty(tb.Text); }
		}

		
	}
}