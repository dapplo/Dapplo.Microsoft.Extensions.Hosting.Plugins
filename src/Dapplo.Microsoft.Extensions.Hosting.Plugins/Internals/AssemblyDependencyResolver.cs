// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !NETCOREAPP
using System;
using System.IO;
using System.Reflection;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals
{
    /// <summary>
    /// This is a wrapper class to simulate the behavior of the AssemblyDependencyResolver under the .NET Framework
    /// </summary>
    public class AssemblyDependencyResolver
    {
        private readonly string _pluginPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginPath">string with the path for the plugin and where his dependencies are loaded from</param>
        public AssemblyDependencyResolver(string pluginPath)
        {
            _pluginPath = System.IO.Path.GetDirectoryName(pluginPath);
        }

        /// <summary>
        /// Find the assembly in the directory of the plugin
        /// </summary>
        /// <param name="assemblyName">AssemblyName</param>
        /// <returns>string path for the assembly</returns>
        /// <exception cref="NotImplementedException"></exception>
        public string ResolveAssemblyToPath(AssemblyName assemblyName)
        {
            var assemblyPath = Path.Combine(_pluginPath, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }

            return null;
        }

        /// <summary>
        /// Find the location of an unmanaged DLL
        /// </summary>
        /// <param name="unmanagedDllName">string</param>
        /// <returns>string with the path</returns>
        /// <exception cref="NotImplementedException"></exception>
        public string ResolveUnmanagedDllToPath(string unmanagedDllName)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
