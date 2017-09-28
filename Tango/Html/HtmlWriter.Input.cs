using System;

namespace Tango.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this IHtmlWriter w, InputName name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void TextBox(this IHtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Text).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void Password(this IHtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Password).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void Hidden(this IHtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Hidden).Value(value).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this IHtmlWriter w, InputName name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Checkbox).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this IHtmlWriter w, InputName name, string value, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Type(InputType.Checkbox).Value(value).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void RadioButton(this IHtmlWriter w, string name, string id, string value = null, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => ta.Name(name).ID(id).Type(InputType.Radio).Value(value).Checked(isChecked).Set(attributes);
			w.WriteSelfClosingTag("input", a);
		}
		public static void TextArea(this IHtmlWriter w, InputName name, string value = null, Action<TextAreaTagAttributes> attributes = null)
		{
			Action<TextAreaTagAttributes> a = ta => ta.Name(name.Name).ID(name.ID).Set(attributes);
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
			w.SubmitButton(a => a.Class("btn btn-primary").Set(attributes), () => w.Write(text));
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