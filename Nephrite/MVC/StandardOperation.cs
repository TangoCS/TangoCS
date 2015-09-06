using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.AccessControl;
using Nephrite.Data;
using Nephrite.Html.Controls;
using Nephrite.Http;
using Nephrite.SettingsManager;

namespace Nephrite.MVC
{
	public class StandardOperation<TDTO>
		where TDTO : new()
	{
		protected string _controllerName;
		protected IDataContext _dataContext;
		protected IAccessControl _accessControl;

		public StandardOperation(
			string controllerName,
			IDataContext dataContext,
			IAccessControl accessControl)
		{
			_controllerName = controllerName;
			_dataContext = dataContext;
			_accessControl = accessControl;
		}

		public ActionResult CreateNewView(string viewName, string securableObjectKey = null)
		{
			if (securableObjectKey.IsEmpty()) securableObjectKey = _controllerName + "." + viewName;
			var res = Check(securableObjectKey, null);
			if (!res.Value) return new MessageResult(res.Message);

			TDTO viewModel = new TDTO();
			_dataContext.GetTable<TDTO>().InsertOnSubmit(viewModel);

			return new ViewResult(_controllerName, viewName, viewModel);
		}

		public ActionResult CreateFromView<TKey>(string viewName, TKey id, string securableObjectKey = null)
		{
			return CreateFromView(viewName, Get<TKey>(id), securableObjectKey);
		}

		public ActionResult CreateFromView(string viewName, TDTO viewModel, string securableObjectKey = null)
		{
			if (viewModel == null)
			{
				return new MessageResult("Объект не существует");
			}

			if (securableObjectKey.IsEmpty()) securableObjectKey = _controllerName + "." + viewName;
			var res = Check(securableObjectKey, viewModel);
			if (!res.Value) return new MessageResult(res.Message);

			if (!(viewModel is ICloneable))
			{
				return new MessageResult("Объект не реализует интерфейс ICloneable");
			}
			var newobj = (viewModel as ICloneable).Clone();
			return new ViewResult(_controllerName, viewName, newobj);
		}

		public ActionResult ListView(string viewName, IQueryable<TDTO> viewModel = null, string securableObjectKey = null)
		{
			if (securableObjectKey.IsEmpty()) securableObjectKey = _controllerName + "." + viewName;
			var res = Check(securableObjectKey, null);
			if (!res.Value) return new MessageResult(res.Message);

			if (viewModel == null)
			{
				viewModel = _dataContext.GetTable<TDTO>().Filtered();
			}
			return new ViewResult(_controllerName, viewName, viewModel);
		}

		public ActionResult SingleObjectView<TKey>(string viewName, TKey id, string securableObjectKey = null)
		{
			return SingleObjectView(viewName, Get<TKey>(id), securableObjectKey);
		}

		public ActionResult SingleObjectView(string viewName, TDTO viewModel, string securableObjectKey = null)
		{
			if (viewModel == null)
			{
				return new MessageResult("Объект не существует");
			}

			if (securableObjectKey.IsEmpty()) securableObjectKey = _controllerName + "." + viewName;
			var res = Check(securableObjectKey, viewModel);
			if (!res.Value) return new MessageResult(res.Message);


			return new ViewResult(_controllerName, viewName, viewModel);
		}


		public ActionResult MoveUp<TKey>(TKey id)
		{
			var viewModel = Get<TKey>(id);

			if (viewModel == null)
			{
				return new MessageResult("Объект не существует");
			}

			var collection = _dataContext.GetTable<TDTO>().Filtered();
			var withKey = default(TDTO) as IWithKey<TDTO, TKey>;
			var b = collection.SingleOrDefault<TDTO>(withKey.KeySelector(id)) as IWithSeqNo;

			IWithSeqNo b1 = (from br in collection.Cast<IWithSeqNo>() where br.SeqNo < b.SeqNo orderby br.SeqNo descending select br).FirstOrDefault() as IWithSeqNo;
			if (b1 != null)
			{
				int s1 = b1.SeqNo;
				b1.SeqNo = b.SeqNo;
				b.SeqNo = s1;
			}

			_dataContext.SubmitChanges();
			return new RedirectBackResult();
		}

		public ActionResult MoveDown<TKey>(TKey id)
		{
			var viewModel = Get<TKey>(id);
			if (viewModel == null)
			{
				return new MessageResult("Объект не существует");
			}

			var collection = _dataContext.GetTable<TDTO>().Filtered();
			var withKey = default(TDTO) as IWithKey<TDTO, TKey>;
			var b = collection.SingleOrDefault<TDTO>(withKey.KeySelector(id)) as IWithSeqNo;

			IWithSeqNo b1 = (from br in collection.Cast<IWithSeqNo>() where br.SeqNo > b.SeqNo orderby br.SeqNo select br).FirstOrDefault() as IWithSeqNo;
			if (b1 != null)
			{
				int s1 = b1.SeqNo;
				b1.SeqNo = b.SeqNo;
				b.SeqNo = s1;
			}

			_dataContext.SubmitChanges();
			return new RedirectBackResult();
		}

		public TDTO Get<TKey>(TKey id)
		{
			var viewModel = new TDTO();
			if (!(viewModel is IWithKey<TDTO, TKey>))
			{
				throw new Exception("Объект не реализует интерфейс IWithKey");
			}
			var viewModelWithKey = viewModel as IWithKey<TDTO, TKey>;

			return _dataContext.GetTable<TDTO>().Filtered().FirstOrDefault(viewModelWithKey.KeySelector(id));
		}

		BoolResult Check(string securableObjectKey, object viewModel)
		{
			return _accessControl.CheckPredicate(securableObjectKey, viewModel);
			//if (!res.Value)
			//{
			//	return new Tuple<bool, ActionResult>(false, new MessageResult("Недостаточно полномочий для выполнения операции" + (res.Message.IsEmpty() ? "" : (": " + res.Message))));
			//}

			//if (!res.Value && res.Code == CheckWithPredicateResultCode.Subject)
			//{
			//	return new Tuple<bool, ActionResult>(false, new RedirectToLoginResult());
			//}

			//return new Tuple<bool, ActionResult>(true, null);
		}
	}
}
