// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dapplo.Microsoft.Extensions.Hosting.AppServices.Internal
{
    /// <summary>
    /// This maintains the mutex lifetime
    /// </summary>
    internal class MutexLifetimeService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IMutexBuilder _mutexBuilder;
        private ResourceMutex _resourceMutex;

        public MutexLifetimeService(ILogger<MutexLifetimeService> logger, IHostEnvironment hostEnvironment, IHostApplicationLifetime hostApplicationLifetime, IMutexBuilder mutexBuilder)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _hostApplicationLifetime = hostApplicationLifetime;
            _mutexBuilder = mutexBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _resourceMutex = ResourceMutex.Create(null, _mutexBuilder.MutexId, _hostEnvironment.ApplicationName, _mutexBuilder.IsGlobal);

            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
            if (!_resourceMutex.IsLocked)
            {
                _mutexBuilder.WhenNotFirstInstance?.Invoke(_hostEnvironment, _logger);
                _logger.LogDebug("Application {0} already running, stopping application.", _hostEnvironment.ApplicationName);
                _hostApplicationLifetime.StopApplication();
            }

            return Task.CompletedTask;
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called, closing mutex.");
            _resourceMutex.Dispose();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}