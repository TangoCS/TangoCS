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
	public partial class fDate : System.Web.UI.UserControl, IFormatableXMLFormControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

			if (Day.Items.Count == 0)
			{
				Day.Items.Add(new ListItem("День", "0"));
				for (int i = 1; i < 32; i++)
				{
					Day.Items.Add(i.ToString());
				}
			}


			if (Year.Items.Count == 0)
			{
				Year.Items.Add(new ListItem("Год", "0"));
				for (int i = DateTime.Now.Year + 1; i > 1900; i--)
				{
					Year.Items.Add(i.ToString());
				}
			}
		}

		public void SetValue(XElement element)
		{
			if (!String.IsNullOrEmpty(element.Value))
			{
				DateTime dt = Convert.ToDateTime(element.Value);
				Day.SelectedValue = dt.Day.ToString();
				Month.SelectedValue = dt.Month.ToString();
				Year.SelectedValue = dt.Year.ToString();
			}
		}

		public XElement GetValue(XElement primaryElement)
		{
			if (HasValue)
			{
				DateTime dt = new DateTime(
					Convert.ToInt32(Year.SelectedValue),
					Convert.ToInt32(Month.SelectedValue),
					Convert.ToInt32(Day.SelectedValue)
					);
				return new XElement(primaryElement.Name, dt);
			}
			else
				return primaryElement;
		}

		public bool HasValue
		{
			get 
			{
				int d = Convert.ToInt32(Day.SelectedValue);
				int m = Convert.ToInt32(Month.SelectedValue);
				int y = Convert.ToInt32(Year.SelectedValue);
				DateTime dt;
				return (d > 0 && m > 0 && y > 0 && DateTime.TryParse(y.ToString() + "/" + m.ToString() + "/" + d.ToString(), out dt)); 

			}
		}

		public XElement GetValue(XElement primaryElement, string format)
		{
			if (HasValue)
			{
				DateTime dt = new DateTime(
					Convert.ToInt32(Year.SelectedValue),
					Convert.ToInt32(Month.SelectedValue),
					Convert.ToInt32(Day.SelectedValue)
					);
				return new XElement(primaryElement.Name, dt.ToString(format));
			}
			else
				return new XElement(primaryElement.Name, String.Empty);
		}
	}
}