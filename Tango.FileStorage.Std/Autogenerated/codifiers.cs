using System;
using System.Collections.Generic;
using System.Linq;
using Tango;
using Tango.Html;
using Tango.Localization;

namespace Tango.FileStorage.Std.Model
{
    public partial struct StorageType
    {
		public static List<SelectListItem> ToSelectListItems(IResourceManager res)
        {
            if (_selectList == null)
            {
                _selectList = new List<SelectListItem>{
                	new SelectListItem(res.Get("Tango.FileStorage.Std.Model.StorageType.Disk"), "D"),
                	new SelectListItem(res.Get("Tango.FileStorage.Std.Model.StorageType.Database"), "B"),
                };
            }
            return _selectList;
        }
		
		static List<SelectListItem> _selectList = null;
        public static string Title(IResourceManager res, string code)
        {
            var cv = ToSelectListItems(res).FirstOrDefault(o => o.Value == code);
            return cv != null ? cv.Text : String.Empty;
        }
    }
}
