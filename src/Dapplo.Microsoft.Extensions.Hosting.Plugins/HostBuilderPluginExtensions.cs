﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2019 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Microsoft.Extensions.Hosting
// 
//  Dapplo.Microsoft.Extensions.Hosting is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Microsoft.Extensions.Hosting is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Microsoft.Extensions.Hosting. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins
{
    /// <summary>
    /// Extensions for adding plug-ins to your host
    /// </summary>
    public static class HostBuilderPluginExtensions
    {
        private const string PluginBuilderKey = "PluginBuilder";

        /// <summary>
        /// Helper method to retrieve the plugin builder
        /// </summary>
        /// <param name="properties">IDictionary</param>
        /// <param name="pluginBuilder">IPluginBuilder out value</param>
        /// <returns>bool if there was a matcher</returns>
        private static bool TryRetrievePluginBuilder(this IDictionary<object, object> properties, out IPluginBuilder pluginBuilder)
        {
            if (properties.TryGetValue(PluginBuilderKey, out var pluginBuilderObject))
            {
                pluginBuilder = pluginBuilderObject as IPluginBuilder;
                return true;

            }
            pluginBuilder = new PluginBuilder();
            properties[PluginBuilderKey] = pluginBuilder;
            return false;
        }

        /// <summary>
        /// Configure the plugins
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        /// <param name="configurePlugin">Action to configure the IPluginBuilder</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder ConfigurePlugins(this IHostBuilder hostBuilder, Action<IPluginBuilder> configurePlugin)
        {
            if (!hostBuilder.Properties.TryRetrievePluginBuilder(out var pluginBuilder))
            {
                // Configure a single time
                ConfigurePluginScanAndLoad(hostBuilder);
            }
            configurePlugin(pluginBuilder);

            return hostBuilder;
        }

        /// <summary>
        /// This enables scanning for and loading of plug-ins
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder</param>
        private static void ConfigurePluginScanAndLoad(IHostBuilder hostBuilder)
        {
            // Configure the actual scanning & loading
            hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                hostBuilder.Properties.TryRetrievePluginBuilder(out var pluginBuilder);

                if (pluginBuilder.UseContentRoot)
                {
                    var contentRootPath = hostBuilderContext.HostingEnvironment.ContentRootPath;
                    pluginBuilder.AddScanDirectories(contentRootPath);
                }

                if (pluginBuilder.FrameworkDirectories.Count > 0)
                {
                    foreach (var frameworkScanRoot in pluginBuilder.FrameworkDirectories)
                    {
                        // Do the globbing and try to load the framework files into the default AssemblyLoadContext
                        foreach (var frameworkAssemblyPath in pluginBuilder.FrameworkMatcher.GetResultsInFullPath(frameworkScanRoot))
                        {
                            var frameworkAssemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(frameworkAssemblyPath));
                            if (AssemblyLoadContext.Default.TryGetAssembly(frameworkAssemblyName, out _))
                            {
                                continue;
                            }

                            // TODO: Log the loading?
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(frameworkAssemblyPath);
                        }
                    }
                }

                if (pluginBuilder.PluginDirectories.Count > 0)
                {
                    foreach (var pluginScanRootPath in pluginBuilder.PluginDirectories)
                    {
                        // Do the globbing and try to load the plug-ins
                        var pluginPaths = pluginBuilder.PluginMatcher.GetResultsInFullPath(pluginScanRootPath);
                        var plugins = pluginPaths
                            .Select(LoadPlugin)
                            .Where(plugin => plugin != null)
                            .OrderBy(plugin => plugin.GetOrder());
                        foreach (var plugin in plugins)
                        {
                            plugin.ConfigureHost(hostBuilderContext, serviceCollection);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Helper method to process the PluginOrder attribute
        /// </summary>
        /// <param name="plugin">IPlugin</param>
        /// <returns>int</returns>
        private static int GetOrder(this IPlugin plugin)
        {
            return plugin.GetType().GetCustomAttribute<PluginOrderAttribute>()?.Order ?? 0;
        }

        /// <summary>
        /// Helper method to load an assembly which contains a single plugin
        /// </summary>
        /// <param name="pluginLocation">string</param>
        /// <returns>IPlugin</returns>
        private static IPlugin LoadPlugin(string pluginLocation)
        {
            if (!File.Exists(pluginLocation))
            {
                // TODO: Log an error, how to get a logger here?
                return null;
            }
            
            // TODO: Log verbose that we are loading a plugin
            var pluginName = Path.GetFileNameWithoutExtension(pluginLocation);
            // TODO: Decide if we rather have this to come up with the name: AssemblyName.GetAssemblyName(pluginLocation)
            var pluginAssemblyName = new AssemblyName(pluginName);
            if (AssemblyLoadContext.Default.TryGetAssembly(pluginAssemblyName, out _))
            {
                return null;
            }
            var loadContext = new PluginLoadContext(pluginLocation, pluginName);
            var assembly = loadContext.LoadFromAssemblyName(pluginAssemblyName);

            // TODO: Check if we want to scan all assemblies, or have a specify class on a predetermined location?
            var interfaceType = typeof(IPlugin);
            foreach (var type in assembly.GetExportedTypes())
            {
                if (!type.GetInterfaces().Contains(interfaceType))
                {
                    continue;
                }
                var plugin = Activator.CreateInstance(type) as IPlugin;
                return plugin;
            }
            return null;
        }
    }
}
