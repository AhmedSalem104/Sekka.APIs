# Sekka API - Admin Drivers Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/admin/drivers`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` with **Admin** role

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Drivers](#21-get-drivers)
   - [Get Driver by ID](#22-get-driver-by-id)
   - [Activate Driver](#23-activate-driver)
   - [Deactivate Driver](#24-deactivate-driver)
   - [Get Driver Performance](#25-get-driver-performance)
   - [Get Driver Locations](#26-get-driver-locations)
3. [DTOs Reference](#3-dtos-reference)

---

## 1. Overview

The Admin Drivers API allows administrators to manage drivers: view, filter, activate/deactivate, monitor performance, and track real-time locations.

All endpoints require the **Admin** role (`[Authorize(Roles = "Admin")]`).

---

## 2. Endpoints

### 2.1 Get Drivers

```
GET /api/v1/admin/drivers?page=1&pageSize=20&isActive=true&isOnline=true&searchTerm=ahmed&regionId={guid}
```

| Query Param | Type | Default | Description |
|------------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| isActive | bool? | null | Filter by active status |
| isOnline | bool? | null | Filter by online status |
| searchTerm | string? | null | Search by name/phone |
| regionId | Guid? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Ahmed Mohamed",
        "phone": "01012345678",
        "isActive": true,
        "isOnline": true,
        "totalOrders": 450,
        "averageRating": 4.8,
        "createdAt": "2026-01-15T10:00:00Z"
      }
    ],
    "totalCount": 150,
    "page": 1,
    "pageSize": 20,
    "totalPages": 8
  }
}
```

---

### 2.2 Get Driver by ID

```
GET /api/v1/admin/drivers/{id}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Driver ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ahmed Mohamed",
    "phone": "01012345678",
    "email": "ahmed@example.com",
    "profileImageUrl": "https://api.sekka.app/uploads/profiles/abc.jpg",
    "isActive": true,
    "isOnline": true,
    "totalOrders": 450,
    "averageRating": 4.8,
    "createdAt": "2026-01-15T10:00:00Z",
    "cashOnHand": 1500.00,
    "subscriptionPlan": "Pro",
    "lastLocationUpdate": "2026-03-07T15:25:00Z"
  }
}
```

**Response** `404 Not Found`:
```json
{
  "success": false,
  "message": "السائق غير موجود"
}
```

---

### 2.3 Activate Driver

```
PUT /api/v1/admin/drivers/{id}/activate
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Driver ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تفعيل حساب السائق",
  "data": true
}
```

---

### 2.4 Deactivate Driver

```
PUT /api/v1/admin/drivers/{id}/deactivate
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Driver ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تعطيل حساب السائق",
  "data": true
}
```

---

### 2.5 Get Driver Performance

```
GET /api/v1/admin/drivers/{id}/performance?fromDate=2026-01-01&toDate=2026-03-07
```

| Path Param | Type | Description |
|-----------|------|-------------|
| id | Guid | Driver ID |

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| fromDate | DateTime | No | Start date filter |
| toDate | DateTime | No | End date filter |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "totalOrders": 450,
    "deliveredOrders": 420,
    "successRate": 93.33,
    "averageRating": 4.8,
    "totalEarnings": 45000.00,
    "averageDeliveryTime": 28.5
  }
}
```

---

### 2.6 Get Driver Locations

```
GET /api/v1/admin/drivers/locations
```

Returns real-time locations of all drivers (for admin map view).

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverName": "Ahmed Mohamed",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "isOnline": true,
      "lastUpdate": "2026-03-07T15:25:00Z"
    },
    {
      "driverId": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverName": "Omar Hassan",
      "latitude": 30.0131,
      "longitude": 31.2089,
      "isOnline": false,
      "lastUpdate": "2026-03-07T14:00:00Z"
    }
  ]
}
```

---

## 3. DTOs Reference

### AdminDriverDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Driver ID |
| name | string | Full name |
| phone | string | Phone number |
| isActive | bool | Account active status |
| isOnline | bool | Currently online |
| totalOrders | int | Total orders count |
| averageRating | decimal | Average rating |
| createdAt | DateTime | Registration date |

### AdminDriverDetailDto (extends AdminDriverDto)
| Field | Type | Description |
|-------|------|-------------|
| email | string? | Email address |
| profileImageUrl | string? | Profile image URL |
| cashOnHand | decimal | Current cash amount |
| subscriptionPlan | string? | Current subscription plan |
| lastLocationUpdate | DateTime? | Last GPS update |

### AdminDriverFilterDto
| Field | Type | Description |
|-------|------|-------------|
| page | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 20) |
| isActive | bool? | Filter by active status |
| isOnline | bool? | Filter by online status |
| searchTerm | string? | Search name/phone |
| regionId | Guid? | Filter by region |

### DriverPerformanceDto
| Field | Type | Description |
|-------|------|-------------|
| totalOrders | int | Total orders in period |
| deliveredOrders | int | Successfully delivered |
| successRate | decimal | Delivery success rate % |
| averageRating | decimal | Average customer rating |
| totalEarnings | decimal | Total earnings (EGP) |
| averageDeliveryTime | decimal | Average delivery time (min) |

### DriverLocationDto
| Field | Type | Description |
|-------|------|-------------|
| driverId | Guid | Driver ID |
| driverName | string | Driver name |
| latitude | double? | Current latitude |
| longitude | double? | Current longitude |
| isOnline | bool | Online status |
| lastUpdate | DateTime? | Last location update |
