using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
	public class Paging : System.Web.UI.Control
	{
		LinkButton btn = new LinkButton();
		HiddenField hf = new HiddenField();
		public Paging()
		{
			PageIndex = Query.GetInt("page", 1);
			if (PageIndex <= 0)
				PageIndex = 1;
			PageSize = Paging.DefaultPageSize;
			Visible = false;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			btn.ID = "btn" + ClientID;
			Controls.Add(btn);
			btn.Click += new EventHandler(btn_Click);
			hf.ID = "hf" + ClientID;
			Controls.Add(hf);

			Page.ClientScript.RegisterClientScriptBlock(GetType(), "paging_" + ClientID, String.Format(@"function Paging{0}_gotoPage(page) {{
	document.getElementById('{1}').value = page;
    {2};
}}", ClientID, hf.ClientID, Page.ClientScript.GetPostBackEventReference(btn, "")), true);
		}

		void btn_Click(object sender, EventArgs e)
		{
			PageIndex = hf.Value.ToInt32(1);
			OnPage(e);
		}

		protected internal virtual void OnPage(EventArgs e)
		{
			if (Paged != null)
			{
				Paged(this, e);
			}
		}

		/// <summary>
		/// Доп. обработчик для UsePostBack = true
		/// </summary>
		public event EventHandler Paged;

		/// <summary>
		/// Номер текущей страницы, начиная с 1
		/// </summary>
		public int PageIndex { get; set; }

		/// <summary>
		/// Количество элементов на странице
		/// </summary>
		public int PageSize { get; set; }

		public static int DefaultPageSize 
		{
			get 
			{
				if (HttpContext.Current.Items["DefaultPageSize"] == null)
					return 50;
				return Convert.ToInt32(HttpContext.Current.Items["DefaultPageSize"]); 
			}
			set { HttpContext.Current.Items["DefaultPageSize"] = value; }
		}

		int itemsCount;
		public int ItemsCount { get { return itemsCount; } }

		public void SetItemsCount(int count)
		{
			itemsCount = count;
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			if (ViewState[ClientID + "_PageIndex"] != null)
				PageIndex = (int)ViewState[ClientID + "_PageIndex"];
		}

		protected override object SaveViewState()
		{
			ViewState[ClientID + "_PageIndex"] = PageIndex;
			return base.SaveViewState();
		}

		public IQueryable<T> ApplyPaging<T>(IQueryable<T> tquery)
		{
			itemsCount = tquery.Count();
			if (PageSize == 0)
				return tquery;
			if (PageIndex == 0) PageIndex = 1;
			
			int pageCount = (int)Math.Ceiling((float)itemsCount / (float)PageSize);

			if (PageIndex > pageCount && pageCount > 0)
				PageIndex = pageCount;

			if (pageCount > 0)
				return tquery = tquery.Skip<T>((PageIndex - 1) * PageSize).Take<T>(PageSize);

			return tquery;
		}

		public IEnumerable<T> ApplyPaging<T>(IEnumerable<T> tquery)
		{
			itemsCount = tquery.Count();
			if (PageSize == 0)
				return tquery;
			if (PageIndex == 0) PageIndex = 1;
			
			int pageCount = (int)Math.Ceiling((float)itemsCount / (float)PageSize);

			if (PageIndex > pageCount && pageCount > 0)
				PageIndex = pageCount;

			if (pageCount > 0)
				return tquery = tquery.Skip<T>((PageIndex - 1) * PageSize).Take<T>(PageSize);

			return tquery;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			writer.Write(Render());
		}

		public bool UsePostBack { get; set; }

		public string Render()
		{
			int pageCount = (int)Math.Ceiling((float)itemsCount / (float)PageSize);
			if (!UsePostBack)
			{
				// Рендер обычного пейджинга, если родитель не UpdatePanel
				return AppWeb.Layout.RenderPager(QuickFilter.SetSearchQuery(), PageIndex, pageCount, ItemsCount);
			}
			else
			{
				return AppWeb.Layout.RenderPager("Paging" + ClientID + "_gotoPage", PageIndex, pageCount, ItemsCount);
			}
		}
	}
}
