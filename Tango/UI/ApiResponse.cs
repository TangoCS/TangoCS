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
		public List<IWidget> Widgets { get; set; } = new List<IWidget>();
		public List<ClientAction> ClientActions { get; set; } = new List<ClientAction>();
		public HashSet<string> Includes { get; set; } = new HashSet<string>();

		List<ContentWidgetPostRendered> _widgetsToRender = new List<ContentWidgetPostRendered>();
		string _idprefix = "";
		Func<string, string> _namefunc = name => name.ToLower();

		public ApiResponse()
		{

		}

		public ApiResponse(IInteractionFlowElement element)
		{
			WithWritersFor(element as IViewElement);
		}

		public ApiResponse WithWritersFor(IViewElement view)
		{
			_idprefix = view?.ClientID ?? "";
			return this;
		}

		public ApiResponse WithWritersFor(IViewElement view, Action content)
		{
			var lastPrefix = _idprefix;
			_idprefix = view?.ClientID ?? "";
			content();
			_idprefix = lastPrefix;
			return this;
		}

		public ApiResponse WithRootWriters(Action content)
		{
			return WithWritersFor(null, content);
		}

		public ApiResponse WithNamesFor(IViewElement view)
		{
			if (view != null)
				_namefunc = name => view.GetClientID(name);
			else
				_namefunc = name => name.ToLower();
			return this;
		}

		public ApiResponse WithNamesAndWritersFor(IViewElement view)
		{
			return WithNamesFor(view).WithWritersFor(view);
		}

		public void Insert(ApiResponse resp)
		{
			for (int i = resp.Widgets.Count - 1; i >= 0; i--)
				Widgets.Insert(0, resp.Widgets[i]);

			for (int i = resp._widgetsToRender.Count - 1; i >= 0; i--)
				_widgetsToRender.Insert(0, resp._widgetsToRender[i]);

			for (int i = resp.ClientActions.Count - 1; i >= 0; i--)
				ClientActions.Insert(0, resp.ClientActions[i]);
		}

		public void AddClientAction(string service, string method, object args)
		{
			ClientActions.Add(new ClientAction(service, method, args));
		}

		public void AddClientAction(ClientAction action)
		{
			ClientActions.Add(action);
		}

		public void RedirectBack(ActionContext context)
		{
			Data.Add("url", context.GetArg(Constants.ReturnUrl));
		}

		#region dom actions
		public virtual void SetElementValue(string id, string value)
		{
			AddClientAction("domActions", "setValue", new { id = _namefunc(id), value = value });
		}

		public virtual void SetElementVisibility(string id, bool visible)
		{
			AddClientAction("domActions", "setVisible", new { id = _namefunc(id), visible = visible });
		}

		public virtual void SetElementAttribute(string id, string attrName, string attrValue)
		{
			AddClientAction("domActions", "setAttribute", new { id = _namefunc(id), attrName = attrName, attrValue = attrValue });
		}

		public virtual void RemoveElementAttribute(string id, string attrName)
		{
			AddClientAction("domActions", "removeAttribute", new { id = _namefunc(id), attrName = attrName });
		}

		public virtual void SetElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "setClass", new { id = _namefunc(id), clsName = clsName });
		}

		public virtual void RemoveElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "removeClass", new { id = _namefunc(id), clsName = clsName });
		}
		#endregion	

		#region main widget methods, string content
		public virtual ApiResponse AddWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget{ Name = _namefunc(name), Content = content, Action = "add" });
			return this;
		}

		public virtual ApiResponse ReplaceWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget { Name = _namefunc(name), Content = content, Action = "replace" });
			return this;
		}

		public virtual ApiResponse RemoveWidget(string name)
		{
			Widgets.Add(new Widget { Name = _namefunc(name), Action = "remove" });
			return this;
		}

		public virtual ApiResponse AddRootWidget(string name, string content)
		{
			AddAdjacentWidget(null, name, content);
			return this;
		}

		public virtual ApiResponse AddAdjacentWidget(string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			Widgets.Add(new AdjacentWidget { Name = _namefunc(name), Parent = parent, Content = content, Action = "adjacent", Position = position.ToString() });
			return this;
		}
		#endregion

		#region root widget, no prefix, content = action
		public ApiResponse AddWidget(string name, Action<LayoutWriter> content)
		{
			var w = new ContentWidgetPostRendered { Name = _namefunc(name), RenderAction = content, Action = "add",
				IDPrefix = _idprefix };
			Widgets.Add(w);
			_widgetsToRender.Add(w);
			return this;
		}

		public ApiResponse ReplaceWidget(string name, Action<LayoutWriter> content)
		{
			var w = new ContentWidgetPostRendered { Name = _namefunc(name), RenderAction = content, Action = "replace",
				IDPrefix = _idprefix };
			Widgets.Add(w);
			_widgetsToRender.Add(w);
			return this;
		}

		public ApiResponse AddRootWidget(string name, Action<LayoutWriter> content)
		{
			return AddAdjacentWidget(null, name, AdjacentHTMLPosition.BeforeEnd, content);
		}

		public ApiResponse AddChildWidget(string parent, string name, Action<LayoutWriter> content)
		{
			return AddAdjacentWidget(parent, name, AdjacentHTMLPosition.BeforeEnd, content);
		}

		public ApiResponse AddAdjacentWidget(string parent, string name, AdjacentHTMLPosition position, Action<LayoutWriter> content)
		{
			var w = new AdjacentWidgetPostRendered { Name = _namefunc(name), Parent = parent, RenderAction = content,
				Action = "adjacent", Position = position.ToString(), IDPrefix = _idprefix };
			Widgets.Add(w);
			_widgetsToRender.Add(w);
			return this;
		}
		#endregion

		public override string Serialize(ActionContext context)
		{
			var container = context.GetArg("c-type");
			if (!container.IsEmpty())
			{
				var coll = context.RequestServices.GetService(typeof(IContainerCollection)) as IContainerCollection;
				if (coll.TryGetValue(container, out var item))
				{
					var id = context.GetArg("c-id");
					//var prefix = context.GetArg("c-prefix");

					var mappings = item.Mapping.ToDictionary(
						o => o.Key,
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
			{
				foreach (var widget in _widgetsToRender)
					widget.Render(context, ClientActions, Includes);

				Data.Add("widgets", Widgets);
			}
			if (ClientActions.Count > 0)
				Data.Add("clientactions", ClientActions);
			if (Includes.Count > 0)
				Data.Add("includes", Includes.Select(o => GlobalSettings.JSPath + o));

			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}

		static LayoutWriter RenderContent(ActionContext context, string prefix, Action<LayoutWriter> content)
		{
			var w = new LayoutWriter(context, prefix);
			content?.Invoke(w);
			return w;
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

	public class ContentWidgetPostRendered : ContentWidget
	{
		[JsonIgnore]
		public Action<LayoutWriter> RenderAction { get; set; }
		[JsonIgnore]
		public string IDPrefix { get; set; }

		public void Render(ActionContext context, ICollection<ClientAction> clientActions, ICollection<string> includes)
		{
			var w = new LayoutWriter(context, IDPrefix);
			RenderAction?.Invoke(w);
			Content = w.ToString();

			foreach (var i in w.ClientActions)
				clientActions.Add(i);
			foreach (var i in w.Includes)
				includes.Add(i);
		}
	}

	public class AdjacentWidgetPostRendered : ContentWidgetPostRendered
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