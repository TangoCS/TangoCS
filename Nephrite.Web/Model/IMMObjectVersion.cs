using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web
{
    public interface IMMObjectVersion : IModelObject, IWithTimeStamp
    {
        int VersionID { get; }
		Guid VersionGUID { get; }
		IModelObject Object { get; }
        int VersionNumber { get; }
        bool IsCurrentVersion { get; }
    }

	public interface IMMObjectVersion2 : IMMObjectVersion
	{
		int ClassVersionID { get; set; }
	}
}
