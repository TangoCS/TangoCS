using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
    public interface IMMObjectVersion : IMMObject
    {
        int VersionID { get; }
		Guid VersionGUID { get; }
        IMMObject Object { get; }
        int VersionNumber { get; }
        bool IsCurrentVersion { get; }
    }

	public interface IMMObjectVersion2 : IMMObjectVersion
	{
		int ClassVersionID { get; set; }
	}
}
