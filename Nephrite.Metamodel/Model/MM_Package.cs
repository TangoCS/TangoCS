using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel.Model
{
    partial class MM_Package : IMM_ModelElement, IMovableObject
    {
        public string Icon
        {
            get { return "icon_package.gif"; }
        }

        public int ID
        {
            get { return PackageID; }
        }

        public int Level { get; set; }

        public string ClassName { get { return "Пакет"; } }

        public string FullSysName
        {
            get
            {
                if (ParentPackageID.HasValue)
                    return MM_Package1.FullSysName + "." + SysName;
                return SysName;
            }
        }

        public string ControlPath
        {
            get
            {
                return (ParentPackageID.HasValue ? MM_Package1.ControlPath + "/" : "") + SysName;
            }
        }

		public string ElementSysName
		{
			get { return SysName; }
		}
    }
}
