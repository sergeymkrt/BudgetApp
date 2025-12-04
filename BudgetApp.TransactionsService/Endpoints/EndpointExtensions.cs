namespace BudgetApp.TransactionsService.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapTransactionEndpoints();
        app.MapCategoryEndpoints();
        app.MapBudgetEndpoints();
        app.MapAlertEndpoints();
        app.MapAccountEndpoints();

        return app;
    }
}

