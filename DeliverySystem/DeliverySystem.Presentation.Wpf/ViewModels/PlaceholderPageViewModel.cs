using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public sealed class PlaceholderPageViewModel : ObservableObject
{
    public string Titulo { get; }
    public string Texto { get; }

    public PlaceholderPageViewModel(string titulo, string texto)
    {
        Titulo = titulo;
        Texto = texto;
    }
}

