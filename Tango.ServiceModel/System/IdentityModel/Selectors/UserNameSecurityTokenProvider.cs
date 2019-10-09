using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace System.IdentityModel.Selectors
{
	/// <summary>Represents a security token provider that provides <see cref="T:System.IdentityModel.Tokens.UserNameSecurityToken" /> security tokens for a SOAP message sender.</summary>
	/// <filterpriority>2</filterpriority>
	public class UserNameSecurityTokenProvider : SecurityTokenProvider
	{
		private UserNameSecurityToken userNameToken;

		/// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Selectors.UserNameSecurityTokenProvider" /> class using the specified username and password. </summary>
		/// <param name="userName">The username to get <see cref="T:System.IdentityModel.Tokens.UserNameSecurityToken" /> security token for.</param>
		/// <param name="password">The password of the user to get a <see cref="T:System.IdentityModel.Tokens.UserNameSecurityToken" /> security token for.</param>
		/// <filterpriority>2</filterpriority>
		public UserNameSecurityTokenProvider(string userName, string password)
		{
			if (userName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("userName");
			}
			this.userNameToken = new UserNameSecurityToken(userName, password);
		}

		/// <summary>Gets a security token based on the username and password specified in the constructor. </summary>
		/// <returns>The <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that represents the security token to get.</returns>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that specifies the timeout value for the message that gets the security token.</param>
		protected override SecurityToken GetTokenCore(TimeSpan timeout)
		{
			return this.userNameToken;
		}
	}
}
