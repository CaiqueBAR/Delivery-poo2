namespace DeliveryMarketplace.Presentation.WPF.ViewModels.Items;

/// <summary>Etapa do roteiro de desenvolvimento exibida na página inicial.</summary>
public sealed record FaseItem(int Numero, string Nome, bool Concluida)
{
    public string Rotulo => $"Fase {Numero}: {Nome}";
    public string Situacao => Concluida ? "Concluída" : "Pendente";
}
