using System;
using System.Web.UI;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web;
using System.Linq.Expressions;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Nephrite.Identity;
using Nephrite.Web;
using Nephrite.Multilanguage;
using Nephrite.Meta;
using Nephrite.AccessControl;
using Nephrite.Http;
using Nephrite.Data;

namespace Nephrite.Web.Controls
{
	public partial class Filter : BaseUserControl
	{
		[Inject]
		public IAccessControl AccessControl { get; set; }

		IDC_ListFilter dc
		{
			get
			{
				return (IDC_ListFilter)A.Model;
			}
		}

		public string Width { get; set; }
		protected bool ViewEditMode { get; set; }

		protected PersistentFilter filterObject = new PersistentFilter(UrlHelper.Current().Action);
		public PersistentFilter FilterObject { get { return filterObject; } }
		public List<FilterItem> FilterList
		{
			get
			{
				if (ViewState["FilterItems"] == null)
					return filterObject.Items;
				else
					return (List<FilterItem>)ViewState["FilterItems"];
			}
		}

		public IEnumerable<IListColumn> Columns { get; set; }

		public string Sort { get { return filterObject.Sort; } }
		public string Group1Sort { get { return filterObject.Group1Sort; } }
		public string Group2Sort { get { return filterObject.Group2Sort; } }
		public int? Group1Column { get { return filterObject.Group1Column; } }
		public int? Group2Column { get { return filterObject.Group2Column; } }
		public int ItemsOnPage { get { return filterObject.ItemsOnPage; } }

		protected List<Field> fieldList = new List<Field>();

		string actionName = UrlHelper.Current().Action;
		public void SetActionName(string action)
		{
			actionName = action;
			filterObject = new PersistentFilter(action);
		}

		public bool HasValue
		{
			get
			{
				if (UrlHelper.Current().GetString("filterid").IsEmpty())
					return false;
				else
					return !filterObject.Columns.IsEmpty() || !filterObject.Sort.IsEmpty() ||
					filterObject.Items.Count > 0 || filterObject.ItemsOnPage > 0 ||
					filterObject.Group1Column.HasValue || filterObject.Group2Column.HasValue ||
					!filterObject.Group1Sort.IsEmpty() || !filterObject.Group2Sort.IsEmpty();
			}
		}

		public List<IN_Filter> GetViews()
		{
			int subjectID = Subject.Current.ID;
			var flist = (from f in dc.IN_Filter
						 where f.ListName == UrlHelper.Current().Controller + "_" + actionName &&
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
			filterObject = new PersistentFilter(filterID);
		}

		protected override void OnInit(EventArgs e)
		{
			tp0.Title = TextResource.Get("System.Filter.Tabs.Properties", "Свойства");
			tp1.Title = TextResource.Get("System.Filter.Tabs.Filter", "Фильтр");
			tp2.Title = TextResource.Get("System.Filter.Tabs.List", "Список");
			tp3.Title = TextResource.Get("System.Filter.Tabs.Sorting", "Сортировка");
			tpGrouping.Title = TextResource.Get("System.Filter.Tabs.Grouping", "Группировка");

			base.OnInit(e);
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

			if (Columns != null && !filterObject.Columns.IsEmpty())
			{
				//lMsg2.Text = filterObject.Columns + "<br>";
				var vc = filterObject.Columns.Split(new char[] { ',' }).Select(o => o.ToInt32(0));
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
			filterObject.editMode = ViewEditMode;
			if (filter.IsFirstPopulate)
			{
				mode.Value = filterObject.Items.Any(o => o.Advanced) ? "A" : "S";

				if (id > 0 || HasValue)
				{
					if (id > 0)
					{
						tbTitle.Text = filterObject.Name;
						cbDefault.Checked = filterObject.IsDefault;
						cbPersonal.Checked = filterObject.SubjectID.HasValue;
					}
					else
					{
						tbTitle.Text = "";
						cbDefault.Checked = false;
						cbPersonal.Checked = true;
					}
					tbParms.Text = filterObject.ListParms;
					tbItemsOnPage.Text = filterObject.ItemsOnPage.ToString();
					cbShared.Checked = !cbPersonal.Checked;
					if (!filterObject.Group1Sort.IsEmpty())
						ddlGroup1Order.SelectedValue = filterObject.Group1Sort;
					if (!filterObject.Group2Sort.IsEmpty())
						ddlGroup2Order.SelectedValue = filterObject.Group2Sort;
					if (filterObject.Group1Column.HasValue && Columns != null)
						ddlGroup1.SelectedValue = filterObject.Group1Column.ToString();
					if (filterObject.Group2Column.HasValue && Columns != null)
						ddlGroup2.SelectedValue = filterObject.Group2Column.ToString();
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
					filterObject = new PersistentFilter(actionName);
				}
				rptFilter.DataSource = filterObject.Items;
				rptFilter.DataBind();
				rptAdvFilter.DataSource = filterObject.Items;
				rptAdvFilter.DataBind();
				ViewState["FilterItems"] = filterObject.Items;

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
			filterObject = new PersistentFilter(id);
			StoreRepeater();
			addItem_Click(this, EventArgs.Empty);
			filterObject.Name = tbTitle.Text != "" ? tbTitle.Text : null;
			filterObject.ListParms = tbParms.Text;
			filterObject.IsDefault = cbDefault.Checked;
			filterObject.ListName = UrlHelper.Current().Controller + "_" + UrlHelper.Current().Action;
			filterObject.editMode = filter.Argument.ToInt32(0) > 0;
			filterObject.Columns = cblColumns.GetSelectedValues().Join(",");
			filterObject.ItemsOnPage = tbItemsOnPage.Text.ToInt32(0);

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
			filterObject.Sort = sortRes.Join(",");

			if (!cbPersonal.Checked && AccessControl.Check("filter.managecommonviews"))
				filterObject.SubjectID = null;
			else
				filterObject.SubjectID = Subject.Current.ID;

			if (ddlGroup1Order.SelectedValue == "")
			{
				filterObject.Group1Sort = null;
				filterObject.Group1Column = null;
			}
			else
			{
				filterObject.Group1Sort = ddlGroup1Order.SelectedValue;
				filterObject.Group1Column = ddlGroup1.SelectedValue.ToInt32();
			}

			if (ddlGroup2Order.SelectedValue == "")
			{
				filterObject.Group2Sort = null;
				filterObject.Group2Column = null;
			}
			else
			{
				filterObject.Group2Sort = ddlGroup2Order.SelectedValue;
				filterObject.Group2Column = ddlGroup2.SelectedValue.ToInt32();
			}

			if (filterObject.IsDefault)
			{
				foreach (var ff in dc.IN_Filter.Where(o => o.IsDefault && o.ListName.ToLower() == filterObject.ListName.ToLower() &&
					o.FilterID != id))
					ff.IsDefault = false;
			}

			try
			{
				GetPolishNotation();
			}
			catch (Exception ex)
			{
				lMsg.Text = ex.Message;
				filter.Reopen();
				return;
			}
			foreach (var item in FilterList)
				item.Advanced = mode.Value == "A";

			filterObject.Save(FilterList);
			Response.Redirect(UrlHelper.Current().SetParameter("filterid", filterObject.FilterID.ToString()));
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
		public void AddField<TClass, TValue>(MetaAttribute prop)
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
		public void AddField<TClass, TValue>(MetaReference prop)
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

		#region Применение фильтра
		List<object> GetPolishNotation()
		{
			// Преобразование в ОПН
			Stack<object> stack = new Stack<object>();
			List<object> pnlist = new List<object>();
			foreach (FilterItem item in FilterList)
			{
				if (item.Not)
					stack.Push('!');
				for (int i = 0; i < item.OpenBracketCount; i++)
					stack.Push('(');
				pnlist.Add(item);
				if (item.OpenBracketCount == 0 && item.Not)
				{
					pnlist.Add(stack.Pop());
				}
				for (int i = 0; i < item.CloseBracketCount; i++)
				{
					if (stack.Count == 0)
						throw new Exception(TextResource.Get("System.Filter.Error.BracketsNotConsistent", "Скобки не согласованы"));
					object obj = stack.Pop();
					do
					{
						if ((char)obj == '(')
							break;
						pnlist.Add(obj);
						if (stack.Count == 0)
							throw new Exception(TextResource.Get("System.Filter.Error.BracketsNotConsistent", "Скобки не согласованы"));
						obj = stack.Pop();
					} while ((char)obj != '(');
				}
				if (item.Operation == FilterItemOperation.And)
					stack.Push('&');
				else
					stack.Push('|');
			}
			if (stack.Count == 0)
				return pnlist;
			// Выкинуть последний символ
			stack.Pop();
			while (stack.Count() > 0)
			{
				object si = stack.Pop();
				if (si is char && (char)si == '(')
					throw new Exception(TextResource.Get("System.Filter.Error.BracketsNotConsistent", "Скобки не согласованы"));
				pnlist.Add(si);
			}

			return pnlist;
		}
		public IQueryable<T> GetQuery<T>(IQueryable<T> query)
		{
			if (filter.IsVisible)
				return query.Where(o => false);

			IQueryable<T> newquery = query;

			// Преобразование в ОПН
			Stack<object> stack = new Stack<object>();
			List<object> pnlist = new List<object>();
			try
			{
				pnlist = GetPolishNotation();
			}
			catch { }

			if (pnlist.Count == 0)
				return query;

			foreach (object it in pnlist)
			{
				if (it is FilterItem)
				{
					var item = it as FilterItem;

					Expression<Func<T, bool>> expr = null;
					Field f = fieldList.SingleOrDefault<Field>(f1 => f1.Title == item.Title);
					if (f == null)
						continue;

					LambdaExpression column = f.Column as LambdaExpression;
					if (column == null)
						column = ((List<object>)f.Column)[0] as LambdaExpression;

					if (item.Condition == TextResource.Get("System.Filter.Contains", "содержит") && f.FieldType == FieldType.String)
					{
						MethodCallExpression mc = Expression.Call(column.Body,
							typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
							Expression.Constant(item.Value.ToLower()));
						expr = Expression.Lambda<Func<T, bool>>(mc, column.Parameters);
					}

					if (item.Condition == TextResource.Get("System.Filter.StartsWith", "начинается с") && f.FieldType == FieldType.String)
					{
						MethodCallExpression mc = Expression.Call(column.Body,
							typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
							Expression.Constant(item.Value.ToLower()));
						expr = Expression.Lambda<Func<T, bool>>(mc, column.Parameters);
					}

					object val = item.Value.StartsWith("$") ?
						MacroManager.Evaluate(item.Value.Substring(1)) : item.Value;
					Type valType = column.Body is UnaryExpression ? ((UnaryExpression)column.Body).Operand.Type : column.Body.Type;

					if (f.FieldType == FieldType.Date)
					{
						DateTime dt;
						double d;

						if (item.Condition == TextResource.Get("System.Filter.LastXDays", "последние x дней"))
						{
							if (!double.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
								d = 0;
							val = DateTime.Now.AddDays(-d);
						}
						else
						{
							if (!DateTime.TryParseExact(item.Value, "d.MM.yyyy", null, DateTimeStyles.None, out dt))
							{ DateTime? dtn = null; val = dtn; } // dt = DateTime.Today;
							else
								val = dt;
						}
					}

					if (f.FieldType == FieldType.DateTime)
					{
						DateTime dt;
						double d;


						if (item.Condition == TextResource.Get("System.Filter.LastXDays", "последние x дней"))
						{
							if (!double.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
								d = 0;
							val = DateTime.Now.AddDays(-d);
						}
						else
						{
							if (!DateTime.TryParseExact(item.Value, "d.MM.yyyy", null, DateTimeStyles.None, out dt))
							{ DateTime? dtn = null; val = dtn; } // dt = DateTime.Today;
							else
								val = dt;
						}
					}

					if (f.FieldType == FieldType.Number || f.FieldType == FieldType.CustomInt)
					{
						decimal d;
						if (!decimal.TryParse(item.Value, NumberStyles.None, CultureInfo.GetCultureInfo("ru-ru"), out d))
							d = 0;
						val = d;
					}

					if (f.FieldType == FieldType.Boolean)
					{
						bool b;

						if (!bool.TryParse(item.Value, out b))
							b = false;

						if (A.DBType == DBType.DB2)
						{
							val = b ? 1 : 0;
							valType = typeof(int);
						}
						else
						{	
							val = b;
						}
						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);
						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);	
					}
					else if (f.FieldType == FieldType.String || f.FieldType == FieldType.DDL)
					{
						if (valType == typeof(Char) && val is string)
							val = ((string)val)[0];
						if ((valType == typeof(int?) || valType == typeof(int)) && val is string)
							val = int.Parse((string)val);

						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);

						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Constant(val, valType)), column.Parameters);
					}
					else if (f.FieldType == FieldType.CustomInt)
					{
						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, int, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, int, bool>>;
						val = Convert.ToInt32(val);
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else if (f.FieldType == FieldType.CustomString)
					{
						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, string, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, string, bool>>;
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else if (f.FieldType == FieldType.CustomObject)
					{
						if (valType == typeof(Char) && val is string)
							val = ((string)val)[0];
						if ((valType == typeof(int?) || valType == typeof(int)) && val is string)
							val = int.Parse((string)val);

						int num = f.Operator.IndexOf(item.Condition);
						// Взять Expression<Func<T, int, bool>> и подставить во второй параметр значение
						Expression<Func<T, object, bool>> col2 = ((List<object>)f.Column)[num] as Expression<Func<T, object, bool>>;
						expr = Expression.Lambda<Func<T, bool>>(ReplaceParameterExpression(col2.Body, col2.Parameters[1].Name, val), col2.Parameters[0]);
					}
					else
					{
						if (item.Condition == "=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == ">=" || item.Condition == TextResource.Get("System.Filter.LastXDays", "последние x дней"))
							expr = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == ">")
							expr = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<")
							expr = Expression.Lambda<Func<T, bool>>(Expression.LessThan(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<=")
							expr = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);

						if (item.Condition == "<>")
							expr = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Convert(column.Body, valType), Expression.Convert(Expression.Constant(val), valType)), column.Parameters);
					}
					if (expr != null)
					{
						stack.Push(expr);
					}
				}
				if (it is char)
				{
					var op = (char)it;
					if (op == '!')
					{
						Expression<Func<T, bool>> operand = stack.Pop() as Expression<Func<T, bool>>;
						var newop1 = Expression.Not(operand.Body);
						stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand.Parameters));
					}
					if (op == '&')
					{
						Expression<Func<T, bool>> operand1 = stack.Pop() as Expression<Func<T, bool>>;
						Expression<Func<T, bool>> operand2 = stack.Pop() as Expression<Func<T, bool>>;
						stack.Push(And(operand1, operand2));
						//var newop1 = Expression.And(operand1.Body, operand2.Body);
						//stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand2.Parameters));
					}
					if (op == '|')
					{
						Expression<Func<T, bool>> operand1 = stack.Pop() as Expression<Func<T, bool>>;
						Expression<Func<T, bool>> operand2 = stack.Pop() as Expression<Func<T, bool>>;
						stack.Push(Or(operand1, operand2));
						//var newop1 = Expression.Or(operand1.Body, operand2.Body);
						//stack.Push(Expression.Lambda<Func<T, bool>>(newop1, operand2.Parameters));
					}
				}
			}
			if (stack.Count > 0)
				newquery = newquery.Where<T>(stack.Pop() as Expression<Func<T, bool>>);

			return newquery;
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
		{
			return GetQuery<T>(query);
		}

		Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			return Expression.Lambda<Func<T, bool>>
			   (Expression.Or(expr1.Body, ReplaceParameterExpression(expr2.Body, expr1.Parameters[0])), expr1.Parameters);
		}

		Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			return Expression.Lambda<Func<T, bool>>
			   (Expression.And(expr1.Body, ReplaceParameterExpression(expr2.Body, expr1.Parameters[0])), expr1.Parameters);
		}

		Expression ReplaceParameterExpression(Expression x, ParameterExpression o)
		{
			if (x == null)
				return null;

			if (x is ConstantExpression)
				return x;

			if (x is ParameterExpression)
				return ((ParameterExpression)x).Name == "o" ? o : x;

			if (x is MemberExpression)
			{
				var x1 = x as MemberExpression;
				return Expression.MakeMemberAccess(ReplaceParameterExpression(x1.Expression, o), x1.Member);
			}
			if (x is UnaryExpression)
			{
				var x1 = x as UnaryExpression;
				return Expression.MakeUnary(x1.NodeType, ReplaceParameterExpression(x1.Operand, o), x1.Type, x1.Method);
			}
			if (x is BinaryExpression)
			{
				var x1 = x as BinaryExpression;
				return Expression.MakeBinary(x1.NodeType, ReplaceParameterExpression(x1.Left, o), ReplaceParameterExpression(x1.Right, o), x1.IsLiftedToNull, x1.Method, x1.Conversion);
			}
			if (x is ConditionalExpression)
			{
				var x1 = x as ConditionalExpression;
				return Expression.Condition(ReplaceParameterExpression(x1.Test, o), ReplaceParameterExpression(x1.IfTrue, o), ReplaceParameterExpression(x1.IfFalse, o));
			}
			if (x is MethodCallExpression)
			{
				var x1 = x as MethodCallExpression;
				return Expression.Call(ReplaceParameterExpression(x1.Object, o), x1.Method, x1.Arguments.Select(o1 => ReplaceParameterExpression(o1, o)));
			}
			if (x is LambdaExpression)
			{
				var x1 = x as LambdaExpression;
				return Expression.Lambda(x1.Type, ReplaceParameterExpression(x1.Body, o), x1.Parameters);
			}

			throw new Exception(TextResource.Get("System.Filter.Error.UnsupportedTypeExpression", "Неподдерживаемый тип Expression:") + x.GetType().ToString() + " : " + x.GetType().BaseType.Name);
		}

		Expression ReplaceParameterExpression(Expression x, string name, object value)
		{
			if (x == null)
				return null;

			if (x is ConstantExpression)
				return x;

			if (x is ParameterExpression)
				return ((ParameterExpression)x).Name == name ? Expression.Constant(value) : x;

			if (x is MemberExpression)
			{
				var x1 = x as MemberExpression;
				return Expression.MakeMemberAccess(ReplaceParameterExpression(x1.Expression, name, value), x1.Member);
			}
			if (x is UnaryExpression)
			{
				var x1 = x as UnaryExpression;
				return Expression.MakeUnary(x1.NodeType, ReplaceParameterExpression(x1.Operand, name, value), x1.Type, x1.Method);
			}
			if (x is BinaryExpression)
			{
				var x1 = x as BinaryExpression;
				return Expression.MakeBinary(x1.NodeType, ReplaceParameterExpression(x1.Left, name, value), ReplaceParameterExpression(x1.Right, name, value), x1.IsLiftedToNull, x1.Method, x1.Conversion);
			}
			if (x is ConditionalExpression)
			{
				var x1 = x as ConditionalExpression;
				return Expression.Condition(ReplaceParameterExpression(x1.Test, name, value), ReplaceParameterExpression(x1.IfTrue, name, value), ReplaceParameterExpression(x1.IfFalse, name, value));
			}
			if (x is MethodCallExpression)
			{
				var x1 = x as MethodCallExpression;
				return Expression.Call(ReplaceParameterExpression(x1.Object, name, value), x1.Method, x1.Arguments.Select(o1 => ReplaceParameterExpression(o1, name, value)));
			}
			if (x is LambdaExpression)
			{
				var x1 = x as LambdaExpression;
				return Expression.Lambda(x1.Type, ReplaceParameterExpression(x1.Body, name, value), x1.Parameters);
			}

			throw new Exception(TextResource.Get("System.Filter.Error.UnsupportedTypeExpression", "Неподдерживаемый тип Expression:") + x.GetType().ToString() + " : " + x.GetType().BaseType.Name);
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
	}

	[Serializable]
	public class FilterItem
	{
		public string Title { get; set; }
		public string Condition { get; set; }
		public string Value { get; set; }
		public string ValueTitle { get; set; }
		public FieldType FieldType { get; set; }
		public bool Not { get; set; }
		public int OpenBracketCount { get; set; }
		public int CloseBracketCount { get; set; }
		public FilterItemOperation Operation { get; set; }
		public bool Advanced { get; set; }

		public string OpenBrackets
		{
			get { return "".PadLeft(OpenBracketCount, '('); }
		}

		public string CloseBrackets
		{
			get { return "".PadLeft(CloseBracketCount, ')'); }
		}

		public override string ToString()
		{
			return Title + " " + (Condition == TextResource.Get("System.Filter.LastXDays", "последние x дней") ? String.Format(TextResource.Get("System.Filter.LastDays", "последние &quot;{0}&quot; дней"), ValueTitle) : Condition + " &quot;" + HttpUtility.HtmlEncode(ValueTitle) + "&quot;");
		}
	}

	public class Field
	{
		public string Title { get; set; }
		public object Column { get; set; }
		public FieldType FieldType { get; set; }
		public object DataSource { get; set; }
		public string DisplayMember { get; set; }
		public string ValueMember { get; set; }
		public List<string> Operator { get; set; }
	}

	public enum FieldType
	{
		String,
		Number,
		Date,
		DateTime,
		DDL,
		Boolean,
		CustomInt,
		CustomString,
		CustomObject
	}

	public enum FilterItemOperation
	{
		And,
		Or
	}

	public class PersistentFilter
	{
		IDC_ListFilter dc
		{
			get
			{
				return (IDC_ListFilter)A.Model;
			}
		}

		IN_Filter _filter;
		List<FilterItem> _items;
		public bool editMode = false;

		public PersistentFilter(string action)
		{
			_filter = dc.IN_Filter.SingleOrDefault(o => o.FilterID == UrlHelper.Current().GetInt("filterid", 0) && o.ListName.ToLower() == UrlHelper.Current().Controller.ToLower() + "_" + action.ToLower());
			_items = _filter != null && _filter.FilterValue != null ? XMLSerializer.Deserialize<List<FilterItem>>(_filter.FilterValue.Root) : new List<FilterItem>();
		}

		public PersistentFilter(int filterID)
		{
			_filter = dc.IN_Filter.SingleOrDefault(o => o.FilterID == filterID);
			_items = _filter != null && _filter.FilterValue != null ? XMLSerializer.Deserialize<List<FilterItem>>(_filter.FilterValue.Root) : new List<FilterItem>();
		}

		public List<FilterItem> Items
		{
			get { return _items; }
		}

		public void NewFilter()
		{
			_filter = dc.NewIN_Filter();
			_filter.FilterValue = _filter != null ? _filter.FilterValue :
					new XDocument(XMLSerializer.Serialize<List<FilterItem>>(new List<FilterItem>()));
			dc.IN_Filter.InsertOnSubmit(_filter);
		}

		IN_Filter Filter
		{
			get
			{
				if (_filter == null)
				{
					_filter = dc.NewIN_Filter();
					dc.IN_Filter.InsertOnSubmit(_filter);
				}
				return _filter;
			}
		}

		public string Name
		{
			get { return Filter.FilterName; }
			set { Filter.FilterName = value; }
		}

		public string ListParms
		{
			get { return Filter.ListParms; }
			set { Filter.ListParms = value; }
		}

		public string Columns
		{
			get { return Filter.Columns; }
			set { Filter.Columns = value; }
		}
		public string Sort
		{
			get { return Filter.Sort; }
			set { Filter.Sort = value; }
		}
		public int ItemsOnPage
		{
			get { return Filter.ItemsOnPage < 1 ? Paging.DefaultPageSize : Filter.ItemsOnPage; }
			set { Filter.ItemsOnPage = value; }
		}


		public string ListName
		{
			get { return Filter.ListName; }
			set { Filter.ListName = value; }
		}

		public bool IsDefault
		{
			get { return Filter.IsDefault; }
			set { Filter.IsDefault = value; }
		}

		public int FilterID
		{
			get { return Filter.FilterID; }
		}

		public int? SubjectID
		{
			get { return Filter.SubjectID; }
			set { Filter.SubjectID = value; }
		}

		#region Группировки
		public string Group1Sort
		{
			get { return Filter.Group1Sort; }
			set { Filter.Group1Sort = value; }
		}

		public string Group2Sort
		{
			get { return Filter.Group2Sort; }
			set { Filter.Group2Sort = value; }
		}

		public int? Group1Column
		{
			get { return Filter.Group1Column; }
			set { Filter.Group1Column = value; }
		}

		public int? Group2Column
		{
			get { return Filter.Group2Column; }
			set { Filter.Group2Column = value; }
		}
		#endregion

		public void Save(List<FilterItem> items)
		{
			_items = items;
			Filter.FilterValue = new XDocument(XMLSerializer.Serialize<List<FilterItem>>(_items));
			dc.SubmitChanges();
		}
	}

	public interface IN_Filter : IEntity
	{
		int FilterID { get; set; }
		int? SubjectID { get; set; }
		string ListName { get; set; }
		XDocument FilterValue { get; set; }
		string FilterName { get; set; }
		bool IsDefault { get; set; }
		System.Nullable<int> Group1Column { get; set; }
		string Group1Sort { get; set; }
		System.Nullable<int> Group2Column { get; set; }
		string Group2Sort { get; set; }
		string ListParms { get; set; }
		string Columns { get; set; }
		string Sort { get; set; }
		int ItemsOnPage { get; set; }
	}

	public interface IDC_ListFilter : IDataContext
	{
		ITable<IN_Filter> IN_Filter { get; }
		IN_Filter NewIN_Filter();
	}
}
