using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

namespace Tango.TaskManager
{
	public static class WebClient
	{
		public static void Post(string url, string userName, string password, int timeOut = 3, Dictionary<string, string> data = null)
		{
			HttpClientHandler handler = null;
			if (!string.IsNullOrWhiteSpace(userName))
			{
				handler = new HttpClientHandler();
				handler.Credentials = new NetworkCredential(userName, password);
			}
			using var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
			httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10;
			httpClient.Timeout = timeOut == -1 ? Timeout.InfiniteTimeSpan : timeOut == 0 ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(timeOut);

			var message = new HttpRequestMessage(HttpMethod.Post, url);
			message.Headers.Add("x-request-guid", Guid.NewGuid().ToString());
			if (data != null && data.Count > 0)
			{
				var content = JsonContent.Create(data);
				message.Content = content;
			}

			var response = httpClient.Send(message);
			string responseText = response.Content.ReadAsStringAsync().Result;
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Ошибка вызова {url}{Environment.NewLine}Код ошибки ({(int)response.StatusCode}){Environment.NewLine}{responseText}");
		}
	}
}
