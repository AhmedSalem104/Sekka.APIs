# Sekka API - Intelligence & Customer Insights Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-27

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Interest Engine Flow](#3-interest-engine-flow)
4. [RFM Scoring Model](#4-rfm-scoring-model)
5. [Customer Insights Endpoints](#5-customer-insights-endpoints)
   - [Get Customer Profile](#51-get-customer-profile)
   - [Get Customer Interests](#52-get-customer-interests)
   - [Get Recommendations](#53-get-recommendations)
   - [Mark Recommendation Read](#54-mark-recommendation-read)
   - [Dismiss Recommendation](#55-dismiss-recommendation)
   - [Mark Recommendation Acted Upon](#56-mark-recommendation-acted-upon)
   - [Get Customer Behavior](#57-get-customer-behavior)
   - [Get Top Interests](#58-get-top-interests)
   - [Get Segments Summary](#59-get-segments-summary)
   - [Get Segment Customers](#510-get-segment-customers)
6. [Admin Segments Endpoints](#6-admin-segments-endpoints)
   - [List Segments](#61-list-segments)
   - [Get Segment](#62-get-segment)
   - [Create Segment](#63-create-segment)
   - [Update Segment](#64-update-segment)
   - [Delete Segment](#65-delete-segment)
   - [Refresh Segment](#66-refresh-segment)
   - [Get Segment Members](#67-get-segment-members)
   - [Add Member to Segment](#68-add-member-to-segment)
   - [Remove Member from Segment](#69-remove-member-from-segment)
   - [Get Segment Analytics](#610-get-segment-analytics)
7. [Admin Campaigns Endpoints](#7-admin-campaigns-endpoints)
   - [List Campaigns](#71-list-campaigns)
   - [Get Campaign](#72-get-campaign)
   - [Create Campaign](#73-create-campaign)
   - [Update Campaign](#74-update-campaign)
   - [Delete Campaign](#75-delete-campaign)
   - [Launch Campaign](#76-launch-campaign)
   - [Pause Campaign](#77-pause-campaign)
   - [Resume Campaign](#78-resume-campaign)
   - [Get Campaign Stats](#79-get-campaign-stats)
   - [Get Campaign Analytics](#710-get-campaign-analytics)
8. [Admin Insights Endpoints](#8-admin-insights-endpoints)
   - [Get Overview](#81-get-overview)
   - [Get Interest Heatmap](#82-get-interest-heatmap)
   - [Get Interest Trends](#83-get-interest-trends)
   - [Get Engagement Distribution](#84-get-engagement-distribution)
   - [Get RFM Analysis](#85-get-rfm-analysis)
   - [Get Global Behavior Summary](#86-get-global-behavior-summary)
   - [Get Category Performance](#87-get-category-performance)
9. [Enums](#9-enums)
10. [DTOs Reference](#10-dtos-reference)
11. [Flutter Integration](#11-flutter-integration)

---

## 1. Overview

Phase 7 (Intelligence) provides customer analytics, interest tracking, behavior analysis, segmentation, and targeted campaign management. This module powers personalized recommendations and data-driven decision making.

| Feature | Detail |
|---------|--------|
| Interest Tracking | Automatic signal-based scoring per category |
| Behavior Analysis | Order patterns, preferred times, spending tiers |
| RFM Scoring | Recency, Frequency, Monetary (1-5 scale each) |
| Segmentation | Automatic, Manual, RFM-based, Behavior-based |
| Recommendations | AI-generated suggestions with lifecycle tracking |
| Campaigns | Targeted messaging to segments with analytics |

### Controllers

| Controller | Route | Auth | Endpoints |
|-----------|-------|------|-----------|
| CustomerInsightsController | `/api/v1/customer-insights` | Bearer Token | 10 |
| AdminSegmentsController | `/api/v1/admin/segments` | Admin Role | 10 |
| AdminCampaignsController | `/api/v1/admin/campaigns` | Admin Role | 10 |
| AdminInsightsController | `/api/v1/admin/insights` | Admin Role | 7 |

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

### HTTP Status Codes Used
| Code | Meaning |
|------|---------|
| 200 | Success |
| 400 | Bad Request / Feature Under Development |
| 401 | Unauthorized (invalid/missing token) |
| 403 | Forbidden (not Admin role) |
| 404 | Not Found |
| 409 | Conflict |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Interest Engine Flow

The Interest Engine automatically builds customer profiles by collecting behavioral signals from the order lifecycle.

```
 ┌──────────────────────────────────────────────────────────────────────┐
 │                    INTEREST ENGINE PIPELINE                         │
 └──────────────────────────────────────────────────────────────────────┘

  ┌─────────────┐     ┌──────────────┐     ┌───────────────┐
  │  Order       │     │  Rating      │     │  Behavior     │
  │  Events      │     │  Events      │     │  Signals      │
  │ (create,     │     │ (high/low    │     │ (reorder,     │
  │  deliver,    │     │  ratings)    │     │  frequency,   │
  │  cancel)     │     │              │     │  high value)  │
  └──────┬───────┘     └──────┬───────┘     └──────┬────────┘
         │                    │                     │
         └────────────┬───────┴─────────────────────┘
                      │
              ┌───────▼────────┐
              │  Signal        │
              │  Collector     │
              │  (SignalType)  │
              └───────┬────────┘
                      │
              ┌───────▼────────┐
              │  Interest      │
              │  Scorer        │
              │  (per category,│
              │   weighted by  │
              │   signal type) │
              └───────┬────────┘
                      │
         ┌────────────┼───────────────┐
         │            │               │
  ┌──────▼─────┐ ┌───▼──────┐ ┌──────▼──────┐
  │ Customer   │ │ Behavior │ │ Segment     │
  │ Interest   │ │ Pattern  │ │ Assignment  │
  │ Profiles   │ │ Detection│ │ Engine      │
  │ (scores,   │ │ (time,   │ │ (auto/RFM/  │
  │  trends,   │ │  area,   │ │  behavior)  │
  │  confidence│ │  spend)  │ │             │
  │  levels)   │ │          │ │             │
  └──────┬─────┘ └───┬──────┘ └──────┬──────┘
         │           │               │
         └───────────┼───────────────┘
                     │
             ┌───────▼────────┐
             │ Recommendation │
             │ Engine         │
             │ (rules-based,  │
             │  generates     │
             │  suggestions)  │
             └───────┬────────┘
                     │
            ┌────────▼─────────┐
            │ Driver App       │
            │ (profile view,   │
            │  recommendations,│
            │  segment info)   │
            └──────────────────┘
```

### Signal Weights (approximate)

| Signal | Weight | Description |
|--------|--------|-------------|
| OrderCreated | 1.0 | Customer placed an order in category |
| OrderDelivered | 1.5 | Order successfully delivered |
| OrderReordered | 2.0 | Customer reordered same category |
| HighRating | 1.5 | Customer rated 4-5 stars |
| LowRating | -1.0 | Customer rated 1-2 stars (negative signal) |
| HighValue | 2.0 | Order value above average |
| RecurringOrder | 2.5 | Detected recurring pattern |
| Cancellation | -1.5 | Customer cancelled order |
| ReturnOrder | -1.0 | Customer returned order |

---

## 4. RFM Scoring Model

RFM (Recency, Frequency, Monetary) scoring segments customers based on their purchasing behavior.

```
  ┌─────────────────────────────────────────────────────┐
  │                  RFM SCORING                        │
  │                                                     │
  │   Recency (R)      How recently did they order?     │
  │   Score 1-5         5 = very recent, 1 = long ago   │
  │                                                     │
  │   Frequency (F)    How often do they order?          │
  │   Score 1-5         5 = very frequent, 1 = rare      │
  │                                                     │
  │   Monetary (M)     How much do they spend?           │
  │   Score 1-5         5 = high spender, 1 = low        │
  │                                                     │
  │   Total Score = R + F + M  (range: 3-15)            │
  └─────────────────────────────────────────────────────┘
```

### RFM Segment Mapping

| Total Score | Segment | Description |
|-------------|---------|-------------|
| 12-15 | Champions | Best customers, high value and active |
| 9-11 | Loyal Customers | Regular buyers, good spending |
| 6-8 | Potential Loyalists | Moderate activity, growth opportunity |
| 4-5 | At Risk | Declining activity, needs attention |
| 3 | Hibernating | Inactive, may need reactivation |

---

## 5. Customer Insights Endpoints

> **Base**: `/api/v1/customer-insights`
>
> **Auth**: Bearer Token required on ALL endpoints

---

### 5.1 Get Customer Profile

Returns the full interest profile for a customer, including interests, segments, behavior, and RFM score.

```
GET /customer-insights/{customerId}/profile
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `customerId` (GUID) - The customer ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerName": "سارة أحمد",
    "customerPhone": "+201012345678",
    "engagementLevel": "Active",
    "lifetimeValue": 4500.00,
    "totalOrders": 32,
    "topInterests": [
      {
        "categoryId": "a1b2c3d4-...",
        "categoryName": "Electronics",
        "categoryNameAr": "إلكترونيات",
        "categoryColor": "#FF5722",
        "score": 87.5,
        "signalCount": 24,
        "trendDirection": "Rising",
        "confidenceLevel": 0.92,
        "lastSignalAt": "2026-03-26T14:30:00Z"
      }
    ],
    "currentSegments": [
      {
        "segmentId": "b2c3d4e5-...",
        "name": "High Spenders",
        "nameAr": "المنفقين الكبار",
        "colorHex": "#4CAF50",
        "joinedAt": "2026-02-15T10:00:00Z"
      }
    ],
    "behaviorSummary": {
      "preferredOrderTime": "Evening",
      "preferredDayOfWeek": "Friday",
      "averageOrderValue": 140.63,
      "orderFrequencyPerMonth": 8.0,
      "preferredPaymentMethod": "Cash",
      "preferredAreas": ["مدينة نصر", "مصر الجديدة"],
      "spendingTier": "High",
      "patterns": [
        {
          "patternType": "PreferredTime",
          "patternKey": "order_time",
          "patternValue": "18:00-22:00",
          "confidence": 0.85,
          "occurrences": 22
        }
      ]
    },
    "lastOrderDate": "2026-03-26T14:30:00Z",
    "daysSinceLastOrder": 1,
    "rfmScore": {
      "recencyScore": 5,
      "frequencyScore": 4,
      "monetaryScore": 5,
      "totalScore": 14,
      "segment": "Champions"
    }
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 401 | Unauthorized |
| 404 | Customer not found |

---

### 5.2 Get Customer Interests

Returns the interest scores per category for a customer.

```
GET /customer-insights/{customerId}/interests
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `customerId` (GUID)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "categoryId": "a1b2c3d4-...",
      "categoryName": "Electronics",
      "categoryNameAr": "إلكترونيات",
      "categoryColor": "#FF5722",
      "score": 87.5,
      "signalCount": 24,
      "trendDirection": "Rising",
      "confidenceLevel": 0.92,
      "lastSignalAt": "2026-03-26T14:30:00Z"
    },
    {
      "categoryId": "c3d4e5f6-...",
      "categoryName": "Food",
      "categoryNameAr": "طعام",
      "categoryColor": "#4CAF50",
      "score": 65.0,
      "signalCount": 15,
      "trendDirection": "Stable",
      "confidenceLevel": 0.78,
      "lastSignalAt": "2026-03-20T09:15:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.3 Get Recommendations

Returns active recommendations for a customer.

```
GET /customer-insights/{customerId}/recommendations
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `customerId` (GUID)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "d4e5f6a7-...",
      "recommendationType": "CrossSell",
      "title": "عرض خاص على الإلكترونيات",
      "message": "العميل يطلب كثير من الأكل - جرب تعرض عليه إلكترونيات",
      "categoryName": "Electronics",
      "categoryColor": "#FF5722",
      "relevanceScore": 0.89,
      "status": "Active",
      "expiresAt": "2026-04-01T00:00:00Z",
      "createdAt": "2026-03-25T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.4 Mark Recommendation Read

Marks a recommendation as read by the driver.

```
PUT /customer-insights/recommendations/{recommendationId}/read
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `recommendationId` (GUID)

**Success Response** `200 OK`:
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
| 404 | Recommendation not found |
| 401 | Unauthorized |

---

### 5.5 Dismiss Recommendation

Dismisses a recommendation (driver not interested).

```
PUT /customer-insights/recommendations/{recommendationId}/dismiss
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `recommendationId` (GUID)

**Success Response** `200 OK`:
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
| 404 | Recommendation not found |
| 401 | Unauthorized |

---

### 5.6 Mark Recommendation Acted Upon

Marks a recommendation as acted upon (driver followed through).

```
PUT /customer-insights/recommendations/{recommendationId}/act
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `recommendationId` (GUID)

**Success Response** `200 OK`:
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
| 404 | Recommendation not found |
| 401 | Unauthorized |

---

### 5.7 Get Customer Behavior

Returns the behavior analysis summary for a customer.

```
GET /customer-insights/{customerId}/behavior
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `customerId` (GUID)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "preferredOrderTime": "Evening",
    "preferredDayOfWeek": "Friday",
    "averageOrderValue": 140.63,
    "orderFrequencyPerMonth": 8.0,
    "preferredPaymentMethod": "Cash",
    "preferredAreas": ["مدينة نصر", "مصر الجديدة"],
    "spendingTier": "High",
    "patterns": [
      {
        "patternType": "PreferredTime",
        "patternKey": "order_time",
        "patternValue": "18:00-22:00",
        "confidence": 0.85,
        "occurrences": 22
      },
      {
        "patternType": "PreferredArea",
        "patternKey": "delivery_area",
        "patternValue": "مدينة نصر",
        "confidence": 0.72,
        "occurrences": 15
      },
      {
        "patternType": "PaymentPreference",
        "patternKey": "payment_method",
        "patternValue": "Cash",
        "confidence": 0.90,
        "occurrences": 28
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 5.8 Get Top Interests

Returns the top interest categories across all customers assigned to the current driver.

```
GET /customer-insights/top-interests
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| limit | int | 10 | Max categories to return |
| dateFrom | DateTime? | null | Filter signals from this date |
| dateTo | DateTime? | null | Filter signals to this date |

**Example**: `GET /customer-insights/top-interests?limit=5&dateFrom=2026-03-01`

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "categoryId": "a1b2c3d4-...",
      "categoryName": "Electronics",
      "categoryNameAr": "إلكترونيات",
      "categoryColor": "#FF5722",
      "customerCount": 145,
      "totalOrders": 890,
      "totalRevenue": 125000.00,
      "averageScore": 72.3,
      "trendDirection": "Rising"
    },
    {
      "categoryId": "b2c3d4e5-...",
      "categoryName": "Food",
      "categoryNameAr": "طعام",
      "categoryColor": "#4CAF50",
      "customerCount": 320,
      "totalOrders": 2100,
      "totalRevenue": 84000.00,
      "averageScore": 68.1,
      "trendDirection": "Stable"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.9 Get Segments Summary

Returns a summary of all customer segments visible to the current driver.

```
GET /customer-insights/segments
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "segmentId": "c3d4e5f6-...",
      "name": "High Spenders",
      "nameAr": "المنفقين الكبار",
      "colorHex": "#4CAF50",
      "memberCount": 85,
      "percentageOfTotal": 12.5
    },
    {
      "segmentId": "d4e5f6a7-...",
      "name": "At Risk",
      "nameAr": "معرضين للخطر",
      "colorHex": "#F44336",
      "memberCount": 42,
      "percentageOfTotal": 6.2
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.10 Get Segment Customers

Returns paginated list of customers in a specific segment.

```
GET /customer-insights/segments/{segmentId}/customers
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `segmentId` (GUID)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Example**: `GET /customer-insights/segments/{segmentId}/customers?page=1&pageSize=10`

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "customerId": "3fa85f64-...",
        "customerName": "سارة أحمد",
        "customerPhone": "+201012345678",
        "engagementLevel": "Active",
        "lifetimeValue": 4500.00,
        "totalOrders": 32,
        "lastOrderDate": "2026-03-26T14:30:00Z"
      }
    ],
    "totalCount": 85,
    "page": 1,
    "pageSize": 10,
    "totalPages": 9
  },
  "message": null,
  "errors": null
}
```

---

## 6. Admin Segments Endpoints

> **Base**: `/api/v1/admin/segments`
>
> **Auth**: Admin Role required on ALL endpoints
>
> **Status**: Under Development (all endpoints return 400 with feature-under-development message)

---

### 6.1 List Segments

Returns a filtered, paginated list of all segments.

```
GET /admin/segments
```

**Auth Required**: Yes (Admin Role)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| segmentType | string? | null | Filter by segment type (see [SegmentType](#segmenttype)) |
| isActive | bool? | null | Filter by active status |
| searchTerm | string? | null | Search by name |

**Example**: `GET /admin/segments?segmentType=Automatic&isActive=true&page=1`

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "c3d4e5f6-...",
        "name": "High Spenders",
        "nameAr": "المنفقين الكبار",
        "segmentType": "RFMBased",
        "description": "Customers with high monetary RFM scores",
        "colorHex": "#4CAF50",
        "isAutomatic": true,
        "memberCount": 85,
        "isActive": true,
        "lastRefreshedAt": "2026-03-27T06:00:00Z",
        "createdAt": "2026-02-01T10:00:00Z"
      }
    ],
    "totalCount": 12,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

---

### 6.2 Get Segment

Returns detailed info for a single segment.

```
GET /admin/segments/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "c3d4e5f6-...",
    "name": "High Spenders",
    "nameAr": "المنفقين الكبار",
    "segmentType": "RFMBased",
    "description": "Customers with high monetary RFM scores",
    "colorHex": "#4CAF50",
    "isAutomatic": true,
    "memberCount": 85,
    "isActive": true,
    "lastRefreshedAt": "2026-03-27T06:00:00Z",
    "createdAt": "2026-02-01T10:00:00Z",
    "rules": "{\"monetaryScore\": {\"min\": 4}}",
    "minScore": 4.0,
    "maxScore": null,
    "topInterests": ["Electronics", "Fashion", "Food"],
    "engagementDistribution": {
      "VeryActive": 30,
      "Active": 35,
      "Moderate": 15,
      "Low": 4,
      "Inactive": 1
    }
  },
  "message": null,
  "errors": null
}
```

---

### 6.3 Create Segment

Creates a new customer segment.

```
POST /admin/segments
```

**Auth Required**: Yes (Admin Role)

**Request Body**:
```json
{
  "name": "Weekend Shoppers",
  "nameAr": "متسوقي نهاية الأسبوع",
  "segmentType": "BehaviorBased",
  "description": "Customers who primarily order on weekends",
  "colorHex": "#2196F3",
  "rules": "{\"preferredDay\": [\"Friday\", \"Saturday\"]}",
  "isAutomatic": true,
  "minScore": null,
  "maxScore": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Segment name (English) |
| nameAr | string | Yes | Segment name (Arabic) |
| segmentType | string | Yes | See [SegmentType](#segmenttype) enum |
| description | string | No | Description |
| colorHex | string | No | Color hex code (e.g., `#2196F3`) |
| rules | string | No | JSON rules for automatic segments |
| isAutomatic | bool | Yes | Auto-assign customers based on rules |
| minScore | decimal? | No | Minimum score threshold |
| maxScore | decimal? | No | Maximum score threshold |

**Expected Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": { "id": "new-segment-guid-..." },
  "message": "تم إنشاء الشريحة بنجاح",
  "errors": null
}
```

---

### 6.4 Update Segment

Updates an existing segment. Only provided fields are updated.

```
PUT /admin/segments/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Request Body**:
```json
{
  "name": "Weekend Shoppers V2",
  "description": "Updated description",
  "colorHex": "#673AB7"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Segment name (English) |
| nameAr | string | No | Segment name (Arabic) |
| segmentType | string | No | See [SegmentType](#segmenttype) enum |
| description | string | No | Description |
| colorHex | string | No | Color hex code |
| rules | string | No | JSON rules |
| isAutomatic | bool? | No | Auto-assign toggle |
| minScore | decimal? | No | Min score threshold |
| maxScore | decimal? | No | Max score threshold |

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الشريحة بنجاح",
  "errors": null
}
```

---

### 6.5 Delete Segment

Deletes a segment.

```
DELETE /admin/segments/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الشريحة بنجاح",
  "errors": null
}
```

---

### 6.6 Refresh Segment

Re-evaluates segment rules and updates member list.

```
POST /admin/segments/{id}/refresh
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تحديث الشريحة بنجاح",
  "errors": null
}
```

---

### 6.7 Get Segment Members

Returns paginated list of members in a segment.

```
GET /admin/segments/{id}/members
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "customerId": "3fa85f64-...",
        "customerName": "سارة أحمد",
        "customerPhone": "+201012345678",
        "driverName": "أحمد محمد",
        "joinedAt": "2026-02-15T10:00:00Z",
        "score": 87.5,
        "totalOrders": 32,
        "lastOrderDate": "2026-03-26T14:30:00Z",
        "engagementLevel": "Active"
      }
    ],
    "totalCount": 85,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  },
  "message": null,
  "errors": null
}
```

---

### 6.8 Add Member to Segment

Manually adds a customer to a segment.

```
POST /admin/segments/{id}/members/{customerId}
```

**Auth Required**: Yes (Admin Role)

**URL Params**:
- `id` (GUID) - Segment ID
- `customerId` (GUID) - Customer ID

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إضافة العضو للشريحة بنجاح",
  "errors": null
}
```

---

### 6.9 Remove Member from Segment

Removes a customer from a segment.

```
DELETE /admin/segments/{id}/members/{customerId}
```

**Auth Required**: Yes (Admin Role)

**URL Params**:
- `id` (GUID) - Segment ID
- `customerId` (GUID) - Customer ID

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إزالة العضو من الشريحة بنجاح",
  "errors": null
}
```

---

### 6.10 Get Segment Analytics

Returns aggregate analytics across all segments.

```
GET /admin/segments/analytics
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalSegments": 12,
    "automaticSegments": 8,
    "manualSegments": 4,
    "totalMembers": 680,
    "averageMembersPerSegment": 56.7,
    "growthTrend": [
      {
        "date": "2026-03-20",
        "newMembers": 15,
        "removedMembers": 3,
        "netGrowth": 12
      }
    ],
    "distribution": [
      {
        "segmentName": "High Spenders",
        "colorHex": "#4CAF50",
        "memberCount": 85,
        "percentage": 12.5
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 7. Admin Campaigns Endpoints

> **Base**: `/api/v1/admin/campaigns`
>
> **Auth**: Admin Role required on ALL endpoints
>
> **Status**: Under Development (all endpoints return 400 with feature-under-development message)

---

### 7.1 List Campaigns

Returns a filtered, paginated list of campaigns.

```
GET /admin/campaigns
```

**Auth Required**: Yes (Admin Role)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 20 | Items per page |
| campaignType | string? | null | Filter by type (see [CampaignType](#campaigntype)) |
| status | string? | null | Filter by status (see [CampaignStatus](#campaignstatus)) |
| dateFrom | DateTime? | null | Filter campaigns from date |
| dateTo | DateTime? | null | Filter campaigns to date |

**Example**: `GET /admin/campaigns?status=Running&campaignType=Promotional`

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "e5f6a7b8-...",
        "name": "رمضان خصم 20%",
        "campaignType": "Promotional",
        "segmentName": "High Spenders",
        "categoryName": "Food",
        "channel": "Push",
        "targetCount": 85,
        "sentCount": 80,
        "openCount": 45,
        "conversionCount": 12,
        "conversionRate": 15.0,
        "status": "Running",
        "scheduledAt": "2026-03-15T08:00:00Z",
        "createdAt": "2026-03-10T14:00:00Z"
      }
    ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

---

### 7.2 Get Campaign

Returns detailed info for a single campaign.

```
GET /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "e5f6a7b8-...",
    "name": "رمضان خصم 20%",
    "campaignType": "Promotional",
    "segmentName": "High Spenders",
    "categoryName": "Food",
    "channel": "Push",
    "targetCount": 85,
    "sentCount": 80,
    "openCount": 45,
    "conversionCount": 12,
    "conversionRate": 15.0,
    "status": "Running",
    "scheduledAt": "2026-03-15T08:00:00Z",
    "createdAt": "2026-03-10T14:00:00Z",
    "messageTemplate": "خصم 20% على كل الأكل في رمضان! اطلب دلوقتي 🌙",
    "startedAt": "2026-03-15T08:00:00Z",
    "completedAt": null,
    "createdByName": "مدير النظام"
  },
  "message": null,
  "errors": null
}
```

---

### 7.3 Create Campaign

Creates a new campaign.

```
POST /admin/campaigns
```

**Auth Required**: Yes (Admin Role)

**Request Body**:
```json
{
  "name": "رمضان خصم 20%",
  "campaignType": "Promotional",
  "segmentId": "c3d4e5f6-...",
  "categoryId": "a1b2c3d4-...",
  "messageTemplate": "خصم 20% على كل الأكل في رمضان! اطلب دلوقتي",
  "channel": "Push",
  "scheduledAt": "2026-03-15T08:00:00Z"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Campaign name |
| campaignType | string | Yes | See [CampaignType](#campaigntype) enum |
| segmentId | GUID? | No | Target segment |
| categoryId | GUID? | No | Target category |
| messageTemplate | string | Yes | Message content template |
| channel | string | Yes | Delivery channel (Push, SMS, Email) |
| scheduledAt | DateTime? | No | Schedule for later (null = draft) |

**Expected Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": { "id": "new-campaign-guid-..." },
  "message": "تم إنشاء الحملة بنجاح",
  "errors": null
}
```

---

### 7.4 Update Campaign

Updates an existing campaign. Only provided fields are updated. Only `Draft` or `Paused` campaigns can be updated.

```
PUT /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Request Body**:
```json
{
  "name": "رمضان خصم 30%",
  "messageTemplate": "خصم 30% بدل 20%!",
  "scheduledAt": "2026-03-16T08:00:00Z"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Campaign name |
| messageTemplate | string | No | Message content |
| channel | string | No | Delivery channel |
| scheduledAt | DateTime? | No | Schedule time |

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعديل الحملة بنجاح",
  "errors": null
}
```

---

### 7.5 Delete Campaign

Deletes a campaign. Only `Draft` campaigns can be deleted.

```
DELETE /admin/campaigns/{id}
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف الحملة بنجاح",
  "errors": null
}
```

---

### 7.6 Launch Campaign

Launches a draft or scheduled campaign immediately.

```
POST /admin/campaigns/{id}/launch
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إطلاق الحملة بنجاح",
  "errors": null
}
```

**Campaign Status Transition**: `Draft` / `Scheduled` -> `Running`

---

### 7.7 Pause Campaign

Pauses a running campaign.

```
POST /admin/campaigns/{id}/pause
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إيقاف الحملة بنجاح",
  "errors": null
}
```

**Campaign Status Transition**: `Running` -> `Paused`

---

### 7.8 Resume Campaign

Resumes a paused campaign.

```
POST /admin/campaigns/{id}/resume
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم استئناف الحملة بنجاح",
  "errors": null
}
```

**Campaign Status Transition**: `Paused` -> `Running`

---

### 7.9 Get Campaign Stats

Returns aggregate stats across all campaigns.

```
GET /admin/campaigns/stats
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCampaigns": 24,
    "activeCampaigns": 3,
    "completedCampaigns": 18,
    "draftCampaigns": 3,
    "averageConversionRate": 12.5,
    "totalMessagesSent": 15400,
    "totalConversions": 1925
  },
  "message": null,
  "errors": null
}
```

---

### 7.10 Get Campaign Analytics

Returns detailed analytics for a specific campaign.

```
GET /admin/campaigns/{id}/analytics
```

**Auth Required**: Yes (Admin Role)

**URL Params**: `id` (GUID)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalSent": 80,
    "totalOpened": 45,
    "totalConversions": 12,
    "openRate": 56.25,
    "conversionRate": 15.0,
    "revenueGenerated": 3600.00,
    "dailyStats": [
      {
        "date": "2026-03-15",
        "sent": 80,
        "opened": 20,
        "conversions": 5
      },
      {
        "date": "2026-03-16",
        "sent": 0,
        "opened": 15,
        "conversions": 4
      },
      {
        "date": "2026-03-17",
        "sent": 0,
        "opened": 10,
        "conversions": 3
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

## 8. Admin Insights Endpoints

> **Base**: `/api/v1/admin/insights`
>
> **Auth**: Admin Role required on ALL endpoints
>
> **Status**: Under Development (all endpoints return 400 with feature-under-development message)

---

### 8.1 Get Overview

Returns a high-level overview of the intelligence system.

```
GET /admin/insights/overview
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCustomers": 680,
    "activeCustomers": 520,
    "atRiskCustomers": 95,
    "churnedCustomers": 65,
    "averageEngagementScore": 68.4,
    "averageLifetimeValue": 2340.00,
    "totalSegments": 12,
    "totalCategories": 18,
    "totalSignalsToday": 1250,
    "totalRecommendationsGenerated": 340
  },
  "message": null,
  "errors": null
}
```

---

### 8.2 Get Interest Heatmap

Returns a heatmap of interest scores by category and segment.

```
GET /admin/insights/heatmap
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "cells": [
      {
        "categoryName": "Electronics",
        "segmentName": "High Spenders",
        "score": 85.2,
        "customerCount": 42
      },
      {
        "categoryName": "Electronics",
        "segmentName": "At Risk",
        "score": 35.0,
        "customerCount": 12
      },
      {
        "categoryName": "Food",
        "segmentName": "High Spenders",
        "score": 72.1,
        "customerCount": 65
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 8.3 Get Interest Trends

Returns interest trend data over time per category.

```
GET /admin/insights/trends
```

**Auth Required**: Yes (Admin Role)

**Query Params**:

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| dateFrom | DateTime? | null | Start date |
| dateTo | DateTime? | null | End date |
| period | string | "daily" | Granularity: `daily`, `weekly`, `monthly` |
| limit | int? | null | Max categories to include |

**Example**: `GET /admin/insights/trends?period=weekly&limit=5`

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "categoryId": "a1b2c3d4-...",
      "categoryName": "Electronics",
      "categoryNameAr": "إلكترونيات",
      "categoryColor": "#FF5722",
      "trendDirection": "Rising",
      "changePercent": 12.5,
      "dataPoints": [
        { "date": "2026-03-01", "value": 65.0, "count": 45 },
        { "date": "2026-03-08", "value": 68.3, "count": 52 },
        { "date": "2026-03-15", "value": 72.1, "count": 58 },
        { "date": "2026-03-22", "value": 75.8, "count": 63 }
      ]
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 8.4 Get Engagement Distribution

Returns the distribution of customers across engagement levels.

```
GET /admin/insights/engagement-distribution
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "highEngagement": 180,
    "mediumEngagement": 250,
    "lowEngagement": 130,
    "atRisk": 75,
    "churned": 45,
    "highPercentage": 26.5,
    "mediumPercentage": 36.8,
    "lowPercentage": 19.1,
    "atRiskPercentage": 11.0,
    "churnedPercentage": 6.6
  },
  "message": null,
  "errors": null
}
```

---

### 8.5 Get RFM Analysis

Returns aggregate RFM analysis across all customers.

```
GET /admin/insights/rfm-analysis
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalCustomersAnalyzed": 680,
    "averageRecencyScore": 3.2,
    "averageFrequencyScore": 2.8,
    "averageMonetaryScore": 3.0,
    "segmentDistribution": {
      "Champions": 85,
      "Loyal Customers": 150,
      "Potential Loyalists": 210,
      "At Risk": 145,
      "Hibernating": 90
    },
    "segments": [
      {
        "segmentName": "Champions",
        "customerCount": 85,
        "averageRecency": 4.8,
        "averageFrequency": 4.5,
        "averageMonetary": 4.7,
        "percentage": 12.5
      },
      {
        "segmentName": "Loyal Customers",
        "customerCount": 150,
        "averageRecency": 4.0,
        "averageFrequency": 3.5,
        "averageMonetary": 3.8,
        "percentage": 22.1
      },
      {
        "segmentName": "Potential Loyalists",
        "customerCount": 210,
        "averageRecency": 3.2,
        "averageFrequency": 2.5,
        "averageMonetary": 2.8,
        "percentage": 30.9
      },
      {
        "segmentName": "At Risk",
        "customerCount": 145,
        "averageRecency": 2.0,
        "averageFrequency": 1.8,
        "averageMonetary": 2.2,
        "percentage": 21.3
      },
      {
        "segmentName": "Hibernating",
        "customerCount": 90,
        "averageRecency": 1.2,
        "averageFrequency": 1.1,
        "averageMonetary": 1.0,
        "percentage": 13.2
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 8.6 Get Global Behavior Summary

Returns behavior analysis summary across all customers.

```
GET /admin/insights/behavior-summary
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "mostPopularOrderTime": "Evening",
    "mostPopularDayOfWeek": "Friday",
    "averageOrderValue": 125.50,
    "averageOrderFrequency": 4.2,
    "mostPopularPaymentMethod": "Cash",
    "topAreas": ["مدينة نصر", "المعادي", "مصر الجديدة", "التجمع الخامس"],
    "spendingTierDistribution": {
      "High": 18.5,
      "Medium": 52.0,
      "Low": 29.5
    },
    "paymentMethodDistribution": {
      "Cash": 420,
      "CreditCard": 150,
      "Wallet": 110
    }
  },
  "message": null,
  "errors": null
}
```

---

### 8.7 Get Category Performance

Returns performance metrics for all categories.

```
GET /admin/insights/category-performance
```

**Auth Required**: Yes (Admin Role)

**Expected Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "categoryId": "a1b2c3d4-...",
      "categoryName": "Electronics",
      "categoryNameAr": "إلكترونيات",
      "categoryColor": "#FF5722",
      "totalOrders": 890,
      "totalRevenue": 125000.00,
      "uniqueCustomers": 145,
      "averageOrderValue": 140.45,
      "growthPercent": 15.3,
      "trendDirection": "Rising"
    },
    {
      "categoryId": "b2c3d4e5-...",
      "categoryName": "Food",
      "categoryNameAr": "طعام",
      "categoryColor": "#4CAF50",
      "totalOrders": 2100,
      "totalRevenue": 84000.00,
      "uniqueCustomers": 320,
      "averageOrderValue": 40.00,
      "growthPercent": 3.2,
      "trendDirection": "Stable"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 9. Enums

### TrendDirection
| Value | Name | Description |
|-------|------|-------------|
| 0 | Rising | Interest is increasing |
| 1 | Stable | Interest is steady |
| 2 | Falling | Interest is declining |

### SignalType
| Value | Name | Description |
|-------|------|-------------|
| 0 | OrderCreated | Customer placed an order |
| 1 | OrderDelivered | Order was delivered |
| 2 | OrderReordered | Customer reordered same category |
| 3 | HighRating | Customer rated 4-5 stars |
| 4 | LowRating | Customer rated 1-2 stars |
| 5 | FrequentAddress | Order to frequently used address |
| 6 | HighValue | Order value above average |
| 7 | Cancellation | Customer cancelled order |
| 8 | ReturnOrder | Customer returned order |
| 9 | RecurringOrder | Detected recurring pattern |
| 10 | PartnerOrder | Order from a partner store |
| 11 | TimeSlotBooked | Customer booked a time slot |

### SegmentType
| Value | Name | Description |
|-------|------|-------------|
| 0 | Automatic | Auto-assigned by rules engine |
| 1 | Manual | Manually curated by admin |
| 2 | RFMBased | Based on RFM score thresholds |
| 3 | BehaviorBased | Based on behavior patterns |

### BehaviorPatternType
| Value | Name | Description |
|-------|------|-------------|
| 0 | PreferredTime | Preferred order time of day |
| 1 | PreferredDay | Preferred day of week |
| 2 | SpendingPattern | Spending habits |
| 3 | OrderFrequency | How often they order |
| 4 | PreferredArea | Preferred delivery area |
| 5 | PaymentPreference | Preferred payment method |
| 6 | PartnerPreference | Preferred partner stores |

### RecommendationType
| Value | Name | Description |
|-------|------|-------------|
| 0 | ProductSuggestion | Suggest a product/category |
| 1 | TimeOptimization | Suggest optimal delivery time |
| 2 | CustomerRetention | Retention action for at-risk customer |
| 3 | CrossSell | Suggest related category |
| 4 | UpSell | Suggest higher-value option |
| 5 | ReactivationOffer | Offer to reactivate dormant customer |

### RecommendationStatus
| Value | Name | Description |
|-------|------|-------------|
| 0 | Active | New, not yet seen |
| 1 | Read | Driver has seen it |
| 2 | Dismissed | Driver dismissed it |
| 3 | ActedUpon | Driver acted on it |
| 4 | Expired | Past expiry date |

### CampaignType
| Value | Name | Description |
|-------|------|-------------|
| 0 | Promotional | Discounts and offers |
| 1 | Retention | Keep active customers engaged |
| 2 | Reactivation | Win back dormant customers |
| 3 | CrossSell | Promote related categories |
| 4 | Announcement | General announcements |

### CampaignStatus
| Value | Name | Description |
|-------|------|-------------|
| 0 | Draft | Not yet launched |
| 1 | Scheduled | Scheduled for future launch |
| 2 | Running | Currently active |
| 3 | Paused | Temporarily stopped |
| 4 | Completed | Finished sending |
| 5 | Cancelled | Cancelled by admin |

### Campaign Status Transitions
```
Draft ──────► Scheduled ──────► Running ──────► Completed
  │                               │  ▲
  │                               ▼  │
  └──────────► Running ◄───── Paused
                  │
                  └──────────► Cancelled
```

### EngagementLevel
| Value | Name | Description |
|-------|------|-------------|
| 0 | Inactive | No recent activity |
| 1 | Low | Minimal engagement |
| 2 | Moderate | Regular engagement |
| 3 | Active | Frequent engagement |
| 4 | VeryActive | Highly engaged customer |

---

## 10. DTOs Reference

### Intelligence DTOs (Driver App)

#### CustomerInterestProfileDto
```
customerId          Guid        Customer unique ID
customerName        string?     Customer name (nullable)
customerPhone       string      Customer phone (+201XXXXXXXXX)
engagementLevel     string      EngagementLevel enum name
lifetimeValue       decimal     Total spending amount
totalOrders         int         Total order count
topInterests        List<CustomerInterestDto>
currentSegments     List<SegmentBriefDto>
behaviorSummary     CustomerBehaviorSummaryDto?
lastOrderDate       DateTime?   Last order timestamp
daysSinceLastOrder  int?        Days since last order
rfmScore            RfmScoreDto?
```

#### CustomerInterestDto
```
categoryId          Guid        Category ID
categoryName        string      English name
categoryNameAr      string      Arabic name
categoryColor       string?     Hex color code
score               decimal     Interest score (0-100)
signalCount         int         Number of signals collected
trendDirection      string      TrendDirection enum name
confidenceLevel     decimal     Confidence (0.0-1.0)
lastSignalAt        DateTime?   Last signal timestamp
```

#### CustomerBehaviorSummaryDto
```
preferredOrderTime      string?     e.g. "Evening", "Morning"
preferredDayOfWeek      string?     e.g. "Friday"
averageOrderValue       decimal     Average order amount
orderFrequencyPerMonth  decimal     Orders per month
preferredPaymentMethod  string?     e.g. "Cash", "CreditCard"
preferredAreas          List<string>  Preferred delivery areas
spendingTier            string      "High", "Medium", "Low"
patterns                List<BehaviorPatternDto>
```

#### BehaviorPatternDto
```
patternType     string      BehaviorPatternType enum name
patternKey      string      Pattern identifier key
patternValue    string      Detected value
confidence      decimal     Confidence (0.0-1.0)
occurrences     int         Number of occurrences
```

#### CustomerRecommendationDto
```
id                  Guid        Recommendation ID
recommendationType  string      RecommendationType enum name
title               string      Display title
message             string      Recommendation message
categoryName        string?     Related category
categoryColor       string?     Category hex color
relevanceScore      decimal     Relevance (0.0-1.0)
status              string      RecommendationStatus enum name
expiresAt           DateTime?   Expiry timestamp
createdAt           DateTime    Creation timestamp
```

#### RfmScoreDto
```
recencyScore    int     1-5 (5 = most recent)
frequencyScore  int     1-5 (5 = most frequent)
monetaryScore   int     1-5 (5 = highest spender)
totalScore      int     3-15 (sum of R+F+M)
segment         string  RFM segment name
```

#### SegmentBriefDto
```
segmentId   Guid        Segment ID
name        string      English name
nameAr      string      Arabic name
colorHex    string?     Hex color code
joinedAt    DateTime    When customer joined segment
```

#### TopInterestsQueryDto (query params)
```
limit       int         Default: 10
dateFrom    DateTime?   Filter from date
dateTo      DateTime?   Filter to date
```

#### InterestCategorySummaryDto
```
categoryId      Guid        Category ID
categoryName    string      English name
categoryNameAr  string      Arabic name
categoryColor   string?     Hex color code
customerCount   int         Customers interested
totalOrders     int         Total orders in category
totalRevenue    decimal     Total revenue
averageScore    decimal     Average interest score
trendDirection  string      TrendDirection enum name
```

#### CustomerSegmentSummaryDto
```
segmentId           Guid        Segment ID
name                string      English name
nameAr              string      Arabic name
colorHex            string?     Hex color code
memberCount         int         Number of members
percentageOfTotal   decimal     % of all customers
```

#### CustomerEngagementDto
```
customerId              Guid        Customer ID
engagementLevel         string      EngagementLevel enum name
engagementScore         decimal     Numeric score
lastInteraction         DateTime?   Last interaction time
interactionCount30Days  int         Interactions in past 30 days
isAtRisk                bool        Flagged as at-risk
```

### Admin Segment DTOs

#### AdminSegmentDto
```
id              Guid        Segment ID
name            string      English name
nameAr          string      Arabic name
segmentType     string      SegmentType enum name
description     string?     Description
colorHex        string?     Hex color code
isAutomatic     bool        Auto-assign enabled
memberCount     int         Number of members
isActive        bool        Active status
lastRefreshedAt DateTime?   Last refresh timestamp
createdAt       DateTime    Creation timestamp
```

#### AdminSegmentDetailDto (extends AdminSegmentDto)
```
rules                       string?                JSON rules definition
minScore                    decimal?               Min score threshold
maxScore                    decimal?               Max score threshold
topInterests                List<string>           Top interest category names
engagementDistribution      Dictionary<string,int> Engagement level counts
```

#### CreateSegmentDto
```
name            string      Required
nameAr          string      Required
segmentType     string      Required (SegmentType enum name)
description     string?     Optional
colorHex        string?     Optional
rules           string?     Optional (JSON)
isAutomatic     bool        Required
minScore        decimal?    Optional
maxScore        decimal?    Optional
```

#### UpdateSegmentDto
```
name            string?     Optional
nameAr          string?     Optional
segmentType     string?     Optional
description     string?     Optional
colorHex        string?     Optional
rules           string?     Optional
isAutomatic     bool?       Optional
minScore        decimal?    Optional
maxScore        decimal?    Optional
```

#### SegmentFilterDto (extends PaginationDto)
```
page            int         Default: 1
pageSize        int         Default: 20
segmentType     string?     Filter by SegmentType
isActive        bool?       Filter by active status
searchTerm      string?     Search by name
```

#### SegmentMemberDto
```
customerId      Guid        Customer ID
customerName    string?     Customer name
customerPhone   string      Customer phone
driverName      string      Assigned driver name
joinedAt        DateTime    Segment join date
score           decimal?    Customer score
totalOrders     int         Order count
lastOrderDate   DateTime?   Last order date
engagementLevel string      EngagementLevel enum name
```

#### SegmentAnalyticsDto
```
totalSegments               int
automaticSegments           int
manualSegments              int
totalMembers                int
averageMembersPerSegment    decimal
growthTrend                 List<SegmentGrowthDto>
distribution                List<SegmentDistributionItemDto>
```

#### SegmentGrowthDto
```
date            DateOnly    Date
newMembers      int         New members added
removedMembers  int         Members removed
netGrowth       int         Net change
```

#### SegmentDistributionItemDto
```
segmentName     string      Segment name
colorHex        string?     Color hex
memberCount     int         Member count
percentage      decimal     % of total
```

### Admin Campaign DTOs

#### AdminCampaignDto
```
id              Guid        Campaign ID
name            string      Campaign name
campaignType    string      CampaignType enum name
segmentName     string?     Target segment name
categoryName    string?     Target category name
channel         string      Delivery channel
targetCount     int         Total targets
sentCount       int         Messages sent
openCount       int         Messages opened
conversionCount int         Conversions
conversionRate  decimal     Conversion %
status          string      CampaignStatus enum name
scheduledAt     DateTime?   Scheduled launch time
createdAt       DateTime    Creation timestamp
```

#### AdminCampaignDetailDto (extends AdminCampaignDto)
```
messageTemplate string      Message template content
startedAt       DateTime?   Actual start time
completedAt     DateTime?   Completion time
createdByName   string      Admin who created it
```

#### CreateCampaignDto
```
name            string      Required
campaignType    string      Required (CampaignType enum name)
segmentId       Guid?       Optional target segment
categoryId      Guid?       Optional target category
messageTemplate string      Required
channel         string      Required (Push, SMS, Email)
scheduledAt     DateTime?   Optional schedule time
```

#### UpdateCampaignDto
```
name            string?     Optional
messageTemplate string?     Optional
channel         string?     Optional
scheduledAt     DateTime?   Optional
```

#### CampaignFilterDto (extends PaginationDto)
```
page            int         Default: 1
pageSize        int         Default: 20
campaignType    string?     Filter by CampaignType
status          string?     Filter by CampaignStatus
dateFrom        DateTime?   Filter from date
dateTo          DateTime?   Filter to date
```

#### CampaignStatsDto
```
totalCampaigns          int
activeCampaigns         int
completedCampaigns      int
draftCampaigns          int
averageConversionRate   decimal
totalMessagesSent       int
totalConversions        int
```

#### CampaignAnalyticsDto
```
totalSent           int
totalOpened         int
totalConversions    int
openRate            decimal     % opened
conversionRate      decimal     % converted
revenueGenerated    decimal     Revenue from conversions
dailyStats          List<CampaignDailyStatDto>
```

#### CampaignDailyStatDto
```
date            DateOnly    Date
sent            int         Sent that day
opened          int         Opened that day
conversions     int         Conversions that day
```

### Admin Insights DTOs

#### InsightsOverviewDto
```
totalCustomers                  int
activeCustomers                 int
atRiskCustomers                 int
churnedCustomers                int
averageEngagementScore          decimal
averageLifetimeValue            decimal
totalSegments                   int
totalCategories                 int
totalSignalsToday               int
totalRecommendationsGenerated   int
```

#### InterestHeatmapDto
```
cells       List<HeatmapCellDto>
```

#### HeatmapCellDto
```
categoryName    string      Category name
segmentName     string      Segment name
score           decimal     Interest score
customerCount   int         Number of customers
```

#### InterestTrendDto
```
categoryId      Guid        Category ID
categoryName    string      English name
categoryNameAr  string      Arabic name
categoryColor   string?     Hex color
trendDirection  string      TrendDirection enum name
changePercent   decimal     % change in period
dataPoints      List<TrendDataPointDto>
```

#### TrendDataPointDto
```
date    DateOnly    Date
value   decimal     Score value
count   int         Signal count
```

#### TrendsQueryDto (query params)
```
dateFrom    DateTime?   Start date
dateTo      DateTime?   End date
period      string      "daily" (default), "weekly", "monthly"
limit       int?        Max categories
```

#### EngagementDistributionDto
```
highEngagement      int         Count
mediumEngagement    int         Count
lowEngagement       int         Count
atRisk              int         Count
churned             int         Count
highPercentage      decimal     %
mediumPercentage    decimal     %
lowPercentage       decimal     %
atRiskPercentage    decimal     %
churnedPercentage   decimal     %
```

#### RfmAnalysisDto
```
totalCustomersAnalyzed      int
averageRecencyScore         decimal
averageFrequencyScore       decimal
averageMonetaryScore        decimal
segmentDistribution         Dictionary<string, int>
segments                    List<RfmSegmentDetailDto>
```

#### RfmSegmentDetailDto
```
segmentName         string
customerCount       int
averageRecency      decimal
averageFrequency    decimal
averageMonetary     decimal
percentage          decimal
```

#### GlobalBehaviorSummaryDto
```
mostPopularOrderTime        string
mostPopularDayOfWeek        string
averageOrderValue           decimal
averageOrderFrequency       decimal
mostPopularPaymentMethod    string
topAreas                    List<string>
spendingTierDistribution    Dictionary<string, decimal>
paymentMethodDistribution   Dictionary<string, int>
```

#### CategoryPerformanceDto
```
categoryId          Guid
categoryName        string
categoryNameAr      string
categoryColor       string?
totalOrders         int
totalRevenue        decimal
uniqueCustomers     int
averageOrderValue   decimal
growthPercent       decimal
trendDirection      string      TrendDirection enum name
```

---

## 11. Flutter Integration

### Dio Setup

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Add token interceptor (see AUTH_API.md for full setup)
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

### Get Customer Profile

```dart
Future<CustomerInterestProfile?> getCustomerProfile(String customerId) async {
  try {
    final response = await dio.get('/customer-insights/$customerId/profile');
    if (response.data['isSuccess'] == true) {
      return CustomerInterestProfile.fromJson(response.data['data']);
    }
    showError(response.data['message']);
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Get Customer Interests

```dart
Future<List<CustomerInterest>> getCustomerInterests(String customerId) async {
  try {
    final response = await dio.get('/customer-insights/$customerId/interests');
    if (response.data['isSuccess'] == true) {
      return (response.data['data'] as List)
          .map((e) => CustomerInterest.fromJson(e))
          .toList();
    }
    return [];
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return [];
  }
}
```

### Get Recommendations and Handle Actions

```dart
Future<List<CustomerRecommendation>> getRecommendations(String customerId) async {
  try {
    final response = await dio.get('/customer-insights/$customerId/recommendations');
    if (response.data['isSuccess'] == true) {
      return (response.data['data'] as List)
          .map((e) => CustomerRecommendation.fromJson(e))
          .toList();
    }
    return [];
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return [];
  }
}

// Mark recommendation as read
Future<bool> markRecommendationRead(String recommendationId) async {
  try {
    final response = await dio.put(
      '/customer-insights/recommendations/$recommendationId/read',
    );
    return response.data['isSuccess'] == true;
  } on DioException {
    return false;
  }
}

// Dismiss recommendation
Future<bool> dismissRecommendation(String recommendationId) async {
  try {
    final response = await dio.put(
      '/customer-insights/recommendations/$recommendationId/dismiss',
    );
    return response.data['isSuccess'] == true;
  } on DioException {
    return false;
  }
}

// Mark recommendation as acted upon
Future<bool> markRecommendationActed(String recommendationId) async {
  try {
    final response = await dio.put(
      '/customer-insights/recommendations/$recommendationId/act',
    );
    return response.data['isSuccess'] == true;
  } on DioException {
    return false;
  }
}
```

### Get Behavior Summary

```dart
Future<CustomerBehaviorSummary?> getBehaviorSummary(String customerId) async {
  try {
    final response = await dio.get('/customer-insights/$customerId/behavior');
    if (response.data['isSuccess'] == true) {
      return CustomerBehaviorSummary.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Get Top Interests with Filters

```dart
Future<List<InterestCategorySummary>> getTopInterests({
  int limit = 10,
  DateTime? dateFrom,
  DateTime? dateTo,
}) async {
  try {
    final queryParams = <String, dynamic>{'limit': limit};
    if (dateFrom != null) queryParams['dateFrom'] = dateFrom.toIso8601String();
    if (dateTo != null) queryParams['dateTo'] = dateTo.toIso8601String();

    final response = await dio.get(
      '/customer-insights/top-interests',
      queryParameters: queryParams,
    );
    if (response.data['isSuccess'] == true) {
      return (response.data['data'] as List)
          .map((e) => InterestCategorySummary.fromJson(e))
          .toList();
    }
    return [];
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return [];
  }
}
```

### Get Segments and Segment Customers

```dart
Future<List<CustomerSegmentSummary>> getSegments() async {
  try {
    final response = await dio.get('/customer-insights/segments');
    if (response.data['isSuccess'] == true) {
      return (response.data['data'] as List)
          .map((e) => CustomerSegmentSummary.fromJson(e))
          .toList();
    }
    return [];
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return [];
  }
}

Future<PagedResult<dynamic>?> getSegmentCustomers(
  String segmentId, {
  int page = 1,
  int pageSize = 20,
}) async {
  try {
    final response = await dio.get(
      '/customer-insights/segments/$segmentId/customers',
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    if (response.data['isSuccess'] == true) {
      return PagedResult.fromJson(response.data['data']);
    }
    return null;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return null;
  }
}
```

### Recommendation Lifecycle Widget Pattern

```dart
/// Example: Showing recommendations with swipe-to-dismiss
class RecommendationCard extends StatelessWidget {
  final CustomerRecommendation recommendation;
  final VoidCallback onDismiss;
  final VoidCallback onAct;

  // Call markRecommendationRead() when card becomes visible
  // Call dismissRecommendation() on swipe left
  // Call markRecommendationActed() on tap/CTA button

  // Recommendation status flow:
  //   Active -> Read (auto on view)
  //   Read -> Dismissed (swipe)
  //   Read -> ActedUpon (tap CTA)
  //   Active -> Expired (server-side, check expiresAt)
}
```

### Model Classes Example

```dart
class CustomerInterestProfile {
  final String customerId;
  final String? customerName;
  final String customerPhone;
  final String engagementLevel;
  final double lifetimeValue;
  final int totalOrders;
  final List<CustomerInterest> topInterests;
  final List<SegmentBrief> currentSegments;
  final CustomerBehaviorSummary? behaviorSummary;
  final DateTime? lastOrderDate;
  final int? daysSinceLastOrder;
  final RfmScore? rfmScore;

  CustomerInterestProfile.fromJson(Map<String, dynamic> json)
      : customerId = json['customerId'],
        customerName = json['customerName'],
        customerPhone = json['customerPhone'],
        engagementLevel = json['engagementLevel'],
        lifetimeValue = (json['lifetimeValue'] as num).toDouble(),
        totalOrders = json['totalOrders'],
        topInterests = (json['topInterests'] as List)
            .map((e) => CustomerInterest.fromJson(e))
            .toList(),
        currentSegments = (json['currentSegments'] as List)
            .map((e) => SegmentBrief.fromJson(e))
            .toList(),
        behaviorSummary = json['behaviorSummary'] != null
            ? CustomerBehaviorSummary.fromJson(json['behaviorSummary'])
            : null,
        lastOrderDate = json['lastOrderDate'] != null
            ? DateTime.parse(json['lastOrderDate'])
            : null,
        daysSinceLastOrder = json['daysSinceLastOrder'],
        rfmScore = json['rfmScore'] != null
            ? RfmScore.fromJson(json['rfmScore'])
            : null;
}

class CustomerInterest {
  final String categoryId;
  final String categoryName;
  final String categoryNameAr;
  final String? categoryColor;
  final double score;
  final int signalCount;
  final String trendDirection;
  final double confidenceLevel;
  final DateTime? lastSignalAt;

  CustomerInterest.fromJson(Map<String, dynamic> json)
      : categoryId = json['categoryId'],
        categoryName = json['categoryName'],
        categoryNameAr = json['categoryNameAr'],
        categoryColor = json['categoryColor'],
        score = (json['score'] as num).toDouble(),
        signalCount = json['signalCount'],
        trendDirection = json['trendDirection'],
        confidenceLevel = (json['confidenceLevel'] as num).toDouble(),
        lastSignalAt = json['lastSignalAt'] != null
            ? DateTime.parse(json['lastSignalAt'])
            : null;
}

class RfmScore {
  final int recencyScore;
  final int frequencyScore;
  final int monetaryScore;
  final int totalScore;
  final String segment;

  RfmScore.fromJson(Map<String, dynamic> json)
      : recencyScore = json['recencyScore'],
        frequencyScore = json['frequencyScore'],
        monetaryScore = json['monetaryScore'],
        totalScore = json['totalScore'],
        segment = json['segment'];
}

class CustomerRecommendation {
  final String id;
  final String recommendationType;
  final String title;
  final String message;
  final String? categoryName;
  final String? categoryColor;
  final double relevanceScore;
  final String status;
  final DateTime? expiresAt;
  final DateTime createdAt;

  CustomerRecommendation.fromJson(Map<String, dynamic> json)
      : id = json['id'],
        recommendationType = json['recommendationType'],
        title = json['title'],
        message = json['message'],
        categoryName = json['categoryName'],
        categoryColor = json['categoryColor'],
        relevanceScore = (json['relevanceScore'] as num).toDouble(),
        status = json['status'],
        expiresAt = json['expiresAt'] != null
            ? DateTime.parse(json['expiresAt'])
            : null,
        createdAt = DateTime.parse(json['createdAt']);
}
```
