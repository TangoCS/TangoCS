using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Nephrite.Web.SPM;
using Nephrite.Web.Model;

namespace Nephrite.Web.App
{
	public interface ISettingsManager { }
	public class AppSettings
	{
		static ISettingsManager _settingsManager = null;
		public static ISettingsManager SettingsManager
		{
			get
			{
				if (_settingsManager == null)
				{
					Type t = Type.GetType(Settings.SettingsManager);
					_settingsManager = Activator.CreateInstance(t) as ISettingsManager;
				}

				return _settingsManager;
			}
		}

		static AppSettings _instance;


		public static AppSettings Instance
		{
			get
			{
				if (_instance == null) _instance = new AppSettings();
				
				return _instance;
			}
		}

        public static void ResetCache()
        {
            _settings = null;
        }

		static List<N_Setting> _settings = null;
		public static string Get(string name)
		{
			if (_settings == null) _settings = AppWeb.DataContext.N_Settings.ToList();
			N_Setting s = _settings.Where(o => o.SystemName == name).SingleOrDefault();
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
            N_Setting s = AppWeb.DataContext.N_Settings.Where(o => o.SystemName == name).SingleOrDefault();
            if (s == null)
            {
				AppWeb.DataContext.N_Settings.InsertOnSubmit(new N_Setting
                {
                    SystemName = name,
                    Title = name,
                    Value = value
                });
				AppWeb.DataContext.UserActivities.InsertOnSubmit(new UserActivity
				{
					Action = "Редактирование",
					IP = HttpContext.Current.Request.UserHostAddress,
					LastModifiedDate = DateTime.Now,
					LastModifiedUserID = Subject.Current.ID,
					ObjectKey = name,
					ObjectTypeSysName = "N_Settings",
					ObjectTypeTitle = "Параметр системы",
					Title = name + " = " + value,
					UserTitle = Subject.Current.Title
				});
            }
            else
            {
				if (s.Value != value)
				{
					s.Value = value;

					AppWeb.DataContext.UserActivities.InsertOnSubmit(new UserActivity
					{
						Action = "Редактирование",
						IP = HttpContext.Current.Request.UserHostAddress,
						LastModifiedDate = DateTime.Now,
						LastModifiedUserID = Subject.Current.ID,
						ObjectKey = name,
						ObjectTypeSysName = "N_Settings",
						ObjectTypeTitle = "Параметр системы",
						Title = s.Title + " = " + value,
						UserTitle = Subject.Current.Title
					});
				}
            }
			_settings = null;
			AppWeb.DataContext.SubmitChanges();
        }

	}

	public class WebSettingsManager : ISettingsManager
	{
	
	}
}
