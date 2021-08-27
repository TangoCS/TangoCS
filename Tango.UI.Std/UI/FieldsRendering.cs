using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI
{
	public static class FieldRender
	{
		public static void Render<T>(this T field, Action<T> content)
			where T : IField
		{
			content(field);
		}

		public static void FormField(this LayoutWriter w, IField field, Action content, GridPosition grid = null)
		{
			w.FormField(
				field.ID,
				field.Caption,
				content,
				grid,
				field.IsRequired,
				field.ShowDescription ? field.Description : null,
				field.IsVisible,
				field.Hint,
				field.WithCheckBox,
				field.Disabled
			);
		}

		public static void FormFieldCaption<TValue>(this LayoutWriter w, IField<TValue> field)
        {
			w.FieldBlockRenderer.FormFieldCaption(w, field.ID, () => w.Write(field.Caption), field.IsRequired, field.Hint);
        }

		public static void TextBox<TValue>(this LayoutWriter w, IField<TValue> field, GridPosition grid = null, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(field, () => w.TextBox(field.ID, field.StringValue, a => {
				if (field.Disabled) a.Disabled(true);
				else if (field.ReadOnly) a.Readonly(true);
				a.Set(attributes);
			}), grid) ;
		}
		public static void TextBox2<TValue>(this LayoutWriter w, IField<TValue> field, GridPosition grid = null, Action<InputTagAttributes> attributes = null)
		{
			w.TextBox(field.ID, field.StringValue, a => {
				if (field.Disabled) a.Disabled(true);
				else if (field.ReadOnly) a.Readonly(true);
				a.Set(attributes);
			});
		}

		public static void Hidden<TValue>(this LayoutWriter w, IField<TValue> field)
		{
			w.TextBox(field.ID, field.Value.ToString(), a => a.Type(InputType.Hidden));
		}

		public static void PlainText<TValue>(this LayoutWriter w, IField<TValue> field, GridPosition grid = null)
		{
			var val = field.StringValue;
			if (typeof(TValue) == typeof(bool) && (val == "True" || val == "False"))
				val = (field as IField<bool>).Value.Icon();
			w.FormField(field.ID, field.Caption, () => w.Div(a => a.ID(field.ID).Class("field-plaintext"), val), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void PlainText(this LayoutWriter w, IField field, Action content, GridPosition grid = null)
		{
			w.FormField(field.ID, field.Caption, () => w.Div(a => a.ID(field.ID).Class("field-plaintext"), content), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void PlainText(this LayoutWriter w, string caption, Action content, string description = null, GridPosition grid = null)
		{
			w.FormField(null, caption, () => w.Div(a => a.Class("field-plaintext"), content), grid, false, description);
		}

		public static void PlainText<T>(this LayoutWriter w, string caption, T value, string description  = null, GridPosition grid = null)
		{
			w.FormField(null, caption, () => w.Div(a => a.Class("field-plaintext"), value?.ToString()), grid, false, description);
		}

		public static void PlainText<TEntity, T, T2>(this LayoutWriter w, Expression<Func<TEntity, T>> expr, T2 value, GridPosition grid = null)
		{
			var key = expr.GetResourceKey();
			var caption = w.Resources.Get(key);
			w.FormField(null, caption, () => w.Div(a => a.Class("field-plaintext"), value.ToString()), grid);
		}

		public static void PlainText<TEntity, TRefClass, TRefKey>(this LayoutWriter w, EntityReferenceManyField<TEntity, TRefClass, TRefKey> field, GridPosition grid = null)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			void val() => w.Div(a => a.ID(field.ID).Class("field-plaintext"), () => w.Write(field.StringValueCollection.Join("<br/>")));
			w.FormField(field.ID, field.Caption, val, grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void Password(this LayoutWriter w, IField<string> field, GridPosition grid = null)
		{
			w.FormField(field, () => w.Password(field.ID), grid);
		}

		public static void TextArea(this LayoutWriter w, IField<string> field, GridPosition grid = null, Action<TextAreaTagAttributes> attributes = null)
		{
			w.FormField(field, () => w.TextArea(field.ID, field.Value, a => {
				if (field.Disabled) a.Disabled(true);
				a.Set(attributes);
			}), grid);
		}

		public static void CheckBox(this LayoutWriter w, IField<bool> field, GridPosition grid = null)
		{
			w.FormField(field.ID, field.Caption, () => w.CheckBox(field.ID, field.Value, a => {
				if (field.Disabled) a.Disabled(true);
				if (field.ReadOnly) a.Readonly(true).OnChange("event.preventDefault(); this.checked = !this.checked; return false;");
			}), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void CheckBox(this LayoutWriter w, IField<bool?> field, GridPosition grid = null)
		{
			w.FormField(field.ID, field.Caption, () => w.CheckBox(field.ID, field.Value ?? false, a => {
				if (field.Disabled) a.Disabled(true);
				if (field.ReadOnly) a.Readonly(true).OnChange("event.preventDefault(); this.checked = !this.checked; return false;");
			}), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void ToggleSwitch(this LayoutWriter w, IField<bool> field, GridPosition grid = null, Action<InputTagAttributes> attributes = null)
		{
            if (field.FireOnChangeEvent && field.IsVisible)                         
                attributes += a => a.Data("e", $"On{field.ID}Changed").Data("r", field.EventReceiver);
            
            w.FormField(field.ID, field.Caption, () => w.ToggleSwitch(field.ID, field.Value, field.Disabled, field.ReadOnly, attributes), 
				grid, false, field.ShowDescription ? field.Description : null, field.IsVisible, field.Hint, field.WithCheckBox,field.Disabled);           
        }

		public static void ToggleSwitch2(this LayoutWriter w, IField<bool> field, GridPosition grid = null, Action<InputTagAttributes> attributes = null)
		{
			if (field.FireOnChangeEvent && !field.Disabled && field.IsVisible)
				attributes += a => a.Data("e", $"On{field.ID}Changed").Data("r", field.EventReceiver);

			w.ToggleSwitch(field.ID, field.Value, field.Disabled, field.ReadOnly, attributes);
		}
		public static void DropDownList2<TValue>(this LayoutWriter w, IField<TValue> field, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attrs = null)
		{
			var value = typeof(TValue).IsEnum ?
				field.Value == null ? "" : Convert.ChangeType(field.Value, Enum.GetUnderlyingType(typeof(TValue))).ToString() :
				field.Value?.ToString();

			if (field.FireOnChangeEvent && !field.Disabled && field.IsVisible)
				attrs += a => a.OnChangePostEvent($"On{field.ID}Changed", field.EventReceiver);

			if (field.Disabled)
				attrs += a => a.Disabled(true);

			if (field.ReadOnly)
				w.AddClientAction("domActions", "setAttribute", f => new { id = f(field.ID), attrName = "readonly", attrValue = "readonly" });

			w.DropDownList(field.ID, value, items, attrs);
		}

		public static void DropDownList<TValue>(this LayoutWriter w, IField<TValue> field, IEnumerable<SelectListItem> items, GridPosition grid = null, Action<SelectTagAttributes> attrs = null, string hint = null)
		{
			var value = typeof(TValue).IsEnum ?
				field.Value == null ? "" : Convert.ChangeType(field.Value, Enum.GetUnderlyingType(typeof(TValue))).ToString() : 
				field.Value?.ToString();

			if (field.FireOnChangeEvent && !field.Disabled && field.IsVisible)
				attrs += a => a.OnChangePostEvent($"On{field.ID}Changed", field.EventReceiver);

			if (field.Disabled)
				attrs += a => a.Disabled(true);

			if (field.ReadOnly)
				w.AddClientAction("domActions", "setAttribute", f => new { id = f(field.ID), attrName = "readonly", attrValue = "readonly" });

			w.FormField(field, () => w.DropDownList(field.ID, value, items, attrs), grid);
		}

		//public static void Calendar(this LayoutWriter w, IField<DateTime> field, Action<InputTagAttributes> attributes = null, GridPosition grid = null)
		//{		
		//	var state = field.Disabled ? EnabledState.Disabled : (field.ReadOnly ? EnabledState.ReadOnly : EnabledState.Enabled);

		//	w.FormField(field, () => w.Calendar(field.ID, field.Value, state, attributes: attributes), grid);
		//}

		public static void Calendar(this LayoutWriter w, IField<DateTime> field, CalendarOptions opt = null, GridPosition grid = null)
		{
			var state = field.Disabled ? EnabledState.Disabled : (field.ReadOnly ? EnabledState.ReadOnly : EnabledState.Enabled);
			if (opt == null)
				w.FormField(field, () => w.Calendar(field.ID, field.Value, state), grid);
			else
			{
				opt.Enabled = state;
				w.FormField(field, () => w.Calendar(field.ID, field.Value, opt), grid);
			}
		}

		public static void Calendar(this LayoutWriter w, IField<DateTime?> field, CalendarOptions opt = null, GridPosition grid = null)
		{
			var state = field.Disabled ? EnabledState.Disabled : (field.ReadOnly ? EnabledState.ReadOnly : EnabledState.Enabled);
            if (opt == null)
                w.FormField(field, () => w.Calendar(field.ID, field.Value, state), grid);
            else
            {
                opt.Enabled = state;
                w.FormField(field, () => w.Calendar(field.ID, field.Value, opt), grid);
            }
        }
		
		public static void Calendar(this LayoutWriter w, IField<DateTime?> field, Action<InputTagAttributes> attributes, GridPosition grid = null)
		{
			var state = field.Disabled ? EnabledState.Disabled : (field.ReadOnly ? EnabledState.ReadOnly : EnabledState.Enabled);

			w.FormField(field, () => w.Calendar(field.ID, field.Value, state, attributes: attributes), grid);
		}

		public static void FileUpload(this LayoutWriter w, IField<byte[]> field, GridPosition grid = null, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(field, () => w.FileUpload(field.ID, a => {
				if (field.Disabled) a.Disabled(true);
				a.Set(attributes);
			}), grid);
		}
	}

	public static class FieldActionLinkExtensions
	{
		public static ActionLink ForField(this ActionLink link, IField field)
		{
			return link.WithTitle(field.StringValue);
		}
	}

	
}
