using System;
using System.Collections.Generic;
using Nephrite.Localization;
using Newtonsoft.Json;
using Nephrite.Html;

namespace Nephrite.UI
{
	public abstract class InteractionFlowElement
	{
		public string ID { get; protected set; }
		public ActionContext Context { get; protected set; }
	}

	public abstract class ViewElement : InteractionFlowElement, IWithPropertyInjection
	{
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
		public virtual void CreateChildControls() { }

		public string GetElementID(string id)
		{
			return (!ID.IsEmpty() ? ID + (!id.IsEmpty() ? "_" + id : "") : id).ToLower();
		}

		public T CreateControl<T>(string id, Action<T> init = null)
			where T : ViewComponent, new()
		{
			T c = new T();
			if (c.UsePropertyInjection) c.InjectProperties(Context.RequestServices);
			c.Init(GetElementID(id), Context, TextResource);

			Context.EventReceivers.Add(c.ID, c);

			init?.Invoke(c);
			c.CreateChildControls();
			return c;
		}


		protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		public T GetPosted<T>(string name, T defaultValue = default(T))
		{
			return Context.FormData.Parse<T>(name, defaultValue);
		}

		public T GetPostedJson<T>(string name, Func<T> defaultValue = null)
		{
			var s = GetPosted<string>(name);
			T res = default(T);
			if (!s.IsEmpty()) res = JsonConvert.DeserializeObject<T>(s);
			if (res == null && defaultValue != null) res = defaultValue();
			return res;
		}

		//protected string GetSenderArgs()
		//{
		//	return Context.FormData.Parse<string>(GetElementID("_senderArgs"));
		//}
	}

	public abstract class ViewComponent : ViewElement
	{
		public DataCollection DataCollection { get; set; } = new DataCollection();
	}
	public abstract class ViewComponent<T> : ViewComponent
		where T : ViewComponent
	{
		public Action<T> InitFunc { get; set; }

		public ViewComponent() { }
		public ViewComponent(Action<T> initFunc)
		{
			InitFunc = initFunc;
		}
	}


	public abstract class ViewContainer : ViewComponent
	{
		public abstract ActionResult Execute();	
	}

	public delegate void ViewElementEventHandler(ApiResponse response);
}
