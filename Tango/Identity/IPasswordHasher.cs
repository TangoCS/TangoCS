namespace Tango.Identity
{
	public interface IPasswordHasher
	{
		string CreateHash(string password);
		bool ValidatePassword(string password, string correctHash);
	}
}