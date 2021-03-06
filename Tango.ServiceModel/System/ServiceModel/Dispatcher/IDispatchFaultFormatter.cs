//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
	internal interface IDispatchFaultFormatter
    {
        MessageFault Serialize(FaultException faultException, out string action);
    }

    internal interface IDispatchFaultFormatterWrapper
    {
        IDispatchFaultFormatter InnerFaultFormatter
        {
            get;
            set;
        }
    }
}

