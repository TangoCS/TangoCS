using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel.TextTransform
{
    public static class CodeGen
    {
        public static MM_ObjectType GetObjectType(string defaultSysName)
        {
            string sysName = (string)AppDomain.CurrentDomain.GetData("ObjectTypeSysName") ?? defaultSysName;
            return AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName.ToUpper() == sysName.ToUpper());
        }
    }
}
