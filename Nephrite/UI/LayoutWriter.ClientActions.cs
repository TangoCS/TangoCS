using System;

namespace Nephrite.UI
{
	public static class LayoutWriterClientActionsExtensions
	{
		public static void AddClientAction(this LayoutWriter w, string service, string method, object args)
		{
			w.ClientActions.Add(new ClientAction(service, method, args));
		}

		public static void BindEventGet(this LayoutWriter w, string elementId, string clientEvent, Action<ApiResponse> serverEvent, string serverEventReceiver = null)
		{
			w.BindEvent(elementId, clientEvent, serverEvent.Method.Name, "get", serverEventReceiver);
		}

		public static void BindEventPost(this LayoutWriter w, string elementId, string clientEvent, Action<ApiResponse> serverEvent, string serverEventReceiver = null)
		{
			w.BindEvent(elementId, clientEvent, serverEvent.Method.Name, "post", serverEventReceiver);
		}

		static void BindEvent(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string method, string serverEventReceiver = null)
		{
			w.ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
				Id = GetTrueName(w, elementId), ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver,
				Method = method
			}));
		}

		static string GetTrueName(LayoutWriter w, string name)
		{
			return (!w.IDPrefix.IsEmpty() ? w.IDPrefix + "_" + name : name).ToLower();
		}
	}
}
