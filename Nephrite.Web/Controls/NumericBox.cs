using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.UI;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
	public class NumericBox : TextBox
	{
		public int? Decimals { get; set; }
		public decimal? MaxValue { get; set; }
		public decimal? MinValue { get; set; }
		public string ViewDataBind { get; set; }
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ViewDataBind != null)
			{
				var v = findViewControl(this);
				if (v != null && v.ViewData != null)
				{
					var p = v.ViewData.GetType().GetProperty(ViewDataBind);
					if (p == null)
						throw new Exception("У класса " + v.ViewData.GetType().FullName + " отсутствует свойство " + ViewDataBind);
			
					if (Page.IsPostBack)
						storeValue(v, p);
					else
						readValue(v, p);
				}
			}
		}
		ViewControl findViewControl(Control c)
		{
			if (c.Parent == null)
				return null;
			if (c.Parent is ViewControl)
				return (ViewControl)c.Parent;
			return findViewControl(c.Parent);
		}
		void readValue(ViewControl view, System.Reflection.PropertyInfo p)
		{
			if (p.PropertyType == typeof(int?) || p.PropertyType == typeof(int))
				IntValue = (int?)p.GetValue(view.ViewData, null);
			
			if (p.PropertyType == typeof(decimal?) || p.PropertyType == typeof(decimal))
				Value = (decimal?)p.GetValue(view.ViewData, null);
		}
		void storeValue(ViewControl view, System.Reflection.PropertyInfo p)
		{
			if (p.PropertyType == typeof(int))
				p.SetValue(view.ViewData, IntValue ?? 0, null);

			if (p.PropertyType == typeof(int?))
				p.SetValue(view.ViewData, IntValue, null);
	
			if (p.PropertyType == typeof(decimal))
				p.SetValue(view.ViewData, Value ?? 0, null);
			
			if (p.PropertyType == typeof(decimal?))
				p.SetValue(view.ViewData, Value, null);
		}
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		
			Page.ClientScript.RegisterClientScriptInclude("jquery-autonumeric", Settings.JSPath + "autoNumeric-1.7.4.js");
			var options = String.Format("aSep: '', aDec: '{0}'", AppWeb.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			if (MinValue.HasValue)
				options += String.Format(", vMin: '{0}'", MinValue.Value.ToString(CultureInfo.InvariantCulture));
			if (MaxValue.HasValue)
				options += String.Format(", vMax: '{0}'", MaxValue.Value.ToString(CultureInfo.InvariantCulture));
			if (Decimals.HasValue)
				options += String.Format(", mDec: '{0}'", Decimals.Value);

			string script = "$('#" + ClientID + "').autoNumeric({" + options + "});";
			ScriptManager.RegisterStartupScript(this, GetType(), ClientID + "_autonumeric", script, true);
		}

		public decimal? Value
		{
			get
			{
				return Text.ToDecimal();
			}
			set
			{
				if (value.HasValue)
				{
					if (Decimals.HasValue)
						Text = value.Value.ToString("#####################0." + "".PadRight(Decimals.Value, '0'), AppWeb.CurrentCulture);
					else
						Text = value.Value.ToString(AppWeb.CurrentCulture);
				}
				else
					Text = "";
			}
		}

		public int? IntValue
		{
			get
			{
				return Text.ToInt32();
			}
			set
			{
				Text = value.ToString();
			}
		}
	}
}