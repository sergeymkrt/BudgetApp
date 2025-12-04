using BudgetApp.Domain.Models;
using System.Net.Http.Json;

namespace BudgetApp.NotificationsWorker;

public class NotificationsBackgroundService(IHttpClientFactory _httpClientFactory, ILogger<NotificationsBackgroundService> logger) : BackgroundService
{
    private DateTime _lastBudgetCheckUtc = DateTime.MinValue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notifications Background Service is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessUncategorizedTransactions(stoppingToken);
                await CheckBudgetsIfNeeded(stoppingToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error in notifications worker loop");
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessUncategorizedTransactions(CancellationToken ct)
    {
        var txClient = _httpClientFactory.CreateClient("transactions-service");
        var rulesClient = _httpClientFactory.CreateClient("rules-service");

        // 1) Get uncategorized transactions
        List<Transaction> txs;
        try
        {
            var response = await txClient.GetAsync("/internal/transactions/uncategorized?take=20", ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get uncategorized transactions: {Status}", response.StatusCode);
                return;
            }
            
            txs = await response.Content.ReadFromJsonAsync<List<Transaction>>(ct) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error fetching uncategorized transactions");
            return;
        }

        if (txs.Count == 0)
        {
            logger.LogDebug("No uncategorized transactions found");
            return;
        }

        logger.LogInformation("Processing {Count} uncategorized transactions", txs.Count);

        foreach (var tx in txs)
        {
            if (ct.IsCancellationRequested) break;

            var text = tx.Description ?? string.Empty;
            var merchant = tx.Merchant;

            var classifyBody = new
            {
                Description = text,
                Merchant = merchant,
                tx.Amount,
                tx.Type
            };

            var classifyResponse = await rulesClient.PostAsJsonAsync("/internal/rules/classify", classifyBody, ct);

            if (!classifyResponse.IsSuccessStatusCode)
            {
                var error = await classifyResponse.Content.ReadAsStringAsync(ct);
                logger.LogWarning("RulesService error for tx {Id}: {Status} {Error}",
                    tx.Id, classifyResponse.StatusCode, error);
                continue;
            }

            var classification = await classifyResponse.Content.ReadFromJsonAsync<ClassificationResult>(cancellationToken: ct);

            if (classification?.CategoryId is not null)
            {
                logger.LogInformation("Classified tx {Id} as category {CategoryId} (priority {Priority})",
                    tx.Id, classification.CategoryId, classification.Priority);

                var applyBody = new
                {
                    CategoryId = classification.CategoryId.Value,
                    Status = "Processed"
                };

                var applyResponse =
                    await txClient.PostAsJsonAsync($"/internal/transactions/{tx.Id}/apply-category", applyBody, ct);

                if (!applyResponse.IsSuccessStatusCode)
                {
                    var error = await applyResponse.Content.ReadAsStringAsync(ct);
                    logger.LogWarning("Failed to apply category for tx {Id}: {Status} {Error}",
                        tx.Id, applyResponse.StatusCode, error);
                }
            }
            else
            {
                logger.LogInformation("No rule matched for tx {Id} ({Description})", tx.Id, tx.Description);
            }
        }
    }

    private async Task CheckBudgetsIfNeeded(CancellationToken ct)
    {
        // Run once per minute
        if ((DateTime.UtcNow - _lastBudgetCheckUtc) < TimeSpan.FromMinutes(1))
            return;

        _lastBudgetCheckUtc = DateTime.UtcNow;

        var analyticsClient = _httpClientFactory.CreateClient("analytics-service");
        var txClient = _httpClientFactory.CreateClient("transactions-service");

        var now = DateTimeOffset.UtcNow;
        var year = now.Year;
        var month = now.Month;

        var url = $"/internal/analytics/budget-status?year={year}&month={month}";
        List<BudgetStatusResult> budgetStatuses;
        try
        {
            var response = await analyticsClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get budget status: {Status}", response.StatusCode);
                return;
            }
            
            budgetStatuses = await response.Content.ReadFromJsonAsync<List<BudgetStatusResult>>(ct) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error fetching budget status");
            return;
        }

        foreach (var status in budgetStatuses.Where(s => s.IsOver))
        {
            var message = $"Budget exceeded for {status.CategoryName}: " +
                          $"spent {status.Spent} / limit {status.LimitAmount}";

            logger.LogWarning("Budget exceeded: {Message}", message);

            var alertBody = new
            {
                Type = "BudgetExceeded",
                Message = message,
                CategoryId = status.CategoryId
            };

            var response = await txClient.PostAsJsonAsync("/internal/alerts", alertBody, ct);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                logger.LogWarning("Failed to create alert: {Status} {Error}",
                    response.StatusCode, error);
            }
        }
    }
}


// Using shared DTOs from Application layer
internal sealed record ClassificationResult(int? CategoryId, int Priority);

internal sealed record BudgetStatusResult(
    int CategoryId, 
    string CategoryName, 
    decimal LimitAmount, 
    decimal Spent, 
    bool IsOver);

