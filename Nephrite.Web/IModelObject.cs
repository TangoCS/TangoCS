using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nephrite.Web
{
    public interface IModelObject
    {
		string Title { get; }
        int ObjectID { get; }
		Guid ObjectGUID { get; }
        string GetClassName();
    }

	public interface IMovableObject
	{
		int SeqNo { get; set; }
	}

    public interface IChildObject : IModelObject
    {
        IModelObject ParentObject { get; set; }
        void SetParent(int parentID);
		void SetParent(Guid parentGUID);
        Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject;
		Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject;
    }

	public interface IActiveFlag
	{
		bool IsActive { get; set; }
	}

    public interface IDeletedFlag
    {
        bool IsDeleted { get; set; }
    }

    public interface ILastModifiedDate
    {
        DateTime LastModifiedDate { get; set; }
    }

	public interface ILastModifiedStamp
	{
		DateTime? LastModifiedDate { get; set; }
		string LastModifiedUser { get; set; }
	}

	public static class ModelHelper
	{
		public static Type GetPrimaryKeyType(this Type t)
		{
			return t.GetProperties().Where(o => o.GetCustomAttributes(typeof(global::System.Data.Linq.Mapping.ColumnAttribute), false).
				Cast<global::System.Data.Linq.Mapping.ColumnAttribute>().Any(o1 => o1.IsPrimaryKey)).First().PropertyType;
		}
	}
}
