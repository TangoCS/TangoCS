using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Nephrite.Web;
using System.Xml.Linq;

namespace Nephrite.Metamodel
{
	public class ObjectPropertyType
	{
		public const string String = "S";
		public const string Date = "D";
		public const string DateTime = "T";
		public const string Number = "N";
		public const string BigNumber = "U";
		public const string Data = "X";
		public const string Object = "O";
		public const string Boolean = "B";
        public const string Guid = "G";
		public const string Decimal = "M";
		public const string File = "F";
		public const string FileEx = "E";
		//public const string Image = "I";
        public const string Code = "C";
        public const string ZoneDateTime = "Z";
		

		public static string StringTitle { get { return "Строка"; } }
		public static string DateTitle { get { return "Дата"; } }
		public static string DateTimeTitle { get { return "Дата-время"; } }
		public static string NumberTitle { get { return "Число"; } }
		public static string BigNumberTitle { get { return "Большое число"; } }
		public static string DataTitle { get { return "Двоичные данные"; } }
		public static string ObjectTitle { get { return "Объект"; } }
		public static string BooleanTitle { get { return "Логический"; } }
        public static string GuidTitle { get { return "GUID"; } }
		public static string DecimalTitle { get { return "Дробный"; } }
		public static string FileTitle { get { return "Файл"; } }
		public static string FileExTitle { get { return "Файл хранилища"; } }
		//public static string ImageTitle { get { return "Изображение"; } }
        public static string CodeTitle { get { return "Код"; } }
        public static string ZoneDateTimeTitle { get { return "Дата-время с часовым поясом"; } }
		

		protected static List<CodifierValue> _list = null;
		public static List<CodifierValue> ToList()
		{
			if (ObjectPropertyType._list == null)
			{
				ObjectPropertyType._list = new List<CodifierValue> { 
					new CodifierValue { Code = ObjectPropertyType.String, Title = ObjectPropertyType.StringTitle },
					new CodifierValue { Code = ObjectPropertyType.Date, Title = ObjectPropertyType.DateTitle },
					new CodifierValue { Code = ObjectPropertyType.DateTime, Title = ObjectPropertyType.DateTimeTitle },
					new CodifierValue { Code = ObjectPropertyType.Number, Title = ObjectPropertyType.NumberTitle },
					new CodifierValue { Code = ObjectPropertyType.BigNumber, Title = ObjectPropertyType.BigNumberTitle },
					new CodifierValue { Code = ObjectPropertyType.Data, Title = ObjectPropertyType.DataTitle },
					new CodifierValue { Code = ObjectPropertyType.Object, Title = ObjectPropertyType.ObjectTitle },
					new CodifierValue { Code = ObjectPropertyType.Boolean, Title = ObjectPropertyType.BooleanTitle },
					new CodifierValue { Code = ObjectPropertyType.Guid, Title = ObjectPropertyType.GuidTitle },
					new CodifierValue { Code = ObjectPropertyType.Decimal, Title = ObjectPropertyType.DecimalTitle },
					new CodifierValue { Code = ObjectPropertyType.File, Title = ObjectPropertyType.FileTitle },
					new CodifierValue { Code = ObjectPropertyType.FileEx, Title = ObjectPropertyType.FileExTitle },
					//new CodifierValue { Code = ObjectPropertyType.Image, Title = ObjectPropertyType.ImageTitle },
					new CodifierValue { Code = ObjectPropertyType.Code, Title = ObjectPropertyType.CodeTitle },
					new CodifierValue { Code = ObjectPropertyType.ZoneDateTime, Title = ObjectPropertyType.ZoneDateTimeTitle }
				};
			}
			return ObjectPropertyType._list;
		}

		public static string Title(string code)
		{
			CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
			return res != null ? res.Title : "";
		}

		
	}

    public class TaggedValueType
    {
        public const string String = "S";
        public const string DateTime = "D";
        public const string Number = "N";
        public const string Boolean = "B";

        public static string StringTitle { get { return "Строка"; } }
        public static string DateTimeTitle { get { return "Дата-время"; } }
        public static string NumberTitle { get { return "Число"; } }
        public static string BooleanTitle { get { return "Логический"; } }


        protected static List<CodifierValue> _list = null;
        public static List<CodifierValue> ToList()
        {
            if (TaggedValueType._list == null)
            {
                TaggedValueType._list = new List<CodifierValue> { 
					new CodifierValue { Code = TaggedValueType.String, Title = TaggedValueType.StringTitle },
					new CodifierValue { Code = TaggedValueType.DateTime, Title = TaggedValueType.DateTimeTitle },
					new CodifierValue { Code = TaggedValueType.Number, Title = TaggedValueType.NumberTitle },
					new CodifierValue { Code = TaggedValueType.Boolean, Title = TaggedValueType.BooleanTitle }
				};
            }
            return TaggedValueType._list;
        }

        public static string Title(string code)
        {
            CodifierValue res = ToList().Where(o => o.Code == code).SingleOrDefault();
            return res != null ? res.Title : "";
        }
    }

    public class TaggedValueTypeControls
    {
        public static Dictionary<string, List<ObjectPropertyControl>> Controls = new Dictionary<string, List<ObjectPropertyControl>> {
			{ "S", new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода", ControlName = FormControl.TextBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Многострочный текст", ControlName = FormControl.TextArea, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Форматированный текст", ControlName = FormControl.RichEditor, UpperBounds = "1" }
			}},
			{ "D", new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Выбор даты из списков", ControlName = FormControl.DateLists, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Календарь с датой", ControlName = FormControl.Calendar, UpperBounds = "1" }
			}},
			{ "N", new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода", ControlName = FormControl.TextBox, UpperBounds = "1" }
			}},
			{ "B", new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Чекбокс", ControlName = FormControl.CheckBox, UpperBounds = "1" },
			}}
        };

        public static string Title(int name)
        {
            var res = Controls.SelectMany(o => o.Value).FirstOrDefault(o => o.ControlName == (FormControl)name);
            return res != null ? res.Title : "";
        }
    }

	public class TypeViewValue
	{
		public static Dictionary<string, Func<XElement, string>> Value = new Dictionary<string, Func<XElement, string>>() {
			{ "S", obj => obj.Value },
			{ "D", obj => Convert.ToDateTime(obj.Value).ToString("dd.MM.yyyy") },
			{ "N", obj => obj.Value },
			{ "B", obj => Convert.ToBoolean(obj.Value).Icon() }
		};
	}

	public class ObjectPropertyControl
	{
		public string Title { get; set; }
		public FormControl ControlName { get; set; }
		public string UpperBounds { get; set; }
	}

	public class FieldAcceptableAttribute
	{
		public string Title { get; set; }
		public string SysName { get; set; }
		public string DefaultValue { get; set; }
		public FieldAcceptableAttributeType Type { get; set; }

		public FieldAcceptableAttribute()
		{
			Type = FieldAcceptableAttributeType.String;
		}
	}

	public enum FieldAcceptableAttributeType
	{
		String,
		Boolean
	}


	public enum FormControl
	{
		TextBox = 1,
		TextArea = 2,
		RichEditor = 3,
		Label = 4,
		StringArray = 5,
		DateLists = 6,
		Calendar = 7,
		File = 8,
		Image = 9,
		CheckBox = 10,
		MMCodifier = 11,
		NestedForm = 12,
		DropDownList = 13,
		DropDownListConst = 14,
		FileArray = 15,
		SelectObjectHierarchic = 16,
		FileSimple = 17,
		TextObject = 18,
		FormattedTextBox = 19,
		FormattedLabel = 20,
        ZoneCalendar = 21,
		Modal = 22,
		MultiSelectObjectHierarchic = 23,
		MultiSelectObjectHierarchicText = 24,
		DropDownListText = 25,
		SingleObject = 26,
		MultiObject = 27
	}

	public class ObjectPropertyControls
	{
		public static Dictionary<string, List<ObjectPropertyControl>> Controls = new Dictionary<string, List<ObjectPropertyControl>> {
			{ ObjectPropertyType.String, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода", ControlName = FormControl.TextBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Многострочный текст", ControlName = FormControl.TextArea, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Форматированный текст", ControlName = FormControl.RichEditor, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Выбор из списка", ControlName = FormControl.DropDownListText, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Множественный выбор из окна выбора", ControlName = FormControl.MultiSelectObjectHierarchicText, UpperBounds = "1" }
			//new ObjectPropertyControl { Title = "Выбор объекта с intellisense", ControlName = FormControl.TextObject, UpperBounds = "1" }
			//new ObjectPropertyControl { Title = "CSS класс", ControlName = "CSSClass", UpperBounds = "1" },
			//new ObjectPropertyControl { Title = "Коллекция строк", ControlName = FormControl.StringArray, UpperBounds = "*" }
			}},
			{ ObjectPropertyType.Date, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Выбор даты из списков", ControlName = FormControl.DateLists, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Календарь с датой", ControlName = FormControl.Calendar, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" }//,
			//new ObjectPropertyControl { Title = "Поле с маской дата", ControlName = "", UpperBounds = "1" },
			}},
			{ ObjectPropertyType.DateTime, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Календарь с датой и временем", ControlName = FormControl.Calendar, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" }//,
			//new ObjectPropertyControl { Title = "Поле с маской дата", ControlName = "", UpperBounds = "1" },
			}},
			{ ObjectPropertyType.Number, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода с форматом", ControlName = FormControl.FormattedTextBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст с форматом", ControlName = FormControl.FormattedLabel, UpperBounds = "1" },
			}},
			{ ObjectPropertyType.BigNumber, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода с форматом", ControlName = FormControl.FormattedTextBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст с форматом", ControlName = FormControl.FormattedLabel, UpperBounds = "1" },
			}},
			{ ObjectPropertyType.Decimal, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Текстовое поле ввода с форматом", ControlName = FormControl.FormattedTextBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Простой нередактируемый текст с форматом", ControlName = FormControl.FormattedLabel, UpperBounds = "1" },
			}},
			{ ObjectPropertyType.File, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Файл с моментальной загрузкой", ControlName = FormControl.File, UpperBounds = "1"},
			new ObjectPropertyControl { Title = "Файл с загрузкой по кнопке ОК", ControlName = FormControl.FileSimple, UpperBounds = "1"},
			new ObjectPropertyControl { Title = "Картинка", ControlName = FormControl.Image, UpperBounds = "1"},
			new ObjectPropertyControl { Title = "Коллекция файлов", ControlName = FormControl.FileArray, UpperBounds = "*"},
			}},
			/*{ ObjectPropertyType.Image, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Картинка", ControlName = FormControl.Image, UpperBounds = "1"},
			}},*/
			{ ObjectPropertyType.Boolean, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Чекбокс", ControlName = FormControl.CheckBox, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Нередактирумое поле", ControlName = FormControl.Label, UpperBounds = "1" },
			}},
			{ ObjectPropertyType.Object, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Выпадающий список", ControlName = FormControl.DropDownList, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Выбор из окна выбора", ControlName = FormControl.SingleObject, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Выбор из окна выбора (мод.)", ControlName = FormControl.SelectObjectHierarchic, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Ввод в модальном окне", ControlName = FormControl.Modal, UpperBounds = "*" },
			new ObjectPropertyControl { Title = "Множественный выбор из окна выбора (расширяемый код)", ControlName = FormControl.MultiSelectObjectHierarchic, UpperBounds = "*" },
			new ObjectPropertyControl { Title = "Множественный выбор из окна выбора (контрол)", ControlName = FormControl.MultiObject, UpperBounds = "*" }
			//new ObjectPropertyControl { Title = "Список предустановленных значений", ControlName = FormControl.DropDownListConst, UpperBounds = "1" },
			//new ObjectPropertyControl { Title = "Форма", ControlName = FormControl.NestedForm, UpperBounds = "1" },
			//new ObjectPropertyControl { Title = "Форма (*)", ControlName = FormControl.NestedForm, UpperBounds = "*" }

			}},
			{ ObjectPropertyType.Code, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Кодификатор", ControlName = FormControl.MMCodifier, UpperBounds = "1" },
			}},
			{ ObjectPropertyType.ZoneDateTime, new List<ObjectPropertyControl> {
			new ObjectPropertyControl { Title = "Простой нередактируемый текст", ControlName = FormControl.Label, UpperBounds = "1" },
			new ObjectPropertyControl { Title = "Календарь с часовым поясом", ControlName = FormControl.ZoneCalendar, UpperBounds = "1" },
			}}
		};

		public static Dictionary<FormControl, List<FieldAcceptableAttribute>> Attributes = new Dictionary<FormControl, List<FieldAcceptableAttribute>>
		{
			{ 
				FormControl.DropDownList,
				new List<FieldAcceptableAttribute> {
					/*new FieldAcceptableAttribute { SysName = "Table", Title = "Таблица" },*/
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Поле для отображения" },
					/*new FieldAcceptableAttribute { SysName = "DataValueField", Title = "Поле для значения" }*/
				}
			},
			{ 
				FormControl.DropDownListText,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Class", Title = "Класс" },
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Поле для отображения" },
					new FieldAcceptableAttribute { SysName = "DataValueField", Title = "Поле для значения" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр" }
				}
			},
			/*{ 
				FormControl.DropDownListConst,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Values", Title = "Значения", DefaultValue = "<items><item value=\"\" text=\"\" /></items>" }
				}
			},*/
			{ 
				FormControl.TextBox,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Width", Title = "Длина", DefaultValue = "100%" }
				}
			},
			{ 
				FormControl.FormattedTextBox,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Width", Title = "Длина", DefaultValue = "100%" },
					new FieldAcceptableAttribute { SysName = "Format", Title = "Формат", DefaultValue = "" }
				}
			},
			{ 
				FormControl.FormattedLabel,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Format", Title = "Формат", DefaultValue = "" }
				}
			},
			{ 
				FormControl.TextArea,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Width", Title = "Длина", DefaultValue = "100%" },
					new FieldAcceptableAttribute { SysName = "Height", Title = "Высота", DefaultValue = "200px" }
				}
			},
			{ 
				FormControl.RichEditor,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "TagFile", Title = "Файл разрешенных тегов", DefaultValue = "" }
				}
			},
			{ 
				FormControl.Image,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "MaxWidth", Title = "Максимальная ширина", DefaultValue = "" },
					new FieldAcceptableAttribute { SysName = "MaxHeight", Title = "Максимальная высота", DefaultValue = "" }
				}
			},
			/*{ 
				FormControl.TextObject,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Class", Title = "Выбираемый класс", DefaultValue = "" },
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			},*/
			{ 
				FormControl.SelectObjectHierarchic,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			}
			,
			{ 
				FormControl.SingleObject,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			}
			,
			{ 
				FormControl.MultiSelectObjectHierarchic,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			},
			{ 
				FormControl.MultiObject,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			},
			{ 
				FormControl.MultiSelectObjectHierarchicText,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "Class", Title = "Выбираемый класс", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "DataValueField", Title = "Свойство для данных", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "DataTextField", Title = "Отображаемое свойство", DefaultValue = "Title" },
					new FieldAcceptableAttribute { SysName = "Filter", Title = "Фильтр", DefaultValue = "o => !o.IsDeleted" },
					new FieldAcceptableAttribute { SysName = "SearchExpression", Title = "Выражение для поиска", DefaultValue = "" }
				}
			}
			/*{ 
				FormControl.NestedForm,
				new List<FieldAcceptableAttribute> {
					new FieldAcceptableAttribute { SysName = "FormName", Title = "Форма", DefaultValue = "" },
					new FieldAcceptableAttribute { SysName = "ViewName", Title = "Представление", DefaultValue = "" }
				}
			}*/

		};
	}


	public class DataValidation
	{
		public static Dictionary<string, List<ValidationRuleType>> Values = new Dictionary<string, List<ValidationRuleType>>
		{
			{
				ObjectPropertyType.Boolean,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Равен true", Expression = "{0} == true", HasParameters = false },
					new ValidationRuleType { Title = "Равен false", Expression = "{0} == false", HasParameters = false },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.Code,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.Date,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше", Expression = "{0} > {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше или равно", Expression = "{0} >= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше", Expression = "{0} < {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше или равно", Expression = "{0} <= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.DateTime,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше", Expression = "{0} > {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше или равно", Expression = "{0} >= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше", Expression = "{0} < {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше или равно", Expression = "{0} <= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.Decimal,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше", Expression = "{0} > {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше или равно", Expression = "{0} >= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше", Expression = "{0} < {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше или равно", Expression = "{0} <= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.Number,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше", Expression = "{0} > {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше или равно", Expression = "{0} >= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше", Expression = "{0} < {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше или равно", Expression = "{0} <= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.BigNumber,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше", Expression = "{0} > {1}", HasParameters = true },
					new ValidationRuleType { Title = "Больше или равно", Expression = "{0} >= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше", Expression = "{0} < {1}", HasParameters = true },
					new ValidationRuleType { Title = "Меньше или равно", Expression = "{0} <= {1}", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "{0}.HasValue", HasParameters = false, Nullable = true }
				}
			},
			{
				ObjectPropertyType.String,
				new List<ValidationRuleType> {
					new ValidationRuleType { Title = "Совпадает с", Expression = "{0} == {1}", HasParameters = true },
					new ValidationRuleType { Title = "Содержит", Expression = "{0}.IndexOf({1}) > 0", HasParameters = true },
					new ValidationRuleType { Title = "Начинается с", Expression = "{0}.StartWith({1})", HasParameters = true },
					new ValidationRuleType { Title = "Заканчивается", Expression = "{0}.EndWith({1})", HasParameters = true },
					new ValidationRuleType { Title = "Соответствует шаблону", Expression = "Regex.IsMatch({0}, {1})", HasParameters = true },
					new ValidationRuleType { Title = "Есть значение", Expression = "!String.IsNullOrEmpty({0})", HasParameters = false, Nullable = true }
				}
			}
		};
	}

	public class ValidationRuleType
	{
		public string Title { get; set; }
		public string Expression { get; set; }
		public bool HasParameters { get; set; }
		public bool Nullable { get; set; }
	}

}
