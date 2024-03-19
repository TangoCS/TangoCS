namespace Tango
{
	public interface IPersistentSettings
	{
		string Get(string name, string defaultValue = "");
		bool GetBool(string name, bool defaultValue = false);
        void ResetCache();
        string this[string name] { get; }
	}
	public interface IUserPersistentSettings<TUserKey>
	{
		bool GetBoolForUser(string name, TUserKey userid, bool defaultValue = false);
		string GetForUser(string name, TUserKey userid, string defaultValue = "");
	}
}
