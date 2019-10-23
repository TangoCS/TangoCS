using System;
using Tango.Identity.Std;

namespace Tango.EntityBehaviour
{
    public class BSetTimestamp : IEntityBehaviour
    {
		[Inject]
		protected IIdentityManager IdentityManager { get; set; }

		[BehaviourContext]
		protected IWithUserTimeStamp ViewData { get; set; }

		public void Invoke(Action next)
		{
			ViewData.LastModifiedDate = DateTime.Now;
			ViewData.LastModifiedUserID = IdentityManager.CurrentUser.Id;
			next();
		}
	}
}
