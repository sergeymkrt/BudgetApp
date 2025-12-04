using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Infrastructure.Caching;
using BudgetApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BudgetApp.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services including database context and Redis caching.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BudgetDb")
                               ?? configuration["BudgetDb:ConnectionString"];

        services.AddDbContext<BudgetDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(sp => 
            sp.GetRequiredService<BudgetDbContext>());

        // Redis caching is configured via Aspire's AddRedisDistributedCache
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>
    /// Adds infrastructure services with Aspire Redis integration.
    /// </summary>
    public static IHostApplicationBuilder AddInfrastructureWithAspire(
        this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("BudgetDb")
                               ?? builder.Configuration["BudgetDb:ConnectionString"];

        builder.Services.AddDbContext<BudgetDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddScoped<IApplicationDbContext>(sp => 
            sp.GetRequiredService<BudgetDbContext>());

        // Add Redis distributed caching via Aspire
        builder.AddRedisDistributedCache("redis");
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        return builder;
    }

    /// <summary>
    /// Ensures the database is created and all migrations are applied.
    /// </summary>
    public static void EnsureDatabaseMigrated(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
        db.Database.Migrate();
    }
}


