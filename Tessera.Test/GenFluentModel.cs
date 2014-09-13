using System;
using System.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;

namespace Solution.Model
{
	public partial class MM_Package { }
	public partial class MM_ObjectType { }
	public partial class MM_ObjectProperty { }
	public partial class MM_FormField { }
	public partial class MM_Codifier { }
	public partial class MM_CodifierValue { }
	public partial class MM_FormFieldAttribute { }
	public partial class MM_FormView { }
	public partial class MM_Method { }
	public partial class MM_MethodParameter { }
	public partial class MM_FormFieldGroup { }
	public partial class MM_Predicate { }

	public class mmPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{
			p.AddEnum("VersioningType", "Тип версионности")
				.Value("O", "Object", "Версионность объектов")
				.Value("M", "IdentifierMiss", "Версионность справочника без сохранения идентификаторов   ")
				.Value("R", "IdentifiersRetain", "Версионность справочника с сохранением идентификаторов   ")
				.Value("N", "None", "Нет");

			p.AddEnum("TemplateType", "Тип представления")
				.Value("A", "Ascx", "Ascx")
				.Value("S", "SiteView", "Представление сайта")
				.Value("P", "Aspx", "Aspx")
				.Value("M", "Asmx", "Asmx")
				.Value("H", "Ashx", "Ashx")
				.Value("C", "Svc", "Svc");

			p.AddEnum("ViewDataBound", "Мощность представления")
				.Value("0", "None", "Без объекта")
				.Value("1", "Single", "Один объект")
				.Value("*", "Collection", "Коллекция объектов");

			p.AddEnum("NoticeType", "Тип оповещения")
				.Value("T", "Timeline", "Хронология")
				.Value("S", "Static", "Статические");



			p.AddClass<MM_Package>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Attribute("Version", "Version", MetaStringType.Null())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("IsDataReplicated", "IsDataReplicated", MetaBooleanType.NotNull())
				.ComputedAttribute("ControlsPath", "ControlsPath", MetaStringType.Null())
				.ComputedAttribute("FullSysName", "FullSysName", MetaStringType.Null())
				.Reference<MM_Package>("ParentPackage", "Родительский пакет", x => x.InverseProperty("ChildPackages"))
				.Reference<MM_Package>("ChildPackages", "Дочерние пакеты", x => x.Multiple().InverseProperty("ParentPackage"))
				.Reference<MM_Codifier>("Codifiers", "Справочники", x => x.Multiple().InverseProperty("Package"))
				.Reference<MM_ObjectType>("ObjectTypes", "Классы", x => x.Multiple().InverseProperty("Package"))
				.OperationDelete() 
				.Operation("Export", "Экспорт", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("export")
				)
				.Operation("ExportObjectType", "Экспорт класса", x => x 
					.ParmInt("id")
					.ParmInt("objectTypeID")
					.InvokesSingleObjectView("exportobjecttype")
				)
				.Operation("Import", "Импорт", x => x 
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "import")
				)
				.OperationEdit() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_ObjectType>()
				.IntKey()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("LastModifiedUserID", "Последний редактировавший пользователь", MetaIntType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("IsEnableSPM", "УПБ", MetaBooleanType.NotNull())
				.Attribute("Guid", "GUID", MetaGuidType.NotNull())
				.Attribute("IsSeparateTable", "Отдельная таблица", MetaBooleanType.NotNull())
				.Attribute("IsTemplate", "Шаблон", MetaBooleanType.NotNull())
				.Attribute("TitlePlural", "TitlePlural", MetaStringType.Null())
				.Attribute("DefaultOrderBy", "DefaultOrderBy", MetaStringType.Null())
				.Attribute("LogicalDelete", "LogicalDelete", MetaStringType.Null())
				.Attribute("IsReplicate", "IsReplicate", MetaBooleanType.NotNull())
				.Attribute("IsEnableUserViews", "IsEnableUserViews", MetaBooleanType.NotNull())
				.Attribute("SecurityPackageSystemName", "SecurityPackageSystemName", MetaStringType.Null())
				.Attribute("IsEnableObjectHistory", "IsEnableObjectHistory", MetaBooleanType.NotNull())
				.Attribute("Interface", "Interface", MetaStringType.Null())
				.Attribute("HistoryTypeCode", "HistoryTypeCode", MetaEnumType.NotNull(""))
				.Attribute("IsDataReplicated", "IsDataReplicated", MetaBooleanType.NotNull())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("Description", "Description", MetaStringType.Null())
				.ComputedAttribute("ControlsPath", "ControlsPath", MetaStringType.Null())
				.ComputedAttribute("FullSysName", "FullSysName", MetaStringType.Null())
				.Reference<MM_ObjectType>("BaseObjectType", "Базовый тип")
				.Reference<MM_Method>("Methods", "Методы", x => x.Multiple().InverseProperty("ObjectType"))
				.Reference<MM_ObjectProperty>("Properties", "Свойства", x => x.Multiple().InverseProperty("ObjectType"))
				.Reference<MM_FormView>("FormViews", "Представления", x => x.Multiple().InverseProperty("ObjectType"))
				.Reference<MM_FormFieldGroup>("FormFieldGroups", "Группы полей", x => x.Multiple().InverseProperty("ObjectType"))
				.Reference<MM_Predicate>("DataValidations", "Проверки данных", x => x.Multiple().InverseProperty("ObjectType"))
				.Reference<MM_Package>("Package", "Пакет", x => x.InverseProperty("ObjectTypes"))
				.Reference<MM_ObjectType>("Stereotypes", "Стереотипы", x => x.Multiple())
				.OperationDelete() 
				.Operation("DisableSPM", "Запретить УПБ", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("disablespm")
				)
				.OperationEdit() 
				.Operation("EnableSPM", "Разрешить УПБ", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("enablespm")
				)
				.Operation("GenerateEditControl", "Сгенерировать форму Edit", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("generateeditcontrol")
				)
				.Operation("GenerateViewControl", "Сгенерировать форму View", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("generateviewcontrol")
				)
				.Operation("GenerateListControl", "Сгенерировать форму List", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("generatlistcontrol")
				)
				.OperationList() 
				.Operation("EditRights", "Редактировать права", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("spm")
				)
				.OperationView() 
				.Operation("MassCreate", "Массовое создание классов", x => x 
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "masscreate")
				)
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("CreateFromTemplate", "Создать по шаблону", x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_ObjectProperty>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Attribute("Guid", "GUID", MetaGuidType.NotNull())
				.Attribute("IsMultilingual", "Многоязычное", MetaBooleanType.NotNull())
				.Attribute("IsPrimaryKey", "Первичный ключ", MetaBooleanType.NotNull())
				.Attribute("IsSystem", "Системное свойство", MetaBooleanType.NotNull())
				.Attribute("TypeCode", "TypeCode", MetaEnumType.NotNull(""))
				.Attribute("IsNavigable", "IsNavigable", MetaBooleanType.NotNull())
				.Attribute("IsAggregate", "IsAggregate", MetaBooleanType.NotNull())
				.Attribute("LowerBound", "LowerBound", MetaIntType.NotNull())
				.Attribute("UpperBound", "UpperBound", MetaIntType.NotNull())
				.Attribute("Expression", "Expression", MetaStringType.Null())
				.Attribute("IsReferenceToVersion", "IsReferenceToVersion", MetaBooleanType.NotNull())
				.Attribute("ValueFilter", "ValueFilter", MetaStringType.Null())
				.Attribute("Precision", "Precision", MetaIntType.Null())
				.Attribute("Scale", "Scale", MetaIntType.Null())
				.Attribute("Length", "Length", MetaIntType.Null())
				.Attribute("DeleteRule", "DeleteRule", MetaEnumType.NotNull(""))
				.Attribute("KindCode", "KindCode", MetaEnumType.NotNull(""))
				.Attribute("DefaultDBValue", "DefaultDBValue", MetaStringType.Null())
				.Attribute("Description", "Description", MetaStringType.Null())
				.Attribute("IsIdentity", "Автоинкрементный идентификатор", MetaBooleanType.NotNull())
				.Reference<MM_ObjectType>("ObjectType", "Класс", x => x.InverseProperty("Properties"))
				.Reference<MM_ObjectProperty>("RefObjectProperty", "Ссылается на свойство")
				.Reference<MM_Codifier>("Codifier", "Кодификатор")
				.Reference<MM_ObjectType>("RefObjectType", "Ссылается на класс")
				.OperationDelete() 
				.OperationEdit() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("kind")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_FormField>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("ControlName", "Элемент управления", MetaIntType.Null())
				.Attribute("Title", "Заголовок", MetaStringType.Null())
				.Attribute("DefaultValue", "Значение по умолчанию", MetaStringType.Null())
				.Attribute("Comment", "Подсказка", MetaStringType.Null())
				.Attribute("ShowInList", "ShowInList", MetaBooleanType.NotNull())
				.Attribute("ShowInEdit", "ShowInEdit", MetaBooleanType.NotNull())
				.Attribute("ShowInView", "ShowInView", MetaBooleanType.NotNull())
				.Attribute("ValueFunction", "ValueFunction", MetaStringType.Null())
				.Attribute("SortExpression", "SortExpression", MetaStringType.Null())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("ListColumnWidth", "ListColumnWidth", MetaStringType.Null())
				.Attribute("ValueFunctionExecType", "ValueFunctionExecType", MetaBooleanType.NotNull())
				.Reference<MM_ObjectProperty>("ObjectProperty", "Свойство")
				.Reference<MM_FormFieldAttribute>("Attributes", "Атрибуты", x => x.Multiple().InverseProperty("FormField"))
				.Reference<MM_FormFieldGroup>("FormFieldGroup", "Группа полей", x => x.InverseProperty("FormFields"))
				.OperationEdit() 
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_Codifier>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Reference<MM_CodifierValue>("Values", "Значения", x => x.Multiple().InverseProperty("Codifier"))
				.Reference<MM_Package>("Package", "Пакет", x => x.InverseProperty("Codifiers"))
				.OperationView() 
				.OperationDelete() 
				.OperationEdit() 
				.OperationList() 
			;	
			p.AddClass<MM_CodifierValue>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Code", "Код", MetaEnumType.NotNull(""))
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "SysName", MetaStringType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Reference<MM_Codifier>("Codifier", "Кодификатор", x => x.InverseProperty("Values"))
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_FormFieldAttribute>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Value", "Значение", MetaStringType.NotNull())
				.Attribute("IsEvent", "Признак Событие", MetaBooleanType.NotNull())
				.Reference<MM_FormField>("FormField", "Поле", x => x.InverseProperty("Attributes"))
			;	
			p.AddClass<MM_FormView>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("ViewTemplate", "Шаблон формы", MetaStringType.Null())
				.Attribute("TemplateTypeCode", "Тип представления", MetaEnumType.NotNull("TemplateType"))
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Attribute("IsCustom", "Пользовательское", MetaBooleanType.NotNull())
				.Attribute("IsCaching", "IsCaching", MetaBooleanType.NotNull())
				.Attribute("CacheKeyParams", "CacheKeyParams", MetaStringType.Null())
				.Attribute("CacheTimeout", "CacheTimeout", MetaIntType.NotNull())
				.Attribute("BaseClass", "Базовый класс", MetaStringType.NotNull())
				.ComputedAttribute("ControlsPath", "ControlsPath", MetaStringType.Null())
				.ComputedAttribute("FullSysName", "FullSysName", MetaStringType.Null())
				.Reference<MM_Package>("Package", "Пакет")
				.Reference<MM_ObjectType>("ObjectType", "Класс", x => x.InverseProperty("FormViews"))
				.Operation("Caching", "Настроить кэширование", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("caching")
				)
				.OperationDelete() 
				.OperationEdit() 
				.Operation("Spm", "Настроить права доступа", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("spm")
				)
				.Operation("ViewVersion", "Просмотр версий", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesView("ViewControl<HST_{0}>", "viewversion")
				)
				.Operation("LastChanges", "Последние изменения в представлениях", x => x 
					.InvokesObjectListView("lastchanges")
				)
				.Operation("CreateForObjectType", "Создать для класса", x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("CreateForPackage", "Создать для пакета", x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_Method>()
				.IntKey()
				.Attribute("SysName", "SysName", MetaStringType.NotNull())
				.Attribute("Title", "Title", MetaStringType.NotNull())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Attribute("IsDefault", "IsDefault", MetaBooleanType.NotNull())
				.Attribute("Icon", "Icon", MetaStringType.Null())
				.Attribute("Code", "Code", MetaStringType.Null())
				.Attribute("ViewPath", "ViewPath", MetaStringType.Null())
				.Attribute("PredicateCode", "PredicateCode", MetaStringType.Null())
				.Attribute("Comment", "Comment", MetaStringType.Null())
				.Attribute("Parameters", "Parameters", MetaStringType.Null())
				.Reference<MM_MethodParameter>("MethodParameters", "Параметры", x => x.Multiple().InverseProperty("Method"))
				.Reference<MM_ObjectType>("ObjectType", "Класс", x => x.InverseProperty("Methods"))
				.Reference<MM_FormView>("FormView", "Представление")
				.OperationEdit() 
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_MethodParameter>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Наименование", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.Null())
				.Attribute("Type", "Тип данных", MetaStringType.Null())
				.Reference<MM_Method>("Method", "Метод", x => x.InverseProperty("MethodParameters"))
			;	
			p.AddClass<MM_FormFieldGroup>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Title", MetaStringType.NotNull())
				.Attribute("SeqNo", "SeqNo", MetaIntType.NotNull())
				.Attribute("SelectObjectPrefix", "SelectObjectPrefix", MetaStringType.Null())
				.Attribute("SelectObjectClass", "SelectObjectClass", MetaStringType.Null())
				.Attribute("ShowTitle", "ShowTitle", MetaBooleanType.NotNull())
				.Attribute("SelectObjectDataTextField", "SelectObjectDataTextField", MetaStringType.Null())
				.Attribute("SelectObjectFilter", "SelectObjectFilter", MetaStringType.Null())
				.Attribute("SelectObjectSearchExpression", "SelectObjectSearchExpression", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Reference<MM_ObjectType>("ObjectType", "Класс", x => x.InverseProperty("FormFieldGroups"))
				.Reference<MM_ObjectProperty>("SelectObjectProperty", "Свойство класса")
				.Reference<MM_FormField>("FormFields", "Поля", x => x.Multiple().InverseProperty("FormFieldGroup"))
				.OperationEdit() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MM_Predicate>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("DesignerData", "DesignerData", MetaStringType.Null())
				.Attribute("Message", "Message", MetaStringType.NotNull())
				.Attribute("Title", "Наименовение", MetaStringType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Body", "Body", MetaStringType.NotNull())
				.Attribute("Type", "Тип", MetaEnumType.NotNull(""))
				.Reference<MM_ObjectType>("ObjectType", "Класс", x => x.InverseProperty("DataValidations"))
				.OperationDelete() 
				.OperationEdit() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			return p;
		}
	}
	public partial class SPM_Subject { }
	public partial class SPM_Role { }
	public partial class SPM_Action { }
	public partial class SPM_ActionAsso { }
	public partial class SPM_RoleAsso { }
	public partial class SPM_SubjectRole { }
	public partial class SPM_RoleGroup { }
	public partial class SPM_RoleAccess { }
	public partial class SPM_CasheFlag { }
	public partial class SPM_SubjectDelegate { }
	public partial class SPM_AvailableRoleForGrant { }
	public partial class SPM_C_RoleType { }

	public class SPMPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<SPM_Subject>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SystemName", "Системное имя", MetaStringType.NotNull())
				.Attribute("SID", "SID", MetaStringType.NotNull())
				.Attribute("PasswordHash", "PasswordHash", MetaByteArrayType.Null())
				.Attribute("RegMagicString", "RegMagicString", MetaStringType.Null())
				.Attribute("RegDate", "RegDate", MetaDateTimeType.Null())
				.Attribute("IsActive", "IsActive", MetaBooleanType.NotNull())
				.Attribute("EMail", "EMail", MetaStringType.Null())
				.Attribute("MustChangePassword", "MustChangePassword", MetaBooleanType.NotNull())
				.Attribute("PasswordExpDate", "PasswordExpDate", MetaDateTimeType.Null())
				.OperationEdit() 
				.OperationList() 
				.Operation("Register", "Зарегистрироваться", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "Register")
				)
				.Operation("RegistrationComplete", "Регистрация завершена", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "registrationcomplete")
				)
				.Operation("RestorePassword", "Восстановить пароль", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "restorepassword")
				)
				.OperationDelete() 
				.Operation("Logoff", "Выйти") 
				.Operation("Activate", "Активировать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("ChangePassword", "Изменить пароль", x => x 
					.ParmString("magicString")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.Operation("Deactivate", "Деактивировать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<SPM_Role>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Attribute("Description", "Описание", MetaStringType.Null())
				.Attribute("RoleForGrantCondition", "Условие выбора ролей для назначения пользователям", MetaIntType.NotNull())
				.Attribute("SID", "SID группы AD", MetaStringType.Null())
				.Reference<SPM_RoleGroup>("RoleGroup", "Группа ролей", x => x.InverseProperty("Roles"))
				.Reference<SPM_C_RoleType>("RoleType", "Тип роли")
				.OperationDelete() 
				.OperationEdit() 
				.OperationList() 
				.OperationView() 
				.Operation("EditRoles", "Редактировать роли для назначения пользователям", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editroles")
				)
				.Operation("SubjectsList", "Список пользователей", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("subjectslist")
				)
				.Operation("RemoveSubject", "Отвязять пользователя", x => x 
					.ParmInt("roleID")
					.ParmInt("subjectID")
					.ParmString("returnurl")
				)
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<SPM_Action>()
				.IntKey()
				.Attribute("Title", "Title", MetaStringType.NotNull())
				.Attribute("Type", "Type (устарело)", MetaIntType.NotNull())
				.Attribute("SystemName", "SystemName", MetaStringType.Null())
				.Attribute("CategoryID", "CategoryID", MetaIntType.Null())
				.Attribute("ItemGUID", "Ид объекта", MetaGuidType.Null())
				.Attribute("ActionTypeID", "Тип операции", MetaIntType.NotNull())
				.Attribute("ClassGUID", "Ид класса", MetaGuidType.Null())
				.Attribute("SubCategoryGUID", "SubCategoryGUID", MetaGuidType.Null())
				.Reference<MM_Predicate>("Predicate", "Предикат")
				.Reference<MM_Package>("Package", "Пакет")
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.ParmGuid("classguid")
					.ParmString("returnurl")
				)
				.OperationEdit() 
				.OperationList() 
				.Operation("ViewListTree", "Список - дерево", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "treelist")
				)
			;	
			p.AddClass<SPM_ActionAsso>()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.ReferenceKey<SPM_Action>("Action", "Action")
				.ReferenceKey<SPM_Action>("ParentAction", "ParentAction")
			;	
			p.AddClass<SPM_RoleAsso>()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.ReferenceKey<SPM_Role>("ParentRole", "ParentRole")
				.ReferenceKey<SPM_Role>("Role", "Role")
			;	
			p.AddClass<SPM_SubjectRole>()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.ReferenceKey<SPM_Role>("Role", "Role")
				.ReferenceKey<SPM_Subject>("Subject", "Subject")
			;	
			p.AddClass<SPM_RoleGroup>()
				.IntKey()
				.Attribute("Title", "Название", MetaStringType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Reference<SPM_Role>("Roles", "Роли", x => x.InverseProperty("RoleGroup"))
				.OperationDelete() 
				.OperationEdit() 
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<SPM_RoleAccess>()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.ReferenceKey<SPM_Action>("Action", "Action")
				.ReferenceKey<SPM_Role>("Role", "Role")
			;	
			p.AddClass<SPM_CasheFlag>()
				.Attribute("IsChange", "IsChange", MetaBooleanType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<SPM_SubjectDelegate>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("BeginDate", "Дата начала", MetaDateTimeType.NotNull())
				.Attribute("EndDate", "Дата окончания", MetaDateTimeType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.PersistentComputedAttribute("IsDeleted", "Признак Удалено", MetaBooleanType.Null())
				.Reference<SPM_Subject>("DelegatedFrom", "От кого делегировано")
				.Reference<SPM_Subject>("DelegatedTo", "Кому делегировано")
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<SPM_AvailableRoleForGrant>()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.ReferenceKey<SPM_Role>("Role", "Role")
				.ReferenceKey<SPM_Role>("RoleForGrant", "RoleForGrant")
			;	
			p.AddClass<SPM_C_RoleType>()
				.IntKey("RoleTypeID")
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			return p;
		}
	}

	public class SiteViewsPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("AdminLogin", "Форма логина в админку", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "AdminLogin")
			);
			p.Operation("homeheader", "Шапка домашней страницы", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "homeheader")
			);
			p.Operation("NavMenu", "Меню навигации админки", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "NavMenu")
			);
			p.Operation("ChangePassword", "Изменение пароля", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "ChangePassword")
			);
			p.Operation("utilsmenu", "Меню Утилиты шапки страницы", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "utilsmenu")
			);
			p.Operation("runup", "Накат", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "runup")
			);

			return p;
		}
	}
	public partial class N_TimeZone { }
	public partial class N_RssFeed { }
	public partial class UserActivity { }
	public partial class N_TextResource { }
	public partial class TM_Task { }
	public partial class TM_TaskParameter { }
	public partial class MailMessage { }
	public partial class C_Help { }
	public partial class MailTemplate { }
	public partial class CalendarDay { }
	public partial class N_DDL { }
	public partial class TM_TaskExecution { }
	public partial class N_Settings { }
	public partial class N_SettingsGroup { }
	public partial class N_TaskType { }
	public partial class N_Task { }

	public class SystemPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{
			p.AddEnum("StorageType", "Тип хранения файлов")
				.Value("D", "Disk", "Диск")
				.Value("B", "Database", "База данных");



			p.AddClass<N_TimeZone>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("GMTOffset", "Смещение относительно GMT", MetaIntType.NotNull())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.ComputedAttribute("DisplayTitle", "Отображаемое имя", MetaStringType.Null())
				.OperationDelete() 
				.OperationEdit() 
				.OperationList() 
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<N_RssFeed>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Description", "Описание", MetaStringType.NotNull())
				.Attribute("Copyright", "Информация об авторском праве", MetaStringType.NotNull())
				.Attribute("ObjectTypeSysName", "Класс", MetaStringType.NotNull())
				.Attribute("ViewFormSysName", "Представление", MetaStringType.NotNull())
				.Attribute("Ttl", "Время жизни", MetaIntType.NotNull())
				.Attribute("Predicate", "Предикат", MetaStringType.NotNull())
				.Attribute("PubDate", "Дата публикации", MetaStringType.NotNull())
				.Attribute("Author", "Автор элемента", MetaStringType.NotNull())
				.Attribute("WebMaster", "E-mail веб-мастера", MetaStringType.NotNull())
				.Attribute("LinkParams", "LinkParams", MetaStringType.Null())
				.OperationEdit() 
				.OperationCreateNew() 
				.OperationList() 
				.OperationDelete() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<UserActivity>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Название объекта", MetaStringType.NotNull())
				.Attribute("ObjectKey", "Ид объекта", MetaStringType.NotNull())
				.Attribute("ObjectTypeSysName", "Класс", MetaStringType.Null())
				.Attribute("Action", "Операция", MetaStringType.NotNull())
				.Attribute("ObjectTypeTitle", "Тип объекта", MetaStringType.NotNull())
				.Attribute("UserTitle", "Ф.И.О.", MetaStringType.NotNull())
				.Attribute("IP", "IP", MetaStringType.NotNull())
				.OperationList() 
				.Operation("ExcelExport", "Экспорт в Excel", x => x 
					.ParmString("returnurl")
					.InvokesObjectListView("excelexport")
				)
			;	
			p.AddClass<N_TextResource>()
				.IntKey()
				.Attribute("Title", "Название", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Text", "Текст", MetaStringType.Null(), true)
				.OperationList() 
				.OperationDelete() 
				.OperationEdit() 
				.OperationCreateNew() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<TM_Task>()
				.IntKey()
				.Attribute("Title", "Название", MetaStringType.NotNull())
				.Attribute("Class", "Класс", MetaStringType.NotNull())
				.Attribute("StartType", "Тип старта", MetaBooleanType.NotNull())
				.Attribute("Method", "Метод", MetaStringType.NotNull())
				.Attribute("Interval", "Интервал", MetaIntType.NotNull())
				.Attribute("LastStartDate", "Последний запуск", MetaDateTimeType.Null())
				.Attribute("IsSuccessfull", "Признак успешности", MetaBooleanType.NotNull())
				.Attribute("IsActive", "Активна", MetaBooleanType.NotNull())
				.Attribute("StartFromService", "Старт службой", MetaBooleanType.NotNull())
				.Attribute("ErrorLogID", "Ид ошибки в журнале", MetaIntType.Null())
				.Attribute("ExecutionTimeout", "Таймаут выполнения задачи, мин", MetaIntType.NotNull())
				.Reference<TM_TaskParameter>("Parameters", "Параметры", x => x.Multiple().InverseProperty("Parent"))
				.OperationEdit() 
				.OperationView() 
				.OperationList() 
				.OperationDelete() 
				.Operation("Start", "Запуск", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<TM_TaskParameter>()
				.IntKey()
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Value", "Значение", MetaStringType.Null())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Attribute("Title", "Название", MetaStringType.NotNull())
				.Reference<TM_Task>("Parent", "Задача", x => x.InverseProperty("Parameters"))
				.OperationDelete() 
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MailMessage>()
				.IntKey()
				.Attribute("Recipients", "Получатели", MetaStringType.Null())
				.Attribute("Subject", "Тема", MetaStringType.Null())
				.Attribute("Body", "Сообщение", MetaStringType.Null())
				.Attribute("IsSent", "Отправлено", MetaBooleanType.NotNull())
				.Attribute("Attachment", "Вложение", MetaByteArrayType.Null())
				.Attribute("AttachmentName", "Наименование вложения", MetaStringType.Null())
				.Attribute("Error", "Ошибка", MetaStringType.Null())
				.Attribute("CopyRecipients", "Получатели копии", MetaStringType.Null())
				.Attribute("LastSendAttemptDate", "Последняя попытка отправки", MetaDateTimeType.Null())
				.Attribute("AttemptsToSendCount", "Количество попыток отправки", MetaIntType.NotNull())
				.ComputedAttribute("Title", "Название", MetaStringType.Null())
				.OperationView() 
				.OperationList() 
			;	
			p.AddClass<C_Help>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Text", "Текст", MetaStringType.Null())
				.Attribute("FormViewFullSysName", "Представление", MetaStringType.NotNull())
				.Attribute("Title", "Форма системы", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<MailTemplate>()
				.IntKey()
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("IsSystem", "Системное", MetaBooleanType.NotNull())
				.Attribute("TemplateBody", "Сообщение", MetaStringType.NotNull())
				.Attribute("TemplateSubject", "Тема", MetaStringType.NotNull())
				.Attribute("Title", "Имя", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationEdit() 
				.OperationList() 
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<CalendarDay>()
				.IntKey()
				.Attribute("Date", "Дата", MetaDateType.NotNull())
				.Attribute("IsWorkingDay", "Рабочий/Выходной", MetaBooleanType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.OperationDelete() 
				.OperationList() 
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationEdit(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<N_DDL>()
				.IntKey()
				.Attribute("Title", "Команда", MetaStringType.NotNull())
				.OperationList() 
			;	
			p.AddClass<TM_TaskExecution>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("ExecutionLog", "Журнал выполнения", MetaStringType.Null())
				.Attribute("FinishDate", "Дата/время окончания", MetaDateTimeType.Null())
				.Attribute("IsSuccessfull", "Результат", MetaBooleanType.NotNull())
				.Attribute("MachineName", "Имя машины", MetaStringType.NotNull())
				.Attribute("ResultXml", "Результат XML", MetaStringType.Null())
				.Attribute("StartDate", "Дата/время запуска", MetaDateTimeType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<TM_Task>("Task", "Задача")
				.OperationList() 
				.OperationView() 
			;	
			p.AddClass<N_Settings>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("SystemName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Value", "Значение", MetaStringType.NotNull())
				.Attribute("IsSystem", "Свойство является системным", MetaBooleanType.NotNull())
				.Attribute("AcceptableValues", "Список разрешенных значений", MetaStringType.Null())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Reference<N_SettingsGroup>("Group", "Группа параметра", x => x.InverseProperty("Settings"))
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<N_SettingsGroup>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Reference<N_Settings>("Settings", "Параметры системы", x => x.Multiple().InverseProperty("Group"))
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<N_TaskType>()
				.IntKey()
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Class", "Класс", MetaStringType.NotNull())
				.Attribute("Method", "Метод", MetaStringType.NotNull())
				.Attribute("IsActive", "Признак Активный", MetaBooleanType.NotNull())
				.Attribute("ArgumentClass", "Класс параметров", MetaStringType.Null())
				.Attribute("ResultClass", "Класс результата", MetaStringType.Null())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationView() 
			;	
			p.AddClass<N_Task>()
				.GuidKey("GUID")
				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())
				.Attribute("FinishTime", "Дата завершения", MetaDateTimeType.Null())
				.Attribute("Notify", "Уведомлять по e-mail", MetaBooleanType.NotNull())
				.Attribute("ErrorLogID", "Ид ошибки", MetaIntType.Null())
				.Attribute("ErrorLogMessage", "Текст ошибки", MetaStringType.Null())
				.Attribute("AbortRequestDate", "Дата запроса прерывания", MetaDateTimeType.Null())
				.Attribute("Status", "Статус", MetaIntType.NotNull())
				.Attribute("PercentDone", "% прогресса выполнения задания", MetaDecimalType.NotNull())
				.Attribute("PercentDoneDate", "Дата/время обновления прогресса", MetaDateTimeType.Null())
				.Attribute("Argument", "Параметры задания, сериализованные в формате JSON", MetaStringType.Null())
				.Attribute("Result", "Результат выполнения задания, сериализованный в формате JSON", MetaStringType.Null())
				.Attribute("StartTime", "Дата запуска", MetaDateTimeType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<SPM_Subject>("Creator", "Создал")
				.Reference<N_TaskType>("TaskType", "Тип задания")
				.OperationDelete() 
				.OperationEdit() 
				.OperationList() 
				.OperationView() 
			;	
			return p;
		}
	}
	public partial class MMS_ClassStereotype { }
	public partial class MMS_Versioning { }
	public partial class MMS_ChangeLog { }
	public partial class MMS_Replication { }

	public class StereotypesPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<MMS_ClassStereotype>()
				.IntKey()
			;	
			p.AddClass<MMS_Versioning>()
				.IntKey()
				.Attribute("Type", "Тип", MetaEnumType.NotNull("VersioningType"))
				.OperationEdit() 
			;	
			p.AddClass<MMS_ChangeLog>()
				.IntKey()
			;	
			p.AddClass<MMS_Replication>()
				.IntKey()
			;	
			return p;
		}
	}
	public partial class N_SqlStatementLog { }

	public class SqlExecutionPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<N_SqlStatementLog>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Текст запроса", MetaStringType.NotNull())
				.Attribute("IP", "IP-адрес", MetaStringType.NotNull())
				.OperationView() 
				.OperationList() 
				.Operation("Execute", "Выполнение SQL", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "exec")
				)
				.Operation("ExecuteHistory", "Выполнить из журнала", x => x 
					.ParmInt("oid")
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "exec")
				)
			;	
			return p;
		}
	}
	public partial class N_VirusScanLog { }
	public partial class N_Folder { }
	public partial class V_N_FolderFile { }
	public partial class N_File { }
	public partial class N_FileLibraryType { }
	public partial class N_FileLibrary { }
	public partial class N_DownloadLog { }

	public class FileStoragePackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<N_VirusScanLog>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Имя файла", MetaStringType.NotNull())
				.Attribute("ResultCode", "Код результата", MetaIntType.NotNull())
				.OperationEdit(x => x 
					.InvokesSingleObjectView("view")
				)
				.OperationView() 
				.OperationList() 
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
			;	
			p.AddClass<N_Folder>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("FullPath", "Полный путь", MetaStringType.Null())
				.Attribute("StorageType", "Тип хранения", MetaEnumType.NotNull("StorageType"))
				.Attribute("StorageParameter", "Параметр хранения", MetaStringType.Null())
				.Attribute("Guid", "Гуид", MetaGuidType.NotNull())
				.Attribute("GuidPath", "Путь Гуид", MetaStringType.Null())
				.Attribute("EnableVersioning", "Включить версионность", MetaBooleanType.NotNull())
				.Attribute("IsReplicable", "Реплицируемый", MetaBooleanType.NotNull())
				.Attribute("SPMActionItemGUID", "Гуид объекта проверки прав", MetaGuidType.NotNull())
				.Attribute("PublishDate", "Дата публикации", MetaDateTimeType.Null())
				.Attribute("Tag", "Tag", MetaStringType.Null())
				.ComputedAttribute("PhysicalPath", "Путь на диске", MetaStringType.Null())
				.Reference<N_Folder>("Parent", "Родительская папка", x => x.InverseProperty("Folders"))
				.Reference<N_Folder>("Folders", "Папки", x => x.Multiple().InverseProperty("Parent"))
				.Reference<N_File>("Files", "Файлы", x => x.Multiple().InverseProperty("Folder"))
				.Reference<SPM_Subject>("Creator", "Создавший пользователь")
				.OperationDelete() 
				.OperationList() 
				.Operation("CreateNew", "Создать папку", x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit")
				)
				.OperationEdit() 
				.Operation("Upload", "Загрузить файлы", x => x 
					.ParmString("parent")
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "upload")
				)
				.Operation("Predicates", "Права доступа", x => x 
					.InvokesSingleObjectView("predicates")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
				.Operation("PackAndDownload", "Упаковать и скачать", x => x 
					.ParmGuid("id")
				)
			;	
			p.AddClass<V_N_FolderFile>()
				.IntKey("ID")
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("ParentID", "Ид родительского объекта", MetaIntType.Null())
				.Attribute("Extension", "Тип", MetaStringType.NotNull())
				.Attribute("LastModifiedUserID", "Последний редактировавший пользователь", MetaIntType.NotNull())
				.Attribute("LastModifiedUserTitle", "Последний изменивший пользователь", MetaStringType.Null())
				.OperationList() 
				.OperationCreateNew(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationDelete(x => x 
					.InvokesSingleObjectView("delete")
				)
				.OperationEdit(x => x 
					.InvokesSingleObjectView("edit")
				)
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<N_File>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Password", "Password", MetaStringType.Null())
				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())
				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.NotNull())
				.Attribute("Extension", "Расширение", MetaStringType.NotNull())
				.Attribute("FeatureGUID", "Guid элемента", MetaGuidType.Null())
				.Attribute("Guid", "Guid", MetaGuidType.NotNull())
				.Attribute("GuidPath", "Путь Guid", MetaStringType.Null())
				.Attribute("IsDiskStorage", "Хранится на диске", MetaBooleanType.NotNull())
				.Attribute("StorageType", "Тип хранения", MetaEnumType.NotNull(""))
				.Attribute("Tag", "Tag", MetaStringType.Null())
				.Attribute("VersionNumber", "Номер версии", MetaIntType.NotNull())
				.Attribute("PublishDate", "Дата публикации", MetaDateTimeType.Null())
				.Attribute("Length", "Размер, байт", MetaLongType.NotNull())
				.Attribute("MainGUID", "Guid основной версии", MetaGuidType.Null())
				.Attribute("Path", "Путь", MetaStringType.Null())
				.Attribute("StorageParameter", "Параметр хранения", MetaStringType.Null())
				.ComputedAttribute("PhysicalPath", "Путь на диске", MetaStringType.Null())
				.PersistentComputedAttribute("IsDeleted", "Признак Удалено", MetaBooleanType.Null())
				.Reference<N_Folder>("Folder", "Папка", x => x.InverseProperty("Files"))
				.Reference<SPM_Subject>("CheckedOutBy", "Кем извлечено")
				.Reference<SPM_Subject>("Creator", "Создавший пользователь")
				.OperationEdit() 
				.Operation("EditG", "Редактировать", x => x 
					.ParmGuid("id")
					.InvokesSingleObjectView("edit")
				)
				.Operation("CreateNew", "Создать файл", x => x 
					.ParmInt("folderid")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit")
				)
				.OperationDelete() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
				.OperationList(x => x 
					.InvokesObjectListView("list")
				)
			;	
			p.AddClass<N_FileLibraryType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Extensions", "Расширения", MetaStringType.NotNull())
				.Attribute("ClassName", "Имя класса", MetaStringType.Null())
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
			;	
			p.AddClass<N_FileLibrary>()
				.IntKey("FolderID")
				.Attribute("MaxFileSize", "Максимальный размер файла", MetaIntType.NotNull())
				.Reference<N_FileLibraryType>("FileLibraryType", "Тип")
			;	
			p.AddClass<N_DownloadLog>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("FileGUID", "ИД Файла", MetaGuidType.NotNull())
				.Attribute("IP", "IP", MetaStringType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<N_File>("File", "Файл")
				.OperationList() 
			;	
			return p;
		}
	}
	public partial class N_ObjectChange { }
	public partial class N_ObjectPropertyChange { }

	public class ChangeHistoryPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<N_ObjectChange>()
				.IntKey()
				.Attribute("Details", "Дополнительная информация", MetaStringType.NotNull())
				.Attribute("IP", "IP", MetaStringType.NotNull())
				.Attribute("ObjectKey", "Ид объекта", MetaStringType.NotNull())
				.Attribute("ObjectTitle", "Название объекта", MetaStringType.NotNull())
				.Attribute("ObjectTypeSysName", "Системное имя типа объекта", MetaStringType.NotNull())
				.Attribute("ObjectTypeTitle", "Тип объекта", MetaStringType.NotNull())
				.Attribute("SubjectID", "Ид субъекта доступа", MetaIntType.NotNull())
				.Attribute("Title", "Действие", MetaStringType.NotNull())
				.Attribute("UserLogin", "Логин пользователя", MetaStringType.NotNull())
				.Attribute("UserTitle", "Имя пользователя", MetaStringType.NotNull())
				.Reference<N_ObjectPropertyChange>("PropertyChanges", "Изменения свойств", x => x.Multiple().InverseProperty("ObjectChange"))
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<N_ObjectPropertyChange>()
				.IntKey()
				.Attribute("NewValue", "Новое значение", MetaStringType.Null())
				.Attribute("NewValueTitle", "Отображаемый текст нового значения", MetaStringType.NotNull())
				.Attribute("OldValue", "Старое значение", MetaStringType.Null())
				.Attribute("OldValueTitle", "Отображаемый текст старого значения", MetaStringType.NotNull())
				.Attribute("PropertySysName", "Системное имя свойства", MetaStringType.NotNull())
				.Attribute("Title", "Наименование свойства", MetaStringType.NotNull())
				.Reference<N_ObjectChange>("ObjectChange", "Изменение объекта", x => x.InverseProperty("PropertyChanges"))
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			return p;
		}
	}
	public partial class N_Navig { }
	public partial class N_NavigItem { }
	public partial class N_Node { }
	public partial class CMSFormView { }

	public class NavigationPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{
			p.AddEnum("NavigItemType", "Тип элемента меню")
				.Value("T", "Text", "Текст")
				.Value("L", "Link", "Ссылка")
				.Value("N", "Node", "Страница сайта")
				.Value("C", "Control", "Компонент")
				.Value("M", "Method", "Метод, мастер-страница по умолчанию")
				.Value("V", "View", "Представление пакета, мастер-страница по умолчанию");



			p.AddClass<N_Navig>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("MaxLevels", "Максимальное количество уровней", MetaIntType.Null())
				.Reference<N_NavigItem>("Items", "Элементы", x => x.Multiple().InverseProperty("Navig"))
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationView() 
				.OperationList() 
			;	
			p.AddClass<N_NavigItem>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull(), true)
				.Attribute("FURL", "Friendly URL", MetaStringType.Null())
				.Attribute("URL", "Реальный URL", MetaStringType.Null())
				.Attribute("Type", "Тип", MetaEnumType.NotNull("NavigItemType"))
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Attribute("Tag", "Дополнительные данные", MetaStringType.Null())
				.Attribute("SPMActionGUID", "Защищаемая операция", MetaGuidType.Null())
				.Attribute("OldID", "Старый ИД", MetaIntType.Null())
				.Attribute("ImageURL", "URL картинки", MetaStringType.Null())
				.Attribute("Tooltip", "Подсказка", MetaStringType.Null())
				.Attribute("Expression", "Вычисляемая часть наименования", MetaStringType.Null())
				.Reference<N_Navig>("Navig", "Меню", x => x.InverseProperty("Items"))
				.Reference<N_NavigItem>("Parent", "Родительский элемент", x => x.InverseProperty("Child"))
				.Reference<N_Node>("Node", "Страница сайта")
				.Reference<MM_Method>("Method", "Метод")
				.Reference<MM_FormView>("FormView", "Представление")
				.Reference<N_NavigItem>("Child", "Дочерние элементы", x => x.Multiple().InverseProperty("Parent"))
				.OperationCreateNew(x => x 
					.ParmGuid("menuid")
					.ParmGuid("parentid")
					.ParmString("returnurl")
				)
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationUnDelete() 
				.Operation("EditRights", "Редактировать права", x => x 
					.InvokesSingleObjectView("spm")
				)
				.Operation("SortChildren", "Отсортировать дочерние элементы", x => x 
					.ParmGuid("id")
					.ParmString("returnurl")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmGuid("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmGuid("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<N_Node>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("NodeID", "Старый ид", MetaIntType.Null())
				.Attribute("IsDeleted", "Признак Опубликовано", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull(), true)
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Alt", "Подсказка", MetaStringType.Null(), true)
				.Attribute("ImageURL", "URL изображения", MetaStringType.Null())
				.Attribute("Parameters", "Параметры мастер-страницы", MetaStringType.Null())
				.Attribute("FURL", "Friendly URL", MetaStringType.Null())
				.Attribute("Description", "Описание", MetaStringType.Null(), true)
				.Attribute("Keywords", "Ключевые слова", MetaStringType.Null(), true)
				.Attribute("PageTitle", "Заголовок страниц", MetaStringType.Null(), true)
				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())
				.Attribute("PublishDate", "Дата публикации", MetaDateTimeType.Null())
				.Reference<MM_FormView>("MasterPage", "Мастер-страница")
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationView() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<CMSFormView>()
				.IntKey()
				.OperationCreateNew(x => x 
					.InvokesView("Nephrite.Web.ViewControl", "createnew")
				)
				.OperationDelete() 
				.OperationList() 
			;	
			return p;
		}
	}
	public partial class WF_Workflow { }
	public partial class WF_Activity { }
	public partial class WF_Transition { }

	public class WorkflowPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<WF_Workflow>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("IsActive", "Действующий", MetaBooleanType.NotNull())
				.Reference<MM_ObjectType>("ObjectType", "Тип объекта")
				.Reference<WF_Activity>("Activities", "Активности", x => x.Multiple().InverseProperty("Workflow"))
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationView() 
				.Operation("Generate", "Сгенерировать методы", x => x 
					.InvokesSingleObjectView("generate")
				)
			;	
			p.AddClass<WF_Activity>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("IsActive", "Действующее", MetaBooleanType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Reference<WF_Workflow>("Workflow", "Рабочий процесс", x => x.InverseProperty("Activities"))
				.Reference<WF_Activity>("ChildActivities", "Дочерние активности", x => x.Multiple().InverseProperty("ParentActivity"))
				.Reference<WF_Activity>("ParentActivity", "Родительская активность", x => x.InverseProperty("ChildActivities"))
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<WF_Transition>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("IsActive", "Действующий", MetaBooleanType.NotNull())
				.Attribute("SeqNo", "Порядковый номер", MetaIntType.NotNull())
				.Reference<WF_Activity>("Parent", "Исходная активность")
				.Reference<WF_Activity>("TargetActivity", "Целевая активность")
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationDelete() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView(x => x 
					.InvokesSingleObjectView("view")
				)
				.Operation("MoveUp", "Переместить вверх", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("MoveDown", "Переместить вниз", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			return p;
		}
	}

	public class TemplatesPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			return p;
		}
	}

	public class MasterPagesPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("InternalPage", "Шаблон страницы внутренней части", x => x 
					.InvokesView("MasterControl", "InternalPage")
			);
			p.Operation("InternalPageTitle", "Заголовок страниц", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "InternalPageTitle")
			);
			p.Operation("Redirect301", "301 redirect", x => x 
					.InvokesView("MasterControl", "Redirect301")
			);
			p.Operation("SitePageBootstrap", "Шаблон bootstrap", x => x 
					.InvokesView("MasterControl", "SitePageBootstrap")
			);

			return p;
		}
	}
	public partial class ErrorLog { }

	public class UtilsPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("DBReport", "Отчет по базе данных", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "DBReport")
			);
			p.Operation("utils", "Утилиты", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "utils")
			);
			p.Operation("theme", "Тема", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "theme")
			);
			p.Operation("spmtable", "Отчет по правам доступа", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "spmtable")
			);
			p.Operation("DbBackup", "Бэкап", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "DbBackup")
			);

			p.AddClass<ErrorLog>()
				.IntKey()
				.Attribute("ErrorDate", "Дата ошибки", MetaDateTimeType.NotNull())
				.Attribute("ErrorText", "Текст ошибки", MetaStringType.NotNull())
				.Attribute("Url", "Адрес", MetaStringType.Null())
				.Attribute("UrlReferrer", "Адрес, с которого перешли", MetaStringType.Null())
				.Attribute("UserHostName", "Имя хоста пользователя", MetaStringType.Null())
				.Attribute("UserHostAddress", "Адрес хоста пользователя", MetaStringType.Null())
				.Attribute("UserAgent", "User-Agent", MetaStringType.Null())
				.Attribute("RequestType", "Тип запроса", MetaStringType.Null())
				.Attribute("Headers", "Заголовки", MetaStringType.Null())
				.Attribute("SqlLog", "Протокол SQL", MetaStringType.Null())
				.Attribute("UserName", "Имя пользователя", MetaStringType.Null())
				.Attribute("Hash", "Хэш", MetaByteArrayType.Null())
				.Attribute("SimilarErrorID", "Похожая ошибка", MetaIntType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Operation("Delete", "Очистить", x => x 
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "delete")
				)
				.OperationList() 
				.OperationView() 
			;	
			return p;
		}
	}
	public partial class DbFolder { }
	public partial class DbFile { }
	public partial class DbItem { }

	public class FileStorage2Package
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<DbFolder>()
				.GuidKey("ID")
				.NonPersistent()
				.OperationDelete() 
				.OperationEdit() 
				.Operation("Create", "Создать папку", x => x 
					.ParmString("parentFolderID")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit")
				)
				.Operation("Upload", "Загрузка файлов", x => x 
					.InvokesSingleObjectView("upload")
				)
				.OperationView() 
				.Operation("PackAndDownload", "Упаковать и скачать", x => x 
					.ParmGuid("id")
				)
			;	
			p.AddClass<DbFile>()
				.GuidKey("ID")
				.NonPersistent()
				.OperationDelete() 
				.OperationEdit() 
				.Operation("Create", "Создать файл", x => x 
					.ParmString("parentFolderID")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit")
				)
				.OperationView() 
				.Operation("CheckOut", "Извлечь", x => x 
					.ParmGuid("id")
					.ParmString("returnurl")
				)
				.Operation("CheckIn", "Вернуть", x => x 
					.ParmGuid("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<DbItem>()
				.GuidKey("ID")
				.NonPersistent()
				.Operation("ViewIcons", "Список. Плитка", x => x 
					.InvokesObjectListView("icons")
				)
				.Operation("Tags", "Теги", x => x 
					.ParmGuid("iod")
					.ParmString("returnurl")
					.InvokesSingleObjectView("tags")
				)
				.OperationList() 
				.Operation("Comments", "Комментарии", x => x 
					.ParmGuid("iod")
					.ParmString("returnurl")
					.InvokesSingleObjectView("comments")
				)
			;	
			return p;
		}
	}
	public partial class C_FIAS_ActualStatus { }
	public partial class C_FIAS_AddressObject { }
	public partial class C_FIAS_CenterStatus { }
	public partial class C_FIAS_OperationStatus { }
	public partial class C_FIAS_CurrentStatus { }
	public partial class C_FIAS_NormativeDocument { }
	public partial class C_FIAS_House { }
	public partial class C_FIAS_EstateStatus { }
	public partial class C_FIAS_IntervalStatus { }
	public partial class C_FIAS_StructureStatus { }
	public partial class C_FIAS_HouseStateStatus { }
	public partial class C_FIAS_HouseInterval { }
	public partial class C_FIAS_Landmark { }
	public partial class C_FIAS_AddressObjectType { }

	public class FIASPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<C_FIAS_ActualStatus>()
				.IntKey("ActStatID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_AddressObject>()
				.GuidKey("AoID")
				.Attribute("AoGUID", "Глобальный уникальный идентификатор", MetaGuidType.NotNull())
				.Attribute("AoLevel", "Уровень адресного объекта", MetaIntType.NotNull())
				.Attribute("AreaCode", "Код района", MetaStringType.NotNull())
				.Attribute("AutoCode", "Код автономии ", MetaStringType.NotNull())
				.Attribute("CityCode", "Код города", MetaStringType.NotNull())
				.Attribute("Code", "Код адресного объекта одной строкой с признаком актуальности из КЛАДР 4.0.", MetaStringType.Null())
				.Attribute("CtarCode", "Код внутригородского района", MetaStringType.NotNull())
				.Attribute("EndDate", "Окончание действия записи", MetaDateTimeType.NotNull())
				.Attribute("ExtrCode", "Код дополнительного адресообразующего элемента", MetaStringType.NotNull())
				.Attribute("FormalName", "Формализованное наименование", MetaStringType.NotNull())
				.Attribute("IFNSFL", "Код ИФНС ФЛ", MetaStringType.Null())
				.Attribute("IFNSUL", "Код ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("LiveStatus", "Признак действующего адресного объекта", MetaIntType.NotNull())
				.Attribute("NextID", "Идентификатор записи  связывания с последующей исторической записью", MetaGuidType.Null())
				.Attribute("OffName", "Официальное наименование", MetaStringType.Null())
				.Attribute("OKATO", "OKATO", MetaStringType.Null())
				.Attribute("OKTMO", "OKTMO", MetaStringType.Null())
				.Attribute("ParentGuid", "Идентификатор объекта родительского объекта", MetaGuidType.Null())
				.Attribute("PlaceCode", "Код населенного пункта", MetaStringType.NotNull())
				.Attribute("PostalCode", "Почтовый индекс", MetaStringType.Null())
				.Attribute("PrevID", "Идентификатор записи связывания с предыдушей исторической записью", MetaGuidType.Null())
				.Attribute("RegionCode", "Код региона", MetaStringType.NotNull())
				.Attribute("SextCode", "Код подчиненного дополнительного адресообразующего элемента", MetaStringType.NotNull())
				.Attribute("ShortName", "Краткое наименование типа объекта", MetaStringType.NotNull())
				.Attribute("StartDate", "Начало действия записи", MetaDateTimeType.NotNull())
				.Attribute("StreetCode", "Код улицы", MetaStringType.NotNull())
				.Attribute("TerrIFNSFL", "Код территориального участка ИФНС ФЛ", MetaStringType.Null())
				.Attribute("TerrIFNSUL", "Код территориального участка ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("UpdateDate", "Дата  внесения записи", MetaDateTimeType.NotNull())
				.Attribute("PlainCode", "Код адресного объекта из КЛАДР 4.0 одной строкой без признака актуальности (последних двух цифр)", MetaStringType.Null())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Полное наименование", MetaStringType.Null())
				.Reference<C_FIAS_ActualStatus>("ActStatus", "Статус актуальности адресного объекта ФИАС")
				.Reference<C_FIAS_CenterStatus>("CentStatus", "Статус центра")
				.Reference<C_FIAS_CurrentStatus>("CurrStatus", "Статус актуальности КЛАДР 4 (последние две цифры в коде)")
				.Reference<C_FIAS_NormativeDocument>("NormDoc", "Внешний ключ на нормативный документ")
				.Reference<C_FIAS_OperationStatus>("OperStatus", "Статус действия над записью – причина появления записи")
			;	
			p.AddClass<C_FIAS_CenterStatus>()
				.IntKey("CenterStID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_OperationStatus>()
				.IntKey("OperStatID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_CurrentStatus>()
				.IntKey("CurentStID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_NormativeDocument>()
				.GuidKey("NormDocID")
				.Attribute("DocDate", "Дата документа", MetaDateTimeType.Null())
				.Attribute("DocDateSpecified", "DocDateSpecified", MetaBooleanType.NotNull())
				.Attribute("DocImgID", "Идентификатор образа (внешний ключ)", MetaIntType.Null())
				.Attribute("DocName", "Наименование документа", MetaStringType.Null())
				.Attribute("DocNum", "Номер документа", MetaStringType.Null())
				.Attribute("DocType", "Тип документа", MetaIntType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_House>()
				.GuidKey("HouseID")
				.Attribute("BuildNum", "Номер корпуса", MetaStringType.Null())
				.Attribute("Counter", "Счетчик записей домов для КЛАДР 4", MetaIntType.NotNull())
				.Attribute("EndDate", "Окончание действия записи", MetaDateTimeType.NotNull())
				.Attribute("HouseGuid", "Глобальный уникальный идентификатор дома", MetaGuidType.NotNull())
				.Attribute("IFNSUL", "Код ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("OKATO", "OKATO", MetaStringType.Null())
				.Attribute("OKTMO", "OKTMO", MetaStringType.Null())
				.Attribute("PostalCode", "Почтовый индекс", MetaStringType.Null())
				.Attribute("StartDate", "Начало действия записи", MetaDateTimeType.NotNull())
				.Attribute("StrucNum", "Номер строения", MetaStringType.Null())
				.Attribute("TerrIFNSFL", "Код территориального участка ИФНС ФЛ", MetaStringType.Null())
				.Attribute("TerrIFNSUL", "Код территориального участка ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("UpdateDate", "Дата время внесения записи", MetaDateTimeType.NotNull())
				.Attribute("HouseNum", "Номер дома", MetaStringType.Null())
				.Attribute("IFNSFL", "Код ИФНС ФЛ", MetaStringType.Null())
				.PersistentComputedAttribute("Title", "Полное наименование", MetaStringType.Null())
				.Reference<C_FIAS_AddressObject>("Ao", "Родительский объект (улица, город, населеннй пункт и т.п.)")
				.Reference<C_FIAS_EstateStatus>("EstStatus", "Признак владения")
				.Reference<C_FIAS_NormativeDocument>("NormDoc", "Нормативный документ")
				.Reference<C_FIAS_HouseStateStatus>("StatStatus", "Состояние дома")
				.Reference<C_FIAS_StructureStatus>("StrStatus", "Признак строения")
			;	
			p.AddClass<C_FIAS_EstateStatus>()
				.IntKey("EstStatID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.Attribute("ShortName", "Краткое наименование", MetaStringType.Null())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_IntervalStatus>()
				.IntKey("IntvStatID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_StructureStatus>()
				.IntKey("StrStatID")
				.Attribute("Name", "Наименование", MetaStringType.Null())
				.Attribute("ShortName", "Краткое наименование", MetaStringType.Null())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_HouseStateStatus>()
				.IntKey("HouseStID")
				.Attribute("Name", "Наименование", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			p.AddClass<C_FIAS_HouseInterval>()
				.GuidKey("HouseIntID")
				.Attribute("Counter", "Счетчик записей домов для КЛАДР 4", MetaIntType.NotNull())
				.Attribute("EndDate", "Окончание действия записи", MetaDateTimeType.NotNull())
				.Attribute("IFNSFL", "Код ИФНС ФЛ", MetaStringType.Null())
				.Attribute("IFNSUL", "Код ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("IntEnd", "Значение окончания интервала", MetaIntType.NotNull())
				.Attribute("IntGUID", "Глобальный уникальный идентификатор", MetaGuidType.NotNull())
				.Attribute("IntStart", "Значение начала интервала", MetaIntType.NotNull())
				.Attribute("OKATO", "OKATO", MetaStringType.Null())
				.Attribute("OKTMO", "OKTMO", MetaStringType.Null())
				.Attribute("PostalCode", "Почтовый индекс", MetaStringType.Null())
				.Attribute("StartDate", "Начало действия записи", MetaDateTimeType.NotNull())
				.Attribute("TerrIFNSFL", "Код территориального участка ИФНС ФЛ", MetaStringType.Null())
				.Attribute("TerrIFNSUL", "Код территориального участка ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("UpdateDate", "Дата  внесения записи", MetaDateTimeType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.Reference<C_FIAS_AddressObject>("Ao", "Идентификатор объекта")
				.Reference<C_FIAS_IntervalStatus>("IntStatus", "Статус интервала")
				.Reference<C_FIAS_NormativeDocument>("NormDoc", "Нормативный документ")
			;	
			p.AddClass<C_FIAS_Landmark>()
				.GuidKey("LandID")
				.Attribute("EndDate", "Окончание действия записи", MetaDateTimeType.NotNull())
				.Attribute("IFNSFL", "Код ИФНС ФЛ", MetaStringType.Null())
				.Attribute("IFNSUL", "Код ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("LandGUID", "Глобальный уникальный идентификатор", MetaGuidType.NotNull())
				.Attribute("Location", "Месторасположение ориентира", MetaStringType.NotNull())
				.Attribute("OKATO", "OKATO", MetaStringType.Null())
				.Attribute("OKTMO", "OKTMO", MetaStringType.Null())
				.Attribute("PostalCode", "Почтовый индекс ", MetaStringType.Null())
				.Attribute("StartDate", "Начало действия записи", MetaDateTimeType.NotNull())
				.Attribute("TerrIFNSFL", "Код территориального участка ИФНС ФЛ", MetaStringType.Null())
				.Attribute("TerrIFNSUL", "Код территориального участка ИФНС ЮЛ", MetaStringType.Null())
				.Attribute("UpdateDate", "Дата  внесения записи", MetaDateTimeType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.Reference<C_FIAS_AddressObject>("Ao", "Родительский объект (улици, город и т.п.)")
				.Reference<C_FIAS_NormativeDocument>("NormDoc", "Нормативный документ")
			;	
			p.AddClass<C_FIAS_AddressObjectType>()
				.IntKey("KOD_T_ST")
				.Attribute("Level", "Уровень адресного объекта", MetaIntType.NotNull())
				.Attribute("ScName", "Краткое наименование типа объекта", MetaStringType.Null())
				.Attribute("SocrName", "Полное наименование типа объекта", MetaStringType.NotNull())
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
			;	
			return p;
		}
	}
	public partial class OrgUnit { }
	public partial class C_OrgUnitHie { }
	public partial class OrgUnitAsso { }
	public partial class C_OrgUnitType { }
	public partial class OrgUnitHieRule { }
	public partial class Employee { }
	public partial class OrgUnitResponsible { }

	public class OrgStructurePackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<OrgUnit>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("VipNetCode", "VipNet код", MetaStringType.Null())
				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())
				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.Null())
				.Attribute("ExternalID", "Ид во внешней системе", MetaStringType.Null())
				.Attribute("SysName", "Системное имя", MetaStringType.Null())
				.Attribute("Title", "Наименование", MetaStringType.NotNull(), true)
				.Attribute("VersionNumber", "Номер версии", MetaIntType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("PostAddress", "Почтовый адрес", MetaStringType.Null())
				.Attribute("EMail", "Адрес электронной почты", MetaStringType.Null())
				.Attribute("ChiefPost", "Должность руководителя", MetaStringType.Null())
				.Attribute("ChiefPhone", "Телефон руководителя", MetaStringType.Null())
				.Attribute("ContactName", "Ф.И.О. контактного лица", MetaStringType.Null())
				.Attribute("ContactPost", "Должность контактного лица", MetaStringType.Null())
				.Attribute("ContactPhone", "Телефон контактного лица", MetaStringType.Null())
				.Attribute("ContactEMail", "Адрес электронной почты контактного лица", MetaStringType.Null())
				.Attribute("ChiefName", "Ф.И.О. руководителя", MetaStringType.Null())
				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())
				.Attribute("TitleGenetive", "Наименование (родительный падеж)", MetaStringType.Null())
				.Attribute("SpecCalc", "Спецрасчет", MetaBooleanType.NotNull())
				.Reference<OrgUnitAsso>("FromChildAsso", "Ассоциация дочернего элемента", x => x.Multiple().InverseProperty("ChildOrgUnit"))
				.Reference<OrgUnitAsso>("FromParentAsso", "Ассоциация родительского элемента", x => x.Multiple().InverseProperty("ParentOrgUnit"))
				.Reference<OrgUnit>("Main", "Основная версия")
				.Reference<C_OrgUnitType>("Type", "Тип")
				.Reference<C_RFSubject>("RFSubjects", "Субъекты РФ", x => x.Multiple())
				.Reference<C_PostPart>("PostParts", "Перечень должностей", x => x.Multiple())
				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")
				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")
				.OperationView() 
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("typeid")
					.ParmString("returnurl")
				)
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<C_OrgUnitHie>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<OrgUnitAsso>()
				.GuidKey()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.Reference<OrgUnit>("ChildOrgUnit", "Дочерний элемент", x => x.InverseProperty("FromChildAsso"))
				.Reference<C_OrgUnitHie>("OrgUnitHie", "Иерархия")
				.Reference<OrgUnit>("ParentOrgUnit", "Родительский элемент", x => x.InverseProperty("FromParentAsso"))
			;	
			p.AddClass<C_OrgUnitType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull(), true)
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.OperationEdit() 
				.OperationCreateNew() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationDelete() 
			;	
			p.AddClass<OrgUnitHieRule>()
				.GuidKey()
				.ComputedAttribute("Title", "Title", MetaStringType.Null())
				.Reference<C_OrgUnitType>("ChildOrgUnitType", "Дочерний элемент")
				.Reference<C_OrgUnitHie>("OrgUnitHie", "Иерархия")
				.Reference<C_OrgUnitType>("ParentOrgUnitType", "Родительский элемент")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<Employee>()
				.GuidKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())
				.Attribute("BirthDate", "Дата рождения", MetaDateType.Null())
				.Attribute("Email", "E-mail", MetaStringType.Null())
				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.Null())
				.Attribute("Firstname", "Имя", MetaStringType.NotNull(), true)
				.Attribute("HaveAccessToPC", "Наличие доступа к компьютеру", MetaBooleanType.Null())
				.Attribute("IsChief", "Руководитель подразделения", MetaBooleanType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Job", "Должность", MetaStringType.Null(), true)
				.Attribute("Mobile", "Мобильный телефон", MetaStringType.Null())
				.Attribute("Patronymic", "Отчество", MetaStringType.Null(), true)
				.Attribute("Phone", "Рабочий телефон", MetaStringType.Null())
				.Attribute("Photo", "Фото", MetaFileType.Null())
				.Attribute("Surname", "Фамилия", MetaStringType.NotNull(), true)
				.Attribute("VersionNumber", "Номер версии", MetaIntType.NotNull())
				.Attribute("ExternalID", "Внешний идентификатор", MetaStringType.Null())
				.PersistentComputedAttribute("Title", "ФИО", MetaStringType.Null(), true)
				.Reference<Employee>("Chief", "Вышестоящий сотрудник")
				.Reference<Employee>("Main", "Основная версия")
				.Reference<OrgUnit>("OrgUnit", "Организационная единица")
				.Reference<SPM_Subject>("Subject", "Пользователь")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<OrgUnitResponsible>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Ф.И.О", MetaStringType.NotNull())
				.Attribute("Phone", "Телефон", MetaStringType.Null())
				.Attribute("EMail", "Адрес электронной почты", MetaStringType.Null())
				.Attribute("IsReserved", "Дублирующий", MetaBooleanType.NotNull())
				.Attribute("BeginDate", "Дата начала замены", MetaDateType.Null())
				.Attribute("EndDate", "Дата окончания замены", MetaDateType.Null())
				.Attribute("Post", "Должность", MetaStringType.Null())
				.Attribute("StructUnit", "Структурное подразделение", MetaStringType.Null())
				.Reference<OrgUnit>("OrgUnit", "Отделение ПФР")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			return p;
		}
	}
	public partial class Doc { }
	public partial class Citizen { }
	public partial class DocTask { }
	public partial class PensionFile { }
	public partial class JuridicalCase { }
	public partial class CitizenRequest { }
	public partial class DocTaskOperation { }
	public partial class WorkInfo { }
	public partial class DocAsso { }
	public partial class Appendix { }
	public partial class DocTaskRequest { }
	public partial class CitizenPension { }
	public partial class MassCalc { }
	public partial class ControlTask { }
	public partial class Complaint { }
	public partial class CourtSession { }
	public partial class DocTaskComplaint { }
	public partial class OutDocSend { }
	public partial class OfficeNote { }
	public partial class MassOperationQueue { }
	public partial class InfoDoc { }

	public class CivilServantsPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("About", "О системе", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "About")
			);
			p.Operation("GenerateDB2Inserts", "Импорт данных в DB2", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "GenerateDB2Inserts")
			);
			p.Operation("FileStorageService", "Сервис файлового хранилища", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "FileStorageService")
			);
			p.Operation("depersonalize", "Подготовка дампа с деперсонализацией", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "depersonalize")
			);

			p.AddClass<Doc>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("PrintNo", "Номер для печати", MetaStringType.Null())
				.Attribute("IsSecond", "Повторное", MetaBooleanType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("PPAddress", "Адрес проживания", MetaStringType.Null())
				.Attribute("PPPhone", "Телефон", MetaStringType.Null())
				.Attribute("OuterRegDate", "Дата регистрации", MetaDateType.Null())
				.Attribute("SendDate", "Дата отправки", MetaDateType.Null())
				.Attribute("File", "Электронный образ документа", MetaFileType.Null())
				.Attribute("CitizenBirthdate", "Дата рождения", MetaDateType.Null())
				.Attribute("CitizenSnils", "СНИЛС", MetaStringType.Null())
				.Attribute("CompleteDate", "Дата завершения обработки документа", MetaDateType.Null())
				.Attribute("JPSignPerson", "Подписант документа (Ф.И.О.)", MetaStringType.Null())
				.Attribute("PPName", "Ф.И.О.", MetaStringType.Null())
				.Attribute("RegDate", "Дата регистрации", MetaDateType.Null())
				.Attribute("RegNo", "Номер регистрации", MetaStringType.Null())
				.Attribute("IsCheckDeadline", "Признак контроля исполнения документа", MetaBooleanType.NotNull())
				.Attribute("MilestoneDate", "Контрольный срок исполнения документа", MetaDateType.Null())
				.Attribute("OuterRegNo", "Номер регистрации", MetaStringType.Null())
				.Attribute("JPSignPersonPost", "Должность подписанта", MetaStringType.Null())
				.Attribute("CitizenSurname", "Фамилия", MetaStringType.Null())
				.Attribute("CitizenFirstname", "Имя", MetaStringType.Null())
				.Attribute("CitizenPatronymic", "Отчество", MetaStringType.Null())
				.Attribute("CitizenAddress", "Адрес", MetaStringType.Null())
				.Attribute("IsApproved", "Проверен", MetaBooleanType.NotNull())
				.Attribute("Guid", "Гуид", MetaGuidType.Null())
				.Attribute("CitizenIndex", "Индекс", MetaStringType.Null())
				.Attribute("CitizenVIP", "VIP", MetaBooleanType.NotNull())
				.Attribute("DecisionDocNo", "Номер решения ФО", MetaStringType.Null())
				.Attribute("DecisionDocDate", "Дата решения ФО", MetaDateType.Null())
				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())
				.Attribute("RecordStatementNo", "Номер описи документов", MetaStringType.Null())
				.Attribute("RecordStatementDate", "Дата формирования описи", MetaDateType.Null())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("LastRegDate", "Дата регистрации последнего документа в цепочке", MetaDateType.Null())
				.Attribute("DocNo", "Порядовый номер для регномера", MetaIntType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.ComputedAttribute("TitleWithSender", "Наименование с отправителем", MetaStringType.Null())
				.PersistentComputedAttribute("CitizenTitle", "Ф.И.О. гражданина", MetaStringType.Null())
				.Reference<OrgUnit>("Federal", "Федеральный орган")
				.Reference<C_District>("District", "Территория")
				.Reference<DocTaskOperation>("DecisionOperation", "Решен операцией")
				.Reference<C_DocClass>("DocClass", "Тип документа")
				.Reference<C_DocType>("DocType", "Вид документа")
				.Reference<Employee>("Creator", "Автор")
				.Reference<OrgUnit>("Sender", "Отправитель")
				.Reference<Employee>("InnerSignPerson", "Подписант документа")
				.Reference<C_ProvisionMode>("ProvisionMode", "Способ передачи документа")
				.Reference<Appendix>("Appendix", "Приложения", x => x.Multiple().InverseProperty("Doc"))
				.Reference<C_RFSubject>("CitizenRFSubject", "Субъект РФ")
				.Reference<Employee>("RegEmployee", "Регистратор документа")
				.Reference<DocTask>("DocTasks", "Задания", x => x.Multiple().InverseProperty("Doc"))
				.Reference<C_SenderCategory>("SenderCategory", "Категория отправителя")
				.Reference<PensionFile>("PensionFile", "Пенсионное дело")
				.Reference<DocTask>("DocTask", "Является результатом задания", x => x.InverseProperty("ResultDocs"))
				.Reference<MassCalc>("MassCalc", "Является результатом массового перерасчета")
				.Reference<DocTaskOperation>("DocTaskOperation", "Является результатом операции")
				.Reference<WF_Activity>("Activity", "Activity")
				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")
				.Operation("PrintExcel", "Экспорт в Excel", x => x 
					.ParmInt("filterid")
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "printexcel")
				)
				.Operation("SendingOut", "Формирование пакетов документов для отправки", x => x 
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "sendingout")
				)
				.Operation("CreateOutForTask", "Приложить исх. к заданию", x => x 
					.ParmInt("taskid")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editoutfortask")
				)
				.Operation("ViewEl", "Просмотр для пакета (эл.)", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("viewpackel")
				)
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("UploadFiles", "Загрузка образов документов", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("uploadfiles")
				)
				.Operation("Register", "Регистрация", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("register")
				)
				.Operation("Print", "Печать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("print")
				)
				.Operation("ViewList_In", "Входящие документы", x => x 
					.InvokesObjectListView("list_in")
				)
				.Operation("ViewList_Out", "Исходящие документы", x => x 
					.InvokesObjectListView("list_out")
				)
				.Operation("ViewList_9", "Письменные обращения граждан", x => x 
					.InvokesObjectListView("list_9")
				)
				.OperationList(x => x 
					.ParmInt("classid")
					.ParmString("types")
				)
				.Operation("PrintWord", "Печать приложения", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.OperationCreateNew(x => x 
					.ParmInt("classid")
					.ParmString("types")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<Citizen>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Surname", "Фамилия", MetaStringType.NotNull())
				.Attribute("Firstname", "Имя", MetaStringType.NotNull())
				.Attribute("Patronymic", "Отчество", MetaStringType.Null())
				.Attribute("Birthdate", "Дата рождения", MetaDateType.Null())
				.Attribute("Deathdate", "Дата смерти", MetaDateType.Null())
				.Attribute("Passport", "Паспортные данные", MetaStringType.Null())
				.Attribute("Phone", "Домашний телефон", MetaStringType.Null())
				.Attribute("Address", "Адрес", MetaStringType.Null())
				.Attribute("Snils", "СНИЛС", MetaStringType.Null())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())
				.Attribute("SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())
				.Attribute("FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())
				.Attribute("FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())
				.Attribute("PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())
				.Attribute("PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())
				.Attribute("IsTrackingData", "Признак учета контрольных данных", MetaBooleanType.NotNull())
				.Attribute("Sex", "Пол", MetaEnumType.NotNull(""))
				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())
				.Attribute("InWork", "В работе", MetaBooleanType.NotNull())
				.Attribute("RFSubjectText", "Субъект РФ (название)", MetaStringType.Null())
				.Attribute("Index", "Индекс", MetaStringType.Null())
				.Attribute("VIP", "VIP", MetaBooleanType.NotNull())
				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())
				.PersistentComputedAttribute("Title", "Ф.И.О.", MetaStringType.Null())
				.PersistentComputedAttribute("FullTitle", "Ф.И.О. полное", MetaStringType.Null())
				.PersistentComputedAttribute("FullTitleWithBirth", "Ф.И.О. полное с др", MetaStringType.Null())
				.Reference<C_SpecialNote>("SpecialNote", "Особые отметки", x => x.Multiple())
				.Reference<C_District>("District", "Территория")
				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")
				.Reference<C_CitizenCategory>("Category", "Категория")
				.Reference<OrgUnit>("OrgUnit", "Отделение ПФР")
				.Reference<CitizenPension>("CitizenPension", "Трудовая пенсия", x => x.Multiple().InverseProperty("Citizen"))
				.Operation("ViewList_Compl", "С жалобами", x => x 
					.InvokesObjectListView("list_compl")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("ViewList_Pens", "Получатели ГПО", x => x 
					.InvokesObjectListView("list_pens")
				)
				.Operation("ViewList_Req", "Обратившиеся граждане", x => x 
					.InvokesObjectListView("list_req")
				)
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
			;	
			p.AddClass<DocTask>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("ReturnedDate", "Дата возврата", MetaDateTimeType.Null())
				.Attribute("CheckedDate", "Дата окончания проверки", MetaDateTimeType.Null())
				.Attribute("DocTaskResultID", "Результат выполнения", MetaIntType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата выдачи задания", MetaDateTimeType.Null())
				.Attribute("PlanCompleteDate", "Плановая дата исполнения задания", MetaDateType.Null())
				.Attribute("AssignmentDate", "Дата назначения исполнителя", MetaDateTimeType.Null())
				.Attribute("CompleteDate", "Дата исполнения задания", MetaDateTimeType.Null())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("CloseDate", "Дата закрытия задания", MetaDateTimeType.Null())
				.Attribute("OnCheckDate", "Дата начала проверки", MetaDateTimeType.Null())
				.Attribute("Copy_Citizen_Surname", "Фамилия", MetaStringType.Null())
				.Attribute("Copy_Citizen_SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_Firstname", "Имя", MetaStringType.Null())
				.Attribute("Copy_Citizen_FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_Patronymic", "Отчество", MetaStringType.Null())
				.Attribute("Copy_Citizen_PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_RFSubjectText", "Субъект РФ", MetaStringType.Null())
				.Attribute("Copy_Citizen_Index", "Индекс", MetaStringType.Null())
				.Attribute("Copy_Citizen_Address", "Адрес", MetaStringType.Null())
				.Attribute("Copy_Citizen_VIP", "VIP", MetaBooleanType.NotNull())
				.Attribute("Copy_Citizen_FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())
				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())
				.Attribute("AnnulmentDate", "Дата аннулирования", MetaDateType.Null())
				.Attribute("SuspendDate", "Дата откладывания", MetaDateType.Null())
				.Attribute("State", "Состояние", MetaStringType.Null())
				.Attribute("SignDate", "Дата подписи", MetaDateType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.PersistentComputedAttribute("Status", "Статус", MetaStringType.Null())
				.Reference<Doc>("Doc", "Документ - основание", x => x.InverseProperty("DocTasks"))
				.Reference<Employee>("Performer", "Исполнитель")
				.Reference<Employee>("ChiefPerformer", "Ответственный исполнитель")
				.Reference<Employee>("Author", "Автор")
				.Reference<Citizen>("Citizen", "Гражданин")
				.Reference<C_DocTaskType>("Type", "Тип")
				.Reference<Doc>("ResultDocs", "Документы - результаты", x => x.Multiple().InverseProperty("DocTask"))
				.Reference<WF_Activity>("Activity", "Activity")
				.Operation("CreateDocs", "Создать документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("createdocs")
				)
				.Operation("Complete", "Выполнить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("complete")
				)
				.Operation("Checked", "Проверить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("close")
				)
				.Operation("UnApprove", "На доработку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unapprove")
				)
				.Operation("StatusHistory", "История изменения статуса", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("statushistory")
				)
				.Operation("DownloadDocs", "Скачать документы ", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("downloaddocs")
				)
				.Operation("Close", "Завершить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("closecorrected")
				)
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("ViewList", "Все задания") 
				.Operation("MyViewList", "Мои задания") 
				.Operation("ViewListOnCheck", "Задания на проверку") 
				.Operation("ViewListExceeded", "Задания на контроле") 
				.Operation("ViewListAnnulment", "Аннулированные задания") 
				.Operation("ViewListSuspended", "Отложенные задания") 
				.Operation("ViewListSigning", "Задания на подписании") 
				.Operation("ViewListClosed", "Завершенные задания") 
				.Operation("DeleteDocs", "Удалить документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("ReturnTasks", "Вернуть на доработку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("SignTasks", "Подписать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<PensionFile>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Номер", MetaStringType.NotNull())
				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())
				.Attribute("ArchiveDate", "Дата передачи в архив", MetaDateType.Null())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Reference<Citizen>("Citizen", "Гражданин")
				.Reference<Employee>("CreateEmployee", "Сотрудник, создавший дело")
				.Reference<Employee>("CloseEmployee", "Сотрудник, закрывший дело")
				.Reference<Employee>("ArchiveEmployee", "Сотрудник, передавший дело в архив")
				.Operation("Archive", "Архивирование", x => x 
					.ParmString("returnurl")
					.InvokesView("Nephrite.Web.ViewControl", "archive")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationView() 
				.OperationList() 
				.Operation("ViewListArch", "Список - архив") 
				.Operation("UnArchieve", "Восстановить из архива", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<JuridicalCase>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Номер", MetaStringType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())
				.Attribute("ArchiveDate", "Дата передачи в архив", MetaDateType.Null())
				.Reference<Citizen>("Citizen", "Гражданин")
				.Reference<Employee>("CreateEmployee", "Сотрудник, создавший дело")
				.Reference<Employee>("CloseEmployee", "Сотрудник, закрывший дело")
				.Reference<Employee>("ArchiveEmployee", "Сотрудник, передавший дело в архив")
				.Reference<OrgUnit>("OrgUnit", "Судебный орган")
				.OperationDelete() 
				.OperationCreateNew() 
				.Operation("Edit", "Архивирование", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit")
				)
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("ViewListArch", "Список - архив") 
				.Operation("UnArchieve", "Восстановить из архива", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<CitizenRequest>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Регистрационный номер", MetaStringType.NotNull())
				.Attribute("RequestDate", "Дата обращения", MetaDateTimeType.NotNull())
				.Attribute("Surname", "Фамилия", MetaStringType.NotNull())
				.Attribute("Firstname", "Имя", MetaStringType.NotNull())
				.Attribute("Patronymic", "Отчество", MetaStringType.Null())
				.Attribute("Description", "Краткое описание проблемы", MetaStringType.NotNull())
				.Reference<Employee>("RecorderEmployee", "Сотрудник зарегистрировавший обращение")
				.Reference<C_RequestMethod>("RequestMethod", "Способ обращения")
				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")
				.Reference<C_RequestCategory>("RequestCategory", "Категория обращения")
				.Reference<C_RequestResultCategory>("RequestResult", "Результат обращения")
				.Reference<Citizen>("Citizen", "Гражданин")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<DocTaskOperation>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Basis", "Основа для расчета", MetaBooleanType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("CalcAmount", "Рассчитанный размер", MetaDecimalType.Null())
				.Attribute("CalcSalary", "СДВ", MetaDecimalType.Null())
				.Attribute("Seniority", "Стаж", MetaIntType.Null())
				.Attribute("IsApprove", "Решение", MetaBooleanType.NotNull())
				.Attribute("PaymentStartDate", "Дата начала исполнения", MetaDateType.Null())
				.Attribute("FGASeniority", "Стаж (фед)", MetaIntType.Null())
				.Attribute("FGAPercent", "% начисл. (фед)", MetaDecimalType.Null())
				.Attribute("FGAMidSalary", "СМЗ (фед)", MetaDecimalType.Null())
				.Attribute("FGASalary", "Должностной оклад (фед)", MetaDecimalType.Null())
				.Attribute("Restriction", "Верхний коэффициент ограничения", MetaDecimalType.Null())
				.Attribute("DecisionDate", "Дата решения", MetaDateType.Null())
				.Attribute("SalaryIncS", "Надбавка за сложность", MetaDecimalType.Null())
				.Attribute("SalaryIncK", "Надбавка за квалификацию", MetaDecimalType.Null())
				.Attribute("SalaryIncU", "Надбавка за особые условия", MetaDecimalType.Null())
				.Attribute("SalaryIncL", "Надбавка за выслугу лет", MetaDecimalType.Null())
				.Attribute("Bonus", "Премия", MetaDecimalType.Null())
				.Attribute("PaymentEndDate", "Дата окончания исполнения", MetaDateType.Null())
				.Attribute("RestrictionMin", "Нижний коэффициент ограничения", MetaDecimalType.Null())
				.Attribute("SalaryIncSek", "Надбавка за работу с секретными сведениями", MetaDecimalType.Null())
				.Attribute("IsTracking", "Признак Контрольная", MetaBooleanType.NotNull())
				.Attribute("IsLast", "Признак Последняя", MetaBooleanType.NotNull())
				.Attribute("RegionalRatioValue", "Значение регионального коэффициента", MetaDecimalType.Null())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("SumIndexing", "Суммарный коэффициент индексации", MetaDecimalType.NotNull())
				.Attribute("SalaryDate", "Дата оклада", MetaDateType.Null())
				.Attribute("SumIndexingPrev", "Суммарный коэффициент индексации до даты оклада", MetaDecimalType.NotNull())
				.Attribute("RaiseRatioValue", "Значение коэффициента увеличения", MetaDecimalType.Null())
				.Attribute("ActAblative", "НПА для перерасчета", MetaStringType.Null())
				.Attribute("Copy_Citizen_Surname", "Фамилия", MetaStringType.Null())
				.Attribute("Copy_Citizen_SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_Firstname", "Имя", MetaStringType.Null())
				.Attribute("Copy_Citizen_FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_Patronymic", "Отчество", MetaStringType.Null())
				.Attribute("Copy_Citizen_PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())
				.Attribute("Copy_Citizen_Index", "Индекс", MetaStringType.Null())
				.Attribute("Copy_Citizen_Address", "Адрес", MetaStringType.Null())
				.Attribute("Copy_Citizen_RFSubjectText", "Субъект РФ", MetaStringType.Null())
				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())
				.Attribute("RaiseRatioSum", "Сумма после коэффициента увеличения", MetaDecimalType.Null())
				.Attribute("Copy_Citizen_SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())
				.Attribute("OtherDecisionDate", "Дата решения по иным периодам", MetaDateType.Null())
				.Attribute("OrderDate", "Дата поручения", MetaDateType.Null())
				.Attribute("OpNo", "Номер", MetaIntType.Null())
				.PersistentComputedAttribute("Title", "Номер", MetaStringType.Null())
				.Reference<WF_Activity>("Activity", "Статус")
				.Reference<C_District>("Copy_Citizen_District", "Территория")
				.Reference<DocTask>("DocTask", "Задание")
				.Reference<WorkInfo>("WorkInfo", "Сведения о государственной службе", x => x.Multiple().InverseProperty("DocTaskOperation"))
				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")
				.Reference<C_PostSalaryIndexing>("Indexing", "Индексация", x => x.Multiple())
				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")
				.Reference<C_PaymentStatus>("PaymentStatus", "Состояние выплаты")
				.Reference<C_OperationReason>("OperationReason", "Причина")
				.Reference<C_RegionalRatio>("RegionalRatio", "Районный коэффициент")
				.Reference<C_RaiseRatio>("RaiseRatio", "Коэффициент увеличения")
				.Reference<C_RestrictionRatio>("RestrictionRatio", "Коэффициент ограничения")
				.Reference<MassCalc>("MassCalc", "Результат массового перерасчета")
				.Reference<C_OperationType>("Type", "Вид операции")
				.Reference<C_RaiseRatio>("RaiseRatios", "Коэффициенты увеличения СМЗ", x => x.Multiple())
				.Reference<C_PensionType>("PensionType", "Вид трудовой пенсии")
				.Reference<C_RFSubject>("Copy_Citizen_RFSubject", "Субъект РФ")
				.OperationDelete() 
				.Operation("EditCitizen", "Редактировать данные гражданина", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editcitizen")
				)
				.Operation("EditSalary", "Редактирование надбавок", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editsalary")
				)
				.Operation("Correction", "Корректировка", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("correction")
				)
				.Operation("EditCategory", "Редактировать категорию", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editcategory")
				)
				.Operation("EditResult", "Редактировать результат", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editresult")
				)
				.Operation("EditPrecalc2", "Редактировать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editprecalc2")
				)
				.Operation("ViewListClosed", "Индивидуальные операции", x => x 
					.InvokesObjectListView("closedlist")
				)
				.Operation("Cancel", "Отменить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("cancel")
				)
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.Operation("OtherPeriods", "Иные периоды", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("Duplication", "Дублирование операций", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<WorkInfo>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("LeavingDate", "Дата увольнения", MetaDateType.Null())
				.Attribute("CalcSalary", "СМЗ (расчет)", MetaDecimalType.Null())
				.Attribute("Seniority", "Стаж", MetaIntType.Null())
				.Attribute("BeginDate", "Дата начала службы", MetaDateType.Null())
				.Attribute("IsLast", "Признак Последний", MetaBooleanType.NotNull())
				.Attribute("OrgUnitTitle", "Гос. орган (текст)", MetaStringType.Null())
				.Attribute("CalcPostTitle", "Должность (текст)", MetaStringType.Null())
				.Attribute("ExtraPayPostTitle", "Прочая должность (текст)", MetaStringType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<DocTaskOperation>("DocTaskOperation", "Операция", x => x.InverseProperty("WorkInfo"))
				.Reference<OrgUnit>("OrgUnit", "Гос. орган")
				.Reference<C_Post>("ExtraPayPost", "Прочая должность")
				.Reference<C_Post>("CalcPost", "Должность (расчет)")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<DocAsso>()
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.ReferenceKey<Doc>("ParentDoc", "Родительский документ")
				.ReferenceKey<Doc>("ChildDoc", "Дочерний документ")
			;	
			p.AddClass<Appendix>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Author", "Орган, выдавший приложение", MetaStringType.Null())
				.Attribute("CreateDate", "Дата выдачи приложения", MetaDateType.Null())
				.Attribute("File", "Электронный образ документа", MetaFileType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<DocTaskOperation>("DocTaskOperation", "Результат операции массового перерасчета")
				.Reference<Doc>("Doc", "Документ", x => x.InverseProperty("Appendix"))
				.Reference<C_DocName>("AppendixName", "Название приложения ")
				.Operation("Scan", "Сканировать", x => x 
					.InvokesSingleObjectView("scan")
				)
				.OperationView() 
				.OperationDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
			;	
			p.AddClass<DocTaskRequest>()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Ответ на обращение", MetaStringType.Null())
				.ReferenceKey<DocTask>("DocTask", "Ид")
				.Reference<C_RequestCategory>("RequestCategory", "Категория обращения")
				.Reference<C_RequestResultCategory>("RequestResultCategory", "Категория результата")
				.Reference<C_CitizenCategory>("CitizenCategory", "Категория гражданина")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.Operation("EditAnswer", "Редактировать ответ", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editanswer")
				)
			;	
			p.AddClass<CitizenPension>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата записи", MetaDateType.Null())
				.Attribute("PensionValue", "Сумма трудовой пенсии", MetaDecimalType.Null())
				.Attribute("BeginDate", "Дата начала", MetaDateType.Null())
				.Attribute("EndDate", "Дата окончания", MetaDateType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<C_PensionType>("PensionType", "Вид трудовой пенсии")
				.Reference<C_PaymentStatus>("PaymentStatus", "Состояние выплаты")
				.Reference<Citizen>("Citizen", "Гражданин", x => x.InverseProperty("CitizenPension"))
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MassCalc>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("SendingOrgUnit", "Отправка уведомлений Управлением", MetaBooleanType.NotNull())
				.Attribute("WorkBeginDate", "Расчетный период - с", MetaDateType.Null())
				.Attribute("WorkEndDate", "Расчетный период - по", MetaDateType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())
				.Attribute("Act", "Нормативно-правовой акт", MetaStringType.Null())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("StatusDate", "Дата установления текущего статуса", MetaDateTimeType.Null())
				.Attribute("DecisionDate", "Дата решения об изменении ГПО", MetaDateTimeType.Null())
				.Attribute("PensionChangeDate", "Дата, с которой изменен размер ГПО", MetaDateTimeType.Null())
				.Attribute("PostCondition", "Условие отбора государственных должностей", MetaIntType.NotNull())
				.Attribute("OrgUnitCondition", "Условия отбора органов госдарственного управления", MetaIntType.NotNull())
				.Attribute("PostPartCondition", "Условия отбора должностей федеральных служащих", MetaIntType.NotNull())
				.Attribute("PensionBeginDate", "Дата начала периода назначения ГПО", MetaDateType.Null())
				.Attribute("PensionEndDate", "Дата завершения периода назначения ГПО", MetaDateType.Null())
				.Attribute("PensionAmountFrom", "Сумма ГПО с", MetaDecimalType.Null())
				.Attribute("PensionAmountTo", "Сумма ГПО по", MetaDecimalType.Null())
				.Attribute("PostDate", "Дата установления применяемого должностного оклада", MetaDateType.Null())
				.Attribute("CompleteDate", "Дата исполнения", MetaDateTimeType.Null())
				.Attribute("OnCheckDate", "Дата начала проверки", MetaDateTimeType.Null())
				.Attribute("CloseDate", "Дата закрытия", MetaDateTimeType.Null())
				.Attribute("IsCalculated", "Рассчет выполнен", MetaBooleanType.NotNull())
				.Attribute("RFSubjectCondition", "Условие отбора субъектов РФ", MetaIntType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.PersistentComputedAttribute("Status", "Статус", MetaStringType.Null())
				.Reference<C_SpecialNote>("SpecialNote", "Особые отметки", x => x.Multiple())
				.Reference<C_PensionerCategory>("PensionerCategory", "Категория получателей ГПО")
				.Reference<C_Post>("Posts", "Государственные должности", x => x.Multiple())
				.Reference<OrgUnit>("OrgUnits", "Органы государственного управления", x => x.Multiple())
				.Reference<C_PostPart>("PostParts", "Должности федеральных служащих", x => x.Multiple())
				.Reference<Employee>("Employee", "Исполнитель")
				.Reference<C_PostSalaryIndexing>("Indexing", "Коэффициент индексации")
				.Reference<C_RaiseRatio>("RaiseRatio", "Коэффициент увеличения СМЗ")
				.Reference<C_RestrictionRatio>("RestrictionRatio", "Коэффициент ограничения СМЗ")
				.Reference<C_PaymentStatus>("PaymentStatuses", "Состояния ГПО", x => x.Multiple())
				.Reference<Citizen>("Citizens", "Граждане для перерасчета", x => x.Multiple())
				.Reference<C_RFSubject>("RFSubjects", "Субъекты РФ", x => x.Multiple())
				.Operation("UnDecision", "Выгрузить решения", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unloaddecision")
				)
				.Operation("UnOrder", "Выгрузить поручения ПД", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unloadorder")
				)
				.Operation("UnNotice", "Выгрузить уведомления", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unloadnotice")
				)
				.Operation("Select", "Отобрать ПД", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("select")
				)
				.Operation("UnOrderOU", "Выгрузить поручения ОПФР", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unloadordersou")
				)
				.OperationDelete() 
				.OperationView() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.Operation("AddItem", "Добавить элемент", x => x 
					.ParmInt("id")
					.ParmInt("citizenid")
					.ParmString("returnurl")
				)
				.Operation("DeleteItem", "Удалить элемент", x => x 
					.ParmInt("id")
					.ParmInt("citizenid")
					.ParmString("returnurl")
				)
				.Operation("Complete", "Исполнить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("Close", "Проверить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("UnApprove", "На доработку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("CreateDocs", "Создать документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("CreateUnload", "Создать выгрузку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("DeleteDocs", "Удалить сводные документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("Calc", "Рассчитать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("UnCalc", "Отменить рассчет", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("ExportExcel", "Экспорт в Эксель", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("CreateDocsInd", "Создать индивидуальные документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("CreateUnloadInd", "Создать выгрузку индивидуальных", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("DeleteDocsInd", "Удалить индивидуальные документы", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<ControlTask>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("CalcAmount", "Рассчитанный размер", MetaDecimalType.Null())
				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())
				.Reference<Citizen>("Citizen", "Гражданин")
				.Reference<MassCalc>("MassCalc", "Результат массового перерасчета")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<Complaint>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Содержание жалобы", MetaStringType.NotNull())
				.Attribute("ComplaintDate", "Дата подачи", MetaDateType.Null())
				.Attribute("ExternalFileNo", "Номер дела в судебном органе", MetaStringType.Null())
				.Attribute("DecisionDate", "Дата судебного решения", MetaDateType.Null())
				.Attribute("DecisionText", "Содержание судебного решения", MetaStringType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())
				.Reference<C_JuridicalInstance>("Instance", "Инстанция")
				.Reference<C_RequestCategory>("Category", "Категория жалобы")
				.Reference<CourtSession>("Sessions", "Судебные заседания", x => x.Multiple().InverseProperty("Complaint"))
				.Reference<JuridicalCase>("JuridicalCase", "Судебное дело")
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("EditResult", "Редактирование результата", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editresult")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
			;	
			p.AddClass<CourtSession>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("SessionDate", "Дата", MetaDateType.Null())
				.Attribute("FIO", "Ф.И.О. контактного лица", MetaStringType.Null())
				.Attribute("Post", "Должность контактного лица", MetaStringType.Null())
				.Attribute("Phone", "Телефон", MetaStringType.Null())
				.Reference<Complaint>("Complaint", "Жалоба", x => x.InverseProperty("Sessions"))
				.Reference<C_CourtSessionStatus>("Status", "Статус")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<DocTaskComplaint>()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.ReferenceKey<DocTask>("DocTask", "Ид")
				.Reference<Complaint>("Complaint", "Жалоба")
			;	
			p.AddClass<OutDocSend>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("NoUnload", "Номер выгрузки", MetaIntType.Null())
				.Attribute("NoCalc", "Номер перерасчета", MetaIntType.NotNull())
				.Attribute("SigningDate", "Дата выгрузки", MetaDateType.Null())
				.Attribute("SignedDate", "Дата подписания", MetaDateType.Null())
				.Attribute("Title", "Номер пакета", MetaStringType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("SendDate", "Дата отправки пакета", MetaDateType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("SendType", "Тип отправки", MetaIntType.NotNull())
				.PersistentComputedAttribute("UnloadTitle", "Наименование выгрузки", MetaStringType.Null())
				.Reference<OrgUnitResponsible>("Responsible", "Ответственный в ОПФР")
				.Reference<WF_Activity>("Activity", "Статус")
				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")
				.Reference<C_District>("District", "Территория")
				.Reference<OrgUnit>("Recipient", "Отделение ПФР")
				.Reference<Employee>("Employee", "Исполнитель")
				.Reference<Doc>("Doc", "Документы", x => x.Multiple())
				.Reference<C_DocType>("DocType", "Вид документов")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("PrintWord", "Печать списка", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("doclistword")
				)
				.Operation("ViewEl", "Просмотр пакета (эл.)", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("viewel")
				)
				.Operation("EditEl", "Редактировать пакет (эл.)", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editel")
				)
				.Operation("ViewUn", "Просмотр выгрузки (эл.)", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("viewunel")
				)
				.Operation("PrintWordEl", "Печать выгрузки", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("unloadword")
				)
				.Operation("DeleteUn", "Удалить выгрузку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("deleteunel")
				)
				.Operation("EditUn", "Редактировать выгрузку (эл.)", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("editunel")
				)
				.OperationCreateNew() 
				.Operation("ViewListEl", "Реестр отправок в электронном виде") 
				.Operation("ViewListP", "Реестр отправок в бумажном виде") 
				.Operation("DeleteDoc", "Удалить документ", x => x 
					.ParmInt("id")
					.ParmInt("docid")
					.ParmString("returnurl")
				)
				.Operation("CheckPack", "Проверить пакет", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("BackPack", "Вернуть на доработку", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
				.Operation("AddDocsEl", "Добавить в пакет", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<OfficeNote>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("NoteNo", "Номер записки", MetaIntType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())
				.Attribute("SignDate", "Дата подписания", MetaDateType.Null())
				.Attribute("OperationType", "Тип операции", MetaIntType.Null())
				.Attribute("AddresseeFIO", "Ф.И.О. адресата", MetaStringType.Null())
				.Attribute("AddresseePost", "Должность адресата", MetaStringType.Null())
				.Attribute("SignerFIO", "Ф.И.О. подписанта", MetaStringType.Null())
				.Attribute("SignerPost", "Должность подписанта", MetaStringType.Null())
				.PersistentComputedAttribute("Title", "Номер служебной записки", MetaStringType.Null())
				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")
				.Reference<Doc>("Docs", "Документы", x => x.Multiple())
				.OperationView() 
				.Operation("Sign", "Подписать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("sign")
				)
				.Operation("PrintWord", "Печать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("printword")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.Operation("DeleteDoc", "Удалить документ", x => x 
					.ParmInt("id")
					.ParmInt("docid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<MassOperationQueue>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("AbortDate", "Дата прерывания операции", MetaDateTimeType.Null())
				.Attribute("CancelDate", "Дата отмены операции", MetaDateTimeType.Null())
				.Attribute("Creator", "Сотрудник", MetaGuidType.Null())
				.Attribute("StartAbortDate", "Дата начала прерывания", MetaDateTimeType.Null())
				.Attribute("StartCancelDate", "Дата начала отмены", MetaDateTimeType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("MassCalcID", "Ид массового перерасчета", MetaIntType.NotNull())
				.Attribute("TotalCount", "Кол-во карточек для операции", MetaIntType.NotNull())
				.Attribute("ProcessedCount", "Кол-во обработанных карточек", MetaIntType.NotNull())
				.Attribute("InitDate", "Дата инициации операции", MetaDateTimeType.NotNull())
				.Attribute("StartDate", "Дата начала операции", MetaDateTimeType.Null())
				.Attribute("FinishDate", "Дата завершения операции", MetaDateTimeType.Null())
				.Attribute("ChangeDate", "Дата последнего изменения операции", MetaDateTimeType.Null())
				.Reference<C_MassOperationType>("Operation", "Операция")
				.Reference<C_MassOperationState>("State", "Состояние операции")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<InfoDoc>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("CreateDate", "Дата выпуска", MetaDateType.Null())
				.Attribute("Author", "Источник", MetaStringType.Null())
				.Attribute("BeginDate", "Дата начала действия", MetaDateType.Null())
				.Attribute("EndDate", "Дата прекращения действия", MetaDateType.Null())
				.Attribute("File", "Электронный образ документа", MetaFileType.Null())
				.Attribute("Comment", "Комментарий", MetaStringType.Null())
				.Attribute("PublishDate", "Дата размещения", MetaDateType.Null())
				.Attribute("AccessCondition", "Доступ к документу", MetaIntType.NotNull())
				.Reference<C_InfoDocType>("InfoDocType", "Тип документа")
				.Reference<Employee>("Employee", "Сотрудник, разместивший документ")
				.Reference<Employee>("Access", "Перечень пользователей для доступа к документу", x => x.Multiple())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList(x => x 
					.ParmInt("typeid")
				)
				.OperationUnDelete() 
				.OperationView() 
			;	
			return p;
		}
	}
	public partial class C_RFSubject { }
	public partial class C_CitizenCategory { }
	public partial class C_Post { }
	public partial class C_ExtraPayPost { }
	public partial class C_RegionalRatio { }
	public partial class C_OperationReason { }
	public partial class C_DocName { }
	public partial class C_RegLog { }
	public partial class C_DocType { }
	public partial class C_ProvisionMode { }
	public partial class C_RequestCategory { }
	public partial class C_RequestResultCategory { }
	public partial class C_DocClass { }
	public partial class C_PostSalary { }
	public partial class C_PostType { }
	public partial class C_OperationType { }
	public partial class C_SenderCategory { }
	public partial class C_PensionType { }
	public partial class C_PostSalaryIndexing { }
	public partial class C_PensionerCategory { }
	public partial class C_PaymentStatus { }
	public partial class C_RestrictionRatio { }
	public partial class C_RaiseRatio { }
	public partial class C_RequestMethod { }
	public partial class C_Seniority { }
	public partial class C_PostPart { }
	public partial class C_MassCalcStatus { }
	public partial class C_DocTaskType { }
	public partial class C_Sign { }
	public partial class C_PaymentType { }
	public partial class C_JuridicalInstance { }
	public partial class C_CourtSessionStatus { }
	public partial class C_MassOperationType { }
	public partial class C_MassOperationState { }
	public partial class C_InfoDocType { }
	public partial class C_SpecialNote { }
	public partial class C_District { }

	public class DictPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{


			p.AddClass<C_RFSubject>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Code", "Код", MetaStringType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())
				.Attribute("Capital", "Административный центр", MetaStringType.Null())
				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())
				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.NotNull())
				.Attribute("MasterObjectGUID", "Идентификатор версионного объекта", MetaGuidType.NotNull())
				.Attribute("TitleDative", "Наименование (дательный падеж)", MetaStringType.Null())
				.Attribute("FDCode", "Код ферерального округа", MetaIntType.Null())
				.Attribute("OKATO", "OKATO", MetaStringType.Null())
				.Attribute("TitleLocative", "Наименование (предложный падеж)", MetaStringType.Null())
				.Attribute("TitleGenitive", "Наименование (родительный падеж)", MetaStringType.Null())
				.ComputedAttribute("DisplayTitle", "Отображаемое имя", MetaStringType.Null())
				.PersistentComputedAttribute("IsDeleted", "Удален", MetaBooleanType.Null())
				.Reference<C_District>("Districts", "Территории", x => x.Multiple().InverseProperty("RFSubject"))
				.Reference<N_TimeZone>("TimeZone", "Часовой пояс")
				.OperationList() 
				.OperationUnDelete() 
				.Operation("Versions", "Журнал версий", x => x 
					.InvokesSingleObjectView("versions")
				)
				.OperationView() 
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationEdit() 
				.OperationCreateNew() 
			;	
			p.AddClass<C_CitizenCategory>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_Post>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Category", "Категория", MetaStringType.Null())
				.Attribute("Group", "Группа", MetaStringType.Null())
				.Attribute("Code", "Код", MetaStringType.Null())
				.Attribute("Part", "Раздел", MetaIntType.Null())
				.Attribute("Subpart", "Подраздел", MetaIntType.Null())
				.Attribute("Chapter", "Глава", MetaIntType.Null())
				.Attribute("BeginDate", "Дата учреждения", MetaDateType.Null())
				.Attribute("EndDate", "Дата упразднения", MetaDateType.Null())
				.Attribute("PTK", "Код ПТК", MetaIntType.Null())
				.Attribute("TitleGenetive", "Наименование (родительный падеж)", MetaStringType.Null())
				.Reference<C_PostSalary>("Salary", "Размер должностного оклада", x => x.Multiple().InverseProperty("Post"))
				.Reference<C_PostType>("Type", "Тип Государственная/Федеральная")
				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")
				.Reference<C_PostPart>("PostPart", "Раздел")
				.Reference<C_Post>("ParentPost", "Должность для расчета доплаты")
				.Reference<OrgUnit>("OrgUnit", "Государственный, законодательный или международный орган")
				.Reference<C_PostType>("ParentType", "Тип должности - соответствия")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("Edit3", "Редактировать прочие", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("edit3")
				)
				.Operation("Clean", "Очистить", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("clean")
				)
				.Operation("Calc", "Индексировать", x => x 
					.ParmInt("id")
					.ParmString("returnurl")
					.InvokesSingleObjectView("indexing")
				)
				.Operation("ViewList3", "Прочие должности") 
				.Operation("ViewList2", "Должности федеральных служащих") 
				.Operation("ViewList4", "Воинские должности") 
				.OperationCreateNew(x => x 
					.ParmInt("typeid")
					.ParmString("returnurl")
				)
				.Operation("ViewList1", "Государственные должности") 
			;	
			p.AddClass<C_ExtraPayPost>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("BeginDate", "Дата учреждения", MetaDateType.Null())
				.Attribute("EndDate", "Дата упразднения", MetaDateType.Null())
				.Reference<OrgUnit>("OrgUnit", "Государственный, законодательный или международный орган")
				.Reference<C_Post>("Post", "Должность для расчета доплаты")
				.Reference<C_PostType>("PostType", "Тип должности")
				.Reference<C_PostPart>("PostPart", "Раздел")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RegionalRatio>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("ATE", "Административно-территориальные единицы", MetaStringType.Null())
				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())
				.Attribute("Act", "НПА", MetaStringType.Null())
				.Attribute("PTK", "ПТК", MetaIntType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())
				.Reference<C_RFSubject>("RFSubject", "Субьект РФ")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_OperationReason>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование основания", MetaStringType.NotNull())
				.Attribute("PTK", "ПТК", MetaStringType.Null())
				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())
				.Attribute("Act", "Акт", MetaBooleanType.NotNull())
				.Attribute("Claim", "Личное заявление", MetaBooleanType.NotNull())
				.Attribute("DecisionType", "Вид решения", MetaStringType.Null())
				.Reference<C_OperationType>("Type", "Наименование операции")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_DocName>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("TitleGenitive", "Наименование (родительный падеж)", MetaStringType.Null())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RegLog>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("SpecialWork", "Специальное делопроизводство", MetaBooleanType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("FormatClass", "Формат номера документа", MetaStringType.NotNull())
				.Attribute("CurrentNo", "Текущий номер документа", MetaIntType.NotNull())
				.Attribute("Code", "Код", MetaIntType.Null())
				.Attribute("Prefix", "Префикс", MetaStringType.Null())
				.Attribute("Postfix", "Постфикс", MetaStringType.Null())
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
			;	
			p.AddClass<C_DocType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("SenderNeeded", "Отправитель", MetaBooleanType.NotNull())
				.Attribute("FederalNeeded", "Решение ФО", MetaBooleanType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("PlanExecutionPeriod", "Плановый срок выполнения", MetaIntType.Null())
				.Attribute("SignNeeded", "Требуется вторая подпись", MetaBooleanType.NotNull())
				.Attribute("SendNeeded", "Включается в пакет отправки", MetaBooleanType.NotNull())
				.Attribute("SendType", "Вид отправки", MetaIntType.NotNull())
				.Attribute("CheckPeriod", "Срок проверки", MetaIntType.Null())
				.Attribute("CompleteWarning", "Предупреждение о сроке выполнения ", MetaIntType.NotNull())
				.Attribute("FormWarning", "Предупреждение о сроке оформления ", MetaIntType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<C_DocName>("DocName", "Наименование документа")
				.Reference<C_DocClass>("DocClass", "Тип")
				.Reference<C_RegLog>("RegLog", "Журнал регистрации документов")
				.Reference<C_DocName>("Appendix", "Приложения", x => x.Multiple())
				.Reference<C_SenderCategory>("SenderCategory", "Категория отправителя")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_ProvisionMode>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RequestCategory>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RequestResultCategory>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.OperationDelete() 
			;	
			p.AddClass<C_DocClass>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PostSalary>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("CreateDate", "Дата установления", MetaDateType.NotNull())
				.Attribute("PublishDate", "Дата ввода в действие", MetaDateType.NotNull())
				.Attribute("Ratio", "Коэффициент индексации", MetaDecimalType.NotNull())
				.Attribute("Amount", "Сумма", MetaDecimalType.NotNull())
				.Attribute("EndDate", "Дата прекращения действия", MetaDateType.Null())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<C_Post>("Post", "Должность", x => x.InverseProperty("Salary"))
				.Reference<C_PostSalaryIndexing>("Indexing", "Является результатом индексации")
				.Reference<C_Post>("ParentPost", "Соответствующая должность")
				.Reference<C_PostPart>("PostPart", "Раздел")
				.OperationDelete() 
				.OperationEdit() 
				.OperationUnDelete() 
				.OperationCreateNew(x => x 
					.ParmInt("parentid")
					.ParmString("returnurl")
				)
			;	
			p.AddClass<C_PostType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationEdit(x => x 
					.InvokesSingleObjectView("undelete")
				)
				.OperationUnDelete() 
			;	
			p.AddClass<C_OperationType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("TitlePrepositional", "Наименование операции (предложный падеж)", MetaStringType.Null())
				.Attribute("Action", "Действие операции", MetaStringType.Null())
				.Attribute("Result", "Результат", MetaStringType.Null())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_SenderCategory>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PensionType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PostSalaryIndexing>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Нормативно-правовой акт", MetaStringType.NotNull())
				.Attribute("ActDate", "Дата НПА", MetaDateType.Null())
				.Attribute("IndexingDate", "Дата индексации", MetaDateType.Null())
				.Attribute("Value", "Коэффициент", MetaDecimalType.Null())
				.Attribute("Condition", "Условие применения", MetaIntType.NotNull())
				.Attribute("IsCalcComplete", "Применение выполнено", MetaBooleanType.NotNull())
				.Attribute("PTK", "ПТК", MetaStringType.Null())
				.Attribute("TitleAblative", "НПА (творительный падеж)", MetaStringType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())
				.Reference<C_PostType>("PostType", "Тип должности")
				.Reference<C_Post>("Posts", "Индексируемые должности", x => x.Multiple())
				.Reference<C_PostPart>("PostParts", "Индексируемые разделы", x => x.Multiple())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PensionerCategory>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("ReasonChange", "Основание для изменения", MetaStringType.Null())
				.Attribute("ExceptFull", "Исключение полное", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("LegalAct", "Нормативно-правовой акт", MetaStringType.Null())
				.Attribute("Amounts", "Учитываемые суммы", MetaStringType.Null())
				.Attribute("Exception", "Исключение", MetaStringType.Null())
				.Attribute("SignPost", "Подпись - должность", MetaStringType.Null())
				.Attribute("SignFio", "Подпись - ФИО", MetaStringType.Null())
				.Attribute("ReceiverStatus", "Статус получателя", MetaStringType.Null())
				.Attribute("AddInfo", "Дополнение", MetaStringType.Null())
				.Attribute("AmountsInstrumental", "Учитываемые суммы (творительный падеж)", MetaStringType.Null())
				.Reference<C_PostType>("PostType", "Тип должности")
				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PaymentStatus>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RestrictionRatio>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())
				.Attribute("Title", "Примечание", MetaStringType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())
				.Reference<C_PostType>("Type", "Тип получателя ГПО")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RaiseRatio>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Примечание", MetaStringType.Null())
				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("EndDate", "Дата окончания действия", MetaDateType.Null())
				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())
				.Reference<C_PostType>("Type", "Тип получателя ГПО")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_RequestMethod>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_Seniority>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование организации", MetaStringType.NotNull())
				.Attribute("Reason", "Основание", MetaStringType.Null())
				.Attribute("DocNo", "Номер документа", MetaStringType.Null())
				.Attribute("IsCountedToward", "Признак стаж засчитывается/не засчитывается", MetaBooleanType.NotNull())
				.Attribute("Comment", "Примечание", MetaStringType.Null())
				.Attribute("OrgUnitTitle", "Наименование органа ГУ", MetaStringType.Null())
				.Attribute("DocDate", "Дата документа", MetaDateType.Null())
				.Attribute("BeginDate", "Дата начала действия", MetaDateType.Null())
				.Attribute("EndDate", "Дата окончания действия", MetaDateType.Null())
				.Attribute("CreateDate", "Дата создания записи", MetaDateType.Null())
				.Attribute("Folder", "Папка", MetaStringType.Null())
				.Reference<Employee>("Employee", "Исполнитель")
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PostPart>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("Part", "Раздел", MetaStringType.NotNull())
				.Attribute("Subpart", "Подраздел", MetaStringType.NotNull())
				.Attribute("Chapter", "Глава", MetaStringType.NotNull())
				.Attribute("Description", "Описание", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("ShortDescription", "Краткое описание", MetaStringType.Null())
				.PersistentComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.PersistentComputedAttribute("ShortTitle", "Краткое наименование", MetaStringType.Null())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_MassCalcStatus>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_DocTaskType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_Sign>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("TitleDative", "Ф.И.О. (дательный падеж)", MetaStringType.Null())
				.Attribute("PostDative", "Должность (дательный падеж)", MetaStringType.Null())
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Ф.И.О.", MetaStringType.NotNull())
				.Attribute("Post", "Должность", MetaStringType.Null())
				.Attribute("Sign1PostType1", "1 подпись для типа Госдолжность", MetaBooleanType.NotNull())
				.Attribute("Sign2PostType1", "2 подпись для типа Госдолжность", MetaBooleanType.NotNull())
				.Attribute("Sign1PostType2", "1 подпись для типа Федеральные служащие", MetaBooleanType.NotNull())
				.Attribute("Sign2PostType2", "2 подпись для типа Федеральные служащие", MetaBooleanType.NotNull())
				.Attribute("SignRequest", "Подпись для ответов на обращения", MetaBooleanType.NotNull())
				.Attribute("SignComplaint", "Подпись для ответов в СО", MetaBooleanType.NotNull())
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_PaymentType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("TitleGenitive", "Наименование (родительный падеж)", MetaStringType.Null())
				.Attribute("TitleAccusative", "Наименование (винительный падеж)", MetaStringType.Null())
				.Attribute("TitlePrepositional", "Наименование (предложный падеж)", MetaStringType.Null())
				.Attribute("TitleAblative", "Наименование (творительный падеж)", MetaStringType.Null())
				.Attribute("ShortTitleGenitive", "Краткое наименование (родительный падеж)", MetaStringType.Null())
				.Attribute("ShortTitleAblative", "Краткое наименование (творительный падеж)", MetaStringType.Null())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_JuridicalInstance>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_CourtSessionStatus>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
			;	
			p.AddClass<C_MassOperationType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_MassOperationState>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_InfoDocType>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_SpecialNote>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<C_District>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Наименование", MetaStringType.NotNull())
				.Attribute("Code", "Код", MetaStringType.Null())
				.Reference<C_RFSubject>("RFSubject", "Субъект РФ", x => x.InverseProperty("Districts"))
				.OperationDelete() 
				.OperationEdit(x => x 
					.InvokesSingleObjectView("undelete")
				)
				.OperationUnDelete() 
			;	
			return p;
		}
	}

	public class ReportsPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("gpo_ReportRecipients", "Отчет по получателям ГПО", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "gpo_ReportRecipients")
			);
			p.Operation("gpo_ReportImplementation", "Отчет по осуществлению ГПО", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "gpo_ReportImplementation")
			);
			p.Operation("ReportDocTaskRequests", "Отчет по работе с обращениями", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "ReportDocTaskRequests")
			);
			p.Operation("ReportComplaints", "Отчет по работе с жалобами", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "ReportComplaints")
			);
			p.Operation("ReportInDoc", "Отчет по входящим документам", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "ReportInDoc")
			);
			p.Operation("ReportOutDoc", "Отчет по исходящим документам", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "ReportOutDoc")
			);

			return p;
		}
	}
	public partial class C_Scanner { }
	public partial class ScanUserSettings { }

	public class NetScanPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{
			p.AddEnum("NetScanFileFormat", "Формат файла")
				.Value("J", "Jpeg", "JPEG")
				.Value("P", "Pdf", "PDF")
				.Value("N", "Png", "PNG")
				.Value("T", "Tiff", "TIFF")
				.Value("G", "Gif", "GIF");

			p.AddEnum("NetScanPaperFormat", "Формат бумаги")
				.Value("L", "Letter", "Letter")
				.Value("G", "Legal", "Legal")
				.Value("A", "A4", "A4");

			p.AddEnum("NetScanResolution", "Разрешение")
				.Value("3", "Dpi300", "300x300 dpi")
				.Value("6", "Dpi600", "600x600 dpi")
				.Value("1", "Dpi150", "150x150 dpi");



			p.AddClass<C_Scanner>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("Title", "Имя сканера", MetaStringType.NotNull())
				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())
				.Attribute("Model", "Марка/модель сканера", MetaStringType.Null())
				.Attribute("Location", "Расположение сканера", MetaStringType.Null())
				.Attribute("IsAvailable", "Доступен", MetaBooleanType.NotNull())
				.Attribute("FileFormat", "Формат файла", MetaEnumType.NotNull("NetScanFileFormat"))
				.Attribute("IsColoredScan", "Цветное сканирование", MetaBooleanType.NotNull())
				.Attribute("PaperFormat", "Формат бумаги", MetaEnumType.NotNull("NetScanPaperFormat"))
				.Attribute("Resolution", "Разрешение", MetaEnumType.NotNull("NetScanResolution"))
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			p.AddClass<ScanUserSettings>()
				.IntKey()
				.TimeStamp<SPM_Subject>()
				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())
				.Attribute("FileFormat", "Формат файла", MetaEnumType.NotNull("NetScanFileFormat"))
				.Attribute("PaperFormat", "Формат бумаги", MetaEnumType.NotNull("NetScanPaperFormat"))
				.Attribute("Resolution", "Разрешение", MetaEnumType.NotNull("NetScanResolution"))
				.Attribute("IsColoredScan", "Цветное сканирование", MetaBooleanType.NotNull())
				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())
				.Reference<SPM_Subject>("Subject", "Пользователь")
				.Reference<C_Scanner>("Scanner", "Сканер")
				.Operation("ObjectChangeHistory", "История изменений", x => x 
					.InvokesSingleObjectView("changehistory")
				)
				.OperationDelete() 
				.OperationCreateNew() 
				.OperationEdit() 
				.OperationList() 
				.OperationUnDelete() 
				.OperationView() 
			;	
			return p;
		}
	}

	public class AssemblyGenPackage
	{
		public static MetaPackage Init(MetaPackage p)
		{

			p.Operation("templategen", "Генерация по шаблону", x => x 
					.InvokesView("Nephrite.Web.ViewControl", "templategen")
			);

			return p;
		}
	}
}

