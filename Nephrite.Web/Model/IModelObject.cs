﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nephrite.Meta;

namespace Nephrite.Web
{
    public interface IModelObject
    {
		string Title { get; }
        int ObjectID { get; }
		Guid ObjectGUID { get; }
		MetaClass MetaClass { get; }
    }



	//public interface IChildObject : IModelObject
	//{
	//	IModelObject ParentObject { get; set; }
	//	void SetParent(int parentID);
	//	void SetParent(Guid parentGUID);
	//	Expression<Func<T, bool>> FilterByParentID<T>(int id) where T : IModelObject;
	//	Expression<Func<T, bool>> FilterByParentGUID<T>(Guid guid) where T : IModelObject;
	//}

	//public interface IActiveFlag
	//{
	//	bool IsActive { get; set; }
	//}

	//public interface IDeletedFlag
	//{
	//	bool IsDeleted { get; set; }
	//}

	//public interface ILastModifiedDate
	//{
	//	DateTime LastModifiedDate { get; set; }
	//}

	//public interface ILastModifiedStamp
	//{
	//	DateTime? LastModifiedDate { get; set; }
	//	string LastModifiedUser { get; set; }
	//}
}