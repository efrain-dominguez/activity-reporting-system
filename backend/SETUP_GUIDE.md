# Setup Guide

Step-by-step instructions to get the API running locally.

## Prerequisites

- .NET 9.0 SDK
- MongoDB (local or Docker)
- Azure AD tenant
- Git

---

## 1. Clone Repository

```bash
git clone https://github.com/yourusername/ActivityReportingSystem.git
cd ActivityReportingSystem/backend
```

---

## 2. Start MongoDB

### Option A: Docker (Recommended)

```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

### Option B: Local Install

Install MongoDB and start the service on port 27017.

---

## 3. Azure AD Configuration

### Create App Registration

1. Azure Portal → Azure Active Directory → App registrations
2. Click "New registration"
3. Name: `ActivityReportingSystem-API`
4. Click "Register"

### Add API Scope

1. Go to "Expose an API"
2. Add scope: `access_as_user`
3. Allow admins and users

### Create Roles

Go to "App roles" and create:

- Admin
- PMO
- Entity
- Directorate
- Team

### Assign Role to Yourself

1. Azure Active Directory → Enterprise applications
2. Find your app → Users and groups
3. Add yourself with PMO role

### Copy Configuration

From "Overview" page, copy:
- Tenant ID
- Client ID

---

## 4. Configure appsettings.json

Update `ARS.API/appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "Scopes": "access_as_user"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ActivityReportingDB"
  }
}
```

---

## 5. Build and Run

```bash
dotnet restore
dotnet build
dotnet run --project ARS.API
```

API available at: `https://localhost:7041`

---

## 6. Get Access Token (Postman)

### Configure OAuth 2.0

- Grant Type: `Implicit`
- Auth URL: `https://login.microsoftonline.com/{TENANT_ID}/oauth2/v2.0/authorize`
- Client ID: `{YOUR_CLIENT_ID}`
- Scope: `api://{YOUR_CLIENT_ID}/access_as_user`
- Callback: `https://oauth.pstmn.io/v1/callback`

Click "Get New Access Token" → Sign in → Use token

---

## 7. Test API

```http
POST https://localhost:7041/api/users/sync-current-user
Authorization: Bearer {your-token}
```

Expected: 200 OK with your user data

---

## 8. Verify Database

```bash
mongosh
use ActivityReportingDB
db.users.find().pretty()
```

You should see your synced user.

---

## Troubleshooting

**401 Unauthorized:** Get a fresh token  
**403 Forbidden:** Check your role assignment in Azure AD  
**Connection error:** Verify MongoDB is running  
**Build errors:** Run `dotnet clean` then `dotnet build`