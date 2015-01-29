using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web.Layout
{
	internal static class LayoutHelper
	{
		public static StringBuilder AppendAttributes(this StringBuilder sb, object attributes, string defaultClass)
		{
			bool c = !defaultClass.IsEmpty();
			if (attributes == null)
			{
				if (c) sb.AppendFormat(@" class=""{0}""", defaultClass);
				return sb;
			}

			if (attributes is string)
			{
				if (c) sb.AppendFormat(@" class=""{0}""", defaultClass);
				sb.AppendFormat(@" style=""width:{0}""", attributes);
				return sb;
			}

			bool b = false;
			foreach (var p in attributes.GetType().GetProperties())
			{
				if (p.Name.ToLower() == "class")
				{
					sb.AppendFormat(@" {0}=""{1}""", p.Name, p.GetValue(attributes, null) + " " + defaultClass);
					b = true;
				}
				else
					sb.AppendFormat(@" {0}=""{1}""", p.Name, p.GetValue(attributes, null));
			}
			if (!b && c) sb.AppendFormat(@" class=""{0}""", defaultClass);
			return sb;
		}
	}
}