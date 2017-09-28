using System.Security.Claims;

namespace Tango.Identity.Std
{
	/// <summary>
	/// Represents a claim that a user possesses. 
	/// </summary>
	public class IdentityUserClaim
	{
		/// <summary>
		/// Gets or sets the claim type for this claim.
		/// </summary>
		public virtual string ClaimType { get; set; }

		/// <summary>
		/// Gets or sets the claim value for this claim.
		/// </summary>
		public virtual string ClaimValue { get; set; }

		public virtual Claim ToClaim()
		{
			return new Claim(ClaimType, ClaimValue);
		}

		public virtual void FromClaim(Claim other)
		{
			ClaimType = other.Type;
			ClaimValue = other.Value;
		}
	}
}
