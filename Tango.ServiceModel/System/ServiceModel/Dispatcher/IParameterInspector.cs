//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Dispatcher
{
	public interface IParameterInspector
    {
        object BeforeCall(string operationName, object[] inputs);
        void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState);
    }
}
