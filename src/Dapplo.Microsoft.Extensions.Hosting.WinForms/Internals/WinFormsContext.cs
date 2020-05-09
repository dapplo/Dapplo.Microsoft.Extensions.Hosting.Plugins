// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.UiThread;
using System.Windows.Threading;

namespace Dapplo.Microsoft.Extensions.Hosting.WinForms.Internals
{
    /// <inheritdoc cref="IWinFormsContext"/>
    public class WinFormsContext : BaseUiContext, IWinFormsContext
    {
        /// <inheritdoc />
        public bool EnableVisualStyles { get; set; } = true;

        /// <inheritdoc />
        public Dispatcher Dispatcher { get; set; }
    }
}