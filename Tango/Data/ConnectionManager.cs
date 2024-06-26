﻿using System.Collections.Generic;

namespace Tango.Data
{
	public static class ConnectionManager
    {
		public static string ConnectionString { get; set; }

		public static Dictionary<string, string> ConnectionStrings { get; set; }
		public static string DefaultConnection { get; set; }
		public static DBType DBType { get; set; }
	}
}
