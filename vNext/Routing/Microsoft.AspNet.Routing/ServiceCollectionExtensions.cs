// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions_Routing
    {
        /// <summary>
        /// Configures a set of <see cref="RouteOptions"/> for the application.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="RouteOptions"/>.</param>
        public static void ConfigureRouting(
            this IServiceCollection services,
            [NotNull] Action<RouteOptions> setupAction)
        {
            services.Configure(setupAction);
        }
    }
}