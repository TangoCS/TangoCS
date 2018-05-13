using System;
using System.Collections.Generic;

namespace Tango.Html
{
	public abstract class TagAttributes<T>
		where T : TagAttributes<T>
	{
		protected T _this;

		public TagAttributes()
		{
			_this = this as T;
		}

		public Action<string, string, bool> AttributeFunc { get; set; }
		public Action<string, string> IDAttributeFunc { get; set; }


		protected void AttributeFuncInt(string key, string value) => AttributeFunc(key, value, true);

		public T ID() { IDAttributeFunc("id", null); return _this; }
		public T ID<TValue>(TValue value) { IDAttributeFunc("id", value.ToString()); return _this; }

		public T AccessKey(string value) { AttributeFuncInt("accesskey", value); return _this; }
		public T Aria(string key, string value) { AttributeFuncInt("aria-" + key.ToLower(), value); return _this; }
		public T Class(string value, bool replaceExisting = false) { AttributeFunc("class", value, replaceExisting); return _this; }
		public T ContentEditable(bool value) { if (value) AttributeFuncInt("contenteditable", "Contenteditable"); return _this; }
		public T Data<TValue>(string key, TValue value) { AttributeFuncInt("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T DataIDValue<TValue>(string key, TValue value) { IDAttributeFunc("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T Dir(Dir value) { AttributeFuncInt("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) AttributeFuncInt("draggable", "Draggable"); return _this; }
		public T Lang(string value) { AttributeFuncInt("lang", value); return _this; }
		public T Role(string value) { AttributeFuncInt("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { AttributeFunc("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { AttributeFuncInt("tabindex", value.ToString()); return _this; }
		public T Title(string value) { AttributeFuncInt("title", value); return _this; }

		public T OnLoad(string value) { AttributeFuncInt("onload", value); return _this; }
		public T OnResize(string value) { AttributeFuncInt("onresize", value); return _this; }
		public T OnUnload(string value) { AttributeFuncInt("onunload", value); return _this; }

		public T OnBlur(string value) { AttributeFuncInt("onblur", value); return _this; }
		public T OnChange(string value) { AttributeFuncInt("onchange", value); return _this; }
		public T OnFocus(string value) { AttributeFuncInt("onfocus", value); return _this; }
		public T OnSelect(string value) { AttributeFuncInt("onselect", value); return _this; }
		public T OnSubmit(string value) { AttributeFuncInt("onsubmit", value); return _this; }

		public T OnKeyDown(string value) { AttributeFuncInt("onkeydown", value); return _this; }
		public T OnKeyPress(string value) { AttributeFuncInt("onkeypress", value); return _this; }
		public T OnKeyUp(string value) { AttributeFuncInt("onkeyup", value); return _this; }

		public T OnClick(string value) { AttributeFuncInt("onclick", value); return _this; }
		public T OnDblClick(string value) { AttributeFuncInt("ondblclick", value); return _this; }
		public T OnDrag(string value) { AttributeFuncInt("ondrag", value); return _this; }
		public T OnDragEnd(string value) { AttributeFuncInt("ondragend", value); return _this; }
		public T OnDragEnter(string value) { AttributeFuncInt("ondragenter", value); return _this; }
		public T OnDragLeave(string value) { AttributeFuncInt("ondragleave", value); return _this; }
		public T OnDragOver(string value) { AttributeFuncInt("ondragover", value); return _this; }
		public T OnDragStart(string value) { AttributeFuncInt("ondragstart", value); return _this; }
		public T OnDrop(string value) { AttributeFuncInt("ondrop", value); return _this; }
		public T OnMouseDown(string value) { AttributeFuncInt("onmousedown", value); return _this; }
		public T OnMouseMove(string value) { AttributeFuncInt("onmousemove", value); return _this; }
		public T OnMouseOut(string value) { AttributeFuncInt("onmouseout", value); return _this; }
		public T OnMouseOver(string value) { AttributeFuncInt("onmouseover", value); return _this; }
		public T OnMouseUp(string value) { AttributeFuncInt("onmouseup", value); return _this; }

		public T Set(Action<T> attrs) { attrs?.Invoke(_this); return _this; }

		public T Data(DataCollection d) 
		{
			if (d != null) 
				foreach (var v in d.Value) 
					Data(v.Key, v.Value); 
			return _this; 
		}
	}

	public class TagAttributes : TagAttributes<TagAttributes>
	{
	}

	public class ATagAttributes : TagAttributes<ATagAttributes>
	{
		public ATagAttributes Href(string value) { AttributeFuncInt("href", value); return this; }
		public ATagAttributes HrefLang(string value) { AttributeFuncInt("hreflang", value); return this; }
		public ATagAttributes Media(string value) { AttributeFuncInt("media", value); return this; }
		public ATagAttributes Rel(Rel value) { AttributeFuncInt("rel", value.ToString().ToLower()); return this; }
		public ATagAttributes Target(Target value) { AttributeFuncInt("target", value.ToString().ToLower()); return this; }
		public ATagAttributes Type(string value) { AttributeFuncInt("type", value); return this; }
	}

	public class CanvasTagAttributes : TagAttributes<CanvasTagAttributes>
	{
		public CanvasTagAttributes Height(int value) { AttributeFuncInt("height", value.ToString()); return this; }
		public CanvasTagAttributes Width(int value) { AttributeFuncInt("width", value.ToString()); return this; }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes<ColTagAttributes>
	{
		public ColTagAttributes Span(int value) { AttributeFuncInt("span", value.ToString()); return this; }
	}

	public class FormTagAttributes : TagAttributes<FormTagAttributes>
	{
		public FormTagAttributes Action(string value) { AttributeFuncInt("action", value); return this; }
		public FormTagAttributes EncType(string value) { AttributeFuncInt("enctype", value); return this; }
		public FormTagAttributes Method(Method value) { AttributeFuncInt("method", value.ToString().ToLower()); return this; }
		public FormTagAttributes Name(string value) { AttributeFuncInt("name", value); return this; }
		public FormTagAttributes Target(Target value) { AttributeFuncInt("target", value.ToString().ToLower()); return this; }
	}

	public class HtmlTagAttributes : TagAttributes<HtmlTagAttributes>
	{
		public HtmlTagAttributes Manifest(string value) { AttributeFuncInt("manifest", value); return this; }
		public HtmlTagAttributes Xmlns(string value) { AttributeFuncInt("xmlns", value); return this; }
		public HtmlTagAttributes Xmlns(string prefix, string value) { AttributeFuncInt("xmlns:" + prefix, value); return this; }
	}

	public class ImgTagAttributes : TagAttributes<ImgTagAttributes>
	{
		public ImgTagAttributes Alt(string value) { AttributeFunc("alt", value, true); return this; }
		public ImgTagAttributes Height(int value) { AttributeFuncInt("height", value.ToString()); return this; }
		public ImgTagAttributes IsMap(bool value) { if (value) AttributeFuncInt("ismap", "ismap"); return this; }
		public ImgTagAttributes Src(string value) { AttributeFunc("src", value, true); return this; }
		public ImgTagAttributes UseMap(string value) { AttributeFuncInt("usemap", value); return this; }
		public ImgTagAttributes Width(int value) { AttributeFuncInt("width", value.ToString()); return this; }
	}

	public class InputTagAttributes : TagAttributes<InputTagAttributes>
	{
		public InputTagAttributes Name(string value) { AttributeFuncInt("name", value?.ToLower()); return _this; }

		public InputTagAttributes Accept(string value) { AttributeFuncInt("accept", value); return this; }
		public InputTagAttributes Alt(string value) { AttributeFuncInt("alt", value); return this; }
		public InputTagAttributes Autocomplete(bool value) { AttributeFuncInt("autocomplete", value ? "on" : "off"); return this; }
		public InputTagAttributes Autofocus(bool value) { if (value) AttributeFuncInt("autofocus", "autofocus"); return this; }
		public InputTagAttributes Checked(bool value) { if (value) AttributeFuncInt("checked", "checked"); return this; }
		public InputTagAttributes Disabled(bool value) { if (value) AttributeFuncInt("disabled", "disabled"); return this; }
		public InputTagAttributes FormAction(string value) { AttributeFuncInt("formaction", value); return this; }
		public InputTagAttributes FormEnctype(string value) { AttributeFuncInt("formenctype", value); return this; }
		public InputTagAttributes FormMethod(Method value) { AttributeFuncInt("formmethod", value.ToString().ToLower()); return this; }
		public InputTagAttributes FormTarget(Target value) { AttributeFuncInt("formtarget", value.ToString().ToLower()); return this; }
		public InputTagAttributes Height(int value) { AttributeFuncInt("height", value.ToString()); return this; }
		public InputTagAttributes Max(string value) { AttributeFuncInt("max", value); return this; }
		public InputTagAttributes MaxLength(int value) { AttributeFuncInt("maxlength", value.ToString()); return this; }
		public InputTagAttributes Min(string value) { AttributeFuncInt("min", value); return this; }
		public InputTagAttributes Multiple(bool value) { if (value) AttributeFuncInt("multiple", "multiple"); return this; }
		public InputTagAttributes Placeholder(string value) { AttributeFuncInt("placeholder", value); return this; }
		public InputTagAttributes Readonly(bool value) { if (value) AttributeFuncInt("readonly", "readonly"); return this; }
		public InputTagAttributes Size(int value) { AttributeFuncInt("size", value.ToString()); return this; }
		public InputTagAttributes Src(string value) { AttributeFuncInt("src", value); return this; }
		public InputTagAttributes Step(int value) { AttributeFuncInt("step", value.ToString()); return this; }
		public InputTagAttributes Type(InputType value) { AttributeFuncInt("type", value.ToString().ToLower()); return this; }
		public InputTagAttributes Value(string value) { AttributeFuncInt("value", value); return this; }
		public InputTagAttributes Width(int value) { AttributeFuncInt("width", value.ToString()); return this; }
	}

	public class ButtonTagAttributes : TagAttributes<ButtonTagAttributes>
	{
		public ButtonTagAttributes Autofocus(bool value) { if (value) AttributeFuncInt("autofocus", "autofocus"); return this; }
		public ButtonTagAttributes Disabled(bool value) { if (value) AttributeFuncInt("disabled", "disabled"); return this; }
		public ButtonTagAttributes FormAction(string value) { AttributeFuncInt("formaction", value); return this; }
		public ButtonTagAttributes FormEnctype(string value) { AttributeFuncInt("formenctype", value); return this; }
		public ButtonTagAttributes FormMethod(Method value) { AttributeFuncInt("formmethod", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes FormTarget(Target value) { AttributeFuncInt("formtarget", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Name(string value) { AttributeFuncInt("name", value); return this; }
		public ButtonTagAttributes Type(ButtonType value) { AttributeFuncInt("type", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Value(string value) { AttributeFuncInt("value", value); return this; }
	}

	public class LabelTagAttributes : TagAttributes<LabelTagAttributes>
	{
		public LabelTagAttributes For(string value) { IDAttributeFunc("for", value); return this; }
		public LabelTagAttributes Form(string value) { AttributeFuncInt("form", value); return this; }
	}

	public class LiTagAttributes : TagAttributes<LiTagAttributes>
	{
		public LiTagAttributes Value(int value) { AttributeFuncInt("value", value.ToString()); return this; }
	}

	public class LinkTagAttributes : TagAttributes<LinkTagAttributes>
	{
		public LinkTagAttributes Href(string value) { AttributeFuncInt("href", value); return this; }
		public LinkTagAttributes HrefLang(string value) { AttributeFuncInt("hreflang", value); return this; }
		public LinkTagAttributes Media(string value) { AttributeFuncInt("media", value); return this; }
		public LinkTagAttributes Rel(LinkRel value) { AttributeFuncInt("rel", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Target(Target value) { AttributeFuncInt("target", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Type(string value) { AttributeFuncInt("type", value); return this; }
	}

	public class MetaTagAttributes : TagAttributes<MetaTagAttributes>
	{
		public MetaTagAttributes CharSet(string value) { AttributeFuncInt("charset", value); return this; }
		public MetaTagAttributes Content(string value) { AttributeFuncInt("content", value); return this; }
		public MetaTagAttributes HttpEquiv(string value) { AttributeFuncInt("http-equiv", value); return this; }
		public MetaTagAttributes Name(string value) { AttributeFuncInt("name", value); return this; }
	}

	public class OlTagAttributes : TagAttributes<OlTagAttributes>
	{
		public OlTagAttributes Start(int value) { AttributeFuncInt("start", value.ToString()); return this; }
		public OlTagAttributes Type(string value) { AttributeFuncInt("type", value); return this; }
	}

	public class OptionTagAttributes : TagAttributes<OptionTagAttributes>
	{
		public OptionTagAttributes Disabled(bool value) { if (value) AttributeFuncInt("disabled", "disabled"); return this; }
		public OptionTagAttributes Label(string value) { AttributeFuncInt("label", value); return this; }
		public OptionTagAttributes Selected(bool value) { if (value) AttributeFuncInt("selected", "selected"); return this; }
		public OptionTagAttributes Value(string value) { AttributeFuncInt("value", value); return this; }
	}

	public class QTagAttributes : TagAttributes<QTagAttributes>
	{
		public QTagAttributes Cite(string value) { AttributeFuncInt("cite", value); return this; }
	}

	public class SelectTagAttributes : TagAttributes<SelectTagAttributes>
	{
		public SelectTagAttributes Name(string value) { AttributeFuncInt("name", value?.ToLower()); return _this; }

		public SelectTagAttributes Autofocus(bool value) { if (value) AttributeFuncInt("autofocus", "autofocus"); return this; }
		public SelectTagAttributes Disabled(bool value) { if (value) AttributeFuncInt("disabled", "disabled"); return this; }
		public SelectTagAttributes Multiple(bool value) { if (value) AttributeFuncInt("multiple", "multiple"); return this; }
		public SelectTagAttributes Size(int value) { AttributeFuncInt("size", value.ToString()); return this; }
	}

	public class ScriptTagAttributes : TagAttributes<ScriptTagAttributes>
	{
		public ScriptTagAttributes CharSet(string value) { AttributeFuncInt("type", value); return this; }
		public ScriptTagAttributes Src(string value) { AttributeFuncInt("src", value); return this; }
		public ScriptTagAttributes Type(string value) { AttributeFuncInt("type", value); return this; }
		public ScriptTagAttributes Async() { AttributeFuncInt("async", "async"); return this; }
	}

	public class TdTagAttributes : TagAttributes<TdTagAttributes>
	{
		public TdTagAttributes ColSpan(int value) { AttributeFuncInt("colspan", value.ToString()); return this; }
		public TdTagAttributes Headers(string value) { AttributeFuncInt("headers", value); return this; }
		public TdTagAttributes RowSpan(int value) { AttributeFuncInt("rowspan", value.ToString()); return this; }
	}

	public class TextAreaTagAttributes : TagAttributes<TextAreaTagAttributes>
	{
		public TextAreaTagAttributes Name(string value) { AttributeFuncInt("name", value?.ToLower()); return _this; }

		public TextAreaTagAttributes Autofocus(bool value) { if (value) AttributeFuncInt("autofocus", "autofocus"); return this; }
		public TextAreaTagAttributes Cols(int value) { AttributeFuncInt("cols", value.ToString()); return this; }
		public TextAreaTagAttributes Disabled(bool value) { if (value) AttributeFuncInt("disabled", "disabled"); return this; }
		public TextAreaTagAttributes Placeholder(string value) { AttributeFuncInt("placeholder", value); return this; }
		public TextAreaTagAttributes Readonly(bool value) { if (value) AttributeFuncInt("readonly", "readonly"); return this; }
		public TextAreaTagAttributes Rows(int value) { AttributeFuncInt("rows", value.ToString()); return this; }
		public TextAreaTagAttributes Wrap(Wrap value) { AttributeFuncInt("wrap", value.ToString().ToLower()); return this; }
	}

	public class ThTagAttributes : TagAttributes<ThTagAttributes>
	{
		public ThTagAttributes Abbr(string value) { AttributeFuncInt("abbr", value); return this; }
		public ThTagAttributes ColSpan(int value) { AttributeFuncInt("colspan", value.ToString()); return this; }
		public ThTagAttributes Headers(string value) { AttributeFuncInt("headers", value); return this; }
		public ThTagAttributes RowSpan(int value) { AttributeFuncInt("rowspan", value.ToString()); return this; }
		public ThTagAttributes Scope(Scope value) { AttributeFuncInt("scope", value.ToString().ToLower()); return this; }
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

	public class DataCollection
	{
		public Dictionary<string, string> Value = new Dictionary<string, string>();
		public DataCollection Add(string key, string value)
		{
			Value.Add(key, value);
			return this;
		}

		public DataCollection Ref(string id)
		{
			Value.Add("ref-" + id, "");
			return this;
		}

		public DataCollection Parm<T>(string key, T value)
		{
			Value.Add("p-" + key, value?.ToString());
			return this;
		}

		public DataCollection Add(DataCollection source)
		{
			foreach(var d in source.Value)
				Value.Add(d.Key, d.Value);
			return this;
		}
	}
}