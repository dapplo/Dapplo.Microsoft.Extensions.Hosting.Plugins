// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.UiThread;
using System.Windows;
using System.Windows.Threading;

namespace Dapplo.Microsoft.Extensions.Hosting.Wpf
{
    /// <summary>
    /// The WPF context contains all information about the WPF application and how it's started and stopped
    /// </summary>
    public interface IWpfContext : IUiContext
    {
        /// <summary>
        /// This is the WPF ShutdownMode used for the WPF application lifetime, default is OnLastWindowClose
        /// </summary>
        ShutdownMode ShutdownMode { get; set; }

        /// <summary>
        /// The Application
        /// </summary>
        Application WpfApplication { get; set; }

        /// <summary>
        /// This WPF dispatcher
        /// </summary>
        Dispatcher Dispatcher { get; }
    }
}