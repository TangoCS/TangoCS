using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
    public class MenuItemType
    {
        public const string Form = "F";
		public const string Url = "U";
		public const string None = "N";
		public const string External = "E";
		public const string View = "V";

        public static string FormTitle = "Метод";
        public static string UrlTitle = "URL";
        public static string NoneTitle = "Без ссылки";
        public static string ExternalTitle = "Внешний элемент управления";
        public static string ViewTitle = "Представление пакета";

        protected static List<CodifierValue> _list = null;
        public static List<CodifierValue> ToList()
        {
            if (MenuItemType._list == null)
            {
                MenuItemType._list = new List<CodifierValue> { 
					new CodifierValue { Code = MenuItemType.Form, Title = MenuItemType.FormTitle },
					new CodifierValue { Code = MenuItemType.Url, Title = MenuItemType.UrlTitle },
					new CodifierValue { Code = MenuItemType.None, Title = MenuItemType.NoneTitle },
					new CodifierValue { Code = MenuItemType.External, Title = MenuItemType.ExternalTitle },
					new CodifierValue { Code = MenuItemType.View, Title = MenuItemType.ViewTitle }
				};
            }
            return MenuItemType._list;
        }

		public static string Title(string code)
        {
            CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
            return res != null ? res.Title : "";
        }
    }
}
