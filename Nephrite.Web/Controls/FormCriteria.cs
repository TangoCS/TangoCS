using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class FormCriteria : System.Web.UI.UserControl
	{
		IFormCriteriaFactory _factory = null;
		Button bOK = new Button();

		public object TableStyle { get; set; }

		public FormCriteria()
		{
			bOK.Click += new EventHandler(bOK_Click);
			bOK.Text = "Найти";
			Controls.Add(bOK);

			TableStyle = new { style = "width:100%" };
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
				_factory.Load();
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			if (_factory != null)
				_factory.Render(writer);
			else
			{
				writer.Write("<p>Необходима инициализация</p>");
				return;
			}
			bOK.RenderControl(writer);

			string url = Url.Current.RemoveParameter(_factory.Parameters.Keys.ToArray()).RemoveParameter("r");
			writer.Write("&nbsp;<input type='submit' value='Очистить' onclick='window.location=\"" + url + "\"; return false;'>");
		}

		void bOK_Click(object sender, EventArgs e)
		{
			var p = _factory.Prepare();
			if (_factory.Messages.Count > 0) return;

			Response.Redirect(Url.Current.RemoveParameter(p.Keys.ToArray()).RemoveParameter("r") + "&" + p.Select(o => o.Key + "=" + o.Value).Join("&") + "&r=1");
		}

		/*public FormCriteriaFactory<T> CreateFactory<T>(IQueryable<T> viewData)
		{
			FormCriteriaFactory<T> f = new FormCriteriaFactory<T>(this, viewData);
			_factory = f;
			return f;
		}*/
		public FormCriteriaFactory<T> CreateFactory<T>()
		{
			FormCriteriaFactory<T> f = new FormCriteriaFactory<T>(this);
			_factory = f;
			return f;
		}
	}

	public interface IFormCriteriaFactory
	{
		Dictionary<string, string> Parameters { get; }
		List<string> Messages { get; set; }

		void Load();
		Dictionary<string, string> Prepare();
		void Render(System.Web.UI.HtmlTextWriter writer);
	}

	public class FormCriteriaFactory<T> : IFormCriteriaFactory
	{
		Dictionary<string, Criteria<T>> _criteries = new Dictionary<string, Criteria<T>>();
		//IQueryable<T> _viewData = null;
		FormCriteria _form = null;
		bool _loaded = false;
		Dictionary<string, string> _parms = new Dictionary<string, string>();

		//public IQueryable<T> ViewData { get { return _viewData; } set { _viewData = value; } }
		public Dictionary<string, string> Parameters { get { return _parms; } }
		internal FormCriteria Form { get { return _form; } }

		public List<string> Messages { get; set; }

		public FormCriteriaFactory(FormCriteria form/*, IQueryable<T> viewData*/)
		{
			_form = form;
		}

		public void Load()
		{
			if (_loaded) return;

			foreach (string parm in Form.Request.QueryString.Keys)
				if (_criteries.Values.Any(o => parm.StartsWith(o.Name)))
					_parms.Add(parm, Form.Request.QueryString[parm]);

			foreach (Criteria<T> c in _criteries.Values)
				c.SetValue(_parms);

			_loaded = true;
		}

		public IQueryable<T> Apply(IQueryable<T> viewData)
		{
			bool b = Query.GetString("r") == "1";
			foreach (Criteria<T> c in _criteries.Values)
				if (b && c.Visible() && c.Predicate != null && c.HasValue())
					viewData = viewData.Where(c.Predicate());
			return viewData;
		}

		public void Render(HtmlTextWriter writer)
		{
			writer.Write(AppWeb.Layout.FormTableBegin(_form.TableStyle));
			foreach (Criteria<T> c in _criteries.Values)
			{
				if (!c.Visible()) continue;

				writer.Write(AppWeb.Layout.FormRowBegin(c.Caption, c.Comment, c.IsRequired));
				c.Render(writer);
				writer.Write(AppWeb.Layout.FormRowEnd());
			}
			writer.Write(AppWeb.Layout.FormTableEnd());
		}

		public Dictionary<string, string> Prepare()
		{
			Dictionary<string, string> parms = new Dictionary<string, string>();
			Messages = new List<string>();

			foreach (Criteria<T> c in _criteries.Values)
			{
				string s = c.Validate();
				if (!s.IsEmpty()) Messages.Add(s);

				var cparms = c.GetValue();
				foreach (string parm in cparms.Keys)
					parms.Add(parm, cparms[parm]);
			}

			return parms;
		}

		public List<string> GetPrintValues()
		{
			return _criteries.Values.Select(o => o.PrintValue()).Where(o => !o.IsEmpty()).ToList();
		}

		public bool HasValue()
		{
			foreach (var c in _criteries.Values)
				if (c.HasValue()) return true;
			return false;
		}

		public CriteriaTextBox<T> AddTextBox(string name, string caption, bool isRequired)
		{
			CriteriaTextBox<T> res = new CriteriaTextBox<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaCalendarPeriod<T> AddCalendarPeriod(string name, string caption, bool isRequired)
		{
			CriteriaCalendarPeriod<T> res = new CriteriaCalendarPeriod<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaDateLists<T> AddDateLists(string name, string caption, bool isRequired)
		{
			CriteriaDateLists<T> res = new CriteriaDateLists<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaDropDownList<T> AddDropDownList(string name, string caption, bool isRequired)
		{
			CriteriaDropDownList<T> res = new CriteriaDropDownList<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaCheckBoxList<T> AddCheckBoxList(string name, string caption, bool isRequired)
		{
			CriteriaCheckBoxList<T> res = new CriteriaCheckBoxList<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaCheckBox<T> AddCheckBox(string name, string caption, bool isRequired)
		{
			CriteriaCheckBox<T> res = new CriteriaCheckBox<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaInterval<T> AddInterval(string name, string caption, bool isRequired)
		{
			CriteriaInterval<T> res = new CriteriaInterval<T>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}

		public CriteriaSingleObject<T, ST> AddSingleObject<ST>(string name, string caption, bool isRequired)
			where ST : IModelObject, new()
		{
			CriteriaSingleObject<T, ST> res = new CriteriaSingleObject<T, ST>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
		public CriteriaMultiObject<T, ST> AddMultiObject<ST>(string name, string caption, bool isRequired)
			where ST : IModelObject, new()
		{
			CriteriaMultiObject<T, ST> res = new CriteriaMultiObject<T, ST>(Form, name, caption, isRequired);
			_criteries.Add(res.Name, res);
			return res;
		}
	}


	public abstract class Criteria<T>
	{
		public string Caption { get; set; }
		public string Comment { get; set; }
		public string Name { get; set; }
		public bool IsRequired { get; set; }
		UserControl _control = null;
		public UserControl control { get { return _control; } }

		public Criteria(UserControl control,  string name, string caption, bool isRequired)
		{
			Caption = caption;
			Name = name;
			IsRequired = isRequired;
			_control = control;
			Visible = () => true;
			Predicate = null;
		}

		public abstract string PrintValue();
		public abstract string Validate();
		public abstract Dictionary<string, string> GetValue();
		public abstract void SetValue(Dictionary<string, string> value);
		public abstract void Render(System.Web.UI.HtmlTextWriter writer);
		public abstract bool HasValue();
		public Func<Expression<Func<T, bool>>> Predicate { get; set; }

		public Func<bool> Visible { get; set; }
	}

	public class CriteriaTextBox<T> : Criteria<T>
	{
		public TextBox TextBox { get; set; }

		internal CriteriaTextBox(UserControl control, string name, string caption, bool isRequired)
			: base(control,name, caption, isRequired)
		{
			TextBox = new TextBox();
			TextBox.Width = new Unit("100%");
			control.Controls.Add(TextBox);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, TextBox.Text);
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name)) TextBox.Text = value[Name];
		}
		public override string PrintValue()
		{
			return HasValue() ? (Caption + ": " + GetValue()[Name]) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return TextBox.Text.IsEmpty() ? Caption : "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			TextBox.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return !TextBox.Text.IsEmpty();
		}
	}

	public class CriteriaCalendarPeriod<T> : Criteria<T>
	{
		JSCalendar _calendarFrom = new JSCalendar();
		JSCalendar _calendarTo = new JSCalendar();

		public JSCalendar CalendarFrom { get { return _calendarFrom; } }
		public JSCalendar CalendarTo { get { return _calendarTo; } }

		internal CriteriaCalendarPeriod(UserControl control, string name, string caption, bool isRequired) 
			: base(control, name, caption, isRequired)
		{
			control.Controls.Add(CalendarFrom);
			control.Controls.Add(CalendarTo);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name + "From", CalendarFrom.Date.HasValue ? CalendarFrom.Date.Value.ToString("dd.MM.yyyy") : "");
			d.Add(Name + "To", CalendarTo.Date.HasValue ? CalendarTo.Date.Value.ToString("dd.MM.yyyy") : "");
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name + "From")) CalendarFrom.Date = value[Name + "From"].ToDate();
			if (value.ContainsKey(Name + "To")) CalendarTo.Date = value[Name + "To"].ToDate();
		}
		public override string PrintValue()
		{
			Dictionary<string, string> d = GetValue();
			return HasValue() ? (Caption + ": с " + d[Name + "From"] + " по " + d[Name + "To"]) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("с&nbsp;");
			CalendarFrom.RenderControl(writer);
			writer.Write("&nbsp;по&nbsp;");
			CalendarTo.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return CalendarFrom.Date.HasValue || CalendarTo.Date.HasValue;
		}
	}

	public class CriteriaDateLists<T> : Criteria<T>
	{
		DateLists _listFrom = new DateLists();
		DateLists _listTo = new DateLists();

		public DateLists ListFrom { get { return _listFrom; } }
		public DateLists ListTo { get { return _listTo; } }

		internal CriteriaDateLists(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			control.Controls.Add(ListFrom);
			control.Controls.Add(ListTo);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name + "From", ListFrom.Date.HasValue ? ListFrom.Date.Value.ToString("dd.MM.yyyy") : "");
			d.Add(Name + "To", ListTo.Date.HasValue ? ListTo.Date.Value.ToString("dd.MM.yyyy") : "");
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name + "From")) ListFrom.Date = value[Name + "From"].ToDate();
			if (value.ContainsKey(Name + "To")) ListTo.Date = value[Name + "To"].ToDate();
		}
		public override string PrintValue()
		{
			Dictionary<string, string> d = GetValue();
			return HasValue() ? (Caption + ": с " + d[Name + "From"] + " по " + d[Name + "To"]) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("с&nbsp;");
			ListFrom.RenderControl(writer);
			writer.Write("&nbsp;по&nbsp;");
			ListTo.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return ListFrom.Date.HasValue || ListTo.Date.HasValue;
		}
	}

	public class CriteriaDropDownList<T> : Criteria<T>
	{
		public DropDownList DropDownList { get; set; }

		internal CriteriaDropDownList(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			DropDownList = new DropDownList();
			DropDownList.Width = new Unit("100%");
			DropDownList.DataTextField = "Title";
			DropDownList.DataValueField = "ObjectID";
			control.Controls.Add(DropDownList);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, DropDownList.SelectedValue);
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name)) DropDownList.SetValue(value[Name]);
		}
		public override string PrintValue()
		{
			return HasValue() ? (Caption + ": " + DropDownList.SelectedItem.Text) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return DropDownList.SelectedValue.IsEmpty() ? Caption : "";
		}
		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			DropDownList.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return !DropDownList.SelectedValue.IsEmpty();
		}
	}

	public class CriteriaCheckBoxList<T> : Criteria<T>
	{
		public CheckBoxList CheckBoxList { get; set; }

		internal CriteriaCheckBoxList(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			CheckBoxList = new CheckBoxList();
			CheckBoxList.DataTextField = "Title";
			CheckBoxList.DataValueField = "ObjectID";
			control.Controls.Add(CheckBoxList);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, CheckBoxList.GetSelectedValues().Join(","));
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name)) CheckBoxList.SelectItems(value[Name].Split(new char[] { ',' }));
		}
		public override string PrintValue()
		{
			return HasValue() ? (Caption + ": " + CheckBoxList.GetSelected().Select(o => o.Text).Join(",")) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return CheckBoxList.GetSelectedValues().Count() == 0 ? Caption : "";
		}
		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			CheckBoxList.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return true;
		}
	}


	public class CriteriaCheckBox<T> : Criteria<T>
	{
		public CheckBox CheckBox { get; set; }

		internal CriteriaCheckBox(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			CheckBox = new CheckBox();
			control.Controls.Add(CheckBox);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, CheckBox.Checked ? "1" : "0");
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name)) CheckBox.Checked = value[Name] == "1";
		}
		public override string PrintValue()
		{
			return HasValue() ? (Caption + ": " + GetValue()[Name]) : "";
		}
		public override string Validate()
		{
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			CheckBox.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return CheckBox.Checked;
		}
	}

	public class CriteriaInterval<T> : Criteria<T>
	{
		TextBox _valueFrom = new TextBox();
		TextBox _valueTo = new TextBox();

		public TextBox ValueFrom { get { return _valueFrom; } }
		public TextBox ValueTo { get { return _valueTo; } }

		internal CriteriaInterval(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			control.Controls.Add(ValueFrom);
			control.Controls.Add(ValueTo);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name + "From", ValueFrom.Text);
			d.Add(Name + "To", ValueTo.Text);
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name + "From")) ValueFrom.Text = value[Name + "From"];
			if (value.ContainsKey(Name + "To")) ValueTo.Text = value[Name + "To"];
		}
		public override string PrintValue()
		{
			Dictionary<string, string> d = GetValue();
			return HasValue() ? (Caption + ": от " + d[Name + "From"] + " до " + d[Name + "To"]) : "";
		}
		public override string Validate()
		{
			if (!IsRequired) return "";
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("от&nbsp;");
			ValueFrom.RenderControl(writer);
			writer.Write("&nbsp;до&nbsp;");
			ValueTo.RenderControl(writer);
		}
		public override bool HasValue()
		{
			return !(ValueFrom.Text.IsEmpty() && ValueTo.Text.IsEmpty());
		}
	}

	public class CriteriaSingleObject<T, ST> : Criteria<T>
		where ST : IModelObject, new()
	{
		public SingleObject SingleObject { get; set; }

		internal CriteriaSingleObject(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			SingleObject = control.Page.LoadControl(Settings.BaseControlsPath + "/SingleObject.ascx") as SingleObject;
			SingleObject.DataTextField = "Title";
			SingleObject.DataValueField = "ObjectID";
			control.Controls.Add(SingleObject);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, SingleObject.ObjectID == 0 ? SingleObject.ObjectGUID.ToString() : SingleObject.ObjectID.ToString());
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name))
			{
				ST obj = new ST();
				if (value[Name].ToInt32(0) > 0)
					obj = SingleObject.AllObjects.Cast<ST>().SingleOrDefault(obj.FindByID<ST>(value[Name].ToInt32(0)));
				if (value[Name].ToGuid() != Guid.Empty)
					obj = SingleObject.AllObjects.Cast<ST>().SingleOrDefault(obj.FindByGUID<ST>(value[Name].ToGuid()));
				SingleObject.SetObject(obj);
			} 
		}
		public override string PrintValue()
		{
			if (!HasValue()) return "";
 
			string value = GetValue()[Name];
			ST obj = new ST();
			if (value.ToInt32(0) > 0)
				obj = SingleObject.AllObjects.Cast<ST>().SingleOrDefault(obj.FindByID<ST>(value.ToInt32(0)));
			if (value.ToGuid() != Guid.Empty)
				obj = SingleObject.AllObjects.Cast<ST>().SingleOrDefault(obj.FindByGUID<ST>(value.ToGuid()));

			return Caption + ": " + obj.Title;
		}
		public override string Validate()
		{
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			SingleObject.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return SingleObject.HasObject;
		}
	}

	public class CriteriaMultiObject<T, ST> : Criteria<T>
	where ST : IModelObject, new()
	{
		public MultiObject MultiObject { get; set; }

		internal CriteriaMultiObject(UserControl control, string name, string caption, bool isRequired)
			: base(control, name, caption, isRequired)
		{
			MultiObject = control.Page.LoadControl(Settings.BaseControlsPath + "/MultiObject.ascx") as MultiObject;
			MultiObject.DataTextField = "Title";
			control.Controls.Add(MultiObject);
		}

		public override Dictionary<string, string> GetValue()
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add(Name, MultiObject.ObjectIDs.Select(o => o.ToString()).Join(","));
			return d;
		}
		public override void SetValue(Dictionary<string, string> value)
		{
			if (value.ContainsKey(Name))
			{
				int[] ids = value[Name].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.ToInt32(0)).ToArray();
				if (ids.Length == 0) return;
				ST empty = new ST();
				IQueryable<ST> objs = MultiObject.AllObjects.Cast<ST>().Where(empty.FindByIDs<ST>(ids));
				MultiObject.LoadObjects<ST>(objs);
			}
		}
		public override string PrintValue()
		{
			if (!HasValue()) return "";

			string value = GetValue()[Name];
			int[] ids = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.ToInt32(0)).ToArray();
			if (ids.Length == 0) return "";
			ST empty = new ST();
			IQueryable<ST> objs = MultiObject.AllObjects.Cast<ST>().Where(empty.FindByIDs<ST>(ids));

			return Caption + ": " + objs.Select(o => o.Title).Join(",");
		}
		public override string Validate()
		{
			return "";
		}

		public override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			MultiObject.RenderControl(writer);
		}

		public override bool HasValue()
		{
			return MultiObject.ObjectIDs.Count() > 0;
		}
	}

}