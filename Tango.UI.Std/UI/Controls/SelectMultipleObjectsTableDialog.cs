using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Std;

namespace Tango.UI.Controls
{
	public class SelectMultipleObjectsTableDialog<TRef, TRefKey, TControl> :
		SelectMultipleObjectsDialog<TRef, TRefKey, AbstractSelectMultipleObjectsField<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		where TControl : default_list_rep<TRef>, new()
	{
		public TControl Control { get; private set; }
		public bool ClearSelectionOnSubmit { get; set; } = false;

		public override void OnInit()
		{
			base.OnInit();
			Control = CreateControl<TControl>("list");
			DialogOptions.ModalBodyPadding = false;
			DialogOptions.Height = DialogHeight.Height100;
			Control.DataCollection = Field.DataCollection;
			Control.Filter.DataCollection = Field.DataCollection;
		}

		public override void OpenDialog(ApiResponse response)
		{
			response.WithWritersFor(Control);
			response.AddWidget("title", Field.Title());
			response.AddWidget("body", w => w.Div(a => a.ID("container").Style("height:100%")));
			response.AddWidget("footer", Footer);
			response.WithNamesFor(Control);
			var rc = Control.GetContainer();
			rc.ToRemove.Add("contentheader");
			rc.Render(response);

			var ids = Context.GetListArg<TRefKey>(Field.ID);
			Control.OnLoad(response);
		}

		public override void Footer(LayoutWriter w)
		{
			if (Field.AllowSelectAll)
			{
				w.Button(a => a.OnClickPostEvent(SubmitDialogAll)
					.DataResult(1).DataRef($"{Field.ClientID}").Data(DataCollection)
					.Style("float:left"), "Выбрать все");
			}

			base.Footer(w);
		}

		public void SubmitDialogAll(ApiResponse response)
		{
			var selectedValues = Control.GetAllData();
			SubmitDialog(response, selectedValues);
		}

		public override void SubmitDialog(ApiResponse response)
		{
			var ids = Context.GetListArg<TRefKey>(Constants.SelectedValues);
			var selectedValues = Field.GetObjectsByIDs(ids);
			SubmitDialog(response, selectedValues);
		}

		void SubmitDialog(ApiResponse response, IEnumerable<TRef> selectedValues)
		{
			response.WithNamesAndWritersFor(Field.ParentElement);
			response.SetElementValue(Field.ID, selectedValues.Select(x => Field.DataValueField(x).ToString()).Join(","));
			response.WithNamesAndWritersFor(Field);
			response.ReplaceWidget("placeholder", w => {
				Render(w, selectedValues);
			});
			Field.OnChange(response, selectedValues);

			if (ClearSelectionOnSubmit)
				response.AddClientAction("listview", "clearselection", f => Control.ClientID);
		}
	}
}
