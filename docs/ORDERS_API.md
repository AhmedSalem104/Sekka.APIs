# Sekka API - Orders & Delivery Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-26

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Order Lifecycle](#3-order-lifecycle)
4. [Endpoints — Orders](#4-endpoints--orders)
   - [Create Order](#41-create-order)
   - [List Orders](#42-list-orders)
   - [Get Order Details](#43-get-order-details)
   - [Update Order](#44-update-order)
   - [Delete Order](#45-delete-order)
   - [Update Order Status](#46-update-order-status)
   - [Mark as Delivered](#47-mark-as-delivered)
   - [Register Failed Delivery](#48-register-failed-delivery)
   - [Cancel Order](#49-cancel-order)
   - [Transfer to Another Driver](#410-transfer-to-another-driver)
   - [Partial Delivery](#411-partial-delivery)
   - [Bulk Import](#412-bulk-import)
   - [Check Duplicate](#413-check-duplicate)
   - [Calculate Worth Score](#414-calculate-worth-score)
   - [Upload Photo](#415-upload-photo)
   - [Swap Address](#416-swap-address)
   - [Start Waiting Timer](#417-start-waiting-timer)
   - [Stop Waiting Timer](#418-stop-waiting-timer)
   - [Calculate Price](#419-calculate-price)
5. [Endpoints — Routes](#5-endpoints--routes)
   - [Optimize Route](#51-optimize-route)
   - [Get Active Route](#52-get-active-route)
   - [Reorder Route](#53-reorder-route)
   - [Add Order to Route](#54-add-order-to-route)
   - [Complete Route](#55-complete-route)
6. [Endpoints — Recurring Orders](#6-endpoints--recurring-orders)
   - [List Recurring Orders](#61-list-recurring-orders)
   - [Create Recurring Order](#62-create-recurring-order)
   - [Update Recurring Pattern](#63-update-recurring-pattern)
   - [Pause Recurring Order](#64-pause-recurring-order)
   - [Resume Recurring Order](#65-resume-recurring-order)
   - [Delete Recurring Order](#66-delete-recurring-order)
7. [Endpoints — Sync (Offline Support)](#7-endpoints--sync-offline-support)
   - [Push Offline Changes](#71-push-offline-changes)
   - [Pull Server Updates](#72-pull-server-updates)
   - [Resolve Sync Conflict](#73-resolve-sync-conflict)
   - [Sync Status](#74-sync-status)
8. [Endpoints — Public Tracking](#8-endpoints--public-tracking)
   - [Track Order by Code](#81-track-order-by-code)
9. [Endpoints — Timeline](#9-endpoints--timeline)
   - [Daily Timeline](#91-daily-timeline)
   - [Range Timeline](#92-range-timeline)
   - [Filtered Timeline](#93-filtered-timeline)
10. [Endpoints — OCR (Under Development)](#10-endpoints--ocr-under-development)
    - [Scan Invoice](#101-scan-invoice)
    - [Scan to Order](#102-scan-to-order)
    - [Scan Batch](#103-scan-batch)
11. [Endpoints — Admin Orders](#11-endpoints--admin-orders)
    - [All Orders](#111-all-orders)
    - [Kanban Board](#112-kanban-board)
    - [Admin Create Order](#113-admin-create-order)
    - [Assign to Driver](#114-assign-to-driver)
    - [Auto Distribute](#115-auto-distribute)
    - [Order Timeline](#116-order-timeline)
    - [Override Status](#117-override-status)
    - [Export Orders](#118-export-orders)
    - [Auto-Assign with Config](#119-auto-assign-with-config)
    - [Suggested Drivers](#1110-suggested-drivers)
12. [Endpoints — Admin Time Slots](#12-endpoints--admin-time-slots)
    - [Get Time Slots](#121-get-time-slots)
    - [Create Time Slot](#122-create-time-slot)
    - [Update Time Slot](#123-update-time-slot)
    - [Delete Time Slot](#124-delete-time-slot)
    - [Generate Week Slots](#125-generate-week-slots)
    - [Slot Statistics](#126-slot-statistics)
13. [Enums](#13-enums)
14. [DTOs Reference](#14-dtos-reference)
15. [Flutter/Dio Integration](#15-flutterdio-integration)

---

## 1. Overview

وحدة الطلبات والتوصيل هي الوحدة الأساسية في سكّة. تتيح للسائقين إنشاء وإدارة طلبات التوصيل، تتبع حالتها، تحسين المسارات، والعمل بدون إنترنت مع مزامنة تلقائية.

| Feature | Detail |
|---------|--------|
| Orders CRUD | إنشاء، عرض، تعديل، حذف (soft delete) |
| Order Lifecycle | Pending → Accepted → PickedUp → InTransit → Delivered/Failed/Cancelled |
| Route Optimization | تحسين ترتيب التوصيل حسب الموقع |
| Offline Sync | دعم العمل بدون إنترنت مع مزامنة |
| Public Tracking | رابط تتبع عام للعميل (بدون تسجيل دخول) |
| Recurring Orders | طلبات متكررة (يومي/أسبوعي/شهري) |
| OCR | مسح الفواتير وتحويلها لطلبات (قيد التطوير) |
| Admin Panel | لوحة تحكم كاملة للمشرفين |

---

## 2. Response Format

جميع الاستجابات تتبع نفس التنسيق الموحد:

### Success Response
```json
{
  "isSuccess": true,
  "data": { ... },
  "message": "رسالة النجاح",
  "errors": null
}
```

### Paged Response
```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "totalCount": 150,
    "page": 1,
    "pageSize": 20,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
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

### HTTP Status Codes Used
| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid/missing token) |
| 403 | Forbidden (insufficient role) |
| 404 | Not Found |
| 409 | Conflict (duplicate order) |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Order Lifecycle

```
                        ┌──────────────┐
                        │   Pending    │  ← الطلب تم إنشاؤه
                        └──────┬───────┘
                               │
                        ┌──────▼───────┐
                        │   Accepted   │  ← السائق قبل الطلب
                        └──────┬───────┘
                               │
                        ┌──────▼───────┐
                        │  PickedUp    │  ← تم استلام الشحنة
                        └──────┬───────┘
                               │
                        ┌──────▼───────┐
                        │  InTransit   │  ← في الطريق للعميل
                        └──────┬───────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
       ┌──────▼───────┐ ┌─────▼──────┐ ┌───────▼──────┐
       │  Delivered   │ │   Failed   │ │  Cancelled   │
       │  (تم التسليم) │ │ (فشل التسليم)│ │   (ملغي)     │
       └──────────────┘ └─────┬──────┘ └──────────────┘
                              │
                       ┌──────▼───────┐
                       │  RetryPending│  ← إعادة المحاولة
                       └──────────────┘
```

### Status Transitions
| From | To | Trigger |
|------|----|---------|
| Pending | Accepted | PUT /{id}/status |
| Accepted | PickedUp | PUT /{id}/status |
| PickedUp | InTransit | PUT /{id}/status |
| InTransit | Delivered | POST /{id}/deliver |
| InTransit | Failed | POST /{id}/fail |
| Any (except Delivered) | Cancelled | POST /{id}/cancel |
| Failed | RetryPending | PUT /{id}/status |
| InTransit | PartiallyDelivered | POST /{id}/partial |

---

## 4. Endpoints -- Orders

### 4.1 Create Order

إنشاء طلب توصيل جديد.

```
POST /orders
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "customerName": "محمد أحمد",
  "customerPhone": "01098765432",
  "partnerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "طرد ملابس - 2 كيس",
  "amount": 150.00,
  "paymentMethod": 0,
  "priority": 1,
  "pickupAddress": "23 شارع التحرير، الدقي، الجيزة",
  "pickupLatitude": 30.0444,
  "pickupLongitude": 31.2357,
  "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
  "deliveryLatitude": 30.0561,
  "deliveryLongitude": 31.3467,
  "notes": "الدور الثالث - شقة 5",
  "itemCount": 2,
  "timeWindowStart": "2026-03-26T14:00:00Z",
  "timeWindowEnd": "2026-03-26T18:00:00Z",
  "scheduledDate": "2026-03-26",
  "isRecurring": false,
  "recurrencePattern": null,
  "expectedChangeAmount": 50.00,
  "idempotencyKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| customerName | string? | No | اسم العميل |
| customerPhone | string? | No | رقم موبايل العميل |
| partnerId | Guid? | No | معرّف الشريك/التاجر |
| description | string? | No | وصف الشحنة |
| amount | decimal | **Yes** | المبلغ المطلوب تحصيله |
| paymentMethod | int | **Yes** | 0=Cash, 1=Visa, 2=Wallet (see [Enums](#13-enums)) |
| priority | int | No | 0=Normal, 1=Urgent, 2=VIP (default: 0) |
| pickupAddress | string? | No | عنوان الاستلام |
| pickupLatitude | double? | No | خط عرض الاستلام |
| pickupLongitude | double? | No | خط طول الاستلام |
| deliveryAddress | string | **Yes** | عنوان التسليم |
| deliveryLatitude | double? | No | خط عرض التسليم |
| deliveryLongitude | double? | No | خط طول التسليم |
| notes | string? | No | ملاحظات للسائق |
| itemCount | int | No | عدد القطع (default: 1) |
| timeWindowStart | DateTime? | No | بداية نافذة التسليم |
| timeWindowEnd | DateTime? | No | نهاية نافذة التسليم |
| scheduledDate | Date? | No | تاريخ التسليم المجدول |
| isRecurring | bool | No | هل هو طلب متكرر (default: false) |
| recurrencePattern | string? | No | نمط التكرار (Daily/Weekly/Monthly) |
| expectedChangeAmount | decimal? | No | مبلغ الفكة المتوقع |
| idempotencyKey | string? | No | مفتاح منع التكرار (لدعم الـ offline) |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "customerName": "محمد أحمد",
    "customerPhone": "01098765432",
    "partnerName": "متجر الأمل",
    "partnerColor": "#FF5722",
    "description": "طرد ملابس - 2 كيس",
    "amount": 150.00,
    "commissionAmount": 15.00,
    "paymentMethod": 0,
    "status": 0,
    "priority": 1,
    "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
    "distanceKm": 12.5,
    "sequenceIndex": null,
    "worthScore": null,
    "createdAt": "2026-03-26T10:30:00Z",
    "deliveredAt": null
  },
  "message": "تم إنشاء الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `عنوان التسليم مطلوب` |
| 400 | `المبلغ يجب أن يكون أكبر من صفر` |
| 400 | `طريقة الدفع غير صالحة` |
| 409 | `طلب مكرر — يوجد طلب بنفس الـ idempotencyKey` |

---

### 4.2 List Orders

عرض قائمة الطلبات مع فلترة وترتيب وتصفح.

```
GET /orders
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| status | int? | null | فلتر حسب الحالة (see [OrderStatus](#orderstatus)) |
| partnerId | Guid? | null | فلتر حسب الشريك |
| dateFrom | DateTime? | null | من تاريخ |
| dateTo | DateTime? | null | إلى تاريخ |
| searchTerm | string? | null | بحث بالاسم أو رقم الطلب أو العنوان |
| paymentMethod | int? | null | فلتر حسب طريقة الدفع |
| priority | int? | null | فلتر حسب الأولوية |
| page | int | 1 | رقم الصفحة |
| pageSize | int | 20 | عدد النتائج بالصفحة |

**Example**:
```
GET /orders?status=3&dateFrom=2026-03-20&page=1&pageSize=10
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "customerName": "محمد أحمد",
        "amount": 150.00,
        "status": 3,
        "priority": 1,
        "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
        "partnerColor": "#FF5722",
        "sequenceIndex": 2,
        "createdAt": "2026-03-26T10:30:00Z"
      },
      {
        "id": "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f",
        "orderNumber": "SK-20260326-0043",
        "customerName": "سارة علي",
        "amount": 320.50,
        "status": 3,
        "priority": 0,
        "deliveryAddress": "8 شارع الهرم، الجيزة",
        "partnerColor": "#4CAF50",
        "sequenceIndex": 3,
        "createdAt": "2026-03-26T11:15:00Z"
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 4.3 Get Order Details

عرض تفاصيل طلب واحد بالكامل.

```
GET /orders/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "customerName": "محمد أحمد",
    "customerPhone": "01098765432",
    "partnerName": "متجر الأمل",
    "partnerColor": "#FF5722",
    "description": "طرد ملابس - 2 كيس",
    "amount": 150.00,
    "commissionAmount": 15.00,
    "paymentMethod": 0,
    "status": 3,
    "priority": 1,
    "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
    "distanceKm": 12.5,
    "sequenceIndex": 2,
    "worthScore": 78.5,
    "createdAt": "2026-03-26T10:30:00Z",
    "deliveredAt": null
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

### 4.4 Update Order

تعديل بيانات طلب (فقط في حالة Pending أو Accepted).

```
PUT /orders/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**: نفس body الخاص بـ [Create Order](#41-create-order)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "customerName": "محمد أحمد",
    "customerPhone": "01098765432",
    "partnerName": "متجر الأمل",
    "partnerColor": "#FF5722",
    "description": "طرد ملابس - 3 كيس",
    "amount": 200.00,
    "commissionAmount": 20.00,
    "paymentMethod": 0,
    "status": 0,
    "priority": 1,
    "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
    "distanceKm": 12.5,
    "sequenceIndex": null,
    "worthScore": null,
    "createdAt": "2026-03-26T10:30:00Z",
    "deliveredAt": null
  },
  "message": "تم تعديل الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن تعديل طلب في هذه الحالة` |
| 404 | `الطلب غير موجود` |

---

### 4.5 Delete Order

حذف طلب (soft delete). فقط للطلبات في حالة Pending.

```
DELETE /orders/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن حذف طلب في هذه الحالة` |
| 404 | `الطلب غير موجود` |

---

### 4.6 Update Order Status

تغيير حالة الطلب (Accepted, PickedUp, InTransit, etc.).

```
PUT /orders/{id}/status
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "status": 1,
  "latitude": 30.0444,
  "longitude": 31.2357,
  "notes": "تم القبول — في الطريق للاستلام"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| status | int | **Yes** | الحالة الجديدة (see [OrderStatus](#orderstatus)) |
| latitude | double? | No | موقع السائق الحالي |
| longitude | double? | No | موقع السائق الحالي |
| notes | string? | No | ملاحظات |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "status": 1,
    "previousStatus": 0,
    "updatedAt": "2026-03-26T10:45:00Z"
  },
  "message": "تم تحديث حالة الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن الانتقال من حالة {from} إلى حالة {to}` |
| 404 | `الطلب غير موجود` |

---

### 4.7 Mark as Delivered

تسجيل تسليم الطلب بنجاح.

```
POST /orders/{id}/deliver
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "actualCollectedAmount": 150.00,
  "latitude": 30.0561,
  "longitude": 31.3467,
  "notes": "تم التسليم للعميل شخصيًا",
  "ratingValue": 5
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| actualCollectedAmount | decimal? | No | المبلغ المحصل فعليًا |
| latitude | double? | No | موقع التسليم |
| longitude | double? | No | موقع التسليم |
| notes | string? | No | ملاحظات التسليم |
| ratingValue | int? | No | تقييم العميل (1-5) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "status": 5,
    "deliveredAt": "2026-03-26T14:22:00Z",
    "actualCollectedAmount": 150.00,
    "commissionAmount": 15.00,
    "netAmount": 135.00
  },
  "message": "تم تسليم الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الطلب ليس في حالة تسمح بالتسليم` |
| 400 | `التقييم يجب أن يكون بين 1 و 5` |
| 404 | `الطلب غير موجود` |

---

### 4.8 Register Failed Delivery

تسجيل محاولة توصيل فاشلة.

```
POST /orders/{id}/fail
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "reason": 0,
  "reasonText": "العميل لم يرد على الموبايل",
  "latitude": 30.0561,
  "longitude": 31.3467,
  "sendAutoMessage": true
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| reason | int | **Yes** | سبب الفشل (see [DeliveryFailReason](#deliveryfailreason)) |
| reasonText | string? | No | تفاصيل إضافية |
| latitude | double? | No | موقع المحاولة |
| longitude | double? | No | موقع المحاولة |
| sendAutoMessage | bool | No | إرسال رسالة تلقائية للعميل (default: true) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "status": 6,
    "failAttemptNumber": 1,
    "maxAttempts": 3,
    "canRetry": true,
    "nextRetryAt": "2026-03-27T10:00:00Z"
  },
  "message": "تم تسجيل محاولة التوصيل الفاشلة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `سبب الفشل مطلوب` |
| 400 | `الطلب ليس في حالة تسمح بتسجيل فشل` |
| 404 | `الطلب غير موجود` |

---

### 4.9 Cancel Order

إلغاء طلب مع توثيق الخسائر.

```
POST /orders/{id}/cancel
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "cancellationReason": 2,
  "reasonText": "العميل طلب إلغاء الطلب",
  "lossAmount": 25.00,
  "distanceTravelledKm": 5.3,
  "fuelCostLost": 12.50,
  "transferToDriverId": null
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| cancellationReason | int | **Yes** | سبب الإلغاء (see [CancellationReason](#cancellationreason)) |
| reasonText | string? | No | تفاصيل إضافية |
| lossAmount | decimal | No | مبلغ الخسارة (default: 0) |
| distanceTravelledKm | double? | No | المسافة المقطوعة قبل الإلغاء |
| fuelCostLost | decimal? | No | تكلفة الوقود المهدر |
| transferToDriverId | Guid? | No | تحويل لسائق آخر بدلًا من الإلغاء |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "status": 7,
    "cancellationReason": 2,
    "lossAmount": 25.00,
    "cancelledAt": "2026-03-26T13:00:00Z"
  },
  "message": "تم إلغاء الطلب",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `سبب الإلغاء مطلوب` |
| 400 | `لا يمكن إلغاء طلب تم تسليمه` |
| 404 | `الطلب غير موجود` |

---

### 4.10 Transfer to Another Driver

تحويل طلب لسائق آخر.

```
POST /orders/{id}/transfer
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "targetDriverId": "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
  "reason": "أنا بعيد عن منطقة التسليم",
  "transferWithItems": true
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| targetDriverId | Guid | **Yes** | معرّف السائق المستلم |
| reason | string? | No | سبب التحويل |
| transferWithItems | bool | No | هل الشحنة مع السائق الحالي (default: true) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "transferId": "e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b",
    "transferStatus": 0,
    "fromDriverName": "أحمد محمد",
    "toDriverName": "عمر حسن",
    "transferredAt": "2026-03-26T12:30:00Z"
  },
  "message": "تم تحويل الطلب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن تحويل طلب في هذه الحالة` |
| 400 | `لا يمكن التحويل لنفس السائق` |
| 404 | `الطلب غير موجود` |
| 404 | `السائق المستلم غير موجود` |

---

### 4.11 Partial Delivery

تسجيل تسليم جزئي (بعض القطع فقط).

```
POST /orders/{id}/partial
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "deliveredItemCount": 1,
  "totalItemCount": 3,
  "collectedAmount": 100.00,
  "remainingAmount": 200.00,
  "reason": "العميل رفض قطعة واحدة — تالفة",
  "latitude": 30.0561,
  "longitude": 31.3467
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| deliveredItemCount | int | **Yes** | عدد القطع المسلّمة |
| totalItemCount | int | **Yes** | إجمالي عدد القطع |
| collectedAmount | decimal | **Yes** | المبلغ المحصل |
| remainingAmount | decimal | **Yes** | المبلغ المتبقي |
| reason | string? | No | سبب التسليم الجزئي |
| latitude | double? | No | موقع التسليم |
| longitude | double? | No | موقع التسليم |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "status": 8,
    "deliveredItemCount": 1,
    "totalItemCount": 3,
    "collectedAmount": 100.00,
    "remainingAmount": 200.00
  },
  "message": "تم تسجيل التسليم الجزئي",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `عدد القطع المسلّمة يجب أن يكون أقل من الإجمالي` |
| 404 | `الطلب غير موجود` |

---

### 4.12 Bulk Import

استيراد طلبات متعددة من نص منسوخ (clipboard).

```
POST /orders/bulk
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "text": "محمد أحمد\t01098765432\t15 شارع النحاس\t150\nسارة علي\t01112345678\t8 شارع الهرم\t320",
  "delimiter": "\t",
  "defaultPaymentMethod": 0,
  "defaultPriority": 0,
  "partnerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| text | string | **Yes** | النص المنسوخ (كل سطر = طلب) |
| delimiter | string | No | الفاصل (default: `\t` tab) |
| defaultPaymentMethod | int | No | طريقة الدفع الافتراضية (default: 0) |
| defaultPriority | int | No | الأولوية الافتراضية (default: 0) |
| partnerId | Guid? | No | الشريك الافتراضي |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "importedCount": 2,
    "failedCount": 0,
    "orders": [
      {
        "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0044",
        "customerName": "محمد أحمد",
        "amount": 150.00
      },
      {
        "id": "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f",
        "orderNumber": "SK-20260326-0045",
        "customerName": "سارة علي",
        "amount": 320.00
      }
    ],
    "errors": []
  },
  "message": "تم استيراد 2 طلبات بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `النص المدخل فارغ` |
| 400 | `فشل تحليل السطر رقم {n}` |

---

### 4.13 Check Duplicate

التحقق من وجود طلب مكرر قبل الإنشاء.

```
POST /orders/check-duplicate
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "customerPhone": "01098765432",
  "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
  "amount": 150.00,
  "idempotencyKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "isDuplicate": true,
    "existingOrderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "existingOrderNumber": "SK-20260326-0042",
    "matchReason": "نفس رقم العميل والعنوان والمبلغ"
  },
  "message": null,
  "errors": null
}
```

---

### 4.14 Calculate Worth Score

حساب نقاط القيمة (Worth Score) لتحديد أولوية الطلب.

```
POST /orders/{id}/worth
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "worthScore": 78.5,
    "breakdown": {
      "amountScore": 30.0,
      "distanceScore": 20.5,
      "priorityScore": 15.0,
      "timeWindowScore": 8.0,
      "partnerScore": 5.0
    }
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

### 4.15 Upload Photo

رفع صورة توثيقية للطلب (إثبات تسليم، صورة شحنة، إلخ).

```
POST /orders/{id}/photos
```

**Auth Required**: Yes (Bearer Token)

**Content-Type**: `multipart/form-data`

**URL Params**: `id` (GUID) - معرّف الطلب

**Form Fields**:
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| file | File | **Yes** | ملف الصورة (JPEG, PNG, WebP — max 10MB) |
| photoType | int | **Yes** | نوع الصورة (see [PhotoType](#phototype)) |
| description | string? | No | وصف الصورة |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "photoId": "f1a2b3c4-d5e6-7f8a-9b0c-1d2e3f4a5b6c",
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "photoUrl": "https://sekka.runasp.net/uploads/orders/2026/03/26/photo_abc123.jpg",
    "photoType": 0,
    "uploadedAt": "2026-03-26T14:25:00Z"
  },
  "message": "تم رفع الصورة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الملف مطلوب` |
| 400 | `حجم الملف يتجاوز الحد المسموح (10MB)` |
| 400 | `نوع الملف غير مدعوم` |
| 404 | `الطلب غير موجود` |

---

### 4.16 Swap Address

تغيير عنوان التسليم أثناء الرحلة.

```
POST /orders/{id}/swap-address
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "newDeliveryAddress": "22 شارع الملك فيصل، الجيزة",
  "newLatitude": 30.0131,
  "newLongitude": 31.2089,
  "reason": "العميل طلب التغيير"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| newDeliveryAddress | string | **Yes** | العنوان الجديد |
| newLatitude | double? | No | خط عرض العنوان الجديد |
| newLongitude | double? | No | خط طول العنوان الجديد |
| reason | string? | No | سبب التغيير |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "previousAddress": "15 شارع مصطفى النحاس، مدينة نصر، القاهرة",
    "newAddress": "22 شارع الملك فيصل، الجيزة",
    "additionalDistanceKm": 3.2,
    "updatedAt": "2026-03-26T13:15:00Z"
  },
  "message": "تم تغيير عنوان التسليم بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `العنوان الجديد مطلوب` |
| 400 | `لا يمكن تغيير العنوان بعد التسليم` |
| 404 | `الطلب غير موجود` |

---

### 4.17 Start Waiting Timer

بدء مؤقت الانتظار عند العميل.

```
POST /orders/{id}/waiting/start
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "latitude": 30.0561,
  "longitude": 31.3467
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "waitingStartedAt": "2026-03-26T14:10:00Z",
    "freeWaitingMinutes": 10,
    "chargePerMinute": 1.50
  },
  "message": "تم بدء مؤقت الانتظار",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `مؤقت الانتظار يعمل بالفعل` |
| 400 | `الطلب ليس في حالة InTransit` |
| 404 | `الطلب غير موجود` |

---

### 4.18 Stop Waiting Timer

إيقاف مؤقت الانتظار.

```
POST /orders/{id}/waiting/stop
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "waitingStartedAt": "2026-03-26T14:10:00Z",
    "waitingEndedAt": "2026-03-26T14:25:00Z",
    "totalWaitingMinutes": 15,
    "freeMinutes": 10,
    "chargeableMinutes": 5,
    "waitingCharge": 7.50
  },
  "message": "تم إيقاف مؤقت الانتظار",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يوجد مؤقت انتظار نشط` |
| 404 | `الطلب غير موجود` |

---

### 4.19 Calculate Price

حساب السعر المقترح للتوصيل بناءً على المسافة والمنطقة.

```
POST /orders/calculate-price
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "pickupLatitude": 30.0444,
  "pickupLongitude": 31.2357,
  "deliveryLatitude": 30.0561,
  "deliveryLongitude": 31.3467,
  "itemCount": 2,
  "priority": 1,
  "vehicleType": 0
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| pickupLatitude | double | **Yes** | خط عرض الاستلام |
| pickupLongitude | double | **Yes** | خط طول الاستلام |
| deliveryLatitude | double | **Yes** | خط عرض التسليم |
| deliveryLongitude | double | **Yes** | خط طول التسليم |
| itemCount | int | No | عدد القطع (default: 1) |
| priority | int | No | الأولوية |
| vehicleType | int | No | نوع المركبة |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "suggestedPrice": 45.00,
    "distanceKm": 12.5,
    "estimatedDurationMinutes": 35,
    "breakdown": {
      "baseFare": 15.00,
      "distanceFare": 25.00,
      "prioritySurcharge": 5.00,
      "itemSurcharge": 0.00
    }
  },
  "message": null,
  "errors": null
}
```

---

## 5. Endpoints -- Routes

### 5.1 Optimize Route

تحسين ترتيب الطلبات في المسار لأقصر مسافة/وقت.

```
POST /routes/optimize
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "orderIds": [
    "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f",
    "d9a3e4f5-6b7c-8d9e-0f1a-2b3c4d5e6f7a"
  ],
  "startLatitude": 30.0444,
  "startLongitude": 31.2357,
  "optimizeFor": "distance"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| orderIds | Guid[] | **Yes** | قائمة معرّفات الطلبات |
| startLatitude | double | **Yes** | موقع البداية |
| startLongitude | double | **Yes** | موقع البداية |
| optimizeFor | string | No | `distance` أو `time` (default: `distance`) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "routeId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "totalDistanceKm": 28.3,
    "estimatedDurationMinutes": 75,
    "optimizedOrder": [
      {
        "sequenceIndex": 1,
        "orderId": "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f",
        "orderNumber": "SK-20260326-0043",
        "deliveryAddress": "8 شارع الهرم، الجيزة",
        "distanceFromPrevious": 8.1
      },
      {
        "sequenceIndex": 2,
        "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
        "distanceFromPrevious": 10.7
      },
      {
        "sequenceIndex": 3,
        "orderId": "d9a3e4f5-6b7c-8d9e-0f1a-2b3c4d5e6f7a",
        "orderNumber": "SK-20260326-0044",
        "deliveryAddress": "3 شارع المعز، الحسين",
        "distanceFromPrevious": 9.5
      }
    ]
  },
  "message": "تم تحسين المسار بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `يجب اختيار طلبين على الأقل` |
| 400 | `بعض الطلبات لا تحتوي على إحداثيات` |

---

### 5.2 Get Active Route

عرض المسار النشط الحالي للسائق.

```
GET /routes/active
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "routeId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": "Active",
    "totalDistanceKm": 28.3,
    "completedDistanceKm": 8.1,
    "totalOrders": 3,
    "completedOrders": 1,
    "estimatedRemainingMinutes": 50,
    "orders": [
      {
        "sequenceIndex": 1,
        "orderId": "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f",
        "orderNumber": "SK-20260326-0043",
        "customerName": "سارة علي",
        "deliveryAddress": "8 شارع الهرم، الجيزة",
        "status": 5,
        "isCompleted": true
      },
      {
        "sequenceIndex": 2,
        "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "customerName": "محمد أحمد",
        "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
        "status": 3,
        "isCompleted": false
      },
      {
        "sequenceIndex": 3,
        "orderId": "d9a3e4f5-6b7c-8d9e-0f1a-2b3c4d5e6f7a",
        "orderNumber": "SK-20260326-0044",
        "customerName": "ياسمين خالد",
        "deliveryAddress": "3 شارع المعز، الحسين",
        "status": 1,
        "isCompleted": false
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
| 404 | `لا يوجد مسار نشط حاليًا` |

---

### 5.3 Reorder Route

إعادة ترتيب الطلبات يدويًا في المسار.

```
PUT /routes/{id}/reorder
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف المسار

**Request Body**:
```json
{
  "orderSequence": [
    { "orderId": "d9a3e4f5-6b7c-8d9e-0f1a-2b3c4d5e6f7a", "sequenceIndex": 1 },
    { "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e", "sequenceIndex": 2 },
    { "orderId": "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f", "sequenceIndex": 3 }
  ]
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إعادة ترتيب المسار بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `ترتيب غير صالح — عدد الطلبات لا يتطابق` |
| 404 | `المسار غير موجود` |

---

### 5.4 Add Order to Route

إضافة طلب جديد للمسار النشط.

```
POST /routes/{id}/add-order
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف المسار

**Request Body**:
```json
{
  "orderId": "e0b4f5a6-7c8d-9e0f-1a2b-3c4d5e6f7a8b",
  "insertAtIndex": 2
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| orderId | Guid | **Yes** | معرّف الطلب المراد إضافته |
| insertAtIndex | int? | No | موضع الإدراج (null = آخر المسار) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "routeId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "totalOrders": 4,
    "newTotalDistanceKm": 32.1,
    "estimatedDurationMinutes": 85
  },
  "message": "تم إضافة الطلب للمسار",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الطلب موجود بالفعل في المسار` |
| 404 | `المسار غير موجود` |
| 404 | `الطلب غير موجود` |

---

### 5.5 Complete Route

إنهاء المسار الحالي.

```
PUT /routes/{id}/complete
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف المسار

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "routeId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": "Completed",
    "totalOrders": 3,
    "deliveredOrders": 2,
    "failedOrders": 1,
    "totalDistanceKm": 28.3,
    "actualDurationMinutes": 82,
    "totalCollected": 470.50,
    "totalCommission": 47.05,
    "completedAt": "2026-03-26T16:30:00Z"
  },
  "message": "تم إنهاء المسار بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `يوجد طلبات لم تُنهَ بعد` |
| 404 | `المسار غير موجود` |

---

## 6. Endpoints -- Recurring Orders

### 6.1 List Recurring Orders

عرض الطلبات المتكررة.

```
GET /orders/recurring
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| page | int | 1 | رقم الصفحة |
| pageSize | int | 20 | عدد النتائج |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "r1a2b3c4-d5e6-7f8a-9b0c-1d2e3f4a5b6c",
        "customerName": "شركة ABC للشحن",
        "deliveryAddress": "المنطقة الصناعية، 6 أكتوبر",
        "amount": 500.00,
        "recurrencePattern": "Daily",
        "nextOccurrence": "2026-03-27T09:00:00Z",
        "isActive": true,
        "totalGenerated": 15,
        "createdAt": "2026-03-10T08:00:00Z"
      }
    ],
    "totalCount": 3,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 6.2 Create Recurring Order

إنشاء طلب متكرر.

```
POST /orders/recurring
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "customerName": "شركة ABC للشحن",
  "customerPhone": "01055512345",
  "deliveryAddress": "المنطقة الصناعية، 6 أكتوبر",
  "amount": 500.00,
  "paymentMethod": 0,
  "recurrencePattern": "Daily",
  "recurrenceStartDate": "2026-03-27",
  "recurrenceEndDate": "2026-06-27",
  "timeWindowStart": "09:00:00",
  "timeWindowEnd": "12:00:00",
  "daysOfWeek": [0, 1, 2, 3, 4],
  "notes": "شحنة يومية — أيام العمل فقط"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| customerName | string | **Yes** | اسم العميل |
| customerPhone | string? | No | رقم العميل |
| deliveryAddress | string | **Yes** | عنوان التسليم |
| amount | decimal | **Yes** | المبلغ |
| paymentMethod | int | **Yes** | طريقة الدفع |
| recurrencePattern | string | **Yes** | `Daily`, `Weekly`, `Monthly` |
| recurrenceStartDate | Date | **Yes** | تاريخ البداية |
| recurrenceEndDate | Date? | No | تاريخ النهاية (null = بلا نهاية) |
| timeWindowStart | Time? | No | بداية وقت التسليم |
| timeWindowEnd | Time? | No | نهاية وقت التسليم |
| daysOfWeek | int[]? | No | أيام الأسبوع (0=Sunday ... 6=Saturday) |
| notes | string? | No | ملاحظات |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "r1a2b3c4-d5e6-7f8a-9b0c-1d2e3f4a5b6c",
    "recurrencePattern": "Daily",
    "nextOccurrence": "2026-03-27T09:00:00Z",
    "isActive": true
  },
  "message": "تم إنشاء الطلب المتكرر بنجاح",
  "errors": null
}
```

---

### 6.3 Update Recurring Pattern

تعديل نمط التكرار.

```
PUT /orders/recurring/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب المتكرر

**Request Body**: نفس body الخاص بـ [Create Recurring Order](#62-create-recurring-order)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الطلب المتكرر بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب المتكرر غير موجود` |

---

### 6.4 Pause Recurring Order

إيقاف الطلب المتكرر مؤقتًا.

```
POST /orders/recurring/{id}/pause
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب المتكرر

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إيقاف الطلب المتكرر مؤقتًا",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الطلب المتكرر متوقف بالفعل` |
| 404 | `الطلب المتكرر غير موجود` |

---

### 6.5 Resume Recurring Order

استئناف الطلب المتكرر.

```
POST /orders/recurring/{id}/resume
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب المتكرر

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "nextOccurrence": "2026-03-28T09:00:00Z"
  },
  "message": "تم استئناف الطلب المتكرر",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الطلب المتكرر نشط بالفعل` |
| 404 | `الطلب المتكرر غير موجود` |

---

### 6.6 Delete Recurring Order

حذف طلب متكرر (لا يحذف الطلبات التي تم إنشاؤها بالفعل).

```
DELETE /orders/recurring/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - معرّف الطلب المتكرر

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الطلب المتكرر",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الطلب المتكرر غير موجود` |

---

## 7. Endpoints -- Sync (Offline Support)

> دعم العمل بدون إنترنت — السائق يقدر يشتغل وهو offline والتطبيق يزامن التغييرات لما النت يرجع.

### 7.1 Push Offline Changes

رفع التغييرات التي تمت بدون إنترنت.

```
POST /sync/push
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "changes": [
    {
      "entityType": "Order",
      "entityId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
      "operation": "Update",
      "data": {
        "status": 5,
        "deliveredAt": "2026-03-26T14:22:00Z",
        "actualCollectedAmount": 150.00
      },
      "timestamp": "2026-03-26T14:22:00Z",
      "localId": "local_001"
    },
    {
      "entityType": "Order",
      "entityId": null,
      "operation": "Create",
      "data": {
        "customerName": "خالد محمود",
        "deliveryAddress": "5 شارع فيصل",
        "amount": 75.00,
        "paymentMethod": 0
      },
      "timestamp": "2026-03-26T14:25:00Z",
      "localId": "local_002"
    }
  ],
  "deviceId": "device_abc123",
  "lastSyncTimestamp": "2026-03-26T10:00:00Z"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| changes | array | **Yes** | قائمة التغييرات |
| changes[].entityType | string | **Yes** | `Order`, `Route`, etc. |
| changes[].entityId | Guid? | No | null إذا كان Create |
| changes[].operation | string | **Yes** | `Create`, `Update`, `Delete` |
| changes[].data | object | **Yes** | البيانات المتغيرة |
| changes[].timestamp | DateTime | **Yes** | وقت التغيير المحلي |
| changes[].localId | string | **Yes** | معرّف محلي لربط النتائج |
| deviceId | string | **Yes** | معرّف الجهاز |
| lastSyncTimestamp | DateTime | **Yes** | آخر وقت مزامنة |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "syncedCount": 2,
    "conflictCount": 0,
    "results": [
      {
        "localId": "local_001",
        "status": "Synced",
        "serverId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e"
      },
      {
        "localId": "local_002",
        "status": "Synced",
        "serverId": "f0a1b2c3-d4e5-6f7a-8b9c-0d1e2f3a4b5c"
      }
    ],
    "conflicts": [],
    "serverTimestamp": "2026-03-26T14:30:00Z"
  },
  "message": "تم المزامنة بنجاح",
  "errors": null
}
```

---

### 7.2 Pull Server Updates

جلب التحديثات من السيرفر بعد آخر مزامنة.

```
GET /sync/pull
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Notes |
|-------|------|----------|-------|
| since | DateTime | **Yes** | آخر وقت مزامنة |
| entityTypes | string | No | أنواع الكيانات مفصولة بفاصلة (e.g., `Order,Route`) |

**Example**:
```
GET /sync/pull?since=2026-03-26T10:00:00Z&entityTypes=Order,Route
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "updates": [
      {
        "entityType": "Order",
        "entityId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "operation": "Update",
        "data": {
          "status": 1,
          "updatedAt": "2026-03-26T11:00:00Z"
        },
        "serverTimestamp": "2026-03-26T11:00:00Z"
      }
    ],
    "deletions": [],
    "serverTimestamp": "2026-03-26T14:30:00Z",
    "hasMore": false
  },
  "message": null,
  "errors": null
}
```

---

### 7.3 Resolve Sync Conflict

حل تعارض في المزامنة يدويًا.

```
POST /sync/resolve-conflict
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "conflictId": "conf_001",
  "resolution": "KeepLocal",
  "entityType": "Order",
  "entityId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
  "resolvedData": {
    "status": 5,
    "deliveredAt": "2026-03-26T14:22:00Z"
  }
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| conflictId | string | **Yes** | معرّف التعارض |
| resolution | string | **Yes** | `KeepLocal`, `KeepServer`, `Merge` |
| entityType | string | **Yes** | نوع الكيان |
| entityId | Guid | **Yes** | معرّف الكيان |
| resolvedData | object? | No | البيانات المدمجة (مطلوب إذا `Merge`) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حل التعارض بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `التعارض غير موجود` |
| 400 | `resolvedData مطلوب عند اختيار Merge` |

---

### 7.4 Sync Status

عرض حالة المزامنة الحالية.

```
GET /sync/status
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "lastSyncAt": "2026-03-26T14:30:00Z",
    "pendingChanges": 0,
    "unresolvedConflicts": 0,
    "serverVersion": "2026.3.26.1",
    "isUpToDate": true
  },
  "message": null,
  "errors": null
}
```

---

## 8. Endpoints -- Public Tracking

### 8.1 Track Order by Code

صفحة تتبع عامة — العميل يقدر يتتبع الطلب بدون تسجيل دخول.

```
GET /tracking/{code}
```

**Auth Required**: No (Public)

**URL Params**: `code` (string) - كود التتبع (يُرسل للعميل عبر SMS)

**Example**:
```
GET /tracking/SK42-7X3M
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderNumber": "SK-20260326-0042",
    "status": 3,
    "statusText": "في الطريق إليك",
    "driverName": "أحمد م.",
    "driverPhone": "+20101234****",
    "estimatedArrival": "2026-03-26T14:30:00Z",
    "estimatedMinutesRemaining": 15,
    "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
    "currentLatitude": 30.0500,
    "currentLongitude": 31.3200,
    "timeline": [
      {
        "status": 0,
        "statusText": "تم إنشاء الطلب",
        "timestamp": "2026-03-26T10:30:00Z"
      },
      {
        "status": 1,
        "statusText": "تم قبول الطلب",
        "timestamp": "2026-03-26T10:45:00Z"
      },
      {
        "status": 2,
        "statusText": "تم استلام الشحنة",
        "timestamp": "2026-03-26T12:00:00Z"
      },
      {
        "status": 3,
        "statusText": "في الطريق إليك",
        "timestamp": "2026-03-26T13:30:00Z"
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
| 404 | `كود التتبع غير صالح` |

---

## 9. Endpoints -- Timeline

### 9.1 Daily Timeline

عرض الجدول الزمني ليوم واحد (كل الأحداث والتحركات).

```
GET /timeline/daily
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Notes |
|-------|------|----------|-------|
| date | Date | **Yes** | التاريخ (format: `yyyy-MM-dd`) |

**Example**:
```
GET /timeline/daily?date=2026-03-26
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-26",
    "totalEvents": 12,
    "totalOrders": 5,
    "deliveredOrders": 3,
    "totalCollected": 670.50,
    "events": [
      {
        "id": "evt_001",
        "type": 0,
        "title": "بداية اليوم",
        "description": "تم تسجيل الدخول",
        "timestamp": "2026-03-26T08:00:00Z",
        "orderId": null,
        "orderNumber": null,
        "latitude": 30.0444,
        "longitude": 31.2357
      },
      {
        "id": "evt_002",
        "type": 1,
        "title": "طلب جديد",
        "description": "SK-20260326-0042 — محمد أحمد — 150 ج.م",
        "timestamp": "2026-03-26T10:30:00Z",
        "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "latitude": null,
        "longitude": null
      },
      {
        "id": "evt_003",
        "type": 3,
        "title": "تم التسليم",
        "description": "SK-20260326-0042 — تم تحصيل 150 ج.م",
        "timestamp": "2026-03-26T14:22:00Z",
        "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "latitude": 30.0561,
        "longitude": 31.3467
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 9.2 Range Timeline

عرض الجدول الزمني لفترة زمنية.

```
GET /timeline/range
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Notes |
|-------|------|----------|-------|
| dateFrom | Date | **Yes** | من تاريخ |
| dateTo | Date | **Yes** | إلى تاريخ |

**Example**:
```
GET /timeline/range?dateFrom=2026-03-20&dateTo=2026-03-26
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-20",
    "dateTo": "2026-03-26",
    "totalEvents": 85,
    "summary": {
      "totalOrders": 32,
      "deliveredOrders": 28,
      "failedOrders": 3,
      "cancelledOrders": 1,
      "totalCollected": 4850.00,
      "totalCommission": 485.00,
      "totalDistanceKm": 156.7
    },
    "dailyBreakdown": [
      {
        "date": "2026-03-20",
        "ordersCount": 5,
        "deliveredCount": 4,
        "collected": 620.00
      },
      {
        "date": "2026-03-21",
        "ordersCount": 6,
        "deliveredCount": 5,
        "collected": 780.50
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 9.3 Filtered Timeline

عرض الجدول الزمني مع فلترة حسب نوع الحدث.

```
GET /timeline/daily/filter
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Notes |
|-------|------|----------|-------|
| date | Date | **Yes** | التاريخ |
| eventTypes | string | **Yes** | أنواع الأحداث مفصولة بفاصلة (see [TimelineEventType](#timelineeventtype)) |

**Example**:
```
GET /timeline/daily/filter?date=2026-03-26&eventTypes=0,1,2
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-26",
    "filteredEventTypes": [0, 1, 2],
    "totalEvents": 5,
    "events": [
      {
        "id": "evt_001",
        "type": 0,
        "title": "بداية اليوم",
        "description": "تم تسجيل الدخول",
        "timestamp": "2026-03-26T08:00:00Z",
        "orderId": null,
        "orderNumber": null
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 10. Endpoints -- OCR (Under Development)

> **تحذير**: هذه الـ endpoints قيد التطوير وقد تتغير بشكل كبير.

### 10.1 Scan Invoice

مسح فاتورة واستخراج البيانات.

```
POST /ocr/scan-invoice
```

**Auth Required**: Yes (Bearer Token)

**Content-Type**: `multipart/form-data`

**Form Fields**:
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| file | File | **Yes** | صورة الفاتورة (JPEG, PNG — max 15MB) |
| language | string | No | `ar` أو `en` (default: `ar`) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "confidence": 0.87,
    "extractedData": {
      "customerName": "محمد أحمد",
      "customerPhone": "01098765432",
      "address": "15 شارع النحاس، مدينة نصر",
      "amount": 150.00,
      "items": [
        { "name": "قميص أزرق", "quantity": 2, "price": 75.00 }
      ]
    },
    "rawText": "فاتورة بيع\nالعميل: محمد أحمد\n..."
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الملف مطلوب` |
| 400 | `فشل تحليل الصورة — جودة الصورة ضعيفة` |

---

### 10.2 Scan to Order

مسح فاتورة وإنشاء طلب مباشرة.

```
POST /ocr/scan-to-order
```

**Auth Required**: Yes (Bearer Token)

**Content-Type**: `multipart/form-data`

**Form Fields**:
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| file | File | **Yes** | صورة الفاتورة |
| autoCreate | bool | No | إنشاء الطلب تلقائيًا (default: false) |
| paymentMethod | int | No | طريقة الدفع (default: 0) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "confidence": 0.87,
    "extractedOrder": {
      "customerName": "محمد أحمد",
      "customerPhone": "01098765432",
      "deliveryAddress": "15 شارع النحاس، مدينة نصر",
      "amount": 150.00,
      "itemCount": 2
    },
    "orderId": null,
    "orderCreated": false
  },
  "message": "تم استخراج بيانات الفاتورة — راجعها قبل الإنشاء",
  "errors": null
}
```

---

### 10.3 Scan Batch

مسح عدة فواتير دفعة واحدة.

```
POST /ocr/scan-batch
```

**Auth Required**: Yes (Bearer Token)

**Content-Type**: `multipart/form-data`

**Form Fields**:
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| files | File[] | **Yes** | مجموعة صور (max 10 صور) |
| language | string | No | `ar` أو `en` (default: `ar`) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalScanned": 3,
    "successCount": 2,
    "failedCount": 1,
    "results": [
      {
        "fileIndex": 0,
        "fileName": "invoice1.jpg",
        "success": true,
        "confidence": 0.92,
        "extractedData": {
          "customerName": "محمد أحمد",
          "amount": 150.00
        }
      },
      {
        "fileIndex": 1,
        "fileName": "invoice2.jpg",
        "success": true,
        "confidence": 0.78,
        "extractedData": {
          "customerName": "سارة علي",
          "amount": 320.00
        }
      },
      {
        "fileIndex": 2,
        "fileName": "invoice3.jpg",
        "success": false,
        "confidence": 0.15,
        "error": "فشل تحليل الصورة — جودة الصورة ضعيفة"
      }
    ]
  },
  "message": "تم مسح 2 من 3 فواتير بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `يجب رفع صورة واحدة على الأقل` |
| 400 | `الحد الأقصى 10 صور` |

---

## 11. Endpoints -- Admin Orders

> **ملاحظة**: جميع endpoints الأدمن تتطلب Bearer Token + دور `Admin`.

### 11.1 All Orders

عرض جميع الطلبات في النظام (لجميع السائقين).

```
GET /admin/orders
```

**Auth Required**: Yes (Admin role)

**Query Parameters**: نفس [OrderFilterDto](#orderfilterdt) + الإضافات التالية:

| Param | Type | Default | Notes |
|-------|------|---------|-------|
| driverId | Guid? | null | فلتر حسب السائق |
| includeDeleted | bool | false | تضمين الطلبات المحذوفة |
| sortBy | string | `createdAt` | الترتيب: `createdAt`, `amount`, `status` |
| sortDesc | bool | true | ترتيب تنازلي |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "customerName": "محمد أحمد",
        "amount": 150.00,
        "status": 3,
        "priority": 1,
        "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
        "driverName": "أحمد محمد",
        "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "partnerColor": "#FF5722",
        "createdAt": "2026-03-26T10:30:00Z"
      }
    ],
    "totalCount": 1250,
    "page": 1,
    "pageSize": 20,
    "totalPages": 63,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 11.2 Kanban Board

عرض الطلبات بتنسيق Kanban board (مجمّعة حسب الحالة).

```
GET /admin/orders/board
```

**Auth Required**: Yes (Admin role)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| date | Date? | today | فلتر حسب التاريخ |
| driverId | Guid? | null | فلتر حسب السائق |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "columns": [
      {
        "status": 0,
        "statusName": "Pending",
        "statusNameAr": "في الانتظار",
        "count": 12,
        "totalAmount": 1850.00,
        "orders": [
          {
            "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
            "orderNumber": "SK-20260326-0042",
            "customerName": "محمد أحمد",
            "amount": 150.00,
            "priority": 1,
            "driverName": null,
            "partnerColor": "#FF5722"
          }
        ]
      },
      {
        "status": 1,
        "statusName": "Accepted",
        "statusNameAr": "مقبول",
        "count": 8,
        "totalAmount": 2400.00,
        "orders": [ ... ]
      },
      {
        "status": 3,
        "statusName": "InTransit",
        "statusNameAr": "في الطريق",
        "count": 5,
        "totalAmount": 980.50,
        "orders": [ ... ]
      },
      {
        "status": 5,
        "statusName": "Delivered",
        "statusNameAr": "تم التسليم",
        "count": 25,
        "totalAmount": 5670.00,
        "orders": [ ... ]
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 11.3 Admin Create Order

إنشاء طلب من لوحة الأدمن (يمكن تعيين سائق مباشرة).

```
POST /admin/orders
```

**Auth Required**: Yes (Admin role)

**Request Body**: نفس [CreateOrderDto](#41-create-order) + الإضافات:
```json
{
  "customerName": "محمد أحمد",
  "customerPhone": "01098765432",
  "amount": 150.00,
  "paymentMethod": 0,
  "deliveryAddress": "15 شارع مصطفى النحاس، مدينة نصر",
  "assignToDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "overrideCommission": 10.00,
  "adminNotes": "طلب VIP — أولوية عالية"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| assignToDriverId | Guid? | No | تعيين لسائق مباشرة |
| overrideCommission | decimal? | No | عمولة مخصصة |
| adminNotes | string? | No | ملاحظات أدمن (لا تظهر للسائق) |

**Success Response** `201 Created`: نفس response الـ [Create Order](#41-create-order)

---

### 11.4 Assign to Driver

تعيين طلب لسائق محدد.

```
POST /admin/orders/{id}/assign
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "notifyDriver": true,
  "priority": 1
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| driverId | Guid | **Yes** | معرّف السائق |
| notifyDriver | bool | No | إرسال إشعار (default: true) |
| priority | int? | No | تعديل الأولوية |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverName": "أحمد محمد",
    "assignedAt": "2026-03-26T11:00:00Z"
  },
  "message": "تم تعيين الطلب للسائق بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الطلب معيّن لسائق آخر بالفعل` |
| 404 | `الطلب غير موجود` |
| 404 | `السائق غير موجود` |

---

### 11.5 Auto Distribute

توزيع الطلبات المعلقة تلقائيًا على السائقين المتاحين.

```
POST /admin/orders/auto-distribute
```

**Auth Required**: Yes (Admin role)

**Request Body**:
```json
{
  "orderIds": [
    "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "c8f2d3e4-5a6b-7c8d-9e0f-1a2b3c4d5e6f"
  ],
  "strategy": "NearestDriver",
  "maxOrdersPerDriver": 10,
  "notifyDrivers": true
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| orderIds | Guid[]? | No | طلبات محددة (null = كل المعلّقة) |
| strategy | string | No | `NearestDriver`, `LeastBusy`, `Balanced` (default: `NearestDriver`) |
| maxOrdersPerDriver | int | No | الحد الأقصى لكل سائق (default: 10) |
| notifyDrivers | bool | No | إرسال إشعارات (default: true) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "distributedCount": 8,
    "unassignedCount": 2,
    "assignments": [
      {
        "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
        "orderNumber": "SK-20260326-0042",
        "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "driverName": "أحمد محمد",
        "reason": "أقرب سائق — 2.3 كم"
      }
    ],
    "unassignedReasons": [
      {
        "orderId": "d9a3e4f5-6b7c-8d9e-0f1a-2b3c4d5e6f7a",
        "reason": "لا يوجد سائق متاح في المنطقة"
      }
    ]
  },
  "message": "تم توزيع 8 طلبات بنجاح",
  "errors": null
}
```

---

### 11.6 Order Timeline

عرض السجل الزمني الكامل لطلب (كل التغييرات والأحداث).

```
GET /admin/orders/{id}/timeline
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (GUID) - معرّف الطلب

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "events": [
      {
        "type": "StatusChange",
        "from": null,
        "to": "Pending",
        "timestamp": "2026-03-26T10:30:00Z",
        "actor": "أحمد محمد (سائق)",
        "notes": null
      },
      {
        "type": "Assignment",
        "from": null,
        "to": "أحمد محمد",
        "timestamp": "2026-03-26T10:30:00Z",
        "actor": "System",
        "notes": "تعيين تلقائي"
      },
      {
        "type": "StatusChange",
        "from": "Pending",
        "to": "Accepted",
        "timestamp": "2026-03-26T10:45:00Z",
        "actor": "أحمد محمد (سائق)",
        "notes": null
      },
      {
        "type": "LocationUpdate",
        "from": null,
        "to": "30.0500, 31.3200",
        "timestamp": "2026-03-26T13:30:00Z",
        "actor": "System",
        "notes": "GPS update"
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

### 11.7 Override Status

تغيير حالة الطلب بصلاحيات أدمن (تجاوز قيود الحالة).

```
PUT /admin/orders/{id}/override-status
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "newStatus": 5,
  "reason": "العميل أكّد الاستلام عبر الهاتف",
  "adjustAmount": null
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| newStatus | int | **Yes** | الحالة الجديدة |
| reason | string | **Yes** | سبب التجاوز (مطلوب للتوثيق) |
| adjustAmount | decimal? | No | تعديل المبلغ |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "orderNumber": "SK-20260326-0042",
    "previousStatus": 3,
    "newStatus": 5,
    "overriddenBy": "admin@sekka.com",
    "overriddenAt": "2026-03-26T15:00:00Z"
  },
  "message": "تم تغيير حالة الطلب بصلاحيات أدمن",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `سبب التجاوز مطلوب` |
| 404 | `الطلب غير موجود` |

---

### 11.8 Export Orders

تصدير الطلبات (Excel/CSV).

```
GET /admin/orders/export
```

**Auth Required**: Yes (Admin role)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| format | string | `excel` | `excel` أو `csv` |
| dateFrom | Date? | null | من تاريخ |
| dateTo | Date? | null | إلى تاريخ |
| status | int? | null | فلتر حسب الحالة |
| driverId | Guid? | null | فلتر حسب السائق |

**Example**:
```
GET /admin/orders/export?format=excel&dateFrom=2026-03-01&dateTo=2026-03-26
```

**Success Response** `200 OK`:
```
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
Content-Disposition: attachment; filename="orders_2026-03-26.xlsx"
```

> **Note**: الاستجابة هي ملف مباشر (binary) وليس JSON.

---

### 11.9 Auto-Assign with Config

تعيين طلب تلقائيًا مع إعدادات مخصصة.

```
POST /admin/orders/{id}/auto-assign
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (GUID) - معرّف الطلب

**Request Body**:
```json
{
  "maxDistanceKm": 15.0,
  "preferredVehicleType": 0,
  "excludeDriverIds": ["d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a"],
  "prioritizeByRating": true,
  "notifyDriver": true
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| maxDistanceKm | double | No | الحد الأقصى للمسافة (default: 20) |
| preferredVehicleType | int? | No | نوع المركبة المفضل |
| excludeDriverIds | Guid[]? | No | سائقين مستبعدين |
| prioritizeByRating | bool | No | الأولوية بالتقييم (default: false) |
| notifyDriver | bool | No | إرسال إشعار (default: true) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "orderId": "b7e1c2d3-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    "assignedDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "assignedDriverName": "أحمد محمد",
    "driverDistanceKm": 3.2,
    "driverRating": 4.8,
    "assignedAt": "2026-03-26T11:05:00Z"
  },
  "message": "تم التعيين التلقائي بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يوجد سائق متاح يطابق الشروط` |
| 404 | `الطلب غير موجود` |

---

### 11.10 Suggested Drivers

عرض قائمة السائقين المقترحين لطلب.

```
GET /admin/orders/{id}/suggested-drivers
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (GUID) - معرّف الطلب

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverName": "أحمد محمد",
      "phone": "+201012345678",
      "distanceKm": 3.2,
      "currentOrdersCount": 2,
      "rating": 4.8,
      "vehicleType": 0,
      "isOnline": true,
      "score": 92.5,
      "scoreBreakdown": {
        "distanceScore": 40,
        "ratingScore": 30,
        "availabilityScore": 22.5
      }
    },
    {
      "driverId": "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
      "driverName": "عمر حسن",
      "phone": "+201198765432",
      "distanceKm": 5.8,
      "currentOrdersCount": 1,
      "rating": 4.5,
      "vehicleType": 1,
      "isOnline": true,
      "score": 85.0,
      "scoreBreakdown": {
        "distanceScore": 30,
        "ratingScore": 25,
        "availabilityScore": 30
      }
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

## 12. Endpoints -- Admin Time Slots

### 12.1 Get Time Slots

عرض الفترات الزمنية المتاحة.

```
GET /admin/time-slots
```

**Auth Required**: Yes (Admin role)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| date | Date? | null | فلتر حسب التاريخ |
| isActive | bool? | null | فلتر حسب النشاط |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "ts_001",
      "name": "صباحي",
      "startTime": "08:00:00",
      "endTime": "12:00:00",
      "maxOrders": 50,
      "currentOrders": 32,
      "availableSlots": 18,
      "isActive": true,
      "dayOfWeek": null,
      "date": null
    },
    {
      "id": "ts_002",
      "name": "مسائي",
      "startTime": "14:00:00",
      "endTime": "20:00:00",
      "maxOrders": 80,
      "currentOrders": 45,
      "availableSlots": 35,
      "isActive": true,
      "dayOfWeek": null,
      "date": null
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 12.2 Create Time Slot

إنشاء فترة زمنية جديدة.

```
POST /admin/time-slots
```

**Auth Required**: Yes (Admin role)

**Request Body**:
```json
{
  "name": "فترة ليلية",
  "startTime": "20:00:00",
  "endTime": "23:59:00",
  "maxOrders": 30,
  "isActive": true,
  "dayOfWeek": null,
  "date": null
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| name | string | **Yes** | اسم الفترة |
| startTime | Time | **Yes** | وقت البداية |
| endTime | Time | **Yes** | وقت النهاية |
| maxOrders | int | **Yes** | الحد الأقصى للطلبات |
| isActive | bool | No | نشطة (default: true) |
| dayOfWeek | int? | No | يوم محدد (0=Sunday) |
| date | Date? | No | تاريخ محدد |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "ts_003",
    "name": "فترة ليلية",
    "startTime": "20:00:00",
    "endTime": "23:59:00",
    "maxOrders": 30,
    "isActive": true
  },
  "message": "تم إنشاء الفترة الزمنية بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `وقت البداية يجب أن يكون قبل وقت النهاية` |
| 409 | `يوجد تداخل مع فترة زمنية أخرى` |

---

### 12.3 Update Time Slot

تعديل فترة زمنية.

```
PUT /admin/time-slots/{id}
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (string) - معرّف الفترة

**Request Body**: نفس body الخاص بـ [Create Time Slot](#122-create-time-slot)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الفترة الزمنية بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الفترة الزمنية غير موجودة` |
| 409 | `يوجد تداخل مع فترة زمنية أخرى` |

---

### 12.4 Delete Time Slot

حذف فترة زمنية.

```
DELETE /admin/time-slots/{id}
```

**Auth Required**: Yes (Admin role)

**URL Params**: `id` (string) - معرّف الفترة

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الفترة الزمنية",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن حذف فترة بها طلبات نشطة` |
| 404 | `الفترة الزمنية غير موجودة` |

---

### 12.5 Generate Week Slots

إنشاء فترات زمنية لأسبوع كامل تلقائيًا.

```
POST /admin/time-slots/generate-week
```

**Auth Required**: Yes (Admin role)

**Request Body**:
```json
{
  "startDate": "2026-03-30",
  "templateSlots": [
    { "name": "صباحي", "startTime": "08:00:00", "endTime": "12:00:00", "maxOrders": 50 },
    { "name": "مسائي", "startTime": "14:00:00", "endTime": "20:00:00", "maxOrders": 80 }
  ],
  "skipFriday": true
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| startDate | Date | **Yes** | بداية الأسبوع |
| templateSlots | array | **Yes** | قوالب الفترات |
| skipFriday | bool | No | تخطي يوم الجمعة (default: false) |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "generatedCount": 12,
    "startDate": "2026-03-30",
    "endDate": "2026-04-04",
    "skippedDays": ["2026-04-03"]
  },
  "message": "تم إنشاء 12 فترة زمنية للأسبوع",
  "errors": null
}
```

---

### 12.6 Slot Statistics

عرض إحصائيات الفترات الزمنية.

```
GET /admin/time-slots/stats
```

**Auth Required**: Yes (Admin role)

**Query Parameters**:
| Param | Type | Default | Notes |
|-------|------|---------|-------|
| dateFrom | Date? | null | من تاريخ |
| dateTo | Date? | null | إلى تاريخ |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalSlots": 24,
    "activeSlots": 20,
    "totalCapacity": 1440,
    "totalBooked": 876,
    "utilizationRate": 0.608,
    "busiestSlot": {
      "id": "ts_002",
      "name": "مسائي",
      "utilizationRate": 0.85
    },
    "leastBusySlot": {
      "id": "ts_003",
      "name": "فترة ليلية",
      "utilizationRate": 0.20
    }
  },
  "message": null,
  "errors": null
}
```

---

## 13. Enums

### OrderStatus
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | Pending | في الانتظار | تم إنشاء الطلب |
| 1 | Accepted | مقبول | السائق قبل الطلب |
| 2 | PickedUp | تم الاستلام | تم استلام الشحنة من المرسل |
| 3 | InTransit | في الطريق | السائق في طريقه للعميل |
| 4 | ArrivedAtDestination | وصل للوجهة | السائق وصل لعنوان التسليم |
| 5 | Delivered | تم التسليم | تم التسليم بنجاح |
| 6 | Failed | فشل التسليم | فشلت محاولة التوصيل |
| 7 | Cancelled | ملغي | تم إلغاء الطلب |
| 8 | PartiallyDelivered | تسليم جزئي | تم تسليم جزء من الطلب |
| 9 | RetryPending | إعادة محاولة | في انتظار إعادة المحاولة |
| 10 | Returned | مرتجع | تم إرجاع الشحنة |

### PaymentMethod
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Cash | كاش |
| 1 | Visa | فيزا |
| 2 | Wallet | محفظة إلكترونية |
| 3 | PartnerCredit | حساب الشريك |

### OrderPriority
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Normal | عادي |
| 1 | Urgent | عاجل |
| 2 | VIP | مميز |

### DeliveryFailReason
| Value | Name | Arabic |
|-------|------|--------|
| 0 | CustomerNotAvailable | العميل غير متواجد |
| 1 | CustomerRefused | العميل رفض الاستلام |
| 2 | WrongAddress | عنوان خاطئ |
| 3 | PhoneUnreachable | الهاتف مغلق/لا يرد |
| 4 | AccessDenied | لا يمكن الوصول للمكان |
| 5 | DamagedPackage | الشحنة تالفة |
| 6 | InsufficientPayment | المبلغ غير كافي |
| 7 | SecurityIssue | مشكلة أمنية |
| 8 | WeatherConditions | ظروف جوية |
| 9 | Other | أخرى |

### CancellationReason
| Value | Name | Arabic |
|-------|------|--------|
| 0 | CustomerRequest | بطلب من العميل |
| 1 | DriverRequest | بطلب من السائق |
| 2 | PartnerRequest | بطلب من الشريك |
| 3 | DuplicateOrder | طلب مكرر |
| 4 | FraudSuspicion | اشتباه احتيال |
| 5 | SystemError | خطأ في النظام |
| 6 | OutOfServiceArea | خارج نطاق الخدمة |
| 7 | Other | أخرى |

### PhotoType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | ProofOfDelivery | إثبات تسليم |
| 1 | PackagePhoto | صورة الشحنة |
| 2 | DamagePhoto | صورة التلف |
| 3 | InvoicePhoto | صورة الفاتورة |
| 4 | SignaturePhoto | التوقيع |
| 5 | LocationPhoto | صورة الموقع |

### TransferStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | في الانتظار |
| 1 | Accepted | مقبول |
| 2 | Rejected | مرفوض |
| 3 | Completed | مكتمل |
| 4 | Cancelled | ملغي |

### TimelineEventType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | DayStart | بداية اليوم |
| 1 | OrderCreated | طلب جديد |
| 2 | StatusChange | تغيير حالة |
| 3 | Delivery | تسليم |
| 4 | FailedAttempt | محاولة فاشلة |
| 5 | Cancellation | إلغاء |
| 6 | Transfer | تحويل |
| 7 | LocationUpdate | تحديث موقع |
| 8 | DayEnd | نهاية اليوم |

---

## 14. DTOs Reference

### CreateOrderDto (Request)
```typescript
interface CreateOrderDto {
  customerName?: string;       // اسم العميل
  customerPhone?: string;      // رقم العميل
  partnerId?: string;          // GUID — معرّف الشريك
  description?: string;        // وصف الشحنة
  amount: number;              // decimal — المبلغ (مطلوب)
  paymentMethod: number;       // enum — طريقة الدفع (مطلوب)
  priority?: number;           // enum — الأولوية (default: 0)
  pickupAddress?: string;      // عنوان الاستلام
  pickupLatitude?: number;     // double
  pickupLongitude?: number;    // double
  deliveryAddress: string;     // عنوان التسليم (مطلوب)
  deliveryLatitude?: number;   // double
  deliveryLongitude?: number;  // double
  notes?: string;              // ملاحظات
  itemCount?: number;          // int (default: 1)
  timeWindowStart?: string;    // DateTime ISO 8601
  timeWindowEnd?: string;      // DateTime ISO 8601
  scheduledDate?: string;      // Date yyyy-MM-dd
  isRecurring?: boolean;       // (default: false)
  recurrencePattern?: string;  // Daily/Weekly/Monthly
  expectedChangeAmount?: number; // decimal — مبلغ الفكة
  idempotencyKey?: string;     // مفتاح منع التكرار
}
```

### OrderDto (Response)
```typescript
interface OrderDto {
  id: string;                  // GUID
  orderNumber: string;         // e.g. "SK-20260326-0042"
  customerName?: string;
  customerPhone?: string;
  partnerName?: string;
  partnerColor?: string;       // Hex color e.g. "#FF5722"
  description?: string;
  amount: number;              // decimal
  commissionAmount: number;    // decimal
  paymentMethod: number;       // enum
  status: number;              // enum
  priority: number;            // enum
  deliveryAddress: string;
  distanceKm?: number;         // double
  sequenceIndex?: number;      // int — ترتيب في المسار
  worthScore?: number;         // double — نقاط القيمة
  createdAt: string;           // DateTime ISO 8601
  deliveredAt?: string;        // DateTime ISO 8601
}
```

### OrderListDto (Response — Compact)
```typescript
interface OrderListDto {
  id: string;                  // GUID
  orderNumber: string;
  customerName?: string;
  amount: number;              // decimal
  status: number;              // enum
  priority: number;            // enum
  deliveryAddress: string;
  partnerColor?: string;
  sequenceIndex?: number;      // int
  createdAt: string;           // DateTime ISO 8601
}
```

### OrderFilterDto (Query)
```typescript
interface OrderFilterDto {
  status?: number;             // enum
  partnerId?: string;          // GUID
  dateFrom?: string;           // DateTime
  dateTo?: string;             // DateTime
  searchTerm?: string;         // بحث نصي
  paymentMethod?: number;      // enum
  priority?: number;           // enum
  page?: number;               // int (default: 1)
  pageSize?: number;           // int (default: 20)
}
```

### DeliverOrderDto (Request)
```typescript
interface DeliverOrderDto {
  actualCollectedAmount?: number; // decimal
  latitude?: number;              // double
  longitude?: number;             // double
  notes?: string;
  ratingValue?: number;           // int (1-5)
}
```

### FailOrderDto (Request)
```typescript
interface FailOrderDto {
  reason: number;              // enum DeliveryFailReason (مطلوب)
  reasonText?: string;
  latitude?: number;           // double
  longitude?: number;          // double
  sendAutoMessage?: boolean;   // (default: true)
}
```

### CancelOrderDto (Request)
```typescript
interface CancelOrderDto {
  cancellationReason: number;  // enum CancellationReason (مطلوب)
  reasonText?: string;
  lossAmount?: number;         // decimal (default: 0)
  distanceTravelledKm?: number; // double
  fuelCostLost?: number;       // decimal
  transferToDriverId?: string; // GUID
}
```

---

## 15. Flutter/Dio Integration

### Dio Setup (مع Token Interceptor)

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
  connectTimeout: const Duration(seconds: 30),
  receiveTimeout: const Duration(seconds: 30),
));

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
    } else {
      handler.next(error);
    }
  },
));
```

### Create Order
```dart
Future<OrderDto?> createOrder({
  required String deliveryAddress,
  required double amount,
  required int paymentMethod,
  String? customerName,
  String? customerPhone,
  String? notes,
  int priority = 0,
  String? idempotencyKey,
}) async {
  try {
    final response = await dio.post('/orders', data: {
      'deliveryAddress': deliveryAddress,
      'amount': amount,
      'paymentMethod': paymentMethod,
      if (customerName != null) 'customerName': customerName,
      if (customerPhone != null) 'customerPhone': customerPhone,
      if (notes != null) 'notes': notes,
      'priority': priority,
      if (idempotencyKey != null) 'idempotencyKey': idempotencyKey,
    });

    if (response.data['isSuccess'] == true) {
      return OrderDto.fromJson(response.data['data']);
    }
    showError(response.data['message']);
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### List Orders with Filtering
```dart
Future<PagedResult<OrderListDto>?> getOrders({
  int? status,
  int page = 1,
  int pageSize = 20,
  String? searchTerm,
  DateTime? dateFrom,
  DateTime? dateTo,
}) async {
  try {
    final response = await dio.get('/orders', queryParameters: {
      'page': page,
      'pageSize': pageSize,
      if (status != null) 'status': status,
      if (searchTerm != null) 'searchTerm': searchTerm,
      if (dateFrom != null) 'dateFrom': dateFrom.toIso8601String(),
      if (dateTo != null) 'dateTo': dateTo.toIso8601String(),
    });

    if (response.data['isSuccess'] == true) {
      return PagedResult<OrderListDto>.fromJson(
        response.data['data'],
        (json) => OrderListDto.fromJson(json),
      );
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Mark as Delivered
```dart
Future<bool> markAsDelivered(
  String orderId, {
  double? actualCollectedAmount,
  double? latitude,
  double? longitude,
  int? ratingValue,
}) async {
  try {
    final response = await dio.post('/orders/$orderId/deliver', data: {
      if (actualCollectedAmount != null)
        'actualCollectedAmount': actualCollectedAmount,
      if (latitude != null) 'latitude': latitude,
      if (longitude != null) 'longitude': longitude,
      if (ratingValue != null) 'ratingValue': ratingValue,
    });
    return response.data['isSuccess'] == true;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return false;
  }
}
```

### Upload Photo (multipart)
```dart
Future<bool> uploadOrderPhoto(
  String orderId,
  File imageFile,
  int photoType, {
  String? description,
}) async {
  try {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.path.split('/').last,
      ),
      'photoType': photoType,
      if (description != null) 'description': description,
    });

    final response = await dio.post(
      '/orders/$orderId/photos',
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );
    return response.data['isSuccess'] == true;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل رفع الصورة');
    return false;
  }
}
```

### Offline Sync Flow
```dart
/// مزامنة التغييرات المحلية مع السيرفر
Future<SyncResult?> syncOfflineChanges() async {
  final pendingChanges = await localDb.getPendingChanges();
  if (pendingChanges.isEmpty) return null;

  try {
    final response = await dio.post('/sync/push', data: {
      'changes': pendingChanges.map((c) => c.toJson()).toList(),
      'deviceId': await getDeviceId(),
      'lastSyncTimestamp': await getLastSyncTimestamp(),
    });

    if (response.data['isSuccess'] == true) {
      final result = SyncResult.fromJson(response.data['data']);

      // حفظ الـ server IDs محليًا
      for (final r in result.results) {
        await localDb.updateServerId(r.localId, r.serverId);
      }

      // معالجة التعارضات
      if (result.conflicts.isNotEmpty) {
        await handleConflicts(result.conflicts);
      }

      await setLastSyncTimestamp(result.serverTimestamp);
      return result;
    }
    return null;
  } on DioException catch (e) {
    // حفظ التغييرات محليًا وإعادة المحاولة لاحقًا
    debugPrint('Sync failed: ${e.message}');
    return null;
  }
}

/// جلب التحديثات من السيرفر
Future<void> pullServerUpdates() async {
  final lastSync = await getLastSyncTimestamp();

  try {
    final response = await dio.get('/sync/pull', queryParameters: {
      'since': lastSync,
      'entityTypes': 'Order,Route',
    });

    if (response.data['isSuccess'] == true) {
      final data = response.data['data'];

      // تطبيق التحديثات محليًا
      for (final update in data['updates']) {
        await localDb.applyServerUpdate(update);
      }

      // حذف الكيانات المحذوفة
      for (final deletion in data['deletions']) {
        await localDb.markAsDeleted(deletion['entityId']);
      }

      await setLastSyncTimestamp(data['serverTimestamp']);
    }
  } on DioException catch (_) {
    // التطبيق يعمل من البيانات المحلية
  }
}
```

### Route Optimization
```dart
Future<RouteOptimizationResult?> optimizeRoute(
  List<String> orderIds,
  double startLat,
  double startLng,
) async {
  try {
    final response = await dio.post('/routes/optimize', data: {
      'orderIds': orderIds,
      'startLatitude': startLat,
      'startLongitude': startLng,
      'optimizeFor': 'distance',
    });

    if (response.data['isSuccess'] == true) {
      return RouteOptimizationResult.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل تحسين المسار');
    return null;
  }
}
```

### Public Tracking (No Auth)
```dart
/// يمكن استخدامه بدون تسجيل دخول
Future<TrackingInfo?> trackOrder(String trackingCode) async {
  try {
    // لاحظ: بدون Bearer Token
    final publicDio = Dio(BaseOptions(
      baseUrl: 'https://sekka.runasp.net/api/v1',
    ));

    final response = await publicDio.get('/tracking/$trackingCode');

    if (response.data['isSuccess'] == true) {
      return TrackingInfo.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    if (e.response?.statusCode == 404) {
      showError('كود التتبع غير صالح');
    }
    return null;
  }
}
```

---

> **Built for the Sekka Flutter team** -- For questions, check the [Auth API docs](AUTH_API.md) for authentication setup, or contact the backend team.
