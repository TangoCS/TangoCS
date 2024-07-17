using System;
using System.Drawing;
using System.Linq;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
    public static class ValidationFormatter
    {
        public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val, Action<BlockCollapsibleBuilder> collapsed = null) => 
            ValidationBlockImpl(w, val, a => a.Class("widthstd"), collapsed);

        public static LayoutWriter ValidationBlock100Percent(this LayoutWriter w, ValidationMessageCollection val, Action<BlockCollapsibleBuilder> collapsed = null) =>
            ValidationBlockImpl(w, val, a => a.Class("width100"), collapsed);

        public static void ValidationBlockContainer(this LayoutWriter w, ValidationMessageSeverity severity, Action content, Action<TagAttributes> attributes = null, Action<BlockCollapsibleBuilder> collapsed = null)
        {
            var color =
                severity == ValidationMessageSeverity.Error ? "red" :
                severity == ValidationMessageSeverity.Warning ? "yellow" :
                "skyblue";

			Action title = () => {
                w.Icon("warning", a => a.Style("margin-right: 4px; color:" + color));
                w.Write(w.Resources.Get(GetResName(severity)));
            };

            attributes += a => a.Class("validation-body").GridColumn(Grid.OneWhole);

            w.BlockCollapsible(opt => {
                opt.SetLeftTitle(title).SetContentFieldsBlock(() => {
                    w.Div(attributes, () => {
                        content();
                    });
                });
            }, collapsed);
        }

        internal static string GetResName(ValidationMessageSeverity severity)
        {
            var resName = severity == ValidationMessageSeverity.Error ? "Common.ValidationError" :
                    severity == ValidationMessageSeverity.Warning ? "Common.ValidationWarning" :
                    "Common.ValidationInfo";
            return resName;
        }

        private static LayoutWriter ValidationBlockImpl(this LayoutWriter w, ValidationMessageCollection val, Action<TagAttributes> attributes = null, Action<BlockCollapsibleBuilder> collapsed = null)
        {
            var severity = val.Count == 0 ? ValidationMessageSeverity.Error :
               val.Any(o => o.Severity == ValidationMessageSeverity.Error) ? ValidationMessageSeverity.Error :
               val.Any(o => o.Severity == ValidationMessageSeverity.Warning) ? ValidationMessageSeverity.Warning :
               ValidationMessageSeverity.Information;

            w.ValidationBlockContainer(severity, () => {
                foreach (var item in val)
                    w.P(() => w.Write(item.Message));
            }, attributes, collapsed);

            return w;
        }
    }
}