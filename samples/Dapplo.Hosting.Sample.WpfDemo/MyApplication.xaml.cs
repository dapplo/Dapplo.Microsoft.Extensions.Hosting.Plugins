// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System.Windows;

namespace Dapplo.Hosting.Sample.WpfDemo
{
    /// <summary>
    /// Interaction logic for MyApplication.xaml
    /// </summary>
    public partial class MyApplication : Application
    {
        public MyApplication(ILogger<MyApplication> logger)
        {
            InitializeComponent();

            logger.LogInformation("MyApplication was created");
        }
    }
}
