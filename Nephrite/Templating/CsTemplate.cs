using System;
using System.Collections.Generic;
using Nephrite.MVC;

namespace Nephrite.Templating
{
	public static class CsTemplateCache
	{
		static Dictionary<string, Type> _collection = new Dictionary<string, Type>();
		public static void Add<T>() where T : ICsTemplate
		{
			_collection.Add(typeof(T).Name.ToLower(), typeof(T));
		}
		public static void Add<T>(string name) where T : ICsTemplate
		{
			_collection.Add(name.ToLower(), typeof(T));
		}
		public static Type Get(string name)
		{
			if (!_collection.ContainsKey(name.ToLower())) return null;
			return _collection[name.ToLower()];
		}
	}

	public interface ICsTemplate
	{
		CsTemplateContext Context { get; }
		CsTemplate SetContext(CsTemplateContext context);
		ActionResult Execute();
		string ToString();
	}

	public abstract class CsTemplate : ICsTemplate
	{
		public CsTemplateContext Context { get; private set; }
		public abstract ActionResult Execute();

		public CsTemplate SetContext(CsTemplateContext context)
		{
			Context = context;
			return this;
		}
	}

	public class CsTemplateContext
	{
		public string Service { get; set; }
		public string Action { get; set; }
		public string Event { get; set; }

		public dynamic ActionArgs { get; set; }
		public dynamic EventArgs { get; set; }

		public dynamic Data { get; set; }

		public CsTemplateContext()
		{
			ActionArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
			EventArgs = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);
		}
	}
}
