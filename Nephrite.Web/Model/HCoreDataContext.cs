using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;
using NHibernate.SqlCommand;

namespace Nephrite.Web.Model
{
	public class HCoreDataContext : HDataContext
	{
		public HCoreDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
			: base(dbConfig)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(MailTemplateMap));
			l.Add(typeof(HST_N_TimeZoneMap));
			l.Add(typeof(N_TimeZoneMap));
			l.Add(typeof(N_VirusScanLogMap));
			l.Add(typeof(N_FolderMap));
			l.Add(typeof(N_FileMap));
			l.Add(typeof(CalendarDayMap));
			l.Add(typeof(MailMessageMap));
			l.Add(typeof(N_DownloadLogMap));
			l.Add(typeof(N_TextResourceMap));
			l.Add(typeof(N_TextResourceDataMap));
			l.Add(typeof(C_LanguageMap));
			l.Add(typeof(N_TableInfoMap));
			l.Add(typeof(TM_TaskMap));
			l.Add(typeof(TM_TaskExecutionMap));
			l.Add(typeof(TM_TaskParameterMap));
			l.Add(typeof(N_FilterMap));
			l.Add(typeof(N_SettingMap));
			l.Add(typeof(N_CacheMap));
			l.Add(typeof(UserActivityMap));
			return l;
		}

		public HTable<MailTemplate> MailTemplates
		{
			get
			{
				return new HTable<MailTemplate>(this, Session.Query<MailTemplate>());
			}
		}
		public HTable<HST_N_TimeZone> HST_N_TimeZones
		{
			get
			{
				return new HTable<HST_N_TimeZone>(this, Session.Query<HST_N_TimeZone>());
			}
		}
		public HTable<N_TimeZone> N_TimeZones
		{
			get
			{
				return new HTable<N_TimeZone>(this, Session.Query<N_TimeZone>());
			}
		}
		public HTable<N_VirusScanLog> N_VirusScanLogs
		{
			get
			{
				return new HTable<N_VirusScanLog>(this, Session.Query<N_VirusScanLog>());
			}
		}
		public HTable<N_Folder> N_Folders
		{
			get
			{
				return new HTable<N_Folder>(this, Session.Query<N_Folder>());
			}
		}
		public HTable<N_File> N_Files
		{
			get
			{
				return new HTable<N_File>(this, Session.Query<N_File>());
			}
		}
		public HTable<CalendarDay> CalendarDays
		{
			get
			{
				return new HTable<CalendarDay>(this, Session.Query<CalendarDay>());
			}
		}
		public HTable<MailMessage> MailMessages
		{
			get
			{
				return new HTable<MailMessage>(this, Session.Query<MailMessage>());
			}
		}
		public HTable<N_DownloadLog> N_DownloadLogs
		{
			get
			{
				return new HTable<N_DownloadLog>(this, Session.Query<N_DownloadLog>());
			}
		}
		public HTable<N_TextResource> N_TextResources
		{
			get
			{
				return new HTable<N_TextResource>(this, Session.Query<N_TextResource>());
			}
		}
		public HTable<N_TextResourceData> N_TextResourceDatas
		{
			get
			{
				return new HTable<N_TextResourceData>(this, Session.Query<N_TextResourceData>());
			}
		}
		public HTable<C_Language> C_Languages
		{
			get
			{
				return new HTable<C_Language>(this, Session.Query<C_Language>());
			}
		}
		public HTable<N_TableInfo> N_TableInfos
		{
			get
			{
				return new HTable<N_TableInfo>(this, Session.Query<N_TableInfo>());
			}
		}
		public HTable<TM_Task> TM_Tasks
		{
			get
			{
				return new HTable<TM_Task>(this, Session.Query<TM_Task>());
			}
		}
		public HTable<TM_TaskExecution> TM_TaskExecutions
		{
			get
			{
				return new HTable<TM_TaskExecution>(this, Session.Query<TM_TaskExecution>());
			}
		}
		public HTable<TM_TaskParameter> TM_TaskParameters
		{
			get
			{
				return new HTable<TM_TaskParameter>(this, Session.Query<TM_TaskParameter>());
			}
		}
		public HTable<N_Filter> N_Filters
		{
			get
			{
				return new HTable<N_Filter>(this, Session.Query<N_Filter>());
			}
		}
		public HTable<N_Setting> N_Settings
		{
			get
			{
				return new HTable<N_Setting>(this, Session.Query<N_Setting>());
			}
		}
		public HTable<N_Cache> N_Caches
		{
			get
			{
				return new HTable<N_Cache>(this, Session.Query<N_Cache>());
			}
		}
		public HTable<UserActivity> UserActivities
		{
			get
			{
				return new HTable<UserActivity>(this, Session.Query<UserActivity>());
			}
		}
	}
}