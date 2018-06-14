using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tango.Data
{
	public class QueryTranslator : ExpressionVisitor
	{
		StringBuilder sb;

		List<StringBuilder> sbWhere = new List<StringBuilder>();
		List<StringBuilder> sbOrder = new List<StringBuilder>();

		string _beforeConstant = "";
		string _afterConstant = "";
		Func<bool, string> _boolConstant = o => o.ToString().ToLower();

		string _orderBy = string.Empty;
		string _whereClause = string.Empty;
		Dictionary<string, object> _parms = new Dictionary<string, object>();

		public string OrderBy => _orderBy;
		public string WhereClause => _whereClause;
		public IReadOnlyDictionary<string, object> Parms => _parms;

		bool _hasvalueexpression = false;

		public void Translate(Expression expression)
		{
			Visit(expression);
			_whereClause = sbWhere.Select(o => o.ToString()).Join(" and ");
			_orderBy = sbOrder.Select(o => o.ToString()).Join(", ");
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
			else if (m.Method.Name == "get_Item")
			{
				sb.Append((m.Arguments[0] as ConstantExpression).Value);
				return m;
			}

			throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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

			var nullcheckconst = b.Right.NodeType == ExpressionType.Constant && ((ConstantExpression)b.Right).Value == null;
			var isnull = nullcheckconst && b.NodeType == ExpressionType.Equal;
			var isnotnull = nullcheckconst && b.NodeType == ExpressionType.NotEqual;

			if (isnull)
				sb.Append(" IS NULL");
			else if (isnotnull)
				sb.Append(" IS NOT NULL");
			else
			{
				switch (b.NodeType)
				{
					case ExpressionType.And:
					case ExpressionType.AndAlso:
						sb.Append(" AND ");
						break;
					case ExpressionType.Or:
					case ExpressionType.OrElse:
						sb.Append(" OR ");
						break;
					case ExpressionType.Equal:
						sb.Append(_hasvalueexpression ? " IS " : " = ");
						break;
					case ExpressionType.NotEqual:
						sb.Append(" <> ");
						break;
					case ExpressionType.LessThan:
						sb.Append(" < ");
						break;
					case ExpressionType.LessThanOrEqual:
						sb.Append(" <= ");
						break;
					case ExpressionType.GreaterThan:
						sb.Append(" > ");
						break;

					case ExpressionType.GreaterThanOrEqual:
						sb.Append(" >= ");
						break;

					default:
						throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

				}
				Visit(b.Right);
			}
			
			sb.Append(")");
			return b;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Value is IQueryable)
				return c;

			switch (Type.GetTypeCode(c.Value.GetType()))
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
					throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

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
			if (m.Expression?.NodeType == ExpressionType.Parameter)
			{
				sb.Append(m.Member.Name);
				return m;
			}

			if (m.Expression?.NodeType == ExpressionType.Constant)
			{
				var val = ((FieldInfo)m.Member).GetValue((m.Expression as ConstantExpression).Value);
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
			}

			throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
		}

		protected void ParseContainsMethod(MethodCallExpression m)
		{		
			if (m.Arguments[0].Type == typeof(string))
			{
				Visit(m.Object);
				sb.Append(" ILIKE ");
				_beforeConstant = "'%'||";
				_afterConstant = "||'%'";
				Visit(m.Arguments[0]);
				_beforeConstant = "";
				_afterConstant = "";
			}
			else
			{
				Visit(m.Arguments[1]);
				sb.Append(" = ANY ");
				_beforeConstant = "(";
				_afterConstant = ")";
				Visit(m.Arguments[0]);
				_beforeConstant = "";
				_afterConstant = "";
			}
		}

		protected void ParseStartsWithMethod(MethodCallExpression m)
		{
			Visit(m.Object);
			sb.Append(" ILIKE ");
			_afterConstant = "||'%'"; 
			Visit(m.Arguments[0]);
			_afterConstant = "";
		}

		protected void ParseEndsWithMethod(MethodCallExpression m)
		{
			Visit(m.Object);
			sb.Append(" ILIKE ");
			_beforeConstant = "'%'||";
			Visit(m.Arguments[0]);
			_beforeConstant = "";
		}

		private void ParseOrderByExpression(MethodCallExpression expression, string order)
		{
			sb = new StringBuilder();
			sbOrder.Add(sb);

			Visit(expression.Arguments[1]);

			sb.Append(" " + order);
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


	public sealed class EnumMapper<TKey, TValue> where TKey : struct, IConvertible
	{
		private struct FlaggedValue<T>
		{
			public bool flag;
			public T value;
		}

		private static readonly int size;
		private readonly Func<TKey, int> func;
		private FlaggedValue<TValue>[] flaggedValues;

		public TValue this[TKey key]
		{
			get
			{
				int index = this.func.Invoke(key);
				FlaggedValue<TValue> flaggedValue = this.flaggedValues[index];
				if (flaggedValue.flag == false)
				{
					ThrowNoMappingException(); // Don't want the exception code in the method. Make this callsite as small as possible to promote JIT inlining and squeeze out every last bit of performance.
				}

				return flaggedValue.value;
			}
		}

		static EnumMapper()
		{
			Type keyType = typeof(TKey);

			if (keyType.IsEnum == false)
			{
				throw new Exception("The key type [" + keyType.AssemblyQualifiedName + "] is not an enumeration.");
			}

			Type underlyingType = Enum.GetUnderlyingType(keyType);
			if (underlyingType != typeof(int))
			{
				throw new Exception("The key type's underlying type [" + underlyingType.AssemblyQualifiedName + "] is not a 32-bit signed integer.");
			}

			var values = (int[])Enum.GetValues(keyType);
			int maxValue = 0;

			foreach (int value in values)
			{
				if (value < 0)
				{
					throw new Exception("The key type has a constant with a negative value.");
				}

				if (value > maxValue)
				{
					maxValue = value;
				}
			}

			size = maxValue + 1;
		}

		public EnumMapper(Func<TKey, int> func)
		{
			this.func = func ?? throw new ArgumentNullException("func", "The func cannot be a null reference.");
			this.flaggedValues = new FlaggedValue<TValue>[EnumMapper<TKey, TValue>.size];
		}

		public static EnumMapper<TKey, TValue> Construct(Func<TKey, int> func)
		{
			return new EnumMapper<TKey, TValue>(func);
		}

		public EnumMapper<TKey, TValue> Map(TKey key, TValue value)
		{
			int index = this.func.Invoke(key);

			FlaggedValue<TValue> flaggedValue;
			flaggedValue.flag = true;
			flaggedValue.value = value;
			this.flaggedValues[index] = flaggedValue;

			return this;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowNoMappingException()
		{
			throw new Exception("No mapping exists corresponding to the key.");
		}
	}
}
