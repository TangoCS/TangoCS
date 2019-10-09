using Tango.Html;

namespace Tango.UI
{
	public static class TooltipExtensions
	{
		public static void Tooltip(this LayoutWriter w, string id, string text, string tooltip)
		{
			if (tooltip.IsEmpty()) return;

			w.Span(a => a.ID(id), text);
			tooltip = tooltip.Replace("\n", "<br/>");
			//w.Includes.Add("tipso.js");
			w.AddClientAction(
				"$", f => "#" + f(id), 
				("tipso", f => new { content = tooltip, delay = 0, maxWidth = 400 })
			);
		}
	}
}
