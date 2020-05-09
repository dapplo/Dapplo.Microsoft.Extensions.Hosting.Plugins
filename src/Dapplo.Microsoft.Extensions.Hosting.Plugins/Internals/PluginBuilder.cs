// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals
{
    /// <summary>
    /// This is the implementation of the plugin builder
    /// </summary>
    internal class PluginBuilder : IPluginBuilder
    {
        /// <inheritdoc />
        public Matcher FrameworkMatcher { get; } = new Matcher();

        /// <inheritdoc />
        public Matcher PluginMatcher { get; } = new Matcher();

        /// <inheritdoc />
        public Func<string, bool> ValidatePlugin { get; set; } = s => true;

        /// <inheritdoc />
        public Func<Assembly, IEnumerable<IPlugin>> AssemblyScanFunc { get; set; } = PluginScanner.ScanForPluginInstances;

        /// <inheritdoc />
        public IList<string> PluginDirectories { get; } = new List<string>();

        /// <inheritdoc />
        public IList<string> FrameworkDirectories { get; } = new List<string>();

        /// <inheritdoc />
        public bool UseContentRoot { get; set; }
    }
}
