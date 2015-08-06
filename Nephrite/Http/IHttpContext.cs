using Microsoft.AspNet.Http;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Text;
using System.IO;

namespace Nephrite.Http
{
	public interface IHttpContext : IDisposable
	{
		IHttpRequest Request { get; }
		IHttpResponse Response { get; }
		IDictionary Items { get; }
		//IPersistentSettings Settings { get; } 
		IPrincipal User { get; }

		//object GetFeature(Type type);
		//void SetFeature(Type type, object instance);
		IServiceProvider ApplicationServices { get; set; }
		IServiceProvider RequestServices { get; set; }

		T GetFeature<T>();
		void SetFeature<T>(T instance);
	}

	public interface IHttpRequest
	{
		NameValueCollection Query { get; }
		IReadableStringCollection Cookies { get; }
		Uri Url { get; }
		NameValueCollection Headers { get; }
		NameValueCollection Form { get; }

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