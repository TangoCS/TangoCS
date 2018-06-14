using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Tango.Html;
using System.Net;

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
		class WidgetToRender
		{
			public ContentWidget widget;
			public ActionContext context;
			public string prefix;
			public Action<LayoutWriter> content;
		}

		public List<IWidget> Widgets { get; set; } = new List<IWidget>();
		public List<ClientAction> ClientActions { get; set; } = new List<ClientAction>();
		public HashSet<string> Includes { get; set; } = new HashSet<string>();

		List<WidgetToRender> _widgetsToRender = new List<WidgetToRender>();
		string _idprefix = "";
		IDictionary<string, string> _mapping;
		Func<string, string> _namefunc = name => name.ToLower();

		public ApiResponse()
		{

		}

		public ApiResponse(IInteractionFlowElement element)
		{
			var context = element.Context;

			ViewContainer containerObj = null;

			if (context.AddContainer)
			{
				if (!context.ContainerType.IsEmpty())
				{
					var t = new ContainersCache().Get(context.ContainerType);
					if (t != null)
						containerObj = Activator.CreateInstance(t) as ViewContainer;
				}
				else if (element is IContainerItem)
				{
					containerObj = (element as IContainerItem).GetDefaultContainer();
				}
			}

			if (containerObj != null && element is IViewElement)
			{
				containerObj.Context = context;

				var ve = element as IViewElement;
			
				containerObj.ID = context.ContainerPrefix;
				ve.ParentElement = containerObj;

				_mapping = containerObj.Mapping;
				_idprefix = context.ContainerPrefix;

				_namefunc = name => HtmlWriterHelpers.GetID(context.ContainerPrefix, name);
				containerObj.Render(this);
				_namefunc = name => _mapping.ContainsKey(name) ? HtmlWriterHelpers.GetID(context.ContainerPrefix, _mapping[name]) : name;
			}

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
			WithWritersFor(view);
			content();
			_idprefix = lastPrefix;
			return this;
		}

		public ApiResponse WithNamesFor(IViewElement view)
		{
			if (view != null)
				_namefunc = name => view.GetClientID(name);
			else
				_namefunc = name => name.ToLower();
			return this;
		}

		public ApiResponse WithNamesFor(IViewElement view, Action content)
		{
			var lastNamefunc = _namefunc;
			WithNamesFor(view);
			content();
			_namefunc = lastNamefunc;
			return this;
		}

		public ApiResponse WithRootWriters(Action content) => WithWritersFor(null, content);
		public ApiResponse WithRootNames(Action content) => WithNamesFor(null, content);
		public ApiResponse WithNamesAndWritersFor(IViewElement view) => WithNamesFor(view).WithWritersFor(view);

		public void Insert(ApiResponse resp)
		{
			for (int i = resp.Widgets.Count - 1; i >= 0; i--)
				Widgets.Insert(0, resp.Widgets[i]);

			for (int i = resp._widgetsToRender.Count - 1; i >= 0; i--)
				_widgetsToRender.Insert(0, resp._widgetsToRender[i]);

			for (int i = resp.ClientActions.Count - 1; i >= 0; i--)
				ClientActions.Insert(0, resp.ClientActions[i]);
		}

		public void AddClientAction(string service, string method, Func<Func<string, string>, object> args)
		{
			var resolvedArgs = args != null ? args(_namefunc) : null;
			ClientActions.Add(new ClientAction(service, method, resolvedArgs));
		}

		public void AddClientAction(ClientAction action)
		{
			ClientActions.Add(action);
		}

		public void RedirectBack(ActionContext context)
		{
			if (context.ReturnTarget == null) return;
			var retctx = context.ReturnTargetContext();

			var cache = retctx.RequestServices.GetService(typeof(ITypeActivatorCache)) as ITypeActivatorCache;
			(var type, var invoker) = cache?.Get(retctx.Service + "." + retctx.Action) ?? (null, null);

			
			var result = invoker?.Invoke(retctx, type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };

			if (result is AjaxResult ajax && ajax.ApiResponse is ApiResponse resp)
			{
				for (int i = resp._widgetsToRender.Count - 1; i >= 0; i--)
					resp._widgetsToRender[i].context = retctx;
				Insert(resp);
			}

			Data.Add("redirect", new {
				Url = WebUtility.UrlDecode(context.ReturnUrl),
				retctx.Service,
				retctx.Action,
				Parms = retctx.AllArgs
			});
		}

		#region dom actions
		public virtual void SetElementValue(string id, string value)
		{
			AddClientAction("domActions", "setValue", f => new { id = f(id), value });
		}

		public virtual void SetElementVisibility(string id, bool visible)
		{
			AddClientAction("domActions", "setVisible", f => new { id = f(id), visible });
		}

		public virtual void SetElementAttribute(string id, string attrName, string attrValue)
		{
			AddClientAction("domActions", "setAttribute", f => new { id = f(id), attrName, attrValue });
		}

		public virtual void RemoveElementAttribute(string id, string attrName)
		{
			AddClientAction("domActions", "removeAttribute", f => new { id = f(id), attrName });
		}

		public virtual void SetElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "setClass", f => new { id = f(id), clsName });
		}

		public virtual void RemoveElementClass(string id, string clsName)
		{
			AddClientAction("domActions", "removeClass", f => new { id = f(id), clsName });
		}
		#endregion	

		#region main widget methods, string content
		public virtual ApiResponse AddWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget{ Name = _namefunc(name), Content = content, Action = WidgetAction.Add });
			return this;
		}

		public virtual ApiResponse ReplaceWidget(string name, string content)
		{
			Widgets.Add(new ContentWidget { Name = _namefunc(name), Content = content, Action = WidgetAction.Replace });
			return this;
		}

		public virtual ApiResponse RemoveWidget(string name)
		{
			Widgets.Add(new Widget { Name = _namefunc(name), Action = WidgetAction.Remove });
			return this;
		}

		public virtual ApiResponse AddRootWidget(string name, string content)
		{
			AddAdjacentWidget(null, name, content);
			return this;
		}

		public virtual ApiResponse AddAdjacentWidget(string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			Widgets.Add(new AdjacentWidget { Name = _namefunc(name), Parent = parent, Content = content, Action = WidgetAction.Adjacent, Position = position });
			return this;
		}
		#endregion

		#region root widget, no prefix, content = action
		public ApiResponse AddWidget(string name, Action<LayoutWriter> content)
		{
			var w = new ContentWidget { Name = _namefunc(name), Action = WidgetAction.Add };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}

		public ApiResponse ReplaceWidget(string name, Action<LayoutWriter> content)
		{
			var w = new ContentWidget { Name = _namefunc(name), Action = WidgetAction.Replace };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
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
			var w = new AdjacentWidget { Name = _namefunc(name), Parent = parent, Action = WidgetAction.Adjacent, Position = position };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}
		#endregion

		public override string Serialize(ActionContext context)
		{
			if (Widgets.Count > 0)
			{
				foreach (var r in _widgetsToRender)
				{
					var w = new LayoutWriter(r.context ?? context, r.prefix);
					r.content?.Invoke(w);
					r.widget.Content = w.ToString();

					foreach (var i in w.ClientActions)
						ClientActions.Add(i);
					foreach (var i in w.Includes)
						Includes.Add(i);
				}

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

	public enum WidgetAction
	{
		Add,
		Remove,
		Replace,
		Adjacent
	}

	public interface IWidget
	{
		string Name { get; set; }
	}

	public class Widget : IWidget
	{
		public string Name { get; set; }
		public WidgetAction Action { get; set; }
		
	}
	public class ContentWidget : Widget
	{
		public string Content { get; set; }
	}

	public class AdjacentWidget : ContentWidget
	{
		public AdjacentHTMLPosition Position { get; set; }
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

		public class ChainElement
		{
			public string Method { get; set; }
			public object Args { get; set; }
		}
	}
}