using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nephrite.Web.Controls
{
	public class HtmlDropDownList
	{
		public string Name { get; set; }
		public string SelectedValue { get; set; }
		public string DataTextField { get; set; }
		public string DataValueField { get; set; }
		public string ID { get; set; }
		public string Width { get; set; }
		public object DataSource { get; set; }
		public bool InsertEmptyValue { get; set; }
		public bool Enabled { get; set; }

		public HtmlDropDownList()
		{
			Enabled = true;
		}

		public override string ToString()
		{
			StringBuilder res = new StringBuilder();
			res.Append("<select ");
			if (!String.IsNullOrEmpty(Width))
				res.AppendFormat(@"style=""width: {0};"" ", Width);
			if (!String.IsNullOrEmpty(ID))
				res.AppendFormat(@"id=""{0}"" ", ID);
			if (!String.IsNullOrEmpty(Name))
				res.AppendFormat(@"name=""{0}"" ", Name);
			if (!Enabled)
				res.Append(@"disabled=""disabled"">");
			else
				res.Append(">");
			if (SelectedValue == null)
				SelectedValue = HttpContext.Current.Request.Form[Name];
			if (DataTextField == null)
				DataTextField = "Title";
			if (DataValueField == null)
				DataValueField = "ObjectID";
			if (InsertEmptyValue)
				res.AppendFormat(@"<option value=""""></option>");
			if (DataSource != null)
			{
				System.Web.UI.WebControls.DropDownList ddl = new System.Web.UI.WebControls.DropDownList();
				ddl.DataTextField = DataTextField;
				ddl.DataValueField = DataValueField;
				ddl.DataSource = DataSource;
				ddl.DataBind();
				foreach(System.Web.UI.WebControls.ListItem li in ddl.Items)
					res.AppendFormat(@"<option value=""{0}""{2}>{1}</option>", li.Value, HttpUtility.HtmlEncode(li.Text), li.Value == SelectedValue ? @" selected=""selected""" : "");
			}
	
			res.Append("</select>");
			return res.ToString();
		}
	}
}