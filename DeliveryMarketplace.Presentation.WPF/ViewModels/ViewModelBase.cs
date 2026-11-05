using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliveryMarketplace.Presentation.WPF.ViewModels;

/// <summary>
/// Classe base de todos os ViewModels de página.
/// Title e Subtitle são exibidos no cabeçalho da janela principal.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    public abstract string Title { get; }

    public virtual string Subtitle => string.Empty;
}
