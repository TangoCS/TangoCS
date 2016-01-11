using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Nephrite.AccessControl;
using Nephrite.Html;

namespace Nephrite.Web
{
	//public class RouteUrlResolver : AbstractUrlResolver
	//{
	//	protected string _routeName;

	//	public virtual RouteUrlResolver UseRoute(string routeName)
	//	{
	//		_routeName = routeName;
	//		return this;
	//	}

	//	public override StringBuilder Resolve(IDictionary<string, object> parameters, bool isHashPart = false)
	//	{
	//		return GeneratePathAndQueryFromRoute(_routeName, parameters, isHashPart) ?? new StringBuilder();
	//	}

	//	public StringBuilder GeneratePathAndQueryFromRoute(string routeName, IDictionary<string, object> values, bool isHashPart)
	//	{
	//		if (routeName != null)
	//		{
	//			var route = RouteTable.Routes[routeName];
	//			if (route == null) return null;
	//			return CreateUrl((route as Route).Url, values, isHashPart);
	//		}
	//		else
	//		{
	//			var d = new RouteValueDictionary(values);
	//			var r = HttpContext.Current.Request.RequestContext;
	//			VirtualPathData virtualPathData = RouteTable.Routes.GetVirtualPath(r, d);
	//			if (virtualPathData == null) return null;
	//			return CreateUrl((virtualPathData.Route as Route).Url, values, isHashPart);
	//		}
	//	}	
	//}

	
}
