using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeliveryMarketplace.Controllers;

namespace DeliveryMarketplace.Views;

/// <summary>Ajudantes reutilizados pelas telas.</summary>
public static class Componentes
{
    /// <summary>
    /// Executa uma ação do controller. Erros de regra de negócio viram uma mensagem de aviso.
    /// Retorna true se deu certo.
    /// </summary>
    public static bool Executar(Action acao, string? mensagemSucesso = null)
    {
        try
        {
            acao();
            if (mensagemSucesso is not null)
            {
                MessageBox.Show(mensagemSucesso, "Pronto", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return true;
        }
        catch (RegraException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }

    /// <summary>Preenche um painel com cartões de indicador (título + valor).</summary>
    public static void MostrarIndicadores(Panel painel, Dictionary<string, string> indicadores)
    {
        painel.Children.Clear();
        foreach (var (titulo, valor) in indicadores)
        {
            var conteudo = new StackPanel();
            conteudo.Children.Add(new TextBlock { Text = titulo, Style = (Style)Application.Current.FindResource("CaptionText") });
            conteudo.Children.Add(new TextBlock
            {
                Text = valor,
                FontSize = 22,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)Application.Current.FindResource("TextBrush"),
                Margin = new Thickness(0, 4, 0, 0)
            });

            painel.Children.Add(new Border
            {
                Style = (Style)Application.Current.FindResource("CardBorder"),
                Width = 210,
                Margin = new Thickness(0, 0, 12, 12),
                Child = conteudo
            });
        }
    }

    /// <summary>Converte texto em valor (aceita "12,50" ou "12.50").</summary>
    public static decimal LerValor(string texto, string campo)
    {
        if (!decimal.TryParse(texto.Replace('.', ','), out var valor))
        {
            throw new RegraException($"{campo}: informe um número válido.");
        }

        return valor;
    }

    /// <summary>Item da linha onde o botão foi clicado (o botão guarda o item no Tag).</summary>
    public static T Item<T>(object sender) => (T)((FrameworkElement)sender).Tag;
}
