using Nephrite.Templating;

namespace Nephrite.Html.Layout
{
	public static class LayoutWriterClientActionsExtensions
	{
		public static void AddClientAction(this LayoutWriter w, string service, string method, object args)
		{
			w.ClientActions.Add(new ClientAction(service, method, args));
		}

		public static void BindEvent(this LayoutWriter w, string elementId, string clientEvent, string serverEvent, string serverEventReceiver = null)
		{
			w.ClientActions.Add(new ClientAction("ajaxUtils", "bindevent", new {
				Id = GetTrueName(w, elementId), ClientEvent = clientEvent,
				ServerEvent = serverEvent, ServerEventReceiver = serverEventReceiver
			}));
		}

		static string GetTrueName(LayoutWriter w, string name)
		{
			return !w.IDPrefix.IsEmpty() ? w.IDPrefix + "_" + name : name;
		}
	}
}
