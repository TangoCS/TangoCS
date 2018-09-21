using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tango
{
	public class CodifierValue
	{
		public string Code { get; set; }
		public string Title { get; set; }
	}

	public class SelectListItem
	{
		public string Text { get; set; }
		public string Value { get; set; }
		public bool Selected { get; set; }

		public SelectListItem()
		{
		}

		public SelectListItem(object text, object value, bool selected)
		{
			Text = text?.ToString();
			Value = value?.ToString();
			Selected = selected;
		}

		public SelectListItem(object text, object value)
		{
			Text = text?.ToString();
			Value = value?.ToString();
		}

		public SelectListItem(object text)
		{
			Text = text?.ToString();
			Value = text?.ToString();
		}
	}

	public static class SelectListItemExtensions
	{
		public static List<SelectListItem> AddEmptyItem(this List<SelectListItem> list)
		{
			list.Insert(0, new SelectListItem());
			return list;
		}

		public static List<SelectListItem> AddEmptyItem(this IEnumerable<SelectListItem> list)
		{
			return list.ToList().AddEmptyItem();
		}
	}
}
