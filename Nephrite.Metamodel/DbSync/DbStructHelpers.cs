using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.DbStruct
{
	public static class DbStructHelpers
	{
		public static Table Get(this List<Table> tlist, string name)
		{
			return tlist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static ForeignKey Get(this List<ForeignKey> fklist, string name)
		{
			return fklist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static View Get(this List<View> vlist, string name)
		{
			return vlist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static StoredProcedure Get(this List<StoredProcedure> splist, string name)
		{
			return splist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static StoredFunction Get(this List<StoredFunction> sflist, string name)
		{
			return sflist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static Index Get(this List<Index> ilist, string name)
		{
			return ilist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static Trigger Get(this List<Trigger> trlist, string name)
		{
			return trlist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}

		public static TableColumn Get(this List<TableColumn> tclist, string name)
		{
			return tclist.SingleOrDefault(o => o.Name.ToLower() == name.ToLower());
		}
	}

	partial class TableColumn
	{
		public string FullType
		{
			get
			{
				if (Type == "char" || Type == "nchar" || Type == "varchar" || Type == "nvarchar")
					return Type + "(" + (MaximumLength == -1 ? "max" : MaximumLength.ToString()) + ")";
				if (Type == "decimal")
					return Type + "(" + Precision + "," + Scale + ")";
				return Type;
			}
		}
	}
}