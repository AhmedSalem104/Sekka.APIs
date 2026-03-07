# Sekka API - Demo Mode Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/demo`
>
> **Last Updated**: 2026-03-07

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Start Demo](#21-start-demo)
   - [Get Demo Data](#22-get-demo-data)
   - [End Demo](#23-end-demo)
   - [Convert to Real Account](#24-convert-to-real-account)
3. [DTOs Reference](#3-dtos-reference)
4. [Flow Diagram](#4-flow-diagram)

---

## 1. Overview

The Demo API allows users to try the app without registration. A demo session creates a temporary driver account with sample data that expires after **24 hours**.

| Feature | Detail |
|---------|--------|
| Session Expiry | 24 hours |
| Authentication | JWT token provided on start |
| Conversion | Demo account can be converted to a real account |

---

## 2. Endpoints

### 2.1 Start Demo

```
POST /api/v1/demo/start
```

> **No authentication required**

No request body needed.

**Response** `201 Created`:
```json
{
  "success": true,
  "data": {
    "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "expiresAt": "2026-03-08T15:30:00Z",
    "demoDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "token": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

> Use the returned `token` as `Authorization: Bearer <token>` for subsequent demo requests.

---

### 2.2 Get Demo Data

```
GET /api/v1/demo/data?sessionId={sessionId}
Authorization: Bearer <demo_token>
```

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| sessionId | Guid | Yes | Demo session ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "orders": [
      { "id": "...", "status": "Delivered", "amount": 150.00 }
    ],
    "customers": [
      { "name": "عميل تجريبي", "phone": "0101234XXXX" }
    ],
    "partners": [
      { "name": "مطعم تجريبي", "address": "القاهرة" }
    ],
    "dailyStats": {
      "totalOrders": 10,
      "totalEarnings": 500.00
    },
    "remainingMinutes": 1380
  }
}
```

**Response** `404 Not Found` (expired/invalid session):
```json
{
  "success": false,
  "message": "الجلسة التجريبية غير موجودة أو منتهية"
}
```

---

### 2.3 End Demo

```
POST /api/v1/demo/end?sessionId={sessionId}
Authorization: Bearer <demo_token>
```

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| sessionId | Guid | Yes | Demo session ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "message": "تم إنهاء الجلسة التجريبية"
  }
}
```

---

### 2.4 Convert to Real Account

```
POST /api/v1/demo/convert?sessionId={sessionId}
Authorization: Bearer <demo_token>
```

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| sessionId | Guid | Yes | Demo session ID |

**Request Body** (CompleteRegistrationDto):
```json
{
  "phone": "01012345678",
  "password": "MyP@ssw0rd",
  "confirmPassword": "MyP@ssw0rd",
  "name": "Ahmed Mohamed",
  "vehicleType": 0
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g...",
    "expiresAt": "2026-03-08T15:30:00Z",
    "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
}
```

---

## 3. DTOs Reference

### DemoSessionDto
| Field | Type | Description |
|-------|------|-------------|
| sessionId | Guid | Unique session identifier |
| expiresAt | DateTime | When the demo expires |
| demoDriverId | Guid | Temporary driver ID |
| token | string | JWT token for demo access |

### DemoDataDto
| Field | Type | Description |
|-------|------|-------------|
| orders | List | Sample order data |
| customers | List | Sample customer data |
| partners | List | Sample partner data |
| dailyStats | object | Sample daily statistics |
| remainingMinutes | int | Minutes until session expires |

---

## 4. Flow Diagram

```
┌─────────────────┐
│  User opens app  │
└────────┬────────┘
         │
    ┌────▼────┐
    │ Try Demo │ POST /demo/start
    └────┬────┘
         │ Returns token + sessionId
         │
    ┌────▼─────────────┐
    │ Explore demo data │ GET /demo/data?sessionId=...
    └────┬─────────────┘
         │
    ┌────▼──────────────────────┐
    │ Decision: Continue or End? │
    └────┬──────────┬───────────┘
         │          │
    ┌────▼────┐ ┌──▼──────────────┐
    │End Demo │ │ Convert Account  │
    │POST /end│ │POST /demo/convert│
    └─────────┘ └────┬────────────┘
                     │ Returns real JWT
                ┌────▼──────────┐
                │ Full app access│
                └───────────────┘
```

---

## Flutter/Dio Integration

```dart
// Start demo (no auth needed)
final demo = await Dio().post(
  'https://sekka.runasp.net/api/v1/demo/start',
);
final sessionId = demo.data['data']['sessionId'];
final demoToken = demo.data['data']['token'];

// Set demo token for subsequent requests
dio.options.headers['Authorization'] = 'Bearer $demoToken';

// Get demo data
final data = await dio.get('/api/v1/demo/data', queryParameters: {
  'sessionId': sessionId,
});

// Convert to real account
final result = await dio.post('/api/v1/demo/convert',
  queryParameters: {'sessionId': sessionId},
  data: {
    'phone': '01012345678',
    'password': 'MyP@ssw0rd',
    'confirmPassword': 'MyP@ssw0rd',
    'name': 'Ahmed Mohamed',
    'vehicleType': 0,
  },
);
// Save real tokens from result
```
