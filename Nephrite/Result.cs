using System;

namespace Nephrite
{
	public class Result
	{
		public Result(int code, string message)
		{
			Code = code;
			Message = message;
		}

		public int Code { get; set; }
		public string Message { get; set; }
	}

	public class BoolResult
	{
		static BoolResult _true = new BoolResult(true);
		public static BoolResult True
		{
			get { return _true; }
		}

		public BoolResult(bool value, string message = "")
		{
			Value = value;
			Message = message;
		}

		public bool Value { get; set; }
		public string Message { get; set; }
	}

	public class ValidationMessage
	{
		public string Name { get; set; }
		public ValidationMessageSeverity Severity { get; private set; }
		public string Message { get; private set; }

		public ValidationMessage(string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Name = name;
			Message = message;
			Severity = severity;
		}

		public ValidationMessage(string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Message = message;
			Severity = severity;
		}
	}

	public enum ValidationMessageSeverity
	{
		Error,
		Warning,
		Information
	}
}