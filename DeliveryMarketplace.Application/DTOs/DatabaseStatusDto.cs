namespace DeliveryMarketplace.Application.DTOs;

/// <summary>
/// Informações de diagnóstico da configuração do banco SQLite,
/// exibidas na tela "Status do sistema".
/// </summary>
/// <param name="Provedor">Provedor do EF Core em uso.</param>
/// <param name="VersaoSqlite">Versão do motor SQLite carregado.</param>
/// <param name="CaminhoArquivo">Caminho absoluto do arquivo delivery.db.</param>
/// <param name="ArquivoExiste">Indica se o arquivo já foi criado (acontece na Fase 2).</param>
/// <param name="ProvedorFuncional">Indica se foi possível abrir uma conexão SQLite.</param>
/// <param name="Erro">Mensagem de erro, se houver.</param>
public sealed record DatabaseStatusDto(
    string Provedor,
    string VersaoSqlite,
    string CaminhoArquivo,
    bool ArquivoExiste,
    bool ProvedorFuncional,
    string? Erro);
