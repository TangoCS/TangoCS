using Tango.Html;

namespace Tango.UI.Std
{
	public static class ValidationFormatter
	{
		public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val)
		{
			w.Fieldset(a => a.Style("margin-bottom:8px"), () => {
				w.Ul(a => a.Class("savewarning"), () => {
					foreach (var item in val)
						w.Li(() => w.Write(item.Message));
				});
			});
			return w;
		}
	}
}
