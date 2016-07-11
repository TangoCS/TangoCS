using System.Collections.Generic;

namespace Tango.Html
{
	public class HtmlPage
	{
		//ActionContext _context;

		Dictionary<string, string> _css = new Dictionary<string, string>();
		Dictionary<string, string> _js = new Dictionary<string, string>();
		Dictionary<string, string> _startupjs = new Dictionary<string, string>();

		//public ActionContext ActionContext { get { return _context; } }

		//public HtmlPage(ActionContext context)
		//{
		//	_context = context;
		//}

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