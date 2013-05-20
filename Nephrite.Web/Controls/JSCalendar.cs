using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI.HtmlControls;
using Nephrite.Web.CalendarDays;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
    public class JSCalendar : CompositeControl
    {
        string BasePath = Settings.JSPath + "Calendar";

        TextBox date = new TextBox();
        //ImageButton btn = new ImageButton();
		Image btn = new Image();
        RequiredFieldValidator validator = new RequiredFieldValidator();

        public bool ShowTime
        {
            get { return (string)ViewState["ShowTime"] == "true"; }
            set { ViewState["ShowTime"] = value.ToString().ToLower(); }
        }

		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
				if (btn != null)
					btn.Visible = value;
			}
		}

        public bool Required { get; set; }

        public bool AutoPostBack { get; set; }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            Page.ClientScript.RegisterClientScriptInclude("calendar", BasePath + "/calendar_stripped.js");
            Page.ClientScript.RegisterClientScriptInclude("calendar-ru", BasePath + "/lang/calendar-ru.js");
            Page.ClientScript.RegisterClientScriptInclude("calendar-setup", BasePath + "/calendar-setup_stripped.js");
			if (ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true")
			{
				Page.ClientScript.RegisterClientScriptBlock(GetType(), "calendar-workdays", @"
function jscal_calendarDate_c(arr, date) {
	for(var i = 0; i < arr.length; i++)
		if( arr[i].toString() == date.toString())
			return true;
	return false;
}
function jscal_calendarDate(date, y, m, d){" + CalendarDayCache.ToJSArray("w", "h") + @"
	if (jscal_calendarDate_c(w, new Date(date.getFullYear(), date.getMonth(), d)))
		return 'cal-workingday';
	if (jscal_calendarDate_c(h, new Date(date.getFullYear(), date.getMonth(), d)))
		return 'cal-holiday';
	if (date.getDay() == 0 || date.getDay() == 6)
		return 'cal-holiday';
	else
		return 'cal-workingday';
}", true);
			}
            date.ID = "txtDate";
			date.Attributes.Add("onkeypress", "return jscal_calendarHelper(event)");
			date.Attributes.Add("placeholder", "ДД.ММ.ГГГГ");
			
            Controls.Add(date);
			/* Работает криво
			if (ShowTime)
				MaskedInput.Apply(date, "99.99.9999 99:99");
			else
				MaskedInput.Apply(date, "99.99.9999");*/

			Controls.Add(new LiteralControl(" "));
			
            btn.ID = "btnCalendar";
            btn.ToolTip = "Календарь";
            btn.ImageUrl = BasePath + "/img.gif";
            btn.BorderStyle = BorderStyle.None;

            Controls.Add(btn);
            if (Required)
            {
                Controls.Add(new LiteralControl("&nbsp;"));
                Controls.Add(validator);
                validator.Display = ValidatorDisplay.Dynamic;
                validator.ControlToValidate = date.ID;
                validator.ErrorMessage = "Введите дату";
            }

            if (AutoPostBack)
                date.AutoPostBack = true;

            date.TextChanged += new EventHandler(date_TextChanged);
        }

        void date_TextChanged(object sender, EventArgs e)
        {
            OnChange(EventArgs.Empty);
        }

        /// <summary>
        /// Выбранная дата
        /// </summary>
        public DateTime? Date
        {
            get
            {
                EnsureChildControls();
                DateTime dt;
                if (ShowTime)
                {
					if (DateTime.TryParseExact(date.Text, "d.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dt))
					{
						if (dt.Year < 1900 || dt.Year > 2100)
						{
							date.Text = "";
							return null;
						}
						return dt;
					}
                }
                else
                {
					if (DateTime.TryParseExact(date.Text, "d.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dt))
					{
						if (dt.Year < 1900 || dt.Year > 2100)
						{
							date.Text = "";
							return null;
						}
						return dt;
					}
                }

                return null;
            }
            set
            {
                EnsureChildControls();
                if (!value.HasValue)
                    date.Text = String.Empty;
                else
                {
                    if (ShowTime)
                        date.Text = value.Value.ToString("dd.MM.yyyy HH:mm");
                    else
                        date.Text = value.Value.ToString("dd.MM.yyyy");
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Visible)
            {
                btn.Visible = date.Enabled;
                if (!ShowTime)
                    date.Width = Unit.Pixel(100);
                else
                    date.Width = Unit.Pixel(130);

                string script = @"if (document.getElementById('" + date.ClientID + @"') != null) Calendar.setup(
                    {
                      inputField  : """ + date.ClientID + @""",
                      button      : """ + btn.ClientID + @""",
                      showOthers  : true,
                      weekNumbers : false,";
				if (ConfigurationManager.AppSettings["UseCalendarDaysInJSCalendar"] == "true")
					script += "dateStatusFunc : jscal_calendarDate,";
                if (AutoPostBack)
                {
                    script += @"onSelect : on" + ID + "Select,";
                }
                if (ShowTime)
                {
                    script += @"
                      ifFormat    : ""%d.%m.%Y %H:%M"",
                      showsTime   : true,
                      timeFormat  : ""24""
                    }
                 );";
                }
                else
                {
                    script += @"
                      ifFormat    : ""%d.%m.%Y""
                    }
                 );";
                }
                ScriptManager.RegisterClientScriptBlock(this, GetType(), ID + "onSelect", "function on" + ID + "Select(calendar, date) {document.getElementById('" + date.ClientID + "').value = date; " + Page.ClientScript.GetPostBackEventReference(date, "") + ";}", true);
				if (Enabled)
					ScriptManager.RegisterStartupScript(this, GetType(), ClientID + "-calendar-panel", script, true);
            }
        }

        public event EventHandler Change;
        protected internal virtual void OnChange(EventArgs e)
        {
            if (Change != null)
                Change(this, e);
        }
    }
}
