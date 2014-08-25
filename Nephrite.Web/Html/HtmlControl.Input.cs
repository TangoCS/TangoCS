using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Web.Html
{
	public partial class HtmlControl
	{
		public void TextBox(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Type = Type.Text, Name = name };
			if (value != null) ta.Value = value;
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void Password(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Type = Type.Password, Name = name };
			if (value != null) ta.Value = value;
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void Hidden(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Type = Type.Hidden, Name = name };
			if (value != null) ta.Value = value;
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void CheckBox(string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Type = Type.Hidden, Name = name, Checked = isChecked };
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void RadioButton(string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Type = Type.Radio, Name = name, Checked = isChecked };
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void TextArea(string name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("textarea");
			TextAreaTagAttributes ta = new TextAreaTagAttributes(tb) { Name = name };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(value);
			Write(tb.Render(TagRenderMode.EndTag));
		}

		public void Button(string name, string title, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Name = name, Type = Type.Button };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(title);
			Write(tb.Render(TagRenderMode.EndTag));
		}
		public void SubmitButton(string name, string title, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Name = name, Type = Type.Submit };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(title);
			Write(tb.Render(TagRenderMode.EndTag));
		}
		public void ResetButton(string name, string title, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb) { Name = name, Type = Type.Reset };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(title);
			Write(tb.Render(TagRenderMode.EndTag));
		}

	}
}