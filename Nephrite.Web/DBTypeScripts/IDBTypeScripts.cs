using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.DBTypeScripts
{
	public interface IDBTypeScripts
	{
		string FromLogin { get; }
		string FromSID { get; }
		string FromID { get; }
		string FromEmail { get; }
		string Menu { get; }
	}
}