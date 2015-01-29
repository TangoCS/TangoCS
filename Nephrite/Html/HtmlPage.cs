using System;
using System.Collections.Generic;

namespace Nephrite.Html
{
	public class HtmlPage
	{
		IAppContext _context;

		Dictionary<string, string> _css = new Dictionary<string, string>();
		Dictionary<string, string> _js = new Dictionary<string, string>();
		Dictionary<string, string> _startupjs = new Dictionary<string, string>();

		public IAppContext AppContext { get { return _context; } }

		public HtmlPage(IAppContext context)
		{
			_context = context;
		}

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