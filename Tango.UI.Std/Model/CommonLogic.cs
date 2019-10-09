using System;
using System.Linq;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Logic
{
	public static class CommonLogic
	{
		public static T New<T>(this IDataContext dc)
			where T: class, new()
		{
			var obj = new T();
			dc.InsertOnSubmit(obj);
			return obj;
		}

		public static T NewFrom<T, TKey>(this IDataContext dc, TKey id)
			where T : class, ICloneable, IWithKey<T, TKey>, new()
		{
			var obj = GetFiltered<T, TKey>(dc, id);
			return obj.Clone() as T;
		}

		public static T GetFiltered<T, TKey>(this IDataContext dc, TKey id)
			where T : class, IWithKey<T, TKey>, new()
		{
			var obj = new T();
			return dc.GetTable<T>().Filtered().FirstOrDefault(obj.KeySelector(id));
		}

		public static void Delete<T>(this IDataContext dc, T obj)
			where T : class, new()
		{
			if (obj is IWithLogicalDelete)
			{
				(obj as IWithLogicalDelete).IsDeleted = true;
			}
			else
			{
				dc.DeleteOnSubmit(obj);
			}
			dc.SubmitChanges();
		}

        public static void UnDelete<T>(this IDataContext dc, T obj)
            where T : class, new()
        {
            if (obj is IWithLogicalDelete)
            {
                (obj as IWithLogicalDelete).IsDeleted = false;
                dc.SubmitChanges();
            }
        }

        public static void MoveUp<T, TKey>(IDataContext dc, TKey id, Expression<Func<T, bool>> filter = null)
			where T : class, IWithKey<T, TKey>, IWithSeqNo, new()
		{
			var obj = GetFiltered<T, TKey>(dc, id);

			var collection = dc.GetTable<T>().Filtered();
			if (filter != null) collection = collection.Where(filter);
			var upperObj = (from br in collection where br.SeqNo < obj.SeqNo orderby br.SeqNo descending select br).FirstOrDefault();
			if (upperObj != null)
			{
				int s1 = upperObj.SeqNo;
				upperObj.SeqNo = obj.SeqNo;
				obj.SeqNo = s1;
			}
		}

		public static void MoveDown<T, TKey>(IDataContext dc, TKey id, Expression<Func<T, bool>> filter = null)
			where T : class, IWithKey<T, TKey>, IWithSeqNo, new()
		{
			var obj = GetFiltered<T, TKey>(dc, id);

			var collection = dc.GetTable<T>().Filtered();
			if (filter != null) collection = collection.Where(filter);
			var lowerObj = (from br in collection where br.SeqNo > obj.SeqNo orderby br.SeqNo select br).FirstOrDefault();
			if (lowerObj != null)
			{
				int s1 = lowerObj.SeqNo;
				lowerObj.SeqNo = obj.SeqNo;
				obj.SeqNo = s1;
			}
		}
	}
}
