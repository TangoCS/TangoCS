using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Nephrite.Web.SettingsManager
{
	public interface IDC_Settings : IDataContext
	{
		IQueryable<IN_Setting> IN_Setting { get; }
		IN_Setting NewIN_Setting();
	}

	public interface IN_Setting : IEntity
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