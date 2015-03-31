using System;
using System.Collections.Generic;
using System.IO;

namespace Nephrite.Html
{
	public partial class HtmlWriter
	{
		public void DocType()
		{
			WriteLine("<!DOCTYPE html>");
		}

		public void A(Action<ATagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("a");
			if (attributes != null) attributes(new ATagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}

		public void B(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("b");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Body(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("body");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Br(Action<TagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("br");
			if (attributes != null) attributes(new TagAttributes(tb));
			Write(tb);
		}
		public void Canvas(Action<CanvasTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("canvas");
			if (attributes != null) attributes(new CanvasTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Col(Action<ColTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("col");
			if (attributes != null) attributes(new ColTagAttributes(tb));
			Write(tb);
		}
		public void Colgroup(Action<ColTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("colgroup");
			if (attributes != null) attributes(new ColTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Dd(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dd");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Div(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("div");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Dl(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dl");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Dt(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("dt");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Em(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("em");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Fieldset(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("fieldset");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Form(Action<FormTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("form");
			if (attributes != null) attributes(new FormTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H1(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h1");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H2(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h2");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H3(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h3");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H4(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h4");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H5(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h5");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void H6(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("h6");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Head(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("head");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Html(Action<HtmlTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("html");
			if (attributes != null) attributes(new HtmlTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void I(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("i");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Img(Action<ImgTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("body");
			if (attributes != null) attributes(new ImgTagAttributes(tb));
			Write(tb);
		}
		public void Label(Action<LabelTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("label");
			if (attributes != null) attributes(new LabelTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Li(Action<LiTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("li");
			if (attributes != null) attributes(new LiTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Link(Action<LinkTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("link");
			if (attributes != null) attributes(new LinkTagAttributes(tb));
			Write(tb);
		}
		public void Meta(Action<MetaTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("meta");
			if (attributes != null) attributes(new MetaTagAttributes(tb));
			Write(tb);
		}
		public void Ol(Action<OlTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("ol");
			if (attributes != null) attributes(new OlTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void P(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("p");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Pre(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("pre");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Q(Action<QTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("q");
			if (attributes != null) attributes(new QTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void S(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("s");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Small(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("small");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Span(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("span");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Sub(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("sub");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Sup(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("sup");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Table(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("table");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Tbody(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("tbody");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Td(Action<TdTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("td");
			if (attributes != null) attributes(new TdTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Th(Action<ThTagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("th");
			if (attributes != null) attributes(new ThTagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Thead(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("thead");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Title(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("title");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Tr(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("tr");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void U(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("u");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}
		public void Ul(Action<TagAttributes> attributes = null, Action inner = null)
		{
			TagBuilder tb = new TagBuilder("ul");
			if (attributes != null) attributes(new TagAttributes(tb));
			if (inner != null)
			{
				Write(tb.Render(TagRenderMode.StartTag));
				inner();
				Write(tb.Render(TagRenderMode.EndTag));
			}
			else
				Write(tb);
		}


		public void A(Action inner) { A(null, inner); }
		public void B(Action inner) { B(null, inner); }
		public void Body(Action inner) { Body(null, inner); }
		public void Br() { Br(null); }
		public void Canvas(Action inner) { Canvas(null, inner); }
		public void Col() { Col(null); }
		public void Colgroup(Action inner) { Colgroup(null, inner); }
		public void Dd(Action inner) { Dd(null, inner); }
		public void Div(Action inner) { Div(null, inner); }
		public void Dl(Action inner) { Dl(null, inner); }
		public void Dt(Action inner) { Dt(null, inner); }
		public void Em(Action inner) { Em(null, inner); }
		public void Fieldset(Action inner) { Fieldset(null, inner); }
		public void Form(Action inner) { Form(null, inner); }
		public void H1(Action inner) { H1(null, inner); }
		public void H2(Action inner) { H2(null, inner); }
		public void H3(Action inner) { H3(null, inner); }
		public void H4(Action inner) { H4(null, inner); }
		public void H5(Action inner) { H5(null, inner); }
		public void H6(Action inner) { H6(null, inner); }
		public void Head(Action inner) { Head(null, inner); }
		public void Html(Action inner) { Html(null, inner); }
		public void I(Action inner) { I(null, inner); }
		public void Img() { Img(null); }
		public void Label(Action inner) { Label(null, inner); }
		public void Li(Action inner) { Li(null, inner); }
		public void Link(Action inner) { Li(null, inner); }
		public void Meta() { Meta(null); }
		public void Ol(Action inner) { Ol(null, inner); }
		public void P(Action inner) { P(null, inner); }
		public void Pre(Action inner) { Pre(null, inner); }
		public void Q(Action inner) { Q(null, inner); }
		public void S(Action inner) { S(null, inner); }
		public void Small(Action inner) { Small(null, inner); }
		public void Span(Action inner) { Span(null, inner); }
		public void Sub(Action inner) { Sub(null, inner); }
		public void Sup(Action inner) { Sup(null, inner); }
		public void Table(Action inner) { Table(null, inner); }
		public void Tbody(Action inner) { Tbody(null, inner); }
		public void Td(Action inner) { Td(null, inner); }
		public void Th(Action inner) { Th(null, inner); }
		public void Thead(Action inner) { Thead(null, inner); }
		public void Title(Action inner) { Title(null, inner); }
		public void Tr(Action inner) { Tr(null, inner); }
		public void U(Action inner) { U(null, inner); }
		public void Ul(Action inner) { Ul(null, inner); }
	}
}