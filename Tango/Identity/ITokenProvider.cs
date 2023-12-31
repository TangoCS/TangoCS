using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Identity
{
	public interface ITokenProvider
	{
		string Generate(string purpose);
		bool Validate(string purpose, string token);
	}
}
