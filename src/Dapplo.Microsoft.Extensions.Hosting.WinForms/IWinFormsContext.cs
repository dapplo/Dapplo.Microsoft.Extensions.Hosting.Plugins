// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.UiThread;
using System.Windows.Threading;

namespace Dapplo.Microsoft.Extensions.Hosting.WinForms
{
    /// <summary>
    /// The WinForms context contains all information about the WinForms application and how it's started and stopped
    /// </summary>
    public interface IWinFormsContext : IUiContext
    {
        /// <summary>
        /// Specify if the visual styles need to be set, default is true
        /// </summary>
        bool EnableVisualStyles { get; set; }

        /// <summary>
        /// The dispatcher to send information to forms
        /// </summary>
        Dispatcher Dispatcher { get; set; }
    }
}