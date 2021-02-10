using System;
using Tango.Html;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public static class LayoutWriterStdExtensions
	{
		public static void FormValidationBlock(this LayoutWriter w)
		{
			w.Div(a => a.ID("validation"));
		}

		public static void LastModifiedBlock<TUser>(this LayoutWriter w, IWithUserTimeStamp<TUser> obj)
			where TUser : IWithTitle
		{
			w.Div(a => a.Class("lastmodified"), () => {
				w.Write(w.Resources.Get("Common.LastModified"));
				w.Write(": ");
				w.Write(obj.LastModifiedDate.DateTimeToString());
				w.Write(" ");
				w.Write(obj.LastModifiedUser.Title);
			});
		}

		public static void TimeStampExBlock<TUser>(this LayoutWriter w, IWithUserTimeStampEx<TUser> obj)
			where TUser : IWithTitle
		{
			w.Div(a => a.Class("lastmodified"), () => {
				w.Write(w.Resources.Get("Common.Created"));
				w.Write(": ");
				w.Write(obj.CreateDate.DateTimeToString());
				w.Write(" ");
				w.Write(obj.Creator?.Title);
				w.Write(w.NewLine);
				w.Write(w.Resources.Get("Common.LastModified"));
				w.Write(": ");
				w.Write(obj.LastModifiedDate.DateTimeToString());
				w.Write(" ");
				w.Write(obj.LastModifiedUser?.Title);
			});
		}

        public static void TimeStampExBlock(this LayoutWriter w, IWithUserTimeStampEx obj)
        {
            w.Div(a => a.Class("lastmodified"), () => {
                w.Write(w.Resources.Get("Common.Created"));
                w.Write(": ");
                w.Write(obj.CreateDate.DateTimeToString());
                w.Write(" ");
                w.Write(obj.Creator);
                w.Write(w.NewLine);
                w.Write(w.Resources.Get("Common.LastModified"));
                w.Write(": ");
                w.Write(obj.LastModifiedDate.DateTimeToString());
                w.Write(" ");
                w.Write(obj.LastModifiedUser);
            });
        }

        public static void ButtonsBar_view(this LayoutWriter w)
		{
			w.ButtonsBar(() => {
				w.ButtonsBarRight(() => w.BackButton());
			});
		}

		public static void ButtonsBar_edit(this LayoutWriter w)
		{
			w.ButtonsBar(() => {
				w.ButtonsBarRight(() => {
					w.SubmitButton();
					w.BackButton();
				});
			});
		}

		public static void CollapsibleSidebar(this LayoutWriter w, string title, Action content)
		{
			w.Div(a => a.ID("sidebar").Class("sidebar").DataCtrl("sidebar"), () => {
				w.Div(a => a.Class("sidebar-menu"), () => {
					w.Ul(() => {
						w.Li(() => w.Span(a => a.ID("sidebartabtitle"), title));
					});
				});
				w.Div(a => a.Class("sidebar-panel"), () => {
					w.Div(a => a.Class("sidebar-header"), () => {
						w.H3(a => a.ID("sidebarcontenttitle"), title);
						w.Div(a => a.Class("sidebar-close"), () => w.Icon("begin"));
					});
					content();
				});
			});
		}

		public static void CollapsibleSidebar(this LayoutWriter w, Tabs tabs)
		{
			w.Div(a => a.ID("sidebar").Class("sidebar").DataCtrl("sidebar"), () => {
				w.Div(a => a.Class("sidebar-menu"), () => {
					w.Ul(() => {
						w.Li(() => w.Span(tabs.Pages[0].Title));
					});
				});
				w.Div(a => a.Class("sidebar-panel"), () => {
					w.Div(a => a.Class("sidebar-header"), () => {
						tabs.RenderTabs(w);
						//w.H3(title);
						w.Div(a => a.Class("sidebar-close"), () => w.Icon("begin"));
					});
					tabs.RenderPages(w);
					//content();
				});
			});
		}
	}

	public enum CollapsibleSidebarPosition
	{
		Left,
		Right
	}
}
