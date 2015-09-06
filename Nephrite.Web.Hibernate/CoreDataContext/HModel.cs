using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Xml.Linq;
using Nephrite.Html;
using Nephrite.Meta;
using Nephrite.Web.Controls;
using Nephrite.ErrorLog;
using Nephrite.Web.FormsEngine;
using Nephrite.Web.Hibernate;
using Nephrite.Web.Mailer;
using Nephrite.Multilanguage;
using Nephrite.RSS;
using Nephrite.SettingsManager;

using Nephrite.Web.TaskManager;
using Nephrite.Html.Controls;
using Nephrite.EntityAudit;
using Nephrite.FileStorage;

namespace Nephrite.Web.CoreDataContext
{
	public class ErrorLog : IErrorLog, IWithKey<ErrorLog, int>, IWithoutEntityAudit
	{
		public virtual int ErrorLogID { get; set; }
		public virtual DateTime ErrorDate { get; set; }
		public virtual string ErrorText { get; set; }
		public virtual string Url { get; set; }
		public virtual string UrlReferrer { get; set; }
		public virtual string UserHostName { get; set; }
		public virtual string UserHostAddress { get; set; }
		public virtual string UserAgent { get; set; }
		public virtual string RequestType { get; set; }
		public virtual string Headers { get; set; }
		public virtual string SqlLog { get; set; }
		public virtual string UserName { get; set; }
		public virtual byte[] Hash { get; set; }
		public virtual Nullable<int> SimilarErrorID { get; set; }

		public int ID
		{
			get
			{
				return ErrorLogID;
			}
		}

		public virtual Expression<Func<ErrorLog, bool>> KeySelector(int id)
		{
			return o => o.ErrorLogID == id;
		}
	} 
	public partial class CalendarDay : ICalendarDay
	{
		public virtual int CalendarDayID { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual bool IsWorkingDay { get; set; }
	}
	 
	public partial class MailMessage : IMailMessage
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
		public virtual Nullable<DateTime> LastSendAttemptDate { get; set; }
		public virtual int AttemptsToSendCount { get; set; }
	}

	public class MailTemplate : IMailTemplate, IWithTitle
	{
		public virtual int MailTemplateID { get; set; }
		public virtual string Title { get; set; }
		public virtual string TemplateSubject { get; set; }
		public virtual string TemplateBody { get; set; }
		public virtual string Comment { get; set; }
		public virtual bool IsSystem { get; set; }

		public virtual string GetTitle()
		{
			return Title;
		}

		public virtual Func<MailTemplate, string> OrderByTitle()
		{
			return o => o.Title;
		}
	}

	public partial class N_TimeZone : IN_TimeZone
	{
		public virtual int TimeZoneID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual int GMTOffset { get; set; }
		public virtual string Comment { get; set; }
	}


	public partial class C_Language : ILanguageObject
	{
		public virtual string Code { get; set; }
		public virtual string Title { get; set; }
		public virtual bool IsDefault { get; set; }
	}

	public partial class V_N_TextResource : IN_TextResource
	{
 
		public virtual int TextResourceID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Text { get; set; }
		public virtual string LanguageCode { get; set; }
	}


	public partial class N_Filter : IN_Filter, IWithoutEntityAudit
	{
		public virtual int FilterID { get; set; }
		public virtual int? SubjectID { get; set; }
		public virtual string ListName { get; set; }
		public virtual XDocument FilterValue { get; set; }
		public virtual string FilterName { get; set; }
		public virtual bool IsDefault { get; set; }
		public virtual Nullable<int> Group1Column { get; set; }
		public virtual string Group1Sort { get; set; }
		public virtual Nullable<int> Group2Column { get; set; }
		public virtual string Group2Sort { get; set; }
		public virtual string ListParms { get; set; }
		public virtual string Columns { get; set; }
		public virtual string Sort { get; set; }
		public virtual int ItemsOnPage { get; set; }
	}

	public partial class TM_Task : ITM_Task
	{
		public virtual int TaskID { get; set; }
		public virtual string Title { get; set; }
		public virtual string Class { get; set; }
		public virtual bool StartType { get; set; }
		public virtual string Method { get; set; }
		public virtual int Interval { get; set; }
		public virtual Nullable<DateTime> LastStartDate { get; set; }
		public virtual bool IsSuccessfull { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool StartFromService { get; set; }
		public virtual Nullable<int> ErrorLogID { get; set; }
		public virtual int ExecutionTimeout { get; set; }
	}

	public partial class TM_TaskExecution : ITM_TaskExecution, IWithoutEntityAudit
	{
		public virtual int TaskExecutionID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual Nullable<DateTime> FinishDate { get; set; }
		public virtual bool IsSuccessfull { get; set; }
		public virtual string MachineName { get; set; }
		public virtual string ResultXml { get; set; }
		public virtual string ExecutionLog { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
	}

	public partial class TM_TaskParameter : ITM_TaskParameter, IWithSeqNo, IWithKey<TM_TaskParameter, int>
	{
		public virtual int TaskParameterID { get; set; }
		public virtual int ParentID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Value { get; set; }
		public virtual int SeqNo { get; set; }

		public int ID
		{
			get
			{
				return TaskParameterID;
			}
		}

		public virtual Expression<Func<TM_TaskParameter, bool>> KeySelector(int id)
		{
			return o => o.TaskParameterID == id;
		}
	}

	public partial class N_DownloadLog : IN_DownloadLog, IWithoutEntityAudit
	{
		public virtual int DownloadLogID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		//public virtual N_File N_File { get; set; }
		public virtual Guid FileGUID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual string IP { get; set; }
	}

	public partial class N_FileData : IDbFileData, IWithoutEntityAudit
	{
		public virtual byte[] Data { get; set; }
		public virtual string Extension { get; set; }
		public virtual Guid FileGUID { get; set; }
		public virtual int Size { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual Guid? Owner { get; set; }
		public virtual string Title { get; set; }
	}

	public partial class N_VirusScanLog : IN_VirusScanLog, IWithoutEntityAudit
	{
		public virtual int VirusScanLogID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual int ResultCode { get; set; }
	}

	/*public partial class N_TableInfo
	{
		public virtual string TableName { get; set; }
		public virtual DateTime LastDataModify { get; set; }
	}*/

	public partial class N_Settings : IN_Settings
	{
		public virtual Guid SettingsGUID { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual string SystemName { get; set; }
		public virtual string Title { get; set; }
		public virtual string Value { get; set; }
		public virtual bool IsSystem { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual string AcceptableValues { get; set; }

		public virtual int? GroupID { get; set; }
	}

	public partial class N_ObjectChange : IN_ObjectChange, IWithoutEntityAudit
	{
		public virtual int ObjectChangeID { get; set; }
		public virtual int SubjectID { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual string Title { get; set; }
		public virtual string ObjectKey { get; set; }
		public virtual string ObjectTypeSysName { get; set; }
		public virtual string ObjectTitle { get; set; }
		public virtual string UserTitle { get; set; }
		public virtual string UserLogin { get; set; }
		public virtual string ObjectTypeTitle { get; set; }
		public virtual string IP { get; set; }
		public virtual string Details { get; set; }
	}

	public partial class N_ObjectPropertyChange : IN_ObjectPropertyChange, IWithoutEntityAudit
	{
		public virtual Int32 ObjectPropertyChangeID { get; set; }
		public virtual String Title { get; set; }
		public virtual String PropertySysName { get; set; }
		public virtual String OldValue { get; set; }
		public virtual String NewValue { get; set; }
		public virtual String OldValueTitle { get; set; }
		public virtual String NewValueTitle { get; set; }
		public virtual Int32 ObjectChangeID
		{
			get
			{

				if (ObjectChange == null) return 0;
				return ObjectChange.ObjectChangeID;
			}
			set
			{

				ObjectChange = new N_ObjectChange { ObjectChangeID = value };
			}
		}
		public virtual IN_ObjectChange ObjectChange { get; set; }
		//public virtual IN_ObjectChange IObjectChange
		//{
		//	get
		//	{
		//		return ObjectChange as IN_ObjectChange;
		//	}
		//	set
		//	{
		//		ObjectChange = (N_ObjectChange)value;
		//	}
		//}
	}


	public partial class N_RssFeed : IN_RssFeed
	{
		public virtual int RssFeedID { get; set; }
		public virtual string Copyright { get; set; }
		public virtual string Description { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual string ObjectTypeSysName { get; set; }
		public virtual string Predicate { get; set; }
		public virtual string PubDate { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Title { get; set; }
		public virtual int Ttl { get; set; }
		public virtual string ViewFormSysName { get; set; }
		public virtual string Author { get; set; }
		public virtual string WebMaster { get; set; }
		public virtual string LinkParams { get; set; }
	}

}