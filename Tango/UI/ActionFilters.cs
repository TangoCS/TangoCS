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

	public class ResourceExecutingContext
	{
		public ActionResult CancelResult { get; set; }
		public ActionContext ActionContext { get; }

		public ResourceExecutingContext(ActionContext actionContext)
		{
			ActionContext = actionContext;
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
		List<IBeforeActionFilter> beforeActionFilters = new List<IBeforeActionFilter>();
		List<IAfterActionFilter> afterActionFilters = new List<IAfterActionFilter>();

		public IReadOnlyList<IResourceFilter> ResourceFilters => resourceFilters;
		public IReadOnlyList<IBeforeActionFilter> BeforeActionFilters => beforeActionFilters;
		public IReadOnlyList<IAfterActionFilter> AfterActionFilters => afterActionFilters;

		public FilterCollection AddResourceFilters(params IResourceFilter[] filters)
		{
			resourceFilters.AddRange(filters);
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
}
