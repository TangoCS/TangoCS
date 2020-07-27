using System;
using Tango.UI;

namespace Tango
{
	public interface IHelpManager
    {
		Guid Get(string name);
        Guid this[string name] { get; }
        void ResetCache();
        void Render(LayoutWriter w, string service = null, string action = null);
    }
}
