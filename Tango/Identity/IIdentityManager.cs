using System;
using System.Globalization;
using System.Text;

namespace Tango.Identity
{
	public interface IIdentityManager<TUser>
		where TUser : class
	{
		IIdentityOptions Options { get; }

		TUser DefaultUser { get; }
		TUser CurrentUser { get; }
		TUser SystemUser { get; }

		void RunAs(TUser user, Action action);
	}

	public interface IUserIdAccessor<TKey>
	{
		TKey CurrentUserID { get; }
		TKey SystemUserID { get; }
	}

	public interface IIdentityStore<TUser, TKey>
		where TKey : IEquatable<TKey>
	{
		TUser UserFromName(string name);
		TUser UserFromProviderKey(string providerName, string providerKey);
		TUser UserFromID(TKey id);
		TUser UserFromEmail(string email);
		TUser UserFromSecurityStamp(string securityStamp);
		void Activate(TKey id);
		void Deactivate(TKey id);

		void SetSecurityStamp(TKey id, string securityStamp);
		void SetNewPassword(TKey id, string newHash);
	}
	
	/*public static class IndetityHelper
	{
		public const string ResetPasswordTokenPurpose = "ResetPassword";
		public const string ConfirmEmailTokenPurpose = "EmailConfirmation";

		public static string CreateSecurityStamp()
		{
			return Base32.ToBase32(Rfc6238AuthenticationService.GenerateRandomKey());
		}


		public static string GenerateEmailConfirmationToken<TKey>(IdentityUser<TKey> user)
			 where TKey : IEquatable<TKey>
		{
			var modifier = "Email:" + ConfirmEmailTokenPurpose + ":" + user.Email;
			return GenerateUserToken(modifier, user);
		}

		public static string GenerateResetPasswordToken<TKey>(IdentityUser<TKey> user)
			 where TKey : IEquatable<TKey>
		{
			var modifier = "Totp:" + ResetPasswordTokenPurpose + ":" + user.Id.ToString();
			return GenerateUserToken(modifier, user);
		}

		public static bool ValidateResetPasswordToken<TKey>(IdentityUser<TKey> user, string token)
			 where TKey : IEquatable<TKey>
		{
			var modifier = "Totp:" + ResetPasswordTokenPurpose + ":" + user.Id.ToString();
			return ValidateUserToken(modifier, user, token);
		}

		static string GenerateUserToken<TKey>(string modifier, IdentityUser<TKey> user)
			 where TKey : IEquatable<TKey>
		{
			var token = Encoding.Unicode.GetBytes(user.SecurityStamp);
			var code = Rfc6238AuthenticationService.GenerateCode(token, modifier);
			return code.ToString("D6", CultureInfo.InvariantCulture);
		}

		static bool ValidateUserToken<TKey>(string modifier, IdentityUser<TKey> user, string token)
			where TKey : IEquatable<TKey>
		{
			int code;
			if (!int.TryParse(token, out code))
			{
				return false;
			}
			var securityToken = Encoding.Unicode.GetBytes(user.SecurityStamp);
			return securityToken != null && Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier);
		}
	}*/

}
