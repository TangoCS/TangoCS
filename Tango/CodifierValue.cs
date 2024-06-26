﻿using System;
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
		public bool Disabled { get; set; }

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

		public override bool Equals(object obj)
		{
			return Value?.ToLower() == ((SelectListItem)obj).Value?.ToLower();
		}

		public override int GetHashCode() => Value?.GetHashCode() ?? 0;
	}

	public static class SelectListItemExtensions
	{
		public static List<SelectListItem> AddEmptyItem(this List<SelectListItem> list)
		{
			list.Insert(0, new SelectListItem());
			return list;
		}

		public static List<SelectListItem> AddItem(this IEnumerable<SelectListItem> list, string text, string value = null)
		{
			var l = list.ToList();
			l.Add(new SelectListItem(text, value));
			return l;
		}

		public static List<SelectListItem> AddEmptyItem(this IEnumerable<SelectListItem> list)
		{
			return list.ToList().AddEmptyItem();
		}
		
	}
}
