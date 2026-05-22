IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AvaliacoesProduto] (
    [Id] int NOT NULL IDENTITY,
    [ProdutoId] int NOT NULL,
    [ClienteId] int NOT NULL,
    [Nota] int NOT NULL,
    [Comentario] nvarchar(1000) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    CONSTRAINT [PK_AvaliacoesProduto] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [Nome] nvarchar(200) NOT NULL,
    [Telefone] nvarchar(50) NOT NULL,
    [EnderecoPadrao] nvarchar(300) NOT NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Cupons] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(50) NOT NULL,
    [Tipo] int NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Cupons] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Entregadores] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(200) NOT NULL,
    [Telefone] nvarchar(50) NOT NULL,
    [Veiculo] nvarchar(100) NOT NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Entregadores] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Pedidos] (
    [Id] int NOT NULL IDENTITY,
    [DataPedido] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [ValorTotal] decimal(18,2) NOT NULL,
    [ClienteNome] nvarchar(200) NOT NULL,
    [EnderecoEntrega] nvarchar(300) NOT NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Restaurantes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(200) NOT NULL,
    [Endereco] nvarchar(300) NOT NULL,
    [Telefone] nvarchar(50) NOT NULL,
    [Categoria] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Restaurantes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(200) NOT NULL,
    [PasswordHash] nvarchar(200) NOT NULL,
    [Role] int NOT NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Produtos] (
    [Id] int NOT NULL IDENTITY,
    [RestauranteId] int NOT NULL,
    [Nome] nvarchar(200) NOT NULL,
    [Descricao] nvarchar(1000) NOT NULL,
    [Preco] decimal(18,2) NOT NULL,
    [TempoPreparoMin] int NOT NULL,
    [ImagemPath] nvarchar(500) NULL,
    CONSTRAINT [PK_Produtos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Produtos_Restaurantes_RestauranteId] FOREIGN KEY ([RestauranteId]) REFERENCES [Restaurantes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Bebidas] (
    [Id] int NOT NULL,
    [VolumeMl] int NOT NULL,
    [Gelada] bit NOT NULL,
    [Tipo] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Bebidas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bebidas_Produtos_Id] FOREIGN KEY ([Id]) REFERENCES [Produtos] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ItensPedido] (
    [Id] int NOT NULL IDENTITY,
    [Quantidade] int NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [ProdutoId] int NOT NULL,
    [PedidoId] int NULL,
    CONSTRAINT [PK_ItensPedido] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItensPedido_Pedidos_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItensPedido_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Pratos] (
    [Id] int NOT NULL,
    [TipoCozinha] nvarchar(100) NOT NULL,
    [Tamanho] nvarchar(50) NOT NULL,
    [Ingredientes] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_Pratos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Pratos_Produtos_Id] FOREIGN KEY ([Id]) REFERENCES [Produtos] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'Codigo', N'Tipo', N'Valor') AND [object_id] = OBJECT_ID(N'[Cupons]'))
    SET IDENTITY_INSERT [Cupons] ON;
INSERT INTO [Cupons] ([Id], [Ativo], [Codigo], [Tipo], [Valor])
VALUES (1, CAST(1 AS bit), N'CUPOM10', 1, 10.0),
(2, CAST(1 AS bit), N'DESCONTO20', 1, 20.0),
(3, CAST(1 AS bit), N'FRETEGRATIS', 3, 0.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'Codigo', N'Tipo', N'Valor') AND [object_id] = OBJECT_ID(N'[Cupons]'))
    SET IDENTITY_INSERT [Cupons] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Endereco', N'Nome', N'Telefone') AND [object_id] = OBJECT_ID(N'[Restaurantes]'))
    SET IDENTITY_INSERT [Restaurantes] ON;
INSERT INTO [Restaurantes] ([Id], [Categoria], [Endereco], [Nome], [Telefone])
VALUES (1, N'Hamburguer', N'Rua A, 123', N'Burger House', N'(11) 99999-0000'),
(2, N'Japonesa', N'Av. B, 456', N'Sushi Now', N'(11) 98888-0000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Endereco', N'Nome', N'Telefone') AND [object_id] = OBJECT_ID(N'[Restaurantes]'))
    SET IDENTITY_INSERT [Restaurantes] OFF;
GO

CREATE UNIQUE INDEX [IX_Cupons_Codigo] ON [Cupons] ([Codigo]);
GO

CREATE INDEX [IX_ItensPedido_PedidoId] ON [ItensPedido] ([PedidoId]);
GO

CREATE INDEX [IX_ItensPedido_ProdutoId] ON [ItensPedido] ([ProdutoId]);
GO

CREATE INDEX [IX_Produtos_RestauranteId] ON [Produtos] ([RestauranteId]);
GO

CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [Usuarios] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515131044_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

