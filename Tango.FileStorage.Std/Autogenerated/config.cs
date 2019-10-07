 
using System;
using System.Collections.Generic;
using System.Linq;
using Tango;
using Tango.Data;
using Tango.Localization;

namespace Tango.FileStorage.Std.Model
{
	public static class AutogenerateModule
	{
		public static void Config()
		{
			DefaultTableFilters.AddFor<N_Folder>("sort", x => x.OrderBy(o => o.Title)); 
			DefaultTableFilters.AddFor<N_File>("sort", x => x.OrderBy(o => o.Title)); 
			DefaultTableFilters.AddFor<N_FileData>("sort", x => x.OrderBy(o => o.Title)); 
			DefaultTableFilters.AddFor<N_FileLibraryType>("sort", x => x.OrderBy(o => o.Title)); 
		}
	}
}
