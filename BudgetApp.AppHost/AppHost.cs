var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("budget-postgres")
    .WithDataVolume()
    .AddDatabase("budgetdb");

var transactionsService = builder.AddProject<Projects.BudgetApp_TransactionsService>("transactions-service")
    .WithReference(db)
    .WaitFor(db);

var analyticsService = builder
    .AddProject<Projects.BudgetApp_AnalyticsService>("analytics-service")
    .WithReference(db)
    .WaitFor(db);

var rulesService = builder
    .AddProject<Projects.BudgetApp_RulesService>("rules-service")
    .WithReference(db)
    .WaitFor(db);

var notificationsWorker = builder
    .AddProject<Projects.BudgetApp_NotificationsWorker>("notifications-worker")
    .WithReference(transactionsService)
    .WithReference(analyticsService)
    .WithReference(rulesService);

var gatewayApi = builder.AddProject<Projects.BudgetApp_ApiService>("gateway-api")
    .WithHttpHealthCheck("/health")
    .WithReference(transactionsService)
    .WithReference(analyticsService)
    .WithReference(rulesService)
    .WaitFor(transactionsService)
    .WaitFor(analyticsService)
    .WaitFor(rulesService);

builder.AddProject<Projects.BudgetApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(gatewayApi)
    .WaitFor(gatewayApi);

builder.Build().Run();
