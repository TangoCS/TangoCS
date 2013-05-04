using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Metamodel.Model;
using System.Xml.Linq;

namespace Nephrite.Metamodel.FormControls
{
	public partial class fTextArea : System.Web.UI.UserControl, IXMLFormControl
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
			return new XElement(primaryElement.Name, tb.Text);
		}

		public bool HasValue
		{
			get { return !String.IsNullOrEmpty(tb.Text); }
		}
	}
}