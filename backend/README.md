# Activity Reporting System - Backend API

A production-ready REST API for managing project activities, tracking requests, and team assignments with comprehensive Azure AD authentication and role-based authorization.

## 🚀 Features

### Core Functionality
- **Activity Management** - Track team activities with file attachments and status updates
- **Tracking Requests** - Create and manage project tracking requests with recurring schedules
- **Assignment System** - Assign requests to entities with delegation support
- **Review Process** - Multi-level review and approval workflow
- **Notifications** - Real-time notification system for users
- **User Management** - Complete user profile and role management

### Security & Authentication
- **Microsoft Entra ID (Azure AD)** - Enterprise-grade authentication using OAuth 2.0
- **JWT Token Validation** - Secure token-based authentication
- **Role-Based Authorization** - Five role levels (Admin, PMO, Entity, Directorate, Team)
- **Claims-Based Security** - Fine-grained permissions using Azure AD claims

### Production Features
- **Structured Logging** - Comprehensive logging with Serilog (console + file)
- **Exception Handling** - Global exception middleware with detailed error tracking
- **Request Tracking** - Performance monitoring and user activity logging
- **CORS Support** - Configurable cross-origin resource sharing
- **API Documentation** - Interactive API docs with Scalar

## 🏗️ Architecture

Built following **Clean Architecture** principles:


- **Presentation** (ARS.API) : Controllers, Middleware, DTOs, Validators 
- **Application** (ARS.Application): DTOs, Validators, Services 
- **Infrastructure** (ARS.Infrastructure): Repositories, Data Context, Services
- **Domain** (ARS.Domain): Entities, Enums, Interfaces


## 🛠️ Technologies

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **MongoDB** - NoSQL database
- **Microsoft Identity Web** - Azure AD integration
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Scalar** - API documentation

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [MongoDB](https://www.mongodb.com/try/download/community) (or Docker)
- [Azure AD Tenant](https://azure.microsoft.com/services/active-directory/) (for authentication)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## ⚡ Quick Start

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/ActivityReportingSystem.git
cd ActivityReportingSystem/backend
```

### 2. Start MongoDB

**Using Docker:**
```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

**Or install MongoDB locally** and ensure it's running on `localhost:27017`

### 3. Configure Azure AD

See [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed Azure AD configuration.

Update `appsettings.json`:
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID"
  }
}
```

### 4. Run the API

```bash
dotnet restore
dotnet build
dotnet run --project ARS.API
```

API will be available at: `https://localhost:7041`

### 5. Access API Documentation

Open browser: `https://localhost:7041/scalar/v1`

## 📁 Project Structure

backend \
├── ARS.API/                    # Presentation Layer \
│   ├── Controllers/            # API Controllers \
│   ├── Middleware/             # Custom middleware \
│   │   ├── ExceptionHandlingMiddleware.cs \
│   │   └── RequestLoggingMiddleware.cs \
│   └── Program.cs              # Application entry point \
├── ARS.Application/            # Application Layer \
│   ├── DTOs/                   # Data Transfer Objects \
│   ├── Validators/             # FluentValidation validators \
│   └── Services/               # Service interfaces \
├── ARS.Infrastructure/         # Infrastructure Layer \
│   ├── Data/                   # Database context \
│   ├── Repositories/           # Data access \
│   └── Services/               # Service implementations \
└── ARS.Domain/                 # Domain Layer \
├── Entities/                   # Domain models \
└── Enums/                      # Enumerations

## 🔐 Authentication & Authorization

### Roles

| Role | Permissions |
|------|-------------|
| **Admin** | Full system access, user management |
| **PMO** | Create tracking requests, review submissions |
| **Entity** | Receive and delegate requests |
| **Directorate** | Manage delegated requests |
| **Team** | Submit activities and work on assignments |

### Getting a Token

1. **Register app in Azure AD** (see SETUP_GUIDE.md)
2. **Use OAuth 2.0 implicit flow** in Postman/client
3. **Include token in requests:**

Authorization: Bearer YOUR_JWT_TOKEN

## 📊 API Endpoints

### Activities
- `GET /api/activities` - Get all activities
- `POST /api/activities` - Create activity (Team+)
- `GET /api/activities/{id}` - Get activity by ID
- `PUT /api/activities/{id}` - Update activity
- `DELETE /api/activities/{id}` - Delete activity

### Tracking Requests
- `GET /api/trackingrequests` - Get all requests
- `POST /api/trackingrequests` - Create request (PMO only)
- `GET /api/trackingrequests/{id}` - Get request by ID
- `PUT /api/trackingrequests/{id}` - Update request (PMO only)
- `DELETE /api/trackingrequests/{id}` - Delete request (PMO only)

See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for complete endpoint reference.

## 📝 Logging

Logs are written to:
- **Console** - Colored, formatted output for development
- **Files** - `Logs/app-YYYY-MM-DD.log` (30-day retention)

Log levels:
- **Information** - Normal requests (200-399)
- **Warning** - Client errors (400-499), slow requests (>3s)
- **Error** - Server errors (500+), exceptions

## 🧪 Testing

**Create a test user:**
```bash
POST /api/users/sync-current-user
Authorization: Bearer YOUR_TOKEN
```

**Test exception handling:**
```bash
GET /api/users/test-error
```

## 🚀 Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md) for Azure deployment instructions.

## 📈 Future Enhancements

- [ ] Unit & Integration tests
- [ ] GraphQL support
- [ ] Real-time notifications (SignalR)
- [ ] File storage (Azure Blob)
- [ ] Email notifications
- [ ] Advanced reporting
- [ ] Audit trail

## 👨‍💻 Author

**Efraín Domínguez**
- Email: efrain.dominguez@unison.mx
- Location: Hermosillo, Sonora, Mexico

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Built with Clean Architecture principles
- Follows Microsoft's security best practices
- Implements industry-standard patterns

---

**Last Updated:** April 9, 2026