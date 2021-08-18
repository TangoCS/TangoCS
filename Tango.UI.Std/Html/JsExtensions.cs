using System;
using System.Collections.Generic;
using Tango.UI;

namespace Tango.Html
{
	public static class JsExtensions
	{
		public static ATagAttributes OnClickHideShow(this ATagAttributes a, string id)
		{
			return a.OnClick($"domActions.hideShow('{a.Writer.GetID(id)}')");
		}

		public static T OnKeyUpRunHrefDelayed<T>(this TagAttributes<T> a)
			where T : TagAttributes<T>
		{
			return a.OnKeyUp("ajaxUtils.delay(this, function(caller) { ajaxUtils.runHrefWithApiResponse(caller); })");
		}
		
		public static T OnKeyUpPostHrefDelayed<T>(this TagAttributes<T> a)
			where T : TagAttributes<T>
		{
			return a.OnKeyUp("ajaxUtils.delay(this, function(caller) { ajaxUtils.postEventFromElementWithApiResponse(caller); })");
		}
		
		public static T OnKeyUpRunHrefDelayed<T>(this TagAttributes<T> a, int timeout)
			where T : TagAttributes<T>
		{
			return a.OnKeyUp("ajaxUtils.delay(this, function(caller) { ajaxUtils.runHrefWithApiResponse(caller); }, " +
			                 $"{timeout})");
		}

		public static T OnClickRunHref<T>(this T a, Action<ApiResponse> action = null)
			where T : TagAttributes<T>
		{
			if (action != null)
				a.DataEvent(action);
			return a.OnClick("ajaxUtils.runHrefWithApiResponse(this); return false;");
		}

		public static T OnChangeRunHref<T>(this T a)
			where T : TagAttributes<T>
		{
			return a.OnChange("ajaxUtils.runHrefWithApiResponse(this); return false;");
		}

		public static T OnClickRunEvent<T>(this TagAttributes<T> a, Action<ApiResponse> action = null)
			where T : TagAttributes<T>
		{
			if (action != null)
				a.DataEvent(action);
			return a.OnClick("ajaxUtils.runEventFromElementWithApiResponse(this); return false;");
		}

		public static T OnClickRunEvent<T>(this TagAttributes<T> a, string name, string receiver = null)
			where T : TagAttributes<T>
		{
			if (receiver != null) a.Data("r", receiver);
			return a.Data("e", name).OnClick("ajaxUtils.runEventFromElementWithApiResponse(this); return false;");
		}

	
		public static T OnClickPostEvent<T>(this TagAttributes<T> a)
			where T : TagAttributes<T>
		{
			return a.OnClick("ajaxUtils.postEventFromElementWithApiResponse(this); return false;");
		}
        public static T OnClickClipboardToElementId<T>(this TagAttributes<T> a, string id)
            where T : TagAttributes<T>
        {
            return a.OnClick($"commonUtils.clipboardToElementId('#{id}', this);");
        }
        public static T OnClickClipboardToElementIdAndSubmit<T>(this TagAttributes<T> a, string id)
	        where T : TagAttributes<T>
        {
	        return a.OnClick($"commonUtils.clipboardToElementIdAndSubmit('#{id}', this);");
        }

        public static T OnClickPostEvent<T>(this TagAttributes<T> a, Action<ApiResponse> action)
			where T : TagAttributes<T>
		{
			return a.DataEvent(action).OnClickPostEvent();
		}

		public static T OnClickPostEvent<T>(this TagAttributes<T> a, Func<ActionResult> func)
			where T : TagAttributes<T>
		{
			return a.DataEvent(func).OnClickPostEvent();
		}

		public static T OnClickPostEvent<T>(this TagAttributes<T> a, string name, string receiver = null)
			where T : TagAttributes<T>
		{
			if (receiver != null) a.Data("r", receiver);
			return a.Data("e", name).OnClickPostEvent();
		}

		public static SelectTagAttributes OnChangePostEvent(this SelectTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnChange("ajaxUtils.postEventFromElementWithApiResponse(this)");
		}

		public static SelectTagAttributes OnChangePostEvent(this SelectTagAttributes a, string name, string receiver = null)
		{
			if (receiver != null) a.Data("r", receiver);
			return a.Data("e", name).OnChange("ajaxUtils.postEventFromElementWithApiResponse(this)");
		}

		public static SelectTagAttributes OnChangeRunEvent(this SelectTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnChange("ajaxUtils.runEventFromElementWithApiResponse(this)");
		}

		public static InputTagAttributes OnChangePostEvent(this InputTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnChange("ajaxUtils.postEventFromElementWithApiResponse(this)");
		}

		public static InputTagAttributes OnChangePostEvent(this InputTagAttributes a, string name, string receiver = null)
		{          
            if (receiver != null) a.Data("r", receiver);
            return a.Data("e", name).OnChange("ajaxUtils.postEventFromElementWithApiResponse(this)");
        }

		public static InputTagAttributes OnEnterPostEvent(this InputTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnKeyUp("if (event.key=='Enter') ajaxUtils.postEventFromElementWithApiResponse(this)");
		}

		public static InputTagAttributes OnInputPostEvent(this InputTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnInput("ajaxUtils.postEventFromElementWithApiResponse(this)");
		}


		public static InputTagAttributes OnChangeRunEvent(this InputTagAttributes a, Action<ApiResponse> action)
		{
			return a.DataEvent(action).OnChange("ajaxUtils.runEventFromElementWithApiResponse(this)");
		}

		public static T DataParm<T, TValue>(this TagAttributes<T> a, string key, TValue value)
			where T : TagAttributes<T>
		{
			a.Data("p-" + key.ToString().ToLower(), value);
			return a as T;
		}

		public static T DataParm<T>(this TagAttributes<T> a, params (string key, object value)[] args)
			where T : TagAttributes<T>
		{
			foreach (var (key, value) in args)
				a.Data("p-" + key.ToString().ToLower(), value);
			return a as T;
		}

		public static T DataParm<T, TValue>(this TagAttributes<T> a, IEnumerable<KeyValuePair<string, TValue>> args)
			where T : TagAttributes<T>
		{
			foreach (var arg in args)
				a.Data("p-" + arg.Key.ToLower(), arg.Value);

			return a as T;
		}

		public static T DataRef<T>(this TagAttributes<T> a, string id)
			where T : TagAttributes<T>
		{
			if (id.StartsWith("#"))
				return a.Data("ref-" + id.Substring(1), id.Substring(1));
			else
				return a.DataIDValue("ref-" + id, id);
		}

		public static T DataRef<T>(this TagAttributes<T> a, IViewElement owner, string id)
			where T : TagAttributes<T>
		{
			var clientid = owner.GetClientID(id);
			return a.Data("ref-" + clientid, clientid);
		}

		public static T DataRef<T>(this TagAttributes<T> a, IViewElement element)
			where T : TagAttributes<T>
		{
			return a.Data("ref-" + element.ClientID, element.ClientID);
		}

		public static T DataFormat<T>(this TagAttributes<T> a, string format)
			where T : TagAttributes<T>
		{
			return a.Data("format", format);
		}

		public static T DataHref<T>(this TagAttributes<T> a, string url)
			where T : TagAttributes<T>
		{
			return a.Data("href", url);
		}

		public static T DataHref<T>(this TagAttributes<T> a, ActionContext context, Action<ActionLink> attrs)
			where T : TagAttributes<T>
		{
			var link = new ActionLink(context);
			attrs(link);
			return a.Data("href", link.Url);
		}

		//public static T DataAction<T>(this TagAttributes<T> a, string service, string action)
		//	where T : TagAttributes<T>
		//{
		//	return a.Data("s", service).Data("a", action);
		//}

		public static T DataNewContainer<T>(this TagAttributes<T> a, string type, string prefix = null)
			where T : TagAttributes<T>
		{
			return a.Data(Constants.ContainerType, type.ToLower())
				.Data(Constants.ContainerPrefix, prefix?.ToLower())
				.Data(Constants.ContainerNew, "1");
		}

		public static T DataNewContainer<T>(this TagAttributes<T> a, Type type, string prefix = null)
			where T : TagAttributes<T>
		{
			return a.Data(Constants.ContainerType, type.Name.Replace("Container", "").ToLower())
				.Data(Constants.ContainerPrefix, prefix?.ToLower())
				.Data(Constants.ContainerNew, "1");
		}

		public static T DataIsContainer<T>(this TagAttributes<T> a, string type, string prefix = null)
			where T : TagAttributes<T>
		{
			return a.Data(Constants.Container)
				.Data(Constants.ContainerType, type.ToLower())
				.Data(Constants.ContainerPrefix, prefix?.ToLower());
		}

		public static T DataIsContainer<T>(this TagAttributes<T> a, Type type, string prefix = null)
			where T : TagAttributes<T>
		{
			return a.Data(Constants.Container)
				.Data(Constants.ContainerType, type.Name.Replace("Container", "").ToLower())
				.Data(Constants.ContainerPrefix, prefix?.ToLower());
		}

		public static T DataContainerExternal<T>(this TagAttributes<T> a, string externalElementId)
			where T : TagAttributes<T>
		{
			return a.Data(Constants.ContainerExternal, externalElementId.ToLower());
		}

		public static T DataEvent<T>(this TagAttributes<T> a, string name, string receiver = null)
			where T : TagAttributes<T>
		{
			if (receiver != null) a.Data("r", receiver);
			return a.Data("e", name);
		}

		public static T DataEvent<T>(this TagAttributes<T> a, Action<ApiResponse> action)
			where T : TagAttributes<T>
		{
			if (!(action.Target is ViewElement el))
				throw new InvalidCastException("Invalid class type for action.Target; must be of type ViewElement");

			if (!el.ClientID.IsEmpty()) a.Data("r", el.ClientID);
			return a.Data("e", action.Method.Name.ToLower());
		}

		public static T DataEvent<T>(this TagAttributes<T> a, Func<ActionResult> func)
			where T : TagAttributes<T>
		{
			if (!(func.Target is ViewElement el))
				throw new InvalidCastException("Invalid class type for func.Target; must be of type ViewElement");

			if (!el.ClientID.IsEmpty()) a.Data("r", el.ClientID);
			return a.Data("e", func.Method.Name.ToLower());
		}

		public static T DataReceiver<T>(this TagAttributes<T> a, IViewElement element)
			where T : TagAttributes<T>
		{
			return a.Data("r", element.ClientID);
		}

		public static T DataCtrl<T>(this TagAttributes<T> a, string ctrlType, string ctrlId = null)
			where T : TagAttributes<T>
		{
			if (ctrlId != null) a.Data("ctrl-id", ctrlId);
			return a.Data("ctrl", ctrlType);
		}

		public static T DataResult<T>(this TagAttributes<T> a, int code)
			where T : TagAttributes<T>
		{
			return a.Data("res", code);
		}

		public static T DataResultPostponed<T>(this TagAttributes<T> a, int code)
			where T : TagAttributes<T>
		{
			return a.Data("res-postponed", code);
		}

		public static T DataResultHandler<T>(this TagAttributes<T> a)
			where T : TagAttributes<T>
		{
			return a.Data("res-handler", 1);
		}

		public static T DataHasClientState<T>(this TagAttributes<T> a, ClientStateType type = ClientStateType.Value, string ownerControl = null, string varName = null)
			where T : TagAttributes<T>
		{	
			if (ownerControl != null) a.Data("clientstate-owner", ownerControl);
			if (varName != null) a.Data("clientstate-name", varName);
			return a.Data("hasclientstate", type.ToString().ToLower());
		}


	}

	public enum ClientStateType
	{
		Value,
		Array
	}
}
