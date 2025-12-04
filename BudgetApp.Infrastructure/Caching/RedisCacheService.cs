using System.Text.Json;
using BudgetApp.Application.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace BudgetApp.Infrastructure.Caching;

/// <summary>
/// Redis-based distributed cache implementation.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    // JSON serialization options for caching
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedData = await _cache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(cachedData))
        {
            var deserialized = JsonSerializer.Deserialize<T>(cachedData, JsonOptions);
            if (deserialized is not null)
                return deserialized;
        }

        var value = await factory();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        var serialized = JsonSerializer.Serialize(value, JsonOptions);
        await _cache.SetStringAsync(key, serialized, options);

        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public void RemoveByPrefix(string prefix)
    {
        // Note: Redis doesn't support prefix deletion natively in IDistributedCache.
        // For production, you'd use IConnectionMultiplexer directly with SCAN + DEL.
        // For now, we'll remove known keys manually.
        // This is a limitation of IDistributedCache abstraction.
        
        // Known analytics cache keys to clear
        if (prefix == "analytics:")
        {
            var now = DateTime.UtcNow;
            // Clear current and previous month's analytics cache
            for (int i = -1; i <= 1; i++)
            {
                var date = now.AddMonths(i);
                _cache.Remove(CacheKeys.MonthlySummary(date.Year, date.Month));
                _cache.Remove(CacheKeys.BudgetStatus(date.Year, date.Month));
            }
        }
    }
}

