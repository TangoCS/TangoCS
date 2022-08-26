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
		public string Width { get; set; } = "700px";
		public Paging Paging { get; set; }
		public TField Field { get; set; }

		public override void OnInit()
		{
			Paging = CreateControl<Paging>("page", p => {
				p.PageIndex = Context.GetIntArg(p.ClientID, 1);
				p.PageSize = 20;
			});
			DataCollection = Field.DataCollection;
			DataCollection.Ref(Field.GetClientID("filter"));
		}

		public virtual void OpenDialog(ApiResponse response)
		{
			var q = Field.DataQuery(Paging);
			var data = Field.DataProvider.MaterializeList(q);

			response.AddWidget("contenttitle", Field.Title());
			response.AddWidget("contentbody", w => List(w, data));
			response.AddWidget("contenttoolbar", w => w.Toolbar(ToolbarLeft, ToolbarRight));
			response.AddWidget("buttonsbar", Footer);
		}

		public virtual void ToolbarLeft(MenuBuilder t)
		{
			t.Item(w => w.TextBox(Field.FilterFieldName, Field.FilterValue, a =>
				a.Class("selectdialog_filter").Data(DataCollection).Placeholder("Поиск").Autofocus(true).Autocomplete(false)
			));
		}

		public virtual void ToolbarRight(MenuBuilder t)
		{
			t.Item(RenderPaging);
		}

		public virtual void RenderList(ApiResponse response)
		{
			var q = Field.DataQuery(Paging);
			var data = Field.DataProvider.MaterializeList(q);

			// TODO решить проблему префиксов при name = prefix
			response.AddWidget("body", w => List(w, data));
			response.WithNamesAndWritersFor(this);
			response.AddWidget(Paging.ID, RenderPaging);
		}

		public abstract void SubmitDialog(ApiResponse response);
		public abstract void List(LayoutWriter w, IEnumerable<TRef> data);
		public Action<LayoutWriter> AfterList { get; set; }

		public virtual void Footer(LayoutWriter w)
		{
			w.Button(a => a.DataResult(1).DataParm(Field.SubmitDataParms).OnClickPostEvent(SubmitDialog).DataRef("#" + Field.ClientID), "OK");
			//w.SubmitButton();
			w.Write("&nbsp;");
			w.BackButton();
		}
		public abstract void Render(LayoutWriter w, TValue selectedValue);
		public abstract void RenderSelected(LayoutWriter w, TValue selectedValue);
		public virtual void RenderPaging(LayoutWriter w)
		{
			var q = Field.ItemsCountQuery();
			Paging.Render(w, Field.DataProvider.MaterializeCount(q), a => a.PostEvent(RenderList));
		}
	}

	public class SelectSingleObjectDialog<TRef, TRefKey> : SelectObjectDialog<TRef, TRefKey, TRef, ISelectSingleObjectField<TRef, TRefKey, TRef>>
		where TRef : class, IWithKey<TRef, TRefKey>, new()
	{
		public override void List(LayoutWriter w, IEnumerable<TRef> data)
		{
			w.Div(a => a.ID().DataCtrl("selectSingleObjectDialog"), () => {
				w.Div(a => a.Class("radiobuttonlist"), () => {
					foreach (var o in data)
					{
						var value = Field.DataValueField(o);
						w.Label(a => a.For("item" + value), () => {
							w.RadioButton("item", "item" + value, value);
							Field.DataRow(w, o);
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
				RenderSelected(cw, selectedValue);
			});
		}

		public override void RenderSelected(LayoutWriter cw, TRef selectedValue)
		{
			cw.Div(a => a.ID("selected"), () => {
				if (Field.Disabled && Field.TextWhenDisabled != null)
					Field.TextWhenDisabled.Invoke(cw);
				else if (selectedValue != null)
					cw.Write(Field.SelectedObjectTextField(selectedValue));
				else
					Field.TextWhenNothingSelected?.Invoke(cw);
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

	public class SelectMultipleObjectsDialog<TRef, TRefKey, TField> : 
		SelectObjectDialog<TRef, TRefKey, IEnumerable<TRef>, TField>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		where TField : AbstractSelectMultipleObjectsField<TRef, TRefKey>
	{
		public string OpenDialogLinkTitle { get; set; } = null;
		public OpenDialogLinkStyle OpenDialogLinkStyle { get; set; } = OpenDialogLinkStyle.Link;

		public bool ShowSelectedValues { get; set; } = true;
		public bool ShowClearAction { get; set; } = true;
		public DialogOptions DialogOptions { get; set; } = new DialogOptions();

		public override void List(LayoutWriter w, IEnumerable<TRef> data)
		{
			w.Div(a => a.ID().DataCtrl("selectMultipleObjectsDialog"), () => {
				w.Div(a => a.Class("checkboxlist"), () => {
					foreach (var o in data)
					{
						var value = Field.DataValueField(o);
						w.Label(a => a.For("item" + value), () => {
							w.CheckBox("item", false, a => a.ID("item" + value).Value(value));
							Field.DataRow(w, o);
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
				pw.Hidden(Field.ID, selectedValues?.Select(o => Field.DataValueField(o)).Join(","), 
					a => a.DataHasClientState(ClientStateType.Array, ClientID, "selectedvalues"));
				if (!Field.Disabled)
				{
					cw.A(a => {
						if (OpenDialogLinkStyle == OpenDialogLinkStyle.Button)
							a.Class("actionbtn");
						a.Data(Field.DataCollection);
						if (Field.DoCallbackToCurrent)
							a.CallbackToCurrent(Context);
						a.AsDialogPost(OpenDialog, Field.ClientID);
						if (DialogOptions != null)
							foreach (var parm in DialogOptions.ToParms())
								a.DataParm("c-" + parm.Key, parm.Value);
					}, OpenDialogLinkTitle ?? Resources.Get("Common.SelectObjects_Field"));

					if (ShowClearAction)
					{
						cw.Write("&nbsp;");
						if (!Field.PostOnClearEvent)
							cw.A(a => a.OnClick(ClearButtonScript), Resources.Get("Common.Clear"));
						else
							cw.A(a => a.OnClickPostEvent(OnClear), Resources.Get("Common.Clear"));
					}

					if (Field.FieldExtensions != null) 
					{ 
						cw.Write("&nbsp;"); 
						Field.FieldExtensions(cw); 
					}
				}
				RenderSelected(cw, selectedValues);
			});
		}

		public override void RenderSelected(LayoutWriter cw, IEnumerable<TRef> selectedValues)
		{
			if (!ShowSelectedValues) return;

			cw.Div(a => a.ID("selected"), () => {
				if (Field.Disabled && Field.TextWhenDisabled != null)
					Field.TextWhenDisabled?.Invoke(cw);
				else if (selectedValues != null && selectedValues.Count() > 0)
					cw.WriteLines(selectedValues.Select(o => Field.SelectedObjectTextField(o)));
				else
					Field.TextWhenNothingSelected?.Invoke(cw);
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

		protected virtual string ClearButtonScript => $"selectMultipleObjectsDialog.clear('{Field.ClientID}', true)";
	}

	public class SelectMultipleObjectsDialog<TRef, TRefKey> :
		SelectMultipleObjectsDialog<TRef, TRefKey, AbstractSelectMultipleObjectsField<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
	{
	}

	public enum OpenDialogLinkStyle
	{
		Link, Button
	}
}
