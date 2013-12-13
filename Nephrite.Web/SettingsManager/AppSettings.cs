using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Nephrite.Web.SPM;
using Nephrite.Web.UserActivity;
using System.Data.Linq;
using Nephrite.Meta;

namespace Nephrite.Web.SettingsManager
{
	public class AppSettings
	{
		static AppSettings _instance;

		static IDC_Settings _dc
		{
			get
			{
				return ((IDC_Settings)A.Model);
			}
		}
		static IDC_UserActivity _dcUserActivity
		{
			get
			{
				return ((IDC_UserActivity)A.Model);
			}
		}


		public static AppSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AppSettings();
				}
				
				return _instance;
			}
		}

        public static void ResetCache()
        {
            _settings = null;
        }

		static List<IN_Setting> _settings = null;
		public static string Get(string name)
		{
			if (_settings == null) _settings = _dc.N_Setting.ToList();
			IN_Setting s = _settings.Where(o => o.SystemName == name).SingleOrDefault();
			if (s == null)
				return "";
			else
				return s.Value;
		}

		public static bool GetBool(string name)
		{
			var val = Get(name);
			return val == "1" || val.ToLower() == "true";
		}

		public static void SetBool(string name, bool value)
		{
			Set(name, value.ToString());
		}

        public static void Set(string name, string value)
        {
			IN_Setting s = _dc.N_Setting.Where(o => o.SystemName == name).SingleOrDefault();
            if (s == null)
            {
				s = _dc.NewN_Setting();
				s.SystemName = name;
                s.Title = name;
				s.Value = value;
				_dc.N_Setting.InsertOnSubmit(s);

				var ua = _dcUserActivity.NewUserActivity(A.Meta.GetOperation("N_Settings", "Edit"), name, name + " = " + value);
				_dcUserActivity.UserActivity.InsertOnSubmit(ua);
            }
            else
            {
				if (s.Value != value)
				{
					s.Value = value;
					var ua = _dcUserActivity.NewUserActivity(A.Meta.GetOperation("N_Settings", "Edit"), name, name + " = " + value);
					_dcUserActivity.UserActivity.InsertOnSubmit(ua);
				}
            }
			_settings = null;
			_dc.SubmitChanges();
        }

	}
}
