using System;
using System.Linq;
using System.Collections.Generic;
using Nephrite.Multilanguage;
using Nephrite.Html.Controls;
using Nephrite.Html;

namespace Nephrite.MVC
{
	public class HtmlHelper
	{
		public ITextResource TextResource { get; private set; }
		public AbstractQueryString Query { get; private set; }

		public HtmlHelper(
			AbstractQueryString query, 
			ITextResource textResource)
		{
			Query = query;
			TextResource = textResource;
        }

		public void Repeater<T>(IEnumerable<T> items, Action<T> render)
		{
			HtmlRepeaters.Repeater(items, render);
		}
		
		public void Repeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Action<T, string> render)
		{
			HtmlRepeaters.Repeater(items, className, classNameAlt, render);
		}

		public void GroupingRepeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Func<T, object> GroupBy1Property,
			Func<T, object> GroupBy2Property,
			bool Group1Asc,
			bool Group2Asc,
			Action<T, string> renderItems,
			Action<T, string> renderGroup1,
			Action<T, string> renderGroup2) where T : IWithTitle
		{
			HtmlRepeaters.GroupingRepeater(items, className, classNameAlt, GroupBy1Property, GroupBy2Property, Group1Asc, Group2Asc, renderItems, renderGroup1, renderGroup2);
		}

		//public ActionLink ActionLink(string title = null, string image = null, Action<ATagAttributes> aTagAttributes = null)
		//{
		//	return new ActionLink().Title(title).Image(image).Attr(aTagAttributes);
		//}

		//public ActionLink ActionImage(string title = null, string image = null, Action<ATagAttributes> aTagAttributes = null)
		//{
		//	return new ActionImage().Title(title).Image(image).Attr(aTagAttributes);
		//}

		//public ActionLink ActionImageConfirm(string linkTitle, string image, string confirmString)
		//{
		//	return ActionImage(linkTitle, image, a => a.OnClick("return confirm('{0}')" + confirmString));
		//}

		//public ActionLink ActionLinkConfirm(string linkTitle, string image, string confirmString)
		//{
		//	return ActionLink(linkTitle, image, a => a.OnClick("return confirm('{0}')" + confirmString));
		//}

		public string InternalLink(string onClick, string linkText)
		{
			return String.Format("<a href='#' onclick=\"{0}\">{1}</a>", onClick, linkText);
		}

		public string InternalImage(string onClick, string linkText, string image)
		{
			return String.Format("<a href='#' onclick=\"{0}\"><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>", onClick, linkText, image, IconSet.RootPath);
		}

		public string InternalImageLink(string onClick, string linkText, string image)
		{
			return String.Format("<a href='#' onclick=\"{0}\"><img src='{3}{2}' alt='{1}' title='{1}' class='middle' /></a>&nbsp;<a href='#' onclick='{0}'>{1}</a>", onClick, linkText, image, IconSet.RootPath);
		}


    }


	public static class HtmlRepeaters
	{
		public static void Repeater<T>(IEnumerable<T> items, Action<T> render)
		{
			if (items == null)
				return;
			foreach (var item in items)
				render(item);
		}

		public static void Repeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Action<T, string> render)
		{
			if (items == null)
				return;

			int i = 0;

			foreach (var item in items)
				render(item, (i++ % 2 == 0) ? className : classNameAlt);
		}

		public static void GroupingRepeater<T>(IEnumerable<T> items,
			string className,
			string classNameAlt,
			Func<T, object> GroupBy1Property,
			Func<T, object> GroupBy2Property,
			bool Group1Asc,
			bool Group2Asc,
			Action<T, string> renderItems,
			Action<T, string> renderGroup1,
			Action<T, string> renderGroup2) where T : IWithTitle
		{
			if (items == null)
				return;

			int i = 0;

			Dictionary<string, Dictionary<string, List<T>>> list = new Dictionary<string, Dictionary<string, List<T>>>();

			foreach (var item in items)
			{
				string val1s = "";
				string val2s = "";
				if (GroupBy1Property != null)
				{
					object val1 = GroupBy1Property(item);

					if (val1 is IWithTitle)
					{
						val1s = ((IWithTitle)val1).Title;
					}
					else
					{
						if (val1 != null)
							val1s = val1.ToString();
					}
				}
				if (GroupBy2Property != null)
				{
					object val2 = GroupBy2Property(item);

					if (val2 is IWithTitle)
					{
						val2s = ((IWithTitle)val2).Title;
					}
					else
					{
						if (val2 != null)
							val2s = val2.ToString();
					}
				}
				if (!list.ContainsKey(val1s))
					list.Add(val1s, new Dictionary<string, List<T>>());

				if (!list[val1s].ContainsKey(val2s))
					list[val1s].Add(val2s, new List<T>());

				list[val1s][val2s].Add(item);
			}

			foreach (string k1 in Group1Asc ? list.Keys.OrderBy(k => k) : list.Keys.OrderByDescending(k => k))
			{
				bool rg1 = false;
				if (k1 != "")
					rg1 = true;
				foreach (string k2 in Group2Asc ? list[k1].Keys.OrderBy(k => k) : list[k1].Keys.OrderByDescending(k => k))
				{
					bool rg2 = false;
					if (k2 != "")
						rg2 = true;
					foreach (var item in list[k1][k2])
					{
						if (rg1)
						{
							renderGroup1(item, k1);
							rg1 = false;
						}
						if (rg2)
						{
							renderGroup2(item, k2);
							rg2 = false;
						}
						renderItems(item, (i++ % 2 == 0) ? className : classNameAlt);
					}
				}
			}
		}
	}


}
