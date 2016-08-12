using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			return user.Claims.FirstOrDefault(o => o.ClaimType == claimType).ClaimValue;
		}
	}
}
