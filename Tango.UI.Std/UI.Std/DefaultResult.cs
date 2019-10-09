using System;
using System.Text;
using Newtonsoft.Json;

namespace Tango.UI.Std
{
	public class DefaultResult : HttpResult
	{
		public string Title { get; set; }
		public string Html { get; set; }

		public DefaultResult(string title, string html)
		{
			Title = title;
			Html = html;
			ContentType = "application/json";
			ContentFunc = ctx => {
				ApiResponse data = new ApiResponse();
				data.AddWidget("contenttitle", Title);
				data.AddWidget("contentbody", Html);
				return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Json.StdSettings));
			};
		}

		public DefaultResult(string html) : this("Внимание!", html)
		{
		}
	}
}
