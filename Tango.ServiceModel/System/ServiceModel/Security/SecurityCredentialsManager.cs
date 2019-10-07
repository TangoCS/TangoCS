//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.IdentityModel.Selectors;

namespace System.ServiceModel.Security
{
    public abstract class SecurityCredentialsManager
    {
        protected SecurityCredentialsManager() { }

        public abstract SecurityTokenManager CreateSecurityTokenManager();
    }
}
