using System;
using Tango.AccessControl;
using Tango.Localization;

namespace Tango.UI.Std
{
	public static partial class ActionLinkStdExtensions
	{
		public static void CellActionLink(this LayoutWriter w, Action<ActionLink> urlAttributes, object title)
		{
			w.ActionLink(x => { urlAttributes(x); x.WithTitle(title?.ToString()); });
		}

		public static ActionLink UseSecurableUrlResolver(this ActionLink actionUrl, IAccessControl ac, Action<SecurableUrlResolver> resolverSetup)
		{
			var resolver = new SecurableUrlResolver(ac, actionUrl.Context.Routes["default"]);
			resolverSetup(resolver);
			return actionUrl.UseResolver(resolver);
		}


		static (string serviceName, string fullServiceName) ProcessType(Type entityType)
		{
			entityType = entityType.GetResourceType();
			var serviceName = entityType.Name.Replace("Controller", "");
			var fullServiceName = entityType.FullName.Replace("Controller", "");
			return (serviceName, fullServiceName);
		}

		static ActionLink To(this ActionLink link, string serviceName, string fullServiceName, string actionName, string returnurl)
		{
			link.WithTitle(r => {
				var title = r.Get(fullServiceName + "." + actionName);
				if (title.IsEmpty()) title = r.Get("Common." + actionName);
				if (title.IsEmpty()) title = actionName;
				return title;
			});

			link.To(serviceName, actionName);
			if (!returnurl.IsEmpty())
				link.WithArg("returnurl", returnurl == "this" ? link.Context.CreateReturnUrl(1) : returnurl);
			return link;
		}


		public static ActionLink To(this ActionLink link, Type entityType, string actionName, IAccessControl ac, object predicateContext = null, string returnurl = "this")
		{
			var (serviceName, fullServiceName) = ProcessType(entityType);
			return link.To(serviceName, fullServiceName, actionName, returnurl)
				.UseSecurableUrlResolver(ac, x => x.WithKey(serviceName + "." + actionName)
				.WithPredicate(predicateContext)); ;
		}

		public static ActionLink To(this ActionLink link, Type entityType, string actionName, string returnurl = "this")
		{
			var (serviceName, fullServiceName) = ProcessType(entityType);
			return link.To(serviceName, fullServiceName, actionName, returnurl)
				.UseDefaultResolver();
		}

		public static ActionLink To(this ActionLink link, string serviceName, string actionName, IAccessControl ac, object predicateContext = null, string returnurl = "this")
		{
			return link.To(serviceName, serviceName, actionName, returnurl)
				.UseSecurableUrlResolver(ac, x => x.WithKey(serviceName + "." + actionName)
				.WithPredicate(predicateContext));
		}

		public static ActionLink To<T>(this ActionLink link, string actionName, IAccessControl ac, object predicateContext = null, string returnurl = "this")
		{
			return link.To(typeof(T), actionName, ac, predicateContext, returnurl);
		}

		public static ActionLink To<T>(this ActionLink link, string actionName, string returnurl = "this")
		{
			return link.To(typeof(T), actionName, returnurl);
		}

		public static ActionLink ToCreateNew<T>(this ActionLink url, IAccessControl ac, object predicateContext = null, string returnUrl = "this")
		{
			return url.To<T>("CreateNew", ac, predicateContext, returnUrl).WithImage("New");
		}

		public static ActionLink ToCreateNew<T>(this ActionLink url, string returnUrl = "this")
		{
			return url.To<T>("CreateNew", returnUrl).WithImage("New");
		}

		public static ActionLink ToDelete<T>(this ActionLink url, IAccessControl ac, object id, object predicateContext = null, string returnUrl = "this")
		{
			return url.To<T>("Delete", ac, predicateContext, returnUrl).WithArg("oid", id).WithImage("Delete");
		}

		public static ActionLink ToDelete<T>(this ActionLink url, object id, string returnUrl = "this")
		{
			return url.To<T>("Delete", returnUrl).WithArg("oid", id).WithImage("Delete");
		}

		public static ActionLink ToUndelete<T>(this ActionLink url, IAccessControl ac, object id, object predicateContext = null, string returnUrl = "this")
		{
			return url.To<T>("Undelete", ac, predicateContext, returnUrl).WithArg("oid", id).WithImage("Undelete");
		}

		public static ActionLink ToDeleteBulk<T>(this ActionLink url, IAccessControl ac, string returnUrl = "this")
		{
			return url.To<T>("Delete", ac, null, returnUrl).WithImage("Delete");
		}

		public static ActionLink ToEdit<T>(this ActionLink url, IAccessControl ac, object id, object predicateContext = null, string returnUrl = "this")
		{
			return url.To<T>("Edit", ac, predicateContext, returnUrl).WithArg("oid", id).WithImage("Edit");
		}

		public static ActionLink ToEdit<T>(this ActionLink url, object id, string returnUrl = "this")
		{
			return url.To<T>("Edit", returnUrl).WithArg("oid", id).WithImage("Edit");
		}

		public static ActionLink ToView<T>(this ActionLink url, IAccessControl ac, object id, object predicateContext = null, string returnUrl = "this")
		{
			return url.To<T>("View", ac, predicateContext, returnUrl).WithArg("oid", id).WithImage("Documents");
		}

		public static ActionLink ToView<TKey>(this ActionLink url, IAccessControl ac, IWithKey<TKey> obj, string returnUrl = "this")
		{
			return url.To(obj.GetType(), "View", ac, obj, returnUrl).WithArg("oid", obj.ID).WithImage("Documents");
		}

		public static ActionLink ToView<T>(this ActionLink url, object id, string returnUrl = "this")
		{
			return url.To<T>("View", returnUrl).WithArg("oid", id).WithImage("Documents");
		}

		public static ActionLink ToView<TKey>(this ActionLink url, IWithKey<TKey> obj, string returnUrl = "this")
		{
			return url.To(obj.GetType(), "View", returnUrl).WithArg("oid", obj.ID).WithImage("Documents");
		}

		public static ActionLink ToEdit<TKey>(this ActionLink url, IAccessControl ac, IWithKey<TKey> obj, string returnUrl = "this")
		{
			var logdelobj = obj as IWithLogicalDelete;
			return url.To(obj.GetType(), "Edit", ac, obj, returnUrl)
				.WithArg("oid", obj.ID)
				.WithCondition(logdelobj == null || !logdelobj.IsDeleted)
				.WithImage("Edit");
		}

		public static ActionLink ToDelete<TKey>(this ActionLink url, IAccessControl ac, IWithKey<TKey> obj, string returnUrl = "this")
		{
			var logdelobj = obj as IWithLogicalDelete;
			return url.To(obj.GetType(), "Delete", ac, obj, returnUrl)
				.WithArg("oid", obj.ID)
				.WithCondition(logdelobj == null || !logdelobj.IsDeleted)
				.WithImage("Delete");
		}

		public static ActionLink ToUndelete<TKey>(this ActionLink url, IAccessControl ac, IWithKey<TKey> obj, string returnUrl = "this")
		{
			var logdelobj = obj as IWithLogicalDelete;
			return url.To(obj.GetType(), "UnDelete", ac, obj, returnUrl)
				.WithArg("oid", obj.ID)
				.WithCondition(logdelobj != null && logdelobj.IsDeleted)
				.WithImage("UnDelete");
		}

		public static ActionLink ToDeleteUndelete<TKey>(this ActionLink url, IAccessControl ac, IWithKey<TKey> obj, string returnUrl = "this")
		{
			var logdelobj = obj as IWithLogicalDelete;
			if (logdelobj != null && logdelobj.IsDeleted)
				return url.ToUndelete(ac, obj, returnUrl);
			else
				return url.ToDelete(ac, obj, returnUrl);
		}

		public static ActionLink ToList<T>(this ActionLink url, IAccessControl ac, string returnUrl = null)
		{
			return url.To(typeof(T), "ViewList", ac, returnurl: returnUrl);
		}

		public static ActionLink ToList<T>(this ActionLink url, string returnUrl = null)
		{
			return url.To(typeof(T), "ViewList", returnUrl);
		}
	}
}