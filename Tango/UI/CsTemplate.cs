using System;
using Tango.Localization;
using Newtonsoft.Json;
using Tango.Html;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Tango.UI
{
	public interface IInteractionFlowElement
	{
		string ID { get; set; }
		ActionContext Context { get; set; }
	}

	public interface IViewElement : IInteractionFlowElement
	{
		Guid UniqueID { get; }
		string ClientID { get; }
		IViewElement ParentElement { get; set; }
		List<IViewElement> ChildElements { get; }
		DataCollection DataCollection { get; set; }
		bool IsLazyLoad { get; set; }
		bool IsModal { get; set; }
		bool IsSubView { get; set; }
		string GetClientID(string id);

		void OnInit();
		//void AfterInit();
		void OnEvent();

		List<string> ElementArgNames { get; set; }
	}

	public interface IContainerItem
	{
		ViewContainer GetContainer();
	}

	public interface IWithCheckAccess
	{
		bool CheckAccess(MethodInfo method);
		ActionResult OnNoAccess();
	}

	public abstract class InteractionFlowElement : IInteractionFlowElement, IWithPropertyInjection
	{
		public virtual string ID { get; set; }
		public ActionContext Context { get; set; }

		//public virtual bool UsePropertyInjection => true;

		public IResourceManager Resources => Context.Resources;
		//protected dynamic FormBag { get { return Context.FormData; } }
		protected DynamicDictionary FormData { get { return Context.FormData; } }

		[Obsolete]
		public T GetPosted<T>(string name, T defaultValue = default)
		{
			return Context.FormData.Parse<T>(name, defaultValue);
		}

		[Obsolete]
		public List<T> GetPostedList<T>(string name)
		{
			return Context.FormData.ParseList<T>(name);
		}

		[Obsolete]
		public DateTime? GetPostedDateTime(string name, string format, DateTime? defaultValue = null)
		{
			return Context.FormData.ParseDateTime(name, format);
		}

		[Obsolete]
		public DateTime GetPostedDateTime(string name, string format, DateTime defaultValue)
		{
			return Context.FormData.ParseDateTime(name, format, defaultValue);
		}

		[Obsolete]
		public T GetPostedJson<T>(string name, Func<T> defaultValue = null)
		{
			var s = Context.GetArg(name);
			s = WebUtility.HtmlDecode(s);
			T res = default;
			if (!s.IsEmpty()) res = JsonConvert.DeserializeObject<T>(s);
			if (res == null && defaultValue != null) res = defaultValue();
			return res;
		}

		public string GetArg(string name)
		{
			return Context.GetArg(name);
		}

		[Obsolete]
		public T GetArg<T>(string name)
		{
			return Context.GetArg<T>(name);
		}
	}

	public abstract class ViewElement : InteractionFlowElement, IViewElement
	{
		public DataCollection DataCollection { get; set; } = new DataCollection();

		public string ClientID => ID != null ? ParentElement?.GetClientID(ID) ?? ID.ToLower() : null;
		IViewElement _parentElement;
		public IViewElement ParentElement {
			get => _parentElement;
			set
			{
				_parentElement?.ChildElements.Remove(this);
				_parentElement = value;
				_parentElement?.ChildElements.Add(this);
			}
		}
		public List<IViewElement> ChildElements { get; set; } = new List<IViewElement>();

		public string GetClientID(string id) => (!ClientID.IsEmpty() ? ClientID + (!id.IsEmpty() ? "_" + id : "") : (id ?? "")).ToLower();

		public virtual void OnInit() { }
		//public virtual void AfterInit() { }
		public virtual void OnEvent() { }
		public bool IsLazyLoad { get; set; }
		public bool IsModal { get; set; }
		public bool IsSubView { get; set; }
		public List<string> ElementArgNames { get; set; }

		public T CreateControl<T>(string id, Action<T> setProperties = null)
			where T : IViewElement, new()
		{
			T c = new T() { Context = Context };
			if (c is IWithPropertyInjection pic)
				pic.InjectProperties(Context.RequestServices);
			c.ID = id;
			c.ParentElement = this;

			if (!Context.EventReceivers.Contains(c))
				Context.EventReceivers.Add(c);

			setProperties?.Invoke(c);
			c.OnInit();
			//c.AfterInit();
			return c;
		}

        public T CreateControl<T>(Action<T> setProperties = null)
            where T : IViewElement, new()
			=> CreateControl<T>(typeof(T).Name, setProperties);

        public T AddControl<T>(T c)
			where T : IViewElement
		{
			if (Context.EventReceivers.Contains(c)) return c;

			c.Context = Context;
			if (c is IWithPropertyInjection pic)
				pic.InjectProperties(Context.RequestServices);
			c.ParentElement = this;

			Context.EventReceivers.Add(c);
			c.OnInit();
			//c.AfterInit();
			return c;
		}

		Guid? uniqueID;
		public virtual Guid UniqueID
		{
			get
			{
				if (!uniqueID.HasValue)
				{
					var formidAttr = GetType().GetCustomAttribute(typeof(UniqueIDAttribute)) as UniqueIDAttribute;
					if (formidAttr != null)
						uniqueID = formidAttr.Guid;
					else
					{
						var key = Context.Service + "/" + Context.Action + "/" + ClientID;
						using (MD5 hasher = MD5.Create())
							uniqueID = new Guid(hasher.ComputeHash(Encoding.UTF8.GetBytes(key)));
					}
				}
				return uniqueID.Value;
			}
		}
	}

	public abstract class ViewComponent : ViewElement
	{
		
	}

	public abstract class ViewContainer : ViewElement
	{
		public string Type => GetType().Name.Replace("Container", "");

		public IDictionary<string, string> Mapping { get; } = new Dictionary<string, string>();
		public HashSet<string> ToRemove { get; } = new HashSet<string>();

		public abstract void Render(ApiResponse response);

		public void ProcessResponse(ApiResponse response, bool addContainer, string prefix)
		{
			OnInit();

			if (addContainer)
			{
				response.WithNameFunc(name => Mapping.ContainsKey(name) ?
					HtmlWriterHelpers.GetID(prefix, Mapping[name]) :
					HtmlWriterHelpers.GetID(prefix, name));
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

		public static ActionResult Invoke<T>(ActionContext context)
			where T: ViewRootElement, new()
		{
			return Invoke(new T(), context);
		}

		public static ActionResult Invoke(ActionContext context, Type t)
		{
			return Invoke(Activator.CreateInstance(t) as ViewRootElement, context);
		}

		static ActionResult Invoke(ViewRootElement form, ActionContext context)
		{
			form.Context = context;
			form.InjectProperties(context.RequestServices);
			return form.RunActionInvokingFilter() ?? form.Execute();
		}
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
