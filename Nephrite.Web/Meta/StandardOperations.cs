using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Meta.Forms;
using Nephrite.Web;
using Nephrite.Web.Controllers;


namespace Nephrite.Meta
{
	public delegate void OperationDelegate();
	public delegate void SingleObjectOperationDelegate<TKey>(TKey id);

	public class SingleObjectViewOperation<TDTO, TKey> : MetaOperation<TDTO, SingleObjectOperationDelegate<TKey>>
		where TDTO: IWithKey<TDTO, TKey>, new()
	{
		public Func<TKey, TDTO> GetDTO { get; set; }

		public SingleObjectViewOperation()
		{
			GetDTO = (id) =>
			{
				var obj = new TDTO();
				return A.Model.Filtered.GetTable<TDTO>().FirstOrDefault(obj.KeySelector(id));
			};

			Delegate = (id) =>
			{
				var viewModel = GetDTO(id);
				if (viewModel == null)
				{
					WebFormRenderer.RenderMessage("Объект не существует");
					return;
				}
				if (!CheckPredicate(viewModel))
				{
					WebFormRenderer.RenderMessage("Недостаточно полномочий для выполнения операции");
					return;
				}
				WebFormRenderer.RenderView(Parent.Name, ViewName, viewModel);
			};

			Invoke = () =>
			{
				TKey id = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(Url.Current.GetString("oid"));
				Delegate(id);
			};
		}
	}

	public class ObjectListViewOperation<TDTO> : MetaOperation<IQueryable<TDTO>, OperationDelegate>
	{
		public Func<IQueryable<TDTO>> GetDTO { get; set; }

		public ObjectListViewOperation()
		{
			GetDTO = () =>
			{
				return A.Model.Filtered.GetTable<TDTO>();
			};

			Delegate = () =>
			{
				var viewModel = GetDTO();
				if (!CheckPredicate(viewModel))
				{
					WebFormRenderer.RenderMessage("Недостаточно полномочий для выполнения операции");
					return;
				}
				WebFormRenderer.RenderView(Parent.Name, ViewName, viewModel);
			};

			Invoke = () => Delegate();
		}
	}

	public class CreateNewOperation<TDTO> : MetaOperation<TDTO, OperationDelegate>
		where TDTO : IEntity, new()
	{
		public CreateNewOperation()
		{
			Delegate = () =>
			{
				TDTO obj = new TDTO();
				A.Model.GetTable<TDTO>().InsertOnSubmit(obj);
				WebFormRenderer.RenderView(Parent.Name, "edit", obj);
			};

			Invoke = () => Delegate();
		}
	}

	public class CreateFromOperation<TDTO, TKey> : MetaOperation<TDTO, SingleObjectOperationDelegate<TKey>>
		where TDTO : IWithKey<TDTO, TKey>, ICloneable, new()
	{
		public Func<TKey, TDTO> GetDTO { get; set; }

		public CreateFromOperation()
		{
			GetDTO = (id) =>
			{
				var obj = new TDTO();
				return A.Model.Filtered.GetTable<TDTO>().FirstOrDefault(obj.KeySelector(id));
			};

			Delegate = (id) =>
			{
				var viewModel = GetDTO(id);
				if (viewModel == null)
				{
					WebFormRenderer.RenderMessage("Объект не существует");
					return;
				}
				if (!CheckPredicate(viewModel))
				{
					WebFormRenderer.RenderMessage("Недостаточно полномочий для выполнения операции");
					return;
				}
				var newobj = viewModel.Clone();
				WebFormRenderer.RenderView(Parent.Name, "edit", newobj);
			};

			Invoke = () =>
			{
				TKey id = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(Url.Current.GetString("oid"));
				Delegate(id);
			};
		}
	}

	public class MoveUpOperation<TDTO, TKey> : MetaOperation<TDTO, SingleObjectOperationDelegate<TKey>>
		where TDTO : IWithKey<TDTO, TKey>, IWithSeqNo, IEntity, new()
	{
		public Func<TKey, TDTO> GetDTO { get; set; }

		public MoveUpOperation()
		{
			GetDTO = (id) =>
			{
				var obj = new TDTO();
				return A.Model.Filtered.GetTable<TDTO>().FirstOrDefault(obj.KeySelector(id));
			};

			Delegate = (id) =>
			{
				var viewModel = GetDTO(id);
				if (viewModel == null)
				{
					WebFormRenderer.RenderMessage("Объект не существует");
					return;
				}
				if (!CheckPredicate(viewModel))
				{
					WebFormRenderer.RenderMessage("Недостаточно полномочий для выполнения операции");
					return;
				}
				SimpleClassMover<TDTO, TKey>.Up(A.Model.GetTable<TDTO>(), id);

				A.Model.SubmitChanges();
				Url.Current.ReturnUrl.Go();
			};

			Invoke = () =>
			{
				TKey id = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(Url.Current.GetString("oid"));
				Delegate(id);
			};
		}
	}

	public class MoveDownOperation<TDTO, TKey> : MetaOperation<TDTO, SingleObjectOperationDelegate<TKey>>
		where TDTO : IWithKey<TDTO, TKey>, IWithSeqNo, IEntity, new()
	{
		public Func<TKey, TDTO> GetDTO { get; set; }

		public MoveDownOperation()
		{
			GetDTO = (id) =>
			{
				var obj = new TDTO();
				return A.Model.Filtered.GetTable<TDTO>().FirstOrDefault(obj.KeySelector(id));
			};

			Delegate = (id) =>
			{
				var viewModel = GetDTO(id);
				if (viewModel == null)
				{
					WebFormRenderer.RenderMessage("Объект не существует");
					return;
				}
				if (!CheckPredicate(viewModel))
				{
					WebFormRenderer.RenderMessage("Недостаточно полномочий для выполнения операции");
					return;
				}
				SimpleClassMover<TDTO, TKey>.Down(A.Model.GetTable<TDTO>(), id);

				A.Model.SubmitChanges();
				Url.Current.ReturnUrl.Go();
			};

			Invoke = () =>
			{
				TKey id = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(Url.Current.GetString("oid"));
				Delegate(id);
			};
		}
	}


	public class ViewOperation : MetaOperation
	{
		public ViewOperation()
		{
			Invoke = () =>
			{
				if (Parent is MetaPackage)
					WebFormRenderer.RenderView(Parent.Name + "Pck", ViewName, null);
				else
					WebFormRenderer.RenderView(Parent.Name, ViewName, null);
			};
		}
	}
}