//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    public interface IExtensibleObject<T>
    where T : IExtensibleObject<T>
    {
        IExtensionCollection<T> Extensions { get; }
    }
}
