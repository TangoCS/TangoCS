using System;
using System.Drawing;
using System.Linq;
using Tango.Html;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
    public static class ValidationFormatter
    {
        public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val) => 
            ValidationBlockImpl(w, val, a => a.Class("widthstd"));

        public static LayoutWriter ValidationBlock100Percent(this LayoutWriter w, ValidationMessageCollection val) =>
            ValidationBlockImpl(w, val, a => a.Class("width100"));

        public static void ValidationBlockContainer(this LayoutWriter w, ValidationMessageSeverity severity, Action content, Action<TagAttributes> attributes = null)
        {
            var color =
                severity == ValidationMessageSeverity.Error ? "red" :
                severity == ValidationMessageSeverity.Warning ? "yellow" :
                "skyblue";

            Action title = () => {
                w.Icon("warning", a => a.Style("margin-right: 4px; color:" + color));
                w.Write("Предупреждение");
            };

            attributes += a => a.Class("validation-body").GridColumn(Grid.OneWhole);

            w.BlockCollapsible(opt =>
            {
                opt.SetLeftTitle(title)
                .SetContentFieldsBlock(() =>
                {
                    w.Div(attributes, () => {
                        content();
                    });
                });
            });
        }

        private static LayoutWriter ValidationBlockImpl(this LayoutWriter w, ValidationMessageCollection val, Action<TagAttributes> attributes = null)
        {
            var severity = val.Count == 0 ? ValidationMessageSeverity.Error :
               val.Any(o => o.Severity == ValidationMessageSeverity.Error) ? ValidationMessageSeverity.Error :
               val.Any(o => o.Severity == ValidationMessageSeverity.Warning) ? ValidationMessageSeverity.Warning :
               ValidationMessageSeverity.Information;

            w.ValidationBlockContainer(severity, () => {
                foreach (var item in val)
                    w.P(() => w.Write(item.Message));
            }, attributes);

            return w;
        }
    }
}