using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace System.IdentityModel
{
	static class EmptyReadOnlyCollection<T>
	{
		public static ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new List<T>());
	}
}
