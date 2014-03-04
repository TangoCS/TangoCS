using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controls
{

    public class NavigScripts
    {
        private INavigScript _inavigScript;
        public NavigScripts()
        {
            _inavigScript = ConfigurationManager.AppSettings["DBType"].ToUpper() == "DB2"
                                ? (INavigScript)new NavigScriptDB2()
                                : new NavigScriptMSSQL();
        }

        private INavigScript Script
        {
            get { return _inavigScript; }
        }
        public string GetMenuScript
        {
            get { return Script.Menu; }
        }
    }
}