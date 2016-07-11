using System;
using Tango.Html;

namespace Tango.UI
{
	public static class ToolbarExtensions
	{
		public static void Toolbar(this LayoutWriter w, Action<ToolbarContentWriter> leftPart, Action<ToolbarContentWriter> rightPart = null)
		{
			w.Div(a => a.Class("ms-menutoolbar"), () =>
			{
				w.Ul(a => a.Class("ms-toolbar-left"), () => leftPart(new ToolbarContentWriter(w)));
				if (rightPart != null)
					w.Ul(a => a.Class("ms-toolbar-right"), () => rightPart(new ToolbarContentWriter(w)));
			});
		}
	}

	public class ToolbarContentWriter
	{
		bool _addSeparator = false;
		LayoutWriter _w;

		public LayoutWriter Writer { get { return _w; } }

		public ToolbarContentWriter(LayoutWriter w)
		{
			_w = w;
		}

		void Separator()
		{
			_w.Li(() => _w.Div(a => a.Class("ms-separator")));
		}

		public void Item(string content)
		{
			if (_addSeparator && !content.IsEmpty())
				Separator();
			_w.Li(() => _w.Write(content));
		}

		public void Item(Action content)
		{
			if (_addSeparator)
				Separator();
            _w.Li(content);
		}

		public void ItemSeparator()
		{
			_addSeparator = true;
		}
	}
}