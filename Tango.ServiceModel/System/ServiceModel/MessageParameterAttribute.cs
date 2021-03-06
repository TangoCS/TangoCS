//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel
{
    using System;

    [AttributeUsage(ServiceModelAttributeTargets.Parameter, Inherited = false)]
    public sealed class MessageParameterAttribute : Attribute
    {
        string name;
        bool isNameSetExplicit;
        internal const string NamePropertyName = "Name";
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value",
                        Res.GetString(Res.S("SFxNameCannotBeEmpty"))));
                }
                name = value; isNameSetExplicit = true;
            }
        }

        internal bool IsNameSetExplicit
        {
            get { return isNameSetExplicit; }
        }
    }
}
