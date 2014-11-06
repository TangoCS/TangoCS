using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	public class FolderActionType
	{
		public const string View = "V";
		public const string Get = "G";
		public const string Edit = "E";
		public const string Delete = "D";

		public static string ViewTitle { get { return "Просмотр списка файлов"; } }
		public static string GetTitle { get { return "Скачивание файлов"; } }
		public static string EditTitle { get { return "Загрузка/редактирование файлов"; } }
		public static string DeleteTitle { get { return "Удаление файлов"; } }
		
		protected static List<CodifierValue> _list = null;
		public static List<CodifierValue> ToList()
		{
			if (_list == null)
			{
				_list = new List<CodifierValue> { 
					new CodifierValue { Code = View, Title = ViewTitle }, 
					new CodifierValue { Code = Get, Title = GetTitle }, 
					new CodifierValue { Code = Edit, Title = EditTitle }, 
					new CodifierValue { Code = Delete, Title = DeleteTitle }
				};
			}
			return _list;
		}

		public static string Title(string code)
		{
			CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
			return res != null ? res.Title : "";
		}
	}
}
