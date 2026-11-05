# Arquitetura: DeliveryMarketplace (WPF)

## Camadas e dependências

```
Presentation.WPF ──► Application ──► Domain
        │                 ▲
        └──► Infrastructure ┘ (implementa as interfaces da Application)

Tests ──► Domain, Application, Infrastructure
```

| Projeto | Responsabilidade | Conteúdo atual (Fase 1) |
|---|---|---|
| `DeliveryMarketplace.Domain` | Entidades, enums, value objects e regras essenciais. Não depende de nenhum outro projeto nem de EF Core. | Pastas `Entities/`, `Enums/`, `Interfaces/` e `ValueObjects/` (vazias até a Fase 2) |
| `DeliveryMarketplace.Application` | Serviços, casos de uso, DTOs, validações e contratos (interfaces). | `IDatabaseStatusService`, `DatabaseStatusDto`, `AddApplication()` |
| `DeliveryMarketplace.Infrastructure` | EF Core, SQLite, repositórios, configurações Fluent API, migrations e seed. | `ApplicationDbContext`, `ApplicationDbContextFactory`, `SqliteConnectionStringResolver`, `DatabaseStatusService`, `AddInfrastructure()` |
| `DeliveryMarketplace.Presentation.WPF` | Views XAML, ViewModels, navegação, estilos e conversores. | `App` (Host), `MainWindow`, `HomeView`, `SystemStatusView`, `StatCard`, `NavigationService` |
| `DeliveryMarketplace.Tests` | Testes unitários e de integração. | 8 testes: resolução do caminho do banco e registro de DI com SQLite real |

## Inicialização (`App.xaml.cs`)

1. `Host.CreateApplicationBuilder` monta a injeção de dependência, lê o `appsettings.json` e configura o logging.
2. Cada camada registra seus próprios serviços: `AddApplication()`, `AddInfrastructure(configuration)` e `AddPresentation()`.
3. A `MainWindow` é obtida do container, que injeta o `MainWindowViewModel` nela.
4. O `MainWindowViewModel` navega para a página inicial.
5. Exceções não tratadas na interface são registradas no log e exibidas em uma mensagem amigável (`DispatcherUnhandledException`).

## MVVM e navegação

- Os ViewModels herdam de `ViewModelBase` (`ObservableObject` do CommunityToolkit) e usam `[ObservableProperty]` e `[RelayCommand]`.
- A navegação é *ViewModel-first*: `INavigationService.NavigateToAsync<TViewModel>()` resolve o ViewModel pelo container, e `Resources/ViewTemplates.xaml` mapeia cada ViewModel para sua View por meio de `DataTemplate`.
- ViewModels que precisam carregar dados ao abrir implementam `INavigationAware`.
- O code-behind das Views só contém `InitializeComponent()` ou DependencyProperties de componentes.

**Para adicionar uma nova tela:**

1. Crie o ViewModel em `ViewModels/`.
2. Crie a View em `Views/`.
3. Registre o ViewModel em `DependencyInjection.cs`.
4. Adicione o `DataTemplate` em `ViewTemplates.xaml`.
5. Se a tela tiver item de menu, inclua um `NavigationItem` no `MainWindowViewModel`.

## Visual

- `Themes/Colors.xaml`: paleta central (cores e pincéis), com fontes Segoe UI Variable e Segoe Fluent Icons.
- `Styles/Controls.xaml`: tipografia, cartões, badges e botões (primário, claro e de menu).
- `Resources/Converters.xaml`: conversores reutilizáveis.
- `Views/Components/`: UserControls reutilizáveis. `StatCard` será a base dos dashboards.

## Decisões

1. **O repositório anterior (`DeliverySystem`) e o protótipo em ASP.NET MVC foram substituídos** por uma solução nova, com os nomes e a estrutura do prompt mestre.
2. **`IDbContextFactory` em vez de um DbContext injetado.** Uma aplicação desktop não tem escopo por requisição, e um DbContext de vida longa acumula entidades rastreadas e causa conflitos. Cada operação cria um contexto curto.
3. **Caminho do banco resolvido para absoluto** (`SqliteConnectionStringResolver`): o `delivery.db` fica ao lado do executável (`bin/.../delivery.db`), independentemente da pasta de onde a aplicação foi iniciada. O `appsettings.json` guarda apenas `Data Source=delivery.db`.
4. **Nenhum banco é criado na Fase 1.** O diagnóstico abre uma conexão SQLite em memória para provar que o provedor funciona. O `delivery.db` definitivo será gerado pelas migrations na Fase 2, a partir do diagrama UML aprovado.
5. **`ApplicationDbContextFactory` (tempo de design)** permite rodar `dotnet ef` apontando para o projeto Infrastructure, sem depender do Host do WPF.
6. **Versões fixas:** todos os pacotes `Microsoft.*` estão em 10.0.14, e o `dotnet-ef` 10.0.14 está fixado no manifesto `dotnet-tools.json`.
7. **Valores monetários (planejado para a Fase 2):** `decimal` no domínio, persistido como centavos (`long`) por meio de um `ValueConverter`, porque o SQLite não tem um tipo decimal nativo.

## Extensibilidade

Novos módulos (marketing, avaliações, financeiro, logística, comunicação, relatórios) seguem o mesmo caminho:

1. Entidade em `Domain/Entities`.
2. Configuração Fluent API em `Infrastructure/Configurations`, aplicada automaticamente por `ApplyConfigurationsFromAssembly`.
3. Interface e serviço em `Application`, com implementação técnica em `Infrastructure` quando necessário.
4. ViewModel e View na apresentação.
5. Uma nova migration.

Nenhuma camada inferior conhece a interface gráfica.
