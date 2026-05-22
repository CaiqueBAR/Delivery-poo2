using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Application.DTOs;

public sealed record RegisterDto(string Email, string Password, UserRole Role);

public sealed record LoginDto(string Email, string Password);

public sealed record LoginResultDto(string Token);
