using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Html
{
	public partial class HtmlControl
	{
		public string GetString(string controlName)
		{
			return Form[controlName];
		}

		public int? GetInt(string controlName)
		{
			return Form[controlName].ToInt32();
		}

		public decimal? GetDecimal(string controlName)
		{
			return Form[controlName].ToDecimal();
		}

		public Guid? GetGuid(string controlName)
		{
			return Form[controlName].IsEmpty() ? (Guid?)null : Form[controlName].ToGuid();
		}

		public DateTime? GetDateTime(string controlName)
		{
			return Form[controlName].ToDateTime();
		}

		public DateTime? GetDate(string controlName)
		{
			return Form[controlName].ToDate();
		}
	}
}