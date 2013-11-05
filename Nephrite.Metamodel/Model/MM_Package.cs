using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel.Model
{
    partial class MM_Package : IMM_ModelElement, IWithSeqNo
    {
		public virtual string Icon
        {
            get { return "icon_package.gif"; }
        }

		public virtual int ID
        {
            get { return PackageID; }
        }

		public virtual int Level { get; set; }

		public virtual string ClassName { get { return "Пакет"; } }

		public virtual string FullSysName
        {
            get
            {
                if (ParentPackageID.HasValue)
                    return ParentPackage.FullSysName + "." + SysName;
                return SysName;
            }
        }

		public virtual string ControlPath
        {
            get
            {
				return (ParentPackageID.HasValue ? ParentPackage.ControlPath + "/" : "") + SysName;
            }
        }

		public virtual string ElementSysName
		{
			get { return SysName; }
		}
    }
}
