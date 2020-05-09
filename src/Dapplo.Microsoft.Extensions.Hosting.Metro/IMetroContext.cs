// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dapplo.Microsoft.Extensions.Hosting.Metro
{
    /// <summary>
    /// The metro context contains all information for MahApps.Metro
    /// </summary>
    public interface IMetroContext
    {
        /// <summary>
        /// Specify all the resource to use to initialize
        /// </summary>
        List<Uri> Resources { get; }

        /// <summary>
        /// Defines all styles to use
        /// </summary>
        List<string> Styles { get; }

        /// <summary>
        /// Defines the theme
        /// </summary>
        string Theme { get; set; }

    }
}