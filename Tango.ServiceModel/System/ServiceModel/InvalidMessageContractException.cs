//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class InvalidMessageContractException : SystemException
    {
        public InvalidMessageContractException()
            : base()
        {
        }

        public InvalidMessageContractException(String message)
            : base(message)
        {
        }

        public InvalidMessageContractException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

