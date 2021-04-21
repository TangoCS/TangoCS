using System;
using Tango.Html;

namespace Tango.UI.Std
{
	public static class ContentHeaders
	{
		public static void Default(LayoutWriter w)
		{
			IHelpManager manager = w.Context.RequestServices.GetService(typeof(IHelpManager)) as IHelpManager;

			w.Div(a => a.ID("cramb").Class("cramb"));
			w.Table(() => {
				w.Tr(() => {
					w.Td(() => w.H2(a => a.ID("contenttitle"), ""));
					w.Td(a => a.Style("vertical-align:top"), () => w.Div(() => manager?.Render(w)));
				});
			});
		}

		public static void Nested(LayoutWriter w)
		{
			w.H3(a => a.ID("contenttitle"), "");
		}
	}

	public abstract class AbstractDefaultContainer : ViewContainer
	{
		public ContainerWidth Width { get; set; } = ContainerWidth.WidthStd;
		public ContainerHeight Height { get; set; } = ContainerHeight.HeightStd;

		public bool GridMode { get; set; }

		public string ContainerClass { get; set; } = "content";
		public string BodyClass { get; set; } = "contentbody";
	}

	public class DefaultContainer : AbstractDefaultContainer
	{
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeaders.Default;

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass));
				});
			});
		}
	}

	public class DefaultNoHeaderContainer : AbstractDefaultContainer
	{
		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataContainer(Type, w.IDPrefix), () => {
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding"));
				});
			});
		}
	}

	public class EditEntityContainer : AbstractDefaultContainer
	{
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeaders.Default;

		public bool AddDataCtrl { get; set; }

		public bool ShowResultBlock { get; set; } = false;
		
		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding").Style(Height == ContainerHeight.Height100 ? "display:flex;flex-direction:column;" : null), () => {
						var cls = "editform";
						if (Width != ContainerWidth.Undefined) cls += " " + Width.ToString().ToLower();
						if (GridMode) cls += " grid60";
						w.AjaxForm("form", a => {
							a.Class(cls).DataResultPostponed(1);
							if (AddDataCtrl)
								a.DataCtrl(w.IDPrefix);
						}, null);

						if (ShowResultBlock)
							w.Div(a => a.ID("result").Style(Height == ContainerHeight.Height100 ? "overflow-y: auto;" : null));
					});
				});
			});
		}
	}

	public class EmptyContainer : ViewContainer
	{
		public override void Render(ApiResponse response)
		{
		}
	}

	public class ViewEntityContainer : AbstractDefaultContainer
	{
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeaders.Default;

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding"), () => {
						var cls = "viewform";
						if (Width != ContainerWidth.Undefined) cls += " " + Width.ToString().ToLower();
						if (GridMode) cls += " grid60";
						w.Div(a => a.ID("form").Class(cls));
					});
				});
			});
		}
	}

	public class ListMasterDetailContainer : AbstractDefaultContainer
	{
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeaders.Default;

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.Class("twocolumnsrow masterdetailcols"), () => {
						w.Div(a => a.ID("contentbody").Class(BodyClass));
						w.Div(a => a.ID("detail"));
					});
				});
			});
		}
	}

	public class ChildFormContainer : ViewContainer
	{
		public ChildFormContainer()
		{
			Mapping.Add("buttonsbar", "buttonsbar");
			Mapping.Add("form", "form");
		}
		public override void Render(ApiResponse response)
		{
			response.AddWidget(Context.Sender, w => {
				w.AjaxForm("form", a => a.DataResultPostponed(1).Class("contentbodypadding"), () => {
					w.Div(a => a.ID("buttonsbar"));
				});
			});
		}
	}


	public enum ContainerWidth
	{
		WidthStd,
		WidthStd2x,
		Width100,
		Undefined
	}

	public enum ContainerHeight
	{
		HeightStd,
		Height100
	}
}
