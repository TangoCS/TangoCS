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
}