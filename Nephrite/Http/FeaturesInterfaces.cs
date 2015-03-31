// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public interface IRequestCookiesFeature
	{
		IReadableStringCollection Cookies { get; }
	}

	public interface IResponseCookiesFeature
	{
		IResponseCookies Cookies { get; }
	}

	public interface IQueryFeature
	{
		IReadableStringCollection Query { get; }
	}

	public interface IItemsFeature
	{
		IDictionary<object, object> Items { get; }
	}

	public interface IFormFeature
	{
		/// <summary>
		/// Indicates if the request has a supported form content-type.
		/// </summary>
		bool HasFormContentType { get; }

		/// <summary>
		/// The parsed form, if any.
		/// </summary>
		IFormCollection Form { get; set; }

		/// <summary>
		/// Parses the request body as a form.
		/// </summary>
		/// <returns></returns>
		IFormCollection ReadForm();

		/// <summary>
		/// Parses the request body as a form.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken);
	}

	public interface IServiceProvidersFeature
	{
		IServiceProvider ApplicationServices { get; set; }
		IServiceProvider RequestServices { get; set; }
	}

	public interface IHttpRequestFeature
	{
		string Protocol { get; set; }
		string Scheme { get; set; }
		string Method { get; set; }
		string PathBase { get; set; }
		string Path { get; set; }
		string QueryString { get; set; }
		IDictionary<string, string[]> Headers { get; set; }
		Stream Body { get; set; }
	}

	public interface IHttpResponseFeature
	{
		int StatusCode { get; set; }
		string ReasonPhrase { get; set; }
		IDictionary<string, string[]> Headers { get; set; }
		Stream Body { get; set; }
		bool HeadersSent { get; }
		void OnSendingHeaders(Action<object> callback, object state);
		void OnResponseCompleted(Action<object> callback, object state);
	}

	public interface IHttpConnectionFeature
	{
		IPAddress RemoteIpAddress { get; set; }
		IPAddress LocalIpAddress { get; set; }
		int RemotePort { get; set; }
		int LocalPort { get; set; }
		bool IsLocal { get; set; }
	}

	public interface IHttpClientCertificateFeature
	{
		/// <summary>
		/// Synchronously retrieves the client certificate, if any.
		/// </summary>
		X509Certificate ClientCertificate { get; set; }

		/// <summary>
		/// Asynchronously retrieves the client certificate, if any.
		/// </summary>
		/// <returns></returns>
		Task<X509Certificate> GetClientCertificateAsync(CancellationToken cancellationToken);
	}

	public interface IHttpWebSocketFeature
	{
		bool IsWebSocketRequest { get; }
		Task<WebSocket> AcceptAsync(IWebSocketAcceptContext context);
	}

	public interface IWebSocketAcceptContext
	{
		string SubProtocol { get; set; }
	}

	public interface IHttpRequestLifetimeFeature
	{
		CancellationToken RequestAborted { get; }
		void Abort();
	}

	// TODO: Is there any reason not to flatten the Factory down into the Feature?
	public interface ISessionFeature
	{
		ISessionFactory Factory { get; set; }
		ISession Session { get; set; }
	}

	public interface ISession
	{
		void Load();
		void Commit();
		bool TryGetValue(string key, out byte[] value);
		void Set(string key, ArraySegment<byte> value);
		void Remove(string key);
		void Clear();
		IEnumerable<string> Keys { get; }
	}

	public interface ISessionFactory
	{
		bool IsAvailable { get; }
		ISession Create();
	}

	public interface IHttpAuthenticationFeature
	{
		ClaimsPrincipal User { get; set; }
		IAuthenticationHandler Handler { get; set; }
	}

	public interface IAuthenticationHandler
	{
		void GetDescriptions(IDescribeSchemesContext context);
		void Authenticate(IAuthenticateContext context);
		Task AuthenticateAsync(IAuthenticateContext context);
		void Challenge(IChallengeContext context);
		void SignIn(ISignInContext context);
		void SignOut(ISignOutContext context);
	}

	public interface ISignInContext
	{
		//IEnumerable<ClaimsPrincipal> Principals { get; }
		ClaimsPrincipal Principal { get; }
		IDictionary<string, string> Properties { get; }
		string AuthenticationScheme { get; }
		void Accept(IDictionary<string, object> description);
	}

	public interface ISignOutContext
	{
		string AuthenticationScheme { get; }
		IDictionary<string, string> Properties { get; }
		void Accept();
	}

	public interface IChallengeContext
	{
		IEnumerable<string> AuthenticationSchemes { get; }
		IDictionary<string, string> Properties { get; }
		void Accept(string authenticationType, IDictionary<string, object> description);
	}

	public interface IAuthenticateContext
	{
		string AuthenticationScheme { get; }
		void Authenticated(ClaimsPrincipal principal, IDictionary<string, string> properties, IDictionary<string, object> description);
		void NotAuthenticated();
	}

	public interface IDescribeSchemesContext
	{
		void Accept(IDictionary<string, object> description);
	}
}
