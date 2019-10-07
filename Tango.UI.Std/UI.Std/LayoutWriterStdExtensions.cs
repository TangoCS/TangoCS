using Tango.Html;

namespace Tango.UI.Std
{
	public static class LayoutWriterStdExtensions
	{
		public static void FormValidationBlock(this LayoutWriter w)
		{
			w.Div(a => a.ID("validation"));
		}

		public static void LastModifiedBlock<TUser>(this LayoutWriter w, IWithTimeStamp<TUser> obj)
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

		public static void TimeStampExBlock<TUser>(this LayoutWriter w, IWithTimeStampEx<TUser> obj)
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
	}
}
