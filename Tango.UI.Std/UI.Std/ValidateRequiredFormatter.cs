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
            w.FieldsBlockCollapsible("<i class='icon icon-warning' style='color: " +
                                     (val.Any(o => o.Severity == ValidationMessageSeverity.Error) ? "red" : "yellow") +
                                     "'></i> Предупреждение", () => w.Div(a => a.Class("widthstd").Style("white-space: nowrap;"), () =>
            {
                foreach (var item in val)
                    w.P(() => w.Write(item.Message));
            }));
            return w;
        }
    }
}