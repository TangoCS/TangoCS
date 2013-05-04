using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web
{
    public interface IRouteHandler
    {
        string ProcessRoute(string[] items);
        string GeneratePath(string querystring);
    }
}
