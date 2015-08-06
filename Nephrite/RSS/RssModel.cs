using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Data;

namespace Nephrite.RSS
{
	public interface IDC_RSS : IDataContext
	{
		ITable<IN_RssFeed> IN_RssFeed { get; } 
	}


	public interface IN_RssFeed
	{
		int RssFeedID { get; set; }
		string Copyright { get; set; }
		string Description { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
		string ObjectTypeSysName { get; set; }
		string Predicate { get; set; }
		string PubDate { get; set; }
		string SysName { get; set; }
		string Title { get; set; }
		int Ttl { get; set; }
		string ViewFormSysName { get; set; }
		string Author { get; set; }
		string WebMaster { get; set; }
		string LinkParams { get; set; }
	}
}