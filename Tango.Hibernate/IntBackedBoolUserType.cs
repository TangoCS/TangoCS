using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Tango.Hibernate
{
	public class IntBackedBoolUserType : BooleanType, IUserType
	{
		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y)) return true;

			if (x == null || y == null) return false;

			return x.Equals(y);
		}

		public object DeepCopy(object value)
		{
			GetHashCode(value);
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

		public override void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			var val = !((bool)value) ? 0 : 1;
			NHibernateUtil.Int32.NullSafeSet(cmd, val, index, session);
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
		{
			return NHibernateUtil.Boolean.NullSafeGet(rs, name, session);
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var val = value is int ? value : !((bool)value) ? 0 : 1;
			cmd.Parameters[index].Value = val;
		}
	}
}