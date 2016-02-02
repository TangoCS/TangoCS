using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nephrite.UI
{
	public interface IJsonResponse
	{
		string Serialize();
	}

	public class ArrayResponse : IJsonResponse
	{
		public List<object> Data { get; set; } = new List<object>();
		public string Serialize()
		{
			return JsonConvert.SerializeObject(Data, Json.CamelCase);
		}
	}

	public class ObjectResponse : IJsonResponse
	{
		public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
		public virtual string Serialize()
		{
			return JsonConvert.SerializeObject(Data, Json.CamelCase);
		}
	}

	public class ApiResponse : ObjectResponse
	{
		public Dictionary<string, object> Widgets { get; set; }
		public List<string> WidgetsForRemove { get; set; }
		public List<ClientAction> ClientActions { get; set; }
		public HashSet<string> Includes { get; set; }

		public ApiResponse()
		{
			Widgets = new Dictionary<string, object>();
			WidgetsForRemove = new List<string>();
			ClientActions = new List<ClientAction>();
			Includes = new HashSet<string>();
		}

		public virtual void BindEvent(string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
				Id = elementId, ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver
			}));
		}

		public void AddClientAction(string service, string method, object args)
		{
			ClientActions.Add(new ClientAction(service, method, args));
		}

		public virtual void AddWidget(string name, string content)
		{
			Widgets.Add(name.ToLower(), content);
		}

		public virtual void RemoveWidget(string name)
		{
			WidgetsForRemove.Add(name.ToLower());
		}

		public virtual void AddRootWidget(string name, string content)
		{
			AddChildWidget(null, name, content);
		}

		public virtual void AddChildWidget(string parent, string name, string content)
		{
			Widgets.Add(name.ToLower(), new { Parent = parent, Content = content });
		}

		public void AddWidget(string name, LayoutWriter content)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			AddWidget(name, content.ToString());
		}

		public void AddRootWidget(string name, LayoutWriter content)
		{
			AddChildWidget(null, name, content);
		}

		public void AddChildWidget(string parent, string name, LayoutWriter content)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			AddChildWidget(parent, name, content.ToString());
		}

		public override string Serialize()
		{
			if (Widgets.Count > 0)
				Data.Add("widgets", Widgets);
			if (WidgetsForRemove.Count > 0)
				Data.Add("widgetsforremove", WidgetsForRemove);
			if (ClientActions.Count > 0)
				Data.Add("clientactions", ClientActions);
			if (Includes.Count > 0)
				Data.Add("includes", Includes);

			return JsonConvert.SerializeObject(Data, Json.CamelCase);
		}
	}

	public class ViewElementResponse : ApiResponse
	{
		Func<string, string> _getID;
		Func<LayoutWriter> _createWriter;

		public ViewElementResponse(Func<string, string> getID, Func<LayoutWriter> createWriter) : base()
		{
			_getID = getID;
			_createWriter = createWriter;
		}

		public override void BindEvent(string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			base.BindEvent(_getID(elementId), clientEvent, serverEvent, serverEventReceiver);
		}

		public override void AddWidget(string name, string content)
		{
			base.AddWidget(_getID(name), content);
		}

		public override void RemoveWidget(string name)
		{
			base.RemoveWidget(_getID(name));
		}

		public override void AddRootWidget(string name, string content)
		{
			base.AddRootWidget(_getID(name), content);
		}

		public override void AddChildWidget(string parent, string name, string content)
		{
			base.AddChildWidget(parent, _getID(name), content);
		}

		public void AddWidget(string name, Action<LayoutWriter> content)
		{
			var w = _createWriter();
			content(w);
			AddWidget(name, w);
		}

		public void AddRootWidget(string name, Action<LayoutWriter> content)
		{
			var w = _createWriter();
			content(w);
			AddRootWidget(name, w);
		}

		public void AddChildWidget(string parent, string name, Action<LayoutWriter> content)
		{
			var w = _createWriter();
			content(w);
			AddChildWidget(parent, name, w);
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

	//public static class ApiResponseExtensions
	//{
	//	public static void Add(this List<KeyValuePair<string, object>> list, string name, object value)
	//	{
	//		list.Add(new KeyValuePair<string, object>(name, value));
	//	}
	//}
}
