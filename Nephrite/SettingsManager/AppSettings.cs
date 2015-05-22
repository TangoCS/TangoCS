using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nephrite.SettingsManager
{
	public class AppSettings : IPersistentSettings
	{
		IDC_Settings _dc;
		public AppSettings(IDC_Settings dataContext)
		{
			_dc = dataContext;
		}

        public static void ResetCache()
        {
            _settings = null;
        }

		static List<IN_Settings> _settings = null;

		public string Get(string name)
		{
			if (_settings == null) _settings = _dc.IN_Settings.ToList();
			IN_Settings s = _settings.Where(o => o.SystemName == name).SingleOrDefault();
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

		public void SetBool(string name, bool value)
		{
			Set(name, value.ToString());
		}

        public void Set(string name, string value)
        {
			IN_Settings s = _dc.IN_Settings.Where(o => o.SystemName == name).SingleOrDefault();
            if (s == null)
            {
				s = _dc.NewIN_Settings();
				s.SystemName = name;
                s.Title = name;
				s.Value = value;
				_dc.IN_Settings.InsertOnSubmit(s);
            }
            else
            {
				if (s.Value != value)
				{
					s.Value = value;
				}
            }
			_settings = null;
			_dc.SubmitChanges();
        }


		public string this[string name]
		{
			get { return Get(name); }
		}
	}
}
