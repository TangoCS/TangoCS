using System;
using System.Collections.Generic;

namespace Tango.Html
{
	public abstract class TagAttributes<T>
		where T : TagAttributes<T>
	{
		protected T _this;

		public string IDPrefix { get; set; }
		public IDictionary<string, string> Attributes { get; private set; }

		public TagAttributes()
		{
			_this = this as T;
			Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
		}

		public void MergeAttribute(string key, string value, bool replaceExisting = true)
		{
			if (replaceExisting || !Attributes.ContainsKey(key))
				Attributes[key] = value;
			else
				Attributes[key] = Attributes[key] + " " + value;
		}

		protected string GetID(string id)
		{
			if (id == null) return null;
			if (!IDPrefix.IsEmpty() && IDPrefix != id)
				return (IDPrefix + (!id.IsEmpty() ? "_" + id : "")).ToLower();
			return id.ToLower();
		}

		public T ID(string value) { MergeAttribute("id", GetID(value)); return _this; }

		public T AccessKey(string value) { MergeAttribute("accesskey", value); return _this; }
		public T Aria(string key, string value) { MergeAttribute("aria-" + key.ToLower(), value); return _this; }
		public T Class(string value, bool replaceExisting = false) { MergeAttribute("class", value, replaceExisting); return _this; }
		public T ContentEditable(bool value) { if (value) MergeAttribute("contenteditable", "Contenteditable"); return _this; }
		public T Data(string key, object value) { MergeAttribute("data-" + key.ToLower(), value.ToString()); return _this; }
		public T Dir(Dir value) { MergeAttribute("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) MergeAttribute("draggable", "Draggable"); return _this; }
		public T Lang(string value) { MergeAttribute("lang", value); return _this; }
		public T Role(string value) { MergeAttribute("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { MergeAttribute("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { MergeAttribute("tabindex", value.ToString()); return _this; }
		public T Title(string value) { MergeAttribute("title", value); return _this; }

		public T OnLoad(string value) { MergeAttribute("onload", value); return _this; }
		public T OnResize(string value) { MergeAttribute("onresize", value); return _this; }
		public T OnUnload(string value) { MergeAttribute("onunload", value); return _this; }

		public T OnBlur(string value) { MergeAttribute("onblur", value); return _this; }
		public T OnChange(string value) { MergeAttribute("onchange", value); return _this; }
		public T OnFocus(string value) { MergeAttribute("onfocus", value); return _this; }
		public T OnSelect(string value) { MergeAttribute("onselect", value); return _this; }
		public T OnSubmit(string value) { MergeAttribute("onsubmit", value); return _this; }

		public T OnKeyDown(string value) { MergeAttribute("onkeydown", value); return _this; }
		public T OnKeyPress(string value) { MergeAttribute("onkeypress", value); return _this; }
		public T OnKeyUp(string value) { MergeAttribute("onkeyup", value); return _this; }

		public T OnClick(string value) { MergeAttribute("onclick", value); return _this; }
		public T OnDblClick(string value) { MergeAttribute("ondblclick", value); return _this; }
		public T OnDrag(string value) { MergeAttribute("ondrag", value); return _this; }
		public T OnDragEnd(string value) { MergeAttribute("ondragend", value); return _this; }
		public T OnDragEnter(string value) { MergeAttribute("ondragenter", value); return _this; }
		public T OnDragLeave(string value) { MergeAttribute("ondragleave", value); return _this; }
		public T OnDragOver(string value) { MergeAttribute("ondragover", value); return _this; }
		public T OnDragStart(string value) { MergeAttribute("ondragstart", value); return _this; }
		public T OnDrop(string value) { MergeAttribute("ondrop", value); return _this; }
		public T OnMouseDown(string value) { MergeAttribute("onmousedown", value); return _this; }
		public T OnMouseMove(string value) { MergeAttribute("onmousemove", value); return _this; }
		public T OnMouseOut(string value) { MergeAttribute("onmouseout", value); return _this; }
		public T OnMouseOver(string value) { MergeAttribute("onmouseover", value); return _this; }
		public T OnMouseUp(string value) { MergeAttribute("onmouseup", value); return _this; }

		public T Set(Action<T> attrs) { if (attrs != null) attrs(_this); return _this; }
		
		public T DataParm(string key, object value) { MergeAttribute("data-p-" + key.ToLower(), value.ToString()); return _this; }
		public T DataRef(string id) { MergeAttribute("data-ref-" + id.ToLower(), ""); return _this; }
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
		public ATagAttributes Href(string value) { MergeAttribute("href", value); return this; }
		public ATagAttributes HrefLang(string value) { MergeAttribute("hreflang", value); return this; }
		public ATagAttributes Media(string value) { MergeAttribute("media", value); return this; }
		public ATagAttributes Rel(Rel value) { MergeAttribute("rel", value.ToString().ToLower()); return this; }
		public ATagAttributes Target(Target value) { MergeAttribute("target", value.ToString().ToLower()); return this; }
		public ATagAttributes Type(string value) { MergeAttribute("type", value); return this; }
	}

	public class CanvasTagAttributes : TagAttributes<CanvasTagAttributes>
	{
		public CanvasTagAttributes Height(int value) { MergeAttribute("height", value.ToString()); return this; }
		public CanvasTagAttributes Width(int value) { MergeAttribute("width", value.ToString()); return this; }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes<ColTagAttributes>
	{
		public ColTagAttributes Span(int value) { MergeAttribute("span", value.ToString()); return this; }
	}

	public class FormTagAttributes : TagAttributes<FormTagAttributes>
	{
		public FormTagAttributes Action(string value) { MergeAttribute("action", value); return this; }
		public FormTagAttributes EncType(string value) { MergeAttribute("enctype", value); return this; }
		public FormTagAttributes Method(Method value) { MergeAttribute("method", value.ToString().ToLower()); return this; }
		public FormTagAttributes Name(string value) { MergeAttribute("name", value); return this; }
		public FormTagAttributes Target(Target value) { MergeAttribute("target", value.ToString().ToLower()); return this; }
	}

	public class HtmlTagAttributes : TagAttributes<HtmlTagAttributes>
	{
		public HtmlTagAttributes Manifest(string value) { MergeAttribute("manifest", value); return this; }
		public HtmlTagAttributes Xmlns(string value) { MergeAttribute("xmlns", value); return this; }
	}

	public class ImgTagAttributes : TagAttributes<ImgTagAttributes>
	{
		public ImgTagAttributes Alt(string value) { MergeAttribute("alt", value, true); return this; }
		public ImgTagAttributes Height(int value) { MergeAttribute("height", value.ToString()); return this; }
		public ImgTagAttributes IsMap(bool value) { if (value) MergeAttribute("ismap", "ismap"); return this; }
		public ImgTagAttributes Src(string value) { MergeAttribute("src", value, true); return this; }
		public ImgTagAttributes UseMap(string value) { MergeAttribute("usemap", value); return this; }
		public ImgTagAttributes Width(int value) { MergeAttribute("width", value.ToString()); return this; }
	}

	public class InputTagAttributes : TagAttributes<InputTagAttributes>
	{
		public InputTagAttributes Name(string value) { MergeAttribute("name", value?.ToLower()); return _this; }

		public InputTagAttributes Accept(string value) { MergeAttribute("accept", value); return this; }
		public InputTagAttributes Alt(string value) { MergeAttribute("alt", value); return this; }
		public InputTagAttributes Autocomplete(bool value) { MergeAttribute("autocomplete", value ? "on" : "off"); return this; }
		public InputTagAttributes Autofocus(bool value) { if (value) MergeAttribute("autofocus", "autofocus"); return this; }
		public InputTagAttributes Checked(bool value) { if (value) MergeAttribute("checked", "checked"); return this; }
		public InputTagAttributes Disabled(bool value) { if (value) MergeAttribute("disabled", "disabled"); return this; }
		public InputTagAttributes FormAction(string value) { MergeAttribute("formaction", value); return this; }
		public InputTagAttributes FormEnctype(string value) { MergeAttribute("formenctype", value); return this; }
		public InputTagAttributes FormMethod(Method value) { MergeAttribute("formmethod", value.ToString().ToLower()); return this; }
		public InputTagAttributes FormTarget(Target value) { MergeAttribute("formtarget", value.ToString().ToLower()); return this; }
		public InputTagAttributes Height(int value) { MergeAttribute("height", value.ToString()); return this; }
		public InputTagAttributes Max(string value) { MergeAttribute("max", value); return this; }
		public InputTagAttributes MaxLength(int value) { MergeAttribute("maxlength", value.ToString()); return this; }
		public InputTagAttributes Min(string value) { MergeAttribute("min", value); return this; }
		public InputTagAttributes Multiple(bool value) { if (value) MergeAttribute("multiple", "multiple"); return this; }
		public InputTagAttributes Placeholder(string value) { MergeAttribute("placeholder", value); return this; }
		public InputTagAttributes Readonly(bool value) { if (value) MergeAttribute("readonly", "readonly"); return this; }
		public InputTagAttributes Size(int value) { MergeAttribute("size", value.ToString()); return this; }
		public InputTagAttributes Src(string value) { MergeAttribute("src", value); return this; }
		public InputTagAttributes Step(int value) { MergeAttribute("step", value.ToString()); return this; }
		public InputTagAttributes Type(InputType value) { MergeAttribute("type", value.ToString().ToLower()); return this; }
		public InputTagAttributes Value(string value) { MergeAttribute("value", value); return this; }
		public InputTagAttributes Width(int value) { MergeAttribute("width", value.ToString()); return this; }
	}

	public class ButtonTagAttributes : TagAttributes<ButtonTagAttributes>
	{
		public ButtonTagAttributes Autofocus(bool value) { if (value) MergeAttribute("autofocus", "autofocus"); return this; }
		public ButtonTagAttributes Disabled(bool value) { if (value) MergeAttribute("disabled", "disabled"); return this; }
		public ButtonTagAttributes FormAction(string value) { MergeAttribute("formaction", value); return this; }
		public ButtonTagAttributes FormEnctype(string value) { MergeAttribute("formenctype", value); return this; }
		public ButtonTagAttributes FormMethod(Method value) { MergeAttribute("formmethod", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes FormTarget(Target value) { MergeAttribute("formtarget", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Name(string value) { MergeAttribute("name", value); return this; }
		public ButtonTagAttributes Type(ButtonType value) { MergeAttribute("type", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Value(string value) { MergeAttribute("value", value); return this; }
	}

	public class LabelTagAttributes : TagAttributes<LabelTagAttributes>
	{
		public LabelTagAttributes For(string value) { MergeAttribute("for", GetID(value)); return this; }
		public LabelTagAttributes Form(string value) { MergeAttribute("form", value); return this; }
	}

	public class LiTagAttributes : TagAttributes<LiTagAttributes>
	{
		public LiTagAttributes Value(int value) { MergeAttribute("value", value.ToString()); return this; }
	}

	public class LinkTagAttributes : TagAttributes<LinkTagAttributes>
	{
		public LinkTagAttributes Href(string value) { MergeAttribute("href", value); return this; }
		public LinkTagAttributes HrefLang(string value) { MergeAttribute("hreflang", value); return this; }
		public LinkTagAttributes Media(string value) { MergeAttribute("media", value); return this; }
		public LinkTagAttributes Rel(LinkRel value) { MergeAttribute("rel", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Target(Target value) { MergeAttribute("target", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Type(string value) { MergeAttribute("type", value); return this; }
	}

	public class MetaTagAttributes : TagAttributes<MetaTagAttributes>
	{
		public MetaTagAttributes CharSet(string value) { MergeAttribute("charset", value); return this; }
		public MetaTagAttributes Content(string value) { MergeAttribute("content", value); return this; }
		public MetaTagAttributes HttpEquiv(string value) { MergeAttribute("http-equiv", value); return this; }
		public MetaTagAttributes Name(string value) { MergeAttribute("name", value); return this; }
	}

	public class OlTagAttributes : TagAttributes<OlTagAttributes>
	{
		public OlTagAttributes Start(int value) { MergeAttribute("start", value.ToString()); return this; }
		public OlTagAttributes Type(string value) { MergeAttribute("type", value); return this; }
	}

	public class OptionTagAttributes : TagAttributes<OptionTagAttributes>
	{
		public OptionTagAttributes Disabled(bool value) { if (value) MergeAttribute("disabled", "disabled"); return this; }
		public OptionTagAttributes Label(string value) { MergeAttribute("label", value); return this; }
		public OptionTagAttributes Selected(bool value) { if (value) MergeAttribute("selected", "selected"); return this; }
		public OptionTagAttributes Value(string value) { MergeAttribute("value", value); return this; }
	}

	public class QTagAttributes : TagAttributes<QTagAttributes>
	{
		public QTagAttributes Cite(string value) { MergeAttribute("cite", value); return this; }
	}

	public class SelectTagAttributes : TagAttributes<SelectTagAttributes>
	{
		public SelectTagAttributes Name(string value) { MergeAttribute("name", value?.ToLower()); return _this; }

		public SelectTagAttributes Autofocus(bool value) { if (value) MergeAttribute("autofocus", "autofocus"); return this; }
		public SelectTagAttributes Disabled(bool value) { if (value) MergeAttribute("disabled", "disabled"); return this; }
		public SelectTagAttributes Multiple(bool value) { if (value) MergeAttribute("multiple", "multiple"); return this; }
		public SelectTagAttributes Size(int value) { MergeAttribute("size", value.ToString()); return this; }
	}

	public class ScriptTagAttributes : TagAttributes<ScriptTagAttributes>
	{
		public ScriptTagAttributes CharSet(string value) { MergeAttribute("type", value); return this; }
		public ScriptTagAttributes Src(string value) { MergeAttribute("src", value); return this; }
		public ScriptTagAttributes Type(string value) { MergeAttribute("type", value); return this; }
	}

	public class TdTagAttributes : TagAttributes<TdTagAttributes>
	{
		public TdTagAttributes ColSpan(int value) { MergeAttribute("colspan", value.ToString()); return this; }
		public TdTagAttributes Headers(string value) { MergeAttribute("headers", value); return this; }
		public TdTagAttributes RowSpan(int value) { MergeAttribute("rowspan", value.ToString()); return this; }
	}

	public class TextAreaTagAttributes : TagAttributes<TextAreaTagAttributes>
	{
		public TextAreaTagAttributes Name(string value) { MergeAttribute("name", value?.ToLower()); return _this; }

		public TextAreaTagAttributes Autofocus(bool value) { if (value) MergeAttribute("autofocus", "autofocus"); return this; }
		public TextAreaTagAttributes Cols(int value) { MergeAttribute("cols", value.ToString()); return this; }
		public TextAreaTagAttributes Disabled(bool value) { if (value) MergeAttribute("disabled", "disabled"); return this; }
		public TextAreaTagAttributes Placeholder(string value) { MergeAttribute("placeholder", value); return this; }
		public TextAreaTagAttributes Readonly(bool value) { if (value) MergeAttribute("readonly", "readonly"); return this; }
		public TextAreaTagAttributes Rows(int value) { MergeAttribute("rows", value.ToString()); return this; }
		public TextAreaTagAttributes Wrap(Wrap value) { MergeAttribute("wrap", value.ToString().ToLower()); return this; }
	}

	public class ThTagAttributes : TagAttributes<ThTagAttributes>
	{
		public ThTagAttributes Abbr(string value) { MergeAttribute("abbr", value); return this; }
		public ThTagAttributes ColSpan(int value) { MergeAttribute("colspan", value.ToString()); return this; }
		public ThTagAttributes Headers(string value) { MergeAttribute("headers", value); return this; }
		public ThTagAttributes RowSpan(int value) { MergeAttribute("rowspan", value.ToString()); return this; }
		public ThTagAttributes Scope(Scope value) { MergeAttribute("scope", value.ToString().ToLower()); return this; }
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

		public DataCollection Parm(string key, string value)
		{
			Value.Add("p-" + key, value);
			return this;
		}

		public DataCollection Data(DataCollection source)
		{
			foreach(var d in source.Value)
				Value.Add(d.Key, d.Value);
			return this;
		}
	}
}