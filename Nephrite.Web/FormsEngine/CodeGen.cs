using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.FormsEngine
{
    public static class CodeGen
    {
        public static IMM_ObjectType GetObjectType(string defaultSysName)
        {
            string sysName = (string)AppDomain.CurrentDomain.GetData("ObjectTypeSysName") ?? defaultSysName;
			return ((IDC_MetaStorage)A.Model).IMM_ObjectType.Single(o => o.SysName.ToUpper() == sysName.ToUpper());
        }
    }
}
