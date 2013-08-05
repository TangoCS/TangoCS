using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Multilanguage
{
	public interface IDC_Multilanguage
	{
		IQueryable<IC_Language> C_Language { get; }
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