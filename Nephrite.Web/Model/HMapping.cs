using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Nephrite.Web.Model
{
	/*
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
			Id(x => x.LanguageCode, map => { map.Generator(Generators.Assigned); map.Length(2);});
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsDefault, map => map.NotNullable(true));
		}
	}

	public class N_TextResourceMap : ClassMapping<N_TextResource>
	{
		public N_TextResourceMap()
		{
			Table("N_TextResource");

			Lazy(true);
			Id(x => x.TextResourceID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Bag(x => x.N_TextResourceDatas, colmap => { colmap.Key(x => x.Column("TextResourceID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
		}
	}

	public class N_TextResourceDataMap : ClassMapping<N_TextResourceData>
	{
		public N_TextResourceDataMap()
		{
			Table("N_TextResourceData");
			Lazy(true);
			Id(x => x.N_TextResourceDataID, map => map.Generator(Generators.Identity));
			Property(x => x.Text);

			ManyToOne(x => x.N_TextResource, map => { map.Column("TextResourceID"); map.Cascade(Cascade.None); map.ForeignKey("FK_N_TextResourceData_N_TextResource"); });
			ManyToOne(x => x.C_Language, map => { map.Column("LanguageCode"); map.Cascade(Cascade.None); map.ForeignKey("FK_N_TextResourceData_C_Language"); });

			Property(x => x.LanguageCode, map => { map.Formula("TextResourceID"); map.NotNullable(true); });
			Property(x => x.TextResourceID, map => { map.Formula("LanguageCode"); map.NotNullable(true); });
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
			ManyToOne(x => x.TM_Task, map => { map.Column("ParentID"); map.Cascade(Cascade.None); map.ForeignKey("FK_TM_TaskParameter_Parent"); });
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
			ManyToOne(x => x.TM_Task, map => { map.Column("TaskID"); map.Cascade(Cascade.None); map.ForeignKey("FK_TM_TaskExecution_Task"); });
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
			Bag(x => x.TM_TaskExecutions, colmap => { colmap.Key(x => x.Column("TaskID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.TM_TaskParameters, colmap => { colmap.Key(x => x.Column("ParentID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
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
			Property(x => x.FileID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}

	public class N_FileMap : ClassMapping<N_File>
	{
		public N_FileMap()
		{
			Table("N_File");
			Lazy(true);
			Id(x => x.FileID, map => map.Generator(Generators.Identity));
			Property(x => x.Length, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDiskStorage, map => map.NotNullable(true));
			Property(x => x.Extension, map => map.NotNullable(true));
			Property(x => x.Path);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.StorageType, map => map.NotNullable(true));
			Property(x => x.StorageParameter);
			Property(x => x.Password);
			Property(x => x.GuidPath);
			Property(x => x.FeatureGUID);
			Property(x => x.BeginDate, map => map.NotNullable(true));
			Property(x => x.EndDate, map => map.NotNullable(true));
			Property(x => x.VersionNumber, map => map.NotNullable(true));
			Property(x => x.MainGUID);
			Property(x => x.Tag);
			Property(x => x.PublishDate);

			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.CheckedOutByID);
			Property(x => x.CreatorID, map => map.NotNullable(true));

			ManyToOne(x => x.N_Folder, map =>
			{
				map.Column("FolderID");
				//map.PropertyRef("FolderID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
				map.ForeignKey("FK_N_File_Folder");
			});
		}
	}

	public class N_FolderMap : ClassMapping<N_Folder>
	{
		public N_FolderMap()
		{
			Table("N_Folder");
			Lazy(true);
			Id(x => x.FolderID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.FullPath);
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.StorageType, map => map.NotNullable(true));
			Property(x => x.StorageParameter);
			Property(x => x.GuidPath);
			Property(x => x.IsReplicable, map => map.NotNullable(true));
			Property(x => x.SPMActionItemGUID, map => map.NotNullable(true));
			Property(x => x.EnableVersioning, map => map.NotNullable(true));
			Property(x => x.Tag);
			Property(x => x.PublishDate);
			Property(x => x.CreatorID, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
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

	public class N_TableInfoMap : ClassMapping<N_TableInfo>
	{

		public N_TableInfoMap()
		{
			Table("N_TableInfo");
			Lazy(true);
			Id(x => x.TableName, map => map.Generator(Generators.Assigned));
			Property(x => x.LastDataModify, map => map.NotNullable(true));
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

	public class N_CacheMap : ClassMapping<N_Cache>
	{
		public N_CacheMap()
		{
			Lazy(true);
			Id(x => x.TimeStamp, map => map.Generator(Generators.Assigned));
		}
	}

	public class UserActivityMap : ClassMapping<UserActivity>
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
	}

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
	 */
}