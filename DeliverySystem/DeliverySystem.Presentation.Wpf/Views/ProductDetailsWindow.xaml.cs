using System.Windows;
using DeliverySystem.Presentation.Wpf.ViewModels;

namespace DeliverySystem.Presentation.Wpf.Views;

public partial class ProductDetailsWindow : Window
{
    public ProductDetailsWindow(ProductDetailsDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += () => Close();
    }
}

