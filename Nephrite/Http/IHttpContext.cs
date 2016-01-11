using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Nephrite.Http
{
	public interface IHttpContext : IDisposable
	{
		IHttpRequest Request { get; }
		IHttpResponse Response { get; }
		IDictionary Items { get; }
		//IPersistentSettings Settings { get; } 
		IPrincipal User { get; }
	}

	public interface IHttpRequest
	{
		string QueryString { get; }
		NameValueCollection Query { get; }
		IReadOnlyDictionary<string, string> Cookies { get; }
		//Uri Url { get; }
		//NameValueCollection Headers { get; }
		//NameValueCollection Form { get; }

		string ContentType { get; }
		string Method { get; }

		Stream Body { get; }

		//Uri UrlReferrer { get; }
		//string UserName { get; }
		//string UserAgent { get; }
		//string UserHostAddress { get; }
	}

	public interface IHttpResponse
	{
		string ContentType { get; set; }
		Stream Body { get; }
        void Redirect(string url);
	}

	public static class IHttpResponseExtensions
	{
		public static void Write(this IHttpResponse response, string text)
		{
			byte[] b = Encoding.UTF8.GetBytes(text);
			response.Body.Write(b, 0, b.Length);
		}
    }
}