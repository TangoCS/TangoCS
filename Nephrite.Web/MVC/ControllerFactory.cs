using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using Nephrite.Web.Properties;
using System.Configuration;

namespace Nephrite.Web
{
    public static class ControllerFactory
    {
        static Dictionary<string, Type> controllerTypeCache = new Dictionary<string,Type>();
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        static Type[] types;

		public static void RegisterControllerType(string name, Type type)
		{
			lock (controllerTypeCache)
			{
				controllerTypeCache[name.Trim().ToLower()] = type;
			}
		}

        public static Type GetControllerType(string controllerName)
        {
            Type t = GetControllerMainType(controllerName.Trim());
			if (t == null)
				return null;
            if (t.IsGenericType)
            {
                lock (typeCache)
                {
					string type = controllerName.Trim().Split(';')[1].ToLower();
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

		static Type GetControllerMainType(string controllerName)
		{
			controllerName = HttpUtility.UrlDecode(controllerName.ToLower());
			lock (controllerTypeCache)
			{
				if (controllerTypeCache.ContainsKey(controllerName))
					return controllerTypeCache[controllerName];

				string className = "";
                
				if (controllerName.Contains(';'))
				{ 
					className = controllerName.Split(';')[0];
				}
				else
				{
					className = controllerName + "controller";
				}


				if (types == null)
				{
                    List<Type> list = new List<Type>();
					foreach (Assembly a in Utils.GetSolutionAssemblies())
					{
                        try
                        {
                            list.AddRange(a.GetTypes());
                        }
                        catch(ReflectionTypeLoadException e)
                        {
							throw new Exception(e.Message + "\r\n" + String.Join("\r\n", e.LoaderExceptions.Select(o => o.Message).ToArray()), e);
                        }
					}
					types = list.OrderBy(t => t.Name).ToArray();
				}

				for (int i = 0; i < types.Length; i++)
                    if (types[i].Name.ToLower().Replace("`1", "") == className)
                    {
                        controllerTypeCache.Add(controllerName, types[i]);
                        return types[i];
                    }
				return null;
			}
		}
    }
}
