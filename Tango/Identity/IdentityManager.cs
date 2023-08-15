using System;
using System.Security.Principal;

namespace Tango.Identity
{
	public abstract class IdentityManager<TUser, TKey> : IIdentityManager<TUser>
	where TUser : class
	where TKey : IEquatable<TKey>
	{
		IIdentityStore<TUser, TKey> _dataContext;
		protected IIdentity _user;
		TUser _currentUser = null;

		public IIdentityOptions Options { get; private set; }

		public abstract TUser DefaultUser { get; }

		public IdentityManager(
			IIdentity user,
			IIdentityStore<TUser, TKey> dataContext,
			IIdentityOptions options)
		{
			_user = user;
			_dataContext = dataContext;
			Options = options ?? new IdentityOptions();
		}

		public TUser CurrentUser
		{
			get
			{
				if (_currentUser != null) return _currentUser;

				var name = GetUserName();
				_currentUser = _dataContext.UserFromName(name) ?? DefaultUser;

				InitCurrentUser(_currentUser, _user);

				return _currentUser;
			}
		}

		protected virtual void InitCurrentUser(TUser user, IIdentity identity)

		{
		}

		protected string GetUserName()
		{
			if (!Options.Enabled || _user == null)
				return Options.AnonymousUserName;

			if (_user is WindowsIdentity wi)
				return wi.IsAnonymous ? Options.AnonymousUserName : (wi.User?.Value ?? Options.AnonymousUserName);
			else
				return _user.Name;
		}

		public TUser SystemUser =>
			_dataContext.UserFromName(Options.SystemUserName) ?? throw new Exception(String.Format("User {0} does not exist in the database", Options.SystemUserName));


		public void RunAs(TUser subject, Action action)
		{
			var oldSubject = _currentUser;
			_currentUser = subject;
			action();
			_currentUser = oldSubject;
		}
	}
}
