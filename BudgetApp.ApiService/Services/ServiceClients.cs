namespace BudgetApp.ApiService.Services;

/// <summary>
/// Typed HTTP client for the Transactions Service.
/// </summary>
public class TransactionsServiceClient
{
    private readonly HttpClient _httpClient;

    public TransactionsServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient Http => _httpClient;
}

/// <summary>
/// Typed HTTP client for the Analytics Service.
/// </summary>
public class AnalyticsServiceClient
{
    private readonly HttpClient _httpClient;

    public AnalyticsServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient Http => _httpClient;
}

/// <summary>
/// Typed HTTP client for the Rules Service.
/// </summary>
public class RulesServiceClient
{
    private readonly HttpClient _httpClient;

    public RulesServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient Http => _httpClient;
}

/// <summary>
/// Extension methods for registering service clients.
/// </summary>
public static class ServiceClientExtensions
{
    public static IServiceCollection AddServiceClients(this IServiceCollection services)
    {
        services.AddHttpClient<TransactionsServiceClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://transactions-service");
        });

        services.AddHttpClient<AnalyticsServiceClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://analytics-service");
        });

        services.AddHttpClient<RulesServiceClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://rules-service");
        });

        return services;
    }
}

