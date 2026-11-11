# Arquitetura: DeliveryMarketplace (WPF + MVC)

## Visão geral

O sistema é um único projeto WPF organizado no padrão **MVC**, com uma camada **Repository** para o banco.

```
 View (XAML + code-behind)  ──chama──►  Controller  ──usa──►  Repository  ──►  AppDbContext (EF Core)  ──►  delivery.db
        ▲                                   │
        └────────── recebe Models ──────────┘
```

| Pasta | Papel | Regra |
|---|---|---|
| `Models/` | Classes de dados com propriedades públicas. Cada uma vira uma tabela. | Não acessa banco nem tela. |
| `Views/` | Telas (`UserControl`) e a `MainWindow`. O code-behind lê os campos, chama o Controller e mostra o resultado. | Não tem regra de negócio nem acesso ao banco. |
| `Controllers/` | Regras de negócio de cada perfil: validações, mudanças de status, pagamento simulado, relatórios. | Lança `RegraException` com mensagem amigável quando algo não é permitido. |
| `Repositories/` | Acesso ao banco. `Repository<T>` tem o CRUD genérico; os repositórios específicos herdam dele e acrescentam consultas. | Cada método abre e fecha seu próprio `AppDbContext`. |
| `Data/` | `AppDbContext` (tabelas e relacionamentos) e `Seguranca` (hash de senha). | |

## Fluxo de uma ação (exemplo: restaurante aceita um pedido)

1. `RestaurantePainelView` recebe o clique em **Avançar** e pega o `Pedido` da linha (`Tag` do botão).
2. Chama `RestauranteController.AvancarPedido(id)` dentro de `Componentes.Executar(...)`.
3. O controller confere se o pedido é do restaurante logado e muda o status (`AguardandoConfirmacao → Confirmado`).
4. `PedidoRepository.AtualizarStatus` grava no banco.
5. Se uma regra falhar, o `RegraException` vira uma mensagem de aviso; a tela recarrega quando dá certo.

## Criação do banco (`DatabaseRepository`)

- `App.xaml.cs` chama `new DatabaseRepository().CriarBanco()` ao iniciar.
- `CriarBanco()` usa `Database.EnsureCreated()`: cria o arquivo e as tabelas se não existirem (sem migrations).
- Se a tabela de usuários estiver vazia, insere os dados de demonstração.
- `RecriarBanco()` apaga e recria tudo (botão na tela **Banco de dados** do administrador).
- `ContarRegistros()` mostra quantos registros existem em cada tabela.

## Navegação e sessão

- `MainWindow` mostra a `LoginView` sem menu. Após o login, `AposLogin` monta o menu do perfil e abre a primeira tela.
- `Sessao` (estática) guarda o usuário logado; os controllers usam `Sessao.Usuario` para saber quem está agindo.

## Regras de negócio principais

| Regra | Onde |
|---|---|
| Fluxo do pedido: Aguardando → Confirmado → Em preparo → Pronto → Em rota → Entregue (ou Cancelado) | `RestauranteController`, `EntregadorController` |
| Cliente só cancela antes de o restaurante aceitar | `ClienteController.CancelarPedido` |
| Carrinho com itens de um único restaurante | `ClienteController.AdicionarAoCarrinho` |
| Pedido só em restaurante ativo e aberto | `ClienteController.FazerPedido` |
| Entregador: uma entrega por vez e só quando disponível | `EntregadorController.AceitarEntrega` |
| Pagamento: Pix aprovado; cartão de 16 dígitos (final 0000 recusa); dinheiro aprovado na entrega; cancelamento estorna | `PagamentoController` |
| Comissão da plataforma: 12% do subtotal; taxa de entrega vai para o entregador | `Pedido.PercentualComissao` |
| Nome e preço do produto copiados para o item do pedido (histórico não muda) | `ClienteController.FazerPedido` |

## Decisões

1. **Um projeto só, em MVC**, para facilitar o entendimento e a apresentação. A versão anterior em camadas (Domain/Application/Infrastructure + MVVM) foi substituída.
2. **`EnsureCreated` em vez de migrations**: mais simples para um projeto acadêmico. Mudou o modelo? Use **Recriar banco** (ou apague o `delivery.db`).
3. **Senha com hash SHA-256** (`Data/Seguranca.cs`). Suficiente para o projeto; em produção, o ideal seria PBKDF2/BCrypt com salt por usuário.
4. **Valores em `decimal`**. O SQLite grava como texto; somas e ordenações são feitas em memória nos controllers/repositórios.
