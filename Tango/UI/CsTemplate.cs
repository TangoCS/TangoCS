using System;
using Tango.Localization;
using Newtonsoft.Json;
using Tango.Html;

namespace Tango.UI
{
	public abstract class InteractionFlowElement
	{
		public string ID { get; protected set; }
		public ActionContext Context { get; protected set; }
	}

	public abstract class ViewElement : InteractionFlowElement, IWithPropertyInjection
	{
		public void Init(string id, ActionContext context)
		{
			ID = id;
			Context = context;
		}

		public virtual LayoutWriter CreateLayoutWriter()
		{
			var w = new LayoutWriter(Context);
			w.IDPrefix = ID;
			return w;
		}

		public virtual bool UsePropertyInjection { get { return false; } }
		public virtual void OnInit() { }

		public string GetElementID(string id)
		{
			return (!ID.IsEmpty() ? ID + (!id.IsEmpty() ? "_" + id : "") : id).ToLower();
		}

		public T CreateControl<T>(string id, Action<T> setProperties = null)
			where T : ViewComponent, new()
		{
			T c = new T();
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.Init(GetElementID(id), Context);

			Context.EventReceivers.Add(c.ID, c);

			setProperties?.Invoke(c);
			c.OnInit();
			return c;
		}

		public T CreateControl<T>(Func<T> constr)
			where T : ViewComponent, new()
		{
			T c = constr();
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.Init(GetElementID(c.ID), Context);

			Context.EventReceivers.Add(c.ID, c);
			c.OnInit();
			return c;
		}

		protected ITextResource TextResource => Context.TextResource;
		protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		public T GetPosted<T>(string name, T defaultValue = default(T))
		{
			return Context.FormData.Parse<T>(name, defaultValue);
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

	public abstract class ViewComponent : ViewElement
	{
		public DataCollection DataCollection { get; set; } = new DataCollection();
	}

	public abstract class ViewContainer : ViewComponent
	{
		public abstract ActionResult Execute();	
	}

	public delegate void ViewElementEventHandler(ApiResponse response);
}
