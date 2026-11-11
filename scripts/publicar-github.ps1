# Faz o commit/push do projeto e cria as issues abertas no GitHub.
# Requisitos: git e GitHub CLI (gh) instalados e logados (gh auth login).
# Uso (na pasta do projeto):  powershell -ExecutionPolicy Bypass -File scripts\publicar-github.ps1

$repo = "CaiqueBAR/Delivery-poo2"

# ===== Commit e push =====
if (-not (Test-Path .git)) {
    git init -b main
    git remote add origin "https://github.com/$repo.git"
    git fetch origin
    git reset --soft origin/main 2>$null
}
git add -A
git commit -m "Reestrutura em MVC com Repository e implementa as fases 2 a 9"
git push -u origin main

# ===== Issues (ficam abertas para continuar depois) =====
$issues = @(
    @{ t = "Fase 2: revisar modelo de dados e diagrama UML"; b = "Conferir docs/MODELO-DADOS.md com o professor. Avaliar validação de CPF/CNPJ e endereço estruturado." },
    @{ t = "Fase 3: melhorar segurança do login"; b = "Trocar SHA-256 por PBKDF2/BCrypt com salt por usuário; permitir troca de senha." },
    @{ t = "Fase 4: administração - editar cadastros"; b = "Editar usuários e restaurantes (hoje só cadastra e ativa/desativa). Listar pagamentos separadamente." },
    @{ t = "Fase 5: restaurante - editar dados e tempo de entrega"; b = "Tela para o gerente alterar taxa, tempo estimado e descrição; excluir categoria vazia." },
    @{ t = "Fase 6: entregador - testar fluxo completo"; b = "Validar aceitar > retirar > entregar com vários entregadores ao mesmo tempo." },
    @{ t = "Fase 7: cliente - quantidade no carrinho e vários endereços"; b = "Botões +/- e remover item no carrinho; cadastro de mais de um endereço." },
    @{ t = "Fase 8: pagamentos - validar cartão (Luhn) e tela de pagamentos"; b = "Validar número do cartão pelo algoritmo de Luhn; mostrar histórico de pagamentos/estornos." },
    @{ t = "Fase 9: relatórios - gráficos e filtro por restaurante"; b = "Adicionar gráfico de vendas por dia e filtro por restaurante; testar exportação CSV no Excel." },
    @{ t = "Testes automatizados"; b = "Criar projeto de testes xUnit para os Controllers (fluxo do pedido, pagamento, cancelamento)." }
)
foreach ($i in $issues) {
    gh issue create -R $repo --title $i.t --body $i.b
}
