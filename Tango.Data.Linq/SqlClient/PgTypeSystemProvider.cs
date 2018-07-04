using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Linq.SqlClient
{
	internal static partial class SqlTypeSystem
	{
		internal static TypeSystemProvider CreatePgProvider()
		{
			return new PgProvider();
		}

		class PgProvider : Sql2008Provider
		{
			internal override ProviderType PredictTypeForBinary(SqlNodeType binaryOp, ProviderType leftType, ProviderType rightType)
			{
				SqlType highest;

				if (leftType.IsSameTypeFamily(this.From(typeof(string))) && rightType.IsSameTypeFamily(this.From(typeof(string))))
				{
					highest = (SqlType)this.GetBestType(leftType, rightType);
				}
				else
				{
					int coercionPrecedence = leftType.ComparePrecedenceTo(rightType);
					highest = (SqlType)(coercionPrecedence > 0 ? leftType : rightType);
				}

				switch (binaryOp)
				{
					case SqlNodeType.Add:
					case SqlNodeType.Sub:
					case SqlNodeType.Mul:
					case SqlNodeType.Div:
					case SqlNodeType.BitAnd:
					case SqlNodeType.BitOr:
					case SqlNodeType.BitXor:
					case SqlNodeType.Mod:
					case SqlNodeType.Coalesce:
						return highest;
					case SqlNodeType.Concat:
						// When concatenating two types with size, the result type after
						// concatenation must have a size equal to the sum of the two sizes.
						if (highest.HasSizeOrIsLarge)
						{
							// get the best type, specifying null for size so we get
							// the maximum allowable size
							ProviderType concatType = this.GetBestType(highest.SqlDbType, null);

							if ((!leftType.IsLargeType && leftType.Size.HasValue) &&
								(!rightType.IsLargeType && rightType.Size.HasValue))
							{
								// If both types are not large types and have size, and the
								// size is less than the default size, return the shortened type.
								int concatSize = leftType.Size.Value + rightType.Size.Value;
								if ((concatSize < concatType.Size) || concatType.IsLargeType)
								{
									return GetBestType(highest.SqlDbType, concatSize);
								}
							}

							return concatType;
						}
						return highest;
					case SqlNodeType.And:
					case SqlNodeType.Or:
					case SqlNodeType.LT:
					case SqlNodeType.LE:
					case SqlNodeType.GT:
					case SqlNodeType.GE:
					case SqlNodeType.EQ:
					case SqlNodeType.NE:
					case SqlNodeType.EQ2V:
					case SqlNodeType.NE2V:
						return theBit;
					default:
						throw Error.UnexpectedNode(binaryOp);
				}
			}

			internal override ProviderType From(object o)
			{
				Type clrType = (o != null) ? o.GetType() : typeof(object);
				if (clrType == typeof(string))
				{
					string str = (string)o;
					return From(clrType, str.Length);
				}
				//else if (clrType == typeof(bool))
				//{
				//	return From(typeof(int));
				//}
				else if (clrType.IsArray)
				{
					Array arr = (Array)o;
					return From(clrType, arr.Length);
				}
				else if (clrType == typeof(decimal))
				{
					decimal d = (decimal)o;
					// The CLR stores the scale of a decimal value in bits
					// 16 to 23 (i.e., mask 0x00FF0000) of the fourth int. 
					int scale = (Decimal.GetBits(d)[3] & 0x00FF0000) >> 16;
					return From(clrType, scale);
				}
				else
				{
					return From(clrType);
				}
			}
		}
	}
}
