// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals
{
    /// <summary>
    /// AssemblyLoadContext extensions
    /// </summary>
    public static class AssemblyLoadContextExtensions
    {
        /// <summary>
        /// Try to get an assembly from the specified AssemblyLoadContext
        /// </summary>
        /// <param name="assemblyLoadContext">AssemblyLoadContext</param>
        /// <param name="assemblyName">AssemblyName to look for</param>
        /// <param name="foundAssembly">Assembly out</param>
        /// <returns>bool</returns>
        public static bool TryGetAssembly(this AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName, out Assembly foundAssembly)
        {
            foreach (var assembly in assemblyLoadContext.Assemblies)
            {
                if (!assembly.GetName().Name.Equals(assemblyName.Name))
                {
                    continue;
                }
                foundAssembly = assembly;
                return true;
            }
            foundAssembly = null;
            return false;
        }
    }
}