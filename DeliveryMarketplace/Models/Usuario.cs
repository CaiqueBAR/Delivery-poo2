namespace DeliveryMarketplace.Models;

/// <summary>
/// Usuário do sistema. O campo Perfil define o que ele pode fazer
/// (cliente, restaurante, entregador ou administrador).
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public string Telefone { get; set; } = "";
    public Perfil Perfil { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.Now;

    // Usados pelo cliente e pelo entregador
    public string? Cpf { get; set; }
    public string? Endereco { get; set; }

    // Usados só pelo entregador
    public string? Veiculo { get; set; }
    public bool Disponivel { get; set; }

    public string PerfilTexto => Perfil.ToString();
    public string SituacaoTexto => Ativo ? "Ativo" : "Inativo";
}
