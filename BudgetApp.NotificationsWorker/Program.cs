using BudgetApp.NotificationsWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient("transactions-service", client =>
{
    client.BaseAddress = new Uri("https+http://transactions-service");
});

builder.Services.AddHttpClient("rules-service", client =>
{
    client.BaseAddress = new Uri("https+http://rules-service");
});

builder.Services.AddHttpClient("analytics-service", client =>
{
    client.BaseAddress = new Uri("https+http://analytics-service");
});

builder.Services.AddHostedService<NotificationsBackgroundService>();

var host = builder.Build();
host.Run();
