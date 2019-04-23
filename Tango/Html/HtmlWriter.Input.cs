using System;
using System.Net;

namespace Tango.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this HtmlWriter w, InputName name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void TextBox(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			value = WebUtility.HtmlEncode(value);
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Text).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void Password(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Password).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void Hidden(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			value = WebUtility.HtmlEncode(value);
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Hidden).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this HtmlWriter w, InputName name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Checkbox).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this HtmlWriter w, InputName name, string value, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Checkbox).Value(value).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void RadioButton(this HtmlWriter w, string name, string id, string value = null, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(id).Type(InputType.Radio).Value(value).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void TextArea(this HtmlWriter w, InputName name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			value = WebUtility.HtmlEncode(value);
			Action<TextAreaTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Set(attributes);
			w.WriteTag("textarea", a, () => w.Write(value));
		}

		public static void Button(this HtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Button).Set(attributes);
			w.WriteTag("button", a, inner);
		}

		public static void SubmitButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Submit).Set(attributes);
			w.WriteTag("button", a, inner);
		}
		public static void SubmitButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.SubmitButton(a => a.Class("btn btn-primary").Set(attributes), () => w.Write(text));
		}
		public static void SubmitButtonConfirm(this HtmlWriter w, Action<ButtonTagAttributes> attributes, string text, string message)
		{
			w.SubmitButton(a => a.Class("btn btn-primary").Data("confirm", message).Set(attributes), () => w.Write(text));
		}
		public static void SubmitButtonConfirm(this HtmlWriter w, string text, string message)
		{
			w.SubmitButtonConfirm(null, text, message);
		}

		public static void ResetButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Reset).Set(attributes);
			w.WriteTag("button", a, inner);
		}

		public static void Button(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.Button(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

		public static void ResetButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "Reset")
		{
			w.ResetButton(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}
	}

	public class InputName
	{
		public string ID { get; set; }
		public string Name { get; set; }

		public static implicit operator InputName(string name)
		{
			return new InputName { ID = name, Name = name };
		}
	}
}