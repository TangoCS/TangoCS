using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
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
		public BoolResult(bool value, string message)
		{
			Value = value;
			Message = message;
		}

		public bool Value { get; set; }
		public string Message { get; set; }
	}
}