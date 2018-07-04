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

			internal override string GetBoolValue(bool value)
			{
				return value ? "true" : "false";
			}

			//internal override SqlExpression VisitVariable(SqlVariable v)
			//{
			//	sb.Append("'" + v.Name + "'");
			//	return v;
			//}

			internal override SqlBlock VisitBlock(SqlBlock block)
			{
				for (int i = 0, n = block.Statements.Count; i < n; i++)
				{
					this.Visit(block.Statements[i]);
					sb.Append(";");
					if (i < n - 1)
					{
						SqlSelect select = block.Statements[i + 1] as SqlSelect;
						if (select == null || !select.DoNotOutput)
						{
							this.NewLine();
							this.NewLine();
						}
					}
				}
				return block;
			}

			internal override SqlExpression VisitUnaryOperator(SqlUnary uo)
			{
				switch (uo.NodeType)
				{
					case SqlNodeType.Not:
					case SqlNodeType.Not2V:
						this.sb.Append(GetOperator(uo.NodeType));
						this.sb.Append(" ");
						this.VisitWithParens(uo.Operand, uo);
						break;
					case SqlNodeType.Negate:
					case SqlNodeType.BitNot:
						this.sb.Append(GetOperator(uo.NodeType));
						this.VisitWithParens(uo.Operand, uo);
						break;
					case SqlNodeType.Count:
					case SqlNodeType.LongCount:
					case SqlNodeType.Max:
					case SqlNodeType.Min:
					case SqlNodeType.Sum:
					case SqlNodeType.Avg:
					case SqlNodeType.Stddev:
					case SqlNodeType.ClrLength:
						{
							this.sb.Append(GetOperator(uo.NodeType));
							this.sb.Append("(");
							if (uo.Operand == null)
							{
								this.sb.Append("*");
							}
							else
							{
								this.Visit(uo.Operand);
							}
							this.sb.Append(")");
							break;
						}
					case SqlNodeType.IsNull:
					case SqlNodeType.IsNotNull:
						{
							this.VisitWithParens(uo.Operand, uo);
							sb.Append(" ");
							sb.Append(GetOperator(uo.NodeType));
							break;
						}
					case SqlNodeType.Convert:
						{
							if (ToQueryString(uo.Operand.SqlType) == ToQueryString(uo.SqlType))
							{
								this.Visit(uo.Operand);
							}
							else
							{
								this.sb.Append("CAST(");
								this.Visit(uo.Operand);
								this.sb.Append(" AS ");
								this.sb.Append(ToQueryString(uo.SqlType));
								this.sb.Append(")");
							}
							break;
						}
					case SqlNodeType.ValueOf:
					case SqlNodeType.OuterJoinedValue:
						this.Visit(uo.Operand); // no op
						break;
					default:
						throw Error.InvalidFormatNode(uo.NodeType);
				}
				return uo;
			}

			internal string ToQueryString(ProviderType sqlType)
			{
				var sqlDbType = (sqlType as SqlTypeSystem.SqlType).SqlDbType;
				StringBuilder sb = new StringBuilder();

				switch (sqlDbType)
				{
					case SqlDbType.BigInt:
					case SqlDbType.Bit:
					case SqlDbType.Date:
					case SqlDbType.Time:
					case SqlDbType.DateTime:
					case SqlDbType.DateTime2:
					case SqlDbType.DateTimeOffset:
					case SqlDbType.Int:
					case SqlDbType.Money:
					case SqlDbType.SmallDateTime:
					case SqlDbType.SmallInt:
					case SqlDbType.SmallMoney:
					case SqlDbType.Timestamp:
					case SqlDbType.TinyInt:
					case SqlDbType.UniqueIdentifier:
					case SqlDbType.Xml:
					case SqlDbType.Image:
					case SqlDbType.NText:
					case SqlDbType.Text:
					case SqlDbType.Udt:
						sb.Append(sqlDbType.ToString());
						break;
					case SqlDbType.Variant:
						sb.Append("sql_variant");
						break;
					case SqlDbType.Binary:
					case SqlDbType.Char:
					case SqlDbType.NChar:
						sb.Append(sqlDbType);
						break;
					case SqlDbType.NVarChar:
					case SqlDbType.VarBinary:
					case SqlDbType.VarChar:
						sb.Append("text");
						break;
					case SqlDbType.Decimal:
					case SqlDbType.Float:
					case SqlDbType.Real:
						sb.Append(sqlDbType);
						break;
				}
				return sb.ToString();
			}
		}
	}
}
