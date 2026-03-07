# Sekka API - Settings Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/settings`
>
> **Last Updated**: 2026-03-07
>
> **Authentication**: All endpoints require `Authorization: Bearer <token>` header

---

## Table of Contents

1. [Overview](#1-overview)
2. [Endpoints](#2-endpoints)
   - [Get Settings](#21-get-settings)
   - [Update Settings](#22-update-settings)
   - [Update Focus Mode](#23-update-focus-mode)
   - [Update Quiet Hours](#24-update-quiet-hours)
   - [Update Notifications](#25-update-notifications)
   - [Update Cost Params](#26-update-cost-params)
   - [Get Notification Channels](#27-get-notification-channels)
   - [Update Notification Channel](#28-update-notification-channel)
   - [Update Notification Channels Bulk](#29-update-notification-channels-bulk)
   - [Update Home Location](#210-update-home-location)
3. [DTOs Reference](#3-dtos-reference)
4. [Enums](#4-enums)

---

## 1. Overview

The Settings API manages driver preferences including theme, language, notifications, focus mode, quiet hours, cost parameters, notification channels, and home location.

All settings updates use **partial update** (PATCH-like) — only send the fields you want to change.

---

## 2. Endpoints

### 2.1 Get Settings

```
GET /api/v1/settings
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": {
    "theme": 0,
    "language": "ar",
    "numberFormat": 0,
    "focusModeAutoTrigger": true,
    "focusModeSpeedThreshold": 40,
    "textToSpeechEnabled": false,
    "hapticFeedback": true,
    "highContrastMode": false,
    "notifyNewOrder": true,
    "notifyCashAlert": true,
    "notifyBreakReminder": true,
    "notifyMaintenance": true,
    "notifySettlement": true,
    "notifyAchievement": true,
    "notifySound": true,
    "notifyVibration": true,
    "quietHoursStart": "23:00:00",
    "quietHoursEnd": "07:00:00",
    "preferredMapApp": 0,
    "maxOrdersPerShift": 50,
    "autoSendReceipt": true,
    "locationTrackingInterval": 10,
    "offlineSyncInterval": 30,
    "homeLatitude": 30.0444,
    "homeLongitude": 31.2357,
    "homeAddress": "القاهرة، مصر",
    "backToBaseAlertEnabled": true,
    "backToBaseRadiusKm": 5.0
  }
}
```

---

### 2.2 Update Settings

```
PUT /api/v1/settings
```

**Request Body** (all fields optional):
```json
{
  "theme": 1,
  "language": "en",
  "numberFormat": 1,
  "focusModeAutoTrigger": false,
  "focusModeSpeedThreshold": 60,
  "textToSpeechEnabled": true,
  "hapticFeedback": false,
  "highContrastMode": true,
  "preferredMapApp": 1,
  "maxOrdersPerShift": 30,
  "autoSendReceipt": false,
  "locationTrackingInterval": 15,
  "offlineSyncInterval": 60
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.3 Update Focus Mode

```
PUT /api/v1/settings/focus-mode
```

**Request Body**:
```json
{
  "autoTrigger": true,
  "speedThreshold": 40
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| autoTrigger | bool | Yes | Auto-trigger when driving |
| speedThreshold | int | Yes | Speed (km/h) to trigger focus mode |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.4 Update Quiet Hours

```
PUT /api/v1/settings/quiet-hours
```

**Request Body**:
```json
{
  "enabled": true,
  "startTime": "23:00:00",
  "endTime": "07:00:00"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| enabled | bool | Yes | Enable/disable quiet hours |
| startTime | TimeOnly | No | Start time (HH:mm:ss) |
| endTime | TimeOnly | No | End time (HH:mm:ss) |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.5 Update Notifications

```
PUT /api/v1/settings/notifications
```

**Request Body** (all fields optional):
```json
{
  "notifyNewOrder": true,
  "notifyCashAlert": true,
  "notifyBreakReminder": false,
  "notifyMaintenance": true,
  "notifySettlement": true,
  "notifyAchievement": true,
  "notifySound": true,
  "notifyVibration": false
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.6 Update Cost Params

```
PUT /api/v1/settings/cost-params
```

**Request Body** (all fields optional):
```json
{
  "fuelPricePerLiter": 12.50,
  "fuelConsumptionPer100Km": 8.5,
  "hourlyRate": 50.00,
  "depreciationPerKm": 0.25
}
```

| Field | Type | Description |
|-------|------|-------------|
| fuelPricePerLiter | decimal | Current fuel price (EGP/liter) |
| fuelConsumptionPer100Km | decimal | Vehicle consumption (liters/100km) |
| hourlyRate | decimal | Driver hourly rate (EGP) |
| depreciationPerKm | decimal | Vehicle depreciation per km (EGP) |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.7 Get Notification Channels

```
GET /api/v1/settings/notification-channels
```

**Response** `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "notificationType": 0,
      "notificationTypeName": "NewOrder",
      "isEnabled": true,
      "soundEnabled": true,
      "soundName": "default",
      "vibrationEnabled": true,
      "vibrationPattern": "default",
      "ledColor": "#00FF00",
      "priority": 2,
      "showInLockScreen": true,
      "groupAlerts": false
    }
  ]
}
```

---

### 2.8 Update Notification Channel

```
PUT /api/v1/settings/notification-channels/{type}
```

| Path Param | Type | Description |
|-----------|------|-------------|
| type | NotificationType (int) | Notification type enum value |

**Request Body** (all fields optional):
```json
{
  "notificationType": 0,
  "isEnabled": true,
  "soundEnabled": true,
  "soundName": "alert_high",
  "vibrationEnabled": true,
  "vibrationPattern": "long",
  "ledColor": "#FF0000",
  "priority": 3,
  "showInLockScreen": true,
  "groupAlerts": false
}
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.9 Update Notification Channels Bulk

```
PUT /api/v1/settings/notification-channels/bulk
```

**Request Body**: Array of channel preferences
```json
[
  {
    "notificationType": 0,
    "isEnabled": true,
    "soundEnabled": true,
    "priority": 3
  },
  {
    "notificationType": 1,
    "isEnabled": false
  }
]
```

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

### 2.10 Update Home Location

```
POST /api/v1/settings/home-location
```

**Request Body**:
```json
{
  "homeLatitude": 30.0444,
  "homeLongitude": 31.2357,
  "homeAddress": "القاهرة، مدينة نصر"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| homeLatitude | double | Yes | Latitude |
| homeLongitude | double | Yes | Longitude |
| homeAddress | string | No | Human-readable address |

**Response** `200 OK`:
```json
{
  "success": true,
  "message": "تم تحديث الإعدادات بنجاح",
  "data": { ... }
}
```

---

## 3. DTOs Reference

### DriverPreferencesDto (Response)
| Field | Type | Description |
|-------|------|-------------|
| theme | ThemeMode | UI theme |
| language | string | App language (ar/en) |
| numberFormat | int | Number display format |
| focusModeAutoTrigger | bool | Auto-trigger focus mode |
| focusModeSpeedThreshold | int | Speed threshold (km/h) |
| textToSpeechEnabled | bool | TTS for navigation |
| hapticFeedback | bool | Haptic feedback enabled |
| highContrastMode | bool | Accessibility mode |
| notifyNewOrder | bool | New order notifications |
| notifyCashAlert | bool | Cash alert notifications |
| notifyBreakReminder | bool | Break reminder notifications |
| notifyMaintenance | bool | Maintenance notifications |
| notifySettlement | bool | Settlement notifications |
| notifyAchievement | bool | Achievement notifications |
| notifySound | bool | Global sound toggle |
| notifyVibration | bool | Global vibration toggle |
| quietHoursStart | TimeOnly | Quiet hours start |
| quietHoursEnd | TimeOnly | Quiet hours end |
| preferredMapApp | MapApp | Preferred navigation app |
| maxOrdersPerShift | int | Max orders per shift |
| autoSendReceipt | bool | Auto-send receipt to customer |
| locationTrackingInterval | int | GPS interval (seconds) |
| offlineSyncInterval | int | Offline sync interval (seconds) |
| homeLatitude | double | Home latitude |
| homeLongitude | double | Home longitude |
| homeAddress | string | Home address text |
| backToBaseAlertEnabled | bool | Alert when near home |
| backToBaseRadiusKm | decimal | Radius for home alert (km) |

---

## 4. Enums

### ThemeMode
| Value | Name |
|-------|------|
| 0 | System |
| 1 | Light |
| 2 | Dark |

### MapApp
| Value | Name |
|-------|------|
| 0 | GoogleMaps |
| 1 | Waze |
| 2 | AppleMaps |

### NotificationType
| Value | Name |
|-------|------|
| 0 | NewOrder |
| 1 | CashAlert |
| 2 | BreakReminder |
| 3 | Maintenance |
| 4 | Settlement |
| 5 | Achievement |
| 6 | SystemAlert |
| 7 | Chat |

### NotificationPriority
| Value | Name |
|-------|------|
| 0 | Low |
| 1 | Default |
| 2 | High |
| 3 | Urgent |

---

## Flutter/Dio Integration

```dart
// Get all settings
final settings = await dio.get('/api/v1/settings');

// Update theme and language
await dio.put('/api/v1/settings', data: {
  'theme': 2, // Dark
  'language': 'ar',
});

// Update focus mode
await dio.put('/api/v1/settings/focus-mode', data: {
  'autoTrigger': true,
  'speedThreshold': 40,
});

// Update quiet hours
await dio.put('/api/v1/settings/quiet-hours', data: {
  'enabled': true,
  'startTime': '23:00:00',
  'endTime': '07:00:00',
});

// Toggle individual notification
await dio.put('/api/v1/settings/notifications', data: {
  'notifyNewOrder': false,
});

// Update notification channel
await dio.put('/api/v1/settings/notification-channels/0', data: {
  'notificationType': 0,
  'soundEnabled': true,
  'soundName': 'alert_high',
  'priority': 3,
});

// Set home location
await dio.post('/api/v1/settings/home-location', data: {
  'homeLatitude': 30.0444,
  'homeLongitude': 31.2357,
  'homeAddress': 'القاهرة، مدينة نصر',
});
```
