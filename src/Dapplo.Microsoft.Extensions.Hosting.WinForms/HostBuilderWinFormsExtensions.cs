// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dapplo.Microsoft.Extensions.Hosting.WinForms.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.WinForms
{
    /// <summary>
    /// This contains the WinForms extensions for Microsoft.Extensions.Hosting
    /// </summary>
    public static class HostBuilderWinFormsExtensions
    {
        private const string WinFormsContextKey = "WinFormsContext";

        /// <summary>
        /// Helper method to retrieve the IWinFormsContext
        /// </summary>
        /// <param name="properties">IDictionary</param>
        /// <param name="winFormsContext">IWinFormsContext out value</param>
        /// <returns>bool if there was already an IWinFormsContext</returns>
        private static bool TryRetrieveWinFormsContext(this IDictionary<object, object> properties, out IWinFormsContext winFormsContext)
        {
            if (properties.TryGetValue(WinFormsContextKey, out var winFormsContextAsObject))
            {
                winFormsContext = (IWinFormsContext)winFormsContextAsObject;
                return true;

            }
            winFormsContext = new WinFormsContext();
            properties[WinFormsContextKey] = winFormsContext;
            return false;
        }

        /// <summary>
        /// Defines that stopping the WinForms application also stops the host (application)
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder UseWinFormsLifetime(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                TryRetrieveWinFormsContext(hostBuilder.Properties, out var winFormsContext);
                winFormsContext.IsLifetimeLinked = true;
            });
            return hostBuilder;
        }

        /// <summary>
        /// Configure an WinForms application
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="configureAction">Action to configure the Application</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureWinForms(this IHostBuilder hostBuilder, Action<IWinFormsContext> configureAction = null)
        {
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                if (!TryRetrieveWinFormsContext(hostBuilder.Properties, out var winFormsContext))
                {
                    serviceCollection.AddSingleton(winFormsContext);
                    serviceCollection.AddSingleton(serviceProvider => new WinFormsThread(serviceProvider));
                    serviceCollection.AddHostedService<WinFormsHostedService>();
                }
                configureAction?.Invoke(winFormsContext);
            });
            return hostBuilder;
        }

        /// <summary>
        /// Configure an WinForms application
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="configureAction">Action to configure the Application</param>
        /// <typeparam name="TView">Type for the View</typeparam>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureWinForms<TView>(this IHostBuilder hostBuilder, Action<IWinFormsContext> configureAction = null) where TView : Form
        {
            hostBuilder.ConfigureWinForms(configureAction);
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                serviceCollection.AddSingleton<TView>();

                // Check if it also implements IWinFormsShell so we can register it as this
                var viewType = typeof(TView);
                var shellInterfaceType = typeof(IWinFormsShell);
                if (shellInterfaceType.IsAssignableFrom(viewType))
                {
                    serviceCollection.AddSingleton(shellInterfaceType, serviceProvider => serviceProvider.GetRequiredService<TView>());
                }
            });
            return hostBuilder;
        }

        /// <summary>
        /// Specify a shell, the primary Form, to start
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <typeparam name="TShell">Type for the shell, must derive from Form and implement IWinFormsShell</typeparam>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureWinFormsShell<TShell>(this IHostBuilder hostBuilder) where TShell : Form, IWinFormsShell
        {
            return hostBuilder.ConfigureWinForms<TShell>();
        }
    }
}