# Sekka API - Digital Badge Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/badge`
>
> **Last Updated**: 2026-03-07

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Digital Badge](#21-get-digital-badge)
   - [Verify Badge (Public)](#22-verify-badge-public)
3. [DTOs Reference](#3-dtos-reference)

---

## 1. Overview

The Badge API provides digital identity badges for drivers. Badges include a QR code that can be scanned by anyone (customers, partners) to verify the driver's identity and status.

- **Get Badge**: Requires authentication (driver sees their own badge)
- **Verify Badge**: Public endpoint (anyone can verify by scanning QR code)

---

## 2. Endpoints

### 2.1 Get Digital Badge

```
GET /api/v1/badge
Authorization: Bearer <token>
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "driverName": "Ahmed Mohamed",
    "profileImageUrl": "https://api.sekka.app/uploads/profiles/abc.jpg",
    "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "vehicleType": 0,
    "averageRating": 4.8,
    "totalDeliveries": 420,
    "memberSince": "2026-01-15T00:00:00Z",
    "level": 5,
    "qrCodeToken": "eyJhbGciOiJIUzI1NiIs...",
    "isVerified": true
  }
}
```

**Response** `404 Not Found`:
```json
{
  "success": false,
  "message": "السائق غير موجود"
}
```

---

### 2.2 Verify Badge (Public)

```
GET /api/v1/badge/verify/{qrToken}
```

> **No authentication required** — This is a public endpoint.

| Path Param | Type | Description |
|-----------|------|-------------|
| qrToken | string | QR code token from the badge |

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "driverName": "Ahmed Mohamed",
    "vehicleType": 0,
    "rating": 4.8,
    "isActive": true,
    "verifiedAt": "2026-03-07T15:30:00Z"
  }
}
```

**Response** `404 Not Found` (invalid/expired token):
```json
{
  "success": false,
  "message": "رمز التحقق غير صالح"
}
```

---

## 3. DTOs Reference

### DigitalBadgeDto
| Field | Type | Description |
|-------|------|-------------|
| driverName | string | Driver's full name |
| profileImageUrl | string? | Profile image URL |
| driverId | Guid | Driver ID |
| vehicleType | VehicleType | Vehicle type enum |
| averageRating | decimal | Average customer rating |
| totalDeliveries | int | Total delivered orders |
| memberSince | DateTime | Registration date |
| level | int | Gamification level |
| qrCodeToken | string | Token for QR code generation |
| isVerified | bool | Whether driver is verified |

### BadgeVerificationDto
| Field | Type | Description |
|-------|------|-------------|
| isValid | bool | Whether the badge is valid |
| driverName | string? | Driver name (if valid) |
| vehicleType | VehicleType? | Vehicle type (if valid) |
| rating | decimal? | Rating (if valid) |
| isActive | bool | Whether driver account is active |
| verifiedAt | DateTime | Verification timestamp |

---

## Flutter/Dio Integration

```dart
// Get my digital badge
final badge = await dio.get('/api/v1/badge');
final qrToken = badge.data['data']['qrCodeToken'];
// Generate QR code from qrToken using qr_flutter package

// Verify a scanned badge (no auth needed)
final verification = await Dio().get(
  'https://sekka.runasp.net/api/v1/badge/verify/$scannedToken',
);
final isValid = verification.data['data']['isValid'];
```
