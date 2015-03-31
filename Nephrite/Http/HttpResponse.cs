// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public abstract class AbstractHttpResponse
	{
		// TODO - review IOwinResponse for completeness

		public abstract AbstractHttpContext HttpContext { get; }
		public abstract int StatusCode { get; set; }
		public abstract IHeaderDictionary Headers { get; }
		public abstract Stream Body { get; set; }

		public abstract long? ContentLength { get; set; }
		public abstract string ContentType { get; set; }

		public abstract IResponseCookies Cookies { get; }

		public abstract bool HeadersSent { get; }

		public abstract void OnSendingHeaders(Action<object> callback, object state);

		public abstract void OnResponseCompleted(Action<object> callback, object state);

		public virtual void Redirect(string location)
		{
			Redirect(location, permanent: false);
		}

		public abstract void Redirect(string location, bool permanent);

		public virtual void Challenge()
		{
			Challenge(new string[0]);
		}

		public virtual void Challenge(AuthenticationProperties properties)
		{
			Challenge(properties, new string[0]);
		}

		public virtual void Challenge(string authenticationScheme)
		{
			Challenge(new[] { authenticationScheme });
		}

		public virtual void Challenge(AuthenticationProperties properties, string authenticationScheme)
		{
			Challenge(properties, new[] { authenticationScheme });
		}

		public virtual void Challenge(params string[] authenticationSchemes)
		{
			Challenge((IEnumerable<string>)authenticationSchemes);
		}

		public virtual void Challenge(IEnumerable<string> authenticationSchemes)
		{
			Challenge(properties: null, authenticationSchemes: authenticationSchemes);
		}

		public virtual void Challenge(AuthenticationProperties properties, params string[] authenticationSchemes)
		{
			Challenge(properties, (IEnumerable<string>)authenticationSchemes);
		}

		public abstract void Challenge(AuthenticationProperties properties, IEnumerable<string> authenticationSchemes);

		public abstract void SignIn(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties = null);

		public virtual void SignOut()
		{
			SignOut(authenticationScheme: null, properties: null);
		}

		public abstract void SignOut(string authenticationScheme);

		public abstract void SignOut(string authenticationScheme, AuthenticationProperties properties);
	}
}
