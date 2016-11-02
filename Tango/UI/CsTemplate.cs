using System;
using Tango.Localization;
using Newtonsoft.Json;
using Tango.Html;
using System.Collections.Generic;

namespace Tango.UI
{
	public interface IInteractionFlowElement
	{
		string ID { get; set; }
		ActionContext Context { get; set; }
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

	public abstract class ViewElement : InteractionFlowElement
	{
		public DataCollection DataCollection { get; set; } = new DataCollection();

		public string ClientID { get; set; }
		public ViewElement ParentElement { get; set; }

		public virtual LayoutWriter CreateLayoutWriter()
		{
			var w = new LayoutWriter(Context);
			w.IDPrefix = ClientID;
			return w;
		}

		public virtual void OnInit() { }

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
			c.ClientID = GetClientID(id);
			c.ParentElement = this;

			Context.EventReceivers.Add(c.ClientID, c);

			setProperties?.Invoke(c);
			c.OnInit();
			return c;
		}

		public T AddControl<T>(T c)
			where T : ViewElement
		{
			if (Context.EventReceivers.ContainsKey(c.ID)) return c;

			c.Context = Context;
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.ClientID = GetClientID(c.ID);
			c.ParentElement = this;

			Context.EventReceivers.Add(c.ClientID, c);
			c.OnInit();
			return c;
		}
	}

	public abstract class ViewComponent : ViewElement
	{
		
	}

	public abstract class ViewContainer : ViewElement
	{
		public abstract ActionResult Execute();	
	}

	public delegate void ViewElementEventHandler(ApiResponse response);
}
