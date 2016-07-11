using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;
using Tango;
using Tango.Html;
using Tango.Razor;
using Solution.Configuration;
//using Tessera3.Views;

namespace Solution
{
	public class RazorRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new RazorHttpHandler(requestContext);
		}
	}

	public class ApiRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new ApiHttpHandler(requestContext);
		}
	}

	public class WebFormsRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new WebFormsHttpHandler(requestContext);
		}
	}


	public class RazorHttpHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return false; }
		}

		public RequestContext RequestContext { get; private set; }

		public RazorHttpHandler(RequestContext requestContext)
		{
			this.RequestContext = requestContext;
		}

		public void ProcessRequest(HttpContext context)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			var r = context.Response;
			var routes = context.Request.RequestContext.RouteData;

			//using (var ctx = new DefaultHttpContext())
			//{
			//	r.Write(RazorRenderer.RenderView(
			//		ctx, 
			//		Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views"), 
			//		"HomePage", 
			//		ctx));
			//}
			
			sw.Stop();
			//r.Write(sw.Elapsed);
			r.End();
		}
	}

	public class ApiHttpHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return false; }
		}

		public RequestContext RequestContext { get; private set; }

		public ApiHttpHandler(RequestContext requestContext)
		{
			this.RequestContext = requestContext;
		}

		public void ProcessRequest(HttpContext context)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var r = context.Response;

			var t = RequestContext.RouteData.Values["mode"].ToString();
			var m = RequestContext.RouteData.Values["action"].ToString();
			Type apiControllerType = TypeFactory.GetType(t);
			object apiController = Activator.CreateInstance(apiControllerType);
			var method = apiControllerType.GetMethod(m);
			method.Invoke(apiController, null);
			
			//string controllerName = this.RequestContext.RouteData.Controller;
			//IControllerFactory controllerFactory = ControllerBuilder.Current.GetControllerFactory();
			//IController controller = controllerFactory.CreateController(this.RequestContext, controllerName);
			//controller.Execute(this.RequestContext);

			sw.Stop();
			r.Write(sw.Elapsed);
			r.End();
		}
	}

	public class WebFormsHttpHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return false; }
		}

		public RequestContext RequestContext { get; private set; }

		public WebFormsHttpHandler(RequestContext requestContext)
		{
			this.RequestContext = requestContext;
		}

		public void ProcessRequest(HttpContext context)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var r = context.Response;

			//NOTE: here you should implement your custom mapping
			string aspxFile = "~/Default.aspx";
			//Get compiled type by path
			Type type = BuildManager.GetCompiledType(aspxFile);
			//create instance of the page
			Page page = (Page)Activator.CreateInstance(type);
			//process request
			page.ProcessRequest(context);


			sw.Stop();
			r.Write(sw.Elapsed);
			r.End();
		}
	}
}
