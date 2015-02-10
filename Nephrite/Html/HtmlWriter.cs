using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Nephrite.Html
{
	public abstract partial class HtmlWriter : StringWriter
	{
		public HtmlPage Page { get; set; }

		NameValueCollection _formValues = null;
		public NameValueCollection FormValues
		{
			get
			{
				if (_formValues == null) _formValues = Page.AppContext.Request.Form;
				return _formValues;
			}
		}

		public abstract void Render();
	}
}