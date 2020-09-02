using System;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class Paging : ViewComponent
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public string ParameterName { get; set; }

		public Paging()
		{
			PageSize = 50;
		}

		public override void OnInit()
		{
			if (ParameterName.IsEmpty())
				ParameterName = ClientID;
		}
	}

	public static class PagingExtensions
	{
		public static IQueryable<T> Apply<T>(this Paging paging, IQueryable<T> query, bool plusOneRow = false)
		{
			if (paging.PageIndex > 1)
				return query.Skip((paging.PageIndex - 1) * paging.PageSize).Take(paging.PageSize + (plusOneRow ? 1 : 0));
			else
				return query.Take(paging.PageSize + (plusOneRow ? 1 : 0));
		}

		public static void Render(this Paging paging, LayoutWriter w, int? itemsCount, Action<ActionLink> pageActionAttributes)
		{
			w.Span(a => a.ID(paging.ID).Class("paging"), () => {
				var r = paging.Resources;
				var pageCount = 1;
				var pageIdx = paging.PageIndex;
				var pname = paging.ParameterName;

				if (itemsCount > paging.PageSize) pageCount = (int)Math.Ceiling((float)itemsCount / (float)paging.PageSize);
				if (itemsCount <= paging.PageSize) pageCount = 1;
				if (pageIdx > pageCount) pageIdx = pageCount;
				if (pageIdx == 0) pageIdx = 1;

				void attrs(ActionLink a) => a.ToCurrent().Set(pageActionAttributes);
				var dc = paging.ParentElement.DataCollection;

				if (pageIdx > 2)
					w.ActionImage(a => a.Set(attrs).WithArg(pname, 1).WithImage("begin"), a => a.Data(dc));

				if (pageIdx > 1)
					w.ActionImage(a => a.Set(attrs).WithArg(pname, pageIdx - 1).WithImage("left"), a => a.ID("pgup").Data(dc));

				if (itemsCount == null || pageCount - pageIdx >= 1)
					w.ActionImage(a => a.Set(attrs).WithArg(pname, pageIdx + 1).WithImage("right"), a => a.ID("pgdown").Data(dc));

				if (pageCount > 1 && pageCount - pageIdx >= 2)
					w.ActionImage(a => a.Set(attrs).WithArg(pname, pageCount).WithImage("end"), a => a.Data(dc));

				w.B(r.Get("Common.Paging.TotalRecords") + ": ");
				if (itemsCount.HasValue)
					w.Write(itemsCount.Value.ToString());
				else
					w.ActionLink(a => a.Set(attrs).WithTitle("?").PostEvent("getobjcount"), a => a.Class("cnt"));
			});
		}

		public static void Render2(this Paging paging, LayoutWriter w, int? itemsCount, Action<ActionLink> pageActionAttributes, Action<ActionLink> objCountActionAttributes)
		{
			var res = paging.Resources;
			var pageCount = 1;
			var pageIdx = paging.PageIndex;
			var pname = paging.ParameterName;

			if (itemsCount > paging.PageSize) pageCount = (int)Math.Ceiling((float)itemsCount / (float)paging.PageSize);
			if (itemsCount <= paging.PageSize) pageCount = 1;
			if (pageIdx > pageCount) pageIdx = pageCount;
			if (pageIdx == 0) pageIdx = 1;

			w.Span(a => a.ID(paging.ID).Class("paging2"), () => {
				if (itemsCount == 0) return;

				w.Span(a => a.ID(paging.ID + "_cnt").Class(itemsCount > paging.PageSize ? "int" : ""), () => {
					w.B(((pageIdx - 1) * paging.PageSize + 1).ToString());
					w.Write("&ndash;");
					w.B((itemsCount != null && pageIdx * paging.PageSize > itemsCount ? itemsCount : pageIdx * paging.PageSize).ToString());
				});

                var dc = paging.ParentElement.DataCollection;

                w.Write($"&nbsp;{res.Get("Common.Paging.From")}&nbsp;");
				w.B(() => {
                    if (itemsCount.HasValue)
                        w.Write(itemsCount.Value.ToString());
                    else
                        w.ActionLink(a => a.ToCurrent().Set(objCountActionAttributes).WithTitle("?"), a => a.Class("cnt").Data(dc));
				});
				w.Write("&nbsp;");

                if (pageIdx > 1)
                    w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx - 1).WithImage("left"), a => a.Data(dc));

				if (itemsCount == null || pageCount - pageIdx >= 1)
					w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx + 1).WithImage("right"), a => a.Data(dc));

                if (itemsCount.HasValue && ((pageIdx > 2) || (pageCount > 1 && pageCount - pageIdx >= 2)))
				{
					w.DropDownForElement(paging.ID + "_cnt", () => {
						if (pageIdx > 2)
							w.ActionLink(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, 1).WithTitle(r => r.Get("Common.Paging.First")), a => a.Data(dc).DataContainerExternal(paging.ParentElement.ClientID));
                        if (pageCount > 1 && pageCount - pageIdx >= 2)
							w.ActionLink(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageCount).WithTitle(r => r.Get("Common.Paging.Last")),a => a.Data(dc).DataContainerExternal(paging.ParentElement.ClientID));
					});
				}
			});
		}
	}
}
