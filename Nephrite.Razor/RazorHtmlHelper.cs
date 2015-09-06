using Nephrite.AccessControl;
using Nephrite.Html;
using Nephrite.Layout;
using Nephrite.MVC;
using Nephrite.Multilanguage;
using RazorEngine.Text;
using System.IO;
using System;

namespace Nephrite.Razor
{
	public class RazorHtmlHelper : HtmlHelper
	{
		public ILayoutList List { get; set; }
		public ILayoutForm Form { get; set; }

		public TextWriter Writer { get; set; }

		public RazorHtmlHelper(
			AbstractQueryString query,
			IUrlHelper urlHelper, 
			IAccessControl accessControl,
			ITextResource textResource,
            ILayoutList list, 
			ILayoutForm form) : base(query, urlHelper, accessControl, textResource)
        {
			List = list;
			Form = form;
        }

		public FormTable FormTableBegin(object attributes)
		{
			var t = new FormTable(Writer, Form);
			Writer.Write(Form.FormTableBegin(attributes));
			return t;
		}

		public ListTable ListTableBegin(object attributes)
		{
			var t = new ListTable(Writer, List);
			Writer.Write(List.ListTableBegin(attributes));
			return t;
		}

		public ButtonsBar ButtonsBarBegin(object attributes)
		{
			var b = new ButtonsBar(Writer, Form);
			Writer.Write(Form.ButtonsBarBegin(attributes));
			return b;
		}

		public IEncodedString GroupTitleBegin(string id)
		{
			return new RawString(Form.GroupTitleBegin(id));
		}

		public IEncodedString GroupTitleEnd()
		{
			return new RawString(Form.GroupTitleEnd());
		}

		public class FormTable : IDisposable
		{
			ILayoutForm _form;
			TextWriter _writer;

			public FormTable(TextWriter writer, ILayoutForm form)
			{
				_form = form;
				_writer = writer;
            }

			public IEncodedString FormRowBegin(string title, string comment, bool required,
				object rowAttributes, object labelAttributes, object requiredAttributes, object commentAttributes, object bodyAttributes)
			{
				return new RawString(_form.FormRowBegin(title, comment, required, rowAttributes, labelAttributes, requiredAttributes, commentAttributes, bodyAttributes));
			}

			public IEncodedString FormRowEnd()
			{
				return new RawString(_form.FormRowEnd());
			}

			public void Dispose()
			{
				_writer.Write(_form.FormTableEnd());
			}
		}

		public class ButtonsBar : IDisposable
		{
			ILayoutForm _form;
			TextWriter _writer;

			public ButtonsBar(TextWriter writer, ILayoutForm form)
			{
				_form = form;
				_writer = writer;
			}

			public IEncodedString WhiteSpace()
			{
				return new RawString(_form.ButtonsBarWhiteSpace());
			}

			public IEncodedString ItemBegin()
			{
				return new RawString(_form.ButtonsBarItemBegin());
			}

			public IEncodedString ItemEnd()
			{
				return new RawString(_form.ButtonsBarItemEnd());
			}

			public void Dispose()
			{
				_writer.Write(_form.ButtonsBarEnd());
			}
		}

		public class ListTable : IDisposable
		{
			ILayoutList _list;
			TextWriter _writer;

			public ListTable(TextWriter writer, ILayoutList list)
			{
				_list = list;
				_writer = writer;
			}

			public IEncodedString ListHeaderBegin(object attributes)
			{
				return new RawString(_list.ListHeaderBegin(attributes));
			}

			public IEncodedString THBegin(object attributes)
			{
				return new RawString(_list.THBegin(attributes));
			}

			public IEncodedString THEnd()
			{
				return new RawString(_list.THEnd());
			}

			public IEncodedString ListHeaderEnd()
			{
				return new RawString(_list.ListHeaderEnd());
			}

			public IEncodedString ListRowBegin(string cssClass, object attributes)
			{
				return new RawString(_list.ListRowBegin(cssClass, attributes));
			}

			public IEncodedString TDBegin(object attributes)
			{
				return new RawString(_list.TDBegin(attributes));
			}

			public IEncodedString TDEnd()
			{
				return new RawString(_list.TDEnd());
			}

			public IEncodedString ListRowEnd()
			{
				return new RawString(_list.ListRowEnd());
			}

			public void Dispose()
			{
				_writer.Write(_list.ListTableEnd());
			}
		}
	}
}
