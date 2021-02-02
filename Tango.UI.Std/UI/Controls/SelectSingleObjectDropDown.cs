using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tango;
using Tango.Html;
using Tango.UI;
using Tango.UI.Controls;

namespace Tango.UI.Controls
{
	public class SelectSingleObjectDropDown<TRef, TRefKey> : SelectObjectDialog<TRef, TRefKey, TRef, ISelectSingleObjectField<TRef, TRefKey, TRef>>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		readonly string ctrlName = "selectObjectDropDown";

		public override void OnInit()
		{
			base.OnInit();
			Paging.PageSize = 10;
		}

		public override void List(LayoutWriter w, IEnumerable<TRef> data)
		{
			w.Div(a => a.ID().DataCtrl(ctrlName, Field.ClientID).DataResultHandler(), () => {
				w.Div(a => a.Class("radiobuttonlist"), () => {
					foreach (var o in data)
					{
						var value = Field.DataValueField(o);
						w.Label(a => a.Class("row").For("item" + value).DataResultPostponed(1), () => {
							w.RadioButton("item", "item" + value, value, false, a => { if (Field.PostOnChangeEvent) a.OnChangePostEvent(SubmitDialog); a.Data(DataCollection).DataRef(Field.ParentElement, Field.ID); }); //DataCollection
							Field.DataRow(w, o);
						});
					}
				});
			});
			AfterList?.Invoke(w);
		}

		public override void ToolbarLeft(MenuBuilder t) { }

		void RenderSelected(LayoutWriter w, TRef selectedValue)
		{
			var cw = w.Clone(Field);

			var val = selectedValue == null ? "" : Field.SelectedObjectTextField(selectedValue);

			cw.Div(a => a.ID("selected").Class("selectedcontainer"), () => {
				if (Field.TextWhenDisabled != null)
					cw.Div(a => a.Class("selected disabledtext").Class(Field.Disabled || Field.ReadOnly ? "" : "hide"), () => Field.TextWhenDisabled.Invoke(cw));

				if (Field.TextWhenNothingSelected != null)
					cw.Div(a => a.Class("selected nothingselectedtext").Class(selectedValue == null ? "" : "hide"), () => Field.TextWhenNothingSelected.Invoke(cw));

				cw.Div(a => a.Class("selected object").Class(val.IsEmpty() ? "hide" : ""), () => {
					cw.Span(val);
					if (!Field.Disabled && !Field.ReadOnly)
					{
						cw.A(a => {
							a.Class("close").OnClick($"{ctrlName}Field.clear(this)");
							if (Field.PostOnClearEvent) a.DataEvent(OnClear);
						}, () => cw.Icon("close"));
					}
				});
			});
		}

		void RenderFilter(LayoutWriter w)
		{
			w.TextBox(Field.FilterFieldName, "", a => {
				a.ID(Field.FilterFieldName).Class("filter").Autocomplete(false);
				if (Field.ReadOnly) a.Readonly(true);
				if (Field.Disabled) a.Disabled(true);
			});
			w.Span(a => a.Class("input-measure").ID("measure"), "");
		}

		public override void Render(LayoutWriter w, TRef selectedValue)
		{
			var cw = w.Clone(Field);
			var pw = w.Clone(Field.ParentElement);
			
			cw.Div(a => {
				a.ID("placeholder").Class("selectsingleobject").DataCtrl("selectObjectDropDownField", Field.ClientID);
				if (Field.Disabled || Field.ReadOnly) a.Data("disabled", true);
			}, () => {
				RenderSelected(w, selectedValue);
				RenderFilter(cw);

				if (!Field.Disabled && !Field.ReadOnly)//делать класс Hide
					cw.I(a => a.ID("btn").Class("btn").Icon("dropdownarrow-angle"));

				var value = selectedValue != null ? Field.DataValueField(selectedValue) : "";
				pw.Hidden(Field.ID, value, a => { 
					a.DataHasClientState(ClientStateType.Value, Field.ClientID, "selectedvalue");
					if (Field.ReadOnly) a.Readonly(true);
					if (Field.Disabled) a.Disabled(true);
				});
			});

			cw.Div(a => a.ID("popup").Class("selectsingleobject-popup").DataRef(Field.FilterFieldName).Data(DataCollection).DataEvent(OpenDialog).DataContainer(typeof(SelectObjectPopupContainer), Field.ClientID));
		}

		public void OnClear(ApiResponse response)
		{
			Field.OnClear(response);
		}

		public override void SubmitDialog(ApiResponse response)
		{
			if (Field.PostOnChangeEvent)
			{
				var id = Context.GetArg<TRefKey>(Field.ID);
				var selectedValue = Field.GetObjectByID(id);

				Field.OnChange(response, selectedValue);
			}
		}
	}

	public class SelectSingleObjectDropDownField<TRef, TRefKey> : SelectObjectField<TRef, TRefKey, TRef>, ISelectSingleObjectField<TRef, TRefKey, TRef>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		public Func<TRefKey, Expression<Func<TRef, bool>>> FilterSelected { get; set; }
		public SelectSingleObjectDropDown<TRef, TRefKey> Strategy { get; set; }

		public override void OnInit()
		{
			Strategy = CreateControl<SelectSingleObjectDropDown<TRef, TRefKey>>("str", c => c.Field = this);
		}

		public TRef GetObjectByID(TRefKey id)
		{
			var keySelector = FilterSelected ?? new TRef().KeySelector;
			return DataProvider.GetObjectByID(id, keySelector(id));
		}
	}

	public static class SelectSingleObjectDropDownExtensions
	{
		public static void FormFieldSelectDialog<TSelected, TSelectedKey>(this LayoutWriter w, string caption, TSelected obj, 
			SelectSingleObjectDropDownField<TSelected, TSelectedKey> dialog, GridPosition grid = null, bool isRequired = false, string description = null)
			where TSelected : class, IWithKey<TSelected, TSelectedKey>, new()
		{
			w.FormField(dialog.ID, caption, () => dialog.Strategy.Render(w, obj), grid, isRequired, description);
		}
	}

	public class SelectObjectPopupContainer : ViewContainer
	{
		public SelectObjectPopupContainer()
		{
			Mapping.Add("contentbody", "body");
			Mapping.Add("contenttitle", "title");
			Mapping.Add("buttonsbar", "footer");
			Mapping.Add("form", "body");
			Mapping.Add("title", "title");
			Mapping.Add("contenttoolbar", "toolbar");
		}
		public override void Render(ApiResponse response)
		{
			response.AddWidget(Context.Sender, w => {
				w.AjaxForm("form", () => w.Div(a => a.ID("body")));
				w.Div(a => a.ID("toolbar"));
			});
		}
	}
}
