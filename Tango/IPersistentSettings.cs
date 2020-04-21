namespace Tango
{
	public interface IPersistentSettings
	{
		string Get(string name, string defaultValue = "");
		bool GetBool(string name);
        void ResetCache();
        string this[string name] { get; }
	}
}
