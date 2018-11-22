using System;

namespace Tango.Excel
{
	public class ExcelWriter : IContentWriter
	{
		public string NewLine => throw new NotImplementedException();

		public void Div(Action<IContentItemAttributes> attributes, Action inner)
		{
			throw new NotImplementedException();
		}

		public void Table(Action<IContentItemAttributes> attributes = null, Action inner = null)
		{
			throw new NotImplementedException();
		}

		public void Td(Action<ITdAttributes> attributes = null, Action inner = null)
		{
			throw new NotImplementedException();
		}

		public void Th(Action<IThAttributes> attributes = null, Action inner = null)
		{
			throw new NotImplementedException();
		}

		public void Tr(Action<IContentItemAttributes> attributes, Action inner = null)
		{
			throw new NotImplementedException();
		}

		public void Write(string text)
		{
			throw new NotImplementedException();
		}
	}
}
