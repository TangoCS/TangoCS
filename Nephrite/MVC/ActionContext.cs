using System;
using System.Collections.Generic;
using Nephrite.Http;

namespace Nephrite.MVC
{
	public class ActionContext
	{
		public ActionContext(
			IHttpContext httpContext,
			ITypeActivatorCache typeActivatorCache,
            IViewRendererFactory viewRendererFactory,
			RouteDataClass routeData)
		{
			HttpContext = httpContext;
			RouteData = routeData;
			ViewRendererFactory = viewRendererFactory;
			TypeActivatorCache = typeActivatorCache;

			ActionArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			PostData = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
		}

		public IHttpContext HttpContext { get; private set; }
		public RouteDataClass RouteData { get; private set; }

		public ITypeActivatorCache TypeActivatorCache { get; private set; }

		public IViewRendererFactory ViewRendererFactory { get; private set; }
		public Type RendererType { get; set; }

		IViewRenderer _renderer;
		public IViewRenderer Renderer
		{
			get
			{
				if (_renderer == null)
					_renderer = ViewRendererFactory.Create(RendererType);
				return _renderer;
            }
			set
			{
				_renderer = value;
			}
		}

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

		public string Service { get; set; }
		public string Action { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public dynamic ActionArgs { get; set; }
		public dynamic EventArgs { get; set; }

		public dynamic PostBag { get { return PostData; } }
		public DynamicDictionary PostData { get; set; }
	}
}
