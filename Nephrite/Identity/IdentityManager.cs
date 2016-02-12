using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Nephrite.Identity
{
	public class IdentityManager<TKey> : IIdentityManager<TKey>
		where TKey : IEquatable<TKey>
	{
		IDC_Identity<IdentityUser<TKey>, TKey> _dataContext;
		IIdentity _user;
		IPasswordHasher _passwordHasher;
		public IIdentityOptions Options { get; private set; }

		public IdentityManager(
			IIdentity user,
			IDC_Identity<IdentityUser<TKey>, TKey> dataContext,
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

		IdentityUser<TKey> _currentUser = null;
		public IdentityUser<TKey> CurrentUser
		{
			get
			{
				
				if (_currentUser != null) return _currentUser;

				IdentityUser<TKey> s = null;
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

		public IdentityUser<TKey> SystemUser
		{
			get
			{
				var name = Options.SystemUserName;
				var s = _dataContext.UserFromName(name);
				if (s == null) throw new Exception(String.Format("User {0} does not exist in the database", name));
				return s;
			}
		}

		public void RunAs(TKey sid, Action action)
		{
			var oldSubject = _currentUser;
			_currentUser = _dataContext.UserFromID(sid);
			action();
			_currentUser = oldSubject;
		}
		public void RunAs(IdentityUser<TKey> subject, Action action)
		{
			var oldSubject = _currentUser;
			_currentUser = subject;
			action();
			_currentUser = oldSubject;
		}
	}

	public interface IDC_Identity<TUser, TKey>
		where TKey : IEquatable<TKey>
	{
		TUser UserFromName(string name);
		TUser UserFromProviderKey(string providerName, string providerKey);
		TUser UserFromID(TKey id);
		TUser UserFromEmail(string email);
	}
}
