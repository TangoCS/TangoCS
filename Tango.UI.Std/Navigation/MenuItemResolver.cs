using System.Collections.Generic;

namespace Tango.UI.Navigation
{
	public interface IMenuItemResolver
	{
		IEnumerable<MenuItem> Resolve(IReadOnlyDictionary<string, string> parms);
	}

	public interface IMenuItemResolverCollection : IReadOnlyDictionary<string, IMenuItemResolver>
	{
		
	}

	public class MenuItemResolverCollection : Dictionary<string, IMenuItemResolver>, IMenuItemResolverCollection
	{

	}

	public class LinkMenuItemResolver : IMenuItemResolver
	{
		string _urlTemplate = "";
		string _resourceKeyTemplate = "";
		string _securableObjectKeyTemplate = "";
		IReadOnlyDictionary<string, string> _defaultParms;

		public LinkMenuItemResolver(string keyTemplate, string urlTemplate, IReadOnlyDictionary<string, string> defaultParms = null)
		{
			_resourceKeyTemplate = keyTemplate;
			_securableObjectKeyTemplate = keyTemplate;
			_urlTemplate = urlTemplate;
			_defaultParms = defaultParms;
		}

		public LinkMenuItemResolver(string resourceKeyTemplate, string securableObjectKeyTemplate, string urlTemplate, 
			IReadOnlyDictionary<string, string> defaultParms = null)
		{
			_resourceKeyTemplate = resourceKeyTemplate;
			_urlTemplate = urlTemplate;
			_securableObjectKeyTemplate = securableObjectKeyTemplate;
			_defaultParms = defaultParms;
		}

		public IEnumerable<MenuItem> Resolve(IReadOnlyDictionary<string, string> parms)
		{
			var res = new List<MenuItem>();
			//var target = new ActionTarget();

			//foreach (var p in parms)
			//	if (p.Key == Constants.ServiceName)
			//		target.Service = p.Value;
			//	else if (p.Key == Constants.ActionName)
			//		target.Action = p.Value;
			//	else if (p.Key != "namespace")
			//		target.Args.Add(p.Key, p.Value);

			var p = new Dictionary<string, string>();

			if (_defaultParms != null)
				foreach (var pp in _defaultParms)
					p.Add(pp.Key, pp.Value);

			foreach (var pp in parms)
				p.Add(pp.Key, pp.Value);

			MenuItem m = new MenuItem {
				ResourceKey = RouteUtils.Resolve(_resourceKeyTemplate, p, null, true).ToString(),
				SecurableObjectKey = RouteUtils.Resolve(_securableObjectKeyTemplate, p, null, true).ToString(),
				Url = RouteUtils.Resolve(_urlTemplate, p, null, true).ToString(),
				//Target = target
			};
			res.Add(m);
			return res;
		}
	}
}
