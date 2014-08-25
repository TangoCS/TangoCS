using System;
using System.Collections.Generic;

namespace Nephrite.Web.Html
{
	public class HtmlPage
	{
		Dictionary<string, string> _css = new Dictionary<string, string>();
		Dictionary<string, string> _js = new Dictionary<string, string>();
		Dictionary<string, string> _startupjs = new Dictionary<string, string>();

		public void RegisterCSS(string name, string path)
		{
			if (!_css.ContainsKey(name)) _css.Add(name, path);
		}

		public void RegisterScript(string name, string path)
		{
			if (!_js.ContainsKey(name)) _js.Add(name, path);
		}

		public void RegisterStartupScript(string name, string script)
		{
			if (!_startupjs.ContainsKey(name)) _startupjs.Add(name, script);
		}
	}
}