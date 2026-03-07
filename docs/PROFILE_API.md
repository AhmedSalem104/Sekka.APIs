# Sekka API - Profile Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/profile`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` header

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Profile](#21-get-profile)
   - [Update Profile](#22-update-profile)
   - [Upload Profile Image](#23-upload-profile-image)
   - [Delete Profile Image](#24-delete-profile-image)
   - [Upload License Image](#25-upload-license-image)
   - [Get Profile Completion](#26-get-profile-completion)
   - [Get Stats](#27-get-stats)
   - [Get Badges](#28-get-badges)
   - [Get Activity Log](#29-get-activity-log)
   - [Get Emergency Contacts](#210-get-emergency-contacts)
   - [Add Emergency Contact](#211-add-emergency-contact)
   - [Delete Emergency Contact](#212-delete-emergency-contact)
   - [Get Subscription](#213-get-subscription)
   - [Upgrade Subscription](#214-upgrade-subscription)
   - [Get Achievements](#215-get-achievements)
   - [Get Challenges](#216-get-challenges)
   - [Get Leaderboard](#217-get-leaderboard)
   - [Get Expenses](#218-get-expenses)
   - [Add Expense](#219-add-expense)
3. [DTOs Reference](#3-dtos-reference)
4. [Enums](#4-enums)

---

## 1. Overview

The Profile API manages driver profile data, stats, gamification (badges, achievements, challenges, leaderboard), emergency contacts, subscriptions, and expense tracking.

All endpoints require JWT authentication. The driver ID is extracted from the token automatically.

---

## 2. Endpoints

### 2.1 Get Profile

```
GET /api/v1/profile
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ahmed Mohamed",
    "phone": "01012345678",
    "email": "ahmed@example.com",
    "profileImageUrl": "https://api.sekka.app/uploads/profiles/abc.jpg",
    "licenseImageUrl": "https://api.sekka.app/uploads/licenses/def.jpg",
    "vehicleType": 0,
    "activeVehicle": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "vehicleType": 0,
      "plateNumber": "أ ب ج 1234",
      "makeModel": "Suzuki Van",
      "color": "White"
    },
    "isOnline": true,
    "defaultRegion": "القاهرة",
    "cashOnHand": 1500.00,
    "walletBalance": 350.00,
    "totalPoints": 2500,
    "level": 5,
    "nextLevelPoints": 3000,
    "subscriptionPlan": "Pro",
    "joinedAt": "2026-01-15T10:00:00Z",
    "totalOrders": 450,
    "totalDelivered": 420,
    "averageRating": 4.8,
    "shiftStatus": 1,
    "shiftStartTime": "2026-03-07T08:00:00Z",
    "healthScore": 85,
    "badgesCount": 12,
    "currentStreak": 7,
    "completionPercentage": 90,
    "todayOrdersCount": 15,
    "todayEarnings": 450.00,
    "referralCode": "AHMED2026"
  }
}
```

---

### 2.2 Update Profile

```
PUT /api/v1/profile
```

**Request Body** (all fields optional):
```json
{
  "name": "Ahmed Ali",
  "email": "ahmed.ali@example.com",
  "vehicleType": 1,
  "defaultRegionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cashAlertThreshold": 2500.00,
  "speedCompleteMode": true
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث البيانات بنجاح",
  "data": { ... }
}
```

---

### 2.3 Upload Profile Image

```
POST /api/v1/profile/image
Content-Type: multipart/form-data
```

**Request**: Form data with `file` field (image file)

| Constraint | Value |
|-----------|-------|
| Max Size | 5 MB |
| Allowed Types | `.jpg`, `.jpeg`, `.png`, `.webp` |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم رفع الصورة بنجاح",
  "data": {
    "imageUrl": "https://api.sekka.app/uploads/profiles/abc123.jpg"
  }
}
```

**Flutter/Dio Example**:
```dart
final formData = FormData.fromMap({
  'file': await MultipartFile.fromFile(
    imagePath,
    filename: 'profile.jpg',
  ),
});

final response = await dio.post(
  '/api/v1/profile/image',
  data: formData,
);
```

---

### 2.4 Delete Profile Image

```
DELETE /api/v1/profile/image
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": null,
  "data": true
}
```

---

### 2.5 Upload License Image

```
PUT /api/v1/profile/license-image
Content-Type: multipart/form-data
```

**Request**: Form data with `file` field (image file). Same constraints as profile image.

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم رفع الصورة بنجاح",
  "data": {
    "imageUrl": "https://api.sekka.app/uploads/licenses/xyz456.jpg"
  }
}
```

---

### 2.6 Get Profile Completion

```
GET /api/v1/profile/completion
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "completionPercentage": 75,
    "completedSteps": ["الاسم", "نوع المركبة"],
    "pendingSteps": [
      {
        "stepName": "صورة الرخصة",
        "stepKey": "licenseImage",
        "isRequired": true,
        "weight": 20
      },
      {
        "stepName": "البريد الإلكتروني",
        "stepKey": "email",
        "isRequired": false,
        "weight": 5
      }
    ],
    "isProfileComplete": false
  }
}
```

---

### 2.7 Get Stats

```
GET /api/v1/profile/stats?fromDate=2026-01-01&toDate=2026-03-07
```

| Query Param | Type | Required | Description |
|------------|------|----------|-------------|
| `fromDate` | DateTime | No | Start date filter |
| `toDate` | DateTime | No | End date filter |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "totalOrders": 450,
    "totalDelivered": 420,
    "totalFailed": 10,
    "totalCancelled": 20,
    "successRate": 93.33,
    "averageRating": 4.8,
    "totalEarnings": 45000.00,
    "totalCommissions": 4500.00,
    "averageDeliveryTimeMinutes": 28.5,
    "bestDay": "2026-02-14T00:00:00Z",
    "bestDayOrders": 35
  }
}
```

---

### 2.8 Get Badges

```
GET /api/v1/profile/badges
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Speed Star",
      "description": "أكمل 100 طلب في أسبوع",
      "iconUrl": "https://api.sekka.app/badges/speed-star.png",
      "earnedAt": "2026-02-20T14:30:00Z",
      "category": "Performance"
    }
  ]
}
```

---

### 2.9 Get Activity Log

```
GET /api/v1/profile/activity-log?page=1&pageSize=20
```

| Query Param | Type | Default | Description |
|------------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "action": "OrderDelivered",
        "description": "تم تسليم الطلب #1234",
        "timestamp": "2026-03-07T15:30:00Z",
        "relatedEntityType": "Order",
        "relatedEntityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
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

### 2.10 Get Emergency Contacts

```
GET /api/v1/profile/emergency-contacts
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Mohamed Ahmed",
      "phone": "01098765432",
      "relationship": "Brother"
    }
  ]
}
```

---

### 2.11 Add Emergency Contact

```
POST /api/v1/profile/emergency-contacts
```

**Request Body**:
```json
{
  "name": "Sara Ahmed",
  "phone": "01112345678",
  "relationship": "Sister"
}
```

**Response** `201 Created`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Sara Ahmed",
    "phone": "01112345678",
    "relationship": "Sister"
  }
}
```

---

### 2.12 Delete Emergency Contact

```
DELETE /api/v1/profile/emergency-contacts/{id}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| `id` | Guid | Emergency contact ID |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": true
}
```

---

### 2.13 Get Subscription

```
GET /api/v1/profile/subscription
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "planName": "Pro",
    "priceMonthly": 199.00,
    "status": 1,
    "startDate": "2026-01-01T00:00:00Z",
    "endDate": "2026-04-01T00:00:00Z",
    "autoRenew": true,
    "features": ["Priority Orders", "Advanced Stats", "Custom Badge"],
    "daysRemaining": 25
  }
}
```

---

### 2.14 Upgrade Subscription

```
POST /api/v1/profile/subscription/upgrade
```

**Request Body**:
```json
{
  "planId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "paymentMethod": "wallet",
  "billingCycle": "monthly"
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

### 2.15 Get Achievements

```
GET /api/v1/profile/achievements
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "challengeName": "First 100 Orders",
      "badgeName": "Century Rider",
      "badgeIconUrl": "https://api.sekka.app/badges/century.png",
      "pointsEarned": 500,
      "completedAt": "2026-02-15T10:00:00Z"
    }
  ]
}
```

---

### 2.16 Get Challenges

```
GET /api/v1/profile/challenges
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Weekend Warrior",
      "description": "أكمل 50 طلب في عطلة نهاية الأسبوع",
      "challengeType": 2,
      "targetValue": 50,
      "currentProgress": 32,
      "progressPercentage": 64.0,
      "rewardPoints": 300,
      "badgeName": "Weekend Warrior",
      "isCompleted": false
    }
  ]
}
```

---

### 2.17 Get Leaderboard

```
GET /api/v1/profile/leaderboard
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "myRank": 12,
    "myPoints": 2500,
    "topDrivers": [
      {
        "rank": 1,
        "driverName": "Omar Hassan",
        "points": 8500,
        "level": 10,
        "ordersThisMonth": 220
      },
      {
        "rank": 2,
        "driverName": "Ali Mohamed",
        "points": 7200,
        "level": 9,
        "ordersThisMonth": 195
      }
    ]
  }
}
```

---

### 2.18 Get Expenses

```
GET /api/v1/profile/expenses?page=1&pageSize=20&category=fuel&fromDate=2026-03-01&toDate=2026-03-07
```

| Query Param | Type | Default | Description |
|------------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |
| `category` | string | null | Filter by category |
| `fromDate` | DateTime | null | Start date filter |
| `toDate` | DateTime | null | End date filter |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "category": "fuel",
        "amount": 250.00,
        "description": "بنزين 92",
        "date": "2026-03-07T00:00:00Z"
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  }
}
```

---

### 2.19 Add Expense

```
POST /api/v1/profile/expenses
```

**Request Body**:
```json
{
  "category": "fuel",
  "amount": 250.00,
  "description": "بنزين 92",
  "date": "2026-03-07T00:00:00Z"
}
```

**Response** `201 Created`:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "category": "fuel",
    "amount": 250.00,
    "description": "بنزين 92",
    "date": "2026-03-07T00:00:00Z"
  }
}
```

---

## 3. DTOs Reference

### DriverProfileDto
| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | Guid | No | Driver ID |
| name | string | No | Full name |
| phone | string | No | Phone number |
| email | string | Yes | Email address |
| profileImageUrl | string | Yes | Profile image URL |
| licenseImageUrl | string | Yes | License image URL |
| vehicleType | VehicleType | No | Vehicle type enum |
| activeVehicle | VehicleInfoDto | Yes | Current vehicle info |
| isOnline | bool | No | Online status |
| defaultRegion | string | Yes | Default region name |
| cashOnHand | decimal | No | Current cash amount |
| walletBalance | decimal | No | Wallet balance |
| totalPoints | int | No | Gamification points |
| level | int | No | Current level |
| nextLevelPoints | int | No | Points needed for next level |
| subscriptionPlan | string | Yes | Current plan name |
| joinedAt | DateTime | No | Registration date |
| totalOrders | int | No | All-time orders |
| totalDelivered | int | No | Successfully delivered |
| averageRating | decimal | No | Average customer rating |
| shiftStatus | ShiftStatus | No | Current shift status |
| shiftStartTime | DateTime | Yes | When shift started |
| healthScore | int | No | Account health score (0-100) |
| badgesCount | int | No | Total badges earned |
| currentStreak | int | No | Consecutive active days |
| completionPercentage | int | No | Profile completion % |
| todayOrdersCount | int | No | Orders today |
| todayEarnings | decimal | No | Earnings today |
| referralCode | string | No | Referral code |

### UpdateProfileDto
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | No | Full name |
| email | string | No | Email address |
| vehicleType | VehicleType | No | Vehicle type enum |
| defaultRegionId | Guid | No | Default region ID |
| cashAlertThreshold | decimal | No | Cash alert threshold (EGP) |
| speedCompleteMode | bool | No | Speed complete mode toggle |

---

## 4. Enums

### VehicleType
| Value | Name |
|-------|------|
| 0 | Motorcycle |
| 1 | Car |
| 2 | Van |
| 3 | Truck |
| 4 | Bicycle |

### ShiftStatus
| Value | Name |
|-------|------|
| 0 | Off |
| 1 | Active |
| 2 | Break |

### SubscriptionStatus
| Value | Name |
|-------|------|
| 0 | Trial |
| 1 | Active |
| 2 | Expired |
| 3 | Cancelled |
| 4 | Suspended |

### ChallengeType
| Value | Name |
|-------|------|
| 0 | Daily |
| 1 | Weekly |
| 2 | Monthly |
| 3 | OneTime |

---

## Flutter/Dio Integration

```dart
// Get profile
final profileResponse = await dio.get('/api/v1/profile');
final profile = profileResponse.data['data'];

// Update profile
await dio.put('/api/v1/profile', data: {
  'name': 'Ahmed Ali',
  'vehicleType': 1,
});

// Upload profile image
final formData = FormData.fromMap({
  'file': await MultipartFile.fromFile(imagePath),
});
await dio.post('/api/v1/profile/image', data: formData);

// Get stats with date range
final stats = await dio.get('/api/v1/profile/stats', queryParameters: {
  'fromDate': '2026-01-01',
  'toDate': '2026-03-07',
});

// Add emergency contact
await dio.post('/api/v1/profile/emergency-contacts', data: {
  'name': 'Mohamed',
  'phone': '01098765432',
  'relationship': 'Brother',
});

// Get expenses with filter
final expenses = await dio.get('/api/v1/profile/expenses', queryParameters: {
  'page': 1,
  'pageSize': 20,
  'category': 'fuel',
});
```
