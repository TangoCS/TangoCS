using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Tango.Html;

namespace Tango.UI
{
	public interface IJsonResponse
	{
		string Serialize(ActionContext context);
	}

	public class ArrayResponse : IJsonResponse
	{
		public List<object> Data { get; set; } = new List<object>();
		public string Serialize(ActionContext context)
		{
			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}
	}

	public class ObjectResponse : IJsonResponse
	{
		public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
		public virtual string Serialize(ActionContext context)
		{
			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}
	}

	public class ApiResponse : ObjectResponse
	{
		public List<IWidget> Widgets { get; set; }
		public List<ClientAction> ClientActions { get; set; }
		public HashSet<string> Includes { get; set; }
		//public Dictionary<string, List<string>> PageContext { get; set; }

		public ApiResponse()
		{
			Widgets = new List<IWidget>();
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

		public virtual void SetElementValue(string id, string value)
		{
			AddClientAction("domActions", "setValue", new { id = id.ToLower(), value = value });
		}

		public void SetElementValue(IViewElement elementOwner, string id, string value)
		{
			SetElementValue(elementOwner.GetClientID(id), value);
		}

		public virtual void SetElementVisibility(string id, bool visible)
		{
			AddClientAction("domActions", "setVisible", new { id = id.ToLower(), visible = visible });
		}
		public virtual void SetElementVisibility(IViewElement elementOwner, string id, bool visible)
		{
			SetElementVisibility(elementOwner.GetClientID(id), visible);
		}

		public virtual void SetElementAttribute(string id, string attrName, string attrValue)
		{
			AddClientAction("domActions", "setAttribute", new { id = id.ToLower(), attrName = attrName, attrValue = attrValue });
		}
		public virtual void SetElementAttribute(IViewElement elementOwner, string id, string attrName, string attrValue)
		{
			SetElementAttribute(elementOwner.GetClientID(id), attrName, attrValue);
		}

		public virtual void RemoveElementAttribute(string id, string attrName)
		{
			AddClientAction("domActions", "removeAttribute", new { id = id.ToLower(), attrName = attrName });
		}
		public virtual void RemoveElementAttribute(IViewElement elementOwner, string id, string attrName)
		{
			RemoveElementAttribute(elementOwner.GetClientID(id), attrName);
		}

		public virtual void SetElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "setClass", new { id = id.ToLower(), clsName = clsName });
		}
		public virtual void SetElementClass(IViewElement elementOwner, string id, string clsName)
		{
			SetElementClass(elementOwner.GetClientID(id), clsName);
		}

		public virtual void RemoveElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "removeClass", new { id = id.ToLower(), clsName = clsName });
		}
		public virtual void RemoveElementClass(IViewElement elementOwner, string id, string clsName)
		{
			RemoveElementClass(elementOwner.GetClientID(id), clsName);
		}

		public virtual ApiResponse AddWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget{ Name = name.ToLower(), Content = content, Action = "add" });
			return this;
		}

		public virtual ApiResponse ReplaceWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget { Name = name.ToLower(), Content = content, Action = "replace" });
			return this;
		}

		public virtual ApiResponse RemoveWidget(string name)
		{
			Widgets.Add(new Widget { Name = name.ToLower(), Action = "remove" });
			return this;
		}

		public virtual ApiResponse AddRootWidget(string name, string content)
		{
			AddAdjacentWidget(null, name, content);
			return this;
		}

		public virtual ApiResponse AddAdjacentWidget(string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			Widgets.Add(new AdjacentWidget { Name = name.ToLower(), Parent = parent, Content = content, Action = "adjacent", Position = position.ToString() });
			return this;
		}

		public ApiResponse AddWidget(string name, LayoutWriter content)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			return AddWidget(name, content.ToString());
		}

		public ApiResponse ReplaceWidget(string name, LayoutWriter content)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes) Includes.Add(i);
			return ReplaceWidget(name, content.ToString());
		}

		public ApiResponse AddRootWidget(string name, LayoutWriter content)
		{
			return AddAdjacentWidget(null, name, content);
		}

		public ApiResponse AddAdjacentWidget(string parent, string name, LayoutWriter content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			ClientActions.AddRange(content.ClientActions);
			foreach (var i in content.Includes)
				Includes.Add(i);
			return AddAdjacentWidget(parent, name, content.ToString(), position);
		}

		public void RedirectBack(ActionContext context)
		{
			Data.Add("url", context.GetArg(Constants.ReturnUrl));
		}

		public override string Serialize(ActionContext context)
		{
			var container = context.GetArg("c-type");
			if (!container.IsEmpty())
			{
				var coll = context.RequestServices.GetService(typeof(IContainerCollection)) as IContainerCollection;
				if (coll.TryGetValue(container, out var item))
				{
					var id = context.GetArg("c-id");
					var prefix = context.GetArg("c-prefix");
					
					var mappings = item.Mapping.ToDictionary(
						o => HtmlWriterHelpers.GetID(prefix, o.Key), 
						o => HtmlWriterHelpers.GetID(id, o.Value)
					);
					foreach (var wgt in Widgets)
					{
						if (mappings.ContainsKey(wgt.Name))
							wgt.Name = mappings[wgt.Name];
					}

					var w = new LayoutWriter(context, id);
					item.Renderer(w);

					Widgets.Insert(0, new AdjacentWidget {
						Name = w.GetID(item.ID),
						Content = w.ToString(),
						Action = "adjacent",
						Position = AdjacentHTMLPosition.BeforeEnd.ToString()
					});
				}
			}

			if (Widgets.Count > 0)
				Data.Add("widgets", Widgets);
			if (ClientActions.Count > 0)
				Data.Add("clientactions", ClientActions);
			if (Includes.Count > 0)
				Data.Add("includes", Includes.Select(o => GlobalSettings.JSPath + o));

			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}

		public ApiResponse AddWidget(IViewElement elementOwner, string name, string content)
		{
			return AddWidget(elementOwner.GetClientID(name), content);
		}

		public ApiResponse ReplaceWidget(IViewElement elementOwner, string name, string content)
		{
			return ReplaceWidget(elementOwner.GetClientID(name), content);
		}

		public ApiResponse RemoveWidget(IViewElement elementOwner, string name)
		{
			return RemoveWidget(elementOwner.GetClientID(name));
		}

		public ApiResponse AddRootWidget(IViewElement elementOwner, string name, string content)
		{
			return AddAdjacentWidget(null, elementOwner.GetClientID(name), content);
		}

		public ApiResponse AddAdjacentWidget(IViewElement elementOwner, string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			return AddAdjacentWidget(parent, elementOwner.GetClientID(name), content, position);
		}

		public ApiResponse AddWidget(IViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			return AddWidget(elementOwner.GetClientID(name), w);
		}

		public ApiResponse ReplaceWidget(IViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			return ReplaceWidget(elementOwner.GetClientID(name), w);
		}

		public ApiResponse AddRootWidget(IViewElement elementOwner, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			ClientActions.AddRange(w.ClientActions);
			foreach (var i in w.Includes) Includes.Add(i);

			return AddAdjacentWidget(null, elementOwner.GetClientID(name), w.ToString());
		}

		public ApiResponse AddChildWidget(IViewElement elementOwner, string parent, string name, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(elementOwner.Context, elementOwner.ClientID);
			content?.Invoke(w);

			ClientActions.AddRange(w.ClientActions);
			foreach (var i in w.Includes) Includes.Add(i);

			return AddAdjacentWidget(parent, elementOwner.GetClientID(name), w.ToString());
		}

		public ApiResponse AddAdjacentWidget(IViewElement elementOwner, string parent, string name, AdjacentHTMLPosition position, Action<LayoutWriter> content)
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

	public interface IWidget
	{
		string Name { get; set; }
	}

	public class Widget : IWidget
	{
		public string Name { get; set; }
		public string Action { get; set; }
		
	}
	public class ContentWidget : Widget
	{
		public string Content { get; set; }
	}
	public class AdjacentWidget : ContentWidget
	{
		public string Position { get; set; }
		public string Parent { get; set; }
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

	public interface IContainerCollection : IDictionary<string, IContainerCollectionItem>
	{

	}

	public interface IContainerCollectionItem
	{
		string ID { get; }
		IDictionary<string, string> Mapping { get; }
		Action<LayoutWriter> Renderer { get; }
	}

	public class ContainerCollectionItem : IContainerCollectionItem
	{
		public string ID { get; set; }
		public IDictionary<string, string> Mapping { get; set; }
		public Action<LayoutWriter> Renderer { get; set; }
	}
}