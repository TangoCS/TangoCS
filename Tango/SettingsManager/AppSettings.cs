﻿using System.Collections.Generic;

namespace Tango.SettingsManager
{
	public class AppSettings : IPersistentSettings
	{
        public static void ResetCache()
        {
			Settings = null;
        }

		public static Dictionary<string, string> Settings { get; set; }

		public string Get(string name, string defaultValue = "")
		{
			string s;
			Settings.TryGetValue(name, out s);
			return s ?? defaultValue;
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