using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Std;

namespace Tango.UI.Controls
{
	public class SelectMultipleObjectsTreeDialog<TRef, TRefKey, TControl> :
		SelectMultipleObjectsDialog<TRef, TRefKey, AbstractSelectMultipleObjectsField<TRef, TRefKey>>
		where TRef : class, IWithTitle, IWithKey<TRefKey>
		where TControl : ViewPagePart, new()
	{
		public TControl Control { get; private set; }

		public override void OnInit()
		{
			base.OnInit();
			Control = CreateControl<TControl>("list");
		}

		public override void OpenDialog(ApiResponse response)
		{
			response.WithWritersFor(Control);
			response.AddWidget("body", w => w.Div(a => a.ID("container").Style("height:100%")));
			response.AddWidget("footer", Footer);
			response.WithNamesFor(Control);
			var rc = Control.GetContainer();
			rc.ToRemove.Add("contentheader");
			rc.Render(response);

			Control.OnLoad(response);

		}
	}
}
