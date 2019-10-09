//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel
{
    using System;

    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = false, AllowMultiple = false)]
    public sealed class DataContractFormatAttribute : Attribute
    {
        OperationFormatStyle style;
        public OperationFormatStyle Style
        {
            get { return style; }
            set
            {
                XmlSerializerFormatAttribute.ValidateOperationFormatStyle(style);
                style = value;
            }
        }

    }
}
