using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public abstract class SelectObjectDialog<TRef, TRefKey, TValue, TField> : ViewComponent
		where TRef : class, IWithKey<TRefKey>
		where TField : ISelectObjectField<TRef>
	{
		string _filterValue = null;
		public string FilterValue {
			get
			{
				if (_filterValue == null) _filterValue = Context.GetArg(FilterFieldName);
				return _filterValue;
			}
		}

		public string Width { get; set; } = "700px";
		public string Title { get; set; }
		public bool HighlightSearchResults { get; set; } = false;
		protected Paging Paging { get; set; }

		public TField Field { get; set; }

		public string FilterFieldName { get; set; } = "filter";

		public override void OnInit()
		{
			Paging = CreateControl<Paging>("page", p => {
				p.PageIndex = Context.GetIntArg(p.ClientID, 1);
				p.PageSize = 20;
			});
			DataCollection = Field.DataCollection;
			DataCollection.Ref(Field.GetClientID("filter"));
			Title = Resources.Get(typeof(TRef).GetResourceType().FullName + "-pl");
		}

		public void OpenDialog(ApiResponse response)
		{
			var data = Field.DataProvider.GetData(Paging, FilterValue);
			if (data.Count() == 1)
				RenderSingleObjectFound(response, data.First());
			else
				RenderMultipleObjectsFound(response, data);

		}

		protected virtual void RenderMultipleObjectsFound(ApiResponse response, IEnumerable<TRef> data)
		{
			response.AddWidget("contenttitle", Title);
			response.AddWidget("contentbody", w => List(w, data));
			response.AddWidget("contenttoolbar", w => w.Toolbar(ToolbarLeft, ToolbarRight));
			response.AddWidget("buttonsbar", Footer);
		}
		protected virtual void RenderSingleObjectFound(ApiResponse response, TRef data)
		{
			RenderMultipleObjectsFound(response, new List<TRef> { data });
		}

		public virtual void ToolbarLeft(MenuBuilder t)
		{
			t.Item(w => w.TextBox(FilterFieldName, FilterValue, a =>
				a.Class("selectdialog_filter").Data(DataCollection).Placeholder("Поиск").Autofocus(true).Autocomplete(false)
			));
		}

		public virtual void ToolbarRight(MenuBuilder t)
		{
			t.Item(RenderPaging);
		}

		public virtual void RenderList(ApiResponse response)
		{
			var data = Field.DataProvider.GetData(Paging, FilterValue);

			// TODO решить проблему префиксов при name = prefix
			response.WithNamesAndWritersFor(this);
			response.AddWidget(null, w => List(w, data));
			response.AddWidget(Paging.ID, RenderPaging);
		}

		public abstract void SubmitDialog(ApiResponse response);
		public abstract void List(LayoutWriter w, IEnumerable<TRef> data);
		public Action<LayoutWriter> AfterList { get; set; }

		public virtual void Footer(LayoutWriter w)
		{
			w.Button(a => a.DataResultPostponed(1).OnClickPostEvent(SubmitDialog).DataRef("#" + Field.ClientID), "OK");
			//w.SubmitButton();
			w.Write("&nbsp;");
			w.BackButton();
		}
		public abstract void Render(LayoutWriter w, TValue selectedValue);
		public virtual void RenderPaging(LayoutWriter w)
		{
			Paging.Render(w, Field.DataProvider.GetCount(FilterValue), a => a.RunEvent(RenderList));
		}
	}

	public class SelectSingleObjectDialog<TRef, TRefKey> : SelectObjectDialog<TRef, TRefKey, TRef, ISelectSingleObjectField<TRef, TRefKey, TRef>>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		public override void List(LayoutWriter w, IEnumerable<TRef> data)
		{
			w.Div(a => a.ID().DataCtrl("selectSingleObjectDialog"), () => {
				w.Div(a => a.Class("radiobuttonlist"), () => {
					foreach (var item in data.Select(o => Field.GetListItem(o, FilterValue, HighlightSearchResults)))
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

		public override void Render(LayoutWriter w, TRef selectedValue)
		{
			var cw = w.Clone(Field);
			var pw = w.Clone(Field.ParentElement);
			cw.Div(a => a.ID("placeholder"), () => {
				var value = selectedValue != null ? Field.DataValueField(selectedValue) : "";
				pw.Hidden(Field.ID, value, a => a.DataHasClientState(ClientStateType.Value, ClientID, "selectedvalue"));
				if (!Field.Disabled)
				{
					cw.A(a => a.Data(Field.DataCollection).CallbackToCurrent(Context).AsDialog(OpenDialog, Field.ClientID), Resources.Get("Common.SelectObject_Field"));
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

	public abstract class SelectMultipleObjectsDialog<TRef, TRefKey, TField> : SelectObjectDialog<TRef, TRefKey, IEnumerable<TRef>, TField>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		where TField : SelectObjectField<TRef, TRefKey, IEnumerable<TRef>>
	{
		public override void List(LayoutWriter w, IEnumerable<TRef> data)
		{
			w.Div(a => a.ID().DataCtrl("selectMultipleObjectsDialog"), () => {
				w.Div(a => a.Class("checkboxlist"), () => {
					foreach (var item in data.Select(o => Field.GetListItem(o, FilterValue, HighlightSearchResults)))
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

		public override void Render(LayoutWriter w, IEnumerable<TRef> selectedValues)
		{
			var cw = w.Clone(Field);
			var pw = w.Clone(Field.ParentElement);
			cw.Div(a => a.ID("placeholder"), () => {
				pw.Hidden(Field.ID, selectedValues?.Select(o => Field.DataValueField(o)).Join(","), a => a.DataHasClientState(ClientStateType.Array, ClientID, "selectedvalues"));
				if (!Field.Disabled)
				{
					cw.A(a => a.Data(Field.DataCollection).CallbackToCurrent(Context).AsDialog(OpenDialog, Field.ClientID), Resources.Get("Common.SelectObjects_Field"));
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
					else if (selectedValues != null && selectedValues.Count() > 0)
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
	}

	public class SelectMultipleObjectsDialog<TRef, TRefKey> : SelectMultipleObjectsDialog<TRef, TRefKey, SelectMultipleObjectsField<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
	{
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
