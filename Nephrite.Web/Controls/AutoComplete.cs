using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using System.Web.UI;

namespace Nephrite.Web
{
    public class AutoCompleteOptions
    {
        public AutoCompleteOptions()
        {
            MultipleSeparator = ",";
            AllowMultipleSelect = true;
        }
        public string MultipleSeparator { get; set; }
        public bool AllowMultipleSelect { get; set; }
		public string SearchQueryParams { get; set; }
		public Control PostBackControl { get; set; }
    }

    public static class AutoComplete
    {
        public static void Apply(TextBox textBox, IQueryable<IModelObject> items, AutoCompleteOptions options)
        {
            Apply(textBox, items.Select(i => i.Title), options);
        }

        public static void Apply(TextBox textBox, IEnumerable<String> items, AutoCompleteOptions options)
        {
            //textBox.Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-autocomplete/lib/jquery.js");
            textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-autocomplete", Settings.JSPath + "jquery-autocomplete/jquery.autocomplete.js");
            string script = @"var items_" + textBox.ClientID + " = [";
            foreach (string s in items)
                script += "\"" + HttpUtility.HtmlEncode(s) + "\",";
            script += @"""""];
$(""#" + textBox.ClientID + @""").autocomplete(items_" + textBox.ClientID + @", {
    multiple: " + options.AllowMultipleSelect.ToString().ToLower() + @",
    mustMatch: true,
    autoFill: true,
    multipleSeparator: """ + options.MultipleSeparator + @"""
});";
            ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_autoComplete", script, true);
            //textBox.Page.ClientScript.RegisterStartupScript(textBox.GetType(), textBox.ClientID + "_autoComplete", script, true);
            textBox.Load += new EventHandler(textBox_Load);
            textBox.Attributes.Add("multipleSeparator", options.MultipleSeparator);
            textBox.Attributes.Add("multipleSelect", options.AllowMultipleSelect.ToString());
            textBox.PreRender += new EventHandler(textBox_PreRender);
        }

		public static void Apply(TextBox textBox, AutoCompleteOptions options)
		{
			//textBox.Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-autocomplete/lib/jquery.js");
			textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-autocomplete", Settings.JSPath + "jquery-autocomplete/jquery.autocomplete.js");
			string script = @"$(""#" + textBox.ClientID + @""").autocomplete(""" + HttpContext.Current.Request.Url.ToString() + (!String.IsNullOrEmpty(options.SearchQueryParams) ? "&" + options.SearchQueryParams : "") + "&autocompleteclientid=" + textBox.ClientID + @""", {
    multiple: " + options.AllowMultipleSelect.ToString().ToLower() + @",
    mustMatch: false,
    autoFill: false,
    multipleSeparator: """ + options.MultipleSeparator + @"""
});";
			if (options.PostBackControl != null)
			{
				script += "$('#" + textBox.ClientID + @"').result(function(event, data, formatted) {" + textBox.Page.ClientScript.GetPostBackEventReference(options.PostBackControl, "") + "});";
			}

			ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_autoComplete", script, true);
			//textBox.Page.ClientScript.RegisterStartupScript(textBox.GetType(), textBox.ClientID + "_autoComplete", script, true);
			textBox.Load += new EventHandler(textBox_Load);
			textBox.Attributes.Add("multipleSeparator", options.MultipleSeparator);
			textBox.Attributes.Add("multipleSelect", options.AllowMultipleSelect.ToString());
			textBox.PreRender += new EventHandler(textBox_PreRender);
		}

        static void textBox_PreRender(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string sep = textBox.Attributes["multipleSeparator"];
            string ms = textBox.Attributes["multipleSelect"];
            if (ms == true.ToString() && !String.IsNullOrEmpty(sep) && textBox.Text.Length > 0)
            {
                textBox.Text += sep;
            }
        }

        static void textBox_Load(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string sep = textBox.Attributes["multipleSeparator"];
            if (!String.IsNullOrEmpty(sep) && textBox.Attributes["multipleSelect"] == true.ToString())
            {
                textBox.Text = String.Join(sep, textBox.Text.Split(new char[] { sep[0] },
                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).
                    Where(s => s.Length > 0).OrderBy(s => s).Distinct().ToArray());
            }
        }

        public static void Validate(TextBox textBox, IEnumerable<IModelObject> items)
        {
            Validate(textBox, items.Select(i => i.Title));
        }

        public static void Validate(TextBox textBox, IEnumerable<String> items)
        {
            string sep = textBox.Attributes["multipleSeparator"];
            if (!String.IsNullOrEmpty(sep))
            {
                String[] str = items.ToArray();
                textBox.Text = String.Join(sep, textBox.Text.Split(new char[] { sep[0] },
                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).
                    Where(s => s.Length > 0 && str.Contains(s)).OrderBy(s => s).Distinct().ToArray());
            }
        }
    }

}
