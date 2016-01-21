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
		public ViewContainer Container { get; set; }

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

		protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		public virtual void CreateChildControls() { }

		protected T GetPosted<T>(string name, T defaultValue = default(T))
		{
			return Context.FormData.Parse<T>(GetElementID(name), defaultValue);
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
			return Context.FormData.Parse<string>(GetElementID("_senderArgs"));
		}

		public string GetElementID(string id)
		{
			return !ID.IsEmpty() ? ID + (!id.IsEmpty() ? "_" + id : "") : id;
		}


	}

	public abstract class ViewComponent : ViewElement
	{
		public ViewElement Component { get; set; }
		
		public T CreateControl<T>(string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			return Container.CreateControl(this, id, init);
		}
	}

	public abstract class ViewContainer : ViewElement
	{
		public string Title { get; set; }

		protected Dictionary<string, ViewElement> _controls = new Dictionary<string, ViewElement>(StringComparer.OrdinalIgnoreCase);
		public IDictionary<string, ViewElement> Controls { get { return _controls; } }

		internal T CreateControl<T>(ViewElement component, string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			T c = new T();
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.Init(GetElementID(id), Context, TextResource);
			c.Container = this;
			this.Controls.Add(c.ID, c);

			c.Component = component;
			if (init != null) init(c);
			c.CreateChildControls();
			return c;
		}

		public T CreateControl<T>(string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			return CreateControl(this, id, init);
		}

		public abstract ActionResult Execute();
	}
}
