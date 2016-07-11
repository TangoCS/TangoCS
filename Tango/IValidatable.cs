using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango
{
	public interface IValidatable
	{
		bool CheckValid();
		List<ValidationMessage> GetValidationMessages();
	}
}