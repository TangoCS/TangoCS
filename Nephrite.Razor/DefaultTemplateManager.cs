using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;
using RazorEngine;
using System.IO;

namespace Nephrite.Razor
{
	public class DefaultTemplateManager : ITemplateManager
	{
		static Dictionary<string, ITemplateSource> _cache;		
		string _rootPath = "";

		public DefaultTemplateManager(string rootPath)
		{
			_cache = new Dictionary<string, ITemplateSource>();
			_rootPath = rootPath;
		}

		public ITemplateSource Resolve(ITemplateKey key)
		{
			string path = Path.Combine(_rootPath, key.Name);
			if (_cache.ContainsKey(path))
				return _cache[path];
			else
			{
				var s = File.ReadAllText(path, System.Text.Encoding.Default);
				var t = new LoadedTemplateSource(s, null);
				_cache.Add(path, t);
				return t;
			}
		}

		public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
		{
			return new NameOnlyTemplateKey(name, resolveType, context);
		}

		public void AddDynamic(ITemplateKey key, ITemplateSource source)
		{
			string path = Path.Combine(_rootPath, key.Name);

			if (!_cache.ContainsKey(path))
			{
				var s = File.ReadAllText(path, System.Text.Encoding.Default);
				var t = new LoadedTemplateSource(s, null);
				_cache.Add(path, t);
			}
		}
	}
}
