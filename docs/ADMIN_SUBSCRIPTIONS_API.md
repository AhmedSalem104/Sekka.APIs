# Sekka API - Admin Subscriptions Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/admin/subscriptions`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` with **Admin** role

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Subscriptions](#21-get-subscriptions)
   - [Get Subscription by ID](#22-get-subscription-by-id)
   - [Extend Subscription](#23-extend-subscription)
   - [Cancel Subscription](#24-cancel-subscription)
   - [Change Plan](#25-change-plan)
   - [Gift Subscription](#26-gift-subscription)
   - [Get Plans](#27-get-plans)
   - [Create Plan](#28-create-plan)
   - [Update Plan](#29-update-plan)
   - [Toggle Plan](#210-toggle-plan)
   - [Get Stats](#211-get-stats)
   - [Get Expiring Soon](#212-get-expiring-soon)
3. [DTOs Reference](#3-dtos-reference)
4. [Enums](#4-enums)

---

## 1. Overview

The Admin Subscriptions API manages subscription plans and driver subscriptions. Admins can create/update plans, extend/cancel subscriptions, gift subscriptions, and view analytics.

---

## 2. Endpoints

### 2.1 Get Subscriptions

```
GET /api/v1/admin/subscriptions?page=1&pageSize=20&status=1&planId={guid}&expiringWithinDays=7&searchTerm=ahmed
```

| Query Param | Type | Default | Description |
|------------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| status | SubscriptionStatus? | null | Filter by status |
| planId | Guid? | null | Filter by plan |
| expiringWithinDays | int? | null | Expiring within N days |
| searchTerm | string? | null | Search driver name/phone |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "driverId": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
        "driverName": "Ahmed Mohamed",
        "driverPhone": "01012345678",
        "planName": "Pro",
        "status": 1,
        "startDate": "2026-01-01T00:00:00Z",
        "endDate": "2026-04-01T00:00:00Z",
        "daysRemaining": 25,
        "amountPaid": 199.00,
        "isGifted": false
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

### 2.2 Get Subscription by ID

```
GET /api/v1/admin/subscriptions/{id}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverName": "Ahmed Mohamed",
    "driverPhone": "01012345678",
    "planName": "Pro",
    "status": 1,
    "startDate": "2026-01-01T00:00:00Z",
    "endDate": "2026-04-01T00:00:00Z",
    "daysRemaining": 25,
    "amountPaid": 199.00,
    "isGifted": false,
    "paymentHistory": [],
    "planChanges": [],
    "giftedBy": null
  }
}
```

---

### 2.3 Extend Subscription

```
PUT /api/v1/admin/subscriptions/{id}/extend
```

**Request Body**:
```json
{
  "additionalDays": 30,
  "reason": "مكافأة للأداء المتميز"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| additionalDays | int | Yes | Days to add |
| reason | string | Yes | Reason for extension |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.4 Cancel Subscription

```
PUT /api/v1/admin/subscriptions/{id}/cancel
```

**Request Body**:
```json
{
  "reason": "طلب العميل",
  "refundAmount": 100.00
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| reason | string | Yes | Cancellation reason |
| refundAmount | decimal? | No | Refund amount (EGP) |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.5 Change Plan

```
PUT /api/v1/admin/subscriptions/{id}/change-plan
```

**Request Body**:
```json
{
  "newPlanId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "reason": "ترقية مجانية",
  "adjustBilling": true
}
```

| Field | Type | Required | Default | Description |
|-------|------|----------|---------|-------------|
| newPlanId | Guid | Yes | - | Target plan ID |
| reason | string | Yes | - | Reason for change |
| adjustBilling | bool | No | true | Adjust billing accordingly |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.6 Gift Subscription

```
POST /api/v1/admin/subscriptions/gift
```

**Request Body**:
```json
{
  "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "planId": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
  "durationDays": 30,
  "reason": "هدية للسائق المتميز"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| driverId | Guid | Yes | Target driver |
| planId | Guid | Yes | Plan to gift |
| durationDays | int | Yes | Subscription duration |
| reason | string | Yes | Gift reason |

**Response** `201 Created`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.7 Get Plans

```
GET /api/v1/admin/subscriptions/plans
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Basic",
      "priceMonthly": 99.00,
      "priceAnnual": 999.00,
      "features": ["Standard Orders", "Basic Stats"],
      "isActive": true
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Pro",
      "priceMonthly": 199.00,
      "priceAnnual": 1999.00,
      "features": ["Priority Orders", "Advanced Stats", "Custom Badge"],
      "isActive": true
    }
  ]
}
```

---

### 2.8 Create Plan

```
POST /api/v1/admin/subscriptions/plans
```

**Request Body**:
```json
{
  "name": "Enterprise",
  "priceMonthly": 499.00,
  "priceAnnual": 4999.00,
  "features": ["Priority Orders", "Advanced Stats", "Custom Badge", "Dedicated Support", "API Access"]
}
```

**Response** `201 Created`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.9 Update Plan

```
PUT /api/v1/admin/subscriptions/plans/{id}
```

**Request Body** (all fields optional):
```json
{
  "name": "Enterprise Plus",
  "priceMonthly": 599.00,
  "priceAnnual": 5999.00,
  "features": ["All Pro features", "Dedicated Support", "API Access", "White Label"]
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": { ... }
}
```

---

### 2.10 Toggle Plan

```
PUT /api/v1/admin/subscriptions/plans/{id}/toggle
```

Activates or deactivates a plan. No request body needed.

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Basic",
    "isActive": false
  }
}
```

---

### 2.11 Get Stats

```
GET /api/v1/admin/subscriptions/stats?fromDate=2026-01-01&toDate=2026-03-07
```

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| fromDate | DateTime | No | Start date |
| toDate | DateTime | No | End date |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "totalActive": 120,
    "totalExpired": 30,
    "totalTrial": 15,
    "totalRevenue": 25000.00,
    "monthlyRecurringRevenue": 8500.00,
    "churnRate": 5.2,
    "planBreakdown": [
      {
        "planName": "Basic",
        "activeCount": 80,
        "totalRevenue": 8000.00,
        "percentage": 66.7
      },
      {
        "planName": "Pro",
        "activeCount": 40,
        "totalRevenue": 17000.00,
        "percentage": 33.3
      }
    ]
  }
}
```

---

### 2.12 Get Expiring Soon

```
GET /api/v1/admin/subscriptions/expiring-soon?days=7
```

| Query Param | Type | Default | Description |
|------------|------|---------|-------------|
| days | int? | 7 | Expiring within N days |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverName": "Ahmed Mohamed",
      "driverPhone": "01012345678",
      "planName": "Pro",
      "endDate": "2026-03-10T00:00:00Z",
      "daysRemaining": 3
    }
  ]
}
```

---

## 3. DTOs Reference

### AdminSubscriptionDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Subscription ID |
| driverId | Guid | Driver ID |
| driverName | string | Driver name |
| driverPhone | string | Driver phone |
| planName | string | Plan name |
| status | SubscriptionStatus | Subscription status |
| startDate | DateTime | Start date |
| endDate | DateTime | End date |
| daysRemaining | int | Days until expiry |
| amountPaid | decimal | Amount paid (EGP) |
| isGifted | bool | Whether it was gifted |

### SubscriptionPlanDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Plan ID |
| name | string | Plan name |
| priceMonthly | decimal | Monthly price (EGP) |
| priceAnnual | decimal? | Annual price (EGP) |
| features | List\<string\> | Feature list |
| isActive | bool | Whether plan is active |

### SubscriptionStatsDto
| Field | Type | Description |
|-------|------|-------------|
| totalActive | int | Active subscriptions |
| totalExpired | int | Expired subscriptions |
| totalTrial | int | Trial subscriptions |
| totalRevenue | decimal | Total revenue (EGP) |
| monthlyRecurringRevenue | decimal | MRR (EGP) |
| churnRate | decimal | Churn rate % |
| planBreakdown | List | Per-plan breakdown |

---

## 4. Enums

### SubscriptionStatus
| Value | Name |
|-------|------|
| 0 | Trial |
| 1 | Active |
| 2 | Expired |
| 3 | Cancelled |
| 4 | Suspended |
