using BudgetApp.Application;
using BudgetApp.Infrastructure;
using BudgetApp.RulesService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infrastructure (Database)
builder.Services.AddInfrastructure(builder.Configuration);

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
app.MapGet("/", () => "Rules Service is running");
app.MapRuleEndpoints();

app.Run();
