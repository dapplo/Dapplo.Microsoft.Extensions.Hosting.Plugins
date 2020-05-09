using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Dapplo.Hosting.Sample.ReactiveDemo
{
    // Second level derived class off ReactiveUserControl which contains the ViewModel property.
    // In our MainWindow when we register the ListBox with the collection of
    // NugetDetailsViewModels if no ItemTemplate has been declared it will search for
    // a class derived off IViewFor<NugetDetailsViewModel> and show that for the item.
    public partial class NugetDetailsView : ReactiveUserControl<NugetDetailsViewModel>
    {
        public NugetDetailsView()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                // Our 4th parameter we convert from Url into a BitmapImage.
                // This is an easy way of doing value conversion using ReactiveUI binding.
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IconUrl,
                    view => view.IconImage.Source,
                    url => url == null ? null : new BitmapImage(url))
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Title,
                    view => view.TitleRun.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Description,
                    view => view.DescriptionRun.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.OpenPage,
                    view => view.OpenButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
