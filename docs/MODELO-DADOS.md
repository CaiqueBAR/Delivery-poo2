# Modelo de dados

Classes em `DeliveryMarketplace/Models`. Cada classe vira uma tabela no `delivery.db`.

```mermaid
classDiagram
    direction LR

    class Usuario {
        +int Id
        +string Nome
        +string Email
        +string SenhaHash
        +string Telefone
        +Perfil Perfil
        +bool Ativo
        +DateTime CriadoEm
        +string? Cpf
        +string? Endereco
        +string? Veiculo
        +bool Disponivel
    }
    class Restaurante {
        +int Id
        +string Nome
        +string Descricao
        +string Endereco
        +decimal TaxaEntrega
        +int TempoEntregaMinutos
        +bool Aberto
        +bool Ativo
        +int GerenteId
    }
    class Categoria {
        +int Id
        +string Nome
        +int RestauranteId
    }
    class Produto {
        +int Id
        +string Nome
        +string Descricao
        +decimal Preco
        +bool Disponivel
        +int RestauranteId
        +int CategoriaId
    }
    class Pedido {
        +int Id
        +StatusPedido Status
        +string EnderecoEntrega
        +string? Observacao
        +decimal Subtotal
        +decimal TaxaEntrega
        +decimal Total
        +DateTime CriadoEm
        +DateTime? EntregueEm
        +int ClienteId
        +int RestauranteId
        +int? EntregadorId
        +decimal Comissao()
    }
    class ItemPedido {
        +int Id
        +int PedidoId
        +int ProdutoId
        +string NomeProduto
        +decimal PrecoUnitario
        +int Quantidade
    }
    class Pagamento {
        +int Id
        +int PedidoId
        +MetodoPagamento Metodo
        +StatusPagamento Status
        +decimal Valor
        +string? CodigoTransacao
    }

    Usuario "1" --> "0..1" Restaurante : gerente
    Restaurante "1" --> "0..*" Categoria
    Restaurante "1" --> "0..*" Produto
    Categoria "1" --> "0..*" Produto
    Usuario "1" --> "0..*" Pedido : cliente
    Usuario "0..1" --> "0..*" Pedido : entregador
    Restaurante "1" --> "0..*" Pedido
    Pedido "1" *-- "1..*" ItemPedido
    Pedido "1" *-- "1" Pagamento
```

## Enums

| Enum | Valores |
|---|---|
| `Perfil` | Cliente, Restaurante, Entregador, Administrador |
| `StatusPedido` | AguardandoConfirmacao, Confirmado, EmPreparo, ProntoParaRetirada, EmRota, Entregue, Cancelado |
| `MetodoPagamento` | Pix, CartaoCredito, CartaoDebito, Dinheiro |
| `StatusPagamento` | Pendente, Aprovado, Recusado, Estornado |

## Ciclo de vida do pedido

```mermaid
stateDiagram-v2
    [*] --> AguardandoConfirmacao : cliente finaliza
    AguardandoConfirmacao --> Confirmado : restaurante aceita
    Confirmado --> EmPreparo
    EmPreparo --> ProntoParaRetirada
    ProntoParaRetirada --> EmRota : entregador retira
    EmRota --> Entregue : entregador entrega
    AguardandoConfirmacao --> Cancelado : cliente ou restaurante
    Confirmado --> Cancelado : restaurante
    EmPreparo --> Cancelado : restaurante
```

## Decisões de modelagem

1. **Um único `Usuario` com o campo `Perfil`**, em vez de uma classe por tipo de usuário. Campos específicos (CPF, endereço, veículo) são opcionais.
2. **Endereço em texto simples**, para manter o modelo enxuto.
3. **Cópia no pedido**: `ItemPedido` guarda nome e preço, e `Pedido` guarda endereço e taxa no momento da compra.
4. **Enums gravados como texto** no banco, para ficarem legíveis ao abrir o `delivery.db`.
