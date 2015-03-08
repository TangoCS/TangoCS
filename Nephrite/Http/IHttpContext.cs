using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nephrite.Http
{
	public interface IHttpContext : IDisposable
	{
		IRequest Request { get; }
		IDictionary Items { get; }
		//IPersistentSettings Settings { get; } 
		IPrincipal User { get; }

		//object GetFeature(Type type);
		//void SetFeature(Type type, object instance);

		T GetFeature<T>();
		void SetFeature<T>(T instance);
	}

	public interface IRequest
	{
		NameValueCollection Query { get; }
		RequestCookiesCollection Cookies { get; }
		Uri Url { get; }
		NameValueCollection Headers { get; }
		NameValueCollection Form { get; }

		//Uri UrlReferrer { get; }
		//string UserName { get; }
		//string UserAgent { get; }
		//string UserHostAddress { get; }
	}
}
