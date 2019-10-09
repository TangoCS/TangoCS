using Tango.Html;

namespace Tango.UI.Std
{
	public static class ValidationFormatter
	{
		public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val)
		{
			//w.Div(a => a.Class("savewarning"), () => {
				//w.Write(w.Resources.Get("Common.Warnings.SpecifyValues") + ":");
				w.Ul(a => a.Class("savewarning"), () => {
					foreach (var item in val)
						w.Li(() => w.Write(item.Message));
				});
			//});
			return w;
		}
	}
}
