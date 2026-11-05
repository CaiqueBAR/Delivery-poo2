using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliveryMarketplace.Presentation.WPF.Navigation;

/// <summary>
/// Item do menu lateral: texto, ícone (glifo Segoe Fluent Icons) e ViewModel de destino.
/// </summary>
public partial class NavigationItem : ObservableObject
{
    public NavigationItem(string title, string icon, Type viewModelType)
    {
        Title = title;
        Icon = icon;
        ViewModelType = viewModelType;
    }

    public string Title { get; }
    public string Icon { get; }
    public Type ViewModelType { get; }

    [ObservableProperty]
    private bool _isSelected;
}
