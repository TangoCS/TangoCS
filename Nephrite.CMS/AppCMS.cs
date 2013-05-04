using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.CMS.Model;
using Nephrite.Web;
using System.IO;

namespace Nephrite.CMS
{
    public class AppCMS
    {
        public static modelDataContext DataContext
        {
            get
            {
                if (HttpContext.Current.Items["CMSmodelDataContext"] == null)
                {
                    modelDataContext dc = new modelDataContext(ConnectionManager.Connection);
					dc.CommandTimeout = 300;
					HttpContext.Current.Items["CMSmodelDataContext"] = dc;
                    dc.Log = new StringWriter();
                }
				return (modelDataContext)HttpContext.Current.Items["CMSmodelDataContext"];
            }
        }
    }
}
