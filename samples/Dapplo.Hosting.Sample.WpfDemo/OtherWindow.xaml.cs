// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System.Windows;

namespace Dapplo.Hosting.Sample.WpfDemo
{
    /// <summary>
    /// A simple example WPF window
    /// </summary>
    public partial class OtherWindow
    {
        private readonly ILogger<OtherWindow> _logger;

        public OtherWindow(ILogger<OtherWindow> logger)
        {
            InitializeComponent();
            _logger = logger;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Closing OtherWindow");
            Close();
        }
    }
}