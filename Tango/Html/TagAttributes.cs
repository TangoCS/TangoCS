using System;
using System.Collections.Generic;
using System.Globalization;
using Tango.UI;

namespace Tango.Html
{
	public abstract class TagAttributes<T> : IContentItemAttributes<T>
		where T : TagAttributes<T>, IContentItemAttributes<T>
	{
		readonly T _this;

		public TagAttributes()
		{
			_this = this as T;
		}

		public HtmlWriter Writer;

		public void SetWriter(IContentWriter writer) => Writer = writer as HtmlWriter;

		public T Custom(string name, string value) { Writer.WriteAttr(name, value); return _this; }

		public T ID() { Writer.WriteAttrID("id", null); return _this; }
		public T ID<TValue>(TValue value) { Writer.WriteAttrID("id", value.ToString()); return _this; }

		public T AccessKey(string value) { Writer.WriteAttr("accesskey", value); return _this; }
		public T Aria(string key, string value) { Writer.WriteAttr("aria-" + key.ToLower(), value); return _this; }
		public T Class(string value, bool replaceExisting = false) { Writer.WriteAttr("class", value, replaceExisting); return _this; }
		public T ContentEditable() { Writer.WriteAttr("contenteditable"); return _this; }
		public T Data<TValue>(string key, TValue value) { Writer.WriteAttr("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T Data(string key) { Writer.WriteAttr("data-" + key.ToLower(), null); return _this; }
		public T DataIDValue<TValue>(string key, TValue value) { Writer.WriteAttrWithPrefix("data-" + key.ToLower(), value?.ToString()); return _this; }
		public T Dir(Dir value) { Writer.WriteAttr("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) Writer.WriteAttr("draggable", "Draggable"); return _this; }
		public T Lang(string value) { Writer.WriteAttr("lang", value); return _this; }
		public T Role(string value) { Writer.WriteAttr("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { Writer.WriteAttr("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { Writer.WriteAttr("tabindex", value.ToString()); return _this; }
		public T Title(string value) { Writer.WriteAttr("title", value); return _this; }

		public T OnLoad(string value) { Writer.WriteAttr("onload", value, false); return _this; }
		public T OnResize(string value) { Writer.WriteAttr("onresize", value, false); return _this; }
		public T OnUnload(string value) { Writer.WriteAttr("onunload", value, false); return _this; }

		public T OnBlur(string value) { Writer.WriteAttr("onblur", value, false); return _this; }
		public T OnChange(string value) { Writer.WriteAttr("onchange", value, false); return _this; }
		public T OnFocus(string value) { Writer.WriteAttr("onfocus", value, false); return _this; }
		public T OnSelect(string value) { Writer.WriteAttr("onselect", value, false); return _this; }
		public T OnSubmit(string value) { Writer.WriteAttr("onsubmit", value, false); return _this; }

		public T OnKeyDown(string value) { Writer.WriteAttr("onkeydown", value, false); return _this; }
		public T OnKeyPress(string value) { Writer.WriteAttr("onkeypress", value, false); return _this; }
		public T OnKeyUp(string value) { Writer.WriteAttr("onkeyup", value, false); return _this; }

		public T OnClick(string value) { Writer.WriteAttr("onclick", value, false); return _this; }
		public T OnDblClick(string value) { Writer.WriteAttr("ondblclick", value, false); return _this; }
		public T OnDrag(string value) { Writer.WriteAttr("ondrag", value, false); return _this; }
		public T OnDragEnd(string value) { Writer.WriteAttr("ondragend", value, false); return _this; }
		public T OnDragEnter(string value) { Writer.WriteAttr("ondragenter", value, false); return _this; }
		public T OnDragLeave(string value) { Writer.WriteAttr("ondragleave", value, false); return _this; }
		public T OnDragOver(string value) { Writer.WriteAttr("ondragover", value, false); return _this; }
		public T OnDragStart(string value) { Writer.WriteAttr("ondragstart", value, false); return _this; }
		public T OnDrop(string value) { Writer.WriteAttr("ondrop", value, false); return _this; }
		public T OnMouseDown(string value) { Writer.WriteAttr("onmousedown", value, false); return _this; }
		public T OnMouseMove(string value) { Writer.WriteAttr("onmousemove", value, false); return _this; }
		public T OnMouseOut(string value) { Writer.WriteAttr("onmouseout", value, false); return _this; }
		public T OnMouseOver(string value) { Writer.WriteAttr("onmouseover", value, false); return _this; }
		public T OnMouseUp(string value) { Writer.WriteAttr("onmouseup", value, false); return _this; }

		public T Set(Action<T> attrs) { attrs?.Invoke(_this); return _this; }

		public T Data(IReadOnlyDictionary<string, string> d) 
		{
			if (d != null)
				foreach (var v in d)
					Data(v.Key, v.Value); 
			return _this; 
		}

		public T Extended<TValue>(string key, TValue value) { Data(key, value); return _this; }

		protected string GetName(string value)
		{
			value = value?.ToLower();
			value = value != null && value.StartsWith("#") ? value.Substring(1) : value;
			return value;
		}
	}

	public class TagAttributes : TagAttributes<TagAttributes>, IContentItemAttributes
	{
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.ID<TValue>(TValue value) => ID(value);
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Class(string value, bool replaceExisting) => Class(value, replaceExisting);
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Style(string value, bool replaceExisting) => Style(value, replaceExisting);
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Extended<TValue>(string key, TValue value) => Extended(key, value);
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Title(string value) => Title(value);
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Data<TValue>(string key, TValue value) => Data(key, value);
	}

	public class ATagAttributes : TagAttributes<ATagAttributes>
	{
		public ATagAttributes Href(string value) { Writer.WriteAttr("href", value); return this; }
		public ATagAttributes HrefLang(string value) { Writer.WriteAttr("hreflang", value); return this; }
		public ATagAttributes Media(string value) { Writer.WriteAttr("media", value); return this; }
		public ATagAttributes Rel(Rel value) { Writer.WriteAttr("rel", value.ToString().ToLower()); return this; }
		public ATagAttributes Target(Target value) { Writer.WriteAttr("target", value.ToString().ToLower()); return this; }
		public ATagAttributes Type(string value) { Writer.WriteAttr("type", value); return this; }
	}

	public class BaseTagAttributes : TagAttributes<BaseTagAttributes>
	{
		public BaseTagAttributes Href(string value) { Writer.WriteAttr("href", value); return this; }
		public BaseTagAttributes Target(Target value) { Writer.WriteAttr("target", value.ToString().ToLower()); return this; }
	}

	public class CanvasTagAttributes : TagAttributes<CanvasTagAttributes>
	{
		public CanvasTagAttributes Height(int value) { Writer.WriteAttr("height", value.ToString()); return this; }
		public CanvasTagAttributes Width(int value) { Writer.WriteAttr("width", value.ToString()); return this; }
	}

	// Col, Colgroup
	public class ColTagAttributes : TagAttributes<ColTagAttributes>
	{
		public ColTagAttributes Span(int value) { Writer.WriteAttr("span", value.ToString()); return this; }
	}

	public class FormTagAttributes : TagAttributes<FormTagAttributes>
	{
		public FormTagAttributes Action(string value) { Writer.WriteAttr("action", value); return this; }
		public FormTagAttributes EncType(string value) { Writer.WriteAttr("enctype", value); return this; }
		public FormTagAttributes Method(Method value) { Writer.WriteAttr("method", value.ToString().ToLower()); return this; }
		public FormTagAttributes Name(string value) { Writer.WriteAttr("name", value); return this; }
		public FormTagAttributes Target(Target value) { Writer.WriteAttr("target", value.ToString().ToLower()); return this; }
	}

	public class HtmlTagAttributes : TagAttributes<HtmlTagAttributes>
	{
		public HtmlTagAttributes Manifest(string value) { Writer.WriteAttr("manifest", value); return this; }
		public HtmlTagAttributes Xmlns(string value) { Writer.WriteAttr("xmlns", value); return this; }
		public HtmlTagAttributes Xmlns(string prefix, string value) { Writer.WriteAttr("xmlns:" + prefix, value); return this; }
	}

	public class IFrameTagAttributes : TagAttributes<IFrameTagAttributes>
	{
		public IFrameTagAttributes Src(string value) { Writer.WriteAttr("src", value, true); return this; }
	}

	public class ImgTagAttributes : TagAttributes<ImgTagAttributes>
	{
		public ImgTagAttributes Alt(string value) { Writer.WriteAttr("alt", value, true); return this; }
		public ImgTagAttributes Height(int value) { Writer.WriteAttr("height", value.ToString()); return this; }
		public ImgTagAttributes IsMap(bool value) { if (value) Writer.WriteAttr("ismap", "ismap"); return this; }
		public ImgTagAttributes Src(string value) { Writer.WriteAttr("src", value, true); return this; }
		public ImgTagAttributes UseMap(string value) { Writer.WriteAttr("usemap", value); return this; }
		public ImgTagAttributes Width(int value) { Writer.WriteAttr("width", value.ToString()); return this; }
	}

	public class InputTagAttributes : TagAttributes<InputTagAttributes>
	{
		public InputTagAttributes Name(string value) { Writer.WriteAttr("name", GetName(value)); return this; }

		public InputTagAttributes Accept(string value) { Writer.WriteAttr("accept", value); return this; }
		public InputTagAttributes Alt(string value) { Writer.WriteAttr("alt", value); return this; }
		public InputTagAttributes Autocomplete(bool value) { Writer.WriteAttr("autocomplete", value ? "on" : "off"); return this; }
		public InputTagAttributes Autofocus(bool value) { if (value) Writer.WriteAttr("autofocus", "autofocus"); return this; }
		public InputTagAttributes Checked(bool value) { if (value) Writer.WriteAttr("checked", "checked"); return this; }
		public InputTagAttributes Disabled(bool value) { if (value) Writer.WriteAttr("disabled", "disabled"); return this; }
		public InputTagAttributes FormAction(string value) { Writer.WriteAttr("formaction", value); return this; }
		public InputTagAttributes FormEnctype(string value) { Writer.WriteAttr("formenctype", value); return this; }
		public InputTagAttributes FormMethod(Method value) { Writer.WriteAttr("formmethod", value.ToString().ToLower()); return this; }
		public InputTagAttributes FormTarget(Target value) { Writer.WriteAttr("formtarget", value.ToString().ToLower()); return this; }
		public InputTagAttributes Height(int value) { Writer.WriteAttr("height", value.ToString()); return this; }
		public InputTagAttributes Max(string value) { Writer.WriteAttr("max", value); return this; }
		public InputTagAttributes MaxLength(int value) { Writer.WriteAttr("maxlength", value.ToString()); return this; }
		public InputTagAttributes Min(string value) { Writer.WriteAttr("min", value); return this; }
		public InputTagAttributes Multiple(bool value) { if (value) Writer.WriteAttr("multiple", "multiple"); return this; }
		public InputTagAttributes OnInput(string value) { Writer.WriteAttr("oninput", value, false); return this; }
		public InputTagAttributes Placeholder(string value) { Writer.WriteAttr("placeholder", value); return this; }
		public InputTagAttributes Readonly(bool value) { if (value) Writer.WriteAttr("readonly", "readonly"); return this; }
		public InputTagAttributes Size(int value) { Writer.WriteAttr("size", value.ToString()); return this; }
		public InputTagAttributes Src(string value) { Writer.WriteAttr("src", value); return this; }
		public InputTagAttributes Step(decimal value) { Writer.WriteAttr("step", value.ToString(CultureInfo.InvariantCulture)); return this; }
		public InputTagAttributes Type(InputType value) { Writer.WriteAttr("type", value.ToString().ToLower()); return this; }
		public InputTagAttributes Value(string value) { Writer.WriteAttr("value", value); return this; }
		public InputTagAttributes Width(int value) { Writer.WriteAttr("width", value.ToString()); return this; }
	}

	public class ButtonTagAttributes : TagAttributes<ButtonTagAttributes>
	{
		public ButtonTagAttributes Autofocus(bool value) { if (value) Writer.WriteAttr("autofocus", "autofocus"); return this; }
		public ButtonTagAttributes Disabled(bool value) { if (value) Writer.WriteAttr("disabled", "disabled"); return this; }
		public ButtonTagAttributes FormAction(string value) { Writer.WriteAttr("formaction", value); return this; }
		public ButtonTagAttributes FormEnctype(string value) { Writer.WriteAttr("formenctype", value); return this; }
		public ButtonTagAttributes FormMethod(Method value) { Writer.WriteAttr("formmethod", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes FormTarget(Target value) { Writer.WriteAttr("formtarget", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Name(string value) { Writer.WriteAttr("name", GetName(value)); return this; }
		public ButtonTagAttributes Type(ButtonType value) { Writer.WriteAttr("type", value.ToString().ToLower()); return this; }
		public ButtonTagAttributes Value(string value) { Writer.WriteAttr("value", value); return this; }
	}

	public class LabelTagAttributes : TagAttributes<LabelTagAttributes>
	{
		public LabelTagAttributes For(string value) { Writer.WriteAttrWithPrefix("for", value); return this; }
		public LabelTagAttributes Form(string value) { Writer.WriteAttr("form", value); return this; }
	}

	public class LiTagAttributes : TagAttributes<LiTagAttributes>
	{
		public LiTagAttributes Value(int value) { Writer.WriteAttr("value", value.ToString()); return this; }
	}

	public class LinkTagAttributes : TagAttributes<LinkTagAttributes>
	{
		public LinkTagAttributes Href(string value) { Writer.WriteAttr("href", value); return this; }
		public LinkTagAttributes HrefLang(string value) { Writer.WriteAttr("hreflang", value); return this; }
		public LinkTagAttributes Media(string value) { Writer.WriteAttr("media", value); return this; }
		public LinkTagAttributes Rel(LinkRel value) { Writer.WriteAttr("rel", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Target(Target value) { Writer.WriteAttr("target", value.ToString().ToLower()); return this; }
		public LinkTagAttributes Type(string value) { Writer.WriteAttr("type", value); return this; }
	}

	public class MetaTagAttributes : TagAttributes<MetaTagAttributes>
	{
		public MetaTagAttributes CharSet(string value) { Writer.WriteAttr("charset", value); return this; }
		public MetaTagAttributes Content(string value) { Writer.WriteAttr("content", value); return this; }
		public MetaTagAttributes HttpEquiv(string value) { Writer.WriteAttr("http-equiv", value); return this; }
		public MetaTagAttributes Name(string value) { Writer.WriteAttr("name", value); return this; }
	}

	public class OlTagAttributes : TagAttributes<OlTagAttributes>
	{
		public OlTagAttributes Start(int value) { Writer.WriteAttr("start", value.ToString()); return this; }
		public OlTagAttributes Type(string value) { Writer.WriteAttr("type", value); return this; }
	}

	public class OptionTagAttributes : TagAttributes<OptionTagAttributes>
	{
		public OptionTagAttributes Disabled(bool value) { if (value) Writer.WriteAttr("disabled", "disabled"); return this; }
		public OptionTagAttributes Label(string value) { Writer.WriteAttr("label", value); return this; }
		public OptionTagAttributes Selected(bool value) { if (value) Writer.WriteAttr("selected", "selected"); return this; }
		public OptionTagAttributes Value(string value) { Writer.WriteAttr("value", value); return this; }
	}

	public class QTagAttributes : TagAttributes<QTagAttributes>
	{
		public QTagAttributes Cite(string value) { Writer.WriteAttr("cite", value); return this; }
	}

	public class SelectTagAttributes : TagAttributes<SelectTagAttributes>
	{
		public SelectTagAttributes Name(string value)
		{
			Writer.WriteAttr("name", GetName(value)); return this;
		}

		public SelectTagAttributes Autofocus(bool value) { if (value) Writer.WriteAttr("autofocus", "autofocus"); return this; }
		public SelectTagAttributes Disabled(bool value) { if (value) Writer.WriteAttr("disabled", "disabled"); return this; }
		public SelectTagAttributes Multiple(bool value) { if (value) Writer.WriteAttr("multiple", "multiple"); return this; }
		public SelectTagAttributes Size(int value) { Writer.WriteAttr("size", value.ToString()); return this; }
	}

	public class ScriptTagAttributes : TagAttributes<ScriptTagAttributes>
	{
		public ScriptTagAttributes CharSet(string value) { Writer.WriteAttr("type", value); return this; }
		public ScriptTagAttributes Src(string value) { Writer.WriteAttr("src", value); return this; }
		public ScriptTagAttributes Type(string value) { Writer.WriteAttr("type", value); return this; }
		public ScriptTagAttributes Async() { Writer.WriteAttr("async", "async"); return this; }
	}

	public class TdTagAttributes : TagAttributes<TdTagAttributes>, ITdAttributes
	{
		public TdTagAttributes ColSpan(int value) { Writer.WriteAttr("colspan", value.ToString()); return this; }
		public TdTagAttributes Headers(string value) { Writer.WriteAttr("headers", value); return this; }
		public TdTagAttributes RowSpan(int value) { Writer.WriteAttr("rowspan", value.ToString()); return this; }

		ITdAttributes IContentItemAttributes<ITdAttributes>.ID<TValue>(TValue value) => ID(value);
		ITdAttributes IContentItemAttributes<ITdAttributes>.Class(string value, bool replaceExisting) => Class(value, replaceExisting);
		ITdAttributes IContentItemAttributes<ITdAttributes>.Style(string value, bool replaceExisting) => Style(value, replaceExisting);
		ITdAttributes IContentItemAttributes<ITdAttributes>.Extended<TValue>(string key, TValue value) => Extended(key, value);
		ITdAttributes IContentItemAttributes<ITdAttributes>.Title(string value) => Title(value);
		ITdAttributes IContentItemAttributes<ITdAttributes>.Data<TValue>(string key, TValue value) => Data(key, value);
		ITdAttributes ITdAttributes.ColSpan(int value) => ColSpan(value);
		ITdAttributes ITdAttributes.RowSpan(int value) => RowSpan(value);
	}

	public class TextAreaTagAttributes : TagAttributes<TextAreaTagAttributes>
	{
		public TextAreaTagAttributes Name(string value)
		{
			Writer.WriteAttr("name", GetName(value)); return this;
		}

		public TextAreaTagAttributes Autofocus(bool value) { if (value) Writer.WriteAttr("autofocus", "autofocus"); return this; }
		public TextAreaTagAttributes Cols(int value) { Writer.WriteAttr("cols", value.ToString()); return this; }
		public TextAreaTagAttributes Disabled(bool value) { if (value) Writer.WriteAttr("disabled", "disabled"); return this; }
		public TextAreaTagAttributes Placeholder(string value) { Writer.WriteAttr("placeholder", value); return this; }
		public TextAreaTagAttributes Readonly(bool value) { if (value) Writer.WriteAttr("readonly", "readonly"); return this; }
		public TextAreaTagAttributes Rows(int value) { Writer.WriteAttr("rows", value.ToString()); return this; }
		public TextAreaTagAttributes Wrap(Wrap value) { Writer.WriteAttr("wrap", value.ToString().ToLower()); return this; }
	}

	public class ThTagAttributes : TagAttributes<ThTagAttributes>, IThAttributes
	{
		public ThTagAttributes Abbr(string value) { Writer.WriteAttr("abbr", value); return this; }
		public ThTagAttributes ColSpan(int value) { Writer.WriteAttr("colspan", value.ToString()); return this; }
		public ThTagAttributes Headers(string value) { Writer.WriteAttr("headers", value); return this; }
		public ThTagAttributes RowSpan(int value) { Writer.WriteAttr("rowspan", value.ToString()); return this; }
		public ThTagAttributes Scope(Scope value) { Writer.WriteAttr("scope", value.ToString().ToLower()); return this; }

		IThAttributes IContentItemAttributes<IThAttributes>.ID<TValue>(TValue value) => ID(value);
		IThAttributes IContentItemAttributes<IThAttributes>.Class(string value, bool replaceExisting) => Class(value, replaceExisting);
		IThAttributes IContentItemAttributes<IThAttributes>.Style(string value, bool replaceExisting) => Style(value, replaceExisting);
		IThAttributes IContentItemAttributes<IThAttributes>.Extended<TValue>(string key, TValue value) => Extended(key, value);
		IThAttributes IContentItemAttributes<IThAttributes>.Title(string value) => Title(value);
		IThAttributes IContentItemAttributes<IThAttributes>.Data<TValue>(string key, TValue value) => Data(key, value);
		IThAttributes IThAttributes.ColSpan(int value) => ColSpan(value);
		IThAttributes IThAttributes.RowSpan(int value) => RowSpan(value);
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
			var key = "ref-" + id;
			if (!ContainsKey(key))
				Add(key, id);
			return this;
		}

		public DataCollection Ref(IViewElement owner, string id)
		{
			var clientid = owner.GetClientID(id);
			var key = "ref-" + clientid;
			if (!ContainsKey(key))
				Add(key, clientid);
			return this;
		}

		public DataCollection Parm<T>(string key, T value)
		{
			var pkey = "p-" + key;
			if (!ContainsKey(pkey))
				Add(pkey, value?.ToString());
			return this;
		}

		//public DataCollection Add(IReadOnlyDictionary<string, string> source)
		//{
		//	foreach(var d in source)
		//		Add(d.Key, d.Value);
		//	return this;
		//}
	}
}