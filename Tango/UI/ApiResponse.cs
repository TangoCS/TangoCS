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
		public Dictionary<string, List<string>> PageContext { get; set; }

		public ApiResponse()
		{
			Widgets = new Dictionary<string, object>();
			WidgetsForRemove = new List<string>();
			ClientActions = new List<ClientAction>();
			Includes = new HashSet<string>();
		}

		public void AddClientAction(string service, string method, object args)
		{
			ClientActions.Add(new ClientAction(service, method, args));
		}

		public void AddClientAction(ClientAction action)
		{
			ClientActions.Add(action);
		}

		public virtual void SetElementValue(string name, string value)
		{
			AddClientAction("domActions", "setValue", new { elName = name.ToLower(), value = value });
		}

		public void SetElementValue(ViewElement elementOwner, string name, string value)
		{
			SetElementValue(elementOwner.GetClientID(name), value);
		}

		public virtual void SetElementVisibility(string name, bool visible)
		{
			AddClientAction("domActions", "setVisible", new { elName = name.ToLower(), visible = visible });
		}
		public virtual void SetElementVisibility(ViewElement elementOwner, string name, bool visible)
		{
			SetElementVisibility(elementOwner.GetClientID(name), visible);
		}

		public virtual void SetElementAttribute(string name, string attrName, string attrValue)
		{
			AddClientAction("domActions", "setAttribute", new { elName = name.ToLower(), attrName = attrName, attrValue = attrValue });
		}
		public virtual void SetElementAttribute(ViewElement elementOwner, string name, string attrName, string attrValue)
		{
			SetElementAttribute(elementOwner.GetClientID(name), attrName, attrValue);
		}

		public virtual void RemoveElementAttribute(string name, string attrName)
		{
			AddClientAction("domActions", "removeAttribute", new { elName = name.ToLower(), attrName = attrName });
		}
		public virtual void RemoveElementAttribute(ViewElement elementOwner, string name, string attrName)
		{
			RemoveElementAttribute(elementOwner.GetClientID(name), attrName);
		}

		public virtual void SetElementClass(string name, string clsName)
		{
			AddClientAction("domActions", "setClass", new { elName = name.ToLower(), clsName = clsName });
		}
		public virtual void SetElementClass(ViewElement elementOwner, string name, string clsName)
		{
			SetElementClass(elementOwner.GetClientID(name), clsName);
		}

		public virtual void RemoveElementClass(string name, string clsName)
		{
			AddClientAction("domActions", "removeClass", new { elName = name.ToLower(), clsName = clsName });
		}
		public virtual void RemoveElementClass(ViewElement elementOwner, string name, string clsName)
		{
			RemoveElementClass(elementOwner.GetClientID(name), clsName);
		}

		public virtual ApiResponse AddWidget(string name, string content)
		{
			Widgets.Add(name.ToLower(), content);
			return this;
		}

		public virtual ApiResponse RemoveWidget(string name)
		{
			WidgetsForRemove.Add(name.ToLower());
			return this;
		}

		public virtual ApiResponse AddRootWidget(string name, string content)
		{
			AddAdjacentWidget(null, name, content);
			return this;
		}

		public virtual ApiResponse AddAdjacentWidget(string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			Widgets.Add(name.ToLower(), new { Parent = parent, Content = content, Position = position.ToString() });
			return this;
		}

		public ApiResponse AddWidget(string name, LayoutWriter content)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			return AddWidget(name, content.ToString());
		}

		public ApiResponse AddRootWidget(string name, LayoutWriter content)
		{
			return AddAdjacentWidget(null, name, content);
		}

		public ApiResponse AddAdjacentWidget(string parent, string name, LayoutWriter content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			return AddAdjacentWidget(parent, name, content.ToString(), position);
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

		public ApiResponse AddWidget(ViewElement elementOwner, string name, string content)
		{
			return AddWidget(elementOwner.GetClientID(name), content);
		}

		public ApiResponse RemoveWidget(ViewElement elementOwner, string name)
		{
			return RemoveWidget(elementOwner.GetClientID(name));
		}

		public ApiResponse AddRootWidget(ViewElement elementOwner, string name, string content)
		{
			return AddAdjacentWidget(null, elementOwner.GetClientID(name), content);
		}

		public ApiResponse AddAdjacentWidget(ViewElement elementOwner, string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			return AddAdjacentWidget(parent, elementOwner.GetClientID(name), content, position);
		}

		public ApiResponse AddWidget(ViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			return AddWidget(elementOwner.GetClientID(name), w);
		}

		public ApiResponse AddRootWidget(ViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			ClientActions.AddRange(w.ClientActions);
			foreach (var i in w.Includes) Includes.Add(i);

			return AddAdjacentWidget(null, elementOwner.GetClientID(name), w.ToString());
		}

		public ApiResponse AddChildWidget(ViewElement elementOwner, string parent, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			ClientActions.AddRange(w.ClientActions);
			foreach (var i in w.Includes) Includes.Add(i);

			return AddAdjacentWidget(parent, elementOwner.GetClientID(name), w.ToString());
		}

		public ApiResponse AddAdjacentWidget(ViewElement elementOwner, string parent, string name, AdjacentHTMLPosition position, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			ClientActions.AddRange(w.ClientActions);
			foreach (var i in w.Includes) Includes.Add(i);

			return AddAdjacentWidget(parent, elementOwner.GetClientID(name), w.ToString(), position);
		}
	}

	public enum AdjacentHTMLPosition
	{
		BeforeEnd,
		BeforeBegin,
		AfterBegin,
		AfterEnd
	}

	public class ClientAction
	{
		public string Service { get; set; }
		public List<ChainElement> CallChain { get; set; } = new List<ChainElement>();

		public ClientAction(string service, string method, object args)
		{
			Service = service;
			CallChain.Add(new ChainElement { Method = method, Args = args });
		}
		public ClientAction(string service, params object[] args) : this(service, "apply", args)
		{

		}

		public ClientAction Then(string method, object args = null)
		{
			CallChain.Add(new ChainElement { Method = method, Args = args });
			return this;
		}

		public ClientAction CallWith(params object[] args)
		{
			return Then("apply", args);
		}

		public class ChainElement
		{
			public string Method { get; set; }
			public object Args { get; set; }
		}
	}
}