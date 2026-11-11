namespace DeliveryMarketplace.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";

    public int RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    public override string ToString() => Nome;
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public decimal Preco { get; set; }
    public bool Disponivel { get; set; } = true;

    public int RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public string CategoriaNome => Categoria?.Nome ?? "";
    public string SituacaoTexto => Disponivel ? "Disponível" : "Indisponível";
}
