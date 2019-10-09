//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime;
    //using System.Runtime.Remoting.Messaging;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    class ProxyOperationRuntime
    {
        static internal readonly ParameterInfo[] NoParams = new ParameterInfo[0];
        static internal readonly object[] EmptyArray = new object[0];

        readonly IClientMessageFormatter formatter;
        readonly bool isInitiating;
        readonly bool isOneWay;
        readonly bool isTerminating;
        readonly bool isSessionOpenNotificationEnabled;
        readonly string name;
        readonly IParameterInspector[] parameterInspectors;
        readonly IClientFaultFormatter faultFormatter;
        readonly ImmutableClientRuntime parent;
        bool serializeRequest;
        bool deserializeReply;
        string action;
        string replyAction;

        MethodInfo beginMethod;
        MethodInfo syncMethod;
        MethodInfo taskMethod;
        ParameterInfo[] inParams;
        ParameterInfo[] outParams;
        ParameterInfo[] endOutParams;
        ParameterInfo returnParam;

        internal ProxyOperationRuntime(ClientOperation operation, ImmutableClientRuntime parent)
        {
            if (operation == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            if (parent == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");

            this.parent = parent;
            this.formatter = operation.Formatter;
            this.isInitiating = operation.IsInitiating;
            this.isOneWay = operation.IsOneWay;
            this.isTerminating = operation.IsTerminating;
            this.isSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            this.name = operation.Name;
            this.parameterInspectors = EmptyArray<IParameterInspector>.ToArray(operation.ParameterInspectors);
            this.faultFormatter = operation.FaultFormatter;
            this.serializeRequest = operation.SerializeRequest;
            this.deserializeReply = operation.DeserializeReply;
            this.action = operation.Action;
            this.replyAction = operation.ReplyAction;
            this.beginMethod = operation.BeginMethod;
            this.syncMethod = operation.SyncMethod;
            this.taskMethod = operation.TaskMethod;
            this.TaskTResult = operation.TaskTResult;

            if (this.beginMethod != null)
            {
                this.inParams = ServiceReflector.GetInputParameters(this.beginMethod, true);
                if (this.syncMethod != null)
                {
                    this.outParams = ServiceReflector.GetOutputParameters(this.syncMethod, false);
                }
                else
                {
                    this.outParams = NoParams;
                }
                this.endOutParams = ServiceReflector.GetOutputParameters(operation.EndMethod, true);
                this.returnParam = operation.EndMethod.ReturnParameter;
            }
            else if (this.syncMethod != null)
            {
                this.inParams = ServiceReflector.GetInputParameters(this.syncMethod, false);
                this.outParams = ServiceReflector.GetOutputParameters(this.syncMethod, false);
                this.returnParam = this.syncMethod.ReturnParameter;
            }

            if (this.formatter == null && (serializeRequest || deserializeReply))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("ClientRuntimeRequiresFormatter0"), this.name)));
            }
        }

        internal string Action
        {
            get { return this.action; }
        }

        internal IClientFaultFormatter FaultFormatter
        {
            get { return this.faultFormatter; }
        }

        internal bool IsInitiating
        {
            get { return this.isInitiating; }
        }

        internal bool IsOneWay
        {
            get { return this.isOneWay; }
        }

        internal bool IsTerminating
        {
            get { return this.isTerminating; }
        }

        internal bool IsSessionOpenNotificationEnabled
        {
            get { return this.isSessionOpenNotificationEnabled; }
        }

        internal string Name
        {
            get { return this.name; }
        }

        internal ImmutableClientRuntime Parent
        {
            get { return this.parent; }
        }

        internal string ReplyAction
        {
            get { return this.replyAction; }
        }

        internal bool DeserializeReply
        {
            get { return this.deserializeReply; }
        }

        internal bool SerializeRequest
        {
            get { return this.serializeRequest; }
        }

        internal Type TaskTResult 
        { 
            get; 
            set; 
        }

        internal void AfterReply(ref ProxyRpc rpc)
        {
            if (!this.isOneWay)
            {
                Message reply = rpc.Reply;

                if (this.deserializeReply)
                {
                    rpc.ReturnValue = this.formatter.DeserializeReply(reply, rpc.OutputParameters);
                }
                else
                {
                    rpc.ReturnValue = reply;
                }

                int offset = this.parent.ParameterInspectorCorrelationOffset;
                try
                {
                    for (int i = parameterInspectors.Length - 1; i >= 0; i--)
                    {
                        this.parameterInspectors[i].AfterCall(this.name,
                                                              rpc.OutputParameters,
                                                              rpc.ReturnValue,
                                                              rpc.Correlation[offset + i]);
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
                }

                if (parent.ValidateMustUnderstand)
                {
                    Collection<MessageHeaderInfo> headersNotUnderstood = reply.Headers.GetHeadersNotUnderstood();
                    if (headersNotUnderstood != null && headersNotUnderstood.Count > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(Res.GetString(Res.S("SFxHeaderNotUnderstood"), headersNotUnderstood[0].Name, headersNotUnderstood[0].Namespace)));
                    }
                }
            }
        }

        internal void BeforeRequest(ref ProxyRpc rpc)
        {
            int offset = this.parent.ParameterInspectorCorrelationOffset;
            try
            {
                for (int i = 0; i < parameterInspectors.Length; i++)
                {
                    rpc.Correlation[offset + i] = this.parameterInspectors[i].BeforeCall(this.name, rpc.InputParameters);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }

            if (this.serializeRequest)
            {
                rpc.Request = this.formatter.SerializeRequest(rpc.MessageVersion, rpc.InputParameters);
            }
            else
            {
                if (rpc.InputParameters[0] == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("SFxProxyRuntimeMessageCannotBeNull"), this.name)));
                }

                rpc.Request = (Message)rpc.InputParameters[0];
                if (!IsValidAction(rpc.Request, Action))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(Res.GetString(Res.S("SFxInvalidRequestAction"), this.Name, rpc.Request.Headers.Action ?? "{NULL}", this.Action)));
            }
        }

        internal static object GetDefaultParameterValue(Type parameterType)
        {
            return (parameterType.IsValueType && parameterType != typeof(void)) ? Activator.CreateInstance(parameterType) : null;
        }

		internal bool IsSyncCall(MethodCall methodCall)
		{
			if (syncMethod == null)
			{
				return false;
			}

			return methodCall.MethodBase.Equals(syncMethod);
		}

		internal bool IsBeginCall(MethodCall methodCall)
		{
			if (beginMethod == null)
			{
				return false;
			}

			return methodCall.MethodBase.Equals(beginMethod);
		}

		internal bool IsTaskCall(MethodCall methodCall)
		{
			if (taskMethod == null)
			{
				return false;
			}

			return methodCall.MethodBase.Equals(taskMethod);
		}

		internal object[] MapSyncInputs(MethodCall methodCall, out object[] outs)
		{
			if (outParams.Length == 0)
			{
				outs = Array.Empty<object>();
			}
			else
			{
				outs = new object[outParams.Length];
			}
			if (inParams.Length == 0)
				return Array.Empty<object>();
			return methodCall.Args;
		}

		internal object[] MapAsyncBeginInputs(MethodCall methodCall, out AsyncCallback callback, out object asyncState)
		{
			object[] ins;
			if (inParams.Length == 0)
			{
				ins = Array.Empty<object>();
			}
			else
			{
				ins = new object[inParams.Length];
			}

			object[] args = methodCall.Args;
			for (int i = 0; i < ins.Length; i++)
			{
				ins[i] = args[inParams[i].Position];
			}

			callback = args[methodCall.Args.Length - 2] as AsyncCallback;
			asyncState = args[methodCall.Args.Length - 1];
			return ins;
		}

		internal void MapAsyncEndInputs(MethodCall methodCall, out IAsyncResult result, out object[] outs)
		{
			outs = new object[endOutParams.Length];
			result = methodCall.Args[methodCall.Args.Length - 1] as IAsyncResult;
		}

		internal object[] MapSyncOutputs(MethodCall methodCall, object[] outs, ref object ret)
		{
			return MapOutputs(outParams, methodCall, outs, ref ret);
		}

		internal object[] MapAsyncOutputs(MethodCall methodCall, object[] outs, ref object ret)
		{
			return MapOutputs(endOutParams, methodCall, outs, ref ret);
		}

		private object[] MapOutputs(ParameterInfo[] parameters, MethodCall methodCall, object[] outs, ref object ret)
		{
			if (ret == null && returnParam != null)
			{
				ret = GetDefaultParameterValue(TypeLoader.GetParameterType(returnParam));
			}

			if (parameters.Length == 0)
			{
				return null;
			}

			object[] args = methodCall.Args;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (outs[i] == null)
				{
					// the RealProxy infrastructure requires a default value for value types
					args[parameters[i].Position] = GetDefaultParameterValue(TypeLoader.GetParameterType(parameters[i]));
				}
				else
				{
					args[parameters[i].Position] = outs[i];
				}
			}

			return args;
		}

		static internal bool IsValidAction(Message message, string action)
        {
            if (message == null)
            {
                return false;
            }

            if (message.IsFault)
            {
                return true;
            }

            if (action == MessageHeaders.WildcardAction)
            {
                return true;
            }

            return (String.CompareOrdinal(message.Headers.Action, action) == 0);
        }
    }
}
