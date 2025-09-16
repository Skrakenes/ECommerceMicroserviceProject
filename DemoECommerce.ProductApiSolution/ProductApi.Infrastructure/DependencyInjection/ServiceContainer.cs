
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;

namespace ProductApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        // Add database connectivity
        // Add authentication scheme
        SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FineName"]!);

        // Create Dependency Injection (DI)
        services.AddScoped<IProduct, ProductRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middleware such as:
        // Global Exception: handles external errors.
        // Listen to ONpy Api Gateway: blocks all outsiders API calls.
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
