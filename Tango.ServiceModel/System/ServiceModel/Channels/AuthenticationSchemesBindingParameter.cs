//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System.Net;
    using System.Runtime;

    class AuthenticationSchemesBindingParameter
    {
        AuthenticationSchemes authenticationSchemes = AuthenticationSchemes.None;

        public AuthenticationSchemesBindingParameter(AuthenticationSchemes authenticationSchemes)
        {
            this.authenticationSchemes = authenticationSchemes;
        }

        public AuthenticationSchemes AuthenticationSchemes
        {
            get { return this.authenticationSchemes; }
        }

        public static bool TryExtract(BindingParameterCollection collection, out AuthenticationSchemes authenticationSchemes)
        {
            authenticationSchemes = AuthenticationSchemes.None;
            AuthenticationSchemesBindingParameter instance = collection.Find<AuthenticationSchemesBindingParameter>();
            if (instance != null)
            {
                authenticationSchemes = instance.AuthenticationSchemes;
                return true;
            }
            return false;
        }
    }
}
