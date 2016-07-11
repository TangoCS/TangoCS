using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Identity
{
	public class OldPasswordHasher : IPasswordHasher
	{
		public string CreateHash(string password) 
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] h = SHA512.Create().ComputeHash(encoding.GetBytes(password));
			return BitConverter.ToString(h).Replace("-", "");
        }

		public bool ValidatePassword(string password, string correctHash)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] h = SHA512.Create().ComputeHash(encoding.GetBytes(password));

			byte[] h2 = Enumerable.Range(0, correctHash.Length)
					 .Where(x => x % 2 == 0)
					 .Select(x => Convert.ToByte(correctHash.Substring(x, 2), 16))
					 .ToArray();
			return h2.SequenceEqual(h);
		}
	}
}
