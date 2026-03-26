# Sekka API - Financial Module Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-26

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Cash Management Flow](#3-cash-management-flow)
4. [Settlement Workflow](#4-settlement-workflow)
5. [Payment Request Lifecycle](#5-payment-request-lifecycle)
6. [Endpoints](#6-endpoints)
   - [Wallet](#61-walletcontroller)
     - [Get Balance](#611-get-balance)
     - [Get Transactions](#612-get-transactions)
     - [Get Summary](#613-get-summary)
     - [Get Cash Status](#614-get-cash-status)
   - [Settlements](#62-settlementcontroller)
     - [Get Settlement History](#621-get-settlement-history)
     - [Create Settlement](#622-create-settlement)
     - [Get Partner Balance](#623-get-partner-balance)
     - [Get Daily Summary](#624-get-daily-summary)
     - [Upload Receipt](#625-upload-receipt)
   - [Statistics](#63-statisticscontroller)
     - [Daily Stats](#631-daily-stats)
     - [Weekly Stats](#632-weekly-stats)
     - [Monthly Stats](#633-monthly-stats)
     - [Heatmap Data](#634-heatmap-data)
     - [Export Reports](#635-export-reports)
   - [Payment Requests](#64-paymentrequestcontroller)
     - [Get My Requests](#641-get-my-requests)
     - [Get Request Details](#642-get-request-details)
     - [Create Payment Request](#643-create-payment-request)
     - [Upload Transfer Proof](#644-upload-transfer-proof)
     - [Cancel Request](#645-cancel-request)
   - [Invoices](#65-invoicecontroller)
     - [Get My Invoices](#651-get-my-invoices)
     - [Get Invoice Details](#652-get-invoice-details)
     - [Download PDF](#653-download-pdf)
     - [Get Invoice Summary](#654-get-invoice-summary)
   - [Analytics](#66-analyticscontroller)
     - [Source Breakdown](#661-source-breakdown)
     - [Customer Profitability](#662-customer-profitability)
     - [Region Analysis](#663-region-analysis)
     - [Time Analysis](#664-time-analysis)
     - [Cancellation Report](#665-cancellation-report)
     - [Profitability Trends](#666-profitability-trends)
7. [Admin Endpoints](#7-admin-endpoints)
   - [AdminWalletController](#71-adminwalletcontroller)
   - [AdminSettlementsController](#72-adminsettlementscontroller)
   - [AdminPaymentController](#73-adminpaymentcontroller)
   - [AdminStatisticsController](#74-adminstatisticscontroller)
   - [AdminDisputesController](#75-admindisputescontroller)
   - [AdminInvoiceController](#76-admininvoicecontroller)
   - [AdminRefundController](#77-adminrefundcontroller)
8. [Enums](#8-enums)
9. [Key DTOs Reference](#9-key-dtos-reference)
10. [Error Messages Reference](#10-error-messages-reference)
11. [Flutter Integration Examples](#11-flutter-integration-examples)

---

## 1. Overview

The Financial module handles all money-related operations for Sekka drivers: wallet balances, cash tracking, settlements with partners, subscription payments, invoicing, and performance analytics.

| Feature | Detail |
|---------|--------|
| Currency | EGP (Egyptian Pound) |
| Wallet Model | Real-time cash-on-hand tracking |
| Settlement Partners | Stores, restaurants, other drivers |
| Payment Methods | Vodafone Cash, Instapay, Fawry, Bank Transfer |
| Invoice Cycle | Weekly (auto-generated) |
| Export Formats | PDF, Excel |
| Cash Alert Levels | Safe / Warning / Danger / Critical |
| Analytics Window | Up to 90 days historical data |

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

### Paginated Response
```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 47,
    "totalPages": 5,
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
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 409 | Conflict |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Cash Management Flow

Drivers collect cash from customers on delivery. The system tracks how much cash a driver is holding and alerts them when they should settle.

```
                    ┌──────────────┐
                    │  Order       │
                    │  Delivered   │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │  Cash on    │
                    │  Hand += $  │
                    └──────┬──────┘
                           │
                    ┌──────▼──────────┐
                    │  Check Alert    │
                    │  Threshold      │
                    └──┬───┬───┬───┬──┘
                       │   │   │   │
                    Safe Warning Danger Critical
                     │     │     │      │
                     │     │     │   ┌──▼──────────────┐
                     │     │     │   │ MUST settle now  │
                     │     │     │   │ (account paused) │
                     │     │     │   └─────────────────┘
                     │     │  ┌──▼──────────────┐
                     │     │  │ Settlement       │
                     │     │  │ strongly advised  │
                     │     │  └─────────────────┘
                     │  ┌──▼──────────────┐
                     │  │ Consider settling│
                     │  └─────────────────┘
                  ┌──▼──────────────┐
                  │ All good        │
                  └─────────────────┘

Cash Alert Thresholds:
  Safe     = 0% - 50%  of threshold
  Warning  = 50% - 75% of threshold
  Danger   = 75% - 100% of threshold
  Critical = 100%+ of threshold
```

### Settlement Reduces Cash
```
Driver Cash On Hand: 2500 EGP
  - Settlement to Partner: -1500 EGP
  = New Cash On Hand: 1000 EGP
```

---

## 4. Settlement Workflow

```
┌────────────┐     ┌──────────────┐     ┌──────────────┐
│  Driver     │     │   System     │     │   Partner    │
│  creates    ├────►│   records    ├────►│   receives   │
│  settlement │     │   & updates  │     │   WhatsApp   │
└─────┬──────┘     │   wallet     │     │   (optional) │
      │            └──────┬───────┘     └──────────────┘
      │                   │
      │            ┌──────▼───────┐
      │            │  Cash on     │
      │            │  Hand -= $   │
      │            └──────┬───────┘
      │                   │
      ▼                   ▼
┌─────────────┐   ┌──────────────┐
│ Upload      │   │ Transaction  │
│ Receipt     │   │ logged in    │
│ (optional)  │   │ wallet       │
└─────────────┘   └──────────────┘
```

### Settlement Types
| Type | Description |
|------|-------------|
| CashToPartner | Cash handoff to partner/store |
| BankTransfer | Transfer to company bank account |
| VodafoneCash | Vodafone Cash mobile payment |
| Instapay | Instapay bank transfer |
| Fawry | Fawry payment kiosk |

---

## 5. Payment Request Lifecycle

Payment requests are used for subscription plan payments (manual transfer methods).

```
┌─────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  Driver      │     │   Pending    │     │   Under      │     │  Approved    │
│  creates    ├────►│   Awaiting   ├────►│   Review     ├────►│  Plan        │
│  request     │     │   proof      │     │   by admin   │     │  activated   │
└──────────────┘     └──────┬───────┘     └──────┬───────┘     └──────────────┘
                            │                    │
                     ┌──────▼───────┐     ┌──────▼───────┐
                     │  Driver      │     │   Rejected   │
                     │  uploads     │     │   (reason    │
                     │  proof image │     │   provided)  │
                     └──────────────┘     └──────────────┘
                                                 │
                                          ┌──────▼───────┐
                                          │  Driver can  │
                                          │  re-upload   │
                                          │  or cancel   │
                                          └──────────────┘

Statuses: Pending → UnderReview → Approved / Rejected → (Cancelled by driver)
```

---

## 6. Endpoints

All endpoints require `Authorization: Bearer <token>` unless noted otherwise.

---

### 6.1 WalletController

**Base Path**: `api/v1/wallet`

---

#### 6.1.1 Get Balance

Returns the driver's current wallet balance, cash on hand, and pending amounts.

```
GET /wallet/balance
```

**Auth Required**: Yes (Bearer Token)

**Headers**:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "cashOnHand": 1850.50,
    "cashAlertThreshold": 3000.00,
    "cashAlertPercentage": 61.68,
    "todayCollected": 750.00,
    "todayCommissions": 112.50,
    "pendingSettlements": 500.00,
    "totalBalance": 1350.50,
    "availableBalance": 850.50,
    "lastUpdatedAt": "2026-03-26T14:30:00Z"
  },
  "message": null,
  "errors": null
}
```

**WalletBalanceDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| cashOnHand | decimal | Total cash the driver is currently holding |
| cashAlertThreshold | decimal | Maximum cash threshold before alerts trigger |
| cashAlertPercentage | decimal | Current percentage of threshold (cashOnHand / threshold * 100) |
| todayCollected | decimal | Cash collected from deliveries today |
| todayCommissions | decimal | Commission fees deducted today |
| pendingSettlements | decimal | Settlements created but not yet confirmed |
| totalBalance | decimal | Net balance (cashOnHand - pendingSettlements) |
| availableBalance | decimal | Amount available for withdrawal |
| lastUpdatedAt | datetime | Last wallet update timestamp |

---

#### 6.1.2 Get Transactions

Returns paginated transaction history with filtering by type and date range.

```
GET /wallet/transactions?pageNumber=1&pageSize=10&type=0&dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | int | No | 1 | Page number |
| pageSize | int | No | 10 | Items per page (max 50) |
| type | int | No | null | Filter by TransactionType enum |
| dateFrom | date | No | null | Start date (inclusive) |
| dateTo | date | No | null | End date (inclusive) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "type": 0,
        "typeName": "OrderEarning",
        "typeNameAr": "ربح من طلب",
        "amount": 45.00,
        "balanceAfter": 1850.50,
        "description": "ربح من الطلب #ORD-20260326-001",
        "referenceId": "order-id-here",
        "createdAt": "2026-03-26T14:25:00Z"
      },
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "type": 3,
        "typeName": "Settlement",
        "typeNameAr": "تسوية",
        "amount": -1500.00,
        "balanceAfter": 1805.50,
        "description": "تسوية مع محل الشريك",
        "referenceId": "settlement-id-here",
        "createdAt": "2026-03-26T12:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 87,
    "totalPages": 9,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

**TransactionDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | guid | Transaction ID |
| type | int | TransactionType enum value |
| typeName | string | English type name |
| typeNameAr | string | Arabic type name |
| amount | decimal | Positive = credit, Negative = debit |
| balanceAfter | decimal | Wallet balance after this transaction |
| description | string | Human-readable description (Arabic) |
| referenceId | string | Related entity ID (order, settlement, etc.) |
| createdAt | datetime | Transaction timestamp |

---

#### 6.1.3 Get Summary

Returns wallet summary with income, expenses, and net profit for a period.

```
GET /wallet/summary?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Start of current month | Period start |
| dateTo | date | No | Today | Period end |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalIncome": 12500.00,
    "totalExpenses": 3750.00,
    "totalCommissions": 1875.00,
    "totalSettlements": 8500.00,
    "netProfit": 8750.00,
    "transactionCount": 156,
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "incomeBreakdown": {
      "orderEarnings": 11000.00,
      "tips": 1200.00,
      "bonuses": 300.00
    },
    "expenseBreakdown": {
      "commissions": 1875.00,
      "fines": 150.00,
      "subscriptionFees": 250.00,
      "other": 1475.00
    }
  },
  "message": null,
  "errors": null
}
```

---

#### 6.1.4 Get Cash Status

Returns the current cash alert level with a suggested action.

```
GET /wallet/cash-status
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "cashOnHand": 2400.00,
    "threshold": 3000.00,
    "percentage": 80.0,
    "alertLevel": "danger",
    "suggestedAction": "يجب تسوية المبلغ النقدي في أقرب وقت. قم بزيارة أقرب شريك لتسليم النقدي."
  },
  "message": null,
  "errors": null
}
```

**CashStatusDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| cashOnHand | decimal | Current cash on hand |
| threshold | decimal | Cash alert threshold |
| percentage | decimal | Percentage of threshold reached |
| alertLevel | string | `safe` / `warning` / `danger` / `critical` |
| suggestedAction | string | Arabic action suggestion for the driver |

**Alert Levels**:
| Level | Percentage | Meaning |
|-------|-----------|---------|
| safe | 0% - 50% | No action needed |
| warning | 50% - 75% | Consider settling soon |
| danger | 75% - 100% | Settle as soon as possible |
| critical | 100%+ | Account paused until settlement |

---

### 6.2 SettlementController

**Base Path**: `api/v1/settlements`

---

#### 6.2.1 Get Settlement History

Returns paginated settlement history.

```
GET /settlements?pageNumber=1&pageSize=10&dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | int | No | 1 | Page number |
| pageSize | int | No | 10 | Items per page (max 50) |
| dateFrom | date | No | null | Start date |
| dateTo | date | No | null | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "partnerId": "d4e5f6a7-b8c9-0123-def0-234567890123",
        "partnerName": "محل أبو علي",
        "amount": 1500.00,
        "settlementType": 0,
        "settlementTypeName": "CashToPartner",
        "notes": "تسليم نقدي يومي",
        "receiptImageUrl": "https://sekka.runasp.net/uploads/receipts/abc123.jpg",
        "whatsAppSent": true,
        "createdAt": "2026-03-26T12:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 23,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

#### 6.2.2 Create Settlement

Creates a new settlement record with a partner. Reduces cash on hand.

```
POST /settlements
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "partnerId": "d4e5f6a7-b8c9-0123-def0-234567890123",
  "amount": 1500.00,
  "settlementType": 0,
  "notes": "تسليم نقدي يومي",
  "sendWhatsApp": true
}
```

**CreateSettlementDto Fields**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| partnerId | guid | Yes | Partner to settle with |
| amount | decimal | Yes | Amount to settle (must be > 0 and <= cashOnHand) |
| settlementType | int | Yes | SettlementType enum value (0-4) |
| notes | string | No | Optional notes (max 500 chars) |
| sendWhatsApp | bool | Yes | Send WhatsApp notification to partner |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
    "partnerId": "d4e5f6a7-b8c9-0123-def0-234567890123",
    "partnerName": "محل أبو علي",
    "amount": 1500.00,
    "settlementType": 0,
    "settlementTypeName": "CashToPartner",
    "notes": "تسليم نقدي يومي",
    "receiptImageUrl": null,
    "whatsAppSent": true,
    "createdAt": "2026-03-26T14:30:00Z"
  },
  "message": "تم إنشاء التسوية بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `المبلغ يجب أن يكون أكبر من صفر` |
| 400 | `المبلغ أكبر من النقدي المتاح` |
| 404 | `الشريك غير موجود` |

---

#### 6.2.3 Get Partner Balance

Returns the outstanding balance with a specific partner.

```
GET /settlements/partner-balance/{partnerId}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `partnerId` (GUID) - Partner ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "partnerId": "d4e5f6a7-b8c9-0123-def0-234567890123",
    "partnerName": "محل أبو علي",
    "totalSettled": 15000.00,
    "pendingAmount": 2500.00,
    "lastSettlementAt": "2026-03-26T12:00:00Z",
    "settlementCount": 23
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

#### 6.2.4 Get Daily Summary

Returns today's settlement summary.

```
GET /settlements/daily-summary?date=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| date | date | No | Today | Date for the summary |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-26",
    "totalSettled": 3500.00,
    "settlementCount": 3,
    "byType": [
      { "type": 0, "typeName": "CashToPartner", "amount": 2000.00, "count": 2 },
      { "type": 2, "typeName": "VodafoneCash", "amount": 1500.00, "count": 1 }
    ],
    "topPartner": {
      "partnerId": "d4e5f6a7-b8c9-0123-def0-234567890123",
      "partnerName": "محل أبو علي",
      "amount": 2000.00
    }
  },
  "message": null,
  "errors": null
}
```

---

#### 6.2.5 Upload Receipt

Uploads a receipt image for an existing settlement.

```
POST /settlements/{id}/receipt
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Settlement ID

**Content-Type**: `multipart/form-data`

**Form Data**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| file | file | Yes | Receipt image (JPEG/PNG, max 5 MB) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "receiptImageUrl": "https://sekka.runasp.net/uploads/receipts/abc123.jpg"
  },
  "message": "تم رفع الإيصال بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الملف مطلوب` |
| 400 | `حجم الملف يتجاوز الحد الأقصى (5 ميجابايت)` |
| 400 | `نوع الملف غير مدعوم. الأنواع المسموح بها: JPEG, PNG` |
| 404 | `التسوية غير موجودة` |

---

### 6.3 StatisticsController

**Base Path**: `api/v1/statistics`

---

#### 6.3.1 Daily Stats

Returns detailed statistics for a specific day.

```
GET /statistics/daily?date=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| date | date | No | Today | The date to get stats for |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "date": "2026-03-26",
    "totalOrders": 18,
    "successfulOrders": 15,
    "failedOrders": 1,
    "cancelledOrders": 2,
    "earnings": 675.00,
    "commissions": 101.25,
    "expenses": 45.00,
    "netProfit": 528.75,
    "distanceKm": 87.3,
    "timeWorkedMinutes": 480,
    "successRate": 83.33,
    "averageOrderValue": 45.00,
    "averageDeliveryTimeMinutes": 28,
    "tips": 120.00,
    "peakHour": 14,
    "peakHourOrders": 5
  },
  "message": null,
  "errors": null
}
```

**DailyStatsDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| date | date | The stats date |
| totalOrders | int | Total orders received |
| successfulOrders | int | Successfully delivered orders |
| failedOrders | int | Failed delivery attempts |
| cancelledOrders | int | Cancelled orders |
| earnings | decimal | Total earnings (EGP) |
| commissions | decimal | Platform commission deducted |
| expenses | decimal | Expenses (fuel, etc.) |
| netProfit | decimal | Net profit (earnings - commissions - expenses) |
| distanceKm | decimal | Total distance traveled (km) |
| timeWorkedMinutes | int | Total active working time |
| successRate | decimal | Percentage of successful orders |
| averageOrderValue | decimal | Average earning per order |
| averageDeliveryTimeMinutes | int | Average time per delivery |
| tips | decimal | Total tips received |
| peakHour | int | Hour with most orders (0-23) |
| peakHourOrders | int | Number of orders in peak hour |

---

#### 6.3.2 Weekly Stats

Returns aggregated statistics for a week.

```
GET /statistics/weekly?weekStart=2026-03-20
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| weekStart | date | No | Current week start | Start of the week (Saturday) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "weekStart": "2026-03-20",
    "weekEnd": "2026-03-26",
    "totalOrders": 98,
    "successfulOrders": 89,
    "failedOrders": 3,
    "cancelledOrders": 6,
    "earnings": 4410.00,
    "commissions": 661.50,
    "expenses": 320.00,
    "netProfit": 3428.50,
    "distanceKm": 542.7,
    "timeWorkedMinutes": 2880,
    "successRate": 90.82,
    "averageOrderValue": 49.55,
    "dailyBreakdown": [
      { "date": "2026-03-20", "orders": 14, "earnings": 630.00 },
      { "date": "2026-03-21", "orders": 16, "earnings": 720.00 },
      { "date": "2026-03-22", "orders": 12, "earnings": 540.00 },
      { "date": "2026-03-23", "orders": 18, "earnings": 810.00 },
      { "date": "2026-03-24", "orders": 15, "earnings": 675.00 },
      { "date": "2026-03-25", "orders": 8, "earnings": 360.00 },
      { "date": "2026-03-26", "orders": 15, "earnings": 675.00 }
    ],
    "bestDay": "2026-03-23",
    "worstDay": "2026-03-25",
    "comparisonWithLastWeek": {
      "ordersChange": 12.5,
      "earningsChange": 8.3,
      "successRateChange": 2.1
    }
  },
  "message": null,
  "errors": null
}
```

---

#### 6.3.3 Monthly Stats

Returns aggregated statistics for a month.

```
GET /statistics/monthly?month=3&year=2026
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| month | int | No | Current month | Month number (1-12) |
| year | int | No | Current year | Year (e.g., 2026) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "month": 3,
    "year": 2026,
    "totalOrders": 412,
    "successfulOrders": 378,
    "failedOrders": 12,
    "cancelledOrders": 22,
    "earnings": 18540.00,
    "commissions": 2781.00,
    "expenses": 1250.00,
    "netProfit": 14509.00,
    "distanceKm": 2340.5,
    "timeWorkedMinutes": 12480,
    "successRate": 91.75,
    "averageOrderValue": 49.05,
    "averageDailyOrders": 15.85,
    "averageDailyEarnings": 713.08,
    "weeklyBreakdown": [
      { "weekStart": "2026-03-01", "orders": 95, "earnings": 4275.00 },
      { "weekStart": "2026-03-08", "orders": 108, "earnings": 4860.00 },
      { "weekStart": "2026-03-15", "orders": 111, "earnings": 4995.00 },
      { "weekStart": "2026-03-22", "orders": 98, "earnings": 4410.00 }
    ],
    "comparisonWithLastMonth": {
      "ordersChange": 5.6,
      "earningsChange": 7.2,
      "successRateChange": 1.3
    }
  },
  "message": null,
  "errors": null
}
```

---

#### 6.3.4 Heatmap Data

Returns order density data for heatmap visualization on a map.

```
GET /statistics/heatmap?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | Yes | - | Start date |
| dateTo | date | Yes | - | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "totalPoints": 412,
    "points": [
      {
        "latitude": 30.0444,
        "longitude": 31.2357,
        "weight": 15,
        "areaName": "وسط البلد"
      },
      {
        "latitude": 30.0626,
        "longitude": 31.2497,
        "weight": 22,
        "areaName": "مصر الجديدة"
      },
      {
        "latitude": 30.0131,
        "longitude": 31.2089,
        "weight": 8,
        "areaName": "المعادي"
      }
    ],
    "hotZones": [
      { "areaName": "مصر الجديدة", "orderCount": 22, "averageEarning": 52.30 },
      { "areaName": "وسط البلد", "orderCount": 15, "averageEarning": 38.50 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

#### 6.3.5 Export Reports

Exports statistics as a downloadable file (PDF or Excel).

```
GET /statistics/export?format=pdf&dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| format | string | No | pdf | Export format: `pdf` or `excel` |
| dateFrom | date | Yes | - | Start date |
| dateTo | date | Yes | - | End date |

**Success Response** `200 OK`:

Returns a file download response.

```
Content-Type: application/pdf (or application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)
Content-Disposition: attachment; filename="statistics_2026-03-01_2026-03-26.pdf"
```

> **Note**: This endpoint returns a binary file, not a JSON response. Use Dio's `responseType: ResponseType.bytes` for downloading.

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `تاريخ البداية مطلوب` |
| 400 | `تاريخ النهاية مطلوب` |
| 400 | `الفترة يجب ألا تتجاوز 90 يوم` |

---

### 6.4 PaymentRequestController

**Base Path**: `api/v1/payment-requests`

---

#### 6.4.1 Get My Requests

Returns the driver's payment requests (for subscription payments).

```
GET /payment-requests?pageNumber=1&pageSize=10&status=0
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | int | No | 1 | Page number |
| pageSize | int | No | 10 | Items per page (max 50) |
| status | int | No | null | Filter by PaymentRequestStatus |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "e5f6a7b8-c9d0-1234-ef01-345678901234",
        "planId": "f6a7b8c9-d0e1-2345-f012-456789012345",
        "planName": "الباقة الشهرية - بريميوم",
        "amount": 299.00,
        "paymentMethod": 0,
        "paymentMethodName": "VodafoneCash",
        "senderPhone": "01012345678",
        "senderName": "أحمد محمد",
        "notes": null,
        "status": 0,
        "statusName": "Pending",
        "statusNameAr": "قيد الانتظار",
        "proofImageUrl": null,
        "rejectionReason": null,
        "reviewedAt": null,
        "createdAt": "2026-03-26T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 5,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

#### 6.4.2 Get Request Details

Returns details of a specific payment request.

```
GET /payment-requests/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Payment Request ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-345678901234",
    "planId": "f6a7b8c9-d0e1-2345-f012-456789012345",
    "planName": "الباقة الشهرية - بريميوم",
    "planDescription": "باقة بريميوم مع عمولة مخفضة 10%",
    "amount": 299.00,
    "paymentMethod": 0,
    "paymentMethodName": "VodafoneCash",
    "senderPhone": "01012345678",
    "senderName": "أحمد محمد",
    "notes": null,
    "status": 0,
    "statusName": "Pending",
    "statusNameAr": "قيد الانتظار",
    "proofImageUrl": null,
    "rejectionReason": null,
    "reviewedBy": null,
    "reviewedAt": null,
    "createdAt": "2026-03-26T10:00:00Z",
    "updatedAt": "2026-03-26T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `طلب الدفع غير موجود` |

---

#### 6.4.3 Create Payment Request

Creates a new payment request for a subscription plan.

```
POST /payment-requests
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "planId": "f6a7b8c9-d0e1-2345-f012-456789012345",
  "paymentMethod": 0,
  "senderPhone": "01012345678",
  "senderName": "أحمد محمد",
  "notes": "تم التحويل من فودافون كاش"
}
```

**CreatePaymentRequestDto Fields**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| planId | guid | Yes | Subscription plan ID |
| paymentMethod | int | Yes | ManualPaymentMethod enum (0-3) |
| senderPhone | string | No | Phone used for the transfer |
| senderName | string | No | Name of the sender on the transfer |
| notes | string | No | Additional notes (max 500 chars) |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-c9d0-1234-ef01-345678901234",
    "planName": "الباقة الشهرية - بريميوم",
    "amount": 299.00,
    "paymentMethod": 0,
    "paymentMethodName": "VodafoneCash",
    "status": 0,
    "statusNameAr": "قيد الانتظار",
    "createdAt": "2026-03-26T10:00:00Z"
  },
  "message": "تم إنشاء طلب الدفع بنجاح. قم برفع إثبات التحويل.",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الباقة غير موجودة` |
| 400 | `طريقة الدفع غير صالحة` |
| 409 | `يوجد طلب دفع معلق بالفعل لهذه الباقة` |

---

#### 6.4.4 Upload Transfer Proof

Uploads a screenshot/image proving the payment transfer.

```
POST /payment-requests/{id}/upload-proof
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Payment Request ID

**Content-Type**: `multipart/form-data`

**Form Data**:
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| file | file | Yes | Proof image (JPEG/PNG, max 5 MB) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "proofImageUrl": "https://sekka.runasp.net/uploads/payment-proofs/xyz789.jpg",
    "status": 1,
    "statusNameAr": "قيد المراجعة"
  },
  "message": "تم رفع إثبات التحويل بنجاح. سيتم مراجعة طلبك قريباً.",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الملف مطلوب` |
| 400 | `حجم الملف يتجاوز الحد الأقصى (5 ميجابايت)` |
| 400 | `نوع الملف غير مدعوم. الأنواع المسموح بها: JPEG, PNG` |
| 400 | `لا يمكن رفع إثبات لطلب في حالة: مقبول` |
| 404 | `طلب الدفع غير موجود` |

---

#### 6.4.5 Cancel Request

Cancels a pending payment request.

```
DELETE /payment-requests/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Payment Request ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء طلب الدفع بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن إلغاء طلب في حالة: مقبول` |
| 404 | `طلب الدفع غير موجود` |

---

### 6.5 InvoiceController

**Base Path**: `api/v1/invoices`

---

#### 6.5.1 Get My Invoices

Returns the driver's invoices.

```
GET /invoices?pageNumber=1&pageSize=10&status=0
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | int | No | 1 | Page number |
| pageSize | int | No | 10 | Items per page (max 50) |
| status | int | No | null | Filter by InvoiceStatus enum |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-111111111111",
        "invoiceNumber": "INV-2026-0342",
        "periodStart": "2026-03-15",
        "periodEnd": "2026-03-21",
        "totalOrders": 98,
        "totalEarnings": 4410.00,
        "totalCommissions": 661.50,
        "totalExpenses": 320.00,
        "netAmount": 3428.50,
        "status": 0,
        "statusName": "Pending",
        "statusNameAr": "معلقة",
        "issuedAt": "2026-03-22T00:00:00Z",
        "dueDate": "2026-03-29",
        "paidAt": null
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 12,
    "totalPages": 2,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "message": null,
  "errors": null
}
```

---

#### 6.5.2 Get Invoice Details

Returns detailed information about a specific invoice.

```
GET /invoices/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Invoice ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-111111111111",
    "invoiceNumber": "INV-2026-0342",
    "periodStart": "2026-03-15",
    "periodEnd": "2026-03-21",
    "totalOrders": 98,
    "totalEarnings": 4410.00,
    "totalCommissions": 661.50,
    "totalExpenses": 320.00,
    "netAmount": 3428.50,
    "status": 0,
    "statusName": "Pending",
    "statusNameAr": "معلقة",
    "issuedAt": "2026-03-22T00:00:00Z",
    "dueDate": "2026-03-29",
    "paidAt": null,
    "lineItems": [
      {
        "description": "أرباح التوصيل",
        "quantity": 98,
        "unitPrice": 45.00,
        "total": 4410.00,
        "type": "earning"
      },
      {
        "description": "عمولة المنصة (15%)",
        "quantity": 1,
        "unitPrice": -661.50,
        "total": -661.50,
        "type": "commission"
      },
      {
        "description": "مصاريف تشغيلية",
        "quantity": 1,
        "unitPrice": -320.00,
        "total": -320.00,
        "type": "expense"
      }
    ],
    "settlements": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "amount": 1500.00,
        "date": "2026-03-18T12:00:00Z",
        "partnerName": "محل أبو علي"
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
| 404 | `الفاتورة غير موجودة` |

---

#### 6.5.3 Download PDF

Downloads the invoice as a PDF file.

```
GET /invoices/{id}/pdf
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Invoice ID

**Success Response** `200 OK`:

Returns a PDF file download.

```
Content-Type: application/pdf
Content-Disposition: attachment; filename="INV-2026-0342.pdf"
```

> **Note**: This endpoint returns a binary file. Use Dio's `responseType: ResponseType.bytes` for downloading.

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الفاتورة غير موجودة` |

---

#### 6.5.4 Get Invoice Summary

Returns an overall summary of invoices.

```
GET /invoices/summary
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalInvoices": 12,
    "pendingInvoices": 1,
    "paidInvoices": 10,
    "overdueInvoices": 1,
    "totalEarnings": 52800.00,
    "totalCommissions": 7920.00,
    "totalNetAmount": 44880.00,
    "totalPaid": 41451.50,
    "totalOutstanding": 3428.50,
    "averageInvoiceAmount": 4400.00
  },
  "message": null,
  "errors": null
}
```

---

### 6.6 AnalyticsController

**Base Path**: `api/v1/analytics`

---

#### 6.6.1 Source Breakdown

Returns order source analysis (where orders come from).

```
GET /analytics/source-breakdown?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "totalOrders": 412,
    "sources": [
      {
        "source": "App",
        "sourceAr": "التطبيق",
        "orderCount": 245,
        "percentage": 59.47,
        "totalEarnings": 11025.00,
        "averageOrderValue": 45.00
      },
      {
        "source": "Partner",
        "sourceAr": "الشريك",
        "orderCount": 120,
        "percentage": 29.13,
        "totalEarnings": 5400.00,
        "averageOrderValue": 45.00
      },
      {
        "source": "WhatsApp",
        "sourceAr": "واتساب",
        "orderCount": 47,
        "percentage": 11.41,
        "totalEarnings": 2115.00,
        "averageOrderValue": 45.00
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

#### 6.6.2 Customer Profitability

Returns profitability analysis by customer.

```
GET /analytics/customer-profitability?dateFrom=2026-03-01&dateTo=2026-03-26&top=10
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |
| top | int | No | 10 | Number of top customers to return |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "customers": [
      {
        "customerId": "cust-id-1",
        "customerName": "مطعم الشرقاوي",
        "orderCount": 45,
        "totalRevenue": 2025.00,
        "totalCommissions": 303.75,
        "netProfit": 1721.25,
        "averageOrderValue": 45.00,
        "cancellationRate": 2.2,
        "profitabilityScore": 95.5
      },
      {
        "customerId": "cust-id-2",
        "customerName": "صيدلية العزبي",
        "orderCount": 32,
        "totalRevenue": 1440.00,
        "totalCommissions": 216.00,
        "netProfit": 1224.00,
        "averageOrderValue": 45.00,
        "cancellationRate": 0.0,
        "profitabilityScore": 92.3
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

#### 6.6.3 Region Analysis

Returns performance analysis by region/area.

```
GET /analytics/region-analysis?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "regions": [
      {
        "regionName": "مصر الجديدة",
        "orderCount": 85,
        "totalEarnings": 4250.00,
        "averageOrderValue": 50.00,
        "successRate": 94.1,
        "averageDeliveryTimeMinutes": 25,
        "distanceKm": 340.5,
        "demandLevel": "high"
      },
      {
        "regionName": "وسط البلد",
        "orderCount": 72,
        "totalEarnings": 3240.00,
        "averageOrderValue": 45.00,
        "successRate": 88.9,
        "averageDeliveryTimeMinutes": 35,
        "distanceKm": 288.0,
        "demandLevel": "high"
      },
      {
        "regionName": "المعادي",
        "orderCount": 45,
        "totalEarnings": 2250.00,
        "averageOrderValue": 50.00,
        "successRate": 95.6,
        "averageDeliveryTimeMinutes": 22,
        "distanceKm": 180.0,
        "demandLevel": "medium"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

#### 6.6.4 Time Analysis

Returns analysis of best working hours and days.

```
GET /analytics/time-analysis?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "bestHours": [
      { "hour": 13, "label": "1:00 PM", "orderCount": 52, "averageEarning": 48.50 },
      { "hour": 14, "label": "2:00 PM", "orderCount": 48, "averageEarning": 47.00 },
      { "hour": 19, "label": "7:00 PM", "orderCount": 45, "averageEarning": 52.00 },
      { "hour": 20, "label": "8:00 PM", "orderCount": 43, "averageEarning": 50.50 }
    ],
    "bestDays": [
      { "dayOfWeek": 4, "dayName": "Thursday", "dayNameAr": "الخميس", "orderCount": 78, "averageEarning": 3510.00 },
      { "dayOfWeek": 5, "dayName": "Friday", "dayNameAr": "الجمعة", "orderCount": 72, "averageEarning": 3240.00 }
    ],
    "hourlyDistribution": [
      { "hour": 0, "orderCount": 2 },
      { "hour": 1, "orderCount": 0 },
      { "hour": 8, "orderCount": 12 },
      { "hour": 9, "orderCount": 18 },
      { "hour": 10, "orderCount": 22 },
      { "hour": 11, "orderCount": 28 },
      { "hour": 12, "orderCount": 38 },
      { "hour": 13, "orderCount": 52 },
      { "hour": 14, "orderCount": 48 },
      { "hour": 15, "orderCount": 32 },
      { "hour": 16, "orderCount": 25 },
      { "hour": 17, "orderCount": 30 },
      { "hour": 18, "orderCount": 38 },
      { "hour": 19, "orderCount": 45 },
      { "hour": 20, "orderCount": 43 },
      { "hour": 21, "orderCount": 35 },
      { "hour": 22, "orderCount": 20 },
      { "hour": 23, "orderCount": 8 }
    ],
    "peakPeriods": [
      { "label": "وقت الغداء", "start": "12:00", "end": "15:00", "percentage": 30.1 },
      { "label": "وقت العشاء", "start": "19:00", "end": "22:00", "percentage": 29.5 }
    ]
  },
  "message": null,
  "errors": null
}
```

---

#### 6.6.5 Cancellation Report

Returns analysis of order cancellations.

```
GET /analytics/cancellation-report?dateFrom=2026-03-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "totalOrders": 412,
    "cancelledOrders": 22,
    "cancellationRate": 5.34,
    "estimatedLostRevenue": 990.00,
    "byReason": [
      { "reason": "CustomerCancelled", "reasonAr": "إلغاء العميل", "count": 10, "percentage": 45.45 },
      { "reason": "DriverCancelled", "reasonAr": "إلغاء السائق", "count": 5, "percentage": 22.73 },
      { "reason": "NoResponse", "reasonAr": "عدم الرد", "count": 4, "percentage": 18.18 },
      { "reason": "AddressIssue", "reasonAr": "مشكلة في العنوان", "count": 3, "percentage": 13.64 }
    ],
    "trend": [
      { "date": "2026-03-01", "cancellationRate": 6.2 },
      { "date": "2026-03-08", "cancellationRate": 5.8 },
      { "date": "2026-03-15", "cancellationRate": 4.9 },
      { "date": "2026-03-22", "cancellationRate": 4.5 }
    ],
    "comparisonWithLastPeriod": {
      "previousRate": 6.1,
      "change": -0.76,
      "improved": true
    }
  },
  "message": null,
  "errors": null
}
```

---

#### 6.6.6 Profitability Trends

Returns profit trend analysis over time.

```
GET /analytics/profitability-trends?dateFrom=2026-03-01&dateTo=2026-03-26&interval=weekly
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| dateFrom | date | No | Last 30 days | Start date |
| dateTo | date | No | Today | End date |
| interval | string | No | weekly | Grouping: `daily`, `weekly`, `monthly` |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "dateFrom": "2026-03-01",
    "dateTo": "2026-03-26",
    "interval": "weekly",
    "overallProfitMargin": 78.27,
    "trends": [
      {
        "periodStart": "2026-03-01",
        "periodEnd": "2026-03-07",
        "revenue": 4275.00,
        "commissions": 641.25,
        "expenses": 280.00,
        "netProfit": 3353.75,
        "profitMargin": 78.45,
        "orderCount": 95
      },
      {
        "periodStart": "2026-03-08",
        "periodEnd": "2026-03-14",
        "revenue": 4860.00,
        "commissions": 729.00,
        "expenses": 310.00,
        "netProfit": 3821.00,
        "profitMargin": 78.62,
        "orderCount": 108
      },
      {
        "periodStart": "2026-03-15",
        "periodEnd": "2026-03-21",
        "revenue": 4995.00,
        "commissions": 749.25,
        "expenses": 340.00,
        "netProfit": 3905.75,
        "profitMargin": 78.19,
        "orderCount": 111
      },
      {
        "periodStart": "2026-03-22",
        "periodEnd": "2026-03-26",
        "revenue": 4410.00,
        "commissions": 661.50,
        "expenses": 320.00,
        "netProfit": 3428.50,
        "profitMargin": 77.74,
        "orderCount": 98
      }
    ],
    "projectedMonthEnd": {
      "estimatedRevenue": 19800.00,
      "estimatedNetProfit": 15500.00,
      "estimatedProfitMargin": 78.28
    }
  },
  "message": null,
  "errors": null
}
```

---

## 7. Admin Endpoints

> **Note**: Admin endpoints require an admin-role Bearer token. These are summarized here for reference; the Flutter driver app does not call these directly.

---

### 7.1 AdminWalletController

**Base Path**: `api/v1/admin/wallets`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/wallets` | List all driver wallets (paged, searchable) |
| GET | `/admin/wallets/{driverId}` | Get specific driver wallet details |
| POST | `/admin/wallets/{driverId}/adjust` | Manual wallet adjustment (credit/debit) |
| GET | `/admin/wallets/{driverId}/transactions` | Get driver transaction history |
| PUT | `/admin/wallets/{driverId}/threshold` | Update driver cash alert threshold |
| POST | `/admin/wallets/{driverId}/freeze` | Freeze driver wallet |
| POST | `/admin/wallets/{driverId}/unfreeze` | Unfreeze driver wallet |
| GET | `/admin/wallets/alerts` | Get all drivers with critical cash levels |
| GET | `/admin/wallets/summary` | Platform-wide wallet statistics |

**Wallet Adjustment Request**:
```json
{
  "amount": 100.00,
  "adjustmentType": 0,
  "reason": "تعويض عن مشكلة تقنية",
  "notes": "Ticket #1234"
}
```

---

### 7.2 AdminSettlementsController

**Base Path**: `api/v1/admin/settlements`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/settlements` | List all settlements (paged, filtered) |
| GET | `/admin/settlements/{id}` | Get settlement details |
| PUT | `/admin/settlements/{id}/confirm` | Confirm a settlement |
| PUT | `/admin/settlements/{id}/reject` | Reject a settlement |
| GET | `/admin/settlements/pending` | Get all pending settlements |
| GET | `/admin/settlements/daily-report` | Daily settlement report |
| GET | `/admin/settlements/partner-report/{partnerId}` | Partner settlement report |
| POST | `/admin/settlements/bulk-confirm` | Bulk confirm settlements |
| GET | `/admin/settlements/export` | Export settlements report |

---

### 7.3 AdminPaymentController

**Base Path**: `api/v1/admin/payments`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/payments/requests` | List all payment requests (paged, filtered) |
| GET | `/admin/payments/requests/{id}` | Get payment request details |
| POST | `/admin/payments/requests/{id}/approve` | Approve payment request |
| POST | `/admin/payments/requests/{id}/reject` | Reject payment request (with reason) |
| GET | `/admin/payments/requests/pending` | Get all pending requests |
| GET | `/admin/payments/summary` | Payment statistics summary |
| GET | `/admin/payments/export` | Export payment report |

**Reject Payment Request**:
```json
{
  "rejectionReason": "صورة الإثبات غير واضحة. برجاء إعادة الرفع."
}
```

---

### 7.4 AdminStatisticsController

**Base Path**: `api/v1/admin/statistics`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/statistics/overview` | Platform overview dashboard |
| GET | `/admin/statistics/drivers` | Driver performance rankings |
| GET | `/admin/statistics/revenue` | Revenue statistics |
| GET | `/admin/statistics/orders` | Order statistics |
| GET | `/admin/statistics/regions` | Region performance comparison |
| GET | `/admin/statistics/trends` | Platform growth trends |
| GET | `/admin/statistics/export` | Export admin reports |

---

### 7.5 AdminDisputesController

**Base Path**: `api/v1/admin/disputes`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/disputes` | List all disputes (paged, filtered) |
| GET | `/admin/disputes/{id}` | Get dispute details |
| POST | `/admin/disputes` | Create dispute on behalf of driver |
| PUT | `/admin/disputes/{id}/assign` | Assign dispute to admin |
| PUT | `/admin/disputes/{id}/resolve` | Resolve dispute |
| PUT | `/admin/disputes/{id}/escalate` | Escalate dispute |
| GET | `/admin/disputes/pending` | Get unresolved disputes |
| GET | `/admin/disputes/statistics` | Dispute statistics |
| POST | `/admin/disputes/{id}/comment` | Add comment to dispute |
| GET | `/admin/disputes/export` | Export disputes report |

**Resolve Dispute Request**:
```json
{
  "resolution": "تم التعويض بمبلغ 50 جنيه",
  "refundAmount": 50.00,
  "adjustWallet": true
}
```

---

### 7.6 AdminInvoiceController

**Base Path**: `api/v1/admin/invoices`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/invoices` | List all invoices (paged, filtered) |
| GET | `/admin/invoices/{id}` | Get invoice details |
| POST | `/admin/invoices/generate` | Manually generate invoices for a period |
| PUT | `/admin/invoices/{id}/mark-paid` | Mark invoice as paid |
| PUT | `/admin/invoices/{id}/void` | Void an invoice |
| GET | `/admin/invoices/overdue` | Get overdue invoices |
| GET | `/admin/invoices/summary` | Invoice statistics |
| GET | `/admin/invoices/export` | Export invoices report |

---

### 7.7 AdminRefundController

**Base Path**: `api/v1/admin/refunds`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/refunds` | List all refunds (paged, filtered) |
| GET | `/admin/refunds/{id}` | Get refund details |
| POST | `/admin/refunds` | Create a refund |
| POST | `/admin/refunds/{id}/approve` | Approve refund |
| POST | `/admin/refunds/{id}/reject` | Reject refund |
| GET | `/admin/refunds/pending` | Get pending refunds |
| GET | `/admin/refunds/summary` | Refund statistics |
| GET | `/admin/refunds/export` | Export refunds report |

**Create Refund Request**:
```json
{
  "driverId": "driver-guid-here",
  "orderId": "order-guid-here",
  "amount": 75.00,
  "reason": 0,
  "notes": "العميل لم يستلم الطلب"
}
```

---

## 8. Enums

### TransactionType
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | OrderEarning | ربح من طلب | Earning from a delivered order |
| 1 | Commission | عمولة | Platform commission deduction |
| 2 | Tip | بقشيش | Tip from customer |
| 3 | Settlement | تسوية | Cash settlement with partner |
| 4 | Bonus | مكافأة | Bonus/incentive payment |
| 5 | Fine | غرامة | Penalty/fine deduction |
| 6 | Refund | استرداد | Refund to/from customer |
| 7 | SubscriptionFee | رسوم اشتراك | Subscription plan payment |
| 8 | Adjustment | تعديل | Manual admin adjustment |
| 9 | Expense | مصروف | Recorded expense |

### SettlementType
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | CashToPartner | نقدي للشريك | Cash handoff to partner |
| 1 | BankTransfer | تحويل بنكي | Bank transfer |
| 2 | VodafoneCash | فودافون كاش | Vodafone Cash mobile payment |
| 3 | Instapay | انستاباي | Instapay transfer |
| 4 | Fawry | فوري | Fawry payment kiosk |

### ExpenseType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Fuel | وقود |
| 1 | Maintenance | صيانة |
| 2 | Parking | انتظار |
| 3 | Toll | رسوم طريق |
| 4 | Food | طعام |
| 5 | Phone | تليفون |
| 6 | Other | أخرى |

### ManualPaymentMethod
| Value | Name | Arabic |
|-------|------|--------|
| 0 | VodafoneCash | فودافون كاش |
| 1 | Instapay | انستاباي |
| 2 | Fawry | فوري |
| 3 | BankTransfer | تحويل بنكي |

### PaymentRequestStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | UnderReview | قيد المراجعة |
| 2 | Approved | مقبول |
| 3 | Rejected | مرفوض |
| 4 | Cancelled | ملغي |

### DisputeType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | OrderIssue | مشكلة في الطلب |
| 1 | PaymentIssue | مشكلة في الدفع |
| 2 | SettlementIssue | مشكلة في التسوية |
| 3 | CommissionDispute | اعتراض على العمولة |
| 4 | FineDispute | اعتراض على الغرامة |
| 5 | Other | أخرى |

### DisputeStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Open | مفتوح |
| 1 | Assigned | معين |
| 2 | UnderReview | قيد المراجعة |
| 3 | Resolved | تم الحل |
| 4 | Escalated | تم التصعيد |
| 5 | Closed | مغلق |

### InvoiceStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | معلقة |
| 1 | Paid | مدفوعة |
| 2 | Overdue | متأخرة |
| 3 | Voided | ملغاة |

### RefundReason
| Value | Name | Arabic |
|-------|------|--------|
| 0 | OrderNotDelivered | الطلب لم يتم توصيله |
| 1 | WrongOrder | طلب خاطئ |
| 2 | DamagedOrder | طلب تالف |
| 3 | OverCharged | مبلغ زائد |
| 4 | SystemError | خطأ في النظام |
| 5 | CustomerComplaint | شكوى عميل |
| 6 | Other | أخرى |

### RefundStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | قيد الانتظار |
| 1 | Approved | مقبول |
| 2 | Rejected | مرفوض |
| 3 | Processed | تم التنفيذ |

### WalletAdjustmentType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Credit | إضافة |
| 1 | Debit | خصم |

---

## 9. Key DTOs Reference

### WalletBalanceDto
```typescript
{
  cashOnHand: number;           // Current cash driver is holding
  cashAlertThreshold: number;   // Threshold before alerts trigger
  cashAlertPercentage: number;  // % of threshold reached
  todayCollected: number;       // Cash collected today from deliveries
  todayCommissions: number;     // Commission fees today
  pendingSettlements: number;   // Settlements awaiting confirmation
  totalBalance: number;         // Net balance
  availableBalance: number;     // Available for withdrawal
  lastUpdatedAt: string;        // ISO 8601 datetime
}
```

### CashStatusDto
```typescript
{
  cashOnHand: number;       // Current cash
  threshold: number;        // Alert threshold
  percentage: number;       // % of threshold
  alertLevel: string;       // "safe" | "warning" | "danger" | "critical"
  suggestedAction: string;  // Arabic action text
}
```

### CreateSettlementDto
```typescript
{
  partnerId: string;        // GUID
  amount: number;           // Must be > 0 and <= cashOnHand
  settlementType: number;   // SettlementType enum (0-4)
  notes?: string;           // Optional, max 500 chars
  sendWhatsApp: boolean;    // Notify partner via WhatsApp
}
```

### DailyStatsDto
```typescript
{
  date: string;                    // "YYYY-MM-DD"
  totalOrders: number;
  successfulOrders: number;
  failedOrders: number;
  cancelledOrders: number;
  earnings: number;                // Total earnings (EGP)
  commissions: number;            // Commission deducted
  expenses: number;               // Tracked expenses
  netProfit: number;              // earnings - commissions - expenses
  distanceKm: number;            // Total km driven
  timeWorkedMinutes: number;     // Active working time
  successRate: number;           // % successful
  averageOrderValue: number;
  averageDeliveryTimeMinutes: number;
  tips: number;
  peakHour: number;              // 0-23
  peakHourOrders: number;
}
```

### CreatePaymentRequestDto
```typescript
{
  planId: string;               // GUID — subscription plan
  paymentMethod: number;        // ManualPaymentMethod enum (0-3)
  senderPhone?: string;         // Phone used for transfer
  senderName?: string;          // Name on the transfer
  notes?: string;               // Max 500 chars
}
```

### InvoiceDto
```typescript
{
  id: string;                   // GUID
  invoiceNumber: string;        // e.g., "INV-2026-0342"
  periodStart: string;          // "YYYY-MM-DD"
  periodEnd: string;            // "YYYY-MM-DD"
  totalOrders: number;
  totalEarnings: number;
  totalCommissions: number;
  totalExpenses: number;
  netAmount: number;
  status: number;               // InvoiceStatus enum
  statusName: string;
  statusNameAr: string;
  issuedAt: string;             // ISO 8601 datetime
  dueDate: string;              // "YYYY-MM-DD"
  paidAt?: string;              // ISO 8601 datetime (null if unpaid)
}
```

### TransactionDto
```typescript
{
  id: string;                   // GUID
  type: number;                 // TransactionType enum
  typeName: string;             // English name
  typeNameAr: string;           // Arabic name
  amount: number;               // Positive = credit, Negative = debit
  balanceAfter: number;         // Balance after this transaction
  description: string;          // Arabic description
  referenceId: string;          // Related entity ID
  createdAt: string;            // ISO 8601 datetime
}
```

---

## 10. Error Messages Reference

### Wallet Errors
| Message | When |
|---------|------|
| `المحفظة غير موجودة` | Wallet not found |
| `المحفظة مجمدة` | Wallet is frozen |
| `الرصيد غير كافي` | Insufficient balance |

### Settlement Errors
| Message | When |
|---------|------|
| `المبلغ يجب أن يكون أكبر من صفر` | Amount is zero or negative |
| `المبلغ أكبر من النقدي المتاح` | Amount exceeds cash on hand |
| `الشريك غير موجود` | Partner not found |
| `التسوية غير موجودة` | Settlement not found |

### Payment Request Errors
| Message | When |
|---------|------|
| `طلب الدفع غير موجود` | Payment request not found |
| `الباقة غير موجودة` | Subscription plan not found |
| `طريقة الدفع غير صالحة` | Invalid payment method |
| `يوجد طلب دفع معلق بالفعل لهذه الباقة` | Duplicate pending request |
| `لا يمكن إلغاء طلب في حالة: {status}` | Cannot cancel non-pending request |
| `لا يمكن رفع إثبات لطلب في حالة: {status}` | Cannot upload proof for non-pending |

### Invoice Errors
| Message | When |
|---------|------|
| `الفاتورة غير موجودة` | Invoice not found |

### File Upload Errors
| Message | When |
|---------|------|
| `الملف مطلوب` | No file provided |
| `حجم الملف يتجاوز الحد الأقصى (5 ميجابايت)` | File exceeds 5 MB |
| `نوع الملف غير مدعوم. الأنواع المسموح بها: JPEG, PNG` | Invalid file type |

### Validation Errors
| Message | When |
|---------|------|
| `{field} مطلوب` | Required field is empty |
| `{field} يجب ألا يتجاوز {n} حرف` | Exceeds max length |
| `التاريخ غير صالح` | Invalid date format |
| `الفترة يجب ألا تتجاوز 90 يوم` | Date range exceeds 90 days |

---

## 11. Flutter Integration Examples

### Dio Setup (shared with Auth)

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
    } else {
      handler.next(error);
    }
  },
));
```

### Get Wallet Balance

```dart
Future<WalletBalance?> getWalletBalance() async {
  try {
    final response = await dio.get('/wallet/balance');
    if (response.data['isSuccess'] == true) {
      return WalletBalance.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Get Cash Status with Alert Handling

```dart
Future<void> checkCashStatus() async {
  try {
    final response = await dio.get('/wallet/cash-status');
    if (response.data['isSuccess'] == true) {
      final status = CashStatus.fromJson(response.data['data']);

      switch (status.alertLevel) {
        case 'safe':
          // No action needed
          break;
        case 'warning':
          showSnackBar('تنبيه: ${status.suggestedAction}');
          break;
        case 'danger':
          showWarningDialog(status.suggestedAction);
          break;
        case 'critical':
          showBlockingDialog(
            'حسابك موقوف مؤقتاً',
            status.suggestedAction,
          );
          break;
      }
    }
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
  }
}
```

### Get Transaction History (Paginated)

```dart
Future<PagedResult<Transaction>?> getTransactions({
  int page = 1,
  int pageSize = 10,
  int? type,
  DateTime? dateFrom,
  DateTime? dateTo,
}) async {
  try {
    final response = await dio.get('/wallet/transactions', queryParameters: {
      'pageNumber': page,
      'pageSize': pageSize,
      if (type != null) 'type': type,
      if (dateFrom != null) 'dateFrom': dateFrom.toIso8601String().split('T')[0],
      if (dateTo != null) 'dateTo': dateTo.toIso8601String().split('T')[0],
    });

    if (response.data['isSuccess'] == true) {
      return PagedResult.fromJson(
        response.data['data'],
        (json) => Transaction.fromJson(json),
      );
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Create Settlement

```dart
Future<Settlement?> createSettlement({
  required String partnerId,
  required double amount,
  required int settlementType,
  String? notes,
  bool sendWhatsApp = true,
}) async {
  try {
    final response = await dio.post('/settlements', data: {
      'partnerId': partnerId,
      'amount': amount,
      'settlementType': settlementType,
      'notes': notes,
      'sendWhatsApp': sendWhatsApp,
    });

    if (response.data['isSuccess'] == true) {
      showSuccess(response.data['message']);
      return Settlement.fromJson(response.data['data']);
    } else {
      showError(response.data['message']);
      return null;
    }
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Upload Settlement Receipt (multipart/form-data)

```dart
Future<String?> uploadReceipt(String settlementId, File imageFile) async {
  try {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(
        imageFile.path,
        filename: 'receipt_${DateTime.now().millisecondsSinceEpoch}.jpg',
      ),
    });

    final response = await dio.post(
      '/settlements/$settlementId/receipt',
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );

    if (response.data['isSuccess'] == true) {
      showSuccess(response.data['message']);
      return response.data['data']['receiptImageUrl'];
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل رفع الإيصال');
    return null;
  }
}
```

### Create Payment Request + Upload Proof

```dart
// Step 1: Create the payment request
Future<String?> createPaymentRequest({
  required String planId,
  required int paymentMethod,
  String? senderPhone,
  String? senderName,
  String? notes,
}) async {
  try {
    final response = await dio.post('/payment-requests', data: {
      'planId': planId,
      'paymentMethod': paymentMethod,
      if (senderPhone != null) 'senderPhone': senderPhone,
      if (senderName != null) 'senderName': senderName,
      if (notes != null) 'notes': notes,
    });

    if (response.data['isSuccess'] == true) {
      showSuccess(response.data['message']);
      return response.data['data']['id'];
    } else {
      showError(response.data['message']);
      return null;
    }
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}

// Step 2: Upload transfer proof
Future<bool> uploadPaymentProof(String requestId, File proofImage) async {
  try {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(
        proofImage.path,
        filename: 'proof_${DateTime.now().millisecondsSinceEpoch}.jpg',
      ),
    });

    final response = await dio.post(
      '/payment-requests/$requestId/upload-proof',
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );

    if (response.data['isSuccess'] == true) {
      showSuccess(response.data['message']);
      return true;
    }
    return false;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل رفع الإثبات');
    return false;
  }
}
```

### Get Daily Statistics

```dart
Future<DailyStats?> getDailyStats({DateTime? date}) async {
  try {
    final response = await dio.get('/statistics/daily', queryParameters: {
      if (date != null) 'date': date.toIso8601String().split('T')[0],
    });

    if (response.data['isSuccess'] == true) {
      return DailyStats.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Download Invoice PDF

```dart
Future<File?> downloadInvoicePdf(String invoiceId, String savePath) async {
  try {
    final response = await dio.get(
      '/invoices/$invoiceId/pdf',
      options: Options(responseType: ResponseType.bytes),
    );

    final file = File(savePath);
    await file.writeAsBytes(response.data);
    return file;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل تحميل الفاتورة');
    return null;
  }
}
```

### Export Statistics Report

```dart
Future<File?> exportStatistics({
  required String format, // "pdf" or "excel"
  required DateTime dateFrom,
  required DateTime dateTo,
  required String savePath,
}) async {
  try {
    final response = await dio.get(
      '/statistics/export',
      queryParameters: {
        'format': format,
        'dateFrom': dateFrom.toIso8601String().split('T')[0],
        'dateTo': dateTo.toIso8601String().split('T')[0],
      },
      options: Options(responseType: ResponseType.bytes),
    );

    final file = File(savePath);
    await file.writeAsBytes(response.data);
    return file;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'فشل تصدير التقرير');
    return null;
  }
}
```

### Analytics: Get Profitability Trends

```dart
Future<ProfitabilityTrends?> getProfitabilityTrends({
  DateTime? dateFrom,
  DateTime? dateTo,
  String interval = 'weekly',
}) async {
  try {
    final response = await dio.get('/analytics/profitability-trends',
      queryParameters: {
        if (dateFrom != null) 'dateFrom': dateFrom.toIso8601String().split('T')[0],
        if (dateTo != null) 'dateTo': dateTo.toIso8601String().split('T')[0],
        'interval': interval,
      },
    );

    if (response.data['isSuccess'] == true) {
      return ProfitabilityTrends.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Paginated List Helper

```dart
/// Generic paginated result model
class PagedResult<T> {
  final List<T> items;
  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final int totalPages;
  final bool hasNextPage;
  final bool hasPreviousPage;

  PagedResult({
    required this.items,
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
    required this.hasNextPage,
    required this.hasPreviousPage,
  });

  factory PagedResult.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonT,
  ) {
    return PagedResult(
      items: (json['items'] as List).map((e) => fromJsonT(e)).toList(),
      pageNumber: json['pageNumber'],
      pageSize: json['pageSize'],
      totalCount: json['totalCount'],
      totalPages: json['totalPages'],
      hasNextPage: json['hasNextPage'],
      hasPreviousPage: json['hasPreviousPage'],
    );
  }
}
```
