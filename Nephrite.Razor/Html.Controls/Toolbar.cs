using System;
using System.IO;
using Nephrite.Layout;
using Nephrite.Razor;
using RazorEngine.Text;

namespace Nephrite.Html.Controls
{
	//public static class ToolbarExtension
	//{
	//	public static Toolbar BeginToolbar(this RazorHtmlHelper c)
	//	{
	//		var layout = DI.GetService<ILayoutToolbar2>();
 //           var t = new Toolbar(c.Writer, layout);
	//		c.Writer.Write(layout.ToolbarBegin());
	//		return t;
	//	}

	//	public class Toolbar : IDisposable
	//	{
	//		bool _canAddSeparator = false;
	//		int _partStatus = 0;

	//		ILayoutToolbar2 _layout;
	//		TextWriter _writer;

	//		public Toolbar(TextWriter writer, ILayoutToolbar2 layout)
	//		{
	//			_layout = layout;
	//			_writer = writer;
 //           }

	//		public IEncodedString Item(ActionLink link)
	//		{
	//			_canAddSeparator = true;
	//			if (_partStatus == 0)
	//			{
	//				_writer.Write(_layout.ToolbarPartBegin("ms-toolbar-left"));					
 //               }
	//			if (_partStatus == 2)
	//			{
	//				_writer.Write(_layout.ToolbarPartEnd());
	//				_writer.Write(_layout.ToolbarPartBegin("ms-toolbar-right"));
	//			}
	//			_partStatus++;
	//			return new RawString(_layout.ToolbarItem(link));
	//		}

	//		public IEncodedString ItemSeparator()
	//		{
	//			if (!_canAddSeparator) return null;
	//			_canAddSeparator = false;
	//			return new RawString(_layout.ToolbarSeparator());
	//		}

	//		public IEncodedString WhiteSpace()
	//		{
	//			if (_partStatus > 1) return null;
	//			_canAddSeparator = false;
	//			if (_partStatus == 1)
	//			{
	//				_writer.Write(_layout.ToolbarPartEnd());					
 //               }
	//			_partStatus++;
	//			return null;
	//		}

	//		public void Dispose()
	//		{
	//			if (_partStatus != 0)
	//			{
	//				_writer.Write(_layout.ToolbarPartEnd());
	//			}
 //               _writer.Write(_layout.ToolbarEnd());
	//		}
	//	}
	//}
}
