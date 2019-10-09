 
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Data;
using Tango.Hibernate;

namespace Tango.FileStorage.Std.Model
{
	public partial class TangoFileStorageStdTableFunctions
	{
		public void Init(Configuration cfg)
		{
		}
	}

	public static class TangoFileStorageStdDataContextExtensions
	{
		public static IQueryable<N_Folder> N_Folder(this IDataContext dc) => dc.GetTable<N_Folder>();
		public static IQueryable<N_File> N_File(this IDataContext dc) => dc.GetTable<N_File>();
		public static IQueryable<N_FileData> N_FileData(this IDataContext dc) => dc.GetTable<N_FileData>();
		public static IQueryable<N_FileLibraryType> N_FileLibraryType(this IDataContext dc) => dc.GetTable<N_FileLibraryType>();
		public static IQueryable<N_FileLibrary> N_FileLibrary(this IDataContext dc) => dc.GetTable<N_FileLibrary>();
		public static IQueryable<N_DownloadLog> N_DownloadLog(this IDataContext dc) => dc.GetTable<N_DownloadLog>();
		public static IQueryable<V_N_FullFolder> V_N_FullFolder(this IDataContext dc) => dc.GetTable<V_N_FullFolder>();
	}
}
