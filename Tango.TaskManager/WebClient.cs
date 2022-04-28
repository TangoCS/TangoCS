using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Tango.TaskManager
{
	public static class WebClient
	{
		public static void Post(string url, string userName, string password, int timeOut = 3, Dictionary<string, string> data = null)
		{
			var request = HttpWebRequest.Create(url);
            if (!string.IsNullOrWhiteSpace(userName))
				request.Credentials = new NetworkCredential(userName, password);
			request.Timeout = timeOut == -1 ? Timeout.Infinite : timeOut == 0 ? 60000 : (timeOut * 60000);
			request.Method = "POST";
			request.ContentType = "application/json; charset=utf-8";
			request.ContentLength = 0;
			request.Headers.Add("x-request-guid", Guid.NewGuid().ToString());

			if (data != null && data.Count > 0)
			{
				string json = "{";
				foreach (var item in data)
				{
					json += $"'{item.Key}':'{item.Value}'";
				}
				json +="}";
				var byteArray = Encoding.UTF8.GetBytes(json);
				request.ContentLength = byteArray.Length;

				using (var writer = request.GetRequestStream())
				{
					writer.Write(byteArray, 0, byteArray.Length);
				}
			}

			using (var response = request.GetResponse() as HttpWebResponse)
			using (var dataStream = response.GetResponseStream())
			using (var reader = new StreamReader(dataStream))
			{
				string responseFromServer = reader.ReadToEnd();
				if (response.StatusCode != HttpStatusCode.OK)
					throw new Exception("Ошибка вызова " + url + ":" + Environment.NewLine + Environment.NewLine + responseFromServer);
            }
        }
	}
}
