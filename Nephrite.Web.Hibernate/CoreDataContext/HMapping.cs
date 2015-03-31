using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Nephrite.Web.Hibernate;
using Nephrite.Web.TaskManager;

namespace Nephrite.Web.CoreDataContext
{
	public class IErrorLogImplMap : SubclassMapping<ErrorLog>
	{
		public IErrorLogImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class ICalendarDayImplMap : SubclassMapping<CalendarDay>
	{
		public ICalendarDayImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IMailMessageImplMap : SubclassMapping<MailMessage>
	{
		public IMailMessageImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IMailTemplateImplMap : SubclassMapping<MailTemplate>
	{
		public IMailTemplateImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_TimeZoneImplMap : SubclassMapping<N_TimeZone>
	{
		public IN_TimeZoneImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IC_LanguageImplMap : SubclassMapping<C_Language>
	{
		public IC_LanguageImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_TextResourceImplMap : SubclassMapping<V_N_TextResource>
	{
		public IN_TextResourceImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_FilterImplMap : SubclassMapping<N_Filter>
	{
		public IN_FilterImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_SettingsImplMap : SubclassMapping<N_Settings>
	{
		public IN_SettingsImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_ObjectChangeImplMap : SubclassMapping<N_ObjectChange>
	{
		public IN_ObjectChangeImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_ObjectPropertyChangeImplMap : SubclassMapping<N_ObjectPropertyChange>
	{
		public IN_ObjectPropertyChangeImplMap()
		{
			DiscriminatorValue("0");
		}
	}
	public class IN_RssFeedImplMap : SubclassMapping<N_RssFeed>
	{
		public IN_RssFeedImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IN_DownloadLogImplMap : SubclassMapping<N_DownloadLog>
	{
		public IN_DownloadLogImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IDbFileImplMap : SubclassMapping<V_DbFile>
	{
		public IDbFileImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IDbFolderImplMap : SubclassMapping<V_DbFolder>
	{
		public IDbFolderImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IDbItemImplMap : SubclassMapping<V_DbItem>
	{
		public IDbItemImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IDbFileDataImplMap : SubclassMapping<N_FileData>
	{
		public IDbFileDataImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class IN_VirusScanLogImplMap : SubclassMapping<N_VirusScanLog>
	{
		public IN_VirusScanLogImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class ITM_TaskParameterImplMap : SubclassMapping<TM_TaskParameter>
	{
		public ITM_TaskParameterImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class ITM_TaskExecutionImplMap : SubclassMapping<TM_TaskExecution>
	{
		public ITM_TaskExecutionImplMap()
		{
			DiscriminatorValue("0");
		}
	}

	public class ITM_TaskImplMap : SubclassMapping<TM_Task>
	{
		public ITM_TaskImplMap()
		{
			DiscriminatorValue("0");
		}
	}


}