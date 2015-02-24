



























using System;
using System.Linq;
using Nephrite.Meta;
using Nephrite.Meta.Fluent;

namespace Solution.Model
{
	// 

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

	public partial class SiteViews { }

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

	public partial class MMS_ClassStereotype { }

	public partial class MMS_Versioning { }

	public partial class MMS_ChangeLog { }

	public partial class MMS_Replication { }

	public partial class N_SqlStatementLog { }

	public partial class N_VirusScanLog { }

	public partial class N_Folder { }

	public partial class V_N_FolderFile { }

	public partial class N_File { }

	public partial class N_FileLibraryType { }

	public partial class N_FileLibrary { }

	public partial class N_DownloadLog { }

	public partial class N_ObjectChange { }

	public partial class N_ObjectPropertyChange { }

	public partial class N_Navig { }

	public partial class N_NavigItem { }

	public partial class N_Node { }

	public partial class CMSFormView { }

	public partial class WF_Workflow { }

	public partial class WF_Activity { }

	public partial class WF_Transition { }

	public partial class MasterPages { }

	public partial class Utils { }

	public partial class ErrorLog { }

	public partial class DbFolder { }

	public partial class DbFile { }

	public partial class DbItem { }

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

	public partial class AssemblyGen { }



	public partial class N_TaskType { }

	public partial class N_Task { }

	public partial class OrgUnit { }

	public partial class C_OrgUnitHie { }

	public partial class OrgUnitAsso { }

	public partial class C_OrgUnitType { }

	public partial class OrgUnitHieRule { }

	public partial class Employee { }

	public partial class OrgUnitResponsible { }

	public partial class CivilServants { }

	public partial class Doc { }

	public partial class DocAsso { }

	public partial class Appendix { }

	public partial class Citizen { }

	public partial class DocTask { }

	public partial class PensionFile { }

	public partial class JuridicalCase { }

	public partial class CitizenRequest { }

	public partial class DocTaskOperation { }

	public partial class WorkInfo { }

	public partial class DocTaskRequest { }

	public partial class CitizenPension { }

	public partial class MassCalc { }

	public partial class ControlTask { }

	public partial class InfoDoc { }

	public partial class Complaint { }

	public partial class CourtSession { }

	public partial class DocTaskComplaint { }

	public partial class OutDocSend { }

	public partial class OfficeNote { }

	public partial class MassOperationQueue { }

	public partial class ForeCast { }

	public partial class PensionFileCheckout { }

	public partial class ForeCastGpo { }

	public partial class ForeCastPension { }

	public partial class ForeCastResult { }

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

	public partial class C_PaymentType { }

	public partial class C_RestrictionRatio { }

	public partial class C_RaiseRatio { }

	public partial class C_RequestMethod { }

	public partial class C_Seniority { }

	public partial class C_PostPart { }

	public partial class C_MassCalcStatus { }

	public partial class C_DocTaskType { }

	public partial class C_Sign { }

	public partial class C_InfoDocType { }

	public partial class C_JuridicalInstance { }

	public partial class C_CourtSessionStatus { }

	public partial class C_MassOperationType { }

	public partial class C_MassOperationState { }

	public partial class C_SpecialNote { }

	public partial class C_District { }

	public partial class C_DocSending { }

	public partial class C_RequestResultManagement { }

	public partial class C_PensionFileCategory { }

	public partial class C_ComplaintResult { }

	public partial class Reports { }

	public partial class C_Scanner { }

	public partial class ScanUserSettings { }


	public class ApplicationDomain
	{
		public static MetaSolution Init(MetaSolution s)
		{

			s.AddEnum("TemplateType", "Тип представления")

				.Value("A", "Ascx", "Ascx")


				.Value("S", "SiteView", "Представление сайта")


				.Value("P", "Aspx", "Aspx")


				.Value("M", "Asmx", "Asmx")


				.Value("H", "Ashx", "Ashx")


				.Value("C", "Svc", "Svc");




			s.AddEnum("ViewDataBound", "Мощность представления")

				.Value("0", "None", "Без объекта")


				.Value("1", "Single", "Один объект")


				.Value("*", "Collection", "Коллекция объектов");




			s.AddEnum("NetScanFileFormat", "Формат файла")

				.Value("J", "Jpeg", "JPEG")


				.Value("P", "Pdf", "PDF")


				.Value("N", "Png", "PNG")


				.Value("T", "Tiff", "TIFF")


				.Value("G", "Gif", "GIF");




			s.AddEnum("NetScanPaperFormat", "Формат бумаги")

				.Value("L", "Letter", "Letter")


				.Value("G", "Legal", "Legal")


				.Value("A", "A4", "A4");




			s.AddEnum("NetScanResolution", "Разрешение")

				.Value("3", "Dpi300", "300x300 dpi")


				.Value("6", "Dpi600", "600x600 dpi")


				.Value("1", "Dpi150", "150x150 dpi");








			s.AddClass<N_TaskType>("Тип задания")

				.IntKey()






				.Attribute("ArgumentClass", "Класс параметров", MetaStringType.Null())


				.Attribute("Class", "Класс", MetaStringType.NotNull())


				.Attribute("IsActive", "Признак Активный", MetaBooleanType.NotNull())


				.Attribute("Method", "Метод", MetaStringType.NotNull())


				.Attribute("ResultClass", "Класс результата", MetaStringType.Null())


				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationView() 






			;	

			s.AddClass<N_Task>("Задание")


				.GuidKey("GUID")





				.Attribute("AbortRequestDate", "Дата запроса прерывания", MetaDateTimeType.Null())


				.Attribute("Argument", "Параметры задания, сериализованные в формате JSON", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())


				.Attribute("ErrorLogID", "Ид ошибки", MetaIntType.Null())


				.Attribute("ErrorLogMessage", "Текст ошибки", MetaStringType.Null())


				.Attribute("FinishTime", "Дата завершения", MetaDateTimeType.Null())


				.Attribute("Notify", "Уведомлять по e-mail", MetaBooleanType.NotNull())


				.Attribute("PercentDone", "% прогресса выполнения задания", MetaDecimalType.NotNull())


				.Attribute("PercentDoneDate", "Дата/время обновления прогресса", MetaDateTimeType.Null())


				.Attribute("Result", "Результат выполнения задания, сериализованный в формате JSON", MetaStringType.Null())


				.Attribute("StartTime", "Дата запуска", MetaDateTimeType.Null())


				.Attribute("Status", "Статус", MetaIntType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<SPM_Subject>("Creator", "Создал", x => x.Required())



				.Reference<N_TaskType>("TaskType", "Тип задания", x => x.Required())




				.OperationDelete() 






				.OperationEdit() 






				.OperationList() 






				.OperationView() 






			;	

			s.AddClass<OrgUnit>("Организационная единица")


				.GuidKey()



				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())


				.Attribute("ChiefName", "Ф.И.О. руководителя", MetaStringType.Null())


				.Attribute("ChiefPhone", "Телефон руководителя", MetaStringType.Null())


				.Attribute("ChiefPost", "Должность руководителя", MetaStringType.Null())


				.Attribute("ContactEMail", "Адрес электронной почты контактного лица", MetaStringType.Null())


				.Attribute("ContactName", "Ф.И.О. контактного лица", MetaStringType.Null())


				.Attribute("ContactPhone", "Телефон контактного лица", MetaStringType.Null())


				.Attribute("ContactPost", "Должность контактного лица", MetaStringType.Null())


				.Attribute("EMail", "Адрес электронной почты", MetaStringType.Null())


				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.Null())


				.Attribute("ExternalID", "Ид во внешней системе", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PostAddress", "Почтовый адрес", MetaStringType.Null())


				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())


				.Attribute("SpecCalc", "Спецрасчет", MetaBooleanType.NotNull())


				.Attribute("SysName", "Системное имя", MetaStringType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull(), x => x.Multilingual())


				.Attribute("TitleGenetive", "Наименование (родительный падеж)", MetaStringType.Null())


				.Attribute("VersionNumber", "Номер версии", MetaIntType.NotNull())


				.Attribute("VipNetCode", "VipNet код", MetaStringType.Null())


				.Reference<OrgUnitAsso>("FromChildAsso", "Ассоциация дочернего элемента", x => x.Multiple().InverseProperty("ChildOrgUnit"))



				.Reference<OrgUnitAsso>("FromParentAsso", "Ассоциация родительского элемента", x => x.Multiple().InverseProperty("ParentOrgUnit"))



				.Reference<OrgUnit>("Main", "Основная версия")



				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")



				.Reference<C_PostPart>("PostParts", "Перечень должностей", x => x.Multiple())



				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")



				.Reference<C_RFSubject>("RFSubjects", "Субъекты РФ", x => x.Multiple())



				.Reference<C_OrgUnitType>("Type", "Тип", x => x.Required())




				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("typeid")

					.ParmString("returnurl")


					.InvokesSingleObjectView("edit")


				)




				.OperationEdit() 






				.OperationList() 






				.OperationDelete() 






				.OperationUnDelete() 






				.Operation("ObjectChangeHistory", "История изменений", x => x 


					.InvokesSingleObjectView("changehistory")


				)




				.OperationView() 






			;	

			s.AddClass<C_OrgUnitHie>("Иерархия орг. единиц")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())



				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationDelete() 






				.OperationUnDelete() 






			;	

			s.AddClass<OrgUnitAsso>("Связь орг. единиц")


				.GuidKey()




				.ComputedAttribute("Title", "Title", MetaStringType.Null())


				.Reference<OrgUnit>("ChildOrgUnit", "Дочерний элемент", x => x.Required().InverseProperty("FromChildAsso"))



				.Reference<C_OrgUnitHie>("OrgUnitHie", "Иерархия", x => x.Required())



				.Reference<OrgUnit>("ParentOrgUnit", "Родительский элемент", x => x.Required().InverseProperty("FromParentAsso"))




			;	

			s.AddClass<C_OrgUnitType>("Тип орг. единицы")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull(), x => x.Multilingual())



				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationDelete() 






				.OperationUnDelete() 






			;	

			s.AddClass<OrgUnitHieRule>("Правило связи орг. единиц")


				.GuidKey()




				.ComputedAttribute("Title", "Title", MetaStringType.Null())


				.Reference<C_OrgUnitType>("ChildOrgUnitType", "Дочерний элемент", x => x.Required())



				.Reference<C_OrgUnitHie>("OrgUnitHie", "Иерархия", x => x.Required())



				.Reference<C_OrgUnitType>("ParentOrgUnitType", "Родительский элемент", x => x.Required())




				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationDelete() 






				.OperationUnDelete() 






			;	

			s.AddClass<Employee>("Сотрудник")


				.GuidKey()



				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())


				.Attribute("BirthDate", "Дата рождения", MetaDateType.Null())


				.Attribute("Email", "E-mail", MetaStringType.Null())


				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.Null())


				.Attribute("ExternalID", "Внешний идентификатор", MetaStringType.Null())


				.Attribute("Firstname", "Имя", MetaStringType.NotNull(), x => x.Multilingual())


				.Attribute("HaveAccessToPC", "Наличие доступа к компьютеру", MetaBooleanType.Null())


				.Attribute("IsChief", "Руководитель подразделения", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Job", "Должность", MetaStringType.Null(), x => x.Multilingual())


				.Attribute("Mobile", "Мобильный телефон", MetaStringType.Null())


				.Attribute("Patronymic", "Отчество", MetaStringType.Null(), x => x.Multilingual())


				.Attribute("Phone", "Рабочий телефон", MetaStringType.Null())


				.Attribute("Photo", "Фото", MetaFileType.Null())


				.Attribute("Surname", "Фамилия", MetaStringType.NotNull(), x => x.Multilingual())


				.Attribute("VersionNumber", "Номер версии", MetaIntType.NotNull())

				.PersistentComputedAttribute("Title", "ФИО", MetaStringType.Null(), x => x.Multilingual())


				.Reference<Employee>("Chief", "Вышестоящий сотрудник")



				.Reference<Employee>("Main", "Основная версия")



				.Reference<OrgUnit>("OrgUnit", "Организационная единица", x => x.Required())



				.Reference<SPM_Subject>("Subject", "Пользователь")




				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationDelete() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<OrgUnitResponsible>("Ответственный в ОПФР")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала замены", MetaDateType.Null())


				.Attribute("EMail", "Адрес электронной почты", MetaStringType.Null())


				.Attribute("EndDate", "Дата окончания замены", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsReserved", "Дублирующий", MetaBooleanType.NotNull())


				.Attribute("Phone", "Телефон", MetaStringType.Null())


				.Attribute("Post", "Должность", MetaStringType.Null())


				.Attribute("StructUnit", "Структурное подразделение", MetaStringType.Null())


				.Attribute("Title", "Ф.И.О", MetaStringType.NotNull())


				.Reference<OrgUnit>("OrgUnit", "Отделение ПФР", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<CivilServants>("Госслужащие")


				.NonPersistent()





				.Operation("FileStorageService", "Сервис файлового хранилища", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "FileStorageService")


				)




				.Operation("About", "О системе", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "About")


				)




				.Operation("GenerateDB2Inserts", "Импорт данных в DB2", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "GenerateDB2Inserts")


				)




				.Operation("depersonalize", "Подготовка дампа с деперсонализацией", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "depersonalize")


				)




			;	

			s.AddClass<Doc>("Документ")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("CitizenAddress", "Адрес", MetaStringType.Null())


				.Attribute("CitizenBirthdate", "Дата рождения", MetaDateType.Null())


				.Attribute("CitizenFirstname", "Имя", MetaStringType.Null())


				.Attribute("CitizenIndex", "Индекс", MetaStringType.Null())


				.Attribute("CitizenPatronymic", "Отчество", MetaStringType.Null())


				.Attribute("CitizenSnils", "СНИЛС", MetaStringType.Null())


				.Attribute("CitizenSurname", "Фамилия", MetaStringType.Null())


				.Attribute("CitizenVIP", "VIP", MetaBooleanType.NotNull())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CompleteDate", "Дата завершения обработки документа", MetaDateType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("DecisionDocDate", "Дата решения ФО", MetaDateType.Null())


				.Attribute("DecisionDocNo", "Номер решения ФО", MetaStringType.Null())


				.Attribute("DocNo", "Порядовый номер для регномера", MetaIntType.Null())


				.Attribute("File", "Электронный образ документа", MetaFileType.Null())


				.Attribute("Guid", "Гуид", MetaGuidType.Null())


				.Attribute("IsApproved", "Проверен", MetaBooleanType.NotNull())


				.Attribute("IsCheckDeadline", "Признак контроля исполнения документа", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsIndividual", "Индивидуальный", MetaBooleanType.NotNull())


				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())


				.Attribute("IsSecond", "Повторное", MetaBooleanType.NotNull())


				.Attribute("JPSignPerson", "Подписант документа (Ф.И.О.)", MetaStringType.Null())


				.Attribute("JPSignPersonPost", "Должность подписанта", MetaStringType.Null())


				.Attribute("LastRegDate", "Дата регистрации последнего документа в цепочке", MetaDateType.Null())


				.Attribute("MilestoneDate", "Контрольный срок исполнения документа", MetaDateType.Null())


				.Attribute("OuterRegDate", "Дата регистрации", MetaDateType.Null())


				.Attribute("OuterRegNo", "Номер регистрации", MetaStringType.Null())


				.Attribute("PPAddress", "Адрес проживания", MetaStringType.Null())


				.Attribute("PPName", "Ф.И.О.", MetaStringType.Null())


				.Attribute("PPPhone", "Телефон", MetaStringType.Null())


				.Attribute("PrintNo", "Номер для печати", MetaStringType.Null())


				.Attribute("RecordStatementDate", "Дата формирования описи", MetaDateType.Null())


				.Attribute("RecordStatementNo", "Номер описи документов", MetaStringType.Null())


				.Attribute("RegDate", "Дата регистрации", MetaDateType.Null())


				.Attribute("RegNo", "Номер регистрации", MetaStringType.Null())


				.Attribute("SendDate", "Дата отправки", MetaDateType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())

				.ComputedAttribute("TitleWithSender", "Наименование с отправителем", MetaStringType.Null())

				.PersistentComputedAttribute("CitizenTitle", "Ф.И.О. гражданина", MetaStringType.Null())


				.Reference<WF_Activity>("Activity", "Activity")



				.Reference<Appendix>("Appendix", "Приложения", x => x.Multiple().InverseProperty("Doc"))



				.Reference<C_RFSubject>("CitizenRFSubject", "Субъект РФ")



				.Reference<Employee>("Creator", "Автор")



				.Reference<DocTaskOperation>("DecisionOperation", "Решен операцией")



				.Reference<C_District>("District", "Территория")



				.Reference<C_DocClass>("DocClass", "Тип документа", x => x.Required())



				.Reference<DocTask>("DocTask", "Является результатом задания", x => x.InverseProperty("ResultDocs"))



				.Reference<DocTaskOperation>("DocTaskOperation", "Является результатом операции")



				.Reference<DocTask>("DocTasks", "Задания", x => x.Multiple().InverseProperty("Doc"))



				.Reference<C_DocType>("DocType", "Вид документа", x => x.Required())



				.Reference<OrgUnit>("Federal", "Федеральный орган")



				.Reference<Employee>("InnerSignPerson", "Подписант документа")



				.Reference<MassCalc>("MassCalc", "Является результатом массового перерасчета")



				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")



				.Reference<PensionFile>("PensionFile", "Пенсионное дело")



				.Reference<C_ProvisionMode>("ProvisionMode", "Способ передачи документа")



				.Reference<Employee>("RegEmployee", "Регистратор документа")



				.Reference<OrgUnit>("Sender", "Отправитель")



				.Reference<C_SenderCategory>("SenderCategory", "Категория отправителя")




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("classid")

					.ParmString("types")

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.Operation("ViewList", "Список", x => x 

					.ParmInt("classid")

					.ParmString("types")



				)




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




				.Operation("PrintWord", "Печать приложения", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



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




				.Operation("CreateOutForRequest", "Приложить исх. к обращению", x => x 

					.ParmInt("taskid")

					.ParmString("returnurl")



				)




				.Operation("ViewList_Int", "Внутренние документы", x => x 


					.InvokesObjectListView("list_int")


				)




			;	

			s.AddClass<DocAsso>("Связь документов")




				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.ReferenceKey<Doc>("ChildDoc", "Дочерний документ", x => x.Required())



				.ReferenceKey<Doc>("ParentDoc", "Родительский документ", x => x.Required())




			;	

			s.AddClass<Appendix>("Приложение к документу")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Author", "Орган, выдавший приложение", MetaStringType.Null())


				.Attribute("CreateDate", "Дата выдачи приложения", MetaDateType.Null())


				.Attribute("File", "Электронный образ документа", MetaFileType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<C_DocName>("AppendixName", "Название приложения ", x => x.Required())



				.Reference<Doc>("Doc", "Документ", x => x.Required().InverseProperty("Appendix"))



				.Reference<DocTaskOperation>("DocTaskOperation", "Результат операции массового перерасчета")




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")


					.InvokesSingleObjectView("edit")


				)




				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("Scan", "Сканировать", x => x 


					.InvokesSingleObjectView("scan")


				)




			;	

			s.AddClass<Citizen>("Гражданин")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Address", "Адрес", MetaStringType.Null())


				.Attribute("Birthdate", "Дата рождения", MetaDateType.Null())


				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("Deathdate", "Дата смерти", MetaDateType.Null())


				.Attribute("Firstname", "Имя", MetaStringType.NotNull())


				.Attribute("FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())


				.Attribute("FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())


				.Attribute("Index", "Индекс", MetaStringType.Null())


				.Attribute("InWork", "В работе", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())


				.Attribute("IsTrackingData", "Признак учета контрольных данных", MetaBooleanType.NotNull())


				.Attribute("Passport", "Паспортные данные", MetaStringType.Null())


				.Attribute("Patronymic", "Отчество", MetaStringType.Null())


				.Attribute("PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())


				.Attribute("PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())


				.Attribute("Phone", "Домашний телефон", MetaStringType.Null())


				.Attribute("RFSubjectText", "Субъект РФ (название)", MetaStringType.Null())


				.Attribute("Sex", "Пол", MetaEnumType.NotNull(""))


				.Attribute("Snils", "СНИЛС", MetaStringType.Null())


				.Attribute("Surname", "Фамилия", MetaStringType.NotNull())


				.Attribute("SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())


				.Attribute("SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())


				.Attribute("VIP", "VIP", MetaBooleanType.NotNull())

				.PersistentComputedAttribute("FullTitle", "Ф.И.О. полное", MetaStringType.Null())

				.PersistentComputedAttribute("FullTitleWithBirth", "Ф.И.О. полное с др", MetaStringType.Null())

				.PersistentComputedAttribute("Title", "Ф.И.О.", MetaStringType.Null())


				.Reference<C_CitizenCategory>("Category", "Категория")



				.Reference<CitizenPension>("CitizenPension", "Трудовая пенсия", x => x.Multiple().InverseProperty("Citizen"))



				.Reference<C_District>("District", "Территория")



				.Reference<OrgUnit>("OrgUnit", "Отделение ПФР")



				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")



				.Reference<C_SpecialNote>("SpecialNote", "Особые отметки", x => x.Multiple())




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




				.Operation("ViewList_Compl", "С жалобами", x => x 


					.InvokesObjectListView("list_compl")


				)




				.Operation("Duplicates", "Отчет по дубликатам", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "duplicates")


				)




			;	

			s.AddClass<DocTask>("Задание")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("AnnulmentDate", "Дата аннулирования", MetaDateType.Null())


				.Attribute("AssignmentDate", "Дата назначения исполнителя", MetaDateTimeType.Null())


				.Attribute("CheckedDate", "Дата окончания проверки", MetaDateTimeType.Null())


				.Attribute("CloseDate", "Дата закрытия задания", MetaDateTimeType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CompleteDate", "Дата исполнения задания", MetaDateTimeType.Null())


				.Attribute("Copy_Citizen_Address", "Адрес", MetaStringType.Null())


				.Attribute("Copy_Citizen_Birthdate", "Дата рождения", MetaDateType.Null())


				.Attribute("Copy_Citizen_Firstname", "Имя", MetaStringType.Null())


				.Attribute("Copy_Citizen_FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_Index", "Индекс", MetaStringType.Null())


				.Attribute("Copy_Citizen_Patronymic", "Отчество", MetaStringType.Null())


				.Attribute("Copy_Citizen_PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_RFSubjectText", "Субъект РФ", MetaStringType.Null())


				.Attribute("Copy_Citizen_Surname", "Фамилия", MetaStringType.Null())


				.Attribute("Copy_Citizen_SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_VIP", "VIP", MetaBooleanType.NotNull())


				.Attribute("CreateDate", "Дата выдачи задания", MetaDateTimeType.Null())


				.Attribute("DocTaskResultID", "Результат выполнения", MetaIntType.Null())


				.Attribute("ForDepartmentsDate", "Дата перевода подразделениям ИД", MetaDateType.Null())


				.Attribute("InCaseDate", "Дата перевода в деле", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())


				.Attribute("OnCheckDate", "Дата начала проверки", MetaDateTimeType.Null())


				.Attribute("PlanCompleteDate", "Плановая дата исполнения задания", MetaDateType.Null())


				.Attribute("ReturnedDate", "Дата возврата", MetaDateTimeType.Null())


				.Attribute("SignDate", "Дата подписи", MetaDateType.Null())


				.Attribute("State", "Состояние", MetaStringType.Null())


				.Attribute("SuspendDate", "Дата откладывания", MetaDateType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())

				.PersistentComputedAttribute("Status", "Статус", MetaStringType.Null())


				.Reference<WF_Activity>("Activity", "Activity")



				.Reference<Employee>("Author", "Автор")



				.Reference<Employee>("ChiefPerformer", "Ответственный исполнитель")



				.Reference<Citizen>("Citizen", "Гражданин")



				.Reference<C_RFSubject>("Copy_Citizen_RFSubject", "Субъект РФ")



				.Reference<Doc>("Doc", "Документ - основание", x => x.InverseProperty("DocTasks"))



				.Reference<Employee>("Performer", "Исполнитель")



				.Reference<Doc>("ResultDocs", "Документы - результаты", x => x.Multiple().InverseProperty("DocTask"))



				.Reference<C_DocTaskType>("Type", "Тип")




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.Operation("ViewList", "Все задания") 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("MyViewList", "Мои задания") 






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




				.Operation("ViewListOnCheck", "Задания на проверку") 






				.Operation("ViewListExceeded", "Задания на контроле") 






				.Operation("CreateDocs", "Создать документы", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("createdocs")


				)




				.Operation("ViewListAnnulment", "Аннулированные задания") 






				.Operation("ViewListSuspended", "Отложенные задания") 






				.Operation("StatusHistory", "История изменения статуса", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("statushistory")


				)




				.Operation("ViewListSigning", "Задания на подписании") 






				.Operation("ViewListClosed", "Завершенные задания") 






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




				.Operation("DeleteDocs", "Удалить документы", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("ReturnTasks", "Возвратить на проверку", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("return")


				)




				.Operation("SignTasks", "Подписать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("signing")


				)




				.Operation("Renew", "Возобновить", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("renew")


				)




				.Operation("CreateForMassCalc", "Создать для перерасчета", x => x 

					.ParmInt("masscalcid")

					.ParmInt("citizenid")

					.ParmString("returnurl")



				)




				.Operation("Reserve", "Забронировать номер", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("signing")


				)




				.Operation("ViewListInCase", "Задания в деле") 






				.Operation("ViewListForDepartments", "Задания подразделениям ИД") 






			;	

			s.AddClass<PensionFile>("Пенсионное дело")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ArchiveDate", "Дата передачи в архив", MetaDateType.Null())


				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsLost", "Утрачено", MetaBooleanType.NotNull())


				.Attribute("RegDate", "Дата приема", MetaDateType.Null())


				.Attribute("Status", "Статус", MetaStringType.Null())


				.Attribute("Title", "Номер", MetaStringType.NotNull())

				.ComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<Employee>("ArchiveEmployee", "Сотрудник, передавший дело в архив")



				.Reference<C_PensionFileCategory>("Category", "Раздел")



				.Reference<Citizen>("Citizen", "Гражданин", x => x.Required())



				.Reference<Employee>("CloseEmployee", "Сотрудник, закрывший дело")



				.Reference<Employee>("CreateEmployee", "Сотрудник, создавший дело")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("ViewListArch", "Список - архив") 






				.Operation("UnArchieve", "Восстановить из архива", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("Archive", "Архивирование", x => x 

					.ParmString("returnurl")


					.InvokesView("Nephrite.Web.ViewControl", "archive")


				)




				.Operation("ViewListForCheckout", "Список - затребованные") 






				.Operation("ViewListCheckouted", "Список - выданные") 






				.Operation("ViewListClosed", "Список - закрытые") 






				.Operation("ViewListLost", "Список - утраченные") 






				.Operation("ViewListNew", "Список - новые") 






				.Operation("CheckOut", "Запросить", x => x 

					.ParmString("returnurl")


					.InvokesView("Nephrite.Web.ViewControl", "checkout")


				)




			;	

			s.AddClass<JuridicalCase>("Судебное дело")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ArchiveDate", "Дата передачи в архив", MetaDateType.Null())


				.Attribute("CloseDate", "Дата закрытия", MetaDateType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Номер", MetaStringType.NotNull())


				.Reference<Employee>("ArchiveEmployee", "Сотрудник, передавший дело в архив")



				.Reference<Citizen>("Citizen", "Гражданин", x => x.Required())



				.Reference<Employee>("CloseEmployee", "Сотрудник, закрывший дело")



				.Reference<Employee>("CreateEmployee", "Сотрудник, создавший дело")



				.Reference<OrgUnit>("OrgUnit", "Судебный орган", x => x.Required())




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

			s.AddClass<CitizenRequest>("Устное обращение гражданина")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Address", "Адрес", MetaStringType.Null())


				.Attribute("Birthdate", "Дата рождения", MetaDateType.Null())


				.Attribute("Description", "Краткое описание проблемы", MetaStringType.NotNull())


				.Attribute("EMail", "EMail", MetaStringType.Null())


				.Attribute("Firstname", "Имя", MetaStringType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Patronymic", "Отчество", MetaStringType.Null())


				.Attribute("RequestDate", "Дата обращения", MetaDateTimeType.NotNull())


				.Attribute("Surname", "Фамилия", MetaStringType.NotNull())


				.Attribute("Title", "Регистрационный номер", MetaStringType.NotNull())


				.Reference<Citizen>("Citizen", "Гражданин")



				.Reference<Employee>("RecorderEmployee", "Сотрудник зарегистрировавший обращение", x => x.Required())



				.Reference<C_RequestCategory>("RequestCategory", "Категория обращения", x => x.Required())



				.Reference<C_RequestMethod>("RequestMethod", "Способ обращения", x => x.Required())



				.Reference<C_RequestResultCategory>("RequestResult", "Результат обращения")



				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<DocTaskOperation>("Операция")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ActAblative", "НПА для перерасчета", MetaStringType.Null())


				.Attribute("Basis", "Основа для расчета", MetaBooleanType.NotNull())


				.Attribute("Bonus", "Премия", MetaDecimalType.Null())


				.Attribute("CalcAmount", "Рассчитанный размер", MetaDecimalType.Null())


				.Attribute("CalcSalary", "СДВ", MetaDecimalType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("Copy_Citizen_Address", "Адрес", MetaStringType.Null())


				.Attribute("Copy_Citizen_Firstname", "Имя", MetaStringType.Null())


				.Attribute("Copy_Citizen_FirstnameDative", "Имя (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_FirstnameGenitive", "Имя (родительный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_Index", "Индекс", MetaStringType.Null())


				.Attribute("Copy_Citizen_Patronymic", "Отчество", MetaStringType.Null())


				.Attribute("Copy_Citizen_PatronymicDative", "Отчество (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_PatronymicGenitive", "Отчество (родительный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_RFSubjectText", "Субъект РФ", MetaStringType.Null())


				.Attribute("Copy_Citizen_Surname", "Фамилия", MetaStringType.Null())


				.Attribute("Copy_Citizen_SurnameDative", "Фамилия (дательный падеж)", MetaStringType.Null())


				.Attribute("Copy_Citizen_SurnameGenitive", "Фамилия (родительный падеж)", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("DecisionDate", "Дата решения", MetaDateType.Null())


				.Attribute("FGAMidSalary", "СМЗ (фед)", MetaDecimalType.Null())


				.Attribute("FGAPercent", "% начисл. (фед)", MetaDecimalType.Null())


				.Attribute("FGASalary", "Должностной оклад (фед)", MetaDecimalType.Null())


				.Attribute("FGASeniority", "Стаж (фед)", MetaIntType.Null())


				.Attribute("IsApprove", "Решение", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsLast", "Признак Последняя", MetaBooleanType.NotNull())


				.Attribute("IsMigrationData", "IsMigrationData", MetaBooleanType.NotNull())


				.Attribute("IsTracking", "Признак Контрольная", MetaBooleanType.NotNull())


				.Attribute("OpNo", "Номер", MetaIntType.Null())


				.Attribute("OrderDate", "Дата поручения", MetaDateType.Null())


				.Attribute("OtherDecisionDate", "Дата решения по иным периодам", MetaDateType.Null())


				.Attribute("PaymentEndDate", "Дата окончания исполнения", MetaDateType.Null())


				.Attribute("PaymentStartDate", "Дата начала исполнения", MetaDateType.Null())


				.Attribute("RaiseRatioSum", "Сумма после коэффициента увеличения", MetaDecimalType.Null())


				.Attribute("RaiseRatioValue", "Значение коэффициента увеличения", MetaDecimalType.Null())


				.Attribute("RegionalRatioValue", "Значение регионального коэффициента", MetaDecimalType.Null())


				.Attribute("Restriction", "Верхний коэффициент ограничения", MetaDecimalType.Null())


				.Attribute("RestrictionMin", "Нижний коэффициент ограничения", MetaDecimalType.Null())


				.Attribute("SalaryDate", "Дата оклада", MetaDateType.Null())


				.Attribute("SalaryIncK", "Надбавка за квалификацию", MetaDecimalType.Null())


				.Attribute("SalaryIncL", "Надбавка за выслугу лет", MetaDecimalType.Null())


				.Attribute("SalaryIncS", "Надбавка за сложность", MetaDecimalType.Null())


				.Attribute("SalaryIncSek", "Надбавка за работу с секретными сведениями", MetaDecimalType.Null())


				.Attribute("SalaryIncU", "Надбавка за особые условия", MetaDecimalType.Null())


				.Attribute("Seniority", "Стаж", MetaIntType.Null())


				.Attribute("SumIndexing", "Суммарный коэффициент индексации", MetaDecimalType.NotNull())


				.Attribute("SumIndexingPrev", "Суммарный коэффициент индексации до даты оклада", MetaDecimalType.NotNull())

				.PersistentComputedAttribute("Title", "Номер", MetaStringType.Null())


				.Reference<WF_Activity>("Activity", "Статус")



				.Reference<C_District>("Copy_Citizen_District", "Территория")



				.Reference<C_RFSubject>("Copy_Citizen_RFSubject", "Субъект РФ")



				.Reference<DocTask>("DocTask", "Задание", x => x.Required())



				.Reference<C_PostSalaryIndexing>("Indexing", "Индексация", x => x.Multiple())



				.Reference<MassCalc>("MassCalc", "Результат массового перерасчета")



				.Reference<C_OperationReason>("OperationReason", "Причина")



				.Reference<C_PaymentStatus>("PaymentStatus", "Состояние выплаты")



				.Reference<C_PaymentType>("PaymentType", "Вид ГПО")



				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")



				.Reference<C_PensionType>("PensionType", "Вид трудовой пенсии")



				.Reference<C_RaiseRatio>("RaiseRatio", "Коэффициент увеличения")



				.Reference<C_RaiseRatio>("RaiseRatios", "Коэффициенты увеличения СМЗ", x => x.Multiple())



				.Reference<C_RegionalRatio>("RegionalRatio", "Районный коэффициент")



				.Reference<C_RestrictionRatio>("RestrictionRatio", "Коэффициент ограничения")



				.Reference<C_OperationType>("Type", "Вид операции")



				.Reference<WorkInfo>("WorkInfo", "Сведения о государственной службе", x => x.Multiple().InverseProperty("DocTaskOperation"))




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




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




				.Operation("EditCitizen", "Редактировать данные гражданина", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("editcitizen")


				)




				.Operation("OtherPeriods", "Иные периоды", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("Correction", "Корректировка", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("correction")


				)




				.Operation("Duplication", "Дублирование операций", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("EditSalary", "Редактирование надбавок", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("editsalary")


				)




				.Operation("Cancel", "Отменить", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("cancel")


				)




			;	

			s.AddClass<WorkInfo>("Сведения о государственной службе")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала службы", MetaDateType.Null())


				.Attribute("CalcPostTitle", "Должность (текст)", MetaStringType.Null())


				.Attribute("CalcSalary", "СМЗ (расчет)", MetaDecimalType.Null())


				.Attribute("ExtraPayPostTitle", "Прочая должность (текст)", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsLast", "Признак Последний", MetaBooleanType.NotNull())


				.Attribute("LeavingDate", "Дата увольнения", MetaDateType.Null())


				.Attribute("OrgUnitTitle", "Гос. орган (текст)", MetaStringType.Null())


				.Attribute("Seniority", "Стаж", MetaIntType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<C_Post>("CalcPost", "Должность (расчет)")



				.Reference<DocTaskOperation>("DocTaskOperation", "Операция", x => x.Required().InverseProperty("WorkInfo"))



				.Reference<C_Post>("ExtraPayPost", "Прочая должность")



				.Reference<OrgUnit>("OrgUnit", "Гос. орган")




				.OperationDelete() 






				.OperationEdit() 






				.OperationUnDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




			;	

			s.AddClass<DocTaskRequest>("Обращение")



				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Ответ на обращение", MetaStringType.Null())


				.Reference<C_CitizenCategory>("CitizenCategory", "Категория гражданина")



				.ReferenceKey<DocTask>("DocTask", "Ид", x => x.Required())



				.Reference<C_RequestCategory>("RequestCategory", "Категория обращения")



				.Reference<C_RequestResultCategory>("RequestResultCategory", "Категория результата")



				.Reference<C_RequestResultManagement>("ResultManagement", "Категория результата из АП")




				.OperationDelete() 






				.OperationEdit() 






				.OperationUnDelete() 






				.Operation("EditAnswer", "Редактировать ответ", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("editanswer")


				)




				.Operation("CreateDocs", "Сформировать документы", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("createdocs")


				)




			;	

			s.AddClass<CitizenPension>("Трудовая пенсия")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала", MetaDateType.Null())


				.Attribute("CreateDate", "Дата записи", MetaDateType.Null())


				.Attribute("DependantsCount", "Количество иждивенцев", MetaIntType.Null())


				.Attribute("EndDate", "Дата окончания", MetaDateType.Null())


				.Attribute("FBR", "ФБР", MetaDecimalType.Null())


				.Attribute("FBRRaise", "Увеличение ФБР", MetaDecimalType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("IsImported", "Импортировано", MetaBooleanType.NotNull())


				.Attribute("IsInvalid", "Инвалид 1 гр.", MetaBooleanType.NotNull())


				.Attribute("PensionServiceValue", "Размер пенсии за выслугу лет", MetaDecimalType.Null())


				.Attribute("PensionValue", "Размер трудовой пенсии", MetaDecimalType.Null())


				.Attribute("RegionalRatioState", "Районный коэффициент", MetaIntType.Null())


				.Attribute("SCh", "Доля СЧ", MetaDecimalType.Null())


				.Attribute("SCh2", "СЧ без ФБР, Д, В", MetaDecimalType.Null())


				.Attribute("Valorization", "Валоризация", MetaDecimalType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())

				.PersistentComputedAttribute("PensionAmount", "Сумма трудовой пенсии", MetaDecimalType.Null())

				.PersistentComputedAttribute("PensionSum", "Сумма выплаты", MetaDecimalType.Null())


				.Reference<Citizen>("Citizen", "Гражданин", x => x.Required().InverseProperty("CitizenPension"))



				.Reference<C_PaymentStatus>("PaymentServiceStatus", "Состояние выплаты ПВЛ")



				.Reference<C_PaymentStatus>("PaymentStatus", "Состояние выплаты трудовой пенсии")



				.Reference<C_PensionType>("PensionType", "Вид трудовой пенсии")




				.OperationDelete() 






				.OperationEdit() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




			;	

			s.AddClass<MassCalc>("Массовые перерасчеты")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Act", "Нормативно-правовой акт", MetaStringType.Null())


				.Attribute("CloseDate", "Дата закрытия", MetaDateTimeType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CompleteDate", "Дата исполнения", MetaDateTimeType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())


				.Attribute("DecisionDate", "Дата решения об изменении ГПО", MetaDateTimeType.Null())


				.Attribute("IsCalculated", "Рассчет выполнен", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("OnCheckDate", "Дата начала проверки", MetaDateTimeType.Null())


				.Attribute("OrgUnitCondition", "Условия отбора органов госдарственного управления", MetaIntType.NotNull())


				.Attribute("PensionAmountFrom", "Сумма ГПО с", MetaDecimalType.Null())


				.Attribute("PensionAmountTo", "Сумма ГПО по", MetaDecimalType.Null())


				.Attribute("PensionBeginDate", "Дата начала периода назначения ГПО", MetaDateType.Null())


				.Attribute("PensionChangeDate", "Дата, с которой изменен размер ГПО", MetaDateTimeType.Null())


				.Attribute("PensionEndDate", "Дата завершения периода назначения ГПО", MetaDateType.Null())


				.Attribute("PostCondition", "Условие отбора государственных должностей", MetaIntType.NotNull())


				.Attribute("PostDate", "Дата установления применяемого должностного оклада", MetaDateType.Null())


				.Attribute("PostGroup", "Группа", MetaStringType.Null())


				.Attribute("PostPartCondition", "Условия отбора должностей федеральных служащих", MetaIntType.NotNull())


				.Attribute("RFSubjectCondition", "Условие отбора субъектов РФ", MetaIntType.Null())


				.Attribute("SendingOrgUnit", "Отправка уведомлений Управлением", MetaBooleanType.NotNull())


				.Attribute("StatusDate", "Дата установления текущего статуса", MetaDateTimeType.Null())


				.Attribute("WorkBeginDate", "Расчетный период - с", MetaDateType.Null())


				.Attribute("WorkEndDate", "Расчетный период - по", MetaDateType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())

				.PersistentComputedAttribute("Status", "Статус", MetaStringType.Null())


				.Reference<Citizen>("Citizens", "Граждане для перерасчета", x => x.Multiple())



				.Reference<Employee>("Employee", "Исполнитель")



				.Reference<C_PostSalaryIndexing>("Indexing", "Коэффициент индексации")



				.Reference<OrgUnit>("OrgUnits", "Органы государственного управления", x => x.Multiple())



				.Reference<C_PaymentStatus>("PaymentStatuses", "Состояния ГПО", x => x.Multiple())



				.Reference<C_PensionerCategory>("PensionerCategory", "Категория получателей ГПО")



				.Reference<C_PostPart>("PostParts", "Должности федеральных служащих", x => x.Multiple())



				.Reference<C_Post>("Posts", "Государственные должности", x => x.Multiple())



				.Reference<C_RaiseRatio>("RaiseRatio", "Коэффициент увеличения СМЗ")



				.Reference<C_RestrictionRatio>("RestrictionRatio", "Коэффициент ограничения СМЗ")



				.Reference<C_RFSubject>("RFSubjects", "Субъекты РФ", x => x.Multiple())



				.Reference<C_SpecialNote>("SpecialNote", "Особые отметки", x => x.Multiple())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






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




				.Operation("Select", "Отобрать ПД", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("select")


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




				.Operation("UnOrderOU", "Выгрузить поручения ОПФР", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("unloadordersou")


				)




			;	

			s.AddClass<ControlTask>("Контрольная операция")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("CalcAmount", "Рассчитанный размер", MetaDecimalType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<Citizen>("Citizen", "Гражданин", x => x.Required())



				.Reference<MassCalc>("MassCalc", "Результат массового перерасчета")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<InfoDoc>("Информационный материал")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("AccessCondition", "Доступ к документу", MetaIntType.NotNull())


				.Attribute("Author", "Источник", MetaStringType.Null())


				.Attribute("BeginDate", "Дата начала действия", MetaDateType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CreateDate", "Дата выпуска", MetaDateType.Null())


				.Attribute("EndDate", "Дата прекращения действия", MetaDateType.Null())


				.Attribute("File", "Электронный образ документа", MetaFileType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PublishDate", "Дата размещения", MetaDateType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<Employee>("Access", "Перечень пользователей для доступа к документу", x => x.Multiple())



				.Reference<Employee>("Employee", "Сотрудник, разместивший документ", x => x.Required())



				.Reference<C_InfoDocType>("InfoDocType", "Тип документа", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.Operation("ViewList", "Список", x => x 

					.ParmInt("typeid")


					.InvokesObjectListView("list")


				)




				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<Complaint>("Жалоба")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ComplaintDate", "Дата подачи", MetaDateType.Null())


				.Attribute("DecisionDate", "Дата судебного решения", MetaDateType.Null())


				.Attribute("DecisionText", "Содержание судебного решения", MetaStringType.Null())


				.Attribute("ExternalFileNo", "Номер дела в судебном органе", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Содержание жалобы", MetaStringType.NotNull())

				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<C_RequestCategory>("Category", "Категория жалобы")



				.Reference<C_ComplaintResult>("ComplaintResult", "Результат рассмотрения")



				.Reference<C_JuridicalInstance>("Instance", "Инстанция")



				.Reference<JuridicalCase>("JuridicalCase", "Судебное дело", x => x.Required())



				.Reference<CourtSession>("Sessions", "Судебные заседания", x => x.Multiple().InverseProperty("Complaint"))




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("EditResult", "Редактирование результата", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("editresult")


				)




			;	

			s.AddClass<CourtSession>("Судебное заседание")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("FIO", "Ф.И.О. контактного лица", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Phone", "Телефон", MetaStringType.Null())


				.Attribute("Post", "Должность контактного лица", MetaStringType.Null())


				.Attribute("SessionDate", "Дата", MetaDateType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<Complaint>("Complaint", "Жалоба", x => x.Required().InverseProperty("Sessions"))



				.Reference<C_CourtSessionStatus>("Status", "Статус")




				.OperationDelete() 






				.OperationEdit() 






				.OperationUnDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




			;	

			s.AddClass<DocTaskComplaint>("Задание на жалобу")



				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<Complaint>("Complaint", "Жалоба")



				.ReferenceKey<DocTask>("DocTask", "Ид", x => x.Required())




			;	

			s.AddClass<OutDocSend>("Отправка")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Code", "Код отправки", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("NoCalc", "Номер перерасчета", MetaIntType.NotNull())


				.Attribute("NoUnload", "Номер выгрузки", MetaIntType.Null())


				.Attribute("SendDate", "Дата отправки пакета", MetaDateType.Null())


				.Attribute("SendType", "Тип отправки", MetaIntType.NotNull())


				.Attribute("SignedDate", "Дата подписания", MetaDateType.Null())


				.Attribute("SigningDate", "Дата выгрузки", MetaDateType.Null())


				.Attribute("Title", "Номер пакета", MetaStringType.NotNull())

				.PersistentComputedAttribute("UnloadTitle", "Наименование выгрузки", MetaStringType.Null())


				.Reference<WF_Activity>("Activity", "Статус")



				.Reference<C_District>("District", "Территория")



				.Reference<Doc>("Doc", "Документы", x => x.Multiple())



				.Reference<C_DocSending>("DocSending", "Журнал отправки")



				.Reference<C_DocType>("DocType", "Вид документов (устаревшее)")



				.Reference<Employee>("Employee", "Исполнитель", x => x.Required())



				.Reference<OrgUnit>("Recipient", "Отделение ПФР")



				.Reference<OrgUnitResponsible>("Responsible", "Ответственный в ОПФР")



				.Reference<C_RFSubject>("RFSubject", "Субъект РФ")




				.OperationDelete() 






				.Operation("CreateNew", "Создать бумажную", x => x 

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.Operation("ViewListEl", "Реестр отправок в электронном виде") 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("PrintWord", "Печать списка", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("doclistword")


				)




				.Operation("ViewListP", "Реестр отправок в бумажном виде") 






				.Operation("DeleteDoc", "Удалить документ", x => x 

					.ParmInt("id")

					.ParmInt("docid")

					.ParmString("returnurl")



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




				.Operation("CreateEl", "Создать электронную", x => x 

					.ParmString("returnurl")



				)




			;	

			s.AddClass<OfficeNote>("Служебная записка")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("AddresseeFIO", "Ф.И.О. адресата", MetaStringType.Null())


				.Attribute("AddresseePost", "Должность адресата", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("NoteNo", "Номер записки", MetaIntType.Null())


				.Attribute("OperationType", "Тип операции", MetaIntType.Null())


				.Attribute("SignDate", "Дата подписания", MetaDateType.Null())


				.Attribute("SignerFIO", "Ф.И.О. подписанта", MetaStringType.Null())


				.Attribute("SignerPost", "Должность подписанта", MetaStringType.Null())

				.PersistentComputedAttribute("Title", "Номер служебной записки", MetaStringType.Null())


				.Reference<Doc>("Docs", "Документы", x => x.Multiple())



				.Reference<C_PaymentType>("PaymentType", "Вид ГПО", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("Sign", "Подписать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("sign")


				)




				.Operation("DeleteDoc", "Удалить документ", x => x 

					.ParmInt("id")

					.ParmInt("docid")

					.ParmString("returnurl")



				)




				.Operation("PrintWord", "Печать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("printword")


				)




			;	

			s.AddClass<MassOperationQueue>("Очередь массовых операций")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("AbortDate", "Дата прерывания операции", MetaDateTimeType.Null())


				.Attribute("CancelDate", "Дата отмены операции", MetaDateTimeType.Null())


				.Attribute("ChangeDate", "Дата последнего изменения операции", MetaDateTimeType.Null())


				.Attribute("Creator", "Сотрудник", MetaGuidType.Null())


				.Attribute("FinishDate", "Дата завершения операции", MetaDateTimeType.Null())


				.Attribute("InitDate", "Дата инициации операции", MetaDateTimeType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("MassCalcID", "Ид массовой операции", MetaIntType.NotNull())


				.Attribute("OtherID", "Ид для дополнительной ссылки", MetaIntType.Null())


				.Attribute("ProcessedCount", "Кол-во обработанных карточек", MetaIntType.NotNull())


				.Attribute("StartAbortDate", "Дата начала прерывания", MetaDateTimeType.Null())


				.Attribute("StartCancelDate", "Дата начала отмены", MetaDateTimeType.Null())


				.Attribute("StartDate", "Дата начала операции", MetaDateTimeType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("TotalCount", "Кол-во карточек для операции", MetaIntType.NotNull())


				.Reference<C_MassOperationType>("Operation", "Операция", x => x.Required())



				.Reference<C_MassOperationState>("State", "Состояние операции", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<ForeCast>("Прогнозирование")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала прогноза", MetaDateType.Null())


				.Attribute("CloseDate", "Дата закрытия", MetaDateTimeType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания", MetaDateTimeType.NotNull())


				.Attribute("EndDate", "Дата завершения прогноза", MetaDateType.Null())


				.Attribute("IncreasePercent", "Увеличение ДО/ДВ (%) (устаревший)", MetaDecimalType.Null())


				.Attribute("Indexing", "Коэффициент индексации (устаревший)", MetaDecimalType.Null())


				.Attribute("IsCalculated", "Расчет выполнен (устаревшее)", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("OrgUnitCondition", "Отбор федеральных органов", MetaIntType.Null())


				.Attribute("PensionBeginDate", "Период назначения с", MetaDateType.Null())


				.Attribute("PensionEndDate", "Период назначения по", MetaDateType.Null())


				.Attribute("PostCondition", "Отбор государственных должностей", MetaIntType.Null())


				.Attribute("PostGroups", "Группы должностей", MetaStringType.Null())


				.Attribute("PostPartCondition", "Отбор должностей федеральных служащих", MetaIntType.Null())


				.Attribute("RaiseRatio", "Коэффициент увеличения СМЗ (устаревший)", MetaDecimalType.Null())


				.Attribute("RestrictionRatio", "Коэффициент ограничения СМЗ (устаревший)", MetaDecimalType.Null())


				.Attribute("RFSubjectCondition", "Отбор субъектов РФ", MetaIntType.Null())


				.Attribute("SubtractSum", "Вычитаемые суммы", MetaStringType.Null())


				.Attribute("ViewBeginDate", "Отображаемая дата начала прогноза", MetaDateType.Null())


				.Attribute("ViewEndDate", "Отображаемая дата окончания прогноза", MetaDateType.Null())


				.Attribute("WorkBeginDate", "Расчетный период с", MetaDateType.Null())


				.Attribute("WorkEndDate", "Расчетный период по", MetaDateType.Null())


				.Attribute("YearRaise", "Годовой прирост получателей ГПО", MetaIntType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())

				.PersistentComputedAttribute("Status", "Статус", MetaStringType.Null())


				.Reference<Employee>("Employee", "Исполнитель")



				.Reference<ForeCastGpo>("ForeCastGpos", "Условия для ГПО", x => x.Multiple().InverseProperty("ForeCast"))



				.Reference<ForeCastPension>("ForeCastPensions", "Условия для ТП", x => x.Multiple().InverseProperty("ForeCast"))



				.Reference<OrgUnit>("OrgUnits", "Федеральные органы", x => x.Multiple())



				.Reference<C_PaymentStatus>("PaymentStatuses", "Состояние выплаты", x => x.Multiple())



				.Reference<C_PensionerCategory>("PensionerCategory", "Категория получателей ГПО", x => x.Required())



				.Reference<C_PostPart>("PostParts", "Должности федеральных служащих", x => x.Multiple())



				.Reference<C_Post>("Posts", "Государственные должности", x => x.Multiple())



				.Reference<C_RFSubject>("RFSubjects", "Субъекты РФ", x => x.Multiple())



				.Reference<C_SpecialNote>("SpecialNotes", "Особые отметки", x => x.Multiple())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationView() 






				.Operation("Select", "Отобрать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("select")


				)




				.Operation("Calc", "Рассчитать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("UnCalc", "Отменить рассчет", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("ExportExcel", "Экспорт в эксель", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("Report", "Отчет", x => x 

					.ParmInt("id")

					.ParmString("returnurl")



				)




				.Operation("Close", "Выполнить", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("close")


				)




				.Operation("DeleteItem", "Удалить элемент", x => x 

					.ParmInt("id")

					.ParmInt("citizenid")

					.ParmString("returnurl")



				)




				.Operation("AddItem", "Добавить элемент", x => x 

					.ParmInt("id")

					.ParmInt("citizenid")

					.ParmString("returnurl")



				)




				.Operation("CreateResult", "Сформировать таблицу результата", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("result")


				)




			;	

			s.AddClass<PensionFileCheckout>("Выдача пенсионного дела")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("CheckinDate", "Дата возврата", MetaDateType.Null())


				.Attribute("CheckoutDate", "Дата выдачи", MetaDateType.Null())


				.Attribute("Comment", "Комментарий", MetaStringType.Null())


				.Attribute("CreateDate", "Дата запроса", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<Employee>("Employee", "Исполнитель", x => x.Required())



				.Reference<PensionFile>("PensionFile", "Пенсионное дело", x => x.Required())




				.OperationDelete() 






				.OperationEdit(x => x 


					.InvokesSingleObjectView("undelete")


				)




				.OperationUnDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




			;	

			s.AddClass<ForeCastGpo>("Условие расчета прогноза для ГПО")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("GpoDate", "Дата", MetaDateType.NotNull())


				.Attribute("IncreasePercent", "Увеличение ДО/ДВ (%)", MetaDecimalType.Null())


				.Attribute("Indexing", "Коэффициент индексации", MetaDecimalType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("RaiseRatio", "Коэффициент увеличения СМЗ", MetaDecimalType.Null())


				.Attribute("RestrictionRatio", "Коэффициент ограничения СМЗ", MetaDecimalType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<ForeCast>("ForeCast", "Прогноз", x => x.Required().InverseProperty("ForeCastGpos"))




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<ForeCastPension>("Условие расчета прогноза для ТП")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IndexFBR", "Коэффициент индексации ФБР", MetaDecimalType.Null())


				.Attribute("IndexFBRRaise", "Коэффициент индексации УФБР", MetaDecimalType.Null())


				.Attribute("IndexSCh", "Коэффициент индексации Доли СЧ", MetaDecimalType.Null())


				.Attribute("IndexSCh2", "Коэффициент индексации СЧ без ФБР, Д, В", MetaDecimalType.Null())


				.Attribute("IndexValorization", "Коэффициент индексации Валоризации", MetaDecimalType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PensionDate", "Дата", MetaDateType.NotNull())


				.Attribute("PensionMin", "Минимальный размер ПВЛ ", MetaDecimalType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<ForeCast>("ForeCast", "Прогноз", x => x.Required().InverseProperty("ForeCastPensions"))




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.OperationList() 






				.Operation("UnDelete", "Отменить удаления", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("undelete")


				)




				.OperationView() 






			;	

			s.AddClass<ForeCastResult>("Результат прогноза")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ActualCost", "Фактические затраты", MetaDecimalType.Null())


				.Attribute("BeginDate", "Начало периода", MetaDateType.NotNull())


				.Attribute("EndDate", "Окончание периода", MetaDateType.NotNull())


				.Attribute("IsCalculated", "Расчет выполнен", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("LessMinimum", "ПВЛ<Мин", MetaDecimalType.Null())


				.Attribute("MediumAmount", "Средняя общая", MetaDecimalType.Null())


				.Attribute("PensionAmount", "Средняя ТП", MetaDecimalType.Null())


				.Attribute("PensionCount", "Количество получателей", MetaIntType.NotNull())


				.Attribute("ServiceAmount", "Средняя ПВЛ", MetaDecimalType.Null())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<ForeCastGpo>("ConditionGpo", "Условие расчета для ГПО")



				.Reference<ForeCastPension>("ConditionPension", "Условие расчета для ТП")



				.Reference<ForeCast>("ForeCast", "Прогноз", x => x.Required())




				.Operation("Calc", "Рассчитать", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("calc")


				)




				.Operation("UnCalc", "Отменить рассчет", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("uncalc")


				)




			;	

			s.AddClass<C_RFSubject>("Субъект РФ")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала действия", MetaDateTimeType.NotNull())


				.Attribute("Capital", "Административный центр", MetaStringType.Null())


				.Attribute("Code", "Код", MetaStringType.NotNull())


				.Attribute("EndDate", "Дата окончания действия", MetaDateTimeType.NotNull())


				.Attribute("FDCode", "Код ферерального округа", MetaIntType.Null())


				.Attribute("MasterObjectGUID", "Идентификатор версионного объекта", MetaGuidType.NotNull())


				.Attribute("OKATO", "OKATO", MetaStringType.Null())


				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("TitleDative", "Наименование (дательный падеж)", MetaStringType.Null())


				.Attribute("TitleGenitive", "Наименование (родительный падеж)", MetaStringType.Null())


				.Attribute("TitleLocative", "Наименование (предложный падеж)", MetaStringType.Null())

				.ComputedAttribute("DisplayTitle", "Отображаемое имя", MetaStringType.Null())

				.PersistentComputedAttribute("IsDeleted", "Удален", MetaBooleanType.Null())


				.Reference<C_District>("Districts", "Территории", x => x.Multiple().InverseProperty("RFSubject"))



				.Reference<N_TimeZone>("TimeZone", "Часовой пояс")




				.OperationList() 






				.OperationEdit() 






				.OperationCreateNew() 






				.OperationDelete() 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("ObjectChangeHistory", "История изменений", x => x 


					.InvokesSingleObjectView("changehistory")


				)




				.Operation("Versions", "Журнал версий", x => x 


					.InvokesSingleObjectView("versions")


				)




			;	

			s.AddClass<C_CitizenCategory>("Категория граждан")

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

			s.AddClass<C_Post>("Реестр должностей")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата учреждения", MetaDateType.Null())


				.Attribute("Category", "Категория", MetaStringType.Null())


				.Attribute("Chapter", "Глава", MetaIntType.Null())


				.Attribute("Code", "Код", MetaStringType.Null())


				.Attribute("EndDate", "Дата упразднения", MetaDateType.Null())


				.Attribute("Group", "Группа", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Part", "Раздел", MetaIntType.Null())


				.Attribute("PTK", "Код ПТК", MetaIntType.Null())


				.Attribute("Subpart", "Подраздел", MetaIntType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("TitleGenetive", "Наименование (родительный падеж)", MetaStringType.Null())


				.Reference<OrgUnit>("OrgUnit", "Государственный, законодательный или международный орган")



				.Reference<C_Post>("ParentPost", "Должность для расчета доплаты")



				.Reference<C_PostType>("ParentType", "Тип должности - соответствия")



				.Reference<C_PensionerCategory>("PensionerCategory", "Категория пенсионера")



				.Reference<C_PostPart>("PostPart", "Раздел")



				.Reference<C_PostSalary>("Salary", "Размер должностного оклада", x => x.Multiple().InverseProperty("Post"))



				.Reference<C_PostType>("Type", "Тип Государственная/Федеральная", x => x.Required())




				.OperationDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("typeid")

					.ParmString("returnurl")



				)




				.OperationEdit() 






				.Operation("ViewList1", "Государственные должности") 






				.OperationUnDelete() 






				.OperationView() 






				.Operation("ViewList3", "Прочие должности") 






				.Operation("ViewList2", "Должности федеральных служащих") 






				.Operation("Edit3", "Редактировать прочие", x => x 

					.ParmInt("id")

					.ParmString("returnurl")


					.InvokesSingleObjectView("edit3")


				)




				.Operation("ViewList4", "Воинские должности") 






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




			;	

			s.AddClass<C_ExtraPayPost>("Перечень должностей, дающих право на установление доплаты")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата учреждения", MetaDateType.Null())


				.Attribute("EndDate", "Дата упразднения", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<OrgUnit>("OrgUnit", "Государственный, законодательный или международный орган", x => x.Required())



				.Reference<C_Post>("Post", "Должность для расчета доплаты", x => x.Required())



				.Reference<C_PostPart>("PostPart", "Раздел")



				.Reference<C_PostType>("PostType", "Тип должности", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_RegionalRatio>("Районный коэффициент")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Act", "НПА", MetaStringType.Null())


				.Attribute("ATE", "Административно-территориальные единицы", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PTK", "ПТК", MetaIntType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())

				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<C_RFSubject>("RFSubject", "Субьект РФ")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_OperationReason>("Основание для операций с пенсиями")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Act", "Акт", MetaBooleanType.NotNull())


				.Attribute("Claim", "Личное заявление", MetaBooleanType.NotNull())


				.Attribute("DecisionType", "Вид решения", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PTK", "ПТК", MetaStringType.Null())


				.Attribute("ShortTitle", "Краткое наименование", MetaStringType.Null())


				.Attribute("Title", "Наименование основания", MetaStringType.NotNull())


				.Reference<C_OperationType>("Type", "Наименование операции", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_DocName>("Наименование документов")

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

			s.AddClass<C_RegLog>("Журнал регистрации документов")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Code", "Код", MetaIntType.Null())


				.Attribute("CurrentNo", "Текущий номер документа", MetaIntType.NotNull())


				.Attribute("FormatClass", "Формат номера документа", MetaStringType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Postfix", "Постфикс", MetaStringType.Null())


				.Attribute("Prefix", "Префикс", MetaStringType.Null())


				.Attribute("SpecialWork", "Специальное делопроизводство", MetaBooleanType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_DocType>("Вид обрабатываемых документов")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("CheckPeriod", "Срок проверки", MetaIntType.Null())


				.Attribute("CompleteWarning", "Предупреждение о сроке выполнения ", MetaIntType.NotNull())


				.Attribute("FederalNeeded", "Решение ФО", MetaBooleanType.NotNull())


				.Attribute("FormWarning", "Предупреждение о сроке оформления ", MetaIntType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PlanExecutionPeriod", "Плановый срок выполнения", MetaIntType.Null())


				.Attribute("SenderNeeded", "Отправитель", MetaBooleanType.NotNull())


				.Attribute("SendNeeded", "Включается в пакет отправки (устаревший)", MetaBooleanType.NotNull())


				.Attribute("SendType", "Тип блока", MetaIntType.NotNull())


				.Attribute("SignNeeded", "Требуется вторая подпись", MetaBooleanType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<C_DocName>("Appendix", "Приложения", x => x.Multiple())



				.Reference<C_DocClass>("DocClass", "Тип", x => x.Required())



				.Reference<C_DocName>("DocName", "Наименование документа", x => x.Required())



				.Reference<C_DocSending>("DocSending", "Журнал отправки")



				.Reference<C_RegLog>("RegLog", "Журнал регистрации документов", x => x.Required())



				.Reference<C_SenderCategory>("SenderCategory", "Категория отправителя")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_ProvisionMode>("Способ передачи документов")

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

			s.AddClass<C_RequestCategory>("Категория обращений")

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

			s.AddClass<C_RequestResultCategory>("Категория результатов обращений")

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

			s.AddClass<C_DocClass>("Тип документа")

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

			s.AddClass<C_PostSalary>("Размер должностного оклада")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Amount", "Сумма", MetaDecimalType.NotNull())


				.Attribute("CreateDate", "Дата установления", MetaDateType.NotNull())


				.Attribute("EndDate", "Дата прекращения действия", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PublishDate", "Дата ввода в действие", MetaDateType.NotNull())


				.Attribute("Ratio", "Коэффициент индексации", MetaDecimalType.NotNull())

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<C_PostSalaryIndexing>("Indexing", "Является результатом индексации")



				.Reference<C_Post>("ParentPost", "Соответствующая должность")



				.Reference<C_Post>("Post", "Должность", x => x.Required().InverseProperty("Salary"))



				.Reference<C_PostPart>("PostPart", "Раздел")




				.OperationDelete() 






				.OperationEdit() 






				.OperationUnDelete() 






				.Operation("CreateNew", "Создать", x => x 

					.ParmInt("parentid")

					.ParmString("returnurl")



				)




			;	

			s.AddClass<C_PostType>("Тип должности")

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

			s.AddClass<C_OperationType>("Тип операции")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Action", "Действие операции", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Result", "Результат", MetaStringType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("TitlePrepositional", "Наименование операции (предложный падеж)", MetaStringType.Null())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_SenderCategory>("Категория отправителя")

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

			s.AddClass<C_PensionType>("Вид трудовой пенсии")

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

			s.AddClass<C_PostSalaryIndexing>("Индексация")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("ActDate", "Дата НПА", MetaDateType.Null())


				.Attribute("Condition", "Условие применения", MetaIntType.NotNull())


				.Attribute("IndexingDate", "Дата индексации", MetaDateType.Null())


				.Attribute("IsCalcComplete", "Применение выполнено", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PTK", "ПТК", MetaStringType.Null())


				.Attribute("Rounding", "Округление", MetaIntType.NotNull())


				.Attribute("Title", "Нормативно-правовой акт", MetaStringType.NotNull())


				.Attribute("TitleAblative", "НПА (творительный падеж)", MetaStringType.Null())


				.Attribute("Value", "Коэффициент", MetaDecimalType.Null())

				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<C_PostPart>("PostParts", "Индексируемые разделы", x => x.Multiple())



				.Reference<C_Post>("Posts", "Индексируемые должности", x => x.Multiple())



				.Reference<C_PostType>("PostType", "Тип должности", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_PensionerCategory>("Категория пенсионера")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("AddInfo", "Дополнение", MetaStringType.Null())


				.Attribute("Amounts", "Учитываемые суммы", MetaStringType.Null())


				.Attribute("AmountsInstrumental", "Учитываемые суммы (творительный падеж)", MetaStringType.Null())


				.Attribute("ExceptFull", "Исключение полное", MetaStringType.Null())


				.Attribute("Exception", "Исключение", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("LegalAct", "Нормативно-правовой акт", MetaStringType.Null())


				.Attribute("ReasonChange", "Основание для изменения", MetaStringType.Null())


				.Attribute("ReceiverStatus", "Статус получателя", MetaStringType.Null())


				.Attribute("SignFio", "Подпись - ФИО", MetaStringType.Null())


				.Attribute("SignPost", "Подпись - должность", MetaStringType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<C_PaymentType>("PaymentType", "Вид ГПО", x => x.Required())



				.Reference<C_PostType>("PostType", "Тип должности", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_PaymentStatus>("Состояние выплаты")

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

			s.AddClass<C_PaymentType>("Вид выплаты")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("ShortTitleAblative", "Краткое наименование (творительный падеж)", MetaStringType.Null())


				.Attribute("ShortTitleGenitive", "Краткое наименование (родительный падеж)", MetaStringType.Null())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Attribute("TitleAblative", "Наименование (творительный падеж)", MetaStringType.Null())


				.Attribute("TitleAccusative", "Наименование (винительный падеж)", MetaStringType.Null())


				.Attribute("TitleGenitive", "Наименование (родительный падеж)", MetaStringType.Null())


				.Attribute("TitlePrepositional", "Наименование (предложный падеж)", MetaStringType.Null())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_RestrictionRatio>("Коэффициент ограничения СМЗ")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Примечание", MetaStringType.Null())


				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())

				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<C_PostType>("Type", "Тип получателя ГПО", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_RaiseRatio>("Коэффициент увеличения СМЗ")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("EndDate", "Дата окончания действия", MetaDateType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Примечание", MetaStringType.Null())


				.Attribute("Value", "Коэффициент", MetaDecimalType.NotNull())

				.PersistentComputedAttribute("DisplayTitle", "Отображаемое наименование", MetaStringType.Null())


				.Reference<C_PostType>("Type", "Тип получателя ГПО", x => x.Required())




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_RequestMethod>("Способ обращения")

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

			s.AddClass<C_Seniority>("Стаж государственной службы")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("BeginDate", "Дата начала действия", MetaDateType.Null())


				.Attribute("Comment", "Примечание", MetaStringType.Null())


				.Attribute("CreateDate", "Дата создания записи", MetaDateType.Null())


				.Attribute("DocDate", "Дата документа", MetaDateType.Null())


				.Attribute("DocNo", "Номер документа", MetaStringType.Null())


				.Attribute("EndDate", "Дата окончания действия", MetaDateType.Null())


				.Attribute("Folder", "Папка", MetaStringType.Null())


				.Attribute("IsCountedToward", "Признак стаж засчитывается/не засчитывается", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("OrgUnitTitle", "Наименование органа ГУ", MetaStringType.Null())


				.Attribute("Reason", "Основание", MetaStringType.Null())


				.Attribute("Title", "Наименование организации", MetaStringType.NotNull())


				.Reference<Employee>("Employee", "Исполнитель")




				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_PostPart>("Элемент структуры должностей федеральных служащих")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Chapter", "Глава", MetaStringType.NotNull())


				.Attribute("Description", "Описание", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Part", "Раздел", MetaStringType.NotNull())


				.Attribute("ShortDescription", "Краткое описание", MetaStringType.Null())


				.Attribute("Subpart", "Подраздел", MetaStringType.NotNull())

				.PersistentComputedAttribute("ShortTitle", "Краткое наименование", MetaStringType.Null())

				.PersistentComputedAttribute("Title", "Наименование", MetaStringType.Null())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_MassCalcStatus>("Статус массового перерасчета")

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

			s.AddClass<C_DocTaskType>("Тип задания")

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

			s.AddClass<C_Sign>("Подпись")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Post", "Должность", MetaStringType.Null())


				.Attribute("PostDative", "Должность (дательный падеж)", MetaStringType.Null())


				.Attribute("Sign1PostType1", "1 подпись для типа Госдолжность", MetaBooleanType.NotNull())


				.Attribute("Sign1PostType2", "1 подпись для типа Федеральные служащие", MetaBooleanType.NotNull())


				.Attribute("Sign2PostType1", "2 подпись для типа Госдолжность", MetaBooleanType.NotNull())


				.Attribute("Sign2PostType2", "2 подпись для типа Федеральные служащие", MetaBooleanType.NotNull())


				.Attribute("SignComplaint", "Подпись для ответов в СО", MetaBooleanType.NotNull())


				.Attribute("SignRequest", "Подпись для ответов на обращения", MetaBooleanType.NotNull())


				.Attribute("Title", "Ф.И.О.", MetaStringType.NotNull())


				.Attribute("TitleDative", "Ф.И.О. (дательный падеж)", MetaStringType.Null())



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

			s.AddClass<C_InfoDocType>("Тип информационного документа")

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

			s.AddClass<C_JuridicalInstance>("Инстанция")

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

			s.AddClass<C_CourtSessionStatus>("Статус судебного заседания")

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

			s.AddClass<C_MassOperationType>("Массовая операция")

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

			s.AddClass<C_MassOperationState>("Состояние массовой операции")

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

			s.AddClass<C_SpecialNote>("Особая отметка")

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

			s.AddClass<C_District>("Территория")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Code", "Код", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())


				.Reference<C_RFSubject>("RFSubject", "Субъект РФ", x => x.Required().InverseProperty("Districts"))




				.OperationDelete() 






				.OperationEdit(x => x 


					.InvokesSingleObjectView("undelete")


				)




				.OperationUnDelete() 






			;	

			s.AddClass<C_DocSending>("Журнал отправки документов")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("Code", "Код", MetaStringType.Null())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("SendRegion", "По регионам", MetaBooleanType.NotNull())


				.Attribute("SendType", "Вид отправки", MetaIntType.NotNull())


				.Attribute("Title", "Наименование", MetaStringType.NotNull())



				.OperationDelete() 






				.OperationCreateNew() 






				.OperationEdit() 






				.OperationList() 






				.OperationUnDelete() 






				.OperationView() 






			;	

			s.AddClass<C_RequestResultManagement>("Категория результатов обращений из АП")

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

			s.AddClass<C_PensionFileCategory>("Раздел хранилища")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("IsDefault", "Признак По умолчанию", MetaBooleanType.NotNull())


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

			s.AddClass<C_ComplaintResult>("Результат рассмотрения иска")

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

			s.AddClass<Reports>("Отчеты")


				.NonPersistent()





				.Operation("gpo_ReportRecipients", "Отчет по получателям ГПО", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "gpo_ReportRecipients")


				)




				.Operation("gpo_ReportImplementation", "Отчет по операциям", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "gpo_ReportImplementation")


				)




				.Operation("ReportDocTaskRequests", "Отчет по работе с обращениями", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "ReportDocTaskRequests")


				)




				.Operation("ReportComplaints", "Отчет по работе с жалобами", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "ReportComplaints")


				)




				.Operation("ReportInDoc", "Отчет по входящим документам", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "ReportInDoc")


				)




				.Operation("ReportOutDoc", "Отчет по исходящим документам", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "ReportOutDoc")


				)




				.Operation("gpo_ReportAchievement", "Отчет по достижению сроков выплаты ГПО", x => x 


					.InvokesView("Nephrite.Web.ViewControl", "gpo_ReportAchievement")


				)




			;	

			s.AddClass<C_Scanner>("Сканер")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("FileFormat", "Формат файла", MetaEnumType.NotNull("NetScanFileFormat"))


				.Attribute("IsAvailable", "Доступен", MetaBooleanType.NotNull())


				.Attribute("IsColoredScan", "Цветное сканирование", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("Location", "Расположение сканера", MetaStringType.Null())


				.Attribute("Model", "Марка/модель сканера", MetaStringType.Null())


				.Attribute("PaperFormat", "Формат бумаги", MetaEnumType.NotNull("NetScanPaperFormat"))


				.Attribute("Resolution", "Разрешение", MetaEnumType.NotNull("NetScanResolution"))


				.Attribute("SysName", "Системное имя", MetaStringType.NotNull())


				.Attribute("Title", "Имя сканера", MetaStringType.NotNull())



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

			s.AddClass<ScanUserSettings>("Настройки сканирования")

				.IntKey()




				.TimeStamp<SPM_Subject>()



				.Attribute("FileFormat", "Формат файла", MetaEnumType.NotNull("NetScanFileFormat"))


				.Attribute("IsColoredScan", "Цветное сканирование", MetaBooleanType.NotNull())


				.Attribute("IsDeleted", "Удален", MetaBooleanType.NotNull())


				.Attribute("PaperFormat", "Формат бумаги", MetaEnumType.NotNull("NetScanPaperFormat"))


				.Attribute("Resolution", "Разрешение", MetaEnumType.NotNull("NetScanResolution"))

				.ComputedAttribute("Title", "Наименование", MetaStringType.Null())


				.Reference<C_Scanner>("Scanner", "Сканер")



				.Reference<SPM_Subject>("Subject", "Пользователь", x => x.Required())




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

			return s;
		}
	}

}

