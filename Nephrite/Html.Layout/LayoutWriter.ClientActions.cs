using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Templating;

namespace Nephrite.Html.Layout
{
	public static class LayoutWriterClientActionsExtensions
	{
		public static void AddWidget(this ApiResponse r, string name, LayoutWriter content)
		{
			r.ClientActions.AddRange(content.ClientActions);
			r.AddWidget(name, content.ToString());
		}

		public static void AddRootWidget(this ApiResponse r, string name, LayoutWriter content)
		{
			r.AddChildWidget(null, name, content);
		}

		public static void AddChildWidget(this ApiResponse r, string parent, string name, LayoutWriter content)
		{
			r.ClientActions.AddRange(content.ClientActions);
			r.AddChildWidget(parent, name, content.ToString());
		}

		public static void AddWidget(this ApiResponse r, string name, Action<LayoutWriter> content)
		{
			if (r.Template == null) throw new Exception("Template is not defined");
			var w = r.Template.CreateLayoutWriter();
			content(w);
			r.AddWidget(name, w);
		}

		public static void AddRootWidget(this ApiResponse r, string name, Action<LayoutWriter> content)
		{
			if (r.Template == null) throw new Exception("Template is not defined");
			var w = r.Template.CreateLayoutWriter();
			content(w);
			r.AddRootWidget(name, w);
		}

		public static void AddChildWidget(this ApiResponse r, string parent, string name, Action<LayoutWriter> content)
		{
			if (r.Template == null) throw new Exception("Template is not defined");
			var w = r.Template.CreateLayoutWriter();
			content(w);
			r.AddChildWidget(parent, name, w);
		}

		public static void AddClientAction(this LayoutWriter w, string service, string method, object args)
		{
			w.ClientActions.Add(new ClientAction(service, method, args));
		}

		public static void BindEvent(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			w.ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
				Id = GetTrueName(w, elementId), ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver
			}));
		}

		static string GetTrueName(LayoutWriter w, string name)
		{
			return !w.IDPrefix.IsEmpty() ? w.IDPrefix + "_" + name : name;
		}
	}
}
