using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public sealed partial class NavTabViewModel : ObservableObject
{
    public string Key { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _isSelected;

    public NavTabViewModel(string key, string label, bool isSelected = false)
    {
        Key = key;
        Label = label;
        IsSelected = isSelected;
    }
}

