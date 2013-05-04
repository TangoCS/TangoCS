using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web
{
    public static class MacroManager
    {
        static Dictionary<string, Func<Object>> macrolist = new Dictionary<string, Func<object>>();

        public static void Register(string name, Func<Object> evaluateFunc)
        {
            lock (macrolist)
            {
				if (macrolist.ContainsKey(name))
					return;
                macrolist.Add(name, evaluateFunc);
            }
        }

        public static object Evaluate(string name)
        {
            if (macrolist.ContainsKey(name))
            {
                return macrolist[name]();
            }
            return name;
        }
    }
}
