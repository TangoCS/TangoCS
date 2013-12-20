using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Nephrite.Web.CoreDataContext
{
	public class ErrorLogMap : ClassMapping<ErrorLog>
	{
		public ErrorLogMap()
		{
			Table("ErrorLog");
			Lazy(true);
			Id(x => x.ErrorLogID, map => map.Generator(Generators.Identity));
			Property(x => x.ErrorDate, map => map.NotNullable(true));
			Property(x => x.ErrorText, map => map.NotNullable(true));
			Property(x => x.Url);
			Property(x => x.UrlReferrer);
			Property(x => x.UserHostName);
			Property(x => x.UserHostAddress);
			Property(x => x.UserAgent);
			Property(x => x.RequestType);
			Property(x => x.Headers);
			Property(x => x.SqlLog);
			Property(x => x.UserName);
			Property(x => x.Hash);
			Property(x => x.SimilarErrorID);
		}
	}
	
	public class CalendarDayMap : ClassMapping<CalendarDay>
	{
		public CalendarDayMap()
		{
			Table("CalendarDay");
			Lazy(true);
			Id(x => x.CalendarDayID, map => map.Generator(Generators.Identity));
			Property(x => x.Date, map => map.NotNullable(true));
			Property(x => x.IsWorkingDay, map => map.NotNullable(true));
		}
	}
	 
	public class MailMessageMap : ClassMapping<MailMessage>
	{
		public MailMessageMap()
		{
			Table("MailMessage");
			Lazy(true);
			Id(x => x.MailMessageID, map => map.Generator(Generators.Identity));
			Property(x => x.Recipients);
			Property(x => x.Subject);
			Property(x => x.Body);
			Property(x => x.IsSent, map => map.NotNullable(true));
			Property(x => x.Attachment, map => map.Type<BinaryBlobType>());
			Property(x => x.AttachmentName);
			Property(x => x.Error);
			Property(x => x.CopyRecipients);
			Property(x => x.LastSendAttemptDate);
			Property(x => x.AttemptsToSendCount, map => map.NotNullable(true));
		}
	}

	public class MailTemplateMap : ClassMapping<MailTemplate>
	{
		public MailTemplateMap()
		{
			Lazy(true);
			Id(x => x.MailTemplateID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.TemplateSubject, map => map.NotNullable(true));
			Property(x => x.TemplateBody, map => map.NotNullable(true));
			Property(x => x.Comment);
			Property(x => x.IsSystem, map => map.NotNullable(true));
		}
	}

	public class N_TimeZoneMap : ClassMapping<N_TimeZone>
	{
		public N_TimeZoneMap()
		{
			Table("N_TimeZone");
			Lazy(true);
			Id(x => x.TimeZoneID, map => map.Generator(Generators.Identity));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.GMTOffset, map => map.NotNullable(true));
			Property(x => x.Comment);
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}

	public class C_LanguageMap : ClassMapping<C_Language>
	{
		public C_LanguageMap()
		{
			Table("C_Language");
			Lazy(true);
			Id(x => x.Code, map => { map.Generator(Generators.Assigned); map.Length(2); map.Column("LanguageCode"); });
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsDefault, map => map.NotNullable(true));
		}
	}

	public class V_N_TextResourceMap : ClassMapping<V_N_TextResource>
	{
		public V_N_TextResourceMap()
		{
			Table("V_N_TextResource");

			Lazy(true);
			Id(x => x.TextResourceID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Text);
			Property(x => x.LanguageCode, map => { map.NotNullable(true); });
			Property(x => x.TextResourceID, map => { map.NotNullable(true); });
		}
	}

	public class N_FilterMap : ClassMapping<N_Filter>
	{
		public N_FilterMap()
		{
			Table("N_Filter");
			Lazy(true);
			Id(x => x.FilterID, map => map.Generator(Generators.Identity));
			Property(x => x.ListName);
			Property(x => x.FilterValue, map => map.Type<XDocType>());
			Property(x => x.FilterName);
			Property(x => x.IsDefault, map => map.NotNullable(true));
			Property(x => x.Group1Column);
			Property(x => x.Group1Sort);
			Property(x => x.Group2Column);
			Property(x => x.Group2Sort);
			Property(x => x.ListParms);
			Property(x => x.Columns);
			Property(x => x.Sort);
			Property(x => x.ItemsOnPage, map => map.NotNullable(true));
			Property(x => x.SubjectID, map =>
			{
				map.Column("SubjectID");
			});
		}
	}

	

	public class TM_TaskParameterMap : ClassMapping<TM_TaskParameter>
	{
		public TM_TaskParameterMap()
		{
			Table("TM_TaskParameter");
			Lazy(true);
			Id(x => x.TaskParameterID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Value);
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.ParentID, map => map.NotNullable(true));
		}
	}

	public class TM_TaskExecutionMap : ClassMapping<TM_TaskExecution>
	{
		public TM_TaskExecutionMap()
		{
			Table("TM_TaskExecution");
			Lazy(true);
			Id(x => x.TaskExecutionID, map => map.Generator(Generators.Identity));
			Property(x => x.StartDate, map => map.NotNullable(true));
			Property(x => x.FinishDate);
			Property(x => x.IsSuccessfull, map => map.NotNullable(true));
			Property(x => x.MachineName, map => map.NotNullable(true));
			Property(x => x.ResultXml);
			Property(x => x.ExecutionLog);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.TaskID, map => map.NotNullable(true));
		}
	}

	public class TM_TaskMap : ClassMapping<TM_Task>
	{
		public TM_TaskMap()
		{
			Table("TM_Task");
			Lazy(true);
			Id(x => x.TaskID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Class, map => map.NotNullable(true));
			Property(x => x.StartType, map => map.NotNullable(true));
			Property(x => x.Method, map => map.NotNullable(true));
			Property(x => x.Interval, map => map.NotNullable(true));
			Property(x => x.LastStartDate);
			Property(x => x.IsSuccessfull, map => map.NotNullable(true));
			Property(x => x.IsActive, map => map.NotNullable(true));
			Property(x => x.StartFromService, map => map.NotNullable(true));
			Property(x => x.ErrorLogID);
			Property(x => x.ExecutionTimeout, map => map.NotNullable(true));
		}
	}

	public class N_DownloadLogMap : ClassMapping<N_DownloadLog>
	{
		public N_DownloadLogMap()
		{
			Table("N_DownloadLog");
			Lazy(true);
			Id(x => x.DownloadLogID, map => map.Generator(Generators.Identity));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.IP);
			Property(x => x.FileGUID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}

	public class V_DbFileMap : ClassMapping<V_DbFile>
	{
		public V_DbFileMap()
		{
			Table("V_DbFile");
			Lazy(true);
			Id(x => x.ID, map => map.Generator(Generators.Guid));
			Property(x => x.Size, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Extension, map => map.NotNullable(true));
			Property(x => x.Path);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Path);
			Property(x => x.FullPath);
			Property(x => x.FeatureGUID);
			Property(x => x.VersionNumber, map => { map.NotNullable(true); map.Insert(false); map.Update(false); });
			Property(x => x.MainID, map => { map.Insert(false); map.Update(false); });
			Property(x => x.Tag);
			Property(x => x.PublishDate);

			Property(x => x.CreatorID, map => map.NotNullable(true));
			Property(x => x.Creator);
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserName, map => map.NotNullable(true));
			Property(x => x.CheckedOutByID);
			Property(x => x.CheckedOutBy);

			Property(x => x.IsDeleted, map => { map.Insert(false); map.Update(false); });
			
			Property(x => x.SPMActionItemGUID, map => { map.Insert(false); map.Update(false); });
			Property(x => x.IsValid);
			Property(x => x.ParentFolderID);
		}
	}

	public class V_DbFolderMap : ClassMapping<V_DbFolder>
	{
		public V_DbFolderMap()
		{
			Table("V_DbFolder");
			Lazy(true);
			Id(x => x.ID, map => map.Generator(Generators.Guid));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.FullPath);
			Property(x => x.StorageType, map => map.NotNullable(true));
			Property(x => x.StorageParameter);
			Property(x => x.Path);
			Property(x => x.SPMActionItemGUID, map => { map.NotNullable(true); map.Insert(false); map.Update(false); });
			Property(x => x.EnableVersioning, map => map.NotNullable(true));
			Property(x => x.Tag);
			Property(x => x.PublishDate);
			Property(x => x.CreatorID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));

			Property(x => x.Size, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserName, map => map.NotNullable(true)); 
			Property(x => x.FullPath);
			Property(x => x.Creator);
			Property(x => x.ParentFolderID);
			Property(x => x.IsDeleted, map => map.NotNullable(true));
		}
	}

	public class V_DbItemMap : ClassMapping<V_DbItem>
	{
		public V_DbItemMap()
		{
			Table("V_DbItem");
			Lazy(true);
			Id(x => x.ID, map => map.Generator(Generators.Guid));
			Property(x => x.Size, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Extension, map => map.NotNullable(true));
			Property(x => x.Path);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Path);
			Property(x => x.FullPath, map => { map.Insert(false); map.Update(false); });
			Property(x => x.Tag);
			Property(x => x.PublishDate);
			
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserName, map => map.NotNullable(true));
			Property(x => x.CheckedOutByID, map => { map.Insert(false); map.Update(false); });
			Property(x => x.CheckedOutBy, map => { map.Insert(false); map.Update(false); });
			Property(x => x.CreatorID, map => map.NotNullable(true));
			Property(x => x.Creator);

			Property(x => x.IsDeleted, map => { map.Insert(false); map.Update(false); });
			Property(x => x.EnableVersioning, map => { map.NotNullable(true); map.Insert(false); map.Update(false); });
			Property(x => x.SPMActionItemGUID, map => { map.Insert(false); map.Update(false); });

			Property(x => x.Type, map => { map.NotNullable(true); map.Type<int>(); });
		}
	}

	public class N_FileDataMap : ClassMapping<N_FileData>
	{
		public N_FileDataMap()
		{
			Table("N_FileData");
			Lazy(true);
			Id(x => x.FileGUID, map => map.Generator(Generators.Guid));
			Property(x => x.Data);
			Property(x => x.Extension);
		}
	}

	public class N_VirusScanLogMap : ClassMapping<N_VirusScanLog>
	{
		public N_VirusScanLogMap()
		{
			Table("N_VirusScanLog");

			Lazy(true);
			Id(x => x.VirusScanLogID, map => map.Generator(Generators.Identity));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.ResultCode, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}


	public class N_SettingMap : ClassMapping<N_Setting>
	{
		public N_SettingMap()
		{
			Table("N_Settings");
			Lazy(true);
			Id(x => x.SettingsGUID, map => map.Generator(Generators.Assigned));
			Property(x => x.SystemName, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Value, map => map.NotNullable(true));
			Property(x => x.IsSystem, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.AcceptableValues);
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}


	/*public class UserActivityMap : ClassMapping<UserActivity>
	{
		public UserActivityMap()
		{
			Lazy(true);
			Id(x => x.UserActivityID, map => map.Generator(Generators.Identity));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Title);
			Property(x => x.ObjectKey, map => map.NotNullable(true));
			Property(x => x.ObjectTypeSysName);
			Property(x => x.Action, map => map.NotNullable(true));
			Property(x => x.UserTitle, map => map.NotNullable(true));
			Property(x => x.ObjectTypeTitle, map => map.NotNullable(true));
			Property(x => x.IP, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}*/

	public class N_RssFeedMap : ClassMapping<N_RssFeed>
	{
		public N_RssFeedMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.RssFeedID, map => map.Generator(Generators.Identity));
			Property(x => x.Copyright, map => map.NotNullable(true));
			Property(x => x.Description, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.ObjectTypeSysName, map => map.NotNullable(true));
			Property(x => x.Predicate, map => map.NotNullable(true));
			Property(x => x.PubDate, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Ttl, map => map.NotNullable(true));
			Property(x => x.ViewFormSysName, map => map.NotNullable(true));
			Property(x => x.Author, map => map.NotNullable(true));
			Property(x => x.WebMaster, map => map.NotNullable(true));
			Property(x => x.LinkParams);
		}
	}

	public class MM_PackageMap : ClassMapping<MM_Package>
	{
		public MM_PackageMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.PackageID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.Version);
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.ParentPackageID);
		}
	}

	public class MM_ObjectTypeMap : ClassMapping<MM_ObjectType>
	{
		public MM_ObjectTypeMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.ObjectTypeID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsEnableSPM, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsSeparateTable, map => map.NotNullable(true));
			Property(x => x.IsTemplate, map => map.NotNullable(true));
			Property(x => x.TitlePlural);
			Property(x => x.DefaultOrderBy);
			Property(x => x.LogicalDelete);
			Property(x => x.IsReplicate, map => map.NotNullable(true));
			Property(x => x.IsEnableUserViews, map => map.NotNullable(true));
			Property(x => x.SecurityPackageSystemName);
			Property(x => x.IsEnableObjectHistory, map => map.NotNullable(true));
			Property(x => x.Interface);
			Property(x => x.HistoryTypeCode, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.Description);

			Property(x => x.PackageID);
			Property(x => x.BaseObjectTypeID);
		}
	}

	public class MM_ObjectPropertyMap : ClassMapping<MM_ObjectProperty>
	{

		public MM_ObjectPropertyMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.ObjectPropertyID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.TypeCode, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsMultilingual, map => map.NotNullable(true));
			Property(x => x.IsPrimaryKey, map => map.NotNullable(true));
			Property(x => x.IsSystem, map => map.NotNullable(true));
			Property(x => x.IsNavigable, map => map.NotNullable(true));
			Property(x => x.IsAggregate, map => map.NotNullable(true));
			Property(x => x.LowerBound, map => map.NotNullable(true));
			Property(x => x.UpperBound, map => map.NotNullable(true));
			Property(x => x.Expression);
			Property(x => x.IsReferenceToVersion, map => map.NotNullable(true));
			Property(x => x.ValueFilter);
			Property(x => x.Precision);
			Property(x => x.Scale);
			Property(x => x.Length);
			Property(x => x.DeleteRule, map => map.NotNullable(true));
			Property(x => x.KindCode, map => map.NotNullable(true));
			Property(x => x.DefaultDBValue);
			Property(x => x.Description);
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsIdentity, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID, map => { map.NotNullable(true);});
			Property(x => x.RefObjectPropertyID);
			Property(x => x.RefObjectTypeID);

		}
	}

	public class MM_FormViewMap : ClassMapping<MM_FormView>
	{
		public MM_FormViewMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FormViewID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.ViewTemplate);
			Property(x => x.TemplateTypeCode);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsCustom, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsCaching, map => map.NotNullable(true));
			Property(x => x.CacheKeyParams);
			Property(x => x.CacheTimeout, map => map.NotNullable(true));
			Property(x => x.BaseClass, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID);
			Property(x => x.PackageID);
		}
	}

	public class N_CacheMap : ClassMapping<N_Cache>
	{
		public N_CacheMap()
		{
			Schema("dbo");
			Lazy(true);
			Property(x => x.TimeStamp, map => map.NotNullable(true));
		}
	}
}