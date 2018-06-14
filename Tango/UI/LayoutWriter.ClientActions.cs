using System;
using Tango.Html;

namespace Tango.UI
{
	public static class LayoutWriterClientActionsExtensions
	{
		public static void AddClientAction(this LayoutWriter w, string service, string method, Func<Func<string, string>, object> args)
		{
			var resolvedArgs = args != null ? args(id => w.GetID(id)) : null;
			w.ClientActions.Add(new ClientAction(service, method, resolvedArgs));
		}

		public static void AddClientAction(this LayoutWriter w, string service, Func<Func<string, string>, object> args)
		{
			w.AddClientAction(service, "apply", args);
		}

		public static void AddClientAction(this LayoutWriter w, string service, string method, Func<Func<string, string>, object> args, params (string method, Func<Func<string, string>, object> args)[] then)
		{
			var resolvedArgs = args != null ? args(id => w.GetID(id)) : null;
			var ca = new ClientAction(service, method, resolvedArgs);
			w.ClientActions.Add(ca);
			
			if (then != null && then.Length > 0)
			{
				foreach (var ch in then)
				{
					var resolvedChainArgs = ch.args != null ? ch.args(id => w.GetID(id)) : null;
					ca.CallChain.Add(new ClientAction.ChainElement { Method = ch.method, Args = resolvedChainArgs });
				}
			}
		}

		public static void AddClientAction(this LayoutWriter w, string service, Func<Func<string, string>, object> args, params (string method, Func<Func<string, string>, object> args)[] then)
		{
			w.AddClientAction(service, "apply", args, then);
		}

		public static void BindEventGet(this LayoutWriter w, string elementId, string clientEvent, Action<ApiResponse> serverEvent)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			w.BindEvent(elementId, clientEvent, serverEvent.Method.Name, "get", el.ClientID);
		}

		public static void BindEventGet(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			w.BindEvent(elementId, clientEvent, serverEvent, "get", serverEventReceiver);
		}

		public static void BindEventPost(this LayoutWriter w, string elementId, string clientEvent, Action<ApiResponse> serverEvent)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			w.BindEvent(elementId, clientEvent, serverEvent.Method.Name, "post", el.ClientID);
		}

		public static void BindEventPost(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			w.BindEvent(elementId, clientEvent, serverEvent, "post", serverEventReceiver);
		}

		static void BindEvent(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string method, string serverEventReceiver = null)
		{
			w.AddClientAction("ajaxUtils", "bindevent", f => new {
				Id = f(elementId), ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver,
				Method = method
			});
		}

		//static string GetClientId(LayoutWriter w, string name)
		//{
		//	return (!w.IDPrefix.IsEmpty() ? w.IDPrefix + "_" + name : name).ToLower();
		//}
	}
}
