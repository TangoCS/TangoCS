using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Linq.Expressions;


namespace Nephrite.Web.Controls
{
    public partial class Sorter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        string RenderCustomSort(int column, string text)
        {
            string result = String.Empty;
            
            if (ColumnNo == column && SortAsc)
            {
				result += @" <a onclick=""" + ClientID + "_sort('" + column + @"_desc'); return false;"" href=""/"" title=""" + Resources.Common.SortDesc + @""">" + text + @"</a> <img src=""" + Settings.ImagesPath + @"down.gif"" style=""border:0; vertical-align:middle;"" />";
            }
            else if (ColumnNo == column && !SortAsc)
            {
				result += @" <a onclick=""" + ClientID + "_sort('" + column + @"'); return false;"" href=""/"" title=""" + Resources.Common.SortAsc + @""">" + text + @"</a> <img src=""" + Settings.ImagesPath + @"up.gif"" style=""border:0; vertical-align:middle;"" />";
            }
            else
            {
                result += @" <a onclick=""" + ClientID + "_sort('" + column + @"'); return false;"" href=""/"" title=""" + Resources.Common.SortAsc + @""">" + text + @"</a>";
            }
            return result;
        }

        class SortColumn
        {
            public string Title { get; set; }
            public int SeqNo { get; set; }
            public string Name { get; set; }            
        }

        class SortColumn<T> : SortColumn
        {
            public Func<IQueryable<T>, IQueryable<T>> OrderAsc { get; set; }
            public Func<IQueryable<T>, IQueryable<T>> OrderDesc { get; set; }
            public Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> OrderAscOE { get; set; }
            public Func<IOrderedEnumerable<T>, IOrderedEnumerable<T>> OrderDescOE { get; set; }
        }

        ArrayList sortColumns = new ArrayList();
		public string AddSortColumn<T, TColumn>(string title, Expression<Func<T, TColumn>> column)
		{
			return RenderSort<T, TColumn>(title, column);
		}
        public string RenderSort<T, TColumn>(string title, Expression<Func<T, TColumn>> column)
        {
            SortColumn<T> sc = new SortColumn<T>
            {
                Title = title,
                SeqNo = sortColumns.Count + 1,
                OrderAsc = q => q.OrderBy<T, TColumn>(column),
                OrderDesc = q => q.OrderByDescending<T, TColumn>(column),
                OrderAscOE = q => q.OrderBy<T, TColumn>(column.Compile()),
                OrderDescOE = q => q.OrderByDescending<T, TColumn>(column.Compile()),
                Name = ""
            };
            sortColumns.Add(sc);
            return RenderCustomSort(sc.SeqNo, title);
        }

        public string RenderSort(string title, string name)
        {
            SortColumn sc = new SortColumn
            {
                Title = title,
                SeqNo = sortColumns.Count + 1,
                Name = name
            };
            sortColumns.Add(sc);
            return RenderCustomSort(sc.SeqNo, title);
        }

        public bool SortAsc
        {
            get { return !hfSort.Value.ToLower().EndsWith("_desc"); }
        }

        public string ColumnName
        {
            get { return ((SortColumn)sortColumns[ColumnNo - 1]).Name; }
        }

        int ColumnNo
        {
            get
            {
                return hfSort.Value.ToLower().Replace("_desc", "").ToInt32(0);
            }
        }

        public IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query)
        {
            if (query == null)
                return null;
            if (ColumnNo < 1 || ColumnNo > sortColumns.Count)
                return query;
            if (SortAsc)
                return ((SortColumn<T>)sortColumns[ColumnNo - 1]).OrderAsc(query);
            else
                return ((SortColumn<T>)sortColumns[ColumnNo - 1]).OrderDesc(query);
        }

        public IOrderedEnumerable<T> ApplyOrderBy<T>(IOrderedEnumerable<T> query)
        {
            if (query == null)
                return null;
            if (ColumnNo < 1 || ColumnNo > sortColumns.Count)
                return query;
            if (SortAsc)
                return ((SortColumn<T>)sortColumns[ColumnNo - 1]).OrderAscOE(query);
            else
                return ((SortColumn<T>)sortColumns[ColumnNo - 1]).OrderDescOE(query);
        }
    }
}