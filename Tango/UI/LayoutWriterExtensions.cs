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

        public static void BlockCollapsible(this LayoutWriter w, Action<FieldsBlockCollapsibleOptions> inner)
        {
            var options = FieldsBlockCollapsibleOptions.Make(inner);
            var width = $"grid-column-end: span {(int)options.Grid};";
            var id = Guid.NewGuid().ToString();
            w.Div(a => {
                a.ID(id).Class("block block-collapsible").Style(width);
                if (options.IsCollapsed) a.Class("collapsed");
            }, () => {
                w.Div(a => a.Class("block-header"), () => {
                    var js = "domActions.toggleClass({id: '" + w.GetID(id) + "', clsName: 'collapsed' })";
                    w.Div(a => a.Class("block-header-left").OnClick(js), () =>
                    {
                        w.Div(a => a.Class("block-btn"), () => w.Icon("right"));
                        w.Div(a => a.Class("block-title-left"), options.LeftTitle?.GetAction(w));
                    });

                    if (options.RightTitle != null)
                    {
                        w.Div(a => a.Class("block-header-right"), () =>
                        {
                            w.Div(a => a.Class("block-title-right"), options.RightTitle?.GetAction(w));
                        });
                    }
                });
                options.Content?.Invoke(w);
            });
        }
    }
}
