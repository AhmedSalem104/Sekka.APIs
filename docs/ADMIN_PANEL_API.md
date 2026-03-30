# Sekka API - Admin Panel Complete Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-30
>
> **Total Endpoints**: 214
>
> **Authorization**: All endpoints require `Admin` role

---

## Table of Contents

1. [Overview](#1-overview)
2. [Authentication & Authorization](#2-authentication--authorization)
3. [Response Format](#3-response-format)
4. [Admin Role Setup Guide](#4-admin-role-setup-guide)
5. [Controllers Summary](#5-controllers-summary)
6. [AdminDriversController](#6-admindriverscontroller)
7. [AdminRolesController](#7-adminrolescontroller)
8. [AdminSubscriptionsController](#8-adminsubscriptionscontroller)
9. [AdminOrdersController](#9-adminorderscontroller)
10. [AdminTimeSlotsController](#10-admintimeslotscontroller)
11. [AdminCustomersController](#11-admincustomerscontroller)
12. [AdminPartnersController](#12-adminpartnerscontroller)
13. [AdminBlacklistController](#13-adminblacklistcontroller)
14. [AdminSettlementsController](#14-adminsettlementscontroller)
15. [AdminPaymentController](#15-adminpaymentcontroller)
16. [AdminWalletController](#16-adminwalletcontroller)
17. [AdminDisputesController](#17-admindisputescontroller)
18. [AdminInvoiceController](#18-admininvoicecontroller)
19. [AdminRefundController](#19-adminrefundcontroller)
20. [AdminStatisticsController](#20-adminstatisticscontroller)
21. [AdminNotificationsController](#21-adminnotificationscontroller)
22. [AdminSOSController](#22-adminsoscontroller)
23. [AdminVehiclesController](#23-adminvehiclescontroller)
24. [AdminConfigController](#24-adminconfigcontroller)
25. [AdminRegionsController](#25-adminregionscontroller)
26. [AdminAuditLogsController](#26-adminauditlogscontroller)
27. [AdminSegmentsController](#27-adminsegmentscontroller)
28. [AdminCampaignsController](#28-admincampaignscontroller)
29. [AdminInsightsController](#29-admininsightscontroller)
30. [AdminSavingsCirclesController](#30-adminsavingscirclescontroller)
31. [Enums Reference](#31-enums-reference)
32. [Pagination & Filtering](#32-pagination--filtering)
33. [Error Codes Reference](#33-error-codes-reference)

---

## 1. Overview

The Sekka Admin Panel API provides comprehensive management capabilities for the Sekka delivery platform. Through 25 controllers and 214 endpoints, administrators can manage every aspect of the platform including:

- **Driver Management** -- Activate/deactivate drivers, monitor performance, track locations
- **Order Operations** -- Create, assign, redistribute, and track orders with kanban board support
- **Financial Management** -- Wallets, settlements, payments, invoices, refunds, and disputes
- **Customer & Partner Management** -- Block/unblock customers, verify partners, view stats
- **Platform Configuration** -- Settings, feature flags, maintenance windows, app versions, commissions
- **Analytics & Insights** -- Real-time statistics, heatmaps, trends, RFM analysis, revenue breakdowns
- **Communication** -- Broadcast notifications, SOS management, campaign orchestration
- **Security** -- Role management, blacklisting, audit logs, fraud detection
- **Subscriptions** -- Plan management, gifting, extensions, expiration monitoring
- **Customer Intelligence** -- Segmentation, campaigns, behavioral insights, engagement tracking

All 214 endpoints are protected by `[Authorize(Roles = "Admin")]` and require a valid JWT Bearer token with the Admin role claim.

---

## 2. Authentication & Authorization

### Obtaining an Admin Token

Admin tokens are obtained via the standard auth login endpoint:

```
POST /api/v1/auth/login
```

**Request:**
```json
{
  "phone": "01012345678",
  "password": "AdminP@ssw0rd!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "fullName": "Admin User",
    "phone": "01012345678",
    "roles": ["Admin"],
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "r_abc123def456...",
    "tokenExpiration": "2026-03-31T14:30:00Z"
  },
  "message": "تم تسجيل الدخول بنجاح",
  "errors": null
}
```

### Using the Token

Include the token in the `Authorization` header of every admin request:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Details

| Property | Value |
|----------|-------|
| Type | JWT (Bearer) |
| Expiry | 24 hours (1440 min) |
| Refresh Token Expiry | 30 days |
| Required Role Claim | `Admin` |
| Refresh Endpoint | `POST /api/v1/auth/refresh-token` |

### Unauthorized Response (Missing/Invalid Token)

```json
{
  "isSuccess": false,
  "data": null,
  "message": "غير مصرح",
  "errors": null
}
```
**HTTP Status**: `401 Unauthorized`

### Forbidden Response (Valid Token but Non-Admin)

```json
{
  "isSuccess": false,
  "data": null,
  "message": "ليس لديك صلاحية للوصول",
  "errors": null
}
```
**HTTP Status**: `403 Forbidden`

---

## 3. Response Format

All API responses follow the standard `ApiResponse<T>` wrapper:

### Success Response

```json
{
  "isSuccess": true,
  "data": { ... },
  "message": "رسالة النجاح",
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

### Paginated Response

```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 15,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
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
| 401 | Unauthorized (invalid/missing token) |
| 403 | Forbidden (not Admin role) |
| 404 | Not Found |
| 409 | Conflict (duplicate resource) |
| 422 | Unprocessable Entity |
| 429 | Too Many Requests (rate limited) |
| 500 | Internal Server Error |

---

## 4. Admin Role Setup Guide

### Step 1: Create the Admin Role

```
POST /api/v1/admin/roles
Authorization: Bearer {super-admin-token}
```

```json
{
  "name": "Admin",
  "description": "Full platform administrator"
}
```

### Step 2: Assign the Admin Role to a User

```
POST /api/v1/admin/roles/assign
Authorization: Bearer {super-admin-token}
```

```json
{
  "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "roleName": "Admin"
}
```

### Step 3: Verify Role Assignment

```
GET /api/v1/admin/roles/users/a1b2c3d4-e5f6-7890-abcd-ef1234567890
Authorization: Bearer {super-admin-token}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": ["Admin"],
  "message": null,
  "errors": null
}
```

### Step 4: Login with Admin Credentials

```
POST /api/v1/auth/login
```

```json
{
  "phone": "01012345678",
  "password": "AdminP@ssw0rd!"
}
```

The returned JWT token will contain the `Admin` role claim, granting access to all 214 admin endpoints.

---

## 5. Controllers Summary

| # | Controller | Route Prefix | Endpoints | Description |
|---|-----------|-------------|-----------|-------------|
| 1 | AdminDriversController | `/admin/drivers` | 6 | Driver management, activation, performance |
| 2 | AdminRolesController | `/admin/roles` | 7 | Role CRUD, assignment, revocation |
| 3 | AdminSubscriptionsController | `/admin/subscriptions` | 12 | Subscription & plan management |
| 4 | AdminOrdersController | `/admin/orders` | 10 | Order operations, assignment, export |
| 5 | AdminTimeSlotsController | `/admin/time-slots` | 6 | Delivery time slot management |
| 6 | AdminCustomersController | `/admin/customers` | 5 | Customer management, blocking |
| 7 | AdminPartnersController | `/admin/partners` | 9 | Partner verification, settlements |
| 8 | AdminBlacklistController | `/admin/blacklist` | 6 | Phone blacklisting, fraud prevention |
| 9 | AdminSettlementsController | `/admin/settlements` | 5 | Financial settlement processing |
| 10 | AdminPaymentController | `/admin/payments` | 6 | Payment approval workflow |
| 11 | AdminWalletController | `/admin/wallets` | 11 | Driver wallet operations |
| 12 | AdminDisputesController | `/admin/disputes` | 7 | Dispute resolution workflow |
| 13 | AdminInvoiceController | `/admin/invoices` | 7 | Invoice generation & management |
| 14 | AdminRefundController | `/admin/refunds` | 6 | Refund approval workflow |
| 15 | AdminStatisticsController | `/admin/statistics` | 19 | Platform analytics & reporting |
| 16 | AdminNotificationsController | `/admin/notifications` | 3 | Push notification management |
| 17 | AdminSOSController | `/admin/sos` | 10 | Emergency SOS management |
| 18 | AdminVehiclesController | `/admin/vehicles` | 11 | Vehicle approval & maintenance |
| 19 | AdminConfigController | `/admin/config` | 20 | Platform configuration |
| 20 | AdminRegionsController | `/admin/regions` | 4 | Geographic region management |
| 21 | AdminAuditLogsController | `/admin/audit-logs` | 6 | Audit trail & activity logs |
| 22 | AdminSegmentsController | `/admin/segments` | 10 | Customer segmentation |
| 23 | AdminCampaignsController | `/admin/campaigns` | 10 | Marketing campaign management |
| 24 | AdminInsightsController | `/admin/insights` | 7 | Business intelligence & analytics |
| 25 | AdminSavingsCirclesController | `/admin/savings-circles` | 10 | Savings circle (game3ya) management |
| | **Total** | | **214** | |

---

## 6. AdminDriversController

**Route Prefix**: `/api/v1/admin/drivers`

Manage platform drivers including activation status, performance monitoring, and real-time location tracking. This controller provides the tools to onboard, monitor, and manage the driver fleet.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/drivers` | Get all drivers (filtered + paginated) |
| GET | `/admin/drivers/{id}` | Get driver details by ID |
| PUT | `/admin/drivers/{id}/activate` | Activate a driver account |
| PUT | `/admin/drivers/{id}/deactivate` | Deactivate a driver account |
| GET | `/admin/drivers/{id}/performance` | Get driver performance metrics |
| GET | `/admin/drivers/locations` | Get real-time driver locations |

### 6.1 Get All Drivers

```
GET /api/v1/admin/drivers?pageNumber=1&pageSize=10&status=Active&search=ahmed
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | Filter by status: `Active`, `Inactive`, `Pending` |
| search | string | No | Search by name or phone |
| regionId | Guid | No | Filter by region |
| vehicleType | string | No | Filter by vehicle type |
| sortBy | string | No | Sort field (default: `createdAt`) |
| sortDirection | string | No | `asc` or `desc` (default: `desc`) |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
        "fullName": "احمد محمد",
        "phone": "01012345678",
        "email": "ahmed@example.com",
        "status": "Active",
        "vehicleType": "Motorcycle",
        "plateNumber": "أ ب ج 1234",
        "region": "القاهرة",
        "rating": 4.7,
        "totalOrders": 342,
        "completionRate": 96.5,
        "joinedAt": "2025-08-15T10:30:00Z",
        "lastActiveAt": "2026-03-30T14:20:00Z",
        "isOnline": true
      }
    ],
    "totalCount": 85,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 6.2 Get Driver by ID

```
GET /api/v1/admin/drivers/d1a2b3c4-e5f6-7890-abcd-ef1234567890
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
    "fullName": "احمد محمد",
    "phone": "01012345678",
    "email": "ahmed@example.com",
    "nationalId": "29901XXXXXXXXXX",
    "status": "Active",
    "vehicleType": "Motorcycle",
    "plateNumber": "أ ب ج 1234",
    "licenseNumber": "LIC-12345",
    "licenseExpiry": "2027-06-15",
    "region": "القاهرة",
    "rating": 4.7,
    "totalOrders": 342,
    "completedOrders": 330,
    "cancelledOrders": 12,
    "completionRate": 96.5,
    "averageDeliveryTime": 28.5,
    "walletBalance": 1250.00,
    "subscriptionPlan": "Premium",
    "subscriptionExpiry": "2026-06-30T23:59:59Z",
    "joinedAt": "2025-08-15T10:30:00Z",
    "lastActiveAt": "2026-03-30T14:20:00Z",
    "isOnline": true,
    "currentLocation": {
      "latitude": 30.0444,
      "longitude": 31.2357
    }
  },
  "message": null,
  "errors": null
}
```

### 6.3 Activate Driver

```
PUT /api/v1/admin/drivers/d1a2b3c4-e5f6-7890-abcd-ef1234567890/activate
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تفعيل السائق بنجاح",
  "errors": null
}
```

### 6.4 Deactivate Driver

```
PUT /api/v1/admin/drivers/d1a2b3c4-e5f6-7890-abcd-ef1234567890/deactivate
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء تفعيل السائق بنجاح",
  "errors": null
}
```

### 6.5 Get Driver Performance

```
GET /api/v1/admin/drivers/d1a2b3c4-e5f6-7890-abcd-ef1234567890/performance
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
    "driverName": "احمد محمد",
    "period": "Last30Days",
    "totalOrders": 87,
    "completedOrders": 84,
    "cancelledOrders": 3,
    "completionRate": 96.55,
    "averageDeliveryTimeMinutes": 28.5,
    "averageRating": 4.7,
    "totalRatings": 72,
    "onTimeDeliveryRate": 91.2,
    "customerComplaints": 1,
    "totalEarnings": 4350.00,
    "averageDailyEarnings": 145.00,
    "peakHours": ["12:00-14:00", "18:00-21:00"],
    "topRegions": ["المعادي", "مدينة نصر", "المهندسين"],
    "dailyBreakdown": [
      {
        "date": "2026-03-29",
        "orders": 5,
        "earnings": 175.00,
        "rating": 4.8
      }
    ]
  },
  "message": null,
  "errors": null
}
```

### 6.6 Get Driver Locations

```
GET /api/v1/admin/drivers/locations
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
      "driverName": "احمد محمد",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "isOnline": true,
      "currentOrderId": "ord-abc123",
      "vehicleType": "Motorcycle",
      "lastUpdated": "2026-03-30T14:20:00Z"
    },
    {
      "driverId": "d2b3c4d5-f6a7-8901-bcde-f21234567890",
      "driverName": "محمد علي",
      "latitude": 30.0626,
      "longitude": 31.2497,
      "isOnline": true,
      "currentOrderId": null,
      "vehicleType": "Car",
      "lastUpdated": "2026-03-30T14:19:45Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 7. AdminRolesController

**Route Prefix**: `/api/v1/admin/roles`

Manage platform roles including creation, assignment, and revocation. Controls who has access to admin features and other role-protected resources.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/roles` | Get all roles |
| POST | `/admin/roles` | Create a new role |
| PUT | `/admin/roles/{id}` | Update a role |
| DELETE | `/admin/roles/{id}` | Delete a role |
| POST | `/admin/roles/assign` | Assign role to user |
| DELETE | `/admin/roles/revoke` | Revoke role from user |
| GET | `/admin/roles/users/{userId}` | Get user's roles |

### 7.1 Get All Roles

```
GET /api/v1/admin/roles
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "r1a2b3c4-d5e6-7890-abcd-ef1234567890",
      "name": "Admin",
      "description": "Full platform administrator",
      "usersCount": 3
    },
    {
      "id": "r2b3c4d5-e6f7-8901-bcde-f21234567890",
      "name": "Driver",
      "description": "Delivery driver",
      "usersCount": 85
    },
    {
      "id": "r3c4d5e6-f7a8-9012-cdef-312345678901",
      "name": "Customer",
      "description": "Platform customer",
      "usersCount": 1250
    },
    {
      "id": "r4d5e6f7-a8b9-0123-def0-423456789012",
      "name": "Partner",
      "description": "Business partner / merchant",
      "usersCount": 42
    }
  ],
  "message": null,
  "errors": null
}
```

### 7.2 Create Role

```
POST /api/v1/admin/roles
```

**Request:**
```json
{
  "name": "Supervisor",
  "description": "Regional supervisor with limited admin access"
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "r5e6f7a8-b9c0-1234-ef01-534567890123",
    "name": "Supervisor",
    "description": "Regional supervisor with limited admin access"
  },
  "message": "تم إنشاء الدور بنجاح",
  "errors": null
}
```

### 7.3 Update Role

```
PUT /api/v1/admin/roles/r5e6f7a8-b9c0-1234-ef01-534567890123
```

**Request:**
```json
{
  "name": "Supervisor",
  "description": "Regional supervisor with read-only admin access"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تحديث الدور بنجاح",
  "errors": null
}
```

### 7.4 Delete Role

```
DELETE /api/v1/admin/roles/r5e6f7a8-b9c0-1234-ef01-534567890123
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم حذف الدور بنجاح",
  "errors": null
}
```

### 7.5 Assign Role to User

```
POST /api/v1/admin/roles/assign
```

**Request:**
```json
{
  "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "roleName": "Admin"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تعيين الدور بنجاح",
  "errors": null
}
```

### 7.6 Revoke Role from User

```
DELETE /api/v1/admin/roles/revoke
```

**Request:**
```json
{
  "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "roleName": "Admin"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء الدور بنجاح",
  "errors": null
}
```

### 7.7 Get User Roles

```
GET /api/v1/admin/roles/users/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

**Response:**
```json
{
  "isSuccess": true,
  "data": ["Admin", "Supervisor"],
  "message": null,
  "errors": null
}
```

---

## 8. AdminSubscriptionsController

**Route Prefix**: `/api/v1/admin/subscriptions`

Manage driver subscriptions and subscription plans. Supports plan CRUD, subscription lifecycle management (extend, cancel, change plan), gifting subscriptions, and monitoring expirations.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/subscriptions` | Get all subscriptions (filtered + paginated) |
| GET | `/admin/subscriptions/{id}` | Get subscription details |
| PUT | `/admin/subscriptions/{id}/extend` | Extend subscription duration |
| PUT | `/admin/subscriptions/{id}/cancel` | Cancel a subscription |
| PUT | `/admin/subscriptions/{id}/change-plan` | Change subscription plan |
| POST | `/admin/subscriptions/gift` | Gift a subscription to a driver |
| GET | `/admin/subscriptions/plans` | Get all subscription plans |
| POST | `/admin/subscriptions/plans` | Create a subscription plan |
| PUT | `/admin/subscriptions/plans/{id}` | Update a subscription plan |
| PUT | `/admin/subscriptions/plans/{id}/toggle` | Toggle plan active status |
| GET | `/admin/subscriptions/stats` | Get subscription statistics |
| GET | `/admin/subscriptions/expiring-soon` | Get subscriptions expiring soon |

### 8.1 Get All Subscriptions

```
GET /api/v1/admin/subscriptions?pageNumber=1&pageSize=10&status=Active&planId=xxx
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Active`, `Expired`, `Cancelled` |
| planId | Guid | No | Filter by subscription plan |
| search | string | No | Search by driver name or phone |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "sub-a1b2c3d4",
        "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
        "driverName": "احمد محمد",
        "driverPhone": "01012345678",
        "planId": "plan-001",
        "planName": "Premium",
        "status": "Active",
        "startDate": "2026-01-01T00:00:00Z",
        "endDate": "2026-06-30T23:59:59Z",
        "price": 299.00,
        "isGifted": false,
        "createdAt": "2025-12-28T10:00:00Z"
      }
    ],
    "totalCount": 72,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 8.2 Get Subscription by ID

```
GET /api/v1/admin/subscriptions/sub-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "sub-a1b2c3d4",
    "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
    "driverName": "احمد محمد",
    "driverPhone": "01012345678",
    "planId": "plan-001",
    "planName": "Premium",
    "status": "Active",
    "startDate": "2026-01-01T00:00:00Z",
    "endDate": "2026-06-30T23:59:59Z",
    "price": 299.00,
    "isGifted": false,
    "giftedBy": null,
    "cancelledAt": null,
    "cancellationReason": null,
    "history": [
      {
        "action": "Created",
        "date": "2025-12-28T10:00:00Z",
        "details": "Subscription created via payment"
      }
    ],
    "createdAt": "2025-12-28T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

### 8.3 Extend Subscription

```
PUT /api/v1/admin/subscriptions/sub-a1b2c3d4/extend
```

**Request:**
```json
{
  "days": 30,
  "reason": "تعويض عن فترة الصيانة"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "newEndDate": "2026-07-30T23:59:59Z"
  },
  "message": "تم تمديد الاشتراك بنجاح",
  "errors": null
}
```

### 8.4 Cancel Subscription

```
PUT /api/v1/admin/subscriptions/sub-a1b2c3d4/cancel
```

**Request:**
```json
{
  "reason": "طلب السائق الإلغاء"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء الاشتراك بنجاح",
  "errors": null
}
```

### 8.5 Change Subscription Plan

```
PUT /api/v1/admin/subscriptions/sub-a1b2c3d4/change-plan
```

**Request:**
```json
{
  "newPlanId": "plan-002",
  "prorateRefund": true
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "oldPlanName": "Premium",
    "newPlanName": "Basic",
    "refundAmount": 75.00,
    "newEndDate": "2026-06-30T23:59:59Z"
  },
  "message": "تم تغيير خطة الاشتراك بنجاح",
  "errors": null
}
```

### 8.6 Gift Subscription

```
POST /api/v1/admin/subscriptions/gift
```

**Request:**
```json
{
  "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
  "planId": "plan-001",
  "durationDays": 30,
  "reason": "مكافأة لأفضل سائق في الشهر"
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "subscriptionId": "sub-new123",
    "startDate": "2026-03-30T00:00:00Z",
    "endDate": "2026-04-29T23:59:59Z"
  },
  "message": "تم إهداء الاشتراك بنجاح",
  "errors": null
}
```

### 8.7 Get Subscription Plans

```
GET /api/v1/admin/subscriptions/plans
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "plan-001",
      "name": "Premium",
      "nameAr": "بريميوم",
      "description": "Full access with priority support",
      "descriptionAr": "وصول كامل مع دعم أولوي",
      "price": 299.00,
      "durationDays": 30,
      "features": ["Priority orders", "24/7 Support", "No commission cap"],
      "isActive": true,
      "subscribersCount": 45
    },
    {
      "id": "plan-002",
      "name": "Basic",
      "nameAr": "أساسي",
      "description": "Standard access",
      "descriptionAr": "وصول عادي",
      "price": 149.00,
      "durationDays": 30,
      "features": ["Standard orders", "Business hours support"],
      "isActive": true,
      "subscribersCount": 27
    }
  ],
  "message": null,
  "errors": null
}
```

### 8.8 Create Subscription Plan

```
POST /api/v1/admin/subscriptions/plans
```

**Request:**
```json
{
  "name": "Enterprise",
  "nameAr": "مؤسسات",
  "description": "Enterprise plan with fleet management",
  "descriptionAr": "خطة مؤسسات مع إدارة الأسطول",
  "price": 599.00,
  "durationDays": 30,
  "features": ["Fleet management", "Dedicated account manager", "API access"],
  "isActive": true
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "plan-003",
    "name": "Enterprise"
  },
  "message": "تم إنشاء خطة الاشتراك بنجاح",
  "errors": null
}
```

### 8.9 Update Subscription Plan

```
PUT /api/v1/admin/subscriptions/plans/plan-003
```

**Request:**
```json
{
  "name": "Enterprise Plus",
  "price": 699.00,
  "features": ["Fleet management", "Dedicated account manager", "API access", "Custom reports"]
}
```

### 8.10 Toggle Plan Active Status

```
PUT /api/v1/admin/subscriptions/plans/plan-003/toggle
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "isActive": false
  },
  "message": "تم تغيير حالة الخطة بنجاح",
  "errors": null
}
```

### 8.11 Get Subscription Statistics

```
GET /api/v1/admin/subscriptions/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalActive": 72,
    "totalExpired": 145,
    "totalCancelled": 23,
    "totalRevenue": 21528.00,
    "monthlyRecurringRevenue": 7176.00,
    "averageSubscriptionDays": 68.5,
    "churnRate": 9.6,
    "planBreakdown": [
      { "planName": "Premium", "count": 45, "revenue": 13455.00 },
      { "planName": "Basic", "count": 27, "revenue": 4023.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 8.12 Get Expiring Soon

```
GET /api/v1/admin/subscriptions/expiring-soon?days=7
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| days | int | No | Days until expiry threshold (default: 7) |

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "subscriptionId": "sub-expiring1",
      "driverName": "محمد خالد",
      "driverPhone": "01112345678",
      "planName": "Basic",
      "endDate": "2026-04-03T23:59:59Z",
      "daysRemaining": 4
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 9. AdminOrdersController

**Route Prefix**: `/api/v1/admin/orders`

Comprehensive order management with kanban board view, admin order creation, manual/auto assignment, timeline tracking, status overrides, and data export.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/orders` | Get all orders (filtered + paginated) |
| GET | `/admin/orders/board` | Get kanban board view |
| POST | `/admin/orders` | Admin create order |
| POST | `/admin/orders/{id}/assign` | Manually assign driver |
| POST | `/admin/orders/auto-distribute` | Auto-distribute pending orders |
| GET | `/admin/orders/{id}/timeline` | Get order timeline/history |
| PUT | `/admin/orders/{id}/override-status` | Override order status |
| GET | `/admin/orders/export` | Export orders to CSV/Excel |
| POST | `/admin/orders/{id}/auto-assign` | Auto-assign best driver for order |
| GET | `/admin/orders/{id}/suggested-drivers` | Get suggested drivers for order |

### 9.1 Get All Orders

```
GET /api/v1/admin/orders?pageNumber=1&pageSize=20&status=InProgress&fromDate=2026-03-01&toDate=2026-03-30
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Pending`, `Assigned`, `PickedUp`, `InProgress`, `Delivered`, `Cancelled`, `Returned` |
| fromDate | DateTime | No | Filter from date |
| toDate | DateTime | No | Filter to date |
| driverId | Guid | No | Filter by driver |
| customerId | Guid | No | Filter by customer |
| partnerId | Guid | No | Filter by partner |
| regionId | Guid | No | Filter by region |
| search | string | No | Search by order number or tracking code |
| sortBy | string | No | Sort field |
| sortDirection | string | No | `asc` or `desc` |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "ord-a1b2c3d4",
        "orderNumber": "ORD-20260330-0042",
        "trackingCode": "TRK-XYZ789",
        "status": "InProgress",
        "customerName": "سارة أحمد",
        "customerPhone": "01234567890",
        "driverName": "احمد محمد",
        "driverPhone": "01012345678",
        "partnerName": "مطعم السلطان",
        "pickupAddress": "15 شارع التحرير، الدقي",
        "deliveryAddress": "28 شارع الملك فيصل، الجيزة",
        "totalAmount": 125.50,
        "deliveryFee": 25.00,
        "estimatedDeliveryTime": "2026-03-30T15:30:00Z",
        "createdAt": "2026-03-30T14:00:00Z",
        "assignedAt": "2026-03-30T14:05:00Z",
        "pickedUpAt": "2026-03-30T14:20:00Z"
      }
    ],
    "totalCount": 342,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 18,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 9.2 Get Kanban Board

```
GET /api/v1/admin/orders/board?regionId=xxx&date=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "pending": {
      "count": 12,
      "orders": [
        {
          "id": "ord-pending1",
          "orderNumber": "ORD-20260330-0050",
          "customerName": "خالد حسن",
          "pickupAddress": "المعادي",
          "deliveryAddress": "مدينة نصر",
          "totalAmount": 89.00,
          "createdAt": "2026-03-30T14:30:00Z",
          "waitingMinutes": 15
        }
      ]
    },
    "assigned": {
      "count": 8,
      "orders": []
    },
    "pickedUp": {
      "count": 5,
      "orders": []
    },
    "inProgress": {
      "count": 15,
      "orders": []
    },
    "delivered": {
      "count": 42,
      "orders": []
    },
    "cancelled": {
      "count": 3,
      "orders": []
    }
  },
  "message": null,
  "errors": null
}
```

### 9.3 Admin Create Order

```
POST /api/v1/admin/orders
```

**Request:**
```json
{
  "customerId": "cust-a1b2c3d4",
  "partnerId": "part-e5f6a7b8",
  "pickupAddress": "15 شارع التحرير، الدقي",
  "pickupLatitude": 30.0384,
  "pickupLongitude": 31.2120,
  "deliveryAddress": "28 شارع الملك فيصل، الجيزة",
  "deliveryLatitude": 30.0131,
  "deliveryLongitude": 31.2089,
  "totalAmount": 150.00,
  "deliveryFee": 30.00,
  "notes": "طلب عاجل - تم الإنشاء بواسطة الإدارة",
  "items": [
    {
      "name": "طرد كبير",
      "quantity": 1,
      "price": 150.00
    }
  ],
  "scheduledAt": null
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "ord-new123",
    "orderNumber": "ORD-20260330-0055",
    "trackingCode": "TRK-ADM456"
  },
  "message": "تم إنشاء الطلب بنجاح",
  "errors": null
}
```

### 9.4 Assign Driver to Order

```
POST /api/v1/admin/orders/ord-pending1/assign
```

**Request:**
```json
{
  "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
  "reason": "السائق الأقرب للموقع"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تعيين السائق بنجاح",
  "errors": null
}
```

### 9.5 Auto-Distribute Orders

```
POST /api/v1/admin/orders/auto-distribute
```

**Request:**
```json
{
  "regionId": "reg-cairo",
  "maxOrdersPerDriver": 5,
  "prioritizeRating": true
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalDistributed": 8,
    "assignments": [
      {
        "orderId": "ord-pending1",
        "driverId": "d1a2b3c4",
        "driverName": "احمد محمد",
        "distanceKm": 2.3
      },
      {
        "orderId": "ord-pending2",
        "driverId": "d2b3c4d5",
        "driverName": "محمد علي",
        "distanceKm": 1.8
      }
    ],
    "unassigned": 4,
    "unassignedReason": "No available drivers in range"
  },
  "message": "تم توزيع الطلبات تلقائياً",
  "errors": null
}
```

### 9.6 Get Order Timeline

```
GET /api/v1/admin/orders/ord-a1b2c3d4/timeline
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "ord-a1b2c3d4",
    "orderNumber": "ORD-20260330-0042",
    "events": [
      {
        "event": "OrderCreated",
        "timestamp": "2026-03-30T14:00:00Z",
        "details": "طلب جديد من سارة أحمد",
        "actor": "Customer"
      },
      {
        "event": "DriverAssigned",
        "timestamp": "2026-03-30T14:05:00Z",
        "details": "تم تعيين السائق احمد محمد",
        "actor": "System"
      },
      {
        "event": "DriverAccepted",
        "timestamp": "2026-03-30T14:06:30Z",
        "details": "السائق قبل الطلب",
        "actor": "Driver"
      },
      {
        "event": "PickedUp",
        "timestamp": "2026-03-30T14:20:00Z",
        "details": "تم استلام الطلب من المطعم",
        "actor": "Driver"
      },
      {
        "event": "InTransit",
        "timestamp": "2026-03-30T14:22:00Z",
        "details": "الطلب في الطريق إلى العميل",
        "actor": "Driver"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

### 9.7 Override Order Status

```
PUT /api/v1/admin/orders/ord-a1b2c3d4/override-status
```

**Request:**
```json
{
  "newStatus": "Delivered",
  "reason": "العميل أكد الاستلام عبر الهاتف",
  "adminNote": "تم التحقق يدوياً"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تحديث حالة الطلب بنجاح",
  "errors": null
}
```

### 9.8 Export Orders

```
GET /api/v1/admin/orders/export?format=csv&fromDate=2026-03-01&toDate=2026-03-30&status=Delivered
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| format | string | No | `csv` or `excel` (default: `csv`) |
| fromDate | DateTime | No | Start date filter |
| toDate | DateTime | No | End date filter |
| status | string | No | Status filter |
| regionId | Guid | No | Region filter |

**Response:** File download (Content-Type: `text/csv` or `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`)

### 9.9 Auto-Assign Best Driver

```
POST /api/v1/admin/orders/ord-pending1/auto-assign
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
    "driverName": "احمد محمد",
    "distanceKm": 1.5,
    "estimatedArrivalMinutes": 8,
    "rating": 4.7,
    "matchScore": 95.2
  },
  "message": "تم تعيين السائق الأنسب تلقائياً",
  "errors": null
}
```

### 9.10 Get Suggested Drivers

```
GET /api/v1/admin/orders/ord-pending1/suggested-drivers
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "d1a2b3c4",
      "driverName": "احمد محمد",
      "distanceKm": 1.5,
      "estimatedArrivalMinutes": 8,
      "rating": 4.7,
      "currentOrders": 2,
      "vehicleType": "Motorcycle",
      "matchScore": 95.2,
      "isOnline": true
    },
    {
      "driverId": "d2b3c4d5",
      "driverName": "محمد علي",
      "distanceKm": 2.1,
      "estimatedArrivalMinutes": 12,
      "rating": 4.5,
      "currentOrders": 1,
      "vehicleType": "Car",
      "matchScore": 88.7,
      "isOnline": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 10. AdminTimeSlotsController

**Route Prefix**: `/api/v1/admin/time-slots`

Manage delivery time slots for scheduled orders. Create individual slots or generate a full week of slots. View statistics on slot utilization.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/time-slots` | Get all time slots |
| POST | `/admin/time-slots` | Create a time slot |
| PUT | `/admin/time-slots/{id}` | Update a time slot |
| DELETE | `/admin/time-slots/{id}` | Delete a time slot |
| POST | `/admin/time-slots/generate-week` | Generate slots for a week |
| GET | `/admin/time-slots/stats` | Get time slot statistics |

### 10.1 Get All Time Slots

```
GET /api/v1/admin/time-slots?date=2026-03-30&regionId=xxx
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "ts-001",
      "date": "2026-03-30",
      "startTime": "09:00",
      "endTime": "11:00",
      "maxOrders": 20,
      "currentOrders": 14,
      "availableSlots": 6,
      "regionId": "reg-cairo",
      "regionName": "القاهرة",
      "isActive": true
    },
    {
      "id": "ts-002",
      "date": "2026-03-30",
      "startTime": "11:00",
      "endTime": "13:00",
      "maxOrders": 25,
      "currentOrders": 25,
      "availableSlots": 0,
      "regionId": "reg-cairo",
      "regionName": "القاهرة",
      "isActive": true
    }
  ],
  "message": null,
  "errors": null
}
```

### 10.2 Create Time Slot

```
POST /api/v1/admin/time-slots
```

**Request:**
```json
{
  "date": "2026-04-01",
  "startTime": "09:00",
  "endTime": "11:00",
  "maxOrders": 20,
  "regionId": "reg-cairo",
  "isActive": true
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "ts-new001"
  },
  "message": "تم إنشاء الفترة الزمنية بنجاح",
  "errors": null
}
```

### 10.3 Update Time Slot

```
PUT /api/v1/admin/time-slots/ts-001
```

**Request:**
```json
{
  "maxOrders": 30,
  "isActive": true
}
```

### 10.4 Delete Time Slot

```
DELETE /api/v1/admin/time-slots/ts-001
```

### 10.5 Generate Week Slots

```
POST /api/v1/admin/time-slots/generate-week
```

**Request:**
```json
{
  "startDate": "2026-04-01",
  "regionId": "reg-cairo",
  "slots": [
    { "startTime": "09:00", "endTime": "11:00", "maxOrders": 20 },
    { "startTime": "11:00", "endTime": "13:00", "maxOrders": 25 },
    { "startTime": "13:00", "endTime": "15:00", "maxOrders": 15 },
    { "startTime": "15:00", "endTime": "18:00", "maxOrders": 30 },
    { "startTime": "18:00", "endTime": "21:00", "maxOrders": 35 },
    { "startTime": "21:00", "endTime": "23:00", "maxOrders": 20 }
  ],
  "excludeDays": ["Friday"]
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalSlotsCreated": 36,
    "daysGenerated": 6
  },
  "message": "تم إنشاء الفترات الزمنية للأسبوع بنجاح",
  "errors": null
}
```

### 10.6 Get Time Slot Statistics

```
GET /api/v1/admin/time-slots/stats?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalSlots": 540,
    "totalCapacity": 12500,
    "totalBooked": 9875,
    "utilizationRate": 79.0,
    "peakSlot": "18:00-21:00",
    "lowestSlot": "13:00-15:00",
    "byDay": [
      { "day": "Saturday", "avgUtilization": 85.2 },
      { "day": "Sunday", "avgUtilization": 78.5 },
      { "day": "Monday", "avgUtilization": 72.1 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 11. AdminCustomersController

**Route Prefix**: `/api/v1/admin/customers`

Manage platform customers including viewing details, blocking/unblocking accounts, and viewing aggregate statistics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/customers` | Get all customers (filtered + paginated) |
| GET | `/admin/customers/{id}` | Get customer details |
| POST | `/admin/customers/{id}/block` | Block a customer |
| POST | `/admin/customers/{id}/unblock` | Unblock a customer |
| GET | `/admin/customers/stats` | Get customer statistics |

### 11.1 Get All Customers

```
GET /api/v1/admin/customers?pageNumber=1&pageSize=10&status=Active&search=sara
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Active`, `Blocked` |
| search | string | No | Search by name or phone |
| sortBy | string | No | Sort field |
| sortDirection | string | No | `asc` or `desc` |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "cust-a1b2c3d4",
        "fullName": "سارة أحمد",
        "phone": "01234567890",
        "email": "sara@example.com",
        "status": "Active",
        "totalOrders": 28,
        "totalSpent": 3450.00,
        "averageOrderValue": 123.21,
        "rating": 4.9,
        "joinedAt": "2025-09-01T10:00:00Z",
        "lastOrderAt": "2026-03-28T18:30:00Z"
      }
    ],
    "totalCount": 1250,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 125,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 11.2 Get Customer by ID

```
GET /api/v1/admin/customers/cust-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "cust-a1b2c3d4",
    "fullName": "سارة أحمد",
    "phone": "01234567890",
    "email": "sara@example.com",
    "status": "Active",
    "addresses": [
      {
        "id": "addr-001",
        "label": "المنزل",
        "address": "28 شارع الملك فيصل، الجيزة",
        "latitude": 30.0131,
        "longitude": 31.2089,
        "isDefault": true
      }
    ],
    "totalOrders": 28,
    "completedOrders": 26,
    "cancelledOrders": 2,
    "totalSpent": 3450.00,
    "averageOrderValue": 123.21,
    "rating": 4.9,
    "joinedAt": "2025-09-01T10:00:00Z",
    "lastOrderAt": "2026-03-28T18:30:00Z",
    "recentOrders": [
      {
        "orderId": "ord-recent1",
        "orderNumber": "ORD-20260328-0018",
        "status": "Delivered",
        "totalAmount": 145.00,
        "createdAt": "2026-03-28T18:30:00Z"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

### 11.3 Block Customer

```
POST /api/v1/admin/customers/cust-a1b2c3d4/block
```

**Request:**
```json
{
  "reason": "سلوك مسيء متكرر تجاه السائقين",
  "duration": null
}
```

> **Note:** If `duration` is `null`, the block is permanent. Otherwise, specify duration in hours.

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم حظر العميل بنجاح",
  "errors": null
}
```

### 11.4 Unblock Customer

```
POST /api/v1/admin/customers/cust-a1b2c3d4/unblock
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء حظر العميل بنجاح",
  "errors": null
}
```

### 11.5 Get Customer Statistics

```
GET /api/v1/admin/customers/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalCustomers": 1250,
    "activeCustomers": 1180,
    "blockedCustomers": 12,
    "newThisMonth": 85,
    "newThisWeek": 22,
    "averageOrdersPerCustomer": 8.3,
    "averageLifetimeValue": 1025.50,
    "retentionRate": 72.5,
    "topRegions": [
      { "region": "القاهرة", "customers": 580 },
      { "region": "الجيزة", "customers": 320 },
      { "region": "الإسكندرية", "customers": 180 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 12. AdminPartnersController

**Route Prefix**: `/api/v1/admin/partners`

Manage business partners (merchants) including verification, activation, order history, financial settlements, and statistics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/partners` | Get all partners (filtered + paginated) |
| GET | `/admin/partners/{id}` | Get partner details |
| PUT | `/admin/partners/{id}/activate` | Activate a partner |
| PUT | `/admin/partners/{id}/deactivate` | Deactivate a partner |
| PUT | `/admin/partners/{id}/verify` | Verify a partner |
| GET | `/admin/partners/{id}/orders` | Get partner's orders |
| GET | `/admin/partners/{id}/settlements` | Get partner's settlements |
| GET | `/admin/partners/stats` | Get partner statistics |
| GET | `/admin/partners/pending-verification` | Get partners awaiting verification |

### 12.1 Get All Partners

```
GET /api/v1/admin/partners?pageNumber=1&pageSize=10&status=Active&search=sultan
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "part-e5f6a7b8",
        "businessName": "مطعم السلطان",
        "contactName": "محمود السلطان",
        "phone": "01512345678",
        "email": "sultan@restaurant.com",
        "status": "Active",
        "isVerified": true,
        "category": "Restaurant",
        "address": "45 شارع الهرم، الجيزة",
        "totalOrders": 580,
        "totalRevenue": 72500.00,
        "rating": 4.6,
        "joinedAt": "2025-07-01T10:00:00Z"
      }
    ],
    "totalCount": 42,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 12.2 Get Partner by ID

```
GET /api/v1/admin/partners/part-e5f6a7b8
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "part-e5f6a7b8",
    "businessName": "مطعم السلطان",
    "contactName": "محمود السلطان",
    "phone": "01512345678",
    "email": "sultan@restaurant.com",
    "status": "Active",
    "isVerified": true,
    "verifiedAt": "2025-07-05T14:00:00Z",
    "category": "Restaurant",
    "address": "45 شارع الهرم، الجيزة",
    "latitude": 30.0131,
    "longitude": 31.2089,
    "commercialRegistration": "CR-12345",
    "taxId": "TAX-67890",
    "bankAccount": "EG****7890",
    "commissionRate": 12.5,
    "totalOrders": 580,
    "completedOrders": 562,
    "cancelledOrders": 18,
    "totalRevenue": 72500.00,
    "pendingSettlement": 3250.00,
    "rating": 4.6,
    "joinedAt": "2025-07-01T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

### 12.3 Activate Partner

```
PUT /api/v1/admin/partners/part-e5f6a7b8/activate
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تفعيل الشريك بنجاح",
  "errors": null
}
```

### 12.4 Deactivate Partner

```
PUT /api/v1/admin/partners/part-e5f6a7b8/deactivate
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء تفعيل الشريك بنجاح",
  "errors": null
}
```

### 12.5 Verify Partner

```
PUT /api/v1/admin/partners/part-e5f6a7b8/verify
```

**Request:**
```json
{
  "isApproved": true,
  "notes": "تم التحقق من السجل التجاري والبطاقة الضريبية"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم التحقق من الشريك بنجاح",
  "errors": null
}
```

### 12.6 Get Partner Orders

```
GET /api/v1/admin/partners/part-e5f6a7b8/orders?pageNumber=1&pageSize=10
```

### 12.7 Get Partner Settlements

```
GET /api/v1/admin/partners/part-e5f6a7b8/settlements?pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "stl-001",
        "amount": 5250.00,
        "ordersCount": 42,
        "periodFrom": "2026-03-01",
        "periodTo": "2026-03-15",
        "status": "Paid",
        "paidAt": "2026-03-18T10:00:00Z"
      }
    ],
    "totalCount": 12,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 12.8 Get Partner Statistics

```
GET /api/v1/admin/partners/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalPartners": 42,
    "activePartners": 38,
    "pendingVerification": 4,
    "totalRevenue": 325000.00,
    "averageCommissionRate": 12.5,
    "topPartners": [
      { "name": "مطعم السلطان", "orders": 580, "revenue": 72500.00 }
    ],
    "byCategory": [
      { "category": "Restaurant", "count": 22, "revenue": 185000.00 },
      { "category": "Grocery", "count": 12, "revenue": 95000.00 },
      { "category": "Pharmacy", "count": 8, "revenue": 45000.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 12.9 Get Pending Verification

```
GET /api/v1/admin/partners/pending-verification
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "part-pending1",
      "businessName": "صيدلية الشفاء",
      "contactName": "أحمد حسين",
      "phone": "01098765432",
      "category": "Pharmacy",
      "appliedAt": "2026-03-28T10:00:00Z",
      "documentsSubmitted": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 13. AdminBlacklistController

**Route Prefix**: `/api/v1/admin/blacklist`

Manage phone number blacklisting for fraud prevention and abuse control. Verify numbers against the blacklist, view reports, and manage blacklist entries.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/blacklist` | Get all blacklisted entries (paginated) |
| POST | `/admin/blacklist` | Add phone to blacklist |
| POST | `/admin/blacklist/verify/{phone}` | Check if phone is blacklisted |
| DELETE | `/admin/blacklist/{phone}` | Remove phone from blacklist |
| GET | `/admin/blacklist/stats` | Get blacklist statistics |
| GET | `/admin/blacklist/reports` | Get blacklist reports |

### 13.1 Get All Blacklisted Entries

```
GET /api/v1/admin/blacklist?pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "phone": "01098765432",
        "reason": "احتيال متكرر - محاولات دفع وهمية",
        "blacklistedBy": "Admin User",
        "blacklistedAt": "2026-03-15T10:00:00Z",
        "reportCount": 5
      }
    ],
    "totalCount": 34,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 4,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 13.2 Add to Blacklist

```
POST /api/v1/admin/blacklist
```

**Request:**
```json
{
  "phone": "01087654321",
  "reason": "سلوك احتيالي - إنشاء طلبات وهمية"
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إضافة الرقم إلى القائمة السوداء بنجاح",
  "errors": null
}
```

### 13.3 Verify Phone

```
POST /api/v1/admin/blacklist/verify/01087654321
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "isBlacklisted": true,
    "reason": "سلوك احتيالي - إنشاء طلبات وهمية",
    "blacklistedAt": "2026-03-15T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

### 13.4 Remove from Blacklist

```
DELETE /api/v1/admin/blacklist/01087654321
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إزالة الرقم من القائمة السوداء بنجاح",
  "errors": null
}
```

### 13.5 Get Blacklist Statistics

```
GET /api/v1/admin/blacklist/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalBlacklisted": 34,
    "addedThisMonth": 5,
    "removedThisMonth": 2,
    "topReasons": [
      { "reason": "Fraud", "count": 18 },
      { "reason": "Abuse", "count": 10 },
      { "reason": "Spam", "count": 6 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 13.6 Get Blacklist Reports

```
GET /api/v1/admin/blacklist/reports
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "phone": "01098765432",
      "reportedBy": "Driver - احمد محمد",
      "reportType": "Fraud",
      "description": "العميل أعطى عنوان وهمي مرتين",
      "reportedAt": "2026-03-25T18:00:00Z",
      "status": "Reviewed"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 14. AdminSettlementsController

**Route Prefix**: `/api/v1/admin/settlements`

Process and manage financial settlements with partners and drivers. Approve or reject pending settlements and view financial summaries.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/settlements` | Get all settlements (filtered + paginated) |
| GET | `/admin/settlements/{id}` | Get settlement details |
| POST | `/admin/settlements/{id}/approve` | Approve a settlement |
| POST | `/admin/settlements/{id}/reject` | Reject a settlement |
| GET | `/admin/settlements/summary` | Get settlements summary |

### 14.1 Get All Settlements

```
GET /api/v1/admin/settlements?pageNumber=1&pageSize=10&status=Pending
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Pending`, `Approved`, `Rejected`, `Paid` |
| fromDate | DateTime | No | Start date filter |
| toDate | DateTime | No | End date filter |
| partnerId | Guid | No | Filter by partner |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "stl-a1b2c3d4",
        "partnerName": "مطعم السلطان",
        "amount": 5250.00,
        "commission": 656.25,
        "netAmount": 4593.75,
        "ordersCount": 42,
        "periodFrom": "2026-03-16",
        "periodTo": "2026-03-30",
        "status": "Pending",
        "createdAt": "2026-03-30T10:00:00Z"
      }
    ],
    "totalCount": 15,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 14.2 Get Settlement by ID

```
GET /api/v1/admin/settlements/stl-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "stl-a1b2c3d4",
    "partnerId": "part-e5f6a7b8",
    "partnerName": "مطعم السلطان",
    "bankAccount": "EG****7890",
    "amount": 5250.00,
    "commission": 656.25,
    "commissionRate": 12.5,
    "netAmount": 4593.75,
    "ordersCount": 42,
    "periodFrom": "2026-03-16",
    "periodTo": "2026-03-30",
    "status": "Pending",
    "orders": [
      {
        "orderId": "ord-001",
        "orderNumber": "ORD-20260320-0012",
        "amount": 125.00,
        "commission": 15.63,
        "date": "2026-03-20"
      }
    ],
    "createdAt": "2026-03-30T10:00:00Z",
    "approvedAt": null,
    "approvedBy": null,
    "rejectedAt": null,
    "rejectionReason": null
  },
  "message": null,
  "errors": null
}
```

### 14.3 Approve Settlement

```
POST /api/v1/admin/settlements/stl-a1b2c3d4/approve
```

**Request:**
```json
{
  "notes": "تم التحقق من جميع الطلبات"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم اعتماد التسوية بنجاح",
  "errors": null
}
```

### 14.4 Reject Settlement

```
POST /api/v1/admin/settlements/stl-a1b2c3d4/reject
```

**Request:**
```json
{
  "reason": "يوجد طلبات مرتجعة لم يتم احتسابها"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم رفض التسوية",
  "errors": null
}
```

### 14.5 Get Settlements Summary

```
GET /api/v1/admin/settlements/summary?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalSettlements": 45,
    "totalAmount": 125000.00,
    "totalCommission": 15625.00,
    "totalNetAmount": 109375.00,
    "pendingCount": 8,
    "pendingAmount": 18500.00,
    "approvedCount": 32,
    "approvedAmount": 95000.00,
    "rejectedCount": 5,
    "rejectedAmount": 11500.00
  },
  "message": null,
  "errors": null
}
```

---

## 15. AdminPaymentController

**Route Prefix**: `/api/v1/admin/payments`

Manage payment transactions including approval workflow for pending payments and financial summaries.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/payments` | Get all payments (filtered + paginated) |
| GET | `/admin/payments/{id}` | Get payment details |
| POST | `/admin/payments/{id}/approve` | Approve a payment |
| POST | `/admin/payments/{id}/reject` | Reject a payment |
| GET | `/admin/payments/pending` | Get all pending payments |
| GET | `/admin/payments/summary` | Get payment summary |

### 15.1 Get All Payments

```
GET /api/v1/admin/payments?pageNumber=1&pageSize=10&status=Pending&method=CashOnDelivery
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Pending`, `Approved`, `Rejected`, `Completed` |
| method | string | No | `CashOnDelivery`, `Wallet`, `Card`, `BankTransfer` |
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "pay-a1b2c3d4",
        "orderId": "ord-001",
        "orderNumber": "ORD-20260330-0042",
        "amount": 125.50,
        "method": "CashOnDelivery",
        "status": "Pending",
        "payerName": "سارة أحمد",
        "payerPhone": "01234567890",
        "createdAt": "2026-03-30T14:00:00Z"
      }
    ],
    "totalCount": 85,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 15.2 Get Payment by ID

```
GET /api/v1/admin/payments/pay-a1b2c3d4
```

### 15.3 Approve Payment

```
POST /api/v1/admin/payments/pay-a1b2c3d4/approve
```

**Request:**
```json
{
  "notes": "تم التحقق من الدفع"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم اعتماد الدفعة بنجاح",
  "errors": null
}
```

### 15.4 Reject Payment

```
POST /api/v1/admin/payments/pay-a1b2c3d4/reject
```

**Request:**
```json
{
  "reason": "مبلغ الدفع لا يتطابق مع قيمة الطلب"
}
```

### 15.5 Get Pending Payments

```
GET /api/v1/admin/payments/pending
```

### 15.6 Get Payment Summary

```
GET /api/v1/admin/payments/summary?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalPayments": 1250,
    "totalAmount": 156250.00,
    "pendingCount": 18,
    "pendingAmount": 2250.00,
    "approvedCount": 1200,
    "approvedAmount": 150000.00,
    "rejectedCount": 32,
    "rejectedAmount": 4000.00,
    "byMethod": [
      { "method": "CashOnDelivery", "count": 780, "amount": 97500.00 },
      { "method": "Wallet", "count": 320, "amount": 40000.00 },
      { "method": "Card", "count": 150, "amount": 18750.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 16. AdminWalletController

**Route Prefix**: `/api/v1/admin/wallets`

Full driver wallet management including balance viewing, manual adjustments, freeze/unfreeze operations, bulk operations, and export capabilities.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/wallets/driver/{driverId}` | Get driver wallet info |
| GET | `/admin/wallets/driver/{driverId}/transactions` | Get driver wallet transactions |
| POST | `/admin/wallets/driver/{driverId}/adjust` | Adjust driver wallet balance |
| POST | `/admin/wallets/driver/{driverId}/freeze` | Freeze driver wallet |
| DELETE | `/admin/wallets/driver/{driverId}/freeze` | Unfreeze driver wallet |
| GET | `/admin/wallets/frozen` | Get all frozen wallets |
| POST | `/admin/wallets/bulk-adjust` | Bulk adjust multiple wallets |
| GET | `/admin/wallets/high-balance` | Get high-balance wallets |
| GET | `/admin/wallets/stats` | Get wallet statistics |
| GET | `/admin/wallets/export` | Export wallet data |

### 16.1 Get Driver Wallet

```
GET /api/v1/admin/wallets/driver/d1a2b3c4-e5f6-7890-abcd-ef1234567890
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
    "driverName": "احمد محمد",
    "balance": 1250.00,
    "pendingBalance": 350.00,
    "totalEarnings": 45000.00,
    "totalWithdrawals": 43750.00,
    "isFrozen": false,
    "frozenReason": null,
    "lastTransactionAt": "2026-03-30T14:00:00Z"
  },
  "message": null,
  "errors": null
}
```

### 16.2 Get Driver Transactions

```
GET /api/v1/admin/wallets/driver/d1a2b3c4-e5f6-7890-abcd-ef1234567890/transactions?pageNumber=1&pageSize=20
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "txn-001",
        "type": "Credit",
        "amount": 45.00,
        "balanceAfter": 1250.00,
        "description": "أرباح طلب ORD-20260330-0042",
        "referenceId": "ord-a1b2c3d4",
        "referenceType": "Order",
        "createdAt": "2026-03-30T14:00:00Z"
      },
      {
        "id": "txn-002",
        "type": "Debit",
        "amount": 500.00,
        "balanceAfter": 1205.00,
        "description": "سحب رصيد",
        "referenceId": null,
        "referenceType": "Withdrawal",
        "createdAt": "2026-03-29T16:00:00Z"
      }
    ],
    "totalCount": 250,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 13,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 16.3 Adjust Wallet Balance

```
POST /api/v1/admin/wallets/driver/d1a2b3c4-e5f6-7890-abcd-ef1234567890/adjust
```

**Request:**
```json
{
  "amount": 100.00,
  "type": "Credit",
  "reason": "مكافأة أداء - أفضل سائق في الأسبوع"
}
```

> **Note:** Use `type: "Debit"` for deductions.

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "newBalance": 1350.00,
    "transactionId": "txn-adj001"
  },
  "message": "تم تعديل الرصيد بنجاح",
  "errors": null
}
```

### 16.4 Freeze Wallet

```
POST /api/v1/admin/wallets/driver/d1a2b3c4-e5f6-7890-abcd-ef1234567890/freeze
```

**Request:**
```json
{
  "reason": "تحقيق في معاملات مشبوهة"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تجميد المحفظة بنجاح",
  "errors": null
}
```

### 16.5 Unfreeze Wallet

```
DELETE /api/v1/admin/wallets/driver/d1a2b3c4-e5f6-7890-abcd-ef1234567890/freeze
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إلغاء تجميد المحفظة بنجاح",
  "errors": null
}
```

### 16.6 Get Frozen Wallets

```
GET /api/v1/admin/wallets/frozen
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "d3c4d5e6",
      "driverName": "علي حسن",
      "balance": 2500.00,
      "frozenReason": "تحقيق في معاملات مشبوهة",
      "frozenAt": "2026-03-28T10:00:00Z",
      "frozenBy": "Admin User"
    }
  ],
  "message": null,
  "errors": null
}
```

### 16.7 Bulk Adjust Wallets

```
POST /api/v1/admin/wallets/bulk-adjust
```

**Request:**
```json
{
  "adjustments": [
    {
      "driverId": "d1a2b3c4",
      "amount": 50.00,
      "type": "Credit"
    },
    {
      "driverId": "d2b3c4d5",
      "amount": 50.00,
      "type": "Credit"
    }
  ],
  "reason": "مكافأة عيد الأضحى"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalAdjusted": 2,
    "totalAmount": 100.00,
    "failed": 0
  },
  "message": "تم تعديل الأرصدة بنجاح",
  "errors": null
}
```

### 16.8 Get High-Balance Wallets

```
GET /api/v1/admin/wallets/high-balance?threshold=5000
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "d5e6f7a8",
      "driverName": "محمد إبراهيم",
      "balance": 7500.00,
      "lastWithdrawalAt": "2026-03-15T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 16.9 Get Wallet Statistics

```
GET /api/v1/admin/wallets/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalWallets": 85,
    "totalBalance": 125000.00,
    "averageBalance": 1470.59,
    "frozenWallets": 3,
    "frozenBalance": 8500.00,
    "totalTransactionsToday": 342,
    "totalCreditToday": 15000.00,
    "totalDebitToday": 12500.00
  },
  "message": null,
  "errors": null
}
```

### 16.10 Export Wallet Data

```
GET /api/v1/admin/wallets/export?format=csv
```

**Response:** File download

---

## 17. AdminDisputesController

**Route Prefix**: `/api/v1/admin/disputes`

Handle order disputes including resolution, rejection, escalation, statistics, and data export.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/disputes` | Get all disputes (filtered + paginated) |
| GET | `/admin/disputes/{id}` | Get dispute details |
| PUT | `/admin/disputes/{id}/resolve` | Resolve a dispute |
| PUT | `/admin/disputes/{id}/reject` | Reject a dispute |
| PUT | `/admin/disputes/{id}/escalate` | Escalate a dispute |
| GET | `/admin/disputes/stats` | Get dispute statistics |
| GET | `/admin/disputes/export` | Export disputes data |

### 17.1 Get All Disputes

```
GET /api/v1/admin/disputes?pageNumber=1&pageSize=10&status=Open&priority=High
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Open`, `InReview`, `Resolved`, `Rejected`, `Escalated` |
| priority | string | No | `Low`, `Medium`, `High`, `Critical` |
| type | string | No | `Payment`, `Delivery`, `Quality`, `Other` |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "dsp-a1b2c3d4",
        "orderId": "ord-001",
        "orderNumber": "ORD-20260328-0018",
        "type": "Delivery",
        "priority": "High",
        "status": "Open",
        "description": "الطلب وصل متأخر ساعة كاملة",
        "filedBy": "سارة أحمد",
        "filedByRole": "Customer",
        "filedAt": "2026-03-28T20:00:00Z",
        "amount": 125.00
      }
    ],
    "totalCount": 23,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 17.2 Get Dispute by ID

```
GET /api/v1/admin/disputes/dsp-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "dsp-a1b2c3d4",
    "orderId": "ord-001",
    "orderNumber": "ORD-20260328-0018",
    "type": "Delivery",
    "priority": "High",
    "status": "Open",
    "description": "الطلب وصل متأخر ساعة كاملة",
    "filedBy": "سارة أحمد",
    "filedByRole": "Customer",
    "filedAt": "2026-03-28T20:00:00Z",
    "amount": 125.00,
    "evidence": [
      { "type": "Screenshot", "url": "https://storage.sekka.com/evidence/ev001.jpg" }
    ],
    "messages": [
      {
        "sender": "سارة أحمد",
        "role": "Customer",
        "message": "الطلب تأخر أكثر من ساعة",
        "sentAt": "2026-03-28T20:00:00Z"
      }
    ],
    "orderDetails": {
      "customerName": "سارة أحمد",
      "driverName": "احمد محمد",
      "partnerName": "مطعم السلطان",
      "totalAmount": 125.00,
      "deliveryAddress": "28 شارع الملك فيصل، الجيزة",
      "estimatedDelivery": "2026-03-28T19:00:00Z",
      "actualDelivery": "2026-03-28T20:05:00Z"
    }
  },
  "message": null,
  "errors": null
}
```

### 17.3 Resolve Dispute

```
PUT /api/v1/admin/disputes/dsp-a1b2c3d4/resolve
```

**Request:**
```json
{
  "resolution": "تعويض العميل بخصم 50% على الطلب القادم",
  "refundAmount": 62.50,
  "compensationType": "PartialRefund",
  "notes": "تم التحقق من التأخير عبر سجل الطلب"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم حل النزاع بنجاح",
  "errors": null
}
```

### 17.4 Reject Dispute

```
PUT /api/v1/admin/disputes/dsp-a1b2c3d4/reject
```

**Request:**
```json
{
  "reason": "لم يتم تقديم دليل كافٍ"
}
```

### 17.5 Escalate Dispute

```
PUT /api/v1/admin/disputes/dsp-a1b2c3d4/escalate
```

**Request:**
```json
{
  "reason": "مبلغ النزاع يتجاوز الحد المسموح",
  "escalateTo": "SeniorAdmin"
}
```

### 17.6 Get Dispute Statistics

```
GET /api/v1/admin/disputes/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalDisputes": 145,
    "openDisputes": 12,
    "inReviewDisputes": 8,
    "resolvedDisputes": 110,
    "rejectedDisputes": 15,
    "averageResolutionTimeHours": 4.5,
    "totalRefundAmount": 8500.00,
    "byType": [
      { "type": "Delivery", "count": 65 },
      { "type": "Payment", "count": 42 },
      { "type": "Quality", "count": 28 },
      { "type": "Other", "count": 10 }
    ],
    "byPriority": [
      { "priority": "Critical", "count": 5 },
      { "priority": "High", "count": 25 },
      { "priority": "Medium", "count": 75 },
      { "priority": "Low", "count": 40 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 17.7 Export Disputes

```
GET /api/v1/admin/disputes/export?format=csv&fromDate=2026-03-01&toDate=2026-03-30
```

**Response:** File download

---

## 18. AdminInvoiceController

**Route Prefix**: `/api/v1/admin/invoices`

Generate and manage invoices for partners and drivers. Supports individual and bulk invoice generation, status management, and export.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/invoices` | Get all invoices (filtered + paginated) |
| GET | `/admin/invoices/{id}` | Get invoice details |
| POST | `/admin/invoices/generate` | Generate a single invoice |
| POST | `/admin/invoices/generate-bulk` | Generate invoices in bulk |
| PUT | `/admin/invoices/{id}/status` | Update invoice status |
| GET | `/admin/invoices/stats` | Get invoice statistics |
| GET | `/admin/invoices/export` | Export invoices |

### 18.1 Get All Invoices

```
GET /api/v1/admin/invoices?pageNumber=1&pageSize=10&status=Unpaid
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Draft`, `Unpaid`, `Paid`, `Overdue`, `Cancelled` |
| type | string | No | `Partner`, `Driver` |
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "inv-a1b2c3d4",
        "invoiceNumber": "INV-2026-0342",
        "type": "Partner",
        "recipientName": "مطعم السلطان",
        "amount": 5250.00,
        "taxAmount": 787.50,
        "totalAmount": 6037.50,
        "status": "Unpaid",
        "issuedAt": "2026-03-30T10:00:00Z",
        "dueDate": "2026-04-15"
      }
    ],
    "totalCount": 125,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 13,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 18.2 Get Invoice by ID

```
GET /api/v1/admin/invoices/inv-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "inv-a1b2c3d4",
    "invoiceNumber": "INV-2026-0342",
    "type": "Partner",
    "recipientId": "part-e5f6a7b8",
    "recipientName": "مطعم السلطان",
    "recipientPhone": "01512345678",
    "recipientAddress": "45 شارع الهرم، الجيزة",
    "taxId": "TAX-67890",
    "lineItems": [
      {
        "description": "عمولة توصيل - 16 مارس إلى 30 مارس",
        "quantity": 42,
        "unitPrice": 125.00,
        "amount": 5250.00
      }
    ],
    "subtotal": 5250.00,
    "taxRate": 15.0,
    "taxAmount": 787.50,
    "totalAmount": 6037.50,
    "status": "Unpaid",
    "issuedAt": "2026-03-30T10:00:00Z",
    "dueDate": "2026-04-15",
    "paidAt": null,
    "notes": null
  },
  "message": null,
  "errors": null
}
```

### 18.3 Generate Invoice

```
POST /api/v1/admin/invoices/generate
```

**Request:**
```json
{
  "type": "Partner",
  "recipientId": "part-e5f6a7b8",
  "periodFrom": "2026-03-16",
  "periodTo": "2026-03-30",
  "dueDate": "2026-04-15",
  "notes": "فاتورة النصف الثاني من مارس"
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "invoiceId": "inv-new001",
    "invoiceNumber": "INV-2026-0343"
  },
  "message": "تم إنشاء الفاتورة بنجاح",
  "errors": null
}
```

### 18.4 Generate Bulk Invoices

```
POST /api/v1/admin/invoices/generate-bulk
```

**Request:**
```json
{
  "type": "Partner",
  "periodFrom": "2026-03-16",
  "periodTo": "2026-03-30",
  "dueDate": "2026-04-15",
  "partnerIds": null
}
```

> **Note:** If `partnerIds` is `null`, invoices are generated for all active partners.

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalGenerated": 38,
    "totalAmount": 185000.00,
    "failed": 0
  },
  "message": "تم إنشاء الفواتير بنجاح",
  "errors": null
}
```

### 18.5 Update Invoice Status

```
PUT /api/v1/admin/invoices/inv-a1b2c3d4/status
```

**Request:**
```json
{
  "status": "Paid",
  "notes": "تم الدفع عبر تحويل بنكي"
}
```

### 18.6 Get Invoice Statistics

```
GET /api/v1/admin/invoices/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalInvoices": 342,
    "totalAmount": 525000.00,
    "unpaidCount": 45,
    "unpaidAmount": 67500.00,
    "overdueCount": 8,
    "overdueAmount": 12000.00,
    "paidThisMonth": 95,
    "paidAmountThisMonth": 142500.00,
    "averagePaymentDays": 12.5
  },
  "message": null,
  "errors": null
}
```

### 18.7 Export Invoices

```
GET /api/v1/admin/invoices/export?format=csv&fromDate=2026-03-01&toDate=2026-03-30
```

**Response:** File download

---

## 19. AdminRefundController

**Route Prefix**: `/api/v1/admin/refunds`

Process refund requests with approval workflow, statistics tracking, and export capabilities.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/refunds` | Get all refunds (filtered + paginated) |
| GET | `/admin/refunds/{id}` | Get refund details |
| PUT | `/admin/refunds/{id}/approve` | Approve a refund |
| PUT | `/admin/refunds/{id}/reject` | Reject a refund |
| GET | `/admin/refunds/stats` | Get refund statistics |
| GET | `/admin/refunds/export` | Export refunds data |

### 19.1 Get All Refunds

```
GET /api/v1/admin/refunds?pageNumber=1&pageSize=10&status=Pending
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Pending`, `Approved`, `Rejected`, `Processed` |
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "ref-a1b2c3d4",
        "orderId": "ord-001",
        "orderNumber": "ORD-20260328-0018",
        "customerName": "سارة أحمد",
        "amount": 125.00,
        "reason": "طلب ملغى بعد التأخير",
        "status": "Pending",
        "requestedAt": "2026-03-28T20:30:00Z"
      }
    ],
    "totalCount": 18,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 19.2 Get Refund by ID

```
GET /api/v1/admin/refunds/ref-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "ref-a1b2c3d4",
    "orderId": "ord-001",
    "orderNumber": "ORD-20260328-0018",
    "customerId": "cust-a1b2c3d4",
    "customerName": "سارة أحمد",
    "customerPhone": "01234567890",
    "amount": 125.00,
    "originalPaymentMethod": "Card",
    "refundMethod": "OriginalMethod",
    "reason": "طلب ملغى بعد التأخير",
    "status": "Pending",
    "requestedAt": "2026-03-28T20:30:00Z",
    "approvedAt": null,
    "approvedBy": null,
    "processedAt": null,
    "rejectedAt": null,
    "rejectionReason": null
  },
  "message": null,
  "errors": null
}
```

### 19.3 Approve Refund

```
PUT /api/v1/admin/refunds/ref-a1b2c3d4/approve
```

**Request:**
```json
{
  "notes": "تم التحقق من سبب الإلغاء - التأخير مؤكد",
  "adjustedAmount": null
}
```

> **Note:** Set `adjustedAmount` to approve a partial refund, or `null` for full refund.

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم اعتماد الاسترداد بنجاح",
  "errors": null
}
```

### 19.4 Reject Refund

```
PUT /api/v1/admin/refunds/ref-a1b2c3d4/reject
```

**Request:**
```json
{
  "reason": "الطلب تم توصيله بنجاح وفقاً لسجل التتبع"
}
```

### 19.5 Get Refund Statistics

```
GET /api/v1/admin/refunds/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalRefunds": 85,
    "totalRefundAmount": 10625.00,
    "pendingCount": 8,
    "pendingAmount": 1000.00,
    "approvedCount": 62,
    "approvedAmount": 7750.00,
    "rejectedCount": 15,
    "rejectedAmount": 1875.00,
    "averageRefundAmount": 125.00,
    "refundRate": 6.8,
    "topReasons": [
      { "reason": "Late delivery", "count": 35 },
      { "reason": "Wrong items", "count": 22 },
      { "reason": "Damaged items", "count": 18 },
      { "reason": "Other", "count": 10 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 19.6 Export Refunds

```
GET /api/v1/admin/refunds/export?format=csv&fromDate=2026-03-01&toDate=2026-03-30
```

**Response:** File download

---

## 20. AdminStatisticsController

**Route Prefix**: `/api/v1/admin/statistics`

Comprehensive platform analytics covering daily/weekly/monthly metrics, driver rankings, revenue analysis, regional data, growth tracking, and real-time dashboards.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/statistics/platform` | Get platform overview stats |
| GET | `/admin/statistics/platform/daily` | Get daily statistics |
| GET | `/admin/statistics/platform/weekly` | Get weekly statistics |
| GET | `/admin/statistics/platform/monthly` | Get monthly statistics |
| GET | `/admin/statistics/drivers/ranking` | Get driver rankings |
| GET | `/admin/statistics/drivers/{driverId}` | Get individual driver stats |
| GET | `/admin/statistics/revenue` | Get revenue statistics |
| GET | `/admin/statistics/revenue/breakdown` | Get revenue breakdown |
| GET | `/admin/statistics/orders/status-breakdown` | Get orders by status |
| GET | `/admin/statistics/orders/hourly` | Get hourly order distribution |
| GET | `/admin/statistics/regions` | Get regional statistics |
| GET | `/admin/statistics/regions/heatmap` | Get regions heatmap data |
| GET | `/admin/statistics/cancellations` | Get cancellation analytics |
| GET | `/admin/statistics/customers/top` | Get top customers |
| GET | `/admin/statistics/partners/top` | Get top partners |
| GET | `/admin/statistics/growth` | Get growth metrics |
| GET | `/admin/statistics/delivery-performance` | Get delivery performance metrics |
| GET | `/admin/statistics/financial-summary` | Get financial summary |
| GET | `/admin/statistics/export` | Export statistics |
| GET | `/admin/statistics/realtime` | Get real-time dashboard data |

### 20.1 Get Platform Overview

```
GET /api/v1/admin/statistics/platform
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalOrders": 15420,
    "totalRevenue": 1925000.00,
    "totalDrivers": 85,
    "activeDrivers": 62,
    "totalCustomers": 1250,
    "activeCustomers": 980,
    "totalPartners": 42,
    "averageRating": 4.5,
    "averageDeliveryTimeMinutes": 32.5,
    "completionRate": 94.2,
    "todayOrders": 145,
    "todayRevenue": 18125.00
  },
  "message": null,
  "errors": null
}
```

### 20.2 Get Daily Statistics

```
GET /api/v1/admin/statistics/platform/daily?date=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-30",
    "totalOrders": 145,
    "completedOrders": 132,
    "cancelledOrders": 8,
    "pendingOrders": 5,
    "revenue": 18125.00,
    "averageOrderValue": 125.00,
    "activeDrivers": 52,
    "newCustomers": 8,
    "averageDeliveryTimeMinutes": 30.2,
    "peakHour": "19:00",
    "peakHourOrders": 22
  },
  "message": null,
  "errors": null
}
```

### 20.3 Get Weekly Statistics

```
GET /api/v1/admin/statistics/platform/weekly?startDate=2026-03-24
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "weekStart": "2026-03-24",
    "weekEnd": "2026-03-30",
    "totalOrders": 980,
    "revenue": 122500.00,
    "averageOrdersPerDay": 140,
    "growthVsLastWeek": 5.2,
    "dailyBreakdown": [
      { "date": "2026-03-24", "orders": 128, "revenue": 16000.00 },
      { "date": "2026-03-25", "orders": 135, "revenue": 16875.00 },
      { "date": "2026-03-26", "orders": 142, "revenue": 17750.00 },
      { "date": "2026-03-27", "orders": 150, "revenue": 18750.00 },
      { "date": "2026-03-28", "orders": 138, "revenue": 17250.00 },
      { "date": "2026-03-29", "orders": 142, "revenue": 17750.00 },
      { "date": "2026-03-30", "orders": 145, "revenue": 18125.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 20.4 Get Monthly Statistics

```
GET /api/v1/admin/statistics/platform/monthly?year=2026&month=3
```

### 20.5 Get Driver Rankings

```
GET /api/v1/admin/statistics/drivers/ranking?metric=completionRate&limit=10
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| metric | string | No | `completionRate`, `rating`, `totalOrders`, `earnings` (default: `completionRate`) |
| limit | int | No | Number of drivers to return (default: 10) |
| period | string | No | `week`, `month`, `all` (default: `month`) |

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "rank": 1,
      "driverId": "d1a2b3c4",
      "driverName": "احمد محمد",
      "totalOrders": 87,
      "completionRate": 98.9,
      "rating": 4.9,
      "earnings": 4350.00
    },
    {
      "rank": 2,
      "driverId": "d2b3c4d5",
      "driverName": "محمد علي",
      "totalOrders": 82,
      "completionRate": 97.6,
      "rating": 4.8,
      "earnings": 4100.00
    }
  ],
  "message": null,
  "errors": null
}
```

### 20.6 Get Individual Driver Stats

```
GET /api/v1/admin/statistics/drivers/d1a2b3c4?period=month
```

### 20.7 Get Revenue Statistics

```
GET /api/v1/admin/statistics/revenue?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalRevenue": 485000.00,
    "deliveryFeeRevenue": 145500.00,
    "commissionRevenue": 60625.00,
    "subscriptionRevenue": 21528.00,
    "otherRevenue": 5000.00,
    "totalExpenses": 125000.00,
    "netProfit": 360000.00,
    "growthVsLastPeriod": 8.5,
    "dailyAverage": 16166.67,
    "projectedMonthly": 495000.00
  },
  "message": null,
  "errors": null
}
```

### 20.8 Get Revenue Breakdown

```
GET /api/v1/admin/statistics/revenue/breakdown?period=month
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "bySource": [
      { "source": "DeliveryFees", "amount": 145500.00, "percentage": 30.0 },
      { "source": "Commissions", "amount": 60625.00, "percentage": 12.5 },
      { "source": "Subscriptions", "amount": 21528.00, "percentage": 4.4 },
      { "source": "OrderValues", "amount": 252347.00, "percentage": 52.0 },
      { "source": "Other", "amount": 5000.00, "percentage": 1.1 }
    ],
    "byRegion": [
      { "region": "القاهرة", "amount": 250000.00, "percentage": 51.5 },
      { "region": "الجيزة", "amount": 125000.00, "percentage": 25.8 },
      { "region": "الإسكندرية", "amount": 75000.00, "percentage": 15.5 }
    ],
    "byPartnerCategory": [
      { "category": "Restaurant", "amount": 280000.00, "percentage": 57.7 },
      { "category": "Grocery", "amount": 125000.00, "percentage": 25.8 },
      { "category": "Pharmacy", "amount": 80000.00, "percentage": 16.5 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 20.9 Get Orders Status Breakdown

```
GET /api/v1/admin/statistics/orders/status-breakdown?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "total": 4250,
    "byStatus": [
      { "status": "Delivered", "count": 3990, "percentage": 93.9 },
      { "status": "Cancelled", "count": 170, "percentage": 4.0 },
      { "status": "Returned", "count": 42, "percentage": 1.0 },
      { "status": "InProgress", "count": 28, "percentage": 0.7 },
      { "status": "Pending", "count": 12, "percentage": 0.3 },
      { "status": "Assigned", "count": 8, "percentage": 0.2 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 20.10 Get Hourly Order Distribution

```
GET /api/v1/admin/statistics/orders/hourly?date=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-30",
    "hours": [
      { "hour": "08:00", "orders": 5 },
      { "hour": "09:00", "orders": 8 },
      { "hour": "10:00", "orders": 12 },
      { "hour": "11:00", "orders": 15 },
      { "hour": "12:00", "orders": 22 },
      { "hour": "13:00", "orders": 18 },
      { "hour": "14:00", "orders": 10 },
      { "hour": "15:00", "orders": 8 },
      { "hour": "16:00", "orders": 6 },
      { "hour": "17:00", "orders": 10 },
      { "hour": "18:00", "orders": 18 },
      { "hour": "19:00", "orders": 22 },
      { "hour": "20:00", "orders": 20 },
      { "hour": "21:00", "orders": 15 },
      { "hour": "22:00", "orders": 8 },
      { "hour": "23:00", "orders": 3 }
    ],
    "peakHour": "12:00",
    "quietHour": "23:00"
  },
  "message": null,
  "errors": null
}
```

### 20.11 Get Regional Statistics

```
GET /api/v1/admin/statistics/regions
```

### 20.12 Get Regions Heatmap

```
GET /api/v1/admin/statistics/regions/heatmap
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "latitude": 30.0444,
      "longitude": 31.2357,
      "intensity": 0.95,
      "ordersCount": 580,
      "regionName": "وسط القاهرة"
    },
    {
      "latitude": 30.0626,
      "longitude": 31.2497,
      "intensity": 0.72,
      "ordersCount": 420,
      "regionName": "مدينة نصر"
    }
  ],
  "message": null,
  "errors": null
}
```

### 20.13 Get Cancellation Analytics

```
GET /api/v1/admin/statistics/cancellations?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalCancellations": 170,
    "cancellationRate": 4.0,
    "byReason": [
      { "reason": "Customer cancelled", "count": 85, "percentage": 50.0 },
      { "reason": "Driver unavailable", "count": 42, "percentage": 24.7 },
      { "reason": "Partner closed", "count": 25, "percentage": 14.7 },
      { "reason": "Other", "count": 18, "percentage": 10.6 }
    ],
    "byInitiator": [
      { "initiator": "Customer", "count": 100 },
      { "initiator": "Driver", "count": 45 },
      { "initiator": "System", "count": 15 },
      { "initiator": "Admin", "count": 10 }
    ],
    "trend": [
      { "date": "2026-03-01", "cancellations": 4 },
      { "date": "2026-03-02", "cancellations": 6 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 20.14 Get Top Customers

```
GET /api/v1/admin/statistics/customers/top?limit=10&metric=totalSpent
```

### 20.15 Get Top Partners

```
GET /api/v1/admin/statistics/partners/top?limit=10&metric=revenue
```

### 20.16 Get Growth Metrics

```
GET /api/v1/admin/statistics/growth?period=monthly
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "period": "monthly",
    "metrics": [
      {
        "month": "2026-01",
        "newCustomers": 120,
        "newDrivers": 12,
        "newPartners": 5,
        "orders": 3800,
        "revenue": 475000.00
      },
      {
        "month": "2026-02",
        "newCustomers": 105,
        "newDrivers": 8,
        "newPartners": 3,
        "orders": 3950,
        "revenue": 493750.00
      },
      {
        "month": "2026-03",
        "newCustomers": 85,
        "newDrivers": 10,
        "newPartners": 4,
        "orders": 4250,
        "revenue": 531250.00
      }
    ],
    "customerGrowthRate": 8.5,
    "revenueGrowthRate": 7.6,
    "orderGrowthRate": 7.6
  },
  "message": null,
  "errors": null
}
```

### 20.17 Get Delivery Performance

```
GET /api/v1/admin/statistics/delivery-performance?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "averageDeliveryTimeMinutes": 32.5,
    "onTimeRate": 88.5,
    "lateRate": 11.5,
    "averagePickupTimeMinutes": 8.2,
    "averageTransitTimeMinutes": 24.3,
    "byTimeOfDay": [
      { "period": "Morning (8-12)", "avgMinutes": 28.5 },
      { "period": "Afternoon (12-17)", "avgMinutes": 35.2 },
      { "period": "Evening (17-22)", "avgMinutes": 32.8 },
      { "period": "Night (22-8)", "avgMinutes": 22.1 }
    ],
    "byVehicleType": [
      { "type": "Motorcycle", "avgMinutes": 25.5 },
      { "type": "Car", "avgMinutes": 35.2 },
      { "type": "Van", "avgMinutes": 42.8 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 20.18 Get Financial Summary

```
GET /api/v1/admin/statistics/financial-summary?period=month
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalIncome": 531250.00,
    "totalExpenses": 125000.00,
    "netProfit": 406250.00,
    "profitMargin": 76.5,
    "pendingSettlements": 18500.00,
    "pendingRefunds": 2250.00,
    "walletBalances": 125000.00,
    "subscriptionRevenue": 21528.00,
    "commissionRevenue": 66406.25
  },
  "message": null,
  "errors": null
}
```

### 20.19 Export Statistics

```
GET /api/v1/admin/statistics/export?type=revenue&format=csv&fromDate=2026-03-01&toDate=2026-03-30
```

**Response:** File download

### 20.20 Get Real-Time Dashboard

```
GET /api/v1/admin/statistics/realtime
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "activeOrders": 28,
    "pendingOrders": 5,
    "onlineDrivers": 52,
    "availableDrivers": 18,
    "ordersLastHour": 22,
    "revenueLastHour": 2750.00,
    "averageWaitTimeMinutes": 4.2,
    "activeSOS": 0,
    "systemLoad": "Normal"
  },
  "message": null,
  "errors": null
}
```

---

## 21. AdminNotificationsController

**Route Prefix**: `/api/v1/admin/notifications`

Send push notifications to all users or specific drivers. View notification delivery history.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/admin/notifications/broadcast` | Broadcast notification to all users |
| GET | `/admin/notifications/history` | Get notification history |
| POST | `/admin/notifications/send-to-driver` | Send notification to specific driver |

### 21.1 Broadcast Notification

```
POST /api/v1/admin/notifications/broadcast
```

**Request:**
```json
{
  "title": "عروض عيد الأضحى",
  "titleAr": "عروض عيد الأضحى",
  "body": "احصل على توصيل مجاني لجميع الطلبات خلال العيد!",
  "bodyAr": "احصل على توصيل مجاني لجميع الطلبات خلال العيد!",
  "targetAudience": "All",
  "data": {
    "type": "Promotion",
    "promoCode": "EID2026"
  },
  "scheduledAt": null
}
```

> **Note:** `targetAudience` can be `All`, `Drivers`, `Customers`, `Partners`.

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "notificationId": "notif-broadcast001",
    "recipientsCount": 1377,
    "deliveredCount": 1350,
    "failedCount": 27
  },
  "message": "تم إرسال الإشعار بنجاح",
  "errors": null
}
```

### 21.2 Get Notification History

```
GET /api/v1/admin/notifications/history?pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "notif-broadcast001",
        "title": "عروض عيد الأضحى",
        "type": "Broadcast",
        "targetAudience": "All",
        "recipientsCount": 1377,
        "deliveredCount": 1350,
        "openRate": 42.5,
        "sentAt": "2026-03-30T10:00:00Z",
        "sentBy": "Admin User"
      }
    ],
    "totalCount": 85,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 21.3 Send to Driver

```
POST /api/v1/admin/notifications/send-to-driver
```

**Request:**
```json
{
  "driverId": "d1a2b3c4-e5f6-7890-abcd-ef1234567890",
  "title": "تحديث مهم",
  "body": "يرجى تحديث بيانات رخصة القيادة قبل انتهاء صلاحيتها",
  "data": {
    "type": "Action",
    "action": "UpdateLicense"
  }
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "delivered": true
  },
  "message": "تم إرسال الإشعار للسائق بنجاح",
  "errors": null
}
```

---

## 22. AdminSOSController

**Route Prefix**: `/api/v1/admin/sos`

Manage emergency SOS alerts from drivers. Track, acknowledge, escalate, resolve, or mark as false alarms. View response time analytics and geographic heatmaps.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/sos/active` | Get all active SOS alerts |
| GET | `/admin/sos` | Get all SOS alerts (filtered + paginated) |
| GET | `/admin/sos/{id}` | Get SOS alert details |
| PUT | `/admin/sos/{id}/acknowledge` | Acknowledge an SOS alert |
| PUT | `/admin/sos/{id}/escalate` | Escalate an SOS alert |
| PUT | `/admin/sos/{id}/resolve` | Resolve an SOS alert |
| PUT | `/admin/sos/{id}/mark-false-alarm` | Mark SOS as false alarm |
| GET | `/admin/sos/stats` | Get SOS statistics |
| GET | `/admin/sos/response-times` | Get SOS response time analytics |
| GET | `/admin/sos/heatmap` | Get SOS geographic heatmap |

### 22.1 Get Active SOS Alerts

```
GET /api/v1/admin/sos/active
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "sos-a1b2c3d4",
      "driverId": "d1a2b3c4",
      "driverName": "احمد محمد",
      "driverPhone": "01012345678",
      "type": "Accident",
      "status": "Active",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "شارع التحرير، وسط البلد",
      "message": "حادث سير - أحتاج مساعدة",
      "orderId": "ord-001",
      "triggeredAt": "2026-03-30T14:15:00Z",
      "minutesElapsed": 5
    }
  ],
  "message": null,
  "errors": null
}
```

### 22.2 Get All SOS Alerts

```
GET /api/v1/admin/sos?pageNumber=1&pageSize=10&status=Active&type=Accident
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Active`, `Acknowledged`, `Escalated`, `Resolved`, `FalseAlarm` |
| type | string | No | `Accident`, `Threat`, `Medical`, `VehicleBreakdown`, `Other` |
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

### 22.3 Get SOS by ID

```
GET /api/v1/admin/sos/sos-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "sos-a1b2c3d4",
    "driverId": "d1a2b3c4",
    "driverName": "احمد محمد",
    "driverPhone": "01012345678",
    "type": "Accident",
    "status": "Active",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "address": "شارع التحرير، وسط البلد",
    "message": "حادث سير - أحتاج مساعدة",
    "orderId": "ord-001",
    "orderNumber": "ORD-20260330-0042",
    "triggeredAt": "2026-03-30T14:15:00Z",
    "acknowledgedAt": null,
    "acknowledgedBy": null,
    "resolvedAt": null,
    "resolution": null,
    "timeline": [
      {
        "event": "SOSTriggered",
        "timestamp": "2026-03-30T14:15:00Z",
        "details": "السائق أطلق إشارة الطوارئ"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

### 22.4 Acknowledge SOS

```
PUT /api/v1/admin/sos/sos-a1b2c3d4/acknowledge
```

**Request:**
```json
{
  "notes": "جاري التواصل مع السائق"
}
```

### 22.5 Escalate SOS

```
PUT /api/v1/admin/sos/sos-a1b2c3d4/escalate
```

**Request:**
```json
{
  "reason": "لم يرد السائق على المكالمات - قد تكون حالة خطرة",
  "escalateTo": "EmergencyServices"
}
```

### 22.6 Resolve SOS

```
PUT /api/v1/admin/sos/sos-a1b2c3d4/resolve
```

**Request:**
```json
{
  "resolution": "تم التواصل مع السائق - حادث بسيط وتم تأمين الموقع",
  "actionsTaken": ["Contacted driver", "Reassigned order", "Notified customer"]
}
```

### 22.7 Mark as False Alarm

```
PUT /api/v1/admin/sos/sos-a1b2c3d4/mark-false-alarm
```

**Request:**
```json
{
  "notes": "السائق ضغط الزر بالخطأ"
}
```

### 22.8 Get SOS Statistics

```
GET /api/v1/admin/sos/stats?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalAlerts": 28,
    "activeAlerts": 1,
    "resolvedAlerts": 22,
    "falseAlarms": 5,
    "averageResponseTimeMinutes": 3.2,
    "byType": [
      { "type": "Accident", "count": 8 },
      { "type": "VehicleBreakdown", "count": 10 },
      { "type": "Threat", "count": 3 },
      { "type": "Medical", "count": 2 },
      { "type": "Other", "count": 5 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 22.9 Get Response Times

```
GET /api/v1/admin/sos/response-times
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "averageAcknowledgeTimeMinutes": 2.1,
    "averageResolutionTimeMinutes": 15.5,
    "p50AcknowledgeMinutes": 1.5,
    "p95AcknowledgeMinutes": 5.0,
    "p50ResolutionMinutes": 12.0,
    "p95ResolutionMinutes": 30.0,
    "byTimeOfDay": [
      { "period": "Day (8-20)", "avgAcknowledge": 1.8, "avgResolve": 12.5 },
      { "period": "Night (20-8)", "avgAcknowledge": 3.5, "avgResolve": 22.0 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 22.10 Get SOS Heatmap

```
GET /api/v1/admin/sos/heatmap
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "latitude": 30.0444,
      "longitude": 31.2357,
      "alertCount": 5,
      "area": "وسط البلد"
    },
    {
      "latitude": 30.0626,
      "longitude": 31.2497,
      "alertCount": 3,
      "area": "مدينة نصر"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 23. AdminVehiclesController

**Route Prefix**: `/api/v1/admin/vehicles`

Manage driver vehicles including approval workflow, maintenance tracking, activation/deactivation, and fleet statistics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/vehicles` | Get all vehicles (filtered + paginated) |
| GET | `/admin/vehicles/{id}` | Get vehicle details |
| POST | `/admin/vehicles/{id}/approve` | Approve a vehicle |
| POST | `/admin/vehicles/{id}/reject` | Reject a vehicle |
| POST | `/admin/vehicles/{id}/flag-maintenance` | Flag vehicle for maintenance |
| POST | `/admin/vehicles/{id}/deactivate` | Deactivate a vehicle |
| POST | `/admin/vehicles/{id}/activate` | Activate a vehicle |
| GET | `/admin/vehicles/pending` | Get vehicles pending approval |
| GET | `/admin/vehicles/maintenance-due` | Get vehicles due for maintenance |
| GET | `/admin/vehicles/stats` | Get vehicle fleet statistics |
| GET | `/admin/vehicles/by-type` | Get vehicles grouped by type |

### 23.1 Get All Vehicles

```
GET /api/v1/admin/vehicles?pageNumber=1&pageSize=10&status=Active&type=Motorcycle
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Active`, `Inactive`, `PendingApproval`, `Maintenance`, `Rejected` |
| type | string | No | `Motorcycle`, `Car`, `Van`, `Truck` |
| search | string | No | Search by plate number or driver name |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "veh-a1b2c3d4",
        "driverId": "d1a2b3c4",
        "driverName": "احمد محمد",
        "type": "Motorcycle",
        "make": "Honda",
        "model": "PCX 150",
        "year": 2024,
        "plateNumber": "أ ب ج 1234",
        "color": "أسود",
        "status": "Active",
        "lastMaintenanceAt": "2026-02-15",
        "nextMaintenanceDue": "2026-05-15",
        "registeredAt": "2025-08-15T10:00:00Z"
      }
    ],
    "totalCount": 85,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 23.2 Get Vehicle by ID

```
GET /api/v1/admin/vehicles/veh-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "veh-a1b2c3d4",
    "driverId": "d1a2b3c4",
    "driverName": "احمد محمد",
    "type": "Motorcycle",
    "make": "Honda",
    "model": "PCX 150",
    "year": 2024,
    "plateNumber": "أ ب ج 1234",
    "color": "أسود",
    "status": "Active",
    "licenseNumber": "LIC-12345",
    "licenseExpiry": "2027-06-15",
    "insuranceExpiry": "2027-01-15",
    "photos": [
      { "type": "Front", "url": "https://storage.sekka.com/vehicles/veh-a1b2c3d4/front.jpg" },
      { "type": "Side", "url": "https://storage.sekka.com/vehicles/veh-a1b2c3d4/side.jpg" },
      { "type": "License", "url": "https://storage.sekka.com/vehicles/veh-a1b2c3d4/license.jpg" }
    ],
    "lastMaintenanceAt": "2026-02-15",
    "nextMaintenanceDue": "2026-05-15",
    "maintenanceHistory": [
      {
        "date": "2026-02-15",
        "type": "Routine",
        "description": "صيانة دورية - تغيير زيت وفلتر",
        "cost": 250.00
      }
    ],
    "registeredAt": "2025-08-15T10:00:00Z",
    "approvedAt": "2025-08-16T10:00:00Z",
    "approvedBy": "Admin User"
  },
  "message": null,
  "errors": null
}
```

### 23.3 Approve Vehicle

```
POST /api/v1/admin/vehicles/veh-pending1/approve
```

**Request:**
```json
{
  "notes": "تم التحقق من جميع المستندات والصور"
}
```

### 23.4 Reject Vehicle

```
POST /api/v1/admin/vehicles/veh-pending1/reject
```

**Request:**
```json
{
  "reason": "صور المركبة غير واضحة - يرجى إعادة الرفع"
}
```

### 23.5 Flag for Maintenance

```
POST /api/v1/admin/vehicles/veh-a1b2c3d4/flag-maintenance
```

**Request:**
```json
{
  "reason": "انتهاء موعد الصيانة الدورية",
  "deactivateUntilMaintained": true
}
```

### 23.6 Deactivate Vehicle

```
POST /api/v1/admin/vehicles/veh-a1b2c3d4/deactivate
```

### 23.7 Activate Vehicle

```
POST /api/v1/admin/vehicles/veh-a1b2c3d4/activate
```

### 23.8 Get Pending Vehicles

```
GET /api/v1/admin/vehicles/pending
```

### 23.9 Get Maintenance Due

```
GET /api/v1/admin/vehicles/maintenance-due
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "vehicleId": "veh-maint1",
      "driverName": "محمد خالد",
      "plateNumber": "د ه و 5678",
      "type": "Motorcycle",
      "nextMaintenanceDue": "2026-04-01",
      "daysUntilDue": 2,
      "lastMaintenanceAt": "2026-01-01"
    }
  ],
  "message": null,
  "errors": null
}
```

### 23.10 Get Vehicle Statistics

```
GET /api/v1/admin/vehicles/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalVehicles": 92,
    "activeVehicles": 78,
    "pendingApproval": 5,
    "inMaintenance": 4,
    "deactivated": 5,
    "maintenanceDueSoon": 8,
    "expiredLicenses": 2,
    "expiredInsurance": 1,
    "byType": [
      { "type": "Motorcycle", "count": 55 },
      { "type": "Car", "count": 25 },
      { "type": "Van", "count": 10 },
      { "type": "Truck", "count": 2 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 23.11 Get Vehicles by Type

```
GET /api/v1/admin/vehicles/by-type
```

---

## 24. AdminConfigController

**Route Prefix**: `/api/v1/admin/config`

Comprehensive platform configuration management including app settings, version control with force update support, maintenance windows, feature flags, and commission rates.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/config/settings` | Get all settings |
| GET | `/admin/config/settings/{key}` | Get setting by key |
| PUT | `/admin/config/settings/{key}` | Update a setting |
| GET | `/admin/config/versions` | Get all app versions |
| POST | `/admin/config/versions` | Create app version |
| PUT | `/admin/config/versions/{id}` | Update app version |
| DELETE | `/admin/config/versions/{id}` | Delete app version |
| PUT | `/admin/config/versions/{id}/force-update` | Set force update for version |
| GET | `/admin/config/maintenance` | Get maintenance windows |
| POST | `/admin/config/maintenance` | Create maintenance window |
| PUT | `/admin/config/maintenance/{id}` | Update maintenance window |
| DELETE | `/admin/config/maintenance/{id}` | Delete maintenance window |
| POST | `/admin/config/maintenance/instant` | Start instant maintenance |
| GET | `/admin/config/feature-flags` | Get all feature flags |
| POST | `/admin/config/feature-flags` | Create feature flag |
| PUT | `/admin/config/feature-flags/{id}` | Update feature flag |
| DELETE | `/admin/config/feature-flags/{id}` | Delete feature flag |
| PUT | `/admin/config/feature-flags/{id}/toggle` | Toggle feature flag |
| GET | `/admin/config/commissions` | Get commission rates |
| PUT | `/admin/config/commissions` | Update commission rates |

### 24.1 Get All Settings

```
GET /api/v1/admin/config/settings
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "key": "MaxOrdersPerDriver",
      "value": "10",
      "description": "Maximum concurrent orders per driver",
      "category": "Orders",
      "lastUpdatedAt": "2026-03-15T10:00:00Z",
      "lastUpdatedBy": "Admin User"
    },
    {
      "key": "DefaultDeliveryRadius",
      "value": "15",
      "description": "Default delivery radius in kilometers",
      "category": "Delivery",
      "lastUpdatedAt": "2026-03-10T14:00:00Z",
      "lastUpdatedBy": "Admin User"
    },
    {
      "key": "OTPExpiryMinutes",
      "value": "5",
      "description": "OTP code expiry in minutes",
      "category": "Auth",
      "lastUpdatedAt": "2026-01-01T10:00:00Z",
      "lastUpdatedBy": "Admin User"
    }
  ],
  "message": null,
  "errors": null
}
```

### 24.2 Get Setting by Key

```
GET /api/v1/admin/config/settings/MaxOrdersPerDriver
```

### 24.3 Update Setting

```
PUT /api/v1/admin/config/settings/MaxOrdersPerDriver
```

**Request:**
```json
{
  "value": "12",
  "reason": "زيادة الحد الأقصى لتلبية الطلب المتزايد"
}
```

### 24.4 Get App Versions

```
GET /api/v1/admin/config/versions
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "ver-001",
      "platform": "Android",
      "versionNumber": "2.1.0",
      "minimumVersion": "1.8.0",
      "isForceUpdate": false,
      "releaseNotes": "تحسينات في الأداء وإصلاح أخطاء",
      "releaseNotesAr": "تحسينات في الأداء وإصلاح أخطاء",
      "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka",
      "createdAt": "2026-03-20T10:00:00Z"
    },
    {
      "id": "ver-002",
      "platform": "iOS",
      "versionNumber": "2.1.0",
      "minimumVersion": "1.8.0",
      "isForceUpdate": false,
      "releaseNotes": "Performance improvements and bug fixes",
      "releaseNotesAr": "تحسينات في الأداء وإصلاح أخطاء",
      "storeUrl": "https://apps.apple.com/app/sekka/id123456789",
      "createdAt": "2026-03-20T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 24.5 Create App Version

```
POST /api/v1/admin/config/versions
```

**Request:**
```json
{
  "platform": "Android",
  "versionNumber": "2.2.0",
  "minimumVersion": "2.0.0",
  "isForceUpdate": false,
  "releaseNotes": "New features: savings circles, campaign support",
  "releaseNotesAr": "ميزات جديدة: حلقات التوفير، دعم الحملات",
  "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka"
}
```

### 24.6 Update App Version

```
PUT /api/v1/admin/config/versions/ver-001
```

### 24.7 Delete App Version

```
DELETE /api/v1/admin/config/versions/ver-001
```

### 24.8 Set Force Update

```
PUT /api/v1/admin/config/versions/ver-001/force-update
```

**Request:**
```json
{
  "isForceUpdate": true,
  "reason": "ثغرة أمنية حرجة - يجب التحديث فوراً"
}
```

### 24.9 Get Maintenance Windows

```
GET /api/v1/admin/config/maintenance
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "maint-001",
      "title": "صيانة دورية للخوادم",
      "titleAr": "صيانة دورية للخوادم",
      "message": "سيتم إيقاف الخدمة مؤقتاً للصيانة",
      "messageAr": "سيتم إيقاف الخدمة مؤقتاً للصيانة",
      "startAt": "2026-04-05T02:00:00Z",
      "endAt": "2026-04-05T04:00:00Z",
      "isActive": false,
      "createdAt": "2026-03-30T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 24.10 Create Maintenance Window

```
POST /api/v1/admin/config/maintenance
```

**Request:**
```json
{
  "title": "Database Migration",
  "titleAr": "ترقية قاعدة البيانات",
  "message": "The system will be unavailable for approximately 2 hours",
  "messageAr": "سيتم إيقاف النظام لمدة ساعتين تقريباً",
  "startAt": "2026-04-10T02:00:00Z",
  "endAt": "2026-04-10T04:00:00Z"
}
```

### 24.11 Update Maintenance Window

```
PUT /api/v1/admin/config/maintenance/maint-001
```

### 24.12 Delete Maintenance Window

```
DELETE /api/v1/admin/config/maintenance/maint-001
```

### 24.13 Start Instant Maintenance

```
POST /api/v1/admin/config/maintenance/instant
```

**Request:**
```json
{
  "message": "Emergency maintenance in progress",
  "messageAr": "صيانة طارئة جارية",
  "estimatedDurationMinutes": 30
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "maintenanceId": "maint-instant001",
    "startedAt": "2026-03-30T14:30:00Z",
    "estimatedEndAt": "2026-03-30T15:00:00Z"
  },
  "message": "تم بدء الصيانة الفورية",
  "errors": null
}
```

### 24.14 Get Feature Flags

```
GET /api/v1/admin/config/feature-flags
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "ff-001",
      "key": "SavingsCircles",
      "name": "Savings Circles Feature",
      "description": "Enable savings circles (game3ya) functionality",
      "isEnabled": true,
      "targetAudience": "All",
      "createdAt": "2026-03-01T10:00:00Z"
    },
    {
      "id": "ff-002",
      "key": "DarkMode",
      "name": "Dark Mode",
      "description": "Enable dark mode in the app",
      "isEnabled": false,
      "targetAudience": "BetaTesters",
      "createdAt": "2026-03-15T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 24.15 Create Feature Flag

```
POST /api/v1/admin/config/feature-flags
```

**Request:**
```json
{
  "key": "LiveTracking",
  "name": "Live Order Tracking",
  "description": "Enable real-time live tracking for customers",
  "isEnabled": false,
  "targetAudience": "All"
}
```

### 24.16 Update Feature Flag

```
PUT /api/v1/admin/config/feature-flags/ff-001
```

### 24.17 Delete Feature Flag

```
DELETE /api/v1/admin/config/feature-flags/ff-001
```

### 24.18 Toggle Feature Flag

```
PUT /api/v1/admin/config/feature-flags/ff-002/toggle
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "key": "DarkMode",
    "isEnabled": true
  },
  "message": "تم تغيير حالة الميزة بنجاح",
  "errors": null
}
```

### 24.19 Get Commission Rates

```
GET /api/v1/admin/config/commissions
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "defaultCommissionRate": 12.5,
    "byCategory": [
      { "category": "Restaurant", "rate": 15.0 },
      { "category": "Grocery", "rate": 10.0 },
      { "category": "Pharmacy", "rate": 8.0 },
      { "category": "Other", "rate": 12.5 }
    ],
    "driverCommission": {
      "baseRate": 80.0,
      "bonusRate": 5.0,
      "peakHourMultiplier": 1.5
    }
  },
  "message": null,
  "errors": null
}
```

### 24.20 Update Commission Rates

```
PUT /api/v1/admin/config/commissions
```

**Request:**
```json
{
  "defaultCommissionRate": 14.0,
  "byCategory": [
    { "category": "Restaurant", "rate": 16.0 },
    { "category": "Grocery", "rate": 11.0 },
    { "category": "Pharmacy", "rate": 9.0 }
  ],
  "reason": "تعديل نسب العمولة للربع الثاني"
}
```

---

## 25. AdminRegionsController

**Route Prefix**: `/api/v1/admin/regions`

Manage geographic service regions. Regions define the areas where the Sekka delivery service operates.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/regions` | Get all regions |
| POST | `/admin/regions` | Create a region |
| PUT | `/admin/regions/{id}` | Update a region |
| DELETE | `/admin/regions/{id}` | Delete a region |

### 25.1 Get All Regions

```
GET /api/v1/admin/regions
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "reg-cairo",
      "name": "القاهرة",
      "nameEn": "Cairo",
      "isActive": true,
      "driversCount": 35,
      "ordersCount": 8500,
      "boundaries": {
        "northEast": { "latitude": 30.13, "longitude": 31.35 },
        "southWest": { "latitude": 29.95, "longitude": 31.15 }
      },
      "createdAt": "2025-06-01T10:00:00Z"
    },
    {
      "id": "reg-giza",
      "name": "الجيزة",
      "nameEn": "Giza",
      "isActive": true,
      "driversCount": 22,
      "ordersCount": 4200,
      "boundaries": {
        "northEast": { "latitude": 30.08, "longitude": 31.25 },
        "southWest": { "latitude": 29.90, "longitude": 31.10 }
      },
      "createdAt": "2025-06-01T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 25.2 Create Region

```
POST /api/v1/admin/regions
```

**Request:**
```json
{
  "name": "المنصورة",
  "nameEn": "Mansoura",
  "isActive": true,
  "boundaries": {
    "northEast": { "latitude": 31.08, "longitude": 31.42 },
    "southWest": { "latitude": 31.00, "longitude": 31.32 }
  }
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "reg-mansoura"
  },
  "message": "تم إنشاء المنطقة بنجاح",
  "errors": null
}
```

### 25.3 Update Region

```
PUT /api/v1/admin/regions/reg-mansoura
```

**Request:**
```json
{
  "name": "المنصورة الكبرى",
  "isActive": true,
  "boundaries": {
    "northEast": { "latitude": 31.10, "longitude": 31.45 },
    "southWest": { "latitude": 30.98, "longitude": 31.30 }
  }
}
```

### 25.4 Delete Region

```
DELETE /api/v1/admin/regions/reg-mansoura
```

---

## 26. AdminAuditLogsController

**Route Prefix**: `/api/v1/admin/audit-logs`

View comprehensive audit trail of all admin actions. Track who did what, when, and on which entity. Essential for compliance and security monitoring.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/audit-logs` | Get all audit logs (filtered + paginated) |
| GET | `/admin/audit-logs/entity/{entityType}/{entityId}` | Get logs for a specific entity |
| GET | `/admin/audit-logs/user/{userId}` | Get logs for a specific user |
| GET | `/admin/audit-logs/summary` | Get audit log summary |
| GET | `/admin/audit-logs/actions` | Get list of tracked actions |
| GET | `/admin/audit-logs/entities` | Get list of tracked entity types |

### 26.1 Get All Audit Logs

```
GET /api/v1/admin/audit-logs?pageNumber=1&pageSize=20&action=Update&fromDate=2026-03-01
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20) |
| action | string | No | Filter by action type |
| entityType | string | No | Filter by entity type |
| userId | Guid | No | Filter by user |
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "log-a1b2c3d4",
        "action": "Update",
        "entityType": "Order",
        "entityId": "ord-001",
        "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "userName": "Admin User",
        "description": "Status overridden from InProgress to Delivered",
        "oldValues": { "status": "InProgress" },
        "newValues": { "status": "Delivered" },
        "ipAddress": "192.168.1.100",
        "timestamp": "2026-03-30T14:30:00Z"
      }
    ],
    "totalCount": 5420,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 271,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 26.2 Get Entity Logs

```
GET /api/v1/admin/audit-logs/entity/Order/ord-001
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "log-001",
      "action": "Create",
      "userName": "سارة أحمد",
      "description": "Order created",
      "timestamp": "2026-03-30T14:00:00Z"
    },
    {
      "id": "log-002",
      "action": "Update",
      "userName": "System",
      "description": "Driver assigned",
      "timestamp": "2026-03-30T14:05:00Z"
    },
    {
      "id": "log-003",
      "action": "Update",
      "userName": "Admin User",
      "description": "Status overridden to Delivered",
      "timestamp": "2026-03-30T14:30:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 26.3 Get User Logs

```
GET /api/v1/admin/audit-logs/user/a1b2c3d4-e5f6-7890-abcd-ef1234567890?pageNumber=1&pageSize=20
```

### 26.4 Get Audit Summary

```
GET /api/v1/admin/audit-logs/summary?fromDate=2026-03-01&toDate=2026-03-30
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalLogs": 5420,
    "byAction": [
      { "action": "Create", "count": 1850 },
      { "action": "Update", "count": 2800 },
      { "action": "Delete", "count": 320 },
      { "action": "Login", "count": 450 }
    ],
    "byEntity": [
      { "entityType": "Order", "count": 3200 },
      { "entityType": "Driver", "count": 850 },
      { "entityType": "Customer", "count": 620 },
      { "entityType": "Payment", "count": 450 },
      { "entityType": "Setting", "count": 300 }
    ],
    "topUsers": [
      { "userName": "Admin User", "actionsCount": 2500 },
      { "userName": "System", "actionsCount": 1800 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 26.5 Get Tracked Actions

```
GET /api/v1/admin/audit-logs/actions
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    "Create", "Update", "Delete", "Login", "Logout",
    "Assign", "Approve", "Reject", "Activate", "Deactivate",
    "Block", "Unblock", "Freeze", "Unfreeze", "Override",
    "Export", "Import", "Broadcast", "Escalate", "Resolve"
  ],
  "message": null,
  "errors": null
}
```

### 26.6 Get Tracked Entities

```
GET /api/v1/admin/audit-logs/entities
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    "Order", "Driver", "Customer", "Partner", "Vehicle",
    "Payment", "Settlement", "Invoice", "Refund", "Dispute",
    "Subscription", "Plan", "Wallet", "Role", "Setting",
    "FeatureFlag", "MaintenanceWindow", "Version", "Region",
    "Notification", "SOS", "Campaign", "Segment", "SavingsCircle"
  ],
  "message": null,
  "errors": null
}
```

---

## 27. AdminSegmentsController

**Route Prefix**: `/api/v1/admin/segments`

Customer segmentation for targeted marketing. Create rule-based or manual segments, manage segment membership, and view segment analytics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/segments` | Get all segments |
| GET | `/admin/segments/{id}` | Get segment details |
| POST | `/admin/segments` | Create a segment |
| PUT | `/admin/segments/{id}` | Update a segment |
| DELETE | `/admin/segments/{id}` | Delete a segment |
| POST | `/admin/segments/{id}/refresh` | Refresh segment membership |
| GET | `/admin/segments/{id}/members` | Get segment members |
| POST | `/admin/segments/{id}/members/{customerId}` | Add member to segment |
| DELETE | `/admin/segments/{id}/members/{customerId}` | Remove member from segment |
| GET | `/admin/segments/analytics` | Get segments analytics |

### 27.1 Get All Segments

```
GET /api/v1/admin/segments
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "seg-vip",
      "name": "VIP Customers",
      "nameAr": "عملاء مميزون",
      "description": "Customers with 20+ orders and 4.5+ rating",
      "type": "RuleBased",
      "memberCount": 85,
      "isActive": true,
      "rules": {
        "minOrders": 20,
        "minRating": 4.5,
        "minLifetimeValue": 2000.00
      },
      "lastRefreshedAt": "2026-03-30T06:00:00Z",
      "createdAt": "2026-01-15T10:00:00Z"
    },
    {
      "id": "seg-inactive",
      "name": "Inactive Customers",
      "nameAr": "عملاء غير نشطين",
      "description": "Customers who haven't ordered in 30+ days",
      "type": "RuleBased",
      "memberCount": 220,
      "isActive": true,
      "rules": {
        "inactiveDays": 30
      },
      "lastRefreshedAt": "2026-03-30T06:00:00Z",
      "createdAt": "2026-02-01T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 27.2 Get Segment by ID

```
GET /api/v1/admin/segments/seg-vip
```

### 27.3 Create Segment

```
POST /api/v1/admin/segments
```

**Request:**
```json
{
  "name": "High Spenders",
  "nameAr": "كبار المنفقين",
  "description": "Customers who spent more than 5000 EGP",
  "type": "RuleBased",
  "rules": {
    "minLifetimeValue": 5000.00
  },
  "isActive": true,
  "autoRefresh": true,
  "refreshIntervalHours": 24
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "seg-highspend",
    "memberCount": 42
  },
  "message": "تم إنشاء الشريحة بنجاح",
  "errors": null
}
```

### 27.4 Update Segment

```
PUT /api/v1/admin/segments/seg-highspend
```

### 27.5 Delete Segment

```
DELETE /api/v1/admin/segments/seg-highspend
```

### 27.6 Refresh Segment

```
POST /api/v1/admin/segments/seg-vip/refresh
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "previousCount": 85,
    "newCount": 92,
    "added": 9,
    "removed": 2
  },
  "message": "تم تحديث أعضاء الشريحة بنجاح",
  "errors": null
}
```

### 27.7 Get Segment Members

```
GET /api/v1/admin/segments/seg-vip/members?pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "customerId": "cust-a1b2c3d4",
        "fullName": "سارة أحمد",
        "phone": "01234567890",
        "totalOrders": 28,
        "totalSpent": 3450.00,
        "rating": 4.9,
        "addedAt": "2026-03-30T06:00:00Z"
      }
    ],
    "totalCount": 85,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 27.8 Add Member to Segment

```
POST /api/v1/admin/segments/seg-vip/members/cust-newmember
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إضافة العضو للشريحة بنجاح",
  "errors": null
}
```

### 27.9 Remove Member from Segment

```
DELETE /api/v1/admin/segments/seg-vip/members/cust-newmember
```

### 27.10 Get Segments Analytics

```
GET /api/v1/admin/segments/analytics
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalSegments": 8,
    "totalUniqueCustomers": 980,
    "segmentedCustomers": 720,
    "unsegmentedCustomers": 260,
    "segmentOverlap": [
      {
        "segment1": "VIP Customers",
        "segment2": "High Spenders",
        "overlapCount": 35
      }
    ],
    "segmentPerformance": [
      {
        "segmentName": "VIP Customers",
        "memberCount": 85,
        "averageOrderFrequency": 3.2,
        "averageOrderValue": 145.00,
        "revenueContribution": 35.2
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 28. AdminCampaignsController

**Route Prefix**: `/api/v1/admin/campaigns`

Marketing campaign management with full lifecycle support: create, launch, pause, resume, and track campaign performance with detailed analytics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/campaigns` | Get all campaigns (filtered + paginated) |
| GET | `/admin/campaigns/{id}` | Get campaign details |
| POST | `/admin/campaigns` | Create a campaign |
| PUT | `/admin/campaigns/{id}` | Update a campaign |
| DELETE | `/admin/campaigns/{id}` | Delete a campaign |
| POST | `/admin/campaigns/{id}/launch` | Launch a campaign |
| POST | `/admin/campaigns/{id}/pause` | Pause a running campaign |
| POST | `/admin/campaigns/{id}/resume` | Resume a paused campaign |
| GET | `/admin/campaigns/stats` | Get campaign statistics |
| GET | `/admin/campaigns/{id}/analytics` | Get detailed campaign analytics |

### 28.1 Get All Campaigns

```
GET /api/v1/admin/campaigns?pageNumber=1&pageSize=10&status=Active
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `Draft`, `Active`, `Paused`, `Completed`, `Cancelled` |
| type | string | No | `Notification`, `SMS`, `InApp`, `Email` |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "camp-spring2026",
        "name": "Spring Promo 2026",
        "nameAr": "عروض الربيع 2026",
        "type": "Notification",
        "status": "Active",
        "targetSegment": "VIP Customers",
        "targetCount": 85,
        "sentCount": 85,
        "openRate": 62.4,
        "conversionRate": 18.5,
        "startDate": "2026-03-20",
        "endDate": "2026-04-20",
        "createdAt": "2026-03-18T10:00:00Z"
      }
    ],
    "totalCount": 12,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 28.2 Get Campaign by ID

```
GET /api/v1/admin/campaigns/camp-spring2026
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "camp-spring2026",
    "name": "Spring Promo 2026",
    "nameAr": "عروض الربيع 2026",
    "description": "Spring promotion with free delivery for VIP customers",
    "type": "Notification",
    "status": "Active",
    "targetSegmentId": "seg-vip",
    "targetSegmentName": "VIP Customers",
    "targetCount": 85,
    "content": {
      "title": "Free Delivery This Spring!",
      "titleAr": "توصيل مجاني هذا الربيع!",
      "body": "Enjoy free delivery on all orders until April 20th. Use code SPRING26",
      "bodyAr": "استمتع بتوصيل مجاني لجميع الطلبات حتى 20 أبريل. استخدم كود SPRING26",
      "imageUrl": "https://storage.sekka.com/campaigns/spring2026.jpg"
    },
    "promoCode": "SPRING26",
    "discount": {
      "type": "FreeDelivery",
      "value": null,
      "maxUsesPerCustomer": 5,
      "totalBudget": 5000.00
    },
    "startDate": "2026-03-20",
    "endDate": "2026-04-20",
    "sentCount": 85,
    "openRate": 62.4,
    "clickRate": 45.2,
    "conversionRate": 18.5,
    "totalRevenue": 12500.00,
    "totalDiscount": 1875.00,
    "roi": 566.7,
    "launchedAt": "2026-03-20T08:00:00Z",
    "createdAt": "2026-03-18T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

### 28.3 Create Campaign

```
POST /api/v1/admin/campaigns
```

**Request:**
```json
{
  "name": "Ramadan Offers",
  "nameAr": "عروض رمضان",
  "description": "Special Ramadan promotion for all customers",
  "type": "Notification",
  "targetSegmentId": null,
  "targetAudience": "Customers",
  "content": {
    "title": "Ramadan Kareem!",
    "titleAr": "رمضان كريم!",
    "body": "Get 30% off all orders during Ramadan",
    "bodyAr": "احصل على خصم 30% على جميع الطلبات خلال رمضان"
  },
  "promoCode": "RAMADAN26",
  "discount": {
    "type": "Percentage",
    "value": 30,
    "maxDiscount": 50.00,
    "maxUsesPerCustomer": 10,
    "totalBudget": 25000.00
  },
  "startDate": "2026-04-01",
  "endDate": "2026-04-30"
}
```

**Response (201):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "camp-ramadan26",
    "name": "Ramadan Offers"
  },
  "message": "تم إنشاء الحملة بنجاح",
  "errors": null
}
```

### 28.4 Update Campaign

```
PUT /api/v1/admin/campaigns/camp-ramadan26
```

### 28.5 Delete Campaign

```
DELETE /api/v1/admin/campaigns/camp-ramadan26
```

### 28.6 Launch Campaign

```
POST /api/v1/admin/campaigns/camp-ramadan26/launch
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "launchedAt": "2026-04-01T00:00:00Z",
    "targetRecipientsCount": 1180
  },
  "message": "تم إطلاق الحملة بنجاح",
  "errors": null
}
```

### 28.7 Pause Campaign

```
POST /api/v1/admin/campaigns/camp-spring2026/pause
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم إيقاف الحملة مؤقتاً",
  "errors": null
}
```

### 28.8 Resume Campaign

```
POST /api/v1/admin/campaigns/camp-spring2026/resume
```

### 28.9 Get Campaign Statistics

```
GET /api/v1/admin/campaigns/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalCampaigns": 12,
    "activeCampaigns": 3,
    "completedCampaigns": 7,
    "draftCampaigns": 2,
    "totalReach": 5420,
    "averageOpenRate": 55.8,
    "averageConversionRate": 15.2,
    "totalRevenue": 45000.00,
    "totalDiscountGiven": 6750.00,
    "averageROI": 425.0,
    "topPerformingCampaign": {
      "name": "Spring Promo 2026",
      "conversionRate": 18.5,
      "revenue": 12500.00
    }
  },
  "message": null,
  "errors": null
}
```

### 28.10 Get Campaign Analytics

```
GET /api/v1/admin/campaigns/camp-spring2026/analytics
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "campaignId": "camp-spring2026",
    "campaignName": "Spring Promo 2026",
    "reach": 85,
    "impressions": 85,
    "opens": 53,
    "openRate": 62.4,
    "clicks": 38,
    "clickRate": 45.2,
    "conversions": 16,
    "conversionRate": 18.5,
    "revenue": 12500.00,
    "discountGiven": 1875.00,
    "roi": 566.7,
    "dailyPerformance": [
      {
        "date": "2026-03-20",
        "opens": 45,
        "clicks": 28,
        "conversions": 8,
        "revenue": 5000.00
      },
      {
        "date": "2026-03-21",
        "opens": 8,
        "clicks": 10,
        "conversions": 8,
        "revenue": 7500.00
      }
    ],
    "topConvertedCustomers": [
      {
        "customerName": "سارة أحمد",
        "orders": 3,
        "spent": 435.00
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 29. AdminInsightsController

**Route Prefix**: `/api/v1/admin/insights`

Business intelligence and customer behavior analytics. Provides overview dashboards, heatmaps, trend analysis, engagement distribution, RFM analysis, behavior summaries, and category performance.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/insights/overview` | Get insights overview dashboard |
| GET | `/admin/insights/heatmap` | Get activity heatmap |
| GET | `/admin/insights/trends` | Get trend analysis |
| GET | `/admin/insights/engagement-distribution` | Get engagement distribution |
| GET | `/admin/insights/rfm-analysis` | Get RFM analysis |
| GET | `/admin/insights/behavior-summary` | Get behavior summary |
| GET | `/admin/insights/category-performance` | Get category performance |

### 29.1 Get Insights Overview

```
GET /api/v1/admin/insights/overview?period=month
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "period": "month",
    "activeCustomers": 980,
    "newCustomers": 85,
    "returningCustomers": 895,
    "churnedCustomers": 45,
    "churnRate": 4.6,
    "averageOrderFrequency": 3.4,
    "averageOrderValue": 125.00,
    "customerLifetimeValue": 1025.50,
    "netPromoterScore": 72,
    "keyMetrics": {
      "customerSatisfaction": 4.5,
      "driverSatisfaction": 4.2,
      "partnerSatisfaction": 4.3,
      "platformReliability": 99.2
    }
  },
  "message": null,
  "errors": null
}
```

### 29.2 Get Activity Heatmap

```
GET /api/v1/admin/insights/heatmap?period=week
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "period": "week",
    "heatmapData": [
      { "day": "Saturday", "hour": 12, "intensity": 0.95, "orders": 22 },
      { "day": "Saturday", "hour": 13, "intensity": 0.82, "orders": 18 },
      { "day": "Saturday", "hour": 19, "intensity": 0.98, "orders": 24 },
      { "day": "Sunday", "hour": 12, "intensity": 0.88, "orders": 20 },
      { "day": "Sunday", "hour": 19, "intensity": 0.91, "orders": 21 }
    ],
    "peakTime": { "day": "Saturday", "hour": 19 },
    "quietestTime": { "day": "Tuesday", "hour": 3 }
  },
  "message": null,
  "errors": null
}
```

### 29.3 Get Trends

```
GET /api/v1/admin/insights/trends?fromDate=2026-01-01&toDate=2026-03-30&metric=orders
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |
| metric | string | No | `orders`, `revenue`, `customers`, `drivers` |
| granularity | string | No | `daily`, `weekly`, `monthly` |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "metric": "orders",
    "granularity": "monthly",
    "trend": "Upward",
    "growthRate": 7.6,
    "dataPoints": [
      { "period": "2026-01", "value": 3800 },
      { "period": "2026-02", "value": 3950 },
      { "period": "2026-03", "value": 4250 }
    ],
    "forecast": [
      { "period": "2026-04", "value": 4575, "confidence": 0.85 },
      { "period": "2026-05", "value": 4925, "confidence": 0.75 }
    ]
  },
  "message": null,
  "errors": null
}
```

### 29.4 Get Engagement Distribution

```
GET /api/v1/admin/insights/engagement-distribution
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "distribution": [
      { "segment": "Highly Engaged", "count": 250, "percentage": 20.0, "avgOrders": 12.5 },
      { "segment": "Engaged", "count": 375, "percentage": 30.0, "avgOrders": 6.2 },
      { "segment": "Moderate", "count": 312, "percentage": 25.0, "avgOrders": 3.1 },
      { "segment": "Low", "count": 188, "percentage": 15.0, "avgOrders": 1.5 },
      { "segment": "At Risk", "count": 125, "percentage": 10.0, "avgOrders": 0.3 }
    ],
    "totalCustomers": 1250
  },
  "message": null,
  "errors": null
}
```

### 29.5 Get RFM Analysis

```
GET /api/v1/admin/insights/rfm-analysis
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "segments": [
      {
        "name": "Champions",
        "description": "Recent, frequent, high value",
        "count": 85,
        "percentage": 6.8,
        "avgRecencyDays": 3,
        "avgFrequency": 15.2,
        "avgMonetaryValue": 3500.00,
        "recommendedAction": "Reward with exclusive offers"
      },
      {
        "name": "Loyal Customers",
        "description": "Regular customers with good frequency",
        "count": 180,
        "percentage": 14.4,
        "avgRecencyDays": 8,
        "avgFrequency": 8.5,
        "avgMonetaryValue": 1800.00,
        "recommendedAction": "Upsell premium features"
      },
      {
        "name": "At Risk",
        "description": "Used to be active, declining activity",
        "count": 120,
        "percentage": 9.6,
        "avgRecencyDays": 35,
        "avgFrequency": 2.1,
        "avgMonetaryValue": 450.00,
        "recommendedAction": "Re-engagement campaign with discounts"
      },
      {
        "name": "Hibernating",
        "description": "Inactive for extended period",
        "count": 220,
        "percentage": 17.6,
        "avgRecencyDays": 60,
        "avgFrequency": 1.2,
        "avgMonetaryValue": 150.00,
        "recommendedAction": "Win-back campaign with heavy incentives"
      },
      {
        "name": "New Customers",
        "description": "Recently joined, few orders",
        "count": 165,
        "percentage": 13.2,
        "avgRecencyDays": 5,
        "avgFrequency": 1.5,
        "avgMonetaryValue": 200.00,
        "recommendedAction": "Onboarding sequence with first-order discount"
      },
      {
        "name": "Potential Loyalists",
        "description": "Recent with moderate frequency",
        "count": 280,
        "percentage": 22.4,
        "avgRecencyDays": 7,
        "avgFrequency": 4.5,
        "avgMonetaryValue": 850.00,
        "recommendedAction": "Loyalty program enrollment"
      },
      {
        "name": "Lost",
        "description": "No recent activity, low historical value",
        "count": 200,
        "percentage": 16.0,
        "avgRecencyDays": 90,
        "avgFrequency": 0.8,
        "avgMonetaryValue": 100.00,
        "recommendedAction": "Survey to understand churn reasons"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

### 29.6 Get Behavior Summary

```
GET /api/v1/admin/insights/behavior-summary
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "orderingPatterns": {
      "averageOrdersPerWeek": 3.4,
      "mostPopularDay": "Saturday",
      "mostPopularTime": "19:00",
      "averageBasketSize": 2.3,
      "averageOrderValue": 125.00
    },
    "preferences": {
      "topCategories": ["Restaurant", "Grocery", "Pharmacy"],
      "preferredPaymentMethod": "CashOnDelivery",
      "preferredDeliveryTime": "Evening",
      "repeatOrderRate": 35.5
    },
    "satisfaction": {
      "averageRating": 4.5,
      "complaintRate": 2.1,
      "reorderRate": 68.5,
      "referralRate": 12.3
    }
  },
  "message": null,
  "errors": null
}
```

### 29.7 Get Category Performance

```
GET /api/v1/admin/insights/category-performance?period=month
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "category": "Restaurant",
      "orders": 2450,
      "revenue": 306250.00,
      "averageOrderValue": 125.00,
      "growthRate": 8.5,
      "customerCount": 780,
      "topPartners": [
        { "name": "مطعم السلطان", "orders": 580 },
        { "name": "بيتزا هت", "orders": 420 }
      ]
    },
    {
      "category": "Grocery",
      "orders": 1200,
      "revenue": 150000.00,
      "averageOrderValue": 125.00,
      "growthRate": 12.2,
      "customerCount": 520,
      "topPartners": [
        { "name": "سوبر ماركت خير زمان", "orders": 350 }
      ]
    },
    {
      "category": "Pharmacy",
      "orders": 600,
      "revenue": 75000.00,
      "averageOrderValue": 125.00,
      "growthRate": 5.8,
      "customerCount": 380,
      "topPartners": [
        { "name": "صيدلية العزبي", "orders": 220 }
      ]
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 30. AdminSavingsCirclesController

**Route Prefix**: `/api/v1/admin/savings-circles`

Manage savings circles (game3ya / جمعية) -- a traditional Egyptian group savings mechanism. Admins can approve, reject, freeze, close circles, manage members, view payments, and track statistics.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/admin/savings-circles` | Get all savings circles (filtered + paginated) |
| GET | `/admin/savings-circles/{id}` | Get savings circle details |
| POST | `/admin/savings-circles/{id}/approve` | Approve a savings circle |
| POST | `/admin/savings-circles/{id}/reject` | Reject a savings circle |
| POST | `/admin/savings-circles/{id}/freeze` | Freeze a savings circle |
| POST | `/admin/savings-circles/{id}/unfreeze` | Unfreeze a savings circle |
| POST | `/admin/savings-circles/{id}/close` | Close a savings circle |
| GET | `/admin/savings-circles/{id}/members` | Get circle members |
| DELETE | `/admin/savings-circles/{id}/members/{memberId}` | Remove member from circle |
| GET | `/admin/savings-circles/{id}/payments` | Get circle payment history |
| GET | `/admin/savings-circles/stats` | Get savings circles statistics |

### 30.1 Get All Savings Circles

```
GET /api/v1/admin/savings-circles?pageNumber=1&pageSize=10&status=Active
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10) |
| status | string | No | `PendingApproval`, `Active`, `Frozen`, `Completed`, `Closed`, `Rejected` |
| search | string | No | Search by circle name or creator |

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "sc-a1b2c3d4",
        "name": "جمعية شارع المعز",
        "creatorName": "سارة أحمد",
        "status": "Active",
        "monthlyAmount": 500.00,
        "totalAmount": 5000.00,
        "membersCount": 10,
        "maxMembers": 10,
        "currentRound": 4,
        "totalRounds": 10,
        "startDate": "2026-01-01",
        "nextPaymentDate": "2026-04-01",
        "createdAt": "2025-12-20T10:00:00Z"
      }
    ],
    "totalCount": 28,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 30.2 Get Savings Circle by ID

```
GET /api/v1/admin/savings-circles/sc-a1b2c3d4
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "sc-a1b2c3d4",
    "name": "جمعية شارع المعز",
    "description": "جمعية شهرية بين الجيران",
    "creatorId": "cust-a1b2c3d4",
    "creatorName": "سارة أحمد",
    "status": "Active",
    "monthlyAmount": 500.00,
    "totalAmount": 5000.00,
    "membersCount": 10,
    "maxMembers": 10,
    "currentRound": 4,
    "totalRounds": 10,
    "startDate": "2026-01-01",
    "endDate": "2026-10-01",
    "nextPaymentDate": "2026-04-01",
    "payoutSchedule": [
      { "round": 1, "date": "2026-01-01", "recipientName": "سارة أحمد", "status": "Paid" },
      { "round": 2, "date": "2026-02-01", "recipientName": "محمد خالد", "status": "Paid" },
      { "round": 3, "date": "2026-03-01", "recipientName": "فاطمة علي", "status": "Paid" },
      { "round": 4, "date": "2026-04-01", "recipientName": "أحمد حسن", "status": "Upcoming" }
    ],
    "totalCollected": 15000.00,
    "totalPaidOut": 15000.00,
    "latePayments": 2,
    "createdAt": "2025-12-20T10:00:00Z",
    "approvedAt": "2025-12-22T10:00:00Z",
    "approvedBy": "Admin User"
  },
  "message": null,
  "errors": null
}
```

### 30.3 Approve Savings Circle

```
POST /api/v1/admin/savings-circles/sc-pending1/approve
```

**Request:**
```json
{
  "notes": "تم التحقق من جميع الأعضاء"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم اعتماد الجمعية بنجاح",
  "errors": null
}
```

### 30.4 Reject Savings Circle

```
POST /api/v1/admin/savings-circles/sc-pending1/reject
```

**Request:**
```json
{
  "reason": "عدد الأعضاء أقل من الحد الأدنى المطلوب"
}
```

### 30.5 Freeze Savings Circle

```
POST /api/v1/admin/savings-circles/sc-a1b2c3d4/freeze
```

**Request:**
```json
{
  "reason": "تأخر متكرر في الدفع من عدة أعضاء"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": null,
  "message": "تم تجميد الجمعية بنجاح",
  "errors": null
}
```

### 30.6 Unfreeze Savings Circle

```
POST /api/v1/admin/savings-circles/sc-a1b2c3d4/unfreeze
```

### 30.7 Close Savings Circle

```
POST /api/v1/admin/savings-circles/sc-a1b2c3d4/close
```

**Request:**
```json
{
  "reason": "طلب المنشئ إغلاق الجمعية",
  "refundRemaining": true
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "refundedAmount": 3500.00,
    "membersRefunded": 7
  },
  "message": "تم إغلاق الجمعية بنجاح",
  "errors": null
}
```

### 30.8 Get Circle Members

```
GET /api/v1/admin/savings-circles/sc-a1b2c3d4/members
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "memberId": "mem-001",
      "customerId": "cust-a1b2c3d4",
      "customerName": "سارة أحمد",
      "phone": "01234567890",
      "position": 1,
      "isCreator": true,
      "paymentStatus": "Current",
      "totalPaid": 2000.00,
      "latePayments": 0,
      "joinedAt": "2025-12-20T10:00:00Z"
    },
    {
      "memberId": "mem-002",
      "customerId": "cust-b2c3d4e5",
      "customerName": "محمد خالد",
      "phone": "01198765432",
      "position": 2,
      "isCreator": false,
      "paymentStatus": "Current",
      "totalPaid": 2000.00,
      "latePayments": 1,
      "joinedAt": "2025-12-21T14:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

### 30.9 Remove Member from Circle

```
DELETE /api/v1/admin/savings-circles/sc-a1b2c3d4/members/mem-003
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "refundAmount": 1500.00
  },
  "message": "تم إزالة العضو من الجمعية بنجاح",
  "errors": null
}
```

### 30.10 Get Circle Payments

```
GET /api/v1/admin/savings-circles/sc-a1b2c3d4/payments?pageNumber=1&pageSize=20
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "pay-sc001",
        "round": 3,
        "memberId": "mem-001",
        "memberName": "سارة أحمد",
        "amount": 500.00,
        "status": "Paid",
        "dueDate": "2026-03-01",
        "paidAt": "2026-02-28T18:00:00Z",
        "isLate": false
      },
      {
        "id": "pay-sc002",
        "round": 3,
        "memberId": "mem-002",
        "memberName": "محمد خالد",
        "amount": 500.00,
        "status": "Paid",
        "dueDate": "2026-03-01",
        "paidAt": "2026-03-02T10:00:00Z",
        "isLate": true
      }
    ],
    "totalCount": 30,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

### 30.11 Get Savings Circles Statistics

```
GET /api/v1/admin/savings-circles/stats
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "totalCircles": 28,
    "activeCircles": 18,
    "completedCircles": 5,
    "frozenCircles": 2,
    "pendingApproval": 3,
    "totalMembers": 245,
    "totalMoneyCirculating": 122500.00,
    "totalCollected": 85000.00,
    "totalPaidOut": 82000.00,
    "latePaymentRate": 4.5,
    "averageCircleSize": 8.75,
    "averageMonthlyAmount": 450.00,
    "defaultRate": 1.2,
    "byStatus": [
      { "status": "Active", "count": 18 },
      { "status": "Completed", "count": 5 },
      { "status": "PendingApproval", "count": 3 },
      { "status": "Frozen", "count": 2 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 31. Enums Reference

### Order Status

| Value | Description |
|-------|-------------|
| `Pending` | Order created, awaiting driver assignment |
| `Assigned` | Driver assigned, awaiting pickup |
| `PickedUp` | Driver picked up the order |
| `InProgress` | Order in transit |
| `Delivered` | Order delivered successfully |
| `Cancelled` | Order cancelled |
| `Returned` | Order returned to sender |

### Driver Status

| Value | Description |
|-------|-------------|
| `Active` | Driver is active and can receive orders |
| `Inactive` | Driver account is deactivated |
| `Pending` | Driver registration pending approval |

### Subscription Status

| Value | Description |
|-------|-------------|
| `Active` | Subscription is currently active |
| `Expired` | Subscription has expired |
| `Cancelled` | Subscription was cancelled |

### Payment Status

| Value | Description |
|-------|-------------|
| `Pending` | Payment awaiting processing |
| `Approved` | Payment approved |
| `Rejected` | Payment rejected |
| `Completed` | Payment fully processed |

### Payment Method

| Value | Description |
|-------|-------------|
| `CashOnDelivery` | Cash payment on delivery |
| `Wallet` | Driver/customer wallet |
| `Card` | Credit/debit card |
| `BankTransfer` | Bank transfer |

### Settlement Status

| Value | Description |
|-------|-------------|
| `Pending` | Settlement awaiting approval |
| `Approved` | Settlement approved for payment |
| `Rejected` | Settlement rejected |
| `Paid` | Settlement paid out |

### Dispute Status

| Value | Description |
|-------|-------------|
| `Open` | Dispute newly filed |
| `InReview` | Dispute under review |
| `Resolved` | Dispute resolved |
| `Rejected` | Dispute rejected |
| `Escalated` | Dispute escalated to senior admin |

### Dispute Priority

| Value | Description |
|-------|-------------|
| `Low` | Low priority |
| `Medium` | Medium priority |
| `High` | High priority |
| `Critical` | Critical -- requires immediate attention |

### Invoice Status

| Value | Description |
|-------|-------------|
| `Draft` | Invoice in draft |
| `Unpaid` | Invoice issued, not yet paid |
| `Paid` | Invoice paid |
| `Overdue` | Invoice past due date |
| `Cancelled` | Invoice cancelled |

### SOS Type

| Value | Description |
|-------|-------------|
| `Accident` | Traffic accident |
| `Threat` | Driver threatened |
| `Medical` | Medical emergency |
| `VehicleBreakdown` | Vehicle breakdown |
| `Other` | Other emergency |

### SOS Status

| Value | Description |
|-------|-------------|
| `Active` | SOS alert active |
| `Acknowledged` | SOS acknowledged by admin |
| `Escalated` | SOS escalated to emergency services |
| `Resolved` | SOS resolved |
| `FalseAlarm` | SOS marked as false alarm |

### Vehicle Status

| Value | Description |
|-------|-------------|
| `Active` | Vehicle active and approved |
| `Inactive` | Vehicle deactivated |
| `PendingApproval` | Vehicle awaiting admin approval |
| `Maintenance` | Vehicle flagged for maintenance |
| `Rejected` | Vehicle rejected |

### Vehicle Type

| Value | Description |
|-------|-------------|
| `Motorcycle` | Motorcycle |
| `Car` | Car |
| `Van` | Van |
| `Truck` | Truck |

### Campaign Status

| Value | Description |
|-------|-------------|
| `Draft` | Campaign created but not launched |
| `Active` | Campaign is running |
| `Paused` | Campaign temporarily paused |
| `Completed` | Campaign finished |
| `Cancelled` | Campaign cancelled |

### Campaign Type

| Value | Description |
|-------|-------------|
| `Notification` | Push notification campaign |
| `SMS` | SMS campaign |
| `InApp` | In-app message campaign |
| `Email` | Email campaign |

### Savings Circle Status

| Value | Description |
|-------|-------------|
| `PendingApproval` | Circle awaiting admin approval |
| `Active` | Circle is active and collecting |
| `Frozen` | Circle frozen by admin |
| `Completed` | All rounds completed |
| `Closed` | Circle closed early |
| `Rejected` | Circle rejected by admin |

### Wallet Transaction Type

| Value | Description |
|-------|-------------|
| `Credit` | Money added to wallet |
| `Debit` | Money deducted from wallet |

### Segment Type

| Value | Description |
|-------|-------------|
| `RuleBased` | Auto-populated based on rules |
| `Manual` | Manually managed membership |

---

## 32. Pagination & Filtering

### Standard Pagination Parameters

All paginated endpoints accept these query parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Page number (1-based) |
| `pageSize` | int | 10 | Items per page (max: 100) |
| `sortBy` | string | `createdAt` | Field to sort by |
| `sortDirection` | string | `desc` | `asc` or `desc` |

### Paginated Response Structure

```json
{
  "items": [],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 15,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Date Filtering

Date parameters accept ISO 8601 format:

```
?fromDate=2026-03-01T00:00:00Z&toDate=2026-03-30T23:59:59Z
```

Or simplified date format:

```
?fromDate=2026-03-01&toDate=2026-03-30
```

### Search

Search parameters perform case-insensitive partial matching:

```
?search=ahmed     // Matches "Ahmed Mohamed", "ahmed@email.com", etc.
```

### Export Formats

Export endpoints support these formats:

| Format | Content-Type | Extension |
|--------|-------------|-----------|
| `csv` | `text/csv` | `.csv` |
| `excel` | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` | `.xlsx` |

```
?format=csv
?format=excel
```

---

## 33. Error Codes Reference

### Standard Error Responses

| HTTP Status | Error Code | Arabic Message | Description |
|------------|------------|---------------|-------------|
| 400 | `BAD_REQUEST` | طلب غير صالح | Validation error or malformed request |
| 401 | `UNAUTHORIZED` | غير مصرح | Missing or invalid authentication token |
| 403 | `FORBIDDEN` | ليس لديك صلاحية للوصول | Valid token but insufficient permissions |
| 404 | `NOT_FOUND` | غير موجود | Requested resource does not exist |
| 409 | `CONFLICT` | تعارض في البيانات | Duplicate resource or conflicting state |
| 422 | `UNPROCESSABLE` | لا يمكن معالجة الطلب | Business rule violation |
| 429 | `RATE_LIMITED` | تم تجاوز الحد المسموح | Too many requests |
| 500 | `SERVER_ERROR` | خطأ في الخادم | Internal server error |

### Common Validation Errors

```json
{
  "isSuccess": false,
  "data": null,
  "message": "خطأ في التحقق من البيانات",
  "errors": [
    "الحقل 'reason' مطلوب",
    "الحقل 'amount' يجب أن يكون أكبر من صفر"
  ]
}
```

### Business Rule Errors

```json
{
  "isSuccess": false,
  "data": null,
  "message": "لا يمكن تعيين السائق - السائق غير متاح حالياً",
  "errors": null
}
```

### Rate Limiting

Admin endpoints are rate limited to prevent abuse:

| Endpoint Type | Limit | Window |
|--------------|-------|--------|
| Read (GET) | 100 requests | Per minute |
| Write (POST/PUT/DELETE) | 30 requests | Per minute |
| Export | 5 requests | Per minute |
| Broadcast | 10 requests | Per hour |

Rate limit headers are included in responses:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1711810800
```

---

> **End of Admin Panel API Documentation**
>
> Total Controllers: 25 | Total Endpoints: 214 | Base URL: `https://sekka.runasp.net/api/v1`
>
> For authentication endpoints, see [AUTH_API.md](./AUTH_API.md)
>
> For other domain-specific APIs, see the corresponding documentation files in this directory.
