using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public partial class HtmlControl
	{
		public string GetString(string controlName)
		{
			return FormValues[controlName];
		}

		public int? GetInt(string controlName)
		{
			return FormValues[controlName].ToInt32();
		}

		public decimal? GetDecimal(string controlName)
		{
			return FormValues[controlName].ToDecimal();
		}

		public Guid? GetGuid(string controlName)
		{
			return FormValues[controlName].IsEmpty() ? (Guid?)null : FormValues[controlName].ToGuid();
		}

		public DateTime? GetDateTime(string controlName)
		{
			return FormValues[controlName].ToDateTime();
		}

		public DateTime? GetDate(string controlName)
		{
			return FormValues[controlName].ToDate();
		}
	}
}