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

		public LinkMenuItemResolver(string keyTemplate, string urlTemplate)
		{
			_resourceKeyTemplate = keyTemplate;
			_securableObjectKeyTemplate = keyTemplate;
			_urlTemplate = urlTemplate;
		}

		public LinkMenuItemResolver(string resourceKeyTemplate, string securableObjectKeyTemplate, string urlTemplate)
		{
			_resourceKeyTemplate = resourceKeyTemplate;
			_urlTemplate = urlTemplate;
			_securableObjectKeyTemplate = securableObjectKeyTemplate;
		}

		public IEnumerable<MenuItem> Resolve(IReadOnlyDictionary<string, string> parms)
		{
			var res = new List<MenuItem>();
			var target = new ActionTarget();

			foreach (var p in parms)
				if (p.Key == Constants.ServiceName)
					target.Service = p.Value;
				else if (p.Key == Constants.ActionName)
					target.Action = p.Value;
				else if (p.Key != "namespace")
					target.Args.Add(p.Key, p.Value);

			MenuItem m = new MenuItem {
				ResourceKey = RouteUtils.Resolve(_resourceKeyTemplate, parms, null, true).ToString(),
				SecurableObjectKey = RouteUtils.Resolve(_securableObjectKeyTemplate, parms, null, true).ToString(),
				Url = RouteUtils.Resolve(_urlTemplate, parms, null, true).ToString(),
				Target = target
			};
			res.Add(m);
			return res;
		}
	}
}
