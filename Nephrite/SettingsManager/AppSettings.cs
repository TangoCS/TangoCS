using System.Collections.Generic;
using System.Linq;

namespace Nephrite.SettingsManager
{
	public class PersistentSetting
	{
		public string SystemName { get; set; }
		public string Value { get; set; }
	}

	public class AppSettings : IPersistentSettings
	{
        public static void ResetCache()
        {
			Settings = null;
        }

		public static List<PersistentSetting> Settings { get; set; }

		public string Get(string name)
		{
			var s = Settings.Where(o => o.SystemName == name).SingleOrDefault();
			if (s == null)
				return "";
			else
				return s.Value;
		}

		public bool GetBool(string name)
		{
			var val = Get(name);
			return val == "1" || val.ToLower() == "true";
		}

		public string this[string name]
		{
			get { return Get(name); }
		}
	}
}
