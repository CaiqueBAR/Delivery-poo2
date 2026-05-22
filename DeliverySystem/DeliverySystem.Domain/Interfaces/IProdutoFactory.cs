using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Interfaces;

public interface IProdutoFactory<out TProduto> where TProduto : Produto
{
    TProduto Criar();
}
