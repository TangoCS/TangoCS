using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public class SelectMultipleObjectsTreeDialog<TRef, TRefKey> : 
		SelectObjectDialog<TRef, TRefKey, IEnumerable<TRef>, AbstractSelectMultipleObjectsField<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		//where TField : AbstractSelectMultipleObjectsField<TRef, TRefKey>
	{
		public string OpenDialogLinkTitle { get; set; } = null;

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
						a.Data(Field.DataCollection);
						if (Field.DoCallbackToCurrent)
							a.CallbackToCurrent(Context);

						a.AsDialog(OpenDialog, Field.ClientID);
					}, OpenDialogLinkTitle ?? Resources.Get("Common.SelectObjects_Field"));
					cw.Write("&nbsp;");
					if (!Field.PostOnClearEvent)
						cw.A(a => a.OnClick($"selectMultipleObjectsDialog.clear('{Field.ClientID}', true)"),
							Resources.Get("Common.Clear"));
					else
						cw.A(a => a.OnClickPostEvent(OnClear), Resources.Get("Common.Clear"));
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
	}
}
