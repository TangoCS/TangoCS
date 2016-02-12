namespace Nephrite.Identity
{
	public interface IIdentityOptions
	{
		bool AllowPasswordReset { get; }
		bool AllowRegister { get; }
		bool AllowRememberMe { get; }
		string AnonymousUserName { get; }
		bool Enabled { get; }
		string SystemUserName { get; }
	}

	public class IdentityOptions : IIdentityOptions
	{
		public string AnonymousUserName { get; set; }
		public string SystemUserName { get; set; }
		public bool Enabled { get; set; }
		public bool AllowRegister { get; set; }
		public bool AllowPasswordReset { get; set; }
		public bool AllowRememberMe { get; set; }

		public IdentityOptions()
		{
			AnonymousUserName = "anonymous";
			SystemUserName = "system";
			Enabled = true;
			AllowRegister = false;
			AllowPasswordReset = false;
			AllowRememberMe = true;
		}
	}
}