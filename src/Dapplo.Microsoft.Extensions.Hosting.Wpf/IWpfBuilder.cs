// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dapplo.Microsoft.Extensions.Hosting.Wpf
{
    /// <summary>
    /// Interface used for configuring Wpf 
    /// </summary>
    public interface IWpfBuilder
    {
        /// <summary>
        /// Type of the application that will be used
        /// </summary>
        Type ApplicationType { get; set; }
        /// <summary>
        /// Type of the windows that will be used
        /// </summary>
        IList<Type> WindowTypes { get; }
        /// <summary>
        /// Action to configure the Wpf context
        /// </summary>
        Action<IWpfContext> ConfigureContextAction { get; set; }
    }
}
