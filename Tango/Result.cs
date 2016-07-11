using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tango
{
	public class Result
	{
		public Result(int code, string message)
		{
			Code = code;
			Message = message;
		}

		public int Code { get; private set; }
		public string Message { get; private set; }
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

		public bool Value { get; private set; }
		public string Message { get; private set; }

		public static implicit operator bool(BoolResult b) => b.Value;
		public static bool operator true(BoolResult b) => b.Value;
		public static bool operator false(BoolResult b) => !b.Value;

		public static BoolResult operator &(BoolResult left, BoolResult right)
		{
			return new BoolResult(left && right);
		}

		public static BoolResult operator |(BoolResult left, BoolResult right)
		{
			return new BoolResult(left || right);
		}
	}
}