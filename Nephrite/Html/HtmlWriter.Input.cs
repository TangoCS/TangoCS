using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public partial class HtmlWriter
	{
		public void Input(string name, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void TextBox(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Text);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void Password(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Password);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void Hidden(string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Hidden);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void CheckBox(string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Checkbox).Checked(isChecked);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void RadioButton(string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Radio).Checked(isChecked);
			if (attributes != null) attributes(ta);
			Write(tb);
		}
		public void TextArea(string name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("textarea");
			TextAreaTagAttributes ta = new TextAreaTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(value);
			Write(tb.Render(TagRenderMode.EndTag));
		}

		public void Button(string name, Action<ButtonTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("button");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}

        public void Button(string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			Button(name, attributes, () => Write(title));
        }
		public void SubmitButton(string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name).Type(ButtonType.Submit);
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(title);
			Write(tb.Render(TagRenderMode.EndTag));
		}
		public void ResetButton(string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name).Type(ButtonType.Reset);
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));
			Write(title);
			Write(tb.Render(TagRenderMode.EndTag));
		}

	}
}