using System;
using System.Collections.Generic;

namespace Tango.Help
{
	public class HelpManager : IHelpManager
    {
        public void ResetCache()
        {
			Helps = null;
        }

		public static Dictionary<string, Guid> Helps { get; set; }

		public Guid Get(string name)
		{
			Guid s = Guid.Empty;
			Helps.TryGetValue(name, out s);
			return s;
		}

		public Guid this[string name]
		{
			get { return Get(name); }
		}
	}
}
