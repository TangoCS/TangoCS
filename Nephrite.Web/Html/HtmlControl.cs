using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Nephrite.Web.Html
{
	public abstract partial class HtmlControl
	{
		public HtmlPage Page { get; set; }

		NameValueCollection _formValues = null;
		public NameValueCollection FormValues
		{
			get
			{
				if (_formValues == null) _formValues = HttpContext.Current.Request.Form;
				return _formValues;
			}
		}

		public abstract void Render();
	}
}