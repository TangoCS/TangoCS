using System.Security.Principal;

namespace Tango.Identity.Std
{
	public class IdentityManager : IdentityManager<IdentityUser, long>, IIdentityManager, IUserIdAccessor<long>, IUserIdAccessor<object>
	{
		public IdentityManager(IIdentity user, IIdentityStore dataContext, IIdentityOptions options) : base(user, dataContext, options)
		{
			
		}

		public long CurrentUserID => CurrentUser.Id;

		public long SystemUserID => SystemUser.Id;
		
		object IUserIdAccessor<object>.SystemUserID => SystemUserID;

		object IUserIdAccessor<object>.CurrentUserID => CurrentUserID;

		public override IdentityUser DefaultUser => new IdentityUser { Id = -1, UserName = "anonymous" };
	}
}
