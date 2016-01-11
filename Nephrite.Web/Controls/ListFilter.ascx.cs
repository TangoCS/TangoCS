using System;
using System.Web.UI;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using Nephrite.Identity;
using Nephrite.Multilanguage;
using Nephrite.Meta;
using Nephrite.AccessControl;
using Nephrite.Controls;
using Nephrite.Data;

namespace Nephrite.Web.Controls
{
	public partial class Filter : BaseUserControl
	{
		[Inject]
		public IAccessControl AccessControl { get; set; }

		[Inject]
		public ITextResource TextResource { get; set; }

		[Inject]
		public IdentityManager<int> Identity { get; set; }

		IDC_ListFilter dc
		{
			get
			{
				return (IDC_ListFilter)A.Model;
			}
		}

		ListFilterEngine _engine = null;
		protected ListFilterEngine Engine
		{
			get
			{
				if (_engine == null)
				{
					_engine = new ListFilterEngine(TextResource, ConnectionManager.DBType, FilterList, new List<Field>());
				}
				return _engine;
            }
		}

		public string Width { get; set; }
		protected bool ViewEditMode { get; set; }

		PersistentFilter _filterObject = null;
		public PersistentFilter FilterObject
		{
			get
			{
				if (_filterObject == null)
				{
					_filterObject = new PersistentFilter(dc, Query.GetInt("filterid", 0));
				}
				return _filterObject;
			}
			private set
			{
				_filterObject = value;
			}
		}
		public List<FilterItem> FilterList
		{
			get
			{
				if (ViewState["FilterItems"] == null)
					return FilterObject.Criteria;
				else
					return (List<FilterItem>)ViewState["FilterItems"];
			}
		}

		public IEnumerable<IListColumn> Columns { get; set; }

		public string Sort { get { return FilterObject.Sort; } }
		public string Group1Sort { get { return FilterObject.Group1Sort; } }
		public string Group2Sort { get { return FilterObject.Group2Sort; } }
		public int? Group1Column { get { return FilterObject.Group1Column; } }
		public int? Group2Column { get { return FilterObject.Group2Column; } }
		public int ItemsOnPage { get { return FilterObject.ItemsOnPage; } }

		protected List<Field> fieldList { get { return Engine.FieldList; } }

		string actionName = null;
		public void SetActionName(string action)
		{
			actionName = action;
			FilterObject = new PersistentFilter(dc, Query.GetInt("filterid", 0));
		}

		public bool HasValue
		{
			get
			{
				if (Query.GetString("filterid").IsEmpty())
					return false;
				else
					return !FilterObject.Columns.IsEmpty() || !FilterObject.Sort.IsEmpty() ||
					FilterObject.Criteria.Count > 0 || FilterObject.ItemsOnPage > 0 ||
					FilterObject.Group1Column.HasValue || FilterObject.Group2Column.HasValue ||
					!FilterObject.Group1Sort.IsEmpty() || !FilterObject.Group2Sort.IsEmpty();
			}
		}

		public List<IN_Filter> GetViews()
		{
			int subjectID = Identity.CurrentSubject.ID;
			var flist = (from f in dc.IN_Filter
						 where f.ListName == Query.Controller + "_" + (actionName ?? Query.Action) &&
									(!f.SubjectID.HasValue || f.SubjectID.Value == subjectID) && f.FilterName != null
						 select f).ToList();
			int i = 0;
			while (i < flist.Count)
			{
				bool del = false;
				if (!flist[i].ListParms.IsEmpty())
				{
					string[] pa = flist[i].ListParms.Split(new char[] { ',' });
					foreach (string p in pa)
					{
						string[] cv = p.Split(new char[] { '=' });
						if (cv.Length == 2)
						{
							if (Query.GetString(cv[0]).ToLower() != cv[1].ToLower())
							{
								flist.RemoveAt(i);
								del = true;
							}
						}
					}
				}
				if (!del) i++;
			}
			return flist;
		}
		public void SetView(int filterID)
		{
			FilterObject = new PersistentFilter(dc, filterID);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			
			

			tp0.Title = TextResource.Get("System.Filter.Tabs.Properties", "Свойства");
			tp1.Title = TextResource.Get("System.Filter.Tabs.Filter", "Фильтр");
			tp2.Title = TextResource.Get("System.Filter.Tabs.List", "Список");
			tp3.Title = TextResource.Get("System.Filter.Tabs.Sorting", "Сортировка");
			tpGrouping.Title = TextResource.Get("System.Filter.Tabs.Grouping", "Группировка");
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				filter.Title = TextResource.Get("System.Filter.TitleWindow", "Представление");

				cbPersonal.Text = TextResource.Get("System.Filter.Tabs.Properties.Personal", "личное");
				cbPersonal.ToolTip = TextResource.Get("System.Filter.Tabs.Properties.ToolTip.Personal", "Представление видно только создавшему пользователю");
				cbShared.Text = TextResource.Get("System.Filter.Tabs.Properties.Shared", "общее");
				cbShared.ToolTip = TextResource.Get("System.Filter.Tabs.Properties.ToolTip.Shared", "Представление видно всем пользователям");
				cbDefault.Text = TextResource.Get("System.Filter.Tabs.Properties.DefaultView", "представление по умолчанию");
				cbDefault.ToolTip = TextResource.Get("System.Filter.Tabs.Properties.ToolTip.DefaultView",
													 "Предустановленное представление при отображении списка");

				addItem.Text = TextResource.Get("System.Filter.AddCriteria", "Добавить критерий");

				ddlSortOrder.Items.Add(new ListItem(TextResource.Get("System.Filter.Asc", "по возрастанию"), "0"));
				ddlSortOrder.Items.Add(new ListItem(TextResource.Get("System.Filter.Desc", "по убыванию"), "1"));

				ddlGroup1Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Grouping.No", "нет"), ""));
				ddlGroup1Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Asc", "по возрастанию"), "A"));
				ddlGroup1Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Desc", "по убыванию"), "D"));

				ddlGroup2Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Grouping.No", "нет"), ""));
				ddlGroup2Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Asc", "по возрастанию"), "A"));
				ddlGroup2Order.Items.Add(new ListItem(TextResource.Get("System.Filter.Desc", "по убыванию"), "D"));


			}
			if (!String.IsNullOrEmpty(Width))
				filter.Width = Width;

			if (Columns != null && !FilterObject.Columns.IsEmpty())
			{
				//lMsg2.Text = filterObject.Columns + "<br>";
				var vc = FilterObject.Columns.Split(new char[] { ',' }).Select(o => o.ToInt32(0));
				for (int i = 0; i < Columns.Count(); i++)
					if (!Columns.ElementAt(i).Fixed)
					{
						Columns.ElementAt(i).Visible = vc.Contains(i);
						//lMsg2.Text += i.ToString() + ";" + vc.Contains(i).ToString() + "<br>";
					}
			}
			if (Columns != null)
			{
				ddlSortColumn.DataBindOnce(Columns.Where(o => o.Sortable));
				ddlGroup1.DataBindOnce(Columns.Where(o => o.Sortable), true);
				ddlGroup2.DataBindOnce(Columns.Where(o => o.Sortable), true);
			}

			ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "filtersort", "initsorttab();", true);

		}

		protected void lbSwitchMode_Click(object sender, EventArgs e)
		{
			if (mode.Value == "S") // Simple
				mode.Value = "A"; //Advanced
			else
				mode.Value = "S";
			lbSwitchMode.Text = mode.Value == "A"
									? TextResource.Get("System.Filter.SimpleFilter", "Простой фильтр")
									: TextResource.Get("System.Filter.AdvancedFilter", "Расширенный фильтр");
		}

		protected void upItem_Click(object sender, EventArgs e)
		{
			StoreRepeater();
			int num = ((RepeaterItem)((Control)sender).Parent).ItemIndex;
			if (num > 0)
				FilterList.Reverse(num - 1, 2);

			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
		}

		protected void downItem_Click(object sender, EventArgs e)
		{
			StoreRepeater();
			int num = ((RepeaterItem)((Control)sender).Parent).ItemIndex;
			if (num < FilterList.Count - 1)
				FilterList.Reverse(num, 2);
			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
		}

		protected void lbClear_Click(object sender, EventArgs e)
		{
			FilterList.Clear();
			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
		}

		protected override void OnPreRender(EventArgs e)
		{
			filter.Visible = Visible;
			base.OnPreRender(e);
		}

		#region render
		public string RenderMethod()
		{
			return filter.RenderRun();
		}
		public string RenderEditViewMethod(int savedFilterID)
		{
			return filter.RenderRun(savedFilterID);
		}
		public string RenderCreateViewMethod()
		{
			return filter.RenderRun(0, "N");
		}
		#endregion

		protected void filter_Populate(object sender, EventArgs e)
		{
			int id = filter.Argument.ToInt32(0);
			ViewEditMode = id > 0 || filter.Mode == "N";
			FilterObject.editMode = ViewEditMode;
			if (filter.IsFirstPopulate)
			{
				mode.Value = FilterObject.Criteria.Any(o => o.Advanced) ? "A" : "S";

				if (id > 0 || HasValue)
				{
					if (id > 0)
					{
						tbTitle.Text = FilterObject.Name;
						cbDefault.Checked = FilterObject.IsDefault;
						cbPersonal.Checked = FilterObject.SubjectID.HasValue;
					}
					else
					{
						tbTitle.Text = "";
						cbDefault.Checked = false;
						cbPersonal.Checked = true;
					}
					tbParms.Text = FilterObject.ListParms;
					tbItemsOnPage.Text = FilterObject.ItemsOnPage.ToString();
					cbShared.Checked = !cbPersonal.Checked;
					if (!FilterObject.Group1Sort.IsEmpty())
						ddlGroup1Order.SelectedValue = FilterObject.Group1Sort;
					if (!FilterObject.Group2Sort.IsEmpty())
						ddlGroup2Order.SelectedValue = FilterObject.Group2Sort;
					if (FilterObject.Group1Column.HasValue && Columns != null)
						ddlGroup1.SelectedValue = FilterObject.Group1Column.ToString();
					if (FilterObject.Group2Column.HasValue && Columns != null)
						ddlGroup2.SelectedValue = FilterObject.Group2Column.ToString();
				}
				else
				{
					tbTitle.Text = "";
					cbDefault.Checked = false;
					cbPersonal.Checked = true;
					cbShared.Checked = false;
					rptFilter.DataSource = null;
					rptFilter.DataBind();
					rptAdvFilter.DataSource = null;
					rptAdvFilter.DataBind();
					FilterObject = new PersistentFilter(dc, Query.GetInt("filterid", 0));
				}
				rptFilter.DataSource = FilterObject.Criteria;
				rptFilter.DataBind();
				rptAdvFilter.DataSource = FilterObject.Criteria;
				rptAdvFilter.DataBind();
				ViewState["FilterItems"] = FilterObject.Criteria;

				if (Columns != null)
				{
					int i = 0;
					cblColumns.Items.Clear();
					foreach (IListColumn c in Columns)
					{
						if (!c.Fixed)
							cblColumns.Items.Add(new ListItem { Selected = c.Visible, Text = c.Name, Value = i.ToString() });
						i++;
					}
				}


				tp0.Visible = ViewEditMode;
				tp2.Visible = Columns != null;
				tp3.Visible = Columns != null;
				tpGrouping.Visible = Columns != null;
			}

			if (ddlItem.Items.Count == 0)
			{
				ddlItem.DataSource = fieldList.OrderBy(o => o.Title);
				ddlItem.DataBind();
				ddlItem.Items.Insert(0, "");
				ddlItem.SelectedValue = ddlItem.Items[0].Value;

			}

			BindConditions();
			lbSwitchMode.Text = mode.Value == "A" ? TextResource.Get("System.Filter.SimpleFilter", "Простой фильтр")
									: TextResource.Get("System.Filter.AdvancedFilter", "Расширенный фильтр");
		}

		protected void update_items(object sender, EventArgs e)
		{
			StoreRepeater();
			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
		}

		#region Группировка
		/*int group1ColumnIndex = Query.GetString("group1").Length > 1 ? Query.GetString("group1").Substring(1).ToInt32(-1) : -1;

		int group2ColumnIndex = Query.GetString("group2").Length > 1 ? Query.GetString("group2").Substring(1).ToInt32(-1) : -1;

		public Func<T, object> Group1Column<T>()
		{
			if (group1ColumnIndex >= 0 && group1ColumnIndex < fieldList.Count)
			{
				Expression<Func<T, object>> expr = (Expression<Func<T, object>>)fieldList[group1ColumnIndex].Column;
				return expr.Compile();
			}
			return null;
		}

		public Func<T, object> Group2Column<T>()
		{
			if (group2ColumnIndex >= 0 && group2ColumnIndex < fieldList.Count)
			{
				Expression<Func<T, object>> expr = (Expression<Func<T, object>>)fieldList[group2ColumnIndex].Column;
				return expr.Compile();
			}
			return null;
		}

		public string Group1ColumnName
		{
			get
			{
				if (group1ColumnIndex >= 0 && group1ColumnIndex < fieldList.Count)
				{
					LambdaExpression expr = (LambdaExpression)fieldList[group1ColumnIndex].Column;
					if (expr.Body is UnaryExpression)
					{
						var ue = expr.Body as UnaryExpression;
						if (ue.Operand is MemberExpression)
						{
							var me = ue.Operand as MemberExpression;
							return me.Member.Name;
						}
					}

					return "";
				}
				return null;
			}
		}

		public string Group2ColumnName
		{
			get
			{
				if (group2ColumnIndex >= 0 && group2ColumnIndex < fieldList.Count)
				{
					LambdaExpression expr = (LambdaExpression)fieldList[group2ColumnIndex].Column;
					if (expr.Body is UnaryExpression)
					{
						var ue = expr.Body as UnaryExpression;
						if (ue.Operand is MemberExpression)
						{
							var me = ue.Operand as MemberExpression;
							return me.Member.Name;
						}
					}

					return "";
				}
				return null;
			}
		}

		public string Group1ColumnTitle
		{
			get
			{
				if (group1ColumnIndex >= 0 && group1ColumnIndex < fieldList.Count)
					return fieldList[group1ColumnIndex].Title;
				return null;
			}
		}

		public string Group2ColumnTitle
		{
			get
			{
				if (group2ColumnIndex >= 0 && group2ColumnIndex < fieldList.Count)
					return fieldList[group2ColumnIndex].Title;
				return null;
			}
		}

		public bool Group1Asc
		{
			get
			{
				if (Query.GetString("group1").Length > 1)
				{
					return Query.GetString("group1").ToLower()[0] == 'a';
				}
				return true;
			}
		}

		public bool Group2Asc
		{
			get
			{
				if (Query.GetString("group2").Length > 1)
				{
					return Query.GetString("group2").ToLower()[0] == 'a';
				}
				return true;
			}
		}*/
		#endregion

		protected void filter_OKClick(object sender, EventArgs e)
		{
			lMsg.Text = "";

			int id = filter.Argument.ToInt32(0);
			FilterObject = new PersistentFilter(dc, id);
			StoreRepeater();
			addItem_Click(this, EventArgs.Empty);
			FilterObject.Name = tbTitle.Text != "" ? tbTitle.Text : null;
			FilterObject.ListParms = tbParms.Text;
			FilterObject.IsDefault = cbDefault.Checked;
			FilterObject.ListName = UrlHelper.Current().Controller + "_" + UrlHelper.Current().Action;
			FilterObject.editMode = filter.Argument.ToInt32(0) > 0;
			FilterObject.Columns = cblColumns.GetSelectedValues().Join(",");
			FilterObject.ItemsOnPage = tbItemsOnPage.Text.ToInt32(0);

			IEnumerable<string> qs = Request.Form.AllKeys.Where(o => o.StartsWith(ddlSortColumn.UniqueID));
			List<string> sortRes = new List<string>();
			foreach (string q in qs)
			{
				if (String.IsNullOrEmpty(Request.Form[q])) continue;
				string prefix = "";
				if (q != ddlSortColumn.UniqueID)
				{
					string[] qparts = q.Split(new char[] { '_' });
					prefix = "_" + qparts[qparts.Length - 1];
				}

				string s = Request.Form[ddlSortColumn.UniqueID + prefix];
				if (Request.Form[ddlSortOrder.UniqueID + prefix] == "1")
					s += "_desc";

				sortRes.Add(s);
			}
			FilterObject.Sort = sortRes.Join(",");

			if (!cbPersonal.Checked && AccessControl.Check("filter.managecommonviews"))
				FilterObject.SubjectID = null;
			else
				FilterObject.SubjectID = Identity.CurrentSubject.ID;

			if (ddlGroup1Order.SelectedValue == "")
			{
				FilterObject.Group1Sort = null;
				FilterObject.Group1Column = null;
			}
			else
			{
				FilterObject.Group1Sort = ddlGroup1Order.SelectedValue;
				FilterObject.Group1Column = ddlGroup1.SelectedValue.ToInt32();
			}

			if (ddlGroup2Order.SelectedValue == "")
			{
				FilterObject.Group2Sort = null;
				FilterObject.Group2Column = null;
			}
			else
			{
				FilterObject.Group2Sort = ddlGroup2Order.SelectedValue;
				FilterObject.Group2Column = ddlGroup2.SelectedValue.ToInt32();
			}

			if (FilterObject.IsDefault)
			{
				foreach (var ff in dc.IN_Filter.Where(o => o.IsDefault && o.ListName.ToLower() == FilterObject.ListName.ToLower() &&
					o.FilterID != id))
					ff.IsDefault = false;
			}

			try
			{
				Engine.GetPolishNotation();
			}
			catch (Exception ex)
			{
				lMsg.Text = ex.Message;
				filter.Reopen();
				return;
			}
			foreach (var item in FilterList)
				item.Advanced = mode.Value == "A";

			FilterObject.Save(FilterList);
			Response.Redirect(UrlHelper.Current().SetParameter("filterid", FilterObject.FilterID.ToString()));
		}

		void StoreRepeater()
		{
			if (mode.Value == "A")
			{
				int num = 0;
				foreach (var item in FilterList)
				{
					var ri = rptAdvFilter.Items[num];
					item.Not = ((DropDownList)ri.FindControl("ddlNot")).SelectedValue == "N";
					item.OpenBracketCount = ((TextBox)ri.FindControl("lp")).Text.Length;
					item.CloseBracketCount = ((TextBox)ri.FindControl("rp")).Text.Length;
					item.Operation = ((DropDownList)ri.FindControl("ddlOp")).SelectedValue == "&" ? FilterItemOperation.And : FilterItemOperation.Or;
					num++;
				}
			}
		}
		protected void rptFilter_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			((ImageButton)e.Item.FindControl("ImageButton1")).ToolTip = TextResource.Get("System.Filter.Delete", "Удалить");
			((ImageButton)e.Item.FindControl("ImageButton1")).AlternateText = TextResource.Get("System.Filter.Delete", "Удалить");
		}

		protected void rptAdvFilter_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			((TextBox)e.Item.FindControl("lp")).ToolTip = TextResource.Get("System.Filter.ToolTip.OpeningBrackets", "Открывающие скобки");
			((TextBox)e.Item.FindControl("rp")).ToolTip = TextResource.Get("System.Filter.ToolTip.ClosingBrackets", "Закрывающие скобки");
			((ImageButton)e.Item.FindControl("up")).ToolTip = TextResource.Get("System.Filter.ToolTip.MoveUp", "Переместить вверх");
			((ImageButton)e.Item.FindControl("up")).AlternateText = TextResource.Get("System.Filter.Up", "Вверх");
			((ImageButton)e.Item.FindControl("down")).ToolTip = TextResource.Get("System.Filter.ToolTip.MoveDown", "Переместить вниз");
			((ImageButton)e.Item.FindControl("down")).AlternateText = TextResource.Get("System.Filter.Down", "Вниз");
			((ImageButton)e.Item.FindControl("delItem")).ToolTip = TextResource.Get("System.Filter.ToolTip.Delete", "Удалить");
			((ImageButton)e.Item.FindControl("delItem")).AlternateText = TextResource.Get("System.Filter.ToolTip.Delete", "Удалить");

			((DropDownList)e.Item.FindControl("ddlNot")).ToolTip = TextResource.Get("System.Filter.ToolTip.Negation", "Отрицание");
			((DropDownList)e.Item.FindControl("ddlNot")).Items.Add(new ListItem(TextResource.Get("System.Filter.NOT", "НЕ"), "N"));
			((DropDownList)e.Item.FindControl("ddlNot")).SelectedValue = ((FilterItem)e.Item.DataItem).Not ? "N" : "";

			var ddlOp = (DropDownList)e.Item.FindControl("ddlOp");
			ddlOp.ToolTip = TextResource.Get("System.Filter.Operator", "Оператор");
			ddlOp.Items.Add(new ListItem(TextResource.Get("System.Filter.AND", "И"), "&"));
			ddlOp.Items.Add(new ListItem(TextResource.Get("System.Filter.OR", "ИЛИ"), "|"));
			ddlOp.Visible = e.Item.ItemIndex < FilterList.Count() - 1;
			ddlOp.SelectedValue = ((FilterItem)e.Item.DataItem).Operation == FilterItemOperation.And ? "&" : "|";
			((TextBox)e.Item.FindControl("lp")).Text = ((FilterItem)e.Item.DataItem).OpenBrackets;
			((TextBox)e.Item.FindControl("rp")).Text = ((FilterItem)e.Item.DataItem).CloseBrackets;
		}

		void BindConditions()
		{
			BindConditions(false);
		}

		void BindConditions(bool alwaysRefreshCondition)
		{
			if (ddlItem.SelectedIndex <= 0)
			{
				ddlCondition.Visible = false;
				txtValue.Visible = false;
				ddlValue.Visible = false;
				calValue.Visible = false;
				cbxValue.Visible = false;
				return;
			}

			ddlCondition.Visible = true;
			//int fieldno = ddlItem.SelectedIndex - 1;
			Field curField = fieldList.Single(o => o.Title == ddlItem.SelectedValue);

			if (ViewState["CurrentFieldType"] == null || (FieldType)ViewState["CurrentFieldType"] != curField.FieldType || alwaysRefreshCondition)
			{
				ddlCondition.Items.Clear();
				switch (curField.FieldType)
				{
					case FieldType.String:
						ddlCondition.Items.Add(new ListItem(TextResource.Get("System.Filter.StartsWith", "начинается с"), TextResource.Get("System.Filter.StartsWith", "начинается с")));
						ddlCondition.Items.Add(new ListItem(TextResource.Get("System.Filter.Contains", "содержит"), TextResource.Get("System.Filter.Contains", "содержит")));
						ddlCondition.Items.Add(new ListItem("=", "="));
						ddlCondition.Items.Add(new ListItem("<>", "<>"));
						txtValue.Visible = true;
						ddlValue.Visible = false;
						calValue.Visible = false;
						cbxValue.Visible = false;
						break;
					case FieldType.Number:
						ddlCondition.Items.Add(new ListItem("=", "="));
						ddlCondition.Items.Add(new ListItem(">", ">"));
						ddlCondition.Items.Add(new ListItem(">=", ">="));
						ddlCondition.Items.Add(new ListItem("<", "<"));
						ddlCondition.Items.Add(new ListItem("<=", "<="));
						ddlCondition.Items.Add(new ListItem("<>", "<>"));
						txtValue.Visible = true;
						ddlValue.Visible = false;
						calValue.Visible = false;
						cbxValue.Visible = false;
						break;
					case FieldType.Date:
						ddlCondition.Items.Add(new ListItem("=", "="));
						ddlCondition.Items.Add(new ListItem(">", ">"));
						ddlCondition.Items.Add(new ListItem(">=", ">="));
						ddlCondition.Items.Add(new ListItem("<", "<"));
						ddlCondition.Items.Add(new ListItem("<=", "<="));
						ddlCondition.Items.Add(new ListItem("<>", "<>"));
						ddlCondition.Items.Add(new ListItem(TextResource.Get("System.Filter.LastXDays", "последние x дней"), "d"));
						ddlCondition.AutoPostBack = true;
						txtValue.Visible = false;
						ddlValue.Visible = false;
						calValue.Visible = true;
						calValue.ShowTime = false;
						cbxValue.Visible = false;
						break;
					case FieldType.DateTime:
						ddlCondition.Items.Add(new ListItem("=", "="));
						ddlCondition.Items.Add(new ListItem(">", ">"));
						ddlCondition.Items.Add(new ListItem(">=", ">="));
						ddlCondition.Items.Add(new ListItem("<", "<"));
						ddlCondition.Items.Add(new ListItem("<=", "<="));
						ddlCondition.Items.Add(new ListItem("<>", "<>"));
						ddlCondition.Items.Add(new ListItem(TextResource.Get("System.Filter.LastXDays", "последние x дней"), "d"));
						ddlCondition.AutoPostBack = true;
						txtValue.Visible = false;
						ddlValue.Visible = false;
						calValue.Visible = true;
						calValue.ShowTime = true;
						cbxValue.Visible = false;
						break;
					case FieldType.DDL:
						ddlCondition.Items.Add(new ListItem("=", "="));
						ddlCondition.Items.Add(new ListItem("<>", "<>"));
						txtValue.Visible = false;
						ddlValue.Visible = true;
						calValue.Visible = false;
						cbxValue.Visible = false;

						ddlValue.DataSource = curField.DataSource;
						ddlValue.DataTextField = curField.DisplayMember;
						ddlValue.DataValueField = curField.ValueMember;
						ddlValue.DataBind();
						break;
					case FieldType.Boolean:
						txtValue.Visible = false;
						ddlValue.Visible = false;
						calValue.Visible = false;
						cbxValue.Visible = true;
						ddlCondition.Items.Add(new ListItem("=", "="));
						break;
					case FieldType.CustomInt:
						foreach (string op in curField.Operator)
							ddlCondition.Items.Add(new ListItem(op, op));
						txtValue.Visible = true;
						ddlValue.Visible = false;
						calValue.Visible = false;
						cbxValue.Visible = false;
						break;
					case FieldType.CustomString:
						foreach (string op in curField.Operator)
							ddlCondition.Items.Add(new ListItem(op, op));
						txtValue.Visible = true;
						ddlValue.Visible = false;
						calValue.Visible = false;
						cbxValue.Visible = false;
						break;
					case FieldType.CustomObject:
						foreach (string op in curField.Operator)
							ddlCondition.Items.Add(new ListItem(op, op));
						txtValue.Visible = false;
						ddlValue.Visible = true;
						calValue.Visible = false;
						cbxValue.Visible = false;

						ddlValue.DataSource = curField.DataSource;
						ddlValue.DataTextField = curField.DisplayMember;
						ddlValue.DataValueField = curField.ValueMember;
						ddlValue.DataBind();
						break;
				}
				ViewState["CurrentFieldType"] = curField.FieldType;
			}
		}

		protected void ddlItem_SelectedIndexChanged(object sender, EventArgs e)
		{
			////StoreRepeater();
			BindConditions(true);
		}

		protected void ddlCondition_SelectedIndexChanged(object sender, EventArgs e)
		{
			////StoreRepeater();
			//int fieldno = ddlItem.SelectedIndex - 1;
			Field curField = fieldList.Single(o => o.Title == ddlItem.SelectedValue);

			if (curField.FieldType == FieldType.Date ||
				curField.FieldType == FieldType.DateTime)
			{
				if (ddlCondition.SelectedValue == "d")
				{
					txtValue.Visible = true;
					calValue.Visible = false;
				}
				else
				{
					txtValue.Visible = false;
					calValue.Visible = true;
				}
			}

		}

		protected void delItem_Click(object sender, EventArgs e)
		{
			StoreRepeater();
			FilterList.RemoveAt(((RepeaterItem)((Control)sender).Parent).ItemIndex);
			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
			ddlItem.SelectedIndex = 0;
			BindConditions();
		}

		protected void addItem_Click(object sender, EventArgs e)
		{
			if (ddlItem.SelectedIndex <= 0)
				return;
			StoreRepeater();
			FilterItem item = new FilterItem
			{
				Title = ddlItem.SelectedItem.Text,
				Condition = ddlCondition.SelectedItem.Text,
				Advanced = mode.Value == "A"
			};
			item.FieldType = fieldList.Single<Field>(f => f.Title == item.Title).FieldType;
			switch (item.FieldType)
			{
				case FieldType.String:
					item.ValueTitle = txtValue.Text;
					item.Value = item.ValueTitle;
					break;
				case FieldType.Number:
					item.ValueTitle = txtValue.Text;
					item.Value = item.ValueTitle;
					break;
				case FieldType.CustomInt:
					item.ValueTitle = txtValue.Text;
					item.Value = item.ValueTitle;
					break;
				case FieldType.CustomString:
					item.ValueTitle = txtValue.Text;
					item.Value = item.ValueTitle;
					break;
				case FieldType.Date:
					if (ddlCondition.SelectedValue == "d")
					{
						item.ValueTitle = txtValue.Text;
						item.Value = item.ValueTitle;
					}
					else
					{
						if (calValue.Date.HasValue)
						{
							item.ValueTitle = calValue.Date.Value.ToString("dd.MM.yyyy");
							item.Value = item.ValueTitle;
						}
						else
						{
							item.ValueTitle = "нет";
							item.Value = "";
						}
					}
					break;
				case FieldType.DateTime:
					if (ddlCondition.SelectedValue == "d")
					{
						item.ValueTitle = txtValue.Text;
						item.Value = item.ValueTitle;
					}
					else
					{
						if (calValue.Date.HasValue)
						{
							item.ValueTitle = calValue.Date.Value.ToString("dd.MM.yyyy HH:mm");
							item.Value = item.ValueTitle;
						}
						else
						{
							item.ValueTitle = "нет";
							item.Value = "";
						}
					}
					break;
				case FieldType.DDL:
					item.ValueTitle = ddlValue.SelectedItem.Text;
					item.Value = ddlValue.SelectedItem.Value;
					break;
				case FieldType.CustomObject:
					item.ValueTitle = ddlValue.SelectedItem.Text;
					item.Value = ddlValue.SelectedItem.Value;
					break;
				case FieldType.Boolean:
					item.Value = cbxValue.Checked.ToString();
					item.ValueTitle = cbxValue.Checked ? TextResource.Get("System.Filter.Yes", "Да") : TextResource.Get("System.Filter.No", "Нет");
					break;
			}

			FilterList.Add(item);
			rptFilter.DataSource = FilterList;
			rptFilter.DataBind();
			rptAdvFilter.DataSource = FilterList;
			rptAdvFilter.DataBind();
			ddlItem.SelectedIndex = 0;
			txtValue.Text = "";
			BindConditions();
			////hfFilterId.Value = filterObject.FilterID.ToString();
		}

		protected void del_Click(object sender, EventArgs e)
		{
			int id = filter.Argument.ToInt32(0);
			dc.IN_Filter.DeleteOnSubmit(dc.IN_Filter.Single(o => o.FilterID == id));
			dc.SubmitChanges();
			Response.Redirect(Query.RemoveParameter("filter", "filterid", "group1", "group2"));
		}

		public string ToText()
		{
			string str = "";
			string op = "";
			foreach (var item in FilterList)
			{
				str += " <b>" + op + "</b>";
				if (item.Not)
					str += string.Format(" <b>{0}</b>", TextResource.Get("System.Filter.NOT", "НЕ"));
				if (item.OpenBracketCount > 0)
					str += " " + item.OpenBrackets;
				str += " " + item.ToString();
				if (item.CloseBracketCount > 0)
					str += " " + item.CloseBrackets;

				op = item.Operation == FilterItemOperation.And ? TextResource.Get("System.Filter.AND", "И") : TextResource.Get("System.Filter.OR", "ИЛИ");
			}
			return str;
		}

		#region добавление полей
		public void AddFieldString<T>(string title, Expression<Func<T, object>> column)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = FieldType.String
			});
		}
		public void AddFieldNumber<T>(string title, Expression<Func<T, object>> column)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = FieldType.Number
			});
		}
		public void AddFieldNumber<T>(string title, string opname, Expression<Func<T, int, bool>> column)
		{
			var f = fieldList.FirstOrDefault(o => o.Title == title);
			if (f == null)
			{
				f = new Field
				{
					Title = title,
					Column = new List<object>(),
					FieldType = FieldType.CustomInt,
					Operator = new List<string>()
				};
				fieldList.Add(f);
			}
			((List<object>)f.Column).Add(column);
			f.Operator.Add(opname);
		}
		public void AddFieldString<T>(string title, string opname, Expression<Func<T, string, bool>> column)
		{
			var f = fieldList.FirstOrDefault(o => o.Title == title);
			if (f == null)
			{
				f = new Field
				{
					Title = title,
					Column = new List<object>(),
					FieldType = FieldType.CustomString,
					Operator = new List<string>()
				};
				fieldList.Add(f);
			}
			((List<object>)f.Column).Add(column);
			f.Operator.Add(opname);
		}

		public void AddFieldBoolean<T>(string title, Expression<Func<T, bool>> column)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = FieldType.Boolean
			});
		}
		public void AddFieldBoolean<T>(string title, Expression<Func<T, bool?>> column)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = FieldType.Boolean
			});
		}
		public void AddFieldDate<T>(string title, Expression<Func<T, object>> column, bool withTime)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = withTime ? FieldType.DateTime : FieldType.Date
			});
		}
		public void AddFieldDDL<T>(string title, Expression<Func<T, object>> column, object dataSource, string displayMember, string valueMember)
		{
			fieldList.Add(new Field
			{
				Title = title,
				Column = column,
				FieldType = FieldType.DDL,
				DataSource = dataSource,
				DisplayMember = displayMember,
				ValueMember = valueMember
			});
		}
		public void AddFieldDDL<T>(string title, string opname, Expression<Func<T, object, bool>> column, object dataSource, string displayMember, string valueMember)
		{
			var f = fieldList.FirstOrDefault(o => o.Title == title);
			if (f == null)
			{
				f = new Field
				{
					Title = title,
					Column = new List<object>(),
					FieldType = FieldType.CustomObject,
					Operator = new List<string>(),
					DataSource = dataSource,
					DisplayMember = displayMember,
					ValueMember = valueMember
				};
				fieldList.Add(f);
			}
			((List<object>)f.Column).Add(column);
			f.Operator.Add(opname);
		}
		public void AddField(MetaAttribute prop)
		{
			var f = new Field();
			f.Title = prop.Caption;
			f.Column = prop.GetValueExpression;

			if (prop.Type is MetaStringType)
				f.FieldType = FieldType.String;
			else if (prop.Type is MetaDateType)
				f.FieldType = FieldType.Date;
			else if (prop.Type is MetaDateTimeType)
				f.FieldType = FieldType.DateTime;
			else if (prop.Type is IMetaNumericType)
				f.FieldType = FieldType.Number;
			else if (prop.Type is MetaBooleanType)
				f.FieldType = FieldType.Boolean;
			else
				return;

			fieldList.Add(f);
		}
		public void AddField(MetaReference prop)
		{
			var f = new Field();
			f.Title = prop.Caption;
			f.Column = prop.GetValueExpression;
			f.FieldType = FieldType.DDL;

			f.DataSource = prop.AllObjects;
			f.DisplayMember = prop.DataTextField;
			f.ValueMember = prop.RefClass.Key.Name;

			fieldList.Add(f);
		}
		#endregion

		public void SetDefaultView()
		{
			int currentViewID = Query.GetInt("filterid", 0);

			var views = GetViews();
			var defaultf = views.Where(o => o.IsDefault).OrderByDescending(o => o.SubjectID ?? 0).FirstOrDefault();

			if (defaultf != null && Query.GetString("filter") != "all")
			{
				if (currentViewID == 0)
				{
					currentViewID = defaultf.FilterID;
					SetView(currentViewID);
				}
			}
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
		{
			return Engine.ApplyFilter(query);
		}
    }





	
}
