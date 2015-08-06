using System;
using System.Collections.Generic;

namespace Nephrite.Html
{
	public class TagAttributes
	{
		protected TagBuilder _builder = null;
		public TagAttributes(TagBuilder builder)
		{
			_builder = builder;
		}

		public string AccessKey { set { _builder.MergeAttribute("accesskey", value); } }
		public string Class { set { _builder.MergeAttribute("class", value); } }
		public bool Contenteditable { set { if (value) _builder.MergeAttribute("contenteditable", "Contenteditable"); } }
		public void Data(string key, string value) { _builder.MergeAttribute("data-" + key.ToLower(), value); }
		public Dir Dir { set { _builder.MergeAttribute("dir", value.ToString().ToLower()); } }
		public bool Draggable { set { if (value) _builder.MergeAttribute("draggable", "Draggable"); } }
		public string ID { set { _builder.MergeAttribute("id", value); } }
		public string Lang { set { _builder.MergeAttribute("lang", value); } }
		public string Style { set { _builder.MergeAttribute("style", value); } }
		public int TabIndex { set { _builder.MergeAttribute("tabindex", value.ToString()); } }
		public string Title { set { _builder.MergeAttribute("title", value); } }

		public string OnLoad { set { _builder.MergeAttribute("onload", value); } }
		public string OnResize { set { _builder.MergeAttribute("onresize", value); } }
		public string OnUnload { set { _builder.MergeAttribute("onunload", value); } }

		public string OnBlur { set { _builder.MergeAttribute("onblur", value); } }
		public string OnChange { set { _builder.MergeAttribute("onchange", value); } }
		public string OnFocus { set { _builder.MergeAttribute("onfocus", value); } }
		public string OnSelect { set { _builder.MergeAttribute("onselect", value); } }
		public string OnSubmit { set { _builder.MergeAttribute("onsubmit", value); } }

		public string OnKeyDown { set { _builder.MergeAttribute("onkeydown", value); } }
		public string OnKeyPress { set { _builder.MergeAttribute("onkeypress", value); } }
		public string OnKeyUp { set { _builder.MergeAttribute("onkeyup", value); } }

		public string OnClick { set { _builder.MergeAttribute("onclick", value); } }
		public string OnDblClick { set { _builder.MergeAttribute("ondblclick", value); } }
		public string OnDrag { set { _builder.MergeAttribute("ondrag", value); } }
		public string OnDragEnd { set { _builder.MergeAttribute("ondragend", value); } }
		public string OnDragEnter { set { _builder.MergeAttribute("ondragenter", value); } }
		public string OnDragLeave { set { _builder.MergeAttribute("ondragleave", value); } }
		public string OnDragOver { set { _builder.MergeAttribute("ondragover", value); } }
		public string OnDragStart { set { _builder.MergeAttribute("ondragstart", value); } }
		public string OnDrop { set { _builder.MergeAttribute("ondrop", value); } }
		public string OnMouseDown { set { _builder.MergeAttribute("onmousedown", value); } }
		public string OnMouseMove { set { _builder.MergeAttribute("onmousemove", value); } }
		public string OnMouseOut { set { _builder.MergeAttribute("onmouseout", value); } }
		public string OnMouseOver { set { _builder.MergeAttribute("onmouseover", value); } }
		public string OnMouseUp { set { _builder.MergeAttribute("onmouseup", value); } }
	}

	public class ATagAttributes : TagAttributes
	{
		public ATagAttributes(TagBuilder builder) : base(builder) { }

		public string Href { set { _builder.MergeAttribute("href", value); } }
		public string HrefLang { set { _builder.MergeAttribute("hreflang", value); } }
		public string Media { set { _builder.MergeAttribute("media", value); } }
		public Rel Rel { set { _builder.MergeAttribute("rel", value.ToString().ToLower()); } }
		public Target Target { set { _builder.MergeAttribute("target", value.ToString().ToLower()); } }
		public string Type { set { _builder.MergeAttribute("type", value); } }
	}

	public class CanvasTagAttributes : TagAttributes
	{
		public CanvasTagAttributes(TagBuilder builder) : base(builder) { }

		public int Height { set { _builder.MergeAttribute("height", value.ToString()); } }
		public int Width { set { _builder.MergeAttribute("width", value.ToString()); } }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes
	{
		public ColTagAttributes(TagBuilder builder) : base(builder) { }
		public int Span { set { _builder.MergeAttribute("span", value.ToString()); } }
	}

	public class FormTagAttributes : TagAttributes
	{
		public FormTagAttributes(TagBuilder builder) : base(builder) { }

		public string Action { set { _builder.MergeAttribute("action", value); } }
		public string EncType { set { _builder.MergeAttribute("enctype", value); } }
		public Method Method { set { _builder.MergeAttribute("method", value.ToString().ToLower()); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
		public Target Target { set { _builder.MergeAttribute("target", value.ToString().ToLower()); } }
	}

	public class HtmlTagAttributes : TagAttributes
	{
		public HtmlTagAttributes(TagBuilder builder) : base(builder) { }

		public string Manifest { set { _builder.MergeAttribute("manifest", value); } }
		public string Xmlns { set { _builder.MergeAttribute("xmlns", value); } }
	}

	public class ImgTagAttributes : TagAttributes
	{
		public ImgTagAttributes(TagBuilder builder) : base(builder) { }

		public string Alt { set { _builder.MergeAttribute("alt", value, true); } }
		public int Height { set { _builder.MergeAttribute("height", value.ToString()); } }
		public bool IsMap { set { if (value) _builder.MergeAttribute("ismap", "ismap"); } }
		public string Src { set { _builder.MergeAttribute("src", value, true); } }
		public string UseMap { set { _builder.MergeAttribute("usemap", value); } }
		public int Width { set { _builder.MergeAttribute("width", value.ToString()); } }
	}

	public class InputTagAttributes : TagAttributes
	{
		public InputTagAttributes(TagBuilder builder) : base(builder) { }

		public string Accept { set { _builder.MergeAttribute("accept", value); } }
		public string Alt { set { _builder.MergeAttribute("alt", value); } }
		public bool Autocomplete { set { _builder.MergeAttribute("autocomplete", value ? "on" : "off"); } }
		public bool Autofocus { set { if (value) _builder.MergeAttribute("autofocus", "autofocus"); } }
		public bool Checked { set { if (value) _builder.MergeAttribute("checked", "checked"); } }
		public bool Disabled { set { if (value) _builder.MergeAttribute("disabled", "disabled"); } }
		public string Formaction { set { _builder.MergeAttribute("formaction", value); } }
		public string Formenctype { set { _builder.MergeAttribute("formenctype", value); } }
		public Method Formmethod { set { _builder.MergeAttribute("formmethod", value.ToString().ToLower()); } }
		public Target Formtarget { set { _builder.MergeAttribute("formtarget", value.ToString().ToLower()); } }
		public int Height { set { _builder.MergeAttribute("height", value.ToString()); } }
		public string Max { set { _builder.MergeAttribute("max", value); } }
		public int Maxlength { set { _builder.MergeAttribute("maxlength", value.ToString()); } }
		public string Min { set { _builder.MergeAttribute("min", value); } }
		public bool Multiple { set { if (value) _builder.MergeAttribute("multiple", "multiple"); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
		public string Placeholder { set { _builder.MergeAttribute("placeholder", value); } }
		public bool Readonly { set { if (value) _builder.MergeAttribute("readonly", "readonly"); } }
		public int Size { set { _builder.MergeAttribute("size", value.ToString()); } }
		public string Src { set { _builder.MergeAttribute("src", value); } }
		public int Step { set { _builder.MergeAttribute("step", value.ToString()); } }
		public InputType Type { set { _builder.MergeAttribute("type", value.ToString().ToLower()); } }
		public string Value { set { _builder.MergeAttribute("value", value); } }
		public int Width { set { _builder.MergeAttribute("width", value.ToString()); } }
	}

	public class ButtonTagAttributes : TagAttributes
	{
		public ButtonTagAttributes(TagBuilder builder) : base(builder) { }

		public bool Autofocus { set { if (value) _builder.MergeAttribute("autofocus", "autofocus"); } }
		public bool Disabled { set { if (value) _builder.MergeAttribute("disabled", "disabled"); } }
		public string Formaction { set { _builder.MergeAttribute("formaction", value); } }
		public string Formenctype { set { _builder.MergeAttribute("formenctype", value); } }
		public Method Formmethod { set { _builder.MergeAttribute("formmethod", value.ToString().ToLower()); } }
		public Target Formtarget { set { _builder.MergeAttribute("formtarget", value.ToString().ToLower()); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
		public ButtonType Type { set { _builder.MergeAttribute("type", value.ToString().ToLower()); } }
		public string Value { set { _builder.MergeAttribute("value", value); } }
	}

	public class LabelTagAttributes : TagAttributes
	{
		public LabelTagAttributes(TagBuilder builder) : base(builder) { }

		public string For { set { _builder.MergeAttribute("for", value); } }
		public string Form { set { _builder.MergeAttribute("form", value); } }
	}

	public class LiTagAttributes : TagAttributes
	{
		public LiTagAttributes(TagBuilder builder) : base(builder) { }

		public int Value { set { _builder.MergeAttribute("value", value.ToString()); } }
	}

	public class LinkTagAttributes : TagAttributes
	{
		public LinkTagAttributes(TagBuilder builder) : base(builder) { }

		public string Href { set { _builder.MergeAttribute("href", value); } }
		public string HrefLang { set { _builder.MergeAttribute("hreflang", value); } }
		public string Media { set { _builder.MergeAttribute("media", value); } }
		public LinkRel Rel { set { _builder.MergeAttribute("rel", value.ToString().ToLower()); } }
		public Target Target { set { _builder.MergeAttribute("target", value.ToString().ToLower()); } }
		public string Type { set { _builder.MergeAttribute("type", value); } }
	}

	public class MetaTagAttributes : TagAttributes
	{
		public MetaTagAttributes(TagBuilder builder) : base(builder) { }

		public string Charset { set { _builder.MergeAttribute("charset", value); } }
		public string Content { set { _builder.MergeAttribute("content", value); } }
		public string HttpEquiv { set { _builder.MergeAttribute("http-equiv", value); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
	}

	public class OlTagAttributes : TagAttributes
	{
		public OlTagAttributes(TagBuilder builder) : base(builder) { }

		public int Start { set { _builder.MergeAttribute("start", value.ToString()); } }
		public string Type { set { _builder.MergeAttribute("type", value); } }
	}

	public class OptionTagAttributes : TagAttributes
	{
		public OptionTagAttributes(TagBuilder builder) : base(builder) { }

		public bool Disabled { set { if (value) _builder.MergeAttribute("disabled", "disabled"); } }
		public string Label { set { _builder.MergeAttribute("label", value); } }
		public bool Selected { set { if (value) _builder.MergeAttribute("selected", "selected"); } }
		public string Value { set { _builder.MergeAttribute("value", value); } }
	}

	public class QTagAttributes : TagAttributes
	{
		public QTagAttributes(TagBuilder builder) : base(builder) { }

		public string Cite { set { _builder.MergeAttribute("cite", value); } }
	}

	public class SelectTagAttributes : TagAttributes
	{
		public SelectTagAttributes(TagBuilder builder) : base(builder) { }

		public bool Autofocus { set { if (value) _builder.MergeAttribute("autofocus", "autofocus"); } }
		public bool Disabled { set { if (value) _builder.MergeAttribute("disabled", "disabled"); } }
		public bool Multiple { set { if (value) _builder.MergeAttribute("multiple", "multiple"); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
		public int Size { set { _builder.MergeAttribute("size", value.ToString()); } }
	}

	public class TdTagAttributes : TagAttributes
	{
		public TdTagAttributes(TagBuilder builder) : base(builder) { }

		public int ColSpan { set { _builder.MergeAttribute("colspan", value.ToString()); } }
		public string Headers { set { _builder.MergeAttribute("headers", value); } }
		public int RowSpan { set { _builder.MergeAttribute("rowspan", value.ToString()); } }
	}

	public class TextAreaTagAttributes : TagAttributes
	{
		public TextAreaTagAttributes(TagBuilder builder) : base(builder) { }

		public bool Autofocus { set { if (value) _builder.MergeAttribute("autofocus", "autofocus"); } }
		public int Cols { set { _builder.MergeAttribute("cols", value.ToString()); } }
		public bool Disabled { set { if (value) _builder.MergeAttribute("disabled", "disabled"); } }
		public string Name { set { _builder.MergeAttribute("name", value); } }
		public string Placeholder { set { _builder.MergeAttribute("placeholder", value); } }
		public bool Readonly { set { if (value) _builder.MergeAttribute("readonly", "readonly"); } }
		public int Rows { set { _builder.MergeAttribute("rows", value.ToString()); } }
		public Wrap Wrap { set { _builder.MergeAttribute("wrap", value.ToString().ToLower()); } }
	}

	public class ThTagAttributes : TagAttributes
	{
		public ThTagAttributes(TagBuilder builder) : base(builder) { }

		public string Abbr { set { _builder.MergeAttribute("abbr", value); } }
		public int ColSpan { set { _builder.MergeAttribute("colspan", value.ToString()); } }
		public string Headers { set { _builder.MergeAttribute("headers", value); } }
		public int RowSpan { set { _builder.MergeAttribute("rowspan", value.ToString()); } }
		public Scope Scope { set { _builder.MergeAttribute("scope", value.ToString().ToLower()); } }
	}

	public enum Dir { Ltr, Rtl, Auto }
	public enum Rel { Alternate, Author, Bookmark, Help, License, Next, Nofollow, Noreferrer, Prefetch, Prev, Search, Tag }
	public enum LinkRel { Alternate, Archives, Author, Bookmark, External, First, Help, Icon, Last, License, Next, Nofollow, Noreferrer, Pingback, Prefetch, Prev, Search, Sidebar, Stylesheet, Tag, Up }
	public enum Target { _blank, _parent, _self, _top }
	public enum Method { Get, Post }
	public enum Scope { Col, ColGroup, Row, RowGroup }
	public enum InputType { Button, Checkbox, File, Hidden, Image, Number, Password, Radio, Range, Reset, Submit, Text }
	public enum ButtonType { Button, Reset, Submit }
	public enum Wrap { Hard, Soft}
}