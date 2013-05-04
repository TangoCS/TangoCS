using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.Model
{
    public interface IMM_ModelElement
    {
        string Title { get; }
        string Icon { get; }
        int ID { get; }
		string ElementSysName { get; }
        int Level { get; set; }
        string ClassName { get; }
    }
}
