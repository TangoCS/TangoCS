using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Nephrite.Templating
{
	public class ActionContext
	{
		public ActionContext()
			//IHttpContext httpContext,
			//ITypeActivatorCache typeActivatorCache)
   //         IViewRendererFactory viewRendererFactory)
		{
			//HttpContext = httpContext;
			//RouteData = new RouteDataClass();
			//ViewRendererFactory = viewRendererFactory;
			//TypeActivatorCache = typeActivatorCache;

			AllArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			ActionArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			PostData = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
		}

		public IServiceProvider RequestServices { get; private set; }
		//public IHttpContext HttpContext { get; private set; }
		//public RouteDataClass RouteData { get; private set; }

		//public ITypeActivatorCache TypeActivatorCache { get; private set; }

		//public IViewRendererFactory ViewRendererFactory { get; private set; }
		//public Type RendererType { get; set; }

		//IViewRenderer _renderer;
		//public IViewRenderer Renderer
		//{
		//	get
		//	{
		//		if (_renderer == null)
		//			_renderer = ViewRendererFactory.Create(RendererType);
		//		return _renderer;
		//          }
		//	set
		//	{
		//		_renderer = value;
		//	}
		//}

		//Url _current;
		//public Url Url { get; set; }
		//{ 
		//	get
		//	{
		//		if (_current == null)
		//		{
		//			_current = new Url(HttpContext.Request.QueryString, RouteData.Values);
		//		}
		//		return _current;
		//	}
		//}

		//public class RouteDataClass
		//{
		//	public IDictionary<string, object> DataTokens { get; set; }
		//	public IDictionary<string, object> Values { get; set; }
		//}
		public void Init(IServiceProvider requestServices, IDictionary<string, string> args, string jsonData)
		{
			RequestServices = requestServices;
			var curArgsCollection = ActionArgs;

			foreach (var arg in args)
			{
				var key = arg.Key;
				var value = arg.Value;

				if (key == TemplatingConstants.ServiceName)
					Service = value.ToString();
				else if (key == TemplatingConstants.ActionName)
					Action = value.ToString();
				else if (key == "e")
				{
					curArgsCollection = EventArgs;
					Event = value.ToString().ToLower();
				}
				else if (key == "r")
				{
					EventReceiver = value.ToString().ToLower();
				}
				else if (key == "oid")
				{
					curArgsCollection.Add("id", value);
					AllArgs.Add(key, value);
				}
				else
				{
					curArgsCollection.Add(key, value);
					AllArgs.Add(key, value);
				}
			}

			if (!jsonData.IsEmpty())
			{
				var converter = new DynamicDictionaryConverter();
				var postData = JsonConvert.DeserializeObject<DynamicDictionary>(jsonData, converter);

				if (postData != null)
					PostData = postData;
			}
		}


		public class ResponseInfo
		{
			StringWriter w = new StringWriter();

			public string ContentType { get; set; }
			public int StatusCode { get; set; } = 200;
			public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

			public void Redirect(string location, bool permanent = false)
			{
				if (permanent)
					StatusCode = 301;
				else
					StatusCode = 302;
				Headers["Location"] = location;
			}

			public void Write(string text)
			{
				w.Write(text);
			}

			public string GetContent()
			{
				return w.ToString();
			}
		}

		public string Service { get; set; }
		public string Action { get; set; }
		public string RequestMethod { get; set; }
		public string Event { get; set; }
		public string EventReceiver { get; set; }

		public DynamicDictionary AllArgs { get; set; }
		public DynamicDictionary ActionArgs { get; set; }
		public DynamicDictionary EventArgs { get; set; }

		public dynamic PostBag { get { return PostData; } }
		public DynamicDictionary PostData { get; set; }

		public ResponseInfo Response { get; set; } = new ResponseInfo();
	}


	public static class ActionContextExtensions
	{
		public static string GetArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return s.ToString();
			return null;
		}
		public static int GetIntArg(this ActionContext ctx, string name, int defaultValue)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Convert.ToInt32(s);
			return defaultValue;
		}
		public static int? GetIntArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Convert.ToInt32(s);
			return null;
		}
		public static Guid GetGuidArg(this ActionContext ctx, string name, Guid defaultValue)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Guid.Parse(s.ToString());
			return defaultValue;
		}
		public static Guid? GetGuidArg(this ActionContext ctx, string name)
		{
			object s = null;
			bool b = ctx.AllArgs.TryGetValue(name, out s);
			if (b) return Guid.Parse(s.ToString());
			return null;
		}
	}
}
