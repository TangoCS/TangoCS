using Tango.Html;
using System;

namespace Tango.UI
{
	public static class FileUploadExtensions
	{
		public static void FileUpload(this HtmlWriter w, string name, Action<InputTagAttributes> attributes = null)
		{
			Action<InputTagAttributes> a = ta => {
				ta.Name(name).ID(name).Type(InputType.File);
				if (attributes != null) attributes(ta);
			};
			w.Input(name, a);
		}

		public static void FormFieldFileUpload(this LayoutWriter w, string name, string caption, bool isRequired = false, string description = null, Action<InputTagAttributes> attributes = null)
		{
			w.FormField(name, caption, () => w.FileUpload(name, a => a.Style("width:100%").Set(attributes)), isRequired, description);
		}
	}
}
