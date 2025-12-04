using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetApp.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services including database context.
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

        return services;
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


