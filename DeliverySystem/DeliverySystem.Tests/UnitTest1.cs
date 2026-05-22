using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Patterns;

namespace DeliverySystem.Tests;

public class UnitTest1
{
    [Fact]
    public void Pedido_DeveCalcularTotalComStrategies()
    {
        var pedido = new Pedido("Ana", "Rua 1");
        var prato = new Prato(1, "Burger", "Smash", 30m, 20, null, "Americana", "Grande", "Pão, carne");

        pedido.AdicionarItem(prato, 2);

        var strategies = new object[]
        {
            new TaxaEntregaStrategy(7.90m),
            new CupomStrategy(new Cupom("CUPOM10", CupomTipo.Percentual, 10m, true))
        }.Cast<DeliverySystem.Domain.Interfaces.IPedidoPricingStrategy>();

        var total = pedido.CalcularTotalComStrategies(strategies);

        Assert.Equal(61.11m, total);
    }

    [Fact]
    public void Prato_DevePrepararComMensagemAmigavel()
    {
        var prato = new Prato(1, "Lasanha", "Bolonhesa", 42m, 35, null, "Italiana", "Grande", "Molho, queijo");

        var mensagem = prato.Preparar();

        Assert.Contains("Lasanha", mensagem);
        Assert.Contains("35", mensagem);
    }
}
