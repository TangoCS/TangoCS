using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tango.Html
{
	public static class HtmlWriterHelpers
	{
		public static string GetID(string prefix, string id)
		{
			return id.StartsWith("#") ? 
				id.Substring(1).ToLower() : 
				(prefix.IsEmpty() ? id.ToLower() : $"{prefix}_{id.ToLower()}");
		}
	}

	public partial class HtmlWriter
	{
		CultureInfo ru = new CultureInfo("ru-RU");

		void WriteTag(string name, Action inner)
		{
			Write('<');
			Write(name);
			Write('>');
			inner?.Invoke();
			Write("</");
			Write(name);
			Write('>');
		}

		void WriteTag<T>(string name, Action<T> attrs, Action inner)
			where T : class, IContentItemAttributes<T>
		{
			Write('<');
			Write(name);
			var o = Fabric<T>();
			o.SetWriter(this);
			attrs?.Invoke(o);
			RenderAttrs();
			Write('>');
			inner?.Invoke();
			Write("</");
			Write(name);
			Write('>');
		}
	}

	public static class HtmlWriterTagsExtensions
	{
		public static void WriteTag<T>(this HtmlWriter w, string name, Action<T> attrs, Action inner)
			where T : TagAttributes<T>, new()
		{
			w.Write('<');
			w.Write(name);
			if (attrs != null)
			{
				var o = new T();
				o.SetWriter(w);
				attrs?.Invoke(o);
				w.RenderAttrs();
			}
			w.Write('>');
			inner?.Invoke();
			w.Write("</");
			w.Write(name);
			w.Write('>');
		}

		public static void WriteBeginTag<T>(this HtmlWriter w, string name, Action<T> attrs)
			where T : TagAttributes<T>, new()
		{
			w.Write('<');
			w.Write(name);
			if (attrs != null)
			{
				var o = new T();
				o.SetWriter(w);
				attrs?.Invoke(o);
				w.RenderAttrs();
			}
			w.Write('>');
		}

		public static void WriteEndTag(this HtmlWriter w, string name)
		{
			w.Write("</");
			w.Write(name);
			w.Write('>');
		}

		public static void WriteSelfClosingTag<T>(this HtmlWriter w, string name, Action<T> attrs)
			where T : TagAttributes<T>, new()
		{
			w.Write('<');
			w.Write(name);
			if (attrs != null)
			{
				var o = new T();
				o.SetWriter(w);
				attrs?.Invoke(o);
				w.RenderAttrs();
			}
			w.Write("/>");
		}

		public static string GetID(this HtmlWriter w, string id) => HtmlWriterHelpers.GetID(w.IDPrefix, id);

		public static void A(this HtmlWriter w, Action<ATagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("a", attributes, inner);
		}
		public static void B(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("b", attributes, inner);
		}		
		public static void Br(this HtmlWriter w, Action<TagAttributes> attributes = null)
		{
			w.WriteSelfClosingTag("br", attributes);
		}
		public static void Canvas(this HtmlWriter w, Action<CanvasTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("canvas", attributes, inner);
		}
		public static void Col(this HtmlWriter w, Action<ColTagAttributes> attributes = null)
		{
			w.WriteSelfClosingTag("col", attributes);
		}
		public static void Colgroup(this HtmlWriter w, Action<ColTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("colgroup", attributes, inner);
		}
		public static void Dd(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("dd", attributes, inner);
		}
		public static void Div(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("div", attributes, inner);
		}
		public static void DivBegin(this HtmlWriter w, Action<TagAttributes> attributes = null)
		{
			w.WriteBeginTag("div", attributes);
		}
		public static void DivEnd(this HtmlWriter w)
		{
			w.WriteEndTag("div");
		}
		public static void Dl(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("dl", attributes, inner);
		}
		public static void Dt(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("dt", attributes, inner);
		}
		public static void Em(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("em", attributes, inner);
		}
		public static void Fieldset(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("fieldset", attributes, inner);
		}
		public static void Form(this HtmlWriter w, Action<FormTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("form", attributes, inner);
		}
		public static void H1(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h1", attributes, inner);
		}
		public static void H2(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h2", attributes, inner);
		}
		public static void H3(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h3", attributes, inner);
		}
		public static void H4(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h4", attributes, inner);
		}
		public static void H5(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h5", attributes, inner);
		}
		public static void H6(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("h6", attributes, inner);
		}
		public static void Header(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("header", attributes, inner);
		}
		public static void I(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("i", attributes, inner);
		}
		public static void Img(this HtmlWriter w, Action<ImgTagAttributes> attributes = null)
		{
			w.WriteSelfClosingTag("img", attributes);
		}
		public static void Label(this HtmlWriter w, Action<LabelTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("label", attributes, inner);
		}
		public static void Legend(this HtmlWriter w, Action<TagAttributes> attributes, string text)
		{
			w.WriteTag("legend", attributes, () => w.Write(text));
		}
		public static void Li(this HtmlWriter w, Action<LiTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("li", attributes, inner);
		}
		public static void Nav(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("nav", attributes, inner);
		}
		public static void Ol(this HtmlWriter w, Action<OlTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("ol", attributes, inner);
		}
		public static void P(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("p", attributes, inner);
		}
		public static void Pre(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("pre", attributes, inner);
		}
		public static void Q(this HtmlWriter w, Action<QTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("q", attributes, inner);
		}
		public static void S(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("s", attributes, inner);
		}
		public static void Script(this HtmlWriter w, Action<ScriptTagAttributes> attributes = null)
		{
			w.WriteTag("script", attributes, () => w.Write(""));
		}
		public static void Small(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("small", attributes, inner);
		}
		public static void Span(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("span", attributes, inner);
		}
		public static void Sub(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("sub", attributes, inner);
		}
		public static void Sup(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("sup", attributes, inner);
		}
		public static void Table(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("table", attributes, inner);
		}
		public static void Tbody(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("tbody", attributes, inner);
		}
		public static void Td(this HtmlWriter w, Action<TdTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("td", attributes, inner);
		}
		public static void Th(this HtmlWriter w, Action<ThTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("th", attributes, inner);
		}
		public static void Thead(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("thead", attributes, inner);
		}		
		public static void Tr(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("tr", attributes, inner);
		}
		public static void TrBegin(this HtmlWriter w, Action<TagAttributes> attributes = null)
		{
			w.WriteBeginTag("tr", attributes);
		}
		public static void TrEnd(this HtmlWriter w)
		{
			w.WriteEndTag("tr");
		}
		public static void U(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("u", attributes, inner);
		}
		public static void Ul(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("ul", attributes, inner);
		}

		public static void A(this HtmlWriter w, Action inner) { w.A(null, inner); }
		public static void B(this HtmlWriter w, Action inner) { w.B(null, inner); }		
		public static void Br(this HtmlWriter w) { w.Br(null); }
		public static void Canvas(this HtmlWriter w, Action inner) { w.Canvas(null, inner); }
		public static void Col(this HtmlWriter w) { w.Col(null); }
		public static void Colgroup(this HtmlWriter w, Action inner) { w.Colgroup(null, inner); }
		public static void Dd(this HtmlWriter w, Action inner) { w.Dd(null, inner); }
		//public static void Div(this HtmlWriter w, Action inner) { w.Div(null, inner); }
		public static void Dl(this HtmlWriter w, Action inner) { w.Dl(null, inner); }
		public static void Dt(this HtmlWriter w, Action inner) { w.Dt(null, inner); }
		public static void Em(this HtmlWriter w, Action inner) { w.Em(null, inner); }
		public static void Fieldset(this HtmlWriter w, Action inner) { w.Fieldset(null, inner); }
		public static void Form(this HtmlWriter w, Action inner) { w.Form(null, inner); }
		public static void H1(this HtmlWriter w, Action inner) { w.H1(null, inner); }
		public static void H2(this HtmlWriter w, Action inner) { w.H2(null, inner); }
		public static void H3(this HtmlWriter w, Action inner) { w.H3(null, inner); }
		public static void H4(this HtmlWriter w, Action inner) { w.H4(null, inner); }
		public static void H5(this HtmlWriter w, Action inner) { w.H5(null, inner); }
		public static void H6(this HtmlWriter w, Action inner) { w.H6(null, inner); }
		public static void Header(this HtmlWriter w, Action inner) { w.Header(null, inner); }
		public static void I(this HtmlWriter w, Action inner) { w.I(null, inner); }
		public static void Img(this HtmlWriter w) { w.Img(null); }
		public static void Label(this HtmlWriter w, Action inner) { w.Label(null, inner); }
		public static void Li(this HtmlWriter w, Action inner) { w.Li(null, inner); }
		public static void Nav(this HtmlWriter w, Action inner) { w.Nav(null, inner); }
		public static void Ol(this HtmlWriter w, Action inner) { w.Ol(null, inner); }
		public static void P(this HtmlWriter w, Action inner) { w.P(null, inner); }
		public static void Pre(this HtmlWriter w, Action inner) { w.Pre(null, inner); }
		public static void Q(this HtmlWriter w, Action inner) { w.Q(null, inner); }
		public static void S(this HtmlWriter w, Action inner) { w.S(null, inner); }
		public static void Small(this HtmlWriter w, Action inner) { w.Small(null, inner); }
		public static void Span(this HtmlWriter w, Action inner) { w.Span(null, inner); }
		public static void Sub(this HtmlWriter w, Action inner) { w.Sub(null, inner); }
		public static void Sup(this HtmlWriter w, Action inner) { w.Sup(null, inner); }
		//public static void Table(this HtmlWriter w, Action inner) { w.Table(null, inner); }
		public static void Tbody(this HtmlWriter w, Action inner) { w.Tbody(null, inner); }
		//public static void Td(this HtmlWriter w, Action inner) { w.Td(null, inner); }
		//public static void Th(this HtmlWriter w, Action inner) { w.Th(null, inner); }
		public static void Thead(this HtmlWriter w, Action inner) { w.Thead(null, inner); }
		
		//public static void Tr(this HtmlWriter w, Action inner) { w.Tr(null, inner); }
		public static void U(this HtmlWriter w, Action inner) { w.U(null, inner); }
		public static void U(this HtmlWriter w, string text) { w.U(null, () => w.Write(text)); }
		public static void Ul(this HtmlWriter w, Action inner) { w.Ul(null, inner); }

		public static void A(this HtmlWriter w, Action<ATagAttributes> attributes, string linkTitle) { w.A(attributes, () => w.Write(linkTitle)); }
		public static void A(this HtmlWriter w, string linkTitle) { w.A(null, () => w.Write(linkTitle)); }
		public static void B(this HtmlWriter w, string text) { w.B(null, () => w.Write(text)); }
		public static void B(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.B(attributes, () => w.Write(text)); }
		//public static void Div(this HtmlWriter w, string text) { w.Div(null, () => w.Write(text)); }
		public static void Div(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.Div(attributes, () => w.Write(text)); }
		public static void I(this HtmlWriter w, string text) { w.I(null, () => w.Write(text)); }
		public static void Label(this HtmlWriter w, string labelFor, string lableTitle)
		{
			w.Label(a => a.For(labelFor), () => w.Write(lableTitle));
		}
		public static void Span(this HtmlWriter w, string text) { w.Span(null, () => w.Write(text)); }
		public static void Span(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.Span(attributes, () => w.Write(text)); }
		public static void Sub(this HtmlWriter w, string text) { w.Sub(null, () => w.Write(text)); }
		public static void Sup(this HtmlWriter w, string text) { w.Sup(null, () => w.Write(text)); }
		public static void H1(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H1(attributes, () => w.Write(text)); }
		public static void H2(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H2(attributes, () => w.Write(text)); }
		public static void H3(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H3(attributes, () => w.Write(text)); }
		public static void H4(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H4(attributes, () => w.Write(text)); }
		public static void H5(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H5(attributes, () => w.Write(text)); }
		public static void H6(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.H6(attributes, () => w.Write(text)); }
		public static void H1(this HtmlWriter w, string text) { w.H1(null, () => w.Write(text)); }
		public static void H2(this HtmlWriter w, string text) { w.H2(null, () => w.Write(text)); }
		public static void H3(this HtmlWriter w, string text) { w.H3(null, () => w.Write(text)); }
		public static void H4(this HtmlWriter w, string text) { w.H4(null, () => w.Write(text)); }
		public static void H5(this HtmlWriter w, string text) { w.H5(null, () => w.Write(text)); }
		public static void H6(this HtmlWriter w, string text) { w.H6(null, () => w.Write(text)); }
		public static void Legend(this HtmlWriter w, string text) { w.Legend(null, text); }
		public static void Li(this HtmlWriter w, string text) { w.Li(null, () => w.Write(text)); }
		public static void P(this HtmlWriter w, string text) { w.P(null, () => w.Write(text)); }
		public static void P(this HtmlWriter w, Action<TagAttributes> attributes, string text) { w.P(attributes, () => w.Write(text)); }
		public static void Script(this HtmlWriter w, string path) { w.Script(a => a.Type("text/javascript").Src(path)); }
		public static void ScriptAsync(this HtmlWriter w, string path) { w.Script(a => a.Type("text/javascript").Src(path).Async()); }
		//public static void Td(this HtmlWriter w, string text) { w.Td(null, () => w.Write(text)); }
		//public static void Td(this HtmlWriter w, Action<TdTagAttributes> attributes, string text) { w.Td(attributes, () => w.Write(text)); }

		public static void WriteLine(this HtmlWriter w, string text) { w.Write(text); w.Br(); }
		public static void WriteLines(this HtmlWriter w, IEnumerable<string> textLines) { if (textLines != null) w.Write(textLines.Join("<br />")); }
	}

	public static class HtmlPageWriterExtensions
	{
		public static void DocType(this HtmlWriter w)
		{
			w.Write("<!DOCTYPE html>");
		}

		public static void Body(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("body", attributes, inner);
		}
		public static void Head(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("head", attributes, inner);
		}
		public static void Html(this HtmlWriter w, Action<HtmlTagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("html", attributes, inner);
		}
		public static void HeadLink(this HtmlWriter w, Action<LinkTagAttributes> attributes = null)
		{
			w.WriteSelfClosingTag("link", attributes);
		}
		public static void HeadMeta(this HtmlWriter w, Action<MetaTagAttributes> attributes = null)
		{
			w.WriteSelfClosingTag("meta", attributes);
		}
		public static void HeadTitle(this HtmlWriter w, Action<TagAttributes> attributes = null, Action inner = null)
		{
			w.WriteTag("title", attributes, inner);
		}

		public static void Body(this HtmlWriter w, Action inner) { w.Body(null, inner); }
		public static void Head(this HtmlWriter w, Action inner) { w.Head(null, inner); }
		public static void Html(this HtmlWriter w, Action inner) { w.Html(null, inner); }
		public static void HeadTitle(this HtmlWriter w, Action inner) { w.HeadTitle(null, inner); }
		public static void HeadTitle(this HtmlWriter w, string title) { w.HeadTitle(() => w.Write(title)); }
		public static void HeadLinkCss(this HtmlWriter w, string path) { w.HeadLink(a => a.Rel(LinkRel.Stylesheet).Type("text/css").Href(path)); }

	}
}