using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		protected const string eFieldLabelContainer = "fieldValue_fieldlabel";
		protected const string eFieldValueContainer = "fieldValue_fieldbody";
		protected const string eFieldDescriptionContainer = "fieldValue_fielddescription";
		protected const string eFieldValue = "fieldValue";
		protected const string eExpression = "expression";
		protected const string eValidation = "validation";


		[Inject]
		public IPersistentFilter<int> PersistentFilter { get; set; }

		//public override bool UsePropertyInjection {
		//	get {
		//		return true;
		//	}
		//}

		public string ParameterName { get; set; }
		public List<Field> FieldList { get; private set; } = new List<Field>();
		public Action FieldsInit { get; set; }

		public Func<bool> AllowDefaultFilters { get; set; }

		string ListName
		{
			get
			{
				var t = ParentElement.GetType();
				var attrs = t.GetCustomAttributes<OnActionAttribute>();
				if (attrs != null && attrs.Count() == 1)
				{
					var a = attrs.First();
					return a.Service + "_" + a.Action;
				}
				else
					return t.Name;
			}
		}

		List<FilterItem> _criteria = null;
		public List<FilterItem> Criteria {
			get {
				if (_criteria == null)
					_criteria = Context.GetJsonArg(hValue, () => new List<FilterItem>());
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

		public override void OnEvent()
		{
			FieldsInit();
		}

		public void LoadPersistent()
		{
			if (_isPersistentLoaded) return;

			var id = Context.GetIntArg(ParameterName);
			
			var loaded = PersistentFilter.Load(id);
			if (!loaded && id == null && (AllowDefaultFilters?.Invoke() ?? true)) 
				loaded = PersistentFilter.LoadDefault(ListName, "");

			if (loaded)
			{
				_criteria = PersistentFilter.Criteria.ToList();
				_isPersistentLoaded = true;
			}
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
		{
			LoadPersistent();
			if (Criteria.Count == 0) return query;
			return _engine.ApplyFilter(query, FieldList, Criteria.Where(x => x.FieldType != FieldType.Sql).ToList());
		}

		public (List<string> filters, IDictionary<string, object> parms) GetSqlFilters()
		{
			LoadPersistent();
			return _engine.GetSqlFilters(FieldList, Criteria.Where(x => x.FieldType == FieldType.Sql).ToList());
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
				w.FieldsBlock100Percent(() => {
					var fieldtitle = Resources.Get("System.Filter.Field");
					var conditiontitle = Resources.Get("System.Filter.Condition");
					var valuetitle = Resources.Get("System.Filter.Value");

					w.FormFieldDropDownList(ddlField, fieldtitle, null, fields.AddEmptyItem(), a => a.OnChangePostEvent(OnFieldChanged));
					w.FormFieldDropDownList(ddlCondition, conditiontitle, "", null, a => a.OnChangePostEvent(OnConditionChanged));
					w.FormField(eFieldValue, valuetitle, null);
				});
				w.Div(a => a.Style("text-align:right"), () => w.Button(a => a.OnClickPostEvent(OnCriterionAdded), addtitle));
				w.Div(a => a.ID(eValidation), "");
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
				var showHint = Resources.TryGet($"{ListName}.{field.Title}", "hint", out var hint);

				response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, cond.FirstOrDefault()?.Value, cond, a => a.OnChangePostEvent(OnConditionChanged)));
				response.AddWidget(eFieldValueContainer, w => op.Renderer(w));
				if (showHint)
					response.AddAdjacentWidget(eFieldLabelContainer, eFieldDescriptionContainer, AdjacentHTMLPosition.BeforeEnd,
						w => w.FormFieldDescription(eFieldValue, () => w.Write(hint)));
				else
					response.RemoveWidget(eFieldDescriptionContainer);
			}
			else
			{
				response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, null, null, a => a.OnChangePostEvent(OnConditionChanged)));
				response.AddWidget(eFieldValueContainer, "");
				response.RemoveWidget(eFieldDescriptionContainer);
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
			{
				response.AddWidget(eFieldValueContainer, "");
				response.RemoveWidget(eFieldDescriptionContainer);
			}
		}

		public void OnCriterionAdded(ApiResponse response)
		{
			var (item, success) = ProcessSubmit(response);
			if (!success) return;

			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, w => RenderSelectedFields(w));
			response.AddWidget(eConditionContainer, w => w.DropDownList(ddlCondition, null, null, a => a.OnChangePostEvent(OnConditionChanged)));
			response.AddWidget(eFieldValueContainer, "");
			response.RemoveWidget(eFieldDescriptionContainer);
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, SerializedCriteria));
			response.SetElementValue(ddlField, "");

			var f = Context.GetIntArg(ddlField, -1);
			if (f >= 0)
			{
				var cond = Context.GetArg(ddlCondition);
				var field = FieldList[f];
				var op = field.Operators[cond];
				op.OnSelected?.Invoke(response);
			}
		}

		public void OnCriterionRemoved(ApiResponse response)
		{
			var del = Criteria.Where(o => o.GetHashCode().ToString() == Context.GetArg("removedcriterion")).ToList();
			foreach (var d in del)
				Criteria.Remove(d);

			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, w => RenderSelectedFields(w));
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, SerializedCriteria));
		}

		public void OnInlineCriterionRemoved(ApiResponse response)
		{
			LoadPersistent();
			var del = Criteria.Where(o => o.GetHashCode().ToString() == Context.GetArg("removedcriterion")).ToList();
			foreach (var d in del)
				Criteria.Remove(d);

			
			PersistentFilter.Criteria = Criteria;
			_isPersistentLoaded = true;

			PersistentFilter.InsertOnSubmit();
			PersistentFilter.SaveCriteria();

			FilterSubmitted?.Invoke(response);
		}

		public void OnClearCriterions(ApiResponse response)
		{
			response.WithNamesAndWritersFor(this);
			response.AddWidget(eExpression, "");
			response.AddChildWidget("content", hValue, w => w.Hidden(hValue, ""));
		}

		public void OnSubmit(ApiResponse response)
		{
			Criteria = Context.GetJsonArg(hValue, () => new List<FilterItem>());

			var (item, success) = ProcessSubmit(response);
			if (!success) return;

			//LoadPersistent();
			PersistentFilter.Criteria = Criteria;
			_isPersistentLoaded = true;

			PersistentFilter.SaveCriteria();

			var f = Context.GetIntArg(ddlField, -1);
			if (f >= 0)
			{
				var cond = Context.GetArg(ddlCondition);
				var field = FieldList[f];
				var op = field.Operators[cond];
				op.OnSelected?.Invoke(response);
			}

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

		(FilterItem item, bool validationSuccess) ProcessSubmit(ApiResponse response)
		{
			var v = new ValidationMessageCollection();
			FilterItem item = null;

			var f = Context.GetIntArg(ddlField, -1);

			if (f >= 0)
			{
				var cond = Context.GetArg(ddlCondition);
				var field = FieldList[f];
				var op = field.Operators[cond];

				item = new FilterItem
				{
					Title = field.Title,
					Condition = cond,
					FieldType = op.FieldType,
					Value = op.FieldType == FieldType.Boolean ?
						Context.GetBoolArg(op.FieldName).ToString() :
						Context.GetArg(op.FieldName),
				};

				item.ValueTitle = op.StringValue(item);

				Criteria.Add(item);

				ValidateItem(field, item, v);
			}

			var duplicates = from c in Criteria
							 where c.FieldType == FieldType.Sql
							 group c by c.Title into grp
							 where grp.Count() > 1
							 select grp.Key;

			foreach (var d in duplicates)
				v.Add("entitycheck", eFieldValue, $"Множественные критерии \"{d}\" не поддерживаются");

			if (v.Count > 0)
			{
				response.AddWidget(eValidation, w => w.ValidationBlock(v));
				response.Success = false;
			}
			else
			{
				response.AddWidget(eValidation, w => w.Write("")); 
			}

			return (item, v.Count == 0);
		}

		void ValidateItem(Field f, FilterItem item, ValidationMessageCollection v)
		{
			var column = _engine.ColumnExpression(f, item);
			if (column == null)
				return;

			var valType = _engine.ColumnType(column);
			var val = _engine.ConvertValue(valType, item);

			if (val == null && (Nullable.GetUnderlyingType(valType) == null || !item.Condition.In("<>", "=")))
				v.Add("entitycheck", eFieldValue, "Значение не может быть пустым");
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
				w.FieldsBlock100Percent(() => {
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

			Criteria = Context.GetJsonArg(hValue, () => new List<FilterItem>());

			var (item, success) = ProcessSubmit(response);
			if (!success) return;

			PersistentFilter.Criteria = Criteria;

			PersistentFilter.SaveView(
				Context.GetArg("title"),
				Context.GetIntArg("isshared") == 2,
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

				w.ActionLink(a => a.ToCurrent().WithArg(ParameterName, 0).WithTitle(r => r.Get("Common.AllItems")), 
					a => a.Data(DataCollection).DataContainerExternal(ParentElement.ClientID).DataEvent("onsetview", ParentElement.ClientID));

				foreach (var view in views)
				{
					void link() => w.ActionLink(a => a.ToCurrent().WithArg(ParameterName, view.ID).WithTitle(view.Name), 
						a => a.Data(DataCollection).DataContainerExternal(ParentElement.ClientID).DataEvent("onsetview", ParentElement.ClientID));
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
				FieldName = eFieldValue + seqNo,
				Renderer = w => w.DropDownList(eFieldValue + seqNo, null, values),
				StringValue = item => values.Where(o => o.Value == item.Value).Select(o => o.Text).FirstOrDefault()
			};

		FieldCriterion FieldCriterionString(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.String,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.String
			};

		FieldCriterion FieldCriterionDate(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Date,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.Calendar(seqNo),
				StringValue = StringValues.Date
			};

		FieldCriterion FieldCriterionDateTime(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.DateTime,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.CalendarWithTime(seqNo),
				StringValue = StringValues.Date
			};

		FieldCriterion FieldCriterionInt(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Int,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.Numeric
			};

		FieldCriterion FieldCriterionDecimal(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Decimal,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.TextBox(seqNo),
				StringValue = StringValues.Numeric
			};

		FieldCriterion FieldCriterionBoolean(int seqNo, object column) =>
			new FieldCriterion {
				Column = column,
				FieldType = FieldType.Boolean,
				FieldName = eFieldValue + seqNo,
				Renderer = Renderers.CheckBox(seqNo),
				StringValue = StringValueBoolean
			};

        FieldCriterion FieldCriterionGuid(int seqNo, object column) =>
            new FieldCriterion
            {
                Column = column,
                FieldType = FieldType.Guid,
				FieldName = eFieldValue + seqNo,
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

		public int AddConditionDDL<TVal>(string title, Expression<Func<T, TVal>> column, IEnumerable<SelectListItem> values)
		{
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionDDL(f.SeqNo, column, values);
			if (column.Body.Type == typeof(Guid))
				data.FieldType = FieldType.Guid;
			else
				data.FieldType = FieldType.String;
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}

		public int AddConditionDDL<TVal>(string title, string opname, Expression<Func<T, TVal, bool>> column, 
			IEnumerable<SelectListItem> values)
		{
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionDDL(f.SeqNo, column, values);
			if (column.Body.Type == typeof(Guid))
				data.FieldType = FieldType.Guid;
			else
				data.FieldType = FieldType.String;
			f.Operators[opname] = data;
			return f.SeqNo;
		}

		public int AddConditionDDL<TVal>(Expression<Func<T, TVal>> column, IEnumerable<SelectListItem> values)
		{
			var title = Resources.Get(column.GetResourceKey());
			return AddConditionDDL(title, column, values);
		}

		public int AddConditionSelectSingleObject<TRefClass, TRefKey>(string title, Expression<Func<T, object>> column, SelectSingleObjectField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var f = CreateOrGetCondition(title);
			var data = new FieldCriterion {
				Column = column,
				FieldType = FieldType.Int,
				FieldName = dialog.ID,
				Renderer = w => dialog.Strategy.Render(w, null),
				StringValue = item => item.Value.IsEmpty() ? "" : dialog.GetObjectByID(item.Value.ConvertTo<TRefKey>())?.Title,
				OnSelected = response => response.SetCtrlInstance(dialog.Strategy.ClientID, new { selectedvalues = "" })
			};
			f.Operators["="] = data;
			f.Operators["<>"] = data;
			return f.SeqNo;
		}

		int AddConditionSelectMultipleObjectsInt<TRefClass, TRefKey>(string title, object column, SelectMultipleObjectsField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var f = CreateOrGetCondition(title);
			var t = typeof(TRefKey);
			FieldType fieldType = default;
			if (t == typeof(int) || t == typeof(int?))
				fieldType = FieldType.IntArray;
			else if (t == typeof(Guid) || t == typeof(Guid?))
				fieldType = FieldType.GuidArray;
			else
				throw new Exception($"SelectMultipleObjects condition: {t.Name} key is not supported");

			var data = new FieldCriterion
			{
				Column = column,
				FieldType = fieldType,
				FieldName = dialog.ID,
				Renderer = w => dialog.Strategy.Render(w, null),
				StringValue = item => {
					if (item.Value.IsEmpty()) return "";
					var ids = item.Value.Split(new char[] { ',' }).Select(x => x.ConvertTo<TRefKey>());
					return dialog.GetObjectsByIDs(ids).Select(x => x.Title).Join(", ");
				},
				OnSelected = response => response.SetCtrlInstance(dialog.Strategy.ClientID, new { selectedvalues = "" })
			};
			f.Operators["="] = data;
			return f.SeqNo;
		}

		public int AddConditionSelectMultipleObjects<TRefClass, TRefKey>(string title, Expression<Func<T, int[], bool>> column, SelectMultipleObjectsField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			return AddConditionSelectMultipleObjectsInt(title, column, dialog);
		}

		public int AddConditionSelectMultipleObjects<TRefClass, TRefKey>(string title, Expression<Func<T, object>> column, SelectMultipleObjectsField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			return AddConditionSelectMultipleObjectsInt(title, column, dialog);
		}

		public int AddConditionSelectMultipleObjects<TRefClass, TRefKey>(Expression<Func<T, object>> column, SelectMultipleObjectsField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var title = Resources.Get(column.GetResourceKey());
			return AddConditionSelectMultipleObjects(title, column, dialog);
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
			{
				var data = FieldCriterionDate(f.SeqNo, expr);
				AddStdOps(f, data);

				var data2 = FieldCriterionDateTime(f.SeqNo, column);
				data2.Renderer = Renderers.TextBox(f.SeqNo);
				f.Operators.AddIfNotExists(Resources.Get("System.Filter.LastXDays"), data2);
			}
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
			var data = FieldCriterionDateTime(f.SeqNo, column);
			AddStdOps(f, data);
			var data2 = FieldCriterionDateTime(f.SeqNo, column);
			data2.Renderer = Renderers.TextBox(f.SeqNo);
			f.Operators.AddIfNotExists(Resources.Get("System.Filter.LastXDays"), data2);
			return f.SeqNo;
		}
		public int AddConditionDateWithTime(Expression<Func<T, DateTime?>> column)
		{
			var title = Resources.Get(column.GetResourceKey());
			var f = CreateOrGetCondition(title);
			var data = FieldCriterionDateTime(f.SeqNo, column);
			AddStdOps(f, data);
			var data2 = FieldCriterionDateTime(f.SeqNo, column);
			data2.Renderer = Renderers.TextBox(f.SeqNo);
			f.Operators.AddIfNotExists(Resources.Get("System.Filter.LastXDays"), data2);
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
			data.FieldType = FieldType.Sql;

			foreach (var op in ops)
				f.Operators.Add(op, data);

			return f.SeqNo;
		}
		public int AddConditionSqlDDL<TVal>(string title, string column, IEnumerable<SelectListItem> values, List<string> operators = null)
		{
			var ops = operators ?? new List<string> { "=" };
			var f = CreateOrGetCondition(title);
			var col = (column, typeof(TVal));
			var data = FieldCriterionDDL(f.SeqNo, col, values);
			data.FieldType = FieldType.Sql;
			foreach (var op in ops)
				f.Operators.Add(op, data);
			return f.SeqNo;
		}
		public int AddConditionSqlSelectSingleObject<TRefClass, TRefKey>(string title, string column, SelectSingleObjectField<TRefClass, TRefKey> dialog)
			where TRefClass : class, IWithTitle, IWithKey<TRefClass, TRefKey>, new()
		{
			var f = CreateOrGetCondition(title);

			var col = (column, typeof(TRefKey));
			var ftype = typeof(TRefKey) == typeof(string) ? FieldType.String :
				typeof(TRefKey) == typeof(int) ? FieldType.Int :
				throw new Exception(typeof(TRefKey).Name + " key not supported");

			var data = new FieldCriterion {
				Column = col,
				FieldType = ftype,
				FieldName = dialog.ID,
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
