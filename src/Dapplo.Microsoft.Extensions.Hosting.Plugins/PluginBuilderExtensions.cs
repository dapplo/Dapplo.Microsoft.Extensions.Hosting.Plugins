// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins
{
    /// <summary>
    ///
    /// </summary>
    public static class PluginBuilderExtensions
    {
        /// <summary>
        /// Add a directory to scan both framework and plug-in assemblies
        /// </summary>
        /// <param name="pluginBuilder">IPluginBuilder</param>
        /// <param name="directories">string array</param>
        public static void AddScanDirectories(this IPluginBuilder pluginBuilder, params string[] directories)
        {
            foreach (var directory in directories)
            {
                var normalizedDirectory = Path.GetFullPath(directory);
                pluginBuilder.FrameworkDirectories.Add(normalizedDirectory);
                pluginBuilder.PluginDirectories.Add(normalizedDirectory);
            }
        }

        /// <summary>
        /// Exclude globs to look for framework assemblies
        /// </summary>
        /// <param name="pluginBuilder">IPluginBuilder</param>
        /// <param name="frameworkGlobs">string array</param>
        public static void ExcludeFrameworks(this IPluginBuilder pluginBuilder, params string[] frameworkGlobs)
        {
            foreach (var glob in frameworkGlobs)
            {
                pluginBuilder.FrameworkMatcher.AddExclude(glob);
            }
        }

        /// <summary>
        /// Exclude globs to look for plug-in assemblies
        /// </summary>
        /// <param name="pluginBuilder">IPluginBuilder</param>
        /// <param name="pluginGlobs">string array</param>
        public static void ExcludePlugins(this IPluginBuilder pluginBuilder, params string[] pluginGlobs)
        {
            foreach (var glob in pluginGlobs)
            {
                pluginBuilder.PluginMatcher.AddExclude(glob);
            }
        }

        /// <summary>
        /// Include globs to look for framework assemblies
        /// </summary>
        /// <param name="pluginBuilder">IPluginBuilder</param>
        /// <param name="frameworkGlobs">string array</param>
        public static void IncludeFrameworks(this IPluginBuilder pluginBuilder, params string[] frameworkGlobs)
        {
            foreach (var glob in frameworkGlobs)
            {
                pluginBuilder.FrameworkMatcher.AddInclude(glob);
            }
        }

        /// <summary>
        /// Include globs to look for plugin assemblies
        /// </summary>
        /// <param name="pluginBuilder">IPluginBuilder</param>
        /// <param name="pluginGlobs">string array</param>
        public static void IncludePlugins(this IPluginBuilder pluginBuilder, params string[] pluginGlobs)
        {
            foreach (var glob in pluginGlobs)
            {
                pluginBuilder.PluginMatcher.AddInclude(glob);
            }
        }

    }
}
