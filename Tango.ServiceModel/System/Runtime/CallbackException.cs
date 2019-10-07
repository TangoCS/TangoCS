//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Runtime
{
	using System;

	[Serializable]
	class CallbackException : FatalException
	{
		public CallbackException()
		{
		}

		public CallbackException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}