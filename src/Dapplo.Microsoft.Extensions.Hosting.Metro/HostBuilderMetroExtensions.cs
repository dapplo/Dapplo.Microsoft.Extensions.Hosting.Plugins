// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dapplo.Microsoft.Extensions.Hosting.Metro.Internals;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.Metro
{
    /// <summary>
    /// This contains the Metro extensions for Microsoft.Extensions.Hosting 
    /// </summary>
    public static class HostBuilderMetroExtensions
    {
        private const string MetroContextKey = "MetroContext";

        /// <summary>
        /// Helper method to retrieve the IMetroContext
        /// </summary>
        /// <param name="properties">IDictionary</param>
        /// <param name="metroContext">IMetroContext out value</param>
        /// <returns>bool if there was already an IMetroContext</returns>
        private static bool TryRetrieveMetroContext(this IDictionary<object, object> properties, out IMetroContext metroContext)
        {
            if (properties.TryGetValue(MetroContextKey, out var metroContextAsObject))
            {
                metroContext = (IMetroContext)metroContextAsObject;
                return true;

            }
            metroContext = new MetroContext();
            properties[MetroContextKey] = metroContext;
            return false;
        }

        /// <summary>
        /// This enables MahApps.Metro
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="configureAction">Action to configure IMetroContext</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureMetro(this IHostBuilder hostBuilder, Action<IMetroContext> configureAction = null)
        {
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                if (!TryRetrieveMetroContext(hostBuilder.Properties, out var metroContext))
                {
                    serviceCollection.AddSingleton(metroContext);
                    serviceCollection.AddSingleton<IWpfService,MetroWpfService>();
                }
                // Configure the default styles
                metroContext.Styles.AddRange(new[] { "Controls", "Fonts" });
                configureAction?.Invoke(metroContext);
            });
            return hostBuilder;
        }

        /// <summary>
        /// Configure WPF to use MahApps.Metro and specify the theme
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="theme">string</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureMetro(this IHostBuilder hostBuilder, string theme)
        {
            return hostBuilder.ConfigureMetro(context => context.Theme = theme);
        }
    }
}