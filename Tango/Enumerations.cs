using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Tango
{
    public class ShortDescriptionAttribute : Attribute
    {
        public ShortDescriptionAttribute(string description)
        {
            Description = description;
        }
        public string Description { get; set; }
    }
    
	public static class Enumerations
	{
        private static string GetDescriptionAttribute(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return "";
            var attributes = fi.GetCustomAttributes<DescriptionAttribute>(false);
            return (attributes != null && attributes.Count() > 0) ? attributes.First().Description : null;
        }
        
        private static string GetShortDescriptionAttribute(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return "";
            var attributes = fi.GetCustomAttributes<ShortDescriptionAttribute>(false);
            return (attributes != null && attributes.Any()) ? attributes.First().Description : null;
        }

        public static string GetEnumDescription(Enum value)
        {
            var p = GetDescriptionAttribute(value);
            return p ?? value.ToString();
        }
        
        public static string GetEnumShortDescription(Enum value)
        {
            var p = GetShortDescriptionAttribute(value);
            return p ?? value.ToString();
        }

        public static IEnumerable<SelectListItem> AsSelectList<T>(Func<T, bool> predicate = null)
            where T : Enum
        {
            var ut = Enum.GetUnderlyingType(typeof(T));
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(predicate ?? (x => true))
				.Select(v => new SelectListItem(
                    GetDescriptionAttribute(v),
                    Convert.ChangeType(v, ut).ToString()
                )).Where(x => x.Text != null);
        }

		//для конструкций вида
		//[TAttr(TAEnum.Val1, TAEnum.Val2)]
		//enum member of T
		public static IEnumerable<SelectListItem> AsSelectList<T, TAttr, TAEnum>(Func<TAttr, TAEnum, bool> filter, TAEnum fVal)
			where T : Enum
			where TAttr : Attribute
			where TAEnum : Enum
		{
			var ut = Enum.GetUnderlyingType(typeof(T));
			return Enum.GetValues(typeof(T))
				.Cast<T>()
				.Where(ll => { var cattr = ll.GetType().GetField(ll.ToString()).GetCustomAttribute<TAttr>(false); return cattr != null ? filter(cattr, fVal) : true; })
				.Select(v => new SelectListItem(
					GetDescriptionAttribute(v),
					Convert.ChangeType(v, ut).ToString()
				)).Where(x => x.Text != null);
		}
	}
}
