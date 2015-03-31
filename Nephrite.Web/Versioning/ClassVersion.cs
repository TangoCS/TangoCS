using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Versioning
{
	//partial class ClassVersion<T> where T : IClassVersion
	//{
	//	T _obj = default(T);

	//	public ClassVersion(T obj)
	//	{
	//		_obj = obj;
	//	}

	//	public bool IsDraft
	//	{
	//		get
	//		{
	//			return Versions.IndexOf(_obj) == 0 && !_obj.IsCurrent;
	//		}
	//	}

	//	public bool IsOld
	//	{
	//		get
	//		{
	//			var current = Versions.SingleOrDefault(o => o.IsCurrent);
	//			if (current == null)
	//				return false;
	//			return Versions.IndexOf(_obj) > Versions.IndexOf(current);
	//		}
	//	}

	//	public string Title
	//	{
	//		get { return "Версия " + _obj.ClassVersionNumber.ToString() + (_obj.IsCurrent ? " (т)" : ""); }
	//	}
            
	//	public static List<T> Versions
	//	{
	//		get
	//		{
	//			string clsName = typeof(T).Name;
	//			if (System.Web.HttpContext.Current.Items["CHST_" + clsName] == null)
	//			{
	//				System.Web.HttpContext.Current.Items["CHST_" + clsName] = A.Model.GetTable<T>().OrderByDescending(o => o.ClassVersionNumber).ToList();
	//			}
	//			return (List<T>)System.Web.HttpContext.Current.Items["CHST_" + clsName];
	//		}
	//	}

	//	public static T CurrentVersion
	//	{
	//		get
	//		{
	//			int classVersionID = UrlHelper.Current().GetInt("versionid", 0);
	//			if (classVersionID == 0)
	//				return Versions.OrderByDescending(o => o.ClassVersionNumber).OrderByDescending(o => o.IsCurrent).FirstOrDefault();
	//			return Versions.SingleOrDefault(o => o.ClassVersionID == classVersionID);
	//		}
	//	}

	//	public static int CurrentVersionID
	//	{
	//		get
	//		{
	//			int classVersionID = UrlHelper.Current().GetInt("versionid", 0);
	//			if (classVersionID == 0)
	//				return Versions.OrderByDescending(o => o.ClassVersionNumber).OrderByDescending(o => o.IsCurrent).Select(o => o.ClassVersionID).FirstOrDefault();
	//			return classVersionID;
	//		}
	//	}
	//}
}