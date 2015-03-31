using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.SettingsManager
{
	public interface IDC_Settings : IDataContext
	{
		ITable<IN_Settings> IN_Settings { get; }
		IN_Settings NewIN_Settings();
	}

	public interface IN_Settings : IEntity
	{
		Guid SettingsGUID { get; set; }
		int LastModifiedUserID { get; set; }
		string SystemName { get; set; }
		string Title { get; set; }
		string Value { get; set; }
		bool IsSystem { get; set; }
		bool IsDeleted { get; set; }
		DateTime LastModifiedDate { get; set; }
		string AcceptableValues { get; set; }
		int? GroupID { get; set; }
	}
}