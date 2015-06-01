// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Http.Authentication
{
    /// <summary>
    /// Contains information describing an authentication provider.
    /// </summary>
    public class AuthenticationDescription
    {
        private const string CaptionPropertyKey = "Caption";
        private const string AuthenticationSchemePropertyKey = "AuthenticationScheme";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDescription"/> class
        /// </summary>
        public AuthenticationDescription()
            : this(items: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDescription"/> class
        /// </summary>
        /// <param name="items"></param>
        public AuthenticationDescription(IDictionary<string, object> items)
        {
            Items = items ?? new Dictionary<string, object>(StringComparer.Ordinal); ;
        }

        /// <summary>
        /// Contains metadata about the authentication provider.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the name used to reference the authentication middleware instance.
        /// </summary>
        public string AuthenticationScheme
        {
            get { return GetString(AuthenticationSchemePropertyKey); }
            set { Items[AuthenticationSchemePropertyKey] = value; }
        }

        /// <summary>
        /// Gets or sets the display name for the authentication provider.
        /// </summary>
        public string Caption
        {
            get { return GetString(CaptionPropertyKey); }
            set { Items[CaptionPropertyKey] = value; }
        }

        private string GetString(string name)
        {
            object value;
            if (Items.TryGetValue(name, out value))
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            return null;
        }
    }
}
