using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Nephrite.Meta
{
	public static class ToStringConverter
	{
		public static Func<decimal, string, IFormatProvider, string> Decimal
		{
			get { return (val, format, provider) => val.ToString(format, provider); }
		}
		public static Func<decimal?, string, IFormatProvider, string> NullableDecimal
		{
			get { return (val, format, provider) => val == null ? "" : val.Value.ToString(format, provider); }
		}
		public static Func<string, string, IFormatProvider, string> String
		{
			get { return (val, format, provider) => val; }
		}
		public static Func<DateTime, string, IFormatProvider, string> DateTime
		{
			get { return (val, format, provider) => val.DateTimeToString(); }
		}
		public static Func<DateTime?, string, IFormatProvider, string> NullableDateTime
		{
			get { return (val, format, provider) => val.DateTimeToString(); }
		}
		public static Func<DateTime, string, IFormatProvider, string> Date
		{
			get { return (val, format, provider) => val.DateToString(); }
		}
		public static Func<DateTime?, string, IFormatProvider, string> NullableDate
		{
			get { return (val, format, provider) => val.DateToString(); }
		}
		public static Func<int, string, IFormatProvider, string> Int
		{
			get { return (val, format, provider) => val.ToString(format, provider); }
		}
		public static Func<int?, string, IFormatProvider, string> NullableInt
		{
			get { return (val, format, provider) => val == null ? "" : val.Value.ToString(format, provider); }
		}
		public static Func<long, string, IFormatProvider, string> Long
		{
			get { return (val, format, provider) => val.ToString(format, provider); }
		}
		public static Func<long?, string, IFormatProvider, string> NullableLong
		{
			get { return (val, format, provider) => val == null ? "" : val.Value.ToString(format, provider); }
		}

		public static Func<Guid, string, IFormatProvider, string> Guid
		{
			get { return (val, format, provider) => val.ToString(format, provider); }
		}
		public static Func<Guid?, string, IFormatProvider, string> NullableGuid
		{
			get { return (val, format, provider) => val == null ? "" : val.Value.ToString(format, provider); }
		}

		public static Func<bool, string, IFormatProvider, string> Bool
		{
			get { return (val, format, provider) => val.Icon(); }
		}
		public static Func<bool?, string, IFormatProvider, string> NullableBool
		{
			get { return (val, format, provider) => val.Icon(); }
		}

		public static Func<XDocument, string, IFormatProvider, string> Xml
		{
			get { return (val, format, provider) => val.ToString(); }
		}
	}
}