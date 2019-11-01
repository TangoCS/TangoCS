using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Tango
{
	public static class Enumerations
	{
		public static string GetEnumDescription(Enum value)
		{
			return value.ToString().Split(',').Select(s => {
				var fi = value.GetType().GetField(s.Trim());
				if (fi == null) return "";
				var attributes = fi.GetCustomAttributes<DescriptionAttribute>(false);
				return (attributes != null && attributes.Count() > 0) ? attributes.First().Description : value.ToString();
			}).Join(", ");
		}

		public static IEnumerable<SelectListItem> AsSelectList<T>()
			where T: Enum
		{
			var ut = Enum.GetUnderlyingType(typeof(T));
			return Enum.GetValues(typeof(T))
				.Cast<T>()
				.Select(v => new SelectListItem(
					GetEnumDescription(v),
					Convert.ChangeType(v, ut).ToString()
				));
		}
        public static IEnumerable<SelectListItem> AsSelectListWithSelected<T>(T selected)
            where T : Enum
        {
            var ut = Enum.GetUnderlyingType(typeof(T));
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(v => new SelectListItem(
                    GetEnumDescription(v),
                    Convert.ChangeType(v, ut).ToString(), v.Equals(selected) ? true : false
                ));
        }
    }
}
