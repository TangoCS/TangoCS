﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Data;
using Tango.Html;

namespace Tango.UI.Std
{
	public abstract class default_undelete<T, TKey> : ViewPagePart
		where T : class, IWithKey<T, TKey>, IWithLogicalDelete, new()
	{
		[Inject]
		protected IDatabase Database { get; set; }

		public override ViewContainer GetContainer() => new EditEntityContainer();

		public override void OnLoad(ApiResponse response)
		{
			var sel = GetArg(Constants.SelectedValues);
			if (sel == null) sel = GetArg(Constants.Id);
			var cnt = sel?.Split(',').Count() ?? 0;
			var bulk = cnt > 1;

			var confirm = bulk ?
				string.Format(Resources.Get("Common.Undelete.Bulk.Confirm"), cnt) :
				Resources.Get("Common.Undelete.Confirm");

			response.AddWidget("form", w => {
				w.P(confirm);
				if (cnt > 0) w.Hidden(Constants.SelectedValues, sel);
				w.FormValidationBlock();
			});

			var title = Resources.Get(bulk ? "Common.Undelete.Bulk.Title" : "Common.Undelete.Title");
			response.AddWidget("contenttitle", title);
			response.AddWidget("#title", title);

			response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, w => w.ButtonsBar(() => {
				w.ButtonsBarRight(() => {
					w.SubmitContinueButton();
					w.BackButton();
				});
			}));
		}

		protected virtual void Undelete(IEnumerable<TKey> ids)
		{
			Database.Repository<T>().Update(u => u.Set(o => o.IsDeleted, false), ids);
		}


		public void OnSubmit(ApiResponse response)
		{
			var sel = GetPostedList<TKey>(Constants.SelectedValues);
			using (var tran = Database.BeginTransaction())
			{
				Undelete(sel);
				tran.Commit();
			}

			response.RedirectBack(Context, 1);
		}
	}
}