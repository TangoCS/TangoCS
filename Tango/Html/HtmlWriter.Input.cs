using System;
using System.Net;
using Tango.UI;

namespace Tango.Html
{
	public static class HtmlWriterInputExtensions
	{
		public static void Input(this HtmlWriter w, InputName name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Set(attributes);
			};
			w.WriteSelfClosingTag("input", a);
		}
		public static void TextBox(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			value = WebUtility.HtmlEncode(value);
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Type(InputType.Text).Value(value).Set(attributes);
			};
			w.WriteSelfClosingTag("input", a);
		}
		public static void Password(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Type(InputType.Password).Value(value).Set(attributes);
			};
			w.WriteSelfClosingTag("input", a);
		}
		public static void Hidden(this HtmlWriter w, InputName name, string value = null, Action<InputTagAttributes> attributes = null)
		{
			value = WebUtility.HtmlEncode(value);
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Type(InputType.Hidden).Value(value).Set(attributes);
			};
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this HtmlWriter w, InputName name, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Type(InputType.Checkbox).Checked(isChecked).Set(attributes);
			};
			w.WriteSelfClosingTag("input", a);
		}
		public static void CheckBox(this HtmlWriter w, InputName name, string value, bool isChecked = false, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Type(InputType.Checkbox).Value(value).Checked(isChecked).Set(attributes);
			};
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
			Action<TextAreaTagAttributes> a = ta => {
				if (name != null) ta.Name(name.Name).ID(name.ID);
				ta.Set(attributes);
			};
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

		public static void ResetButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes, Action inner)
		{
			Action<ButtonTagAttributes> a = ta => ta.Type(ButtonType.Reset).Set(attributes);
			w.WriteTag("button", a, inner);
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

        public static implicit operator InputName((string name, string id) input)
        {
            return new InputName { ID = input.id, Name = input.name };
        }
    }
}