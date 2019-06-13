using System;
using Tango.Localization;
using Newtonsoft.Json;
using Tango.Html;
using System.Collections.Generic;
using System.Reflection;
using System.Net;

namespace Tango.UI
{
	public interface IInteractionFlowElement
	{
		string ID { get; set; }
		ActionContext Context { get; set; }
	}

	public interface IViewElement : IInteractionFlowElement
	{
		string ClientID { get; }
		IViewElement ParentElement { get; set; }
		DataCollection DataCollection { get; set; }
		string GetClientID(string id);
	}

	public interface IContainerItem
	{
		ViewContainer GetContainer();
	}

	public interface IWithCheckAccess
	{
		bool CheckAccess(MethodInfo method);
	}

	public abstract class InteractionFlowElement : IInteractionFlowElement, IWithPropertyInjection
	{
		public virtual string ID { get; set; }
		public ActionContext Context { get; set; }

		public virtual bool UsePropertyInjection => true;

		public IResourceManager Resources => Context.Resources;
		protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		public T GetPosted<T>(string name, T defaultValue = default)
		{
			return Context.FormData.Parse<T>(name, defaultValue);
		}

		public List<T> GetPostedList<T>(string name)
		{
			return Context.FormData.ParseList<T>(name);
		}

		public DateTime? GetPostedDateTime(string name, string format, DateTime? defaultValue = null)
		{
			return Context.FormData.ParseDateTime(name, format);
		}

		public DateTime GetPostedDateTime(string name, string format, DateTime defaultValue)
		{
			return Context.FormData.ParseDateTime(name, format, defaultValue);
		}

		public T GetPostedJson<T>(string name, Func<T> defaultValue = null)
		{
			var s = GetArg<string>(name);
			s = WebUtility.HtmlDecode(s);
			T res = default(T);
			if (!s.IsEmpty()) res = JsonConvert.DeserializeObject<T>(s);
			if (res == null && defaultValue != null) res = defaultValue();
			return res;
		}

		public string GetArg(string name)
		{
			return Context.GetArg(name);
		}

		public T GetArg<T>(string name)
		{
			return Context.GetArg<T>(name);
		}
	}

	public abstract class ViewElement : InteractionFlowElement, IViewElement
	{
		public DataCollection DataCollection { get; set; } = new DataCollection();

		public override string ID {
			get => base.ID;
			set {
				base.ID = value;
				if (value != null)
					ClientID = _parentElement?.GetClientID(value) ?? value.ToLower();
				else
					ClientID = null;
			}
		}

		public string ClientID { get; private set; }
		public string Name => ClientID.IsEmpty() ? GetType().Name.ToLower() : ClientID;

		IViewElement _parentElement;
		public IViewElement ParentElement {
			get => _parentElement;
			set {
				_parentElement = value;
				if (ID != null) ClientID = _parentElement?.GetClientID(ID) ?? ID.ToLower();
			}
		}

		public virtual void OnInit() { }
		public virtual void AfterInit() { }

		public string GetClientID(string id)
		{
			return (!ClientID.IsEmpty() ? ClientID + (!id.IsEmpty() ? "_" + id : "") : (id ?? "")).ToLower();
		}

		public T CreateControl<T>(string id, Action<T> setProperties = null)
			where T : ViewElement, new()
		{
			T c = new T() { Context = Context };
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.ID = id;
			c.ParentElement = this;

			if (!Context.EventReceivers.ContainsKey(c.ClientID))
				Context.EventReceivers.Add(c.ClientID, c);

			setProperties?.Invoke(c);
			c.OnInit();
			c.AfterInit();
			return c;
		}

		public T AddControl<T>(T c)
			where T : ViewElement
		{
			if (Context.EventReceivers.ContainsKey(c.ID)) return c;

			c.Context = Context;
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.ParentElement = this;

			Context.EventReceivers.Add(c.ClientID, c);
			c.OnInit();
			c.AfterInit();
			return c;
		}
	}

	public abstract class ViewComponent : ViewElement
	{
		
	}

	public abstract class ViewContainer : ViewElement
	{
		public string Type => GetType().Name.Replace("Container", "");

		public IDictionary<string, string> Mapping { get; } = new Dictionary<string, string>();

		public abstract void Render(ApiResponse response);

		public void ProcessResponse(ActionContext ctx, ApiResponse response)
		{
			OnInit();

			if (ctx.AddContainer)
			{
				response.WithNameFunc(name => Mapping.ContainsKey(name) ?
					HtmlWriterHelpers.GetID(ctx.ContainerPrefix, Mapping[name]) :
					HtmlWriterHelpers.GetID(ctx.ContainerPrefix, name));
				response.WithWritersFor(this, () => Render(response));
			}

			response.WithNameFunc(name => 
				Mapping.ContainsKey(name) ?
				HtmlWriterHelpers.GetID(ClientID, Mapping[name]) :
				ClientID == name ? name : HtmlWriterHelpers.GetID(ClientID, name));
		}
	}

	public abstract class ViewRootElement : ViewElement
	{
		public abstract ActionResult Execute();	
	}

	public delegate void ViewElementEventHandler(ApiResponse response);

	public class ContainersCache : ITypeObserver
	{
		static readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		protected List<InvokeableTypeInfo> typeInfos = new List<InvokeableTypeInfo>();

		public void LookOver(Type t)
		{
			if (t.IsSubclassOf(typeof(ViewContainer)) && !t.IsAbstract)
				_typeCache.Add(t.Name.Replace("Container", "").ToLower(), t);
		}

		public Type Get(string key)
		{
			if (_typeCache.TryGetValue(key.ToLower(), out Type ret))
				return ret;
			else
				return null;
		}
	}
}
