using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tango.Html
{
	public static class HtmlWriterSvgTagsExtensions
	{
		public static void Svg(this HtmlWriter w, Action<SvgTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("svg", attributes, inner);
		}
		public static void Rect(this HtmlWriter w, Action<RectTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("rect", attributes, inner);
		}
		public static void Title(this HtmlWriter w, string title)
		{
			w.WriteTag<ATagAttributes>("title", a => { }, () => w.Write(title));
		}
	}

	public class SvgTagAttributes : TagAttributes<SvgTagAttributes>
	{
		public SvgTagAttributes Height(int value) { Writer.WriteAttr("height", value.ToString()); return this; }
		public SvgTagAttributes Width(int value) { Writer.WriteAttr("width", value.ToString()); return this; }
		public SvgTagAttributes ViewBox(int minx, int miny, int width, int height) { Writer.WriteAttr("viewBox", $"{minx} {miny} {width} {height}"); return this; }
		public SvgTagAttributes X(int value) { Writer.WriteAttr("x", value.ToString()); return this; }
		public SvgTagAttributes Y(int value) { Writer.WriteAttr("y", value.ToString()); return this; }
		public SvgTagAttributes PreserveAspectRatio(PreserveAspectRatio value, MeetSlice meetSlice)
		{
			Writer.WriteAttr("preserveAspectRatio", $"{value} {meetSlice}"); return this;
		}
	}

	public class RectTagAttributes : TagAttributes<RectTagAttributes>
	{
		public RectTagAttributes Height(int value) { Writer.WriteAttr("height", value.ToString()); return this; }
		public RectTagAttributes Width(int value) { Writer.WriteAttr("width", value.ToString()); return this; }
		public RectTagAttributes X(int value) { Writer.WriteAttr("x", value.ToString()); return this; }
		public RectTagAttributes Y(int value) { Writer.WriteAttr("y", value.ToString()); return this; }
		public RectTagAttributes Rx(int value) { Writer.WriteAttr("rx", value.ToString()); return this; }
		public RectTagAttributes Ry(int value) { Writer.WriteAttr("ry", value.ToString()); return this; }
		public RectTagAttributes PathLength(int value) { Writer.WriteAttr("pathLength", value.ToString()); return this; }
	}

	public enum PreserveAspectRatio { none, xMinYMin, xMidYMin, xMaxYMin, xMinYMid, xMidYMid, xMaxYMid, xMinYMax, xMidYMax, xMaxYMax }
	public enum MeetSlice { meet, slice }
}