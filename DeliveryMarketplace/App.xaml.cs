using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using DeliveryMarketplace.Repositories;
using DeliveryMarketplace.Views;

namespace DeliveryMarketplace;

/// <summary>
/// Ponto de entrada do sistema:
/// 1. configura o idioma (R$ e datas em português);
/// 2. cria o banco de dados pelo DatabaseRepository;
/// 3. abre a janela principal.
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var cultura = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
        Thread.CurrentThread.CurrentCulture = cultura;
        Thread.CurrentThread.CurrentUICulture = cultura;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(cultura.IetfLanguageTag)));

        DispatcherUnhandledException += (_, args) =>
        {
            MessageBox.Show(args.Exception.Message, "Erro inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        try
        {
            new DatabaseRepository().CriarBanco();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível criar o banco de dados.\n\n{ex.Message}", "DeliveryMarketplace",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        MainWindow = new MainWindow();
        MainWindow.Show();
    }
}
