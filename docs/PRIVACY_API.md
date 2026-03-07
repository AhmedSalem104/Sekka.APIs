# Sekka API - Data Privacy Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/privacy`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` header

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Consents](#21-get-consents)
   - [Update Consent](#22-update-consent)
   - [Export Data](#23-export-data)
   - [Delete Data](#24-delete-data)
   - [Get Deletion Status](#25-get-deletion-status)
3. [DTOs Reference](#3-dtos-reference)
4. [Enums](#4-enums)

---

## 1. Overview

The Privacy API handles GDPR-like data privacy features: consent management, data export requests, and data/account deletion requests.

---

## 2. Endpoints

### 2.1 Get Consents

```
GET /api/v1/privacy/consents
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "consentType": "location_tracking",
      "isGranted": true,
      "grantedAt": "2026-01-15T10:00:00Z",
      "description": "السماح بتتبع الموقع أثناء التوصيل"
    },
    {
      "consentType": "marketing_emails",
      "isGranted": false,
      "grantedAt": null,
      "description": "استقبال رسائل تسويقية"
    },
    {
      "consentType": "analytics",
      "isGranted": true,
      "grantedAt": "2026-01-15T10:00:00Z",
      "description": "مشاركة بيانات الاستخدام لتحسين الخدمة"
    }
  ]
}
```

---

### 2.2 Update Consent

```
PUT /api/v1/privacy/consents/{type}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| type | string | Consent type key (e.g., `location_tracking`, `marketing_emails`, `analytics`) |

**Request Body**:
```json
{
  "isGranted": true
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الموافقة بنجاح",
  "data": {
    "consentType": "marketing_emails",
    "isGranted": true,
    "grantedAt": "2026-03-07T15:30:00Z",
    "description": "استقبال رسائل تسويقية"
  }
}
```

---

### 2.3 Export Data

```
POST /api/v1/privacy/export-data
```

No request body required.

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم طلب تصدير البيانات بنجاح",
  "data": {
    "requestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Processing",
    "downloadUrl": null,
    "requestedAt": "2026-03-07T15:30:00Z",
    "readyAt": null,
    "expiresAt": null
  }
}
```

> The data export is processed asynchronously. When ready, `status` changes to `"Ready"` and `downloadUrl` is populated.

---

### 2.4 Delete Data

```
POST /api/v1/privacy/delete-data
```

**Request Body**:
```json
{
  "requestType": "account_deletion",
  "reason": "لم أعد أستخدم التطبيق",
  "status": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| requestType | string | Yes | `"account_deletion"` or `"data_deletion"` |
| reason | string | No | Reason for deletion |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم طلب حذف البيانات بنجاح",
  "data": {
    "requestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Pending",
    "requestedAt": "2026-03-07T15:30:00Z"
  }
}
```

---

### 2.5 Get Deletion Status

```
GET /api/v1/privacy/delete-data/status
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "requestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Pending",
    "requestedAt": "2026-03-07T15:30:00Z"
  }
}
```

**Response** `404 Not Found` (no deletion request):
```json
{
  "success": false,
  "message": "لا يوجد طلب حذف"
}
```

---

## 3. DTOs Reference

### ConsentDto
| Field | Type | Description |
|-------|------|-------------|
| consentType | string | Consent category key |
| isGranted | bool | Whether consent is granted |
| grantedAt | DateTime? | When consent was granted |
| description | string | Human-readable description |

### UpdateConsentDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| isGranted | bool | Yes | Grant or revoke consent |

### DataExportDto
| Field | Type | Description |
|-------|------|-------------|
| requestId | Guid | Export request ID |
| status | string | Processing / Ready / Expired |
| downloadUrl | string? | Download URL when ready |
| requestedAt | DateTime | When requested |
| readyAt | DateTime? | When export was ready |
| expiresAt | DateTime? | Download link expiry |

### DeletionRequestDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| requestType | string | Yes | `account_deletion` or `data_deletion` |
| reason | string | No | Reason for deletion |

---

## 4. Enums

### DeletionRequestStatus
| Value | Name |
|-------|------|
| 0 | Pending |
| 1 | Approved |
| 2 | Processing |
| 3 | Completed |
| 4 | Rejected |
| 5 | Cancelled |

---

## Flutter/Dio Integration

```dart
// Get all consents
final consents = await dio.get('/api/v1/privacy/consents');

// Update consent
await dio.put('/api/v1/privacy/consents/marketing_emails', data: {
  'isGranted': true,
});

// Request data export
await dio.post('/api/v1/privacy/export-data');

// Request account deletion
await dio.post('/api/v1/privacy/delete-data', data: {
  'requestType': 'account_deletion',
  'reason': 'لم أعد أستخدم التطبيق',
});

// Check deletion status
final status = await dio.get('/api/v1/privacy/delete-data/status');
```
