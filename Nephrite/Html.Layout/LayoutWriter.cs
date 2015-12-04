using System;
using System.Collections.Generic;
using Nephrite.Multilanguage;
using Nephrite.MVC;
using Nephrite.Templating;

namespace Nephrite.Html.Layout
{
	public class LayoutWriter : HtmlWriter
	{
		public ITextResource TextResource { get; private set; }
		public ActionContext Context { get; private set; }
		public List<ClientAction> ClientActions { get; set; }

		public LayoutWriter(ActionContext context, ITextResource textResource)
        {
			TextResource = textResource;
			Context = context;
			ClientActions = new List<ClientAction>();
		}

		public void ListTable(Action<TagAttributes> attributes, Action<ListTableWriter> content)
		{
			new ListTableWriter(this, attributes, content);
		}

		public void ButtonsBar(Action<TagAttributes> attributes, Action<ButtonsBarWriter> content)
		{
			new ButtonsBarWriter(this, attributes, content);
		}

		public class ButtonsBarWriter
		{
			LayoutWriter _w;

			public LayoutWriter Writer { get { return _w; } }

			public ButtonsBarWriter(LayoutWriter writer, Action<TagAttributes> attributes, Action<ButtonsBarWriter> content)
			{
				_w = writer;
				_w.Table(a => { a.Class("ms-formtoolbar"); if (attributes != null) attributes(a); }, () => content(this));
			}

			public void WhiteSpace()
			{
				_w.Td(a => a.Style("width:100%"));
			}

			public void Item(Action content)
			{
				_w.Td(a => a.Style("vertical-align:middle"), content);
			}
		}

		public class ListTableWriter
		{
			LayoutWriter _w;

			public LayoutWriter Writer { get { return _w; } }

			public ListTableWriter(LayoutWriter writer, Action<TagAttributes> attributes, Action<ListTableWriter> content)
			{
				_w = writer;
				_w.Table(a => { a.Class("ms-listviewtable"); if (attributes != null) attributes(a); }, () => content(this));
			}

			public void ListHeader(Action<TagAttributes> attributes, Action columns)
			{
				_w.Tr(a => { a.Class("ms-viewheadertr"); if (attributes != null) attributes(a); }, columns);
			}

			public void ColumnHeader(Action<ThTagAttributes> attributes, Action content)
			{
				_w.Th(a => {
					a.Class("ms-vh2"); if (attributes != null) attributes(a); }, 
					() => _w.Div(a => a.Class("ms-vb"), content)
				);
			}

			public void ListRow(Action<TagAttributes> attributes, Action cells)
			{
				_w.Tr(attributes, cells);
			}

			public void Cell(Action<TdTagAttributes> attributes, Action content)
			{
				_w.Td(a => { a.Class("ms-vb2"); if (attributes != null) attributes(a); }, content);
			}
		}

	}	
}
