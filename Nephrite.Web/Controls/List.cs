using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Web.UI;
using System.Text.RegularExpressions;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class List : System.Web.UI.UserControl
	{
		IListFactory _factory = null;

		public Paging Paging { get { return _paging; } }
		public Sorter Sorter { get { return _sorter; } }

		Paging _paging = new Paging();
		Sorter _sorter = new Sorter();

		protected override void OnInit(EventArgs e)
		{
			_sorter.ID = "sorter";
			_paging.ID = "paging";
			_sorter.UsePostBack = false;
			_paging.UsePostBack = false;
			Controls.Add(_paging);
			Controls.Add(_sorter);
			base.OnInit(e);
		}

		public ListFactory<T> CreateFactory<T>(IQueryable<T> viewData)
		{
			ListFactory<T> f = new ListFactory<T>(this, viewData);
			_factory = f;
			return f;
		}
		public ListFactory<T> CreateFactory<T>()
		{
			ListFactory<T> f = new ListFactory<T>(this, null);
			_factory = f;
			return f;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			if (_factory != null)
				_factory.Render(writer);
			else
				writer.Write("<p>Необходима инициализация</p>");
		}
	}


	public interface IListFactory
	{
		void Render(System.Web.UI.HtmlTextWriter writer);
	}

	public interface IListColumn
	{
		string Name { get; set; }
		string Caption { get; set; }
		bool Visible { get; set; }
		bool Fixed { get; set; }
		int SeqNo { get; set; }
		bool Sortable { get; set; }
	}

	public class ListFactory<T> : IListFactory
	{
		List<ListColumn<T>> _columns = new List<ListColumn<T>>();
		IQueryable<T> _viewData = null;
		List _form = null;
		int _seq = 0;

		public List<ListColumn<T>> Columns { get { return _columns; } }
		public string Group1Sort { get; set; }
		public string Group2Sort { get; set; }
		public int? Group1Column { get; set; }
		public int? Group2Column { get; set; }

		public object TableStyle { get; set; }
		public object HeaderRowStyle { get; set; }
		public Func<T, int, string, string> RowStyle { get; set; }
		public IQueryable<T> ViewData
		{
			get { return _viewData; }
			set { _viewData = value; }
		}

		public ListFactory(List form, IQueryable<T> viewData)
		{
			_form = form;
			_viewData = viewData;
			RowStyle = (o, i, css) => css;
		}

		public void Render(HtmlTextWriter writer)
		{
			writer.Write(AppWeb.Layout.ListTableBegin(TableStyle));
			writer.Write(AppWeb.Layout.ListHeaderBegin(HeaderRowStyle));
			foreach (ListColumn<T> c in _columns)
			{
				if (!c.Visible) continue;
				c.RenderHeader(writer);
			}
			writer.Write(AppWeb.Layout.ListHeaderEnd());

			ListColumn<T> gr1column = null;
			ListColumn<T> gr2column = null;
			string gr1value = "", gr2value = "", gr1curvalue = "", gr2curvalue = "";

			_viewData = _form.Sorter.ApplyOrderBy(_viewData);
			if (Group2Column.HasValue && !Group2Sort.IsEmpty())
			{
				gr2column = Columns[Group2Column.Value];
				if (Group2Sort == "A")
					_viewData = _form.Sorter.GetSortColumn<T>(Group2Column.Value).OrderAsc(_viewData);
				else
					_viewData = _form.Sorter.GetSortColumn<T>(Group2Column.Value).OrderDesc(_viewData);
			}
			if (Group1Column.HasValue && !Group1Sort.IsEmpty())
			{
				gr1column = Columns[Group1Column.Value];
				if (Group1Sort == "A")
					_viewData = _form.Sorter.GetSortColumn<T>(Group1Column.Value).OrderAsc(_viewData);
				else
					_viewData = _form.Sorter.GetSortColumn<T>(Group1Column.Value).OrderDesc(_viewData);
			}


			int i = (_form.Paging.PageIndex - 1) * _form.Paging.PageSize + 1;
			int colscnt = _columns.Count(o => o.Visible);
			HtmlHelperBase.Instance.Repeater(_form.Paging.ApplyPaging(_viewData), (o) =>
			{
				
				if (gr1column != null)
				{
					gr1curvalue = gr1column.Value(o, i);
					if (gr1curvalue.IsEmpty()) gr1curvalue = "<значение не задано>";
					if (gr1curvalue != gr1value)
					{
						writer.Write(AppWeb.Layout.ListRowBegin(""));
						writer.Write(AppWeb.Layout.TD(gr1curvalue, new { style = "font-weight:bold", colspan = colscnt.ToString() }));
						writer.Write(AppWeb.Layout.ListRowEnd());
						gr1value = gr1curvalue;
						gr2value = "";
					}
				}

				if (gr2column != null)
				{
					gr2curvalue = gr2column.Value(o, i);
					if (gr2curvalue.IsEmpty()) gr2curvalue = "<значение не задано>";
					if (gr2curvalue != gr2value)
					{
						writer.Write(AppWeb.Layout.ListRowBegin(""));
						writer.Write(AppWeb.Layout.TD(gr2curvalue, new { style = "font-weight:bold; padding-left: 30px", colspan = colscnt.ToString() }));
						writer.Write(AppWeb.Layout.ListRowEnd());
						gr2value = gr2curvalue;
					}
				}

				writer.Write(AppWeb.Layout.ListRowBegin(RowStyle(o, i, (i % 2 == 0) ? "" : HtmlHelperWSS.CSSClassAlternating)));
				foreach (ListColumn<T> c in _columns)
				{
					if (!c.Visible) continue;

					if (c.CellStyle != null)
						writer.Write(AppWeb.Layout.TDBegin(c.CellStyle(o, i)));
					else
						writer.Write(AppWeb.Layout.TDBegin());
					c.Render(writer, o, i);
					writer.Write(AppWeb.Layout.TDEnd());
				}
				writer.Write(AppWeb.Layout.ListRowEnd());
				i++;
			});

			writer.Write(AppWeb.Layout.ListTableEnd());
			writer.Write(_form.Paging.Render());
		}

		public ListColumn<T> AddColumn(string caption)
		{
			ListColumn<T> res = new ListColumn<T>(Regex.Replace(caption, "<.*?>", ""), caption);
			res.SeqNo = _seq;
			_seq++;
			_columns.Add(res);
			return res;
		}
		public ListColumn<T> AddColumn(string caption, Func<T, int, string> value)
		{
			ListColumn<T> res = new ListColumn<T>(Regex.Replace(caption, "<.*?>", ""), caption);
			res.SeqNo = _seq;
			_seq++;
			res.Value = value;
			_columns.Add(res);
			return res;
		}
		public ListColumn<T> AddColumn<TColumn>(string caption, Expression<Func<T, TColumn>> sortExpression)
		{
			ListColumn<T> res = new ListColumn<T>(Regex.Replace(caption, "<.*?>", ""), _form.Sorter.AddSortColumn<T, TColumn>(caption, sortExpression, _seq));
			res.SeqNo = _seq;
			_seq++;
			res.Sortable = true;
			_columns.Add(res);
			return res;
		}
		public ListColumn<T> AddColumn<TColumn>(string caption, Expression<Func<T, TColumn>> sortExpression, Func<T, int, string> value)
		{
			ListColumn<T> res = new ListColumn<T>(Regex.Replace(caption, "<.*?>", ""), _form.Sorter.AddSortColumn<T, TColumn>(caption, sortExpression, _seq));
			res.SeqNo = _seq;
			_seq++;
			res.Value = value;
			res.Sortable = true;
			_columns.Add(res);
			return res;
		}
		
		public ListColumn<T> AddIndexNumberColumn(string caption)
		{
			IndexNumberListColumn<T> res = new IndexNumberListColumn<T>(Regex.Replace(caption, "<.*?>", ""), caption);
			res.SeqNo = _seq;
			_seq++;
			_columns.Add(res);
			return res;
		}
	}


	public class ListColumn<T> : IListColumn
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public object HeaderStyle { get; set; }
		public Func<T, int, object> CellStyle { get; set; }
		public Func<T, int, string> Value { get; set; }
		public bool Visible { get; set; }
		public bool Fixed { get; set; }
		public int SeqNo { get; set; }
		public bool	Sortable { get; set; }
		
		public ListColumn(string name, string caption)
		{
			Visible = true;
			Fixed = false;
			Caption = caption;
			Name = name;
		}

		public virtual void RenderHeader(HtmlTextWriter writer)
		{
			writer.Write(AppWeb.Layout.TH(Caption, HeaderStyle));
		}

		public virtual void Render(HtmlTextWriter writer, T obj, int index)
		{
			writer.Write(Value(obj, index));
		}
	}

	public class CheckBoxListColumn<T> : ListColumn<T>
	{
		public CheckBoxListColumn(string name, string caption) 
			: base(name, caption)
		{
			Fixed = true;
		}
	}

	public class IndexNumberListColumn<T> : ListColumn<T>
	{
		public IndexNumberListColumn(string name, string caption)
			: base(name, caption)
		{
			Fixed = true;
			Value = (o, i) => i.ToString();
		}
	}
}