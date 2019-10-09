// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;

namespace System
{
	internal static partial class FxTrace
	{
		private static ExceptionTrace s_exceptionTrace;

		public static ExceptionTrace Exception {
			get {
				if (s_exceptionTrace == null)
				{
					// don't need a lock here since a true singleton is not required
					s_exceptionTrace = new ExceptionTrace();
				}

				return s_exceptionTrace;
			}
		}
	}
}