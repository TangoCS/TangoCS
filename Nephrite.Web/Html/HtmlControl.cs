using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Nephrite.Web.Html
{
	public abstract partial class HtmlControl
	{
		public HtmlPage Page { get; set; }

		NameValueCollection _form = null;
		public NameValueCollection Form
		{
			get
			{
				if (_form == null) _form = HttpContext.Current.Request.Form;
				return _form;
			}
		}

		public abstract void Render();
	}
}