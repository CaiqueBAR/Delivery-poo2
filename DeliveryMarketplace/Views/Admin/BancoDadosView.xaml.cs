using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Views.Admin;

public partial class BancoDadosView : UserControl
{
    private readonly DatabaseRepository _banco = new();

    public BancoDadosView()
    {
        InitializeComponent();
        Carregar();
    }

    private void Carregar()
    {
        TxtCaminho.Text = _banco.Caminho;
        Componentes.MostrarIndicadores(Indicadores,
            _banco.ContarRegistros().ToDictionary(t => t.Key, t => t.Value.ToString()));
    }

    private void Recriar_Click(object sender, RoutedEventArgs e)
    {
        var resposta = MessageBox.Show("Isso apaga TODOS os dados e volta aos dados de demonstração. Continuar?",
            "Recriar banco", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (resposta != MessageBoxResult.Yes)
        {
            return;
        }

        _banco.RecriarBanco();
        MessageBox.Show("Banco recriado. Entre novamente.", "Pronto");
        ((MainWindow)Window.GetWindow(this)!).MostrarLogin();
    }
}
