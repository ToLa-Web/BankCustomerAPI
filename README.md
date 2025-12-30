# 🏦 BankCustomerAPI

A practice banking API built with ASP.NET Core to learn backend development and clean architecture.

## 📖 About

This project simulates a banking system with user authentication, account management, and financial transactions. I built it to practice real-world backend patterns like Repository, Unit of Work, and proper security implementation.

**Goal**: Go beyond simple CRUD apps and understand how production systems handle security, data consistency, and complex business logic.

## ✨ Features

- **Authentication**: JWT tokens with refresh token support
- **Authorization**: Role-based access control (Customer, Admin, Staff)
- **KYC Verification**: Admin and Staff approval/rejection system for customer verification
- **Accounts**: Create accounts, view balances, manage currencies
- **Transactions**: Deposits, withdrawals, and transfers with balance validation
- **Beneficiary Management**: Save and manage frequent transfer recipients
- **Currency Exchange**: Real-time exchange rates and currency conversion
- **Audit Trail**: Logs all critical actions with user, timestamp, and IP tracking
- **Security**: Password hashing, ownership validation, verified customer checks

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 7+
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **Patterns**: Repository, Unit of Work, Dependency Injection

## 🏗️ Architecture

```
BankCustomerAPI/
├── Core/        # Domain entities, DTOs, interfaces
├── Data/        # EF Core context, repositories, Unit of Work
├── Services/    # Business logic and authentication
└── API/         # Controllers and authorization policies
```

**Clean separation of concerns** with layered architecture for maintainability and testability.

## 🚀 Getting Started

**Requirements**: .NET 7+, SQL Server

```bash
# Clone the repository
git clone https://github.com/yourusername/BankCustomerAPI.git
cd BankCustomerAPI

# Install dependencies
dotnet restore

# Update database with migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at `https://localhost:5001` (or the port shown in console).

## 📡 Key Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Login and get JWT token | No |
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/account/create-account` | Create new account | Verified Customer |
| GET | `/api/account/my-accounts` | Get all my accounts | Verified Customer |
| GET | `/api/account/{id}` | Get account details | Verified Customer |
| GET | `/api/account/{id}/balance` | Get account balance | Verified Customer |
| POST | `/api/account/{id}/deposit` | Deposit funds | Verified Customer |
| POST | `/api/account/{id}/withdraw` | Withdraw funds | Verified Customer |
| POST | `/api/account/transfer` | Transfer between accounts | Verified Customer |
| GET | `/api/account/{id}/transactions` | Get transaction history | Verified Customer |
| GET | `/api/account/transfers/{reference}` | Get transfer by reference | Verified Customer |
| POST | `/api/from-transfer/{reference}` | Save beneficiary from transfer | Verified Customer |
| POST | `/api/beneficiary/create` | Create new beneficiary | Verified Customer |
| GET | `/api/lists` | Get my beneficiaries list | Verified Customer |
| GET | `/api/exchange/rates` | Get current exchange rates | No |
| GET | `/api/exchange/convert` | Convert currency amount | No |
| GET | `/api/admin/customers` | Get all customers | Admin only |
| GET | `/api/admin/customers/{id}` | Get customer by ID | Admin only |
| PUT | `/api/admin/customers/{id}/approve` | Approve customer KYC | Admin only |
| PUT | `/api/admin/customers/{id}/reject` | Reject customer KYC | Admin only |
| PUT | `/api/admin/customers/{id}/suspend` | Suspend customer account | Admin only |
| PUT | `/api/admin/customers/{id}/activate` | Activate customer account | Admin only |
| GET | `/api/admin/audit-logs` | View audit logs | Admin only |
| GET | `/api/staff/customers` | Get all customers | Staff/Admin |
| GET | `/api/staff/customers/{id}` | Get customer by ID | Staff/Admin |
| PUT | `/api/staff/customers/{id}/approve` | Approve customer KYC | Staff/Admin |
| PUT | `/api/staff/customers/{id}/reject` | Reject customer KYC | Staff/Admin |
| PUT | `/api/staff/customers/{id}/suspend` | Suspend customer account | Staff/Admin |
| PUT | `/api/staff/customers/{id}/activate` | Activate customer account | Staff/Admin |
| PUT | `/api/staff/customers/{id}/close` | Close customer account | Staff/Admin |

## 🔐 How Security Works

1. **User registers** → Account created with Pending status
2. **Admin reviews KYC** → Approves or rejects customer verification
3. **User logs in** → Server validates credentials
4. **JWT issued** → Contains user ID, role, and verification status
5. **Client stores token** → Sent with each request in Authorization header
6. **Server validates** → Checks token signature and expiration
7. **Policy check** → `VerifiedCustomerOnly` policy ensures KYC is approved
8. **Authorization check** → Verifies role and ownership
9. **Action executed** → If all checks pass

**Important**: Only verified customers can create accounts and perform transactions. This prevents unverified users from accessing sensitive banking operations.

## 💡 Key Implementation Details

### Unit of Work Pattern
Ensures all database operations succeed or fail together. For example, when transferring money:
- Deduct from sender ✓
- Credit to receiver ✓
- Create transaction records ✓
- Log audit entry ✓

If any step fails, everything rolls back—no partial transactions.

### Repository Pattern
Each entity (User, Account, Transaction) has its own repository that handles database operations. This keeps code organized and testable.

### DTOs (Data Transfer Objects)
Controls exactly what data flows in and out of the API, preventing sensitive data exposure.

### KYC (Know Your Customer) Verification
Both Admins and Staff can approve or reject customer verification requests. Only verified customers can perform banking operations like creating accounts or transferring funds. The system tracks:
- Customer verification status (Pending, Approved, Rejected)
- Who approved/rejected (Admin ID or Staff ID)
- When the action occurred
- Account lifecycle management (Active, Suspended, Closed)

The `VerifiedCustomerOnly` policy enforces that customers must complete KYC before accessing banking features. This is implemented through custom authorization policies that check the customer's verification status in their JWT claims.

**Role Separation**: 
- **Administrators** have full system access including audit logs
- **Staff** can manage customers and perform KYC operations but have limited administrative privileges
- **Customers** can only access their own accounts and transactions after KYC approval

### Request Tracking
Every transaction automatically captures:
- IP Address of the request
- User Agent (device information)
- Timestamp

This information is logged for security auditing and fraud detection.

### Beneficiary Management
Customers can save frequently used transfer recipients for convenience:
- **Save from transfer**: After a successful transfer, save the recipient as a beneficiary
- **Manual creation**: Add beneficiaries directly with account details
- **Quick transfers**: View saved beneficiaries for faster future transactions

This feature improves user experience while maintaining security.

### Currency Exchange
The system supports multi-currency operations:
- **Real-time rates**: Fetch current exchange rates from external API
- **Currency conversion**: Convert amounts between different currencies
- **Multi-currency accounts**: Support for accounts in different currencies

Public endpoints allow anyone to check rates without authentication.

## 📚 What I Learned

**Technical Skills**:
- Implementing JWT authentication with access and refresh tokens
- Building clean architecture with separation of concerns
- Using Repository and Unit of Work patterns for data access
- Handling financial transactions with ACID properties
- Creating role-based authorization with custom policies
- Implementing KYC verification workflow for customer onboarding
- Integrating external APIs for currency exchange rates
- Building beneficiary management for better UX
- Designing RESTful APIs with proper status codes
- Writing secure code (password hashing, SQL injection prevention)

**Soft Skills**:
- Breaking down complex problems into manageable pieces
- Reading documentation and applying best practices
- Structuring projects for long-term maintenance
- Thinking about security from the ground up

## 🎯 Why This Project?

I wanted to challenge myself beyond basic tutorials. Most beginner projects don't cover:
- How to handle money safely (no partial transactions!)
- Real authentication beyond "just store passwords"
- Authorization that actually checks ownership
- KYC verification like real banks (approval workflows)
- Audit trails for compliance
- Clean architecture that scales

This project forced me to think like a real backend developer and understand why certain patterns exist.

## 🔄 What's Next?

Potential improvements I'm considering:
- [ ] Add integration tests
- [ ] Add rate limiting for API endpoints
- [ ] Add Docker support
- [ ] Implement two-factor authentication

## 📝 Notes

This is a **learning project**, not production-ready. Some things I'd add for real production:
- Comprehensive error handling
- More robust validation
- Performance monitoring
- Database backups and recovery
- Load balancing considerations

---

*Built as a learning project to improve my backend development skills. Feedback and suggestions welcome!*
