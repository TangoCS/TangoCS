using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Templating
{
	public class ApiResponse
	{
		public Dictionary<string, object> Data { get; set; }

		public Dictionary<string, object> Widgets { get; set; }
		public List<KeyValuePair<string, object>> ClientActions { get; set; }


		public ApiResponse()
		{
			Data = new Dictionary<string, object>();
			Widgets = new Dictionary<string, object>();
			ClientActions = new List<KeyValuePair<string, object>>();
            Data.Add("widgets", Widgets);
			Data.Add("clientactions", ClientActions);
		}

		public void AddWidget(string name, string content)
		{
			Widgets.Add(name, content);
		}

		public void BindEvent(string elementId, string clientEvent, string serverEvent)
		{
			ClientActions.Add(new KeyValuePair<string, object>("bindevent", new { Id = elementId, ClientEvent = clientEvent, ServerEvent = serverEvent }));
        }

		public void AddClientAction(string name, object data)
		{
			ClientActions.Add(new KeyValuePair<string, object>(name, data));
        }
	}
}
