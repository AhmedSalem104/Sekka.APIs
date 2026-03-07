# Sekka API - Admin Roles Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/admin/roles`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` with **Admin** role

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Roles](#21-get-roles)
   - [Create Role](#22-create-role)
   - [Update Role](#23-update-role)
   - [Delete Role](#24-delete-role)
   - [Assign Role](#25-assign-role)
   - [Revoke Role](#26-revoke-role)
   - [Get User Roles](#27-get-user-roles)
3. [DTOs Reference](#3-dtos-reference)

---

## 1. Overview

The Admin Roles API provides role-based access control (RBAC) management. Administrators can create, update, and delete roles, and assign/revoke roles to/from users.

Default roles seeded on startup: `Admin`, `Driver`, `Support`

---

## 2. Endpoints

### 2.1 Get Roles

```
GET /api/v1/admin/roles
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Admin",
      "usersCount": 3,
      "createdAt": "2026-01-01T00:00:00Z"
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Driver",
      "usersCount": 150,
      "createdAt": "2026-01-01T00:00:00Z"
    },
    {
      "id": "9fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Support",
      "usersCount": 5,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ]
}
```

---

### 2.2 Create Role

```
POST /api/v1/admin/roles
```

**Request Body**:
```json
{
  "name": "Supervisor"
}
```

**Response** `201 Created`:
```json
{
  "success": true,
  "message": "تم إنشاء الدور بنجاح",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Supervisor",
    "usersCount": 0,
    "createdAt": "2026-03-07T15:30:00Z"
  }
}
```

---

### 2.3 Update Role

```
PUT /api/v1/admin/roles/{id}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Role ID |

**Request Body**:
```json
{
  "name": "Senior Supervisor"
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الدور بنجاح",
  "data": { ... }
}
```

---

### 2.4 Delete Role

```
DELETE /api/v1/admin/roles/{id}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Role ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم حذف الدور بنجاح",
  "data": true
}
```

---

### 2.5 Assign Role

```
POST /api/v1/admin/roles/assign
```

**Request Body**:
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleName": "Supervisor"
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم إسناد الدور بنجاح",
  "data": true
}
```

---

### 2.6 Revoke Role

```
DELETE /api/v1/admin/roles/revoke
```

**Request Body**:
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleName": "Supervisor"
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم إزالة الدور بنجاح",
  "data": true
}
```

---

### 2.7 Get User Roles

```
GET /api/v1/admin/roles/users/{userId}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| userId | Guid | User/Driver ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": ["Driver", "Supervisor"]
}
```

**Response** `404 Not Found`:
```json
{
  "success": false,
  "message": "المستخدم غير موجود"
}
```

---

## 3. DTOs Reference

### RoleDto
| Field | Type | Description |
|-------|------|-------------|
| id | string | Role ID |
| name | string | Role name |
| usersCount | int | Number of users with this role |
| createdAt | DateTime | Creation date |

### CreateRoleDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Role name |

### UpdateRoleDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | New role name |

### AssignRoleDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | Guid | Yes | Target user ID |
| roleName | string | Yes | Role name to assign |

### RevokeRoleDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | Guid | Yes | Target user ID |
| roleName | string | Yes | Role name to revoke |
