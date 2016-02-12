using System;
using System.Collections.Generic;
using Nephrite.Controls;
using Nephrite.Data;
using Nephrite.EntityAudit;
using Nephrite.ErrorLog;
using Nephrite.FileStorage;
using Nephrite.UI.Controls;
using Nephrite.RSS;
using Nephrite.Hibernate;
using Nephrite.Hibernate.CoreMapping;
//using Nephrite.Web.Mailer;
//using Nephrite.Web.TaskManager;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Linq;
using System.Data;

namespace Nephrite.Web.CoreDataContext
{

	public class HCoreDataContext : HDataContext,  IDC_ErrorLog,
		IDC_ListFilter, IDC_FileStorage, IDC_CalendarDays, 
		//IDC_Mailer,
		//IDC_TaskManager, 
		IDC_RSS, IDC_EntityAudit
	{
		public HCoreDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig, IDbConnection connection)
			: base(dbConfig, connection)
		{
		}

		public static Action<IDbIntegrationConfigurationProperties> DefaultDBConfig(string connectionString, DBType? dbType = null)
		{
			return c =>
			{
				if (dbType == null) dbType = ConnectionManager.DBType;
				switch (dbType)
				{
					case DBType.MSSQL: c.Dialect<MsSql2008Dialect>(); break;
					case DBType.DB2: c.Dialect<DB2Dialect>(); break;
					case DBType.ORACLE: c.Dialect<Oracle10gDialect>(); break;
					case DBType.POSTGRESQL: c.Dialect<PostgreSQL82Dialect>(); break;
					default: c.Dialect<MsSql2008Dialect>(); break;
				}

				c.ConnectionString = connectionString;
				c.KeywordsAutoImport = Hbm2DDLKeyWords.None;
				c.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
				//c.LogFormattedSql = true;
				if (!System.Configuration.ConfigurationManager.AppSettings["ValidateSchema"].IsEmpty())
					c.SchemaAction = SchemaAutoAction.Validate;
			};
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(IErrorLogMap));
			l.Add(typeof(ICalendarDayMap));
			//l.Add(typeof(IMailMessageMap));
			//l.Add(typeof(IMailTemplateMap));
			//l.Add(typeof(IN_TextResourceMap));
			//l.Add(typeof(IC_LanguageMap));
			l.Add(typeof(IN_VirusScanLogMap));
			//l.Add(typeof(IDbFolderMap));
			//l.Add(typeof(IDbItemMap));
			//l.Add(typeof(IDbFileMap));
			//l.Add(typeof(IDbFileDataMap));
			l.Add(typeof(IN_DownloadLogMap));
			//l.Add(typeof(ITM_TaskMap));
			//l.Add(typeof(ITM_TaskExecutionMap));
			//l.Add(typeof(ITM_TaskParameterMap));
			l.Add(typeof(IN_FilterMap));
			//l.Add(typeof(IN_SettingsMap));
			l.Add(typeof(IN_ObjectChangeMap));
			l.Add(typeof(IN_ObjectPropertyChangeMap));
			l.Add(typeof(IN_RssFeedMap));

			l.Add(typeof(IErrorLogImplMap));
			l.Add(typeof(ICalendarDayImplMap));
			//l.Add(typeof(IMailMessageImplMap));
			//l.Add(typeof(IMailTemplateImplMap));
			//l.Add(typeof(IC_LanguageImplMap));
			//l.Add(typeof(IN_TextResourceImplMap));
			l.Add(typeof(IN_FilterImplMap));
			//l.Add(typeof(IN_SettingsImplMap));
			l.Add(typeof(IN_ObjectChangeImplMap));
			l.Add(typeof(IN_ObjectPropertyChangeImplMap));
			l.Add(typeof(IN_RssFeedImplMap));
			l.Add(typeof(IN_DownloadLogImplMap));
			//l.Add(typeof(IDbFileImplMap));
			//l.Add(typeof(IDbFolderImplMap));
			//l.Add(typeof(IDbItemImplMap));
			l.Add(typeof(IDbFileDataImplMap));
			l.Add(typeof(IN_VirusScanLogImplMap));
			//l.Add(typeof(ITM_TaskParameterImplMap));
			//l.Add(typeof(ITM_TaskExecutionImplMap));
			//l.Add(typeof(ITM_TaskImplMap));


			return l;
		}

		public IN_DownloadLog NewIN_DownloadLog()
		{
			return new N_DownloadLog();
		}

		public IN_VirusScanLog NewIN_VirusScanLog()
		{
			return new N_VirusScanLog();
		}

		//public IDbFolder NewIDbFolder(Guid id)
		//{
		//	return new V_DbFolder { ID = id };
		//}

		//public IDbFile NewIDbFile(Guid id)
		//{
		//	return new V_DbFile { ID = id };
		//}

		public IDbFileData NewIDbFileData(Guid id)
		{
			return new N_FileData { FileGUID = id };
		}

		public IN_Filter NewIN_Filter()
		{
			return new N_Filter();
		}

		//public IMailMessage NewIMailMessage()
		//{
		//	return new MailMessage();
		//}

		//public ITM_TaskExecution NewITM_TaskExecution()
		//{
		//	return new TM_TaskExecution();
		//}

		

		public IN_ObjectChange NewIN_ObjectChange()
		{
			return new N_ObjectChange();
		}

		public IN_ObjectPropertyChange NewIN_ObjectPropertyChange()
		{
			return new N_ObjectPropertyChange();
		}

		public IErrorLog NewIErrorLog()
		{
			return new ErrorLog();
		}

		public ITable<IErrorLog> IErrorLog
		{
			get { return new HTable<IErrorLog>(this, Session.Query<IErrorLog>()); }
		}
		public ITable<ICalendarDay> ICalendarDay
		{
			get { return new HTable<ICalendarDay>(this, Session.Query<ICalendarDay>()); }
		}
		//public ITable<IMailMessage> IMailMessage
		//{
		//	get { return new HTable<IMailMessage>(this, Session.Query<IMailMessage>()); }
		//}
		//public ITable<IMailTemplate> IMailTemplate
		//{
		//	get { return new HTable<IMailTemplate>(this, Session.Query<IMailTemplate>()); }
		//}

		//public ITable<ILanguageObject> IC_Language
		//{
		//	get { return new HTable<ILanguageObject>(this, Session.Query<ILanguageObject>()); }
		//}
		//public ITable<IN_TextResource> IN_TextResource
		//{
		//	get { return new HTable<IN_TextResource>(this, Session.Query<IN_TextResource>()); }
		//}
		public ITable<IN_Filter> IN_Filter
		{
			get { return new HTable<IN_Filter>(this, Session.Query<IN_Filter>()); }
		}
		//public ITable<IN_Settings> IN_Settings
		//{
		//	get { return new HTable<IN_Settings>(this, Session.Query<IN_Settings>()); }
		//}
		public ITable<IN_ObjectChange> IN_ObjectChange
		{
			get { return new HTable<IN_ObjectChange>(this, Session.Query<IN_ObjectChange>()); }
		}
		public ITable<IN_ObjectPropertyChange> IN_ObjectPropertyChange
		{
			get { return new HTable<IN_ObjectPropertyChange>(this, Session.Query<IN_ObjectPropertyChange>()); }
		}
		public ITable<IN_RssFeed> IN_RssFeed
		{
			get { return new HTable<IN_RssFeed>(this, Session.Query<IN_RssFeed>()); }
		}
		public ITable<IN_DownloadLog> IN_DownloadLog
		{
			get { return new HTable<IN_DownloadLog>(this, Session.Query<IN_DownloadLog>()); }
		}
		//public ITable<IDbFile> IDbFile
		//{
		//	get { return new HTable<IDbFile>(this, Session.Query<IDbFile>()); }
		//}
		//public ITable<IDbFolder> IDbFolder
		//{
		//	get { return new HTable<IDbFolder>(this, Session.Query<IDbFolder>()); }
		//}
		//public ITable<IDbItem> IDbItem
		//{
		//	get { return new HTable<IDbItem>(this, Session.Query<IDbItem>()); }
		//}
		public ITable<IDbFileData> IDbFileData
		{
			get { return new HTable<IDbFileData>(this, Session.Query<IDbFileData>()); }
		}
		public ITable<IN_VirusScanLog> IN_VirusScanLog
		{
			get { return new HTable<IN_VirusScanLog>(this, Session.Query<IN_VirusScanLog>()); }
		}
		//public ITable<ITM_TaskParameter> ITM_TaskParameter
		//{
		//	get { return new HTable<ITM_TaskParameter>(this, Session.Query<ITM_TaskParameter>()); }
		//}
		//public ITable<ITM_TaskExecution> ITM_TaskExecution
		//{
		//	get { return new HTable<ITM_TaskExecution>(this, Session.Query<ITM_TaskExecution>()); }
		//}
		//public ITable<ITM_Task> ITM_Task
		//{
		//	get { return new HTable<ITM_Task>(this, Session.Query<ITM_Task>()); }
		//}



		//public override IDataContext NewDataContext()
		//{
		//	return new HCoreDataContext(DefaultDBConfig(ConnectionManager.ConnectionString), null);
		//}

		//public override IDataContext NewDataContext(string connectionString)
		//{
		//	return new HCoreDataContext(DefaultDBConfig(connectionString), null);
		//}
	}

}