using System;
using System.Data;
using System.Text;
using Nephrite.Logger;
using NHibernate;
using NHibernate.AdoNet.Util;

namespace Nephrite.Hibernate
{
	public class LogInterceptor : EmptyInterceptor
	{
		BasicFormatter formatter = new BasicFormatter();
		IRequestLogger logger;

		public LogInterceptor(IRequestLogger logger)
		{
			this.logger = logger;
		}

		public override void OnPrepareCommand(IDbCommand command)
		{
			logger.Write(formatter.Format(GetCommandLineWithParameters(command)));
		}

		string GetCommandLineWithParameters(IDbCommand command)
		{
			string outputText;

			if (command.Parameters.Count == 0)
			{
				outputText = command.CommandText;
			}
			else
			{
				var output = new StringBuilder(command.CommandText.Length + (command.Parameters.Count * 20));
				output.Append(command.CommandText.TrimEnd(' ', ';', '\n'));
				output.Append(";");

				IDataParameter p;
				int count = command.Parameters.Count;
				bool appendComma = false;
				for (int i = 0; i < count; i++)
				{
					if (appendComma)
					{
						output.Append(", ");
					}
					appendComma = true;
					p = (IDataParameter)command.Parameters[i];
					output.Append(string.Format("{0} = {1} [Type: {2}]", p.ParameterName, GetParameterLogableValue(p), GetParameterLogableType(p)));
				}
				outputText = output.ToString();
			}
			return outputText;
		}

		string GetParameterLogableType(IDataParameter dataParameter)
		{
			var p = dataParameter as IDbDataParameter;
			if (p != null)
				return p.DbType + " (" + p.Size + ")";
			return p.DbType.ToString();

		}

		string GetParameterLogableValue(IDataParameter parameter)
		{
			const int maxLogableStringLength = 1000;
			if (parameter.Value == null || DBNull.Value.Equals(parameter.Value))
			{
				return "NULL";
			}
			if (IsStringType(parameter.DbType))
			{
				return string.Concat("'", TruncateWithEllipsis(parameter.Value.ToString(), maxLogableStringLength), "'");
			}
			var buffer = parameter.Value as byte[];
			if (buffer != null)
			{
				return GetBufferAsHexString(buffer);
			}
			return parameter.Value.ToString();
		}

		string GetBufferAsHexString(byte[] buffer)
		{
			const int maxBytes = 128;
			int bufferLength = buffer.Length;

			var sb = new StringBuilder(maxBytes * 2 + 8);
			sb.Append("0x");
			for (int i = 0; i < bufferLength && i < maxBytes; i++)
			{
				sb.Append(buffer[i].ToString("X2"));
			}
			if (bufferLength > maxBytes)
			{
				sb.Append("...");
			}
			return sb.ToString();
		}

		bool IsStringType(DbType dbType)
		{
			return DbType.String.Equals(dbType) || DbType.AnsiString.Equals(dbType)
						 || DbType.AnsiStringFixedLength.Equals(dbType) || DbType.StringFixedLength.Equals(dbType);
		}

		string TruncateWithEllipsis(string source, int length)
		{
			const string ellipsis = "...";
			if (source.Length > length)
			{
				return source.Substring(0, length) + ellipsis;
			}
			return source;
		}
	}
}
