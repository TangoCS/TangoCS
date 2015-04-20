﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Nephrite.Http
{
	public class SignOutContext : ISignOutContext
	{
		private bool _accepted;

		public SignOutContext([NotNull] string authenticationScheme, IDictionary<string, string> properties)
		{
			AuthenticationScheme = authenticationScheme;
			Properties = properties ?? new Dictionary<string, string>(StringComparer.Ordinal);
		}

		public string AuthenticationScheme { get; internal set; }

		public IDictionary<string, string> Properties { get; internal set; }

		public bool Accepted
		{
			get { return _accepted; }
		}

		public void Accept()
		{
			_accepted = true;
		}
	}
}