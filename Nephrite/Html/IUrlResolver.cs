using System.Collections.Generic;
using System.Text;

namespace Nephrite.Html
{
	public interface IUrlResolver
	{
		StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false);
	}

	public abstract class AbstractUrlResolver : IUrlResolver
	{
        public abstract StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false);

		protected StringBuilder CreateUrl(string url, IDictionary<string, object> parms, bool isHashPart)
		{
			StringBuilder _res = new StringBuilder();

			if (!url.IsEmpty())
			{
				string[] parts = url.Split(new char[] { '/' });

				foreach (string part in parts)
				{
					if (part.StartsWith("{"))
					{
						string key = part.Substring(1, part.Length - 2);
						if (parms.ContainsKey(key))
						{
							_res.Append("/").Append(parms[key]);
							parms.Remove(key);
						}
					}
					else
						_res.Append("/").Append(part);
				}
			}
			
            if (parms.Count > 0)
			{
				_res.Append(isHashPart ? '/' : '?');
			}
			bool first = true;
			foreach (var parm in parms)
			{
				if (!first) _res.Append("&");
				_res.Append(isHashPart ? parm.Key.Replace("#", "") : parm.Key);
				if (parm.Value != null) _res.Append("=").Append(parm.Value);
				first = false;
			}

			return _res;
		}
	}

	public class UrlResolver : AbstractUrlResolver
	{
		protected string _routeTemplate = "";

		public virtual UrlResolver UseRouteTemplate(string routeTemplate)
		{
			_routeTemplate = routeTemplate;
			return this;
		}

		public override StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false)
		{
			return CreateUrl(_routeTemplate, parameters, isHashPart);
		}
	}
}
