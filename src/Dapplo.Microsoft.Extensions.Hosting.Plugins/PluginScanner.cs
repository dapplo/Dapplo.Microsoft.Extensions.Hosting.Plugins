// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins
{
    /// <summary>
    /// This contains the different PluginScanner methods
    /// </summary>
    public static class PluginScanner
    {
        /// <summary>
        /// Create a single IPlugin by creating it directly for the naming convention "AssemblyName.Plugin"
        /// </summary>
        /// <param name="pluginAssembly">pluginAssembly</param>
        /// <returns>IEnumerable with one IPlugin, or empty</returns>
        public static IEnumerable<IPlugin> ByNamingConvention(Assembly pluginAssembly)
        {
            var assemblyName = pluginAssembly.GetName().Name;
            var type = pluginAssembly.GetType($"{assemblyName}.Plugin", false, false);
            if (type != null)
            {
                yield return (IPlugin)Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// Create instances of IPlugin found in the assembly
        /// </summary>
        /// <param name="pluginAssembly">pluginAssembly</param>
        /// <returns>IEnumerable of IPlugin</returns>
        public static IEnumerable<IPlugin> ScanForPluginInstances(Assembly pluginAssembly)
        {
            var pluginType = typeof(IPlugin);
            return pluginAssembly.ExportedTypes
                .Where(type => pluginType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type => (IPlugin)Activator.CreateInstance(type));
        }
    }
}
