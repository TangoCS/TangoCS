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

		//public static void AddController<T>(this IServiceCollection sc) where T : Controller
		//{
		//	Add<T>();
		//	sc.AddScoped<T>();
		//}
	}

	public interface ICsTemplate
	{
        void SetData(object model);
		void Execute();
		string ToString();
	}

	public interface ICsTemplate<T> : ICsTemplate
	{
		T Model { get; set; }
	}

	public abstract class CsTemplate<T> : ICsTemplate<T>
	{
		public T Model { get; set; }
		public abstract void Execute();

		public void SetData(object model)
		{
			Model = (T)model;
		}

		public override string ToString()
		{
			return "";
		}
	}
}
