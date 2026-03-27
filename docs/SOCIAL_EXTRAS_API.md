# Sekka API - Phase 9: Social & Extras Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-27

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Enums](#3-enums)
4. [GamificationController](#4-gamificationcontroller) (7 endpoints)
5. [ReferralController](#5-referralcontroller) (4 endpoints)
6. [SearchController](#6-searchcontroller) (1 endpoint)
7. [ShiftController](#7-shiftcontroller) (4 endpoints)
8. [SavingsCircleController](#8-savingscirclecontroller) (8 endpoints)
9. [ColleagueRadarController](#9-colleagueradarcontroller) (6 endpoints)
10. [RoadReportController](#10-roadreportcontroller) (6 endpoints)
11. [AdminSavingsCirclesController](#11-adminsavingscirclescontroller) (11 endpoints)
12. [DTOs Reference](#12-dtos-reference)
13. [Savings Circle Lifecycle](#13-savings-circle-lifecycle)
14. [Shift Flow](#14-shift-flow)
15. [Flutter / Dio Examples](#15-flutter--dio-examples)

---

## 1. Overview

Phase 9 covers the social engagement and extras layer of the Sekka platform: gamification, referrals, global search, driver shifts, savings circles (game'ya), colleague radar with roadside assistance, and road reports.

| Feature | Detail |
|---------|--------|
| Total Endpoints | **47** |
| Auth Required | All (Bearer JWT) |
| Admin Endpoints | 11 (AdminSavingsCircles) |
| Pagination | `pageNumber` & `pageSize` query params |

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
| 201 | Created |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid credentials/token) |
| 403 | Forbidden (admin only) |
| 404 | Not Found |
| 409 | Conflict (duplicate action) |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Enums

### ChallengeType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Daily | يومي |
| 1 | Weekly | أسبوعي |
| 2 | Monthly | شهري |

### ReferralStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | في الانتظار |
| 1 | Registered | مسجل |
| 2 | Rewarded | تمت المكافأة |
| 3 | Expired | منتهي |

### RewardType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | FreePremiumMonth | شهر بريميوم مجاني |
| 1 | Points | نقاط |
| 2 | Cash | كاش |

### ShiftStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | OffShift | خارج الشيفت |
| 1 | OnShift | في الشيفت |
| 2 | OnBreak | في استراحة |

### CircleStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Forming | قيد التشكيل |
| 1 | Active | نشطة |
| 2 | Completed | مكتملة |
| 3 | Frozen | مجمّدة |
| 4 | Dissolved | منحلّة |

### CircleMemberStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Active | نشط |
| 1 | Left | غادر |
| 2 | Removed | تم الإزالة |
| 3 | Defaulted | متعثّر |

### CirclePaymentStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | في الانتظار |
| 1 | Paid | مدفوع |
| 2 | Late | متأخر |
| 3 | Missed | فائت |

### AssistanceType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | FlatTire | إطار مثقوب |
| 1 | FuelEmpty | نفاد الوقود |
| 2 | Accident | حادث |
| 3 | MechanicalIssue | عطل ميكانيكي |
| 4 | Other | أخرى |

### AssistanceStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | في الانتظار |
| 1 | Accepted | مقبول |
| 2 | Resolved | تم الحل |
| 3 | Cancelled | ملغي |
| 4 | Expired | منتهي |

### RoadReportType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | TrafficJam | زحمة |
| 1 | Accident | حادث |
| 2 | RoadClosed | طريق مغلق |
| 3 | Construction | أعمال إنشاء |
| 4 | PoliceCheckpoint | كمين |
| 5 | Hazard | خطر |
| 6 | Flood | فيضان |
| 7 | Other | أخرى |

### ReportSeverity
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Low | منخفض |
| 1 | Medium | متوسط |
| 2 | High | مرتفع |
| 3 | Critical | حرج |

### SubscriptionStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Trial | تجربة |
| 1 | Active | نشط |
| 2 | Expired | منتهي |
| 3 | Cancelled | ملغي |
| 4 | Suspended | معلّق |

---

## 4. GamificationController

**Base**: `/api/v1/gamification`
**Auth**: All endpoints require Bearer token

---

### 4.1 Get Active Challenges

Returns challenges available for the current driver.

```
GET /gamification/challenges
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "سائق البرق",
      "description": "أكمل 10 طلبات اليوم",
      "challengeType": 0,
      "targetValue": 10.0,
      "currentProgress": 4.0,
      "progressPercentage": 40.0,
      "rewardPoints": 50,
      "badgeName": "البرق",
      "isCompleted": false
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.2 Get Achievements

Returns all completed achievements/badges for the driver.

```
GET /gamification/achievements
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "challengeName": "المحارب الأسبوعي",
      "badgeName": "المحارب",
      "badgeIconUrl": "https://sekka.runasp.net/badges/warrior.png",
      "pointsEarned": 200,
      "completedAt": "2026-03-20T14:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.3 Get Leaderboard

Returns the leaderboard with the driver's rank.

```
GET /gamification/leaderboard?period=monthly
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Default | Values |
|-------|------|---------|--------|
| period | string | `"monthly"` | `"daily"`, `"weekly"`, `"monthly"` |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "myRank": 7,
    "myPoints": 1250,
    "topDrivers": [
      {
        "rank": 1,
        "driverName": "محمد أحمد",
        "points": 3200,
        "level": 12,
        "ordersThisMonth": 210
      },
      {
        "rank": 2,
        "driverName": "كريم حسن",
        "points": 2800,
        "level": 10,
        "ordersThisMonth": 185
      }
    ]
  },
  "message": null,
  "errors": null
}
```

---

### 4.4 Claim Challenge Reward

Claims the reward for a completed challenge.

```
POST /gamification/challenges/{challengeId}/claim
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `challengeId` (GUID) - Challenge ID

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم استلام المكافأة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `التحدي غير مكتمل بعد` |
| 404 | `التحدي غير موجود` |
| 409 | `تم استلام المكافأة مسبقًا` |

---

### 4.5 Get Points History

Returns paginated history of points earned/spent.

```
GET /gamification/points/history?pageNumber=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Default |
|-------|------|---------|
| pageNumber | int | 1 |
| pageSize | int | 10 |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "points": 50,
        "reason": "إكمال تحدي سائق البرق",
        "referenceType": "Challenge",
        "referenceId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "createdAt": "2026-03-25T10:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  },
  "message": null,
  "errors": null
}
```

---

### 4.6 Get Total Points

Returns the driver's current total points balance.

```
GET /gamification/points/total
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": 1250,
  "message": null,
  "errors": null
}
```

---

### 4.7 Get Current Level

Returns the driver's current level and progress.

```
GET /gamification/level
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "level": 5,
    "currentPoints": 1250,
    "pointsToNextLevel": 250,
    "nextLevelThreshold": 1500
  },
  "message": null,
  "errors": null
}
```

---

## 5. ReferralController

**Base**: `/api/v1/referrals`
**Auth**: All endpoints require Bearer token

---

### 5.1 Get My Referral Code

Returns the driver's referral code and shareable link.

```
GET /referrals/my-code
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "referralCode": "3FA85F64",
    "shareUrl": "https://sekka.app/join?ref=3FA85F64"
  },
  "message": null,
  "errors": null
}
```

---

### 5.2 Get Referral Stats

Returns referral statistics for the driver.

```
GET /referrals/stats
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "referralCode": "3FA85F64",
    "totalReferrals": 12,
    "completedReferrals": 8,
    "pendingReferrals": 4,
    "totalPointsEarned": 800
  },
  "message": null,
  "errors": null
}
```

---

### 5.3 Apply Referral Code

Applies a referral code from another driver. Can only be used once per account.

```
POST /referrals/apply
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "referralCode": "ABC12345"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| referralCode | string | Yes | Valid referral code |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تطبيق كود الإحالة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكنك استخدام كود الإحالة الخاص بك` |
| 400 | `تم استخدام كود إحالة مسبقًا` |
| 404 | `كود الإحالة غير موجود` |

---

### 5.4 Get My Referrals

Returns a list of all referrals made by the driver.

```
GET /referrals
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "referrerDriverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "referredDriverId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "referralCode": "3FA85F64",
      "referredPhone": "+20101234****",
      "status": 2,
      "rewardType": 1,
      "rewardGiven": true,
      "registeredAt": "2026-03-10T08:00:00Z",
      "rewardedAt": "2026-03-17T12:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 6. SearchController

**Base**: `/api/v1/search`
**Auth**: Bearer token required

> **Note**: This feature is currently under development (stubbed). All calls return `400` with a development message.

---

### 6.1 Omni Search

Global search across orders, customers, and partners.

```
GET /search?query=أحمد
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Required |
|-------|------|----------|
| query | string | Yes |

**Response** `200 OK` (when implemented):
```json
{
  "isSuccess": true,
  "data": {
    "orders": [],
    "customers": [],
    "partners": [],
    "totalResults": 0
  },
  "message": null,
  "errors": null
}
```

**Current Response** `400`:
```json
{
  "isSuccess": false,
  "data": null,
  "message": "ميزة البحث الشامل قيد التطوير حاليًا",
  "errors": null
}
```

---

## 7. ShiftController

**Base**: `/api/v1/shifts`
**Auth**: All endpoints require Bearer token

---

### 7.1 Start Shift

Starts a new shift at the driver's current location.

```
POST /shifts/start
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "latitude": 30.0444,
  "longitude": 31.2357
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| latitude | double | Yes | Valid latitude (-90 to 90) |
| longitude | double | Yes | Valid longitude (-180 to 180) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": 1,
    "startTime": "2026-03-27T08:00:00Z",
    "endTime": null,
    "startLatitude": 30.0444,
    "startLongitude": 31.2357,
    "endLatitude": null,
    "endLongitude": null,
    "ordersCompleted": 0,
    "earningsTotal": 0.0,
    "distanceKm": 0.0
  },
  "message": "تم بدء الشيفت بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 409 | `يوجد شيفت نشط بالفعل` |

---

### 7.2 End Shift

Ends the current active shift.

```
POST /shifts/end
```

**Auth Required**: Yes (Bearer Token)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": 0,
    "startTime": "2026-03-27T08:00:00Z",
    "endTime": "2026-03-27T16:30:00Z",
    "startLatitude": 30.0444,
    "startLongitude": 31.2357,
    "endLatitude": 30.0561,
    "endLongitude": 31.2394,
    "ordersCompleted": 14,
    "earningsTotal": 450.00,
    "distanceKm": 78.5
  },
  "message": "تم إنهاء الشيفت بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `لا يوجد شيفت نشط حاليًا` |

---

### 7.3 Get Current Shift

Returns the driver's currently active shift (if any).

```
GET /shifts/current
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": 1,
    "startTime": "2026-03-27T08:00:00Z",
    "endTime": null,
    "startLatitude": 30.0444,
    "startLongitude": 31.2357,
    "endLatitude": null,
    "endLongitude": null,
    "ordersCompleted": 7,
    "earningsTotal": 210.00,
    "distanceKm": 35.2
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `لا يوجد شيفت نشط حاليًا` |

---

### 7.4 Get Shift Summary

Returns aggregated shift statistics for a date range.

```
GET /shifts/summary?from=2026-03-01&to=2026-03-27
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Required | Format |
|-------|------|----------|--------|
| from | DateOnly | No | `yyyy-MM-dd` |
| to | DateOnly | No | `yyyy-MM-dd` |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalShifts": 22,
    "totalHoursWorked": 176.5,
    "totalOrdersCompleted": 308,
    "totalEarnings": 9240.00,
    "totalDistanceKm": 1720.8,
    "averageShiftDurationHours": 8.02
  },
  "message": null,
  "errors": null
}
```

---

## 8. SavingsCircleController

**Base**: `/api/v1/savings-circles`
**Auth**: All endpoints require Bearer token

> Savings Circles (Game'ya / جمعية) allow groups of drivers to pool monthly savings and take turns receiving the full pot.

---

### 8.1 Create Circle

Creates a new savings circle. The creator becomes the first member.

```
POST /savings-circles
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "name": "جمعية السائقين",
  "monthlyAmount": 500.00,
  "maxMembers": 10,
  "durationMonths": 10,
  "minHealthScore": 80
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| name | string | Yes | Not empty |
| monthlyAmount | decimal | Yes | > 0 |
| maxMembers | int | Yes | >= 2 |
| durationMonths | int | Yes | >= 2, typically equals maxMembers |
| minHealthScore | int | No | Default 80 (0-100) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "جمعية السائقين",
    "monthlyAmount": 500.00,
    "maxMembers": 10,
    "currentMembersCount": 1,
    "durationMonths": 10,
    "currentRound": 0,
    "status": 0,
    "minHealthScore": 80,
    "startDate": null,
    "createdAt": "2026-03-27T10:00:00Z"
  },
  "message": "تم إنشاء الحلقة بنجاح",
  "errors": null
}
```

---

### 8.2 Get Available Circles

Returns paginated list of circles open for joining (status = Forming).

```
GET /savings-circles/available?pageNumber=1&pageSize=10
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Default |
|-------|------|---------|
| pageNumber | int | 1 |
| pageSize | int | 10 |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "جمعية السائقين",
        "monthlyAmount": 500.00,
        "maxMembers": 10,
        "currentMembersCount": 6,
        "durationMonths": 10,
        "currentRound": 0,
        "status": 0,
        "minHealthScore": 80,
        "startDate": null,
        "createdAt": "2026-03-20T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 3,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

---

### 8.3 Get Circle By ID

Returns full details of a specific circle including members and recent payments.

```
GET /savings-circles/{circleId}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `circleId` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "جمعية السائقين",
    "monthlyAmount": 500.00,
    "maxMembers": 10,
    "durationMonths": 10,
    "currentRound": 3,
    "status": 1,
    "minHealthScore": 80,
    "startDate": "2026-01-01T00:00:00Z",
    "creatorDriverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "members": [
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "driverName": "أحمد محمد",
        "turnOrder": 1,
        "status": 0,
        "joinedAt": "2026-03-20T10:00:00Z"
      }
    ],
    "recentPayments": [
      {
        "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "memberId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "memberName": "أحمد محمد",
        "roundNumber": 3,
        "amount": 500.00,
        "status": 1,
        "paidAt": "2026-03-25T09:00:00Z"
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
| 404 | `الحلقة غير موجودة` |

---

### 8.4 Get My Circles

Returns all circles the driver is a member of.

```
GET /savings-circles/my
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "جمعية السائقين",
      "monthlyAmount": 500.00,
      "maxMembers": 10,
      "currentMembersCount": 10,
      "durationMonths": 10,
      "currentRound": 3,
      "status": 1,
      "minHealthScore": 80,
      "startDate": "2026-01-01T00:00:00Z",
      "createdAt": "2025-12-15T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 8.5 Join Circle

Joins an available savings circle. Driver must meet the minimum health score.

```
POST /savings-circles/{circleId}/join
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `circleId` (GUID)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم الانضمام للحلقة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الحلقة ممتلئة` |
| 400 | `مستوى الصحة المالية غير كافي` |
| 404 | `الحلقة غير موجودة` |
| 409 | `أنت عضو في هذه الحلقة بالفعل` |

---

### 8.6 Leave Circle

Leaves a savings circle. Only allowed while the circle is still in Forming status.

```
POST /savings-circles/{circleId}/leave
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `circleId` (GUID)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم مغادرة الحلقة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكن المغادرة بعد بدء الحلقة` |
| 404 | `الحلقة غير موجودة` |
| 404 | `لست عضوًا في هذه الحلقة` |

---

### 8.7 Make Payment

Makes the monthly payment for the current round.

```
POST /savings-circles/{circleId}/pay
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `circleId` (GUID)

**Request Body**: None

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم الدفع بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `الحلقة غير نشطة` |
| 400 | `تم الدفع لهذه الجولة مسبقًا` |
| 404 | `الحلقة غير موجودة` |

---

### 8.8 Get Circle Payments

Returns all payments for a specific circle.

```
GET /savings-circles/{circleId}/payments
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `circleId` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "memberId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "memberName": "أحمد محمد",
      "roundNumber": 1,
      "amount": 500.00,
      "status": 1,
      "paidAt": "2026-01-28T09:00:00Z"
    },
    {
      "id": "d4e5f6a7-b890-1234-defg-234567890123",
      "memberId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "memberName": "أحمد محمد",
      "roundNumber": 2,
      "amount": 500.00,
      "status": 0,
      "paidAt": null
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 9. ColleagueRadarController

**Base**: `/api/v1/colleague-radar`
**Auth**: All endpoints require Bearer token

> Nearby driver discovery and roadside assistance requests.

---

### 9.1 Get Nearby Drivers

Returns drivers near the specified location.

```
GET /colleague-radar/nearby?latitude=30.0444&longitude=31.2357&radiusKm=5
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Required | Default |
|-------|------|----------|---------|
| latitude | double | Yes | - |
| longitude | double | Yes | - |
| radiusKm | double | No | 5 |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "driverName": "كريم حسن",
      "latitude": 30.0480,
      "longitude": 31.2370,
      "distanceKm": 0.8,
      "isAvailable": true,
      "vehicleType": "Car"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 9.2 Create Help Request

Creates a roadside assistance request broadcast to nearby drivers.

```
POST /colleague-radar/help-requests
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "title": "إطار مثقوب",
  "description": "محتاج مساعدة في تغيير الإطار",
  "latitude": 30.0444,
  "longitude": 31.2357,
  "helpType": "FlatTire"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| title | string | Yes | Not empty |
| description | string | No | Optional details |
| latitude | double | Yes | Valid latitude |
| longitude | double | Yes | Valid longitude |
| helpType | string | Yes | See [AssistanceType](#assistancetype) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "driverName": "أحمد محمد",
    "title": "إطار مثقوب",
    "description": "محتاج مساعدة في تغيير الإطار",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "helpType": "FlatTire",
    "status": "Pending",
    "responderId": null,
    "responderName": null,
    "createdAt": "2026-03-27T14:00:00Z",
    "resolvedAt": null
  },
  "message": "تم إرسال طلب المساعدة بنجاح",
  "errors": null
}
```

---

### 9.3 Get Nearby Help Requests

Returns active help requests near the specified location.

```
GET /colleague-radar/help-requests/nearby?latitude=30.0444&longitude=31.2357&radiusKm=10
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Required | Default |
|-------|------|----------|---------|
| latitude | double | Yes | - |
| longitude | double | Yes | - |
| radiusKm | double | No | 10 |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "driverName": "أحمد محمد",
      "title": "إطار مثقوب",
      "description": "محتاج مساعدة في تغيير الإطار",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "helpType": "FlatTire",
      "status": "Pending",
      "responderId": null,
      "responderName": null,
      "createdAt": "2026-03-27T14:00:00Z",
      "resolvedAt": null
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 9.4 Respond to Help Request

Accept / respond to a help request from another driver.

```
POST /colleague-radar/help-requests/{requestId}/respond
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `requestId` (GUID)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم قبول طلب المساعدة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكنك الاستجابة لطلبك الخاص` |
| 404 | `طلب المساعدة غير موجود` |
| 409 | `تم قبول الطلب من سائق آخر` |

---

### 9.5 Resolve Help Request

Marks a help request as resolved. Only the requester can resolve.

```
POST /colleague-radar/help-requests/{requestId}/resolve
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `requestId` (GUID)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حل المشكلة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 401 | `فقط صاحب الطلب يمكنه إغلاقه` |
| 404 | `طلب المساعدة غير موجود` |

---

### 9.6 Get My Help Requests

Returns all help requests created by the driver.

```
GET /colleague-radar/help-requests/my
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "driverName": "أحمد محمد",
      "title": "إطار مثقوب",
      "description": "محتاج مساعدة في تغيير الإطار",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "helpType": "FlatTire",
      "status": "Resolved",
      "responderId": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "responderName": "كريم حسن",
      "createdAt": "2026-03-27T14:00:00Z",
      "resolvedAt": "2026-03-27T14:45:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 10. RoadReportController

**Base**: `/api/v1/road-reports`
**Auth**: All endpoints require Bearer token

> Community-driven road condition reporting. Reports auto-expire and can be confirmed by other drivers.

---

### 10.1 Create Road Report

Creates a new road condition report at the specified location.

```
POST /road-reports
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "type": 0,
  "latitude": 30.0444,
  "longitude": 31.2357,
  "description": "زحمة شديدة على الدائري",
  "severity": 2
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| type | int | Yes | See [RoadReportType](#roadreporttype) (0-7) |
| latitude | double | Yes | Valid latitude |
| longitude | double | Yes | Valid longitude |
| description | string | No | Optional details |
| severity | int | No | See [ReportSeverity](#reportseverity) (0-3), default `1` (Medium) |

**Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "type": 0,
    "latitude": 30.0444,
    "longitude": 31.2357,
    "description": "زحمة شديدة على الدائري",
    "severity": 2,
    "confirmationsCount": 0,
    "isActive": true,
    "expiresAt": "2026-03-27T16:00:00Z",
    "createdAt": "2026-03-27T14:00:00Z"
  },
  "message": "تم إنشاء البلاغ بنجاح",
  "errors": null
}
```

---

### 10.2 Get Nearby Reports

Returns active road reports near the specified location.

```
GET /road-reports/nearby?latitude=30.0444&longitude=31.2357&radiusKm=10
```

**Auth Required**: Yes (Bearer Token)

**Query Params**:
| Param | Type | Required | Default |
|-------|------|----------|---------|
| latitude | double | Yes | - |
| longitude | double | Yes | - |
| radiusKm | double | No | 10 |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "type": 4,
      "latitude": 30.0500,
      "longitude": 31.2400,
      "description": "كمين على كوبري أكتوبر",
      "severity": 1,
      "confirmationsCount": 5,
      "isActive": true,
      "expiresAt": "2026-03-27T16:30:00Z",
      "createdAt": "2026-03-27T14:30:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 10.3 Confirm Report

Confirms or denies an existing road report. Increases/decreases the confirmation count.

```
POST /road-reports/{reportId}/confirm?isConfirmed=true
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `reportId` (GUID)

**Query Params**:
| Param | Type | Default |
|-------|------|---------|
| isConfirmed | bool | `true` |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تأكيد البلاغ",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `لا يمكنك تأكيد بلاغك الخاص` |
| 404 | `البلاغ غير موجود` |
| 409 | `تم التأكيد مسبقًا` |

---

### 10.4 Get Report By ID

Returns details of a specific road report.

```
GET /road-reports/{reportId}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `reportId` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "type": 0,
    "latitude": 30.0444,
    "longitude": 31.2357,
    "description": "زحمة شديدة على الدائري",
    "severity": 2,
    "confirmationsCount": 8,
    "isActive": true,
    "expiresAt": "2026-03-27T16:00:00Z",
    "createdAt": "2026-03-27T14:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `البلاغ غير موجود` |

---

### 10.5 Get My Reports

Returns all road reports created by the driver.

```
GET /road-reports/my
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "type": 0,
      "latitude": 30.0444,
      "longitude": 31.2357,
      "description": "زحمة شديدة على الدائري",
      "severity": 2,
      "confirmationsCount": 8,
      "isActive": true,
      "expiresAt": "2026-03-27T16:00:00Z",
      "createdAt": "2026-03-27T14:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 10.6 Deactivate Report

Deactivates a report. Only the original reporter can deactivate.

```
POST /road-reports/{reportId}/deactivate
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `reportId` (GUID)

**Request Body**: None

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء البلاغ",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 401 | `فقط صاحب البلاغ يمكنه إلغاؤه` |
| 404 | `البلاغ غير موجود` |

---

## 11. AdminSavingsCirclesController

**Base**: `/api/v1/admin/savings-circles`
**Auth**: Bearer token + `Admin` role required

> **Note**: All admin savings circle endpoints are currently stubbed and return `400` with a development message. The API structure is final.

---

### 11.1 Get All Circles

Returns paginated list of all savings circles.

```
GET /admin/savings-circles?pageNumber=1&pageSize=10
```

**Auth Required**: Yes (Admin Bearer Token)

**Query Params**:
| Param | Type | Default |
|-------|------|---------|
| pageNumber | int | 1 |
| pageSize | int | 10 |

**Response** `200 OK` (when implemented):
```json
{
  "isSuccess": true,
  "data": {
    "items": [ { "...CircleDto" } ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3
  },
  "message": null,
  "errors": null
}
```

---

### 11.2 Get Circle By ID

Returns full details of a specific circle.

```
GET /admin/savings-circles/{id}
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK` (when implemented): Returns `CircleDetailDto` (see [8.3](#83-get-circle-by-id))

---

### 11.3 Approve Circle

Approves a circle to start accepting members / transition to Active.

```
POST /admin/savings-circles/{id}/approve
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم اعتماد الحلقة",
  "errors": null
}
```

---

### 11.4 Reject Circle

Rejects a circle request.

```
POST /admin/savings-circles/{id}/reject
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم رفض الحلقة",
  "errors": null
}
```

---

### 11.5 Freeze Circle

Temporarily freezes an active circle (pauses all rounds).

```
POST /admin/savings-circles/{id}/freeze
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تجميد الحلقة",
  "errors": null
}
```

---

### 11.6 Unfreeze Circle

Resumes a frozen circle.

```
POST /admin/savings-circles/{id}/unfreeze
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء تجميد الحلقة",
  "errors": null
}
```

---

### 11.7 Close Circle

Permanently closes/dissolves a circle.

```
POST /admin/savings-circles/{id}/close
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إغلاق الحلقة",
  "errors": null
}
```

---

### 11.8 Get Circle Members

Returns all members of a specific circle.

```
GET /admin/savings-circles/{id}/members
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK` (when implemented):
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "driverId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "driverName": "أحمد محمد",
      "turnOrder": 1,
      "status": 0,
      "joinedAt": "2026-03-20T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 11.9 Remove Member

Removes a member from a circle.

```
DELETE /admin/savings-circles/{id}/members/{memberId}
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**:
| Param | Type |
|-------|------|
| id | GUID (circle ID) |
| memberId | GUID (member ID) |

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إزالة العضو من الحلقة",
  "errors": null
}
```

---

### 11.10 Get Circle Payments

Returns all payments for a specific circle.

```
GET /admin/savings-circles/{id}/payments
```

**Auth Required**: Yes (Admin Bearer Token)

**URL Params**: `id` (GUID)

**Response** `200 OK` (when implemented): Returns `List<CirclePaymentDto>` (see [8.8](#88-get-circle-payments))

---

### 11.11 Get Savings Circle Stats

Returns aggregate statistics for all savings circles.

```
GET /admin/savings-circles/stats
```

**Auth Required**: Yes (Admin Bearer Token)

**Response** `200 OK` (when implemented):
```json
{
  "isSuccess": true,
  "data": {
    "totalCircles": 25,
    "activeCircles": 12,
    "formingCircles": 5,
    "frozenCircles": 2,
    "totalMembers": 180,
    "totalMoneyPooled": 450000.00
  },
  "message": null,
  "errors": null
}
```

---

## 12. DTOs Reference

### Gamification DTOs

#### ChallengeDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Challenge ID |
| name | string | Challenge name |
| description | string | Challenge description |
| challengeType | int | See [ChallengeType](#challengetype) |
| targetValue | decimal | Target to reach |
| currentProgress | decimal | Driver's current progress |
| progressPercentage | decimal | Progress as 0-100% |
| rewardPoints | int | Points awarded on completion |
| badgeName | string? | Badge name (if awarded) |
| isCompleted | bool | Whether driver completed it |

#### DriverAchievementDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Achievement ID |
| challengeName | string | Challenge that was completed |
| badgeName | string? | Badge earned |
| badgeIconUrl | string? | URL for badge image |
| pointsEarned | int | Points from this achievement |
| completedAt | DateTime | When it was completed |

#### LeaderboardDto
| Field | Type | Description |
|-------|------|-------------|
| myRank | int | Driver's rank |
| myPoints | int | Driver's points |
| topDrivers | List\<LeaderboardEntryDto\> | Top drivers list |

#### LeaderboardEntryDto
| Field | Type | Description |
|-------|------|-------------|
| rank | int | Position |
| driverName | string | Driver name |
| points | int | Total points |
| level | int | Driver level |
| ordersThisMonth | int | Orders completed this month |

#### PointsHistoryDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Entry ID |
| points | int | Points earned (positive) or spent (negative) |
| reason | string | Description of why points changed |
| referenceType | string? | Entity type (e.g., "Challenge", "Order") |
| referenceId | Guid? | Related entity ID |
| createdAt | DateTime | When points were recorded |

### Referral DTOs

#### ReferralCodeDto
| Field | Type | Description |
|-------|------|-------------|
| referralCode | string | The driver's unique referral code |
| shareUrl | string | Shareable deep link URL |

#### ReferralStatsDto
| Field | Type | Description |
|-------|------|-------------|
| referralCode | string | The driver's referral code |
| totalReferrals | int | Total referrals sent |
| completedReferrals | int | Referrals that registered |
| pendingReferrals | int | Referrals still pending |
| totalPointsEarned | int | Points earned from referrals |

#### ApplyReferralCodeDto
| Field | Type | Description |
|-------|------|-------------|
| referralCode | string | Code to apply |

#### ReferralDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Referral ID |
| referrerDriverId | Guid | Who referred |
| referredDriverId | Guid? | Who was referred |
| referralCode | string | Code used |
| referredPhone | string? | Referred phone (masked) |
| status | int | See [ReferralStatus](#referralstatus) |
| rewardType | int | See [RewardType](#rewardtype) |
| rewardGiven | bool | Whether reward was distributed |
| registeredAt | DateTime? | When referred user registered |
| rewardedAt | DateTime? | When reward was given |

### Search DTOs

#### OmniSearchResultDto
| Field | Type | Description |
|-------|------|-------------|
| orders | List\<object\> | Matched orders |
| customers | List\<object\> | Matched customers |
| partners | List\<object\> | Matched partners |
| totalResults | int | Total match count |

### Shift DTOs

#### StartShiftDto
| Field | Type | Description |
|-------|------|-------------|
| latitude | double | Start location latitude |
| longitude | double | Start location longitude |

#### ShiftDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Shift ID |
| driverId | Guid | Driver ID |
| status | int | See [ShiftStatus](#shiftstatus) |
| startTime | DateTime | Shift start time |
| endTime | DateTime? | Shift end time (null if active) |
| startLatitude | double | Start latitude |
| startLongitude | double | Start longitude |
| endLatitude | double? | End latitude |
| endLongitude | double? | End longitude |
| ordersCompleted | int | Orders during this shift |
| earningsTotal | decimal | Total earnings |
| distanceKm | double | Distance driven |

#### ShiftSummaryDto
| Field | Type | Description |
|-------|------|-------------|
| totalShifts | int | Number of shifts in range |
| totalHoursWorked | double | Total hours |
| totalOrdersCompleted | int | Total orders |
| totalEarnings | decimal | Total earnings |
| totalDistanceKm | double | Total distance |
| averageShiftDurationHours | double | Average shift length |

### Savings Circle DTOs

#### CreateCircleDto
| Field | Type | Description |
|-------|------|-------------|
| name | string | Circle name |
| monthlyAmount | decimal | Monthly contribution per member |
| maxMembers | int | Maximum members allowed |
| durationMonths | int | Circle duration in months |
| minHealthScore | int | Minimum financial health score (default 80) |

#### CircleDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Circle ID |
| name | string | Circle name |
| monthlyAmount | decimal | Monthly amount |
| maxMembers | int | Max members |
| currentMembersCount | int | Current member count |
| durationMonths | int | Duration in months |
| currentRound | int | Current round number |
| status | int | See [CircleStatus](#circlestatus) |
| minHealthScore | int | Minimum health score required |
| startDate | DateTime? | When the circle started |
| createdAt | DateTime | Creation date |

#### CircleDetailDto
Extends CircleDto with:
| Field | Type | Description |
|-------|------|-------------|
| creatorDriverId | Guid | Who created the circle |
| members | List\<CircleMemberDto\> | All members |
| recentPayments | List\<CirclePaymentDto\> | Recent payment records |

#### CircleMemberDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Membership ID |
| driverId | Guid | Driver ID |
| driverName | string | Driver name |
| turnOrder | int | Payout turn order |
| status | int | See [CircleMemberStatus](#circlememberstatus) |
| joinedAt | DateTime | When joined |

#### CirclePaymentDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Payment ID |
| memberId | Guid | Member ID |
| memberName | string | Member name |
| roundNumber | int | Which round |
| amount | decimal | Payment amount |
| status | int | See [CirclePaymentStatus](#circlepaymentstatus) |
| paidAt | DateTime? | When paid (null if pending) |

### Colleague Radar DTOs

#### NearbyDriverDto
| Field | Type | Description |
|-------|------|-------------|
| driverId | Guid | Driver ID |
| driverName | string | Driver name |
| latitude | double | Current latitude |
| longitude | double | Current longitude |
| distanceKm | double | Distance from query point |
| isAvailable | bool | Whether driver is available |
| vehicleType | string? | Vehicle type name |

#### CreateHelpRequestDto
| Field | Type | Description |
|-------|------|-------------|
| title | string | Short title |
| description | string? | Detailed description |
| latitude | double | Location latitude |
| longitude | double | Location longitude |
| helpType | string | Type of help needed (see [AssistanceType](#assistancetype)) |

#### HelpRequestDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Request ID |
| driverId | Guid | Requester driver ID |
| driverName | string | Requester name |
| title | string | Request title |
| description | string? | Details |
| latitude | double | Location latitude |
| longitude | double | Location longitude |
| helpType | string | Assistance type |
| status | string | See [AssistanceStatus](#assistancestatus) |
| responderId | Guid? | Responder driver ID |
| responderName | string? | Responder name |
| createdAt | DateTime | When created |
| resolvedAt | DateTime? | When resolved |

### Road Report DTOs

#### CreateRoadReportDto
| Field | Type | Description |
|-------|------|-------------|
| type | int | See [RoadReportType](#roadreporttype) |
| latitude | double | Report location latitude |
| longitude | double | Report location longitude |
| description | string? | Optional description |
| severity | int | See [ReportSeverity](#reportseverity), default Medium (1) |

#### RoadReportDto
| Field | Type | Description |
|-------|------|-------------|
| id | Guid | Report ID |
| driverId | Guid | Reporter driver ID |
| type | int | See [RoadReportType](#roadreporttype) |
| latitude | double | Location latitude |
| longitude | double | Location longitude |
| description | string? | Description |
| severity | int | See [ReportSeverity](#reportseverity) |
| confirmationsCount | int | How many drivers confirmed |
| isActive | bool | Whether report is still active |
| expiresAt | DateTime | Auto-expiry time |
| createdAt | DateTime | When reported |

---

## 13. Savings Circle Lifecycle

### State Diagram

```
                    ┌──────────────┐
                    │   Creator    │
                    │ calls POST   │
                    │ /savings-    │
                    │  circles     │
                    └──────┬───────┘
                           │
                    ┌──────▼───────┐
                    │   Forming    │  ← Members join / leave
                    │   (0)        │
                    └──┬───┬───┬───┘
                       │   │   │
          Admin rejects│   │   │ Admin approves
          or dissolves │   │   │ + all slots filled
                       │   │   │
              ┌────────▼┐  │  ┌▼──────────┐
              │Dissolved │  │  │  Active    │ ← Monthly payments
              │  (4)     │  │  │   (1)     │   + round rotation
              └──────────┘  │  └──┬─────┬──┘
                            │     │     │
                   Admin    │     │     │ Admin freezes
                   closes   │     │     │
                            │     │  ┌──▼──────┐
                            │     │  │ Frozen   │
                            │     │  │  (3)     │
                            │     │  └──┬───────┘
                            │     │     │
                            │     │     │ Admin unfreezes
                            │     │     │ (back to Active)
                            │     │     │
                            │  ┌──▼─────▼──┐
                            │  │ All rounds │
                            │  │ completed  │
                            │  └─────┬──────┘
                            │        │
                            │  ┌─────▼──────┐
                            └──│ Completed   │
                               │   (2)       │
                               └─────────────┘
```

### Flow for Drivers

```
1. CREATE    → POST /savings-circles         (status = Forming)
2. BROWSE    → GET  /savings-circles/available
3. JOIN      → POST /savings-circles/{id}/join
4. WAIT      → Circle reaches maxMembers + admin approves → Active
5. PAY       → POST /savings-circles/{id}/pay  (each round)
6. RECEIVE   → On your turn, the pot is distributed to you
7. REPEAT    → Steps 5-6 until all rounds complete
8. COMPLETE  → Circle status = Completed
```

### Key Rules
- A driver can only leave (`POST /leave`) while the circle is **Forming**
- Payments are per-round; a driver can only pay once per round
- `minHealthScore` filters who can join (financial trustworthiness)
- Admin can **freeze** (pause), **unfreeze** (resume), or **close** (dissolve) at any time
- `durationMonths` typically equals `maxMembers` (one payout per month per member)

---

## 14. Shift Flow

### State Diagram

```
    ┌────────────┐   POST /shifts/start   ┌────────────┐
    │  OffShift  │ ──────────────────────► │  OnShift   │
    │    (0)     │ ◄────────────────────── │    (1)     │
    └────────────┘   POST /shifts/end      └────────────┘
```

### Typical Daily Flow

```
1. MORNING     → POST /shifts/start  { latitude, longitude }
                 (returns ShiftDto with status=1, 201 Created)

2. DURING DAY  → GET  /shifts/current
                 (live stats: orders, earnings, distance)

3. END OF DAY  → POST /shifts/end
                 (returns final ShiftDto with totals)

4. REVIEW      → GET  /shifts/summary?from=2026-03-01&to=2026-03-27
                 (aggregated stats across multiple shifts)
```

### Key Rules
- Only **one active shift** at a time per driver (starting a second returns `409`)
- Ending a shift requires an active shift (returns `404` otherwise)
- Summary endpoint accepts optional date range; omit for all-time stats

---

## 15. Flutter / Dio Examples

### Dio Setup (same as Auth — reuse your interceptor)

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Reuse the token interceptor from AUTH_API.md
```

### Gamification: Get Challenges & Claim Reward

```dart
// Fetch active challenges
Future<List<ChallengeDto>> getActiveChallenges() async {
  final response = await dio.get('/gamification/challenges');
  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((e) => ChallengeDto.fromJson(e))
        .toList();
  }
  throw Exception(response.data['message']);
}

// Claim a completed challenge reward
Future<bool> claimReward(String challengeId) async {
  final response = await dio.post('/gamification/challenges/$challengeId/claim');
  return response.data['isSuccess'] == true;
}

// Get leaderboard
Future<LeaderboardDto> getLeaderboard({String period = 'monthly'}) async {
  final response = await dio.get(
    '/gamification/leaderboard',
    queryParameters: {'period': period},
  );
  return LeaderboardDto.fromJson(response.data['data']);
}
```

### Shift: Start & End

```dart
// Start shift
Future<ShiftDto> startShift(double lat, double lng) async {
  final response = await dio.post('/shifts/start', data: {
    'latitude': lat,
    'longitude': lng,
  });
  if (response.data['isSuccess'] == true) {
    return ShiftDto.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// End shift
Future<ShiftDto> endShift() async {
  final response = await dio.post('/shifts/end');
  if (response.data['isSuccess'] == true) {
    return ShiftDto.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Get summary for date range
Future<ShiftSummaryDto> getShiftSummary({
  DateTime? from,
  DateTime? to,
}) async {
  final response = await dio.get('/shifts/summary', queryParameters: {
    if (from != null) 'from': from.toIso8601String().split('T').first,
    if (to != null) 'to': to.toIso8601String().split('T').first,
  });
  return ShiftSummaryDto.fromJson(response.data['data']);
}
```

### Referrals: Apply Code & View Stats

```dart
// Apply referral code
Future<bool> applyReferralCode(String code) async {
  try {
    final response = await dio.post('/referrals/apply', data: {
      'referralCode': code,
    });
    return response.data['isSuccess'] == true;
  } on DioException catch (e) {
    showError(e.response?.data['message'] ?? 'حدث خطأ');
    return false;
  }
}

// Get referral stats
Future<ReferralStatsDto> getReferralStats() async {
  final response = await dio.get('/referrals/stats');
  return ReferralStatsDto.fromJson(response.data['data']);
}
```

### Savings Circles: Create, Join & Pay

```dart
// Create a new circle
Future<CircleDto> createCircle({
  required String name,
  required double monthlyAmount,
  required int maxMembers,
  required int durationMonths,
  int minHealthScore = 80,
}) async {
  final response = await dio.post('/savings-circles', data: {
    'name': name,
    'monthlyAmount': monthlyAmount,
    'maxMembers': maxMembers,
    'durationMonths': durationMonths,
    'minHealthScore': minHealthScore,
  });
  if (response.data['isSuccess'] == true) {
    return CircleDto.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Browse available circles
Future<PagedResult<CircleDto>> getAvailableCircles({
  int page = 1,
  int size = 10,
}) async {
  final response = await dio.get('/savings-circles/available', queryParameters: {
    'pageNumber': page,
    'pageSize': size,
  });
  return PagedResult.fromJson(response.data['data'], CircleDto.fromJson);
}

// Join a circle
Future<bool> joinCircle(String circleId) async {
  final response = await dio.post('/savings-circles/$circleId/join');
  return response.data['isSuccess'] == true;
}

// Make payment for current round
Future<bool> makePayment(String circleId) async {
  final response = await dio.post('/savings-circles/$circleId/pay');
  return response.data['isSuccess'] == true;
}
```

### Colleague Radar: Find Nearby & Request Help

```dart
// Get nearby drivers
Future<List<NearbyDriverDto>> getNearbyDrivers({
  required double lat,
  required double lng,
  double radiusKm = 5,
}) async {
  final response = await dio.get('/colleague-radar/nearby', queryParameters: {
    'latitude': lat,
    'longitude': lng,
    'radiusKm': radiusKm,
  });
  return (response.data['data'] as List)
      .map((e) => NearbyDriverDto.fromJson(e))
      .toList();
}

// Create help request
Future<HelpRequestDto> createHelpRequest({
  required String title,
  String? description,
  required double lat,
  required double lng,
  required String helpType,
}) async {
  final response = await dio.post('/colleague-radar/help-requests', data: {
    'title': title,
    'description': description,
    'latitude': lat,
    'longitude': lng,
    'helpType': helpType,
  });
  return HelpRequestDto.fromJson(response.data['data']);
}

// Respond to a help request
Future<bool> respondToHelp(String requestId) async {
  final response = await dio.post(
    '/colleague-radar/help-requests/$requestId/respond',
  );
  return response.data['isSuccess'] == true;
}
```

### Road Reports: Create & Confirm

```dart
// Create a road report
Future<RoadReportDto> createRoadReport({
  required int type,
  required double lat,
  required double lng,
  String? description,
  int severity = 1,
}) async {
  final response = await dio.post('/road-reports', data: {
    'type': type,
    'latitude': lat,
    'longitude': lng,
    'description': description,
    'severity': severity,
  });
  return RoadReportDto.fromJson(response.data['data']);
}

// Confirm a report
Future<bool> confirmReport(String reportId, {bool isConfirmed = true}) async {
  final response = await dio.post(
    '/road-reports/$reportId/confirm',
    queryParameters: {'isConfirmed': isConfirmed},
  );
  return response.data['isSuccess'] == true;
}

// Get nearby reports (for map overlay)
Future<List<RoadReportDto>> getNearbyReports({
  required double lat,
  required double lng,
  double radiusKm = 10,
}) async {
  final response = await dio.get('/road-reports/nearby', queryParameters: {
    'latitude': lat,
    'longitude': lng,
    'radiusKm': radiusKm,
  });
  return (response.data['data'] as List)
      .map((e) => RoadReportDto.fromJson(e))
      .toList();
}
```

---
