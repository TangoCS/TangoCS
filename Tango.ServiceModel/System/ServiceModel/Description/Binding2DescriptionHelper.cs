using System;
using System.Linq;
using System.Xml;
using System.Globalization;
using WsdlNS = System.Web.Services.Description;

namespace System.ServiceModel.Description
{
	internal static class Binding2DescriptionHelper
	{
		internal static OperationDescription FindOperationDescription(WsdlNS.OperationBinding wsdlOperationBinding, WsdlNS.ServiceDescriptionCollection wsdlDocuments, WsdlEndpointConversionContext endpointContext)
		{

			OperationDescription operation;
			if (endpointContext.ContractConversionContext != null)
			{
				WsdlNS.Operation wsdlOperation = FindWsdlOperation(wsdlOperationBinding, wsdlDocuments);
				operation = endpointContext.ContractConversionContext.GetOperationDescription(wsdlOperation);
			}
			else
			{
				operation = FindOperationDescription(endpointContext.Endpoint.Contract, wsdlOperationBinding);
			}
			return operation;
		}

		internal static WsdlNS.MessageBinding FindMessageBinding(WsdlNS.OperationBinding wsdlOperationBinding, MessageDescription message)
		{
			WsdlNS.MessageBinding wsdlMessageBinding;
			if (message.Direction == MessageDirection.Input)
			{
				wsdlMessageBinding = wsdlOperationBinding.Input;
			}
			else
			{
				wsdlMessageBinding = wsdlOperationBinding.Output;
			}
			return wsdlMessageBinding;
		}

		internal static WsdlNS.FaultBinding FindFaultBinding(WsdlNS.OperationBinding wsdlOperationBinding, FaultDescription fault)
		{
			foreach (WsdlNS.FaultBinding faultBinding in wsdlOperationBinding.Faults)
				if (faultBinding.Name == fault.Name)
					return faultBinding;
			return null;
		}

		static WsdlNS.Operation FindWsdlOperation(WsdlNS.OperationBinding wsdlOperationBinding, WsdlNS.ServiceDescriptionCollection wsdlDocuments)
		{
			WsdlNS.PortType wsdlPortType = wsdlDocuments.GetPortType(wsdlOperationBinding.Binding.Type);

			string wsdlOperationBindingName = wsdlOperationBinding.Name;

			if (wsdlOperationBindingName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("SFxInvalidWsdlBindingOpNoName"), wsdlOperationBinding.Binding.Name)));
			}

			WsdlNS.Operation partialMatchResult = null;

			foreach (WsdlNS.Operation wsdlOperation in wsdlPortType.Operations)
			{
				switch (Match(wsdlOperationBinding, wsdlOperation))
				{
					case MatchResult.None:
						break;
					case MatchResult.Partial:
						partialMatchResult = wsdlOperation;
						break;
					case MatchResult.Exact:
						return wsdlOperation;
					default:
						break;
				}
			}

			if (partialMatchResult != null)
			{
				return partialMatchResult;
			}
			else
			{
				//unable to find wsdloperation for wsdlOperationBinding, invalid wsdl binding
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("SFxInvalidWsdlBindingOpMismatch2"), wsdlOperationBinding.Binding.Name, wsdlOperationBinding.Name)));
			}
		}

		internal enum MatchResult
		{
			None,
			Partial,
			Exact
		}

		internal static MatchResult Match(WsdlNS.OperationBinding wsdlOperationBinding, WsdlNS.Operation wsdlOperation)
		{
			// This method checks if there is a match based on Names, between the specified OperationBinding and Operation.
			// When searching for the Operation associated with an OperationBinding, we need to return an exact match if possible,
			// or a partial match otherwise (when some of the Names are null).
			// Bug 16833 @ CSDMain requires that partial matches are allowed, while the TFS bug 477838 requires that exact matches are done (when possible).
			if (wsdlOperationBinding.Name != wsdlOperation.Name)
			{
				return MatchResult.None;
			}

			MatchResult result = MatchResult.Exact;

			foreach (WsdlNS.OperationMessage wsdlOperationMessage in wsdlOperation.Messages)
			{
				WsdlNS.MessageBinding wsdlMessageBinding;
				if (wsdlOperationMessage is WsdlNS.OperationInput)
					wsdlMessageBinding = wsdlOperationBinding.Input;
				else
					wsdlMessageBinding = wsdlOperationBinding.Output;

				if (wsdlMessageBinding == null)
				{
					return MatchResult.None;
				}

				switch (MatchOperationParameterName(wsdlMessageBinding, wsdlOperationMessage))
				{
					case MatchResult.None:
						return MatchResult.None;
					case MatchResult.Partial:
						result = MatchResult.Partial;
						break;
				}
			}

			return result;
		}

		static MatchResult MatchOperationParameterName(WsdlNS.MessageBinding wsdlMessageBinding, WsdlNS.OperationMessage wsdlOperationMessage)
		{
			string wsdlOperationMessageName = wsdlOperationMessage.Name;
			string wsdlMessageBindingName = wsdlMessageBinding.Name;

			if (wsdlOperationMessageName == wsdlMessageBindingName)
			{
				return MatchResult.Exact;
			}

			string wsdlOperationMessageDecodedName = WsdlNamingHelper.GetOperationMessageName(wsdlOperationMessage).DecodedName;
			if ((wsdlOperationMessageName == null) && (wsdlMessageBindingName == wsdlOperationMessageDecodedName))
			{
				return MatchResult.Partial;
			}
			else if ((wsdlMessageBindingName == null) && (wsdlOperationMessageName == wsdlOperationMessageDecodedName))
			{
				return MatchResult.Partial;
			}
			else
			{
				return MatchResult.None;
			}
		}

		static OperationDescription FindOperationDescription(ContractDescription contract, WsdlNS.OperationBinding wsdlOperationBinding)
		{
			foreach (OperationDescription operationDescription in contract.Operations)
			{
				if (CompareOperations(operationDescription, contract, wsdlOperationBinding))
					return operationDescription;
			}

			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("UnableToLocateOperation2"), wsdlOperationBinding.Name, contract.Name)));
		}

		static bool CompareOperations(OperationDescription operationDescription, ContractDescription parentContractDescription, WsdlNS.OperationBinding wsdlOperationBinding)
		{
			string wsdlOperationName = WsdlExporter.WsdlNamingHelper.GetWsdlOperationName(operationDescription, parentContractDescription);

			if (wsdlOperationName != wsdlOperationBinding.Name)
				return false;

			if (operationDescription.Messages.Count > 2)
				return false;

			// Either both have output message or neither have an output message;
			if (FindMessage(operationDescription.Messages, MessageDirection.Output) != (wsdlOperationBinding.Output != null))
				return false;

			// Either both have output message or neither have an output message;
			if (FindMessage(operationDescription.Messages, MessageDirection.Input) != (wsdlOperationBinding.Input != null))
				return false;

			return true;

		}

		private static bool FindMessage(MessageDescriptionCollection messageDescriptionCollection, MessageDirection transferDirection)
		{
			foreach (MessageDescription message in messageDescriptionCollection)
				if (message.Direction == transferDirection)
					return true;
			return false;
		}

		static class WsdlNamingHelper
		{
			internal static XmlQualifiedName GetBindingName(WsdlNS.Binding wsdlBinding)
			{
				XmlName xmlName = new XmlName(wsdlBinding.Name, true /*isEncoded*/);
				return new XmlQualifiedName(xmlName.EncodedName, wsdlBinding.ServiceDescription.TargetNamespace);
			}

			internal static XmlQualifiedName GetBindingName(WsdlNS.Port wsdlPort)
			{
				// [....]: composing names have potential problem of generating name that looks like an encoded name, consider avoiding '_'
				XmlName xmlName = new XmlName(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", wsdlPort.Service.Name, wsdlPort.Name), true /*isEncoded*/);
				return new XmlQualifiedName(xmlName.EncodedName, wsdlPort.Service.ServiceDescription.TargetNamespace);
			}

			internal static XmlName GetEndpointName(WsdlNS.Port wsdlPort)
			{
				return new XmlName(wsdlPort.Name, true /*isEncoded*/);
			}

			internal static XmlQualifiedName GetContractName(XmlQualifiedName wsdlPortTypeQName)
			{
				return wsdlPortTypeQName;
			}

			internal static string GetOperationName(WsdlNS.Operation wsdlOperation)
			{
				return wsdlOperation.Name;
			}

			internal static XmlName GetOperationMessageName(WsdlNS.OperationMessage wsdlOperationMessage)
			{
				string messageName = null;
				if (!string.IsNullOrEmpty(wsdlOperationMessage.Name))
				{
					messageName = wsdlOperationMessage.Name;
				}
				else if (wsdlOperationMessage.Operation.Messages.Count == 1)
				{
					messageName = wsdlOperationMessage.Operation.Name;
				}
				else if (wsdlOperationMessage.Operation.Messages.IndexOf(wsdlOperationMessage) == 0)
				{
					if (wsdlOperationMessage is WsdlNS.OperationInput)
						messageName = wsdlOperationMessage.Operation.Name + "Request";
					else if (wsdlOperationMessage is WsdlNS.OperationOutput)
						messageName = wsdlOperationMessage.Operation.Name + "Solicit";
				}
				else if (wsdlOperationMessage.Operation.Messages.IndexOf(wsdlOperationMessage) == 1)
				{
					messageName = wsdlOperationMessage.Operation.Name + "Response";
				}
				else
				{
					// [....]: why this is an Assert, and not an exception?
				}
				// names the come from service description documents have to be valid NCNames; XmlName.ctor will validate.
				return new XmlName(messageName, true /*isEncoded*/);
			}
		}
	}
}
