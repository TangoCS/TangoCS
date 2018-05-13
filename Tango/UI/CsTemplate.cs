using System;
using Tango.Localization;
using Newtonsoft.Json;
using Tango.Html;
using System.Collections.Generic;
using System.Reflection;

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

	public interface IWithCheckAccess
	{
		bool CheckAccess(MethodInfo method);
	}

	public abstract class InteractionFlowElement : IInteractionFlowElement, IWithPropertyInjection
	{
		public virtual string ID { get; set; }
		public ActionContext Context { get; set; }

		public virtual bool UsePropertyInjection { get { return false; } }

		public IResourceManager Resources => Context.Resources;
		protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		public T GetPosted<T>(string name, T defaultValue = default(T))
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
			var s = GetPosted<string>(name);
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
				ClientID = _parentElement?.GetClientID(value) ?? value?.ToLower();
			}
		}

		public string ClientID { get; private set; }
		public string Name => ClientID.IsEmpty() ? GetType().Name.ToLower() : ClientID;

		IViewElement _parentElement;
		public IViewElement ParentElement {
			get => _parentElement;
			set {
				_parentElement = value;
				ClientID = _parentElement?.GetClientID(ID) ?? ID?.ToLower();
			}
		}

		public virtual void OnInit() { }
		public virtual void AfterInit() { }

		public string GetClientID(string id)
		{
			return (!ClientID.IsEmpty() ? ClientID + (!id.IsEmpty() ? "_" + id : "") : id).ToLower();
		}

		public T CreateControl<T>(string id, Action<T> setProperties = null)
			where T : ViewElement, new()
		{
			T c = new T() { Context = Context };
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.ID = id;
			c.ParentElement = this;

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

	public abstract class ViewRootElement : ViewElement
	{
		public abstract ActionResult Execute();	
	}

	public delegate void ViewElementEventHandler(ApiResponse response);
}
