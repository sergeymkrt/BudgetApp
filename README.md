# 💰 BudgetApp

A modern personal budget tracking application built with **.NET Aspire** and a microservices architecture. Track your income and expenses, set category-based budgets, and receive automatic alerts when you exceed spending limits.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![Aspire](https://img.shields.io/badge/Aspire-13.0-6C3483?style=flat)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat&logo=postgresql)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?style=flat&logo=blazor)

---

## ✨ Features

- **Transaction Management** — Record income and expenses with merchant details and descriptions
- **Multiple Accounts** — Support for multiple accounts with different currencies
- **Category System** — Organize transactions into customizable categories with color coding
- **Budget Tracking** — Set monthly spending limits per category
- **Automatic Categorization** — Rule-based engine that auto-categorizes transactions based on patterns
- **Budget Alerts** — Real-time notifications when spending exceeds budget limits
- **Analytics Dashboard** — Monthly summary with income, expenses, and net balance

---

## 🏗️ Architecture

BudgetApp follows a **microservices architecture** orchestrated by .NET Aspire, with **CQRS pattern** (Command Query Responsibility Segregation) implemented via MediatR:

```
┌─────────────────────────────────────────────────────────────────────┐
│                           App Host (Aspire)                         │
└─────────────────────────────────────────────────────────────────────┘
                                    │
         ┌──────────────────────────┼──────────────────────────┐
         │                          │                          │
         ▼                          ▼                          ▼
┌─────────────────┐      ┌─────────────────┐       ┌─────────────────┐
│   Web Frontend  │ ───▶ │   Gateway API   │ ◀───  │  Notifications  │
│    (Blazor)     │      │                 │       │     Worker      │
└─────────────────┘      └─────────────────┘       └─────────────────┘
                                    │                       │
         ┌──────────────────────────┼───────────────────────┤
         │                          │                       │
         ▼                          ▼                       ▼
┌─────────────────┐      ┌─────────────────┐       ┌─────────────────┐
│  Transactions   │      │    Analytics    │       │     Rules       │
│    Service      │      │     Service     │       │    Service      │
└─────────────────┘      └─────────────────┘       └─────────────────┘
         │                          │                       │
         └──────────────────────────┼───────────────────────┘
                                    │
                                    ▼
                         ┌─────────────────┐
                         │   PostgreSQL    │
                         │    (budgetdb)   │
                         └─────────────────┘
```

### Services Overview

| Service | Description |
|---------|-------------|
| **AppHost** | Aspire orchestrator — manages service discovery, configuration, and dependencies |
| **Web** | Blazor Server frontend with interactive dashboard and transaction management |
| **Gateway API** | Public-facing API that routes requests to internal microservices |
| **Transactions Service** | CRUD operations for transactions, accounts, categories, budgets, and alerts |
| **Analytics Service** | Computes monthly summaries and budget status reports |
| **Rules Service** | Manages categorization rules and classifies transactions |
| **Notifications Worker** | Background service that auto-categorizes transactions and generates budget alerts |

---

## 📁 Project Structure

```
BudgetApp/
├── BudgetApp.AppHost/              # Aspire orchestrator
├── BudgetApp.ServiceDefaults/      # Shared service configuration
├── BudgetApp.Domain/               # Domain models (shared entities)
│   └── Models/
│       ├── Account.cs
│       ├── Alert.cs
│       ├── Budget.cs
│       ├── Category.cs
│       ├── CategoryRule.cs
│       ├── Transaction.cs
│       ├── TransactionStatus.cs
│       └── TransactionType.cs
├── BudgetApp.Application/          # CQRS Commands, Queries & Handlers (MediatR)
│   ├── Common/
│   │   ├── Behaviours/             # Pipeline behaviors (validation)
│   │   ├── Exceptions/             # Custom exceptions
│   │   ├── Interfaces/             # IApplicationDbContext
│   │   └── Models/                 # Result wrapper
│   └── Features/
│       ├── Accounts/
│       ├── Alerts/
│       ├── Analytics/
│       ├── Budgets/
│       ├── Categories/
│       ├── Rules/
│       └── Transactions/
├── BudgetApp.Infrastructure/       # EF Core DbContext & migrations
├── BudgetApp.ApiService/           # Gateway API
├── BudgetApp.TransactionsService/
├── BudgetApp.AnalyticsService/
├── BudgetApp.RulesService/
├── BudgetApp.NotificationsWorker/
├── BudgetApp.Web/                  # Blazor frontend
│   ├── Components/Pages/
│   │   ├── Home.razor              # Dashboard
│   │   ├── Transactions.razor      # Transaction management
│   │   ├── Accounts.razor          # Account management
│   │   ├── Categories.razor        # Category management
│   │   └── Budgets.razor           # Budget management
│   └── Models/                     # View models
└── BudgetApp.slnx                  # Solution file
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL container)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)

### Running the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/BudgetApp.git
   cd BudgetApp
   ```

2. **Start the application** (from the solution root)
   ```bash
   dotnet run --project BudgetApp.AppHost
   ```

3. **Open the Aspire Dashboard**  
   Navigate to `https://localhost:17231` (or the URL shown in the console) to view all running services.

4. **Access the Web App**  
   The Blazor frontend will be available at the URL shown for `webfrontend` in the Aspire dashboard.

> **Note:** The AppHost automatically provisions a PostgreSQL container with a persistent volume. Database migrations run automatically on startup.

---

## 📡 API Reference

The Gateway API exposes the following endpoints:

### Transactions
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/transactions` | List recent transactions |
| `GET` | `/transactions/{id}` | Get transaction by ID |
| `POST` | `/transactions` | Create a new transaction |
| `PUT` | `/transactions/{id}` | Update a transaction |
| `DELETE` | `/transactions/{id}` | Delete a transaction |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/categories` | List all categories |
| `GET` | `/categories/{id}` | Get category by ID |
| `POST` | `/categories` | Create a category |
| `PUT` | `/categories/{id}` | Update category |
| `DELETE` | `/categories/{id}` | Delete category |

### Budgets
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/budgets?year=&month=` | List budgets (optional filters) |
| `GET` | `/budgets/{id}` | Get budget by ID |
| `POST` | `/budgets` | Create a budget |
| `PUT` | `/budgets/{id}` | Update budget |
| `DELETE` | `/budgets/{id}` | Delete budget |

### Accounts
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/accounts` | List all accounts |
| `GET` | `/accounts/{id}` | Get account by ID |
| `POST` | `/accounts` | Create an account |
| `PUT` | `/accounts/{id}` | Update account |
| `DELETE` | `/accounts/{id}` | Delete account |

### Rules
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/rules` | List categorization rules |
| `GET` | `/rules/{id}` | Get rule by ID |
| `POST` | `/rules` | Create a rule |
| `PUT` | `/rules/{id}` | Update a rule |
| `DELETE` | `/rules/{id}` | Delete a rule |
| `POST` | `/rules/classify` | Classify a transaction |

### Analytics
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/analytics/summary?year=&month=` | Monthly income/expense summary |
| `GET` | `/analytics/budget-status?year=&month=` | Budget vs actual spending |

### Alerts
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/alerts?take=100` | List recent alerts |

---

## 🔧 How It Works

### Automatic Transaction Categorization

1. When you create a transaction, it starts with `Status = New`
2. The **Notifications Worker** polls for uncategorized transactions every 10 seconds
3. For each transaction, it calls the **Rules Service** to find a matching rule
4. Rules match patterns (e.g., `"netflix"`, `"uber"`) against the transaction description/merchant
5. If a rule matches, the transaction is updated with the corresponding category

### Budget Alerts

1. The **Notifications Worker** checks budget status every minute
2. It calls the **Analytics Service** to compare spending vs. budget limits
3. When spending exceeds a budget, an alert is created automatically
4. Alerts appear on the dashboard with details about the overspent category

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Orchestration** | .NET Aspire 13.0 |
| **Backend** | ASP.NET Core Minimal APIs |
| **Frontend** | Blazor Server |
| **Database** | PostgreSQL with EF Core |
| **Caching** | Redis (via Aspire) with auto-invalidation |
| **Architecture** | CQRS with MediatR |
| **Validation** | FluentValidation |
| **Testing** | xUnit, Shouldly, NSubstitute |
| **Observability** | OpenTelemetry |

---

## 🛠️ Development

### Adding a New Migration

```bash
cd BudgetApp.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../BudgetApp.TransactionsService
```

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

The test suite includes **57 unit tests** covering:

| Test Category | Description |
|---------------|-------------|
| **Validator Tests** | FluentValidation rules for all Create/Update commands |
| **Handler Tests** | CQRS command handlers using EF Core InMemory database |
| **Result Tests** | Result pattern and Match() functionality |

**Test Stack:**
- xUnit (test framework)
- Shouldly (assertions)
- NSubstitute (mocking)
- EF Core InMemory (database testing)

### Environment Configuration

Each service has its own `appsettings.json` and `appsettings.Development.json`. The Aspire AppHost injects connection strings and service URLs automatically via service discovery.

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
