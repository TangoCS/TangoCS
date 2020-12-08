using System;
using Tango.Html;

namespace Tango.UI.Std
{
	public class ExpandableRowOptions<TResult>
	{
		public RowCellFlagDelegate<TResult> Collapsed { get; set; } = (o, i) => true;
		public RowCellAttributesDelegate<TResult> Attributes { get; set; }
		public RowCellFlagDelegate<TResult> ContentVisible { get; set; } = (o, i) => true;
		public Action<ThTagAttributes> HeaderAttributes { get; set; }
		public string Tip { get; set; }
	}

	public static class ExpandableRowExtensions
	{
        public static void AddRowExpanderColumn<TResult>(this IFieldCollection<TResult> f, Action<ApiResponse> e, ExpandableRowOptions<TResult> options = null)
        {
			if (options == null)
				options = new ExpandableRowOptions<TResult>();

			f.AddCustomCell(
				new ColumnHeader {
					Attributes = options.HeaderAttributes,
					Content = w => w.Write("")
				},
				new ListColumn<TResult> {
					Attributes = (a, o, i) => {
						a.Class("rowexpandercell").OnClickExpandRow(null, e);
						if (!options.Collapsed(o, i)) a.Data("state", "expanded");
						options.Attributes?.Invoke(a, o, i);
					},
					Content = (w, o, i) => {
						if (options.ContentVisible(o, i))
						{
							w.Icon(options.Collapsed(o, i) ? "collapsed" : "expanded", tip: options.Tip);
						}
					}
				}
			);
        }

        public static void AddExpandableRowContent(this ApiResponse response, ActionContext context, Action<LayoutWriter, string, string> content)
		{
			response.AddExpandableRowContent(context, context.GetIntArg("colspan", 0), content);
		}

		public static void AddExpandableRowContent(this ApiResponse response, ActionContext context, int colspan, Action<LayoutWriter, string, string> content)
		{
			var id = context.FormData.Parse<string>("rowid");
			var level = context.FormData.Parse<int>("level");
			var cellid = context.Sender.IsEmpty() ? "" : context.Sender.Substring(1);
			level++;

			response.AddAdjacentWidget($"#{id.ToLower()}", $"#{id}_content".ToLower(), AdjacentHTMLPosition.AfterEnd, w => {			
				w.Tr(a => a.Data("level", level).Data("cellid", cellid), () => {
					w.Td(a => a.Class("expandablerowcontent").ColSpan(colspan), () => {
						var contentPrefix = (cellid.IsEmpty() ? id : cellid) + "_content";
						content(w.Clone(contentPrefix), id, cellid);
					});
				});
				
			});
		}

		public static T OnClickExpandRow<T>(this TagAttributes<T> attr, object cellid, Action<ApiResponse> e)
			where T: TagAttributes<T>
		{
			if (cellid != null)
				attr.ID(cellid.ToString());

			if (e != null)
				attr.DataEvent(e);

			return attr.OnClick($"listview.togglerow(this)");
		}
	}

	public static class TreeRowExtensions
	{
		public static void TreeRow(this LayoutWriter w, int level, Action content)
		{
			w.Div(a => a.Class($"treerow l{level}"), () => {
				for (int i = 0; i < level - 1; i++)
					w.Div(a => a.Class("level-padding"));
				if (level > 0)
					w.Div(a => a.Class("level-padding last"));
				w.Div(a => a.Class("treerowtitle"), content);
			});
		}

		public static void TreeRow(this LayoutWriter w, int level, string content)
		{
			w.TreeRow(level, () => w.Write(content));
		}
	}
}
