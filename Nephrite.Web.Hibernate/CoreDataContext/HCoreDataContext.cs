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
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Linq;
using System.Data;
using Nephrite.Logger;
using Nephrite.FileStorage.Std;
using System.Linq;

namespace Nephrite.Web.CoreDataContext
{

	//public class HCoreDataContext : HDataContext,  IDC_ErrorLog,
	//	IDC_ListFilter, IDC_FileDataStorage, IDC_CalendarDays, IDC_RSS, IDC_EntityAudit
	//{
	//	public HCoreDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig, IDbConnection connection, IRequestLoggerProvider loggerProvider)
	//		: base(dbConfig, connection, loggerProvider)
	//	{
	//	}

	//	public static Action<IDbIntegrationConfigurationProperties> DefaultDBConfig(string connectionString, DBType? dbType = null)
	//	{
	//		return c =>
	//		{
	//			if (dbType == null) dbType = ConnectionManager.DBType;
	//			switch (dbType)
	//			{
	//				case DBType.MSSQL: c.Dialect<MsSql2008Dialect>(); break;
	//				case DBType.DB2: c.Dialect<DB2Dialect>(); break;
	//				case DBType.ORACLE: c.Dialect<Oracle10gDialect>(); break;
	//				case DBType.POSTGRESQL: c.Dialect<PostgreSQL82Dialect>(); break;
	//				default: c.Dialect<MsSql2008Dialect>(); break;
	//			}

	//			c.ConnectionString = connectionString;
	//			c.KeywordsAutoImport = Hbm2DDLKeyWords.None;
	//			c.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
	//			if (!System.Configuration.ConfigurationManager.AppSettings["ValidateSchema"].IsEmpty())
	//				c.SchemaAction = SchemaAutoAction.Validate;
	//		};
	//	}

	//	public override IEnumerable<Type> GetEntitiesTypes()
	//	{
	//		List<Type> l = new List<Type>();
	//		l.Add(typeof(IErrorLogMap));
	//		l.Add(typeof(ICalendarDayMap));
	//		l.Add(typeof(IN_VirusScanLogMap));
	//		l.Add(typeof(IN_DownloadLogMap));
	//		l.Add(typeof(IN_FilterMap));
	//		l.Add(typeof(IN_ObjectChangeMap));
	//		l.Add(typeof(IN_ObjectPropertyChangeMap));
	//		l.Add(typeof(IN_RssFeedMap));
	//		l.Add(typeof(IErrorLogImplMap));
	//		l.Add(typeof(ICalendarDayImplMap));
	//		l.Add(typeof(IN_FilterImplMap));
	//		l.Add(typeof(IN_ObjectChangeImplMap));
	//		l.Add(typeof(IN_ObjectPropertyChangeImplMap));
	//		l.Add(typeof(IN_RssFeedImplMap));
	//		l.Add(typeof(IN_DownloadLogImplMap));
	//		l.Add(typeof(IDbFileDataImplMap));
	//		l.Add(typeof(IN_VirusScanLogImplMap));
	//		return l;
	//	}

	//	public IN_DownloadLog NewIN_DownloadLog()
	//	{
	//		return new N_DownloadLog();
	//	}

	//	public IN_VirusScanLog NewIN_VirusScanLog()
	//	{
	//		return new N_VirusScanLog();
	//	}

	//	public IDbFileData NewIDbFileData(Guid id)
	//	{
	//		return new N_FileData { FileGUID = id };
	//	}

	//	public IN_Filter NewIN_Filter()
	//	{
	//		return new N_Filter();
	//	}

	//	public IN_ObjectChange NewIN_ObjectChange()
	//	{
	//		return new N_ObjectChange();
	//	}

	//	public IN_ObjectPropertyChange NewIN_ObjectPropertyChange()
	//	{
	//		return new N_ObjectPropertyChange();
	//	}

	//	public IErrorLog NewIErrorLog()
	//	{
	//		return new ErrorLog();
	//	}

	//	public IQueryable<IErrorLog> IErrorLog
	//	{
	//		get { return Session.Query<IErrorLog>(); }
	//	}
	//	public IQueryable<ICalendarDay> ICalendarDay
	//	{
	//		get { return Session.Query<ICalendarDay>(); }
	//	}

	//	public IQueryable<IN_Filter> IN_Filter
	//	{
	//		get { return Session.Query<IN_Filter>(); }
	//	}
	//	public IQueryable<IN_ObjectChange> IN_ObjectChange
	//	{
	//		get { return Session.Query<IN_ObjectChange>(); }
	//	}
	//	public IQueryable<IN_ObjectPropertyChange> IN_ObjectPropertyChange
	//	{
	//		get { return Session.Query<IN_ObjectPropertyChange>(); }
	//	}
	//	public IQueryable<IN_RssFeed> IN_RssFeed
	//	{
	//		get { return Session.Query<IN_RssFeed>(); }
	//	}
	//	public IQueryable<IN_DownloadLog> IN_DownloadLog
	//	{
	//		get { return Session.Query<IN_DownloadLog>(); }
	//	}

	//	public IQueryable<IDbFileData> IDbFileData
	//	{
	//		get { return Session.Query<IDbFileData>(); }
	//	}
	//	public IQueryable<IN_VirusScanLog> IN_VirusScanLog
	//	{
	//		get { return Session.Query<IN_VirusScanLog>(); }
	//	}
	//}
}