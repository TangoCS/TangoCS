using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nephrite
{
	public static class TypeFactory
	{
		static IEnumerable<Assembly> _assemblies = null;
		static Dictionary<string, Type> controllerTypeCache = new Dictionary<string, Type>();
		static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
		static Type[] types;

		public static void RegisterControllerType(string name, Type type)
		{
			lock (controllerTypeCache)
			{
				controllerTypeCache[name.Trim().ToLower()] = type;
			}
		}

		public static Type GetType(string name)
		{
			Type t = GetMainType(name.Trim());
			if (t == null)
				return null;
			if (t.IsGenericType)
			{
				lock (typeCache)
				{
					string type = name.Trim().Split(';')[1].ToLower();
					if (typeCache.ContainsKey(type))
						return t.MakeGenericType(new Type[] { typeCache[type] });

					for (int i = 0; i < types.Length; i++)
						if (types[i].Name.ToLower() == type ||
							types[i].Name.ToLower().EndsWith("." + type))
						{
							typeCache.Add(type, types[i]);
							return t.MakeGenericType(new Type[] { types[i] });
						}
				}
			}
			return t;
		}

		static Type GetMainType(string name)
		{
			//name = HtmlHelpers.UrlDecode(name.ToLower());
			lock (controllerTypeCache)
			{
				if (controllerTypeCache.ContainsKey(name))
					return controllerTypeCache[name];

				string className = "";

				if (name.Contains(';'))
				{
					className = name.Split(';')[0];
				}
				else
				{
					className = name + "controller";
				}


				if (types == null)
				{
					List<Type> list = new List<Type>();
					foreach (Assembly a in GetSolutionAssemblies())
					{
						try
						{
							list.AddRange(a.GetTypes());
						}
						catch (ReflectionTypeLoadException e)
						{
							throw new Exception(e.Message + "\r\n" + String.Join("\r\n", e.LoaderExceptions.Select(o => o.Message).ToArray()), e);
						}
					}
					types = list.OrderBy(t => t.Name).ToArray();
				}

				for (int i = 0; i < types.Length; i++)
					if (types[i].Name.ToLower().Replace("`1", "") == className)
					{
						controllerTypeCache.Add(name, types[i]);
						return types[i];
					}
				return null;
			}
		}

		public static IEnumerable<Assembly> GetSolutionAssemblies()
		{
			if (_assemblies == null)
			{
				string modelAssembly = "Tessera3.Test";
				IEnumerable<string> s = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin", "Nephrite*.dll").Select(o => Path.GetFileName(o).ToLower());
				IEnumerable<Assembly> res = AppDomain.CurrentDomain.GetAssemblies().Where(o => 
					s.Contains(o.ManifestModule.ScopeName.ToLower()) || 
					o.ManifestModule.ScopeName.ToLower() == modelAssembly.ToLower() + ".dll");
				_assemblies = res;
			}
			return _assemblies;
		}
	}
}
