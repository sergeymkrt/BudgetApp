namespace BudgetApp.ApiService.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication MapGatewayEndpoints(this WebApplication app)
    {
        app.MapTransactionGatewayEndpoints();
        app.MapCategoryGatewayEndpoints();
        app.MapBudgetGatewayEndpoints();
        app.MapAccountGatewayEndpoints();
        app.MapAlertGatewayEndpoints();
        app.MapAnalyticsGatewayEndpoints();
        app.MapRulesGatewayEndpoints();

        return app;
    }
}

