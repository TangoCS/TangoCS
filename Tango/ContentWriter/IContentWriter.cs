﻿using System;
using System.Collections.Generic;

namespace Tango
{
	public interface IContentWriter
	{
		void Write(string text);
		string NewLine { get; }

		void Table(Action<IContentItemAttributes> attributes, Action inner);
		void Tr(Action<IContentItemAttributes> attributes, Action inner);
		void Td(Action<ITdAttributes> attributes, Action inner);
        void Td(Action<ITdAttributes> attributes, decimal? n, string format);
		void Th(Action<IThAttributes> attributes, Action inner);
		void Th(Action<IThAttributes> attributes, decimal? n, string format);
		void Div(Action<IContentItemAttributes> attributes, Action inner);
        string Write(IEnumerable<string> text);
    }

	public static class ContentWriterExtensions
	{
		public static void Table(this IContentWriter w, Action inner) { w.Table(null, inner); }
		public static void Tr(this IContentWriter w, Action inner) { w.Tr(null, inner); }
		public static void Td(this IContentWriter w, Action inner) { w.Td(null, inner); }
        public static void Td(this IContentWriter w, decimal? n, string format) { w.Td(null, n, format); }
		public static void Td(this IContentWriter w, Action<ITdAttributes> attributes, decimal? n, string format) { w.Td(attributes, n, format); }
		public static void TdRight(this IContentWriter w, decimal? n, string format) { w.Td(a => a.Class("r"), n, format); }
		public static void TdRight(this IContentWriter w, Action<ITdAttributes> attributes, decimal? n, string format) {
			attributes += a => a.Class("r");
			w.Td(attributes, n, format);
		}
		public static void Th(this IContentWriter w, Action inner) { w.Th(null, inner); }
		public static void Div(this IContentWriter w, Action inner) { w.Div(null, inner); }

		public static void Div(this IContentWriter w, string text) { w.Div(null, () => w.Write(text)); }
		public static void Div(this IContentWriter w, Action<IContentItemAttributes> attrs, string text) { w.Div(attrs, () => w.Write(text)); }

		public static void Th(this IContentWriter w, Action<IThAttributes> attrs, string text) { w.Th(attrs, () => w.Write(text)); }
		public static void Th<T>(this IContentWriter w, T text) { w.Th(null, () => w.Write(text?.ToString())); }

		public static void Td(this IContentWriter w, Action<ITdAttributes> attrs, string text) { w.Td(attrs, () => w.Write(text)); }
		public static void Td<T>(this IContentWriter w, T text) { w.Td(null, () => w.Write(text?.ToString())); }
	}
}
