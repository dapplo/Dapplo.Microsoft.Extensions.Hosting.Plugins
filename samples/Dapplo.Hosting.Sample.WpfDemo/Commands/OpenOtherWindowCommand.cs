using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Dapplo.Hosting.Sample.WpfDemo.ViewModels;

namespace Dapplo.Hosting.Sample.WpfDemo.Commands
{
    /// <summary>
    /// A simple command which opens <see cref="OtherWindow"/> using the <see cref="OtherWindowViewModel"/> view model.
    /// </summary>
    public class OpenOtherWindowCommand : ICommand
    {
        private readonly ILogger<OpenOtherWindowCommand> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public OpenOtherWindowCommand(
          ILogger<OpenOtherWindowCommand> logger,
          IServiceProvider serviceProvider
          )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _logger.LogInformation("Opening {0} with ViewModel {1}", nameof(OtherWindow), nameof(OtherWindowViewModel));
            var window = _serviceProvider.GetService<OtherWindow>();
            var viewModel = _serviceProvider.GetService<OtherWindowViewModel>();
            window.DataContext = viewModel;
            window.Show();
            window.Focus();
        }
    }
}