using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Templating
{
	public class ApiResponse
	{
		public Dictionary<string, object> Data { get; set; }
		public Dictionary<string, object> Widgets { get; set; }
		public List<ClientAction> ClientActions { get; set; }
		public ICsTemplate Template { get; private set; }

		void Init()
		{
			Data = new Dictionary<string, object>();
			Widgets = new Dictionary<string, object>();
			ClientActions = new List<ClientAction>();
			Data.Add("widgets", Widgets);
			Data.Add("clientactions", ClientActions);
		}

		public ApiResponse()
		{
			Init();
		}
		public ApiResponse(ICsTemplate template)
		{
			Init();
			Template = template;
		}
	}

	public class ClientAction
	{
		public string Service { get; set; }
		public string Method { get; set; }
		public object Args { get; set; }

		public ClientAction(string service, string method, object args)
		{
			Service = service;
			Method = method;
			Args = args;
		}
	}

	public static class ApiResponseExtensions
	{
		static string GetTrueName(ApiResponse r, string name)
		{
			return r.Template != null && !r.Template.ID.IsEmpty() ? r.Template.ID + "_" + name : name;
		}

		public static void AddWidget(this ApiResponse r, string name, string content)
		{
			r.Widgets.Add(GetTrueName(r, name), content);
		}

		public static void AddRootWidget(this ApiResponse r, string name, string content)
		{
			r.AddChildWidget(null, GetTrueName(r, name), content);
		}

		public static void AddChildWidget(this ApiResponse r, string parent, string name, string content)
		{
			r.Widgets.Add(GetTrueName(r, name), new { Parent = parent, Content = content });
		}

		public static void AddClientAction(this ApiResponse r, string service, string method, object args)
		{
			r.ClientActions.Add(new ClientAction(service, method, args));
		}

		public static void BindEvent(this ApiResponse r, string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			r.ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
				Id = GetTrueName(r, elementId), ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver
			}));
		}
	}
}
