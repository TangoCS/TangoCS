using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FormsEngine
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class DynamicClassAttribute : Attribute
	{
		public string CodeFile { get; set; }
		public Type TargetType { get; set; }
		public DynamicClassAttribute(string CodeFile, Type Type)
		{
			this.CodeFile = CodeFile;
			this.TargetType = Type;
		}
	}
}