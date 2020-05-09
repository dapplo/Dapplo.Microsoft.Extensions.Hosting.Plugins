// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Dapplo.Hosting.Sample.WpfDemo.ViewModels;

namespace Dapplo.Hosting.Sample.WpfDemo
{
    /// <summary>
    /// A simple example WPF window
    /// </summary>
    public partial class MainWindow : IWpfShell
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(ILogger<MainWindow> logger, IServiceProvider serviceProvider, MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _logger = logger;
            _serviceProvider = serviceProvider;
            DataContext = viewModel;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("Exit-Button was clicked!");
            Application.Current.Shutdown();
        }

        private void ButtonAnotherWindow_Click(object sender, RoutedEventArgs e)
        {
            var otherWindow = _serviceProvider.GetService<OtherWindow>();
            otherWindow.Show();
        }
    }
}