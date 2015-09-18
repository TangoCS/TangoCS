// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Routing;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Builder
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseRouter([NotNull] this IApplicationBuilder builder, [NotNull] IRouter router)
        {
            return builder.UseMiddleware<RouterMiddleware>(router);
        }
    }
}