using BudgetApp.ApiService.Endpoints;
using BudgetApp.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Register typed HTTP clients for downstream services
builder.Services.AddServiceClients();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map all gateway endpoints
app.MapGet("/", () => "Gateway API is running");
app.MapGatewayEndpoints();

app.Run();
