﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;

namespace Nephrite.Metamodel
{
    public abstract class BaseModelObject : ICloneable
    {
        public abstract void WriteDeleteObjectHistory();
        public abstract void WriteInsertObjectHistory();
        public abstract void WriteUpdateObjectHistory();
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}