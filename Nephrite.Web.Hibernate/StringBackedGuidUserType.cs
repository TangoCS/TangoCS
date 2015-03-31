using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Mapping.ByCode.Impl;

namespace Nephrite.Web.Hibernate
{
	public class StringBackedGuidUserType : PrimitiveType, IDiscriminatorType, IUserType
	{
		public StringBackedGuidUserType()
			: base(SqlTypeFactory.GetString(36))
		{
		}

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

		public new SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.GetString(36) }; }
		}

		public Type ReturnedType
		{
			get { return typeof(Guid); }
		}

		public new void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			var val = value == null ? (object)DBNull.Value : value.ToString();
			((IDataParameter)cmd.Parameters[index]).Value = val;
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			return NHibernateUtil.Guid.NullSafeGet(rs, names[0]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			if (rs.GetFieldType(index) == typeof(Guid))
			{
				return rs.GetGuid(index);
			}

			if (rs.GetFieldType(index) == typeof(byte[]))
			{
				return new Guid((byte[])(rs[index]));
			}

			return new Guid(Convert.ToString(rs[index]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(Guid); }
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			var dp = (IDataParameter)cmd.Parameters[index];
			dp.Value = value.ToString();
		}

		/// <summary></summary>
		public override string Name
		{
			get { Type type = GetType(); return type.FullName + ", " + type.Assembly.GetName().Name; }
		}

		public override object FromStringValue(string xml)
		{
			return new Guid(xml);
		}

		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(Guid); }
		}

		public override object DefaultValue
		{
			get { return Guid.Empty; }
		}

		public override string ObjectToSQLString(object value, NHibernate.Dialect.Dialect dialect)
		{
			return "'" + value + "'";
		}
	}
}