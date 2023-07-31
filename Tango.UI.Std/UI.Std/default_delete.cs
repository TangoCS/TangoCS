using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Logic;
using Tango.Data;
using Tango.Html;

namespace Tango.UI.Std
{
	public abstract class default_delete<T, TKey> : ViewPagePart
		where T : class, IWithKey<T, TKey>, new()
	{
		[Inject]
		protected IDatabase Database { get; set; }

		[Inject]
		protected IEntityAudit EntityAudit { get; set; }
		
		protected IRepository<T> Repository { get; set; }
		
		protected virtual string FormTitle { get; }

		public override void OnInit()
		{
			base.OnInit();
			Repository = Context.RequestServices.GetService(typeof(IRepository<T>)) as IRepository<T> ?? Database.Repository<T>();
		}

		public override ViewContainer GetContainer() => new EditEntityContainer();
		
		public virtual void RenderConfirm(LayoutWriter w, int cnt)
		{
			var confirm = cnt > 1 ?
				string.Format(Resources.Get("Common.Delete.Bulk.Confirm"), cnt) :
				Resources.Get("Common.Delete.Confirm");
			
			w.P(confirm);
		}

		public override void OnLoad(ApiResponse response)
		{
			var sel = GetArg(Constants.SelectedValues);
			if (sel == null) sel = GetArg(Constants.Id);
			var cnt = sel?.Split(',').Count() ?? 0;
			var bulk = cnt > 1;

			response.AddWidget("form", w => {
				RenderConfirm(w, cnt);
				if (cnt > 0) w.Hidden(Constants.SelectedValues, sel);
				w.FormValidationBlock();
			});

			var title = FormTitle ?? Resources.Get(bulk ? "Common.Delete.Bulk.Title" : "Common.Delete.Title");
			
			response.AddWidget("contenttitle", title);
			if (!IsSubView && ParentElement == null)
				response.AddWidget("#title", title);

			response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, ButtonsBar);
		}

		protected virtual void ButtonsBar(LayoutWriter w)
		{
			w.ButtonsBar_delete(this);
		}

		protected virtual void Delete(IEnumerable<TKey> ids)
		{
			if (typeof(IWithLogicalDelete).IsAssignableFrom(typeof(T)))
				Repository.Update(u => u.Set(o => (o as IWithLogicalDelete).IsDeleted, true), ids);
			else
				Repository.Delete(ids);
		}

		protected virtual void BeforeDeleteEntity(IEnumerable<TKey> ids) { }
		protected virtual void AfterDeleteEntity(IEnumerable<TKey> ids) { }

		protected virtual void ProcessFormData(IEnumerable<TKey> ids, ValidationMessageCollection val) { }

		protected (bool Result, ValidationMessageCollection Messages) ProcessSubmit(IEnumerable<TKey> ids)
		{
			ValidationMessageCollection m = new ValidationMessageCollection();
			ProcessFormData(ids, m);
			if (m.HasItems()) return (false, m);
			return (true, null);
		}

		public virtual void OnSubmit(ApiResponse response)
		{
			var sel = Context.GetListArg<TKey>(Constants.SelectedValues);

			if (EntityAudit != null)
			{
				var list = new List<ObjectChange>();
				foreach (var id in sel)
				{
					var oc = ObjectChange.Delete<T>(id);
					list.Add(oc);

					if (typeof(T).IsAssignableTo(typeof(IWithTitle)) || typeof(T).IsAssignableTo(typeof(IWithName)))
					{
						T obj = Repository.GetById(id);  //CommonLogic.GetFiltered<T, TKey>(DataContext, id);
						var ot = obj as IWithTitle;
						if (ot != null) oc.Title = () => ot.Title;
						else
						{
							var onm = obj as IWithName;
							if (onm != null) oc.Title = () => onm.Name;
						}
					}
				}
				if (sel.Count == 1)
					EntityAudit.AddChanges(list[0]);
				else if (sel.Count > 1)
				{
					var ocp = ObjectChange.BulkDelete<T>();
					EntityAudit.AddChanges(ocp, secondaryObjects: list);
				}
			}

			var res = ProcessSubmit(sel);
			if (!res.Result)
			{
				RenderValidation(response, res.Messages);
				return;
			}

			using (var tran = Database.BeginTransaction())
			{
				BeforeDeleteEntity(sel);
				Delete(sel);
				AfterDeleteEntity(sel);
				EntityAudit?.WriteObjectChange();
				tran.Commit();
			}

			AfterSubmit(response);
		}

		protected virtual void AfterSubmit(ApiResponse response)
		{
			response.RedirectBack(Context, 1, !IsSubView);
		}

		protected virtual void RenderValidation(ApiResponse response, ValidationMessageCollection m)
		{
			response.WithNamesFor(this).AddWidget("validation", w => w.ValidationBlock(m));
			response.Success = false;
		}
	}
}
