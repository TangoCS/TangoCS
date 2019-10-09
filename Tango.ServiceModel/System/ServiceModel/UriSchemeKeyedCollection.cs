//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;
    using System.Collections.Generic;

    public class UriSchemeKeyedCollection : SynchronizedKeyedCollection<string, Uri>
    {
        internal UriSchemeKeyedCollection(object syncRoot)
            : base(syncRoot)
        {
        }

        public UriSchemeKeyedCollection(params Uri[] addresses)
        {
            if (addresses == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addresses");

            for (int i = 0; i < addresses.Length; i++)
            {
                this.Add(addresses[i]);
            }
        }

        protected override string GetKeyForItem(Uri item)
        {
            return item.Scheme;
        }

        protected override void InsertItem(int index, Uri item)
        {
            ValidateBaseAddress(item, "item");
            if (this.Contains(item.Scheme))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("item", Res.GetString(Res.S("BaseAddressDuplicateScheme"), item.Scheme));

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, Uri item)
        {
            ValidateBaseAddress(item, "item");
            if (this[index].Scheme != item.Scheme)
            {
                if (this.Contains(item.Scheme))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("item", Res.GetString(Res.S("BaseAddressDuplicateScheme"), item.Scheme));
            }
            base.SetItem(index, item);
        }

        internal static void ValidateBaseAddress(Uri uri, string argumentName)
        {
            if (uri == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(argumentName);
            }

            if (!uri.IsAbsoluteUri)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argumentName, Res.GetString(Res.S("BaseAddressMustBeAbsolute")));
            }

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argumentName, Res.GetString(Res.S("BaseAddressCannotHaveUserInfo")));
            }

            if (!string.IsNullOrEmpty(uri.Query))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argumentName, Res.GetString(Res.S("BaseAddressCannotHaveQuery")));
            }

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argumentName, Res.GetString(Res.S("BaseAddressCannotHaveFragment")));
            }
        }
    }
}
