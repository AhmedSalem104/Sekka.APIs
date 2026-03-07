# Sekka API - Lookups Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/lookups`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: No authentication required (public endpoints)

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Vehicle Types](#21-get-vehicle-types)
3. [Enums](#3-enums)

---

## 1. Overview

The Lookups API provides reference data (enums, lists) needed by the frontend for dropdowns and selections. All endpoints are **public** (no authentication required).

---

## 2. Endpoints

### 2.1 Get Vehicle Types

```
GET /api/v1/lookups/vehicle-types
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    { "id": 0, "name": "Motorcycle" },
    { "id": 1, "name": "Car" },
    { "id": 2, "name": "Van" },
    { "id": 3, "name": "Truck" },
    { "id": 4, "name": "Bicycle" }
  ]
}
```

---

## 3. Enums

### VehicleType
| Value | Name | Description |
|-------|------|-------------|
| 0 | Motorcycle | موتوسيكل |
| 1 | Car | سيارة |
| 2 | Van | فان |
| 3 | Truck | تراك |
| 4 | Bicycle | دراجة |

---

## Flutter/Dio Integration

```dart
// Fetch vehicle types for dropdown (no auth needed)
final response = await Dio().get(
  'https://sekka.runasp.net/api/v1/lookups/vehicle-types',
);
final vehicleTypes = (response.data['data'] as List)
    .map((v) => {'id': v['id'], 'name': v['name']})
    .toList();
```
