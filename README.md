# DeliveryMarketplace

Aplicação desktop de delivery no modelo marketplace, que conecta clientes, restaurantes, entregadores e administradores.
É um projeto acadêmico: todos os dados são fictícios e os pagamentos são apenas simulados.

## Tecnologias

| Item | Versão |
|---|---|
| .NET / WPF (XAML) | 10 (LTS), `net10.0-windows` |
| MVVM | CommunityToolkit.Mvvm 8.4.2 |
| Injeção de dependência e configuração | Microsoft.Extensions.Hosting 10.0.14 |
| ORM | Entity Framework Core 10.0.14 (Microsoft.EntityFrameworkCore.Sqlite) |
| Banco de dados | SQLite, arquivo `delivery.db` |
| Testes | xUnit 2.9.3 |

## Pré-requisitos

- Windows 10 ou 11
- .NET SDK 10 (`dotnet --list-sdks` deve listar `10.0.x`)
- Visual Studio 2026, com a carga de trabalho "Desenvolvimento para desktop com .NET"

## Como executar

**No Visual Studio:** abra `DeliveryMarketplace.slnx`, defina `DeliveryMarketplace.Presentation.WPF` como projeto de inicialização e pressione **F5**.

**No terminal:**

```powershell
dotnet tool restore                                   # instala o dotnet-ef local (versão fixa)
dotnet build DeliveryMarketplace.slnx
dotnet run --project DeliveryMarketplace.Presentation.WPF
```

## Como testar

```powershell
dotnet test DeliveryMarketplace.slnx
```

Verificação manual: abra a aplicação e vá em **Status do sistema**. A conexão SQLite deve aparecer como "Funcional", junto com a versão do SQLite e o caminho onde o `delivery.db` será criado.

## Estrutura e decisões

Veja [docs/ARQUITETURA.md](docs/ARQUITETURA.md).

## Andamento

- [x] Fase 1: estrutura inicial WPF, MVVM, DI e configuração do SQLite
- [ ] Fase 2: diagrama de classes UML, entidades, migrations e dados fictícios
- [ ] Fase 3: autenticação e perfis
- [ ] Fase 4: administração
- [ ] Fase 5: restaurante
- [ ] Fase 6: entregador
- [ ] Fase 7: cliente
- [ ] Fase 8: pagamentos simulados
- [ ] Fase 9: relatórios e finalização
