using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using System.Linq.Expressions;
using System.Collections;

using Nephrite.Web.Controls;
using System.Web.UI.WebControls;
using Nephrite.Web.Layout;
using System.Text.RegularExpressions;
using Nephrite.Web.SettingsManager;
using Nephrite.Meta;

namespace Nephrite.Web
{
	[FileLevelControlBuilder(typeof(CustomViewUserControlControlBuilder))]
	public abstract class ViewControl : UserControl, IMasterControl
    {
		HtmlHelperBase html;
		public HtmlHelperBase Html
		{
			get
			{
				//return html.Instance;
				if (html == null)
					html = new HtmlHelperWSS();
				return html;
			}
		}

		public void EnsureHttps()
		{
			if (Request.IsSecureConnection)
				return;
			int port = AppSettings.Get("HttpsPort").ToInt32(443);
			string url = Regex.Replace(Request.Url.ToString(), "^http://", "", RegexOptions.IgnoreCase);
			string host = url.Substring(0, url.IndexOfAny(new char[] { '/', ':' }));
			string httpsWarning = AppSettings.Get("httpsWarning");
			string targetUrl = "https://" + host + (port == 443 ? "" : (":" + port.ToString())) + Request.Url.PathAndQuery;
			if (httpsWarning != null)
				Response.Redirect(httpsWarning.AddQueryParameter("returnurl", HttpUtility.UrlEncode(targetUrl)), true);
			else
				Response.Redirect(targetUrl, true);
		}

        public bool RenderMargin { get; set; }

        protected object viewData = null;
        
        public object ViewData
        {
            get { return viewData; }
        }

        public virtual void SetViewData(object viewData)
        {
            this.viewData = viewData;
        }

        public string enc(string str)
        {
			return HttpUtility.HtmlEncode(str ?? "");
        }

		public string tv(string str)
		{
			return String.IsNullOrEmpty(str) ? "&nbsp;" : str;
		}

		public void Alert(string text)
		{
			Page.ClientScript.RegisterStartupScript(typeof(Page), "alertmsg", "<script>alert('" + text + "')</script>");
		}

		public void SetTitle(string title)
		{
			AppWeb.Title = title;
			HttpContext.Current.Items["title"] = title;
		}


		Paging paging = null;
		public Paging GetPaging()
		{
			if (paging == null) paging = new Paging();
			return paging;
		}

		[Obsolete]
		public int PageCount { get { return 0; } }
		[Obsolete]
		public int PageIndex { get { return GetPaging().PageIndex; } }
		[Obsolete]
		public string RenderPager(int PageCount)
		{
			return GetPaging().Render();
		}
		public string RenderPager()
		{
			return GetPaging().Render();
		}
		public IQueryable<T> ApplyPaging<T>(IQueryable<T> tquery)
		{
			return GetPaging().ApplyPaging<T>(tquery);
		}


		public virtual Toolbar GetToolbar()
		{
			return AppWeb.MasterControl.GetToolbar();
		}

		//public virtual ButtonBar GetButtonBar()
		//{
		//	return AppWeb.MasterControl.GetButtonBar();
		//}

	
		

        /*public void MessageBox(string title, string text)
        {
            MessageBox mb = new MessageBox();
            mb.ID = "msgbox";
            mb.Title = title;
            MessageBoxText lc = new MessageBoxText("<div style='text-align:center' width='100%'>" + HttpUtility.HtmlEncode(text) + "</div");
            mb.ContentTemplate = lc;
            this.Controls.Add(mb);
            mb.Show();
        }
        public void MessageBox(string text)
        {
            MessageBox("Внимание", text);
        }*/




		public string QFind
		{
			get
			{
				return QuickFilter.SearchQuery;
			}
		}

		Dictionary<int, Sorter> sorters = null;
		public Sorter GetSorter(int seqNo = 0)
		{
			if (sorters == null) sorters = new Dictionary<int, Sorter>();
			Sorter s = null;
			if (!sorters.ContainsKey(seqNo))
			{
				s = new Sorter();
				sorters.Add(seqNo, s);
			}
			else
				s = sorters[seqNo];

			s.UsePostBack = false;
			if (seqNo > 0) s.ParameterName = "sort" + seqNo.ToString();
			return s;
		}

		//string DefaultSortColumn = String.Empty;
		/*public virtual string SortColumn
		{
			get
			{
				string sort = Page.Request.QueryString["sort"] ?? DefaultSortColumn;
				return sort.ToLower().Replace("_desc", "");
			}
		}
		public bool SortAsc
		{
			get
			{
				string sort = Page.Request.QueryString["sort"] ?? String.Empty;
				return !sort.ToLower().EndsWith("_desc");
			}
		}*/
        //Dictionary<int, ArrayList> sortColumns = new Dictionary<int, ArrayList>();

		public string AddSortColumn<T, TColumn>(int listNum, string title, Expression<Func<T, TColumn>> column)
		{
			return GetSorter(listNum).AddSortColumn(title, column, true);
		}
		public string AddSortColumn<T, TColumn>(int listNum, string title, Expression<Func<T, TColumn>> column, bool showArrows)
        {
			return GetSorter(listNum).AddSortColumn(title, column, showArrows);
        }
		public string AddSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column)
		{
			return GetSorter().AddSortColumn(title, column, true);
		}
		public string AddSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column, bool showArrows)
        {
			return GetSorter().AddSortColumn(title, column, showArrows);
        }
		public string AddSortColumn<T, TColumn>(MetaProperty prop, bool showArrows = true)
		{
			return GetSorter().AddSortColumn<T, TColumn>(prop, showArrows);
		}
		/*public string AddDefaultSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column)
        {
            if (!sortColumns.ContainsKey(1))
                sortColumns.Add(1, new ArrayList());
            DefaultSortColumn = (sortColumns[1].Count + 1).ToString();
            return AddSortColumn<T, TColumn>(title, column);
        }*/

        public IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query)
        {
            return GetSorter().ApplyOrderBy(query);
        }
        public IQueryable<T> ApplyOrderBy<T>(int listNum, IQueryable<T> query)
        {
			return GetSorter(listNum).ApplyOrderBy(query);
        }

		/*public string RenderSort(string column, string text, bool showArrows = true)
        {
			return RenderSort("sort", column, text, showArrows);
        }
		public string RenderSort(string param, string column, string text, bool showArrows = true)
		{
			string result = String.Empty;
			column = column.ToLower();
			var baseUrl = Url.Current.SetQuickSearchQuery().SetParameter(param, column);
            if (Query.GetString(param).Replace("_desc", "") == column && !Query.GetString(param).ToLower().EndsWith("_desc"))
			{
				baseUrl = baseUrl.SetParameter(param, column + "_desc");
				result += @" <a href=""" + baseUrl + @""" title=""" + Properties.Resources.SortDesc + @""">" + text + @"</a> " + (showArrows ? @"<img src=""" + Settings.ImagesPath + @"up.gif"" style=""border:0; vertical-align:middle;"" />" : "");
			}
            else if (Query.GetString(param).Replace("_desc", "") == column && Query.GetString(param).ToLower().EndsWith("_desc"))
			{
				result += @" <a href=""" + baseUrl + @""" title=""" + Properties.Resources.SortAsc + @""">" + text + @"</a> " + (showArrows ? @"<img src=""" + Settings.ImagesPath + @"down.gif"" style=""border:0; vertical-align:middle;"" />" : "");
			}
			else
			{
				result += @" <a href=""" + baseUrl + @""" title=""" + Properties.Resources.SortAsc + @""">" + text + @"</a>";
			}
			return result;
		}*/

       


        public override void RenderControl(HtmlTextWriter writer)
        {
            if (RenderMargin)
            {
				writer.Write(AppLayout.Current.AutoMargin.MarginBegin());
            }
            base.RenderControl(writer);
			
            if (RenderMargin)
            {
				writer.Write(AppLayout.Current.AutoMargin.MarginEnd());
            }
        }

		public AppLayout Layout
		{
			get { return AppLayout.Current; }
		}
    }

	[FileLevelControlBuilder(typeof(CustomViewUserControlControlBuilder))]
    public abstract class ViewControl<TViewData> : ViewControl
    {
		public ViewControl()
		{
			viewData = default(TViewData);
		}
		
        public new TViewData ViewData
        {
            get { return (TViewData)viewData; }
        }

        public override void SetViewData(object viewData)
        {
            this.viewData = (TViewData)viewData;
		}

		#region Регистрация контролов
		List<Action> actionsRead = new List<Action>();
		List<Action> actionsStore = new List<Action>();
		void setProperty(Expression<Func<TViewData, string>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, int>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, int?>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, decimal>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, decimal?>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, char>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, char?>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, bool>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, DateTime>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, DateTime?>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		void setProperty(Expression<Func<TViewData, IModelObject>> selector, object value)
		{
			viewData.GetType().GetProperty(((MemberExpression)selector.Body).Member.Name).SetValue(viewData, value, null);
		}

		public void Register(TextBox control, Expression<Func<TViewData, string>> selector)
		{
			actionsRead.Add(() =>
			{
				string data = selector.Compile().Invoke(ViewData) ?? "";
				if (control.MaxLength > 0 && data.Length > control.MaxLength)
					data = data.Substring(0, control.MaxLength);
				control.Text = data;
			});
			actionsStore.Add(() => { setProperty(selector, control.Text); });
		}

		public void Register(TextBox control, Expression<Func<TViewData, int>> selector)
		{
			actionsRead.Add(() => { control.Text = selector.Compile().Invoke(ViewData).ToString(); });
			actionsStore.Add(() => { setProperty(selector, control.Text.ToInt32(0)); });
		}
		
		public void Register(TextBox control, Expression<Func<TViewData, int?>> selector)
		{
			actionsRead.Add(() => { control.Text = selector.Compile().Invoke(ViewData).ToString(); });
			actionsStore.Add(() => { setProperty(selector, control.Text.ToInt32()); });
		}

		public void Register(TextBox control, Expression<Func<TViewData, decimal>> selector)
		{
			actionsRead.Add(() => { control.Text = selector.Compile().Invoke(ViewData).ToString(); });
			actionsStore.Add(() => { setProperty(selector, control.Text.ToDecimal(0)); });
		}

		public void Register(TextBox control, Expression<Func<TViewData, decimal?>> selector)
		{
			actionsRead.Add(() => { control.Text = selector.Compile().Invoke(ViewData).ToString(); });
			actionsStore.Add(() => { setProperty(selector, control.Text.ToDecimal()); });
		}

		public void Register(DropDownList control, Expression<Func<TViewData, char>> selector)
		{
			actionsRead.Add(() => { control.SetValue(selector.Compile().Invoke(ViewData)); });
			actionsStore.Add(() => { if (control.SelectedValue != "") setProperty(selector, control.SelectedValue[0]); });
		}

		public void Register(DropDownList control, Expression<Func<TViewData, char?>> selector)
		{
			actionsRead.Add(() => { control.SetValue(selector.Compile().Invoke(ViewData)); });
			actionsStore.Add(() => { setProperty(selector, control.SelectedValue == "" ? (char?)null : control.SelectedValue[0]); });
		}

		public void Register(DropDownList control, Expression<Func<TViewData, int>> selector)
		{
			actionsRead.Add(() => { control.SetValue(selector.Compile().Invoke(ViewData)); });
			actionsStore.Add(() => { setProperty(selector, control.SelectedValue.ToInt32(0)); });
		}

		public void Register(DropDownList control, Expression<Func<TViewData, int?>> selector)
		{
			actionsRead.Add(() => { control.SetValue(selector.Compile().Invoke(ViewData)); });
			actionsStore.Add(() => { setProperty(selector, control.SelectedValue.ToInt32()); });
		}

		public void Register(CheckBox control, Expression<Func<TViewData, bool>> selector)
		{
			actionsRead.Add(() => { control.Checked = selector.Compile().Invoke(ViewData); });
			actionsStore.Add(() => { setProperty(selector, control.Checked); });
		}

		public void Register(JSCalendar control, Expression<Func<TViewData, DateTime>> selector)
		{
			actionsRead.Add(() => { control.Date = selector.Compile().Invoke(ViewData); });
			actionsStore.Add(() => { if (control.Date.HasValue) setProperty(selector, control.Date.Value); });
		}

		public void Register(JSCalendar control, Expression<Func<TViewData, DateTime?>> selector)
		{
			actionsRead.Add(() => { control.Date = selector.Compile().Invoke(ViewData); });
			actionsStore.Add(() => { setProperty(selector, control.Date); });
		}

		public void Register(DateLists control, Expression<Func<TViewData, DateTime>> selector)
		{
			actionsRead.Add(() => { control.Date = selector.Compile().Invoke(ViewData); });
			actionsStore.Add(() => { if (control.Date.HasValue) setProperty(selector, control.Date.Value); });
		}

		public void Register(DateLists control, Expression<Func<TViewData, DateTime?>> selector)
		{
			actionsRead.Add(() => { control.Date = selector.Compile().Invoke(ViewData); });
			actionsStore.Add(() => { setProperty(selector, control.Date); });
		}

		public void Register(SingleObject control, Expression<Func<TViewData, IModelObject>> selector)
		{
			actionsRead.Add(() => { control.SetObject(selector.Compile().Invoke(ViewData)); });
			actionsStore.Add(() => { setProperty(selector, control.GetObject<IModelObject>()); });
		}
		#endregion

		#region Работа с контролами
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (Request.HttpMethod == "GET")
				LoadData();

			if (Request.HttpMethod == "POST")
				SaveData();
		}

		/// <summary>
		/// Загрузить данные в контролы из объекта ViewData
		/// </summary>
		public virtual void LoadData()
		{
			foreach (var a in actionsRead)
				a();
		}

		/// <summary>
		/// Сохранить данные из контролов в объект ViewData
		/// </summary>
		public virtual void SaveData()
		{
			foreach (var a in actionsStore)
				a();
		}
		#endregion
	}
}
