using System;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Std
{
    public static class ValidationFormatter
    {
        // public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val)
        // {
        //     w.Fieldset(a => a.Style("margin-bottom:8px"), () =>
        //     {
        //         w.Ul(a => a.Class("savewarning"), () =>
        //         {
        //             foreach (var item in val)
        //                 w.Li(() => w.Write(item.Message));
        //         });
        //     });
        //     return w;
        // }

        public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val)
        {
            var color = val.Count == 0 ? "red" :
                val.Any(o => o.Severity == ValidationMessageSeverity.Error) ? "red" :
                val.Any(o => o.Severity == ValidationMessageSeverity.Warning) ? "yellow" :
                "skyblue";

            Action title = () => {
                w.Icon("warning", a => a.Style("margin-right: 4px; color:" + color));
                w.Write("Предупреждение");
            };

            w.FieldsBlockCollapsible(title, () => {
                w.Div(a => a.Class("validation-body").Class("widthstd").Style("white-space: nowrap;"), () => {
                    foreach (var item in val)
                        w.P(() => w.Write(item.Message));
                });
            });
            return w;
        }
    }
}