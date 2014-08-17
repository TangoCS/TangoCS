using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Nephrite.Web.SettingsManager
{
	public interface IDC_Settings : IDataContext
	{
		IQueryable<IN_Settings> IN_Settings { get; }
		IN_Settings NewIN_Settings();
	}

	public interface IN_Settings : IEntity
	{
		System.Guid SettingsGUID { get; set; }
		int LastModifiedUserID { get; set; }
		string SystemName { get; set; }
		string Title { get; set; }
		string Value { get; set; }
		bool IsSystem { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string AcceptableValues { get; set; }
	}
}