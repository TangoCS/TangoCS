using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.DBTypeScripts
{
	public interface IDBTypeScripts
	{
		string FromLogin { get; }
		string FromSID { get; }
		string FromID { get; }
		string FromEmail { get; }
		string Menu { get; }
		string GetRolesAccessByIdQuery { get; }
		string GetRolesAccessByNameQuery { get; }
		string GetItemsIdsQuery { get; }
		string GetItemsNamesQuery { get; }
        string F_ReportRecipientsGPO { get; }


	}
}