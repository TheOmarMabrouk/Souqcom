# Souqcom - Comprehensive E-Commerce System

A robust, production-ready e-commerce backend built with **ASP.NET Core**, following **Clean Architecture** and **Service Layer** patterns. This project serves as a dual-purpose solution, providing both an **MVC Web Application** for administration and a **RESTful API** for client-side consumption.

## 🚀 Key Features

* **Advanced Authentication & Security**: Integrated **ASP.NET Core Identity** with a custom **Dynamic Role-Based Access Control (RBAC)**. Supports both **JWT Bearer Authentication** for APIs and **Cookie-based Authentication** for the MVC dashboard.
* **Dynamic Permission Management**: Admin area with dynamic role management and protected operations.
* **Payment Gateway Integration**: Full integration with **Paymob**, supporting a secure 3-step payment flow and automated callback handling.
* **Order Management System**: Implemented an **atomic checkout flow** to ensure data consistency during transactions.
* **Data Handling & Performance**: Server-side **pagination, filtering, and sorting** for optimized performance with large datasets.
* **Modern Tooling**: **AutoMapper** for mapping, **FluentValidation** for validation, and **Serilog** for structured logging.
* **Testing**: Automated unit testing using **xUnit** and **EF Core InMemory** database.

## 🛠️ Technical Stack

* **Framework**: .NET 8 (ASP.NET Core MVC & Web API).
* **Database**: SQL Server with **EF Core** (Database First Approach).
* **Testing**: xUnit, Moq, FluentAssertions.
* **Security**: Identity, JWT, RBAC.
* **Tools**: Postman, Git, GitHub.

## 🏗️ Architecture

The project is structured using the **Service Layer Pattern** to ensure a clear separation of concerns:
* **Web/API Layer**: Handles HTTP requests and response formatting.
* **Service Layer**: Encapsulates the core business logic.
* **Data Access Layer**: Manages database interactions using **Repository Pattern** and **Unit of Work**.

## 🎓 Education

* [cite_start]**B.Sc. in Computer Science & AI**, Beni Suef National University[cite: 19].
* [cite_start]**Specialization**: Cyber Security (reflecting in the secure design of this system)[cite: 19].
* [cite_start]**Academic Standing**: GPA 3.0+[cite: 34].
