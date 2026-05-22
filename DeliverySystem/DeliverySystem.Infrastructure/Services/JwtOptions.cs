namespace DeliverySystem.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "DeliverySystem";
    public string Audience { get; init; } = "DeliverySystem.Clients";
    public string Key { get; init; } = "super-secret-dev-key-with-32-characters";
    public int ExpirationMinutes { get; init; } = 120;
}
