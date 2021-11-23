using System.Linq;

namespace Tango.Identity.Std
{
	/// <summary>
	/// Represents a user in the identity system
	/// </summary>
	public class IdentityUser : IdentityUser<int, IdentityUserClaim, IdentityUserLogin>
	{
	}

	public static class IdentityUserExtensions
	{
		public static string GetClaim(this IdentityUser user, string claimType)
		{
			return user.Claims.FirstOrDefault(o => o.ClaimType == claimType)?.ClaimValue;
		}
	}
}
