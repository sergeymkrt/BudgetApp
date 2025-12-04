using BudgetApp.AnalyticsService.Endpoints;
using BudgetApp.Application;
using BudgetApp.Infrastructure;

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

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map endpoints
app.MapGet("/", () => "Analytics Service is running");
app.MapAnalyticsEndpoints();

app.Run();
