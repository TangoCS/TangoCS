using System;
using System.Collections.Generic;

namespace Nephrite.Html
{
	public abstract class TagAttributes<T>
	{
		protected TagBuilder _builder = null;
		protected T _this;

		public TagAttributes(TagBuilder builder)
		{
			_builder = builder;
		}

		public T AccessKey(string value) { _builder.MergeAttribute("accesskey", value); return _this; }
		public T Aria(string key, string value) { _builder.MergeAttribute("aria-" + key.ToLower(), value); return _this; }
		public T Class(string value, bool replaceExisting = false) { _builder.MergeAttribute("class", value, replaceExisting); return _this; }
		public T Contenteditable(bool value) { if (value) _builder.MergeAttribute("contenteditable", "Contenteditable"); return _this; }
		public T Data(string key, string value) { _builder.MergeAttribute("data-" + key.ToLower(), value); return _this; }
		public T Dir(Dir value) { _builder.MergeAttribute("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) _builder.MergeAttribute("draggable", "Draggable"); return _this; }
		public T ID(string value) { _builder.MergeAttribute("id", value); return _this; }
		public T Lang(string value) { _builder.MergeAttribute("lang", value); return _this; }
		public T Role(string value) { _builder.MergeAttribute("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { _builder.MergeAttribute("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { _builder.MergeAttribute("tabindex", value.ToString()); return _this; }
		public T Title(string value) { _builder.MergeAttribute("title", value); return _this; }

		public T OnLoad(string value) { _builder.MergeAttribute("onload", value); return _this; }
		public T OnResize(string value) { _builder.MergeAttribute("onresize", value); return _this; }
		public T OnUnload(string value) { _builder.MergeAttribute("onunload", value); return _this; }

		public T OnBlur(string value) { _builder.MergeAttribute("onblur", value); return _this; }
		public T OnChange(string value) { _builder.MergeAttribute("onchange", value); return _this; }
		public T OnFocus(string value) { _builder.MergeAttribute("onfocus", value); return _this; }
		public T OnSelect(string value) { _builder.MergeAttribute("onselect", value); return _this; }
		public T OnSubmit(string value) { _builder.MergeAttribute("onsubmit", value); return _this; }

		public T OnKeyDown(string value) { _builder.MergeAttribute("onkeydown", value); return _this; }
		public T OnKeyPress(string value) { _builder.MergeAttribute("onkeypress", value); return _this; }
		public T OnKeyUp(string value) { _builder.MergeAttribute("onkeyup", value); return _this; }

		public T OnClick(string value) { _builder.MergeAttribute("onclick", value); return _this; }
		public T OnDblClick(string value) { _builder.MergeAttribute("ondblclick", value); return _this; }
		public T OnDrag(string value) { _builder.MergeAttribute("ondrag", value); return _this; }
		public T OnDragEnd(string value) { _builder.MergeAttribute("ondragend", value); return _this; }
		public T OnDragEnter(string value) { _builder.MergeAttribute("ondragenter", value); return _this; }
		public T OnDragLeave(string value) { _builder.MergeAttribute("ondragleave", value); return _this; }
		public T OnDragOver(string value) { _builder.MergeAttribute("ondragover", value); return _this; }
		public T OnDragStart(string value) { _builder.MergeAttribute("ondragstart", value); return _this; }
		public T OnDrop(string value) { _builder.MergeAttribute("ondrop", value); return _this; }
		public T OnMouseDown(string value) { _builder.MergeAttribute("onmousedown", value); return _this; }
		public T OnMouseMove(string value) { _builder.MergeAttribute("onmousemove", value); return _this; }
		public T OnMouseOut(string value) { _builder.MergeAttribute("onmouseout", value); return _this; }
		public T OnMouseOver(string value) { _builder.MergeAttribute("onmouseover", value); return _this; }
		public T OnMouseUp(string value) { _builder.MergeAttribute("onmouseup", value); return _this; }
	}

	public class TagAttributes : TagAttributes<TagAttributes>
	{
		public TagAttributes(TagBuilder builder) : base(builder) { _this = this; }
	}

	public class ATagAttributes : TagAttributes<ATagAttributes>
	{
		public ATagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public ATagAttributes Href(string value) { _builder.MergeAttribute("href", value); return this; }
		public ATagAttributes HrefLang(string value) { _builder.MergeAttribute("hreflang", value); return this; }
		public ATagAttributes Media(string value) { _builder.MergeAttribute("media", value); return this; }
		public ATagAttributes Rel(Rel value) { _builder.MergeAttribute("rel", value.ToString().ToLower()); return this; }
		public ATagAttributes Target(Target value) { _builder.MergeAttribute("target", value.ToString().ToLower()); return this; }
		public ATagAttributes Type(string value) { _builder.MergeAttribute("type", value); return this; }
	}

	public class CanvasTagAttributes : TagAttributes<CanvasTagAttributes>
	{
		public CanvasTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public CanvasTagAttributes Height(int value) { _builder.MergeAttribute("height", value.ToString()); return this; }
		public CanvasTagAttributes Width(int value) { _builder.MergeAttribute("width", value.ToString()); return this; }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes<ColTagAttributes>
	{
		public ColTagAttributes(TagBuilder builder) : base(builder) { _this = this; }
		public ColTagAttributes Span(int value) { _builder.MergeAttribute("span", value.ToString()); return this; }
	}

	public class FormTagAttributes : TagAttributes<FormTagAttributes>
	{
		public FormTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public FormTagAttributes Action(string value) { _builder.MergeAttribute("action", value); return this; }
		public FormTagAttributes EncType(string value) { _builder.MergeAttribute("enctype", value); return this; }
		public FormTagAttributes Method(Method value) { _builder.MergeAttribute("method", value.ToString().ToLower()); return this; }
		public FormTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
		public FormTagAttributes Target(Target value) { _builder.MergeAttribute("target", value.ToString().ToLower()); return this; }
	}

	public class HtmlTagAttributes : TagAttributes<HtmlTagAttributes>
	{
		public HtmlTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public HtmlTagAttributes Manifest(string value) { _builder.MergeAttribute("manifest", value); return this; }
		public HtmlTagAttributes Xmlns(string value) { _builder.MergeAttribute("xmlns", value); return this; }
	}

	public class ImgTagAttributes : TagAttributes<ImgTagAttributes>
	{
		public ImgTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public ImgTagAttributes Alt(string value) { _builder.MergeAttribute("alt", value, true); return this; }
		public ImgTagAttributes Height(int value) { _builder.MergeAttribute("height", value.ToString()); return this; }
		public ImgTagAttributes IsMap(bool value) { if (value) _builder.MergeAttribute("ismap", "ismap"); return this; }
		public ImgTagAttributes Src(string value) { _builder.MergeAttribute("src", value, true); return this; }
		public ImgTagAttributes UseMap(string value) { _builder.MergeAttribute("usemap", value); return this; }
		public ImgTagAttributes Width(int value) { _builder.MergeAttribute("width", value.ToString()); return this; }
	}

	public class InputTagAttributes : TagAttributes<InputTagAttributes>
	{
		public InputTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public InputTagAttributes Accept(string value) { _builder.MergeAttribute("accept", value); return this; }
		public InputTagAttributes Alt(string value) { _builder.MergeAttribute("alt", value); return this; }
		public InputTagAttributes Autocomplete(bool value) { _builder.MergeAttribute("autocomplete", value ? "on" : "off"); return this; }
		public InputTagAttributes Autofocus(bool value) { if (value) _builder.MergeAttribute("autofocus", "autofocus"); return this; }
		public InputTagAttributes Checked(bool value) { if (value) _builder.MergeAttribute("checked", "checked"); return this; }
		public InputTagAttributes Disabled(bool value) { if (value) _builder.MergeAttribute("disabled", "disabled"); return this; }
		public InputTagAttributes Formaction(string value) { _builder.MergeAttribute("formaction", value); return this; }
		public InputTagAttributes Formenctype(string value) { _builder.MergeAttribute("formenctype", value); return this; }
		public InputTagAttributes Formmethod(Method value) { _builder.MergeAttribute("formmethod", value.ToString().ToLower()); return this; }
		public InputTagAttributes Formtarget(Target value) { _builder.MergeAttribute("formtarget", value.ToString().ToLower()); return this; }
		public InputTagAttributes Height(int value) { _builder.MergeAttribute("height", value.ToString()); return this; }
		public InputTagAttributes Max(string value) { _builder.MergeAttribute("max", value); return this; }
		public InputTagAttributes Maxlength(int value) { _builder.MergeAttribute("maxlength", value.ToString()); return this; }
		public InputTagAttributes Min(string value) { _builder.MergeAttribute("min", value); return this; }
		public InputTagAttributes Multiple(bool value) { if (value) _builder.MergeAttribute("multiple", "multiple"); return this; }
		public InputTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
		public InputTagAttributes Placeholder(string value) { _builder.MergeAttribute("placeholder", value); return this; }
		public InputTagAttributes Readonly(bool value) { if (value) _builder.MergeAttribute("readonly", "readonly"); return this; }
		public InputTagAttributes Size(int value) { _builder.MergeAttribute("size", value.ToString()); return this; }
		public InputTagAttributes Src(string value) { _builder.MergeAttribute("src", value); return this; }
		public InputTagAttributes Step(int value) { _builder.MergeAttribute("step", value.ToString()); return this; }
		public InputTagAttributes Type(InputType value) { _builder.MergeAttribute("type", value.ToString().ToLower()); return this; }
		public InputTagAttributes Value(string value) { _builder.MergeAttribute("value", value); return this; }
		public InputTagAttributes Width(int value) { _builder.MergeAttribute("width", value.ToString()); return this; }
	}

	public class ButtonTagAttributes : TagAttributes<ButtonTagAttributes>
	{
		public ButtonTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public ButtonTagAttributes Autofocus(bool value) { if (value) _builder.MergeAttribute("autofocus", "autofocus"); return this; }
		public ButtonTagAttributes Disabled(bool value) { if (value) _builder.MergeAttribute("disabled", "disabled"); return this; }
		public ButtonTagAttributes Formaction(string value) { _builder.MergeAttribute("formaction", value); return this; }
		public ButtonTagAttributes Formenctype(string value) { _builder.MergeAttribute("formenctype", value); return this; }
		public ButtonTagAttributes Formmethod(Method value) { _builder.MergeAttribute("formmethod", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Formtarget(Target value) { _builder.MergeAttribute("formtarget", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
		public ButtonTagAttributes Type(ButtonType value) { _builder.MergeAttribute("type", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Value(string value) { _builder.MergeAttribute("value", value); return this; }
	}

	public class LabelTagAttributes : TagAttributes<LabelTagAttributes>
	{
		public LabelTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public LabelTagAttributes For(string value) { _builder.MergeAttribute("for", value); return this; }
		public LabelTagAttributes Form(string value) { _builder.MergeAttribute("form", value); return this; }
	}

	public class LiTagAttributes : TagAttributes
	{
		public LiTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public LiTagAttributes Value(int value) { _builder.MergeAttribute("value", value.ToString()); return this; }
	}

	public class LinkTagAttributes : TagAttributes<LinkTagAttributes>
	{
		public LinkTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public LinkTagAttributes Href(string value) { _builder.MergeAttribute("href", value); return this; }
		public LinkTagAttributes HrefLang(string value) { _builder.MergeAttribute("hreflang", value); return this; }
		public LinkTagAttributes Media(string value) { _builder.MergeAttribute("media", value); return this; }
		public LinkTagAttributes Rel(LinkRel value) { _builder.MergeAttribute("rel", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Target(Target value) { _builder.MergeAttribute("target", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Type(string value) { _builder.MergeAttribute("type", value); return this; }
	}

	public class MetaTagAttributes : TagAttributes<MetaTagAttributes>
	{
		public MetaTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public MetaTagAttributes Charset(string value) { _builder.MergeAttribute("charset", value); return this; }
		public MetaTagAttributes Content(string value) { _builder.MergeAttribute("content", value); return this; }
		public MetaTagAttributes HttpEquiv(string value) { _builder.MergeAttribute("http-equiv", value); return this; }
		public MetaTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
	}

	public class OlTagAttributes : TagAttributes<OlTagAttributes>
	{
		public OlTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public OlTagAttributes Start(int value) { _builder.MergeAttribute("start", value.ToString()); return this; }
		public OlTagAttributes Type(string value) { _builder.MergeAttribute("type", value); return this; }
	}

	public class OptionTagAttributes : TagAttributes<OptionTagAttributes>
	{
		public OptionTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public OptionTagAttributes Disabled(bool value) { if (value) _builder.MergeAttribute("disabled", "disabled"); return this; }
		public OptionTagAttributes Label(string value) { _builder.MergeAttribute("label", value); return this; }
		public OptionTagAttributes Selected(bool value) { if (value) _builder.MergeAttribute("selected", "selected"); return this; }
		public OptionTagAttributes Value(string value) { _builder.MergeAttribute("value", value); return this; }
	}

	public class QTagAttributes : TagAttributes<QTagAttributes>
	{
		public QTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public QTagAttributes Cite(string value) { _builder.MergeAttribute("cite", value); return this; }
	}

	public class SelectTagAttributes : TagAttributes<SelectTagAttributes>
	{
		public SelectTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public SelectTagAttributes Autofocus(bool value) { if (value) _builder.MergeAttribute("autofocus", "autofocus"); return this; }
		public SelectTagAttributes Disabled(bool value) { if (value) _builder.MergeAttribute("disabled", "disabled"); return this; }
		public SelectTagAttributes Multiple(bool value) { if (value) _builder.MergeAttribute("multiple", "multiple"); return this; }
		public SelectTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
		public SelectTagAttributes Size(int value) { _builder.MergeAttribute("size", value.ToString()); return this; }
	}

	public class TdTagAttributes : TagAttributes<TdTagAttributes>
	{
		public TdTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public TdTagAttributes ColSpan(int value) { _builder.MergeAttribute("colspan", value.ToString()); return this; }
		public TdTagAttributes Headers(string value) { _builder.MergeAttribute("headers", value); return this; }
		public TdTagAttributes RowSpan(int value) { _builder.MergeAttribute("rowspan", value.ToString()); return this; }
	}

	public class TextAreaTagAttributes : TagAttributes<TextAreaTagAttributes>
	{
		public TextAreaTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public TextAreaTagAttributes Autofocus(bool value) { if (value) _builder.MergeAttribute("autofocus", "autofocus"); return this; }
		public TextAreaTagAttributes Cols(int value) { _builder.MergeAttribute("cols", value.ToString()); return this; }
		public TextAreaTagAttributes Disabled(bool value) { if (value) _builder.MergeAttribute("disabled", "disabled"); return this; }
		public TextAreaTagAttributes Name(string value) { _builder.MergeAttribute("name", value); return this; }
		public TextAreaTagAttributes Placeholder(string value) { _builder.MergeAttribute("placeholder", value); return this; }
		public TextAreaTagAttributes Readonly(bool value) { if (value) _builder.MergeAttribute("readonly", "readonly"); return this; }
		public TextAreaTagAttributes Rows(int value) { _builder.MergeAttribute("rows", value.ToString()); return this; }
		public TextAreaTagAttributes Wrap(Wrap value) { _builder.MergeAttribute("wrap", value.ToString().ToLower()); return this; }
	}

	public class ThTagAttributes : TagAttributes<ThTagAttributes>
	{
		public ThTagAttributes(TagBuilder builder) : base(builder) { _this = this; }

		public ThTagAttributes Abbr(string value) { _builder.MergeAttribute("abbr", value); return this; }
		public ThTagAttributes ColSpan(int value) { _builder.MergeAttribute("colspan", value.ToString()); return this; }
		public ThTagAttributes Headers(string value) { _builder.MergeAttribute("headers", value); return this; }
		public ThTagAttributes RowSpan(int value) { _builder.MergeAttribute("rowspan", value.ToString()); return this; }
		public ThTagAttributes Scope(Scope value) { _builder.MergeAttribute("scope", value.ToString().ToLower()); return this; }
	}

	public enum Dir { Ltr, Rtl, Auto }
	public enum Rel { Alternate, Author, Bookmark, Help, License, Next, Nofollow, Noreferrer, Prefetch, Prev, Search, Tag }
	public enum LinkRel { Alternate, Archives, Author, Bookmark, External, First, Help, Icon, Last, License, Next, Nofollow, Noreferrer, Pingback, Prefetch, Prev, Search, Sidebar, Stylesheet, Tag, Up }
	public enum Target { _blank, _parent, _self, _top }
	public enum Method { Get, Post }
	public enum Scope { Col, ColGroup, Row, RowGroup }
	public enum InputType { Button, Checkbox, File, Hidden, Image, Number, Password, Radio, Range, Reset, Submit, Text }
	public enum ButtonType { Button, Reset, Submit }
	public enum Wrap { Hard, Soft }
}