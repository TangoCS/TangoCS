﻿using System;
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
		public T Data<TValue>(string key, TValue value, bool replaceExisting = true) { Writer.WriteAttr("data-" + key.ToLower(), value?.ToString(), replaceExisting); return _this; }
		public T Data(string key) { Writer.WriteAttr("data-" + key.ToLower(), null); return _this; }
		public T DataIDValue<TValue>(string key, TValue value, bool replaceExisting = true) { Writer.WriteAttrWithPrefix("data-" + key.ToLower(), value?.ToString(), replaceExisting); return _this; }
		public T Dir(Dir value) { Writer.WriteAttr("dir", value.ToString().ToLower()); return _this; }
		public T Draggable(bool value) { if (value) Writer.WriteAttr("draggable", "Draggable"); return _this; }
		public T Lang(string value) { Writer.WriteAttr("lang", value); return _this; }
		public T Role(string value) { Writer.WriteAttr("role", value); return _this; }
		public T Style(string value, bool replaceExisting = false) { Writer.WriteAttr("style", value, replaceExisting); return _this; }
		public T TabIndex(int value) { Writer.WriteAttr("tabindex", value.ToString()); return _this; }
		public T Title(string value) { Writer.WriteAttr("title", value); return _this; }

		public T OnLoad(string value, bool replaceExisting = false) { Writer.WriteAttr("onload", value, replaceExisting); return _this; }
		public T OnResize(string value, bool replaceExisting = false) { Writer.WriteAttr("onresize", value, replaceExisting); return _this; }
		public T OnUnload(string value, bool replaceExisting = false) { Writer.WriteAttr("onunload", value, replaceExisting); return _this; }

		public T OnBlur(string value, bool replaceExisting = false) { Writer.WriteAttr("onblur", value, replaceExisting); return _this; }
		public T OnChange(string value, bool replaceExisting = false) { Writer.WriteAttr("onchange", value, replaceExisting); return _this; }
		public T OnFocus(string value, bool replaceExisting = false) { Writer.WriteAttr("onfocus", value, replaceExisting); return _this; }
		public T OnSelect(string value, bool replaceExisting = false) { Writer.WriteAttr("onselect", value, replaceExisting); return _this; }
		public T OnSubmit(string value, bool replaceExisting = false) { Writer.WriteAttr("onsubmit", value, replaceExisting); return _this; }

		public T OnKeyDown(string value, bool replaceExisting = false) { Writer.WriteAttr("onkeydown", value, replaceExisting); return _this; }
		public T OnKeyPress(string value, bool replaceExisting = false) { Writer.WriteAttr("onkeypress", value, replaceExisting); return _this; }
		public T OnKeyUp(string value, bool replaceExisting = false) { Writer.WriteAttr("onkeyup", value, replaceExisting); return _this; }

		public T OnClick(string value, bool replaceExisting = false) { Writer.WriteAttr("onclick", value, replaceExisting); return _this; }
		public T OnDblClick(string value, bool replaceExisting = false) { Writer.WriteAttr("ondblclick", value, replaceExisting); return _this; }
		public T OnDrag(string value, bool replaceExisting = false) { Writer.WriteAttr("ondrag", value, replaceExisting); return _this; }
		public T OnDragEnd(string value, bool replaceExisting = false) { Writer.WriteAttr("ondragend", value, replaceExisting); return _this; }
		public T OnDragEnter(string value, bool replaceExisting = false) { Writer.WriteAttr("ondragenter", value, replaceExisting); return _this; }
		public T OnDragLeave(string value, bool replaceExisting = false) { Writer.WriteAttr("ondragleave", value, replaceExisting); return _this; }
		public T OnDragOver(string value, bool replaceExisting = false) { Writer.WriteAttr("ondragover", value, replaceExisting); return _this; }
		public T OnDragStart(string value, bool replaceExisting = false) { Writer.WriteAttr("ondragstart", value, replaceExisting); return _this; }
		public T OnDrop(string value, bool replaceExisting = false) { Writer.WriteAttr("ondrop", value, replaceExisting); return _this; }
		public T OnMouseDown(string value, bool replaceExisting = false) { Writer.WriteAttr("onmousedown", value, replaceExisting); return _this; }
		public T OnMouseMove(string value, bool replaceExisting = false) { Writer.WriteAttr("onmousemove", value, replaceExisting); return _this; }
		public T OnMouseOut(string value, bool replaceExisting = false) { Writer.WriteAttr("onmouseout", value, replaceExisting); return _this; }
		public T OnMouseOver(string value, bool replaceExisting = false) { Writer.WriteAttr("onmouseover", value, replaceExisting); return _this; }
		public T OnMouseUp(string value, bool replaceExisting = false) { Writer.WriteAttr("onmouseup", value, replaceExisting); return _this; }

		public T Set(Action<T> attrs) { attrs?.Invoke(_this); return _this; }

		public T Data(IReadOnlyDictionary<string, string> d) 
		{
			if (d != null)
				foreach (var v in d)
					Data(v.Key, v.Value, false); 
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
		IContentItemAttributes IContentItemAttributes<IContentItemAttributes>.Data<TValue>(string key, TValue value, bool replaceExisting) => Data(key, value, replaceExisting);
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
		public InputTagAttributes Autocomplete(string value) { Writer.WriteAttr("autocomplete", value); return this; }
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
		ITdAttributes IContentItemAttributes<ITdAttributes>.Data<TValue>(string key, TValue value, bool replaceExisting) => Data(key, value, replaceExisting);
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
		IThAttributes IContentItemAttributes<IThAttributes>.Data<TValue>(string key, TValue value, bool replaceExisting) => Data(key, value, replaceExisting);
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
		public const string RefKey = "ref";
		public const string RefSessionStorageKey = "ref-sessionstorage";

		public DataCollection Ref(string id)
		{
			if (!ContainsKey(RefKey))
				Add(RefKey, id);
			else
				this[RefKey] += " " + id;
			return this;
		}

		public DataCollection Ref(IViewElement owner, string id)
		{
			var clientid = owner.GetClientID(id);
			if (!ContainsKey(RefKey))
				Add(RefKey, clientid);
			else
				this[RefKey] += " " + clientid;
			return this;
		}

		public DataCollection RefSessionStorage(string id)
		{
			if (!ContainsKey(RefSessionStorageKey))
				Add(RefSessionStorageKey, id);
			else
				this[RefSessionStorageKey] += " " + id;
			return this;
		}

		public DataCollection RefSessionStorage(IViewElement owner, string id)
		{
			var clientid = owner.GetClientID(id);
			if (!ContainsKey(RefSessionStorageKey))
				Add(RefSessionStorageKey, clientid);
			else
				this[RefSessionStorageKey] += " " + clientid;
			return this;
		}

		public DataCollection Parm<T>(string key, T value)
		{
			var pkey = "p-" + key;
			if (!ContainsKey(pkey))
				Add(pkey, value?.ToString());
			return this;
		}

		public void CopyRefFrom(DataCollection src)
		{
			if (src.ContainsKey(DataCollection.RefKey))
				Ref(src[DataCollection.RefKey]);
		}

		public void CopyParmsFrom(DataCollection src)
		{
			foreach (var key in src.Keys)
				if (key.StartsWith("p-"))
					if (!ContainsKey(key))
						Add(key, src[key]);
		}

		//public DataCollection Add(IReadOnlyDictionary<string, string> source)
		//{
		//	foreach(var d in source)
		//		Add(d.Key, d.Value);
		//	return this;
		//}
	}

	public class Autocomplete
	{
		public static readonly string On = "on";
		public static readonly string Off = "off";
		public static readonly string Name = "name";
		public static readonly string HonorificPrefix = "honorific-prefix";
		public static readonly string GivenName = "given-name";
		public static readonly string AdditionalName = "additional-name";
		public static readonly string FamilyName = "family-name";
		public static readonly string HonorificSuffix = "honorific-suffix";
		public static readonly string Nickname = "nickname";
		public static readonly string Email = "email";
		public static readonly string Username = "username";
		public static readonly string NewPassword = "new-password";
		public static readonly string CurrentPassword = "current-password";
		public static readonly string OrganizationTitle = "organization-title";
		public static readonly string Organization = "organization";
		public static readonly string StreetAddress = "street-address";
		public static readonly string AddressLine1 = "address-line1";
		public static readonly string AddressLine2 = "address-line2";
		public static readonly string AddressLine3 = "address-line3";
		public static readonly string AddressLevel1 = "address-level1";
		public static readonly string AddressLevel2 = "address-level2";
		public static readonly string AddressLevel3 = "address-level3";
		public static readonly string AddressLevel4 = "address-level4";
		public static readonly string Country = "country";
		public static readonly string CountryName = "country-name";
		public static readonly string PostalCode = "postal-code";
		public static readonly string CcName = "cc-name";
		public static readonly string CcGivenName = "cc-given-name";
		public static readonly string CcAdditionalName = "cc-additional-name";
		public static readonly string CcFamilyName = "cc-family-name";
		public static readonly string CcNumber = "cc-number";
		public static readonly string CcExp = "cc-exp";
		public static readonly string CcExpMonth = "cc-exp-month";
		public static readonly string CcExpYear = "cc-exp-year";
		public static readonly string CcCsc = "cc-csc";
		public static readonly string CcType = "cc-type";
		public static readonly string TransactionCurrency = "transaction-currency";
		public static readonly string TransactionAmount = "transaction-amount";
		public static readonly string Language = "language";
		public static readonly string Birthday = "bday";
		public static readonly string BirthdayDay = "bday-day";
		public static readonly string BirthdayMonth = "bday-month";
		public static readonly string BirthdayYear = "bday-year";
		public static readonly string Sex = "sex";
		public static readonly string Url = "url";
		public static readonly string Photo = "photo";
	}
}