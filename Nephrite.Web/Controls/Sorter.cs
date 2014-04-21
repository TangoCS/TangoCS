using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Linq.Expressions;
using Nephrite.Web;
using Nephrite.Web.SettingsManager;
using Nephrite.Meta;


namespace Nephrite.Web.Controls
{
	public partial class Sorter : System.Web.UI.Control
	{
		LinkButton btn = new LinkButton();
		HiddenField hf = new HiddenField();

		public string ParameterName { get; set; }

		public Sorter()
		{
			UsePostBack = true;
			ParameterName = "sort";
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (UsePostBack)
			{
				btn.ID = "btnSort";
				Controls.Add(btn);
				btn.Click += new EventHandler(lbSort_Click);
				hf.ID = "hfSort";
				Controls.Add(hf);

				Page.ClientScript.RegisterClientScriptBlock(GetType(), "sorter_" + ClientID, String.Format(@"function {0}_sort(column) {{
	document.getElementById('{1}').value = column;
    {2};
}}", ClientID, hf.ClientID, Page.ClientScript.GetPostBackEventReference(btn, "")), true);
			}
		}

		protected void lbSort_Click(object sender, EventArgs e)
		{
			OnSort(e);
		}

		protected internal virtual void OnSort(EventArgs e)
		{
			if (Sort != null)
				Sort(this, e);
		}

		public event EventHandler Sort;

		string RenderSortPostBack(int column, string text, bool showArrows)
		{
			string iup = !showArrows ? "" : String.Format("<img src='{0}up.gif' style='border:0; vertical-align:middle;' />", Settings.ImagesPath);
			string idown = !showArrows ? "" : String.Format("<img src='{0}down.gif' style='border:0; vertical-align:middle;' />", Settings.ImagesPath); 

			if (_orderByColumns == null) _orderByColumns = hf.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (_orderByColumns != null && _orderByColumns.Contains(column.ToString()))
			{
				return String.Format("<a onclick=\"{0}_sort('{1}_desc');\" href='#sort={1}_desc' title='{2}'>{3}</a> {4}", ClientID, column, Resources.Common.SortDesc, text, idown);
			}
			else if (_orderByColumns != null && _orderByColumns.Contains(column.ToString() + "_desc"))
			{
				return String.Format("<a onclick=\"{0}_sort('{1}');\" href='#sort={1}' title='{2}'>{3}</a> {4}", ClientID, column, Resources.Common.SortAsc, text, iup);
			}
			else
			{
				return String.Format("<a onclick=\"{0}_sort('{1}');\" href='#sort={1}' title='{2}'>{3}</a>", ClientID, column, Resources.Common.SortAsc, text);
			}
		}

		string RenderSortLink(int column, string text, bool showArrows)
		{
			string iup = !showArrows ? "" : String.Format("<img src='{0}up.gif' style='border:0; vertical-align:middle;' />", Settings.ImagesPath);
			string idown = !showArrows ? "" : String.Format("<img src='{0}down.gif' style='border:0; vertical-align:middle;' />", Settings.ImagesPath); 

			Url baseUrl = Url.Current.SetQuickSearchQuery();
			string curSettings = baseUrl.GetString(ParameterName).ToLower();

			if (curSettings == column.ToString())
			{
				baseUrl = baseUrl.SetParameter(ParameterName, column.ToString() + "_desc");
				return String.Format("<a href='{0}' title='{1}'>{2}</a> {3}", baseUrl, Properties.Resources.SortDesc, text, iup);
			}
			else if (curSettings == column.ToString() + "_desc")
			{
				baseUrl = baseUrl.SetParameter(ParameterName, column.ToString());
				return String.Format("<a href='{0}' title='{1}'>{2}</a> {3}", baseUrl, Properties.Resources.SortAsc, text, idown);
			}
			else
			{
				baseUrl = baseUrl.SetParameter(ParameterName, column.ToString());
				return String.Format("<a href='{0}' title='{1}'>{2}</a>", baseUrl, Properties.Resources.SortAsc, text);
			}
			
		}

		public class SortColumn<T>
		{
			public string Title { get; set; }
			//public int SeqNo { get; set; }

			public Func<IQueryable<T>, IQueryable<T>> OrderAsc { get; set; }
			public Func<IQueryable<T>, IQueryable<T>> OrderDesc { get; set; }
			public Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> OrderAscOE { get; set; }
			public Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> OrderDescOE { get; set; }
		}

		Dictionary<int, object> sortColumns = new Dictionary<int, object>();


		public string AddSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column, bool showArrows = true)
		{
			return AddSortColumn(title, column, sortColumns.Count);
		}
		public string AddSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column, int seqNo, bool showArrows = true)
		{
			SortColumn<T> sc = new SortColumn<T>
			{
				Title = title,
				//SeqNo = seqNo,
				OrderAsc = q => q.OrderBy<T, TColumn>(column),
				OrderDesc = q => q.OrderByDescending<T, TColumn>(column),
				OrderAscOE = q => q.OrderBy<T, TColumn>(column.Compile()),
				OrderDescOE = q => q.OrderByDescending<T, TColumn>(column.Compile())
			};
			sortColumns.Add(seqNo, sc);
			if (UsePostBack)
				return RenderSortPostBack(seqNo, title, showArrows);
			else
				return RenderSortLink(seqNo, title, showArrows);
		}

		public string AddSortColumn<T, TColumn>(MetaProperty prop, bool showArrows = true)
		{
			int seqNo = sortColumns.Count;
			SortColumn<T> sc = new SortColumn<T>
			{
				Title = prop.CaptionShort,
				//SeqNo = seqNo,
				OrderAsc = q => q.OrderBy<T, TColumn>(prop.GetValueExpression as Expression<Func<T, TColumn>>),
				OrderDesc = q => q.OrderByDescending<T, TColumn>(prop.GetValueExpression as Expression<Func<T, TColumn>>),
				OrderAscOE = q => q.OrderBy<T, TColumn>(prop.GetValue as Func<T, TColumn>),
				OrderDescOE = q => q.OrderByDescending<T, TColumn>(prop.GetValue as Func<T, TColumn>)
			};
			sortColumns.Add(seqNo, sc);
			if (UsePostBack)
				return RenderSortPostBack(seqNo, prop.CaptionShort, showArrows);
			else
				return RenderSortLink(seqNo, prop.CaptionShort, showArrows);
		}

		string[] _orderByColumns = null;
		public void SetSortColumns(string columns)
		{
			if (columns.IsEmpty())
			{
				hf.Value = "";
				_orderByColumns = null;
			}
			else if (hf.Value.IsEmpty() && _orderByColumns == null && !columns.IsEmpty())
			{
				hf.Value = columns;
				_orderByColumns = hf.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		public SortColumn<T> GetSortColumn<T>(int seqNo)
		{
			return sortColumns[seqNo] as SortColumn<T>;
		}

		public IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query)
		{
			if (query == null)
				return null;

			IQueryable<T> res = query;
			if (!UsePostBack)
			{
				string s = Url.Current.GetString(ParameterName);
				if (!s.IsEmpty())
					_orderByColumns = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}
			if (_orderByColumns == null) return res;

			for (int i = _orderByColumns.Count() - 1; i >= 0; i--)
			{
				string[] s = _orderByColumns[i].Split(new char[] { '_' });
				if (s.Length == 1)
					res = ((SortColumn<T>)sortColumns[s[0].ToInt32(0)]).OrderAsc(res);
				else
					res = ((SortColumn<T>)sortColumns[s[0].ToInt32(0)]).OrderDesc(res);
			}
			return res;
		}

		public IOrderedEnumerable<T> ApplyOrderBy<T>(IOrderedEnumerable<T> query)
		{
			if (query == null)
				return null;

			IOrderedEnumerable<T> res = query;
			if (!UsePostBack)
			{
				string s = Url.Current.GetString(ParameterName);
				if (!s.IsEmpty())
					_orderByColumns = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}
			if (_orderByColumns == null) return res;
			for (int i = _orderByColumns.Count() - 1; i >= 0; i--)
			{
				string[] s = _orderByColumns[i].Split(new char[] { '_' });
				if (s.Length == 1)
					res = ((SortColumn<T>)sortColumns[s[0].ToInt32(0)]).OrderAscOE(res);
				else
					res = ((SortColumn<T>)sortColumns[s[0].ToInt32(0)]).OrderDescOE(res);
			}
			return res;
		}

		public bool UsePostBack { get; set; }
	}
}