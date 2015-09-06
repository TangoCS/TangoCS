using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Http;

namespace Nephrite.MVC
{
	public class ActionContext
	{
		public ActionContext(IHttpContext httpContext, RouteDataClass routeData)
		{
			HttpContext = httpContext;
			RouteData = routeData;
		}

		public IHttpContext HttpContext { get; private set; }
		public RouteDataClass RouteData { get; private set; }

		public IViewRenderer Renderer { get; set; }
		public IUrlHelper UrlHelper { get; set; }

		Url _current;
		public Url Url 
		{ 
			get
			{
				if (_current == null)
				{
					_current = new Url(HttpContext.Request.Url.PathAndQuery, RouteData.Values);
				}
				return _current;
			} 
		}

		public class RouteDataClass
		{
			public IDictionary<string, object> DataTokens { get; set; }
			public IDictionary<string, object> Values { get; set; }
		}
	}
}
