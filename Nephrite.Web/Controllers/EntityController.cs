using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Web.Multilanguage;

namespace Nephrite.Web
{
	public abstract class EntityController<T, TKey> where T : IEntity, IWithKey<T, TKey>
	{
		public abstract void FilterTable(ref IQueryable<T> table);

		Dictionary<TKey, T> loaded = new Dictionary<TKey, T>();

		protected virtual IQueryable<T> GetTable()
		{
			IQueryable<T> table = A.Model.GetTable<T>();
			FilterTable(ref table);
			if (_tobj is IMultilanguage) table = table.Where(o => (o as IMultilanguage).LanguageCode == Language.Current.Code);
			return table;
		}
		protected T _tobj = default(T);

		public T Get(TKey id)
        {
			if (loaded.ContainsKey(id))
				return loaded[id];

			IQueryable<T> table = GetTable();
            var obj = table.Where(_tobj.KeySelector(id)).FirstOrDefault();

			loaded.Add(id, obj);
			
			return obj;
        }

        public IQueryable<T> GetList()
        {
			return GetTable();
        }
	}

	public interface IWithSeqNoController<T, TKey>
	{

	}

	public static class WithSeqNoController		
	{
		[Nephrite.Web.SPM.SpmActionName("Edit")]
		public static void MoveDown<T, TKey>(this IWithSeqNoController<T, TKey> controller, TKey id, string returnurl)
			where T : class, IEntity, IWithKey<T, TKey>, IWithSeqNo, new()
		{
			Nephrite.Web.Controllers.SimpleClassMover<T, TKey>.Down(A.Model.GetTable<T>(), id);
			A.Model.SubmitChanges();
			Url.Current.ReturnUrl.Go();
		}

		[Nephrite.Web.SPM.SpmActionName("Edit")]
		public static void MoveUp<T, TKey>(this IWithSeqNoController<T, TKey> controller, TKey id, string returnurl)
			where T : class, IEntity, IWithKey<T, TKey>, IWithSeqNo, new()
		{
			Nephrite.Web.Controllers.SimpleClassMover<T, TKey>.Up(A.Model.GetTable<T>(), id);
			A.Model.SubmitChanges();
			Url.Current.ReturnUrl.Go();
		}
	}
}