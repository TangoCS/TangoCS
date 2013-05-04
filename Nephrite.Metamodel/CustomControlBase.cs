using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Nephrite.Metamodel
{
	public class CustomControlBase : UserControl
	{
		protected object viewData = null;
        
        public void SetViewData(object viewData)
        {
            this.viewData = viewData;
        }

        public string enc(string str)
        {
			return HttpUtility.HtmlEncode(str ?? "");
        }
	}
}
