using System.Text;

namespace System.Data.Linq.SqlClient
{
	internal class PgSqlFormatter : SqlFormatter
	{
		public PgSqlFormatter()
		{
			visitor = new PgVisitor();
		}

		internal class PgVisitor : Visitor
		{
			internal override SqlExpression VisitLike(SqlLike like)
			{
				this.VisitWithParens(like.Expression, like);
				sb.Append(" ILIKE ");
				this.Visit(like.Pattern);
				if (like.Escape != null)
				{
					sb.Append(" ESCAPE ");
					this.Visit(like.Escape);
				}
				return like;
			}

			internal override string GetOperator(SqlNodeType nt)
			{
				switch (nt)
				{
					case SqlNodeType.Add: return "+";
					case SqlNodeType.Sub: return "-";
					case SqlNodeType.Mul: return "*";
					case SqlNodeType.Div: return "/";
					case SqlNodeType.Mod: return "%";
					case SqlNodeType.Concat: return "||";
					case SqlNodeType.BitAnd: return "&";
					case SqlNodeType.BitOr: return "|";
					case SqlNodeType.BitXor: return "^";
					case SqlNodeType.And: return "AND";
					case SqlNodeType.Or: return "OR";
					case SqlNodeType.GE: return ">=";
					case SqlNodeType.GT: return ">";
					case SqlNodeType.LE: return "<=";
					case SqlNodeType.LT: return "<";
					case SqlNodeType.EQ: return "=";
					case SqlNodeType.EQ2V: return "=";
					case SqlNodeType.NE: return "<>";
					case SqlNodeType.NE2V: return "<>";
					case SqlNodeType.Not: return "NOT";
					case SqlNodeType.Not2V: return "NOT";
					case SqlNodeType.BitNot: return "~";
					case SqlNodeType.Negate: return "-";
					case SqlNodeType.IsNull: return "IS NULL";
					case SqlNodeType.IsNotNull: return "IS NOT NULL";
					case SqlNodeType.Count: return "COUNT";
					case SqlNodeType.LongCount: return "COUNT_BIG";
					case SqlNodeType.Min: return "MIN";
					case SqlNodeType.Max: return "MAX";
					case SqlNodeType.Sum: return "SUM";
					case SqlNodeType.Avg: return "AVG";
					case SqlNodeType.Stddev: return "STDEV";
					case SqlNodeType.ClrLength: return "CLRLENGTH";
					default:
						throw Error.InvalidFormatNode(nt);
				}
			}

			internal override SqlSelect VisitSelect(SqlSelect ss)
			{
				if (ss.DoNotOutput)
				{
					return ss;
				}
				string from = null;
				if (ss.From != null)
				{
					StringBuilder savesb = this.sb;
					this.sb = new StringBuilder();
					if (this.IsSimpleCrossJoinList(ss.From))
					{
						this.VisitCrossJoinList(ss.From);
					}
					else
					{
						this.Visit(ss.From);
					}
					from = this.sb.ToString();
					this.sb = savesb;
				}

				sb.Append("SELECT ");

				if (ss.IsDistinct)
				{
					sb.Append("DISTINCT ");
				}

				if (ss.Row.Columns.Count > 0)
				{
					this.VisitRow(ss.Row);
				}
				else if (this.isDebugMode)
				{
					this.Visit(ss.Selection);
				}
				else
				{
					sb.Append("NULL");
				}

				if (from != null)
				{
					this.NewLine();
					sb.Append("FROM ");
					sb.Append(from);
				}
				if (ss.Where != null)
				{
					this.NewLine();
					sb.Append("WHERE ");
					this.Visit(ss.Where);
				}
				if (ss.GroupBy.Count > 0)
				{
					this.NewLine();
					sb.Append("GROUP BY ");
					for (int i = 0, n = ss.GroupBy.Count; i < n; i++)
					{
						SqlExpression exp = ss.GroupBy[i];
						if (i > 0)
							sb.Append(", ");
						this.Visit(exp);
					}
					if (ss.Having != null)
					{
						this.NewLine();
						sb.Append("HAVING ");
						this.Visit(ss.Having);
					}
				}
				if (ss.OrderBy.Count > 0 && ss.OrderingType != SqlOrderingType.Never)
				{
					this.NewLine();
					sb.Append("ORDER BY ");
					for (int i = 0, n = ss.OrderBy.Count; i < n; i++)
					{
						SqlOrderExpression exp = ss.OrderBy[i];
						if (i > 0)
							sb.Append(", ");
						this.Visit(exp.Expression);
						if (exp.OrderType == SqlOrderType.Descending)
						{
							sb.Append(" DESC");
						}
					}
				}

				if (ss.Top != null)
				{
					this.NewLine();
					sb.Append("LIMIT ");
					this.Visit(ss.Top);
				}

				if (ss.Offset != null)
				{
					this.NewLine();
					sb.Append("OFFSET ");
					this.Visit(ss.Offset);
				}

				return ss;
			}

			internal override void WriteName(string s)
			{
				sb.Append(s);
			}

			internal override void WriteVariableName(string s)
			{
				if (s.StartsWith("@", StringComparison.Ordinal))
					sb.Append(s);
				else
					sb.Append("@" + s);
			}
		}
	}
}
