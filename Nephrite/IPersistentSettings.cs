using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite
{
	public interface IPersistentSettings
	{
		string Get(string name);
		bool GetBool(string name);
		void SetBool(string name, bool value);
		void Set(string name, string value);

		string this[string name] { get; }
	}
}
