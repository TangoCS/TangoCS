using System;
using Nephrite.Multilanguage;
using Nephrite.Templating;

namespace Nephrite.Html.Layout
{
	public class LayoutWriter : HtmlWriter
	{
		public ITextResource TextResource { get; private set; }
		public CsTemplateContext Context { get; private set; }

		public LayoutWriter(CsTemplateContext context, ITextResource textResource)
        {
			TextResource = textResource;
			Context = context;
        }

		public void FormTable(Action<TagAttributes> attributes, Action content)
		{
			this.Table(a => { a.Class("ms-formtable"); if (attributes != null) attributes(a); }, content);
		}

		public void FormRow(string id, string title, string comment, bool required, Action content)
		{
			this.Tr(a => a.ID(id), () => {
				this.Td(a => a.Class("ms-formlabel").Style("width:190px"), () => {
					Write(title);
					if (required)
						this.Span(a => a.Class("ms-formvalidation"), "&nbsp;*");
					if (!comment.IsEmpty())
						this.Div(
							a => a.Class("ms-descriptiontext").Style("font-weight:normal; margin:3px 0 0 1px"),
							() => Write(comment)
						);
				});
				this.Td(a => a.Class("ms-formbody"), content);
			});
		}

		public void FormRow(string id, string title, string comment, bool required, object content)
		{
			FormRow(id, title, comment, required, () =>
				Write(content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString())
			);
		}



		public void ListTable(Action<TagAttributes> attributes, Action<ListTableWriter> content)
		{
			new ListTableWriter(this, attributes, content);
		}

		public void ButtonsBar(Action<TagAttributes> attributes, Action<ButtonsBarWriter> content)
		{
			new ButtonsBarWriter(this, attributes, content);
		}

		public void GroupTitle(Action<TagAttributes> attributes, Action content)
		{
			this.Div(a => { a.Class("tabletitle"); attributes(a); }, content);
		}

		public void FormMargin(Action inner)
		{
			this.Div(a => a.Style("padding:8px"), inner);
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
