using System.Windows;
using DeliveryMarketplace.Presentation.WPF.ViewModels;

namespace DeliveryMarketplace.Presentation.WPF.Views;

/// <summary>
/// Janela principal. O code-behind apenas recebe o ViewModel via DI;
/// toda a lógica fica no MainWindowViewModel.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
