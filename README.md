🏦 BankCustomerAPI
A Secure, Scalable Banking Platform API
Enterprise-grade banking backend built with ASP.NET Core and Entity Framework Core, demonstrating real-world financial system architecture with production-ready security, transaction safety, and audit compliance.
✨ What Makes This Special
This isn't just CRUD—it's how real banking systems work:

Secure authentication with JWT & refresh tokens
Role-based access control (Customer, Admin, Staff)
Atomic transactions with Unit of Work pattern
Complete audit trail for regulatory compliance
Clean architecture following enterprise best practices

🎯 Core Features

User Management: Authentication, email verification, role-based access
Account Operations: Create accounts, view balances, ownership validation
Transactions: Deposits, withdrawals, transfers with overdraft protection
Security: JWT tokens, claims-based authorization, verified customer enforcement
Audit Logging: Immutable logs tracking who did what, when, and from where

🏗️ Architecture
BankCustomerAPI
├── Core (Domain) - Entities, DTOs, Interfaces
├── Data - DbContext, Repositories, Unit of Work
├── Services - Business Logic, Auth, Audit
└── API - Controllers, Auth Pipeline, Policies
Clean layered architecture ensuring maintainability and testability.
🚀 Quick Start
bashdotnet restore
dotnet build
dotnet ef database update
dotnet run
Requirements: .NET 7+, SQL Server, EF Core
💡 Perfect For

Learning enterprise backend development
University projects or portfolio showcase
Understanding financial system architecture
Interview preparation and demonstrations

🔒 Security Highlights
Multi-layer security enforcement:

JWT token validation
Role verification
Custom policy checks
Ownership validation
Business rule enforcement

📚 Learn From This Project
Study how production systems handle:

Financial transaction consistency
Data integrity with Unit of Work
Secure API design patterns
Audit compliance requirements
Professional error handling
