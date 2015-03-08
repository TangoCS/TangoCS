using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Html;

namespace Nephrite.Layout
{
	public class SimpleTags : ILayoutSimpleTags
	{
		public StringBuilder Link(ILink link)
		{
			StringBuilder sb = new StringBuilder(255);

			if (!link.Image.IsEmpty()) sb.Append(this.Image(link.Image, link.Title));
			sb.Append(@"<a href=""").Append(link.Href).Append(@"""");
			if (link.TargetBlank) sb.Append(@" target=""_blank""");
			if (!link.OnClick.IsEmpty()) sb.AppendFormat(@" onClick=""{0}""", link.OnClick);
			if (!link.AccessKey.IsEmpty()) sb.AppendFormat(@" accessKey=""{0}""", link.AccessKey);
			if (!link.Description.IsEmpty()) sb.AppendFormat(@" title=""{0}""", link.Description);
			sb.AppendAttributes(link.Attributes, "");
			sb.AppendFormat(">{0}</a>", link.Title);

			return sb;
		}

		public StringBuilder ImageLink(ILink link)
		{
			StringBuilder sb = new StringBuilder(255);

			sb.Append(@"<a href=""").Append(link.Href).Append(@"""");
			if (link.TargetBlank) sb.Append(@" target=""_blank""");
			if (!link.OnClick.IsEmpty()) sb.AppendFormat(@" onClick=""{0}""", link.OnClick);
			if (!link.AccessKey.IsEmpty()) sb.AppendFormat(@" accessKey=""{0}""", link.AccessKey);
			if (!link.Description.IsEmpty()) sb.AppendFormat(@" title=""{0}""", link.Description);
			sb.AppendAttributes(link.Attributes, "");
			sb.AppendFormat(">{0}</a>", this.Image(link.Image, link.Title));

			return sb;
		}

		public StringBuilder Image(string src, string alt, object attributes)
		{
			// при необходимости сделать метод DBImage
			//if (src.ToLower().Contains("data.ashx"))
			//	return String.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", DataHandler.GetDataUrl(src.GetQueryParameter("oid").ToInt32(0)), alt);
			//if (src.IndexOf('/', 1) > 0 && !src.StartsWith(".."))
			//	return String.Format("<img src='{0}' class='middle' alt='{1}' title='{1}' />", DataHandler.GetDataUrl(src), alt);
			StringBuilder sb = new StringBuilder(255);
			return sb.AppendFormat("<img src='{0}' class='middle' alt='{1}' title='{1}' />", IconSet.RootPath + src, alt);
		}
	}
}