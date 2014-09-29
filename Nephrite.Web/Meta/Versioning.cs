using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Meta
{
	public class SVersioning : MetaStereotype
	{
		public SVersioning(string versioningType)
		{
			VersioningType = versioningType;
		}
		public string VersioningType { get; set; }
	}

	public static class VersioningType
	{
		public const string None = "N";
		public static string NoneTitle = "Нет";

		public const string Object = "O";
		public static string ObjectTitle = "Версионность объектов";

		public const string IdentifiersRetain = "R";
		public static string IdentifiersRetainTitle = "Версионность справочника с сохранением идентификаторов";

		public const string IdentifiersMiss = "M";
		public static string IdentifiersMissTitle = "Версионность справочника без сохранения идентификаторов";

		public static List<CodifierValue> ToList()
		{
			if (_list == null)
			{
				_list = new List<CodifierValue> {
                    new CodifierValue { Code = None, Title = NoneTitle},
                    new CodifierValue { Code = Object, Title = ObjectTitle},
                    new CodifierValue { Code = IdentifiersRetain, Title = IdentifiersRetainTitle},
                    new CodifierValue { Code = IdentifiersMiss, Title = IdentifiersMissTitle},
                };
			}
			return _list;
		}
		static List<CodifierValue> _list = null;
		public static string Title(string code)
		{
			CodifierValue cv = ToList().FirstOrDefault(o => o.Code == code);
			return cv != null ? cv.Title : String.Empty;
		}
	}
}