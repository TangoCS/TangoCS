using System;

namespace Tango.Html
{
	public partial class HtmlWriter
	{
		void IContentWriter.Table(Action<IContentItemAttributes> attributes, Action inner) => WriteContentTag("table", attributes, inner);
		void IContentWriter.Tr(Action<IContentItemAttributes> attributes, Action inner) => WriteContentTag("tr", attributes, inner);
		void IContentWriter.Td(Action<ITdAttributes> attributes, Action inner) => WriteContentTag("td", attributes, inner);
		void IContentWriter.Th(Action<IThAttributes> attributes, Action inner) => WriteContentTag("th", attributes, inner);
		void IContentWriter.Div(Action<IContentItemAttributes> attributes, Action inner) => WriteContentTag("div", attributes, inner);
		void IContentWriter.Td(Action<ITdAttributes> attributes, decimal? n, string format) => WriteContentTag("td", (Action<ITdAttributes>)(a => { a.Class("r"); attributes?.Invoke(a); }), () => Write(n == null ? "" : n.Value.ToString(format, ru)));
		void IContentWriter.Th(Action<IThAttributes> attributes, decimal? n, string format) => WriteContentTag("th", (Action<IThAttributes>)(a => { a.Class("r"); attributes?.Invoke(a); }), () => Write(n == null ? "" : n.Value.ToString(format, ru)));

		T Fabric<T>()
			where T : class, IContentItemAttributes<T>
		{
			switch (typeof(T))
			{
				case Type td when td == typeof(ITdAttributes):
					return new TdTagAttributes() as T;
				case Type th when th == typeof(IThAttributes):
					return new ThTagAttributes() as T;
				case Type other when other == typeof(IContentItemAttributes):
					return new TagAttributes() as T;
			}

			throw new NotSupportedException();
		}
	}
}
