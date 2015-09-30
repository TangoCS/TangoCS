using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this IHtmlWriter w, string name, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void TextBox(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Text);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void Password(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Password);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void Hidden(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Hidden);
			if (value != null) ta.Value(value);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void CheckBox(this IHtmlWriter w, string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Checkbox).Checked(isChecked);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void RadioButton(this IHtmlWriter w, string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			InputTagAttributes ta = new InputTagAttributes(tb);
			ta.Name(name).Type(InputType.Radio).Checked(isChecked);
			if (attributes != null) attributes(ta);
			tb.Render(w);
		}
		public static void TextArea(this IHtmlWriter w, string name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("textarea");
			TextAreaTagAttributes ta = new TextAreaTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			tb.Render(w, TagRenderMode.StartTag);
			w.Write(value);
			tb.Render(w, TagRenderMode.EndTag);
		}

		public static void Button(this IHtmlWriter w, string name, Action<ButtonTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("button");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

        public static void Button(this IHtmlWriter w, string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			w.Button(name, attributes, () => w.Write(title));
        }
		public static void SubmitButton(this IHtmlWriter w, string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name).Type(ButtonType.Submit);
			if (attributes != null) attributes(ta);
			tb.Render(w, TagRenderMode.StartTag);
			w.Write(title);
			tb.Render(w, TagRenderMode.EndTag);
		}
		public static void ResetButton(this IHtmlWriter w, string name, string title, Action<ButtonTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("input");
			ButtonTagAttributes ta = new ButtonTagAttributes(tb);
			ta.Name(name).Type(ButtonType.Reset);
			if (attributes != null) attributes(ta);
			tb.Render(w, TagRenderMode.StartTag);
			w.Write(title);
			tb.Render(w, TagRenderMode.EndTag);
		}

	}
}