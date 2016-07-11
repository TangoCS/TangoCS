using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Tango.UI
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

		//public virtual void BindEvent(string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		//{
		//	ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
		//		Id = elementId, ClientEvent = clientEvent,
		//		ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver
		//	}));
		//}

		public void AddClientAction(string service, string method, object args)
		{
			ClientActions.Add(new ClientAction(service, method, args));
		}

		public virtual void SetElementValue(string name, string value)
		{
			AddClientAction("domActions", "setValue", new { elName = name.ToLower(), value = value });
		}
		public void SetElementValue(ViewElement elementOwner, string name, string value)
		{
			SetElementValue(elementOwner.GetElementID(name), value);
		}

		public virtual void SetElementVisibility(string name, bool visible)
		{
			AddClientAction("domActions", "setVisible", new { elName = name.ToLower(), visible = visible });
		}
		public virtual void SetElementVisibility(ViewElement elementOwner, string name, bool visible)
		{
			SetElementVisibility(elementOwner.GetElementID(name), visible);
		}

		public virtual void SetElementAttribute(string name, string attrName, string attrValue)
		{
			AddClientAction("domActions", "setAttribute", new { elName = name.ToLower(), attrName = attrName, attrValue = attrValue });
		}
		public virtual void SetElementAttribute(ViewElement elementOwner, string name, string attrName, string attrValue)
		{
			SetElementAttribute(elementOwner.GetElementID(name), attrName, attrValue);
		}

		public virtual void RemoveElementAttribute(string name, string attrName)
		{
			AddClientAction("domActions", "removeAttribute", new { elName = name.ToLower(), attrName = attrName });
		}
		public virtual void RemoveElementAttribute(ViewElement elementOwner, string name, string attrName)
		{
			RemoveElementAttribute(elementOwner.GetElementID(name), attrName);
		}

		public virtual void SetElementClass(string name, string clsName)
		{
			AddClientAction("domActions", "setClass", new { elName = name.ToLower(), clsName = clsName });
		}
		public virtual void SetElementClass(ViewElement elementOwner, string name, string clsName)
		{
			SetElementClass(elementOwner.GetElementID(name), clsName);
		}

		public virtual void RemoveElementClass(string name, string clsName)
		{
			AddClientAction("domActions", "removeClass", new { elName = name.ToLower(), clsName = clsName });
		}
		public virtual void RemoveElementClass(ViewElement elementOwner, string name, string clsName)
		{
			RemoveElementClass(elementOwner.GetElementID(name), clsName);
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

		public void RedirectBack(ActionContext context)
		{
			Data.Add("url", context.GetArg(Constants.ReturnUrl));
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
				Data.Add("includes", Includes.Select(o => GlobalSettings.JSPath + o));

			return JsonConvert.SerializeObject(Data, Json.CamelCase);
		}

		//public override void BindEvent(string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		//{
		//	base.BindEvent(_getID(elementId), clientEvent, serverEvent, serverEventReceiver);
		//}

		public void AddWidget(ViewElement elementOwner, string name, string content)
		{
			AddWidget(elementOwner.GetElementID(name), content);
		}

		public void RemoveWidget(ViewElement elementOwner, string name)
		{
			RemoveWidget(elementOwner.GetElementID(name));
		}

		public void AddRootWidget(ViewElement elementOwner, string name, string content)
		{
			AddRootWidget(elementOwner.GetElementID(name), content);
		}

		public void AddChildWidget(ViewElement elementOwner, string parent, string name, string content)
		{
			AddChildWidget(parent, elementOwner.GetElementID(name), content);
		}

		public void AddWidget(ViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = elementOwner.CreateLayoutWriter();
			content?.Invoke(w);
			AddWidget(elementOwner.GetElementID(name), w);
		}

		public void AddRootWidget(ViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = elementOwner.CreateLayoutWriter();
			content?.Invoke(w);
			AddRootWidget(elementOwner.GetElementID(name), w);
		}

		public void AddChildWidget(ViewElement elementOwner, string parent, string name, Action<LayoutWriter> content)
		{
			var w = elementOwner.CreateLayoutWriter();
			content?.Invoke(w);
			AddChildWidget(parent, elementOwner.GetElementID(name), w);
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
