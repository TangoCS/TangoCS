using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Data;
using Tango.Html;

namespace Tango.UI.Std
{
	public abstract class default_delete<T, TKey> : ViewPagePart
		where T : class, IWithKey<TKey>
	{
		[Inject]
		protected IDatabase Database { get; set; }

		[Inject]
		protected IEntityAudit EntityAudit { get; set; }

		public override ViewContainer GetContainer() => new EditEntityContainer();

		public override void OnLoad(ApiResponse response)
		{
			var sel = GetArg(Constants.SelectedValues);
			if (sel == null) sel = GetArg(Constants.Id);
			var cnt = sel?.Split(',').Count() ?? 0;
			var bulk = cnt > 1;

			var confirm = bulk ?
				string.Format(Resources.Get("Common.Delete.Bulk.Confirm"), cnt) :
				Resources.Get("Common.Delete.Confirm");

			response.AddWidget("form", w => {
				w.P(confirm);
				if (cnt > 0) w.Hidden(Constants.SelectedValues, sel);
				w.FormValidationBlock();
			});

			var title = Resources.Get(bulk ? "Common.Delete.Bulk.Title" : "Common.Delete.Title");
			response.AddWidget("contenttitle", title);
			response.AddWidget("#title", title);

			response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, w => {
				w.ButtonsBar(() => {
					w.ButtonsBarRight(() => {
						w.SubmitDeleteButton();
						w.BackButton();
					});
				});
			});
		}

		protected virtual void Delete(IEnumerable<TKey> ids)
		{
			foreach (var id in ids)
			{
				// TODO переделать метод, чтобы при удалении не требовался объект целиком
				T obj = Database.Repository<T>().GetById(id);
				EntityAudit?.WriteObjectChange<T, TKey>(obj, EntityAuditAction.Delete, null);
			}
			if (typeof(IWithLogicalDelete).IsAssignableFrom(typeof(T)))
				Database.Repository<T>().Update(u => u.Set(o => (o as IWithLogicalDelete).IsDeleted, true), ids);
			else
				Database.Repository<T>().Delete(ids);
		}

		protected virtual void BeforeDelete(IEnumerable<TKey> ids) { }
		protected virtual void AfterDelete(IEnumerable<TKey> ids) { }

		protected virtual void ProcessFormData(IEnumerable<TKey> ids, ValidationMessageCollection val) { }

		protected (bool Result, ValidationMessageCollection Messages) ProcessSubmit(IEnumerable<TKey> ids)
		{
			ValidationMessageCollection m = new ValidationMessageCollection();
			ProcessFormData(ids, m);
			if (m.HasItems()) return (false, m);
			return (true, null);
		}

		public void OnSubmit(ApiResponse response)
		{
			var sel = GetPostedList<TKey>(Constants.SelectedValues);

			var res = ProcessSubmit(sel);
			if (!res.Result)
			{
				RenderValidation(response, res.Messages);
				return;
			}

			BeforeDelete(sel);
			using (var tran = Database.BeginTransaction())
			{
				Delete(sel);
				tran.Commit();
			}
			AfterDelete(sel);

			response.RedirectBack(Context, 1);
		}

		protected virtual void RenderValidation(ApiResponse response, ValidationMessageCollection m)
		{
			response.WithNamesFor(this).AddWidget("validation", w => w.ValidationBlock(m));
			response.Success(false);
		}
	}
}
