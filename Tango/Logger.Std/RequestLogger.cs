using System;
using System.Text;

namespace Tango.Logger.Std
{
	public class RequestLogger : IRequestLogger
	{
		StringBuilder sb = new StringBuilder();

		public bool Enabled => true;

		public void Write(string message)
		{
			sb.AppendLine(message).AppendLine();
		}

		public void WriteFormat(string message, object[] args)
		{
			sb.AppendFormat(message, args).AppendLine();
		}

		public override string ToString()
		{
			return sb.ToString();
		}
	}

	public class EmptyLogger : IRequestLogger
	{
		public bool Enabled => false;

		public void Write(string message)
		{
			
		}

		public void WriteFormat(string message, object[] args)
		{
			
		}

		public override string ToString()
		{
			return "";
		}
	}
}
