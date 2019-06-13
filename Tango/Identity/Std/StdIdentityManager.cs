using System.Security.Principal;

namespace Tango.Identity.Std
{
	public class IdentityManager : IdentityManager<IdentityUser, int>, IIdentityManager, IUserIdAccessor<int>
	{
		public IdentityManager(IIdentity user, IIdentityStore dataContext, IIdentityOptions options) : base(user, dataContext, options)
		{
			
		}

		public int CurrentUserID => CurrentUser.Id;

		public int SystemUserID => SystemUser.Id;

		public override IdentityUser DefaultUser => new IdentityUser { Id = -1, UserName = "anonymous" };
	}
}
