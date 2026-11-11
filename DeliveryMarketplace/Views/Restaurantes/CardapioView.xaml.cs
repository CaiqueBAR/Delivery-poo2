using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Restaurantes;

public partial class CardapioView : UserControl
{
    private readonly RestauranteController _controller = new();
    private int _produtoEditandoId;

    public CardapioView()
    {
        InitializeComponent();
        Componentes.Executar(Carregar);
    }

    private void Carregar()
    {
        CmbCategoria.ItemsSource = _controller.ListarCategorias();
        Grade.ItemsSource = _controller.ListarProdutos();
    }

    private void Grade_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Grade.SelectedItem is not Produto produto)
        {
            return;
        }

        _produtoEditandoId = produto.Id;
        TxtFormTitulo.Text = $"Editar: {produto.Nome}";
        TxtNome.Text = produto.Nome;
        TxtDescricao.Text = produto.Descricao;
        TxtPreco.Text = produto.Preco.ToString("N2");
        CmbCategoria.SelectedItem = ((List<Categoria>)CmbCategoria.ItemsSource).FirstOrDefault(c => c.Id == produto.CategoriaId);
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        var ok = Componentes.Executar(() => _controller.SalvarProduto(_produtoEditandoId, TxtNome.Text, TxtDescricao.Text,
            Componentes.LerValor(TxtPreco.Text, "Preço"), CmbCategoria.SelectedItem as Categoria), "Produto salvo.");

        if (ok)
        {
            Limpar_Click(sender, e);
            Carregar();
        }
    }

    private void Limpar_Click(object sender, RoutedEventArgs e)
    {
        _produtoEditandoId = 0;
        TxtFormTitulo.Text = "Novo produto";
        TxtNome.Text = TxtDescricao.Text = TxtPreco.Text = "";
        Grade.SelectedItem = null;
    }

    private void Disponibilidade_Click(object sender, RoutedEventArgs e)
    {
        var produto = Componentes.Item<Produto>(sender);
        if (Componentes.Executar(() => _controller.AlternarDisponibilidade(produto.Id)))
        {
            Carregar();
        }
    }

    private void Categoria_Click(object sender, RoutedEventArgs e)
    {
        if (Componentes.Executar(() => _controller.CriarCategoria(TxtCategoria.Text)))
        {
            TxtCategoria.Text = "";
            Carregar();
        }
    }
}
