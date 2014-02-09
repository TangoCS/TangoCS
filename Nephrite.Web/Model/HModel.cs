using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Xml.Linq;

namespace Nephrite.Web.Model
{
	public partial class MailMessage
	{
		public virtual int MailMessageID { get; set; }
		public virtual string Recipients { get; set; }
		public virtual string Subject { get; set; }
		public virtual string Body { get; set; }
		public virtual bool IsSent { get; set; }
		public virtual byte[] Attachment { get; set; }
		public virtual string AttachmentName { get; set; }
		public virtual string Error { get; set; }
		public virtual string CopyRecipients { get; set; }
		public virtual System.Nullable<System.DateTime> LastSendAttemptDate { get; set; }
		public virtual int AttemptsToSendCount { get; set; }
	}

	public partial class N_TimeZone
	{
		public N_TimeZone()
		{
			HST_N_TimeZones = new List<HST_N_TimeZone>();
		}
		public virtual int TimeZoneID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual int GMTOffset { get; set; }
		public virtual string Comment { get; set; }
		public virtual IList<HST_N_TimeZone> HST_N_TimeZones { get; set; }
	}

	public partial class HST_N_TimeZone
	{
		public virtual int TimeZoneVersionID { get; set; }
		public virtual N_TimeZone N_TimeZone { get; set; }
		public virtual int TimeZoneID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual int VersionNumber { get; set; }
		public virtual bool IsCurrentVersion { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual int GMTOffset { get; set; }
		public virtual string Comment { get; set; }
	}

	public partial class MailTemplate
	{
		public virtual int MailTemplateID { get; set; }
		public virtual string Title { get; set; }
		public virtual string TemplateSubject { get; set; }
		public virtual string TemplateBody { get; set; }
		public virtual string Comment { get; set; }
		public virtual bool IsSystem { get; set; }
	}

	public partial class C_Language
	{
		public virtual string LanguageCode { get; set; }
		public virtual string Title { get; set; }
		public virtual bool IsDefault { get; set; }

		public virtual string Code
		{
			get
			{
				return LanguageCode;
			}
		}
	}

	public partial class N_TextResource
	{
		public N_TextResource()
		{
			N_TextResourceDatas = new List<N_TextResourceData>();
		}
		public virtual int TextResourceID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual IList<N_TextResourceData> N_TextResourceDatas { get; set; }
	}

	public partial class N_TextResourceData
	{
		public virtual int N_TextResourceDataID { get; set; }
		public virtual N_TextResource N_TextResource { get; set; }
		public virtual C_Language C_Language { get; set; }
		public virtual string Text { get; set; }

		public virtual string LanguageCode { get; set; }
		public virtual int TextResourceID { get; set; }
	}

	public partial class N_Filter
	{
		public virtual int FilterID { get; set; }
		public virtual int? SubjectID { get; set; }
		public virtual string ListName { get; set; }
		public virtual XDocument FilterValue { get; set; }
		public virtual string FilterName { get; set; }
		public virtual bool IsDefault { get; set; }
		public virtual System.Nullable<int> Group1Column { get; set; }
		public virtual string Group1Sort { get; set; }
		public virtual System.Nullable<int> Group2Column { get; set; }
		public virtual string Group2Sort { get; set; }
		public virtual string ListParms { get; set; }
		public virtual string Columns { get; set; }
		public virtual string Sort { get; set; }
		public virtual int ItemsOnPage { get; set; }
	}

	public partial class CalendarDay
	{
		public virtual int CalendarDayID { get; set; }
		public virtual System.DateTime Date { get; set; }
		public virtual bool IsWorkingDay { get; set; }
	}

	public partial class TM_Task 
	{
		public virtual int TaskID { get; set; }
		public virtual string Title { get; set; }
		public virtual string Class { get; set; }
		public virtual bool StartType { get; set; }
		public virtual string Method { get; set; }
		public virtual int Interval { get; set; }
		public virtual System.Nullable<System.DateTime> LastStartDate { get; set; }
		public virtual bool IsSuccessfull { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool StartFromService { get; set; }
		public virtual System.Nullable<int> ErrorLogID { get; set; }
		public virtual int ExecutionTimeout { get; set; }
	}

	public partial class TM_TaskExecution 
	{
		public virtual int TaskExecutionID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual System.DateTime StartDate { get; set; }
		public virtual System.Nullable<System.DateTime> FinishDate { get; set; }
		public virtual bool IsSuccessfull { get; set; }
		public virtual string MachineName { get; set; }
		public virtual string ResultXml { get; set; }
		public virtual string ExecutionLog { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
	}

	public partial class TM_TaskParameter 
	{
		public virtual int TaskParameterID { get; set; }
		public virtual int ParentID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Value { get; set; }
		public virtual int SeqNo { get; set; }

		public virtual Expression<Func<TM_TaskParameter, bool>> KeySelector(int id)
		{
			return o => o.TaskParameterID == id;
		}
	}

	public partial class N_DownloadLog
	{
		public virtual int DownloadLogID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		//public virtual N_File N_File { get; set; }
		public virtual int FileID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string IP { get; set; }
	}

	public partial class N_File
	{
		public virtual int FileID { get; set; }
		public virtual string Title { get; set; }
		public virtual N_Folder N_Folder { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual long Length { get; set; }
		public virtual int? LastModifiedUserID { get; set; }
		public virtual int? CheckedOutByID { get; set; }
		public virtual int CreatorID { get; set; }
		public virtual bool IsDiskStorage { get; set; }
		public virtual string Extension { get; set; }
		public virtual string Path { get; set; }
		public virtual string StorageType { get; set; }
		public virtual string StorageParameter { get; set; }
		public virtual string Password { get; set; }
		public virtual string GuidPath { get; set; }
		public virtual System.Nullable<System.Guid> FeatureGUID { get; set; }
		public virtual System.DateTime BeginDate { get; set; }
		public virtual System.DateTime EndDate { get; set; }
		public virtual int VersionNumber { get; set; }
		public virtual System.Nullable<System.Guid> MainGUID { get; set; }
		public virtual string Tag { get; set; }
		public virtual System.Nullable<System.DateTime> PublishDate { get; set; }
		//public virtual bool IsDeleted { get; set; }
	}

	public partial class N_Folder
	{
		public virtual int FolderID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual int CreatorID { get; set; }
		public virtual string Title { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string FullPath { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual string StorageType { get; set; }
		public virtual string StorageParameter { get; set; }
		public virtual string GuidPath { get; set; }
		public virtual bool IsReplicable { get; set; }
		public virtual System.Guid SPMActionItemGUID { get; set; }
		public virtual bool EnableVersioning { get; set; }
		public virtual string Tag { get; set; }
		public virtual System.Nullable<System.DateTime> PublishDate { get; set; }
	}

	public partial class N_VirusScanLog
	{
		public virtual int VirusScanLogID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual int ResultCode { get; set; }
	}

	public partial class N_TableInfo
	{
		public virtual string TableName { get; set; }
		public virtual System.DateTime LastDataModify { get; set; }
	}



	public partial class N_Setting
	{
		public virtual System.Guid SettingsGUID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual string SystemName { get; set; }
		public virtual string Title { get; set; }
		public virtual string Value { get; set; }
		public virtual bool IsSystem { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string AcceptableValues { get; set; }
	}

	public partial class N_Cache
	{
		public virtual System.DateTime TimeStamp { get; set; }
	}

	public partial class UserActivity
	{
		public virtual int UserActivityID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual string ObjectKey { get; set; }
		public virtual string ObjectTypeSysName { get; set; }
		public virtual string Action { get; set; }
		public virtual string UserTitle { get; set; }
		public virtual string ObjectTypeTitle { get; set; }
		public virtual string IP { get; set; }
	}

	[Serializable]
	public partial class N_TaskType
	{
		public virtual int TaskTypeID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Class { get; set; }
		public virtual string Method { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual string ArgumentClass { get; set; }
		public virtual string ResultClass { get; set; }
	}

	[Serializable]
	public partial class N_Task
	{
		public virtual System.Guid GUID { get; set; }
		public virtual System.DateTime CreateDate { get; set; }
		public virtual int CreatorID { get; set; }
		public virtual int TaskTypeID { get; set; }
		public virtual System.Nullable<System.DateTime> StartTime { get; set; }
		public virtual System.Nullable<System.DateTime> FinishTime { get; set; }
		public virtual bool Notify { get; set; }
		public virtual System.Nullable<int> ErrorLogID { get; set; }
		public virtual string ErrorLogMessage { get; set; }
		public virtual System.Nullable<System.DateTime> AbortRequestDate { get; set; }
		public virtual int Status { get; set; }
		public virtual decimal PercentDone { get; set; }
		public virtual System.Nullable<System.DateTime> PercentDoneDate { get; set; }
		public virtual string Argument { get; set; }
		public virtual string Result { get; set; }
	}
}