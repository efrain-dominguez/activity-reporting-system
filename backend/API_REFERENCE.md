# API Reference

Complete endpoint documentation.

---

## Authentication

All endpoints require JWT token:

```http
Authorization: Bearer {your-token}
```

---

## Users

### Get All Users

```http
GET /api/users
```

**Response:**
```json
[
  {
    "id": "69d701a061754fd62a8ece8b",
    "email": "user@domain.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "PMO",
    "isActive": true
  }
]
```

### Sync Current User

```http
POST /api/users/sync-current-user
```

Creates/updates user from Azure AD token.

### Get User by ID

```http
GET /api/users/{id}
```

### Update User (Admin only)

```http
PUT /api/users/{id}
Content-Type: application/json

{
  "email": "updated@domain.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "role": "Entity",
  "isActive": true
}
```

### Delete User (Admin only)

```http
DELETE /api/users/{id}
```

---

## Activities

### Get All Activities

```http
GET /api/activities
```

### Create Activity

```http
POST /api/activities
Content-Type: application/json

{
  "assignmentId": "69d67f980e22e82b31a5a676",
  "requestId": "69d5a8aec913608aff1ddb57",
  "description": "Completed task",
  "activityDate": "2026-04-09"
}
```

### Get Activity by ID

```http
GET /api/activities/{id}
```

### Update Activity

```http
PUT /api/activities/{id}
Content-Type: application/json

{
  "description": "Updated description",
  "activityDate": "2026-04-10"
}
```

### Delete Activity

```http
DELETE /api/activities/{id}
```

### Get My Activities

```http
GET /api/activities/my-activities
```

Returns activities submitted by current user.

---

## Tracking Requests

### Get All Requests

```http
GET /api/trackingrequests
```

### Create Request (PMO, Admin only)

```http
POST /api/trackingrequests
Content-Type: application/json

{
  "title": "Q1 Report",
  "description": "Quarterly reporting",
  "goalType": "Reporting",
  "targetEntityIds": ["507f1f77bcf86cd799439011"],
  "startDate": "2026-04-01",
  "dueDate": "2026-04-30",
  "isRecurring": false
}
```

### Get Request by ID

```http
GET /api/trackingrequests/{id}
```

### Update Request (PMO, Admin only)

```http
PUT /api/trackingrequests/{id}
```

### Delete Request (PMO, Admin only)

```http
DELETE /api/trackingrequests/{id}
```

---

## Request Assignments

### Get All Assignments

```http
GET /api/requestassignments
```

### Create Assignment

```http
POST /api/requestassignments
Content-Type: application/json

{
  "requestId": "69d5a8aec913608aff1ddb57",
  "assignedToUserId": "69d701a061754fd62a8ece8b",
  "dueDate": "2026-04-30"
}
```

### Delegate Assignment

```http
PATCH /api/requestassignments/{id}/delegate
Content-Type: application/json

{
  "delegatedToUserId": "69d67f980e22e82b31a5a676"
}
```

### Get My Assignments

```http
GET /api/requestassignments/my-assignments
```

---

## Reviews

### Get All Reviews

```http
GET /api/reviews
```

### Create Review (PMO, Admin only)

```http
POST /api/reviews
Content-Type: application/json

{
  "assignmentId": "69d67f980e22e82b31a5a676",
  "status": "Accepted",
  "comments": "Good work"
}
```

### Get My Reviews

```http
GET /api/reviews/my-reviews
```

---

## Notifications

### Get Notifications

```http
GET /api/notifications
```

### Get Unread Notifications

```http
GET /api/notifications/unread
```

### Get Unread Count

```http
GET /api/notifications/unread/count
```

**Response:**
```json
{
  "count": 5
}
```

### Mark as Read

```http
PATCH /api/notifications/{id}/read
```

### Mark All as Read

```http
PATCH /api/notifications/read-all
```

---

## Entities

### Get All Entities

```http
GET /api/entities
```

### Create Entity

```http
POST /api/entities
Content-Type: application/json

{
  "name": "IT Department",
  "description": "Information Technology",
  "isActive": true
}
```

### Get Entity by ID

```http
GET /api/entities/{id}
```

### Update Entity

```http
PUT /api/entities/{id}
```

### Delete Entity

```http
DELETE /api/entities/{id}
```

---

## Error Responses

### 400 Bad Request

```json
{
  "error": {
    "message": "Invalid request",
    "statusCode": 400,
    "requestId": "0HNKMA...",
    "path": "/api/activities",
    "timestamp": "2026-04-09T10:00:00Z"
  }
}
```

### 401 Unauthorized

No valid JWT token provided.

### 403 Forbidden

```json
{
  "error": {
    "message": "Access denied",
    "statusCode": 403,
    "requestId": "0HNKMA...",
    "path": "/api/trackingrequests",
    "timestamp": "2026-04-09T10:00:00Z"
  }
}
```

### 404 Not Found

```json
{
  "error": {
    "message": "Resource not found",
    "statusCode": 404,
    "requestId": "0HNKMA...",
    "path": "/api/users/invalid-id",
    "timestamp": "2026-04-09T10:00:00Z"
  }
}
```

### 500 Internal Server Error

```json
{
  "error": {
    "message": "An unexpected error occurred",
    "statusCode": 500,
    "requestId": "0HNKMA...",
    "path": "/api/activities",
    "timestamp": "2026-04-09T10:00:00Z"
  }
}
```