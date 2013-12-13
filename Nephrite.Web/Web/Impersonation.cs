using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Security;
using System.Runtime.InteropServices;

namespace Nephrite.Web
{
    public class Impersonation : IDisposable
    {
        private IntPtr userToken = IntPtr.Zero;
        private WindowsImpersonationContext impersonatedUser = null;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        public Impersonation(string domain, string user, string password)
        {
            if (user == null || user.Length == 0)
                return;
            if (!LogonUser(user, domain, password,
            LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
            ref userToken))
                throw new SecurityException();
            WindowsIdentity newId = new WindowsIdentity(userToken);
            impersonatedUser = newId.Impersonate();
        }

        public void Dispose()
        {
            if (impersonatedUser != null)
            {
                impersonatedUser.Undo();
                CloseHandle(userToken);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);
    }

}
