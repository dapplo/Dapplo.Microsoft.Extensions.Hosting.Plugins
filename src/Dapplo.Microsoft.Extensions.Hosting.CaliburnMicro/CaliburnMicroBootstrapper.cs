// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
#if NETCOREAPP
using System.Runtime.Loader;
#endif
using Microsoft.Extensions.DependencyInjection;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;

namespace Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro
{
    /// <summary>
    ///     An implementation of the Caliburn Micro Bootstrapper which is started from
    ///     the generic host.
    /// </summary>
    public class CaliburnMicroBootstrapper : BootstrapperBase, IWpfService
    {
        private readonly ILogger<CaliburnMicroBootstrapper> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWindowManager _windowManager;
        private readonly IWpfContext _wpfContext;
        private readonly ICaliburnMicroContext _caliburnMicroContext;

        /// <summary>
        /// CaliburnMicroBootstrapper
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <param name="windowManager">IWindowManager</param>
        /// <param name="wpfContext">IWpfContext</param>
        /// <param name="caliburnMicroContext">ICaliburnMicroContext</param>
        public CaliburnMicroBootstrapper(
            ILogger<CaliburnMicroBootstrapper> logger,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IWindowManager windowManager,
            IWpfContext wpfContext,
            ICaliburnMicroContext caliburnMicroContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
            _windowManager = windowManager;
            _wpfContext = wpfContext ?? throw new ArgumentNullException(nameof(wpfContext));
            _caliburnMicroContext = caliburnMicroContext;
        }

        /// <summary>
        ///     Fill imports of the supplied instance
        /// </summary>
        /// <param name="instance">some object to fill</param>
        protected override void BuildUp(object instance)
        {
            _logger.LogDebug("Should buildup {0}", instance?.GetType().Name);
            // TODO: don't know how to fill imports yet?
            //_bootstrapper.Container.InjectProperties(instance);
        }

        /// <summary>
        ///     Configure Caliburn.Micro
        /// </summary>
        [SuppressMessage("Sonar Code Smell", "S2696:Instance members should not write to static fields", Justification = "This is the only location where it makes sense.")]
        protected override void Configure()
        {
            // Create a logger to log caliburn message
            LogManager.GetLog = type => new CaliburnLogger(_loggerFactory.CreateLogger(type));
            ConfigureViewLocator();

            if (_caliburnMicroContext.EnableOriginalDataContext)
            {
                MessageBinder.SpecialValues.Add("$originalDataContext", context =>
                {
                    var routedEventArgs = context.EventArgs as RoutedEventArgs;
                    var frameworkElement = routedEventArgs?.OriginalSource as FrameworkElement;
                    return frameworkElement?.DataContext;
                });
            }
        }

        /// <summary>
        ///     Add logic to find the base ViewType if the default locator can't find a view.
        /// </summary>
        [SuppressMessage("Sonar Code Smell", "S2696:Instance members should not write to static fields", Justification = "This is the only location where it makes sense.")]
        private void ConfigureViewLocator()
        {
            var defaultLocator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                var viewType = defaultLocator(modelType, displayLocation, context);
                bool initialViewFound = viewType != null;

                if (initialViewFound)
                {
                    return viewType;
                }
                _logger.LogDebug("No view for {0}, looking into base types.", modelType);
                var currentModelType = modelType;
                while (viewType == null && currentModelType != null && currentModelType != typeof(object) && currentModelType != typeof(Screen))
                {
                    currentModelType = currentModelType.BaseType;
                    viewType = defaultLocator(currentModelType, displayLocation, context);
                }
                if (viewType != null)
                {
                    _logger.LogDebug("Found view for {0} in base type {1}, the view is {2}", modelType, currentModelType, viewType);
                }

                return viewType;
            };
        }

        /// <summary>
        ///     Return all instances of a certain service type
        /// </summary>
        /// <param name="service">Type</param>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _serviceProvider.GetServices(service);
        }

        /// <summary>
        ///     Locate an instance of a service, used in Caliburn.
        /// </summary>
        /// <param name="service">Type for the service to locate</param>
        /// <param name="contractName">string with the name of the contract</param>
        /// <returns>instance of the service</returns>
        [SuppressMessage("Sonar Code Smell", "S927:Name parameter to match the base definition", Justification = "The base name is not so clear.")]
        protected override object GetInstance(Type service, string contractName)
        {
            // There is no way to get the service by name
            return _serviceProvider.GetService(service);
        }

        /// <inheritdoc />
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
#if NETCOREAPP
            return AssemblyLoadContext.Default.Assemblies;
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        /// <inheritdoc />
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            foreach (var shell in _serviceProvider.GetServices<ICaliburnMicroShell>())
            {
                var viewModel = shell;
                _windowManager.ShowWindow(viewModel);
            }
        }

        /// <inheritdoc />
        public void Initialize(Application application)
        {
            // Make sure the Application from the IWpfContext is used
            Application = _wpfContext.WpfApplication;
            Initialize();
        }
    }
}