using System;
using System.Collections.Generic;
using Nephrite.Html.Layout;
using Nephrite.Multilanguage;
using Newtonsoft.Json;

namespace Nephrite.Templating
{
	public abstract class InteractionFlowElement
	{
		public ActionContext Context { get; protected set; }
	}

	public abstract class ViewElement : InteractionFlowElement, IWithPropertyInjection
	{
		public string ID { get; protected set; }
		public ITextResource TextResource { get; protected set; }

		public void Init(string id, ActionContext context, ITextResource textResource)
		{
			ID = id;
			Context = context;
			TextResource = textResource;
		}

		public virtual LayoutWriter CreateLayoutWriter()
		{
			var w = new LayoutWriter(Context, TextResource);
			w.IDPrefix = ID;
			return w;
		}

		public virtual bool UsePropertyInjection { get { return false; } }

		protected dynamic PostBag { get { return Context.PostData; } }
		protected DynamicDictionary PostData { get { return Context.PostData; } }

		public virtual void CreateChildControls() { }

		protected T GetPosted<T>(string name, T defaultValue = default(T))
		{
			return Context.PostData.Parse<T>(GetElementID(name), defaultValue);
		}

		protected T GetPostedJson<T>(string name, Func<T> defaultValue = null)
		{
			var s = GetPosted<string>(name);
			T res = default(T);
			if (!s.IsEmpty()) res = JsonConvert.DeserializeObject<T>(s);
			if (res == null && defaultValue != null) res = defaultValue();
			return res;
		}

		protected string GetSenderArgs()
		{
			return Context.PostData.Parse<string>(GetElementID("_senderArgs"));
		}

		public string GetElementID(string id)
		{
			return !ID.IsEmpty() ? ID + "_" + id : id;
		}


	}

	public abstract class ViewComponent : ViewElement
	{
		public ViewContainer Container { get; set; }

		public T CreateControl<T>(string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			return Container.CreateControl<T>(GetElementID(id), init);
		}
	}

	public abstract class ViewContainer : ViewElement
	{
		public string Title { get; set; }

		protected Dictionary<string, ViewComponent> _controls = new Dictionary<string, ViewComponent>(StringComparer.OrdinalIgnoreCase);

		public T CreateControl<T>(string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			T c = new T();
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.Init(id, Context, TextResource);
			c.Container = this;
			if (init != null) init(c);
			_controls.Add(c.ID, c);
			c.CreateChildControls();
			return c;
		}

		public abstract ActionResult Execute();
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class OnActionAttribute : Attribute
	{
		public string Service { get; }
		public string Action { get; }

		public OnActionAttribute(string service, string action)
		{
			Service = service;
			Action = action;
		}
	}
}
