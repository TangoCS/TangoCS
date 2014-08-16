using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Nephrite.Web.Hibernate
{
	public class IntBackedBoolUserType : BooleanType, IUserType
	{
		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y)) return true;

			if (x == null || y == null) return false;

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public override SqlType SqlType
		{
			get
			{
				return new SqlType(DbType.Int32);
			}
		}

		public new SqlType[] SqlTypes
		{
			get { return new SqlType[] { SqlType }; }
		}

		public Type ReturnedType
		{
			get { return typeof(bool); }
		}

		public new void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			var val = !((bool)value) ? 0 : 1;
			NHibernateUtil.Int32.NullSafeSet(cmd, val, index);
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			return NHibernateUtil.Boolean.NullSafeGet(rs, names[0]);
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			var val = value is int ? value : !((bool)value) ? 0 : 1;
			((IDataParameter)cmd.Parameters[index]).Value = val;
		}
	}
}