# Sekka API - Health Score Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/health-score`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` header

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Health Score](#21-get-health-score)
   - [Get Health Tips](#22-get-health-tips)
3. [DTOs Reference](#3-dtos-reference)

---

## 1. Overview

The Health Score API provides an account health assessment for drivers. The score is calculated based on multiple factors: success rate, customer ratings, commitment, activity level, and cash handling.

The overall score ranges from **0 to 100**, with status categories:
- **Excellent** (80-100)
- **Good** (60-79)
- **Fair** (40-59)
- **Poor** (0-39)

---

## 2. Endpoints

### 2.1 Get Health Score

```
GET /api/v1/health-score
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "overallScore": 85,
    "successRateScore": 90,
    "customerRatingScore": 88,
    "commitmentScore": 80,
    "activityScore": 82,
    "cashHandlingScore": 85,
    "status": "Excellent",
    "lastCalculatedAt": "2026-03-07T12:00:00Z",
    "trend": "Up"
  }
}
```

---

### 2.2 Get Health Tips

```
GET /api/v1/health-score/tips
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "category": "SuccessRate",
      "title": "حسّن معدل التسليم",
      "description": "حاول تقليل الطلبات الملغاة عن طريق التأكد من تفاصيل العنوان",
      "currentValue": 88.5,
      "targetValue": 95.0,
      "impactOnScore": 5,
      "priority": 1
    },
    {
      "category": "CashHandling",
      "title": "سلّم الكاش بانتظام",
      "description": "لا تتجاوز حد الكاش المسموح",
      "currentValue": 1800.00,
      "targetValue": 2000.00,
      "impactOnScore": 3,
      "priority": 2
    }
  ]
}
```

---

## 3. DTOs Reference

### AccountHealthDto
| Field | Type | Description |
|-------|------|-------------|
| overallScore | int | Overall health score (0-100) |
| successRateScore | int | Delivery success rate component |
| customerRatingScore | int | Customer ratings component |
| commitmentScore | int | Shift commitment component |
| activityScore | int | Activity level component |
| cashHandlingScore | int | Cash handling component |
| status | string | Excellent / Good / Fair / Poor |
| lastCalculatedAt | DateTime | Last calculation timestamp |
| trend | string | Up / Down / Stable |

### HealthTipDto
| Field | Type | Description |
|-------|------|-------------|
| category | string | Score category this tip relates to |
| title | string | Tip title |
| description | string | Detailed tip description |
| currentValue | decimal | Driver's current metric value |
| targetValue | decimal | Target value to aim for |
| impactOnScore | int | Points gained if target reached |
| priority | int | Tip priority (1 = highest) |

---

## Flutter/Dio Integration

```dart
// Get health score
final health = await dio.get('/api/v1/health-score');
final score = health.data['data']['overallScore'];
final status = health.data['data']['status'];

// Get improvement tips
final tips = await dio.get('/api/v1/health-score/tips');
final tipsList = tips.data['data'] as List;
```
