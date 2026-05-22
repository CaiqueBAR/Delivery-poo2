# DeliverySystem

Sistema desktop de delivery em `C# / .NET 8` com arquitetura em camadas, `WPF`, `ASP.NET Core`, `Entity Framework Core`, `SQL Server`, `JWT`, `BCrypt`, `xUnit` e padrões de projeto.

## Stack

- `Presentation`: `WPF` com `MVVM`, tema dark, sidebar e dashboard operacional
- `API`: `ASP.NET Core Minimal API` com autenticação `JWT`
- `Application`: serviços de aplicação, DTOs e casos de uso
- `Domain`: entidades, enums, interfaces e regras de negócio
- `Infrastructure`: `EF Core`, repositórios, `DbContext`, migrations e serviços técnicos
- `Tests`: testes unitários e de integração com `SQLite in-memory`

## Arquitetura

- `DeliverySystem.Domain`
  - entidades centrais: `Restaurante`, `Produto`, `Prato`, `Bebida`, `Pedido`, `ItemDePedido`, `Cupom`, `Usuario`, `Cliente`, `Entregador`, `AvaliacaoProduto`
- `DeliverySystem.Application`
  - serviços: `RestauranteAppService`, `ProdutoAppService`, `PedidoAppService`, `ClienteAppService`, `AuthAppService`, `RelatorioAppService`
- `DeliverySystem.Infrastructure`
  - persistência com `DeliveryDbContext`
  - `Repository Pattern`, `UnitOfWork`, `Singleton` da conexão, `JwtTokenService`, `BcryptPasswordHasher`
- `DeliverySystem.Api`
  - endpoints para `auth`, `restaurantes`, `produtos`, `pedidos`, `clientes`, `relatorios`
- `DeliverySystem.Presentation.Wpf`
  - shell moderna com cards, listas, navegação lateral e carregamento de dados

## Design Patterns

- `Factory Method`: `PratoFactory`, `BebidaFactory`
- `Singleton`: `DatabaseConnectionSingleton`
- `Repository Pattern`: abstrações em `Application.Interfaces` e implementações EF em `Infrastructure.Repositories`
- `Strategy`: `TaxaEntregaStrategy`, `CupomStrategy`, `PromocaoHorarioStrategy`
- `Observer`: `Pedido` notifica `ClientePedidoObserver` quando o status muda
- `MVVM`: `MainViewModel` e binding no `WPF`

## Funcionalidades

- cadastro e busca de restaurantes
- cadastro de pratos e bebidas
- criação de pedidos com múltiplos itens
- alteração de quantidade e remoção de itens
- cálculo automático com taxa de entrega, cupom e promoção por horário
- autenticação com `JWT` e hash `BCrypt`
- seed inicial de cupons e restaurantes
- dashboard com visão operacional
- catálogo moderno de produtos com cards e painel de detalhes (preço + fotos ao clicar)
- relatórios diário e mensal

## Banco de Dados

- banco alvo: `SQL Server`
- connection string padrão: `LocalDB`
- migrations: `DeliverySystem.Infrastructure/Database/Migrations`
- script SQL gerado: `database-init.sql`

## Como Executar

0. Inicialização automática (Windows PowerShell):

```powershell
.\start.ps1
```

1. Restaurar e compilar:

```bash
dotnet build DeliverySystem.sln
```

2. Aplicar migrations ou deixar a API aplicar automaticamente no startup:

```bash
cd DeliverySystem
dotnet dotnet-ef database update --project .\DeliverySystem.Infrastructure\DeliverySystem.Infrastructure.csproj --startup-project .\DeliverySystem.Api\DeliverySystem.Api.csproj
```

3. Rodar a API:

```bash
dotnet run --project .\DeliverySystem.Api\DeliverySystem.Api.csproj
```

4. Rodar a interface WPF:

```bash
dotnet run --project .\DeliverySystem.Presentation.Wpf\DeliverySystem.Presentation.Wpf.csproj
```

5. Executar testes:

```bash
dotnet test DeliverySystem.sln
```

## Endpoints Principais

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/restaurantes`
- `POST /api/restaurantes`
- `GET /api/produtos/restaurante/{restauranteId}`
- `POST /api/produtos/pratos`
- `POST /api/produtos/bebidas`
- `POST /api/pedidos`
- `POST /api/pedidos/{pedidoId}/itens`
- `PUT /api/pedidos/{pedidoId}/status`
- `GET /api/relatorios/diario`
- `GET /api/relatorios/mensal`

## Segurança

- autenticação `JWT`
- hash de senha com `BCrypt`
- uso de `EF Core` para evitar SQL Injection
- validações básicas de entrada nos endpoints

## Diferenciais

- UI dark mode inspirada em plataformas de delivery
- promoção inteligente por faixa de horário
- observação de mudança de status do pedido
- arquitetura preparada para evolução futura em mobile/web

## Próximos Passos Sugeridos

- upload real de imagens
- integração com SignalR para rastreamento em tempo real
- exportação de PDF/Excel
- chat cliente/restaurante
- recomendação inteligente baseada em histórico
