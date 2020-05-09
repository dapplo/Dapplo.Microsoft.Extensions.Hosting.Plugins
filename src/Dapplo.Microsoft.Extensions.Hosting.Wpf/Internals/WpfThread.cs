// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Dapplo.Microsoft.Extensions.Hosting.UiThread;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Dapplo.Microsoft.Extensions.Hosting.Wpf.Internals
{
    /// <summary>
    /// This contains the logic for the WPF thread
    /// </summary>
    public class WpfThread : BaseUiThread<IWpfContext>
    {
        /// <summary>
        /// This will create the WpfThread
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider</param>
        public WpfThread(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override void PreUiThreadStart()
        {
            // Create our SynchronizationContext, and install it:
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

            // Create the new WPF application
            var wpfApplication = ServiceProvider.GetService<Application>() ?? new Application()
            {
                ShutdownMode = UiContext.ShutdownMode
            };

            // Register to the WPF application exit to stop the host application
            wpfApplication.Exit += (s, e) =>
            {
                UiContext.IsRunning = false;
                if (UiContext.IsLifetimeLinked)
                {
                    //_logger.LogDebug("Stopping host application due to WPF application exit.");
                    ServiceProvider.GetService<IHostApplicationLifetime>().StopApplication();
                }
            };

            // Store the application for others to interact
            UiContext.WpfApplication = wpfApplication;
        }

        /// <inheritdoc />
        protected override void UiThreadStart() {
            // Mark the application as running
            UiContext.IsRunning = true;

            // Use the provided IWpfService
            var wpfServices = ServiceProvider.GetServices<IWpfService>();
            if (wpfServices != null)
            {
                foreach(var wpfService in wpfServices)
                {
                    wpfService.Initialize(UiContext.WpfApplication);
                }
            }
            // Run the WPF application in this thread which was specifically created for it, with the specified shell
            var shellWindows = ServiceProvider.GetServices<IWpfShell>().Cast<Window>().ToList();

            switch (shellWindows.Count)
            {
                case 1:
                    UiContext.WpfApplication.Run(shellWindows[0]);
                    break;
                case 0:
                    UiContext.WpfApplication.Run();
                    break;
                default:
                    UiContext.WpfApplication.Startup += (sender, args) =>
                    {
                        foreach (var window in shellWindows)
                        {
                            window?.Show();
                        }
                    };
                    UiContext.WpfApplication.Run();
                    break;
            }
        }
    }
}