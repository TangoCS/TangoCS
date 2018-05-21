using System;
using System.Collections.Generic;

namespace Tango.Html
{
	public interface IAttributeWriter
	{
		void Write(string key, string value, bool replaceExisting = true);
		void WriteID(string key, string value);
	}

	public abstract class TagAttributes<T>
		where T : TagAttributes<T>
	{
		T _this;

		public TagAttributes()
		{
			_this = this as T;
		}

		public IAttributeWriter AttributeWriter { get; set; }

		public T ID() { AttributeWriter.WriteID("id", null); return _this; }
		public T ID<TValue>(TValue value) { AttributeWriter.WriteID("id", value.ToString()); return _this; }

		public T AccessKey(string value) { AttributeWriter.Write("accesskey", value); return _this; }
		public T Aria(string key, string value) { AttributeWriter.Write("aria-" + key.ToLower(), value); return _this; }
		public T Class(string value, bool replaceExisting = false) { AttributeWriter.Write("class", value, replaceExisting); return _this; }
		public T ContentEditable(bool value) { if (value) AttributeWriter.Write("contenteditable", "Contenteditable"); return _this; }
		public T Data<TValue>(string key, TValue value) { AttributeWriter.Write("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T DataIDValue<TValue>(string key, TValue value) { AttributeWriter.WriteID("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T Dir(Dir value) { AttributeWriter.Write("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) AttributeWriter.Write("draggable", "Draggable"); return _this; }
		public T Lang(string value) { AttributeWriter.Write("lang", value); return _this; }
		public T Role(string value) { AttributeWriter.Write("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { AttributeWriter.Write("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { AttributeWriter.Write("tabindex", value.ToString()); return _this; }
		public T Title(string value) { AttributeWriter.Write("title", value); return _this; }

		public T OnLoad(string value) { AttributeWriter.Write("onload", value); return _this; }
		public T OnResize(string value) { AttributeWriter.Write("onresize", value); return _this; }
		public T OnUnload(string value) { AttributeWriter.Write("onunload", value); return _this; }

		public T OnBlur(string value) { AttributeWriter.Write("onblur", value); return _this; }
		public T OnChange(string value) { AttributeWriter.Write("onchange", value); return _this; }
		public T OnFocus(string value) { AttributeWriter.Write("onfocus", value); return _this; }
		public T OnSelect(string value) { AttributeWriter.Write("onselect", value); return _this; }
		public T OnSubmit(string value) { AttributeWriter.Write("onsubmit", value); return _this; }

		public T OnKeyDown(string value) { AttributeWriter.Write("onkeydown", value); return _this; }
		public T OnKeyPress(string value) { AttributeWriter.Write("onkeypress", value); return _this; }
		public T OnKeyUp(string value) { AttributeWriter.Write("onkeyup", value); return _this; }

		public T OnClick(string value) { AttributeWriter.Write("onclick", value); return _this; }
		public T OnDblClick(string value) { AttributeWriter.Write("ondblclick", value); return _this; }
		public T OnDrag(string value) { AttributeWriter.Write("ondrag", value); return _this; }
		public T OnDragEnd(string value) { AttributeWriter.Write("ondragend", value); return _this; }
		public T OnDragEnter(string value) { AttributeWriter.Write("ondragenter", value); return _this; }
		public T OnDragLeave(string value) { AttributeWriter.Write("ondragleave", value); return _this; }
		public T OnDragOver(string value) { AttributeWriter.Write("ondragover", value); return _this; }
		public T OnDragStart(string value) { AttributeWriter.Write("ondragstart", value); return _this; }
		public T OnDrop(string value) { AttributeWriter.Write("ondrop", value); return _this; }
		public T OnMouseDown(string value) { AttributeWriter.Write("onmousedown", value); return _this; }
		public T OnMouseMove(string value) { AttributeWriter.Write("onmousemove", value); return _this; }
		public T OnMouseOut(string value) { AttributeWriter.Write("onmouseout", value); return _this; }
		public T OnMouseOver(string value) { AttributeWriter.Write("onmouseover", value); return _this; }
		public T OnMouseUp(string value) { AttributeWriter.Write("onmouseup", value); return _this; }

		public T Set(Action<T> attrs) { attrs?.Invoke(_this); return _this; }

		public T Data(IReadOnlyDictionary<string, string> d) 
		{
			if (d != null) 
				foreach (var v in d) 
					Data(v.Key, v.Value); 
			return _this; 
		}
	}

	public class TagAttributes : TagAttributes<TagAttributes>
	{
	}

	public class ATagAttributes : TagAttributes<ATagAttributes>
	{
		public ATagAttributes Href(string value) { AttributeWriter.Write("href", value); return this; }
		public ATagAttributes HrefLang(string value) { AttributeWriter.Write("hreflang", value); return this; }
		public ATagAttributes Media(string value) { AttributeWriter.Write("media", value); return this; }
		public ATagAttributes Rel(Rel value) { AttributeWriter.Write("rel", value.ToString().ToLower()); return this; }
		public ATagAttributes Target(Target value) { AttributeWriter.Write("target", value.ToString().ToLower()); return this; }
		public ATagAttributes Type(string value) { AttributeWriter.Write("type", value); return this; }
	}

	public class CanvasTagAttributes : TagAttributes<CanvasTagAttributes>
	{
		public CanvasTagAttributes Height(int value) { AttributeWriter.Write("height", value.ToString()); return this; }
		public CanvasTagAttributes Width(int value) { AttributeWriter.Write("width", value.ToString()); return this; }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes<ColTagAttributes>
	{
		public ColTagAttributes Span(int value) { AttributeWriter.Write("span", value.ToString()); return this; }
	}

	public class FormTagAttributes : TagAttributes<FormTagAttributes>
	{
		public FormTagAttributes Action(string value) { AttributeWriter.Write("action", value); return this; }
		public FormTagAttributes EncType(string value) { AttributeWriter.Write("enctype", value); return this; }
		public FormTagAttributes Method(Method value) { AttributeWriter.Write("method", value.ToString().ToLower()); return this; }
		public FormTagAttributes Name(string value) { AttributeWriter.Write("name", value); return this; }
		public FormTagAttributes Target(Target value) { AttributeWriter.Write("target", value.ToString().ToLower()); return this; }
	}

	public class HtmlTagAttributes : TagAttributes<HtmlTagAttributes>
	{
		public HtmlTagAttributes Manifest(string value) { AttributeWriter.Write("manifest", value); return this; }
		public HtmlTagAttributes Xmlns(string value) { AttributeWriter.Write("xmlns", value); return this; }
		public HtmlTagAttributes Xmlns(string prefix, string value) { AttributeWriter.Write("xmlns:" + prefix, value); return this; }
	}

	public class ImgTagAttributes : TagAttributes<ImgTagAttributes>
	{
		public ImgTagAttributes Alt(string value) { AttributeWriter.Write("alt", value, true); return this; }
		public ImgTagAttributes Height(int value) { AttributeWriter.Write("height", value.ToString()); return this; }
		public ImgTagAttributes IsMap(bool value) { if (value) AttributeWriter.Write("ismap", "ismap"); return this; }
		public ImgTagAttributes Src(string value) { AttributeWriter.Write("src", value, true); return this; }
		public ImgTagAttributes UseMap(string value) { AttributeWriter.Write("usemap", value); return this; }
		public ImgTagAttributes Width(int value) { AttributeWriter.Write("width", value.ToString()); return this; }
	}

	public class InputTagAttributes : TagAttributes<InputTagAttributes>
	{
		public InputTagAttributes Name(string value) { AttributeWriter.Write("name", value?.ToLower()); return this; }

		public InputTagAttributes Accept(string value) { AttributeWriter.Write("accept", value); return this; }
		public InputTagAttributes Alt(string value) { AttributeWriter.Write("alt", value); return this; }
		public InputTagAttributes Autocomplete(bool value) { AttributeWriter.Write("autocomplete", value ? "on" : "off"); return this; }
		public InputTagAttributes Autofocus(bool value) { if (value) AttributeWriter.Write("autofocus", "autofocus"); return this; }
		public InputTagAttributes Checked(bool value) { if (value) AttributeWriter.Write("checked", "checked"); return this; }
		public InputTagAttributes Disabled(bool value) { if (value) AttributeWriter.Write("disabled", "disabled"); return this; }
		public InputTagAttributes FormAction(string value) { AttributeWriter.Write("formaction", value); return this; }
		public InputTagAttributes FormEnctype(string value) { AttributeWriter.Write("formenctype", value); return this; }
		public InputTagAttributes FormMethod(Method value) { AttributeWriter.Write("formmethod", value.ToString().ToLower()); return this; }
		public InputTagAttributes FormTarget(Target value) { AttributeWriter.Write("formtarget", value.ToString().ToLower()); return this; }
		public InputTagAttributes Height(int value) { AttributeWriter.Write("height", value.ToString()); return this; }
		public InputTagAttributes Max(string value) { AttributeWriter.Write("max", value); return this; }
		public InputTagAttributes MaxLength(int value) { AttributeWriter.Write("maxlength", value.ToString()); return this; }
		public InputTagAttributes Min(string value) { AttributeWriter.Write("min", value); return this; }
		public InputTagAttributes Multiple(bool value) { if (value) AttributeWriter.Write("multiple", "multiple"); return this; }
		public InputTagAttributes Placeholder(string value) { AttributeWriter.Write("placeholder", value); return this; }
		public InputTagAttributes Readonly(bool value) { if (value) AttributeWriter.Write("readonly", "readonly"); return this; }
		public InputTagAttributes Size(int value) { AttributeWriter.Write("size", value.ToString()); return this; }
		public InputTagAttributes Src(string value) { AttributeWriter.Write("src", value); return this; }
		public InputTagAttributes Step(int value) { AttributeWriter.Write("step", value.ToString()); return this; }
		public InputTagAttributes Type(InputType value) { AttributeWriter.Write("type", value.ToString().ToLower()); return this; }
		public InputTagAttributes Value(string value) { AttributeWriter.Write("value", value); return this; }
		public InputTagAttributes Width(int value) { AttributeWriter.Write("width", value.ToString()); return this; }
	}

	public class ButtonTagAttributes : TagAttributes<ButtonTagAttributes>
	{
		public ButtonTagAttributes Autofocus(bool value) { if (value) AttributeWriter.Write("autofocus", "autofocus"); return this; }
		public ButtonTagAttributes Disabled(bool value) { if (value) AttributeWriter.Write("disabled", "disabled"); return this; }
		public ButtonTagAttributes FormAction(string value) { AttributeWriter.Write("formaction", value); return this; }
		public ButtonTagAttributes FormEnctype(string value) { AttributeWriter.Write("formenctype", value); return this; }
		public ButtonTagAttributes FormMethod(Method value) { AttributeWriter.Write("formmethod", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes FormTarget(Target value) { AttributeWriter.Write("formtarget", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Name(string value) { AttributeWriter.Write("name", value); return this; }
		public ButtonTagAttributes Type(ButtonType value) { AttributeWriter.Write("type", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Value(string value) { AttributeWriter.Write("value", value); return this; }
	}

	public class LabelTagAttributes : TagAttributes<LabelTagAttributes>
	{
		public LabelTagAttributes For(string value) { AttributeWriter.WriteID("for", value); return this; }
		public LabelTagAttributes Form(string value) { AttributeWriter.Write("form", value); return this; }
	}

	public class LiTagAttributes : TagAttributes<LiTagAttributes>
	{
		public LiTagAttributes Value(int value) { AttributeWriter.Write("value", value.ToString()); return this; }
	}

	public class LinkTagAttributes : TagAttributes<LinkTagAttributes>
	{
		public LinkTagAttributes Href(string value) { AttributeWriter.Write("href", value); return this; }
		public LinkTagAttributes HrefLang(string value) { AttributeWriter.Write("hreflang", value); return this; }
		public LinkTagAttributes Media(string value) { AttributeWriter.Write("media", value); return this; }
		public LinkTagAttributes Rel(LinkRel value) { AttributeWriter.Write("rel", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Target(Target value) { AttributeWriter.Write("target", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Type(string value) { AttributeWriter.Write("type", value); return this; }
	}

	public class MetaTagAttributes : TagAttributes<MetaTagAttributes>
	{
		public MetaTagAttributes CharSet(string value) { AttributeWriter.Write("charset", value); return this; }
		public MetaTagAttributes Content(string value) { AttributeWriter.Write("content", value); return this; }
		public MetaTagAttributes HttpEquiv(string value) { AttributeWriter.Write("http-equiv", value); return this; }
		public MetaTagAttributes Name(string value) { AttributeWriter.Write("name", value); return this; }
	}

	public class OlTagAttributes : TagAttributes<OlTagAttributes>
	{
		public OlTagAttributes Start(int value) { AttributeWriter.Write("start", value.ToString()); return this; }
		public OlTagAttributes Type(string value) { AttributeWriter.Write("type", value); return this; }
	}

	public class OptionTagAttributes : TagAttributes<OptionTagAttributes>
	{
		public OptionTagAttributes Disabled(bool value) { if (value) AttributeWriter.Write("disabled", "disabled"); return this; }
		public OptionTagAttributes Label(string value) { AttributeWriter.Write("label", value); return this; }
		public OptionTagAttributes Selected(bool value) { if (value) AttributeWriter.Write("selected", "selected"); return this; }
		public OptionTagAttributes Value(string value) { AttributeWriter.Write("value", value); return this; }
	}

	public class QTagAttributes : TagAttributes<QTagAttributes>
	{
		public QTagAttributes Cite(string value) { AttributeWriter.Write("cite", value); return this; }
	}

	public class SelectTagAttributes : TagAttributes<SelectTagAttributes>
	{
		public SelectTagAttributes Name(string value) { AttributeWriter.Write("name", value?.ToLower()); return this; }

		public SelectTagAttributes Autofocus(bool value) { if (value) AttributeWriter.Write("autofocus", "autofocus"); return this; }
		public SelectTagAttributes Disabled(bool value) { if (value) AttributeWriter.Write("disabled", "disabled"); return this; }
		public SelectTagAttributes Multiple(bool value) { if (value) AttributeWriter.Write("multiple", "multiple"); return this; }
		public SelectTagAttributes Size(int value) { AttributeWriter.Write("size", value.ToString()); return this; }
	}

	public class ScriptTagAttributes : TagAttributes<ScriptTagAttributes>
	{
		public ScriptTagAttributes CharSet(string value) { AttributeWriter.Write("type", value); return this; }
		public ScriptTagAttributes Src(string value) { AttributeWriter.Write("src", value); return this; }
		public ScriptTagAttributes Type(string value) { AttributeWriter.Write("type", value); return this; }
		public ScriptTagAttributes Async() { AttributeWriter.Write("async", "async"); return this; }
	}

	public class TdTagAttributes : TagAttributes<TdTagAttributes>
	{
		public TdTagAttributes ColSpan(int value) { AttributeWriter.Write("colspan", value.ToString()); return this; }
		public TdTagAttributes Headers(string value) { AttributeWriter.Write("headers", value); return this; }
		public TdTagAttributes RowSpan(int value) { AttributeWriter.Write("rowspan", value.ToString()); return this; }
	}

	public class TextAreaTagAttributes : TagAttributes<TextAreaTagAttributes>
	{
		public TextAreaTagAttributes Name(string value) { AttributeWriter.Write("name", value?.ToLower()); return this; }

		public TextAreaTagAttributes Autofocus(bool value) { if (value) AttributeWriter.Write("autofocus", "autofocus"); return this; }
		public TextAreaTagAttributes Cols(int value) { AttributeWriter.Write("cols", value.ToString()); return this; }
		public TextAreaTagAttributes Disabled(bool value) { if (value) AttributeWriter.Write("disabled", "disabled"); return this; }
		public TextAreaTagAttributes Placeholder(string value) { AttributeWriter.Write("placeholder", value); return this; }
		public TextAreaTagAttributes Readonly(bool value) { if (value) AttributeWriter.Write("readonly", "readonly"); return this; }
		public TextAreaTagAttributes Rows(int value) { AttributeWriter.Write("rows", value.ToString()); return this; }
		public TextAreaTagAttributes Wrap(Wrap value) { AttributeWriter.Write("wrap", value.ToString().ToLower()); return this; }
	}

	public class ThTagAttributes : TagAttributes<ThTagAttributes>
	{
		public ThTagAttributes Abbr(string value) { AttributeWriter.Write("abbr", value); return this; }
		public ThTagAttributes ColSpan(int value) { AttributeWriter.Write("colspan", value.ToString()); return this; }
		public ThTagAttributes Headers(string value) { AttributeWriter.Write("headers", value); return this; }
		public ThTagAttributes RowSpan(int value) { AttributeWriter.Write("rowspan", value.ToString()); return this; }
		public ThTagAttributes Scope(Scope value) { AttributeWriter.Write("scope", value.ToString().ToLower()); return this; }
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

	public class DataCollection : Dictionary<string, string>
	{
		public DataCollection Ref(string id)
		{
			Add("ref-" + id, "");
			return this;
		}

		public DataCollection Parm<T>(string key, T value)
		{
			Add("p-" + key, value?.ToString());
			return this;
		}

		public DataCollection Add(IReadOnlyDictionary<string, string> source)
		{
			foreach(var d in source)
				Add(d.Key, d.Value);
			return this;
		}
	}
}