using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using Nephrite.Controls;
using Nephrite.EntityAudit;
using Nephrite.ErrorLog;
using Nephrite.FileStorage;
using Nephrite.RSS;
using Nephrite.UI.Controls;
using Nephrite.FileStorage.Std;
//using Nephrite.Web.Mailer;
//using Nephrite.Web.TaskManager;

//namespace Nephrite.Web.CoreDataContext
//{
//	public class ErrorLog : IErrorLog, IWithKey<ErrorLog, int>, IWithoutEntityAudit
//	{
//		public virtual int ErrorLogID { get; set; }
//		public virtual DateTime ErrorDate { get; set; }
//		public virtual string ErrorText { get; set; }
//		public virtual string Url { get; set; }
//		public virtual string UrlReferrer { get; set; }
//		public virtual string UserHostName { get; set; }
//		public virtual string UserHostAddress { get; set; }
//		public virtual string UserAgent { get; set; }
//		public virtual string RequestType { get; set; }
//		public virtual string Headers { get; set; }
//		public virtual string SqlLog { get; set; }
//		public virtual string UserName { get; set; }
//		public virtual byte[] Hash { get; set; }
//		public virtual Nullable<int> SimilarErrorID { get; set; }

//		public int ID
//		{
//			get
//			{
//				return ErrorLogID;
//			}
//		}

//		public virtual Expression<Func<ErrorLog, bool>> KeySelector(int id)
//		{
//			return o => o.ErrorLogID == id;
//		}
//	} 
//	public partial class CalendarDay : ICalendarDay
//	{
//		public virtual int CalendarDayID { get; set; }
//		public virtual DateTime Date { get; set; }
//		public virtual bool IsWorkingDay { get; set; }
//	}
	 
//	public partial class N_Filter : IN_Filter, IWithoutEntityAudit
//	{
//		public virtual int FilterID { get; set; }
//		public virtual int? SubjectID { get; set; }
//		public virtual string ListName { get; set; }
//		public virtual XDocument FilterValue { get; set; }
//		public virtual string FilterName { get; set; }
//		public virtual bool IsDefault { get; set; }
//		public virtual Nullable<int> Group1Column { get; set; }
//		public virtual string Group1Sort { get; set; }
//		public virtual Nullable<int> Group2Column { get; set; }
//		public virtual string Group2Sort { get; set; }
//		public virtual string ListParms { get; set; }
//		public virtual string Columns { get; set; }
//		public virtual string Sort { get; set; }
//		public virtual int ItemsOnPage { get; set; }
//	}

//	public partial class N_DownloadLog : IN_DownloadLog, IWithoutEntityAudit
//	{
//		public virtual int DownloadLogID { get; set; }
//		public virtual int LastModifiedUserID { get; set; }
//		//public virtual N_File N_File { get; set; }
//		public virtual Guid FileGUID { get; set; }
//		public virtual bool IsDeleted { get; set; }
//		public virtual DateTime LastModifiedDate { get; set; }
//		public virtual string IP { get; set; }
//	}

//	public partial class N_FileData : IDbFileData, IWithoutEntityAudit
//	{
//		public virtual byte[] Data { get; set; }
//		public virtual string Extension { get; set; }
//		public virtual Guid FileGUID { get; set; }
//		public virtual long Size { get; set; }
//		public virtual DateTime LastModifiedDate { get; set; }
//		public virtual Guid? Owner { get; set; }
//		public virtual string Title { get; set; }
//	}

//	public partial class N_VirusScanLog : IN_VirusScanLog, IWithoutEntityAudit
//	{
//		public virtual int VirusScanLogID { get; set; }
//		public virtual int LastModifiedUserID { get; set; }
//		public virtual bool IsDeleted { get; set; }
//		public virtual DateTime LastModifiedDate { get; set; }
//		public virtual string Title { get; set; }
//		public virtual int ResultCode { get; set; }
//	}

//	public partial class N_ObjectChange : IN_ObjectChange, IWithoutEntityAudit
//	{
//		public virtual int ObjectChangeID { get; set; }
//		public virtual int SubjectID { get; set; }
//		public virtual DateTime LastModifiedDate { get; set; }
//		public virtual string Title { get; set; }
//		public virtual string ObjectKey { get; set; }
//		public virtual string ObjectTypeSysName { get; set; }
//		public virtual string ObjectTitle { get; set; }
//		public virtual string UserTitle { get; set; }
//		public virtual string UserLogin { get; set; }
//		public virtual string ObjectTypeTitle { get; set; }
//		public virtual string IP { get; set; }
//		public virtual string Details { get; set; }
//	}

//	public partial class N_ObjectPropertyChange : IN_ObjectPropertyChange, IWithoutEntityAudit
//	{
//		public virtual Int32 ObjectPropertyChangeID { get; set; }
//		public virtual String Title { get; set; }
//		public virtual String PropertySysName { get; set; }
//		public virtual String OldValue { get; set; }
//		public virtual String NewValue { get; set; }
//		public virtual String OldValueTitle { get; set; }
//		public virtual String NewValueTitle { get; set; }
//		public virtual Int32 ObjectChangeID
//		{
//			get
//			{

//				if (ObjectChange == null) return 0;
//				return ObjectChange.ObjectChangeID;
//			}
//			set
//			{

//				ObjectChange = new N_ObjectChange { ObjectChangeID = value };
//			}
//		}
//		public virtual IN_ObjectChange ObjectChange { get; set; }
//	}


//	public partial class N_RssFeed : IN_RssFeed
//	{
//		public virtual int RssFeedID { get; set; }
//		public virtual string Copyright { get; set; }
//		public virtual string Description { get; set; }
//		public virtual bool IsDeleted { get; set; }
//		public virtual DateTime LastModifiedDate { get; set; }
//		public virtual int LastModifiedUserID { get; set; }
//		public virtual string ObjectTypeSysName { get; set; }
//		public virtual string Predicate { get; set; }
//		public virtual string PubDate { get; set; }
//		public virtual string SysName { get; set; }
//		public virtual string Title { get; set; }
//		public virtual int Ttl { get; set; }
//		public virtual string ViewFormSysName { get; set; }
//		public virtual string Author { get; set; }
//		public virtual string WebMaster { get; set; }
//		public virtual string LinkParams { get; set; }
//	}

//}