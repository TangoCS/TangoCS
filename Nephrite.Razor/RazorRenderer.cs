using System;
using System.IO;
using Nephrite.MVC;
using RazorEngine;
using RazorEngine.Templating;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.Razor
{
	public class RazorRenderer : IViewRenderer
    {
		string _result;

		public bool IsStringResult
		{
			get
			{
				return true;
			}
		}

		public void RenderHtml(string title, string html)
		{
			throw new NotImplementedException();
		}

		public void RenderMessage(string message)
		{
			throw new NotImplementedException();
		}

		public void RenderView(string folder, string viewName, object viewData)
		{
			var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalSettings.ControlsPath, folder, viewName + ".cshtml");
			var templateKey = Path.Combine(folder.ToLower(), viewName.ToLower() + ".cshtml") + File.GetLastWriteTime(templatePath).ToString();

			if (Engine.Razor.IsTemplateCached(templateKey, viewData.GetType()))
				_result = Engine.Razor.Run(templateKey, viewData.GetType(), viewData);
			else
			{
				_result = Engine.Razor.RunCompile(File.ReadAllText(templatePath), templateKey, viewData.GetType(), viewData);
			}
		}

		public void RenderMessage(string title, string message)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return _result;
		}

		public static string Render(string folder, string viewName, object viewData)
		{
			var r = new RazorRenderer();
			r.RenderView(folder, viewName, viewData);
			return r.ToString();
        }
		public static string Render(string folder, string viewName)
		{
			return Render(folder, viewName, "");
		}
		public static string Render<T>(Func<T, ActionResult> action) where T : Controller
		{
			var actionContext = DI.RequestServices.GetService<ActionContext>();
			var controller = Activator.CreateInstance(typeof(T)) as T;
			actionContext.Renderer = new RazorRenderer();
			controller.ActionContext = actionContext;
			
			var res = action(controller);
			res.ExecuteResult(actionContext);

			return actionContext.Renderer.ToString();
		}
	}
}
