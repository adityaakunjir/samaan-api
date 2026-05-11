# samaan-api — Samaan Backend (ASP.NET Core Web API)

Backend API for **Samaan**, a multi-role grocery delivery platform (Customer + Merchant). Built with **.NET 8**, **ASP.NET Core Web API**, **JWT authentication**, **role-based authorization**, **EF Core**, and **SQL Server / Azure SQL**.

## Live
- **Swagger:** https://samaan-api.onrender.com/swagger

## Related Repositories
- **Customer App:** https://github.com/adityaakunjir/samaan-customer
- **Merchant App:** https://github.com/adityaakunjir/samaan-merchant
- **Portfolio Repo:** https://github.com/adityaakunjir/Samaan

## Tech Stack
- C#, .NET 8, ASP.NET Core Web API (Controllers)
- JWT Bearer Authentication, Role-based Authorization (Customer/Merchant)
- Entity Framework Core, LINQ
- SQL Server / Azure SQL
- Swagger / OpenAPI

## Key Features
- Auth (Register/Login), JWT token generation
- Role-protected endpoints for Customer and Merchant
- Orders: create order (Customer), get order(s), update status (Merchant)
- Products: CRUD and search/filter
- Relational modeling using EF Core

## Local Setup
### Prerequisites
- .NET SDK 8
- SQL Server (local) OR Azure SQL

### Run
```bash
dotnet restore
dotnet run
```

Then open Swagger (local URL depends on your launch profile).

## Deployment
- **Azure App Service:** API hosting
- **Azure SQL:** database
- **Swagger** enabled for testing/verification
