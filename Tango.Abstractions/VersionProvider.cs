using System;

namespace Tango
{
    public interface IVersionProvider
    {
		Version Version { get; }
	}

	public class VersionProvider : IVersionProvider
	{
		Type t;

		public Version Version => t.Assembly.GetName().Version;

		public VersionProvider(Type t)
		{
			this.t = t;
		}
	}
}
