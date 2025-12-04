using BudgetApp.Application;
using BudgetApp.Infrastructure;
using BudgetApp.TransactionsService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infrastructure (Database + Redis)
builder.AddInfrastructureWithAspire();

// Application layer (MediatR + FluentValidation)
builder.Services.AddApplicationServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Run migrations
app.Services.EnsureDatabaseMigrated();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map all endpoints
app.MapGet("/", () => "Transactions Service is running");
app.MapAllEndpoints();

app.UseHttpsRedirection();

app.Run();
