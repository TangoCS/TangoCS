using System;
using Tango.Html;
using Tango.Meta;
using Tango.Localization;
using Tango.UI.Controls;

namespace Tango.UI
{
	public static class LayoutWriterTablesExtensions
	{
		public static void Th(this LayoutWriter l, Action content)
		{
			l.Th(null, content);
		}
		public static void Th(this LayoutWriter l, Action<ThTagAttributes> attributes, string title)
		{
			l.Th(attributes, () => l.Write(title));
		}

		public static void Th(this LayoutWriter l, string title)
		{
			l.Th(null, () => l.Write(title));
		}

		public static void Th(this LayoutWriter l, Action<ThTagAttributes> attributes, IMetaProperty prop)
		{
			l.Th(attributes, l.Resources.CaptionShort(prop));
		}

		public static void Th(this LayoutWriter l, IMetaProperty prop)
		{
			l.Th(l.Resources.CaptionShort(prop));
		}

		public static void Td<T>(this LayoutWriter l, Action<TdTagAttributes> attributes, T? content)
			where T : struct
		{
			l.Td(attributes, () => l.Write(content?.ToString() ?? "&nbsp;"));
		}

		public static void GroupTitle(this LayoutWriter w, Action<TagAttributes> attributes, string content)
		{
			w.GroupTitle(attributes, () => w.Write(content));
		}

		public static void GroupTitle(this LayoutWriter w, Action content)
		{
			w.GroupTitle(null, content);
		}

		public static void GroupTitle(this LayoutWriter w, string content)
		{
			w.GroupTitle(null, content);
		}

		public static void LabelDefault(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-default"), text);
		}

		public static void LabelSuccess(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-success"), text);
		}

		public static void LabelWarning(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-warning"), text);
		}

		public static void LabelImportant(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-important"), text);
		}

		public static void LabelInfo(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-info"), text);
		}

		public static void LabelInverse(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-inverse"), text);
		}

        public static void LabelDanger(this LayoutWriter w, string text)
        {
            w.Span(a => a.Class("label label-danger"), text);
        }

        public static void BlockCollapsible(this LayoutWriter w, Action<BlockCollapsibleBuilder> inner, Action<BlockCollapsibleBuilder> collapsed = null)
        {
            var builder = BlockCollapsibleBuilder.Make(inner);
			collapsed?.Invoke(builder);
            var width = $"grid-column-end: span {(int)builder.Grid};";

            w.Div(a => {
				if (builder.ID.IsEmpty())
					builder.ID = Guid.NewGuid().ToString();

				a.ID(builder.ID).Class("block block-collapsible").Style(width);
                if (builder.IsCollapsed) a.Class("collapsed");
				a.Set(builder.ContainerAttributes);
            }, () => {
                w.Div(a => a.Class("block-header"), () => {
					var js = "domActions.toggleClass({id: '" + w.GetID(builder.ID) + "', clsName: 'collapsed' });";

					w.Div(a => a.Class("block-header-left").OnClick(js).Set(builder.BlockHeaderLeftAttributes), () => {
                        w.Div(a => a.Class("block-btn").Set(builder.BlockBtnAttributes), () => w.Icon("right"));
                        w.Div(a => a.Class("block-title-left").Set(builder.BlockTitleLeftAttributes), builder.LeftTitle?.GetAction(w));
                    });

                    if (builder.RightTitle != null)
                    {
                        w.Div(a => a.Class("block-header-right").Set(builder.BlockHeaderRightAttributes), () => {
                            w.Div(a => a.Class("block-title-right").Set(builder.BlockTitleRightAttributes), builder.RightTitle?.GetAction(w));
                        });
                    }
                });
                builder.Content?.Invoke(w);
            });
        }
    }
}
