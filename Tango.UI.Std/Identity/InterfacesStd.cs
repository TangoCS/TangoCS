using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Identity.Std
{
	public interface IIdentityManager : IIdentityManager<IdentityUser> { }

	public interface IUserValidator : IUserValidator<int> { }

	public interface IIdentityStore : IIdentityStore<IdentityUser, int> { }
}
