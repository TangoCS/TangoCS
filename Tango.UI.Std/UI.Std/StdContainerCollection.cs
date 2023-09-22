using System;
using Tango.Html;

namespace Tango.UI.Std
{
	public static class ContentHeadersConfig
	{
		public static Action<LayoutWriter> Default { get; set; } = ContentHeaders.DefaultWithCrambAndHelp;
		public static Action<LayoutWriter> Nested { get; set; } = ContentHeaders.Nested;
	}

	public static class ContentHeaders
	{
		public static void DefaultWithCrambAndHelp(LayoutWriter w)
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

		public static void DefaultSimple(LayoutWriter w)
		{
			w.H2(a => a.ID("contenttitle"), "");
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
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeadersConfig.Default;

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataIsContainer(Type, w.IDPrefix), () => {
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
				w.Div(a => a.ID("content").Class(ContainerClass).DataIsContainer(Type, w.IDPrefix), () => {
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding"));
				});
			});
		}
	}

	public class EditEntityContainer : AbstractDefaultContainer
	{
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeadersConfig.Default;

		public bool AddDataCtrl { get; set; }

		public bool ShowResultBlock { get; set; } = false;

		protected void FormContainer(LayoutWriter w)
		{
			var cls = "editform";
			if (Width != ContainerWidth.Undefined) cls += " " + Width.ToString().ToLower();
			if (GridMode) cls += " grid60";
			w.AjaxForm("form", a => {
				a.Class(cls).DataResultPostponed(1);
				if (AddDataCtrl)
					a.DataCtrl(w.IDPrefix);
			}, null);
		}

		protected void ResultBlock(LayoutWriter w)
		{
			if (ShowResultBlock)
				w.Div(a => a.ID("result").Style(Height == ContainerHeight.Height100 ? "overflow-y: auto;" : null));
		}


		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataIsContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding").Style(Height == ContainerHeight.Height100 ? "display:flex;flex-direction:column;" : null), () => {
						FormContainer(w);
						ResultBlock(w);
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
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeadersConfig.Default;

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataIsContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					if (!ToRemove.Contains("contenttoolbar"))
						w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class(BodyClass).Class("contentbodypadding").Style(Height == ContainerHeight.Height100 ? "display:flex;flex-direction:column;" : null), () => {
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
		public Action<LayoutWriter> ContentHeader { get; set; } = ContentHeadersConfig.Default;

		public Unit LeftWidth { get; set; } = new Unit(75, UnitType.Percentage);
		public Unit RightWidth { get; set; } = new Unit(25, UnitType.Percentage);

		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").Class(ContainerClass).DataIsContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							ContentHeader(w);
						});
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.Class("masterdetailcols"), () => {
						w.Div(a => a.ID("contentbody").Class(BodyClass).Style($"width:{LeftWidth}"));
						w.Div(a => a.ID("detail").Style($"width:{RightWidth}"));
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

	public class DefaultPopupContainer : ViewContainer
	{
		public DefaultPopupContainer()
		{
			Mapping.Add("contentbody", "body");
		}
		public override void Render(ApiResponse response)
		{
			response.AddWidget(Context.Sender, w => {
				w.Div(a => a.ID("body"));
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
