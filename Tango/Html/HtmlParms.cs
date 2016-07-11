using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Html
{
	public class HtmlParms : Dictionary<string, object>
	{
		public HtmlParms() : base() { }
		public HtmlParms(IDictionary<string, object> dictionary) : base(dictionary) { }

		public override string ToString()
		{
			return this.Select(o => o.Key + "=" + (o.Value ?? "").ToString()).Join("&");
		}

		public HtmlParms(string key, object value)
			: base()
		{
			Add(key, value);
		}
	}
}
