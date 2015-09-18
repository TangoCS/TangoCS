using System;
using System.Text;
using Nephrite.Html;
using Nephrite.Html.Controls;
using Nephrite.Multilanguage;

namespace Nephrite.Layout
{
	public class LayoutWriter : HtmlWriter
	{
		public ISystemLayout Layout { get; private set; }
		public ITextResource TextResource { get; private set; }

		public LayoutWriter()
        {
			Layout = DI.GetService<ISystemLayout>();
			TextResource = DI.GetService<ITextResource>();
        }
		public LayoutWriter(StringBuilder sb) : base(sb)
		{
			Layout = DI.GetService<ISystemLayout>();
			TextResource = DI.GetService<ITextResource>();
		}


		public void FormTable(object attributes, Action<FormTableWriter> inner)
		{
			Write(Layout.Form.FormTableBegin(attributes));
			using (var w = new FormTableWriter(this, Layout.Form))
				inner(w);
        }

		public void ListTable(object attributes, Action<ListTableWriter> inner)
		{
			Write(Layout.List.ListTableBegin(attributes));
			using (var w = new ListTableWriter(this, Layout.List))
				inner(w);
		}

		public void ButtonsBar(object attributes, Action<ButtonsBarWriter> inner)
		{
			Write(Layout.Form.ButtonsBarBegin(attributes));
			using (var w = new ButtonsBarWriter(this, Layout.Form))
				inner(w);
		}

		public void Toolbar(Action<ToolbarWriter> inner)
		{
			Write(Layout.Toolbar.ToolbarBegin());
			using (var w = new ToolbarWriter(this, Layout.Toolbar))
				inner(w);
		}

		public void GroupTitleBegin(string id)
		{
			Write(Layout.Form.GroupTitleBegin(id));
		}

		public void GroupTitleEnd()
		{
			Write(Layout.Form.GroupTitleEnd());
		}

		public void ActionLink(string title, Action<ActionSimpleLink> linkSetup)
		{
			var l = new ActionSimpleLink();
			l.Title(title);
			linkSetup(l);
			Write(l);         
		}

		public void InternalLink(string onClick, string linkText)
		{
			Write(String.Format("<a href='#' onclick=\"{0}\">{1}</a>", onClick, linkText));
		}

		public void InternalImage(string onClick, string linkText, string image)
		{
			Write(String.Format("<a href='#' onclick=\"{0}\"><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>", onClick, linkText, image, IconSet.RootPath));
		}

		public void InternalImageLink(string onClick, string linkText, string image)
		{
			Write(String.Format("<a href='#' onclick=\"{0}\"><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>&nbsp;<a href='#' onclick='{0}'>{1}</a>", onClick, linkText, image, IconSet.RootPath));
		}

		public class FormTableWriter : IDisposable
		{
			ILayoutForm _form;
			HtmlWriter _writer;

			public HtmlWriter Writer { get { return _writer; } }

			public FormTableWriter(HtmlWriter writer, ILayoutForm form)
			{
				_form = form;
				_writer = writer;
			}

			public void FormRowBegin(string title, string comment, bool required,
				object rowAttributes, object labelAttributes, object requiredAttributes, object commentAttributes, object bodyAttributes)
			{
				_writer.Write(_form.FormRowBegin(title, comment, required, rowAttributes, labelAttributes, requiredAttributes, commentAttributes, bodyAttributes));
			}

			public void FormRowEnd()
			{
				_writer.Write(_form.FormRowEnd());
			}

			public void Dispose()
			{
				_writer.Write(_form.FormTableEnd());
			}
		}

		public class ButtonsBarWriter : IDisposable
		{
			ILayoutForm _form;
			HtmlWriter _writer;

			public HtmlWriter Writer { get { return _writer; } }

			public ButtonsBarWriter(HtmlWriter writer, ILayoutForm form)
			{
				_form = form;
				_writer = writer;
			}

			public void WhiteSpace()
			{
				_writer.Write(_form.ButtonsBarWhiteSpace());
			}

			public void ItemBegin()
			{
				_writer.Write(_form.ButtonsBarItemBegin());
			}

			public void ItemEnd()
			{
				_writer.Write(_form.ButtonsBarItemEnd());
			}

			public void Dispose()
			{
				_writer.Write(_form.ButtonsBarEnd());
			}
		}

		public class ListTableWriter : IDisposable
		{
			ILayoutList _list;
			HtmlWriter _writer;

			public HtmlWriter Writer { get { return _writer; } }

			public ListTableWriter(HtmlWriter writer, ILayoutList list)
			{
				_list = list;
				_writer = writer;
			}

			public void ListHeader(object attributes, Action columns)
			{
				_writer.Write(_list.ListHeaderBegin(attributes));
				columns();
                _writer.Write(_list.ListHeaderEnd());
			}

			public void THBegin(object attributes)
			{
				_writer.Write(_list.THBegin(attributes));
			}

			public void THEnd()
			{
				_writer.Write(_list.THEnd());
			}

			public void ListRowBegin(string cssClass, object attributes)
			{
				_writer.Write(_list.ListRowBegin(cssClass, attributes));
			}

			public void TDBegin(object attributes)
			{
				_writer.Write(_list.TDBegin(attributes));
			}

			public void TDEnd()
			{
				_writer.Write(_list.TDEnd());
			}

			public void ListRowEnd()
			{
				_writer.Write(_list.ListRowEnd());
			}

			public void Dispose()
			{
				_writer.Write(_list.ListTableEnd());
			}
		}

		public class ToolbarWriter : IDisposable
		{
			bool _canAddSeparator = false;
			int _partStatus = 0;

			ILayoutToolbar2 _layout;
			HtmlWriter _writer;

			public ToolbarWriter(HtmlWriter writer, ILayoutToolbar2 layout)
			{
				_layout = layout;
				_writer = writer;
			}

			public void Item(string content)
			{
				_canAddSeparator = true;
				if (_partStatus == 0)
				{
					_writer.Write(_layout.ToolbarPartBegin("ms-toolbar-left"));
				}
				if (_partStatus == 2)
				{
					_writer.Write(_layout.ToolbarPartEnd());
					_writer.Write(_layout.ToolbarPartBegin("ms-toolbar-right"));
				}
				_partStatus++;
				_writer.Write(_layout.ToolbarItem(content));
			}

			public void ItemSeparator()
			{
				if (!_canAddSeparator) return;
				_canAddSeparator = false;
				_writer.Write(_layout.ToolbarSeparator());
			}

			public void WhiteSpace()
			{
				if (_partStatus > 1) return;
				_canAddSeparator = false;
				if (_partStatus == 1)
				{
					_writer.Write(_layout.ToolbarPartEnd());
				}
				_partStatus++;
			}

			public void Dispose()
			{
				if (_partStatus != 0)
				{
					_writer.Write(_layout.ToolbarPartEnd());
				}
				_writer.Write(_layout.ToolbarEnd());
			}
		}
	}
}
