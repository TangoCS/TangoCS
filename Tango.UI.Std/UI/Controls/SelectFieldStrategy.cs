using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public abstract class SelectObjectDialog<TRef, TRefKey, TValue> : ViewComponent
		where TRef : class, IWithKey<TRefKey>
	{
		public string FilterValue { get; set; }
		public string Width { get; set; } = "700px";
		public string Title { get; set; }
		public bool HighlightSearchResults { get; set; } = false;
		protected Paging Paging { get; set; }

		public string FilterFieldName { get; set; } = "filter";

		public override void OnInit()
		{
			Paging = CreateControl<Paging>("page", p => {
				p.PageIndex = Context.GetIntArg(p.ClientID, 1);
				p.PageSize = 20;
			});
			DataCollection.Ref(GetClientID("filter"));
			Title = Resources.Get(typeof(TRef).GetResourceType().FullName + "-pl");
		}

		public void OpenDialog(ApiResponse response)
		{
			FilterValue = Context.GetArg(FilterFieldName);
			response.AddWidget("contenttitle", Title);
			response.AddWidget("contentbody", List);
			response.AddWidget("contenttoolbar", w => w.Toolbar(ToolbarLeft, ToolbarRight));
			response.AddWidget("buttonsbar", Footer);
		}

		public virtual void ToolbarLeft(MenuBuilder t)
		{
			t.Item(w => w.TextBox(FilterFieldName, FilterValue, a =>
				a.Class("selectdialog_filter").Data(DataCollection).Placeholder("Поиск").Autofocus(true).Autocomplete(false)
			));
		}

		public virtual void ToolbarRight(MenuBuilder t)
		{
		}

		public virtual void RenderList(ApiResponse response)
		{
			FilterValue = Context.GetArg(FilterFieldName);
			//response.WithNamesAndWritersFor(this);
			response.AddWidget(ClientID, List);
			response.AddWidget(Paging.ID, RenderPaging);
		}

		public abstract void SubmitDialog(ApiResponse response);
		public abstract void List(LayoutWriter w);
		public Action<LayoutWriter> AfterList { get; set; }

		protected abstract string FieldID { get; }

		public virtual void Footer(LayoutWriter w)
		{
			w.Button(a => a.DataResultPostponed(1).OnClickPostEvent(SubmitDialog).DataRef("#" + FieldID), "OK");
			//w.SubmitButton();
			w.Write("&nbsp;");
			w.BackButton();
		}
		public abstract void Render(LayoutWriter w, TValue selectedValue);
		public abstract void RenderPaging(LayoutWriter w);
	}

	public class SelectSingleObjectDialog<TRef, TRefKey> : SelectObjectDialog<TRef, TRefKey, TRef>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		public SelectSingleObjectField<TRef, TRefKey> Field { get; set; }
		protected override string FieldID => Field.ClientID;

		public override void OnInit()
		{
			DataCollection = Field.DataCollection;
			base.OnInit();
		}

		public override void List(LayoutWriter w)
		{
			w.Div(a => a.ID().DataCtrl("selectSingleObjectDialog"), () => {
				w.Div(a => a.Class("radiobuttonlist"), () => {
					foreach (var item in Field.DataProvider.GetData(Paging, FilterValue).Select(o => Field.GetListItem(o, FilterValue, HighlightSearchResults)))
					{
						w.Label(a => a.For("item" + item.Value), () => {
							w.RadioButton("item", "item" + item.Value, item.Value);
							w.Write(item.Text);
						});
					}
				});
				
			});
			AfterList?.Invoke(w);
		}

		public override void ToolbarRight(MenuBuilder t)
		{
			t.Item(RenderPaging);
		}

		public override void RenderPaging(LayoutWriter w)
		{
			Paging.Render(w, Field.DataProvider.GetCount(FilterValue), a => a.RunEvent(RenderList));
		}

		public override void Render(LayoutWriter w, TRef selectedValue)
		{
			var cw = w.Clone(Field);
			var pw = w.Clone(Field.ParentElement);
			cw.Div(a => a.ID("placeholder"), () => {
				var value = selectedValue != null ? Field.DataValueField(selectedValue) : "";
				pw.Hidden(Field.ID, value, a => a.DataHasClientState(ClientStateType.Value, ClientID, "selectedvalue"));
				if (!Field.Disabled)
				{
					cw.A(a => a.Data(Field.DataCollection).CallbackToCurrent(Context).AsDialog(OpenDialog), Resources.Get("Common.SelectObject_Field"));
					cw.Write("&nbsp;");
					if (!Field.PostOnClearEvent)
						cw.A(a => a.OnClick($"selectSingleObjectDialog.clear('{Field.ClientID}', true)"), Resources.Get("Common.Clear"));
					else
						cw.A(a => a.OnClickPostEvent(OnClear), Resources.Get("Common.Clear"));
					if (Field.FieldExtensions != null) { cw.Write("&nbsp;"); Field.FieldExtensions(cw); }
				}
				cw.Div(a => a.ID("selected"), () => {
					if (Field.Disabled && Field.TextWhenDisabled != null)
						Field.TextWhenDisabled.Invoke(cw);
					else if (selectedValue != null)
						cw.Write(Field.SelectedObjectTextField(selectedValue));
					else
						Field.TextWhenNothingSelected?.Invoke(cw);
				});
			});
		}

		public void OnClear(ApiResponse response)
		{
			response.AddWidget(Field.ID + "_selected", w => Field.TextWhenNothingSelected?.Invoke(w));
			response.AddClientAction("selectSingleObjectDialog", "clear", Field.ClientID);
			Field.OnClear(response);
		}

		public override void SubmitDialog(ApiResponse response)
		{
			var id = Context.GetArg<TRefKey>(Field.ID);
			var selectedValue = Field.GetObjectByID(id);

			response.WithNamesAndWritersFor(Field);
			response.ReplaceWidget("placeholder", w => {
				Render(w, selectedValue);
			});
			Field.OnChange(response, selectedValue);
		}
	}

	public class SelectMultipleObjectsDialog<TRef, TRefKey> : SelectObjectDialog<TRef, TRefKey, IEnumerable<TRef>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
	{
		public SelectMultipleObjectsField<TRef, TRefKey> Field { get; set; }
		protected override string FieldID => Field.ClientID;

		public override void OnInit()
		{
			DataCollection = Field.DataCollection;
			base.OnInit();
		}

		public override void List(LayoutWriter w)
		{
			w.Div(a => a.ID().DataCtrl("selectMultipleObjectsDialog"), () => {
				w.Div(a => a.Class("checkboxlist"), () => {
					foreach (var item in Field.DataProvider.GetData(Paging, FilterValue).Select(o => Field.GetListItem(o, FilterValue, HighlightSearchResults)))
					{
						w.Label(a => a.For("item" + item.Value), () => {
							w.CheckBox("item", false, a =>
								a.ID("item" + item.Value).Value(item.Value)
							);
							w.Write(item.Text);
						});
					}
				});
			});
			AfterList?.Invoke(w);
		}

		public override void ToolbarRight(MenuBuilder t)
		{
			t.Item(RenderPaging);
		}

		public override void RenderPaging(LayoutWriter w)
		{
			Paging.Render(w, Field.DataProvider.GetCount(FilterValue), a => a.RunEvent(RenderList));
		}

		public override void Render(LayoutWriter w, IEnumerable<TRef> selectedValues)
		{
			var cw = w.Clone(Field);
			var pw = w.Clone(Field.ParentElement);
			cw.Div(a => a.ID("placeholder"), () => {
				pw.Hidden(Field.ID, selectedValues?.Select(o => Field.DataValueField(o)).Join(","), a => a.DataHasClientState(ClientStateType.Array, ClientID, "selectedvalues"));
				if (!Field.Disabled)
				{
					cw.A(a => a.Data(Field.DataCollection).CallbackToCurrent(Context).AsDialog(OpenDialog), Resources.Get("Common.SelectObjects_Field"));
					cw.Write("&nbsp;");
					if (!Field.PostOnClearEvent)
						cw.A(a => a.OnClick($"selectMultipleObjectsDialog.clear('{Field.ClientID}', true)"), Resources.Get("Common.Clear"));
					else
						cw.A(a => a.OnClickPostEvent(OnClear), Resources.Get("Common.Clear"));
					if (Field.FieldExtensions != null) { cw.Write("&nbsp;"); Field.FieldExtensions(cw); }
				}
				cw.Div(a => a.ID("selected"), () => {
					if (Field.Disabled && Field.TextWhenDisabled != null)
						Field.TextWhenDisabled?.Invoke(cw);
					else if (selectedValues != null)
						cw.WriteLines(selectedValues.Select(o => Field.SelectedObjectTextField(o)));
					else
						Field.TextWhenNothingSelected?.Invoke(cw);
				});
			});
		}

		public void OnClear(ApiResponse response)
		{
			response.AddWidget(Field.ID + "_selected", w => Field.TextWhenNothingSelected?.Invoke(w));
			response.AddClientAction("selectMultipleObjectsDialog", "clear", Field.ClientID);
			Field.OnClear(response);
		}

		public override void SubmitDialog(ApiResponse response)
		{
			var ids = Context.GetListArg<TRefKey>(Field.ID);
			var selectedValues = Field.GetObjectsByIDs(ids);

			response.WithNamesAndWritersFor(Field);
			response.ReplaceWidget("placeholder", w => {
				Render(w, selectedValues);
			});
			Field.OnChange(response, selectedValues);
		}
	}
}
