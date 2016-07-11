using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this IHtmlWriter w, string name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(name).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void TextBox(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(name).Type(InputType.Text).Value(value).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void Password(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(name).Type(InputType.Password).Value(value).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void Hidden(this IHtmlWriter w, string name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(name).Type(InputType.Hidden).Value(value).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void CheckBox(this IHtmlWriter w, string name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(name).Type(InputType.Checkbox).Checked(isChecked).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void RadioButton(this IHtmlWriter w, string name, string id, string value = null, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(id).Type(InputType.Radio).Value(value).Checked(isChecked).Set(attributes);
			w.WriteTag("input", a, null, true);
		}
		public static void TextArea(this IHtmlWriter w, string name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			Action<TextAreaTagAttributes> a = ta => ta.Name(name).ID(name).Set(attributes);
			w.WriteTag("textarea", a, () => w.Write(value));
		}

		public static void Button(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Button).Set(attributes);
			w.WriteTag("button", a, inner);
		}

		public static void SubmitButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Submit).Set(attributes);
			w.WriteTag("button", a, inner);
		}
		public static void SubmitButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.SubmitButton(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

		public static void ResetButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Reset).Set(attributes);
			w.WriteTag("button", a, inner);
		}

		public static void Button(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.Button(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

		public static void ResetButton(this IHtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "Reset")
		{
			w.ResetButton(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

	}
}