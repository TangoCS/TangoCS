using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public class ListFilterEngine
	{
		public static bool BoolAsInt = false;
		public static StringContainsMapStrategy StringContainsMapStrategy = StringContainsMapStrategy.Contains;

		IResourceManager Resources { get; set; }
		public ListFilterEngine(IResourceManager resources)
		{
			Resources = resources;
        }

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, List<Field> fieldList, List<FilterItem> criteria)
		{
			IQueryable<T> newquery = query;

			// Преобразование в ОПН
			Stack<object> stack = new Stack<object>();
			List<object> pnlist = new List<object>();
			pnlist = ListFieldEngineHelper.GetPolishNotation(Resources, criteria);

			if (pnlist.Count == 0)
				return query;

			foreach (object it in pnlist)
			{
				if (it is FilterItem)
				{
					var item = it as FilterItem;

					Expression<Func<T, bool>> expr = null;
					Field f = fieldList.SingleOrDefault<Field>(f1 => f1.Title == item.Title);
					if (f == null)
						continue;

					LambdaExpression column = f.Column as LambdaExpression;
					if (column == null)
						column = ((List<object>)f.Column)[0] as LambdaExpression;

					if (item.Condition == Resources.Get("System.Filter.Contains") && f.FieldType == FieldType.String)
					{
						if (StringContainsMapStrategy == StringContainsMapStrategy.Contains)
						{
							MethodCallExpression mc = Expression.Call(column.Body,
								typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
								Expression.Constant(item.Value.ToLower()));
							expr = Expression.Lambda<Func<T, bool>>(mc, column.Parameters);
						}
						else if (StringContainsMapStrategy == StringContainsMapStrategy.IndexOfWithOrdinalIgnoreCase)
						{
							var mc = Expression.GreaterThanOrEqual(
								Expression.Call(column.Body,
								typeof(string).GetMethod("IndexOf", new Type[] { typeof(string), typeof(StringComparison) }),
								Expression.Constant(item.Value.ToLower()), Expression.Constant(StringComparison.OrdinalIgnoreCase)),
								Expression.Constant(0));
							expr = Expression.Lambda<Func<T, bool>>(mc, column.Parameters);
						}
					}

					if (item.Condition == Resources.Get("System.Filter.StartsWith") && f.FieldType == FieldType.String)
					{
						MethodCallExpression mc = Expression.Call(column.Body,
							typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
							Expression.Constant(item.Value.ToLower()));
						expr = Expression.Lambda<Func<T, bool>>(mc, column.Parameters);
					}

					object val = null;
					if (item.Value != null && item.Value.StartsWith("$"))
						val = MacroManager.Evaluate(item.Value.Substring(1));
					else
						val = item.Value;

					Type valType = column.Body is UnaryExpression ? ((UnaryExpression)column.Body).Operand.Type : column.Body.Type;

					if (f.FieldType == FieldType.Date)
					{
						DateTime dt;
						double d;

						if (item.Condition == Resources.Get("System.Filter.LastXDays"))
						{
							if (!double.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
								d = 0;
							val = DateTime.Now.AddDays(-d);
						}
						else
						{
							if (!DateTime.TryParseExact(item.Value, "d.MM.yyyy", null, DateTimeStyles.None, out dt))
							{
								DateTime? dtn = null;
								val = dtn;
							} // dt = DateTime.Today;
							else
								val = dt;
						}
					}

					if (f.FieldType == FieldType.DateTime)
					{
						DateTime dt;
						double d;


						if (item.Condition == Resources.Get("System.Filter.LastXDays"))
						{
							if (!double.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
								d = 0;
							val = DateTime.Now.AddDays(-d);
						}
						else
						{
							if (!DateTime.TryParseExact(item.Value, "d.MM.yyyy", null, DateTimeStyles.None, out dt))
							{
								DateTime? dtn = null;
								val = dtn;
							} // dt = DateTime.Today;
							else
								val = dt;
						}
					}

					if (f.FieldType == FieldType.Number || f.FieldType == FieldType.CustomInt)
					{
						decimal d;
						if (!decimal.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
							d = 0;
						val = d;
					}

					if (f.FieldType == FieldType.Boolean)
					{
						bool b;

						if (!bool.TryParse(item.Value, out b))
							b = false;

						if (BoolAsInt)
						{
							val = b ? 1 : 0;
							valType = typeof(int);
						}
						else
						{
							val = b;
						}
						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);
						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);
					}
					else if (f.FieldType == FieldType.String || f.FieldType == FieldType.DDL)
					{
						if (valType == typeof(Char) && val is string)
							val = ((string)val)[0];
						if ((valType == typeof(int?) || valType == typeof(int)) && val is string)
							val = int.Parse((string)val);

						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);

						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);
					}
					else if (f.FieldType == FieldType.CustomInt)
					{
						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, int, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, int, bool>>;
						val = Convert.ToInt32(val);
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else if (f.FieldType == FieldType.CustomString)
					{
						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, string, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, string, bool>>;
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else if (f.FieldType == FieldType.CustomObject)
					{
						if (valType == typeof(Char) && val is string)
							val = ((string)val)[0];
						if ((valType == typeof(int?) || valType == typeof(int)) && val is string)
							val = int.Parse((string)val);

						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, object, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, object, bool>>;
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else
					{
						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == ">=" || item.Condition == Resources.Get("System.Filter.LastXDays"))
							expr = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == ">")
							expr = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<")
							expr = Expression.Lambda<Func<T, bool>>(Expression.LessThan(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);
					}
					if (expr != null)
					{
						stack.Push(expr);
					}
				}
				if (it is char)
				{
					var op = (char)it;
					if (op == '!')
					{
						Expression<Func<T, bool>> operand = stack.Pop() as Expression<Func<T, bool>>;
						var newop1 = Expression.Not(operand.Body);
						stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand.Parameters));
					}
					if (op == '&')
					{
						Expression<Func<T, bool>> operand1 = stack.Pop() as Expression<Func<T, bool>>;
						Expression<Func<T, bool>> operand2 = stack.Pop() as Expression<Func<T, bool>>;
						stack.Push(And(operand1, operand2));
						//var newop1 = Expression.And(operand1.Body, operand2.Body);
						//stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand2.Parameters));
					}
					if (op == '|')
					{
						Expression<Func<T, bool>> operand1 = stack.Pop() as Expression<Func<T, bool>>;
						Expression<Func<T, bool>> operand2 = stack.Pop() as Expression<Func<T, bool>>;
						stack.Push(Or(operand1, operand2));
						//var newop1 = Expression.Or(operand1.Body, operand2.Body);
						//stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand2.Parameters));
					}
				}
			}
			if (stack.Count > 0)
				newquery = newquery.Where<T>(stack.Pop() as Expression<Func<T, bool>>);

			return newquery;
		}

		Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			return Expression.Lambda<Func<T, bool>>
			   (Expression.Or(expr1.Body, ReplaceParameterExpression(expr2.Body, expr1.Parameters[0])), expr1.Parameters);
		}

		Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			return Expression.Lambda<Func<T, bool>>
			   (Expression.AndAlso(expr1.Body, ReplaceParameterExpression(expr2.Body, expr1.Parameters[0])), expr1.Parameters);
		}

		Expression ReplaceParameterExpression(Expression x, ParameterExpression o)
		{
			if (x == null)
				return null;

			if (x is ConstantExpression)
				return x;

			if (x is ParameterExpression)
				return ((ParameterExpression)x).Name == "o" ? o : x;

			if (x is MemberExpression)
			{
				var x1 = x as MemberExpression;
				return Expression.MakeMemberAccess(ReplaceParameterExpression(x1.Expression, o), x1.Member);
			}
			if (x is UnaryExpression)
			{
				var x1 = x as UnaryExpression;
				return Expression.MakeUnary(x1.NodeType, ReplaceParameterExpression(x1.Operand, o), x1.Type, x1.Method);
			}
			if (x is BinaryExpression)
			{
				var x1 = x as BinaryExpression;
				return Expression.MakeBinary(x1.NodeType, ReplaceParameterExpression(x1.Left, o), ReplaceParameterExpression(x1.Right, o), x1.IsLiftedToNull, x1.Method, x1.Conversion);
			}
			if (x is ConditionalExpression)
			{
				var x1 = x as ConditionalExpression;
				return Expression.Condition(ReplaceParameterExpression(x1.Test, o), ReplaceParameterExpression(x1.IfTrue, o), ReplaceParameterExpression(x1.IfFalse, o));
			}
			if (x is MethodCallExpression)
			{
				var x1 = x as MethodCallExpression;
				return Expression.Call(ReplaceParameterExpression(x1.Object, o), x1.Method, x1.Arguments.Select(o1 => ReplaceParameterExpression(o1, o)));
			}
			if (x is LambdaExpression)
			{
				var x1 = x as LambdaExpression;
				return Expression.Lambda(x1.Type, ReplaceParameterExpression(x1.Body, o), x1.Parameters);
			}

			throw new Exception(Resources.Get("System.Filter.Error.UnsupportedTypeExpression") + x.GetType().ToString() + " : " + x.GetType().BaseType.Name);
		}

		Expression ReplaceParameterExpression(Expression x, string name, object value)
		{
			if (x == null)
				return null;

			if (x is ConstantExpression)
				return x;

			if (x is ParameterExpression)
				return ((ParameterExpression)x).Name == name ? Expression.Constant(value) : x;

			if (x is MemberExpression)
			{
				var x1 = x as MemberExpression;
				return Expression.MakeMemberAccess(ReplaceParameterExpression(x1.Expression, name, value), x1.Member);
			}
			if (x is UnaryExpression)
			{
				var x1 = x as UnaryExpression;
				return Expression.MakeUnary(x1.NodeType, ReplaceParameterExpression(x1.Operand, name, value), x1.Type, x1.Method);
			}
			if (x is BinaryExpression)
			{
				var x1 = x as BinaryExpression;
				return Expression.MakeBinary(x1.NodeType, ReplaceParameterExpression(x1.Left, name, value), ReplaceParameterExpression(x1.Right, name, value), x1.IsLiftedToNull, x1.Method, x1.Conversion);
			}
			if (x is ConditionalExpression)
			{
				var x1 = x as ConditionalExpression;
				return Expression.Condition(ReplaceParameterExpression(x1.Test, name, value), ReplaceParameterExpression(x1.IfTrue, name, value), ReplaceParameterExpression(x1.IfFalse, name, value));
			}
			if (x is MethodCallExpression)
			{
				var x1 = x as MethodCallExpression;
				return Expression.Call(ReplaceParameterExpression(x1.Object, name, value), x1.Method, x1.Arguments.Select(o1 => ReplaceParameterExpression(o1, name, value)));
			}
			if (x is LambdaExpression)
			{
				var x1 = x as LambdaExpression;
				return Expression.Lambda(x1.Type, ReplaceParameterExpression(x1.Body, name, value), x1.Parameters);
			}

			throw new Exception(Resources.Get("System.Filter.Error.UnsupportedTypeExpression") + x.GetType().ToString() + " : " + x.GetType().BaseType.Name);
		}
	}

	public static class ListFieldEngineHelper
	{
		public static List<object> GetPolishNotation(IResourceManager resources, List<FilterItem> criteria)
		{
			// Преобразование в ОПН
			Stack<object> stack = new Stack<object>();
			List<object> pnlist = new List<object>();
			foreach (FilterItem item in criteria)
			{
				if (item.Not)
					stack.Push('!');
				for (int i = 0; i < item.OpenBracketCount; i++)
					stack.Push('(');
				pnlist.Add(item);
				if (item.OpenBracketCount == 0 && item.Not)
				{
					pnlist.Add(stack.Pop());
				}
				for (int i = 0; i < item.CloseBracketCount; i++)
				{
					if (stack.Count == 0)
						throw new Exception(resources.Get("System.Filter.Error.BracketsNotConsistent"));
					object obj = stack.Pop();
					do
					{
						if ((char)obj == '(')
							break;
						pnlist.Add(obj);
						if (stack.Count == 0)
							throw new Exception(resources.Get("System.Filter.Error.BracketsNotConsistent"));
						obj = stack.Pop();
					} while ((char)obj != '(');
				}
				if (item.Operation == FilterItemOperation.And)
					stack.Push('&');
				else
					stack.Push('|');
			}
			if (stack.Count == 0)
				return pnlist;
			// Выкинуть последний символ
			stack.Pop();
			while (stack.Count() > 0)
			{
				object si = stack.Pop();
				if (si is char && (char)si == '(')
					throw new Exception(resources.Get("System.Filter.Error.BracketsNotConsistent"));
				pnlist.Add(si);
			}

			return pnlist;
		}
	}


	[Serializable]
	public class FilterItem
	{
		public string Title { get; set; }
		public string Condition { get; set; }
		public string Value { get; set; }
		public string ValueTitle { get; set; }
		public FieldType FieldType { get; set; }
		public bool Not { get; set; }
		public int OpenBracketCount { get; set; }
		public int CloseBracketCount { get; set; }
		public FilterItemOperation Operation { get; set; }
		//public bool Advanced { get; set; }

		public string OpenBrackets
		{
			get { return "".PadLeft(OpenBracketCount, '('); }
		}

		public string CloseBrackets
		{
			get { return "".PadLeft(CloseBracketCount, ')'); }
		}

		public override int GetHashCode()
		{
			return (Title + Condition + Value).GetHashCode();
		}

		public override string ToString()
		{
			return "";
			//var textResource = DI.GetService<ITextResource>();

			//return Title + " " + (Condition == textResource.Get("System.Filter.LastXDays", "последние x дней") ?
			//	String.Format(textResource.Get("System.Filter.LastDays", "последние &quot;{0}&quot; дней"), ValueTitle) :
			//	Condition + " &quot;" + WebUtility.HtmlEncode(ValueTitle) + "&quot;");
		}
	}

	public class Field
	{
		public int SeqNo { get; set; }
		public string Title { get; set; }
		public object Column { get; set; }
		public FieldType FieldType { get; set; }
		public IEnumerable<SelectListItem> Values { get; set; }
		public List<string> Operator { get; set; }
	}

	public enum FieldType
	{
		String,
		Number,
		Date,
		DateTime,
		DDL,
		Boolean,
		CustomInt,
		CustomString,
		CustomObject
	}

	public enum FilterItemOperation
	{
		And,
		Or
	}

	public enum StringContainsMapStrategy
	{
		Contains,
		IndexOfWithOrdinalIgnoreCase
	}

	public interface IPersistentFilter<TKey>
		where TKey : struct
	{
		bool Load(TKey? id, string listName, string listParms);
		IEnumerable<(TKey ID, string Name, bool IsDefault)> GetViews(string listName, IReadOnlyDictionary<string, object> listParms);

		void SaveCriteria();
		void SaveView(string name, bool isShared, bool isDefault, string listName, string listParms);

		void InsertOnSubmit();

		TKey ID { get; }
		string Name { get; }
		bool IsShared { get; }
		bool IsDefault { get; }

		List<FilterItem> Criteria { get; set; }
	}

	public interface IPersistentFilterEntity<TKey>
	{
		TKey ID { get; }
		string FilterName { get; set; }
		XDocument FilterValue { get; set; }
		bool IsDefault { get; set; }
		string ListName { get; set; }
		string ListParms { get; set; }
		bool IsShared { get; }
	}
}
