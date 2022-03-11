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

        public static IEnumerable<SelectListItem> AsSelectList<T>()
            where T : Enum
        {
            var ut = Enum.GetUnderlyingType(typeof(T));
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(v => new SelectListItem(
                    GetDescriptionAttribute(v),
                    Convert.ChangeType(v, ut).ToString()
                )).Where(x => x.Text != null);
        }
    }
}
