using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Nephrite.Web.Controls;
using Nephrite.Web;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.Multilanguage;

namespace Nephrite.Web
{
    public static partial class ModelExtensions
    {
       
        public static string LastModifiedDateString(this ILastModifiedDate obj)
        {
            if (((IModelObject)obj).ObjectID > 0)
                return obj.LastModifiedDate.ToString("dd.MM.yyyy HH:mm");
            return String.Empty;
        }

        public static Expression<Func<T, bool>> FindByID<T>(this Object obj, int id)
        {
            // Найти свойство с первичным ключом
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
                {
                    ParameterExpression pe_c = ParameterExpression.Parameter(typeof(T), "c");
                    UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
                    // Получение у объекта свойства с именем, соответствующим первичному ключу
                    MemberExpression me_id = MemberExpression.Property(ue_c, pi.Name);
                    // Константа, по которой будем искать объект
                    ConstantExpression ce_val = ConstantExpression.Constant(id, typeof(int));
                    // Сравнение первичного ключа с заданным идентификатором
                    BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
                    // Само лямбда-выражение
                    return Expression.Lambda<Func<T, bool>>(be_eq, pe_c);
                }
            }
            throw new Exception("В классе " + obj.GetType().FullName + " не определён первичный ключ");
        }

		public static Expression<Func<T, bool>> FindByID<T>(this Object obj, object id)
		{
			// Найти свойство с первичным ключом
			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
				if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
				{
					ParameterExpression pe_c = ParameterExpression.Parameter(typeof(T), "c");
					UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
					// Получение у объекта свойства с именем, соответствующим первичному ключу
					MemberExpression me_id = MemberExpression.Property(ue_c, pi.Name);
					// Константа, по которой будем искать объект
					ConstantExpression ce_val = ConstantExpression.Constant(id, pi.PropertyType);
					// Сравнение первичного ключа с заданным идентификатором
					BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
					// Само лямбда-выражение
					return Expression.Lambda<Func<T, bool>>(be_eq, pe_c);
				}
			}
			throw new Exception("В классе " + obj.GetType().FullName + " не определён первичный ключ");
		}

		public static Expression<Func<T, bool>> FindByProperty<T>(this T obj, string propertyName, object propertyValue)
		{
			Type t = obj.GetType();
			PropertyInfo pi = t.GetProperty(propertyName);
			
			if (pi != null)
			{
				ParameterExpression pe_c = Expression.Parameter(typeof(T), "c");
				UnaryExpression ue_c = Expression.Convert(pe_c, t);
				MemberExpression me_id = Expression.Property(ue_c, pi.Name);
				if (propertyValue != null && propertyValue.GetType().IsArray)
				{
					Type arrayElementType = propertyValue.GetType().GetElementType();
					MethodInfo method = typeof(Enumerable).GetMethods()
					.Where(m => m.Name == "Contains" && m.GetParameters().Length == 2)
					.Single().MakeGenericMethod(arrayElementType);
					
					var callContains = Expression.Call(
						method,
						Expression.Convert(Expression.Constant(propertyValue, propertyValue.GetType()),
						typeof(IEnumerable<>).MakeGenericType(arrayElementType)),
						Expression.Convert(me_id, arrayElementType));
					return Expression.Lambda<Func<T, bool>>(callContains, pe_c);
				}
				else
				{
					BinaryExpression be_eq = Expression.Equal(me_id, Expression.Constant(propertyValue, pi.PropertyType));
					return Expression.Lambda<Func<T, bool>>(be_eq, pe_c);
				}
			}

			throw new Exception("В классе " + t.FullName + " не найдено свойство " + propertyName);
		}

		public static Expression<Func<T, bool>> FindByIDs<T>(this Object obj, IEnumerable<int> collection)
		{
			// Найти свойство с первичным ключом
			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
				if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
				{
					ParameterExpression pe_c = ParameterExpression.Parameter(typeof(T), "c");
					UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
					// Получение у объекта свойства с именем, соответствующим первичному ключу
					MemberExpression me_id = MemberExpression.Property(ue_c, pi.Name);

					IEnumerable<Expression> equals = collection.Select(value =>
						(Expression)Expression.Equal(me_id, Expression.Constant(value, typeof(int))));
					Expression body = equals.Aggregate((accumulate, equal) => Expression.Or(accumulate, equal));
					return Expression.Lambda<Func<T, bool>>(body, pe_c);

				}
			}
			throw new Exception("В классе " + obj.GetType().FullName + " не определён первичный ключ");
		}

		public static Expression<Func<T, bool>> FindByGUID<T>(this Object obj, Guid guid)
		{
			// Найти свойство с первичным ключом
			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
				if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
				{
					ParameterExpression pe_c = ParameterExpression.Parameter(typeof(T), "c");
					UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
					// Получение у объекта свойства с именем, соответствующим первичному ключу
					MemberExpression me_id = MemberExpression.Property(ue_c, pi.Name);
					// Константа, по которой будем искать объект
					ConstantExpression ce_val = ConstantExpression.Constant(guid, typeof(Guid));
					// Сравнение первичного ключа с заданным идентификатором
					BinaryExpression be_eq = BinaryExpression.Equal(me_id, ce_val);
					// Само лямбда-выражение
					return Expression.Lambda<Func<T, bool>>(be_eq, pe_c);
				}
			}
			throw new Exception("В классе " + obj.GetType().FullName + " не определён первичный ключ");
		}

		public static Expression<Func<T, bool>> NotID<T>(this Object obj, int id)
		{
			// Найти свойство с первичным ключом
			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				object[] attr = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
				if (attr.Length == 1 && ((ColumnAttribute)attr[0]).IsPrimaryKey == true)
				{
					ParameterExpression pe_c = ParameterExpression.Parameter(typeof(T), "c");
					UnaryExpression ue_c = UnaryExpression.Convert(pe_c, obj.GetType());
					// Получение у объекта свойства с именем, соответствующим первичному ключу
					MemberExpression me_id = MemberExpression.Property(ue_c, pi.Name);
					// Константа, по которой будем искать объект
					ConstantExpression ce_val = ConstantExpression.Constant(id, typeof(int));
					// Сравнение первичного ключа с заданным идентификатором
					BinaryExpression be_eq = BinaryExpression.NotEqual(me_id, ce_val);
					// Само лямбда-выражение
					return Expression.Lambda<Func<T, bool>>(be_eq, pe_c);
				}
			}
			throw new Exception("В классе " + obj.GetType().FullName + " не определён первичный ключ");
		}

        public static void InitSeqNo(this IWithSeqNo obj, IEnumerable<IWithSeqNo> sequenceContext)
        {
            if (sequenceContext == null)
                obj.SeqNo = 1;
            else
            {
				obj.SeqNo = (sequenceContext.Max(o => (int?)o.SeqNo) ?? 0) + 1;
                /*if (sequenceContext.Count() == 0)
                    obj.SeqNo = 1;
                else
                    obj.SeqNo = sequenceContext.Max(o => o.SeqNo) + 1;*/
            }
        }

    }
}

