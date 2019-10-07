//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
	using System;

	public interface IServiceChannel : IContextChannel
    {
        Uri ListenUri { get; }
    }
}
