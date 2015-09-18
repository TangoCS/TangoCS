using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Meta;

namespace Nephrite.IFML
{
	public abstract class InteractionFlow : MetaNamedElement, IInteractionFlow
	{
		public IInteractionFlowElement Source { get; private set; }
		public IInteractionFlowElement Target { get; private set; }

		public InteractionFlow(IInteractionFlowElement source, IInteractionFlowElement target)
		{
			Source = source;
			Target = target;
        }
	}

	public abstract class InteractionFlowElement : MetaNamedElement, IInteractionFlowElement
	{
		public ICollection<IInteractionFlow> OutInteractionFlows { get; set; }
		public ICollection<IParameter> Parameters { get; set; }
	}

	public class NavigationFlow : InteractionFlow, INavigationFlow
	{
		public NavigationFlow(IInteractionFlowElement source, IInteractionFlowElement target) : base(source, target) { }
	}

	public class DataFlow : InteractionFlow, IDataFlow
	{
		public DataFlow(IInteractionFlowElement source, IInteractionFlowElement target) : base(source, target) { }
	}

	public class Parameter : MetaNamedElement, IParameter
	{
	}

	public class ViewElement : InteractionFlowElement, IViewElement
	{
		public IActivationExpression ActivationExpression { get; set; }
		public IViewContainer Container { get; set; }
		public IEnumerable<IViewElementEvent> Events { get; set; }
	}

	public class ViewComponent : ViewElement, IViewComponent
	{
		public ICollection<IViewComponentPart> ViewComponentParts { get; set; }
	}

	public class ViewContainer : ViewElement, IViewContainer
	{
		public bool IsDefault { get; set; }
		public bool IsLandmark { get; set; }
		public bool IsXOR { get; set; }

		public ICollection<IViewElement> ViewElements { get; set; }
	}
}
