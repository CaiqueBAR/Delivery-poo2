using System.Text;
using DeliverySystem.Application;
using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Services;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure;
using DeliverySystem.Infrastructure.Database;
using DeliverySystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (RegisterDto dto, AuthAppService service, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["email"] = ["Email é obrigatório."],
            ["password"] = ["Senha é obrigatória."]
        });
    }

    await service.RegisterAsync(dto, ct);
    return Results.Created("/api/auth/login", null);
});

app.MapPost("/api/auth/login", async (LoginDto dto, AuthAppService service, CancellationToken ct) =>
{
    var result = await service.LoginAsync(dto, ct);
    return Results.Ok(result);
});

var restaurantes = app.MapGroup("/api/restaurantes").RequireAuthorization();
restaurantes.MapGet("/", async (RestauranteAppService service, CancellationToken ct) => Results.Ok(await service.ListAsync(ct)));
restaurantes.MapGet("/buscar", async (string termo, RestauranteAppService service, CancellationToken ct) => Results.Ok(await service.SearchByNomeAsync(termo, ct)));
restaurantes.MapPost("/", async (RestauranteCreateDto dto, RestauranteAppService service, CancellationToken ct) =>
{
    var id = await service.CreateAsync(dto, ct);
    return Results.Created($"/api/restaurantes/{id}", new { id });
});
restaurantes.MapPut("/{id:int}", async (int id, RestauranteUpdateDto dto, RestauranteAppService service, CancellationToken ct) =>
{
    await service.UpdateAsync(id, dto, ct);
    return Results.NoContent();
});
restaurantes.MapDelete("/{id:int}", async (int id, RestauranteAppService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

var produtos = app.MapGroup("/api/produtos").RequireAuthorization();
produtos.MapGet("/buscar", async (string termo, ProdutoAppService service, CancellationToken ct) => Results.Ok(await service.SearchByNomeAsync(termo, ct)));
produtos.MapGet("/restaurante/{restauranteId:int}", async (int restauranteId, ProdutoAppService service, CancellationToken ct) => Results.Ok(await service.ListByRestauranteAsync(restauranteId, ct)));
produtos.MapPost("/pratos", async (PratoCreateDto dto, ProdutoAppService service, CancellationToken ct) =>
{
    var id = await service.CreatePratoAsync(dto, ct);
    return Results.Created($"/api/produtos/{id}", new { id });
});
produtos.MapPost("/bebidas", async (BebidaCreateDto dto, ProdutoAppService service, CancellationToken ct) =>
{
    var id = await service.CreateBebidaAsync(dto, ct);
    return Results.Created($"/api/produtos/{id}", new { id });
});
produtos.MapDelete("/{id:int}", async (int id, ProdutoAppService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
});

var pedidos = app.MapGroup("/api/pedidos").RequireAuthorization();
pedidos.MapPost("/", async (PedidoCreateDto dto, PedidoAppService service, CancellationToken ct) =>
{
    var id = await service.CreateAsync(dto, ct);
    return Results.Created($"/api/pedidos/{id}", new { id });
});
pedidos.MapPost("/{pedidoId:int}/itens", async (int pedidoId, PedidoItemAddDto dto, PedidoAppService service, CancellationToken ct) =>
{
    await service.AddItemAsync(pedidoId, dto, ct);
    return Results.Ok();
});
pedidos.MapPut("/{pedidoId:int}/itens/{produtoId:int}", async (int pedidoId, int produtoId, int quantidade, PedidoAppService service, CancellationToken ct) =>
{
    await service.AlterarQuantidadeAsync(pedidoId, produtoId, quantidade, ct);
    return Results.NoContent();
});
pedidos.MapDelete("/{pedidoId:int}/itens/{produtoId:int}", async (int pedidoId, int produtoId, PedidoAppService service, CancellationToken ct) =>
{
    await service.RemoverItemAsync(pedidoId, produtoId, ct);
    return Results.NoContent();
});
pedidos.MapPost("/{pedidoId:int}/recalcular", async (int pedidoId, string? cupomCodigo, PedidoAppService service, CancellationToken ct) =>
{
    var total = await service.RecalcularTotalAsync(pedidoId, cupomCodigo, ct);
    return Results.Ok(new { total });
});
pedidos.MapPut("/{pedidoId:int}/status", async (int pedidoId, PedidoStatus status, PedidoAppService service, CancellationToken ct) =>
{
    await service.AlterarStatusAsync(pedidoId, status, ct);
    return Results.NoContent();
});
pedidos.MapGet("/status/{status}", async (PedidoStatus status, PedidoAppService service, CancellationToken ct) =>
{
    return Results.Ok(await service.ListByStatusAsync(status, ct));
});

var clientes = app.MapGroup("/api/clientes").RequireAuthorization();
clientes.MapGet("/", async (ClienteAppService service, CancellationToken ct) => Results.Ok(await service.ListAsync(ct)));
clientes.MapPost("/", async (ClienteCreateDto dto, ClienteAppService service, CancellationToken ct) =>
{
    var id = await service.CreateAsync(dto, ct);
    return Results.Created($"/api/clientes/{id}", new { id });
});
clientes.MapPut("/{id:int}", async (int id, ClienteUpdateDto dto, ClienteAppService service, CancellationToken ct) =>
{
    await service.UpdateAsync(id, dto, ct);
    return Results.NoContent();
});

var relatorios = app.MapGroup("/api/relatorios").RequireAuthorization();
relatorios.MapGet("/diario", async (DateOnly? dia, RelatorioAppService service, CancellationToken ct) =>
{
    var date = dia ?? DateOnly.FromDateTime(DateTime.UtcNow);
    var valor = await service.ValorVendidoNoDiaAsync(date, ct);
    return Results.Ok(new { dia = date, valor });
});
relatorios.MapGet("/mensal", async (int ano, int mes, RelatorioAppService service, CancellationToken ct) =>
{
    var valor = await service.ValorVendidoNoMesAsync(ano, mes, ct);
    return Results.Ok(new { ano, mes, valor });
});

app.MapGet("/", () => Results.Ok(new
{
    name = "DeliverySystem API",
    status = "online",
    docs = "/swagger"
}));

app.Run();
