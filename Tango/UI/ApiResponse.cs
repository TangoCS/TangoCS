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

	public class JsonResponse<TData> : IJsonResponse
	{
		public TData Data { get; set; }
		public virtual string Serialize(ActionContext context)
		{
			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}
	}

	public class ArrayResponse : JsonResponse<List<object>>
	{
		public ArrayResponse()
		{
			Data = new List<object>();
		}
	}

	public class ObjectResponse : JsonResponse<Dictionary<string, object>>
	{
		public ObjectResponse()
		{
			Data = new Dictionary<string, object>();
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
		class CtrlInfo
		{
			public object Instance { get; set; }
			public object State { get; set; }
			public object Props { get; set; }
		}

		Dictionary<string, CtrlInfo> Ctrl = new Dictionary<string, CtrlInfo>(); 

		public List<IWidget> Widgets { get; set; } = new List<IWidget>();
		public List<ClientAction> ClientActions { get; set; } = new List<ClientAction>();
		public HashSet<string> Includes { get; set; } = new HashSet<string>();

		List<WidgetToRender> _widgetsToRender = new List<WidgetToRender>();
		string _idprefix = "";
		Func<string, string> _namefunc = name => name.ToLower();

		public ApiResponse WithWritersFor(string prefix)
		{
			_idprefix = prefix ?? "";
			return this;
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

		public ApiResponse WithNameFunc(Func<string, string> nameFunc)
		{
			_namefunc = nameFunc;
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

		public void Add(ApiResponse resp)
		{
			Widgets.AddRange(resp.Widgets);
			_widgetsToRender.AddRange(resp._widgetsToRender);
			ClientActions.AddRange(resp.ClientActions);
		}

		public void ReplaceWith(ApiResponse resp)
		{
			Widgets = resp.Widgets;
			ClientActions = resp.ClientActions;
			Includes = resp.Includes;
			_widgetsToRender = resp._widgetsToRender;
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

		public void AddClientAction(string service, string method, object args)
		{
			AddClientAction(service, method, f => args);
		}

		public void ChangeUrl(List<string> remove, Dictionary<string, object> add)
		{
			AddClientAction("ajaxUtils", "changeUrl", new { remove, add	});
		}
		public void ChangeUrl(Dictionary<string, object> add)
		{
			AddClientAction("ajaxUtils", "changeUrl", new { add });
		}
		public void ChangeUrl(List<string> remove)
		{
			AddClientAction("ajaxUtils", "changeUrl", new { remove });
		}

		void RunRedirect(ActionContext retctx)
		{
			var cache = retctx.GetService<ITypeActivatorCache>();
			(var type, var invoker) = cache?.Get(retctx.Service + "." + retctx.Action) ?? (null, null);
			var result = invoker?.Invoke(retctx, type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };

			if (result is ApiResult ajax)
			{
				for (int i = ajax.ApiResponse._widgetsToRender.Count - 1; i >= 0; i--)
					ajax.ApiResponse._widgetsToRender[i].context = retctx;
				ReplaceWith(ajax.ApiResponse);
			}
		}

		public void RedirectBack(ActionContext context, int code)
		{
			var retctx = context.ReturnTargetContext(code);
			if (retctx == null) return;
			RunRedirect(retctx);
			if (retctx.AddContainer)
				RedirectTo(retctx.BaseUrl().Url, retctx.AllArgs);
		}

		public void RedirectTo(ActionContext context, Action<ActionLink> action)
		{
			var target = new ActionLink(context);
			action(target);
			var retctx = context.TargetContext(target);
			RunRedirect(retctx);
			if (retctx.AddContainer)
				RedirectTo(target.Url, retctx.AllArgs);
		}

		public void RedirectToCurrent(ActionContext context)
		{
			RedirectTo(context, a => a.ToCurrent());
		}

		public void RedirectTo(string url, DynamicDictionary parms = null)
		{
			Data.Add("redirect", new { url, parms });
		}

		public void HardRedirectTo(string url)
		{
			Data.Add("hardredirect", new { url });
		}

		public void SetCtrlState(string clientid, object state)
		{
			if (Ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.State = state;
			else
				Ctrl.Add(clientid, new CtrlInfo { State = state });
		}
		public void SetCtrlProps(string clientid, object props)
		{
			if (Ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.Props = props;
			else
				Ctrl.Add(clientid, new CtrlInfo { Props = props });
		}
		public void SetCtrlInstance(string clientid, object instance)
		{
			if (Ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.Instance = instance;
			else
				Ctrl.Add(clientid, new CtrlInfo { Instance = instance });
		}


		#region dom actions
		public virtual void SetElementValue(string id, string value)
		{
			AddClientAction("ajaxUtils", "setValue", f => new { id = f(id), value });
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
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.Add };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}

		public ApiResponse AddShadowWidget(string name, Action<LayoutWriter> content)
		{
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.AddShadow };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}

		public ApiResponse ReplaceWidget(string name, Action<LayoutWriter> content)
		{
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.Replace };
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
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var p = parent == null ? null : (parent.StartsWith("#") ? parent.Substring(1) : _namefunc(parent));
			var w = new AdjacentWidget { Name = n, Parent = p, Action = WidgetAction.Adjacent, Position = position };
			Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}
		#endregion

		public override string Serialize(ActionContext context)
		{
			try
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

				if (Ctrl.Count > 0)
					Data.Add("ctrl", Ctrl);
			}
			catch (Exception ex)
			{
                var res = context.RequestServices.GetService(typeof(IErrorResult)) as IErrorResult;
                var message = res?.OnError(ex) ?? ex.ToString();
				Data.Clear();
				Data.Add("error", message);
			}

			return JsonConvert.SerializeObject(Data, Json.StdSettings);
		}

		public bool Success
		{
			get
			{
				if (Data.TryGetValue("success", out var val))
					return (bool)val;
				else
					return true;
			}
			set
			{
				Data["success"] = value;
			}
		}

		//static LayoutWriter RenderContent(ActionContext context, string prefix, Action<LayoutWriter> content)
		//{
		//	var w = new LayoutWriter(context, prefix);
		//	content?.Invoke(w);
		//	return w;
		//}
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
		Adjacent,
		AddShadow
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