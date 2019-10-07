// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Runtime
{
	using System;

	[Serializable]
	class FatalException : SystemException
	{
		public FatalException()
		{
		}
		public FatalException(string message) : base(message)
		{
		}

		public FatalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}