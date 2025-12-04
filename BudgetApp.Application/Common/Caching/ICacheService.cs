namespace BudgetApp.Application.Common.Caching;

/// <summary>
/// Abstraction for caching operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value or creates it using the factory.
    /// </summary>
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    
    /// <summary>
    /// Removes a cached value.
    /// </summary>
    void Remove(string key);
    
    /// <summary>
    /// Removes all cached values matching a prefix.
    /// </summary>
    void RemoveByPrefix(string prefix);
}

