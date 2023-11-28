namespace Tango
{
	public interface IPersistentSettings
	{
		string Get(string name, string defaultValue = "");
		bool GetBool(string name, bool defaultValue = false);
        void ResetCache();
        string this[string name] { get; }
	}
}
