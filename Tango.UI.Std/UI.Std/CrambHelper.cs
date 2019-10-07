using System;
using System.Collections.Generic;
using System.Text;
using Tango.Html;

namespace Tango.UI.Std
{
    public static class CrambHelper
    {
		public static void SetCramb(ApiResponse response, params Action<LayoutWriter>[] parts)
		{
			if (parts.Length == 0) return;

			response.ReplaceWidget("cramb", w => {
				w.Div(a => a.Class("cramb"), () => {
					for (int i = 0; i < parts.Length - 1; i++)
					{
						parts[i](w);
						w.Write("&nbsp;/&nbsp;");
					}

					parts[parts.Length - 1](w);
				});
			});
		}
    }
}
