// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals
{
    /// <summary>
    /// This AssemblyLoadContext uses an AssemblyDependencyResolver as described here: https://devblogs.microsoft.com/dotnet/announcing-net-core-3-preview-3/
    /// Before loading an assembly, the current domain is checked if this assembly was not already loaded, if so this is returned.
    /// This way the Assemblies already loaded by the application are available to all the plugins and can provide interaction.
    /// </summary>
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath, string name) : base(name)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <summary>
        /// Returns the path where the specified assembly can be found
        /// </summary>
        /// <param name="assemblyName">AssemblyName</param>
        /// <returns>string with the path</returns>
        public string ResolveAssemblyPath(AssemblyName assemblyName)
        {
            return _resolver.ResolveAssemblyToPath(assemblyName);
        }

        /// <inheritdoc />
        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Try to get the assembly from the AssemblyLoadContext.Default, when it is already loaded
            if (Default.TryGetAssembly(assemblyName, out var alreadyLoadedAssembly))
            {
                return alreadyLoadedAssembly;
            }
            var assemblyPath = ResolveAssemblyPath(assemblyName);
            if (assemblyPath == null)
            {
                return null;
            }

            var resultAssembly = LoadFromAssemblyPath(assemblyPath);
            return resultAssembly;
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath == null)
            {
                return IntPtr.Zero;
            }

            return LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}
