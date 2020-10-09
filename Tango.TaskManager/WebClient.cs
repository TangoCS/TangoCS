using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tango.TaskManager
{
	public static class WebClient
	{
		public static void Post(string url, string userName, string password, string data, int timeOut = 20)
		{
			var wr = HttpWebRequest.Create(url);
			wr.Timeout = timeOut == -1 ? -1 : (timeOut * 60000);
            if (!string.IsNullOrWhiteSpace(userName))
                wr.Credentials = new NetworkCredential(userName, password);
			wr.Method = "POST";
			wr.ContentType = "application/x-www-form-urlencoded";

			var byteArray = Encoding.UTF8.GetBytes(data);
			wr.ContentLength = byteArray.Length;
            wr.Headers.Add("x-request-guid", Guid.NewGuid().ToString());

            using (var req = wr.GetRequestStream())
			{
				req.Write(byteArray, 0, byteArray.Length);
				req.Close();
			}

			using (var response = wr.GetResponse() as HttpWebResponse)
			using (var dataStream = response.GetResponseStream())
			using (var reader = new StreamReader(dataStream))
			{
				string responseFromServer = reader.ReadToEnd();
				dataStream.Close();
				if (response.StatusCode != HttpStatusCode.OK)
					throw new Exception("Ошибка вызова " + url + ":" + Environment.NewLine + Environment.NewLine + responseFromServer);
                response.Close();
            }
        }
	}
}
