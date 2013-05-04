using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web
{
	public enum FileStorageType
	{
		LocalDatabase,
		RemoteDatabase,
		FileSystem,
		External
	}
}