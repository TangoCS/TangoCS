using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Data;

namespace Nephrite.Multilanguage
{
	public interface IDC_Multilanguage
	{
		ITable<IC_Language> IC_Language { get; }
	}

	public interface IC_Language
	{
		string Code { get; set; }
		string Title { get; set; }
		bool IsDefault { get; set; }
	}

	public interface IMultilanguage
	{
		string LanguageCode { get; set; }
	}
}