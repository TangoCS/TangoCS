using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Nephrite.Razor
{
    public static class RazorFormRenderer
    {
		public static void Configure(string rootViewFolder)
		{
			var config = new TemplateServiceConfiguration();
			config.CachingProvider = new DefaultCachingProvider();
			config.TemplateManager = new DefaultTemplateManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootViewFolder));
			var service = RazorEngineService.Create(config);
			Engine.Razor = service;
		}

		public static string RenderMessage(string title, string message)
		{
			return "";
		}

		public static string RenderView(IHttpContext ctx, string folder, string viewName, object viewData)
		{
			var templatePath = Path.Combine(folder, viewName + ".cshtml");
			var templateKey = viewName.ToLower() + ".cshtml";// +File.GetLastWriteTime(templatePath).ToString();

			if (Engine.Razor.IsTemplateCached(templateKey, viewData.GetType()))
				return Engine.Razor.Run(templateKey, viewData.GetType(), viewData);
			else
			{
				return Engine.Razor.RunCompile(File.ReadAllText(templatePath), templateKey, viewData.GetType(), viewData);
			}
		}
    }
}
