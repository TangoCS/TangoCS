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
			public bool nested = false;
		}

		Dictionary<string, CtrlInfo> _ctrl = new Dictionary<string, CtrlInfo>();
		Dictionary<string, List<string>> _argGroups = new Dictionary<string, List<string>>();

		//public List<Widget> Widgets { get; set; } = new List<Widget>();
		public List<ClientAction> ClientActions { get; set; } = new List<ClientAction>();

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
			//for (int i = resp.Widgets.Count - 1; i >= 0; i--)
			//	Widgets.Insert(0, resp.Widgets[i]);

			for (int i = resp._widgetsToRender.Count - 1; i >= 0; i--)
				_widgetsToRender.Insert(0, resp._widgetsToRender[i]);

			for (int i = resp.ClientActions.Count - 1; i >= 0; i--)
				ClientActions.Insert(0, resp.ClientActions[i]);

			foreach (var kv in resp._ctrl)
				_ctrl.Add(kv.Key, kv.Value);
		}

		public void Add(ApiResponse resp)
		{
			//Widgets.AddRange(resp.Widgets);
			ClientActions.AddRange(resp.ClientActions);
			_widgetsToRender.AddRange(resp._widgetsToRender);
			foreach (var kv in resp._ctrl)
				_ctrl.Add(kv.Key, kv.Value);
		}

		public void ReplaceWith(ApiResponse resp)
		{
			//Widgets = resp.Widgets;
			ClientActions = resp.ClientActions;
			_widgetsToRender = resp._widgetsToRender;
			_ctrl = resp._ctrl;
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

		/// <summary>
		/// Возврат на предыдущую форму
		/// </summary>
		/// <param name="context"></param>
		/// <param name="code"></param>
		/// <param name="changeLoc">Меняется ли адрес, или мы возвращаемся внутри этой же страницы (из модального окна/панели)</param>
		public void RedirectBack(ActionContext context, int code, bool changeLoc = true)
		{
			var retctx = context.ReturnTargetContext(code);
			if (retctx == null) return;

			if (context.ReturnState != null)
			{
				var returnState = JsonConvert.DeserializeObject<Dictionary<string, string>>(context.ReturnState);
				foreach (var kv in returnState)
				{
					retctx.AllArgs[kv.Key] = kv.Value;
					retctx.FormData[kv.Key] = kv.Value;
				}
			}

			RunRedirect(retctx);
			if (retctx.AddContainer)
				RedirectTo(retctx.BaseUrl().Url, retctx.AllArgs, changeLoc);
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

		public void RedirectTo(string url, DynamicDictionary parms = null, bool isBack = false)
		{
			Data.Add("redirect", new { url, parms, isBack });
		}

		public void HardRedirectTo(string url)
		{
			Data.Add("hardredirect", new { url });
		}

		public void SetCtrlState(string clientid, object state)
		{
			if (_ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.State = state;
			else
				_ctrl.Add(clientid, new CtrlInfo { State = state });
		}
		public void SetCtrlProps(string clientid, object props)
		{
			if (_ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.Props = props;
			else
				_ctrl.Add(clientid, new CtrlInfo { Props = props });
		}
		public void SetCtrlInstance(string clientid, object instance)
		{
			if (_ctrl.TryGetValue(clientid, out var ctrl))
				ctrl.Instance = instance;
			else
				_ctrl.Add(clientid, new CtrlInfo { Instance = instance });
		}

		public void SetArgGroup(string groupName, List<string> elementArgNames)
		{
			if (_argGroups.TryGetValue(groupName, out var names))
				names.AddRange(elementArgNames);
			else
				_argGroups.Add(groupName, new List<string>(elementArgNames));
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
			var n = _namefunc(name);
			var w = new ContentWidget { Name = n, Content = content, Action = WidgetAction.Add };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w });
			return this;
		}

		public virtual ApiResponse ReplaceWidget(string name, string content)
		{
			var n = _namefunc(name);
			var w = new ContentWidget { Name = n, Content = content, Action = WidgetAction.Replace };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w });
			return this;
		}

		public virtual ApiResponse RemoveWidget(string name)
		{
			var n = _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.Remove };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w });
			return this;
		}

		public virtual ApiResponse AddRootWidget(string name, string content)
		{
			AddAdjacentWidget(null, name, content);
			return this;
		}

		public virtual ApiResponse AddAdjacentWidget(string parent, string name, string content, AdjacentHTMLPosition position = AdjacentHTMLPosition.BeforeEnd)
		{
			var n = _namefunc(name);
			var w = new AdjacentWidget { Name = n, Parent = parent, Content = content, Action = WidgetAction.Adjacent, Position = position };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w });
			return this;
		}
		#endregion

		#region root widget, no prefix, content = action
		public ApiResponse AddWidget(string name, Action<LayoutWriter> content)
		{
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.Add };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}

		public ApiResponse AddShadowWidget(string name, Action<LayoutWriter> content)
		{
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.AddShadow };
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}

		public ApiResponse ReplaceWidget(string name, Action<LayoutWriter> content)
		{
			var n = name?.StartsWith("#") ?? false ? name.Substring(1) : _namefunc(name);
			var w = new ContentWidget { Name = n, Action = WidgetAction.Replace };
			//Widgets.Add(w);
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
			//Widgets.Add(w);
			_widgetsToRender.Add(new WidgetToRender { widget = w, prefix = _idprefix, content = content });
			return this;
		}
		#endregion

		public void ApplyTo(ActionContext context, HtmlWriter pageWriter)
		{
			var json = Serialize(context, pageWriter);
		}

		string Serialize(ActionContext context, HtmlWriter pageWriter)
		{
			try
			{
				List<Widget> widgets = new List<Widget>();

				if (_widgetsToRender.Count > 0)
				{
					foreach (var r in _widgetsToRender)
					{
						HtmlWriter w = null;
						if (r.content != null)
						{
							var lw = new LayoutWriter(r.context ?? context, r.prefix);
							r.content.Invoke(lw);
							if (pageWriter == null)
								r.widget.Content = lw.ToString();

							foreach (var i in lw.ClientActions)
								ClientActions.Add(i);
							foreach (var i in lw.Ctrl)
								_ctrl.Add(i.Key, i.Value);
							w = lw;
						}

						if (pageWriter != null)
						{
							if (r.content == null)
							{
								w = new HtmlWriter();
								w.Write(r.widget.Content);
							}

							switch (r.widget.Action)
							{
								case WidgetAction.Add:
									r.nested = pageWriter.AddWidget(r.widget.Name, w);
									break;
								case WidgetAction.Remove:
									break;
								case WidgetAction.Replace:
									r.nested = pageWriter.ReplaceWidget(r.widget.Name, w);
									break;
								case WidgetAction.Adjacent:
									var adjw = r.widget as AdjacentWidget;
									r.nested = pageWriter.AddAdjacentWidget(adjw.Parent, adjw.Position, w);
									break;
								case WidgetAction.AddShadow:
									break;
								default:
									break;
							}
						}

						if (!r.nested)
							widgets.Add(r.widget);
					}

					Data.Add("widgets", widgets);
				}
				if (pageWriter == null && ClientActions.Count > 0)
					Data.Add("clientactions", ClientActions);

				if (_ctrl.Count > 0)
					Data.Add("ctrl", _ctrl);

				if (_argGroups.Count > 0)
					Data.Add("arggroups", _argGroups);
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

		public override string Serialize(ActionContext context) => Serialize(context, null);

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

	public class Widget
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