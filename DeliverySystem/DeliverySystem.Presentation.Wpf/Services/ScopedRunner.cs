using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.Services;

public sealed class ScopedRunner
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedRunner(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<TResult> RunAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        return await action(scope.ServiceProvider);
    }
}

