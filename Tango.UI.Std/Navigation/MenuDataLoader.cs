using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tango.Localization;

namespace Tango.UI.Navigation
{
	public interface IMenuDataLoader
	{
		IEnumerable<MenuItem> Load(string name);
	}

	public class DefaultMenuDataLoader : IMenuDataLoader
	{
		protected IHostingEnvironment _env;
		protected IResourceManager _resource;
		protected IMenuItemResolverCollection _resolvers;
		//protected IAccessControl _accessControl;

		public DefaultMenuDataLoader(IHostingEnvironment env, IMenuItemResolverCollection resolvers, IResourceManager resource)
		{
			_env = env;
			_resource = resource;
			_resolvers = resolvers;
			//_accessControl = accessControl;
		}

		void ProcessLevel(int level, IDictionary<string, string> parentParms, List<MenuItem> collection, dynamic defaults, dynamic items)
		{
			int seq = 0;
			foreach (var i in items)
			{
				bool? isVisible = i.visible;
				if (isVisible != null && !isVisible.Value) continue;

				string resolver = i.resolver ?? defaults[level].resolver;			
				var parms = (i.parms as DynamicDictionary).ToDictionary(o => o.Key, o => o.Value.ToString());
				if (parentParms != null)
					foreach (var parentParm in parentParms)
						if (!parms.ContainsKey(parentParm.Key))
							parms.Add(parentParm.Key, parentParm.Value);

				var mitems = _resolvers[resolver].Resolve(parms);
				foreach (var m in mitems)
				{
					m.SeqNo = seq++;
					m.Image = i.image;
					m.Title = _resource.Get(m.ResourceKey);
					m.CssClass = i.cssclass;
					
					if (i.children != null)
						ProcessLevel(level + 1, parms, m.Children, defaults, i.children);
				}
				collection.AddRange(mitems);
			}
		}

		protected virtual string GetData(string name)
		{
			var fileName = name + ".json";
			var path = _env.MapPath("data" + Path.DirectorySeparatorChar + fileName);
			if (File.Exists(path)) return File.ReadAllText(path);
			return null;
		}

		public IEnumerable<MenuItem> Load(string name)
		{
			var data = GetData(name);

			var res = new List<MenuItem>();
			if (data == null) return res;
			dynamic menudata = JsonConvert.DeserializeObject<DynamicDictionary>(data, new DynamicDictionaryConverter());
			if (menudata == null || menudata.menu == null) return res;

			ProcessLevel(0, null, res, menudata.defaults, menudata.menu);
			return res;
		}
	}
}
