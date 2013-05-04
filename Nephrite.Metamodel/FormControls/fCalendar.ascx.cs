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
	public partial class fCalendar : System.Web.UI.UserControl, IFormatableXMLFormControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void SetValue(XElement element)
		{
			if (!String.IsNullOrEmpty(element.Value))
				calendar.Date = Convert.ToDateTime(element.Value);
		}

		public XElement GetValue(XElement primaryElement)
		{
			return new XElement(primaryElement.Name, calendar.Date);
		}

		public bool HasValue
		{
			get { return calendar.Date.HasValue; }
		}

		public XElement GetValue(XElement primaryElement, string format)
		{
			if (HasValue)
				return new XElement(primaryElement.Name, calendar.Date.Value.ToString(format));
			else
				return new XElement(primaryElement.Name, String.Empty);
		}
	}
}