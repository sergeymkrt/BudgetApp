using BudgetApp.Application.Common.Caching;

namespace BudgetApp.Tests.Common;

/// <summary>
/// A no-op cache service for testing that always executes the factory.
/// </summary>
public class NullCacheService : ICacheService
{
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        return await factory();
    }

    public void Remove(string key)
    {
        // No-op
    }

    public void RemoveByPrefix(string prefix)
    {
        // No-op
    }
}

