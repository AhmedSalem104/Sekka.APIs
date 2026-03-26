# Sekka API - Customers & Partners Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-26

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Enums](#3-enums)
4. [DTOs Reference](#4-dtos-reference)
5. [Rating System](#5-rating-system)
6. [Customer Lifecycle](#6-customer-lifecycle)
7. [Partner Verification Flow](#7-partner-verification-flow)
8. [CustomerController](#8-customercontroller)
   - [List Customers](#81-list-customers)
   - [Get Customer Details](#82-get-customer-details)
   - [Find Customer by Phone](#83-find-customer-by-phone)
   - [Update Customer](#84-update-customer)
   - [Rate Customer](#85-rate-customer)
   - [Block Customer](#86-block-customer)
   - [Unblock Customer](#87-unblock-customer)
   - [Get Customer Orders](#88-get-customer-orders)
   - [Upload Voice Memo](#89-upload-voice-memo)
   - [Get Customer Interests](#810-get-customer-interests)
   - [Get Customer Engagement](#811-get-customer-engagement)
9. [CallerIdController](#9-calleridcontroller)
   - [Lookup Caller Identity](#91-lookup-caller-identity)
   - [Create Caller ID Note](#92-create-caller-id-note)
   - [Update Caller ID Note](#93-update-caller-id-note)
   - [Delete Caller ID Note](#94-delete-caller-id-note)
   - [Truecaller Lookup](#95-truecaller-lookup)
10. [AddressController](#10-addresscontroller)
    - [Search Addresses](#101-search-addresses)
    - [Save New Address](#102-save-new-address)
    - [Update Address](#103-update-address)
    - [Delete Address](#104-delete-address)
    - [Autocomplete](#105-autocomplete)
    - [Nearby Addresses](#106-nearby-addresses)
11. [PartnerController](#11-partnercontroller)
    - [List Partners](#111-list-partners)
    - [Create Partner](#112-create-partner)
    - [Update Partner](#113-update-partner)
    - [Delete Partner](#114-delete-partner)
    - [Get Partner Orders](#115-get-partner-orders)
    - [Get Partner Pickup Points](#116-get-partner-pickup-points)
    - [Submit Verification Document](#117-submit-verification-document)
    - [Get Verification Status](#118-get-verification-status)
12. [PickupPointController](#12-pickuppointcontroller)
    - [Create Pickup Point](#121-create-pickup-point)
    - [Update Pickup Point](#122-update-pickup-point)
    - [Delete Pickup Point](#123-delete-pickup-point)
    - [Rate Pickup Point](#124-rate-pickup-point)
13. [PartnerPortalController](#13-partnerportalcontroller)
    - [Partner Dashboard](#131-partner-dashboard)
    - [Partner Orders](#132-partner-orders)
    - [Partner Settlements](#133-partner-settlements)
    - [Update Partner Settings](#134-update-partner-settings)
    - [Partner Statistics](#135-partner-statistics)
    - [Partner Invoices](#136-partner-invoices)
14. [AdminCustomersController](#14-admincustomerscontroller)
    - [List All Customers](#141-list-all-customers)
    - [Get Customer Details (Admin)](#142-get-customer-details-admin)
    - [Block Customer (Admin)](#143-block-customer-admin)
    - [Unblock Customer (Admin)](#144-unblock-customer-admin)
    - [Customer Reports](#145-customer-reports)
15. [AdminPartnersController](#15-adminpartnerscontroller)
    - [List All Partners](#151-list-all-partners)
    - [Create Partner (Admin)](#152-create-partner-admin)
    - [Update Partner (Admin)](#153-update-partner-admin)
    - [Delete Partner (Admin)](#154-delete-partner-admin)
    - [Partner Analytics](#155-partner-analytics)
    - [Pending Verification List](#156-pending-verification-list)
    - [Verify/Reject Partner](#157-verifyreject-partner)
    - [Request Additional Document](#158-request-additional-document)
16. [AdminBlacklistController](#16-adminblacklistcontroller)
    - [Blacklist Entries](#161-blacklist-entries)
    - [Verify Community Report](#162-verify-community-report)
    - [Remove from Blacklist](#163-remove-from-blacklist)
    - [New Reports](#164-new-reports)
17. [Flutter/Dio Integration Examples](#17-flutterdio-integration-examples)

---

## 1. Overview

The Customers & Partners module manages the lifecycle of customers (recipients of deliveries), partners (businesses that send deliveries), addresses, caller identification, pickup points, and admin-level management for both.

| Feature | Detail |
|---------|--------|
| Auth Method | Bearer Token (JWT) |
| All endpoints | Require authentication unless noted |
| Admin endpoints | Require `Admin` role |
| Phone Format | Egyptian mobile (010/011/012/015) |
| Rating Scale | 1-5 stars with 8 boolean quick-tags |
| Pagination | `pageNumber` (default 1), `pageSize` (default 10) |

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

### Paged Response
```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "totalCount": 57,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 6,
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
| 204 | Deleted successfully |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (insufficient role) |
| 404 | Not Found |
| 409 | Conflict (duplicate entry) |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Enums

### AddressType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Home | منزل |
| 1 | Work | عمل |
| 2 | Shop | محل |
| 3 | Restaurant | مطعم |
| 4 | Warehouse | مخزن |
| 5 | Other | أخرى |

### ContactType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Customer | عميل |
| 1 | Partner | شريك |
| 2 | Driver | سائق |
| 3 | Other | أخرى |

### PartnerType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Restaurant | مطعم |
| 1 | Shop | محل |
| 2 | Pharmacy | صيدلية |
| 3 | Supermarket | سوبرماركت |
| 4 | Warehouse | مخزن |
| 5 | ECommerce | تجارة إلكترونية |
| 6 | Other | أخرى |

### CommissionType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | FixedPerOrder | مبلغ ثابت لكل طلب |
| 1 | PercentagePerOrder | نسبة من كل طلب |
| 2 | MonthlyFlat | اشتراك شهري |

### VerificationStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد المراجعة |
| 1 | Verified | موثّق |
| 2 | Rejected | مرفوض |
| 3 | DocumentRequested | مطلوب مستند إضافي |

### PaymentMethod
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Cash | كاش |
| 1 | Wallet | محفظة |
| 2 | Card | بطاقة |
| 3 | InstaPay | إنستاباي |

---

## 4. DTOs Reference

### CustomerDto
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "phone": "+201012345678",
  "name": "محمد أحمد علي",
  "averageRating": 4.3,
  "totalDeliveries": 47,
  "successfulDeliveries": 44,
  "isBlocked": false,
  "lastDeliveryDate": "2026-03-20T14:30:00Z"
}
```

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | guid | No | Customer unique ID |
| phone | string | No | Normalized Egyptian phone (+20...) |
| name | string | Yes | Customer display name |
| averageRating | double | No | Average of all ratings (1-5) |
| totalDeliveries | int | No | Total delivery attempts |
| successfulDeliveries | int | No | Successfully completed deliveries |
| isBlocked | bool | No | Whether customer is blocked |
| lastDeliveryDate | datetime | Yes | Last delivery timestamp |

### CreateRatingDto
```json
{
  "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
  "ratingValue": 4,
  "quickResponse": true,
  "clearAddress": true,
  "respectfulBehavior": true,
  "easyPayment": false,
  "wrongAddress": false,
  "noAnswer": false,
  "delayedPickup": false,
  "paymentIssue": false,
  "feedbackText": "عميل ممتاز، بيرد بسرعة ومكانه واضح"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| orderId | guid | No | Related order (if rating after delivery) |
| ratingValue | int | Yes | 1-5 star rating |
| quickResponse | bool | No | Positive: responds quickly |
| clearAddress | bool | No | Positive: address is clear |
| respectfulBehavior | bool | No | Positive: respectful behavior |
| easyPayment | bool | No | Positive: payment went smoothly |
| wrongAddress | bool | No | Negative: gave wrong address |
| noAnswer | bool | No | Negative: did not answer calls |
| delayedPickup | bool | No | Negative: delayed at pickup |
| paymentIssue | bool | No | Negative: payment problems |
| feedbackText | string | No | Free-text feedback |

### PartnerDto
```json
{
  "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
  "name": "مطعم الشيف",
  "partnerType": 0,
  "phone": "+201112223344",
  "address": "15 شارع التحرير، الدقي، الجيزة",
  "commissionType": 1,
  "commissionValue": 12.5,
  "color": "#FF5722",
  "logoUrl": "https://sekka.runasp.net/uploads/partners/logo_chef.png",
  "isActive": true,
  "verificationStatus": 1
}
```

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | guid | No | Partner unique ID |
| name | string | No | Partner business name |
| partnerType | int | No | See [PartnerType](#partnertype) enum |
| phone | string | Yes | Contact phone |
| address | string | Yes | Business address |
| commissionType | int | No | See [CommissionType](#commissiontype) enum |
| commissionValue | decimal | No | Commission amount or percentage |
| color | string | No | Brand color (hex) |
| logoUrl | string | Yes | Logo image URL |
| isActive | bool | No | Whether partner is active |
| verificationStatus | int | No | See [VerificationStatus](#verificationstatus) enum |

### CreatePartnerDto
```json
{
  "name": "صيدلية الحياة",
  "partnerType": 2,
  "phone": "01098765432",
  "address": "8 شارع مصطفى النحاس، مدينة نصر، القاهرة",
  "commissionType": 0,
  "commissionValue": 7.00,
  "defaultPaymentMethod": 0,
  "color": "#4CAF50",
  "receiptHeader": "صيدلية الحياة - نخدمك بإخلاص"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Business name |
| partnerType | int | Yes | See [PartnerType](#partnertype) enum |
| phone | string | No | Contact phone |
| address | string | No | Business address |
| commissionType | int | Yes | See [CommissionType](#commissiontype) enum |
| commissionValue | decimal | Yes | Commission amount/percentage |
| defaultPaymentMethod | int | Yes | See [PaymentMethod](#paymentmethod) enum |
| color | string | No | Brand color (hex), defaults to system color |
| receiptHeader | string | No | Custom receipt header text |

### CallerIdDto
```json
{
  "phoneNumber": "+201012345678",
  "displayName": "محمد أحمد",
  "contactType": 0,
  "customerName": "محمد أحمد علي",
  "partnerName": null,
  "lastOrderDate": "2026-03-18T10:15:00Z",
  "averageRating": 4.5,
  "note": "بيفضل التوصيل الدور الخامس - عمارة بدون أسانسير",
  "isBlocked": false
}
```

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| phoneNumber | string | No | Normalized phone number |
| displayName | string | Yes | Saved display name |
| contactType | int | No | See [ContactType](#contacttype) enum |
| customerName | string | Yes | Name from customer record |
| partnerName | string | Yes | Name from partner record |
| lastOrderDate | datetime | Yes | Last order associated with this number |
| averageRating | double | Yes | Average rating if customer |
| note | string | Yes | Driver's personal note |
| isBlocked | bool | No | Whether this contact is blocked |

### AddressDto
```json
{
  "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
  "addressText": "12 شارع الأزهر، الحسين، القاهرة",
  "latitude": 30.0459,
  "longitude": 31.2625,
  "addressType": 0,
  "visitCount": 5,
  "landmarks": "بجوار مسجد الحسين - فوق صيدلية الأزهر",
  "deliveryNotes": "الدور التالت شقة 7 - الجرس بايظ، كلمني لما توصل"
}
```

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | guid | No | Address unique ID |
| addressText | string | No | Full address text |
| latitude | double | Yes | GPS latitude |
| longitude | double | Yes | GPS longitude |
| addressType | int | No | See [AddressType](#addresstype) enum |
| visitCount | int | No | Number of deliveries to this address |
| landmarks | string | Yes | Nearby landmarks for navigation |
| deliveryNotes | string | Yes | Special delivery instructions |

---

## 5. Rating System

Drivers can rate customers after each delivery using a 1-5 star scale plus 8 boolean quick-tags (4 positive, 4 negative).

### Positive Tags
| Tag | Arabic | Description |
|-----|--------|-------------|
| quickResponse | رد سريع | Customer answers calls/messages quickly |
| clearAddress | عنوان واضح | Address is easy to find |
| respectfulBehavior | تعامل محترم | Customer is polite and respectful |
| easyPayment | دفع سهل | Payment was smooth (exact change, ready) |

### Negative Tags
| Tag | Arabic | Description |
|-----|--------|-------------|
| wrongAddress | عنوان غلط | Customer gave incorrect address |
| noAnswer | مبيردش | Customer did not answer calls |
| delayedPickup | تأخير في الاستلام | Customer was late to receive delivery |
| paymentIssue | مشكلة في الدفع | Payment dispute, no change, refused to pay |

### How Rating Works
```
1. Driver delivers an order to a customer
2. After delivery, driver rates the customer (1-5 stars)
3. Driver optionally selects quick-tags (boolean flags)
4. Driver optionally writes free-text feedback
5. System updates customer's averageRating
6. Tags are aggregated for customer profile insights
7. Low-rated customers may be flagged for review
```

---

## 6. Customer Lifecycle

```
                    ┌──────────────────┐
                    │  New Customer    │
                    │  (First Order)   │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │   Active         │
                    │   (Ordering)     │
                    └──┬────────────┬──┘
                       │            │
              ┌────────▼──┐   ┌────▼───────────┐
              │  Rated by  │   │  No activity   │
              │  Drivers   │   │  (30+ days)    │
              └────────┬──┘   └────┬───────────┘
                       │           │
              ┌────────▼──┐   ┌────▼───────────┐
              │ Good       │   │   Dormant      │
              │ (4-5 avg)  │   │   Customer     │
              └────────────┘   └────────────────┘
                       │
                       │  Multiple negative ratings
                       │  or driver reports
                       ▼
              ┌──────────────┐
              │   Blocked    │────► Admin reviews
              │   Customer   │      and may unblock
              └──────────────┘
                       │
                       │  reportToCommunity = true
                       ▼
              ┌──────────────┐
              │  Blacklisted │────► Visible to all
              │  (Community) │      drivers as warning
              └──────────────┘
```

---

## 7. Partner Verification Flow

```
Step 1: Partner is created (verificationStatus = Pending)
Step 2: Partner uploads verification document (ID, commercial register, etc.)
Step 3: Admin reviews pending verifications
Step 4: Admin either:
        a) Verifies   → verificationStatus = Verified, partner is fully active
        b) Rejects    → verificationStatus = Rejected, partner notified
        c) Requests   → verificationStatus = DocumentRequested, partner must upload more
Step 5: If DocumentRequested, partner uploads additional document → back to Step 3
```

```
  ┌──────────┐     Upload doc     ┌──────────┐
  │ Pending  │──────────────────► │ Pending  │
  └──────────┘                    │ (w/ doc) │
                                  └─────┬────┘
                                        │ Admin reviews
                         ┌──────────────┼──────────────┐
                         ▼              ▼              ▼
                  ┌──────────┐   ┌──────────┐   ┌────────────────┐
                  │ Verified │   │ Rejected │   │ DocumentReq'd  │
                  └──────────┘   └──────────┘   └───────┬────────┘
                                                        │
                                                        │ Partner uploads
                                                        │ additional doc
                                                        ▼
                                                 ┌──────────┐
                                                 │ Pending  │
                                                 │ (w/ doc) │
                                                 └──────────┘
```

---

## 8. CustomerController

**Base Path**: `api/v1/customers`
**Auth Required**: Yes (Bearer Token) for all endpoints

### 8.1 List Customers

Returns a paginated, filterable list of customers.

```
GET /customers
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search by name or phone |
| isBlocked | bool | null | Filter by blocked status |
| minRating | double | null | Minimum average rating |
| sortBy | string | null | Sort field: `name`, `rating`, `deliveries`, `lastDelivery` |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Request Example**:
```
GET /customers?searchTerm=محمد&minRating=3.5&sortBy=rating&pageNumber=1&pageSize=10
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "phone": "+201012345678",
        "name": "محمد أحمد علي",
        "averageRating": 4.3,
        "totalDeliveries": 47,
        "successfulDeliveries": 44,
        "isBlocked": false,
        "lastDeliveryDate": "2026-03-20T14:30:00Z"
      },
      {
        "id": "d4e5f6a7-b8c9-0123-def0-123456789abc",
        "phone": "+201155667788",
        "name": "محمد حسن إبراهيم",
        "averageRating": 3.8,
        "totalDeliveries": 12,
        "successfulDeliveries": 11,
        "isBlocked": false,
        "lastDeliveryDate": "2026-03-15T09:45:00Z"
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 8.2 Get Customer Details

Returns full customer details including addresses, recent orders, and ratings.

```
GET /customers/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "phone": "+201012345678",
    "name": "محمد أحمد علي",
    "averageRating": 4.3,
    "totalDeliveries": 47,
    "successfulDeliveries": 44,
    "isBlocked": false,
    "lastDeliveryDate": "2026-03-20T14:30:00Z",
    "addresses": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "addressText": "12 شارع الأزهر، الحسين، القاهرة",
        "latitude": 30.0459,
        "longitude": 31.2625,
        "addressType": 0,
        "visitCount": 5,
        "landmarks": "بجوار مسجد الحسين",
        "deliveryNotes": "الدور التالت شقة 7"
      }
    ],
    "recentOrders": [
      {
        "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
        "orderDate": "2026-03-20T14:30:00Z",
        "status": "Delivered",
        "total": 185.50
      }
    ],
    "ratings": [
      {
        "ratingValue": 5,
        "feedbackText": "عميل ممتاز",
        "createdAt": "2026-03-20T15:00:00Z",
        "driverName": "أحمد سعيد"
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
| 404 | `العميل غير موجود` |

---

### 8.3 Find Customer by Phone

Finds a customer by their phone number.

```
GET /customers/by-phone/{phone}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `phone` (string) - Phone number in any Egyptian format (e.g., `01012345678`)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "phone": "+201012345678",
    "name": "محمد أحمد علي",
    "averageRating": 4.3,
    "totalDeliveries": 47,
    "successfulDeliveries": 44,
    "isBlocked": false,
    "lastDeliveryDate": "2026-03-20T14:30:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رقم موبايل مصري غير صالح` |
| 404 | `العميل غير موجود` |

---

### 8.4 Update Customer

Updates a customer's editable fields.

```
PUT /customers/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Request Body**:
```json
{
  "name": "محمد أحمد علي الشريف",
  "notes": "يفضل التوصيل بعد الساعة 6 مساءً",
  "preferredPaymentMethod": 0
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Updated display name |
| notes | string | No | Personal notes about this customer |
| preferredPaymentMethod | int | No | See [PaymentMethod](#paymentmethod) enum |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "phone": "+201012345678",
    "name": "محمد أحمد علي الشريف",
    "averageRating": 4.3,
    "totalDeliveries": 47,
    "successfulDeliveries": 44,
    "isBlocked": false,
    "lastDeliveryDate": "2026-03-20T14:30:00Z"
  },
  "message": "تم تحديث بيانات العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 400 | `بيانات غير صالحة` |

---

### 8.5 Rate Customer

Rates a customer with stars and optional quick-tags.

```
POST /customers/{id}/rate
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Request Body**:
```json
{
  "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
  "ratingValue": 4,
  "quickResponse": true,
  "clearAddress": true,
  "respectfulBehavior": true,
  "easyPayment": false,
  "wrongAddress": false,
  "noAnswer": false,
  "delayedPickup": false,
  "paymentIssue": false,
  "feedbackText": "عميل كويس، بس محتاج يجهز الفلوس قبل ما أوصل"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| orderId | guid | No | Related order ID |
| ratingValue | int | Yes | 1-5 |
| quickResponse | bool | No | Positive tag (default false) |
| clearAddress | bool | No | Positive tag (default false) |
| respectfulBehavior | bool | No | Positive tag (default false) |
| easyPayment | bool | No | Positive tag (default false) |
| wrongAddress | bool | No | Negative tag (default false) |
| noAnswer | bool | No | Negative tag (default false) |
| delayedPickup | bool | No | Negative tag (default false) |
| paymentIssue | bool | No | Negative tag (default false) |
| feedbackText | string | No | Max 500 characters |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تقييم العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 400 | `التقييم يجب أن يكون بين 1 و 5` |
| 409 | `تم تقييم هذا العميل لهذا الطلب من قبل` |

---

### 8.6 Block Customer

Blocks a customer, optionally reporting to community blacklist.

```
POST /customers/{id}/block
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Request Body**:
```json
{
  "reason": "بيرفض يدفع بعد التوصيل ومبيردش على التليفون",
  "reportToCommunity": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| reason | string | Yes | Why the customer is being blocked |
| reportToCommunity | bool | No | If true, report is visible to all drivers (default false) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حظر العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 409 | `العميل محظور بالفعل` |

---

### 8.7 Unblock Customer

Unblocks a previously blocked customer.

```
DELETE /customers/{id}/block
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء حظر العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 400 | `العميل غير محظور` |

---

### 8.8 Get Customer Orders

Returns paginated orders for a specific customer.

```
GET /customers/{id}/orders
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

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
        "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
        "orderDate": "2026-03-20T14:30:00Z",
        "status": "Delivered",
        "total": 185.50,
        "pickupAddress": "مطعم الشيف، شارع التحرير",
        "deliveryAddress": "12 شارع الأزهر، الحسين"
      },
      {
        "orderId": "e2d3c4b5-a6f7-8901-bcde-f12345678901",
        "orderDate": "2026-03-18T10:15:00Z",
        "status": "Delivered",
        "total": 95.00,
        "pickupAddress": "صيدلية الحياة، مدينة نصر",
        "deliveryAddress": "12 شارع الأزهر، الحسين"
      }
    ],
    "totalCount": 47,
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

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |

---

### 8.9 Upload Voice Memo

Uploads a voice memo about a customer (multipart form data).

```
POST /customers/{id}/voice-memo
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Content-Type**: `multipart/form-data`

**Form Fields**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| file | binary | Yes | Audio file (mp3, m4a, wav, aac). Max 5 MB |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "voiceMemoUrl": "https://sekka.runasp.net/uploads/voice-memos/vm_a1b2c3d4.m4a",
    "duration": 12.5,
    "createdAt": "2026-03-26T10:30:00Z"
  },
  "message": "تم رفع المذكرة الصوتية بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 400 | `الملف مطلوب` |
| 400 | `نوع الملف غير مدعوم. الأنواع المدعومة: mp3, m4a, wav, aac` |
| 400 | `حجم الملف يتجاوز الحد المسموح (5 ميجابايت)` |

---

### 8.10 Get Customer Interests

Returns interest tags and patterns for a customer based on order history.

```
GET /customers/{id}/interests
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "topCategories": ["طعام", "أدوية", "بقالة"],
    "preferredPartners": [
      { "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901", "partnerName": "مطعم الشيف", "orderCount": 15 },
      { "partnerId": "c3d4e5f6-a7b8-9012-cdef-234567890123", "partnerName": "صيدلية الحياة", "orderCount": 8 }
    ],
    "preferredDeliveryTimes": ["evening", "afternoon"],
    "averageOrderValue": 142.30
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

### 8.11 Get Customer Engagement

Returns engagement level and activity metrics for a customer.

```
GET /customers/{id}/engagement
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "engagementLevel": "High",
    "ordersThisMonth": 8,
    "ordersLastMonth": 6,
    "daysSinceLastOrder": 6,
    "averageOrdersPerMonth": 7.2,
    "lifetimeValue": 6689.50,
    "retentionRisk": "Low"
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

## 9. CallerIdController

**Base Path**: `api/v1/caller-id`
**Auth Required**: Yes (Bearer Token) for all endpoints

### 9.1 Lookup Caller Identity

Looks up a phone number to identify the caller (customer, partner, or unknown).

```
GET /caller-id/lookup/{phone}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `phone` (string) - Phone number in any Egyptian format

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "phoneNumber": "+201012345678",
    "displayName": "محمد أحمد",
    "contactType": 0,
    "customerName": "محمد أحمد علي",
    "partnerName": null,
    "lastOrderDate": "2026-03-20T14:30:00Z",
    "averageRating": 4.3,
    "note": "بيفضل التوصيل الدور الخامس - عمارة بدون أسانسير",
    "isBlocked": false
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رقم موبايل مصري غير صالح` |
| 404 | `الرقم غير مسجل` |

---

### 9.2 Create Caller ID Note

Creates a caller ID entry with a personal note.

```
POST /caller-id
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "phoneNumber": "01023456789",
  "contactType": 0,
  "displayName": "عم حسن البقال",
  "note": "محل بقالة في شارع فيصل - بيطلب توصيل كل يوم جمعة"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| phoneNumber | string | Yes | Valid Egyptian mobile number |
| contactType | int | Yes | See [ContactType](#contacttype) enum |
| displayName | string | No | Custom display name |
| note | string | No | Personal note about this contact |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "phoneNumber": "+201023456789",
    "displayName": "عم حسن البقال",
    "contactType": 0,
    "customerName": null,
    "partnerName": null,
    "lastOrderDate": null,
    "averageRating": null,
    "note": "محل بقالة في شارع فيصل - بيطلب توصيل كل يوم جمعة",
    "isBlocked": false
  },
  "message": "تم حفظ بيانات المتصل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رقم موبايل مصري غير صالح` |
| 409 | `الرقم مسجل بالفعل` |

---

### 9.3 Update Caller ID Note

Updates an existing caller ID entry.

```
PUT /caller-id/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Caller ID entry ID

**Request Body**:
```json
{
  "displayName": "عم حسن - بقالة الأمانة",
  "note": "محل بقالة في شارع فيصل - بيطلب توصيل كل يوم جمعة وسبت"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "phoneNumber": "+201023456789",
    "displayName": "عم حسن - بقالة الأمانة",
    "contactType": 0,
    "customerName": null,
    "partnerName": null,
    "lastOrderDate": null,
    "averageRating": null,
    "note": "محل بقالة في شارع فيصل - بيطلب توصيل كل يوم جمعة وسبت",
    "isBlocked": false
  },
  "message": "تم تحديث بيانات المتصل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `بيانات المتصل غير موجودة` |

---

### 9.4 Delete Caller ID Note

Deletes a caller ID entry.

```
DELETE /caller-id/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Caller ID entry ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف بيانات المتصل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `بيانات المتصل غير موجودة` |

---

### 9.5 Truecaller Lookup

Looks up a phone number using Truecaller integration (stub - returns basic data).

```
GET /caller-id/truecaller-lookup/{phone}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `phone` (string) - Phone number

> **Note**: This endpoint is currently a stub and may return limited data. Full Truecaller integration is planned for a future release.

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "phoneNumber": "+201012345678",
    "truecallerName": "Mohammed Ahmed",
    "spamScore": 0,
    "isSpam": false
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رقم موبايل مصري غير صالح` |
| 404 | `لم يتم العثور على بيانات لهذا الرقم` |

---

## 10. AddressController

**Base Path**: `api/v1/addresses`
**Auth Required**: Yes (Bearer Token) for all endpoints

### 10.1 Search Addresses

Searches and filters addresses.

```
GET /addresses
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search in address text and landmarks |
| customerId | guid | null | Filter by customer ID |
| addressType | int | null | Filter by [AddressType](#addresstype) enum |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Request Example**:
```
GET /addresses?customerId=a1b2c3d4-e5f6-7890-abcd-ef1234567890&addressType=0
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "addressText": "12 شارع الأزهر، الحسين، القاهرة",
        "latitude": 30.0459,
        "longitude": 31.2625,
        "addressType": 0,
        "visitCount": 5,
        "landmarks": "بجوار مسجد الحسين - فوق صيدلية الأزهر",
        "deliveryNotes": "الدور التالت شقة 7 - الجرس بايظ، كلمني لما توصل"
      },
      {
        "id": "d4e5f6a7-b8c9-0123-def0-234567890123",
        "addressText": "شارع المعز لدين الله، الجمالية، القاهرة",
        "latitude": 30.0500,
        "longitude": 31.2620,
        "addressType": 0,
        "visitCount": 2,
        "landmarks": "قدام باب الفتوح",
        "deliveryNotes": null
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 10.2 Save New Address

Creates a new address entry.

```
POST /addresses
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "addressText": "45 شارع عباس العقاد، مدينة نصر، القاهرة",
  "latitude": 30.0561,
  "longitude": 31.3401,
  "addressType": 1,
  "landmarks": "برج القاهرة الطبي - الدور الأرضي بجوار مكتبة دار المعارف",
  "deliveryNotes": "مدخل العمارة من الشارع الجانبي - كلم الأمن الأول"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| customerId | guid | No | Link to a customer |
| addressText | string | Yes | Full address text |
| latitude | double | No | GPS latitude |
| longitude | double | No | GPS longitude |
| addressType | int | Yes | See [AddressType](#addresstype) enum |
| landmarks | string | No | Nearby landmarks |
| deliveryNotes | string | No | Special delivery instructions |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-345678901234",
    "addressText": "45 شارع عباس العقاد، مدينة نصر، القاهرة",
    "latitude": 30.0561,
    "longitude": 31.3401,
    "addressType": 1,
    "visitCount": 0,
    "landmarks": "برج القاهرة الطبي - الدور الأرضي بجوار مكتبة دار المعارف",
    "deliveryNotes": "مدخل العمارة من الشارع الجانبي - كلم الأمن الأول"
  },
  "message": "تم حفظ العنوان بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `العنوان مطلوب` |
| 400 | `نوع العنوان غير صالح` |

---

### 10.3 Update Address

Updates an existing address.

```
PUT /addresses/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Address ID

**Request Body**:
```json
{
  "addressText": "45 شارع عباس العقاد، مدينة نصر، القاهرة (بجوار سيتي ستارز)",
  "latitude": 30.0561,
  "longitude": 31.3401,
  "addressType": 1,
  "landmarks": "قدام سيتي ستارز - برج القاهرة الطبي",
  "deliveryNotes": "مدخل العمارة من الشارع الجانبي"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-345678901234",
    "addressText": "45 شارع عباس العقاد، مدينة نصر، القاهرة (بجوار سيتي ستارز)",
    "latitude": 30.0561,
    "longitude": 31.3401,
    "addressType": 1,
    "visitCount": 0,
    "landmarks": "قدام سيتي ستارز - برج القاهرة الطبي",
    "deliveryNotes": "مدخل العمارة من الشارع الجانبي"
  },
  "message": "تم تحديث العنوان بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العنوان غير موجود` |
| 400 | `بيانات غير صالحة` |

---

### 10.4 Delete Address

Deletes an address.

```
DELETE /addresses/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Address ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف العنوان بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العنوان غير موجود` |

---

### 10.5 Autocomplete

Returns address suggestions based on a search query, optionally biased by location.

```
GET /addresses/autocomplete
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| q | string | Yes | Search query (min 2 characters) |
| latitude | double | No | Current latitude for location bias |
| longitude | double | No | Current longitude for location bias |

**Request Example**:
```
GET /addresses/autocomplete?q=شارع التحرير&latitude=30.0444&longitude=31.2357
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "addressText": "شارع التحرير، الدقي، الجيزة",
      "latitude": 30.0380,
      "longitude": 31.2116,
      "visitCount": 12
    },
    {
      "addressText": "ميدان التحرير، وسط البلد، القاهرة",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "visitCount": 8
    },
    {
      "addressText": "شارع التحرير، المنيل، القاهرة",
      "latitude": 30.0250,
      "longitude": 31.2270,
      "visitCount": 3
    }
  ],
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `نص البحث يجب أن يكون حرفين على الأقل` |

---

### 10.6 Nearby Addresses

Returns addresses within a given radius of a location.

```
GET /addresses/nearby
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| latitude | double | Yes | Center latitude |
| longitude | double | Yes | Center longitude |
| radiusKm | double | No | Radius in kilometers (default 2.0) |

**Request Example**:
```
GET /addresses/nearby?latitude=30.0444&longitude=31.2357&radiusKm=1.5
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "addressText": "15 شارع طلعت حرب، وسط البلد، القاهرة",
      "latitude": 30.0450,
      "longitude": 31.2360,
      "addressType": 2,
      "visitCount": 20,
      "landmarks": "فوق جروبي - الدور الثاني",
      "deliveryNotes": null,
      "distanceKm": 0.08
    },
    {
      "id": "d4e5f6a7-b8c9-0123-def0-234567890123",
      "addressText": "شارع قصر النيل، وسط البلد، القاهرة",
      "latitude": 30.0460,
      "longitude": 31.2370,
      "addressType": 1,
      "visitCount": 7,
      "landmarks": "بجوار بنك مصر",
      "deliveryNotes": "الباب الخلفي للمبنى",
      "distanceKm": 0.22
    }
  ],
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الإحداثيات مطلوبة` |
| 400 | `نطاق البحث يجب أن يكون بين 0.1 و 50 كيلومتر` |

---

## 11. PartnerController

**Base Path**: `api/v1/partners`
**Auth Required**: Yes (Bearer Token) for all endpoints

### 11.1 List Partners

Returns a list of all partners.

```
GET /partners
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search by name or phone |
| partnerType | int | null | Filter by [PartnerType](#partnertype) |
| isActive | bool | null | Filter by active status |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "name": "مطعم الشيف",
        "partnerType": 0,
        "phone": "+201112223344",
        "address": "15 شارع التحرير، الدقي، الجيزة",
        "commissionType": 1,
        "commissionValue": 12.5,
        "color": "#FF5722",
        "logoUrl": "https://sekka.runasp.net/uploads/partners/logo_chef.png",
        "isActive": true,
        "verificationStatus": 1
      },
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-234567890123",
        "name": "صيدلية الحياة",
        "partnerType": 2,
        "phone": "+201098765432",
        "address": "8 شارع مصطفى النحاس، مدينة نصر، القاهرة",
        "commissionType": 0,
        "commissionValue": 7.00,
        "color": "#4CAF50",
        "logoUrl": null,
        "isActive": true,
        "verificationStatus": 1
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 11.2 Create Partner

Creates a new partner.

```
POST /partners
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "name": "سوبرماركت المحمدي",
  "partnerType": 3,
  "phone": "01234567890",
  "address": "شارع الهرم، فيصل، الجيزة",
  "commissionType": 0,
  "commissionValue": 5.00,
  "defaultPaymentMethod": 0,
  "color": "#2196F3",
  "receiptHeader": "سوبرماركت المحمدي - خدمة 24 ساعة"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Business name (max 100 chars) |
| partnerType | int | Yes | See [PartnerType](#partnertype) enum |
| phone | string | No | Contact phone |
| address | string | No | Business address |
| commissionType | int | Yes | See [CommissionType](#commissiontype) enum |
| commissionValue | decimal | Yes | Commission amount or percentage |
| defaultPaymentMethod | int | Yes | See [PaymentMethod](#paymentmethod) enum |
| color | string | No | Brand color (hex) |
| receiptHeader | string | No | Custom receipt header text |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "d4e5f6a7-b8c9-0123-def0-345678901234",
    "name": "سوبرماركت المحمدي",
    "partnerType": 3,
    "phone": "+201234567890",
    "address": "شارع الهرم، فيصل، الجيزة",
    "commissionType": 0,
    "commissionValue": 5.00,
    "color": "#2196F3",
    "logoUrl": null,
    "isActive": true,
    "verificationStatus": 0
  },
  "message": "تم إنشاء الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `اسم الشريك مطلوب` |
| 400 | `نوع العمولة غير صالح` |
| 409 | `شريك بنفس الاسم موجود بالفعل` |

---

### 11.3 Update Partner

Updates an existing partner.

```
PUT /partners/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "name": "سوبرماركت المحمدي - فرع فيصل",
  "phone": "01234567890",
  "address": "شارع الهرم الرئيسي، فيصل، الجيزة",
  "commissionType": 0,
  "commissionValue": 6.00,
  "color": "#1976D2"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "d4e5f6a7-b8c9-0123-def0-345678901234",
    "name": "سوبرماركت المحمدي - فرع فيصل",
    "partnerType": 3,
    "phone": "+201234567890",
    "address": "شارع الهرم الرئيسي، فيصل، الجيزة",
    "commissionType": 0,
    "commissionValue": 6.00,
    "color": "#1976D2",
    "logoUrl": null,
    "isActive": true,
    "verificationStatus": 0
  },
  "message": "تم تحديث بيانات الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `بيانات غير صالحة` |

---

### 11.4 Delete Partner

Deletes a partner (soft delete).

```
DELETE /partners/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `لا يمكن حذف شريك لديه طلبات نشطة` |

---

### 11.5 Get Partner Orders

Returns paginated orders for a specific partner.

```
GET /partners/{id}/orders
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

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
        "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
        "orderDate": "2026-03-25T18:30:00Z",
        "status": "Delivered",
        "total": 245.00,
        "customerName": "محمد أحمد علي",
        "deliveryAddress": "12 شارع الأزهر، الحسين",
        "driverName": "أحمد سعيد"
      },
      {
        "orderId": "e2d3c4b5-a6f7-8901-bcde-f12345678901",
        "orderDate": "2026-03-25T17:15:00Z",
        "status": "InTransit",
        "total": 180.00,
        "customerName": "فاطمة حسن",
        "deliveryAddress": "شارع عباس العقاد، مدينة نصر",
        "driverName": "كريم محمود"
      }
    ],
    "totalCount": 156,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 16,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |

---

### 11.6 Get Partner Pickup Points

Returns all pickup points for a partner.

```
GET /partners/{id}/pickup-points
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "e5f6a7b8-c9d0-1234-ef01-456789012345",
      "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "name": "الفرع الرئيسي - الدقي",
      "address": "15 شارع التحرير، الدقي، الجيزة",
      "latitude": 30.0380,
      "longitude": 31.2116,
      "averageRating": 4.2,
      "averageWaitingMinutes": 8.5
    },
    {
      "id": "f6a7b8c9-d0e1-2345-f012-567890123456",
      "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "name": "فرع المهندسين",
      "address": "شارع شهاب، المهندسين، الجيزة",
      "latitude": 30.0550,
      "longitude": 31.2050,
      "averageRating": 3.8,
      "averageWaitingMinutes": 14.2
    }
  ],
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |

---

### 11.7 Submit Verification Document

Uploads a verification document for a partner (multipart form data).

```
POST /partners/{id}/submit-verification
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

**Content-Type**: `multipart/form-data`

**Form Fields**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| file | binary | Yes | Document image (jpg, png, pdf). Max 10 MB |
| documentType | string | No | Type description (e.g., "سجل تجاري", "بطاقة ضريبية") |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "documentUrl": "https://sekka.runasp.net/uploads/verification/doc_b2c3d4e5.pdf",
    "uploadedAt": "2026-03-26T11:00:00Z",
    "verificationStatus": 0
  },
  "message": "تم رفع المستند بنجاح. سيتم مراجعته خلال 24 ساعة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `الملف مطلوب` |
| 400 | `نوع الملف غير مدعوم. الأنواع المدعومة: jpg, png, pdf` |
| 400 | `حجم الملف يتجاوز الحد المسموح (10 ميجابايت)` |

---

### 11.8 Get Verification Status

Returns the current verification status for a partner.

```
GET /partners/{id}/verification-status
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "verificationStatus": 1,
    "verificationStatusName": "Verified",
    "verifiedAt": "2026-03-20T09:00:00Z",
    "rejectionReason": null,
    "requestedDocuments": [],
    "submittedDocuments": [
      {
        "documentUrl": "https://sekka.runasp.net/uploads/verification/doc_b2c3d4e5.pdf",
        "documentType": "سجل تجاري",
        "uploadedAt": "2026-03-19T14:00:00Z"
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
| 404 | `الشريك غير موجود` |

---

## 12. PickupPointController

**Base Path**: `api/v1/pickup-points`
**Auth Required**: Yes (Bearer Token) for all endpoints

### 12.1 Create Pickup Point

Creates a new pickup point for a partner.

```
POST /pickup-points
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
  "name": "فرع الزمالك",
  "address": "26 شارع 26 يوليو، الزمالك، القاهرة",
  "latitude": 30.0650,
  "longitude": 31.2240
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| partnerId | guid | Yes | Partner this pickup point belongs to |
| name | string | Yes | Pickup point name (max 100 chars) |
| address | string | Yes | Full address |
| latitude | double | No | GPS latitude |
| longitude | double | No | GPS longitude |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a7b8c9d0-e1f2-3456-0123-678901234567",
    "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "name": "فرع الزمالك",
    "address": "26 شارع 26 يوليو، الزمالك، القاهرة",
    "latitude": 30.0650,
    "longitude": 31.2240,
    "averageRating": 0.0,
    "averageWaitingMinutes": 0.0
  },
  "message": "تم إنشاء نقطة الاستلام بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `اسم نقطة الاستلام مطلوب` |
| 400 | `العنوان مطلوب` |
| 404 | `الشريك غير موجود` |

---

### 12.2 Update Pickup Point

Updates an existing pickup point.

```
PUT /pickup-points/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Pickup Point ID

**Request Body**:
```json
{
  "name": "فرع الزمالك - الشارع الرئيسي",
  "address": "26 شارع 26 يوليو، الزمالك، القاهرة",
  "latitude": 30.0650,
  "longitude": 31.2240
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a7b8c9d0-e1f2-3456-0123-678901234567",
    "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "name": "فرع الزمالك - الشارع الرئيسي",
    "address": "26 شارع 26 يوليو، الزمالك، القاهرة",
    "latitude": 30.0650,
    "longitude": 31.2240,
    "averageRating": 4.2,
    "averageWaitingMinutes": 8.5
  },
  "message": "تم تحديث نقطة الاستلام بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `نقطة الاستلام غير موجودة` |

---

### 12.3 Delete Pickup Point

Deletes a pickup point.

```
DELETE /pickup-points/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Pickup Point ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف نقطة الاستلام بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `نقطة الاستلام غير موجودة` |

---

### 12.4 Rate Pickup Point

Rates a pickup point (drivers rate based on waiting time and experience).

```
POST /pickup-points/{id}/rate
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Pickup Point ID

**Request Body**:
```json
{
  "rating": 4,
  "waitingMinutes": 7
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| rating | int | Yes | 1-5 star rating |
| waitingMinutes | int | No | How long the driver waited (minutes) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تقييم نقطة الاستلام بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `نقطة الاستلام غير موجودة` |
| 400 | `التقييم يجب أن يكون بين 1 و 5` |

---

## 13. PartnerPortalController

**Base Path**: `api/v1/partner`
**Auth Required**: Yes (Bearer Token) for all endpoints

> **Note**: These endpoints are for the partner's own portal view. The authenticated user must be a partner.

### 13.1 Partner Dashboard

Returns the partner's dashboard summary.

```
GET /partner/dashboard
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "todayOrders": 23,
    "todayRevenue": 4250.00,
    "pendingOrders": 5,
    "inTransitOrders": 3,
    "todayDelivered": 15,
    "averageDeliveryTime": 32.5,
    "thisMonthOrders": 456,
    "thisMonthRevenue": 78500.00,
    "topPickupPoint": "الفرع الرئيسي - الدقي",
    "verificationStatus": 1
  },
  "message": null,
  "errors": null
}
```

---

### 13.2 Partner Orders

Returns filtered orders for the authenticated partner.

```
GET /partner/orders
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| status | string | null | Filter by order status |
| fromDate | datetime | null | Filter from date |
| toDate | datetime | null | Filter to date |
| pickupPointId | guid | null | Filter by pickup point |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "orderId": "f1e2d3c4-b5a6-7890-abcd-ef1234567890",
        "orderDate": "2026-03-26T12:30:00Z",
        "status": "Pending",
        "total": 320.00,
        "customerName": "هدى مصطفى",
        "customerPhone": "+201155443322",
        "deliveryAddress": "شارع مكرم عبيد، مدينة نصر",
        "pickupPointName": "الفرع الرئيسي - الدقي",
        "driverName": null,
        "estimatedDeliveryTime": null
      }
    ],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 13.3 Partner Settlements

Returns settlement history for the authenticated partner.

```
GET /partner/settlements
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| fromDate | datetime | null | Filter from date |
| toDate | datetime | null | Filter to date |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-111111111111",
        "settlementDate": "2026-03-25T00:00:00Z",
        "totalOrders": 45,
        "grossAmount": 12500.00,
        "commissionAmount": 1562.50,
        "netAmount": 10937.50,
        "status": "Paid",
        "paymentMethod": "InstaPay",
        "referenceNumber": "STL-20260325-001"
      },
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-222222222222",
        "settlementDate": "2026-03-24T00:00:00Z",
        "totalOrders": 38,
        "grossAmount": 9800.00,
        "commissionAmount": 1225.00,
        "netAmount": 8575.00,
        "status": "Pending",
        "paymentMethod": null,
        "referenceNumber": "STL-20260324-001"
      }
    ],
    "totalCount": 30,
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

---

### 13.4 Update Partner Settings

Updates the authenticated partner's settings.

```
PUT /partner/settings
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Request Body**:
```json
{
  "defaultPaymentMethod": 3,
  "receiptHeader": "مطعم الشيف - أكل بيتي على أصوله",
  "autoAcceptOrders": true,
  "notificationsEnabled": true,
  "operatingHoursStart": "09:00",
  "operatingHoursEnd": "23:00"
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `بيانات غير صالحة` |

---

### 13.5 Partner Statistics

Returns detailed statistics for the authenticated partner.

```
GET /partner/stats
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| period | string | "month" | Period: `week`, `month`, `quarter`, `year` |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "period": "month",
    "totalOrders": 456,
    "totalRevenue": 78500.00,
    "totalCommission": 9812.50,
    "averageOrderValue": 172.15,
    "deliverySuccessRate": 96.5,
    "averageDeliveryTimeMinutes": 32.5,
    "topCustomers": [
      { "customerName": "محمد أحمد علي", "orderCount": 15, "totalSpent": 2850.00 },
      { "customerName": "فاطمة حسن", "orderCount": 12, "totalSpent": 2100.00 }
    ],
    "ordersByDay": [
      { "date": "2026-03-01", "count": 18 },
      { "date": "2026-03-02", "count": 22 },
      { "date": "2026-03-03", "count": 15 }
    ],
    "pickupPointPerformance": [
      {
        "pickupPointName": "الفرع الرئيسي - الدقي",
        "orderCount": 280,
        "averageWaitingMinutes": 8.5,
        "rating": 4.2
      },
      {
        "pickupPointName": "فرع المهندسين",
        "orderCount": 176,
        "averageWaitingMinutes": 14.2,
        "rating": 3.8
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 13.6 Partner Invoices

Returns invoices for the authenticated partner.

```
GET /partner/invoices
```

**Auth Required**: Yes (Bearer Token, Partner role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| fromDate | datetime | null | Filter from date |
| toDate | datetime | null | Filter to date |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-333333333333",
        "invoiceNumber": "INV-2026-0312",
        "issueDate": "2026-03-01T00:00:00Z",
        "dueDate": "2026-03-15T00:00:00Z",
        "totalAmount": 9812.50,
        "status": "Paid",
        "paidAt": "2026-03-10T14:30:00Z",
        "downloadUrl": "https://sekka.runasp.net/invoices/INV-2026-0312.pdf"
      },
      {
        "id": "d4e5f6a7-b8c9-0123-def0-444444444444",
        "invoiceNumber": "INV-2026-0225",
        "issueDate": "2026-02-01T00:00:00Z",
        "dueDate": "2026-02-15T00:00:00Z",
        "totalAmount": 8450.00,
        "status": "Paid",
        "paidAt": "2026-02-12T10:00:00Z",
        "downloadUrl": "https://sekka.runasp.net/invoices/INV-2026-0225.pdf"
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

---

## 14. AdminCustomersController

**Base Path**: `api/v1/admin/customers`
**Auth Required**: Yes (Bearer Token, Admin role)

> **Note**: All endpoints in this section require the `Admin` role. Non-admin users will receive `403 Forbidden`.

### 14.1 List All Customers

Returns all customers with admin-level details.

```
GET /admin/customers
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search by name or phone |
| isBlocked | bool | null | Filter by blocked status |
| minRating | double | null | Minimum average rating |
| sortBy | string | null | Sort field |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "phone": "+201012345678",
        "name": "محمد أحمد علي",
        "averageRating": 4.3,
        "totalDeliveries": 47,
        "successfulDeliveries": 44,
        "isBlocked": false,
        "lastDeliveryDate": "2026-03-20T14:30:00Z",
        "createdAt": "2025-06-15T08:00:00Z",
        "blockCount": 0,
        "reportCount": 0
      },
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-aabbccddeeff",
        "phone": "+201155667788",
        "name": "سارة خالد",
        "averageRating": 2.1,
        "totalDeliveries": 23,
        "successfulDeliveries": 15,
        "isBlocked": true,
        "lastDeliveryDate": "2026-02-10T11:00:00Z",
        "createdAt": "2025-09-20T12:00:00Z",
        "blockCount": 3,
        "reportCount": 5
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

---

### 14.2 Get Customer Details (Admin)

Returns full customer details with admin-specific data.

```
GET /admin/customers/{id}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bcde-aabbccddeeff",
    "phone": "+201155667788",
    "name": "سارة خالد",
    "averageRating": 2.1,
    "totalDeliveries": 23,
    "successfulDeliveries": 15,
    "isBlocked": true,
    "lastDeliveryDate": "2026-02-10T11:00:00Z",
    "createdAt": "2025-09-20T12:00:00Z",
    "blockHistory": [
      {
        "blockedBy": "أحمد سعيد",
        "reason": "بيرفض يدفع بعد التوصيل",
        "blockedAt": "2026-01-15T10:00:00Z",
        "reportedToCommunity": true
      },
      {
        "blockedBy": "كريم محمود",
        "reason": "عنوان غلط ومبيردش",
        "blockedAt": "2026-02-01T14:00:00Z",
        "reportedToCommunity": false
      }
    ],
    "ratings": [
      {
        "ratingValue": 1,
        "feedbackText": "عميل صعب جداً",
        "wrongAddress": true,
        "noAnswer": true,
        "paymentIssue": true,
        "createdAt": "2026-02-10T12:00:00Z",
        "driverName": "محمد حسين"
      }
    ],
    "addresses": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "addressText": "شارع المنيل، المنيل، القاهرة",
        "addressType": 0,
        "visitCount": 8
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
| 404 | `العميل غير موجود` |

---

### 14.3 Block Customer (Admin)

Admin blocks a customer (system-wide block).

```
PUT /admin/customers/{id}/block
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Customer ID

**Request Body**:
```json
{
  "reason": "تكرار شكاوى من السائقين - رفض الدفع",
  "notifyDrivers": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| reason | string | Yes | Block reason |
| notifyDrivers | bool | No | Send notification to drivers (default false) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حظر العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 409 | `العميل محظور بالفعل` |

---

### 14.4 Unblock Customer (Admin)

Admin unblocks a customer.

```
DELETE /admin/customers/{id}/block
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Customer ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء حظر العميل بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `العميل غير موجود` |
| 400 | `العميل غير محظور` |

---

### 14.5 Customer Reports

Returns aggregated customer reports and analytics.

```
GET /admin/customers/reports
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| fromDate | datetime | null | Report start date |
| toDate | datetime | null | Report end date |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCustomers": 1250,
    "activeCustomers": 890,
    "blockedCustomers": 45,
    "newCustomersThisMonth": 78,
    "averageRating": 3.9,
    "topRatedCustomers": [
      { "name": "محمد أحمد علي", "phone": "+201012345678", "averageRating": 5.0, "totalDeliveries": 47 }
    ],
    "mostBlockedCustomers": [
      { "name": "سارة خالد", "phone": "+201155667788", "blockCount": 3, "reportCount": 5 }
    ],
    "ratingDistribution": {
      "fiveStars": 320,
      "fourStars": 280,
      "threeStars": 190,
      "twoStars": 85,
      "oneStar": 45
    }
  },
  "message": null,
  "errors": null
}
```

---

## 15. AdminPartnersController

**Base Path**: `api/v1/admin/partners`
**Auth Required**: Yes (Bearer Token, Admin role)

### 15.1 List All Partners

Returns all partners with admin-level details.

```
GET /admin/partners
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search by name |
| partnerType | int | null | Filter by [PartnerType](#partnertype) |
| verificationStatus | int | null | Filter by [VerificationStatus](#verificationstatus) |
| isActive | bool | null | Filter by active status |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "name": "مطعم الشيف",
        "partnerType": 0,
        "phone": "+201112223344",
        "address": "15 شارع التحرير، الدقي، الجيزة",
        "commissionType": 1,
        "commissionValue": 12.5,
        "color": "#FF5722",
        "logoUrl": "https://sekka.runasp.net/uploads/partners/logo_chef.png",
        "isActive": true,
        "verificationStatus": 1,
        "totalOrders": 1580,
        "totalRevenue": 245000.00,
        "createdAt": "2025-08-10T09:00:00Z"
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

---

### 15.2 Create Partner (Admin)

Admin creates a new partner (can set verification status directly).

```
POST /admin/partners
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Request Body**:
```json
{
  "name": "مطعم الكبابجي",
  "partnerType": 0,
  "phone": "01199887766",
  "address": "شارع الملك فيصل، الهرم، الجيزة",
  "commissionType": 1,
  "commissionValue": 10.0,
  "defaultPaymentMethod": 0,
  "color": "#E91E63",
  "receiptHeader": "مطعم الكبابجي - كباب ع الفحم"
}
```

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-567890123456",
    "name": "مطعم الكبابجي",
    "partnerType": 0,
    "phone": "+201199887766",
    "address": "شارع الملك فيصل، الهرم، الجيزة",
    "commissionType": 1,
    "commissionValue": 10.0,
    "color": "#E91E63",
    "logoUrl": null,
    "isActive": true,
    "verificationStatus": 0
  },
  "message": "تم إنشاء الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `اسم الشريك مطلوب` |
| 409 | `شريك بنفس الاسم موجود بالفعل` |

---

### 15.3 Update Partner (Admin)

Admin updates a partner's details.

```
PUT /admin/partners/{id}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "name": "مطعم الكبابجي - الفرع الرئيسي",
  "commissionType": 1,
  "commissionValue": 8.0,
  "isActive": true
}
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-567890123456",
    "name": "مطعم الكبابجي - الفرع الرئيسي",
    "partnerType": 0,
    "phone": "+201199887766",
    "address": "شارع الملك فيصل، الهرم، الجيزة",
    "commissionType": 1,
    "commissionValue": 8.0,
    "color": "#E91E63",
    "logoUrl": null,
    "isActive": true,
    "verificationStatus": 0
  },
  "message": "تم تحديث بيانات الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |

---

### 15.4 Delete Partner (Admin)

Admin deletes a partner.

```
DELETE /admin/partners/{id}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Partner ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الشريك بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `لا يمكن حذف شريك لديه طلبات نشطة` |

---

### 15.5 Partner Analytics

Returns detailed analytics for a specific partner.

```
GET /admin/partners/{id}/analytics
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Partner ID

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| period | string | "month" | Period: `week`, `month`, `quarter`, `year` |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "partnerId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "partnerName": "مطعم الشيف",
    "period": "month",
    "totalOrders": 456,
    "totalRevenue": 78500.00,
    "totalCommission": 9812.50,
    "averageOrderValue": 172.15,
    "deliverySuccessRate": 96.5,
    "averageDeliveryTimeMinutes": 32.5,
    "cancelledOrders": 16,
    "cancellationRate": 3.5,
    "customerRetentionRate": 72.3,
    "peakHours": ["12:00-14:00", "19:00-21:00"],
    "orderTrend": [
      { "date": "2026-03-01", "count": 18, "revenue": 3200.00 },
      { "date": "2026-03-02", "count": 22, "revenue": 3850.00 },
      { "date": "2026-03-03", "count": 15, "revenue": 2600.00 }
    ]
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |

---

### 15.6 Pending Verification List

Returns partners awaiting verification.

```
GET /admin/partners/pending-verification
```

**Auth Required**: Yes (Bearer Token, Admin role)

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
        "id": "d4e5f6a7-b8c9-0123-def0-345678901234",
        "name": "سوبرماركت المحمدي",
        "partnerType": 3,
        "phone": "+201234567890",
        "verificationStatus": 0,
        "createdAt": "2026-03-24T10:00:00Z",
        "submittedDocuments": [
          {
            "documentUrl": "https://sekka.runasp.net/uploads/verification/doc_d4e5f6a7.pdf",
            "documentType": "سجل تجاري",
            "uploadedAt": "2026-03-24T10:30:00Z"
          }
        ]
      },
      {
        "id": "e5f6a7b8-c9d0-1234-ef01-567890123456",
        "name": "مطعم الكبابجي",
        "partnerType": 0,
        "phone": "+201199887766",
        "verificationStatus": 0,
        "createdAt": "2026-03-25T14:00:00Z",
        "submittedDocuments": []
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

### 15.7 Verify/Reject Partner

Admin verifies or rejects a partner's verification request.

```
POST /admin/partners/{id}/verify
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "approved": true,
  "rejectionReason": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| approved | bool | Yes | `true` to verify, `false` to reject |
| rejectionReason | string | No | Required if `approved` is `false` |

**Success Response (Approved)** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم توثيق الشريك بنجاح",
  "errors": null
}
```

**Success Response (Rejected)** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم رفض طلب التوثيق",
  "errors": null
}
```

**Rejection Request Body Example**:
```json
{
  "approved": false,
  "rejectionReason": "المستندات المقدمة غير واضحة. يرجى رفع صور أوضح للسجل التجاري"
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `سبب الرفض مطلوب` |
| 400 | `الشريك موثّق بالفعل` |

---

### 15.8 Request Additional Document

Admin requests an additional document from a partner.

```
POST /admin/partners/{id}/request-document
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Partner ID

**Request Body**:
```json
{
  "documentType": "بطاقة ضريبية",
  "message": "يرجى رفع صورة واضحة من البطاقة الضريبية لاستكمال التوثيق"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| documentType | string | Yes | Type of document requested |
| message | string | No | Message to the partner explaining what is needed |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم طلب المستند الإضافي من الشريك",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الشريك غير موجود` |
| 400 | `نوع المستند مطلوب` |

---

## 16. AdminBlacklistController

**Base Path**: `api/v1/admin/blacklist`
**Auth Required**: Yes (Bearer Token, Admin role)

### 16.1 Blacklist Entries

Returns all blacklisted phone numbers.

```
GET /admin/blacklist
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| searchTerm | string | null | Search by phone or name |
| pageNumber | int | 1 | Page number |
| pageSize | int | 10 | Items per page |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "phoneNumber": "+201155667788",
        "customerName": "سارة خالد",
        "reportCount": 5,
        "lastReportDate": "2026-03-15T10:00:00Z",
        "isVerified": true,
        "verifiedBy": "أدمن النظام",
        "verifiedAt": "2026-03-16T09:00:00Z",
        "reasons": [
          "بيرفض يدفع بعد التوصيل",
          "عنوان غلط ومبيردش",
          "تعامل غير محترم"
        ]
      },
      {
        "phoneNumber": "+201099887766",
        "customerName": null,
        "reportCount": 3,
        "lastReportDate": "2026-03-20T14:00:00Z",
        "isVerified": false,
        "verifiedBy": null,
        "verifiedAt": null,
        "reasons": [
          "محدش فتح الباب 3 مرات",
          "بيعمل طلبات وهمية"
        ]
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

---

### 16.2 Verify Community Report

Admin verifies a community-reported phone number (confirms the blacklist entry).

```
POST /admin/blacklist/verify/{phone}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `phone` (string) - Phone number

**Request Body**:
```json
{
  "notes": "تم التحقق من البلاغات - العميل بالفعل بيرفض الدفع بشكل متكرر"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| notes | string | No | Admin verification notes |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تأكيد البلاغ وتوثيق الحظر",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الرقم غير موجود في القائمة السوداء` |
| 400 | `البلاغ موثّق بالفعل` |

---

### 16.3 Remove from Blacklist

Admin removes a phone number from the community blacklist.

```
DELETE /admin/blacklist/{phone}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `phone` (string) - Phone number

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إزالة الرقم من القائمة السوداء",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الرقم غير موجود في القائمة السوداء` |

---

### 16.4 New Reports

Returns recent community reports that have not yet been verified by an admin.

```
GET /admin/blacklist/reports
```

**Auth Required**: Yes (Bearer Token, Admin role)

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
        "phoneNumber": "+201099887766",
        "reportCount": 3,
        "reporters": [
          {
            "driverName": "أحمد سعيد",
            "reason": "محدش فتح الباب 3 مرات",
            "reportedAt": "2026-03-18T16:00:00Z"
          },
          {
            "driverName": "كريم محمود",
            "reason": "بيعمل طلبات وهمية",
            "reportedAt": "2026-03-19T11:00:00Z"
          },
          {
            "driverName": "محمد حسين",
            "reason": "رفض الدفع والتعامل سيء",
            "reportedAt": "2026-03-20T14:00:00Z"
          }
        ],
        "firstReportDate": "2026-03-18T16:00:00Z",
        "lastReportDate": "2026-03-20T14:00:00Z"
      }
    ],
    "totalCount": 4,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

## 17. Flutter/Dio Integration Examples

### Base Setup (Shared with Auth)

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Token interceptor (see AUTH_API.md for full setup)
dio.interceptors.add(AuthInterceptor());
```

### List Customers with Filters

```dart
Future<PagedResult<CustomerDto>> getCustomers({
  String? searchTerm,
  bool? isBlocked,
  double? minRating,
  String? sortBy,
  int pageNumber = 1,
  int pageSize = 10,
}) async {
  final response = await dio.get('/customers', queryParameters: {
    if (searchTerm != null) 'searchTerm': searchTerm,
    if (isBlocked != null) 'isBlocked': isBlocked,
    if (minRating != null) 'minRating': minRating,
    if (sortBy != null) 'sortBy': sortBy,
    'pageNumber': pageNumber,
    'pageSize': pageSize,
  });

  if (response.data['isSuccess'] == true) {
    return PagedResult.fromJson(
      response.data['data'],
      (json) => CustomerDto.fromJson(json),
    );
  }
  throw ApiException(response.data['message']);
}
```

### Rate a Customer

```dart
Future<void> rateCustomer(String customerId, CreateRatingDto rating) async {
  final response = await dio.post(
    '/customers/$customerId/rate',
    data: {
      'orderId': rating.orderId,
      'ratingValue': rating.ratingValue,
      'quickResponse': rating.quickResponse,
      'clearAddress': rating.clearAddress,
      'respectfulBehavior': rating.respectfulBehavior,
      'easyPayment': rating.easyPayment,
      'wrongAddress': rating.wrongAddress,
      'noAnswer': rating.noAnswer,
      'delayedPickup': rating.delayedPickup,
      'paymentIssue': rating.paymentIssue,
      'feedbackText': rating.feedbackText,
    },
  );

  if (response.data['isSuccess'] != true) {
    throw ApiException(response.data['message']);
  }
}
```

### Caller ID Lookup

```dart
Future<CallerIdDto?> lookupCaller(String phone) async {
  try {
    final response = await dio.get('/caller-id/lookup/$phone');
    if (response.data['isSuccess'] == true) {
      return CallerIdDto.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    if (e.response?.statusCode == 404) return null; // Unknown number
    rethrow;
  }
}
```

### Address Autocomplete

```dart
Future<List<AddressSuggestion>> autocomplete(
  String query, {
  double? latitude,
  double? longitude,
}) async {
  final response = await dio.get('/addresses/autocomplete', queryParameters: {
    'q': query,
    if (latitude != null) 'latitude': latitude,
    if (longitude != null) 'longitude': longitude,
  });

  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((json) => AddressSuggestion.fromJson(json))
        .toList();
  }
  return [];
}
```

### Upload Voice Memo (Multipart)

```dart
Future<void> uploadVoiceMemo(String customerId, File audioFile) async {
  final formData = FormData.fromMap({
    'file': await MultipartFile.fromFile(
      audioFile.path,
      filename: 'voice_memo.m4a',
      contentType: MediaType('audio', 'm4a'),
    ),
  });

  final response = await dio.post(
    '/customers/$customerId/voice-memo',
    data: formData,
    options: Options(contentType: 'multipart/form-data'),
  );

  if (response.data['isSuccess'] != true) {
    throw ApiException(response.data['message']);
  }
}
```

### Upload Partner Verification Document (Multipart)

```dart
Future<void> submitVerification(String partnerId, File document) async {
  final formData = FormData.fromMap({
    'file': await MultipartFile.fromFile(
      document.path,
      filename: 'verification_doc.pdf',
    ),
    'documentType': 'سجل تجاري',
  });

  final response = await dio.post(
    '/partners/$partnerId/submit-verification',
    data: formData,
    options: Options(contentType: 'multipart/form-data'),
  );

  if (response.data['isSuccess'] != true) {
    throw ApiException(response.data['message']);
  }
}
```

### Create Partner

```dart
Future<PartnerDto> createPartner(CreatePartnerDto dto) async {
  final response = await dio.post('/partners', data: {
    'name': dto.name,
    'partnerType': dto.partnerType,
    'phone': dto.phone,
    'address': dto.address,
    'commissionType': dto.commissionType,
    'commissionValue': dto.commissionValue,
    'defaultPaymentMethod': dto.defaultPaymentMethod,
    'color': dto.color,
    'receiptHeader': dto.receiptHeader,
  });

  if (response.data['isSuccess'] == true) {
    return PartnerDto.fromJson(response.data['data']);
  }
  throw ApiException(response.data['message']);
}
```

### Partner Dashboard (Partner Portal)

```dart
Future<PartnerDashboard> getPartnerDashboard() async {
  final response = await dio.get('/partner/dashboard');

  if (response.data['isSuccess'] == true) {
    return PartnerDashboard.fromJson(response.data['data']);
  }
  throw ApiException(response.data['message']);
}
```

### Nearby Addresses

```dart
Future<List<AddressDto>> getNearbyAddresses({
  required double latitude,
  required double longitude,
  double radiusKm = 2.0,
}) async {
  final response = await dio.get('/addresses/nearby', queryParameters: {
    'latitude': latitude,
    'longitude': longitude,
    'radiusKm': radiusKm,
  });

  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((json) => AddressDto.fromJson(json))
        .toList();
  }
  return [];
}
```

### Block Customer with Community Report

```dart
Future<void> blockCustomer(
  String customerId, {
  required String reason,
  bool reportToCommunity = false,
}) async {
  final response = await dio.post(
    '/customers/$customerId/block',
    data: {
      'reason': reason,
      'reportToCommunity': reportToCommunity,
    },
  );

  if (response.data['isSuccess'] != true) {
    throw ApiException(response.data['message']);
  }
}
```
