namespace DeliveryMarketplace.Models;

public class Restaurante
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Endereco { get; set; } = "";
    public decimal TaxaEntrega { get; set; }
    public int TempoEntregaMinutos { get; set; } = 40;
    public bool Aberto { get; set; }
    public bool Ativo { get; set; } = true;

    // Gerente (usuário com perfil Restaurante)
    public int GerenteId { get; set; }
    public Usuario? Gerente { get; set; }

    public List<Categoria> Categorias { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];

    public string SituacaoTexto => !Ativo ? "Desativado" : Aberto ? "Aberto" : "Fechado";
}
