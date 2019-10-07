//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------
namespace System.ServiceModel
{
	using System.Reflection;
	using System.ServiceModel.Channels;
	using System.Globalization;

	static class TraceUtility
	{
		internal static Exception ThrowHelperError(Exception exception, Message message)
		{
			return exception;
		}

		internal static Exception ThrowHelperError(Exception exception, Guid activityId, object source)
		{
			return exception;
		}

		internal static Exception ThrowHelperWarning(Exception exception, Message message)
		{
			return exception;
		}

		internal static ArgumentException ThrowHelperArgument(string paramName, string message, Message msg)
		{
			return (ArgumentException)TraceUtility.ThrowHelperError(new ArgumentException(message, paramName), msg);
		}

		internal static ArgumentNullException ThrowHelperArgumentNull(string paramName, Message message)
		{
			return (ArgumentNullException)TraceUtility.ThrowHelperError(new ArgumentNullException(paramName), message);
		}

		internal static void TraceHttpConnectionInformation(string localEndpoint, string remoteEndpoint, object source)
		{
			
		}

		internal static void TraceUserCodeException(Exception e, MethodInfo method)
		{
			
		}

		static TraceUtility()
		{
			
		}

		
	}
}