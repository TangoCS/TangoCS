//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
	class DataContractSerializerFaultFormatter : FaultFormatter
    {
        internal DataContractSerializerFaultFormatter(Type[] detailTypes)
            : base(detailTypes)
        {
        }

        internal DataContractSerializerFaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfoCollection)
            : base(faultContractInfoCollection)
        {
        }
    }
}
