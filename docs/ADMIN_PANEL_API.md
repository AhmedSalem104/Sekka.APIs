# Sekka API - Admin Panel Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/admin`
>
> **Last Updated**: 2026-03-30

---

## Table of Contents

1. [Overview](#1-overview)
2. [Authentication & Authorization](#2-authentication--authorization)
3. [Response Format](#3-response-format)
4. [Admin Drivers](#4-admin-drivers)
5. [Admin Roles](#5-admin-roles)
6. [Admin Subscriptions](#6-admin-subscriptions)
7. [Admin Orders](#7-admin-orders)
8. [Admin Time Slots](#8-admin-time-slots)
9. [Admin Customers](#9-admin-customers)
10. [Admin Partners](#10-admin-partners)
11. [Admin Blacklist](#11-admin-blacklist)
12. [Admin Settlements](#12-admin-settlements)
13. [Admin Payments](#13-admin-payments)
14. [Admin Wallets](#14-admin-wallets)
15. [Admin Disputes](#15-admin-disputes)
16. [Admin Invoices](#16-admin-invoices)
17. [Admin Refunds](#17-admin-refunds)
18. [Admin Statistics](#18-admin-statistics)
19. [Admin Notifications](#19-admin-notifications)
20. [Admin SOS](#20-admin-sos)
21. [Admin Vehicles](#21-admin-vehicles)
22. [Admin Config](#22-admin-config)
23. [Admin Regions](#23-admin-regions)
24. [Admin Audit Logs](#24-admin-audit-logs)
25. [Admin Segments](#25-admin-segments)
26. [Admin Campaigns](#26-admin-campaigns)
27. [Admin Insights](#27-admin-insights)
28. [Admin Savings Circles](#28-admin-savings-circles)
29. [Enums Reference](#29-enums-reference)

---

## 1. Overview

The Sekka Admin Panel API provides **210 endpoints** across **25 controllers** for complete platform management. Admins can manage drivers, orders, customers, partners, finances, configurations, analytics, and more.

| Feature | Detail |
|---------|--------|
| Total Endpoints | 210 |
| Controllers | 25 |
| Auth Required | All endpoints |
| Required Role | Admin |
| Base URL | `/api/v1/admin` |
| Token Type | JWT (Bearer) |
| Pagination | `pageNumber` + `pageSize` (default: 1, 10) |

### Admin Capabilities Summary

| Module | Controllers | Endpoints | Description |
|--------|------------|-----------|-------------|
| Operations | Drivers, Orders, Time Slots | 22 | Driver management, order processing, scheduling |
| Users | Customers, Partners, Blacklist | 17 | User lifecycle, verification, blocking |
| Finance | Subscriptions, Settlements, Payments, Wallets, Invoices, Refunds, Disputes | 52 | Complete financial management |
| Analytics | Statistics, Insights, Audit Logs | 33 | Dashboards, KPIs, audit trail |
| Platform | Config, Regions, Vehicles, Roles, Notifications, SOS | 52 | System configuration, fleet, access control |
| Marketing | Segments, Campaigns, Savings Circles | 31 | Targeting, promotions, gamification |

---

## 2. Authentication & Authorization

**ALL admin endpoints require:**

```
Authorization: Bearer <admin_jwt_token>
```

The JWT must contain the **Admin** role claim. Requests without a valid admin token receive `401 Unauthorized` or `403 Forbidden`.

### Admin Token Claims (JWT Payload)
```json
{
  "nameid": "admin-guid-here",
  "unique_name": "Admin Name",
  "role": "Admin",
  "jti": "unique-token-id",
  "exp": 1741456200,
  "iss": "Sekka.API",
  "aud": "Sekka.Mobile"
}
```

### Common Headers for All Requests
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json
Accept-Language: ar (optional, defaults to ar)
```

---

## 3. Response Format

All admin API responses follow the standard Sekka response format:

### Success Response
```json
{
  "isSuccess": true,
  "data": { ... },
  "message": "رسالة النجاح",
  "errors": null
}
```

### Paginated Response
```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 150,
    "totalPages": 15,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

### Error Response
```json
{
  "isSuccess": false,
  "data": null,
  "message": "رسالة الخطأ",
  "errors": null
}
```

### HTTP Status Codes
| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful delete) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (not Admin role) |
| 404 | Not Found |
| 409 | Conflict (duplicate) |
| 429 | Too Many Requests |
| 500 | Server Error |

---

## 4. Admin Drivers

> **Base**: `/api/v1/admin/drivers`

Manage driver accounts, activation status, performance metrics, and real-time locations.

---

### 4.1 List All Drivers

```
GET /admin/drivers
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| search | string | null | Search by name or phone |
| status | int? | null | Filter by status (see DriverStatus enum) |
| vehicleType | int? | null | Filter by vehicle type |
| isOnline | bool? | null | Filter by online status |
| sortBy | string | "joinedAt" | Sort field |
| sortDesc | bool | true | Sort descending |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "أحمد محمد",
        "phone": "+201012345678",
        "email": "ahmed@example.com",
        "profileImageUrl": "https://storage.sekka.com/profiles/abc.jpg",
        "vehicleType": 0,
        "vehicleTypeName": "Motorcycle",
        "status": 1,
        "statusName": "Active",
        "isOnline": true,
        "cashOnHand": 1500.00,
        "totalOrders": 342,
        "rating": 4.8,
        "level": 5,
        "totalPoints": 12500,
        "joinedAt": "2026-01-15T10:00:00Z",
        "lastActiveAt": "2026-03-30T14:22:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1250,
    "totalPages": 125,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 4.2 Get Driver Details

```
GET /admin/drivers/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Driver ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "أحمد محمد",
    "phone": "+201012345678",
    "email": "ahmed@example.com",
    "profileImageUrl": "https://storage.sekka.com/profiles/abc.jpg",
    "licenseImageUrl": "https://storage.sekka.com/licenses/xyz.jpg",
    "vehicleType": 0,
    "vehicleTypeName": "Motorcycle",
    "status": 1,
    "statusName": "Active",
    "isOnline": true,
    "cashOnHand": 1500.00,
    "totalOrders": 342,
    "completedOrders": 330,
    "cancelledOrders": 12,
    "rating": 4.8,
    "level": 5,
    "totalPoints": 12500,
    "referralCode": "3FA85F64",
    "walletBalance": 3200.50,
    "currentSubscription": {
      "planName": "Premium",
      "expiresAt": "2026-04-15T00:00:00Z",
      "isActive": true
    },
    "joinedAt": "2026-01-15T10:00:00Z",
    "lastActiveAt": "2026-03-30T14:22:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `السائق غير موجود` |

---

### 4.3 Activate Driver

```
PUT /admin/drivers/{id}/activate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Driver ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل السائق بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `السائق غير موجود` |
| 400 | `السائق مفعّل بالفعل` |

---

### 4.4 Deactivate Driver

```
PUT /admin/drivers/{id}/deactivate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Driver ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تفعيل السائق بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `السائق غير موجود` |
| 400 | `السائق غير مفعّل بالفعل` |

---

### 4.5 Get Driver Performance

```
GET /admin/drivers/{id}/performance
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Driver ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date filter |
| to | DateTime? | null | End date filter |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverName": "أحمد محمد",
    "totalOrders": 342,
    "completedOrders": 330,
    "cancelledOrders": 12,
    "completionRate": 96.5,
    "averageDeliveryTime": 28.5,
    "averageRating": 4.8,
    "totalEarnings": 45000.00,
    "totalTips": 2300.00,
    "onTimeDeliveryRate": 94.2,
    "peakHoursActivity": 78.0,
    "customerComplaints": 3,
    "dailyBreakdown": [
      {
        "date": "2026-03-29",
        "orders": 12,
        "earnings": 1800.00,
        "hoursOnline": 8.5
      }
    ]
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `السائق غير موجود` |

---

### 4.6 Get Driver Locations

```
GET /admin/drivers/locations
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| regionId | int? | null | Filter by region |
| isOnline | bool? | true | Filter by online status |
| vehicleType | int? | null | Filter by vehicle type |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverName": "أحمد محمد",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "isOnline": true,
      "vehicleType": 0,
      "currentOrderId": "order-guid-here",
      "lastUpdatedAt": "2026-03-30T14:22:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 5. Admin Roles

> **Base**: `/api/v1/admin/roles`

Manage roles and role assignments for access control.

---

### 5.1 List All Roles

```
GET /admin/roles
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "role-guid-here",
      "name": "Admin",
      "usersCount": 5,
      "createdAt": "2026-01-01T00:00:00Z"
    },
    {
      "id": "role-guid-2",
      "name": "SuperAdmin",
      "usersCount": 2,
      "createdAt": "2026-01-01T00:00:00Z"
    },
    {
      "id": "role-guid-3",
      "name": "Support",
      "usersCount": 10,
      "createdAt": "2026-02-15T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.2 Create Role

```
POST /admin/roles
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "FinanceManager"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| name | string | Yes | Max 50 characters, unique |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-role-guid",
    "name": "FinanceManager",
    "usersCount": 0,
    "createdAt": "2026-03-30T15:00:00Z"
  },
  "message": "تم إنشاء الدور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 409 | `الدور موجود بالفعل` |

---

### 5.3 Update Role

```
PUT /admin/roles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Role ID

**Request Body**:
```json
{
  "name": "SeniorFinanceManager"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "role-guid",
    "name": "SeniorFinanceManager",
    "usersCount": 3,
    "createdAt": "2026-02-15T00:00:00Z"
  },
  "message": "تم تعديل الدور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الدور غير موجود` |
| 409 | `الدور موجود بالفعل` |

---

### 5.4 Delete Role

```
DELETE /admin/roles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Role ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الدور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الدور غير موجود` |
| 400 | `لا يمكن حذف دور مُعيّن لمستخدمين` |

---

### 5.5 Assign Role to User

```
POST /admin/roles/assign
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "userId": "user-guid-here",
  "roleName": "Support"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | GUID | Yes | Target user ID |
| roleName | string | Yes | Role name to assign |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعيين الدور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المستخدم غير موجود` |
| 404 | `الدور غير موجود` |
| 409 | `المستخدم لديه هذا الدور بالفعل` |

---

### 5.6 Revoke Role from User

```
DELETE /admin/roles/revoke
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "userId": "user-guid-here",
  "roleName": "Support"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء الدور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المستخدم غير موجود` |
| 400 | `المستخدم ليس لديه هذا الدور` |

---

### 5.7 Get User Roles

```
GET /admin/roles/users/{userId}
```

**Auth Required**: Yes (Admin)

**URL Params**: `userId` (GUID) - User ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "userId": "user-guid-here",
    "userName": "أحمد محمد",
    "roles": ["Admin", "Support"]
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المستخدم غير موجود` |

---

## 6. Admin Subscriptions

> **Base**: `/api/v1/admin/subscriptions`

Manage driver subscriptions, plans, and subscription analytics.

---

### 6.1 List Subscriptions

```
GET /admin/subscriptions
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by status (Active, Expired, Cancelled) |
| planId | int? | null | Filter by plan |
| search | string | null | Search by driver name/phone |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "sub-guid",
        "driverId": "driver-guid",
        "driverName": "أحمد محمد",
        "driverPhone": "+201012345678",
        "planId": 1,
        "planName": "Premium",
        "status": 1,
        "statusName": "Active",
        "startDate": "2026-03-01T00:00:00Z",
        "endDate": "2026-04-01T00:00:00Z",
        "amountPaid": 299.00,
        "autoRenew": true,
        "createdAt": "2026-03-01T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 850,
    "totalPages": 85,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 6.2 Get Subscription Details

```
GET /admin/subscriptions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Subscription ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "sub-guid",
    "driverId": "driver-guid",
    "driverName": "أحمد محمد",
    "driverPhone": "+201012345678",
    "planId": 1,
    "planName": "Premium",
    "planPrice": 299.00,
    "status": 1,
    "statusName": "Active",
    "startDate": "2026-03-01T00:00:00Z",
    "endDate": "2026-04-01T00:00:00Z",
    "amountPaid": 299.00,
    "paymentMethod": "Wallet",
    "autoRenew": true,
    "renewalHistory": [
      {
        "date": "2026-03-01T00:00:00Z",
        "amount": 299.00,
        "method": "Wallet"
      }
    ],
    "createdAt": "2026-03-01T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الاشتراك غير موجود` |

---

### 6.3 Extend Subscription

```
PUT /admin/subscriptions/{id}/extend
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Subscription ID

**Request Body**:
```json
{
  "days": 30,
  "reason": "تعويض عن توقف الخدمة"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| days | int | Yes | Number of days to extend |
| reason | string | Yes | Reason for extension |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "sub-guid",
    "newEndDate": "2026-05-01T00:00:00Z",
    "daysAdded": 30
  },
  "message": "تم تمديد الاشتراك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الاشتراك غير موجود` |

---

### 6.4 Cancel Subscription

```
PUT /admin/subscriptions/{id}/cancel
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Subscription ID

**Request Body**:
```json
{
  "reason": "طلب السائق إلغاء الاشتراك"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| reason | string | Yes | Cancellation reason |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء الاشتراك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الاشتراك غير موجود` |
| 400 | `الاشتراك ملغي بالفعل` |

---

### 6.5 Change Subscription Plan

```
PUT /admin/subscriptions/{id}/change-plan
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Subscription ID

**Request Body**:
```json
{
  "newPlanId": 2,
  "reason": "ترقية مجانية للسائق المميز"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| newPlanId | int | Yes | New plan ID |
| reason | string | Yes | Reason for change |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "sub-guid",
    "oldPlanName": "Basic",
    "newPlanName": "Premium",
    "effectiveDate": "2026-03-30T00:00:00Z"
  },
  "message": "تم تغيير خطة الاشتراك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الاشتراك غير موجود` |
| 404 | `الخطة غير موجودة` |

---

### 6.6 Gift Subscription

```
POST /admin/subscriptions/gift
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "driverId": "driver-guid",
  "planId": 1,
  "durationDays": 30,
  "reason": "هدية ترحيبية للسائق الجديد"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| driverId | GUID | Yes | Target driver |
| planId | int | Yes | Plan to gift |
| durationDays | int | Yes | Duration in days |
| reason | string | Yes | Reason for gift |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "subscriptionId": "new-sub-guid",
    "driverName": "أحمد محمد",
    "planName": "Premium",
    "startDate": "2026-03-30T00:00:00Z",
    "endDate": "2026-04-29T00:00:00Z"
  },
  "message": "تم إهداء الاشتراك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `السائق غير موجود` |
| 404 | `الخطة غير موجودة` |
| 409 | `السائق لديه اشتراك نشط بالفعل` |

---

### 6.7 List Subscription Plans

```
GET /admin/subscriptions/plans
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Basic",
      "nameAr": "أساسي",
      "price": 99.00,
      "durationDays": 30,
      "maxOrdersPerDay": 10,
      "features": ["basic_support", "standard_visibility"],
      "isActive": true,
      "subscribersCount": 500,
      "createdAt": "2026-01-01T00:00:00Z"
    },
    {
      "id": 2,
      "name": "Premium",
      "nameAr": "مميز",
      "price": 299.00,
      "durationDays": 30,
      "maxOrdersPerDay": -1,
      "features": ["priority_support", "top_visibility", "analytics"],
      "isActive": true,
      "subscribersCount": 350,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 6.8 Create Subscription Plan

```
POST /admin/subscriptions/plans
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "Enterprise",
  "nameAr": "مؤسسات",
  "price": 599.00,
  "durationDays": 30,
  "maxOrdersPerDay": -1,
  "features": ["priority_support", "top_visibility", "analytics", "api_access"]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Plan name (English) |
| nameAr | string | Yes | Plan name (Arabic) |
| price | decimal | Yes | Monthly price in EGP |
| durationDays | int | Yes | Plan duration |
| maxOrdersPerDay | int | Yes | -1 for unlimited |
| features | string[] | Yes | Feature list |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Enterprise",
    "nameAr": "مؤسسات",
    "price": 599.00,
    "durationDays": 30,
    "isActive": true
  },
  "message": "تم إنشاء الخطة بنجاح",
  "errors": null
}
```

---

### 6.9 Update Subscription Plan

```
PUT /admin/subscriptions/plans/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Plan ID

**Request Body**:
```json
{
  "name": "Enterprise Plus",
  "nameAr": "مؤسسات بلس",
  "price": 699.00,
  "durationDays": 30,
  "maxOrdersPerDay": -1,
  "features": ["priority_support", "top_visibility", "analytics", "api_access", "dedicated_manager"]
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Enterprise Plus",
    "nameAr": "مؤسسات بلس",
    "price": 699.00
  },
  "message": "تم تعديل الخطة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الخطة غير موجودة` |

---

### 6.10 Toggle Subscription Plan

```
PUT /admin/subscriptions/plans/{id}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Plan ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "isActive": false
  },
  "message": "تم تعطيل الخطة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الخطة غير موجودة` |

---

### 6.11 Subscription Statistics

```
GET /admin/subscriptions/stats
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalActive": 850,
    "totalExpired": 320,
    "totalCancelled": 45,
    "monthlyRevenue": 125000.00,
    "averageSubscriptionDuration": 65.3,
    "renewalRate": 78.5,
    "planBreakdown": [
      { "planName": "Basic", "count": 500, "revenue": 49500.00 },
      { "planName": "Premium", "count": 350, "revenue": 104650.00 }
    ],
    "monthlyTrend": [
      { "month": "2026-01", "newSubscriptions": 120, "revenue": 98000.00 },
      { "month": "2026-02", "newSubscriptions": 145, "revenue": 112000.00 },
      { "month": "2026-03", "newSubscriptions": 160, "revenue": 125000.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 6.12 Expiring Soon Subscriptions

```
GET /admin/subscriptions/expiring-soon
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| days | int | 7 | Days until expiry |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "subscriptionId": "sub-guid",
        "driverId": "driver-guid",
        "driverName": "أحمد محمد",
        "driverPhone": "+201012345678",
        "planName": "Premium",
        "expiresAt": "2026-04-02T00:00:00Z",
        "daysRemaining": 3,
        "autoRenew": false
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

## 7. Admin Orders

> **Base**: `/api/v1/admin/orders`

Full order lifecycle management including creation, assignment, status override, and analytics.

---

### 7.1 List Orders

```
GET /admin/orders
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by OrderStatus |
| driverId | GUID? | null | Filter by assigned driver |
| customerId | GUID? | null | Filter by customer |
| partnerId | GUID? | null | Filter by partner |
| regionId | int? | null | Filter by region |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |
| search | string | null | Search by order number |
| sortBy | string | "createdAt" | Sort field |
| sortDesc | bool | true | Sort descending |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "order-guid",
        "orderNumber": "ORD-20260330-001",
        "status": 3,
        "statusName": "InTransit",
        "customerName": "محمد علي",
        "customerPhone": "+201112345678",
        "driverName": "أحمد محمد",
        "driverPhone": "+201012345678",
        "partnerName": "مطعم البيك",
        "pickupAddress": "شارع التحرير، القاهرة",
        "deliveryAddress": "مدينة نصر، القاهرة",
        "totalAmount": 150.00,
        "deliveryFee": 25.00,
        "estimatedDeliveryTime": 30,
        "createdAt": "2026-03-30T14:00:00Z",
        "assignedAt": "2026-03-30T14:02:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 5420,
    "totalPages": 542,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 7.2 Get Order Board

```
GET /admin/orders/board
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| regionId | int? | null | Filter by region |
| date | DateTime? | today | Date to show |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "pending": {
      "count": 15,
      "orders": [
        {
          "id": "order-guid",
          "orderNumber": "ORD-20260330-001",
          "customerName": "محمد علي",
          "pickupAddress": "شارع التحرير",
          "minutesWaiting": 5,
          "totalAmount": 150.00
        }
      ]
    },
    "assigned": {
      "count": 32,
      "orders": [ ... ]
    },
    "inTransit": {
      "count": 28,
      "orders": [ ... ]
    },
    "delivered": {
      "count": 180,
      "orders": [ ... ]
    },
    "cancelled": {
      "count": 8,
      "orders": [ ... ]
    }
  },
  "message": null,
  "errors": null
}
```

---

### 7.3 Create Order

```
POST /admin/orders
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "customerId": "customer-guid",
  "partnerId": "partner-guid",
  "pickupAddress": "شارع التحرير، وسط البلد، القاهرة",
  "pickupLatitude": 30.0444,
  "pickupLongitude": 31.2357,
  "deliveryAddress": "شارع مكرم عبيد، مدينة نصر، القاهرة",
  "deliveryLatitude": 30.0561,
  "deliveryLongitude": 31.3464,
  "items": [
    {
      "name": "وجبة دجاج",
      "quantity": 2,
      "price": 75.00
    }
  ],
  "notes": "الطابق الثالث",
  "scheduledAt": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| customerId | GUID | Yes | Customer ID |
| partnerId | GUID | No | Partner/merchant ID |
| pickupAddress | string | Yes | Pickup address text |
| pickupLatitude | double | Yes | Pickup latitude |
| pickupLongitude | double | Yes | Pickup longitude |
| deliveryAddress | string | Yes | Delivery address text |
| deliveryLatitude | double | Yes | Delivery latitude |
| deliveryLongitude | double | Yes | Delivery longitude |
| items | array | Yes | Order items |
| notes | string | No | Delivery notes |
| scheduledAt | DateTime? | No | Schedule for later (null = immediate) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-order-guid",
    "orderNumber": "ORD-20260330-250",
    "status": 0,
    "statusName": "Pending",
    "totalAmount": 150.00,
    "deliveryFee": 25.00,
    "createdAt": "2026-03-30T15:00:00Z"
  },
  "message": "تم إنشاء الطلب بنجاح",
  "errors": null
}
```

---

### 7.4 Assign Order to Driver

```
POST /admin/orders/{id}/assign
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Order ID

**Request Body**:
```json
{
  "driverId": "driver-guid"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "order-guid",
    "driverId": "driver-guid",
    "driverName": "أحمد محمد",
    "assignedAt": "2026-03-30T15:01:00Z"
  },
  "message": "تم تعيين الطلب للسائق بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب غير موجود` |
| 404 | `السائق غير موجود` |
| 400 | `السائق غير متاح حالياً` |
| 400 | `الطلب معيّن لسائق بالفعل` |

---

### 7.5 Auto-Distribute Orders

```
POST /admin/orders/auto-distribute
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "regionId": 1,
  "maxOrdersPerDriver": 5
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| regionId | int? | No | Limit to region |
| maxOrdersPerDriver | int | No | Max orders per driver (default: system config) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalDistributed": 15,
    "driversAssigned": 8,
    "remainingUnassigned": 3,
    "assignments": [
      {
        "orderId": "order-guid",
        "orderNumber": "ORD-20260330-001",
        "driverId": "driver-guid",
        "driverName": "أحمد محمد"
      }
    ]
  },
  "message": "تم توزيع الطلبات تلقائياً",
  "errors": null
}
```

---

### 7.6 Get Order Timeline

```
GET /admin/orders/{id}/timeline
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Order ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "order-guid",
    "orderNumber": "ORD-20260330-001",
    "events": [
      {
        "status": "Created",
        "timestamp": "2026-03-30T14:00:00Z",
        "actor": "System",
        "notes": null
      },
      {
        "status": "Assigned",
        "timestamp": "2026-03-30T14:02:00Z",
        "actor": "Admin (سارة أحمد)",
        "notes": "تعيين يدوي"
      },
      {
        "status": "PickedUp",
        "timestamp": "2026-03-30T14:15:00Z",
        "actor": "Driver (أحمد محمد)",
        "notes": null
      },
      {
        "status": "InTransit",
        "timestamp": "2026-03-30T14:16:00Z",
        "actor": "System",
        "notes": null
      },
      {
        "status": "Delivered",
        "timestamp": "2026-03-30T14:35:00Z",
        "actor": "Driver (أحمد محمد)",
        "notes": "تم التسليم للعميل"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب غير موجود` |

---

### 7.7 Override Order Status

```
PUT /admin/orders/{id}/override-status
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Order ID

**Request Body**:
```json
{
  "newStatus": 5,
  "reason": "تأكيد التسليم يدوياً بعد مشكلة في التطبيق"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| newStatus | int | Yes | Target OrderStatus enum value |
| reason | string | Yes | Reason for override |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "order-guid",
    "oldStatus": "InTransit",
    "newStatus": "Delivered",
    "overriddenBy": "Admin (سارة أحمد)",
    "reason": "تأكيد التسليم يدوياً بعد مشكلة في التطبيق"
  },
  "message": "تم تغيير حالة الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب غير موجود` |
| 400 | `لا يمكن تغيير حالة الطلب إلى الحالة المطلوبة` |

---

### 7.8 Export Orders

```
GET /admin/orders/export
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| status | int? | null | Filter by status |
| format | string | "csv" | Export format: csv, xlsx |

**Response** `200 OK`:
Returns file download with `Content-Type: application/octet-stream` and `Content-Disposition: attachment; filename=orders_export_20260330.csv`.

---

### 7.9 Auto-Assign Single Order

```
POST /admin/orders/{id}/auto-assign
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Order ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "order-guid",
    "assignedDriverId": "driver-guid",
    "assignedDriverName": "أحمد محمد",
    "estimatedPickupTime": 8,
    "assignmentScore": 95.5
  },
  "message": "تم تعيين السائق تلقائياً",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب غير موجود` |
| 400 | `لا يوجد سائقين متاحين حالياً` |
| 400 | `الطلب معيّن لسائق بالفعل` |

---

### 7.10 Get Suggested Drivers for Order

```
GET /admin/orders/{id}/suggested-drivers
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Order ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "driver-guid-1",
      "driverName": "أحمد محمد",
      "distanceKm": 1.2,
      "estimatedPickupMinutes": 5,
      "rating": 4.8,
      "activeOrders": 1,
      "score": 95.5,
      "vehicleType": 0,
      "vehicleTypeName": "Motorcycle"
    },
    {
      "driverId": "driver-guid-2",
      "driverName": "محمود حسن",
      "distanceKm": 2.5,
      "estimatedPickupMinutes": 10,
      "rating": 4.6,
      "activeOrders": 0,
      "score": 88.2,
      "vehicleType": 1,
      "vehicleTypeName": "Car"
    }
  ],
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب غير موجود` |

---

## 8. Admin Time Slots

> **Base**: `/api/v1/admin/time-slots`

Manage delivery time slot configurations.

---

### 8.1 List Time Slots

```
GET /admin/time-slots
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| isActive | bool? | null | Filter by active status |
| regionId | int? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "صباحي",
      "startTime": "08:00",
      "endTime": "12:00",
      "maxOrders": 100,
      "currentOrders": 45,
      "isActive": true,
      "regionId": 1,
      "regionName": "القاهرة"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 8.2 Get Time Slot Details

```
GET /admin/time-slots/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Time Slot ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "صباحي",
    "startTime": "08:00",
    "endTime": "12:00",
    "maxOrders": 100,
    "currentOrders": 45,
    "surchargePercent": 0,
    "isActive": true,
    "regionId": 1,
    "regionName": "القاهرة",
    "createdAt": "2026-01-01T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 8.3 Create Time Slot

```
POST /admin/time-slots
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "مسائي",
  "startTime": "18:00",
  "endTime": "22:00",
  "maxOrders": 80,
  "surchargePercent": 10,
  "regionId": 1,
  "isActive": true
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 4,
    "name": "مسائي",
    "startTime": "18:00",
    "endTime": "22:00"
  },
  "message": "تم إنشاء الفترة الزمنية بنجاح",
  "errors": null
}
```

---

### 8.4 Update Time Slot

```
PUT /admin/time-slots/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Time Slot ID

**Request Body**:
```json
{
  "name": "مسائي معدّل",
  "startTime": "17:00",
  "endTime": "23:00",
  "maxOrders": 120,
  "surchargePercent": 15,
  "isActive": true
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 4,
    "name": "مسائي معدّل"
  },
  "message": "تم تعديل الفترة الزمنية بنجاح",
  "errors": null
}
```

---

### 8.5 Delete Time Slot

```
DELETE /admin/time-slots/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Time Slot ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الفترة الزمنية بنجاح",
  "errors": null
}
```

---

### 8.6 Toggle Time Slot

```
PUT /admin/time-slots/{id}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Time Slot ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 4,
    "isActive": false
  },
  "message": "تم تغيير حالة الفترة الزمنية بنجاح",
  "errors": null
}
```

---

## 9. Admin Customers

> **Base**: `/api/v1/admin/customers`

Manage customer accounts.

---

### 9.1 List Customers

```
GET /admin/customers
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| search | string | null | Search by name/phone |
| status | int? | null | Filter by status |
| sortBy | string | "createdAt" | Sort field |
| sortDesc | bool | true | Sort descending |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "customer-guid",
        "name": "محمد علي",
        "phone": "+201112345678",
        "email": "mohamed@example.com",
        "status": 1,
        "statusName": "Active",
        "totalOrders": 85,
        "totalSpent": 12500.00,
        "walletBalance": 350.00,
        "joinedAt": "2026-02-01T00:00:00Z",
        "lastOrderAt": "2026-03-29T18:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 3200,
    "totalPages": 320,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 9.2 Get Customer Details

```
GET /admin/customers/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "customer-guid",
    "name": "محمد علي",
    "phone": "+201112345678",
    "email": "mohamed@example.com",
    "profileImageUrl": null,
    "status": 1,
    "statusName": "Active",
    "totalOrders": 85,
    "completedOrders": 80,
    "cancelledOrders": 5,
    "totalSpent": 12500.00,
    "walletBalance": 350.00,
    "addresses": [
      {
        "id": "addr-guid",
        "label": "المنزل",
        "address": "شارع مكرم عبيد، مدينة نصر",
        "latitude": 30.0561,
        "longitude": 31.3464,
        "isDefault": true
      }
    ],
    "joinedAt": "2026-02-01T00:00:00Z",
    "lastOrderAt": "2026-03-29T18:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |

---

### 9.3 Activate Customer

```
PUT /admin/customers/{id}/activate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل العميل بنجاح",
  "errors": null
}
```

---

### 9.4 Deactivate Customer

```
PUT /admin/customers/{id}/deactivate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تفعيل العميل بنجاح",
  "errors": null
}
```

---

### 9.5 Get Customer Order History

```
GET /admin/customers/{id}/orders
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Customer ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by order status |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "order-guid",
        "orderNumber": "ORD-20260329-045",
        "status": 5,
        "statusName": "Delivered",
        "driverName": "أحمد محمد",
        "totalAmount": 150.00,
        "deliveryFee": 25.00,
        "createdAt": "2026-03-29T18:00:00Z",
        "deliveredAt": "2026-03-29T18:35:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 85,
    "totalPages": 9,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

## 10. Admin Partners

> **Base**: `/api/v1/admin/partners`

Manage partner/merchant accounts with verification workflow.

---

### 10.1 List Partners

```
GET /admin/partners
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| search | string | null | Search by name/phone |
| status | int? | null | Filter by PartnerStatus |
| verificationStatus | int? | null | Filter by verification |
| categoryId | int? | null | Filter by category |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "partner-guid",
        "businessName": "مطعم البيك",
        "ownerName": "خالد سعيد",
        "phone": "+201512345678",
        "email": "elbaik@example.com",
        "category": "Restaurants",
        "status": 1,
        "statusName": "Active",
        "verificationStatus": 2,
        "verificationStatusName": "Verified",
        "totalOrders": 1200,
        "rating": 4.7,
        "commissionRate": 15.0,
        "joinedAt": "2026-01-10T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 180,
    "totalPages": 18,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 10.2 Get Partner Details

```
GET /admin/partners/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "partner-guid",
    "businessName": "مطعم البيك",
    "ownerName": "خالد سعيد",
    "phone": "+201512345678",
    "email": "elbaik@example.com",
    "logoUrl": "https://storage.sekka.com/partners/logo.jpg",
    "category": "Restaurants",
    "address": "شارع التحرير، وسط البلد",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "status": 1,
    "statusName": "Active",
    "verificationStatus": 2,
    "verificationStatusName": "Verified",
    "taxId": "123-456-789",
    "commercialRegister": "CR-2026-001",
    "commissionRate": 15.0,
    "totalOrders": 1200,
    "totalRevenue": 450000.00,
    "rating": 4.7,
    "documentsUrls": [
      "https://storage.sekka.com/partners/docs/tax.pdf",
      "https://storage.sekka.com/partners/docs/license.pdf"
    ],
    "joinedAt": "2026-01-10T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 10.3 Activate Partner

```
PUT /admin/partners/{id}/activate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل الشريك بنجاح",
  "errors": null
}
```

---

### 10.4 Deactivate Partner

```
PUT /admin/partners/{id}/deactivate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تفعيل الشريك بنجاح",
  "errors": null
}
```

---

### 10.5 Verify Partner

```
PUT /admin/partners/{id}/verify
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "notes": "تم التحقق من جميع المستندات"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "partner-guid",
    "verificationStatus": 2,
    "verificationStatusName": "Verified",
    "verifiedAt": "2026-03-30T15:00:00Z"
  },
  "message": "تم التحقق من الشريك بنجاح",
  "errors": null
}
```

---

### 10.6 Reject Partner Verification

```
PUT /admin/partners/{id}/reject
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "reason": "المستندات غير مكتملة - يرجى تحميل السجل التجاري"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| reason | string | Yes | Rejection reason |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "partner-guid",
    "verificationStatus": 3,
    "verificationStatusName": "Rejected",
    "rejectionReason": "المستندات غير مكتملة - يرجى تحميل السجل التجاري"
  },
  "message": "تم رفض التحقق من الشريك",
  "errors": null
}
```

---

### 10.7 Update Partner Commission

```
PUT /admin/partners/{id}/commission
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "commissionRate": 12.5,
  "reason": "تخفيض العمولة لشريك مميز"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "partner-guid",
    "oldRate": 15.0,
    "newRate": 12.5
  },
  "message": "تم تعديل نسبة العمولة بنجاح",
  "errors": null
}
```

---

### 10.8 Get Partner Orders

```
GET /admin/partners/{id}/orders
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Partner ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by order status |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "order-guid",
        "orderNumber": "ORD-20260330-001",
        "status": 5,
        "statusName": "Delivered",
        "customerName": "محمد علي",
        "driverName": "أحمد محمد",
        "totalAmount": 150.00,
        "commission": 22.50,
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1200,
    "totalPages": 120,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

## 11. Admin Blacklist

> **Base**: `/api/v1/admin/blacklist`

Manage blacklisted users (drivers, customers, partners).

---

### 11.1 List Blacklisted Users

```
GET /admin/blacklist
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| userType | string | null | Filter: "driver", "customer", "partner" |
| search | string | null | Search by name/phone |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "blacklist-guid",
        "userId": "user-guid",
        "userName": "مستخدم محظور",
        "userPhone": "+201012345678",
        "userType": "driver",
        "reason": "سلوك مخالف متكرر",
        "blockedBy": "Admin (سارة أحمد)",
        "blockedAt": "2026-03-25T10:00:00Z",
        "expiresAt": null
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 11.2 Add to Blacklist

```
POST /admin/blacklist
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "userId": "user-guid",
  "userType": "driver",
  "reason": "سلوك مخالف متكرر",
  "expiresAt": "2026-06-30T00:00:00Z"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | GUID | Yes | User to block |
| userType | string | Yes | "driver", "customer", "partner" |
| reason | string | Yes | Block reason |
| expiresAt | DateTime? | No | Null for permanent |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "blacklist-guid",
    "userId": "user-guid",
    "userName": "مستخدم محظور"
  },
  "message": "تم إضافة المستخدم للقائمة السوداء",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المستخدم غير موجود` |
| 409 | `المستخدم محظور بالفعل` |

---

### 11.3 Remove from Blacklist

```
DELETE /admin/blacklist/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Blacklist entry ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إزالة المستخدم من القائمة السوداء",
  "errors": null
}
```

---

### 11.4 Update Blacklist Entry

```
PUT /admin/blacklist/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Blacklist entry ID

**Request Body**:
```json
{
  "reason": "تحديث سبب الحظر",
  "expiresAt": "2026-09-30T00:00:00Z"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل بيانات الحظر بنجاح",
  "errors": null
}
```

---

## 12. Admin Settlements

> **Base**: `/api/v1/admin/settlements`

Manage financial settlements with drivers and partners.

---

### 12.1 List Settlements

```
GET /admin/settlements
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by SettlementStatus |
| type | string | null | "driver" or "partner" |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "settlement-guid",
        "recipientId": "driver-guid",
        "recipientName": "أحمد محمد",
        "recipientType": "driver",
        "amount": 5000.00,
        "status": 1,
        "statusName": "Pending",
        "periodFrom": "2026-03-01T00:00:00Z",
        "periodTo": "2026-03-15T00:00:00Z",
        "ordersCount": 120,
        "createdAt": "2026-03-16T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 350,
    "totalPages": 35,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 12.2 Get Settlement Details

```
GET /admin/settlements/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Settlement ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "settlement-guid",
    "recipientId": "driver-guid",
    "recipientName": "أحمد محمد",
    "recipientPhone": "+201012345678",
    "recipientType": "driver",
    "amount": 5000.00,
    "deductions": 500.00,
    "netAmount": 4500.00,
    "status": 1,
    "statusName": "Pending",
    "periodFrom": "2026-03-01T00:00:00Z",
    "periodTo": "2026-03-15T00:00:00Z",
    "ordersCount": 120,
    "breakdown": {
      "deliveryFees": 4000.00,
      "tips": 800.00,
      "bonuses": 200.00,
      "penalties": -500.00
    },
    "createdAt": "2026-03-16T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 12.3 Approve Settlement

```
PUT /admin/settlements/{id}/approve
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Settlement ID

**Request Body**:
```json
{
  "notes": "تمت المراجعة والموافقة"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم اعتماد التسوية بنجاح",
  "errors": null
}
```

---

### 12.4 Reject Settlement

```
PUT /admin/settlements/{id}/reject
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Settlement ID

**Request Body**:
```json
{
  "reason": "يوجد خلاف على عدد الطلبات"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم رفض التسوية",
  "errors": null
}
```

---

### 12.5 Process Settlement Payment

```
POST /admin/settlements/{id}/process
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Settlement ID

**Request Body**:
```json
{
  "paymentMethod": "BankTransfer",
  "transactionReference": "TXN-20260330-001"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "settlement-guid",
    "status": 3,
    "statusName": "Paid",
    "paidAt": "2026-03-30T16:00:00Z",
    "transactionReference": "TXN-20260330-001"
  },
  "message": "تم تسوية الدفعة بنجاح",
  "errors": null
}
```

---

## 13. Admin Payments

> **Base**: `/api/v1/admin/payments`

View and manage payment transactions.

---

### 13.1 List Payments

```
GET /admin/payments
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by PaymentStatus |
| method | int? | null | Filter by payment method |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |
| search | string | null | Search by transaction ref |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "payment-guid",
        "transactionRef": "PAY-20260330-001",
        "orderId": "order-guid",
        "orderNumber": "ORD-20260330-001",
        "amount": 150.00,
        "method": 1,
        "methodName": "Wallet",
        "status": 2,
        "statusName": "Completed",
        "payerName": "محمد علي",
        "payerType": "customer",
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 8500,
    "totalPages": 850,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 13.2 Get Payment Details

```
GET /admin/payments/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Payment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "payment-guid",
    "transactionRef": "PAY-20260330-001",
    "orderId": "order-guid",
    "orderNumber": "ORD-20260330-001",
    "amount": 150.00,
    "fees": 2.50,
    "netAmount": 147.50,
    "method": 1,
    "methodName": "Wallet",
    "status": 2,
    "statusName": "Completed",
    "payerName": "محمد علي",
    "payerType": "customer",
    "gatewayResponse": null,
    "createdAt": "2026-03-30T14:00:00Z",
    "completedAt": "2026-03-30T14:00:05Z"
  },
  "message": null,
  "errors": null
}
```

---

### 13.3 Retry Failed Payment

```
POST /admin/payments/{id}/retry
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Payment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "payment-guid",
    "status": 2,
    "statusName": "Completed",
    "retriedAt": "2026-03-30T16:00:00Z"
  },
  "message": "تم إعادة محاولة الدفع بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الدفعة ليست في حالة فشل` |

---

### 13.4 Void Payment

```
POST /admin/payments/{id}/void
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Payment ID

**Request Body**:
```json
{
  "reason": "طلب ملغي - إلغاء الدفعة"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء الدفعة بنجاح",
  "errors": null
}
```

---

### 13.5 Export Payments

```
GET /admin/payments/export
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| format | string | "csv" | Export format: csv, xlsx |

**Response** `200 OK`:
Returns file download.

---

## 14. Admin Wallets

> **Base**: `/api/v1/admin/wallets`

Manage user wallets: balance adjustments, freezing, bulk operations, and statistics.

---

### 14.1 List Wallets

```
GET /admin/wallets
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| ownerType | string | null | "driver", "customer", "partner" |
| search | string | null | Search by owner name/phone |
| minBalance | decimal? | null | Minimum balance filter |
| maxBalance | decimal? | null | Maximum balance filter |
| isFrozen | bool? | null | Filter frozen wallets |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "wallet-guid",
        "ownerId": "driver-guid",
        "ownerName": "أحمد محمد",
        "ownerPhone": "+201012345678",
        "ownerType": "driver",
        "balance": 3200.50,
        "frozenBalance": 0,
        "isFrozen": false,
        "totalCredits": 45000.00,
        "totalDebits": 41799.50,
        "lastTransactionAt": "2026-03-30T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 4500,
    "totalPages": 450,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 14.2 Get Wallet Details

```
GET /admin/wallets/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Wallet ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "wallet-guid",
    "ownerId": "driver-guid",
    "ownerName": "أحمد محمد",
    "ownerPhone": "+201012345678",
    "ownerType": "driver",
    "balance": 3200.50,
    "frozenBalance": 0,
    "isFrozen": false,
    "totalCredits": 45000.00,
    "totalDebits": 41799.50,
    "recentTransactions": [
      {
        "id": "txn-guid",
        "type": "Credit",
        "amount": 150.00,
        "description": "رسوم توصيل طلب ORD-20260330-001",
        "balanceAfter": 3200.50,
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "lastTransactionAt": "2026-03-30T14:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 14.3 Get Wallet Transactions

```
GET /admin/wallets/{id}/transactions
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Wallet ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| type | string | null | "Credit" or "Debit" |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "txn-guid",
        "type": "Credit",
        "amount": 150.00,
        "description": "رسوم توصيل طلب ORD-20260330-001",
        "reference": "order-guid",
        "balanceBefore": 3050.50,
        "balanceAfter": 3200.50,
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 500,
    "totalPages": 25,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 14.4 Adjust Wallet Balance

```
POST /admin/wallets/{id}/adjust
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Wallet ID

**Request Body**:
```json
{
  "amount": 500.00,
  "type": "Credit",
  "reason": "تعويض عن خطأ في النظام"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| amount | decimal | Yes | Positive amount |
| type | string | Yes | "Credit" or "Debit" |
| reason | string | Yes | Adjustment reason |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "walletId": "wallet-guid",
    "newBalance": 3700.50,
    "adjustmentAmount": 500.00,
    "type": "Credit"
  },
  "message": "تم تعديل رصيد المحفظة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رصيد المحفظة غير كافي` |
| 400 | `المحفظة مجمّدة` |

---

### 14.5 Freeze Wallet

```
PUT /admin/wallets/{id}/freeze
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Wallet ID

**Request Body**:
```json
{
  "reason": "تحقيق في عمليات مشبوهة"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تجميد المحفظة بنجاح",
  "errors": null
}
```

---

### 14.6 Unfreeze Wallet

```
PUT /admin/wallets/{id}/unfreeze
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Wallet ID

**Request Body**:
```json
{
  "reason": "اكتمال التحقيق - لا توجد مخالفات"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تجميد المحفظة بنجاح",
  "errors": null
}
```

---

### 14.7 Bulk Wallet Adjustment

```
POST /admin/wallets/bulk-adjust
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "walletIds": ["wallet-guid-1", "wallet-guid-2", "wallet-guid-3"],
  "amount": 100.00,
  "type": "Credit",
  "reason": "مكافأة عيد الفطر"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| walletIds | GUID[] | Yes | Target wallets |
| amount | decimal | Yes | Amount per wallet |
| type | string | Yes | "Credit" or "Debit" |
| reason | string | Yes | Reason |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalWallets": 3,
    "successCount": 3,
    "failedCount": 0,
    "totalAmount": 300.00
  },
  "message": "تم تعديل رصيد المحافظ بنجاح",
  "errors": null
}
```

---

### 14.8 Bulk Freeze Wallets

```
POST /admin/wallets/bulk-freeze
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "walletIds": ["wallet-guid-1", "wallet-guid-2"],
  "reason": "تحقيق أمني"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalWallets": 2,
    "frozenCount": 2
  },
  "message": "تم تجميد المحافظ بنجاح",
  "errors": null
}
```

---

### 14.9 Bulk Unfreeze Wallets

```
POST /admin/wallets/bulk-unfreeze
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "walletIds": ["wallet-guid-1", "wallet-guid-2"],
  "reason": "اكتمال التحقيق"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalWallets": 2,
    "unfrozenCount": 2
  },
  "message": "تم إلغاء تجميد المحافظ بنجاح",
  "errors": null
}
```

---

### 14.10 Wallet Statistics

```
GET /admin/wallets/stats
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalWallets": 4500,
    "totalBalance": 2500000.00,
    "totalFrozenBalance": 85000.00,
    "frozenWalletsCount": 12,
    "averageBalance": 555.56,
    "byOwnerType": {
      "driver": { "count": 1250, "totalBalance": 1200000.00 },
      "customer": { "count": 3200, "totalBalance": 1100000.00 },
      "partner": { "count": 50, "totalBalance": 200000.00 }
    },
    "todayTransactions": {
      "credits": 125000.00,
      "debits": 98000.00,
      "count": 1500
    }
  },
  "message": null,
  "errors": null
}
```

---

## 15. Admin Disputes

> **Base**: `/api/v1/admin/disputes`

Manage order and payment disputes.

---

### 15.1 List Disputes

```
GET /admin/disputes
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by DisputeStatus |
| priority | int? | null | Filter by priority |
| type | int? | null | Filter by dispute type |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "dispute-guid",
        "orderId": "order-guid",
        "orderNumber": "ORD-20260328-045",
        "reporterName": "محمد علي",
        "reporterType": "customer",
        "type": 1,
        "typeName": "WrongItem",
        "priority": 2,
        "priorityName": "High",
        "status": 0,
        "statusName": "Open",
        "description": "الطلب وصل ناقص",
        "createdAt": "2026-03-28T19:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 15.2 Get Dispute Details

```
GET /admin/disputes/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Dispute ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "dispute-guid",
    "orderId": "order-guid",
    "orderNumber": "ORD-20260328-045",
    "reporterName": "محمد علي",
    "reporterPhone": "+201112345678",
    "reporterType": "customer",
    "type": 1,
    "typeName": "WrongItem",
    "priority": 2,
    "priorityName": "High",
    "status": 0,
    "statusName": "Open",
    "description": "الطلب وصل ناقص - ينقص صنف واحد",
    "evidenceUrls": ["https://storage.sekka.com/disputes/photo1.jpg"],
    "assignedTo": null,
    "resolution": null,
    "messages": [
      {
        "sender": "محمد علي",
        "senderType": "customer",
        "message": "الطلب وصل ناقص",
        "createdAt": "2026-03-28T19:00:00Z"
      }
    ],
    "createdAt": "2026-03-28T19:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 15.3 Assign Dispute

```
PUT /admin/disputes/{id}/assign
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Dispute ID

**Request Body**:
```json
{
  "adminId": "admin-guid"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعيين النزاع بنجاح",
  "errors": null
}
```

---

### 15.4 Respond to Dispute

```
POST /admin/disputes/{id}/respond
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Dispute ID

**Request Body**:
```json
{
  "message": "نعتذر عن الخطأ. سيتم إرسال الصنف الناقص أو استرداد قيمته."
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إرسال الرد بنجاح",
  "errors": null
}
```

---

### 15.5 Resolve Dispute

```
PUT /admin/disputes/{id}/resolve
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Dispute ID

**Request Body**:
```json
{
  "resolution": "تم استرداد قيمة الصنف الناقص للعميل",
  "refundAmount": 75.00,
  "favoredParty": "customer"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| resolution | string | Yes | Resolution description |
| refundAmount | decimal? | No | Refund amount if applicable |
| favoredParty | string | Yes | "customer", "driver", "partner" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "dispute-guid",
    "status": 2,
    "statusName": "Resolved",
    "resolution": "تم استرداد قيمة الصنف الناقص للعميل",
    "resolvedAt": "2026-03-30T16:00:00Z"
  },
  "message": "تم حل النزاع بنجاح",
  "errors": null
}
```

---

### 15.6 Escalate Dispute

```
PUT /admin/disputes/{id}/escalate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Dispute ID

**Request Body**:
```json
{
  "reason": "العميل غير راضٍ عن الحل المقترح",
  "newPriority": 3
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تصعيد النزاع بنجاح",
  "errors": null
}
```

---

### 15.7 Get Dispute Statistics

```
GET /admin/disputes/stats
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalOpen": 15,
    "totalInProgress": 8,
    "totalResolved": 320,
    "totalEscalated": 5,
    "averageResolutionHours": 12.5,
    "customerSatisfactionRate": 85.0,
    "byType": [
      { "type": "WrongItem", "count": 12 },
      { "type": "LateDelivery", "count": 8 },
      { "type": "DamagedItem", "count": 3 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 16. Admin Invoices

> **Base**: `/api/v1/admin/invoices`

Manage invoices for partners and settlements.

---

### 16.1 List Invoices

```
GET /admin/invoices
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by InvoiceStatus |
| partnerId | GUID? | null | Filter by partner |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "invoice-guid",
        "invoiceNumber": "INV-2026-0330-001",
        "partnerId": "partner-guid",
        "partnerName": "مطعم البيك",
        "amount": 15000.00,
        "tax": 2100.00,
        "totalAmount": 17100.00,
        "status": 1,
        "statusName": "Issued",
        "dueDate": "2026-04-15T00:00:00Z",
        "createdAt": "2026-03-30T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 200,
    "totalPages": 20,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 16.2 Get Invoice Details

```
GET /admin/invoices/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Invoice ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "invoice-guid",
    "invoiceNumber": "INV-2026-0330-001",
    "partnerId": "partner-guid",
    "partnerName": "مطعم البيك",
    "partnerTaxId": "123-456-789",
    "lineItems": [
      {
        "description": "عمولات الطلبات (1-15 مارس)",
        "quantity": 500,
        "unitPrice": 30.00,
        "amount": 15000.00
      }
    ],
    "subtotal": 15000.00,
    "taxRate": 14.0,
    "tax": 2100.00,
    "totalAmount": 17100.00,
    "status": 1,
    "statusName": "Issued",
    "dueDate": "2026-04-15T00:00:00Z",
    "paidAt": null,
    "createdAt": "2026-03-30T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 16.3 Create Invoice

```
POST /admin/invoices
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "partnerId": "partner-guid",
  "lineItems": [
    {
      "description": "عمولات الطلبات (1-15 مارس)",
      "quantity": 500,
      "unitPrice": 30.00
    }
  ],
  "dueDate": "2026-04-15T00:00:00Z",
  "notes": "فاتورة النصف الأول من مارس"
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-invoice-guid",
    "invoiceNumber": "INV-2026-0330-002",
    "totalAmount": 17100.00
  },
  "message": "تم إنشاء الفاتورة بنجاح",
  "errors": null
}
```

---

### 16.4 Mark Invoice as Paid

```
PUT /admin/invoices/{id}/mark-paid
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Invoice ID

**Request Body**:
```json
{
  "paymentMethod": "BankTransfer",
  "transactionReference": "TXN-20260330-001",
  "paidAt": "2026-03-30T00:00:00Z"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الدفع بنجاح",
  "errors": null
}
```

---

### 16.5 Cancel Invoice

```
PUT /admin/invoices/{id}/cancel
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Invoice ID

**Request Body**:
```json
{
  "reason": "إلغاء بسبب خطأ في الحساب"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء الفاتورة بنجاح",
  "errors": null
}
```

---

### 16.6 Download Invoice PDF

```
GET /admin/invoices/{id}/download
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Invoice ID

**Response** `200 OK`:
Returns PDF file download with `Content-Type: application/pdf`.

---

### 16.7 Send Invoice by Email

```
POST /admin/invoices/{id}/send
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Invoice ID

**Request Body**:
```json
{
  "email": "billing@elbaik.com",
  "message": "مرفق فاتورة شهر مارس"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إرسال الفاتورة بنجاح",
  "errors": null
}
```

---

## 17. Admin Refunds

> **Base**: `/api/v1/admin/refunds`

Process and manage refund requests.

---

### 17.1 List Refunds

```
GET /admin/refunds
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by RefundStatus |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "refund-guid",
        "orderId": "order-guid",
        "orderNumber": "ORD-20260328-045",
        "customerName": "محمد علي",
        "amount": 150.00,
        "reason": "طلب ملغي بعد تأخر التوصيل",
        "status": 0,
        "statusName": "Pending",
        "requestedAt": "2026-03-28T20:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 30,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 17.2 Get Refund Details

```
GET /admin/refunds/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Refund ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "refund-guid",
    "orderId": "order-guid",
    "orderNumber": "ORD-20260328-045",
    "paymentId": "payment-guid",
    "customerName": "محمد علي",
    "customerPhone": "+201112345678",
    "amount": 150.00,
    "originalPaymentAmount": 150.00,
    "reason": "طلب ملغي بعد تأخر التوصيل",
    "status": 0,
    "statusName": "Pending",
    "refundMethod": "Wallet",
    "processedBy": null,
    "requestedAt": "2026-03-28T20:00:00Z",
    "processedAt": null
  },
  "message": null,
  "errors": null
}
```

---

### 17.3 Approve Refund

```
PUT /admin/refunds/{id}/approve
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Refund ID

**Request Body**:
```json
{
  "amount": 150.00,
  "refundMethod": "Wallet",
  "notes": "استرداد كامل المبلغ"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| amount | decimal | Yes | Refund amount (can be partial) |
| refundMethod | string | Yes | "Wallet", "OriginalMethod" |
| notes | string | No | Admin notes |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "refund-guid",
    "status": 1,
    "statusName": "Approved",
    "approvedAmount": 150.00
  },
  "message": "تم الموافقة على الاسترداد بنجاح",
  "errors": null
}
```

---

### 17.4 Reject Refund

```
PUT /admin/refunds/{id}/reject
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Refund ID

**Request Body**:
```json
{
  "reason": "الطلب تم تسليمه بنجاح"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم رفض طلب الاسترداد",
  "errors": null
}
```

---

### 17.5 Process Refund

```
POST /admin/refunds/{id}/process
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Refund ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "refund-guid",
    "status": 2,
    "statusName": "Processed",
    "processedAt": "2026-03-30T16:00:00Z"
  },
  "message": "تم تنفيذ الاسترداد بنجاح",
  "errors": null
}
```

---

### 17.6 Refund Statistics

```
GET /admin/refunds/stats
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalPending": 10,
    "totalApproved": 15,
    "totalProcessed": 280,
    "totalRejected": 25,
    "totalRefundedAmount": 42000.00,
    "averageRefundAmount": 150.00,
    "refundRate": 3.2,
    "topReasons": [
      { "reason": "تأخر التوصيل", "count": 120 },
      { "reason": "طلب ملغي", "count": 95 },
      { "reason": "صنف ناقص", "count": 65 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 18. Admin Statistics

> **Base**: `/api/v1/admin/statistics`

Comprehensive analytics and dashboards with 20 specialized endpoints.

---

### 18.1 Dashboard Overview

```
GET /admin/statistics/dashboard
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "todayOrders": 245,
    "todayRevenue": 36750.00,
    "activeDrivers": 180,
    "onlineDrivers": 95,
    "activeCustomers": 3200,
    "pendingOrders": 15,
    "averageDeliveryTime": 28.5,
    "todayNewUsers": 12,
    "orderCompletionRate": 96.5,
    "comparedToYesterday": {
      "orders": 8.5,
      "revenue": 12.3,
      "newUsers": -5.0
    }
  },
  "message": null,
  "errors": null
}
```

---

### 18.2 Revenue Statistics

```
GET /admin/statistics/revenue
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| groupBy | string | "day" | "day", "week", "month" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalRevenue": 1250000.00,
    "totalOrders": 8500,
    "averageOrderValue": 147.06,
    "commissionEarned": 187500.00,
    "deliveryFeesCollected": 212500.00,
    "timeline": [
      {
        "period": "2026-03-01",
        "revenue": 42000.00,
        "orders": 280,
        "commission": 6300.00
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.3 Order Statistics

```
GET /admin/statistics/orders
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| groupBy | string | "day" | "day", "week", "month" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalOrders": 8500,
    "completedOrders": 8200,
    "cancelledOrders": 300,
    "completionRate": 96.5,
    "averageDeliveryTime": 28.5,
    "byStatus": [
      { "status": "Delivered", "count": 8200 },
      { "status": "Cancelled", "count": 300 }
    ],
    "byHour": [
      { "hour": 12, "count": 850 },
      { "hour": 13, "count": 920 }
    ],
    "timeline": [
      { "period": "2026-03-01", "total": 280, "completed": 270, "cancelled": 10 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.4 Driver Performance Statistics

```
GET /admin/statistics/driver-performance
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| top | int | 10 | Number of top drivers |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalDrivers": 1250,
    "activeDrivers": 980,
    "averageRating": 4.5,
    "averageOrdersPerDriver": 6.8,
    "topDrivers": [
      {
        "driverId": "driver-guid",
        "driverName": "أحمد محمد",
        "completedOrders": 342,
        "rating": 4.9,
        "onTimeRate": 98.2,
        "earnings": 51000.00
      }
    ],
    "ratingDistribution": [
      { "range": "4.5-5.0", "count": 500 },
      { "range": "4.0-4.5", "count": 350 },
      { "range": "3.5-4.0", "count": 100 },
      { "range": "Below 3.5", "count": 30 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.5 Customer Statistics

```
GET /admin/statistics/customers
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCustomers": 3200,
    "newCustomers": 450,
    "activeCustomers": 2100,
    "retentionRate": 72.5,
    "averageOrderFrequency": 3.2,
    "averageSpendPerCustomer": 480.00,
    "topCustomers": [
      {
        "customerId": "customer-guid",
        "customerName": "محمد علي",
        "totalOrders": 85,
        "totalSpent": 12500.00
      }
    ],
    "acquisitionTimeline": [
      { "period": "2026-03-01", "newCustomers": 15 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.6 Region Statistics

```
GET /admin/statistics/regions
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "regionId": 1,
      "regionName": "القاهرة",
      "totalOrders": 5200,
      "revenue": 780000.00,
      "activeDrivers": 120,
      "activeCustomers": 2000,
      "averageDeliveryTime": 25.0,
      "completionRate": 97.0
    },
    {
      "regionId": 2,
      "regionName": "الجيزة",
      "totalOrders": 2100,
      "revenue": 315000.00,
      "activeDrivers": 45,
      "activeCustomers": 800,
      "averageDeliveryTime": 32.0,
      "completionRate": 95.5
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 18.7 Financial Summary

```
GET /admin/statistics/financial-summary
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalRevenue": 1250000.00,
    "totalCommissions": 187500.00,
    "totalDeliveryFees": 212500.00,
    "totalRefunds": 42000.00,
    "totalSettlements": 850000.00,
    "netProfit": 358000.00,
    "subscriptionRevenue": 125000.00,
    "outstandingPayables": 95000.00,
    "outstandingReceivables": 45000.00
  },
  "message": null,
  "errors": null
}
```

---

### 18.8 KPI Dashboard

```
GET /admin/statistics/kpi
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| period | string | "month" | "week", "month", "quarter" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderCompletionRate": { "value": 96.5, "target": 95.0, "trend": 1.2 },
    "averageDeliveryTime": { "value": 28.5, "target": 30.0, "trend": -2.1 },
    "customerSatisfaction": { "value": 4.5, "target": 4.3, "trend": 0.1 },
    "driverUtilization": { "value": 78.0, "target": 80.0, "trend": 3.5 },
    "revenueGrowth": { "value": 12.3, "target": 10.0, "trend": 2.3 },
    "customerRetention": { "value": 72.5, "target": 70.0, "trend": 1.8 },
    "newUserAcquisition": { "value": 450, "target": 400, "trend": 12.5 },
    "refundRate": { "value": 3.2, "target": 5.0, "trend": -0.8 }
  },
  "message": null,
  "errors": null
}
```

---

### 18.9 Comparison Statistics

```
GET /admin/statistics/comparison
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| period1From | DateTime | required | First period start |
| period1To | DateTime | required | First period end |
| period2From | DateTime | required | Second period start |
| period2To | DateTime | required | Second period end |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "period1": {
      "label": "مارس 2026",
      "orders": 8500,
      "revenue": 1250000.00,
      "newCustomers": 450,
      "averageDeliveryTime": 28.5
    },
    "period2": {
      "label": "فبراير 2026",
      "orders": 7200,
      "revenue": 1080000.00,
      "newCustomers": 380,
      "averageDeliveryTime": 30.0
    },
    "changes": {
      "orders": { "absolute": 1300, "percentage": 18.1 },
      "revenue": { "absolute": 170000.00, "percentage": 15.7 },
      "newCustomers": { "absolute": 70, "percentage": 18.4 },
      "averageDeliveryTime": { "absolute": -1.5, "percentage": -5.0 }
    }
  },
  "message": null,
  "errors": null
}
```

---

### 18.10 Real-Time Statistics

```
GET /admin/statistics/realtime
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "activeOrders": 45,
    "pendingOrders": 12,
    "inTransitOrders": 33,
    "onlineDrivers": 95,
    "availableDrivers": 42,
    "busyDrivers": 53,
    "ordersPerMinute": 2.5,
    "averageWaitTime": 3.2,
    "lastUpdated": "2026-03-30T14:30:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 18.11 Partner Performance Statistics

```
GET /admin/statistics/partner-performance
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| top | int | 10 | Number of top partners |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalPartners": 180,
    "activePartners": 150,
    "topPartners": [
      {
        "partnerId": "partner-guid",
        "partnerName": "مطعم البيك",
        "totalOrders": 1200,
        "revenue": 450000.00,
        "rating": 4.7,
        "averagePrepTime": 15.0
      }
    ],
    "categoryBreakdown": [
      { "category": "Restaurants", "count": 80, "orders": 6000 },
      { "category": "Grocery", "count": 40, "orders": 1500 },
      { "category": "Pharmacy", "count": 20, "orders": 500 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.12 Subscription Statistics

```
GET /admin/statistics/subscriptions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalActive": 850,
    "monthlyRecurringRevenue": 125000.00,
    "churnRate": 5.2,
    "conversionRate": 35.0,
    "planDistribution": [
      { "plan": "Basic", "count": 500, "percentage": 58.8 },
      { "plan": "Premium", "count": 350, "percentage": 41.2 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.13 Hourly Heatmap

```
GET /admin/statistics/hourly-heatmap
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |
| regionId | int? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "heatmap": [
      { "dayOfWeek": 0, "hour": 12, "orderCount": 85, "intensity": 0.95 },
      { "dayOfWeek": 0, "hour": 13, "orderCount": 78, "intensity": 0.87 },
      { "dayOfWeek": 1, "hour": 12, "orderCount": 72, "intensity": 0.80 }
    ],
    "peakHour": 12,
    "peakDay": "Friday",
    "quietestHour": 4,
    "quietestDay": "Tuesday"
  },
  "message": null,
  "errors": null
}
```

---

### 18.14 Growth Metrics

```
GET /admin/statistics/growth
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| months | int | 6 | Number of months to analyze |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "monthlyGrowth": [
      {
        "month": "2026-03",
        "newDrivers": 120,
        "newCustomers": 450,
        "newPartners": 15,
        "orders": 8500,
        "revenue": 1250000.00
      }
    ],
    "overallGrowthRate": {
      "drivers": 15.2,
      "customers": 22.5,
      "orders": 18.1,
      "revenue": 15.7
    }
  },
  "message": null,
  "errors": null
}
```

---

### 18.15 Cancellation Analysis

```
GET /admin/statistics/cancellations
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCancellations": 300,
    "cancellationRate": 3.5,
    "byReason": [
      { "reason": "CustomerCancelled", "count": 120, "percentage": 40.0 },
      { "reason": "DriverCancelled", "count": 80, "percentage": 26.7 },
      { "reason": "NoDriverAvailable", "count": 60, "percentage": 20.0 },
      { "reason": "Other", "count": 40, "percentage": 13.3 }
    ],
    "byHour": [
      { "hour": 13, "count": 35 },
      { "hour": 19, "count": 30 }
    ],
    "trend": [
      { "date": "2026-03-25", "count": 12, "rate": 3.8 },
      { "date": "2026-03-26", "count": 10, "rate": 3.2 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.16 Delivery Time Analysis

```
GET /admin/statistics/delivery-times
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| regionId | int? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "averageDeliveryTime": 28.5,
    "medianDeliveryTime": 25.0,
    "percentile95": 45.0,
    "onTimeRate": 94.2,
    "distribution": [
      { "range": "0-15 min", "count": 1200, "percentage": 14.1 },
      { "range": "15-30 min", "count": 4500, "percentage": 52.9 },
      { "range": "30-45 min", "count": 2200, "percentage": 25.9 },
      { "range": "45+ min", "count": 600, "percentage": 7.1 }
    ],
    "byRegion": [
      { "region": "القاهرة", "average": 25.0 },
      { "region": "الجيزة", "average": 32.0 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.17 User Activity Statistics

```
GET /admin/statistics/user-activity
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dailyActiveUsers": 2500,
    "weeklyActiveUsers": 4200,
    "monthlyActiveUsers": 5800,
    "averageSessionDuration": 12.5,
    "peakActivityHour": 13,
    "deviceBreakdown": {
      "android": 65.0,
      "ios": 35.0
    },
    "timeline": [
      { "date": "2026-03-29", "activeUsers": 2450, "sessions": 5200 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.18 Payment Method Statistics

```
GET /admin/statistics/payment-methods
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "methods": [
      { "method": "Cash", "count": 5100, "amount": 765000.00, "percentage": 60.0 },
      { "method": "Wallet", "count": 2550, "amount": 382500.00, "percentage": 30.0 },
      { "method": "Card", "count": 850, "amount": 102500.00, "percentage": 10.0 }
    ],
    "trend": [
      {
        "period": "2026-03-01",
        "cash": 55.0,
        "wallet": 32.0,
        "card": 13.0
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.19 Wallet Flow Statistics

```
GET /admin/statistics/wallet-flow
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalDeposits": 850000.00,
    "totalWithdrawals": 720000.00,
    "netFlow": 130000.00,
    "totalTransactions": 15000,
    "averageDeposit": 250.00,
    "averageWithdrawal": 180.00,
    "timeline": [
      {
        "date": "2026-03-29",
        "deposits": 28000.00,
        "withdrawals": 24000.00,
        "netFlow": 4000.00
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 18.20 Platform Health

```
GET /admin/statistics/platform-health
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "systemUptime": 99.95,
    "apiResponseTime": 120,
    "errorRate": 0.05,
    "activeConnections": 1250,
    "signalRConnections": 890,
    "backgroundJobsHealth": "Healthy",
    "databaseResponseTime": 15,
    "cacheHitRate": 92.0,
    "lastIncident": "2026-03-15T10:00:00Z",
    "scheduledMaintenance": null
  },
  "message": null,
  "errors": null
}
```

---

## 19. Admin Notifications

> **Base**: `/api/v1/admin/notifications`

Send push notifications to users and segments.

---

### 19.1 Send Notification to User

```
POST /admin/notifications/send
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "userId": "user-guid",
  "title": "تحديث مهم",
  "body": "تم تحديث سياسة الخصوصية",
  "data": {
    "type": "announcement",
    "action": "open_settings"
  }
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | GUID | Yes | Target user |
| title | string | Yes | Notification title |
| body | string | Yes | Notification body |
| data | object | No | Custom data payload |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "notificationId": "notif-guid",
    "sentTo": 1,
    "deliveredTo": 1
  },
  "message": "تم إرسال الإشعار بنجاح",
  "errors": null
}
```

---

### 19.2 Send Bulk Notification

```
POST /admin/notifications/send-bulk
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "segmentId": "segment-guid",
  "title": "عرض خاص",
  "body": "خصم 20% على جميع الطلبات اليوم!",
  "data": {
    "type": "promotion",
    "promoCode": "EID20"
  },
  "scheduledAt": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| segmentId | GUID? | No | Target segment (null = all users) |
| title | string | Yes | Notification title |
| body | string | Yes | Notification body |
| data | object | No | Custom data payload |
| scheduledAt | DateTime? | No | Schedule for later |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "notificationId": "notif-guid",
    "targetCount": 3200,
    "sentCount": 3180,
    "scheduledAt": null
  },
  "message": "تم إرسال الإشعارات بنجاح",
  "errors": null
}
```

---

### 19.3 Get Notification History

```
GET /admin/notifications
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| type | string | null | "single", "bulk" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "notif-guid",
        "title": "عرض خاص",
        "body": "خصم 20% على جميع الطلبات اليوم!",
        "type": "bulk",
        "targetCount": 3200,
        "sentCount": 3180,
        "deliveredCount": 3050,
        "openedCount": 1200,
        "sentBy": "Admin (سارة أحمد)",
        "sentAt": "2026-03-30T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 85,
    "totalPages": 9,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

## 20. Admin SOS

> **Base**: `/api/v1/admin/sos`

Manage emergency SOS alerts from drivers.

---

### 20.1 List SOS Alerts

```
GET /admin/sos
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by SOSStatus |
| priority | int? | null | Filter by priority |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "sos-guid",
        "driverId": "driver-guid",
        "driverName": "أحمد محمد",
        "driverPhone": "+201012345678",
        "type": 1,
        "typeName": "Accident",
        "priority": 3,
        "priorityName": "Critical",
        "status": 0,
        "statusName": "Active",
        "latitude": 30.0444,
        "longitude": 31.2357,
        "message": "حادث على الطريق الدائري",
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 5,
    "totalPages": 1,
    "hasNext": false,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 20.2 Get SOS Alert Details

```
GET /admin/sos/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "sos-guid",
    "driverId": "driver-guid",
    "driverName": "أحمد محمد",
    "driverPhone": "+201012345678",
    "type": 1,
    "typeName": "Accident",
    "priority": 3,
    "priorityName": "Critical",
    "status": 0,
    "statusName": "Active",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "message": "حادث على الطريق الدائري",
    "audioUrl": "https://storage.sekka.com/sos/audio1.mp3",
    "imagesUrls": ["https://storage.sekka.com/sos/img1.jpg"],
    "activeOrderId": "order-guid",
    "assignedTo": null,
    "actions": [],
    "createdAt": "2026-03-30T14:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 20.3 Acknowledge SOS

```
PUT /admin/sos/{id}/acknowledge
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم استلام البلاغ",
  "errors": null
}
```

---

### 20.4 Assign SOS to Admin

```
PUT /admin/sos/{id}/assign
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Request Body**:
```json
{
  "adminId": "admin-guid"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعيين البلاغ بنجاح",
  "errors": null
}
```

---

### 20.5 Add SOS Action

```
POST /admin/sos/{id}/actions
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Request Body**:
```json
{
  "action": "تم التواصل مع السائق والاطمئنان عليه",
  "type": "PhoneCall"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| action | string | Yes | Action description |
| type | string | Yes | "PhoneCall", "Dispatch", "Escalation", "Note" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الإجراء بنجاح",
  "errors": null
}
```

---

### 20.6 Resolve SOS

```
PUT /admin/sos/{id}/resolve
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Request Body**:
```json
{
  "resolution": "تم إرسال سيارة إسعاف والسائق بخير",
  "notes": "حادث بسيط - لا إصابات"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حل البلاغ بنجاح",
  "errors": null
}
```

---

### 20.7 Escalate SOS

```
PUT /admin/sos/{id}/escalate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS ID

**Request Body**:
```json
{
  "reason": "الحالة تتطلب تدخل الشرطة",
  "newPriority": 3
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تصعيد البلاغ بنجاح",
  "errors": null
}
```

---

### 20.8 Get Active SOS Count

```
GET /admin/sos/active-count
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "total": 5,
    "critical": 1,
    "high": 2,
    "medium": 1,
    "low": 1
  },
  "message": null,
  "errors": null
}
```

---

### 20.9 Get SOS History for Driver

```
GET /admin/sos/driver/{driverId}
```

**Auth Required**: Yes (Admin)

**URL Params**: `driverId` (GUID) - Driver ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "sos-guid",
      "type": 1,
      "typeName": "Accident",
      "status": 2,
      "statusName": "Resolved",
      "createdAt": "2026-03-30T14:00:00Z",
      "resolvedAt": "2026-03-30T14:30:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 20.10 SOS Statistics

```
GET /admin/sos/stats
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalAlerts": 45,
    "resolvedAlerts": 38,
    "averageResolutionMinutes": 22.5,
    "byType": [
      { "type": "Accident", "count": 15 },
      { "type": "Breakdown", "count": 12 },
      { "type": "Threat", "count": 8 },
      { "type": "Medical", "count": 5 },
      { "type": "Other", "count": 5 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 21. Admin Vehicles

> **Base**: `/api/v1/admin/vehicles`

Manage vehicle registry, inspections, and fleet operations.

---

### 21.1 List Vehicles

```
GET /admin/vehicles
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| type | int? | null | Filter by VehicleType |
| status | int? | null | Filter by VehicleStatus |
| search | string | null | Search by plate/driver |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "vehicle-guid",
        "driverId": "driver-guid",
        "driverName": "أحمد محمد",
        "type": 0,
        "typeName": "Motorcycle",
        "plateNumber": "م ر ع 1234",
        "make": "Honda",
        "model": "PCX 150",
        "year": 2025,
        "color": "أسود",
        "status": 1,
        "statusName": "Active",
        "lastInspectionDate": "2026-03-01T00:00:00Z",
        "nextInspectionDate": "2026-06-01T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1250,
    "totalPages": 125,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 21.2 Get Vehicle Details

```
GET /admin/vehicles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "vehicle-guid",
    "driverId": "driver-guid",
    "driverName": "أحمد محمد",
    "type": 0,
    "typeName": "Motorcycle",
    "plateNumber": "م ر ع 1234",
    "make": "Honda",
    "model": "PCX 150",
    "year": 2025,
    "color": "أسود",
    "status": 1,
    "statusName": "Active",
    "licenseImageUrl": "https://storage.sekka.com/vehicles/license.jpg",
    "insuranceExpiry": "2027-01-01T00:00:00Z",
    "lastInspectionDate": "2026-03-01T00:00:00Z",
    "nextInspectionDate": "2026-06-01T00:00:00Z",
    "inspectionHistory": [
      {
        "date": "2026-03-01T00:00:00Z",
        "result": "Passed",
        "inspector": "Admin (محمد سعيد)",
        "notes": "جميع الفحوصات سليمة"
      }
    ],
    "createdAt": "2026-01-15T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 21.3 Create Vehicle

```
POST /admin/vehicles
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "driverId": "driver-guid",
  "type": 0,
  "plateNumber": "م ر ع 1234",
  "make": "Honda",
  "model": "PCX 150",
  "year": 2025,
  "color": "أسود"
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-vehicle-guid",
    "plateNumber": "م ر ع 1234"
  },
  "message": "تم تسجيل المركبة بنجاح",
  "errors": null
}
```

---

### 21.4 Update Vehicle

```
PUT /admin/vehicles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "plateNumber": "م ر ع 5678",
  "make": "Honda",
  "model": "PCX 160",
  "year": 2026,
  "color": "أبيض"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل بيانات المركبة بنجاح",
  "errors": null
}
```

---

### 21.5 Delete Vehicle

```
DELETE /admin/vehicles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف المركبة بنجاح",
  "errors": null
}
```

---

### 21.6 Activate Vehicle

```
PUT /admin/vehicles/{id}/activate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل المركبة بنجاح",
  "errors": null
}
```

---

### 21.7 Deactivate Vehicle

```
PUT /admin/vehicles/{id}/deactivate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تفعيل المركبة بنجاح",
  "errors": null
}
```

---

### 21.8 Record Vehicle Inspection

```
POST /admin/vehicles/{id}/inspect
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "result": "Passed",
  "notes": "جميع الفحوصات سليمة",
  "nextInspectionDate": "2026-09-01T00:00:00Z"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| result | string | Yes | "Passed", "Failed", "ConditionalPass" |
| notes | string | No | Inspection notes |
| nextInspectionDate | DateTime | Yes | Next scheduled inspection |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الفحص بنجاح",
  "errors": null
}
```

---

### 21.9 Get Vehicles Due for Inspection

```
GET /admin/vehicles/due-inspection
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| days | int | 30 | Days until due |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "vehicle-guid",
        "driverName": "أحمد محمد",
        "plateNumber": "م ر ع 1234",
        "type": "Motorcycle",
        "nextInspectionDate": "2026-04-10T00:00:00Z",
        "daysUntilDue": 11
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 35,
    "totalPages": 4,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 21.10 Get Vehicle Statistics

```
GET /admin/vehicles/stats
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalVehicles": 1250,
    "activeVehicles": 1100,
    "inactiveVehicles": 150,
    "byType": [
      { "type": "Motorcycle", "count": 800 },
      { "type": "Car", "count": 300 },
      { "type": "Van", "count": 100 },
      { "type": "Truck", "count": 40 },
      { "type": "Bicycle", "count": 10 }
    ],
    "inspectionsDue": 35,
    "failedInspections": 8,
    "averageVehicleAge": 2.5
  },
  "message": null,
  "errors": null
}
```

---

### 21.11 Transfer Vehicle to Another Driver

```
PUT /admin/vehicles/{id}/transfer
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "newDriverId": "new-driver-guid",
  "reason": "نقل المركبة بعد استقالة السائق"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "vehicleId": "vehicle-guid",
    "oldDriverName": "أحمد محمد",
    "newDriverName": "محمود حسن"
  },
  "message": "تم نقل المركبة بنجاح",
  "errors": null
}
```

---

## 22. Admin Config

> **Base**: `/api/v1/admin/config`

Platform-wide configuration management: settings, versioning, maintenance mode, feature flags, and commissions.

---

### 22.1 Get All Settings

```
GET /admin/config/settings
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "key": "max_orders_per_driver",
      "value": "10",
      "type": "int",
      "category": "orders",
      "description": "الحد الأقصى للطلبات لكل سائق",
      "lastModifiedAt": "2026-03-15T00:00:00Z",
      "lastModifiedBy": "Admin"
    },
    {
      "key": "base_delivery_fee",
      "value": "15.00",
      "type": "decimal",
      "category": "pricing",
      "description": "رسوم التوصيل الأساسية",
      "lastModifiedAt": "2026-03-01T00:00:00Z",
      "lastModifiedBy": "Admin"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 22.2 Get Setting by Key

```
GET /admin/config/settings/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Setting key

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "key": "max_orders_per_driver",
    "value": "10",
    "type": "int",
    "category": "orders",
    "description": "الحد الأقصى للطلبات لكل سائق"
  },
  "message": null,
  "errors": null
}
```

---

### 22.3 Update Setting

```
PUT /admin/config/settings/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Setting key

**Request Body**:
```json
{
  "value": "15"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الإعداد بنجاح",
  "errors": null
}
```

---

### 22.4 Create Setting

```
POST /admin/config/settings
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "key": "new_setting_key",
  "value": "some_value",
  "type": "string",
  "category": "general",
  "description": "وصف الإعداد"
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إنشاء الإعداد بنجاح",
  "errors": null
}
```

---

### 22.5 Delete Setting

```
DELETE /admin/config/settings/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Setting key

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الإعداد بنجاح",
  "errors": null
}
```

---

### 22.6 Get Settings by Category

```
GET /admin/config/settings/category/{category}
```

**Auth Required**: Yes (Admin)

**URL Params**: `category` (string) - Category name

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "key": "base_delivery_fee",
      "value": "15.00",
      "type": "decimal",
      "category": "pricing",
      "description": "رسوم التوصيل الأساسية"
    },
    {
      "key": "per_km_rate",
      "value": "3.50",
      "type": "decimal",
      "category": "pricing",
      "description": "سعر الكيلومتر"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 22.7 Get App Versions

```
GET /admin/config/versions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "platform": "Android",
      "currentVersion": "2.5.0",
      "minimumVersion": "2.0.0",
      "updateUrl": "https://play.google.com/store/apps/details?id=com.sekka",
      "forceUpdate": false,
      "releaseNotes": "تحسينات الأداء وإصلاح أخطاء",
      "releasedAt": "2026-03-25T00:00:00Z"
    },
    {
      "id": 2,
      "platform": "iOS",
      "currentVersion": "2.5.0",
      "minimumVersion": "2.0.0",
      "updateUrl": "https://apps.apple.com/app/sekka/id123456",
      "forceUpdate": false,
      "releaseNotes": "تحسينات الأداء وإصلاح أخطاء",
      "releasedAt": "2026-03-25T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 22.8 Update App Version

```
PUT /admin/config/versions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Version config ID

**Request Body**:
```json
{
  "currentVersion": "2.6.0",
  "minimumVersion": "2.2.0",
  "forceUpdate": true,
  "releaseNotes": "تحديث أمني مهم - يرجى التحديث فوراً"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل إعدادات الإصدار بنجاح",
  "errors": null
}
```

---

### 22.9 Get Maintenance Status

```
GET /admin/config/maintenance
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "isEnabled": false,
    "message": null,
    "messageAr": null,
    "scheduledStart": null,
    "scheduledEnd": null,
    "allowedIPs": []
  },
  "message": null,
  "errors": null
}
```

---

### 22.10 Enable Maintenance Mode

```
POST /admin/config/maintenance/enable
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "message": "System maintenance in progress",
  "messageAr": "جاري صيانة النظام",
  "scheduledEnd": "2026-03-31T02:00:00Z",
  "allowedIPs": ["192.168.1.1"]
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل وضع الصيانة",
  "errors": null
}
```

---

### 22.11 Disable Maintenance Mode

```
POST /admin/config/maintenance/disable
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء وضع الصيانة",
  "errors": null
}
```

---

### 22.12 Get Feature Flags

```
GET /admin/config/feature-flags
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "key": "savings_circles",
      "isEnabled": true,
      "description": "تفعيل دوائر التوفير",
      "rolloutPercentage": 100,
      "lastModifiedAt": "2026-03-20T00:00:00Z"
    },
    {
      "key": "auto_assign_v2",
      "isEnabled": false,
      "description": "خوارزمية التعيين التلقائي الجديدة",
      "rolloutPercentage": 25,
      "lastModifiedAt": "2026-03-28T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 22.13 Create Feature Flag

```
POST /admin/config/feature-flags
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "key": "new_feature",
  "isEnabled": false,
  "description": "ميزة جديدة قيد التطوير",
  "rolloutPercentage": 0
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إنشاء علم الميزة بنجاح",
  "errors": null
}
```

---

### 22.14 Update Feature Flag

```
PUT /admin/config/feature-flags/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Feature flag key

**Request Body**:
```json
{
  "isEnabled": true,
  "rolloutPercentage": 50,
  "description": "ميزة قيد الاختبار التدريجي"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل علم الميزة بنجاح",
  "errors": null
}
```

---

### 22.15 Toggle Feature Flag

```
PUT /admin/config/feature-flags/{key}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Feature flag key

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "key": "auto_assign_v2",
    "isEnabled": true
  },
  "message": "تم تغيير حالة الميزة بنجاح",
  "errors": null
}
```

---

### 22.16 Delete Feature Flag

```
DELETE /admin/config/feature-flags/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Feature flag key

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف علم الميزة بنجاح",
  "errors": null
}
```

---

### 22.17 Get Commission Settings

```
GET /admin/config/commissions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Standard Partner Commission",
      "type": "partner",
      "rate": 15.0,
      "isPercentage": true,
      "minAmount": 5.00,
      "maxAmount": null,
      "isActive": true
    },
    {
      "id": 2,
      "name": "Driver Platform Fee",
      "type": "driver",
      "rate": 10.0,
      "isPercentage": true,
      "minAmount": 2.00,
      "maxAmount": 50.00,
      "isActive": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 22.18 Create Commission Rule

```
POST /admin/config/commissions
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "Premium Partner Commission",
  "type": "partner",
  "rate": 12.0,
  "isPercentage": true,
  "minAmount": 3.00,
  "maxAmount": null
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Premium Partner Commission"
  },
  "message": "تم إنشاء قاعدة العمولة بنجاح",
  "errors": null
}
```

---

### 22.19 Update Commission Rule

```
PUT /admin/config/commissions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Commission rule ID

**Request Body**:
```json
{
  "name": "Premium Partner Commission (Updated)",
  "rate": 10.0,
  "minAmount": 2.00,
  "maxAmount": 100.00
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل قاعدة العمولة بنجاح",
  "errors": null
}
```

---

### 22.20 Toggle Commission Rule

```
PUT /admin/config/commissions/{id}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Commission rule ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "isActive": false
  },
  "message": "تم تغيير حالة قاعدة العمولة بنجاح",
  "errors": null
}
```

---

### 22.21 Delete Commission Rule

```
DELETE /admin/config/commissions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Commission rule ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف قاعدة العمولة بنجاح",
  "errors": null
}
```

---

## 23. Admin Regions

> **Base**: `/api/v1/admin/regions`

Manage delivery regions/zones.

---

### 23.1 List Regions

```
GET /admin/regions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "القاهرة",
      "nameEn": "Cairo",
      "isActive": true,
      "driversCount": 120,
      "ordersCount": 5200,
      "deliveryFeeMultiplier": 1.0,
      "boundaries": {
        "type": "Polygon",
        "coordinates": [[[31.2, 30.0], [31.4, 30.0], [31.4, 30.1], [31.2, 30.1], [31.2, 30.0]]]
      }
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 23.2 Create Region

```
POST /admin/regions
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "الإسكندرية",
  "nameEn": "Alexandria",
  "deliveryFeeMultiplier": 1.2,
  "boundaries": {
    "type": "Polygon",
    "coordinates": [[[29.9, 31.1], [29.9, 31.3], [30.0, 31.3], [30.0, 31.1], [29.9, 31.1]]]
  }
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "الإسكندرية"
  },
  "message": "تم إنشاء المنطقة بنجاح",
  "errors": null
}
```

---

### 23.3 Update Region

```
PUT /admin/regions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Region ID

**Request Body**:
```json
{
  "name": "الإسكندرية - محدّثة",
  "nameEn": "Alexandria (Updated)",
  "deliveryFeeMultiplier": 1.3,
  "isActive": true
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل المنطقة بنجاح",
  "errors": null
}
```

---

### 23.4 Delete Region

```
DELETE /admin/regions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (int) - Region ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف المنطقة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن حذف منطقة بها طلبات نشطة` |

---

## 24. Admin Audit Logs

> **Base**: `/api/v1/admin/audit-logs`

View system audit trail for all administrative actions.

---

### 24.1 List Audit Logs

```
GET /admin/audit-logs
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| action | string | null | Filter by action type |
| userId | GUID? | null | Filter by acting user |
| entityType | string | null | Filter by entity type |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "log-guid",
        "action": "DriverActivated",
        "entityType": "Driver",
        "entityId": "driver-guid",
        "userId": "admin-guid",
        "userName": "سارة أحمد",
        "ipAddress": "192.168.1.1",
        "oldValues": { "status": 0 },
        "newValues": { "status": 1 },
        "description": "تم تفعيل السائق أحمد محمد",
        "createdAt": "2026-03-30T15:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 5000,
    "totalPages": 250,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 24.2 Get Audit Log Details

```
GET /admin/audit-logs/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Log ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "log-guid",
    "action": "DriverActivated",
    "entityType": "Driver",
    "entityId": "driver-guid",
    "userId": "admin-guid",
    "userName": "سارة أحمد",
    "ipAddress": "192.168.1.1",
    "userAgent": "Mozilla/5.0...",
    "oldValues": { "status": 0, "statusName": "Inactive" },
    "newValues": { "status": 1, "statusName": "Active" },
    "description": "تم تفعيل السائق أحمد محمد",
    "metadata": { "reason": "اكتمال التحقق من المستندات" },
    "createdAt": "2026-03-30T15:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 24.3 Get Audit Logs for Entity

```
GET /admin/audit-logs/entity/{entityType}/{entityId}
```

**Auth Required**: Yes (Admin)

**URL Params**:
- `entityType` (string) - Entity type (e.g., "Driver", "Order")
- `entityId` (GUID) - Entity ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "log-guid",
        "action": "DriverActivated",
        "userName": "سارة أحمد",
        "description": "تم تفعيل السائق",
        "createdAt": "2026-03-30T15:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 12,
    "totalPages": 1,
    "hasNext": false,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 24.4 Get Audit Logs for User

```
GET /admin/audit-logs/user/{userId}
```

**Auth Required**: Yes (Admin)

**URL Params**: `userId` (GUID) - Admin/User ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "log-guid",
        "action": "DriverActivated",
        "entityType": "Driver",
        "entityId": "driver-guid",
        "description": "تم تفعيل السائق أحمد محمد",
        "createdAt": "2026-03-30T15:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 250,
    "totalPages": 13,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 24.5 Get Available Actions

```
GET /admin/audit-logs/actions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    "DriverActivated",
    "DriverDeactivated",
    "OrderCreated",
    "OrderAssigned",
    "OrderStatusOverridden",
    "WalletAdjusted",
    "WalletFrozen",
    "SettingUpdated",
    "RoleAssigned",
    "RoleRevoked",
    "BlacklistAdded",
    "BlacklistRemoved",
    "SubscriptionExtended",
    "SubscriptionCancelled",
    "RefundApproved",
    "RefundRejected",
    "MaintenanceEnabled",
    "MaintenanceDisabled",
    "FeatureFlagToggled"
  ],
  "message": null,
  "errors": null
}
```

---

### 24.6 Export Audit Logs

```
GET /admin/audit-logs/export
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime | required | Start date |
| to | DateTime | required | End date |
| action | string | null | Filter by action |
| format | string | "csv" | Export format: csv, xlsx |

**Response** `200 OK`:
Returns file download.

---

## 25. Admin Segments

> **Base**: `/api/v1/admin/segments`

Create and manage user segments for targeted notifications and campaigns.

---

### 25.1 List Segments

```
GET /admin/segments
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| type | string | null | "driver", "customer", "partner" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "segment-guid",
        "name": "سائقين مميزين",
        "description": "سائقين تقييمهم أعلى من 4.5 وأكملوا أكثر من 100 طلب",
        "type": "driver",
        "usersCount": 250,
        "isActive": true,
        "isDynamic": true,
        "createdAt": "2026-03-01T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 15,
    "totalPages": 2,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 25.2 Get Segment Details

```
GET /admin/segments/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "segment-guid",
    "name": "سائقين مميزين",
    "description": "سائقين تقييمهم أعلى من 4.5 وأكملوا أكثر من 100 طلب",
    "type": "driver",
    "usersCount": 250,
    "isActive": true,
    "isDynamic": true,
    "rules": [
      { "field": "rating", "operator": "greaterThan", "value": "4.5" },
      { "field": "completedOrders", "operator": "greaterThan", "value": "100" }
    ],
    "createdAt": "2026-03-01T00:00:00Z",
    "lastEvaluatedAt": "2026-03-30T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 25.3 Create Segment

```
POST /admin/segments
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "عملاء جدد",
  "description": "عملاء سجلوا في آخر 30 يوم",
  "type": "customer",
  "isDynamic": true,
  "rules": [
    { "field": "joinedAt", "operator": "after", "value": "2026-03-01" }
  ]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Segment name |
| description | string | No | Description |
| type | string | Yes | "driver", "customer", "partner" |
| isDynamic | bool | Yes | Auto-update membership |
| rules | array | Yes for dynamic | Filter rules |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-segment-guid",
    "name": "عملاء جدد",
    "usersCount": 450
  },
  "message": "تم إنشاء الشريحة بنجاح",
  "errors": null
}
```

---

### 25.4 Update Segment

```
PUT /admin/segments/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Request Body**:
```json
{
  "name": "عملاء جدد (محدّث)",
  "description": "عملاء سجلوا في آخر 15 يوم",
  "rules": [
    { "field": "joinedAt", "operator": "after", "value": "2026-03-15" }
  ]
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الشريحة بنجاح",
  "errors": null
}
```

---

### 25.5 Delete Segment

```
DELETE /admin/segments/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الشريحة بنجاح",
  "errors": null
}
```

---

### 25.6 Get Segment Users

```
GET /admin/segments/{id}/users
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "userId": "user-guid",
        "name": "محمد علي",
        "phone": "+201112345678",
        "joinedSegmentAt": "2026-03-15T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 450,
    "totalPages": 45,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 25.7 Add Users to Segment

```
POST /admin/segments/{id}/users
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Request Body**:
```json
{
  "userIds": ["user-guid-1", "user-guid-2", "user-guid-3"]
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "addedCount": 3,
    "alreadyExistedCount": 0
  },
  "message": "تم إضافة المستخدمين للشريحة بنجاح",
  "errors": null
}
```

---

### 25.8 Remove Users from Segment

```
DELETE /admin/segments/{id}/users
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Request Body**:
```json
{
  "userIds": ["user-guid-1"]
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "removedCount": 1
  },
  "message": "تم إزالة المستخدمين من الشريحة بنجاح",
  "errors": null
}
```

---

### 25.9 Refresh Segment (Re-evaluate Rules)

```
POST /admin/segments/{id}/refresh
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "previousCount": 450,
    "newCount": 475,
    "added": 30,
    "removed": 5
  },
  "message": "تم تحديث الشريحة بنجاح",
  "errors": null
}
```

---

### 25.10 Toggle Segment

```
PUT /admin/segments/{id}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Segment ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "segment-guid",
    "isActive": false
  },
  "message": "تم تغيير حالة الشريحة بنجاح",
  "errors": null
}
```

---

## 26. Admin Campaigns

> **Base**: `/api/v1/admin/campaigns`

Create and manage promotional campaigns and discount codes.

---

### 26.1 List Campaigns

```
GET /admin/campaigns
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by CampaignStatus |
| type | int? | null | Filter by campaign type |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "campaign-guid",
        "name": "خصم عيد الفطر",
        "type": 1,
        "typeName": "Discount",
        "status": 1,
        "statusName": "Active",
        "promoCode": "EID2026",
        "discountType": "Percentage",
        "discountValue": 20.0,
        "maxUsage": 1000,
        "currentUsage": 450,
        "startDate": "2026-03-28T00:00:00Z",
        "endDate": "2026-04-05T00:00:00Z",
        "segmentId": "segment-guid",
        "segmentName": "جميع العملاء",
        "createdAt": "2026-03-25T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 12,
    "totalPages": 2,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 26.2 Get Campaign Details

```
GET /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "campaign-guid",
    "name": "خصم عيد الفطر",
    "description": "خصم 20% على جميع الطلبات بمناسبة عيد الفطر",
    "type": 1,
    "typeName": "Discount",
    "status": 1,
    "statusName": "Active",
    "promoCode": "EID2026",
    "discountType": "Percentage",
    "discountValue": 20.0,
    "maxDiscount": 50.00,
    "minOrderAmount": 100.00,
    "maxUsage": 1000,
    "maxUsagePerUser": 3,
    "currentUsage": 450,
    "totalDiscountGiven": 9500.00,
    "startDate": "2026-03-28T00:00:00Z",
    "endDate": "2026-04-05T00:00:00Z",
    "segmentId": "segment-guid",
    "segmentName": "جميع العملاء",
    "applicableRegions": [1, 2],
    "createdAt": "2026-03-25T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 26.3 Create Campaign

```
POST /admin/campaigns
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "خصم الصيف",
  "description": "خصم 15% بمناسبة بداية الصيف",
  "type": 1,
  "promoCode": "SUMMER2026",
  "discountType": "Percentage",
  "discountValue": 15.0,
  "maxDiscount": 40.00,
  "minOrderAmount": 80.00,
  "maxUsage": 2000,
  "maxUsagePerUser": 5,
  "startDate": "2026-06-01T00:00:00Z",
  "endDate": "2026-06-30T00:00:00Z",
  "segmentId": null,
  "applicableRegions": []
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Campaign name |
| description | string | No | Description |
| type | int | Yes | CampaignType enum |
| promoCode | string | No | Promo code (unique) |
| discountType | string | Yes | "Percentage" or "Fixed" |
| discountValue | decimal | Yes | Discount amount/percentage |
| maxDiscount | decimal? | No | Max discount cap (for percentage) |
| minOrderAmount | decimal? | No | Minimum order amount |
| maxUsage | int? | No | Total usage limit |
| maxUsagePerUser | int? | No | Per-user usage limit |
| startDate | DateTime | Yes | Campaign start |
| endDate | DateTime | Yes | Campaign end |
| segmentId | GUID? | No | Target segment (null = all) |
| applicableRegions | int[] | No | Region IDs (empty = all) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-campaign-guid",
    "name": "خصم الصيف",
    "promoCode": "SUMMER2026"
  },
  "message": "تم إنشاء الحملة بنجاح",
  "errors": null
}
```

---

### 26.4 Update Campaign

```
PUT /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Request Body**: Same as create.

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الحملة بنجاح",
  "errors": null
}
```

---

### 26.5 Delete Campaign

```
DELETE /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الحملة بنجاح",
  "errors": null
}
```

---

### 26.6 Activate Campaign

```
PUT /admin/campaigns/{id}/activate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل الحملة بنجاح",
  "errors": null
}
```

---

### 26.7 Pause Campaign

```
PUT /admin/campaigns/{id}/pause
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إيقاف الحملة مؤقتاً",
  "errors": null
}
```

---

### 26.8 Get Campaign Usage

```
GET /admin/campaigns/{id}/usage
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "userId": "customer-guid",
        "userName": "محمد علي",
        "orderId": "order-guid",
        "orderNumber": "ORD-20260329-001",
        "discountAmount": 30.00,
        "usedAt": "2026-03-29T14:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 450,
    "totalPages": 45,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 26.9 Campaign Statistics

```
GET /admin/campaigns/{id}/stats
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Campaign ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalUsage": 450,
    "uniqueUsers": 380,
    "totalDiscountGiven": 9500.00,
    "averageDiscount": 21.11,
    "averageOrderValue": 165.00,
    "conversionRate": 25.0,
    "dailyUsage": [
      { "date": "2026-03-28", "usage": 120 },
      { "date": "2026-03-29", "usage": 180 },
      { "date": "2026-03-30", "usage": 150 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 26.10 Validate Promo Code

```
GET /admin/campaigns/validate/{code}
```

**Auth Required**: Yes (Admin)

**URL Params**: `code` (string) - Promo code

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "isValid": true,
    "campaignId": "campaign-guid",
    "campaignName": "خصم عيد الفطر",
    "discountType": "Percentage",
    "discountValue": 20.0,
    "maxDiscount": 50.00,
    "remainingUsage": 550,
    "expiresAt": "2026-04-05T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

## 27. Admin Insights

> **Base**: `/api/v1/admin/insights`

AI-driven analytics and business insights.

---

### 27.1 Get Business Insights

```
GET /admin/insights
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| category | string | null | "growth", "retention", "operations", "finance" |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "insight-guid",
      "title": "انخفاض معدل الاحتفاظ بالعملاء",
      "description": "انخفض معدل الاحتفاظ بنسبة 5% مقارنة بالشهر السابق. يوصى بإطلاق حملة ترويجية.",
      "category": "retention",
      "severity": "warning",
      "metric": "customerRetention",
      "currentValue": 67.5,
      "previousValue": 72.5,
      "change": -5.0,
      "recommendations": [
        "إطلاق حملة خصومات للعملاء غير النشطين",
        "إرسال إشعارات تذكيرية مخصصة"
      ],
      "generatedAt": "2026-03-30T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 27.2 Get Demand Forecast

```
GET /admin/insights/demand-forecast
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| days | int | 7 | Forecast days ahead |
| regionId | int? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "forecast": [
      {
        "date": "2026-03-31",
        "expectedOrders": 290,
        "confidence": 0.85,
        "peakHours": [12, 13, 19, 20],
        "recommendedDrivers": 100
      },
      {
        "date": "2026-04-01",
        "expectedOrders": 310,
        "confidence": 0.82,
        "peakHours": [12, 13, 14, 19, 20],
        "recommendedDrivers": 110
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 27.3 Get Churn Risk Analysis

```
GET /admin/insights/churn-risk
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| userType | string | "customer" | "customer" or "driver" |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "userId": "customer-guid",
        "userName": "عمرو خالد",
        "userPhone": "+201212345678",
        "riskScore": 85,
        "riskLevel": "High",
        "lastActivityAt": "2026-03-10T00:00:00Z",
        "daysSinceLastOrder": 20,
        "previousOrderFrequency": 5.2,
        "factors": [
          "لم يطلب منذ 20 يوم",
          "تقييم آخر طلب كان منخفض",
          "شكوى مفتوحة لم تُحل"
        ]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 120,
    "totalPages": 12,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 27.4 Get Revenue Optimization Suggestions

```
GET /admin/insights/revenue-optimization
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "suggestions": [
      {
        "title": "زيادة رسوم التوصيل في ساعات الذروة",
        "description": "تطبيق رسوم إضافية 20% خلال الفترة 12-2 ظهراً يمكن أن يزيد الإيرادات بنسبة 8%",
        "estimatedImpact": 100000.00,
        "category": "pricing",
        "priority": "high"
      },
      {
        "title": "توسيع منطقة التغطية لمدينة 6 أكتوبر",
        "description": "هناك طلب غير مُلبى في هذه المنطقة بناءً على عمليات البحث",
        "estimatedImpact": 75000.00,
        "category": "expansion",
        "priority": "medium"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 27.5 Get Driver Supply-Demand Analysis

```
GET /admin/insights/supply-demand
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| regionId | int? | null | Filter by region |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "currentStatus": {
      "onlineDrivers": 95,
      "activeOrders": 45,
      "pendingOrders": 12,
      "ratio": 2.11,
      "status": "Balanced"
    },
    "hourlyForecast": [
      { "hour": 15, "expectedDemand": 55, "expectedSupply": 90, "status": "Surplus" },
      { "hour": 19, "expectedDemand": 120, "expectedSupply": 80, "status": "Shortage" }
    ],
    "recommendations": [
      "إرسال إشعار للسائقين غير النشطين للعودة بحلول الساعة 7 مساءً"
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 27.6 Get Anomaly Detection

```
GET /admin/insights/anomalies
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| from | DateTime? | null | Start date |
| to | DateTime? | null | End date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "anomaly-guid",
      "type": "UnusualOrderVolume",
      "severity": "high",
      "description": "حجم الطلبات أعلى بنسبة 40% من المتوسط في منطقة المعادي",
      "detectedAt": "2026-03-30T13:00:00Z",
      "metric": "orderVolume",
      "expectedValue": 50,
      "actualValue": 70,
      "region": "المعادي",
      "isAcknowledged": false
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 27.7 Get Cohort Analysis

```
GET /admin/insights/cohort-analysis
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| userType | string | "customer" | "customer" or "driver" |
| months | int | 6 | Number of cohort months |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "cohorts": [
      {
        "cohort": "2026-01",
        "initialUsers": 200,
        "retention": [100.0, 75.0, 62.0, 55.0, 50.0, 48.0]
      },
      {
        "cohort": "2026-02",
        "initialUsers": 250,
        "retention": [100.0, 78.0, 65.0, 58.0, 52.0]
      },
      {
        "cohort": "2026-03",
        "initialUsers": 300,
        "retention": [100.0, 80.0, 68.0]
      }
    ],
    "averageRetention": [100.0, 77.7, 65.0, 56.5, 51.0, 48.0]
  },
  "message": null,
  "errors": null
}
```

---

## 28. Admin Savings Circles

> **Base**: `/api/v1/admin/savings-circles`

Manage driver savings circles (gamified group savings feature).

---

### 28.1 List Savings Circles

```
GET /admin/savings-circles
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |
| status | int? | null | Filter by CircleStatus |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "circle-guid",
        "name": "دائرة السائقين المميزين",
        "status": 1,
        "statusName": "Active",
        "membersCount": 10,
        "maxMembers": 10,
        "contributionAmount": 500.00,
        "cycleDays": 30,
        "currentCycle": 3,
        "totalCycles": 10,
        "totalPool": 5000.00,
        "nextPayoutDate": "2026-04-01T00:00:00Z",
        "nextPayoutMember": "أحمد محمد",
        "createdAt": "2026-01-01T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 28.2 Get Savings Circle Details

```
GET /admin/savings-circles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "circle-guid",
    "name": "دائرة السائقين المميزين",
    "description": "دائرة توفير شهرية للسائقين",
    "status": 1,
    "statusName": "Active",
    "creatorId": "driver-guid",
    "creatorName": "أحمد محمد",
    "membersCount": 10,
    "maxMembers": 10,
    "contributionAmount": 500.00,
    "cycleDays": 30,
    "currentCycle": 3,
    "totalCycles": 10,
    "totalPool": 5000.00,
    "members": [
      {
        "driverId": "driver-guid-1",
        "driverName": "أحمد محمد",
        "payoutOrder": 1,
        "hasPaidCurrentCycle": true,
        "hasReceivedPayout": true,
        "payoutReceivedAt": "2026-01-30T00:00:00Z"
      },
      {
        "driverId": "driver-guid-2",
        "driverName": "محمود حسن",
        "payoutOrder": 2,
        "hasPaidCurrentCycle": true,
        "hasReceivedPayout": true,
        "payoutReceivedAt": "2026-02-28T00:00:00Z"
      }
    ],
    "paymentHistory": [
      {
        "cycle": 1,
        "recipientName": "أحمد محمد",
        "amount": 5000.00,
        "paidAt": "2026-01-30T00:00:00Z"
      }
    ],
    "nextPayoutDate": "2026-04-01T00:00:00Z",
    "createdAt": "2026-01-01T00:00:00Z"
  },
  "message": null,
  "errors": null
}
```

---

### 28.3 Create Savings Circle

```
POST /admin/savings-circles
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "دائرة جديدة",
  "description": "دائرة توفير أسبوعية",
  "maxMembers": 5,
  "contributionAmount": 200.00,
  "cycleDays": 7,
  "creatorDriverId": "driver-guid"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Circle name |
| description | string | No | Description |
| maxMembers | int | Yes | Max number of members |
| contributionAmount | decimal | Yes | Per-member contribution per cycle |
| cycleDays | int | Yes | Days per cycle |
| creatorDriverId | GUID | Yes | Creator driver ID |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "new-circle-guid",
    "name": "دائرة جديدة",
    "totalCycles": 5
  },
  "message": "تم إنشاء الدائرة بنجاح",
  "errors": null
}
```

---

### 28.4 Update Savings Circle

```
PUT /admin/savings-circles/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Request Body**:
```json
{
  "name": "دائرة محدّثة",
  "description": "وصف محدّث"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الدائرة بنجاح",
  "errors": null
}
```

---

### 28.5 Suspend Savings Circle

```
PUT /admin/savings-circles/{id}/suspend
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Request Body**:
```json
{
  "reason": "مخالفة شروط الاستخدام"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعليق الدائرة بنجاح",
  "errors": null
}
```

---

### 28.6 Resume Savings Circle

```
PUT /admin/savings-circles/{id}/resume
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم استئناف الدائرة بنجاح",
  "errors": null
}
```

---

### 28.7 Close Savings Circle

```
PUT /admin/savings-circles/{id}/close
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Request Body**:
```json
{
  "reason": "انتهاء جميع الدورات",
  "refundRemaining": true
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "refundedAmount": 2000.00,
    "refundedMembers": 4
  },
  "message": "تم إغلاق الدائرة بنجاح",
  "errors": null
}
```

---

### 28.8 Force Payout

```
POST /admin/savings-circles/{id}/force-payout
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Circle ID

**Request Body**:
```json
{
  "reason": "دفع مبكر بسبب ظروف طارئة للسائق"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "recipientName": "محمود حسن",
    "amount": 5000.00,
    "paidAt": "2026-03-30T16:00:00Z"
  },
  "message": "تم تنفيذ الدفع بنجاح",
  "errors": null
}
```

---

### 28.9 Remove Member from Circle

```
DELETE /admin/savings-circles/{id}/members/{memberId}
```

**Auth Required**: Yes (Admin)

**URL Params**:
- `id` (GUID) - Circle ID
- `memberId` (GUID) - Member (driver) ID

**Request Body**:
```json
{
  "reason": "عدم الالتزام بالدفع",
  "refund": true
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "removedMember": "خالد عبدالله",
    "refundAmount": 1500.00
  },
  "message": "تم إزالة العضو من الدائرة بنجاح",
  "errors": null
}
```

---

### 28.10 Get Savings Circle Statistics

```
GET /admin/savings-circles/stats
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCircles": 25,
    "activeCircles": 18,
    "completedCircles": 5,
    "suspendedCircles": 2,
    "totalMembers": 180,
    "totalPoolAmount": 450000.00,
    "totalPayoutsProcessed": 120,
    "totalPayoutsAmount": 380000.00,
    "defaultRate": 2.5,
    "averageCircleSize": 8
  },
  "message": null,
  "errors": null
}
```

---

### 28.11 Get Defaulting Members

```
GET /admin/savings-circles/defaulters
```

**Auth Required**: Yes (Admin)

**Query Parameters**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "driverId": "driver-guid",
        "driverName": "خالد عبدالله",
        "driverPhone": "+201512345678",
        "circleId": "circle-guid",
        "circleName": "دائرة السائقين المميزين",
        "missedPayments": 2,
        "totalOwed": 1000.00,
        "lastPaymentAt": "2026-02-01T00:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 8,
    "totalPages": 1,
    "hasNext": false,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

## 29. Enums Reference

### OrderStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Accepted | مقبول |
| 2 | PickedUp | تم الاستلام |
| 3 | InTransit | في الطريق |
| 4 | Arrived | وصل |
| 5 | Delivered | تم التسليم |
| 6 | Cancelled | ملغي |
| 7 | Returned | مرتجع |

### DriverStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Inactive | غير نشط |
| 1 | Active | نشط |
| 2 | Suspended | موقوف |
| 3 | Banned | محظور |

### VehicleType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Motorcycle | موتوسيكل |
| 1 | Car | سيارة |
| 2 | Van | فان |
| 3 | Truck | تراك |
| 4 | Bicycle | دراجة |

### SubscriptionStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Active | نشط |
| 2 | Expired | منتهي |
| 3 | Cancelled | ملغي |

### PartnerVerificationStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد المراجعة |
| 1 | InReview | قيد التحقق |
| 2 | Verified | تم التحقق |
| 3 | Rejected | مرفوض |

### DisputeStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Open | مفتوح |
| 1 | InProgress | قيد المعالجة |
| 2 | Resolved | تم الحل |
| 3 | Escalated | مُصعّد |
| 4 | Closed | مغلق |

### SettlementStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Draft | مسودة |
| 1 | Pending | قيد الانتظار |
| 2 | Approved | معتمد |
| 3 | Paid | مدفوع |
| 4 | Rejected | مرفوض |

### PaymentStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Processing | قيد المعالجة |
| 2 | Completed | مكتمل |
| 3 | Failed | فشل |
| 4 | Voided | ملغي |
| 5 | Refunded | مسترد |

### RefundStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Approved | معتمد |
| 2 | Processed | تم التنفيذ |
| 3 | Rejected | مرفوض |

### InvoiceStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Draft | مسودة |
| 1 | Issued | صادرة |
| 2 | Paid | مدفوعة |
| 3 | Overdue | متأخرة |
| 4 | Cancelled | ملغاة |

### SOSType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | General | عام |
| 1 | Accident | حادث |
| 2 | Breakdown | عطل |
| 3 | Threat | تهديد |
| 4 | Medical | طبي |

### SOSStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Active | نشط |
| 1 | Acknowledged | تم الاستلام |
| 2 | Resolved | تم الحل |
| 3 | Escalated | مُصعّد |

### CampaignStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Draft | مسودة |
| 1 | Active | نشطة |
| 2 | Paused | متوقفة |
| 3 | Ended | منتهية |
| 4 | Cancelled | ملغاة |

### CircleStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Active | نشطة |
| 2 | Suspended | معلّقة |
| 3 | Completed | مكتملة |
| 4 | Closed | مغلقة |

---

## Rate Limiting

| Scope | Limit | Window |
|-------|-------|--------|
| Admin endpoints (general) | 200 requests | per minute |
| Export endpoints | 10 requests | per hour |
| Bulk operations | 20 requests | per hour |
| Statistics endpoints | 60 requests | per minute |

When rate limited, you receive:
```
HTTP 429 Too Many Requests
```

---
