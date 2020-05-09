// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Caliburn.Micro;
using Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.Internals;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro
{
    /// <summary>
    /// This contains the CaliburnMicro extensions for Microsoft.Extensions.Hosting
    /// </summary>
    public static class HostBuilderCaliburnMicroExtensions
    {
        private const string CaliburnMicroContextKey = "CaliburnMicroContext";

        /// <summary>
        /// Helper method to retrieve the ICaliburnMicroContext
        /// </summary>
        /// <param name="properties">IDictionary</param>
        /// <param name="caliburnMicroContext">ICaliburnMicroContext</param>
        /// <returns>bool if there was already an </returns>
        private static bool TryRetrieveCaliburnMicroContext(this IDictionary<object, object> properties, out ICaliburnMicroContext caliburnMicroContext)
        {
            if (properties.TryGetValue(CaliburnMicroContextKey, out var caliburnContextAsObject))
            {
                caliburnMicroContext = (ICaliburnMicroContext)caliburnContextAsObject;
                return true;

            }
            caliburnMicroContext = new CaliburnMicroContext();
            properties[CaliburnMicroContextKey] = caliburnMicroContext;
            return false;
        }

        /// <summary>
        /// Configure Caliburn.Micro
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureCaliburnMicro(this IHostBuilder hostBuilder)
        {
            if (!TryRetrieveCaliburnMicroContext(hostBuilder.Properties,out var caliburnMicroContext))
            {
                hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddSingleton(caliburnMicroContext);
                    serviceCollection.AddSingleton<IWindowManager, CaliburnMicroWindowManager>();
                    serviceCollection.AddSingleton<IWpfService, CaliburnMicroBootstrapper>();
                });
            }
            return hostBuilder;
        }

        /// <summary>
        /// Configure Caliburn.Micro with the shell
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigureCaliburnMicro<TShell>(this IHostBuilder hostBuilder) where TShell : class, ICaliburnMicroShell
        {
            hostBuilder.ConfigureCaliburnMicro();
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                serviceCollection.AddSingleton<ICaliburnMicroShell, TShell>();
            });

            return hostBuilder;
        }
    }
}