using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Nephrite.Web
{
    public static class RouteTable
    {
        public static string DefaultPath = String.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultPath"]) ? "/" : ConfigurationManager.AppSettings["DefaultPath"];

        static List<IRouteHandler> routeHandlers = new List<IRouteHandler>();
        public static void Add(IRouteHandler handler)
        {
            routeHandlers.Add(handler);
        }

        public static string ProcessRoute(string[] items)
        {
            string result;
            for (int i = 0; i < routeHandlers.Count; i++)
            {
                result = routeHandlers[i].ProcessRoute(items);
                if (result != null)
                    return result;
            }
            return null;
        }

        static string GeneratePath(string querystring)
        {
            string result;
            for (int i = 0; i < routeHandlers.Count; i++)
            {
                result = routeHandlers[i].GeneratePath(querystring);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static string GenerateUrl(string querystring)
        {
            return GeneratePath(querystring) ?? (DefaultPath + querystring);
        }
    }
}
