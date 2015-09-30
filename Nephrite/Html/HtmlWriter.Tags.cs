using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nephrite.Html
{
	public static class HtmlWriterTagsExtensions
	{
		public static void A(this IHtmlWriter w, Action<ATagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("a");
			if (attributes != null) attributes(new ATagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void B(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("b");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		
		public static void Br(this IHtmlWriter w, Action<TagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("br");
			if (attributes != null) attributes(new TagAttributes(tb));
			tb.Render(w);
		}
		public static void Canvas(this IHtmlWriter w, Action<CanvasTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("canvas");
			if (attributes != null) attributes(new CanvasTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Col(this IHtmlWriter w, Action<ColTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("col");
			if (attributes != null) attributes(new ColTagAttributes(tb));
			tb.Render(w);
		}
		public static void Colgroup(this IHtmlWriter w, Action<ColTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("colgroup");
			if (attributes != null) attributes(new ColTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Dd(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dd");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Div(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("div");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Dl(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dl");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Dt(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dt");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Em(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("em");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Fieldset(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("fieldset");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Form(this IHtmlWriter w, Action<FormTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("form");
			if (attributes != null) attributes(new FormTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H1(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h1");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H2(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h2");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H3(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h3");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H4(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h4");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H5(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h5");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void H6(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h6");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void I(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("i");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Img(this IHtmlWriter w, Action<ImgTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("img");
			if (attributes != null) attributes(new ImgTagAttributes(tb));
			tb.Render(w, TagRenderMode.SelfClosing);
		}
		public static void Label(this IHtmlWriter w, Action<LabelTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("label");
			if (attributes != null) attributes(new LabelTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Li(this IHtmlWriter w, Action<LiTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("li");
			if (attributes != null) attributes(new LiTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Ol(this IHtmlWriter w, Action<OlTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("ol");
			if (attributes != null) attributes(new OlTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void P(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("p");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Pre(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("pre");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Q(this IHtmlWriter w, Action<QTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("q");
			if (attributes != null) attributes(new QTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void S(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("s");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Small(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("small");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Span(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("span");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Sub(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("sub");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Sup(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("sup");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Table(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("table");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Tbody(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("tbody");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Td(this IHtmlWriter w, Action<TdTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("td");
			if (attributes != null) attributes(new TdTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Th(this IHtmlWriter w, Action<ThTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("th");
			if (attributes != null) attributes(new ThTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Thead(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("thead");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		
		public static void Tr(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("tr");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void U(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("u");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}
		public static void Ul(this IHtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("ul");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void A(this IHtmlWriter w, Action inner) { w.A(null, inner); }
		public static void B(this IHtmlWriter w, Action inner) { w.B(null, inner); }		
		public static void Br(this IHtmlWriter w) { w.Br(null); }
		public static void Canvas(this IHtmlWriter w, Action inner) { w.Canvas(null, inner); }
		public static void Col(this IHtmlWriter w) { w.Col(null); }
		public static void Colgroup(this IHtmlWriter w, Action inner) { w.Colgroup(null, inner); }
		public static void Dd(this IHtmlWriter w, Action inner) { w.Dd(null, inner); }
		public static void Div(this IHtmlWriter w, Action inner) { w.Div(null, inner); }
		public static void Dl(this IHtmlWriter w, Action inner) { w.Dl(null, inner); }
		public static void Dt(this IHtmlWriter w, Action inner) { w.Dt(null, inner); }
		public static void Em(this IHtmlWriter w, Action inner) { w.Em(null, inner); }
		public static void Fieldset(this IHtmlWriter w, Action inner) { w.Fieldset(null, inner); }
		public static void Form(this IHtmlWriter w, Action inner) { w.Form(null, inner); }
		public static void H1(this IHtmlWriter w, Action inner) { w.H1(null, inner); }
		public static void H2(this IHtmlWriter w, Action inner) { w.H2(null, inner); }
		public static void H3(this IHtmlWriter w, Action inner) { w.H3(null, inner); }
		public static void H4(this IHtmlWriter w, Action inner) { w.H4(null, inner); }
		public static void H5(this IHtmlWriter w, Action inner) { w.H5(null, inner); }
		public static void H6(this IHtmlWriter w, Action inner) { w.H6(null, inner); }
		public static void I(this IHtmlWriter w, Action inner) { w.I(null, inner); }
		public static void Img(this IHtmlWriter w) { w.Img(null); }
		public static void Label(this IHtmlWriter w, Action inner) { w.Label(null, inner); }
		public static void Li(this IHtmlWriter w, Action inner) { w.Li(null, inner); }
		public static void Ol(this IHtmlWriter w, Action inner) { w.Ol(null, inner); }
		public static void P(this IHtmlWriter w, Action inner) { w.P(null, inner); }
		public static void Pre(this IHtmlWriter w, Action inner) { w.Pre(null, inner); }
		public static void Q(this IHtmlWriter w, Action inner) { w.Q(null, inner); }
		public static void S(this IHtmlWriter w, Action inner) { w.S(null, inner); }
		public static void Small(this IHtmlWriter w, Action inner) { w.Small(null, inner); }
		public static void Span(this IHtmlWriter w, Action inner) { w.Span(null, inner); }
		public static void Sub(this IHtmlWriter w, Action inner) { w.Sub(null, inner); }
		public static void Sup(this IHtmlWriter w, Action inner) { w.Sup(null, inner); }
		public static void Table(this IHtmlWriter w, Action inner) { w.Table(null, inner); }
		public static void Tbody(this IHtmlWriter w, Action inner) { w.Tbody(null, inner); }
		public static void Td(this IHtmlWriter w, Action inner) { w.Td(null, inner); }
		public static void Th(this IHtmlWriter w, Action inner) { w.Th(null, inner); }
		public static void Thead(this IHtmlWriter w, Action inner) { w.Thead(null, inner); }
		
		public static void Tr(this IHtmlWriter w, Action inner) { w.Tr(null, inner); }
		public static void U(this IHtmlWriter w, Action inner) { w.U(null, inner); }
		public static void Ul(this IHtmlWriter w, Action inner) { w.Ul(null, inner); }

		public static void A(this IHtmlWriter w, Action<ATagAttributes> attributes, string linkTitle) { w.A(attributes, () => w.Write(linkTitle)); }
		public static void A(this IHtmlWriter w, string linkTitle) { w.A(null, () => w.Write(linkTitle)); }
		public static void B(this IHtmlWriter w, string text) { w.B(null, () => w.Write(text)); }
		public static void Label(this IHtmlWriter w, string labelFor, string lableTitle)
		{
			w.Label(a => a.For(labelFor), () => w.Write(lableTitle));
		}
		public static void Span(this IHtmlWriter w, string text) { w.Span(null, () => w.Write(text)); }
		public static void Span(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.Span(attributes, () => w.Write(text)); }
		public static void H1(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H1(attributes, () => w.Write(text)); }
		public static void H2(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H2(attributes, () => w.Write(text)); }
		public static void H3(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H3(attributes, () => w.Write(text)); }
		public static void H4(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H4(attributes, () => w.Write(text)); }
		public static void H5(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H5(attributes, () => w.Write(text)); }
		public static void H6(this IHtmlWriter w, Action<TagAttributes> attributes, string text) { w.H6(attributes, () => w.Write(text)); }
	}


	public interface IHtmlPageWriter : IHtmlWriter { }

	public static class HtmlPageWriterExtensions
	{
		public static void DocType(this IHtmlPageWriter w)
		{
			w.Write("<!DOCTYPE html>");
		}

		public static void Body(this IHtmlPageWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("body");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void Head(this IHtmlPageWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("head");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void Html(this IHtmlPageWriter w, Action<HtmlTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("html");
			if (attributes != null) attributes(new HtmlTagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void Link(this IHtmlPageWriter w, Action<LinkTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("link");
			if (attributes != null) attributes(new LinkTagAttributes(tb));
			tb.Render(w);
		}

		public static void Meta(this IHtmlPageWriter w, Action<MetaTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("meta");
			if (attributes != null) attributes(new MetaTagAttributes(tb));
			tb.Render(w);
		}

		public static void Title(this IHtmlPageWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("title");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				tb.Render(w, TagRenderMode.StartTag);
				inner();
				tb.Render(w, TagRenderMode.EndTag);
			}
			else
				tb.Render(w);
		}

		public static void Body(this IHtmlPageWriter w, Action inner) { w.Body(null, inner); }
		public static void Head(this IHtmlPageWriter w, Action inner) { w.Head(null, inner); }
		public static void Html(this IHtmlPageWriter w, Action inner) { w.Html(null, inner); }
		public static void Title(this IHtmlPageWriter w, Action inner) { w.Title(null, inner); }
	}
}