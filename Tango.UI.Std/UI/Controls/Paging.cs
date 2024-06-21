using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class Paging : ViewComponent
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public string ParameterName { get; set; }
		public string PageSizeParameterName { get; set; }

		public Paging()
		{
			PageSize = 50;
		}

		public override void OnInit()
		{
			base.OnInit();

			if (ParameterName.IsEmpty()) ParameterName = ClientID;
			if (PageSizeParameterName.IsEmpty()) PageSizeParameterName = ParentElement.GetClientID("psize");

			PageIndex = Context.GetIntArg(ParameterName, 1);
			var size = Context.GetIntArg(PageSizeParameterName);
			if (size != null)
				PageSize = size.Value;
		}
	}

	public interface IPagingRenderer
	{
		void Render(Paging paging, LayoutWriter w, PagingRenderOptions options);
	}

	public interface ISelectObjectPagingRenderer : IPagingRenderer { }
	public interface IListPagingRenderer : IPagingRenderer { }
	public interface ITreePagingRenderer : IPagingRenderer { }

	public class PagingRenderOptions
	{
		public int? ItemsCount { get; set; }
		public Action<ActionLink> PageActionAttributes { get; set; }
		public Action<ActionLink> ObjCountActionAttributes { get; set; }
		public Action<InputTagAttributes> GoToPageActionAttributes { get; set; }
		public Action<SelectTagAttributes> SetPageSizeActionAttributes { get; set; }
	}

	// для окна выбора
	public class PagingRenderer1 : ISelectObjectPagingRenderer
	{
		public void Render(Paging paging, LayoutWriter w, PagingRenderOptions options)
		{
			w.Span(a => a.ID(paging.ID).Class("paging"), () => {
				var r = paging.Resources;
				var pageCount = 1;
				var pageIdx = paging.PageIndex;
				var pname = paging.ParameterName;
				var itemsCount = options.ItemsCount;
				var pageActionAttributes = options.PageActionAttributes;

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
	}

	// для списков по умолчанию
	public class PagingRenderer2 : IListPagingRenderer, ITreePagingRenderer
	{
		public void Render(Paging paging, LayoutWriter w, PagingRenderOptions options)
		{
			var res = paging.Resources;
			var pageCount = 1;
			var pageIdx = paging.PageIndex;
			var pname = paging.ParameterName;
			var itemsCount = options.ItemsCount;
			var pageActionAttributes = options.PageActionAttributes;
			var objCountActionAttributes = options.ObjCountActionAttributes;

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
							w.ActionLink(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageCount).WithTitle(r => r.Get("Common.Paging.Last")), a => a.Data(dc).DataContainerExternal(paging.ParentElement.ClientID));
					});
				}
			});
		}
	}

	// для списков альтернативный
	public class PagingRenderer3 : IListPagingRenderer, ITreePagingRenderer
	{
		public void Render(Paging paging, LayoutWriter w, PagingRenderOptions options)
		{
			var res = paging.Resources;
			var pageCount = 1;
			var pageIdx = paging.PageIndex;
			var pname = paging.ParameterName;
			var itemsCount = options.ItemsCount;
			var pageActionAttributes = options.PageActionAttributes;
			var objCountActionAttributes = options.ObjCountActionAttributes;

			if (itemsCount > paging.PageSize) pageCount = (int)Math.Ceiling((float)itemsCount / (float)paging.PageSize);
			if (itemsCount <= paging.PageSize) pageCount = 1;
			if (pageIdx > pageCount) pageIdx = pageCount;
			if (pageIdx == 0) pageIdx = 1;

			w.Span(a => a.ID(paging.ID).Class("paging2"), () => {
				if (itemsCount == 0) return;

				var from = (pageIdx - 1) * paging.PageSize + 1;
				var to = itemsCount != null && pageIdx * paging.PageSize > itemsCount ? itemsCount : pageIdx * paging.PageSize;

				w.Span(a => a.ID(paging.ID + "_cnt"), () => {
					w.B(from.ToString());
					w.Write($"&nbsp;-&nbsp;");
					w.B(to.ToString());
					w.Write($"&nbsp;из&nbsp;");
					w.B(itemsCount.ToString());
					w.Write($"&nbsp;&nbsp;");
				});

				var dc = paging.ParentElement.DataCollection;

				w.Write("&nbsp;");
				w.DropDownList(new InputName { ID = "psize", Name = paging.ParentElement.GetClientID("psize") }, 
					paging.PageSize.ToString(), new List<SelectListItem> {
					new SelectListItem("10", "10"),
					new SelectListItem("50", "50"),
					new SelectListItem("100", "100"),
					new SelectListItem("1000", "1000")
				}, a => a.Set(options.SetPageSizeActionAttributes).Data(dc));
				w.Write("&nbsp;");
				w.Span(a => a.Style(pageCount < 2 ? "visibility:hidden" : null), () =>
				{
					w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, 1).WithImage("begin").WithTitle("В начало списка"), a => a.Data(dc).Class(pageIdx <= 2 ? "disabled" : ""));
					w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx - 1).WithImage("left").WithTitle("На предыдущий лист"), a => a.Data(dc).Class(pageIdx <= 1 ? "disabled" : ""));

					w.Span("&nbsp;");
					w.TextBox("go", pageIdx.ToString(), a => a.Style("width:50px; text-align:center").Set(options.GoToPageActionAttributes));
					w.Span(a => a.Style("width: 60px; display: inline-block;"), $"&nbsp;/&nbsp;{pageCount}&nbsp;");

					w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx + 1).WithImage("right").WithTitle("На следующий лист"), a => a.Data(dc).Class(itemsCount != null && pageCount - pageIdx < 1 ? "disabled" : ""));
					w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageCount).WithImage("end").WithTitle("На последний лист"), a => a.Data(dc).Class(pageCount <= 1 || pageCount - pageIdx < 2 ? "disabled" : ""));
				});
			});
		}
	}

	// для списков с несколькими страницами
	public class PagingRenderer4 : IListPagingRenderer
	{
		public void Render(Paging paging, LayoutWriter w, PagingRenderOptions options)
		{
			var res = paging.Resources;
			var pageCount = 1;
			var pageIdx = paging.PageIndex;
			var pname = paging.ParameterName;
			var itemsCount = options.ItemsCount;
			var pageActionAttributes = options.PageActionAttributes;
			var objCountActionAttributes = options.ObjCountActionAttributes;

			if (itemsCount > paging.PageSize) pageCount = (int)Math.Ceiling((float)itemsCount / (float)paging.PageSize);
			if (itemsCount <= paging.PageSize) pageCount = 1;
			if (pageIdx > pageCount) pageIdx = pageCount;
			if (pageIdx == 0) pageIdx = 1;

			w.Div(a => a.ID(paging.ID).Class("paging4"), () => {
				if (itemsCount == 0) return;
				

				var dc = paging.ParentElement.DataCollection;

				w.Div(() => {
					w.DropDownList(new InputName { ID = "psize", Name = paging.ParentElement.GetClientID("psize") },
						paging.PageSize.ToString(), new List<SelectListItem> {
					new SelectListItem("10", "10"),
					new SelectListItem("20", "20"),
					new SelectListItem("50", "50"),
					new SelectListItem("100", "100"),
					new SelectListItem("200", "200"),
					}, a => a.Set(options.SetPageSizeActionAttributes).Data(dc));

					w.Span("/");
					w.Span(a => a.ID(paging.ID + "_cnt"), itemsCount.ToString());
				});

				w.Div(() => {
					if (pageCount == 1) return;

					void btn(int destIdx)
					{
						w.ActionTextButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, destIdx)
							.WithTitle(destIdx.ToString()), a => a.Data(dc).Class(destIdx == pageIdx ? "current" : null));
					}
						

					if (pageIdx > 1)
						w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx - 1).WithImage("left"), a => a.Data(dc));

					if (pageIdx >= 4)
						btn(1);
					if (pageIdx > 4)
						w.Span("...");

					if (pageIdx > 4 && pageIdx + 2 >= pageCount)
						btn(pageIdx - 4);
					if (pageIdx > 3 && pageIdx + 1 >= pageCount)
						btn(pageIdx - 3);
					if (pageIdx > 2)
						btn(pageIdx - 2);
					if (pageIdx > 1)
						btn(pageIdx - 1);

					btn(pageIdx);
					
					if (pageCount > 1 && pageIdx + 1 <= pageCount)
						btn(pageIdx + 1);
					if (pageCount > 2 && pageIdx + 2 <= pageCount)
						btn(pageIdx + 2);
					if (pageCount > 3 && pageIdx <= 2)
						btn(pageIdx + 3);
					if (pageCount > 4 && pageIdx == 1)
						btn(pageIdx + 4);

					if (pageCount > 5 && pageCount > pageIdx + 3)
						w.Span("...");
					if (pageCount > 5 && pageCount >= pageIdx + 3)
						btn(pageCount);

					if (pageCount - pageIdx >= 1)
						w.ActionImageButton(a => a.ToCurrent().Set(pageActionAttributes).WithArg(pname, pageIdx + 1).WithImage("right"), a => a.Data(dc));
				});
			});
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

		public static int GetPageCount(this Paging paging, int? itemsCount)
		{
			return (int) Math.Ceiling((float) itemsCount / (float) paging.PageSize);
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
