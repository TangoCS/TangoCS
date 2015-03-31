using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public class ActionContext
	{
		public IHttpContext HttpContext { get; private set; }
		public IDictionary<string, object> DataTokens { get; private set; }
		public IDictionary<string, object> Values { get; private set; }
		public string Url { get; private set; }

		public ActionContext(IHttpContext httpContext, string url, IDictionary<string, object> dataTokens, IDictionary<string, object> values)
		{
			HttpContext = httpContext;
			Url = url;
			DataTokens = dataTokens;
			Values = values;
		}
	}
}
