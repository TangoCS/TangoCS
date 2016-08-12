using System;
using System.Security.Principal;

namespace Tango.Identity.Std
{
	public class IdentityManager : IIdentityManager
	{
		IIdentityStore _dataContext;
		IIdentity _user;
		IPasswordHasher _passwordHasher;
		public IIdentityOptions Options { get; private set; }

		public IdentityManager(
			IIdentity user,
			IIdentityStore dataContext,
			IPasswordHasher passwordHasher,
			IIdentityOptions options)
		{
			_user = user;
			_passwordHasher = passwordHasher;
			_dataContext = dataContext;
			Options = options ?? new IdentityOptions();
		}

		public IIdentity CurrentIdentity => _user;
		public IPasswordHasher PasswordHasher => _passwordHasher;

		IdentityUser _currentUser = null;
		public IdentityUser CurrentUser
		{
			get
			{				
				if (_currentUser != null) return _currentUser;

				IdentityUser s = null;
				string name;
				if (!Options.Enabled)
				{
					name = Options.AnonymousUserName;
				}
				else
				{
					if (_user == null)
						name = Options.AnonymousUserName;
					else
					{
						WindowsIdentity wi = _user as WindowsIdentity;
						if (wi != null && !wi.IsAnonymous)
						{
							name = wi.User.Value;
							if (s == null) name = Options.AnonymousUserName;
						}
						else
						{
							name = _user.Name;
						}
					}
				}
				s = _dataContext.UserFromName(name);
				if (s == null) throw new Exception(String.Format("User {0} does not exist in the database", name));
				_currentUser = s;
				return s;
			}
		}

		public IdentityUser SystemUser
		{
			get
			{
				var name = Options.SystemUserName;
				var s = _dataContext.UserFromName(name);
				if (s == null) throw new Exception(String.Format("User {0} does not exist in the database", name));
				return s;
			}
		}

		public void RunAs(IdentityUser subject, Action action)
		{
			var oldSubject = _currentUser;
			_currentUser = subject;
			action();
			_currentUser = oldSubject;
		}
	}


}
