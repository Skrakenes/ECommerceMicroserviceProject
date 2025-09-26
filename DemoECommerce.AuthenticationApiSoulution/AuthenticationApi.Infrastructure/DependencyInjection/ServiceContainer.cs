using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        //Add database connectivity
        //JWT Add Authentivation Scheme
        SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

        //Create Depenedncy Injection
        services.AddScoped<IUser, UserRepository>();

        return services;
    }

    public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
    {
        //Register middleware such as:
        //Global Exception : Handle external errors.
        //Listen only to api gateway : block all outside calls.
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
