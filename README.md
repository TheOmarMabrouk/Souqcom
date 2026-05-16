# Souqcom — Production-Ready E-Commerce Backend

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

A robust, production-ready e-commerce backend built with **ASP.NET Core 8**, following **Clean Architecture** and **Service Layer** patterns. Provides both an **MVC Web Application** for administration and a **RESTful API** for client-side consumption — within a single solution.

---

## 🚀 Key Features

- **Dual Authentication System** — JWT Bearer Auth for APIs + Cookie-based Auth for MVC dashboard
- **Dynamic RBAC** — Role-Based Access Control with runtime permission management via ASP.NET Core Identity
- **Paymob Payment Gateway** — Full 3-step secure payment flow with automated callback handling
- **Atomic Order Management** — Checkout flow with full data consistency and order tracking
- **Admin Dashboard** — Full MVC panel with dynamic role/user management and protected operations
- **Performance at Scale** — Server-side pagination, filtering, and sorting
- **Structured Logging** — Global exception handling with Serilog
- **Input Validation** — FluentValidation + AutoMapper for clean data flow
- **Unit Testing** — xUnit + EF Core InMemory database + FluentAssertions

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 (MVC + Web API) |
| ORM | Entity Framework Core (Database First) |
| Database | SQL Server |
| Auth | ASP.NET Core Identity, JWT, Cookie Auth |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Logging | Serilog |
| Testing | xUnit, Moq, FluentAssertions, EF InMemory |
| API Docs | Swagger / OpenAPI |
| Version Control | Git & GitHub |

---

## 🏗️ Architecture

```
Solution
├── Web/API Layer       → HTTP request handling, controllers, response formatting
├── Service Layer       → Core business logic (separated from controllers)
└── Data Access Layer   → Repository Pattern + Unit of Work + EF Core
```

The **Service Layer Pattern** ensures clear separation of concerns and makes the codebase maintainable and testable.

---

## ⚙️ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (local or remote)
- Visual Studio 2022+ or VS Code

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/TheOmarMabrouk/Souqcom.git
cd Souqcom

# 2. Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SouqcomDB;Trusted_Connection=True;"
}

# 3. Apply database migrations
dotnet ef database update

# 4. Run the project
dotnet run
```

### API Documentation
Once running, open Swagger UI at:
```
https://localhost:{port}/swagger
```

---

## 🔐 Authentication Flow

```
API Requests  →  JWT Bearer Token
MVC Dashboard →  Cookie-based Authentication
Admin Panel   →  Dynamic RBAC with runtime role/permission management
```

---

## 💳 Payment Flow (Paymob)

```
1. Order Created → 2. Payment Intent → 3. Secure Redirect → 4. Callback Handled
```

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 👤 Author

**Omar Mabrouk** — .NET Backend Developer
- GitHub: [@TheOmarMabrouk](https://github.com/TheOmarMabrouk)
- Email: mraljzar90@gmail.com
