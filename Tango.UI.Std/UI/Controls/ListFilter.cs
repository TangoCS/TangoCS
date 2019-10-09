using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.Html;
using Newtonsoft.Json;
using Tango.Localization;
using System.Net;
using Tango.UI.Std;
using System.ComponentModel;

namespace Tango.UI.Controls
{
	public class ListFilter : ViewComponent
	{
		protected const string eDialog = "dialog";
		protected const string eTabs = "tabs";
		protected const string tabMain = "tabMain";

		protected const string hValue = "value";
		protected const string ddlField = "ddlField";
		protected const string eConditionContainer = "ddlCondition_fieldbody";
		protected const string ddlCondition = "ddlCondition";
		protected const string eFieldValueContainer = "fieldValue_fieldbody";
		protected const string eFieldValue = "fieldValue";
		protected const string eExpression = "expression";


		[Inject]
		public IPersistentFilter<int> PersistentFilter { get; set; }

		public override bool UsePropertyInjection {
			get {
				return true;
			}
		}

		public string ParameterName { get; set; }
		public List<Field> FieldList { get; private set; } = new List<Field>();

		string ListName => Context.Service + "_" + Context.Action;

		List<FilterItem> _criteria = null;
		public List<FilterItem> Criteria {
			get {
				if (_criteria == null)
					_criteria = GetPostedJson(hValue, () => new List<FilterItem>());
				return _criteria;
			}
			set {
				_criteria = value;
			}
		}

		public IEnumerable<T> GetCriteriaValue<T>(int seqno)
		{
			var f = FieldList.Where(o => o.SeqNo == seqno).FirstOrDefault();
			if (f == null) return default;

			var t = typeof(T);
			t = Nullable.GetUnderlyingType(t) ?? t;

			return Criteria.Where(o => o.Title == f.Title)
				.Select(o => {
					var typeConverter = TypeDescriptor.GetConverter(typeof(T));
					if (o.Value == null || !typeConverter.CanConvertFrom(typeof(string)))
						return default;
					else
					{
						try
						{
							return (T)typeConverter.ConvertFromString(o.Value);
						}
						catch
						{
							return default;
						}
					}
				});
		}

		string SerializedCriteria => WebUtility.HtmlEncode(JsonConvert.SerializeObject(Criteria));

		bool _isPersistentLoaded = false;
		ListFilterEngine _engine = null;

		public event ViewElementEventHandler FilterSubmitted;

		public override void OnInit()
		{
			ParameterName = ClientID + "id";
			_engine = new ListFilterEngine(Resources);
		}

		public void LoadPersistent()
		{
			if (_isPersistentLoaded) return;

			var id = Context.GetIntArg(ParameterName);
			if (PersistentFilter.Load(id, ListName, ""))
			{
				_criteria = PersistentFilter.Criteria;
				_isPersistentLoaded = true;
			}
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
		{
			if (!_isPersistentLoaded) LoadPersistent();
			if (Criteria.Count == 0) return query;
			return _engine.ApplyFilter(query, FieldList, Criteria);
		}

		public (string query, IDictionary<string, object> parms) ApplyFilterSql(string query)
		{
			if (!_isPersistentLoaded) LoadPersistent();
			return _engine.ApplyFilterSql(query, FieldList, Criteria);
		}

		public void OpenFilterDialog(ApiResponse response)
		{
			LoadPersistent();

			response.AddWidget("contenttitle", Resources.Get("System.Filter"));
			response.AddWidget("contentbody", FilterTab);
			response.AddWidget("buttonsbar", w => {
				w.ButtonsBarRight(() => {
					w.Button(a => a.DataResultPostponed(1).OnClickPostEvent(OnSubmit), "OK");
					w.BackButton();
				});
			});
		}

		void FilterTab(LayoutWriter w)
		{
			var fields = FieldList.Select((o, i) => new SelectListItem { Text = o.Title, Value = i.ToString() }).OrderBy(o => o.Text);
			w.Fieldset(() => {
				var title = Resources.Get("System.Filter.Tabs.Filter.Properties");
				var cleartitle = Resources.Get("System.Filter.ClearFilter");

				w.Legend(title);
				w.Div(() => w.A(a => a.OnClickPostEvent(OnClearCriterions), cleartitle));
				w.Div(a => a.ID(eExpression), () => RenderSelectedFields(w));
			});
			w.Fieldset(() => {
				var criteriontitle = Resources.Get("System.Filter.Criterion");
				var addtitle = Resources.Get("System.Filter.AddCriteria");

				w.Legend(criteriontitle);
				w.FormTable100Percent(() => {
					var fieldtitle = Resources.Get("System.Filter.Field");
					var conditiontitle = Resources.Get("System.Filter.Condition");
					var valuetitle = Resources.Get("System.Filter.Value");

					w.FormFieldDropDownList(ddlField, fieldtitle, null, fields.AddEmptyItem(), a => a.OnChangePostEvent(OnFieldChanged));
					w.FormFieldDropDownList(ddlCondition, conditiontitle, "", null, a => a.OnChangePostEvent(OnConditionChanged));
					w.FormField(eFieldValue, valuetitle, null);
				});
				w.Div(a => a.Style("text-align:right"), () => w.Button(a => a.OnClickPostEvent(OnCriterionAdded), addtitle));
			});
			w.Hidden(hValue, SerializedCriteria);
		}

		public void OnFieldChanged(ApiResponse response)
		{
			var f = Context.GetIntArg(ddlField, -1);

			response.WithNamesAndWritersFor(this);
			if (f >= 0)
			{
				var field = FieldList[f];
				var cond = FillConditions(field);

				var op = field.Operators.Values.First();

				response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, cond.FirstOrDefault()?.Value, cond, a => a.OnChangePostEvent(OnConditionChanged)));
				response.AddWidget(eFieldValueContainer, w => op.Renderer(w));
			}
			else
			{
				response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, null, null, a => a.OnChangePostEvent(OnConditionChanged)));
				response.AddWidget(eFieldValueContainer, "");
			}
		}

		public void OnConditionChanged(ApiResponse response)
		{
			var f = Context.GetIntArg(ddlField, -1);
			var cond = Context.GetArg(ddlCondition);

			response.WithNamesAndWritersFor(this);
			if (f >= 0)
			{
				var field = FieldList[f];
				var op = field.Operators[cond];
				response.AddWidget(eFieldValueContainer, w => op.Renderer(w));
			}
			else
				response.AddWidget(eFieldValueContainer, "");
		}

		public void OnCriterionAdded(ApiResponse response)
		{
			var item = GetPostedItem();
			if (item != null) Criteria.Add(item);

			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, w => RenderSelectedFields(w));
			response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, null, null, a => a.OnChangePostEvent(OnConditionChanged)));
			response.AddWidget(eFieldValueContainer, "");
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, SerializedCriteria));
			response.SetElementValue(ddlField, "");
		}

		public void OnCriterionRemoved(ApiResponse response)
		{
			var del = Criteria.Where(o => o.GetHashCode().ToString() == GetArg<string>("removedcriterion")).ToList();
			foreach (var d in del)
				Criteria.Remove(d);

			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, w => RenderSelectedFields(w));
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, SerializedCriteria));
		}

		public void OnClearCriterions(ApiResponse response)
		{
			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, "");
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, ""));
		}

		public void OnSubmit(ApiResponse response)
		{
			Criteria = GetPostedJson<List<FilterItem>>(hValue, () => new List<FilterItem>());

			var item = GetPostedItem();
			if (item != null) Criteria.Add(item);

			//LoadPersistent();
			PersistentFilter.Criteria = Criteria;
			_isPersistentLoaded = true;

			PersistentFilter.SaveCriteria();

			FilterSubmitted?.Invoke(response);
		}

		IEnumerable<SelectListItem> FillConditions(Field field)
		{
			List<SelectListItem> cond = new List<SelectListItem>();
			foreach (string op in field.Operators.Keys)
                cond.Add(new SelectListItem {Selected = op.Contains("содержит") ? true : false, Text = op, Value = op,});
            return cond;
		}

		void RenderSelectedFields(LayoutWriter w)
		{
			foreach (var i in Criteria)
			{
				w.TwoColumnsRowLongFirst(
					() => w.Write(i.Title + " " + i.Condition + " " + i.ValueTitle),
					() => w.A(a => a.OnClickPostEvent(OnCriterionRemoved).DataParm("removedcriterion", i.GetHashCode().ToString()), "<b>x</b>")
				);
			}
		}

		FilterItem GetPostedItem()
		{
			var f = Context.GetIntArg(ddlField, -1);
			var cond = Context.GetArg(ddlCondition);
			if (f < 0) return null;

			var field = FieldList[f];
			var op = field.Operators[cond];

			FilterItem item = new FilterItem
			{
				Title = field.Title,
				Condition = cond,
				FieldType = op.FieldType,
				Value = op.FieldType == FieldType.Boolean ? 
					Context.GetBoolArg(eFieldValue + field.SeqNo).ToString() : 
					Context.GetArg(eFieldValue + field.SeqNo),
			};

			item.ValueTitle = op.StringValue(item);
			return item;
		}

		//views
		public void OpenViewSettingsDialog(ApiResponse response)
		{
			LoadPersistent();
			ViewDialog(response, new ViewFormData {
				Title = PersistentFilter.Name,
				IsShared = PersistentFilter.IsShared ? "2" : "1",
				IsDefault = PersistentFilter.IsDefault
			}, OnViewSumbit);
		}

		public void OpenNewViewDialog(ApiResponse response)
		{
			LoadPersistent();
			ViewDialog(response, new ViewFormData { IsShared = "1" }, OnNewViewSumbit);
		}

		void ViewDialog(ApiResponse response, ViewFormData formData, Action<ApiResponse> submitEvent)
		{
			response.AddWidget("contenttitle", Resources.Get("System.Filter.ViewSettings"));
			response.AddWidget("contentbody", w => {
				w.FormTable100Percent(() => {
					w.FormFieldTextBox("title", Resources.Get("Common.Title"), formData.Title);
					w.FormField("visibility", Resources.Get("System.Filter.PropertiesOfVisibility"), () => {
						w.RadioButtonList("isshared", formData.IsShared, new List<SelectListItem> {
							new SelectListItem(Resources.Get("System.Filter.Tabs.Properties.Personal"), 1),
							new SelectListItem(Resources.Get("System.Filter.Tabs.Properties.Shared"), 2)
						});
					});
					w.FormField("", "", () => {
						w.CheckBox("isdefault", formData.IsDefault);
						w.Label("isdefault", Resources.Get("System.Filter.Tabs.Properties.DefaultView"));
					});					
				});
				FilterTab(w);
			});
			response.AddWidget("buttonsbar", w => {
				w.ButtonsBarRight(() => {
					w.Button(a => a.DataResult(1).OnClickPostEvent(submitEvent), w.Resources.Get("Common.Save"));
					w.BackButton();
				});
			});
		}

		public void OnNewViewSumbit(ApiResponse response)
		{
			LoadPersistent();
			PersistentFilter.InsertOnSubmit();
			OnViewSumbit(response);
		}

		public void OnViewSumbit(ApiResponse response)
		{
			LoadPersistent();

			Criteria = GetPostedJson(hValue, () => new List<FilterItem>());

			var item = GetPostedItem();
			if (item != null) Criteria.Add(item);

			PersistentFilter.Criteria = Criteria;

			PersistentFilter.SaveView(
				Context.GetArg("title"),
				Context.GetBoolArg("isshared", false),
				Context.GetBoolArg("isdefault", false),
				ListName, 
				null
			);
			FilterSubmitted?.Invoke(response);
		}

		public void GetViewsMenu(ApiResponse response)
		{
			response.AddWidget(Context.Sender, w => {
				var views = PersistentFilter.GetViews(ListName, Context.AllArgs);
				LoadPersistent();

				w.ActionLink(a => a.ToCurrent().WithArg(ParameterName, 0).WithTitle(r => r.Get("Common.AllItems")));

				foreach (var view in views)
				{
					void link() => w.ActionLink(a => a.ToCurrent().WithArg(ParameterName, view.ID).WithTitle(view.Name));
					if (view.IsDefault)
						w.B(link);
					else
						link();
				}

				if (!PersistentFilter.Name.IsEmpty() || Criteria.Count > 0)
					w.PopupMenuSeparator();

				if (!PersistentFilter.Name.IsEmpty())
				{
					w.ActionImageLink(a => a.CallbackToCurrent().AsDialog(OpenViewSettingsDialog).WithImage("viewsettings").WithTitle(r => r.Get("System.Filter.EditView")));
				}
				if (Criteria.Count > 0)
				{
					w.ActionImageLink(a => a.CallbackToCurrent().AsDialog(OpenNewViewDialog).WithImage("newview").WithTitle(r => r.Get("System.Filter.CreateView")));
				}
			});
		}

		public void RenderHeaderFilter(ApiResponse response)
		{
			response.AddAdjacentWidget("form", "formbody", AdjacentHTMLPosition.AfterBegin, w => {
				var f = Context.GetIntArg("conditionseqno");
				var field = FieldList.FirstOrDefault(o => o.SeqNo == f);
				var cond = FillConditions(field);

				var op = field.Operators.Values.First();

				w.DropDownList(ddlCondition, cond.FirstOrDefault()?.Value, cond);
				op.Renderer(w);
			});
			response.AddWidget("buttonsbar", w => {
				w.ButtonsBarRight(() => {
					w.Button(a => a.DataResult(1), w.Resources.Get("Common.Save"));
					w.ResetButton();
				});
			});
		}

		class ViewFormData
		{
			public string Title { get; set; }
			public string IsShared { get; set; }
			public bool IsDefault { get; set; }
		}
	}

	public class ListFilter<T> : ListFilter
	{
		public static class Renderers
		{
			public static Action<LayoutWriter> TextBox(int seqNo) => w => w.TextBox(eFieldValue + seqNo);
			public static Action<LayoutWriter> Calendar(int seqNo) => w => w.Calendar(eFieldValue + seqNo);
			public static Action<LayoutWriter> CalendarWithTime(int seqNo) => w => w.Calendar(eFieldValue + seqNo, showTime: true);
			public static Action<LayoutWriter> CheckBox(int seqNo) => w => w.CheckBox(eFieldValue + seqNo);
		}
		public static class StringValues
		{
			public static string String(FilterItem item) => item.Value.IsEmpty() ? "\"\"" : item.Value;
			public static string Numeric(FilterItem item) => item.Value;
			public static string Date(FilterItem item) => item.Condition == "d" ? item.Value : !item.Value.IsEmpty() ? item.Value : "нет";
            public static string Guid(FilterItem item) => item.Value;
        }

        string StringValueBoolean(FilterItem item) => item.Value == "True" ?
				Resources.Get("System.Filter.Yes") :
				Resources.Get("System.Filter.No");

		#region добавление полей
		Field CreateOrGetCondition(string title)
		{
			var f = FieldList.FirstOrDefault(o => o.Title == title);
			if (f == null)
			{
				f = new Field { SeqNo = FieldList.Count + 1, Title = title };
				FieldList.Add(f);
			}
			return f;
		}

		void AddStdOps(Field f, FieldCriterion data)
		{
			f.Operators.AddIfNotExists(">", data);
			f.Operators.AddIfNotExists(">=", data);
			f.Operators.AddIfNotExists("<", data);
			f.Operators.AddIfNotExists("<=", data);
			f.Operators.AddIfNotExists("<>", data);
			f.Operators.AddIfNotExists("=", data);
		}

		FieldCriterion FieldCriterionDDL(int seqNo, object column, IEnumerable<SelectListItem> values) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.String,
				Renderer = w => w.DropDownList(eFieldValue + seqNo, null, values),
				StringValue = item => values.Where(o => o.Value == item.Value).Select(o => o.Text).FirstOrDefault()
			};

		FieldCriterion FieldCriterionString(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.String,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.String
			};

		FieldCriterion FieldCriterionDate(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Date,
				Renderer = Renderers.Calendar(seqNo),
				StringValue = StringValues.Date
			};

		FieldCriterion FieldCriterionDateTime(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.DateTime,
				Renderer = Renderers.CalendarWithTime(seqNo),
				StringValue = StringValues.Date
			};

		FieldCriterion FieldCriterionInt(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Int,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.Numeric
			};

		FieldCriterion FieldCriterionDecimal(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Decimal,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.Numeric
			};

		FieldCriterion FieldCriterionBoolean(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Boolean,
				Renderer = Renderers.CheckBox(seqNo),
				StringValue = StringValueBoolean
			};

        FieldCriterion FieldCriterionGuid(int seqNo, object column) =>
            new FieldCriterion
            {
                Column = column,
                FieldType = FieldType.Guid,
                Renderer = Renderers.TextBox(seqNo),
                StringValue = StringValues.Guid
            };

        public int AddConditionString(string title, string opname, Expression<Func<T, string, bool>> column)
		{
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionString(f.SeqNo, column);
			f.Operators[opname] = data;
			return f.SeqNo;
		}

		public int AddConditionDDL(string title, Expression<Func<T, object>> column, IEnumerable<SelectListItem> values)
		{
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionDDL(f.SeqNo, column, values);
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}

		public int AddConditionDDL(string title, string opname, Expression<Func<T, object, bool>> column, 
			IEnumerable<SelectListItem> values)
		{
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionDDL(f.SeqNo, column, values);
			f.Operators[opname] = data;
			return f.SeqNo;
		}

		public int AddConditionDDL(Expression<Func<T, object>> column, IEnumerable<SelectListItem> values)
		{
			var title = Resources.Get(column.GetResourceKey());
			return AddConditionDDL(title, column, values);
		}

		public int AddConditionSelectSingleObject<TRefClass, TRefKey>(string title, Expression<Func<T, object>> column, SelectSingleObjectField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var f = CreateOrGetCondition(title);
			dialog.ID = eFieldValue + f.SeqNo;
			var data = new FieldCriterion {
				Column = column,
				FieldType = FieldType.Int,
				Renderer = w => dialog.Strategy.Render(w, null),
				StringValue = item => item.Value.IsEmpty() ? "" : dialog.GetObjectByID(item.Value.ConvertTo<TRefKey>())?.Title
			};
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}

		public int AddConditionSelectSingleObject<TRefClass, TRefKey>(Expression<Func<T, object>> column, SelectSingleObjectField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var title = Resources.Get(column.GetResourceKey());
			return AddConditionSelectSingleObject(title, column, dialog);
		}

		public int AddCondition<TVal>(string title, Expression<Func<T, TVal>> column)
		{
			var f = CreateOrGetCondition(title);

			LambdaExpression expr = column;
			if (expr.Body is UnaryExpression)
				expr = Expression.Lambda((expr.Body as UnaryExpression).Operand, expr.Parameters);
			var t = expr.Body.Type;

			if (t == typeof(string))
			{
				var data = FieldCriterionString(f.SeqNo, column);
				f.Operators.AddIfNotExists("=", data);
				f.Operators.AddIfNotExists("<>", data);
				f.Operators.AddIfNotExists(Resources.Get("System.Filter.StartsWith"), data);
				f.Operators.AddIfNotExists(Resources.Get("System.Filter.Contains"), data);
			}
			else if (t.In(typeof(DateTime), typeof(DateTime?)))
				AddStdOps(f, FieldCriterionDate(f.SeqNo, expr));
			else if (t.In(typeof(int), typeof(int?)))
				AddStdOps(f, FieldCriterionInt(f.SeqNo, expr));
			else if (t.In(typeof(decimal), typeof(decimal?), typeof(long), typeof(long?)))
				AddStdOps(f, FieldCriterionDecimal(f.SeqNo, expr));
			else if (t.In(typeof(bool), typeof(bool?)))
				f.Operators.AddIfNotExists("=", FieldCriterionBoolean(f.SeqNo, expr));
			else if (t == typeof(Guid) || t == typeof(Guid?))
            {
                var data = FieldCriterionGuid(f.SeqNo, column);
                f.Operators.AddIfNotExists("=", data);
                f.Operators.AddIfNotExists("<>", data);
            }
            else
            throw new Exception($"Field type {t.Name} not supported");
			
			return f.SeqNo;
		}

		public int AddCondition<TVal>(Expression<Func<T, TVal>> column)
		{
			return AddCondition(Resources.Get(column.GetResourceKey()), column);
		}

		public int AddConditionDateWithTime(Expression<Func<T, DateTime>> column)
		{
			var title = Resources.Get(column.GetResourceKey());
			var f = CreateOrGetCondition(title);
			AddStdOps(f, FieldCriterionDateTime(f.SeqNo, column));
			return f.SeqNo;
		}
		public int AddConditionDateWithTime(Expression<Func<T, DateTime?>> column)
		{
			var title = Resources.Get(column.GetResourceKey());
			var f = CreateOrGetCondition(title);
			AddStdOps(f, FieldCriterionDateTime(f.SeqNo, column));
			return f.SeqNo;
		}
		public int AddConditionSql<TVal>(string title, string column, List<string> operators = null)
		{
			var ops = operators ?? new List<string> { "=" };
			var f = CreateOrGetCondition(title);

			var col = (column, typeof(TVal));
			var t = typeof(TVal);

			FieldCriterion data = null;
			if (t == typeof(string))
				data = FieldCriterionString(f.SeqNo, col);
			else if (t.In(typeof(DateTime), typeof(DateTime?)))
				data = FieldCriterionDate(f.SeqNo, col);
			else if (t.In(typeof(int), typeof(int?)))
				data = FieldCriterionInt(f.SeqNo, col);
			else if (t.In(typeof(decimal), typeof(decimal?), typeof(long), typeof(long?)))
				data = FieldCriterionDecimal(f.SeqNo, col);
			else if (t.In(typeof(bool), typeof(bool?)))
				data = FieldCriterionDate(f.SeqNo, col);
			else
				throw new Exception($"Field type {t.Name} not supported");

			foreach (var op in ops)
				f.Operators.Add(op, data);

			return f.SeqNo;
		}
		public int AddConditionSqlDDL<TVal>(string title, string column, IEnumerable<SelectListItem> values)
		{
			var f = CreateOrGetCondition(title);
			var col = (column, typeof(TVal));
			var data = FieldCriterionDDL(f.SeqNo, col, values);
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}
		public int AddConditionSqlSelectSingleObject<TRefClass, TRefKey>(string title, string column, SelectSingleObjectField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var f = CreateOrGetCondition(title);
			dialog.ID = eFieldValue + f.SeqNo;

			var col = (column, typeof(TRefKey));
			var ftype = typeof(TRefKey) == typeof(string) ? FieldType.String :
				typeof(TRefKey) == typeof(int) ? FieldType.Int :
				throw new Exception(typeof(TRefKey).Name + " key not supported");

			var data = new FieldCriterion {
				Column = col,
				FieldType = ftype,
				Renderer = w => dialog.Strategy.Render(w, null),
				StringValue = item => dialog.GetObjectByID(item.Value.ConvertTo<TRefKey>()).Title
			};
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}
		#endregion
	}

	
}
