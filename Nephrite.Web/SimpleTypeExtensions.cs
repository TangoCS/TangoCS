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

namespace Nephrite.Web
{
    public static partial class SimpleTypeExtensions
    {
        /// <summary>
        /// Вывод текста исключения в поток
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="writer"></param>
        public static void Render(this Exception ex, HtmlTextWriter writer)
        {
            writer.Write("<i>" + HttpContext.Current.Request.Url.AbsoluteUri + "</i>");
            writer.WriteBreak();
            writer.WriteBreak();
            Exception e = ex;
            while (e != null)
            {
                writer.Write("<b>");
                writer.Write(HttpUtility.HtmlEncode(e.Message));
                writer.Write("</b>");
                writer.WriteBreak();
                writer.WriteBreak();
                writer.Write(HttpUtility.HtmlEncode(e.StackTrace).Replace("\n", "<br />"));
                writer.WriteBreak();
                writer.WriteBreak();
                writer.WriteBreak();
                e = e.InnerException;
            }
        }

        public static void DataBindOnce(this DropDownList ddl, object dataSource)
        {
            if (ddl.Items.Count == 0)
            {
                ddl.DataSource = dataSource;
                ddl.DataBind();
            }
        }

        public static void DataBindOnce(this DropDownList ddl, object dataSource, bool insertEmptyString)
        {
            if (ddl.Items.Count == 0)
            {
                ddl.DataSource = dataSource;
                ddl.DataBind();
                if (insertEmptyString) ddl.Items.Insert(0, "");
            }
        }

        public static void SetValue(this DropDownList ddl, object value)
        {
            if (value == null)
            {
                ddl.SelectedValue = null;
                return;
            }
            if (ddl.Items.FindByValue(value.ToString()) != null)
                ddl.SelectedValue = value.ToString();
        }

        public static int? GetValue(this DropDownList ddl)
        {
            return ddl.SelectedValue.ToInt32();
        }

        public static int GetValue(this DropDownList ddl, int defaultValue)
        {
            return ddl.SelectedValue.ToInt32(defaultValue);
        }

		public static Guid? GetValueGuid(this DropDownList ddl)
		{
			if (ddl.SelectedValue.IsEmpty()) return null;
			return new Guid(ddl.SelectedValue);
		}

        public static IEnumerable<ListItem> GetSelected(this CheckBoxList cbl)
        {
            return cbl.Items.OfType<ListItem>().Where(o => o.Selected);
        }
        public static IEnumerable<string> GetSelectedValues(this CheckBoxList cbl)
        {
            return cbl.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value);
        }
        public static IEnumerable<int> GetSelectedValuesInt(this CheckBoxList cbl)
        {
            return cbl.Items.OfType<ListItem>().Where(o => o.Selected).Select(o => o.Value.ToInt32(0));
        }
        public static void SelectItems(this CheckBoxList cbl, IEnumerable<int> values)
        {
            foreach (ListItem li in cbl.Items.OfType<ListItem>().Where(o => values.Contains(o.Value.ToInt32(0))))
                li.Selected = true;
        }
		public static void SelectItems(this CheckBoxList cbl, IEnumerable<string> values)
		{
			foreach (ListItem li in cbl.Items.OfType<ListItem>().Where(o => values.Contains(o.Value)))
				li.Selected = true;
		}

        public static string Icon(this bool src)
        {
            if (src)
                return "<img src='" + Settings.ImagesPath + "tick.png' />";
            return String.Empty;
        }

        public static string Icon(this bool? src)
        {
            if (src.HasValue)
				return src.Value ? "<img src='" + Settings.ImagesPath + "tick.png' />" : "";
            return String.Empty;
        }


        public static void Redirect(this HttpResponse response,
            string url,
            string target,
            string windowFeatures)
        {

            if ((String.IsNullOrEmpty(target) ||
                target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
                String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;
                if (page == null)
                {
                    throw new InvalidOperationException(
                        "Cannot redirect to new window outside Page context.");

                }

                url = page.ResolveClientUrl(url);

                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }

                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page,
                    typeof(Page),
                    "Redirect",
                    script,
                    true);
            }

        }

        public static int ToInt32(this string src, int defaultValue)
        {
            int x;
            if (int.TryParse(src, out x))
                return x;
            return defaultValue;
        }

		public static long ToInt64(this string src, long defaultValue)
		{
			long x;
			if (long.TryParse(src, out x))
				return x;
			return defaultValue;
		}

        public static int? ToInt32(this string src)
        {
            int x;
            if (int.TryParse(src, out x))
                return x;
            return null;
        }

		public static Guid ToGuid(this string src)
		{
			try
			{
				return new Guid(src);
			}
			catch
			{
				return Guid.Empty;
			}
		}

        public static double ToDouble(this string src, double defaultValue)
        {
            double x;
			if (double.TryParse(src.Replace(",", "."), out x))
                return x;
            return defaultValue;
        }

        public static double? ToDouble(this string src)
        {
            double x;
			if (double.TryParse(src.Replace(",", "."), out x))
                return x;
            return null;
        }

		public static decimal ToDecimal(this string src, decimal defaultValue)
		{
			if (src == null)
				return defaultValue;
			decimal x;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return defaultValue;
		}

		public static decimal? ToDecimal(this string src)
		{
			if (src == null)
				return null;
			decimal x;
			if (decimal.TryParse(src.Replace(",", ".").Replace(" ", "").Replace(" ", ""), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
				return x;
			return null;
		}

		public static string MoneyToString(this decimal money)
		{
			return money.ToString("###,###,###,###,##0.00", AppWeb.CurrentCulture);
		}

		public static string MoneyToString(this decimal? money)
		{
			if (money == null)
				return "";
			return money.Value.ToString("###,###,###,###,##0.00", AppWeb.CurrentCulture);
		}
		
        public static DateTime ToDate(this string src, DateTime defaultValue)
        {
            DateTime dt;
            if (DateTime.TryParseExact(src, "d.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
            return defaultValue;
        }

		public static DateTime? ToDate(this string src)
		{
			DateTime dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return null;
		}

        public static DateTime ToDateTime(this string src, DateTime defaultValue)
        {
            DateTime dt;
            src = src.Replace("%20", " ");
            src = src.Replace("%3a", ":");
            src = src.Replace("+", " ");
            if (DateTime.TryParseExact(src, "d.MM.yyyy H:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy H:mm", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return defaultValue;
        }

		public static DateTime? ToDateTime(this string src)
		{
			DateTime dt;
			src = src.Replace("%20", " ");
			src = src.Replace("%3a", ":");
			src = src.Replace("+", " ");
			if (DateTime.TryParseExact(src, "d.MM.yyyy H:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			if (DateTime.TryParseExact(src, "d.MM.yyyy H:mm", null, System.Globalization.DateTimeStyles.None, out dt))
				return dt;
			return null;
		}

        public static string DateToString(this DateTime? src)
        {
            return src.DateToString(String.Empty);
        }

        public static string DateToString(this DateTime src)
        {
            return src.ToString("dd.MM.yyyy");
        }

        public static string DateToString(this DateTime? src, string defaultValue)
        {
            if (src.HasValue)
                return src.Value.ToString("dd.MM.yyyy");
            return defaultValue;
        }

        public static string TimeToString(this DateTime? src)
        {
            if (src.HasValue)
                return src.Value.ToString("HH:mm");
            return String.Empty;
        }

		public static string TimeToString(this DateTime src)
		{
			return src.ToString("HH:mm");
		}

		public static string DateTimeToString(this DateTime? src)
		{
			return src.DateTimeToString(String.Empty);
		}

		public static string DateTimeToString(this DateTime src)
		{
			return src == DateTime.MinValue ? "" : src.ToString("dd.MM.yyyy HH:mm");
		}

		public static string DateTimeToString(this DateTime? src, string defaultValue)
		{
			if (src.HasValue)
				return src.Value.ToString("dd.MM.yyyy HH:mm");
			return defaultValue;
		}


        public static string DateToRussianString(this DateTime? src, bool dayInQuotes)
        {
            if (src.HasValue)
                return src.Value.DateToRussianString(dayInQuotes);
            return "";
        }

        public static string DateToRussianString(this DateTime src, bool dayInQuotes)
        {
            string res = DateToRussianDayMonth(src, dayInQuotes);
            res += " " + src.ToString("yyyy") + " г.";
            return res;
        }

        public static string DateToRussianDayMonth(this DateTime src, bool dayInQuotes)
        {
            string res = dayInQuotes ? "«" : "";
            res += src.Day.ToString();
            res += dayInQuotes ? "» " : " ";

            switch (src.Month)
            {
                case 1:
                    res += "января";
                    break;
                case 2:
                    res += "февраля";
                    break;
                case 3:
                    res += "марта";
                    break;
                case 4:
                    res += "апреля";
                    break;
                case 5:
                    res += "мая";
                    break;
                case 6:
                    res += "июня";
                    break;
                case 7:
                    res += "июля";
                    break;
                case 8:
                    res += "августа";
                    break;
                case 9:
                    res += "сентября";
                    break;
                case 10:
                    res += "октября";
                    break;
                case 11:
                    res += "ноября";
                    break;
                case 12:
                    res += "декабря";
                    break;
            }
            return res;
        }

        public static string DayOfWeekRussian(this DateTime src)
        {
            return ((DateTime?)src).DayOfWeekRussian();
        }

        public static string DayOfWeekRussian(this DateTime? src)
        {
            if (!src.HasValue)
                return String.Empty;

            switch (src.Value.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "пятница";
                case DayOfWeek.Monday:
                    return "понедельник";
                case DayOfWeek.Saturday:
                    return "суббота";
                case DayOfWeek.Sunday:
                    return "воскресенье";
                case DayOfWeek.Thursday:
                    return "четверг";
                case DayOfWeek.Tuesday:
                    return "вторник";
                case DayOfWeek.Wednesday:
                    return "среда";
                default:
                    return "";
            }
        }

        public static string Arg(this string str, int index)
        {
            if (str == null)
                return String.Empty;

            string[] args = str.Split('|');
            if (index >= args.Length)
                return String.Empty;
            return args[index];
        }

        public static string Join(this string[] str, string separator)
        {
            return String.Join(separator, str.Where(s => !String.IsNullOrEmpty(s)).ToArray());
        }

        public static string Join(this IEnumerable<string> str, string separator)
        {
			return String.Join(separator, str.Where(s => !String.IsNullOrEmpty(s)).ToArray());
        }

        static string GetNumber1String(int n)
        {
			
            switch (n)
            {
                case 1:
                    return "первого";
                case 2:
                    return "второго";
                case 3:
                    return "третьего";
                case 4:
                    return "четвертого";
                case 5:
                    return "пятого";
                case 6:
                    return "шестого";
                case 7:
                    return "седьмого";
                case 8:
                    return "восьмого";
                case 9:
                    return "девятого";
                default:
                    return "";
            }
        }

        static string GetNumber2String(int n)
        {
            switch (n / 10)
            {
                case 2:
                    return "двадцать";
                case 3:
                    return "тридцать";
                case 4:
                    return "сорок";
                case 5:
                    return "пятьдесят";
                case 6:
                    return "шестьдесят";
                case 7:
                    return "семьдесят";
                case 8:
                    return "восемьдесят";
                case 9:
                    return "девяносто";
                default:
                    return "";
            }
        }

        static string GetNumber3String(int n)
        {
            switch (n)
            {
                case 100:
                    return "сто";
                case 200:
                    return "двести";
                case 300:
                    return "триста";
                case 400:
                    return "четыреста";
                case 500:
                    return "пятьсот";
                case 600:
                    return "шестьсот";
                case 700:
                    return "семьсот";
                case 800:
                    return "восемьсот";
                case 900:
                    return "девятьсот";
                default:
                    return "";
            }
        }

        public static string GetNumberString(this int n)
        {
            if (n < 10)
            {
                return GetNumber1String(n);
            }
            if (n >= 10 && n < 20)
            {
                switch (n)
                {
                    case 10:
                        return "десятого";
                    case 11:
                        return "одиннадцатого";
                    case 12:
                        return "двенадцатого";
                    case 13:
                        return "тринадцатого";
                    case 14:
                        return "четырнадцатого";
                    case 15:
                        return "пятнадцатого";
                    case 16:
                        return "шестнадцатого";
                    case 17:
                        return "семнадцатого";
                    case 18:
                        return "восемнадцатого";
                    case 19:
                        return "девятнадцатого";
                }
            }
            if (n >= 20 && n < 100)
            {
                switch (n)
                {
                    case 20:
                        return "двадцатого";
                    case 30:
                        return "тридцатого";
                    case 40:
                        return "сорокового";
                    case 50:
                        return "пятидесятого";
                    case 60:
                        return "шестидесятого";
                    case 70:
                        return "семидесятого";
                    case 80:
                        return "восьмидесятого";
                    case 90:
                        return "девяностого";
                }
                return GetNumber2String(n) + " " + GetNumber1String(n - 10 * (n / 10));
            }

            if (n >= 100 && n < 1000)
            {
                switch (n)
                {
                    case 100:
                        return "сотого";
                    case 200:
                        return "двухсотого";
                    case 300:
                        return "трехсотого";
                    case 400:
                        return "четырехсотого";
                    case 500:
                        return "пятисотого";
                    case 600:
                        return "шестисотого";
                    case 700:
                        return "семисотого";
                    case 800:
                        return "восьмисотого";
                    case 900:
                        return "девятисотого";
                }
                return GetNumber3String(100 * (n / 100)) + " " + GetNumberString(n - 100 * (n / 100));
            }

            if (n >= 1000 && n < 10000)
            {
                if (n == 1000)
                    return "тысячного";
                if (n < 2000)
                    return "одна тысяча " + GetNumberString(n - 1000);
                if (n < 3000)
                    return "две тысячи " + GetNumberString(n - 2000);
                if (n < 4000)
                    return "три тысячи " + GetNumberString(n - 3000);
                if (n < 5000)
                    return "четыре тысячи " + GetNumberString(n - 4000);
                if (n < 6000)
                    return "пять тысяч " + GetNumberString(n - 5000);
            }
            return "";
        }

        /// <summary>
        /// Число римскими цифрами (требуется переделка алгоритма)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetRomeString(this int n)
        {
            // Вообще-то надо это переделать по правилам http://ru.wikipedia.org/wiki/%D0%A0%D0%B8%D0%BC%D1%81%D0%BA%D0%B8%D0%B5_%D1%86%D0%B8%D1%84%D1%80%D1%8B
            // или найти готовые исходники
            if (n <= 10)
            {
                switch (n)
                {
                    case 1:
                        return "I";
                    case 2:
                        return "II";
                    case 3:
                        return "III";
                    case 4:
                        return "IV";
                    case 5:
                        return "V";
                    case 6:
                        return "VI";
                    case 7:
                        return "VII";
                    case 8:
                        return "VIII";
                    case 9:
                        return "IX";
                    case 10:
                        return "X";
                }
            }

            int x = n / 10;
            return "".PadLeft(x, 'X') + GetRomeString(x * n - 10);
        }

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

        public static void InitSeqNo(this IMovableObject obj, IEnumerable<IMovableObject> sequenceContext)
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

		public static string RenderControl(this Control ctrl)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter tw = new StringWriter(sb);
			HtmlTextWriter hw = new HtmlTextWriter(tw);

			ctrl.RenderControl(hw);
			return sb.ToString();
		}

        public static bool In(this string str, params string[] items)
        {
            return items.Contains(str);
        }

		public static string ConvertToFtsQuery(this string str)
		{
			string[] words = str.Split(new char[] { ',', ' ', '?' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder(200);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].Replace("*", "").Replace("\"", "").Replace("'", "");
				sb.Append(sb.Length > 0 ? " AND " : "");
				sb.AppendFormat("(\"{0}*\")", words[i]);
			}
			return sb.ToString();
		}

		public static string ConvertToFtsQueryOR(this string str)
		{
			string[] words = str.Split(new char[] { ',', ' ', '?' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder(200);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].Replace("*", "").Replace("\"", "").Replace("'", "");
				sb.Append(sb.Length > 0 ? " OR " : "");
				sb.AppendFormat("(\"{0}*\")", words[i]);
			}
			return sb.ToString();
		}

		public static bool IsEmpty(this string str)
		{
			return String.IsNullOrEmpty(str) ? true : str.Trim() == String.Empty;
		}

		public static bool Validate(this string str, string pattern)
		{
			if (str.IsEmpty())
				return true;

			return Regex.IsMatch(str, pattern);
		}

		private enum State
		{
			AtBeginningOfToken,
			InNonQuotedToken,
			InQuotedToken,
			ExpectingComma,
			InEscapedCharacter
		};

		public static string[] CsvSplit(this String source)
		{
			return CsvSplit(source, ';');
		}
		public static string[] CsvSplit(this String source, char delimiter)
		{
			List<string> splitString = new List<string>();
			List<int> slashesToRemove = null;
			State state = State.AtBeginningOfToken;
			char[] sourceCharArray = source.ToCharArray();
			int tokenStart = 0;
			int len = sourceCharArray.Length;
			for (int i = 0; i < len; ++i)
			{
				switch (state)
				{
					case State.AtBeginningOfToken:
						if (sourceCharArray[i] == '"')
						{
							state = State.InQuotedToken;
							slashesToRemove = new List<int>();
							continue;
						}
						if (sourceCharArray[i] == delimiter)
						{
							splitString.Add("");
							tokenStart = i + 1;
							continue;
						}
						state = State.InNonQuotedToken;
						continue;
					case State.InNonQuotedToken:
						if (sourceCharArray[i] == delimiter)
						{
							splitString.Add(
								source.Substring(tokenStart, i - tokenStart));
							state = State.AtBeginningOfToken;
							tokenStart = i + 1;
						}
						continue;
					case State.InQuotedToken:
						if (sourceCharArray[i] == '"')
						{
							state = State.ExpectingComma;
							continue;
						}
						if (sourceCharArray[i] == '\\')
						{
							state = State.InEscapedCharacter;
							slashesToRemove.Add(i - tokenStart);
							continue;
						}
						continue;
					case State.ExpectingComma:
						if (sourceCharArray[i] != delimiter)
							throw new Exception("Expecting comma. String: " + source + ". Position " + i.ToString());
						string stringWithSlashes =
							source.Substring(tokenStart, i - tokenStart);
						foreach (int item in slashesToRemove.Reverse<int>())
							stringWithSlashes =
								stringWithSlashes.Remove(item, 1);
						splitString.Add(
							stringWithSlashes.Substring(1,
								stringWithSlashes.Length - 2));
						state = State.AtBeginningOfToken;
						tokenStart = i + 1;
						continue;
					case State.InEscapedCharacter:
						state = State.InQuotedToken;
						continue;
				}
			}
			switch (state)
			{
				case State.AtBeginningOfToken:
					splitString.Add("");
					return splitString.ToArray();
				case State.InNonQuotedToken:
					splitString.Add(
						source.Substring(tokenStart,
							source.Length - tokenStart));
					return splitString.ToArray();
				case State.InQuotedToken:
					throw new Exception("Expecting ending quote. String: " + source);
				case State.ExpectingComma:
					string stringWithSlashes =
						source.Substring(tokenStart, source.Length - tokenStart);
					foreach (int item in slashesToRemove.Reverse<int>())
						stringWithSlashes = stringWithSlashes.Remove(item, 1);
					splitString.Add(
						stringWithSlashes.Substring(1,
							stringWithSlashes.Length - 2));
					return splitString.ToArray();
				case State.InEscapedCharacter:
					throw new Exception("Expecting escaped character. String: " + source);
			}
			throw new Exception("Unexpected error");
		}

		public static string GetAttributeValue(this XElement element, XName name)
		{
			var attribute = element.Attribute(name);
			return attribute != null ? attribute.Value : null;
		}

    }
}

