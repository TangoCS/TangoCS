using System.Collections.Generic;
using System.Reflection;

namespace Tango.UI
{
	public interface IBeforeActionFilter
	{
		void BeforeAction(ActionFilterContext context);
	}

	public interface IAfterActionFilter
	{
		void AfterAction(ActionFilterContext context);
	}

	public interface IResourceFilter
	{
		void OnResourceExecuting(ResourceExecutingContext context);
	}

	public interface IActionInvokingFilter
	{
		void OnActionInvoking(ActionInvokingFilterContext context);
	}

	public class ResourceExecutingContext
	{
		public ActionResult CancelResult { get; set; }
		public ActionContext ActionContext { get; }

		public ResourceExecutingContext(ActionContext actionContext)
		{
			ActionContext = actionContext;
		}
	}

	public class ActionInvokingFilterContext
	{
		public IInteractionFlowElement Element { get; }
		public ActionResult CancelResult { get; set; }
		public ActionContext ActionContext => Element.Context;

		public ActionInvokingFilterContext(IInteractionFlowElement element)
		{
			Element = element;
		}
	}

	public class ActionFilterContext
	{
		public IInteractionFlowElement Element { get; }
		public MethodInfo Method { get; }
		public ActionResult Result { get; }
		public ActionResult CancelResult { get; set; }

		public ActionContext ActionContext => Element.Context;

		public ActionFilterContext(IInteractionFlowElement element, MethodInfo method, ActionResult result)
		{
			Element = element;
			Result = result;
			Method = method;
		}
	}

	public class FilterCollection
	{
		List<IResourceFilter> resourceFilters = new List<IResourceFilter>();
		List<IActionInvokingFilter> actionInvokingFilters = new List<IActionInvokingFilter>();
		List<IBeforeActionFilter> beforeActionFilters = new List<IBeforeActionFilter>();
		List<IAfterActionFilter> afterActionFilters = new List<IAfterActionFilter>();

		public IReadOnlyList<IResourceFilter> ResourceFilters => resourceFilters;
		public IReadOnlyList<IBeforeActionFilter> BeforeActionFilters => beforeActionFilters;
		public IReadOnlyList<IAfterActionFilter> AfterActionFilters => afterActionFilters;
		public IReadOnlyList<IActionInvokingFilter> ActionInvokingFilters => actionInvokingFilters;

		public FilterCollection AddResourceFilters(params IResourceFilter[] filters)
		{
			resourceFilters.AddRange(filters);
			return this;
		}
		public FilterCollection AddActionInvokingFilters(params IActionInvokingFilter[] filters)
		{
			actionInvokingFilters.AddRange(filters);
			return this;
		}
		public FilterCollection AddBeforeActionFilters(params IBeforeActionFilter[] filters)
		{
			beforeActionFilters.AddRange(filters);
			return this;
		}
		public FilterCollection AddAfterActionFilters(params IAfterActionFilter[] filters)
		{
			afterActionFilters.AddRange(filters);
			return this;
		}
	}

	public static class ActionFilterExtensions
	{
		public static ActionResult RunResourceFilter(this ActionContext ctx)
		{
			var filtersCollection = ctx.GetService<FilterCollection>();
			var filterContext = new ResourceExecutingContext(ctx);
			foreach (var f in filtersCollection.ResourceFilters)
			{
				f.OnResourceExecuting(filterContext);
				if (filterContext.CancelResult != null)
				{
					return filterContext.CancelResult;
				}
			}
			return null;
		}

		public static ActionResult RunActionInvokingFilter(this IInteractionFlowElement element)
		{
			var filtersCollection = element.Context.GetService<FilterCollection>();
			var filterContext = new ActionInvokingFilterContext(element);
			foreach (var f in filtersCollection.ActionInvokingFilters)
			{
				f.OnActionInvoking(filterContext);
				if (filterContext.CancelResult != null)
				{
					return filterContext.CancelResult;
				}
			}
			return null;
		}
	}
}
