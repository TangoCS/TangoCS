using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.IO;
using Nephrite.HtmlPurifier;
using System.Text.RegularExpressions;

namespace Nephrite.Web.Controls
{
	public class TinyMCEEditor : System.Web.UI.UserControl
	{
		public string Tags { get; set; }
		public Unit Height { get; set; }

		public string Text
		{
			get
			{
				string s = tbBody.Text;
				s = s.Replace("\"image.ashx?", "\"/image.ashx?");
				s = Regex.Replace(s, "<!--.*?-->", "", RegexOptions.Singleline);
				s = Regex.Replace(s, "&lt;!--.*?--&gt;", "", RegexOptions.Singleline);

				//if (!isInitialized)
				if (Tags != "*")
				{
					Purifier.SetTagsXml(File.ReadAllText(Server.MapPath(String.IsNullOrEmpty(Tags) ? "tags.xml" : Tags)));
					//Purifier.SetTagsXml((new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Nephrite.Web.tags.xml"))).ReadToEnd());

					return Purifier.Purify(s);
				}
				else
					return s;
				//return tbBody.Text.Replace("\"image.ashx?", "\"/image.ashx?");
			}
			set
			{
				tbBody.Text = value;
			}
		}
		TextBox tbBody = new TextBox();
		protected void Page_Init(object sender, EventArgs e)
		{
			Options = new Dictionary<string, string>();
			Options.Add("mode", "'specific_textareas'");
			Options.Add("theme", "'advanced'");
			Options.Add("plugins", "'advimage, advlink, inlinepopups, table, media, emotions, fullscreen'");
			Options.Add("language", "'ru'");
			Options.Add("editor_selector", "'mceEditor'");
			Options.Add("convert_urls", "false");
			Options.Add("theme_advanced_disable", "'help,newdocument,styleselect,visualaid,outdent,indent'");
			Options.Add("theme_advanced_buttons2", "'bullist,numlist,|,hr,|,undo,redo,|,link,unlink,anchor,image,code'");
			Options.Add("theme_advanced_buttons3", "'tablecontrols,|,sub,sup,|,charmap,emotions,media,|,fullscreen'");
			Options.Add("extended_valid_elements", "'ol[start|type],script[language|type],input[id|name|type|value|size|maxlength|checked|accept|src|width|height|disabled|readonly|tabindex|accesskey|onfocus|onblur|onchange|onselect]'");

			tbBody.Width = Unit.Percentage(100);
			tbBody.Rows = 30;
			tbBody.TextMode = TextBoxMode.MultiLine;
			tbBody.CssClass = "mceEditor";
			Controls.Add(tbBody);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Height.IsEmpty) Height = Unit.Pixel(300);
			tbBody.Height = Height;

			Page.ClientScript.RegisterClientScriptInclude("tinyMCE", Settings.JSPath + "tiny_mce/tiny_mce_src.js");
			Page.ClientScript.RegisterStartupScript(typeof(Page), "initTinyMCE", @"tinyMCE.init({" + Options.Select(o => o.Key + ": " + o.Value).Join(",") + @"});", true);
		}

		public Dictionary<string, string> Options { get; private set; }
	}
}