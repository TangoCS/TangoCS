using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
	public class ValidationMessage
	{
		public string Message { get; private set; }
		public ValidationMessageSeverity Severity { get; private set; }

		public ValidationMessage(string message, ValidationMessageSeverity severity)
		{
			Message = message;
			Severity = severity;
		}
	}

	public enum ValidationMessageSeverity
	{
		Error,
		Warning
	}
}