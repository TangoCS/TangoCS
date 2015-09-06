using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Multilanguage;

namespace Nephrite.Web.Controls
{
	public class MultilingualField : BaseCompositeControl
	{
		[Inject]
		public ILanguage Language { get; set; }

		TextBox[] textBoxes;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			textBoxes = new TextBox[Language.List.Count];
			for (int i = 0; i < Language.List.Count; i++)
			{
				textBoxes[i] = new TextBox 
				{ 
					ID = "tb" + i.ToString(),
					ToolTip = Language.List[i].Title, 
					Width = Unit.Percentage(100), 
					TextMode = TextMode,
					Rows = Rows,
					MaxLength = MaxLength
				};
				if (TextMode == TextBoxMode.MultiLine && MaxLength > 0)
					textBoxes[i].Attributes.Add("maxlength", MaxLength.ToString());
				Controls.Add(textBoxes[i]);
			}
			/*
			string autoClear = "";
			foreach (var t in textBoxes)
			{
				autoClear += @"$('#" + t.ClientID + "').change(function() {";
				foreach (var t1 in textBoxes)
				{
					if (t1 != t)
					{
						autoClear += @"if ($('#" + t1.ClientID + "').val() == $('#" + t1.ClientID + @"').attr('initialvalue')) ";
						autoClear += @"$('#" + t1.ClientID + @"').val('');";
					}
				}
				autoClear += "});";
			}
			Page.ClientScript.RegisterStartupScript(GetType(), "AutoClear_" + ClientID, autoClear, true);
			if (TextMode == TextBoxMode.MultiLine && MaxLength > 0)
				Page.ClientScript.RegisterStartupScript(GetType(), "MFTextAreaMaxLength", @"$(document).ready(function() {
	$('textarea[maxlength]').keyup(function(){
		//get the limit from maxlength attribute
		var limit = parseInt($(this).attr('maxlength'));
		//get the current text inside the textarea
		var text = $(this).val();
		//count the number of characters in the text
		var chars = text.length;
		
		//check if there are more characters then allowed
		if(chars > limit){
			//and if there are use substr to get the text before the limit
			var new_text = text.substr(0, limit);
			
			//and change the current text with the new text
			$(this).val(new_text);
		}
	});
		
});", true);*/
		}

		public TextBoxMode TextMode { get; set; }

		public override bool Enabled
		{
			get { return textBoxes[0].Enabled; }
			set 
			{
				for (int i = 0; i < Language.List.Count; i++)
					textBoxes[i].Enabled = value;
			}
		}

		public bool ReadOnly
		{
			get { return textBoxes[0].ReadOnly; }
			set
			{
				for (int i = 0; i < Language.List.Count; i++)
					textBoxes[i].ReadOnly = value;
			}
		}

		public int Rows { get; set; }
		public int MaxLength { get; set; }
		public string Required { get; set; }

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("<table style='border:0; width:100%' cellpadding='0' cellspacing='0'>");
			for (int i = 0; i < Language.List.Count; i++)
			{
				writer.Write("<tr><td style='width:10px; vertical-align:middle'>");
				writer.Write(Language.List[i].Code.ToUpper());
				if ((Required ?? "").ToUpper().Contains(Language.List[i].Code.ToUpper()))
					writer.Write("<span class='ms-formvalidation'>&nbsp;*&nbsp;</span></td><td>");
				else
					writer.Write("&nbsp;</td><td>");
				textBoxes[i].RenderControl(writer);
				writer.Write("</td></tr>");
			}
			writer.Write("</table>");
		}

		public void SetValue(string languageCode, string value)
		{
			for (int i = 0; i < Language.List.Count; i++)
			{
				if (Language.List[i].Code == languageCode)
				{
					textBoxes[i].Text = value;
					textBoxes[i].Attributes.Add("initialvalue", textBoxes[i].Text);
				}
			}
		}

		public string GetValue(string languageCode)
		{
			for (int i = 0; i < Language.List.Count; i++)
			{
				if (Language.List[i].Code == languageCode)
					return textBoxes[i].Text;
			}
			return "";
		}

		public void LoadValues(Func<string, string> getValueFunc)
		{
			for (int i = 0; i < Language.List.Count; i++)
			{
				textBoxes[i].Text = getValueFunc(Language.List[i].Code);
				textBoxes[i].Attributes.Add("initialvalue", textBoxes[i].Text);
			}
		}

		public void SaveValues(Action<string, string> setValueFunc)
		{
			for (int i = 0; i < Language.List.Count; i++)
			{
				setValueFunc(Language.List[i].Code, textBoxes[i].Text);
			}
		}
	}
}