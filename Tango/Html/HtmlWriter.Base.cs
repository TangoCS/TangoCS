using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections;
using Tango.UI;

namespace Tango.Html
{
	//public partial class HtmlWriter : StringWriter, IContentWriter
	//{
	//	public const string NEWLINE = "<br/>";
	//	public const string SVGPATH = "/data/icons/svg";

	//	Stack<string> _ids = new Stack<string>(4);

	//	public string IDPrefix { get; private set; }

	//	public HtmlWriter()
	//	{
	//		NewLine = NEWLINE;
	//	}
	//	public HtmlWriter(StringBuilder sb) : base(sb)
	//	{
	//		NewLine = NEWLINE;
	//	}
	//	public HtmlWriter(string idPrefix)
	//	{
	//		NewLine = NEWLINE;
	//		if (idPrefix != null)
	//			idPrefix.Split('_').ForEach(s => _ids.Push(s));
	//		SetPrefix();
	//	}
	//	public HtmlWriter(string idPrefix, StringBuilder sb) : base(sb)
	//	{
	//		NewLine = NEWLINE;
	//		if (idPrefix != null)
	//			idPrefix.Split('_').ForEach(s => _ids.Push(s));
	//		SetPrefix();
	//	}

	//	public bool IsEmpty()
	//	{
	//		return GetStringBuilder().Length > 0;
	//	}

	//	public override void WriteLine(string value)
	//	{
	//		Write(value);
	//		Write("<br/>");
	//	}

	//	public void PushPrefix(string prefix)
	//	{
	//		_ids.Push(prefix.ToLower());
	//		SetPrefix();
	//	}

	//	public void PopPrefix()
	//	{
	//		_ids.Pop();
	//		SetPrefix();
	//	}

	//	void SetPrefix()
	//	{
	//		IDPrefix = String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s)).Reverse());
	//	}

	//	IDictionary<string, string> attributes = new Dictionary<string, string>(7, StringComparer.Ordinal);

	//	CultureInfo ru = new CultureInfo("ru-RU");

	//	void WriteContentTag<T>(string name, Action<T> attrs, Action inner)
	//		where T : class, IContentItemAttributes<T>
	//	{
	//		Write('<');
	//		Write(name);
	//		var o = Fabric<T>();
	//		o.SetWriter(this);
	//		attrs?.Invoke(o);
	//		RenderAttrs();
	//		Write('>');
	//		inner?.Invoke();
	//		Write("</");
	//		Write(name);
	//		Write('>');
	//	}

	//	public void WriteTag<T>(string name, Action<T> attrs, Action inner)
	//		where T : TagAttributes<T>, new()
	//	{
	//		Write('<');
	//		Write(name);
	//		if (attrs != null)
	//		{
	//			var o = new T();
	//			o.SetWriter(this);
	//			attrs?.Invoke(o);
	//			RenderAttrs();
	//		}
	//		Write('>');
	//		inner?.Invoke();
	//		Write("</");
	//		Write(name);
	//		Write('>');
	//	}

	//	public void WriteBeginTag<T>(string name, Action<T> attrs)
	//		where T : TagAttributes<T>, new()
	//	{
	//		Write('<');
	//		Write(name);
	//		if (attrs != null)
	//		{
	//			var o = new T();
	//			o.SetWriter(this);
	//			attrs?.Invoke(o);
	//			RenderAttrs();
	//		}
	//		Write('>');
	//	}

	//	public void WriteEndTag(string name)
	//	{
	//		Write("</");
	//		Write(name);
	//		Write('>');
	//	}

	//	public void WriteSelfClosingTag<T>(string name, Action<T> attrs)
	//		where T : TagAttributes<T>, new()
	//	{
	//		Write('<');
	//		Write(name);
	//		if (attrs != null)
	//		{
	//			var o = new T();
	//			o.SetWriter(this);
	//			attrs?.Invoke(o);
	//			RenderAttrs();
	//		}
	//		Write("/>");
	//	}

	//	public void WriteAttr(string key, string value, bool replaceExisting = true)
	//	{
	//		if (replaceExisting || (!string.IsNullOrEmpty(value) && !attributes.ContainsKey(key)))
	//			attributes[key] = value;
	//		else if (!string.IsNullOrEmpty(value))
	//			attributes[key] = attributes[key] + " " + value;
	//	}

	//	public void WriteAttr(string key)
	//	{
	//		attributes[key] = null;
	//	}

	//	public void WriteAttrID(string key, string value)
	//	{
	//		attributes[key] = value == null ?
	//			IDPrefix :
	//			this.GetID(value);
	//	}

	//	public void WriteAttrWithPrefix(string key, string value) => WriteAttrID(key, value);

	//	public void RenderAttrs()
	//	{
	//		foreach (var attribute in attributes)
	//		{
	//			if (attribute.Value != null)
	//				Write($" {attribute.Key}=\"{attribute.Value}\"");
	//			else
	//				Write($" {attribute.Key}");
	//		}
	//		attributes.Clear();
	//	}

	//	public string Write(IEnumerable<string> text)
	//	{
	//		return String.Join(NEWLINE, text);
	//	}
	//}

	public partial class HtmlWriter : IContentWriter
	{
		public const string NEWLINE = "<br/>";
		public const string SVGPATH = "/data/icons/svg";
		CultureInfo ru = new CultureInfo("ru-RU");
		public bool AllowModify => true;

		Stack<string> _ids = new Stack<string>(4);
		Stack<Stack<string>> _idsStates = new Stack<Stack<string>>(2);
		Node _root = new Node();
		Node _curParent;
		Element _curElement;
		Stack<Element> _unclosedElements = new Stack<Element>(4);
		Dictionary<string, Element> _idElements = new Dictionary<string, Element>();

		public string IDPrefix { get; private set; }
		public string NewLine => NEWLINE;

		public HtmlWriter(string idPrefix = null)
		{
			SetPrefix(idPrefix);
			_curParent = _root;
		}

		public bool IsEmpty()
		{
			return _root.FirstChild == null;
		}

		public void Write<T>(T value) { _curParent.AppendChildText(value); }
		public void Write(string value) { _curParent.AppendChildText(value); }

		public void PushPrefix(string prefix)
		{
			_ids.Push(prefix.ToLower());
			SetPrefix();
		}

		public void PopPrefix()
		{
			_ids.Pop();
			SetPrefix();
		}

		public void WithPrefix(string idPrefix, Action content)
		{
			_idsStates.Push(_ids);
			_ids = new Stack<string>();
			SetPrefix(idPrefix);
			content();
			_ids = _idsStates.Pop();
			SetPrefix();
		}

		void SetPrefix(string idPrefix)
		{
			if (idPrefix != null)
			{
				idPrefix.Split('_').ForEach(s => _ids.Push(s));
				SetPrefix();
			}
		}
		void SetPrefix()
		{
			IDPrefix = String.Join("_", _ids.Where(s => !string.IsNullOrEmpty(s)).Reverse());
		}

		public void WriteTag<T>(string name, Action<T> attrs, Action inner)
			where T : TagAttributes<T>, new()
		{
			var el = new Element { TagName = name };
			_curParent.AppendChild(el);
			_curElement = el;

			if (attrs != null)
			{
				var o = new T();
				o.SetWriter(this);
				attrs?.Invoke(o);
			}

			if (inner != null)
			{
				_curParent = el;
				inner.Invoke();
				_curParent = el.ParentNode;
				_curElement = el;
			}
		}

		public void WriteBeginTag<T>(string name, Action<T> attrs)
			where T : TagAttributes<T>, new()
		{
			var el = new Element { TagName = name };
			_curParent.AppendChild(el);
			_curElement = el;

			if (attrs != null)
			{
				var o = new T();
				o.SetWriter(this);
				attrs?.Invoke(o);
			}

			_curParent = el;
			_unclosedElements.Push(el);
		}

		public void WriteEndTag(string name)
		{
			var el = _unclosedElements.Pop();
			_curParent = el.ParentNode;
			_curElement = el;
		}

		void WriteContentTag<T>(string name, Action<T> attrs, Action inner)
			where T : class, IContentItemAttributes<T>
		{
			var el = new Element { TagName = name };
			_curParent.AppendChild(el);
			_curElement = el;

			if (attrs != null)
			{
				var o = Fabric<T>();
				o.SetWriter(this);
				attrs?.Invoke(o);
			}

			if (inner != null)
			{
				_curParent = el;
				inner.Invoke();
				_curParent = el.ParentNode;
			}
		}

		public void WriteSelfClosingTag<T>(string name, Action<T> attrs)
			where T : TagAttributes<T>, new()
		{
			WriteTag(name, attrs, null);
			_curElement.IsSelfClosing = true;
		}

		public void WriteAttr(string key, string value, bool replaceExisting = true)
		{
			if (_curElement.Attributes == null)
				_curElement.Attributes = new List<ElementAttribute>();

			for (int i = 0; i < _curElement.Attributes.Count; i++)
			{
				if (_curElement.Attributes[i].Name == key)
				{
					if (!replaceExisting)
						_curElement.Attributes[i].Value.Add(value);
					else
						_curElement.Attributes[i] = new ElementAttribute { Name = key, Value = new List<string> { value } };
					return;
				}
			}

			_curElement.Attributes.Add(new ElementAttribute { Name = key, Value = new List<string> { value } });
		}

		public void WriteAttr(string key)
			=> WriteAttr(key, null);

		public void WriteAttrID(string key, string value)
		{
			value = value == null ? IDPrefix : HtmlWriterHelpers.GetID(IDPrefix, value);
			_curElement.ID = value;
			_idElements[value] = _curElement;
			WriteAttr(key, value);
		}

		public void WriteAttrWithPrefix(string key, string value)
		{
			value = value == null ? IDPrefix : HtmlWriterHelpers.GetID(IDPrefix, value);
			WriteAttr(key, value);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			_root.Serialize(sb);
			return sb.ToString();
		}

		public string Write(IEnumerable<string> text)
		{
			return String.Join(NEWLINE, text);
		}

		Element GetElementByID(string id)
		{
			if (!_idElements.TryGetValue(id, out var el))
				return null;

			if (el.IsDeleted)
				return null;

			var p = el.ParentNode;
			while (p != null)
			{
				if (p.IsDeleted)
					return null;
				p = p.ParentNode;
			}

			return el;
		}

		public bool AddWidget(string id, HtmlWriter w)
		{
			var el = GetElementByID(id);
			if (el == null)
				return false;

			el.RemoveChildren();
			foreach (var child in w._root.Children.ToList())
				el.AppendChild(child);

			foreach (var kv in w._idElements)
				_idElements[kv.Key] = kv.Value;

			return true;
		}

		public bool ReplaceWidget(string id, HtmlWriter w)
		{
			var el = GetElementByID(id);
			if (el == null)
				return false;

			var newel = w._root.FirstChild as Element;
			if (newel == null)
				return false;

			el.InsertBefore(newel);
			el.Remove();

			foreach (var kv in w._idElements)
				_idElements[kv.Key] = kv.Value;

			return true;
		}

		public bool AddAdjacentWidget(string parentid, AdjacentHTMLPosition position, HtmlWriter w)
		{
			var el = GetElementByID(parentid);
			if (el == null)
				return false;

			switch (position)
			{
				case AdjacentHTMLPosition.BeforeEnd:
					foreach (var child in w._root.Children.ToList())
					{
						if (child is Element childEl && !childEl.ID.IsEmpty())
						{
							var existedEl = GetElementByID(childEl.ID);

							if (existedEl != null)
								existedEl.Remove();
						}

						el.AppendChild(child);
					}
					break;
				case AdjacentHTMLPosition.BeforeBegin:
					foreach (var child in w._root.Children.ToList())
						el.InsertAdjacent(AdjacentHTMLPosition.BeforeBegin, child);
					break;
				case AdjacentHTMLPosition.AfterBegin:
					foreach (var child in w._root.ChildrenReversed.ToList())
					{
						if (child is Element childEl && !childEl.ID.IsEmpty())
						{
							var existedEl = GetElementByID(childEl.ID);

							if (existedEl != null)
								existedEl.Remove();
						}

						el.PrependChild(child);
					}
					break;
				case AdjacentHTMLPosition.AfterEnd:
					Node cur = el;
					foreach (var child in w._root.Children.ToList())
					{
						cur.InsertAdjacent(AdjacentHTMLPosition.AfterEnd, child);
						cur = child;
					}
					break;
				default:
					break;
			}

			foreach (var kv in w._idElements)
				_idElements[kv.Key] = kv.Value;

			return true;
		}


	}


	public class AbsoluteID
	{
		string Value;

		public string AsRelative() => Value?.Substring(1);

		public static implicit operator AbsoluteID(string value)
		{
			return new AbsoluteID {	Value = "#" + value };
		}
		public static implicit operator string(AbsoluteID id)
		{
			return id.Value;
		}
		public override string ToString()
		{
			return Value;
		}
	}
}