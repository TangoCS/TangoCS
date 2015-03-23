﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Nephrite.Http
{
	internal static class Constants
	{
		internal const string Https = "HTTPS";

		internal const string HttpDateFormat = "r";

		internal static class Headers
		{
			internal const string ContentType = "Content-Type";
			internal const string CacheControl = "Cache-Control";
			internal const string MediaType = "Media-Type";
			internal const string Accept = "Accept";
			internal const string AcceptCharset = "Accept-Charset";
			internal const string Host = "Host";
			internal const string ETag = "ETag";
			internal const string Location = "Location";
			internal const string ContentLength = "Content-Length";
			internal const string Cookie = "Cookie";
			internal const string SetCookie = "Set-Cookie";
			internal const string Expires = "Expires";
			internal const string WebSocketSubProtocols = "Sec-WebSocket-Protocol";
		}

		internal static class BuilderProperties
		{
			internal static string ServerInformation = "server.Information";
			internal static string ApplicationServices = "application.Services";
		}
	}
}