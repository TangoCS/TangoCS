using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this IHtmlWriter w, string name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void TextBox(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.Text).Value(value);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void Password(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.Password).Value(value);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void Hidden(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.Hidden).Value(value);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void CheckBox(this IHtmlWriter w, string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.Checkbox).Checked(isChecked);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void RadioButton(this IHtmlWriter w, string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.Radio).Checked(isChecked);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("input", a, null, true);
		}
		public static void TextArea(this IHtmlWriter w, string name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			Action<TextAreaTagAttributes> a = ta => {
				ta.Name(name).ID(name);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("textarea", a, () => w.Write(value));
		}

		public static void Button(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => {
				ta.Type(ButtonType.Button);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("button", a, inner);
		}

		public static void SubmitButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => {
				ta.Type(ButtonType.Submit);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("button", a, inner);
		}
		public static void ResetButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => {
				ta.Type(ButtonType.Reset);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("button", a, inner);
		}

		public static void Button(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.Button(attributes, () => w.Write(text));
		}

		public static void SubmitButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.SubmitButton(attributes, () => w.Write(text));
		}

		public static void ResetButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "Reset")
		{
			w.ResetButton(attributes, () => w.Write(text));
		}

	}
}