//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    using System;

    [Serializable]
    public class QuotaExceededException : SystemException
    {
        public QuotaExceededException()
            : base()
        {
        }

        public QuotaExceededException(string message)
            : base(message)
        {
        }

        public QuotaExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

