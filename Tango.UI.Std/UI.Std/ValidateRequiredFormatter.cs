using System;
using System.Drawing;
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

        public static void ValidationBlockContainer(this LayoutWriter w, ValidationMessageSeverity severity, Action content)
        {
			var color = 
				severity == ValidationMessageSeverity.Error ? "red" :
				severity == ValidationMessageSeverity.Warning ? "yellow" :
				"skyblue";

			Action title = () => {
				w.Icon("warning", a => a.Style("margin-right: 4px; color:" + color));
				w.Write("Предупреждение");
			};

			w.FieldsBlockCollapsible(title, () => {
				w.Div(a => a.Class("validation-body").Class("widthstd").GridColumn(Grid.OneWhole), () => {
                    content();
				});
			});
		}


		public static LayoutWriter ValidationBlock(this LayoutWriter w, ValidationMessageCollection val)
        {
            var severity = val.Count == 0 ? ValidationMessageSeverity.Error :
                val.Any(o => o.Severity == ValidationMessageSeverity.Error) ? ValidationMessageSeverity.Error :
                val.Any(o => o.Severity == ValidationMessageSeverity.Warning) ? ValidationMessageSeverity.Warning :
				ValidationMessageSeverity.Information;

            w.ValidationBlockContainer(severity, () => {
				foreach (var item in val)
					w.P(() => w.Write(item.Message));
			});

            return w;
        }
    }
}