using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Nephrite.Web;

namespace Nephrite.Metamodel
{
    public class ElementType
    {
		public const string Property = "P";
		public const string ObjectType = "T";
		public const string Method = "M";
		public const string Association = "A";
		public const string Form = "F";

        public static string PropertyTitle { get { return "Свойство"; } }
        public static string ObjectTypeTitle { get { return "Класс"; } }
        public static string MethodTitle { get { return "Метод"; } }
        public static string AssociationTitle { get { return "Ассоциация"; } }
		public static string FormTitle { get { return "Форма"; } }

        protected static List<CodifierValue> _list = null;
        public static List<CodifierValue> ToList()
        {
            if (_list == null)
            {
                _list = new List<CodifierValue> { 
					new CodifierValue { Code = ObjectType, Title = ObjectTypeTitle }, 
					// пока будут только для класса
					//new CodifierValue { Code = Property, Title = PropertyTitle },
					//new CodifierValue { Code = Method, Title = MethodTitle },
					//new CodifierValue { Code = Association, Title = AssociationTitle },
					//new CodifierValue { Code = Form, Title = FormTitle }

				};
            }
            return _list;
        }

        public static string Title(string code)
        {
			CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
			return res != null ? res.Title : "";
        }

        public string DisplayName { get; private set; }
        public string Code { get; private set; }
        public string Table { get; private set; }
        public string PrimaryKey { get; private set; }

        public static readonly ElementType[] Elements = new ElementType[] {
            new ElementType{ Code=ElementType.Association, PrimaryKey="AssociationID", Table="MM_Association", DisplayName="Ассоциация"},
            new ElementType{ Code=ElementType.Form, PrimaryKey="ViewFormID", Table="MM_ViewForm", DisplayName = "Форма"},
            new ElementType{ Code=ElementType.Method, PrimaryKey="MethodID", Table="MM_Method", DisplayName="Метод"},
            new ElementType{ Code=ElementType.ObjectType, PrimaryKey="ObjectTypeID", Table="MM_ObjectType", DisplayName="Тип объекта"},
            new ElementType{ Code=ElementType.Property, PrimaryKey="ObjectPropertyID", Table="MM_ObjectProperty", DisplayName="Свойство"}
        };

        public static ElementType Get(string code)
        {
            return Elements.Single(o => o.Code == code);
        }
    }
}
