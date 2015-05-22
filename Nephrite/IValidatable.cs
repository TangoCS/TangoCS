using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite
{
	public interface IValidatable
	{
		bool CheckValid();
		List<ValidationMessage> GetValidationMessages();
	}
}