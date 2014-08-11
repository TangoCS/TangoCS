using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
	//public static class MMObjectExtensions
	//{
	//	public static T Property<T>(this IMMObject obj, string propertyName)
	//	{
	//		return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
	//	}

	//	public static Expression<Func<IMMObject, object>> GetIdentifierSelector(this IMMObject obj)
	//	{
	//		ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObject), "c");
	//		// Преобразование IMMObject к нужному нам реальному типу объекта
	//		UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
	//		// Получение у объекта свойства с именем, соответствующим первичному ключу
	//		MemberExpression me_id = MemberExpression.Property(ue_c, obj.MetaClass.Key.Name);
	//		UnaryExpression ue_c1 = UnaryExpression.Convert(me_id, typeof(object));
	//		return Expression.Lambda<Func<IMMObject, object>>(ue_c1, pe_c);
	//	}

	//	public static Expression<Func<IMMObjectVersion, object>> GetIdentifierSelector(this IMMObjectVersion obj)
	//	{
	//		ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObject), "c");
	//		// Преобразование IMMObject к нужному нам реальному типу объекта
	//		UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
	//		// Получение у объекта свойства с именем, соответствующим первичному ключу
	//		string pkname = obj.MetaClass.Key.Name;
	//		if (pkname.EndsWith("GUID"))
	//			pkname = pkname.Substring(0, pkname.Length - 4) + "VersionGUID";
	//		else if (pkname.EndsWith("ID"))
	//			pkname = pkname.Substring(0, pkname.Length - 2) + "VersionID";
	//		MemberExpression me_id = MemberExpression.Property(ue_c, pkname);
	//		UnaryExpression ue_c1 = UnaryExpression.Convert(me_id, typeof(object));
	//		return Expression.Lambda<Func<IMMObjectVersion, object>>(ue_c1, pe_c);
	//	}

	//	public static Expression<Func<IMMObjectVersion, bool>> FilterByObjectID(this IMMObjectVersion obj, object id)
	//	{
	//		// Параметр лямбда-выражения типа T
	//		ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObjectVersion), "c");
	//		// Преобразование IMMObject к нужному нам реальному типу объекта
	//		UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
	//		// Получение у объекта свойства с именем, соответствующим первичному ключу
	//		MemberExpression me_id = MemberExpression.Property(ue_c, obj.MetaClass.Key.Name);
	//		// Константа, по которой будем искать объект
	//		ConstantExpression ce_val = ConstantExpression.Constant(id, id.GetType());
	//		// Сравнение первичного ключа с заданным идентификатором
	//		BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
	//		// Само лямбда-выражение
	//		return Expression.Lambda<Func<IMMObjectVersion, bool>>(be_eq, pe_c);
	//	}

	//	public static Expression<Func<IMMObject, bool>> FilterByProperty(this IMMObject obj, string propertyName, string id)
	//	{
	//		// Параметр лямбда-выражения типа T
	//		ParameterExpression pe_c = ParameterExpression.Parameter(typeof(IMMObject), "c");
	//		// Преобразование IMMObject к нужному нам реальному типу объекта
	//		UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
	//		// Получение у объекта свойства с именем, соответствующим свойству
	//		MemberExpression me_id = MemberExpression.Property(ue_c, propertyName);
	//		// Константа, по которой будем искать объект
	//		Type t = obj.GetType().GetProperty(propertyName).PropertyType;
	//		ConstantExpression ce_val = ConstantExpression.Constant(t == typeof(Guid) ? (object)id.ToGuid() : (t == typeof(Guid?) ? (object)id.ToGuid() : id.ToInt32(0)), t);
	//		// Сравнение первичного ключа с заданным идентификатором
	//		BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
	//		// Само лямбда-выражение
	//		return Expression.Lambda<Func<IMMObject, bool>>(be_eq, pe_c);
	//	}
	//}
}
