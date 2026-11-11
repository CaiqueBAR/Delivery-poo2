namespace DeliveryMarketplace.Models;

public enum Perfil
{
    Cliente,
    Restaurante,
    Entregador,
    Administrador
}

public enum StatusPedido
{
    AguardandoConfirmacao,
    Confirmado,
    EmPreparo,
    ProntoParaRetirada,
    EmRota,
    Entregue,
    Cancelado
}

public enum MetodoPagamento
{
    Pix,
    CartaoCredito,
    CartaoDebito,
    Dinheiro
}

public enum StatusPagamento
{
    Pendente,
    Aprovado,
    Recusado,
    Estornado
}
