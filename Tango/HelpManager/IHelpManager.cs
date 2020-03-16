using System;

namespace Tango
{
	public interface IHelpManager
    {
		Guid Get(string name);
        Guid this[string name] { get; }
        void ResetCache();
    }
}
