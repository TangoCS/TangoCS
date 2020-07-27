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

		//public static void FormField(this LayoutWriter w, IField field, Action content)
		//{
		//	w.FormField(
		//		field.ID,
		//		field.Caption,
		//		content,
		//		field.IsRequired,
		//		field.ShowDescription ? field.Description : null,
		//		field.IsVisible,
		//		field.Hint
		//	);
		//}

		public static void FormField(this LayoutWriter w, IField field, Action content, Grid grid = Grid.OneWhole)
		{
			w.FormField(
				field.ID,
				field.Caption,
				content,
				grid,
				field.IsRequired,
				field.ShowDescription ? field.Description : null,
				field.IsVisible,
				field.Hint
			);
		}

		public static void TextBox<TValue>(this LayoutWriter w, IField<TValue> field, Grid grid = Grid.OneWhole, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(field, () => w.TextBox(field.ID, field.StringValue, a => {
				if (field.Disabled) a.Disabled(true);
				else if (field.ReadOnly) a.Readonly(true);
                a.Set(attributes);
			}), grid);
		}      

        public static void Hidden<TValue>(this LayoutWriter w, IField<TValue> field)
		{
			w.TextBox(field.ID, field.Value.ToString());
		}

		public static void PlainText<TValue>(this LayoutWriter w, IField<TValue> field, Grid grid = Grid.OneWhole)
		{
			var val = field.StringValue;
			if (typeof(TValue) == typeof(bool) && (val == "True" || val == "False"))
				val = (field as IField<bool>).Value.Icon();
			w.FormField(field.ID, field.Caption, () => w.Span(a => a.ID(field.ID), val), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void PlainText(this LayoutWriter w, IField field, Action content, Grid grid = Grid.OneWhole)
		{
			w.FormField(field.ID, field.Caption, () => w.Span(a => a.ID(field.ID), content), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void PlainText(this LayoutWriter w, string caption, Action content, string description = null, Grid grid = Grid.OneWhole)
		{
			w.FormField(null, caption, content, grid, false, description);
		}

		public static void PlainText<T>(this LayoutWriter w, string caption, T value, string description  = null, Grid grid = Grid.OneWhole)
		{
			w.FormField(null, caption, () => w.Write(value), grid, false, description);
		}

		public static void PlainText<TEntity, T, T2>(this LayoutWriter w, Expression<Func<TEntity, T>> expr, T2 value, Grid grid = Grid.OneWhole)
		{
			var key = expr.GetResourceKey();
			var caption = w.Resources.Get(key);
			w.FormField(null, caption, () => w.Write(value.ToString()), grid);
		}

		public static void PlainText<TEntity, TRefClass, TRefKey>(this LayoutWriter w, EntityReferenceManyField<TEntity, TRefClass, TRefKey> field, Grid grid = Grid.OneWhole)
			where TEntity : class
			where TRefClass : class, IWithTitle, IWithKey<TRefKey>
		{
			void val() => w.Span(a => a.ID(field.ID), () => w.Write(field.StringValueCollection.Join("<br/>")));
			w.FormField(field.ID, field.Caption, val, grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void Password(this LayoutWriter w, IField<string> field, Grid grid = Grid.OneWhole)
		{
			w.FormField(field, () => w.Password(field.ID), grid);
		}

		public static void TextArea(this LayoutWriter w, IField<string> field, Grid grid = Grid.OneWhole)
		{
			w.FormField(field, () => w.TextArea(field.ID, field.Value, a => {
				if (field.Disabled) a.Disabled(true);
			}), grid);
		}

		public static void CheckBox(this LayoutWriter w, IField<bool> field, Grid grid = Grid.OneWhole)
		{
			w.FormField(field.ID, field.Caption, () => w.CheckBox(field.ID, field.Value, a => {
				if (field.Disabled) a.Disabled(true);
				if (field.ReadOnly) a.Readonly(true).OnChange("event.preventDefault(); this.checked = !this.checked; return false;");
			}), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void CheckBox(this LayoutWriter w, IField<bool?> field, Grid grid = Grid.OneWhole)
		{
			w.FormField(field.ID, field.Caption, () => w.CheckBox(field.ID, field.Value ?? false, a => {
				if (field.Disabled) a.Disabled(true);
				if (field.ReadOnly) a.Readonly(true).OnChange("event.preventDefault(); this.checked = !this.checked; return false;");
			}), grid, false, field.ShowDescription ? field.Description : null, field.IsVisible);
		}

		public static void ToggleSwitch(this LayoutWriter w, IField<bool> field, Grid grid = Grid.OneWhole, Action<InputTagAttributes> attributes = null)
		{
            if (field.FireOnChangeEvent && !field.Disabled && field.IsVisible)                         
                attributes += a => a.Data("e", $"On{field.ID}Changed").Data("r", field.EventReceiver);
            
            w.FormField(field.ID, field.Caption, () => w.ToggleSwitch(field.ID, field.Value, field.Disabled, field.ReadOnly, attributes), 
				grid, false, field.ShowDescription ? field.Description : null, field.IsVisible, field.Hint);           
        }

		public static void DropDownList<TValue>(this LayoutWriter w, IField<TValue> field, IEnumerable<SelectListItem> items, Grid grid = Grid.OneWhole, Action<SelectTagAttributes> attrs = null, string hint = null)
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

		public static void Calendar(this LayoutWriter w, IField<DateTime> field, Action<InputTagAttributes> attributes = null, Grid grid = Grid.OneWhole)
		{		
			var state = field.Disabled ? EnabledState.Disabled : (field.ReadOnly ? EnabledState.ReadOnly : EnabledState.Enabled);

			w.FormField(field, () => w.Calendar(field.ID, field.Value, state, attributes: attributes), grid);
		}

		public static void Calendar(this LayoutWriter w, IField<DateTime?> field, CalendarOptions opt = null, Grid grid = Grid.OneWhole)
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

		public static void FileUpload(this LayoutWriter w, IField<byte[]> field, Grid grid = Grid.OneWhole, Action<InputTagAttributes> attributes = null)
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

	public enum Grid
	{
		OneWhole = 100,
		OneHalf = 50,
		OneThird = 33,
		TwoThirds = 67,
		OneQuater = 25,
		ThreeQuaters = 75,
		OneFifth = 20,
		TwoFifths = 40,
		ThreeFiths = 60,
		FourFifths = 80
	}
}
