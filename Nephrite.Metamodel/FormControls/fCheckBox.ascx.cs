using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Nephrite.Metamodel.Model;
using Nephrite.Web;



namespace Nephrite.Metamodel.FormControls
{
	public partial class fCheckBox : System.Web.UI.UserControl, IXMLFormControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void SetValue(XElement element)
		{
			if (!String.IsNullOrEmpty(element.Value))
				cb.Checked = Convert.ToBoolean(element.Value);
			
		}

		public XElement GetValue(XElement primaryElement)
		{
			return new XElement(primaryElement.Name, cb.Checked);
		}

		public bool HasValue
		{
			get { return true; }
		}
	}
}