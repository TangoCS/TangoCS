using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nephrite.Web.CalendarDays;
using Nephrite.Web.Controls;
using Nephrite.Web.FileStorage;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using Nephrite.Web.Mailer;
using Nephrite.Web.TextResources;
using Nephrite.Web.Multilanguage;
using Nephrite.Web.TaskManager;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.UserActivity;
using Nephrite.Web.RSS;
using Nephrite.Web.Hibernate;

namespace Nephrite.Web.CoreDataContext
{

	public class HCoreDataContext : HDataContext, 
		IDC_TimeZone, IDC_ListFilter, IDC_FileStorage, IDC_CalendarDays, IDC_Mailer,
		IDC_TextResources, IDC_Multilanguage, IDC_TaskManager, IDC_Settings, IDC_RSS
	{
		public HCoreDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
			: base(dbConfig)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(N_TimeZoneMap));
			l.Add(typeof(N_VirusScanLogMap));
			l.Add(typeof(V_DbFolderMap));
			l.Add(typeof(V_DbFileMap));
			l.Add(typeof(N_FileDataMap));
			l.Add(typeof(CalendarDayMap));
			l.Add(typeof(MailMessageMap));
			l.Add(typeof(N_DownloadLogMap));
			l.Add(typeof(V_N_TextResourceMap));
			l.Add(typeof(C_LanguageMap));
			l.Add(typeof(TM_TaskMap));
			l.Add(typeof(TM_TaskExecutionMap));
			l.Add(typeof(TM_TaskParameterMap));
			l.Add(typeof(N_FilterMap));
			l.Add(typeof(N_SettingMap));
			//l.Add(typeof(UserActivityMap));
			l.Add(typeof(N_RssFeed));
			return l;
		}

		public IQueryable<IN_TimeZone> N_TimeZone
		{
			get { return new HTable<IN_TimeZone>(this, Session.Query<N_TimeZone>().Cast<IN_TimeZone>()); }
		}

		public IQueryable<IN_Filter> N_Filter
		{
			get { return new HTable<IN_Filter>(this, Session.Query<N_Filter>().Cast<IN_Filter>()); }
		}

		public IN_Filter NewN_Filter()
		{
			return new N_Filter();
		}

		public IQueryable<IN_DownloadLog> N_DownloadLog
		{
			get { return new HTable<IN_DownloadLog>(this, Session.Query<N_DownloadLog>().Cast<IN_DownloadLog>()); }
		}

		public IQueryable<IN_VirusScanLog> N_VirusScanLog
		{
			get { return new HTable<IN_VirusScanLog>(this, Session.Query<N_VirusScanLog>().Cast<IN_VirusScanLog>()); }
		}

		public IN_DownloadLog NewN_DownloadLog()
		{
			return new N_DownloadLog();
		}

		public IN_VirusScanLog NewN_VirusScanLog()
		{
			return new N_VirusScanLog();
		}

		public IDbFolder NewDbFolder()
		{
			return new V_DbFolder();
		}

		public IDbFile NewDbFile()
		{
			return new V_DbFile();
		}

		public IDbFileData NewDbFileData()
		{
			return new N_FileData();
		}

		public IQueryable<IDbFile> DbFile
		{
			get { return new HTable<IDbFile>(this, Session.Query<V_DbFile>().Cast<IDbFile>()); }
		}

		public IQueryable<IDbFolder> DbFolder
		{
			get { return new HTable<IDbFolder>(this, Session.Query<V_DbFolder>().Cast<IDbFolder>()); }
		}

		public IQueryable<IDbItem> DbItem
		{
			get { return new HTable<IDbItem>(this, Session.Query<V_DbItem>().Cast<IDbItem>()); }
		}

		public IQueryable<IDbFileData> DbFileData
		{
			get { return new HTable<IDbFileData>(this, Session.Query<N_FileData>().Cast<IDbFileData>()); }
		}

		public IQueryable<ICalendarDay> CalendarDay
		{
			get { return new HTable<ICalendarDay>(this, Session.Query<CalendarDay>().Cast<ICalendarDay>()); }
		}

		public IQueryable<IMailMessage> MailMessage
		{
			get { return new HTable<IMailMessage>(this, Session.Query<MailMessage>().Cast<IMailMessage>()); }
		}

		public IQueryable<IMailTemplate> MailTemplate
		{
			get { return new HTable<IMailTemplate>(this, Session.Query<MailTemplate>().Cast<IMailTemplate>()); }
		}

		public IMailMessage NewMailMessage()
		{
			return new MailMessage();
		}

		public IQueryable<IV_N_TextResource> V_N_TextResource
		{
			get { return new HTable<IV_N_TextResource>(this, Session.Query<V_N_TextResource>().Cast<IV_N_TextResource>()); }
		}

		public IQueryable<IC_Language> C_Language
		{
			get { return new HTable<IC_Language>(this, Session.Query<C_Language>().Cast<IC_Language>()); }
		}

		public IQueryable<ITM_Task> TM_Task
		{
			get { return new HTable<ITM_Task>(this, Session.Query<TM_Task>().Cast<ITM_Task>()); }
		}

		public IQueryable<ITM_TaskExecution> TM_TaskExecution
		{
			get { return new HTable<ITM_TaskExecution>(this, Session.Query<TM_TaskExecution>().Cast<ITM_TaskExecution>()); }
		}

		public IQueryable<ITM_TaskParameter> TM_TaskParameter
		{
			get { return new HTable<ITM_TaskParameter>(this, Session.Query<TM_TaskParameter>().Cast<ITM_TaskParameter>()); }
		}

		public ITM_TaskExecution NewTM_TaskExecution()
		{
			return new TM_TaskExecution();
		}

		public IQueryable<IN_Setting> N_Setting
		{
			get { return new HTable<IN_Setting>(this, Session.Query<N_Setting>().Cast<IN_Setting>()); }
		}

		public IN_Setting NewN_Setting()
		{
			return new N_Setting();
		}

		/*public IQueryable<IUserActivity> UserActivity
		{
			get { return new HTable<IUserActivity>(this, Session.Query<UserActivity>().Cast<IUserActivity>()); }
		}

		public IUserActivity NewUserActivity()
		{
			return new UserActivity();
		}*/

		public IQueryable<IN_RssFeed> N_RssFeed
		{
			get { return new HTable<IN_RssFeed>(this, Session.Query<N_RssFeed>().Cast<IN_RssFeed>()); }
		}

		public override IDataContext NewDataContext()
		{
			return new HCoreDataContext(DBConfig(ConnectionManager.ConnectionString));
		}

		public override IDataContext NewDataContext(string connectionString)
		{
			return new HCoreDataContext(DBConfig(connectionString));
		}
	}

}