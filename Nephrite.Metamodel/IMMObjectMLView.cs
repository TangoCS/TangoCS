using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
    public interface IMMObjectMLView : IMMObject
    {
        string LanguageCode { get; }
    }
}
