using Tango.Html;

namespace Tango.UI.Std
{
	public class DefaultContainer : ViewContainer
	{
        public override void Render(ApiResponse response)
		{
			//response.AddAdjacentWidget("#container", "content", AdjacentHTMLPosition.BeforeEnd, w => {
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
					{
                        w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
                            w.Div(a => a.ID("cramb"), "&nbsp;");
                            w.Table(() => {
                                w.Tr(() => {
                                    w.Td(() => w.H2(a => a.ID("contenttitle"), ""));
                                    w.Td(a => a.Style("vertical-align:top"), () => w.Div(a => a.ID("contenthelp"), ""));
                                });
                            });
                        });
					}
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody"));
				});
			});
		}
	}

	public class DefaultNoHeaderContainer : ViewContainer
	{
		public override void Render(ApiResponse response)
		{
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").DataContainer(Type, w.IDPrefix), () => {
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class("contentbodypadding"));
				});
			});
		}
	}

	public class EditEntityContainer : ViewContainer
	{
		public ContainerWidth Width { get; set; } = ContainerWidth.WidthStd;
		public bool AddDataCtrl { get; set; }
		public override void Render(ApiResponse response)
		{
			//response.AddAdjacentWidget("#container", "content", AdjacentHTMLPosition.BeforeEnd, w => {
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
					{
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							w.Div(a => a.ID("cramb"), "&nbsp;");
                            w.Table(() => {
                                w.Tr(() => {
                                    w.Td(() => w.H2(a => a.ID("contenttitle"), ""));
                                    w.Td(a => a.Style("vertical-align:top"), () => w.Div(a => a.ID("contenthelp"), ""));
                                });
                            });
                        });
					}
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class("contentbodypadding"), () => {
					w.AjaxForm("form", a => {
						a.Class(Width.ToString().ToLower()).DataResultPostponed(1);
						if (AddDataCtrl)
						{
							a.DataCtrl(w.IDPrefix);
						}
					}, null);
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

	public class ViewEntityContainer : ViewContainer
	{
		public ContainerWidth Width { get; set; } = ContainerWidth.WidthStd;

		public override void Render(ApiResponse response)
		{
			//response.AddAdjacentWidget("#container", "content", AdjacentHTMLPosition.BeforeEnd, w => {
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
					{
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							w.Div(a => a.ID("cramb"), "&nbsp;");
                            w.Table(() => {
                                w.Tr(() => {
                                    w.Td(() => w.H2(a => a.ID("contenttitle"), ""));
                                    w.Td(a => a.Style("vertical-align:top"), () => w.Div(a => a.ID("contenthelp"), ""));
                                });
                            });
                        });
					}
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.ID("contentbody").Class("contentbodypadding"), () => {
						w.Div(a => a.ID("form").Class(Width.ToString().ToLower()));
					});
				});
			});
		}
	}

	public class ListMasterDetailContainer : ViewContainer
	{
		public override void Render(ApiResponse response)
		{
			//response.AddAdjacentWidget("container", "content", AdjacentHTMLPosition.BeforeEnd, w => {
			response.AddWidget("container", w => {
				w.Div(a => a.ID("content").DataContainer(Type, w.IDPrefix), () => {
					if (!ToRemove.Contains("contentheader"))
					{
						w.Div(a => a.ID("contentheader").Class("contentheader"), () => {
							w.Div(a => a.ID("cramb"), "&nbsp;");
                            w.Table(() => {
                                w.Tr(() => {
                                    w.Td(() => w.H2(a => a.ID("contenttitle"), ""));
                                    w.Td(a => a.Style("vertical-align:top"), () => w.Div(a => a.ID("contenthelp"), ""));
                                });
                            });
                        });
					}
					w.Div(a => a.ID("contenttoolbar"));
					w.Div(a => a.Class("twocolumnsrow masterdetailcols"), () => {
						w.Div(a => a.ID("contentbody"));
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
		Width100
	}
}
