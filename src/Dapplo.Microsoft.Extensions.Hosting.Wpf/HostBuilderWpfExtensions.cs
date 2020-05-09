// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.Wpf.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.Wpf
{
    /// <summary>
    /// This contains the WPF extensions for Microsoft.Extensions.Hosting
    /// </summary>
    public static class HostBuilderWpfExtensions
    {
        private const string WpfContextKey = "WpfContext";

        /// <summary>
        /// Helper method to retrieve the IWpfContext
        /// </summary>
        /// <param name="properties">IDictionary</param>
        /// <param name="wpfContext">IWpfContext out value</param>
        /// <returns>bool if there was already an IWpfContext</returns>
        private static bool TryRetrieveWpfContext(this IDictionary<object, object> properties, out IWpfContext wpfContext)
        {
            if (properties.TryGetValue(WpfContextKey, out var wpfContextAsObject))
            {
                wpfContext = (IWpfContext)wpfContextAsObject;
                return true;
            }
            wpfContext = new WpfContext();
            properties[WpfContextKey] = wpfContext;
            return false;
        }

        /// <summary>
        /// Defines that stopping the WPF application also stops the host (application)
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="shutdownMode">ShutdownMode default is OnLastWindowClose</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder UseWpfLifetime(this IHostBuilder hostBuilder, ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose)
        {
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                if (!TryRetrieveWpfContext(hostBuilder.Properties, out var wpfContext))
                {
                    throw new NotSupportedException("Please configure WPF first!");
                }

                wpfContext.ShutdownMode = shutdownMode;
                wpfContext.IsLifetimeLinked = true;
            });
            return hostBuilder;
        }

        /// <summary>
        /// Configure an WPF application
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="configureDelegate">Action to configure Wpf</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureWpf(this IHostBuilder hostBuilder, Action<IWpfBuilder> configureDelegate = null)
        {
            var wpfBuilder = new WpfBuilder();
            configureDelegate?.Invoke(wpfBuilder);

            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                if (!TryRetrieveWpfContext(hostBuilder.Properties, out var wpfContext))
                {
                    serviceCollection.AddSingleton(wpfContext);
                    serviceCollection.AddSingleton(serviceProvider => new WpfThread(serviceProvider));
                    serviceCollection.AddHostedService<WpfHostedService>();
                }
                wpfBuilder.ConfigureContextAction?.Invoke(wpfContext);
            });

            if (wpfBuilder.ApplicationType != null)
            {
                // Check if the registered application does inherit System.Windows.Application
                var baseApplicationType = typeof(Application);
                if (!baseApplicationType.IsAssignableFrom(wpfBuilder.ApplicationType))
                {
                    throw new ArgumentException("The registered Application type inherit System.Windows.Application", nameof(configureDelegate));
                }

                hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddSingleton(wpfBuilder.ApplicationType);

                    if (wpfBuilder.ApplicationType != baseApplicationType)
                    {
                        serviceCollection.AddSingleton(serviceProvider => (Application)serviceProvider.GetRequiredService(wpfBuilder.ApplicationType));
                    }
                });
            }

            if (wpfBuilder.WindowTypes.Count > 0)
            {
                hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    foreach (var wpfWindowType in wpfBuilder.WindowTypes)
                    {
                        serviceCollection.AddSingleton(wpfWindowType);

                        // Check if it also implements IWpfShell so we can register it as this
                        var shellInterfaceType = typeof(IWpfShell);
                        if (shellInterfaceType.IsAssignableFrom(wpfWindowType))
                        {
                            serviceCollection.AddSingleton(shellInterfaceType, serviceProvider => serviceProvider.GetRequiredService(wpfWindowType));
                        }
                    }
                });
            }

            return hostBuilder;
        }
    }
}