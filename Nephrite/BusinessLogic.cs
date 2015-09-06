using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite
{
	public interface ILogic
	{

	}

	public static class LogicHelper
	{
		public static Delegate GetObjectLogic(object obj, string name)
		{
			var logicType = LogicCache.Get(obj.GetType().Name + "Logic");
			if (logicType == null) return null;

			var logic = DI.RequestServices.GetService(logicType);
			var method = logicType.GetMethod(name);
			if (method == null) return null;

			return method.CreateDelegate(
				Expression.GetDelegateType(
					(from parameter in method.GetParameters() select parameter.ParameterType)
					.Concat(new[] { method.ReturnType })
					.ToArray()
				), logic);
		}
	}

	public static class LogicCache
	{
		static Dictionary<string, Type> _collection = new Dictionary<string, Type>();
		public static void Add<T>() where T : ILogic
		{
			_collection.Add(typeof(T).Name.ToLower(), typeof(T));
		}
		public static void Add<T>(string name) where T : ILogic
		{
			_collection.Add(name.ToLower(), typeof(T));
		}
		public static Type Get(string name)
		{
			if (!_collection.ContainsKey(name.ToLower())) return null;
			return _collection[name.ToLower()];
		}

		public static void AddLogic<T>(this IServiceCollection sc) where T : ILogic
		{
			Add<T>();
			sc.AddScoped<T>();
		}
	}
}
