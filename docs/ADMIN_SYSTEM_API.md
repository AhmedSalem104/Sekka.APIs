# Sekka API - Admin & System Documentation (Phase 8)

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-27

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Controllers Summary](#3-controllers-summary)
4. [WebhookController Endpoints](#4-webhookcontroller-endpoints)
   - [Get All Webhooks](#41-get-all-webhooks)
   - [Get Webhook Logs](#42-get-webhook-logs)
   - [Create Webhook](#43-create-webhook)
   - [Update Webhook](#44-update-webhook)
   - [Delete Webhook](#45-delete-webhook)
   - [Test Webhook](#46-test-webhook)
5. [AppConfigController Endpoints](#5-appconfigcontroller-endpoints)
   - [Check Version](#51-check-version)
   - [Get Notices](#52-get-notices)
   - [Get Features](#53-get-features)
6. [AdminConfigController Endpoints](#6-adminconfigcontroller-endpoints)
   - [Get All Settings](#61-get-all-settings)
   - [Get Setting by Key](#62-get-setting-by-key)
   - [Update Setting](#63-update-setting)
   - [Get Versions](#64-get-versions)
   - [Create Version](#65-create-version)
   - [Update Version](#66-update-version)
   - [Delete Version](#67-delete-version)
   - [Set Force Update](#68-set-force-update)
   - [Get Maintenance Windows](#69-get-maintenance-windows)
   - [Create Maintenance Window](#610-create-maintenance-window)
   - [Update Maintenance Window](#611-update-maintenance-window)
   - [Delete Maintenance Window](#612-delete-maintenance-window)
   - [Start Instant Maintenance](#613-start-instant-maintenance)
   - [Get Feature Flags](#614-get-feature-flags)
   - [Create Feature Flag](#615-create-feature-flag)
   - [Update Feature Flag](#616-update-feature-flag)
   - [Delete Feature Flag](#617-delete-feature-flag)
   - [Toggle Feature Flag](#618-toggle-feature-flag)
   - [Get Commissions](#619-get-commissions)
   - [Update Commissions](#620-update-commissions)
7. [AdminRegionsController Endpoints](#7-adminregionscontroller-endpoints)
   - [Get Regions](#71-get-regions)
   - [Create Region](#72-create-region)
   - [Update Region](#73-update-region)
   - [Delete Region](#74-delete-region)
8. [AdminAuditLogsController Endpoints](#8-adminauditlogscontroller-endpoints)
   - [Get Logs](#81-get-logs)
   - [Get Entity Logs](#82-get-entity-logs)
   - [Get User Logs](#83-get-user-logs)
   - [Get Summary](#84-get-summary)
   - [Get Audit Actions](#85-get-audit-actions)
   - [Get Tracked Entities](#86-get-tracked-entities)
9. [Enums](#9-enums)
10. [DTOs Reference](#10-dtos-reference)
11. [Force Update Flow](#11-force-update-flow)
12. [Feature Flags Explanation](#12-feature-flags-explanation)
13. [Maintenance Mode Flow](#13-maintenance-mode-flow)
14. [Error Messages Reference](#14-error-messages-reference)
15. [Flutter Integration Examples](#15-flutter-integration-examples)

---

## 1. Overview

Phase 8 covers the **Admin & System** layer of Sekka, which provides system-level configuration, version management, maintenance scheduling, feature flags, webhook management, audit logging, and region management.

| Area | Detail |
|------|--------|
| Webhooks | Driver-scoped webhook configuration for event callbacks |
| App Config | Client-facing version check, notices, and feature flags |
| Admin Config | Full system settings, version management, maintenance, feature flags, commissions |
| Admin Regions | Geographical region/zone management |
| Admin Audit Logs | System-wide audit trail for all entity changes |

### Authentication Requirements

| Controller | Route | Auth |
|------------|-------|------|
| WebhookController | `api/v1/webhooks` | Bearer Token (Driver) |
| AppConfigController | `api/v1/config` | Bearer Token (Driver) **except** `check-version` |
| AdminConfigController | `api/v1/admin/config` | Bearer Token + **Admin Role** |
| AdminRegionsController | `api/v1/admin/regions` | Bearer Token + **Admin Role** |
| AdminAuditLogsController | `api/v1/admin/audit-logs` | Bearer Token + **Admin Role** |

> **Note**: All `admin/*` endpoints require `Authorize(Roles = "Admin")`. The JWT must contain an admin role claim.

---

## 2. Response Format

All API responses follow this format:

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
    "totalCount": 50,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

### HTTP Status Codes Used
| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request / Validation Error / Feature Under Development |
| 401 | Unauthorized (invalid or missing token) |
| 403 | Forbidden (not an admin) |
| 404 | Not Found |
| 409 | Conflict (duplicate) |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Controllers Summary

| # | Controller | Base Route | Endpoints | Status |
|---|-----------|------------|-----------|--------|
| 1 | WebhookController | `/api/v1/webhooks` | 6 | Live |
| 2 | AppConfigController | `/api/v1/config` | 3 | Live |
| 3 | AdminConfigController | `/api/v1/admin/config` | 21 | Stub (Under Development) |
| 4 | AdminRegionsController | `/api/v1/admin/regions` | 4 | Stub (Under Development) |
| 5 | AdminAuditLogsController | `/api/v1/admin/audit-logs` | 6 | Live |

> **Stub Endpoints**: AdminConfigController and AdminRegionsController endpoints currently return `400` with message `"الميزة قيد التطوير: {feature}"` ("Feature under development"). The request/response DTOs are defined and documented below for when they go live.

---

## 4. WebhookController Endpoints

**Base Route**: `api/v1/webhooks`
**Auth**: Bearer Token (Driver)

Webhooks allow drivers (and their partners) to receive HTTP callbacks when specific events occur.

---

### 4.1 Get All Webhooks

Returns all webhook configurations for the authenticated driver.

```
GET /webhooks
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Order Updates",
      "url": "https://myserver.com/webhook",
      "partnerName": "شركة التوصيل",
      "events": ["order.created", "order.completed", "order.cancelled"],
      "isActive": true,
      "lastTriggeredAt": "2026-03-27T10:00:00Z",
      "failureCount": 0
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.2 Get Webhook Logs

Returns paginated delivery logs for a specific webhook.

```
GET /webhooks/{id}/logs?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Webhook configuration ID

**Query Params**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "eventType": "order.completed",
        "isSuccess": true,
        "responseStatusCode": 200,
        "retryCount": 0,
        "sentAt": "2026-03-27T09:55:00Z"
      }
    ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | Webhook not found or does not belong to driver |

---

### 4.3 Create Webhook

Creates a new webhook configuration.

```
POST /webhooks
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "name": "Order Notifications",
  "url": "https://myserver.com/webhook",
  "partnerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "events": ["order.created", "order.completed"]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Display name for the webhook |
| url | string | Yes | Callback URL (must be HTTPS) |
| partnerId | GUID | No | Associated partner ID |
| events | string[] | Yes | List of event types to subscribe to |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Order Notifications",
    "url": "https://myserver.com/webhook",
    "partnerName": null,
    "events": ["order.created", "order.completed"],
    "isActive": true,
    "lastTriggeredAt": null,
    "failureCount": 0
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | Validation error (missing name, invalid URL, empty events) |
| 409 | Duplicate webhook URL |

---

### 4.4 Update Webhook

Updates an existing webhook configuration. All fields are optional (partial update).

```
PUT /webhooks/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Webhook configuration ID

**Request Body**:
```json
{
  "name": "Updated Name",
  "url": "https://newserver.com/webhook",
  "events": ["order.created"],
  "isActive": false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Updated display name |
| url | string | No | Updated callback URL |
| events | string[] | No | Updated event subscriptions |
| isActive | bool | No | Enable/disable the webhook |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": { ... },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | Webhook not found |
| 401 | Webhook does not belong to driver |

---

### 4.5 Delete Webhook

Deletes a webhook configuration.

```
DELETE /webhooks/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Webhook configuration ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | Webhook not found |
| 401 | Webhook does not belong to driver |

---

### 4.6 Test Webhook

Sends a test payload to the webhook URL to verify connectivity.

```
POST /webhooks/{id}/test
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Webhook configuration ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | Webhook not found |
| 400 | Test delivery failed (target URL unreachable) |

---

## 5. AppConfigController Endpoints

**Base Route**: `api/v1/config`
**Auth**: Bearer Token (Driver) unless noted otherwise

These are the **client-facing** config endpoints that the Flutter app calls directly.

---

### 5.1 Check Version

Checks if the current app version needs an update. **This is the most critical endpoint for the Flutter team** -- it should be called on every app launch.

```
GET /config/check-version?platform=0&currentVersion=1.2.0
```

**Auth Required**: No (AllowAnonymous)

**Query Params**:
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| platform | int | Yes | `0` = Android, `1` = iOS (see [AppPlatform](#appplatform)) |
| currentVersion | string | Yes | Current app version (e.g., `"1.2.0"`) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "currentVersion": "1.2.0",
    "latestVersion": "1.5.0",
    "minRequiredVersion": "1.3.0",
    "isForceUpdate": true,
    "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka.driver",
    "releaseNotes": "تحسينات في الأداء وإصلاح أخطاء"
  },
  "message": null,
  "errors": null
}
```

**Decision Logic in Flutter**:
```
if (isForceUpdate && currentVersion < minRequiredVersion):
    → Show BLOCKING update dialog (cannot dismiss)
elif (currentVersion < latestVersion):
    → Show OPTIONAL update dialog (can dismiss)
else:
    → Continue to app
```

---

### 5.2 Get Notices

Returns active system notices/banners for the current driver.

```
GET /config/notices
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "عرض خاص",
      "titleEn": "Special Offer",
      "body": "احصل على خصم 20% على العمولة هذا الأسبوع",
      "bodyEn": "Get 20% off commission this week",
      "noticeType": 3,
      "targetAudience": 0,
      "actionUrl": "sekka://promo/weekly",
      "actionLabel": "اعرف المزيد",
      "backgroundColor": "#FF6B00",
      "priority": 1,
      "startsAt": "2026-03-25T00:00:00Z",
      "expiresAt": "2026-04-01T00:00:00Z",
      "isDismissable": true,
      "isActive": true,
      "viewCount": 1250,
      "clickCount": 340
    }
  ],
  "message": null,
  "errors": null
}
```

> **Note**: Notices are filtered server-side by `targetAudience` and driver attributes (level, region, premium status). The `noticeType` determines how to style the banner in the UI (see [SystemNoticeType](#systemnoticetype)).

---

### 5.3 Get Features

Returns the feature flags applicable to the current driver as a simple key-value map.

```
GET /config/features
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "features": {
      "live_tracking": true,
      "in_app_chat": true,
      "cash_collection": true,
      "premium_orders": false,
      "multi_stop": true,
      "schedule_delivery": false
    }
  },
  "message": null,
  "errors": null
}
```

> **Usage**: Use the `features` map to conditionally show/hide UI elements. See [Feature Flags Explanation](#12-feature-flags-explanation).

---

## 6. AdminConfigController Endpoints

**Base Route**: `api/v1/admin/config`
**Auth**: Bearer Token + **Admin Role**

> **Status**: All endpoints are currently **stubs** returning `400` with `"الميزة قيد التطوير: {feature}"`. The DTOs and contracts are finalized below.

---

### 6.1 Get All Settings

Returns all system key-value configuration settings.

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
      "configKey": "max_active_orders",
      "configValue": "5",
      "description": "الحد الأقصى للطلبات النشطة لكل سائق",
      "updatedAt": "2026-03-20T14:00:00Z"
    },
    {
      "configKey": "otp_expiry_minutes",
      "configValue": "5",
      "description": "مدة صلاحية كود التحقق بالدقائق",
      "updatedAt": "2026-03-01T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 6.2 Get Setting by Key

Returns a specific config value by its key.

```
GET /admin/config/settings/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Configuration key name

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "configKey": "max_active_orders",
    "configValue": "5",
    "description": "الحد الأقصى للطلبات النشطة لكل سائق",
    "updatedAt": "2026-03-20T14:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | Config key not found |

---

### 6.3 Update Setting

Updates a config value by its key.

```
PUT /admin/config/settings/{key}
```

**Auth Required**: Yes (Admin)

**URL Params**: `key` (string) - Configuration key name

**Request Body**:
```json
{
  "configValue": "10"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| configValue | string | Yes | New value for the config key |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث الإعداد بنجاح",
  "errors": null
}
```

---

### 6.4 Get Versions

Returns all registered app versions.

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
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "platform": 0,
      "versionCode": "15",
      "versionNumber": "1.5.0",
      "minRequiredVersion": "1.3.0",
      "minRequiredVersionNumber": 13,
      "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka.driver",
      "releaseNotes": "تحسينات في الأداء",
      "isForceUpdate": true,
      "isActive": true,
      "releasedAt": "2026-03-25T00:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 6.5 Create Version

Registers a new app version.

```
POST /admin/config/versions
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "platform": 0,
  "versionCode": "16",
  "versionNumber": "1.6.0",
  "minRequiredVersion": "1.4.0",
  "minRequiredVersionNumber": 14,
  "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka.driver",
  "releaseNotes": "ميزات جديدة وإصلاحات",
  "releaseNotesEn": "New features and fixes",
  "isForceUpdate": false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| platform | int | Yes | `0` = Android, `1` = iOS |
| versionCode | string | Yes | Internal build code (e.g., `"16"`) |
| versionNumber | string | Yes | Semantic version (e.g., `"1.6.0"`) |
| minRequiredVersion | string | Yes | Minimum version users must have |
| minRequiredVersionNumber | int | Yes | Numeric form of minRequiredVersion for comparison |
| storeUrl | string | Yes | App store / Play Store URL |
| releaseNotes | string | No | Arabic release notes |
| releaseNotesEn | string | No | English release notes |
| isForceUpdate | bool | Yes | Whether to block older versions |

**Response** `201 Created` (expected when live):
```json
{
  "isSuccess": true,
  "data": { ... },
  "message": "تم إنشاء الإصدار بنجاح",
  "errors": null
}
```

---

### 6.6 Update Version

Updates an existing app version record. All fields are optional.

```
PUT /admin/config/versions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Version ID

**Request Body**:
```json
{
  "versionCode": "16",
  "versionNumber": "1.6.1",
  "minRequiredVersion": "1.5.0",
  "minRequiredVersionNumber": 15,
  "storeUrl": "https://play.google.com/store/apps/details?id=com.sekka.driver",
  "releaseNotes": "إصلاح خطأ عاجل",
  "releaseNotesEn": "Critical bug fix",
  "isForceUpdate": true,
  "isActive": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| versionCode | string | No | Updated build code |
| versionNumber | string | No | Updated semantic version |
| minRequiredVersion | string | No | Updated minimum version |
| minRequiredVersionNumber | int | No | Numeric form of min version |
| storeUrl | string | No | Updated store URL |
| releaseNotes | string | No | Updated Arabic notes |
| releaseNotesEn | string | No | Updated English notes |
| isForceUpdate | bool | No | Toggle force update |
| isActive | bool | No | Activate/deactivate the version |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث الإصدار بنجاح",
  "errors": null
}
```

---

### 6.7 Delete Version

Deletes an app version record.

```
DELETE /admin/config/versions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Version ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الإصدار بنجاح",
  "errors": null
}
```

---

### 6.8 Set Force Update

Toggles force update for a specific version.

```
PUT /admin/config/versions/{id}/force-update?force=true
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Version ID

**Query Params**:
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| force | bool | Yes | `true` to enable force update, `false` to disable |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث إعداد إجبار التحديث",
  "errors": null
}
```

---

### 6.9 Get Maintenance Windows

Returns all scheduled and active maintenance windows.

```
GET /admin/config/maintenance
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "صيانة مجدولة",
      "titleEn": "Scheduled Maintenance",
      "message": "سيتم إيقاف الخدمة للصيانة من 2 إلى 4 صباحاً",
      "messageEn": "Service will be down for maintenance from 2-4 AM",
      "startTime": "2026-04-01T02:00:00Z",
      "endTime": "2026-04-01T04:00:00Z",
      "isActive": true,
      "isFullBlock": false
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 6.10 Create Maintenance Window

Schedules a new maintenance window.

```
POST /admin/config/maintenance
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "title": "صيانة مجدولة للسيرفر",
  "message": "سيتم إيقاف الخدمة مؤقتاً للتحديث",
  "startTime": "2026-04-01T02:00:00Z",
  "endTime": "2026-04-01T04:00:00Z",
  "isFullBlock": false,
  "affectedServices": ["orders", "tracking"]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | Arabic title |
| message | string | Yes | Arabic message shown to users |
| startTime | DateTime | Yes | When maintenance begins (UTC) |
| endTime | DateTime | Yes | When maintenance ends (UTC) |
| isFullBlock | bool | Yes | `true` = block all access, `false` = show banner only |
| affectedServices | string[] | No | List of affected service names |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إنشاء نافذة الصيانة بنجاح",
  "errors": null
}
```

---

### 6.11 Update Maintenance Window

Updates an existing maintenance window. All fields are optional.

```
PUT /admin/config/maintenance/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Maintenance window ID

**Request Body**:
```json
{
  "title": "صيانة محدّثة",
  "titleEn": "Updated Maintenance",
  "message": "تم تقليل فترة الصيانة",
  "messageEn": "Maintenance window reduced",
  "startTime": "2026-04-01T02:00:00Z",
  "endTime": "2026-04-01T03:00:00Z",
  "isActive": true,
  "isFullBlock": false,
  "affectedServices": ["orders"]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | No | Updated Arabic title |
| titleEn | string | No | Updated English title |
| message | string | No | Updated Arabic message |
| messageEn | string | No | Updated English message |
| startTime | DateTime | No | Updated start time |
| endTime | DateTime | No | Updated end time |
| isActive | bool | No | Activate/deactivate |
| isFullBlock | bool | No | Toggle full block mode |
| affectedServices | string[] | No | Updated affected services |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث نافذة الصيانة بنجاح",
  "errors": null
}
```

---

### 6.12 Delete Maintenance Window

Deletes a maintenance window.

```
DELETE /admin/config/maintenance/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Maintenance window ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف نافذة الصيانة بنجاح",
  "errors": null
}
```

---

### 6.13 Start Instant Maintenance

Starts immediate maintenance mode with a specified duration.

```
POST /admin/config/maintenance/instant
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "title": "صيانة طارئة",
  "message": "جاري إصلاح مشكلة عاجلة. سيعود التطبيق خلال 30 دقيقة",
  "durationMinutes": 30,
  "isFullBlock": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | Arabic title |
| message | string | Yes | Arabic message for users |
| durationMinutes | int | Yes | How long the maintenance lasts |
| isFullBlock | bool | Yes | `true` = block all access, `false` = banner only |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم بدء الصيانة الفورية",
  "errors": null
}
```

---

### 6.14 Get Feature Flags

Returns all feature flags with their configuration.

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
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "featureKey": "live_tracking",
      "displayName": "التتبع المباشر",
      "displayNameEn": "Live Tracking",
      "description": "تفعيل التتبع المباشر للطلبات",
      "isEnabled": true,
      "enabledForPremiumOnly": false,
      "enabledForPercentage": 100,
      "category": "tracking",
      "expiresAt": null
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 6.15 Create Feature Flag

Creates a new feature flag.

```
POST /admin/config/feature-flags
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "featureKey": "schedule_delivery",
  "displayName": "التوصيل المجدول",
  "displayNameEn": "Scheduled Delivery",
  "description": "السماح بجدولة التوصيل مسبقاً",
  "isEnabled": false,
  "enabledForPremiumOnly": true,
  "enabledForPercentage": 50,
  "minAppVersion": "1.5.0",
  "category": "delivery"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| featureKey | string | Yes | Unique key (used in client code) |
| displayName | string | Yes | Arabic display name |
| displayNameEn | string | No | English display name |
| description | string | No | Description of the feature |
| isEnabled | bool | Yes | Global on/off toggle |
| enabledForPremiumOnly | bool | Yes | Restrict to premium drivers |
| enabledForPercentage | int | No | Rollout percentage (0-100), default `100` |
| minAppVersion | string | No | Minimum app version required |
| category | string | No | Grouping category |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إنشاء علم الميزة بنجاح",
  "errors": null
}
```

---

### 6.16 Update Feature Flag

Updates an existing feature flag. All fields are optional.

```
PUT /admin/config/feature-flags/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Feature flag ID

**Request Body**:
```json
{
  "featureKey": "schedule_delivery",
  "displayName": "التوصيل المجدول v2",
  "displayNameEn": "Scheduled Delivery v2",
  "description": "النسخة المحدثة من التوصيل المجدول",
  "isEnabled": true,
  "enabledForPremiumOnly": false,
  "enabledForPercentage": 100,
  "minAppVersion": "1.6.0",
  "category": "delivery",
  "expiresAt": "2026-06-01T00:00:00Z"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| featureKey | string | No | Updated key |
| displayName | string | No | Updated Arabic name |
| displayNameEn | string | No | Updated English name |
| description | string | No | Updated description |
| isEnabled | bool | No | Toggle on/off |
| enabledForPremiumOnly | bool | No | Toggle premium restriction |
| enabledForPercentage | int | No | Updated rollout percentage |
| minAppVersion | string | No | Updated minimum app version |
| category | string | No | Updated category |
| expiresAt | DateTime | No | Auto-disable date (null = never) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث علم الميزة بنجاح",
  "errors": null
}
```

---

### 6.17 Delete Feature Flag

Deletes a feature flag.

```
DELETE /admin/config/feature-flags/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Feature flag ID

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

### 6.18 Toggle Feature Flag

Quickly toggles a feature flag on/off without sending a full update body.

```
PUT /admin/config/feature-flags/{id}/toggle
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Feature flag ID

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تبديل علم الميزة بنجاح",
  "errors": null
}
```

---

### 6.19 Get Commissions

Returns current commission settings.

```
GET /admin/config/commissions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "defaultCommissionPercent": 15.00,
    "minCommission": 5.00,
    "maxCommission": 50.00
  },
  "message": null,
  "errors": null
}
```

---

### 6.20 Update Commissions

Updates commission settings.

```
PUT /admin/config/commissions
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "defaultCommissionPercent": 12.50,
  "minCommission": 3.00,
  "maxCommission": 75.00
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| defaultCommissionPercent | decimal | Yes | Default commission percentage |
| minCommission | decimal | Yes | Minimum commission amount (EGP) |
| maxCommission | decimal | Yes | Maximum commission amount (EGP) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث إعدادات العمولة بنجاح",
  "errors": null
}
```

---

## 7. AdminRegionsController Endpoints

**Base Route**: `api/v1/admin/regions`
**Auth**: Bearer Token + **Admin Role**

> **Status**: All endpoints are currently **stubs** returning `400` with `"الميزة قيد التطوير: {feature}"`.

---

### 7.1 Get Regions

Returns all regions/zones.

```
GET /admin/regions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK` (expected when live):
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "القاهرة",
      "nameEn": "Cairo",
      "parentRegionId": null,
      "centerLatitude": 30.0444,
      "centerLongitude": 31.2357,
      "radiusKm": 25.0,
      "isActive": true
    },
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "مدينة نصر",
      "nameEn": "Nasr City",
      "parentRegionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "centerLatitude": 30.0511,
      "centerLongitude": 31.3656,
      "radiusKm": 5.0,
      "isActive": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 7.2 Create Region

Creates a new region/zone.

```
POST /admin/regions
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "name": "المعادي",
  "nameEn": "Maadi",
  "parentRegionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "centerLatitude": 29.9602,
  "centerLongitude": 31.2569,
  "radiusKm": 4.5
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Arabic name |
| nameEn | string | No | English name |
| parentRegionId | GUID | No | Parent region (for sub-regions) |
| centerLatitude | double | No | Center point latitude |
| centerLongitude | double | No | Center point longitude |
| radiusKm | double | No | Coverage radius in kilometers |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": { ... },
  "message": "تم إنشاء المنطقة بنجاح",
  "errors": null
}
```

---

### 7.3 Update Region

Updates an existing region. All fields are optional.

```
PUT /admin/regions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Region ID

**Request Body**:
```json
{
  "name": "المعادي الجديدة",
  "nameEn": "New Maadi",
  "parentRegionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "centerLatitude": 29.9602,
  "centerLongitude": 31.2569,
  "radiusKm": 6.0,
  "isActive": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Updated Arabic name |
| nameEn | string | No | Updated English name |
| parentRegionId | GUID | No | Updated parent region |
| centerLatitude | double | No | Updated center latitude |
| centerLongitude | double | No | Updated center longitude |
| radiusKm | double | No | Updated radius |
| isActive | bool | No | Activate/deactivate |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث المنطقة بنجاح",
  "errors": null
}
```

---

### 7.4 Delete Region

Deletes a region.

```
DELETE /admin/regions/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - Region ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف المنطقة بنجاح",
  "errors": null
}
```

---

## 8. AdminAuditLogsController Endpoints

**Base Route**: `api/v1/admin/audit-logs`
**Auth**: Bearer Token + **Admin Role**

The audit log system automatically records all entity changes (create, update, delete) and authentication events (login, logout) for compliance and debugging.

---

### 8.1 Get Logs

Returns paginated, filterable audit logs.

```
GET /admin/audit-logs?page=1&pageSize=20&entityType=Order&action=0&dateFrom=2026-03-01&dateTo=2026-03-27
```

**Auth Required**: Yes (Admin)

**Query Params** (all optional):
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| entityType | string | null | Filter by entity type (e.g., `"Order"`, `"Driver"`) |
| userId | GUID | null | Filter by user who performed the action |
| action | int | null | Filter by action type (see [AuditAction](#auditaction)) |
| dateFrom | DateTime | null | Start date filter |
| dateTo | DateTime | null | End date filter |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": 12345,
        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "userName": "أحمد (Admin)",
        "entityType": "Order",
        "entityId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "action": 1,
        "oldValues": "{\"status\": 2}",
        "newValues": "{\"status\": 5}",
        "affectedColumns": "Status",
        "ipAddress": "192.168.1.100",
        "timestamp": "2026-03-27T14:30:00Z"
      }
    ],
    "totalCount": 1500,
    "page": 1,
    "pageSize": 20,
    "totalPages": 75,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

> **Note**: `oldValues` and `newValues` are JSON strings. Parse them in Flutter to show a diff view.

---

### 8.2 Get Entity Logs

Returns all audit logs for a specific entity instance.

```
GET /admin/audit-logs/entity/{entityType}/{entityId}
```

**Auth Required**: Yes (Admin)

**URL Params**:
| Param | Type | Description |
|-------|------|-------------|
| entityType | string | Entity type name (e.g., `"Order"`, `"Driver"`) |
| entityId | string | Entity ID (GUID as string) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 12340,
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "System",
      "entityType": "Order",
      "entityId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "action": 0,
      "oldValues": null,
      "newValues": "{\"status\": 0, \"amount\": 150.00}",
      "affectedColumns": null,
      "ipAddress": null,
      "timestamp": "2026-03-27T10:00:00Z"
    },
    {
      "id": 12345,
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "أحمد (Admin)",
      "entityType": "Order",
      "entityId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "action": 1,
      "oldValues": "{\"status\": 0}",
      "newValues": "{\"status\": 2}",
      "affectedColumns": "Status",
      "ipAddress": "192.168.1.100",
      "timestamp": "2026-03-27T12:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 8.3 Get User Logs

Returns paginated audit logs for a specific user.

```
GET /admin/audit-logs/user/{userId}?page=1&pageSize=20
```

**Auth Required**: Yes (Admin)

**URL Params**: `userId` (GUID) - User ID

**Query Params**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": 12345,
        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "userName": "أحمد (Admin)",
        "entityType": "Driver",
        "entityId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "action": 1,
        "oldValues": "{\"isOnline\": true}",
        "newValues": "{\"isOnline\": false}",
        "affectedColumns": "IsOnline",
        "ipAddress": "192.168.1.100",
        "timestamp": "2026-03-27T14:30:00Z"
      }
    ],
    "totalCount": 200,
    "page": 1,
    "pageSize": 20,
    "totalPages": 10,
    "hasNext": true,
    "hasPrevious": false
  },
  "message": null,
  "errors": null
}
```

---

### 8.4 Get Summary

Returns aggregated statistics for audit logs within a date range.

```
GET /admin/audit-logs/summary?dateFrom=2026-03-01&dateTo=2026-03-27
```

**Auth Required**: Yes (Admin)

**Query Params** (all optional):
| Param | Type | Description |
|-------|------|-------------|
| dateFrom | DateTime | Start date (null = all time) |
| dateTo | DateTime | End date (null = all time) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalActions": 5430,
    "createCount": 2100,
    "updateCount": 2800,
    "deleteCount": 530,
    "topEntities": [
      { "entityType": "Order", "count": 3200 },
      { "entityType": "Driver", "count": 1100 },
      { "entityType": "Customer", "count": 800 },
      { "entityType": "Payment", "count": 330 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 8.5 Get Audit Actions

Returns all available audit action types as an enum list.

```
GET /admin/audit-logs/actions
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    { "value": 0, "name": "Create" },
    { "value": 1, "name": "Update" },
    { "value": 2, "name": "Delete" },
    { "value": 3, "name": "Login" },
    { "value": 4, "name": "Logout" }
  ],
  "message": null,
  "errors": null
}
```

---

### 8.6 Get Tracked Entities

Returns a list of distinct entity types that appear in audit logs.

```
GET /admin/audit-logs/entities
```

**Auth Required**: Yes (Admin)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    "Order",
    "Driver",
    "Customer",
    "Payment",
    "Vehicle",
    "Region"
  ],
  "message": null,
  "errors": null
}
```

> **Usage**: Use this list to populate the entity type filter dropdown in the admin audit logs screen.

---

## 9. Enums

### AuditAction
| Value | Name | Description |
|-------|------|-------------|
| 0 | Create | Entity was created |
| 1 | Update | Entity was modified |
| 2 | Delete | Entity was deleted |
| 3 | Login | User logged in |
| 4 | Logout | User logged out |

### AppPlatform
| Value | Name |
|-------|------|
| 0 | Android |
| 1 | iOS |

### SystemNoticeType
| Value | Name | Arabic | UI Hint |
|-------|------|--------|---------|
| 0 | Info | معلومة | Blue banner |
| 1 | Warning | تحذير | Yellow/amber banner |
| 2 | Success | نجاح | Green banner |
| 3 | Promo | عرض ترويجي | Orange/branded banner |
| 4 | Urgent | عاجل | Red banner (non-dismissable recommended) |

### TargetAudience
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | All | الكل | All users see the notice |
| 1 | DriversOnly | السائقين فقط | Only drivers |
| 2 | PremiumOnly | المميزين فقط | Premium subscription drivers only |
| 3 | FreeOnly | المجانيين فقط | Free-tier drivers only |
| 4 | SpecificRegion | منطقة محددة | Drivers in a specific region |

---

## 10. DTOs Reference

### Webhook DTOs

**CreateWebhookConfigDto** (POST `/webhooks`)
| Field | Type | Required |
|-------|------|----------|
| name | string | Yes |
| url | string | Yes |
| partnerId | GUID? | No |
| events | string[] | Yes |

**UpdateWebhookConfigDto** (PUT `/webhooks/{id}`)
| Field | Type | Required |
|-------|------|----------|
| name | string? | No |
| url | string? | No |
| events | string[]? | No |
| isActive | bool? | No |

**WebhookConfigDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| name | string |
| url | string |
| partnerName | string? |
| events | string[] |
| isActive | bool |
| lastTriggeredAt | DateTime? |
| failureCount | int |

**WebhookLogDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| eventType | string |
| isSuccess | bool |
| responseStatusCode | int? |
| retryCount | int |
| sentAt | DateTime |

---

### App Config DTOs

**AppConfigDto** (Response)
| Field | Type |
|-------|------|
| configKey | string |
| configValue | string |
| description | string? |
| updatedAt | DateTime |

**UpdateConfigDto** (PUT `/admin/config/settings/{key}`)
| Field | Type | Required |
|-------|------|----------|
| configValue | string | Yes |

---

### App Version DTOs

**AppVersionDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| platform | AppPlatform (int) |
| versionCode | string |
| versionNumber | string |
| minRequiredVersion | string |
| minRequiredVersionNumber | int |
| storeUrl | string |
| releaseNotes | string? |
| isForceUpdate | bool |
| isActive | bool |
| releasedAt | DateTime |

**AppVersionCheckDto** (Response for `check-version`)
| Field | Type |
|-------|------|
| currentVersion | string |
| latestVersion | string |
| minRequiredVersion | string |
| isForceUpdate | bool |
| storeUrl | string |
| releaseNotes | string? |

**CreateAppVersionDto** (POST `/admin/config/versions`)
| Field | Type | Required |
|-------|------|----------|
| platform | AppPlatform (int) | Yes |
| versionCode | string | Yes |
| versionNumber | string | Yes |
| minRequiredVersion | string | Yes |
| minRequiredVersionNumber | int | Yes |
| storeUrl | string | Yes |
| releaseNotes | string? | No |
| releaseNotesEn | string? | No |
| isForceUpdate | bool | Yes |

**UpdateAppVersionDto** (PUT `/admin/config/versions/{id}`)
| Field | Type | Required |
|-------|------|----------|
| versionCode | string? | No |
| versionNumber | string? | No |
| minRequiredVersion | string? | No |
| minRequiredVersionNumber | int? | No |
| storeUrl | string? | No |
| releaseNotes | string? | No |
| releaseNotesEn | string? | No |
| isForceUpdate | bool? | No |
| isActive | bool? | No |

---

### Feature Flag DTOs

**FeatureFlagDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| featureKey | string |
| displayName | string |
| displayNameEn | string? |
| description | string? |
| isEnabled | bool |
| enabledForPremiumOnly | bool |
| enabledForPercentage | int |
| category | string? |
| expiresAt | DateTime? |

**FeatureFlagsCheckDto** (Response for `GET /config/features`)
| Field | Type |
|-------|------|
| features | Map<string, bool> |

**CreateFeatureFlagDto** (POST `/admin/config/feature-flags`)
| Field | Type | Required |
|-------|------|----------|
| featureKey | string | Yes |
| displayName | string | Yes |
| displayNameEn | string? | No |
| description | string? | No |
| isEnabled | bool | Yes |
| enabledForPremiumOnly | bool | Yes |
| enabledForPercentage | int | No (default 100) |
| minAppVersion | string? | No |
| category | string? | No |

**UpdateFeatureFlagDto** (PUT `/admin/config/feature-flags/{id}`)
| Field | Type | Required |
|-------|------|----------|
| featureKey | string? | No |
| displayName | string? | No |
| displayNameEn | string? | No |
| description | string? | No |
| isEnabled | bool? | No |
| enabledForPremiumOnly | bool? | No |
| enabledForPercentage | int? | No |
| minAppVersion | string? | No |
| category | string? | No |
| expiresAt | DateTime? | No |

---

### Maintenance Window DTOs

**MaintenanceWindowDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| title | string |
| titleEn | string? |
| message | string |
| messageEn | string? |
| startTime | DateTime |
| endTime | DateTime |
| isActive | bool |
| isFullBlock | bool |

**CreateMaintenanceWindowDto** (POST `/admin/config/maintenance`)
| Field | Type | Required |
|-------|------|----------|
| title | string | Yes |
| message | string | Yes |
| startTime | DateTime | Yes |
| endTime | DateTime | Yes |
| isFullBlock | bool | Yes |
| affectedServices | string[]? | No |

**UpdateMaintenanceWindowDto** (PUT `/admin/config/maintenance/{id}`)
| Field | Type | Required |
|-------|------|----------|
| title | string? | No |
| titleEn | string? | No |
| message | string? | No |
| messageEn | string? | No |
| startTime | DateTime? | No |
| endTime | DateTime? | No |
| isActive | bool? | No |
| isFullBlock | bool? | No |
| affectedServices | string[]? | No |

**InstantMaintenanceDto** (POST `/admin/config/maintenance/instant`)
| Field | Type | Required |
|-------|------|----------|
| title | string | Yes |
| message | string | Yes |
| durationMinutes | int | Yes |
| isFullBlock | bool | Yes |

---

### System Notice DTOs

**SystemNoticeDto** (Response for `GET /config/notices`)
| Field | Type |
|-------|------|
| id | GUID |
| title | string |
| titleEn | string? |
| body | string |
| bodyEn | string? |
| noticeType | SystemNoticeType (int) |
| targetAudience | TargetAudience (int) |
| actionUrl | string? |
| actionLabel | string? |
| backgroundColor | string? |
| priority | int |
| startsAt | DateTime |
| expiresAt | DateTime? |
| isDismissable | bool |
| isActive | bool |
| viewCount | int |
| clickCount | int |

---

### Commission DTOs

**CommissionSettingsDto** (GET/PUT `/admin/config/commissions`)
| Field | Type | Required (PUT) |
|-------|------|----------------|
| defaultCommissionPercent | decimal | Yes |
| minCommission | decimal | Yes |
| maxCommission | decimal | Yes |

---

### Region DTOs

**RegionDto** (Response)
| Field | Type |
|-------|------|
| id | GUID |
| name | string |
| nameEn | string? |
| parentRegionId | GUID? |
| centerLatitude | double? |
| centerLongitude | double? |
| radiusKm | double? |
| isActive | bool |

**CreateRegionDto** (POST `/admin/regions`)
| Field | Type | Required |
|-------|------|----------|
| name | string | Yes |
| nameEn | string? | No |
| parentRegionId | GUID? | No |
| centerLatitude | double? | No |
| centerLongitude | double? | No |
| radiusKm | double? | No |

**UpdateRegionDto** (PUT `/admin/regions/{id}`)
| Field | Type | Required |
|-------|------|----------|
| name | string? | No |
| nameEn | string? | No |
| parentRegionId | GUID? | No |
| centerLatitude | double? | No |
| centerLongitude | double? | No |
| radiusKm | double? | No |
| isActive | bool? | No |

---

### Audit Log DTOs

**AuditLogDto** (Response)
| Field | Type |
|-------|------|
| id | long |
| userId | GUID? |
| userName | string? |
| entityType | string |
| entityId | string |
| action | AuditAction (int) |
| oldValues | string? (JSON) |
| newValues | string? (JSON) |
| affectedColumns | string? |
| ipAddress | string? |
| timestamp | DateTime |

**AuditLogFilterDto** (Query params for `GET /admin/audit-logs`)
| Field | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |
| entityType | string? | null |
| userId | GUID? | null |
| action | AuditAction? (int) | null |
| dateFrom | DateTime? | null |
| dateTo | DateTime? | null |

**AuditActionsSummaryDto** (Response for `GET /admin/audit-logs/summary`)
| Field | Type |
|-------|------|
| totalActions | int |
| createCount | int |
| updateCount | int |
| deleteCount | int |
| topEntities | EntityActionCount[] |

**EntityActionCount**
| Field | Type |
|-------|------|
| entityType | string |
| count | int |

---

## 11. Force Update Flow

The force update mechanism ensures users are always running a compatible app version.

```
                    ┌──────────────┐
                    │   App Launch  │
                    └──────┬───────┘
                           │
              GET /config/check-version
              ?platform=0&currentVersion=1.2.0
                           │
                    ┌──────▼───────┐
                    │   Response    │
                    └──┬───────┬───┘
                       │       │
            isForceUpdate   isForceUpdate
              = true          = false
                │               │
     ┌──────────▼──────┐   ┌───▼──────────────┐
     │  currentVersion  │   │  currentVersion   │
     │      < min       │   │    < latest       │
     │  RequiredVersion │   │    Version        │
     └──┬──────────┬───┘   └──┬──────────┬────┘
        │          │          │          │
       Yes         No        Yes         No
        │          │          │          │
  ┌─────▼─────┐ ┌──▼────┐ ┌──▼────────┐ ┌▼──────────┐
  │  BLOCKING  │ │ Home  │ │  OPTIONAL  │ │   Home    │
  │  Update    │ │Screen │ │  Update    │ │  Screen   │
  │  Dialog    │ │       │ │  Dialog    │ │ (no popup)│
  │ (no close) │ │       │ │(dismissable│ │           │
  └─────┬──────┘ └───────┘ └──┬────┬───┘ └───────────┘
        │                     │    │
   Open Store             Dismiss  Open Store
   (storeUrl)                │     (storeUrl)
        │                 ┌──▼──┐
        │                 │Home │
        │                 │Screen│
        │                 └──────┘
   ┌────▼────────────┐
   │ App Store /     │
   │ Play Store Page │
   └─────────────────┘
```

### Version Comparison Logic

The server compares `currentVersion` against `minRequiredVersion` using semantic versioning:

| currentVersion | minRequiredVersion | latestVersion | isForceUpdate | Result |
|---------------|-------------------|---------------|---------------|--------|
| 1.2.0 | 1.3.0 | 1.5.0 | true | BLOCKING update dialog |
| 1.3.0 | 1.3.0 | 1.5.0 | true | OPTIONAL update dialog |
| 1.5.0 | 1.3.0 | 1.5.0 | true | No dialog (already latest) |
| 1.2.0 | 1.0.0 | 1.5.0 | false | OPTIONAL update dialog |
| 1.5.0 | 1.0.0 | 1.5.0 | false | No dialog |

---

## 12. Feature Flags Explanation

Feature flags allow the backend team to control feature availability without deploying new app versions.

### How It Works

```
┌──────────────────────┐     GET /config/features     ┌──────────────────────┐
│     Flutter App       │ ─────────────────────────── │     Sekka API         │
│                       │                              │                       │
│  On app launch or     │     Response:                │  Evaluates flags      │
│  after login, fetch   │     {                        │  based on:            │
│  the features map     │       "live_tracking": true, │   - isEnabled         │
│                       │       "multi_stop": true,    │   - driver's level    │
│  Cache locally for    │       "premium_orders": false│   - premium status    │
│  offline use          │     }                        │   - app version       │
│                       │                              │   - rollout %         │
└──────────────────────┘                              └──────────────────────┘
```

### Feature Flag Properties

| Property | Purpose |
|----------|---------|
| `featureKey` | Unique identifier used in code (e.g., `"live_tracking"`) |
| `isEnabled` | Global on/off switch |
| `enabledForPremiumOnly` | Only premium drivers see the feature |
| `enabledForPercentage` | Gradual rollout (0-100%). E.g., 25 = 25% of drivers see it |
| `minAppVersion` | Feature only available if app version >= this |
| `expiresAt` | Auto-disable after this date (useful for promos) |
| `category` | Grouping for admin UI (e.g., "delivery", "tracking", "payment") |

### Flutter-Side Usage

```dart
// After fetching features, use them to gate UI:
if (featureFlags['multi_stop'] == true) {
  showMultiStopOption();
}

if (featureFlags['premium_orders'] == true) {
  showPremiumOrdersTab();
}

if (featureFlags['in_app_chat'] == true) {
  showChatButton();
}
```

### Rollout Strategy

```
Phase 1: enabledForPercentage = 10   (10% of drivers)
Phase 2: enabledForPercentage = 50   (50% of drivers)
Phase 3: enabledForPercentage = 100  (all drivers)

At any point, set isEnabled = false to kill the feature globally.
```

---

## 13. Maintenance Mode Flow

Maintenance mode allows admins to gracefully take the system offline for updates.

### Two Modes

| Mode | `isFullBlock` | Behavior |
|------|--------------|----------|
| **Banner Only** | `false` | App shows a warning banner but remains usable |
| **Full Block** | `true` | App shows a full-screen maintenance page, all API calls return `503` |

### Maintenance Flow

```
                    ┌──────────────────┐
                    │  Admin Dashboard  │
                    └────────┬─────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
     POST /maintenance  POST /maintenance  PUT /maintenance/{id}
     (scheduled)        /instant           (update existing)
              │              │              │
              ▼              ▼              ▼
     ┌────────────┐  ┌────────────┐  ┌────────────┐
     │  Scheduled  │  │  Starts    │  │  Modified   │
     │  for later  │  │  NOW for   │  │  existing   │
     │  startTime  │  │  N minutes │  │  window     │
     └──────┬─────┘  └──────┬─────┘  └────────────┘
            │               │
            ▼               ▼
     ┌────────────────────────────────┐
     │      Maintenance Active         │
     │                                 │
     │  isFullBlock = true:            │
     │   → All API calls return 503   │
     │   → App shows maintenance page │
     │                                 │
     │  isFullBlock = false:           │
     │   → APIs work normally          │
     │   → App shows warning banner   │
     └────────────────┬───────────────┘
                      │
               endTime reached
               (or admin deletes)
                      │
                      ▼
     ┌────────────────────────────────┐
     │      Maintenance Ended          │
     │   → Normal operation resumes   │
     └────────────────────────────────┘
```

### Middleware Integration

The Sekka API includes `MaintenanceMiddleware` that:
1. Checks for active maintenance windows on every request
2. If `isFullBlock = true` and current time is within the window:
   - Returns `503 Service Unavailable`
   - Response body includes maintenance `title` and `message`
   - Includes `Retry-After` header with estimated seconds remaining
3. Admin endpoints (`/admin/*`) are excluded from the block

### Flutter Handling

```dart
// In Dio error interceptor:
if (error.response?.statusCode == 503) {
  final data = error.response?.data;
  navigateToMaintenancePage(
    title: data['title'],
    message: data['message'],
    retryAfter: error.response?.headers['retry-after']?.first,
  );
}
```

---

## 14. Error Messages Reference

### Admin Errors
| Message | When |
|---------|------|
| `الميزة قيد التطوير: {feature}` | Stub endpoint called (feature not yet implemented) |
| `غير مصرح` | Missing or invalid admin role |
| `المفتاح غير موجود` | Config key not found |
| `الإصدار غير موجود` | Version ID not found |
| `نافذة الصيانة غير موجودة` | Maintenance window not found |
| `علم الميزة غير موجود` | Feature flag not found |
| `المنطقة غير موجودة` | Region not found |

### Webhook Errors
| Message | When |
|---------|------|
| `الويب هوك غير موجود` | Webhook ID not found |
| `غير مصرح بالوصول لهذا الويب هوك` | Webhook belongs to another driver |
| `عنوان URL الويب هوك مكرر` | Duplicate webhook URL |
| `فشل اختبار الويب هوك` | Test delivery to webhook URL failed |

### Audit Log Errors
| Message | When |
|---------|------|
| `لا توجد سجلات` | No audit logs found for filter |

### General Errors
| Message | When |
|---------|------|
| `حدث خطأ غير متوقع` | Unhandled server error |
| `طلب غير صالح` | Malformed request body |

---

## 15. Flutter Integration Examples

### Dio Setup with Admin Auth

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Token interceptor (same as AUTH_API.md)
dio.interceptors.add(InterceptorsWrapper(
  onRequest: (options, handler) {
    final token = getStoredToken();
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  },
  onError: (error, handler) async {
    if (error.response?.statusCode == 401) {
      final refreshed = await refreshToken();
      if (refreshed) {
        final retryResponse = await dio.fetch(error.requestOptions);
        handler.resolve(retryResponse);
      } else {
        navigateToLogin();
        handler.next(error);
      }
    } else if (error.response?.statusCode == 503) {
      // Maintenance mode
      final data = error.response?.data;
      navigateToMaintenancePage(
        title: data?['title'] ?? 'صيانة',
        message: data?['message'] ?? 'التطبيق تحت الصيانة',
      );
      handler.next(error);
    } else {
      handler.next(error);
    }
  },
));
```

### Check Version on App Launch

```dart
Future<void> checkAppVersion() async {
  try {
    final platform = Platform.isAndroid ? 0 : 1;
    final currentVersion = await PackageInfo.fromPlatform()
        .then((info) => info.version);

    final response = await dio.get('/config/check-version', queryParameters: {
      'platform': platform,
      'currentVersion': currentVersion,
    });

    if (response.data['isSuccess'] == true) {
      final data = response.data['data'];
      final isForceUpdate = data['isForceUpdate'] as bool;
      final latestVersion = data['latestVersion'] as String;
      final minRequired = data['minRequiredVersion'] as String;
      final storeUrl = data['storeUrl'] as String;
      final releaseNotes = data['releaseNotes'] as String?;

      if (isForceUpdate && _isVersionLower(currentVersion, minRequired)) {
        // Show blocking dialog — user MUST update
        showForceUpdateDialog(storeUrl, releaseNotes);
      } else if (_isVersionLower(currentVersion, latestVersion)) {
        // Show optional update dialog
        showOptionalUpdateDialog(storeUrl, releaseNotes);
      }
    }
  } on DioException catch (e) {
    // Silently fail — don't block app launch on network error
    debugPrint('Version check failed: ${e.message}');
  }
}

bool _isVersionLower(String current, String target) {
  final c = current.split('.').map(int.parse).toList();
  final t = target.split('.').map(int.parse).toList();
  for (var i = 0; i < 3; i++) {
    if (c[i] < t[i]) return true;
    if (c[i] > t[i]) return false;
  }
  return false;
}
```

### Fetch Feature Flags

```dart
class FeatureFlagService {
  final Dio _dio;
  Map<String, bool> _features = {};

  FeatureFlagService(this._dio);

  Map<String, bool> get features => _features;

  bool isEnabled(String key) => _features[key] ?? false;

  Future<void> fetchFeatures() async {
    try {
      final response = await _dio.get('/config/features');
      if (response.data['isSuccess'] == true) {
        final data = response.data['data'];
        _features = Map<String, bool>.from(data['features']);
        // Cache locally for offline use
        await _cacheFeatures(_features);
      }
    } on DioException {
      // Fall back to cached features
      _features = await _loadCachedFeatures();
    }
  }

  Future<void> _cacheFeatures(Map<String, bool> features) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('feature_flags', jsonEncode(features));
  }

  Future<Map<String, bool>> _loadCachedFeatures() async {
    final prefs = await SharedPreferences.getInstance();
    final cached = prefs.getString('feature_flags');
    if (cached != null) {
      return Map<String, bool>.from(jsonDecode(cached));
    }
    return {};
  }
}

// Usage in widgets:
if (featureFlagService.isEnabled('live_tracking')) {
  showLiveTrackingWidget();
}
```

### Fetch System Notices

```dart
Future<List<SystemNotice>> getNotices() async {
  try {
    final response = await dio.get('/config/notices');
    if (response.data['isSuccess'] == true) {
      final list = response.data['data'] as List;
      return list.map((e) => SystemNotice.fromJson(e)).toList();
    }
    return [];
  } on DioException {
    return [];
  }
}

// Display notices based on type:
Widget buildNoticeBanner(SystemNotice notice) {
  Color bgColor;
  switch (notice.noticeType) {
    case 0: bgColor = Colors.blue;   break; // Info
    case 1: bgColor = Colors.amber;  break; // Warning
    case 2: bgColor = Colors.green;  break; // Success
    case 3: bgColor = Colors.orange; break; // Promo
    case 4: bgColor = Colors.red;    break; // Urgent
  }

  // Use custom color if provided
  if (notice.backgroundColor != null) {
    bgColor = Color(int.parse(notice.backgroundColor!.replaceFirst('#', '0xFF')));
  }

  return MaterialBanner(
    content: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(notice.title, style: TextStyle(fontWeight: FontWeight.bold)),
        Text(notice.body),
      ],
    ),
    backgroundColor: bgColor.withOpacity(0.1),
    actions: [
      if (notice.actionUrl != null)
        TextButton(
          onPressed: () => launchUrl(Uri.parse(notice.actionUrl!)),
          child: Text(notice.actionLabel ?? 'اعرف المزيد'),
        ),
      if (notice.isDismissable)
        TextButton(
          onPressed: () => dismissNotice(notice.id),
          child: const Text('إغلاق'),
        ),
    ],
  );
}
```

### Webhook Management

```dart
// Get all webhooks
Future<List<WebhookConfig>> getWebhooks() async {
  final response = await dio.get('/webhooks');
  if (response.data['isSuccess'] == true) {
    final list = response.data['data'] as List;
    return list.map((e) => WebhookConfig.fromJson(e)).toList();
  }
  throw Exception(response.data['message']);
}

// Create a webhook
Future<WebhookConfig> createWebhook({
  required String name,
  required String url,
  required List<String> events,
  String? partnerId,
}) async {
  final response = await dio.post('/webhooks', data: {
    'name': name,
    'url': url,
    'events': events,
    if (partnerId != null) 'partnerId': partnerId,
  });

  if (response.data['isSuccess'] == true) {
    return WebhookConfig.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Test a webhook
Future<bool> testWebhook(String webhookId) async {
  try {
    final response = await dio.post('/webhooks/$webhookId/test');
    return response.data['isSuccess'] == true;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل اختبار الويب هوك');
    return false;
  }
}
```

### Admin: Audit Logs with Filtering

```dart
Future<PagedResult<AuditLog>> getAuditLogs({
  int page = 1,
  int pageSize = 20,
  String? entityType,
  String? userId,
  int? action,
  DateTime? dateFrom,
  DateTime? dateTo,
}) async {
  final response = await dio.get('/admin/audit-logs', queryParameters: {
    'page': page,
    'pageSize': pageSize,
    if (entityType != null) 'entityType': entityType,
    if (userId != null) 'userId': userId,
    if (action != null) 'action': action,
    if (dateFrom != null) 'dateFrom': dateFrom.toIso8601String(),
    if (dateTo != null) 'dateTo': dateTo.toIso8601String(),
  });

  if (response.data['isSuccess'] == true) {
    return PagedResult.fromJson(response.data['data'], AuditLog.fromJson);
  }
  throw Exception(response.data['message']);
}

// Get entity history (useful for "View Changes" on any entity detail page)
Future<List<AuditLog>> getEntityHistory(String entityType, String entityId) async {
  final response = await dio.get('/admin/audit-logs/entity/$entityType/$entityId');
  if (response.data['isSuccess'] == true) {
    final list = response.data['data'] as List;
    return list.map((e) => AuditLog.fromJson(e)).toList();
  }
  throw Exception(response.data['message']);
}

// Get audit summary for dashboard charts
Future<AuditSummary> getAuditSummary({DateTime? from, DateTime? to}) async {
  final response = await dio.get('/admin/audit-logs/summary', queryParameters: {
    if (from != null) 'dateFrom': from.toIso8601String(),
    if (to != null) 'dateTo': to.toIso8601String(),
  });

  if (response.data['isSuccess'] == true) {
    return AuditSummary.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}
```

### Admin: Populate Filter Dropdowns

```dart
// Fetch audit action types for dropdown
Future<List<EnumItem>> getAuditActions() async {
  final response = await dio.get('/admin/audit-logs/actions');
  if (response.data['isSuccess'] == true) {
    final list = response.data['data'] as List;
    return list.map((e) => EnumItem(value: e['value'], name: e['name'])).toList();
  }
  return [];
}

// Fetch entity types for dropdown
Future<List<String>> getTrackedEntities() async {
  final response = await dio.get('/admin/audit-logs/entities');
  if (response.data['isSuccess'] == true) {
    return List<String>.from(response.data['data']);
  }
  return [];
}
```
