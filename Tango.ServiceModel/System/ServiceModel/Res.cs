﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public static class Res
    {
		public static string GetString(string str, params object[] p)
		{
			return str + Environment.NewLine + "Data: " + String.Join("; ", p);
		}

		public static string S(string str)
		{
			return str;
		}
	}
}
