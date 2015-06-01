// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Builder
{
    public static class RunExtensions
    {
        public static void Run([NotNull] this IApplicationBuilder app, [NotNull] RequestDelegate handler)
        {
            app.Use(_ => handler);
        }
    }
}