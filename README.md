# DeliveryMarketplace

Aplicação desktop de delivery no modelo marketplace, que conecta clientes, restaurantes, entregadores e administradores.
É um projeto acadêmico: todos os dados são fictícios e os pagamentos são apenas simulados.

## Tecnologias

| Item | Versão |
|---|---|
| .NET / WPF (XAML) | 10, `net10.0-windows` |
| Padrão | MVC (Models, Views, Controllers) + Repository |
| ORM | Entity Framework Core 10.0.14 (Microsoft.EntityFrameworkCore.Sqlite) |
| Banco de dados | SQLite, arquivo `delivery.db` criado automaticamente |

## Como executar

**No Visual Studio:** abra `DeliveryMarketplace.slnx` e pressione **F5**.

**No terminal:**

```powershell
dotnet run --project DeliveryMarketplace
```

Na primeira execução, o `DatabaseRepository` cria o `delivery.db` ao lado do executável e insere os dados de demonstração.

## Contas de demonstração (senha `123456`)

| Perfil | E-mail |
|---|---|
| Administrador | admin@delivery.local |
| Restaurante | pizzaria@delivery.local · burger@delivery.local · sushi@delivery.local |
| Entregador | joao@delivery.local · karina@delivery.local |
| Cliente | fernanda@delivery.local · gustavo@delivery.local · helena@delivery.local |

A tela de login tem botões que preenchem essas contas.

## Estrutura

```
DeliveryMarketplace/
  Models/         Classes que viram tabelas (Usuario, Restaurante, Produto, Pedido...)
  Views/          Telas XAML + code-behind, separadas por perfil (Admin, Restaurantes, Entregadores, Clientes)
  Controllers/    Regras de negócio de cada perfil (as Views só chamam os Controllers)
  Repositories/   Acesso ao banco: Repository<T> genérico, repositórios por entidade e DatabaseRepository (cria o banco)
  Data/           AppDbContext (EF Core) e hash de senha
  Styles/         Cores e estilos visuais
```

Detalhes em [docs/ARQUITETURA.md](docs/ARQUITETURA.md) e modelo de dados em [docs/MODELO-DADOS.md](docs/MODELO-DADOS.md).

## Andamento

- [x] Fase 1: estrutura inicial WPF e configuração do SQLite
- [x] Fase 2: modelo de dados, criação do banco (DatabaseRepository) e dados fictícios
- [x] Fase 3: login, cadastro de cliente e menus por perfil
- [x] Fase 4: administração (painel, usuários, restaurantes, pedidos, banco de dados)
- [x] Fase 5: restaurante (abrir/fechar, cardápio, fluxo de preparo)
- [x] Fase 6: entregador (disponibilidade, aceitar, retirar, entregar, ganhos)
- [x] Fase 7: cliente (vitrine, carrinho, pedido, acompanhamento, cancelamento)
- [x] Fase 8: pagamentos simulados (Pix, cartão, dinheiro, estorno)
- [x] Fase 9: relatórios por período e exportação CSV

As melhorias pendentes estão nas [issues do GitHub](https://github.com/CaiqueBAR/Delivery-poo2/issues).
