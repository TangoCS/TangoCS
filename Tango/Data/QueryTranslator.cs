using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Tango.Exceptions;

namespace Tango.Data
{
	public class QueryTranslator : ExpressionVisitor
	{
		readonly IQueryTranslatorDialect _dialect;
		StringBuilder sb;

		List<StringBuilder> sbWhere = new List<StringBuilder>();
		List<StringBuilder> sbOrder = new List<StringBuilder>();
		List<StringBuilder> sbGroupBy = new List<StringBuilder>();
		List<StringBuilder> sbGroupBySelector = new List<StringBuilder>();

		string _beforeConstant = "";
		string _afterConstant = "";
		readonly Func<bool, string> _boolConstant = o => o.ToString().ToLower();
		Dictionary<string, object> _parms = new Dictionary<string, object>();

		public string OrderBy { get; private set; } = string.Empty;
		public string WhereClause { get; private set; } = string.Empty;
		public string GroupBy { get; private set; } = string.Empty;
		public string GroupBySelector { get; private set; } = string.Empty;

		public IReadOnlyDictionary<string, object> Parms => _parms;

		bool _hasvalueexpression = false;
		bool _nullconstant = false;

		public QueryTranslator(IQueryTranslatorDialect dialect)
		{
			_dialect = dialect;
		}

		public void Translate(Expression expression)
		{
			Visit(expression);
			WhereClause = sbWhere.Select(o => o.ToString()).Join(" and ");
			OrderBy = sbOrder.Select(o => o.ToString()).Join(", ");
			GroupBy = sbGroupBy.Select(o => o.ToString()).Join(", ");
			GroupBySelector = sbGroupBySelector.Select(o => o.ToString()).Join(", ").Replace("@@KEY", GroupBy);
		}

		private static Expression StripQuotes(Expression e)
		{
			while (e.NodeType == ExpressionType.Quote)
			{
				e = ((UnaryExpression)e).Operand;
			}
			return e;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
			{
				sb = new StringBuilder();
				sbWhere.Add(sb);

				var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
				Visit(lambda.Body);

				return Visit(m.Arguments[0]);
			}
			else if (m.Method.Name == "Take")
			{
				if (ParseTakeExpression(m))
				{
					return Visit(m.Arguments[0]);
				}
			}
			else if (m.Method.Name == "Skip")
			{
				if (ParseSkipExpression(m))
				{
					return Visit(m.Arguments[0]);
				}
			}
			else if (m.Method.Name == "OrderBy")
			{
				ParseOrderByExpression(m, "ASC");
				return Visit(m.Arguments[0]);
			}
			else if (m.Method.Name == "GroupBy")
			{
				sb = new StringBuilder();
				sbGroupBy.Add(sb);

				var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
				Visit(lambda.Body);

				return Visit(m.Arguments[0]);
			}
			else if (m.Method.Name == "OrderByDescending")
			{
				ParseOrderByExpression(m, "DESC");
				return Visit(m.Arguments[0]);
			}
			else if (m.Method.Name == "Contains")
			{
				ParseContainsMethod(m);
				return m;
			}
			else if (m.Method.Name == "StartsWith")
			{
				ParseStartsWithMethod(m);
				return m;
			}
			else if (m.Method.Name == "EndsWith")
			{
				ParseEndsWithMethod(m);
				return m;
			}
			else if (m.Method.Name == "ToLower")
			{
				sb.Append("lower(");
				Visit(m.Object);
				sb.Append(")");
				return m;
			}
			else if (m.Method.Name == "ToString")
			{
				ParseToStringMethod(m);
				return m;
			}
			else if (m.Method.Name == "Substring")
			{
				ParseSubstringMethod(m);
				return m;
			}
			else if (m.Method.Name == "get_Item")
			{
				sb.Append((m.Arguments[0] as ConstantExpression).Value);
				return m;
			}
			else if (m.Method.Name == "Select")
			{
				sb = new StringBuilder();
				sbGroupBySelector.Add(sb);

				var lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
				Visit(lambda.Body);

				return Visit(m.Arguments[0]);
			}

			throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
		}

		protected override Expression VisitNew(NewExpression node)
		{
			var s = new List<string>();
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				var arg = node.Arguments[i];
				var m = node.Members[i];

				if (arg is MemberExpression me)
					s.Add(me.Member.Name == "Key" ? "@@KEY" : me.Member.Name);
				else if (arg is MethodCallExpression mce && mce.Method.Name == "Count")
				{
					if (mce.Arguments[0] is ParameterExpression)
					{
						s.Add("count(1) as " + m.Name);
					}
					else if (mce.Arguments[0] is MethodCallExpression mce2)
					{
						if (mce2.Method.Name == "Distinct" &&
							mce2.Arguments[0] is MethodCallExpression mce3 && mce3.Method.Name == "Select")
						{
							var cursb = sb;
							sb = new StringBuilder();
							Visit(mce3.Arguments[1]);
							s.Add($"count(distinct {sb.ToString()}) as {m.Name}");
							sb = cursb;
						}
					}
				}
				else
					throw new NotSupportedException($"Unsupported expression {arg.NodeType}");
			}

			sb.Append(s.Join(", "));
			return node;
		}

		protected override Expression VisitUnary(UnaryExpression u)
		{
			switch (u.NodeType)
			{
				case ExpressionType.Not:
					sb.Append(" NOT ");
					Visit(u.Operand);
					break;
				case ExpressionType.Quote:
				case ExpressionType.Convert:
					Visit(u.Operand);
					break;
				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
			}
			return u;
		}

		
		protected override Expression VisitBinary(BinaryExpression b)
		{
			sb.Append("(");
			_hasvalueexpression = false;
			Visit(b.Left);
			_nullconstant = false;

			var cursb = sb;
			sb = new StringBuilder();
			Visit(b.Right);

			if (_nullconstant)
			{
				cursb.Append(" IS ");
				if (b.NodeType == ExpressionType.NotEqual)
					cursb.Append("NOT ");
			}
			else
			{
				switch (b.NodeType)
				{
					case ExpressionType.And:
					case ExpressionType.AndAlso:
						cursb.Append(" AND ");
						break;
					case ExpressionType.Or:
					case ExpressionType.OrElse:
						cursb.Append(" OR ");
						break;
					case ExpressionType.Equal:
						cursb.Append(_hasvalueexpression ? " IS " : " = ");
						break;
					case ExpressionType.NotEqual:
						cursb.Append(" <> ");
						break;
					case ExpressionType.LessThan:
						cursb.Append(" < ");
						break;
					case ExpressionType.LessThanOrEqual:
						cursb.Append(" <= ");
						break;
					case ExpressionType.GreaterThan:
						cursb.Append(" > ");
						break;
					case ExpressionType.GreaterThanOrEqual:
						cursb.Append(" >= ");
						break;
					case ExpressionType.Add:
						cursb.Append(" + ");
						break;
					default:
						throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
				}
			}
			cursb.Append(sb);
			sb = cursb;

			sb.Append(")");
			_nullconstant = false;
			return b;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Value == null)
			{
				_nullconstant = true;
				sb.Append("NULL");
				return c;
			}

			if (c.Value is IQueryable)
				return c;

			var t = c.Value.GetType();
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Boolean:
					if (_hasvalueexpression)
						sb.Append((bool)c.Value ? "NOT NULL" : "NULL");
					else
					{
						sb.Append(ConvertConstantToParm((bool)c.Value));
					}
					break;

				case TypeCode.String:
				case TypeCode.DateTime:
					sb.Append(_beforeConstant);
					sb.Append(ConvertConstantToParm(c.Value));
					sb.Append(_afterConstant);
					break;

				case TypeCode.Object:
					if (t != typeof(Guid))
						throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
					sb.Append(ConvertConstantToParm(c.Value));
					break;
				default:
					sb.Append(ConvertConstantToParm(c.Value));
					break;
			}

			return c;
		}

		string ConvertConstantToParm(object constant)
		{
			var name = "p" + _parms.Count;
			_parms.Add(name, constant);
			return "@" + name;
		}

		protected override Expression VisitMember(MemberExpression m)
		{
			if (m.Expression?.NodeType == ExpressionType.Parameter ||
				(m.NodeType == ExpressionType.MemberAccess && m.Expression?.NodeType == ExpressionType.Convert))
			{
				var name = m.Member is PropertyInfo pi && 
					m.Member.DeclaringType.GetCustomAttribute<BaseNamingConventionsAttribute>() != null ? 
					QueryHelper.GetPropertyName(pi) : m.Member.Name;

				sb.Append(name == "Key" ? "@@KEY" : name);
				return m;
			}

			if (m.Expression?.NodeType == ExpressionType.Constant)
			{
				var obj = (m.Expression as ConstantExpression).Value;
				var val = m.Member is FieldInfo f ? f.GetValue(obj) :
					m.Member is PropertyInfo p ? p.GetValue(obj) :
					throw new Exception("Unknown member type " + m.Member.GetType().Name);
				sb.Append(_beforeConstant);
				sb.Append(ConvertConstantToParm(val));
				sb.Append(_afterConstant);
				return m;
			}

			if (m.Expression?.NodeType == ExpressionType.MemberAccess)
			{
				switch (m.Member.Name)
				{
					case "HasValue":
						_hasvalueexpression = true;
						Visit(m.Expression);
						return m;
				}

				MemberExpression m2 = (MemberExpression)m.Expression;
				ConstantExpression captureConst = (ConstantExpression)m2.Expression;
				object obj = m2.Member is FieldInfo f ? f.GetValue(captureConst.Value) :
					m2.Member is PropertyInfo p ? p.GetValue(captureConst.Value) :
					throw new Exception("Unknown member type " + m2.Member.GetType().Name);
				object val = ((PropertyInfo)m.Member).GetValue(obj, null);
				sb.Append(ConvertConstantToParm(val));
				return m;
			}

			if (m.NodeType == ExpressionType.MemberAccess && m.Expression == null)
			{
				if (m.Member is FieldInfo fi)
					if (fi.IsStatic)
						sb.Append(ConvertConstantToParm(fi.GetValue(null)));
				return m;
			}

			throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
		}

		protected void ParseContainsMethod(MethodCallExpression m)
		{
			if (m.Arguments[0].Type == typeof(string))
			{
				Visit(m.Object);
				sb.Append($" {_dialect.LikeKeyword} ");
				_beforeConstant = $"'%'{_dialect.Concat}";
				_afterConstant = $"{_dialect.Concat}'%'";
				Visit(m.Arguments[0]);
				_beforeConstant = "";
				_afterConstant = "";
			}
			else if (m.Arguments[0].Type.IsArray || typeof(IEnumerable).IsAssignableFrom(m.Arguments[0].Type))
			{
				Visit(m.Arguments[1]);
				sb.Append($" {_dialect.In} ");
				_beforeConstant = _dialect.BracketsForIn ? "(" : "";
				_afterConstant = _dialect.BracketsForIn ? ")" : "";
				Visit(m.Arguments[0]);
				_beforeConstant = "";
				_afterConstant = "";
			}
			else
			{
				Visit(m.Arguments[0]);
				sb.Append($" {_dialect.In} ");
				_beforeConstant = _dialect.BracketsForIn ? "(" : "";
				_afterConstant = _dialect.BracketsForIn ? ")" : "";
				Visit(m.Object);
				_beforeConstant = "";
				_afterConstant = "";
			}
		}

		protected void ParseStartsWithMethod(MethodCallExpression m)
		{
			Visit(m.Object);
			sb.Append($" {_dialect.LikeKeyword} ");
			_afterConstant = $"{_dialect.Concat}'%'"; 
			Visit(m.Arguments[0]);
			_afterConstant = "";
		}

		protected void ParseEndsWithMethod(MethodCallExpression m)
		{
			Visit(m.Object);
			sb.Append($" {_dialect.LikeKeyword} ");
			_beforeConstant = $"'%'{_dialect.Concat}";
			Visit(m.Arguments[0]);
			_beforeConstant = "";
		}

		protected void ParseSubstringMethod(MethodCallExpression m)
		{
			sb.Append(" substring(");
			Visit(m.Object);
			sb.Append(",");
			Visit(m.Arguments[0]);
			sb.Append(",");
			Visit(m.Arguments[1]);
			sb.Append(")");
		}

        protected void ParseToStringMethod(MethodCallExpression m)
        {
            sb.Append(" cast(");
            Visit(m.Object);
            if (_dialect is QueryTranslatorPostgres)
                sb.Append(" as text)");
            else
                sb.Append(" as nvarchar(max))");
        }

		private void ParseOrderByExpression(MethodCallExpression expression, string order)
		{
			sb = new StringBuilder();

			Visit(expression.Arguments[1]);
			if (!sbOrder.Any(o => o.ToString().StartsWith(sb.ToString())))
			{
				sbOrder.Insert(0, sb);
				sb.Append(" " + order);
			}
		}

		private bool ParseTakeExpression(MethodCallExpression expression)
		{
			ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

			if (int.TryParse(sizeExpression.Value.ToString(), out int size))
			{
				_parms.Add("take", size);
				return true;
			}

			return false;
		}

		private bool ParseSkipExpression(MethodCallExpression expression)
		{
			ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

			if (int.TryParse(sizeExpression.Value.ToString(), out int size))
			{
				_parms.Add("skip", size);
				return true;
			}

			return false;
		}
	}

	public static class QueryHelper
	{
		public static (string query, IReadOnlyDictionary<string, object> args) ApplyExpressionToQuery(string query, Expression expression, IQueryTranslatorDialect dialect)
		{
			var translator = new QueryTranslator(dialect);
			translator.Translate(expression);
			if (!translator.WhereClause.IsEmpty()) query += " where " + translator.WhereClause;
			if (!translator.GroupBy.IsEmpty()) query = $"select {translator.GroupBySelector} from ({query}) t group by {translator.GroupBy} ";
			if (!translator.OrderBy.IsEmpty()) query += " order by " + translator.OrderBy;

			var hasskip = translator.Parms.ContainsKey("skip");
			var hastake = translator.Parms.ContainsKey("take");

			if (dialect is QueryTranslatorPostgres)
			{
				if (hastake) query += " limit @take";
				if (hasskip) query += " offset @skip";
			}
			else if (dialect is QueryTranslatorMSSQL)
			{
				if (hasskip || hastake)
				{
					if (translator.OrderBy.IsEmpty()) query += " order by (select null) ";
					if (hasskip)
						query += " offset @skip rows";
					else
						query += " offset 0 rows";
					if (hastake) query += " fetch next @take rows only";
				}
			}

			return (query, translator.Parms);
		}

		public static string SetNewFieldExpression(string query, string fieldExpression)
		{
			var i = query.IndexOf("--#select");
			if (i == -1)
				return $"select {fieldExpression} from ({query}) t";
			else
			{
				var part1 = query.Substring(0, i);
				var part2 = query.Substring(i + 9);
				return $"{part1} select {fieldExpression} from ({part2}) t";
			}
		}

		public static PropertyInfo GetPropertyByName(Type t, string name)
		{
			name = name.ToLower();

			if (name.EndsWith(DBConventions.IDSuffix.ToLower()))
			{
				name = name.Substring(0, name.Length - DBConventions.IDSuffix.Length) + BaseNamingConventions.IDSuffix;
				var pid = t.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (pid != null)
					return pid;
			}
			if (name.EndsWith(DBConventions.GUIDSuffix.ToLower()))
			{
				name = name.Substring(0, name.Length - DBConventions.GUIDSuffix.Length) + BaseNamingConventions.GUIDSuffix;
				var pguid = t.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (pguid != null)
					return pguid;
			}

			var p = t.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (p != null)
				return p;

			throw new PropertyInfoNotFoundException($"В модели {t.Name} отсутствует свойство {name}");
		}

		public static string GetPropertyName(PropertyInfo p)
		{
			var name = p.Name;

			if (p.PropertyType == typeof(Guid) || p.PropertyType == typeof(Guid?))
			{
				if (name != BaseNamingConventions.GUIDSuffix && name.EndsWith(BaseNamingConventions.GUIDSuffix) && !name.EndsWith(DBConventions.GUIDSuffix))
					return name.Substring(0, name.Length - BaseNamingConventions.GUIDSuffix.Length) + DBConventions.GUIDSuffix;
			}
			else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) ||
				p.PropertyType == typeof(long) || p.PropertyType == typeof(long?) || p.PropertyType == typeof(object)) // TODO: Надо проверить для lastmodifieduserid
			{
				if (name != BaseNamingConventions.IDSuffix && name.EndsWith(BaseNamingConventions.IDSuffix) && !name.EndsWith(DBConventions.IDSuffix))
					return name.Substring(0, name.Length - BaseNamingConventions.IDSuffix.Length) + DBConventions.IDSuffix;
			}

			return name;
		}

		public static string GetStringValue(object value)
		{
			if (value is Guid guid)
				return $"'{guid}'";

			if (value is string str)
				return $"'{str}'";

			if (value is DateTime dateTime)
				return $"'{dateTime.DateTimeToStringISO8601()}'";

			if (value is byte[] bytes)
			{
				return $"{bytes.ToBlobLiterals()}";
			}

			return value.ToString();
		}

		public static IQueryTranslatorDialect CreateDialect(DBType dbType) => dbType == DBType.MSSQL ? (IQueryTranslatorDialect)new QueryTranslatorMSSQL() :
			dbType == DBType.POSTGRESQL ? new QueryTranslatorPostgres() :
			throw new NotSupportedException();
	}

	public interface IQueryTranslatorDialect
	{
		string LikeKeyword { get; }
		string Concat { get; }
		string In { get; }
		bool BracketsForIn { get; }
		string ReturningIdentity(string identityName, string returningIDVariable);
        string InsertDefault { get; }
		string DeclareVariable { get; }
		string VariablePrefix { get; }
		string GetDBType(Type type);

	}

	public class QueryTranslatorMSSQL : IQueryTranslatorDialect
	{
        public string InsertDefault => @"insert into {0} default values";
        
        public string LikeKeyword => "LIKE";
		public string Concat => "+";
		public string In => "IN";
		public bool BracketsForIn => false;

        public string DeclareVariable => "DECLARE {0}{1} {2};";

		public string VariablePrefix => "@";

        public string GetDBType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Int32:
					return "int";
				case TypeCode.Object when type == typeof(Guid):
					return "uniqueidentifier";
				case TypeCode.Int64:
					return "bigint";
				default:
					throw new NotSupportedException($"Unsupported type {type.Name}");
			}
		}

        public string ReturningIdentity(string identityName, string returningIDVariable)
        {
			if (string.IsNullOrEmpty(returningIDVariable))
				return "select SCOPE_IDENTITY()";
			else
				return $"select {VariablePrefix}{returningIDVariable} = SCOPE_IDENTITY()";
		}
    }

	public class QueryTranslatorPostgres : IQueryTranslatorDialect
	{
        public string InsertDefault => @"insert into {0} values(default)";
		public string LikeKeyword => "ILIKE";
		public string Concat => "||";
		public string In => "= ANY";
		public bool BracketsForIn => true;

        public string DeclareVariable => "DECLARE {0}{1} {2};";

        public string VariablePrefix => "";

        public string GetDBType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Int32:
					return "int";
				case TypeCode.Object when type == typeof(Guid):
					return "uuid";
				case TypeCode.Int64:
					return "bigint";
				default:
					throw new NotSupportedException($"Unsupported type {type.Name}");
			}
		}

        public string ReturningIdentity(string identityName, string returningIDVariable)
        {
			if (string.IsNullOrEmpty(returningIDVariable))
				return $"RETURNING {identityName}";
			else
				return $"RETURNING {identityName} INTO {VariablePrefix}{returningIDVariable}";
		}
    }
}