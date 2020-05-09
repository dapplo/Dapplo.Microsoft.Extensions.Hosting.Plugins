// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dapplo.Microsoft.Extensions.Hosting.AppServices.Internal
{
    /// <summary>
    /// This is the configuration for the mutex service
    /// </summary>
    internal class MutexBuilder : IMutexBuilder
    {
        /// <inheritdoc />
        public string MutexId { get; set; }

        /// <inheritdoc />
        public bool IsGlobal { get; set; }

        /// <inheritdoc />
        public Action<IHostEnvironment, ILogger> WhenNotFirstInstance { get; set; }
    }
}
