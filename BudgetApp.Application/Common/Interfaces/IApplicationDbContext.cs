using BudgetApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the database context for use in Application layer.
/// This allows the Application layer to not depend on Infrastructure directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Alert> Alerts { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<CategoryRule> CategoryRules { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

