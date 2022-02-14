using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tango.Html
{
	public static class HtmlWriterHtml5TagsExtensions
	{
		public static void Article(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("article", attributes, inner);
		}

		public static void Aside(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("aside", attributes, inner);
		}

		public static void Details(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("details", attributes, inner);
		}

		public static void Hgroup(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("hgroup", attributes, inner);
		}

		public static void Footer(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("footer", attributes, inner);
		}

		public static void Section(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("section", attributes, inner);
		}

		public static void Summary(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("summary", attributes, inner);
		}

		public static void Bdi(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("bdi", attributes, inner);
		}

		public static void Main(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("main", attributes, inner);
		}

		public static void Mark(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("mark", attributes, inner);
		}

		public static void Output(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("output", attributes, inner);
		}

		public static void Progress(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("progress", attributes, inner);
		}

		public static void Rp(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("rp", attributes, inner);
		}

		public static void Ruby(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("ruby", attributes, inner);
		}

		public static void Wbr(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("wbr", attributes, inner);
		}

		public static void Audio(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("audio", attributes, inner);
		}

		public static void Embed(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("embed", attributes, inner);
		}

		public static void FigCaption(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("figcaption", attributes, inner);
		}

		public static void Figure(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("figure", attributes, inner);
		}

		public static void Source(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("source", attributes, inner);
		}

		public static void Time(this HtmlWriter w, Action<TimeTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("time", attributes, inner);
		}

		public static void Video(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("video", attributes, inner);
		}

		public static void Template(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("template", attributes, inner);
		}
	}

	public class TimeTagAttributes : TagAttributes<TimeTagAttributes>
	{
		public TimeTagAttributes DateTime(DateTime value) { Writer.WriteAttrID("datetime", value.ToString("yyyy-MM-dd HH:mm:ss")); return this; }
	}
}