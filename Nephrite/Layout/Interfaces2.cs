using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Layout
{
	public interface ISystemLayout
	{
		ILayoutList List { get; }
		ILayoutForm Form { get; }
		ILayoutToolbar2 Toolbar { get; }
	}

	public interface ILayoutToolbar2
	{
		string ToolbarBegin();
		string ToolbarEnd();
		string ToolbarPartBegin(string partName);
		string ToolbarPartEnd();
		string ToolbarSeparator();
		string ToolbarWhiteSpace();
		string ToolbarItem(string content);
	}
}
