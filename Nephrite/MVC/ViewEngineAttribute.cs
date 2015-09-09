using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ViewEngineAttribute : Attribute
	{
		public Type ViewEngineType { get; set; }
		public ViewEngineAttribute(Type viewEngineType)
		{
			ViewEngineType = viewEngineType;
		}
	}
}
