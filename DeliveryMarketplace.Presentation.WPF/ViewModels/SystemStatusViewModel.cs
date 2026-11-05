using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliveryMarketplace.Application.Interfaces;
using DeliveryMarketplace.Presentation.WPF.Navigation;
using DeliveryMarketplace.Presentation.WPF.ViewModels.Items;

namespace DeliveryMarketplace.Presentation.WPF.ViewModels;

/// <summary>
/// Tela de diagnóstico: confirma que DI, EF Core e o provedor SQLite estão configurados.
/// Toda a verificação é feita pelo IDatabaseStatusService (camada Infrastructure).
/// </summary>
public partial class SystemStatusViewModel : ViewModelBase, INavigationAware
{
    private readonly IDatabaseStatusService _databaseStatus;

    [ObservableProperty] private string _conexaoTexto = "Verificando...";
    [ObservableProperty] private bool _conexaoOk;
    [ObservableProperty] private string _versaoSqlite = "—";
    [ObservableProperty] private string _provedor = "—";
    [ObservableProperty] private string _caminhoArquivo = "—";
    [ObservableProperty] private bool _arquivoExiste;
    [ObservableProperty] private string _arquivoTexto = "—";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public SystemStatusViewModel(IDatabaseStatusService databaseStatus)
    {
        _databaseStatus = databaseStatus;
    }

    public override string Title => "Status do sistema";
    public override string Subtitle => "Verificação da configuração do SQLite, do Entity Framework Core e da arquitetura";

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string Runtime { get; } = RuntimeInformation.FrameworkDescription;

    public IReadOnlyList<CamadaItem> Camadas { get; } =
    [
        new("DeliveryMarketplace.Domain", "Entidades, enums e regras essenciais do domínio"),
        new("DeliveryMarketplace.Application", "Serviços, casos de uso, DTOs e validações"),
        new("DeliveryMarketplace.Infrastructure", "EF Core, SQLite, repositórios, migrations e seed"),
        new("DeliveryMarketplace.Presentation.WPF", "Views XAML, ViewModels, navegação e estilos"),
        new("DeliveryMarketplace.Tests", "Testes unitários e de integração")
    ];

    public Task OnNavigatedToAsync() => RefreshCommand.ExecuteAsync(null);

    [RelayCommand]
    private async Task RefreshAsync()
    {
        ErrorMessage = null;

        try
        {
            var status = await _databaseStatus.GetStatusAsync();

            Provedor = status.Provedor;
            VersaoSqlite = string.IsNullOrEmpty(status.VersaoSqlite) ? "—" : status.VersaoSqlite;
            ConexaoOk = status.ProvedorFuncional;
            ConexaoTexto = status.ProvedorFuncional ? "Funcional" : "Falhou";
            CaminhoArquivo = status.CaminhoArquivo;
            ArquivoExiste = status.ArquivoExiste;
            ArquivoTexto = status.ArquivoExiste
                ? "Arquivo encontrado"
                : "Ainda não criado: será gerado na Fase 2, a partir do diagrama UML aprovado";
            ErrorMessage = status.Erro;
        }
        catch (Exception ex)
        {
            ConexaoOk = false;
            ConexaoTexto = "Falhou";
            ErrorMessage = $"Não foi possível verificar o banco: {ex.Message}";
        }
    }
}
