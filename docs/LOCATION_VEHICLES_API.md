# Sekka API - Location & Vehicles Documentation (Phase 6)

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-26

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Flows](#3-flows)
4. [Parking Endpoints](#4-parking-endpoints)
   - [Get All Parking Spots](#41-get-all-parking-spots)
   - [Create Parking Spot](#42-create-parking-spot)
   - [Update Parking Spot](#43-update-parking-spot)
   - [Delete Parking Spot](#44-delete-parking-spot)
   - [Get Nearby Parking Spots](#45-get-nearby-parking-spots)
5. [Vehicle Endpoints](#5-vehicle-endpoints)
   - [Get Vehicles](#51-get-vehicles)
   - [Create Vehicle](#52-create-vehicle)
   - [Update Vehicle](#53-update-vehicle)
   - [Delete Vehicle](#54-delete-vehicle)
   - [Add Maintenance Record](#55-add-maintenance-record)
   - [Get Maintenance History](#56-get-maintenance-history)
6. [Break Endpoints](#6-break-endpoints)
   - [Start Break](#61-start-break)
   - [End Break](#62-end-break)
   - [Get Break Suggestion](#63-get-break-suggestion)
   - [Get Break History](#64-get-break-history)
7. [Admin Vehicle Endpoints](#7-admin-vehicle-endpoints)
   - [Get Vehicles (Filtered)](#71-get-vehicles-filtered)
   - [Get Vehicle By ID](#72-get-vehicle-by-id)
   - [Approve Vehicle](#73-approve-vehicle)
   - [Reject Vehicle](#74-reject-vehicle)
   - [Flag Maintenance](#75-flag-maintenance)
   - [Deactivate Vehicle](#76-deactivate-vehicle)
   - [Activate Vehicle](#77-activate-vehicle)
   - [Get Pending Vehicles](#78-get-pending-vehicles)
   - [Get Maintenance Due](#79-get-maintenance-due)
   - [Get Fleet Stats](#710-get-fleet-stats)
   - [Get Vehicles By Type](#711-get-vehicles-by-type)
8. [Enums](#8-enums)
9. [DTOs Reference](#9-dtos-reference)
10. [Error Messages Reference](#10-error-messages-reference)
11. [Flutter Integration](#11-flutter-integration)

---

## 1. Overview

Phase 6 covers **Location & Vehicles** -- parking spot management, vehicle registration and maintenance tracking, driver break management, and admin fleet oversight.

| Feature | Detail |
|---------|--------|
| Parking | Save, share, and discover parking spots |
| Vehicles | Register vehicles, track maintenance |
| Breaks | Smart break suggestions based on driving patterns |
| Admin Fleet | Approve/reject vehicles, fleet statistics |
| Auth | All endpoints require Bearer Token (except admin = Admin role) |

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

### Paged Response
```json
{
  "isSuccess": true,
  "data": {
    "items": [ ... ],
    "totalCount": 50,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
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
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid/missing token) |
| 403 | Forbidden (wrong role) |
| 404 | Not Found |
| 409 | Conflict |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Flows

### 3.1 Break Flow

```
                    ┌──────────────────┐
                    │  Driver On Shift  │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  GET /suggestion  │
                    │  (check if break  │
                    │   is recommended) │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  ShouldBreak?    │
                    └──┬───────────┬───┘
                       │           │
                      Yes          No
                       │           │
              ┌────────▼──┐   ┌───▼──────────┐
              │ Show break │   │ Continue     │
              │ suggestion │   │ driving      │
              │ + nearby   │   └──────────────┘
              │ spots      │
              └────────┬───┘
                       │
              ┌────────▼───────────┐
              │ POST /breaks/start │
              │ (record location,  │
              │  energy level)     │
              └────────┬───────────┘
                       │
              ┌────────▼───────────┐
              │  Driver resting... │
              │  (timer running)   │
              └────────┬───────────┘
                       │
              ┌────────▼───────────┐
              │  POST /breaks/end  │
              │  (record energy    │
              │   after rest)      │
              └────────┬───────────┘
                       │
              ┌────────▼───────────┐
              │  Back to driving   │
              └────────────────────┘
```

### 3.2 Vehicle Approval Flow

```
  ┌──────────────────┐
  │ Driver registers  │
  │ POST /vehicles    │
  └────────┬─────────┘
           │
  ┌────────▼─────────────┐
  │ Vehicle created       │
  │ status = Pending (0)  │
  └────────┬──────────────┘
           │
  ┌────────▼─────────────┐
  │ Admin reviews         │
  │ GET /admin/vehicles/  │
  │     pending           │
  └────────┬──────────────┘
           │
     ┌─────┴──────┐
     │            │
┌────▼────┐  ┌───▼──────┐
│ Approve │  │  Reject  │
│ POST    │  │  POST    │
│ /{id}/  │  │  /{id}/  │
│ approve │  │  reject  │
└────┬────┘  └───┬──────┘
     │           │
┌────▼────┐  ┌───▼──────────┐
│ status  │  │ status       │
│ = 1     │  │ = 2          │
│Approved │  │ Rejected     │
│ Vehicle │  │ (with reason)│
│ Active  │  └──────────────┘
└────┬────┘
     │
     │  (ongoing lifecycle)
     │
┌────▼────────────────────┐
│  Admin can:             │
│  - Flag maintenance     │
│  - Deactivate           │
│  - Activate             │
└─────────────────────────┘
```

### 3.3 Vehicle Maintenance Flow

```
  Driver adds record          Admin flags issue
  POST /vehicles/             POST /admin/vehicles/
  {id}/maintenance            {id}/flag-maintenance
       │                            │
       ▼                            ▼
  ┌──────────────┐          ┌───────────────┐
  │ Maintenance  │          │ Maintenance   │
  │ Record saved │          │ flagged with  │
  │ with cost,   │          │ urgency &     │
  │ mileage,     │          │ deadline      │
  │ next due     │          └───────────────┘
  └──────┬───────┘
         │
         ▼
  Shows in:
  - GET /vehicles/{id}/maintenance  (driver)
  - GET /admin/vehicles/{id}        (admin detail)
  - GET /admin/vehicles/maintenance-due
```

---

## 4. Parking Endpoints

### 4.1 Get All Parking Spots

Returns all parking spots saved by the authenticated driver.

```
GET /parking
```

**Auth Required**: Yes (Bearer Token)

**Headers**:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "address": "شارع التحرير، وسط البلد، القاهرة",
      "qualityRating": 4,
      "isPaid": false,
      "usageCount": 12,
      "lastUsedAt": "2026-03-25T14:30:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.2 Create Parking Spot

Saves a new parking spot for the driver.

```
POST /parking
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "latitude": 30.0444,
  "longitude": 31.2357,
  "address": "شارع التحرير، وسط البلد، القاهرة",
  "notes": "بجانب محطة البنزين",
  "qualityRating": 4,
  "isPaid": false,
  "paidAmount": null,
  "isShared": true
}
```

| Field | Type | Required | Default | Rules |
|-------|------|----------|---------|-------|
| latitude | double | Yes | - | Valid latitude (-90 to 90) |
| longitude | double | Yes | - | Valid longitude (-180 to 180) |
| address | string | No | null | Free-text address |
| notes | string | No | null | Additional notes |
| qualityRating | int | No | 3 | 1-5 rating |
| isPaid | bool | No | false | Is this a paid spot? |
| paidAmount | decimal | No | null | Cost if paid |
| isShared | bool | No | false | Share with other drivers |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "address": "شارع التحرير، وسط البلد، القاهرة",
    "qualityRating": 4,
    "isPaid": false,
    "usageCount": 0,
    "lastUsedAt": "2026-03-26T10:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | Validation errors (invalid coordinates, rating) |
| 401 | Unauthorized |

---

### 4.3 Update Parking Spot

Updates an existing parking spot. Only the spot owner can update it.

```
PUT /parking/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Parking spot ID

**Request Body** (all fields optional -- only send what you want to update):
```json
{
  "latitude": 30.0450,
  "longitude": 31.2360,
  "address": "شارع التحرير، بجانب المترو",
  "notes": "متاح بعد الساعة 8 مساءً",
  "qualityRating": 5,
  "isPaid": true,
  "paidAmount": 10.00,
  "isShared": true
}
```

| Field | Type | Required |
|-------|------|----------|
| latitude | double? | No |
| longitude | double? | No |
| address | string? | No |
| notes | string? | No |
| qualityRating | int? | No |
| isPaid | bool? | No |
| paidAmount | decimal? | No |
| isShared | bool? | No |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "latitude": 30.0450,
    "longitude": 31.2360,
    "address": "شارع التحرير، بجانب المترو",
    "qualityRating": 5,
    "isPaid": true,
    "usageCount": 12,
    "lastUsedAt": "2026-03-25T14:30:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `مكان الركنة غير موجود` |
| 401 | Unauthorized (not the owner) |

---

### 4.4 Delete Parking Spot

Deletes a parking spot. Only the owner can delete it.

```
DELETE /parking/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Parking spot ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف مكان الركنة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `مكان الركنة غير موجود` |
| 401 | Unauthorized (not the owner) |

---

### 4.5 Get Nearby Parking Spots

Finds parking spots near a given location (shared spots from all drivers + driver's own spots).

```
GET /parking/nearby?latitude=30.0444&longitude=31.2357&radiusKm=1.5
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Default | Description |
|-------|------|----------|---------|-------------|
| latitude | double | Yes | - | Current latitude |
| longitude | double | Yes | - | Current longitude |
| radiusKm | double | No | 1.0 | Search radius in kilometers |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "latitude": 30.0450,
      "longitude": 31.2360,
      "address": "شارع التحرير، وسط البلد",
      "qualityRating": 4,
      "isPaid": false,
      "usageCount": 25,
      "lastUsedAt": "2026-03-25T18:00:00Z"
    },
    {
      "id": "7ab12c34-8899-4562-b3fc-2c963f66afa6",
      "latitude": 30.0441,
      "longitude": 31.2350,
      "address": "ميدان التحرير",
      "qualityRating": 3,
      "isPaid": true,
      "usageCount": 8,
      "lastUsedAt": "2026-03-24T09:15:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

## 5. Vehicle Endpoints

### 5.1 Get Vehicles

Returns all vehicles registered by the authenticated driver.

```
GET /vehicles
```

**Auth Required**: Yes (Bearer Token)

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "vehicleType": 0,
      "plateNumber": "أ ب ج 1234",
      "makeModel": "Honda PCX 150",
      "year": 2024,
      "currentMileageKm": 15000.5,
      "fuelConsumptionPer100Km": 2.50,
      "insuranceExpiryDate": "2027-01-15",
      "nextMaintenanceDate": "2026-04-01",
      "isActive": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 5.2 Create Vehicle

Registers a new vehicle for the driver. Vehicle starts with `Pending` approval status.

```
POST /vehicles
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "vehicleType": 0,
  "plateNumber": "أ ب ج 1234",
  "makeModel": "Honda PCX 150",
  "year": 2024,
  "currentMileageKm": 15000.5,
  "fuelConsumptionPer100Km": 2.50,
  "fuelPricePerLiter": 12.25,
  "insuranceExpiryDate": "2027-01-15"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| vehicleType | int | Yes | 0-4 (see [Enums](#8-enums)) |
| plateNumber | string | No | Vehicle plate number |
| makeModel | string | No | Make and model description |
| year | int | No | Manufacturing year |
| currentMileageKm | double | Yes | Current odometer reading (km) |
| fuelConsumptionPer100Km | decimal | No | Fuel consumption rate |
| fuelPricePerLiter | decimal | No | Current fuel price per liter |
| insuranceExpiryDate | string | No | Format: `YYYY-MM-DD` |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "vehicleType": 0,
    "plateNumber": "أ ب ج 1234",
    "makeModel": "Honda PCX 150",
    "year": 2024,
    "currentMileageKm": 15000.5,
    "fuelConsumptionPer100Km": 2.50,
    "insuranceExpiryDate": "2027-01-15",
    "nextMaintenanceDate": null,
    "isActive": true
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | Validation errors |
| 409 | `المركبة مسجلة بالفعل` |

---

### 5.3 Update Vehicle

Updates vehicle information. Only the vehicle owner can update.

```
PUT /vehicles/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body** (all fields optional):
```json
{
  "vehicleType": 1,
  "plateNumber": "أ ب ج 5678",
  "makeModel": "Toyota Corolla 2025",
  "year": 2025,
  "currentMileageKm": 16500.0,
  "fuelConsumptionPer100Km": 7.00,
  "fuelPricePerLiter": 12.50,
  "insuranceExpiryDate": "2027-06-01"
}
```

| Field | Type | Required |
|-------|------|----------|
| vehicleType | int? | No |
| plateNumber | string? | No |
| makeModel | string? | No |
| year | int? | No |
| currentMileageKm | double? | No |
| fuelConsumptionPer100Km | decimal? | No |
| fuelPricePerLiter | decimal? | No |
| insuranceExpiryDate | string? | No |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "vehicleType": 1,
    "plateNumber": "أ ب ج 5678",
    "makeModel": "Toyota Corolla 2025",
    "year": 2025,
    "currentMileageKm": 16500.0,
    "fuelConsumptionPer100Km": 7.00,
    "insuranceExpiryDate": "2027-06-01",
    "nextMaintenanceDate": "2026-04-01",
    "isActive": true
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 401 | Unauthorized (not the owner) |

---

### 5.4 Delete Vehicle

Deletes a vehicle. Only the vehicle owner can delete.

```
DELETE /vehicles/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Vehicle ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف المركبة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 401 | Unauthorized (not the owner) |

---

### 5.5 Add Maintenance Record

Logs a maintenance record for a vehicle.

```
POST /vehicles/{vehicleId}/maintenance
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `vehicleId` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "maintenanceType": 0,
  "cost": 350.00,
  "mileageAtService": 15000.5,
  "nextDueMileage": 20000.0,
  "nextDueDate": "2026-06-01",
  "notes": "تم تغيير الزيت والفلتر"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| maintenanceType | int | Yes | 0-6 (see [Enums](#8-enums)) |
| cost | decimal | No | Cost in EGP |
| mileageAtService | double | Yes | Odometer at time of service |
| nextDueMileage | double | No | Next service due mileage |
| nextDueDate | string | No | Format: `YYYY-MM-DD` |
| notes | string | No | Service notes |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "8bc91e23-4455-4562-b3fc-2c963f66afa6",
    "maintenanceType": 0,
    "cost": 350.00,
    "mileageAtService": 15000.5,
    "nextDueMileage": 20000.0,
    "nextDueDate": "2026-06-01",
    "notes": "تم تغيير الزيت والفلتر",
    "servicedAt": "2026-03-26T12:00:00Z"
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 401 | Unauthorized (not the owner) |

---

### 5.6 Get Maintenance History

Returns all maintenance records for a vehicle.

```
GET /vehicles/{vehicleId}/maintenance
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `vehicleId` (GUID) - Vehicle ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "8bc91e23-4455-4562-b3fc-2c963f66afa6",
      "maintenanceType": 0,
      "cost": 350.00,
      "mileageAtService": 15000.5,
      "nextDueMileage": 20000.0,
      "nextDueDate": "2026-06-01",
      "notes": "تم تغيير الزيت والفلتر",
      "servicedAt": "2026-03-26T12:00:00Z"
    },
    {
      "id": "1de42f56-7788-4562-b3fc-2c963f66afa6",
      "maintenanceType": 2,
      "cost": 800.00,
      "mileageAtService": 12000.0,
      "nextDueMileage": 30000.0,
      "nextDueDate": "2026-09-01",
      "notes": "تم تغيير تيل الفرامل",
      "servicedAt": "2026-01-15T09:30:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 401 | Unauthorized (not the owner) |

---

## 6. Break Endpoints

### 6.1 Start Break

Starts a break session for the driver.

```
POST /breaks/start
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "latitude": 30.0444,
  "longitude": 31.2357,
  "locationDescription": "كافيه بجانب محطة المترو",
  "energyBefore": 3
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| latitude | double | No | Current latitude |
| longitude | double | No | Current longitude |
| locationDescription | string | No | Where the driver is resting |
| energyBefore | int | No | Energy level before break (1-5) |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "startTime": "2026-03-26T14:00:00Z",
    "endTime": null,
    "durationMinutes": null,
    "locationDescription": "كافيه بجانب محطة المترو",
    "energyBefore": 3,
    "energyAfter": null
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 409 | `يوجد استراحة نشطة بالفعل` |

---

### 6.2 End Break

Ends the current active break session.

```
POST /breaks/end
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "energyAfter": 5
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| energyAfter | int | No | Energy level after break (1-5) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "startTime": "2026-03-26T14:00:00Z",
    "endTime": "2026-03-26T14:25:00Z",
    "durationMinutes": 25,
    "locationDescription": "كافيه بجانب محطة المترو",
    "energyBefore": 3,
    "energyAfter": 5
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `لا يوجد استراحة نشطة` |

---

### 6.3 Get Break Suggestion

Gets a smart break suggestion based on the driver's driving patterns, time on shift, and fatigue indicators.

```
GET /breaks/suggestion
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "shouldBreak": true,
    "urgency": 2,
    "suggestedDurationMinutes": 20,
    "reason": "أنت تقود منذ 3 ساعات متواصلة. ننصحك بأخذ استراحة",
    "nearbySpots": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "latitude": 30.0450,
        "longitude": 31.2360,
        "address": "شارع التحرير، وسط البلد",
        "qualityRating": 4,
        "isPaid": false,
        "usageCount": 25,
        "lastUsedAt": "2026-03-25T18:00:00Z"
      }
    ]
  },
  "message": null,
  "errors": null
}
```

> **Note**: The `urgency` field uses the `BreakUrgency` enum (0=Low, 1=Medium, 2=High, 3=Critical). See [Enums](#8-enums).

---

### 6.4 Get Break History

Returns paginated break history for the driver.

```
GET /breaks/history?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Required | Default |
|-------|------|----------|---------|
| page | int | No | 1 |
| pageSize | int | No | 20 |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "startTime": "2026-03-26T14:00:00Z",
        "endTime": "2026-03-26T14:25:00Z",
        "durationMinutes": 25,
        "locationDescription": "كافيه بجانب محطة المترو",
        "energyBefore": 3,
        "energyAfter": 5
      },
      {
        "id": "9ef01234-5678-4562-b3fc-2c963f66afa6",
        "startTime": "2026-03-25T11:00:00Z",
        "endTime": "2026-03-25T11:15:00Z",
        "durationMinutes": 15,
        "locationDescription": "محطة بنزين",
        "energyBefore": 2,
        "energyAfter": 4
      }
    ],
    "totalCount": 35,
    "page": 1,
    "pageSize": 20,
    "totalPages": 2
  },
  "message": null,
  "errors": null
}
```

---

## 7. Admin Vehicle Endpoints

> **All admin endpoints require `Admin` role**. Requests without admin privileges receive `403 Forbidden`.

### 7.1 Get Vehicles (Filtered)

Returns a paginated, filterable list of all vehicles in the fleet.

```
GET /admin/vehicles?page=1&pageSize=20&vehicleType=0&approvalStatus=1&maintenanceDue=true&searchTerm=honda
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Required | Default | Description |
|-------|------|----------|---------|-------------|
| page | int | No | 1 | Page number |
| pageSize | int | No | 20 | Items per page |
| vehicleType | int | No | null | Filter by VehicleType enum (0-4) |
| approvalStatus | int | No | null | Filter by VehicleApprovalStatus (0-2) |
| maintenanceDue | bool | No | null | Filter vehicles needing maintenance |
| searchTerm | string | No | null | Search by plate, make/model, driver name |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "driverId": "a1b2c3d4-5678-4562-b3fc-2c963f66afa6",
        "driverName": "أحمد محمد",
        "vehicleType": 0,
        "plateNumber": "أ ب ج 1234",
        "makeModel": "Honda PCX 150",
        "year": 2024,
        "isActive": true,
        "approvalStatus": 1,
        "nextMaintenanceDate": "2026-04-01",
        "needsAttention": false
      }
    ],
    "totalCount": 150,
    "page": 1,
    "pageSize": 20,
    "totalPages": 8
  },
  "message": null,
  "errors": null
}
```

---

### 7.2 Get Vehicle By ID

Returns detailed vehicle information including maintenance history.

```
GET /admin/vehicles/{id}
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "driverId": "a1b2c3d4-5678-4562-b3fc-2c963f66afa6",
    "driverName": "أحمد محمد",
    "vehicleType": 0,
    "plateNumber": "أ ب ج 1234",
    "makeModel": "Honda PCX 150",
    "year": 2024,
    "isActive": true,
    "approvalStatus": 1,
    "nextMaintenanceDate": "2026-04-01",
    "needsAttention": false,
    "currentMileageKm": 15000.5,
    "fuelConsumptionPer100Km": 2.50,
    "maintenanceHistory": [
      {
        "id": "8bc91e23-4455-4562-b3fc-2c963f66afa6",
        "maintenanceType": 0,
        "cost": 350.00,
        "mileageAtService": 15000.5,
        "nextDueMileage": 20000.0,
        "nextDueDate": "2026-06-01",
        "notes": "تم تغيير الزيت والفلتر",
        "servicedAt": "2026-03-26T12:00:00Z"
      }
    ],
    "totalMaintenanceCost": 1150.00
  },
  "message": null,
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |

---

### 7.3 Approve Vehicle

Approves a pending vehicle registration.

```
POST /admin/vehicles/{id}/approve
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم الموافقة على المركبة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 400 | `المركبة ليست في حالة انتظار` |

---

### 7.4 Reject Vehicle

Rejects a pending vehicle registration with a reason.

```
POST /admin/vehicles/{id}/reject
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "reason": "صور الرخصة غير واضحة. يرجى إعادة الرفع"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| reason | string | Yes | Rejection reason (shown to driver) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم رفض المركبة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |
| 400 | `المركبة ليست في حالة انتظار` |

---

### 7.5 Flag Maintenance

Flags a vehicle for required maintenance.

```
POST /admin/vehicles/{id}/flag-maintenance
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**:
```json
{
  "maintenanceType": 2,
  "urgency": "High",
  "notes": "تآكل في تيل الفرامل - يجب التغيير فوراً",
  "deadlineDate": "2026-04-05"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| maintenanceType | int | Yes | 0-6 (see [Enums](#8-enums)) |
| urgency | string | Yes | "Low", "Medium", "High", "Critical" |
| notes | string | No | Additional notes |
| deadlineDate | string | No | Deadline in `YYYY-MM-DD` format |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل ملاحظة الصيانة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |

---

### 7.6 Deactivate Vehicle

Deactivates a vehicle (removes it from active fleet).

```
POST /admin/vehicles/{id}/deactivate
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إيقاف المركبة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |

---

### 7.7 Activate Vehicle

Re-activates a previously deactivated vehicle.

```
POST /admin/vehicles/{id}/activate
```

**Auth Required**: Yes (Bearer Token, Admin role)

**URL Params**: `id` (GUID) - Vehicle ID

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تفعيل المركبة",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المركبة غير موجودة` |

---

### 7.8 Get Pending Vehicles

Returns all vehicles awaiting admin approval.

```
GET /admin/vehicles/pending
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-5678-4562-b3fc-2c963f66afa6",
      "driverName": "أحمد محمد",
      "vehicleType": 0,
      "plateNumber": "أ ب ج 1234",
      "makeModel": "Honda PCX 150",
      "year": 2024,
      "isActive": false,
      "approvalStatus": 0,
      "nextMaintenanceDate": null,
      "needsAttention": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 7.9 Get Maintenance Due

Returns all vehicles that are due for maintenance.

```
GET /admin/vehicles/maintenance-due
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "driverId": "a1b2c3d4-5678-4562-b3fc-2c963f66afa6",
      "driverName": "أحمد محمد",
      "vehicleType": 0,
      "plateNumber": "أ ب ج 1234",
      "makeModel": "Honda PCX 150",
      "year": 2024,
      "isActive": true,
      "approvalStatus": 1,
      "nextMaintenanceDate": "2026-03-20",
      "needsAttention": true
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 7.10 Get Fleet Stats

Returns fleet-wide vehicle statistics for a date range.

```
GET /admin/vehicles/stats?dateFrom=2026-01-01&dateTo=2026-03-26
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Query Parameters**:
| Param | Type | Required | Description |
|-------|------|----------|-------------|
| dateFrom | string | No | Start date (`YYYY-MM-DD`) |
| dateTo | string | No | End date (`YYYY-MM-DD`) |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "totalVehicles": 250,
    "activeVehicles": 210,
    "pendingApproval": 15,
    "maintenanceDue": 12,
    "byType": [
      { "vehicleType": 0, "count": 120, "percentage": 48.00 },
      { "vehicleType": 1, "count": 60, "percentage": 24.00 },
      { "vehicleType": 2, "count": 40, "percentage": 16.00 },
      { "vehicleType": 3, "count": 20, "percentage": 8.00 },
      { "vehicleType": 4, "count": 10, "percentage": 4.00 }
    ],
    "totalMaintenanceCost": 125000.00
  },
  "message": null,
  "errors": null
}
```

---

### 7.11 Get Vehicles By Type

Returns vehicle count breakdown by type.

```
GET /admin/vehicles/by-type
```

**Auth Required**: Yes (Bearer Token, Admin role)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    { "vehicleType": 0, "count": 120, "percentage": 48.00 },
    { "vehicleType": 1, "count": 60, "percentage": 24.00 },
    { "vehicleType": 2, "count": 40, "percentage": 16.00 },
    { "vehicleType": 3, "count": 20, "percentage": 8.00 },
    { "vehicleType": 4, "count": 10, "percentage": 4.00 }
  ],
  "message": null,
  "errors": null
}
```

---

## 8. Enums

### VehicleType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Motorcycle | موتوسيكل |
| 1 | Car | سيارة |
| 2 | Van | فان |
| 3 | Truck | تراك |
| 4 | Bicycle | دراجة |

### MaintenanceType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | OilChange | تغيير زيت |
| 1 | TireChange | تغيير إطارات |
| 2 | BrakeService | صيانة فرامل |
| 3 | BatteryReplacement | تغيير بطارية |
| 4 | GeneralService | صيانة عامة |
| 5 | EngineRepair | إصلاح محرك |
| 6 | Other | أخرى |

### VehicleApprovalStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Pending | في الانتظار |
| 1 | Approved | مقبول |
| 2 | Rejected | مرفوض |

### BreakUrgency
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Low | منخفض |
| 1 | Medium | متوسط |
| 2 | High | مرتفع |
| 3 | Critical | حرج |

---

## 9. DTOs Reference

### Parking DTOs

#### CreateParkingSpotDto
```json
{
  "latitude": 30.0444,         // double, required
  "longitude": 31.2357,        // double, required
  "address": "string",         // string?, optional
  "notes": "string",           // string?, optional
  "qualityRating": 3,          // int, default 3, range 1-5
  "isPaid": false,             // bool, default false
  "paidAmount": null,          // decimal?, optional
  "isShared": false            // bool, default false
}
```

#### UpdateParkingSpotDto
```json
{
  "latitude": null,            // double?, optional
  "longitude": null,           // double?, optional
  "address": null,             // string?, optional
  "notes": null,               // string?, optional
  "qualityRating": null,       // int?, optional
  "isPaid": null,              // bool?, optional
  "paidAmount": null,          // decimal?, optional
  "isShared": null             // bool?, optional
}
```

#### ParkingSpotDto (Response)
```json
{
  "id": "guid",
  "latitude": 30.0444,
  "longitude": 31.2357,
  "address": "string",
  "qualityRating": 4,
  "isPaid": false,
  "usageCount": 12,
  "lastUsedAt": "2026-03-25T14:30:00Z"
}
```

#### NearbyQueryDto
```json
{
  "latitude": 30.0444,         // double, required
  "longitude": 31.2357,        // double, required
  "radiusKm": 1.0              // double, default 1.0
}
```

### Vehicle DTOs

#### CreateVehicleDto
```json
{
  "vehicleType": 0,                    // int (VehicleType), required
  "plateNumber": "string",             // string?, optional
  "makeModel": "string",               // string?, optional
  "year": 2024,                        // int?, optional
  "currentMileageKm": 15000.5,         // double, required
  "fuelConsumptionPer100Km": 2.50,     // decimal?, optional
  "fuelPricePerLiter": 12.25,          // decimal?, optional
  "insuranceExpiryDate": "2027-01-15"  // DateOnly?, optional
}
```

#### UpdateVehicleDto
```json
{
  "vehicleType": null,                 // int? (VehicleType), optional
  "plateNumber": null,                 // string?, optional
  "makeModel": null,                   // string?, optional
  "year": null,                        // int?, optional
  "currentMileageKm": null,            // double?, optional
  "fuelConsumptionPer100Km": null,     // decimal?, optional
  "fuelPricePerLiter": null,           // decimal?, optional
  "insuranceExpiryDate": null          // DateOnly?, optional
}
```

#### VehicleDto (Response)
```json
{
  "id": "guid",
  "vehicleType": 0,
  "plateNumber": "string",
  "makeModel": "string",
  "year": 2024,
  "currentMileageKm": 15000.5,
  "fuelConsumptionPer100Km": 2.50,
  "insuranceExpiryDate": "2027-01-15",
  "nextMaintenanceDate": "2026-04-01",
  "isActive": true
}
```

#### CreateMaintenanceDto
```json
{
  "maintenanceType": 0,        // int (MaintenanceType), required
  "cost": 350.00,              // decimal?, optional
  "mileageAtService": 15000.5, // double, required
  "nextDueMileage": 20000.0,   // double?, optional
  "nextDueDate": "2026-06-01", // DateOnly?, optional
  "notes": "string"            // string?, optional
}
```

#### MaintenanceRecordDto (Response)
```json
{
  "id": "guid",
  "maintenanceType": 0,
  "cost": 350.00,
  "mileageAtService": 15000.5,
  "nextDueMileage": 20000.0,
  "nextDueDate": "2026-06-01",
  "notes": "string",
  "servicedAt": "2026-03-26T12:00:00Z"
}
```

### Break DTOs

#### StartBreakDto
```json
{
  "latitude": 30.0444,                // double?, optional
  "longitude": 31.2357,               // double?, optional
  "locationDescription": "string",    // string?, optional
  "energyBefore": 3                   // int?, optional (1-5)
}
```

#### EndBreakDto
```json
{
  "energyAfter": 5                    // int?, optional (1-5)
}
```

#### BreakLogDto (Response)
```json
{
  "id": "guid",
  "startTime": "2026-03-26T14:00:00Z",
  "endTime": "2026-03-26T14:25:00Z",
  "durationMinutes": 25,
  "locationDescription": "string",
  "energyBefore": 3,
  "energyAfter": 5
}
```

#### BreakSuggestionDto (Response)
```json
{
  "shouldBreak": true,
  "urgency": 2,
  "suggestedDurationMinutes": 20,
  "reason": "string",
  "nearbySpots": [ /* ParkingSpotDto[] */ ]
}
```

### Admin DTOs

#### AdminVehicleFilterDto
```json
{
  "page": 1,                   // int, default 1
  "pageSize": 20,              // int, default 20
  "vehicleType": null,         // int? (VehicleType), optional
  "approvalStatus": null,      // int? (VehicleApprovalStatus), optional
  "maintenanceDue": null,      // bool?, optional
  "searchTerm": null           // string?, optional
}
```

#### AdminVehicleDto (Response)
```json
{
  "id": "guid",
  "driverId": "guid",
  "driverName": "string",
  "vehicleType": 0,
  "plateNumber": "string",
  "makeModel": "string",
  "year": 2024,
  "isActive": true,
  "approvalStatus": 1,
  "nextMaintenanceDate": "2026-04-01",
  "needsAttention": false
}
```

#### AdminVehicleDetailDto (Response, extends AdminVehicleDto)
```json
{
  "id": "guid",
  "driverId": "guid",
  "driverName": "string",
  "vehicleType": 0,
  "plateNumber": "string",
  "makeModel": "string",
  "year": 2024,
  "isActive": true,
  "approvalStatus": 1,
  "nextMaintenanceDate": "2026-04-01",
  "needsAttention": false,
  "currentMileageKm": 15000.5,
  "fuelConsumptionPer100Km": 2.50,
  "maintenanceHistory": [ /* MaintenanceRecordDto[] */ ],
  "totalMaintenanceCost": 1150.00
}
```

#### RejectVehicleDto
```json
{
  "reason": "string"           // string, required
}
```

#### FlagMaintenanceDto
```json
{
  "maintenanceType": 2,        // int (MaintenanceType), required
  "urgency": "High",           // string, required
  "notes": "string",           // string?, optional
  "deadlineDate": "2026-04-05" // DateOnly?, optional
}
```

#### VehicleFleetStatsDto (Response)
```json
{
  "totalVehicles": 250,
  "activeVehicles": 210,
  "pendingApproval": 15,
  "maintenanceDue": 12,
  "byType": [ /* VehicleTypeBreakdownDto[] */ ],
  "totalMaintenanceCost": 125000.00
}
```

#### VehicleTypeBreakdownDto (Response)
```json
{
  "vehicleType": 0,
  "count": 120,
  "percentage": 48.00
}
```

---

## 10. Error Messages Reference

### Parking Errors
| Message | When |
|---------|------|
| `مكان الركنة غير موجود` | Parking spot ID not found |
| `غير مصرح بالتعديل` | Trying to update/delete another driver's spot |

### Vehicle Errors
| Message | When |
|---------|------|
| `المركبة غير موجودة` | Vehicle ID not found |
| `المركبة مسجلة بالفعل` | Duplicate vehicle registration |
| `غير مصرح بالتعديل` | Trying to modify another driver's vehicle |

### Break Errors
| Message | When |
|---------|------|
| `يوجد استراحة نشطة بالفعل` | Starting a break while one is active |
| `لا يوجد استراحة نشطة` | Ending a break when none is active |

### Admin Vehicle Errors
| Message | When |
|---------|------|
| `المركبة غير موجودة` | Vehicle ID not found |
| `المركبة ليست في حالة انتظار` | Approving/rejecting a non-pending vehicle |

### General Errors
| Message | When |
|---------|------|
| `{field} مطلوب` | Required field is empty |
| `غير مصرح` | Missing or invalid Bearer token |
| `ليس لديك صلاحية` | Non-admin accessing admin endpoints |

---

## 11. Flutter Integration

### Dio Setup (shared with Auth)
```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Token interceptor (see AUTH_API.md for full setup)
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

### Parking Examples

```dart
// Get all my parking spots
Future<List<ParkingSpot>> getMyParkingSpots() async {
  final response = await dio.get('/parking');
  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((e) => ParkingSpot.fromJson(e))
        .toList();
  }
  throw Exception(response.data['message']);
}

// Save a new parking spot
Future<ParkingSpot> saveParkingSpot({
  required double lat,
  required double lng,
  String? address,
  int quality = 3,
  bool shared = true,
}) async {
  final response = await dio.post('/parking', data: {
    'latitude': lat,
    'longitude': lng,
    'address': address,
    'qualityRating': quality,
    'isShared': shared,
  });
  if (response.data['isSuccess'] == true) {
    return ParkingSpot.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Find nearby parking spots
Future<List<ParkingSpot>> findNearbySpots(double lat, double lng,
    {double radiusKm = 1.5}) async {
  final response = await dio.get('/parking/nearby', queryParameters: {
    'latitude': lat,
    'longitude': lng,
    'radiusKm': radiusKm,
  });
  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((e) => ParkingSpot.fromJson(e))
        .toList();
  }
  throw Exception(response.data['message']);
}
```

### Vehicle Examples

```dart
// Register a new vehicle
Future<Vehicle> registerVehicle({
  required int vehicleType,
  required double mileage,
  String? plateNumber,
  String? makeModel,
  int? year,
}) async {
  final response = await dio.post('/vehicles', data: {
    'vehicleType': vehicleType,
    'currentMileageKm': mileage,
    'plateNumber': plateNumber,
    'makeModel': makeModel,
    'year': year,
  });
  if (response.data['isSuccess'] == true) {
    return Vehicle.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Get my vehicles
Future<List<Vehicle>> getMyVehicles() async {
  final response = await dio.get('/vehicles');
  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((e) => Vehicle.fromJson(e))
        .toList();
  }
  throw Exception(response.data['message']);
}

// Add maintenance record
Future<MaintenanceRecord> addMaintenance({
  required String vehicleId,
  required int maintenanceType,
  required double mileage,
  double? cost,
  String? notes,
}) async {
  final response = await dio.post(
    '/vehicles/$vehicleId/maintenance',
    data: {
      'maintenanceType': maintenanceType,
      'mileageAtService': mileage,
      'cost': cost,
      'notes': notes,
    },
  );
  if (response.data['isSuccess'] == true) {
    return MaintenanceRecord.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Get maintenance history
Future<List<MaintenanceRecord>> getMaintenanceHistory(String vehicleId) async {
  final response = await dio.get('/vehicles/$vehicleId/maintenance');
  if (response.data['isSuccess'] == true) {
    return (response.data['data'] as List)
        .map((e) => MaintenanceRecord.fromJson(e))
        .toList();
  }
  throw Exception(response.data['message']);
}
```

### Break Examples

```dart
// Check if driver should take a break
Future<BreakSuggestion> checkBreakSuggestion() async {
  final response = await dio.get('/breaks/suggestion');
  if (response.data['isSuccess'] == true) {
    return BreakSuggestion.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Start a break
Future<BreakLog> startBreak({
  double? lat,
  double? lng,
  String? location,
  int? energyBefore,
}) async {
  final response = await dio.post('/breaks/start', data: {
    'latitude': lat,
    'longitude': lng,
    'locationDescription': location,
    'energyBefore': energyBefore,
  });
  if (response.data['isSuccess'] == true) {
    return BreakLog.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// End a break
Future<BreakLog> endBreak({int? energyAfter}) async {
  final response = await dio.post('/breaks/end', data: {
    'energyAfter': energyAfter,
  });
  if (response.data['isSuccess'] == true) {
    return BreakLog.fromJson(response.data['data']);
  }
  throw Exception(response.data['message']);
}

// Get break history (paginated)
Future<PagedResult<BreakLog>> getBreakHistory({
  int page = 1,
  int pageSize = 20,
}) async {
  final response = await dio.get('/breaks/history', queryParameters: {
    'page': page,
    'pageSize': pageSize,
  });
  if (response.data['isSuccess'] == true) {
    final data = response.data['data'];
    return PagedResult(
      items: (data['items'] as List)
          .map((e) => BreakLog.fromJson(e))
          .toList(),
      totalCount: data['totalCount'],
      page: data['page'],
      pageSize: data['pageSize'],
      totalPages: data['totalPages'],
    );
  }
  throw Exception(response.data['message']);
}
```

### Flutter Model Classes

```dart
// Enums
enum VehicleType { motorcycle, car, van, truck, bicycle }
enum MaintenanceType {
  oilChange, tireChange, brakeService,
  batteryReplacement, generalService, engineRepair, other
}
enum VehicleApprovalStatus { pending, approved, rejected }
enum BreakUrgency { low, medium, high, critical }

// Parking
class ParkingSpot {
  final String id;
  final double latitude;
  final double longitude;
  final String? address;
  final int qualityRating;
  final bool isPaid;
  final int usageCount;
  final DateTime lastUsedAt;

  ParkingSpot({
    required this.id,
    required this.latitude,
    required this.longitude,
    this.address,
    required this.qualityRating,
    required this.isPaid,
    required this.usageCount,
    required this.lastUsedAt,
  });

  factory ParkingSpot.fromJson(Map<String, dynamic> json) => ParkingSpot(
        id: json['id'],
        latitude: json['latitude'],
        longitude: json['longitude'],
        address: json['address'],
        qualityRating: json['qualityRating'],
        isPaid: json['isPaid'],
        usageCount: json['usageCount'],
        lastUsedAt: DateTime.parse(json['lastUsedAt']),
      );
}

// Vehicle
class Vehicle {
  final String id;
  final int vehicleType;
  final String? plateNumber;
  final String? makeModel;
  final int? year;
  final double currentMileageKm;
  final double? fuelConsumptionPer100Km;
  final String? insuranceExpiryDate;
  final String? nextMaintenanceDate;
  final bool isActive;

  Vehicle({
    required this.id,
    required this.vehicleType,
    this.plateNumber,
    this.makeModel,
    this.year,
    required this.currentMileageKm,
    this.fuelConsumptionPer100Km,
    this.insuranceExpiryDate,
    this.nextMaintenanceDate,
    required this.isActive,
  });

  factory Vehicle.fromJson(Map<String, dynamic> json) => Vehicle(
        id: json['id'],
        vehicleType: json['vehicleType'],
        plateNumber: json['plateNumber'],
        makeModel: json['makeModel'],
        year: json['year'],
        currentMileageKm: (json['currentMileageKm'] as num).toDouble(),
        fuelConsumptionPer100Km:
            (json['fuelConsumptionPer100Km'] as num?)?.toDouble(),
        insuranceExpiryDate: json['insuranceExpiryDate'],
        nextMaintenanceDate: json['nextMaintenanceDate'],
        isActive: json['isActive'],
      );
}

// Maintenance
class MaintenanceRecord {
  final String id;
  final int maintenanceType;
  final double? cost;
  final double mileageAtService;
  final double? nextDueMileage;
  final String? nextDueDate;
  final String? notes;
  final DateTime servicedAt;

  MaintenanceRecord({
    required this.id,
    required this.maintenanceType,
    this.cost,
    required this.mileageAtService,
    this.nextDueMileage,
    this.nextDueDate,
    this.notes,
    required this.servicedAt,
  });

  factory MaintenanceRecord.fromJson(Map<String, dynamic> json) =>
      MaintenanceRecord(
        id: json['id'],
        maintenanceType: json['maintenanceType'],
        cost: (json['cost'] as num?)?.toDouble(),
        mileageAtService: (json['mileageAtService'] as num).toDouble(),
        nextDueMileage: (json['nextDueMileage'] as num?)?.toDouble(),
        nextDueDate: json['nextDueDate'],
        notes: json['notes'],
        servicedAt: DateTime.parse(json['servicedAt']),
      );
}

// Break
class BreakLog {
  final String id;
  final DateTime startTime;
  final DateTime? endTime;
  final int? durationMinutes;
  final String? locationDescription;
  final int? energyBefore;
  final int? energyAfter;

  BreakLog({
    required this.id,
    required this.startTime,
    this.endTime,
    this.durationMinutes,
    this.locationDescription,
    this.energyBefore,
    this.energyAfter,
  });

  factory BreakLog.fromJson(Map<String, dynamic> json) => BreakLog(
        id: json['id'],
        startTime: DateTime.parse(json['startTime']),
        endTime:
            json['endTime'] != null ? DateTime.parse(json['endTime']) : null,
        durationMinutes: json['durationMinutes'],
        locationDescription: json['locationDescription'],
        energyBefore: json['energyBefore'],
        energyAfter: json['energyAfter'],
      );
}

class BreakSuggestion {
  final bool shouldBreak;
  final int urgency;
  final int suggestedDurationMinutes;
  final String reason;
  final List<ParkingSpot> nearbySpots;

  BreakSuggestion({
    required this.shouldBreak,
    required this.urgency,
    required this.suggestedDurationMinutes,
    required this.reason,
    required this.nearbySpots,
  });

  factory BreakSuggestion.fromJson(Map<String, dynamic> json) =>
      BreakSuggestion(
        shouldBreak: json['shouldBreak'],
        urgency: json['urgency'],
        suggestedDurationMinutes: json['suggestedDurationMinutes'],
        reason: json['reason'],
        nearbySpots: (json['nearbySpots'] as List)
            .map((e) => ParkingSpot.fromJson(e))
            .toList(),
      );
}

// Generic paged result
class PagedResult<T> {
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final int totalPages;

  PagedResult({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
    required this.totalPages,
  });
}
```
