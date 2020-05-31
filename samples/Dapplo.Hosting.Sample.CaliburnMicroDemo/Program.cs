// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro;
using Dapplo.Hosting.Sample.CaliburnMicroDemo.ViewModels;
using System.Threading.Tasks;

namespace Dapplo.Hosting.Sample.CaliburnMicroDemo
{
    public static class Program
    {
        private const string AppSettingsFilePrefix = "appsettings";
        private const string HostSettingsFile = "hostsettings.json";
        private const string Prefix = "PREFIX_";

        public static Task Main(string[] args)
        {
            var executableLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            
            var host = new HostBuilder()
                .ConfigureWpf()
                .ConfigureLogging()
                .ConfigureConfiguration(args)
                .ConfigureSingleInstance(builder =>
                {
                    builder.MutexId = "{B9CE32C0-59AE-4AF0-BE39-5329AAFF4BE8}";
                    builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                    {
                        // This is called when an instance was already started, this is in the second instance
                        logger.LogWarning("Application {0} already running.", hostingEnvironment.ApplicationName);
                    };
                })
                .ConfigurePlugins(pluginBuilder =>
                {
                    var runtime = Path.GetFileName(executableLocation);
                    var parentDirectory = Directory.GetParent(executableLocation).FullName;
                    var configuration = Path.GetFileName(parentDirectory);
                    var basePath = Path.Combine(executableLocation, @"..\..\..\..\");
                    // Specify the location from where the Dll's are "globbed"
                    pluginBuilder.AddScanDirectories(basePath);
                    // Add the framework libraries which can be found with the specified globs
                    pluginBuilder.IncludeFrameworks(@$"**\bin\{configuration}\netstandard2.0\*.FrameworkLib.dll");
                    // Add the plugins which can be found with the specified globs
                    pluginBuilder.IncludePlugins(@$"**\bin\{configuration}\{runtime}\*.Sample.Plugin*.dll");
                })
                .ConfigureCaliburnMicro<MainViewModel>()
                .ConfigureServices(serviceCollection =>
                {
                    // Make OtherWindow available for DI to MainWindow
                    serviceCollection.AddTransient<OtherViewModel>();
                })
                .UseConsoleLifetime()
                .UseWpfLifetime()
                .Build();

            Console.WriteLine("Run!");
            return host.RunAsync();
        }

        /// <summary>
        /// Configure the loggers
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <returns>IHostBuilder</returns>
        private static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddConsole();
                configLogging.AddDebug();
            });
        }

        /// <summary>
        /// Configure the configuration
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder ConfigureConfiguration(this IHostBuilder hostBuilder, string[] args)
        {
            return hostBuilder.ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile(HostSettingsFile, optional: true);
                configHost.AddEnvironmentVariables(prefix: Prefix);
                configHost.AddCommandLine(args);
            })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile(AppSettingsFilePrefix + ".json", optional: true);
                    if (!string.IsNullOrEmpty(hostContext.HostingEnvironment.EnvironmentName))
                    {
                        configApp.AddJsonFile(AppSettingsFilePrefix + $".{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    }
                    configApp.AddEnvironmentVariables(prefix: Prefix);
                    configApp.AddCommandLine(args);
                });
        }
    }
}
