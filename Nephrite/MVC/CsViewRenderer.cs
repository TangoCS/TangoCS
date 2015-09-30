using System;
using System.Collections.Generic;
using System.IO;
using Nephrite.Html;

namespace Nephrite.MVC
{
	public class CsViewRenderer : IViewRenderer
	{
		public bool IsStringResult
		{
			get
			{
				return true;
			}
		}

		string _result = "";

		public void RenderHtml(string title, string html)
		{
			throw new NotImplementedException();
		}

		public void RenderMessage(string message)
		{
			throw new NotImplementedException();
		}

		public void RenderMessage(string title, string message)
		{
			throw new NotImplementedException();
		}

		public void RenderView(string folder, string viewName, object viewData)
		{
			var tname = (folder + "_" + viewName).ToLower();
			var typeCache = DI.GetService<ITypeActivatorCache>();
            Type t = typeCache.Get(tname);
			ICsTemplate v = Activator.CreateInstance(t) as ICsTemplate;
			v.SetData(viewData);
			v.Execute();
			_result = v.ToString();
        }

		public override string ToString()
		{
			return _result;
		}

		public static string Render(string folder, string viewName, object viewData)
		{
			var r = new CsViewRenderer();
			r.RenderView(folder, viewName, viewData);
			return r.ToString();
		}
		public static string Render(string folder, string viewName)
		{
			return Render(folder, viewName, "");
		}
		public static ActionResult Render<T, TModel>(TModel viewData, string e = null) where T : ICsTemplate<TModel>, new()
		{
			T v = new T();
			v.SetData(viewData);
			return v.Execute(e);
		}
	}

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
		ICsTemplate SetData(object model);
		ActionResult Execute(string e = null);
		string ToString();
	}

	public interface ICsTemplate<T> : ICsTemplate
	{
		T Model { get; }
	}

	public abstract class CsTemplate<T> : ICsTemplate<T>
	{
		public T Model { get; set; }
		public abstract ActionResult Execute(string e = null);

		public ICsTemplate SetData(object model)
		{
			Model = (T)model;
			return this;
		}
	}
}
