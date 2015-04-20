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
		public ActionContext()
		{
			RouteData = new RouteDataClass();
		}

		public IHttpContext HttpContext { get; set; }
		public RouteDataClass RouteData { get; set; }
		public IViewRenderer Renderer { get; set; }

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
