using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
	public interface IValidatable
	{
		bool CheckValid();
		List<ValidationMessage> GetValidationMessages();
	}
}