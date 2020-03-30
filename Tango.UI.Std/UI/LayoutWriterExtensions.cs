using System;
using System.Collections.Generic;
using Tango.Html;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI
{
	public static class LayoutWriterExtensions
	{
		public static void FormField(this LayoutWriter w, string name, Action caption, Action content, bool isRequired = false, Action description = null, bool isVisible = true, string hint = null)
		{
			w.Tr(a => a.ID(name + "_field").Style(isVisible ? "" : "display:none"), () => {
				w.Td(a => a.Class("formlabel"), () => {
					w.Span(a => a.ID(name + "_fieldcaption"), caption);
					if (!String.IsNullOrEmpty(hint))
						w.Sup(a => a.Style("margin-left:2px"), () => w.A(a => a.Title(hint), "?"));
					if (isRequired)
						w.Span(a => a.ID(name + "_fieldrequired").Class("formvalidation"), "&nbsp;*");
					if (description != null)
						w.Div(a => a.ID(name + "_fielddescription").Class("descriptiontext"), description);
				});
				w.Td(a => a.ID(name + "_fieldbody").Class("formbody"), content);
			});
		}

		public static string GetFieldID(this IField field) => field.ID + "_field";
		public static string GetCaptionID(this IField field) => field.ID + "_fieldcaption";
		public static string GetRequiredID(this IField field) => field.ID + "_fieldrequired";
		public static string GetDescriptionID(this IField field) => field.ID + "_fielddescription";
		public static string GetBodyID(this IField field) => field.ID + "_fieldbody";

		public static void TwoColumnsTable(this HtmlWriter w, Action content)
		{
			w.Div(a => a.Class("twocolumnstable"), content);
		}

		public static void TwoColumnsRowLongFirst(this HtmlWriter w, Action leftContent, Action rightContent)
		{
			w.Div(a => a.Class("twocolumnsrow longfirst"), () => {
				w.Div(leftContent);
				w.Div(rightContent);
			});
		}

		public static void TwoColumnsRowLongLast(this HtmlWriter w, Action leftContent, Action rightContent)
		{
			w.Div(a => a.Class("twocolumnsrow longlast"), () => {
				w.Div(leftContent);
				w.Div(rightContent);
			});
		}

		public static void Icon(this HtmlWriter w, string name, string tip = null, string color = null)
		{
			w.I(a => {
				a.Icon(name).Title(tip);
				if (color != null)
					a.Style("color:" + color);
			});
		}

		public static T Icon<T>(this TagAttributes<T> a, string name)
			where T : TagAttributes<T>
		{
			return a.Class("icon icon-" + name?.ToLower());
		}


	}

	public static class LayoutWriterHelpers
	{
		public static void FormField(this LayoutWriter w, string name, string caption, Action content, bool isRequired = false, string description = null, bool isVisible = true, string hint = null)
		{
			w.FormField(name, () => w.Write(caption), content, isRequired, description != null ? () => w.Write(description) : (Action)null, isVisible, hint);
		}

		//public static void FormFieldCaption(this LayoutWriter w, string name, string caption, bool isRequired = false, string description = null)
		//{
		//	w.FormFieldCaption(name, () => w.Write(caption), isRequired, description);
		//}

		public static void FormFieldReadOnly<T, TValue>(this LayoutWriter w, IMetaProperty<T, TValue> prop, T model)
		{
			w.FormField(prop.Name, w.Resources.Caption(prop), () => w.Write(prop.GetStringValue(model)), false, w.Resources.Description(prop));
		}

		public static void FormFieldReadOnly(this LayoutWriter w, IMetaProperty prop, string value)
		{
			w.FormField(prop.Name, w.Resources.Caption(prop), () => w.Write(value), false, w.Resources.Description(prop));
		}

		public static void FormFieldReadOnly(this LayoutWriter w, string name, string caption, string value, string description = null)
		{
			w.FormField(name, caption, () => w.Span(a => a.ID(name), value), false, description);
		}

		public static void FormField(this LayoutWriter w, IMetaProperty prop, Action content)
		{
			w.FormField(prop.Name, w.Resources.Caption(prop), content, prop.IsRequired, w.Resources.Description(prop));
		}

		public static void FormFieldTextBox<T, TValue>(this LayoutWriter w, MetaAttribute<T, TValue> prop, T model) where T : class
		{
			w.FormField(
				prop, 
				() => w.TextBox(prop.Name, model != null ? prop.GetValue(model)?.ToString() : null)
			);
		}

		public static void FormFieldTextBox<T, TValue>(this LayoutWriter w, MetaAttribute<T, TValue> prop, string value = null)
		{
			w.FormField(
				prop,
				() => w.TextBox(prop.Name, value)
			);
		}

		public static void FormFieldTextBox<T>(this LayoutWriter w, string name, string caption, T value, bool isRequired = false, string description = null, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(name, caption, () => w.TextBox(name, value?.ToString(), a => a.Set(attributes)), isRequired, description);
		}

		public static void FormFieldTextArea<T, TValue>(this LayoutWriter w, MetaAttribute<T, TValue> prop, T model, Action<TextAreaTagAttributes> attributes = null)
		{
			w.FormField(
				prop,
				() => w.TextArea(prop.Name, model != null ? prop.GetValue(model)?.ToString() : null, a => a.Set(attributes))
			);
		}


		public static void FormFieldCalendar(this LayoutWriter w, string name, string caption, DateTime? value, bool isRequired = false, string description = "", EnabledState enabled = EnabledState.Enabled, bool showTime = false)
		{
			w.FormField(name, caption, () => w.Calendar(name, value, enabled, showTime), isRequired, description);
		}

		public static void FormFieldCalendar<T>(this LayoutWriter w, MetaAttribute<T, DateTime> prop, T model, EnabledState enabled = EnabledState.Enabled, bool showTime = false)
		{
			w.FormField(
				prop,
				() => w.Calendar(prop.Name, model != null ? prop.GetValue(model) : DateTime.MinValue, enabled, showTime)
			);
		}

		public static void FormFieldCalendar<T>(this LayoutWriter w, MetaAttribute<T, DateTime?> prop, T model, EnabledState enabled = EnabledState.Enabled, bool showTime = false)
		{
			w.FormField(
				prop,
				() => w.Calendar(prop.Name, model != null ? prop.GetValue(model) : null, enabled, showTime)
			);
		}

		public static void FormFieldCheckBox(this LayoutWriter w, string name, string caption, bool value = false, bool isRequired = false, string description = "")
		{
			w.FormField(name, caption, () => w.CheckBox(name, value), isRequired, description);
		}

		public static void FormFieldToggleSwitch(this LayoutWriter w, string name, string caption, bool value = false, bool isRequired = false, string description = "")
		{
			w.FormField(name, caption, () => w.ToggleSwitch(name, value), isRequired, description);
		}

		public static void ToggleSwitch(this LayoutWriter w, string name, bool value, bool disabled = false, bool read_only = false, Action<InputTagAttributes> attributes = null)
		{
			w.CheckBox(name, value, a =>  { a.Set(attributes); if (disabled) a.Disabled(true); if (read_only) a.Readonly(true).OnChange("event.preventDefault(); this.checked = !this.checked; return false;"); });
			w.AddClientAction("$", f => "#" + f(name), ("btnSwitch", f => new { Theme = "Light" }));
		}

		public static void FormFieldCheckBox<T>(this LayoutWriter w, MetaAttribute<T, bool> prop, T model)
		{
			w.FormField(prop.Name, w.Resources.Caption(prop), 
				() => w.CheckBox(prop.Name, model != null ? prop.GetValue(model) : false),
				false, w.Resources.Description(prop));
		}

		public static void FormFieldDropDownList<T, TValue>(this LayoutWriter w, MetaAttribute<T, TValue> prop, T model, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			w.FormField(
				prop, 
				() => w.DropDownList(prop.Name, model != null ? prop.GetValue(model)?.ToString() : null, items, 
					a => a.Set(attributes))
			);
		}

		public static void FormFieldDropDownList<T, TValue, TKey>(this LayoutWriter w, MetaReference<T, TValue, TKey> prop, T model, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			w.FormField(
				prop, 
				() => w.DropDownList(prop.Name, model != null ? prop.GetValueID(model)?.ToString() : null, items, 
					a => a.Set(attributes))
			);
		}

		public static void FormFieldDropDownList(this LayoutWriter w, string name, string caption, string value, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			w.FormField(name, caption, () => w.DropDownList(name, value, items, a => a.Set(attributes)));
		}

		public static void FormFieldRadioButtonsList(this LayoutWriter w, string name, string caption, string value, IEnumerable<SelectListItem> items, Action<TagAttributes> attributes = null)
		{
			w.FormField(name, caption, () => w.RadioButtonList(name, value, items, a => a.Set(attributes)));
		}

		public static void FormFieldCheckBoxList(this LayoutWriter w, string name, string caption, string[] value, IEnumerable<SelectListItem> items, Action<TagAttributes> attributes = null, string hint = null)
		{
			w.FormField(name, caption, () => w.CheckBoxList(name, value, items, a => a.Set(attributes)), hint: hint);
		}

		public static void FormFieldPassword(this LayoutWriter w, string name, string caption, bool isRequired = false, string description = null, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(name, caption, () => w.Password(name, null, a => a.Set(attributes)), isRequired, description);
		}
	}

	public static class ApiResponseExtensions
	{
		public static void SetContentBodyMargin(this ApiResponse response)
		{
			response.SetElementClass("contentbody", "contentbodypadding");
		}

		public static void RemoveContentBodyMargin(this ApiResponse response)
		{
			response.RemoveElementClass("contentbody", "contentbodypadding");
		}
	}
}
