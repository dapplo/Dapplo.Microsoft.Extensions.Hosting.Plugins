// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;

namespace Dapplo.Microsoft.Extensions.Hosting.ReactiveUI
{
    /// <summary>
    /// This contains the rReactiveUi extensions for Microsoft.Extensions.Hosting
    /// </summary>
    public static class HostBuilderReactiveUiExtensions
    {
        /// <summary>
        /// Configure a ReactiveUI application
        /// See https://reactiveui.net/docs/handbook/routing to learn more about routing in RxUI
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureReactiveUi(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                serviceCollection.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
            });

            return hostBuilder;
        }

        /// <summary>
        /// Maps the splat locator to the IServiceProvider.
        /// Use this.Container() to access container
        /// See https://github.com/reactiveui/splat/blob/master/src/Splat.Microsoft.Extensions.DependencyInjection/README.md
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static IHost MapSplatLocator(this IHost host)
        {
            if (_container == null)
            {
                // Since MS DI container is a different type,
                // we need to re-register the built container with Splat again
                _container = host.Services;
                _container.UseMicrosoftDependencyResolver();
            }
            return host;
        }

        /// <summary>
        /// Maps the splat locator to the IServiceProvider.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="serviceProviderFactory">The service provider factory.</param>
        /// <returns></returns>
        public static IHost MapSplatLocator(this IHost host, Action<IServiceProvider> serviceProviderFactory)
        {
            var c = host.Services;
            c.UseMicrosoftDependencyResolver();
            serviceProviderFactory.Invoke(c);
            return host;
        }

        private static IServiceProvider _container;

        /// <summary>
        /// Gets the Application IServiceProvider registered with Splat.
        /// </summary>
        /// <param name="_">The .</param>
        /// <returns></returns>
        public static IServiceProvider Container(this object _) => _container;
    }
}