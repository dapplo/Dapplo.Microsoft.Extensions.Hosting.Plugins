// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;
using System.Reflection;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins
{
    /// <summary>
    /// The plug-in builder is used to configure the plug-in loading.
    /// </summary>
    public interface IPluginBuilder
    {
        /// <summary>
        /// In these directories we will scan for plug-ins
        /// </summary>
        IList<string> PluginDirectories { get; }

        /// <summary>
        /// In these directories we will scan for framework assemblies
        /// </summary>
        IList<string> FrameworkDirectories { get; }

        /// <summary>
        /// Specify to use the content root for scanning
        /// </summary>
        bool UseContentRoot { get; set; }

        /// <summary>
        /// The matcher used to find all the framework assemblies
        /// </summary>
        Matcher FrameworkMatcher { get; }

        /// <summary>
        /// The matcher to find all the plugins
        /// </summary>
        Matcher PluginMatcher { get; }

        /// <summary>
        /// Specifies a way to validate the plugin file before it's being loaded 
        /// </summary>
        Func<string, bool> ValidatePlugin { get; set; }

        /// <summary>
        /// Specify the Assembly scan function, which takes the Assembly and returns the IPlugin(s) for it.
        /// Available functions are:
        /// PluginScanner.ByNamingConvention which is fast, but finds only one IPlugin by convention
        /// PluginScanner.ScanForPluginInstances which is the default and finds all public classes implementing IPlugin
        /// </summary>
        Func<Assembly, IEnumerable<IPlugin>> AssemblyScanFunc { get; set; }
    }
}
