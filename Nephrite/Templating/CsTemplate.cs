using System;
using System.Collections.Generic;
using Nephrite.Html.Layout;
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
		string ID { get; }
		ActionContext Context { get; set; }
		string ToString();
		LayoutWriter CreateLayoutWriter();
	}

	public abstract class CsTemplate : ICsTemplate
	{
		public string ID { get; set; }
		public ActionContext Context { get; set; }
		public abstract ActionResult Execute();
		public abstract LayoutWriter CreateLayoutWriter();

		protected dynamic PostBag { get { return Context.PostData; } }
		protected DynamicDictionary PostData { get { return Context.PostData; } }
	}
}
