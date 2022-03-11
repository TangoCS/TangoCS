using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tango.Meta;

namespace Tango.IFML
{
	/// <summary>
	/// InteractionFlowModel is the user view of the whole application while ViewPoints present only specific aspects of the
	/// system by means of references to sets of InteractionFlowModelElements, which as a whole define a fully functional
	/// portion of the system.
	/// </summary>
	public interface IInteractionFlowModel : IMetaNamedElement
	{
		ICollection<IInteractionFlowModelElement> InteractionFlowModelElements { get; }
	}

	/// <summary>
	/// InteractionFlowModelElement is an abstract class, which is the generalization of every element of an 
	/// InteractionFlowModel.
	/// </summary>
	public interface IInteractionFlowModelElement : IMetaNamedElement
	{

	}

	#region InteractionFlowModelElements
	/// <summary>
	/// An InteractionFlow is a directed connection between two InteractionFlowElements. InteractionFlows may imply 
	/// navigation along the user interface or only a transfer of information by carrying parameter values from one 
	/// InteractionFlowElement to another.
	/// InteractionFlows are specialized into NavigationFlows and DataFlows. A NavigationFlow represents navigation or change
	/// of ViewElement focus, the triggering of an Action processing, or a SystemEvent.
	/// </summary>
	public interface IInteractionFlow : IInteractionFlowModelElement
	{
		IInteractionFlowElement Source { get; }
		IInteractionFlowElement Target { get; }
	}

	/// <summary>
	/// NavigationFlows are followed when Events are triggered.NavigationFlows connect Events of ViewContainers, 
	/// ViewComponents, ViewComponentParts, or Actions with other InteractionFlowElements.When a NavigationFlow is 
	/// followed Parameters may be passed from the Interaction Flow Modeling Language, source InteractionFlowElement 
	/// to the target InteractionFlowElement through ParameterBindings.
	/// </summary>
	public interface INavigationFlow : IInteractionFlow
	{

	}

	/// <summary>
	/// A DataFlow is a kind of InteractionFlow used for passing context information between InteractionFlowElements.
	/// DataFlows are triggered by NavigationFlows, causing Parameter passing but no navigation.
	/// </summary>
	public interface IDataFlow : IInteractionFlow
	{

	}

	/// <summary>
	/// InteractionFlowElements are the building blocks of interactions. They represent the pieces of the system, which 
	/// participate in interaction flows through InteractionFlow connections.
	/// InteractionFlowElements represent pieces of the system,
	/// such as ViewElements, ViewComponentParts, Ports, Actions and Events, which participate in InteractionFlow
	/// connections. 
	/// InteractionFlowElements contain Parameters, which usually flow between InteractionFlowElements as a
	/// consequence of ViewElementEvents (user events), ActionEvents, or SystemEvents.InteractionFlowElements may have
	/// both incoming and outgoing interaction flows.
	/// </summary>
	public interface IInteractionFlowElement : IInteractionFlowModelElement
	{
		ICollection<IInteractionFlow> OutInteractionFlows { get; }
        ICollection<IParameter> Parameters { get; }
	}

	/// <summary>
	/// A Parameter is a typed name, whose instances hold values. Parameters are held by InteractionFlowElements i.e.,
	/// ViewElements, ViewComponentParts, Ports, and Actions.Parameters flow between InteractionFlowElements when
	/// Events are triggered. Considering the flow of a Parameter P from an InteractionFlowElement A to an
	/// InteractionFlowElement B, the Parameter P is considered as an output parameter of InteractionFlowElement A and as an
	/// input Parameter of InteractionFlowElement B.
	/// </summary>
	public interface IParameter : IInteractionFlowModelElement
	{

	}

	/// <summary>
	/// ParameterBindings determine to which input Parameter of a target InteractionFlowElement an output Parameter of a
	/// source InteractionFlowElement is bound.ParameterBindings are in turn grouped into ParameterBindingGroups.
	/// </summary>
	public interface IParameterBinding : IInteractionFlowModelElement
	{
		IParameter SourceParameter { get; }
		IParameter TargetParameter { get; }
	}

	public interface IParameterBindingGroup : IInteractionFlowModelElement
	{
		ICollection<IParameterBinding> Bindings { get; }
	}

	/// <summary>
	/// An Expression defines a statement that will evaluate in a given context to a single instance, a set of instances, or an empty
	/// result.
	/// </summary>
	public interface IExpression : IInteractionFlowModelElement
	{

	}
	#endregion

	#region InteractionFlowElements
	/// <summary>
	/// The elements of an IFML model that are visible at the user interface level, which are specialized
	/// in ViewContainers and ViewComponents.
	/// </summary>
	public interface IViewElement : IInteractionFlowElement
	{
		IViewContainer Container { get; }
		IEnumerable<IViewElementEvent> Events { get; }
		IActivationExpression ActivationExpression { get; }
	}

	/// <summary>
	/// A ViewComponentPart is a part of the ViewComponent that cannot live outside the context of a ViewComponent but may 
	/// have Events and incoming and outgoing InteractionFlows. ViewComponentParts may hierarchically contain other 
	/// ViewComponentParts. A ViewComponentPart may be visible or not at the level of the user interface depending 
	/// on the kind of ViewComponentPart.
	/// </summary>
	public interface IViewComponentPart : IInteractionFlowElement
	{
		IViewComponentPart Parent { get; }
		ICollection<IViewComponentPart> Children { get; }

		ICollection<IViewElementEvent> Events { get; }
		IActivationExpression ActivationExpression { get; }
	}

	public interface IAction : IInteractionFlowElement
	{
		ICollection<IActionEvent> Events { get; }
	}

	/// <summary>
	/// Events are occurrences that can affect the state of the application, and they are a subtype of InteractionFlowElement.
	/// </summary>
	public interface IEvent : IInteractionFlowElement
	{
		IInteractionFlowExpression InteractionFlowExpression { get; }
		IActivationExpression ActivationExpression { get; }
	}

	/// <summary>
	/// ViewContainers, like HTML pages or windows, are containers of other ViewContainers or ViewComponents
	/// </summary>
	public interface IViewContainer : IViewElement
	{
		/// <summary>
		/// A landmark ViewContainer may be reached from any other ViewElement without the need of explicit InteractionFlows.
		/// ViewContainers that are not landmark may be reached only with an InteractionFlow.
		/// </summary>
		bool IsLandmark { get; }
		bool IsDefault { get; }
		bool IsXOR { get; }

		ICollection<IViewElement> ViewElements { get; }
	}

	/// <summary>
	/// ViewComponents are elements of the interface that display content or accept input from the user.
	/// ViewComponents exist only inside ViewContainers. A ViewComponent is an element of the interface that may have
	/// dynamic behavior, display content, or accept input. It may correspond e.g., to a form, a data grid, or an image gallery.
	/// </summary>
	public interface IViewComponent : IViewElement
	{
		ICollection<IViewComponentPart> ViewComponentParts { get; }
	}
	#endregion

	/// <summary>
	/// Events that are generated by the UI
	/// </summary>
	public interface IThrowingEvent : IEvent { }

	/// <summary>
	/// Events that are captured in the UI and that trigger a subsequent interface change
	/// </summary>
	public interface ICatchingEvent : IEvent { }

	/// <summary>
	/// SystemEvents are stand-alone events, which are at the level of the InteractionFlowModel. SystemEvents result from an
	/// Action execution termination event or a triggeringExpression such as a specific moment in time, or special condition
	/// events such as a problem in the network connection.
	/// </summary>
	public interface ISystemEvent : ICatchingEvent
	{
		ICollection<IExpression> TriggeringExpressions { get; }
	}

	/// <summary>
	/// ActionEvents are owned by their related Actions.An Action may trigger ActionEvents during its execution or when 
	/// it terminates, normally or with an exception.
	/// </summary>
	public interface IActionEvent : ICatchingEvent { }

	/// <summary>
	/// ViewElementEvents are owned by their related ViewElements. This means that ViewElements contain Events that allow
	/// a user to activate an interaction in the application, e.g., with the click on a hyperlink or on a button.
	/// </summary>
	public interface IViewElementEvent : ICatchingEvent { }

	/// <summary>
	/// A ConditionalExpression is a ViewComponentPart representing predefined queries contained by DataBindings
	/// that may be executed on them to obtain specific content information from the DomainModel. ConditionalExpressions can
	/// be defined only inside a DataBinding ViewComponentPart.
	/// </summary>
	public interface IConditionalExpression : IExpression, IViewComponentPart { }

	/// <summary>
	/// An InteractionFlowExpression is used to determine which of the InteractionFlows will be followed as a consequence of
	/// the occurrence of an Event. When an Event occurs and it has no InteractionFlowExpression, all the InteractionFlows
	/// associated with the event are followed.
	/// </summary>
	public interface IInteractionFlowExpression : IExpression { }

	/// <summary>
	/// A BooleanExpression is an expression that evaluates to true or false. 
	/// </summary>
	public interface IBooleanExpression : IExpression { }

	/// <summary>
	/// An ActivationExpression determines if a ViewElement, ViewComponentPart or
	/// Event is enabled, and thus available to the user for interaction.
	/// </summary>
	public interface IActivationExpression : IBooleanExpression { }
	
	/// <summary>
	/// Constraint restricts the behavior of any element.
	/// </summary>
	public interface IConstraint : IBooleanExpression { }
}
