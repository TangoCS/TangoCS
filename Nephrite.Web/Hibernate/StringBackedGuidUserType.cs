using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Nephrite.Web.Hibernate
{
	public class StringBackedGuidUserType : IUserType
	{
		public bool Equals(object x, object y)
		{
			return true;
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

		public SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.GetString(36) }; }
		}

		public Type ReturnedType
		{
			get { return typeof(Guid?); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{

			NHibernateUtil.String.NullSafeSet(cmd, value == null ? null : value.ToString(), index);
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			return NHibernateUtil.Guid.NullSafeGet(rs, names[0]);
		}
	}
}