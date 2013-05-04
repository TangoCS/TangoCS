using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Smo;

namespace Nephrite.Metamodel.Model
{
    partial class MM_TaggedValueType
    {
        public DataType DataType
        {
            get
            {
                switch (TypeCode)
                {
                    case TaggedValueType.Boolean:
                        return DataType.Bit;
                    case TaggedValueType.DateTime:
                        return DataType.DateTime;
                    case TaggedValueType.Number:
                        return DataType.Int;
                    case TaggedValueType.String:
                        return DataType.NVarCharMax;
                    default:
                        return DataType.Int;
                }
            }
        }
    }
}
