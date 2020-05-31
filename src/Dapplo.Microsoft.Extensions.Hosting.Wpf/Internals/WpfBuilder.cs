// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows;

namespace Dapplo.Microsoft.Extensions.Hosting.Wpf.Internals
{
    /// <inheritdoc/>
    internal class WpfBuilder : IWpfBuilder
    {
        /// <inheritdoc/>
        public Type ApplicationType { get; set; }

        /// <inheritdoc/>
        public Application Application { get; set; }

        /// <inheritdoc/>
        public IList<Type> WindowTypes { get; } = new List<Type>();

        /// <inheritdoc/>
        public Action<IWpfContext> ConfigureContextAction { get; set; }
    }
}