# Sekka API - Communication Module Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1`
>
> **Last Updated**: 2026-03-26

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Enums](#3-enums)
4. [Notifications](#4-notifications)
   - [Get Notifications (Paged)](#41-get-notifications-paged)
   - [Mark as Read](#42-mark-as-read)
   - [Mark All as Read](#43-mark-all-as-read)
   - [Get Unread Count](#44-get-unread-count)
5. [SOS Emergency](#5-sos-emergency)
   - [Activate SOS](#51-activate-sos)
   - [Dismiss SOS](#52-dismiss-sos)
   - [Resolve SOS](#53-resolve-sos)
   - [Get SOS History](#54-get-sos-history)
   - [SOS Flow Diagram](#55-sos-flow-diagram)
6. [Message Templates](#6-message-templates)
   - [Get Templates](#61-get-templates)
   - [Create Template](#62-create-template)
   - [Update Template](#63-update-template)
   - [Delete Template](#64-delete-template)
   - [Record Template Usage](#65-record-template-usage)
7. [Chat](#7-chat)
   - [Get Conversations](#71-get-conversations)
   - [Create Conversation](#72-create-conversation)
   - [Get Messages](#73-get-messages)
   - [Send Message](#74-send-message)
   - [Close Conversation](#75-close-conversation)
   - [Mark Message Read](#76-mark-message-read)
   - [Get Unread Message Count](#77-get-unread-message-count)
   - [Chat Flow Diagram](#78-chat-flow-diagram)
8. [Admin Notifications](#8-admin-notifications)
   - [Broadcast Notification](#81-broadcast-notification)
   - [Get Notification History](#82-get-notification-history)
   - [Send to Driver](#83-send-to-driver)
9. [Admin SOS](#9-admin-sos)
   - [Get Active SOS](#91-get-active-sos)
   - [Get All SOS](#92-get-all-sos)
   - [Get SOS by ID](#93-get-sos-by-id)
   - [Acknowledge SOS](#94-acknowledge-sos)
   - [Escalate SOS](#95-escalate-sos)
   - [Resolve SOS (Admin)](#96-resolve-sos-admin)
   - [Mark False Alarm](#97-mark-false-alarm)
   - [Get SOS Stats](#98-get-sos-stats)
   - [Get Response Times](#99-get-response-times)
   - [Get SOS Heatmap](#910-get-sos-heatmap)
10. [SignalR Hubs](#10-signalr-hubs)
    - [Chat Hub](#101-chat-hub)
    - [Notification Hub](#102-notification-hub)
    - [Cash Alert Hub](#103-cash-alert-hub)
11. [DTOs Reference](#11-dtos-reference)
12. [Error Messages Reference](#12-error-messages-reference)
13. [Flutter Integration](#13-flutter-integration)
    - [Dio HTTP Examples](#131-dio-http-examples)
    - [SignalR Integration](#132-signalr-integration)

---

## 1. Overview

The Communication module handles all real-time and asynchronous communication between drivers, admin, and the system. It covers push notifications, SOS emergency alerts, in-app chat with support, quick message templates, and real-time updates via SignalR.

| Feature | Detail |
|---------|--------|
| Notifications | Push + In-App with paged history |
| SOS Emergency | GPS-based alert with escalation levels |
| Chat | Driver-to-support with real-time via SignalR |
| Message Templates | Quick replies (system + custom) |
| Real-time | 3 SignalR hubs (Chat, Notifications, CashAlerts) |

**All endpoints require `Authorization: Bearer <token>` unless stated otherwise.**

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

### Pagination Query Parameters
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| page | int | 1 | Page number (1-based) |
| pageSize | int | 20 | Items per page |

### HTTP Status Codes Used
| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid/missing token) |
| 404 | Not Found |
| 409 | Conflict |
| 500 | Server Error |

---

## 3. Enums

### SOSStatus
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | Active | نشط | SOS currently active |
| 1 | Dismissed | ملغي | Dismissed by driver (accidental) |
| 2 | Resolved | تم الحل | Resolved by driver or admin |
| 3 | FalseAlarm | بلاغ كاذب | Marked as false alarm |

### SOSEscalationLevel
| Value | Name | Arabic | Description |
|-------|------|--------|-------------|
| 0 | Normal | عادي | Initial level |
| 1 | Elevated | مرتفع | Escalated by admin |
| 2 | Critical | حرج | Critical situation |
| 3 | PoliceNotified | تم إبلاغ الشرطة | Authorities contacted |

### MessageCategory
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Delivery | توصيل |
| 1 | Payment | دفع |
| 2 | Greeting | تحية |
| 3 | Apology | اعتذار |
| 4 | General | عام |

### ChatType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Support | دعم فني |
| 1 | Complaint | شكوى |
| 2 | Suggestion | اقتراح |
| 3 | General | عام |

### MessageStatus
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Sent | تم الإرسال |
| 1 | Delivered | تم التوصيل |
| 2 | Read | مقروءة |

### NotificationChannel
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Push | إشعار فوري |
| 1 | SMS | رسالة نصية |
| 2 | Email | بريد إلكتروني |
| 3 | WhatsApp | واتساب |
| 4 | InApp | داخل التطبيق |

### NotificationType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | NewOrder | طلب جديد |
| 1 | CashAlert | تنبيه نقدي |
| 2 | BreakReminder | تذكير بالراحة |
| 3 | Maintenance | صيانة |
| 4 | Settlement | تسوية |
| 5 | Achievement | إنجاز |
| 6 | SystemUpdate | تحديث النظام |
| 7 | Chat | محادثة |

---

## 4. Notifications

**Base Path**: `/api/v1/notifications`
**Auth Required**: Yes (Bearer Token)

---

### 4.1 Get Notifications (Paged)

Returns the driver's notifications with pagination.

```
GET /notifications?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "notificationType": "NewOrder",
        "title": "طلب جديد",
        "message": "لديك طلب توصيل جديد في منطقة المعادي",
        "isRead": false,
        "actionType": "navigate",
        "actionData": "{\"screen\":\"order_details\",\"orderId\":\"abc123\"}",
        "priority": 1,
        "createdAt": "2026-03-26T10:30:00Z"
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  },
  "message": null,
  "errors": null
}
```

**NotificationDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Notification ID |
| notificationType | string | Type (see NotificationType enum) |
| title | string | Notification title |
| message | string | Notification body text |
| isRead | bool | Whether the driver has read it |
| actionType | string? | Action type (e.g., "navigate", "url") |
| actionData | string? | JSON payload for the action |
| priority | int | Priority level (0=low, 1=normal, 2=high) |
| createdAt | datetime | When the notification was created |

---

### 4.2 Mark as Read

Marks a single notification as read.

```
PUT /notifications/{id}/read
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Notification ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعليم الإشعار كمقروء",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الإشعار غير موجود` |

---

### 4.3 Mark All as Read

Marks all notifications for the driver as read.

```
PUT /notifications/read-all
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعليم كل الإشعارات كمقروءة",
  "errors": null
}
```

---

### 4.4 Get Unread Count

Returns the number of unread notifications for the driver.

```
GET /notifications/unread-count
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": 7,
  "message": null,
  "errors": null
}
```

---

## 5. SOS Emergency

**Base Path**: `/api/v1/sos`
**Auth Required**: Yes (Bearer Token)

---

### 5.1 Activate SOS

Activates an emergency SOS alert with the driver's current GPS coordinates. Notifies emergency contacts and admin.

```
POST /sos/activate
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "latitude": 30.0444,
  "longitude": 31.2357,
  "notes": "تعرضت لحادث على الطريق الدائري"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| latitude | double | Yes | GPS latitude |
| longitude | double | Yes | GPS longitude |
| notes | string | No | Additional details about the emergency |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "latitude": 30.0444,
    "longitude": 31.2357,
    "status": "Active",
    "notifiedContacts": ["+201012345678", "+201123456789"],
    "adminNotified": true,
    "notes": "تعرضت لحادث على الطريق الدائري",
    "activatedAt": "2026-03-26T10:30:00Z",
    "resolvedAt": null
  },
  "message": "تم تفعيل حالة الطوارئ",
  "errors": null
}
```

---

### 5.2 Dismiss SOS

Dismisses an active SOS alert (e.g., accidental activation).

```
POST /sos/{id}/dismiss
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - SOS alert ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إلغاء حالة الطوارئ",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `بلاغ الطوارئ غير موجود` |
| 400 | `بلاغ الطوارئ تم حله بالفعل` |

---

### 5.3 Resolve SOS

Resolves an active SOS alert with a resolution note.

```
POST /sos/{id}/resolve
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - SOS alert ID

**Request Body**:
```json
{
  "resolution": "تم حل المشكلة بمساعدة المارة",
  "wasFalseAlarm": false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| resolution | string | No | Description of how it was resolved |
| wasFalseAlarm | bool | Yes | Whether it was a false alarm |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حل حالة الطوارئ",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `بلاغ الطوارئ غير موجود` |
| 400 | `بلاغ الطوارئ تم حله بالفعل` |

---

### 5.4 Get SOS History

Returns the driver's SOS alert history with pagination.

```
GET /sos/history?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "latitude": 30.0444,
        "longitude": 31.2357,
        "status": "Resolved",
        "notifiedContacts": ["+201012345678"],
        "adminNotified": true,
        "notes": "تعرضت لحادث",
        "activatedAt": "2026-03-25T14:00:00Z",
        "resolvedAt": "2026-03-25T14:15:00Z"
      }
    ],
    "totalCount": 3,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

**SOSLogDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | GUID | SOS alert ID |
| latitude | double | GPS latitude at activation |
| longitude | double | GPS longitude at activation |
| status | string | Status (Active, Dismissed, Resolved, FalseAlarm) |
| notifiedContacts | string[] | Phone numbers of notified emergency contacts |
| adminNotified | bool | Whether admin was notified |
| notes | string? | Driver's notes |
| activatedAt | datetime | When SOS was activated |
| resolvedAt | datetime? | When SOS was resolved (null if still active) |

---

### 5.5 SOS Flow Diagram

```
                    ┌─────────────────┐
                    │  Driver in       │
                    │  Emergency       │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │ POST /sos/      │
                    │   activate      │
                    │ {lat, lng, note}│
                    └────────┬────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
     ┌────────▼──────┐ ┌────▼─────┐ ┌──────▼──────┐
     │ Notify         │ │ Notify   │ │ SignalR     │
     │ Emergency      │ │ Admin    │ │ Real-time   │
     │ Contacts (SMS) │ │ Dashboard│ │ Update      │
     └────────────────┘ └──────────┘ └─────────────┘
                             │
                    ┌────────▼────────┐
                    │  SOS Active     │
                    │  (Status = 0)   │
                    └────┬───────┬────┘
                         │       │
              ┌──────────┘       └──────────┐
              │                             │
     ┌────────▼──────┐           ┌──────────▼────────┐
     │ Accidental?   │           │ Resolved?         │
     │               │           │                   │
     │ POST /sos/    │           │ POST /sos/        │
     │  {id}/dismiss │           │  {id}/resolve     │
     │               │           │ {resolution,      │
     │ Status => 1   │           │  wasFalseAlarm}   │
     └───────────────┘           └──────────┬────────┘
                                            │
                                 ┌──────────┴──────────┐
                                 │                     │
                          wasFalseAlarm?          wasFalseAlarm?
                            = true                  = false
                                 │                     │
                        ┌────────▼──────┐    ┌─────────▼─────┐
                        │ Status => 3   │    │ Status => 2   │
                        │ (FalseAlarm)  │    │ (Resolved)    │
                        └───────────────┘    └───────────────┘

          ── Admin Escalation Path ──

     ┌─────────────────────────────────────────────────┐
     │  Normal (0) ──► Elevated (1) ──► Critical (2)  │
     │                                    │            │
     │                           PoliceNotified (3)    │
     └─────────────────────────────────────────────────┘
```

---

## 6. Message Templates

**Base Path**: `/api/v1/message-templates`
**Auth Required**: Yes (Bearer Token)

Quick-reply message templates that drivers use during deliveries. The system provides default templates and drivers can create custom ones.

---

### 6.1 Get Templates

Returns all available templates (system defaults + driver's custom templates), sorted by usage.

```
GET /message-templates
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "messageText": "أنا في الطريق إليك",
      "category": 0,
      "usageCount": 42,
      "isSystemTemplate": true,
      "sortOrder": 1
    },
    {
      "id": "7ab85f64-1234-4562-b3fc-2c963f66afa6",
      "messageText": "معلش اتأخرت، هوصل حالا",
      "category": 3,
      "usageCount": 8,
      "isSystemTemplate": false,
      "sortOrder": 5
    }
  ],
  "message": null,
  "errors": null
}
```

**MessageTemplateDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Template ID |
| messageText | string | The template text |
| category | int | MessageCategory enum (0-4) |
| usageCount | int | How many times this template was used |
| isSystemTemplate | bool | true = system default, false = custom |
| sortOrder | int | Display order |

---

### 6.2 Create Template

Creates a custom message template for the driver.

```
POST /message-templates
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "messageText": "الطلب جاهز، هل أنت متاح؟",
  "category": 0
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| messageText | string | Yes | Template text |
| category | int | Yes | MessageCategory (0=Delivery, 1=Payment, 2=Greeting, 3=Apology, 4=General) |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "9ca85f64-5717-4562-b3fc-2c963f66afa6",
    "messageText": "الطلب جاهز، هل أنت متاح؟",
    "category": 0,
    "usageCount": 0,
    "isSystemTemplate": false,
    "sortOrder": 0
  },
  "message": "تم إنشاء القالب بنجاح",
  "errors": null
}
```

---

### 6.3 Update Template

Updates a custom message template. Only the driver's own custom templates can be updated.

```
PUT /message-templates/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Template ID

**Request Body**:
```json
{
  "messageText": "الطلب جاهز وهوصلك حالا",
  "category": 0,
  "sortOrder": 2
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| messageText | string | No | Updated text |
| category | int | No | Updated category (0-4) |
| sortOrder | int | No | Updated display order |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "9ca85f64-5717-4562-b3fc-2c963f66afa6",
    "messageText": "الطلب جاهز وهوصلك حالا",
    "category": 0,
    "usageCount": 5,
    "isSystemTemplate": false,
    "sortOrder": 2
  },
  "message": "تم تحديث القالب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `قالب الرسالة غير موجود` |

---

### 6.4 Delete Template

Deletes a custom message template. System templates cannot be deleted.

```
DELETE /message-templates/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Template ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم حذف القالب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `قالب الرسالة غير موجود` |
| 400 | `لا يمكن حذف قالب النظام` |

---

### 6.5 Record Template Usage

Records that a template was used (increments usage count). Call this when the driver selects a quick-reply template.

```
POST /message-templates/{id}/use
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Template ID

**Request Body**: None

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
| 404 | `قالب الرسالة غير موجود` |

---

## 7. Chat

**Base Path**: `/api/v1/chat`
**Auth Required**: Yes (Bearer Token)

Driver-to-support chat system. Drivers can create conversations categorized by type (support, complaint, suggestion, general) and exchange messages with the admin/support team in real-time via SignalR.

---

### 7.1 Get Conversations

Returns the driver's chat conversations with pagination.

```
GET /chat/conversations?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "chatType": 0,
        "subject": "مشكلة في الدفع",
        "isClosed": false,
        "lastMessage": "تم حل المشكلة، شكرا لتواصلك",
        "lastMessageAt": "2026-03-26T09:45:00Z",
        "unreadCount": 2,
        "createdAt": "2026-03-25T14:00:00Z"
      }
    ],
    "totalCount": 5,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

**ConversationDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Conversation ID |
| chatType | int | ChatType enum (0=Support, 1=Complaint, 2=Suggestion, 3=General) |
| subject | string? | Conversation subject/topic |
| isClosed | bool | Whether the conversation is closed |
| lastMessage | string? | Preview of the last message |
| lastMessageAt | datetime? | When the last message was sent |
| unreadCount | int | Number of unread messages in this conversation |
| createdAt | datetime | When the conversation was created |

---

### 7.2 Create Conversation

Creates a new chat conversation with an initial message.

```
POST /chat/conversations
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "chatType": 0,
  "subject": "مشكلة في الدفع",
  "initialMessage": "مش قادر أستلم المبلغ من العميل عن طريق التطبيق"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| chatType | int | Yes | 0=Support, 1=Complaint, 2=Suggestion, 3=General |
| subject | string | No | Topic/subject of the conversation |
| initialMessage | string | Yes | First message content |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "chatType": 0,
    "subject": "مشكلة في الدفع",
    "isClosed": false,
    "lastMessage": "مش قادر أستلم المبلغ من العميل عن طريق التطبيق",
    "lastMessageAt": "2026-03-26T10:30:00Z",
    "unreadCount": 0,
    "createdAt": "2026-03-26T10:30:00Z"
  },
  "message": "تم إنشاء المحادثة بنجاح",
  "errors": null
}
```

---

### 7.3 Get Messages

Returns messages in a conversation with pagination (newest first).

```
GET /chat/conversations/{id}/messages?page=1&pageSize=20
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Conversation ID

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-5717-4562-b3fc-2c963f66afa6",
        "senderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "senderName": "أحمد محمد",
        "senderType": "Driver",
        "content": "مش قادر أستلم المبلغ من العميل عن طريق التطبيق",
        "attachmentUrl": null,
        "status": 2,
        "createdAt": "2026-03-26T10:30:00Z"
      },
      {
        "id": "b2c3d4e5-5717-4562-b3fc-2c963f66afa6",
        "senderId": "00000000-0000-0000-0000-000000000001",
        "senderName": "فريق الدعم",
        "senderType": "Admin",
        "content": "أهلا أحمد، هنساعدك تحل المشكلة. ممكن تبعتلي رقم الطلب؟",
        "attachmentUrl": null,
        "status": 1,
        "createdAt": "2026-03-26T10:32:00Z"
      }
    ],
    "totalCount": 2,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "message": null,
  "errors": null
}
```

**ChatMessageDto Fields**:
| Field | Type | Description |
|-------|------|-------------|
| id | GUID | Message ID |
| senderId | GUID | ID of the sender |
| senderName | string | Display name of the sender |
| senderType | string | "Driver" or "Admin" |
| content | string | Message text content |
| attachmentUrl | string? | URL to attachment (image, etc.) |
| status | int | MessageStatus enum (0=Sent, 1=Delivered, 2=Read) |
| createdAt | datetime | When the message was sent |

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المحادثة غير موجودة` |

---

### 7.4 Send Message

Sends a message in an existing conversation.

```
POST /chat/conversations/{id}/messages
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Conversation ID

**Request Body**:
```json
{
  "content": "رقم الطلب هو #12345",
  "attachmentUrl": null
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| content | string | Yes | Message text |
| attachmentUrl | string | No | URL to an uploaded attachment |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "id": "c3d4e5f6-5717-4562-b3fc-2c963f66afa6",
    "senderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "senderName": "أحمد محمد",
    "senderType": "Driver",
    "content": "رقم الطلب هو #12345",
    "attachmentUrl": null,
    "status": 0,
    "createdAt": "2026-03-26T10:35:00Z"
  },
  "message": "تم إرسال الرسالة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المحادثة غير موجودة` |
| 400 | `المحادثة مغلقة` |

---

### 7.5 Close Conversation

Closes a conversation. No more messages can be sent after closing.

```
PUT /chat/conversations/{id}/close
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Conversation ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إغلاق المحادثة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `المحادثة غير موجودة` |

---

### 7.6 Mark Message Read

Marks a specific message as read.

```
PUT /chat/messages/{id}/read
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Message ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تعليم الرسالة كمقروءة",
  "errors": null
}
```

---

### 7.7 Get Unread Message Count

Returns the total number of unread chat messages across all conversations.

```
GET /chat/unread-count
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": 3,
  "message": null,
  "errors": null
}
```

---

### 7.8 Chat Flow Diagram

```
     ┌──────────────────┐
     │  Driver opens     │
     │  Chat screen      │
     └────────┬─────────┘
              │
     ┌────────▼─────────┐
     │ GET /chat/        │
     │  conversations    │◄──── List existing conversations
     └────────┬─────────┘
              │
     ┌────────▼──────────────────────┐
     │  Has open conversation?       │
     └────┬─────────────────────┬────┘
          │                     │
         Yes                    No
          │                     │
     ┌────▼──────────┐   ┌─────▼──────────────┐
     │ GET messages   │   │ POST /chat/         │
     │ for that       │   │  conversations      │
     │ conversation   │   │ {chatType, subject, │
     │                │   │  initialMessage}    │
     └────┬──────────┘   └─────┬──────────────┘
          │                     │
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │  Connect SignalR    │
          │  ChatHub            │
          │                     │
          │  JoinConversation   │
          │  (conversationId)   │
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │  Real-time chat     │
          │                     │
          │  Send: invoke       │
          │   "SendMessage"     │
          │                     │
          │  Receive: listen    │
          │   "ReceiveMessage"  │
          │                     │
          │  Typing indicators: │
          │   "StartTyping"     │
          │   "StopTyping"      │
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │  Close conversation │
          │  PUT /chat/         │
          │  conversations/     │
          │   {id}/close        │
          └─────────────────────┘
```

---

## 8. Admin Notifications

**Base Path**: `/api/v1/admin/notifications`
**Auth Required**: Yes (Bearer Token with Admin role)

> **Note**: These endpoints are under development and currently return `400` with a development message.

---

### 8.1 Broadcast Notification

Sends a notification to all drivers (or a specific region).

```
POST /admin/notifications/broadcast
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "title": "تحديث مهم",
  "message": "سيتم إيقاف الخدمة للصيانة من الساعة 2 حتى 4 فجرا",
  "priority": 2,
  "targetRegion": "القاهرة"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | Notification title |
| message | string | Yes | Notification body |
| priority | int | Yes | 0=low, 1=normal, 2=high |
| targetRegion | string | No | Target region (null = all drivers) |

---

### 8.2 Get Notification History

Returns paginated history of sent admin notifications.

```
GET /admin/notifications/history?page=1&pageSize=20
```

**Auth Required**: Yes (Admin)

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

---

### 8.3 Send to Driver

Sends a notification to a specific driver.

```
POST /admin/notifications/send-to-driver
```

**Auth Required**: Yes (Admin)

**Request Body**:
```json
{
  "driverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "تنبيه خاص",
  "message": "يرجى مراجعة حسابك المالي",
  "priority": 1
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| driverId | GUID | Yes | Target driver ID |
| title | string | Yes | Notification title |
| message | string | Yes | Notification body |
| priority | int | Yes | 0=low, 1=normal, 2=high |

---

## 9. Admin SOS

**Base Path**: `/api/v1/admin/sos`
**Auth Required**: Yes (Bearer Token with Admin role)

> **Note**: These endpoints are under development and currently return `400` with a development message.

---

### 9.1 Get Active SOS

Returns all currently active SOS alerts.

```
GET /admin/sos/active
```

**Auth Required**: Yes (Admin)

---

### 9.2 Get All SOS

Returns all SOS alerts with pagination.

```
GET /admin/sos?page=1&pageSize=20
```

**Auth Required**: Yes (Admin)

**Query Parameters**:
| Param | Type | Default |
|-------|------|---------|
| page | int | 1 |
| pageSize | int | 20 |

---

### 9.3 Get SOS by ID

Returns full details of a specific SOS alert.

```
GET /admin/sos/{id}
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS alert ID

---

### 9.4 Acknowledge SOS

Acknowledges receipt of an SOS alert (admin sees it and takes ownership).

```
PUT /admin/sos/{id}/acknowledge
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS alert ID

---

### 9.5 Escalate SOS

Escalates an SOS alert to a higher level (Normal -> Elevated -> Critical -> PoliceNotified).

```
PUT /admin/sos/{id}/escalate
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS alert ID

---

### 9.6 Resolve SOS (Admin)

Admin resolves an SOS alert.

```
PUT /admin/sos/{id}/resolve
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS alert ID

**Request Body**:
```json
{
  "resolution": "تم إرسال مساعدة للسائق وتم حل المشكلة",
  "wasFalseAlarm": false
}
```

---

### 9.7 Mark False Alarm

Marks an SOS alert as a false alarm.

```
PUT /admin/sos/{id}/mark-false-alarm
```

**Auth Required**: Yes (Admin)

**URL Params**: `id` (GUID) - SOS alert ID

---

### 9.8 Get SOS Stats

Returns SOS statistics (total counts by status, average response time, etc.).

```
GET /admin/sos/stats
```

**Auth Required**: Yes (Admin)

---

### 9.9 Get Response Times

Returns SOS response time analytics.

```
GET /admin/sos/response-times
```

**Auth Required**: Yes (Admin)

---

### 9.10 Get SOS Heatmap

Returns geographic data for SOS alerts (for heatmap visualization).

```
GET /admin/sos/heatmap
```

**Auth Required**: Yes (Admin)

---

## 10. SignalR Hubs

All SignalR hubs require authentication. Pass the JWT token as a query parameter when connecting.

**Connection URL format**:
```
wss://sekka.runasp.net/hubs/{hub-name}?access_token={jwt_token}
```

---

### 10.1 Chat Hub

**URL**: `wss://sekka.runasp.net/hubs/chat?access_token={token}`

#### Client-to-Server Methods (invoke)

| Method | Parameters | Description |
|--------|-----------|-------------|
| `JoinConversation` | `conversationId` (string) | Join a conversation group to receive messages |
| `LeaveConversation` | `conversationId` (string) | Leave a conversation group |
| `SendMessage` | `conversationId` (string), `content` (string) | Send a message in real-time |
| `StartTyping` | `conversationId` (string) | Notify others that you are typing |
| `StopTyping` | `conversationId` (string) | Notify others that you stopped typing |

#### Server-to-Client Events (listen)

| Event | Payload | Description |
|-------|---------|-------------|
| `ReceiveMessage` | `{senderId, content, sentAt}` | New message received in conversation |
| `UserJoined` | `userId` (string) | A user joined the conversation |
| `UserLeft` | `userId` (string) | A user left the conversation |
| `UserTyping` | `userId` (string) | A user started typing |
| `UserStoppedTyping` | `userId` (string) | A user stopped typing |
| `ConversationClosed` | `conversationId` (string) | Conversation was closed |

#### ReceiveMessage Payload
```json
{
  "senderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "أهلا، محتاج مساعدة في الطلب",
  "sentAt": "2026-03-26T10:30:00Z"
}
```

---

### 10.2 Notification Hub

**URL**: `wss://sekka.runasp.net/hubs/notifications?access_token={token}`

The hub automatically joins the driver's notification group on connection.

#### Client-to-Server Methods (invoke)

| Method | Parameters | Description |
|--------|-----------|-------------|
| `MarkAsRead` | `notificationId` (string) | Mark a notification as read via hub |
| `MarkAllAsRead` | (none) | Mark all notifications as read via hub |

#### Server-to-Client Events (listen)

| Event | Payload | Description |
|-------|---------|-------------|
| `NewNotification` | NotificationDto | New notification received |
| `NotificationRead` | `notificationId` (string) | Confirmation that notification was marked read |
| `AllNotificationsRead` | (none) | Confirmation that all were marked read |
| `BroadcastMessage` | message object | System-wide broadcast |
| `SettlementApproved` | settlement object | Settlement was approved |
| `PaymentApproved` | payment object | Payment was approved |

#### NewNotification Payload
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "notificationType": "CashAlert",
  "title": "تنبيه نقدي",
  "message": "لديك مبلغ كبير. يرجى التسوية",
  "isRead": false,
  "actionType": "navigate",
  "actionData": "{\"screen\":\"cash_settlement\"}",
  "priority": 2,
  "createdAt": "2026-03-26T10:30:00Z"
}
```

---

### 10.3 Cash Alert Hub

**URL**: `wss://sekka.runasp.net/hubs/cash-alerts?access_token={token}`

The hub automatically joins the driver's cash alert group on connection.

#### Client-to-Server Methods (invoke)

| Method | Parameters | Description |
|--------|-----------|-------------|
| `AcknowledgeAlert` | `alertId` (string) | Acknowledge a cash alert |

#### Server-to-Client Events (listen)

| Event | Payload | Description |
|-------|---------|-------------|
| `CashThresholdExceeded` | cashStatus object | Driver exceeded cash-on-hand threshold |
| `SettlementReminder` | reminder object | Reminder to settle cash |
| `DailySettlementSummary` | summary object | End-of-day settlement summary |
| `DepositConfirmed` | deposit object | Cash deposit was confirmed |
| `AlertAcknowledged` | `alertId` (string) | Confirmation that alert was acknowledged |

---

## 11. DTOs Reference

### NotificationDto
```json
{
  "id": "GUID",
  "notificationType": "string",
  "title": "string",
  "message": "string",
  "isRead": false,
  "actionType": "string?",
  "actionData": "string?",
  "priority": 0,
  "createdAt": "datetime"
}
```

### ActivateSOSDto (Request)
```json
{
  "latitude": 0.0,
  "longitude": 0.0,
  "notes": "string?"
}
```

### ResolveSOSDto (Request)
```json
{
  "resolution": "string?",
  "wasFalseAlarm": false
}
```

### SOSLogDto (Response)
```json
{
  "id": "GUID",
  "latitude": 0.0,
  "longitude": 0.0,
  "status": "string",
  "notifiedContacts": ["string"],
  "adminNotified": false,
  "notes": "string?",
  "activatedAt": "datetime",
  "resolvedAt": "datetime?"
}
```

### CreateMessageTemplateDto (Request)
```json
{
  "messageText": "string",
  "category": 0
}
```

### UpdateMessageTemplateDto (Request)
```json
{
  "messageText": "string?",
  "category": 0,
  "sortOrder": 0
}
```

### MessageTemplateDto (Response)
```json
{
  "id": "GUID",
  "messageText": "string",
  "category": 0,
  "usageCount": 0,
  "isSystemTemplate": false,
  "sortOrder": 0
}
```

### CreateConversationDto (Request)
```json
{
  "chatType": 0,
  "subject": "string?",
  "initialMessage": "string"
}
```

### SendMessageDto (Request)
```json
{
  "content": "string",
  "attachmentUrl": "string?"
}
```

### ConversationDto (Response)
```json
{
  "id": "GUID",
  "chatType": 0,
  "subject": "string?",
  "isClosed": false,
  "lastMessage": "string?",
  "lastMessageAt": "datetime?",
  "unreadCount": 0,
  "createdAt": "datetime"
}
```

### ChatMessageDto (Response)
```json
{
  "id": "GUID",
  "senderId": "GUID",
  "senderName": "string",
  "senderType": "string",
  "content": "string",
  "attachmentUrl": "string?",
  "status": 0,
  "createdAt": "datetime"
}
```

### BroadcastNotificationDto (Admin Request)
```json
{
  "title": "string",
  "message": "string",
  "priority": 0,
  "targetRegion": "string?"
}
```

### SendToDriverDto (Admin Request)
```json
{
  "driverId": "GUID",
  "title": "string",
  "message": "string",
  "priority": 0
}
```

### NotificationLogDto (Response)
```json
{
  "id": 0,
  "channel": "string",
  "subject": "string?",
  "isSuccess": false,
  "errorMessage": "string?",
  "sentAt": "datetime"
}
```

### PaginationDto (Query)
```json
{
  "page": 1,
  "pageSize": 20
}
```

---

## 12. Error Messages Reference

### Notification Errors
| Message | When |
|---------|------|
| `الإشعار غير موجود` | Notification ID not found |

### SOS Errors
| Message | When |
|---------|------|
| `بلاغ الطوارئ غير موجود` | SOS alert ID not found |
| `بلاغ الطوارئ تم حله بالفعل` | Trying to dismiss/resolve an already-resolved SOS |

### Chat Errors
| Message | When |
|---------|------|
| `المحادثة غير موجودة` | Conversation ID not found |
| `المحادثة مغلقة` | Trying to send message in a closed conversation |

### Template Errors
| Message | When |
|---------|------|
| `قالب الرسالة غير موجود` | Template ID not found |
| `لا يمكن حذف قالب النظام` | Trying to delete a system template |

### General Errors
| Message | When |
|---------|------|
| `غير مصرح` | Invalid or missing token |
| `هذه الميزة قيد التطوير: {feature}` | Admin endpoints under development |

---

## 13. Flutter Integration

### 13.1 Dio HTTP Examples

#### Notifications Service
```dart
class NotificationService {
  final Dio _dio;

  NotificationService(this._dio);

  /// Get paged notifications
  Future<PagedResult<NotificationDto>> getNotifications({
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get('/notifications', queryParameters: {
      'page': page,
      'pageSize': pageSize,
    });
    final data = response.data['data'];
    return PagedResult(
      items: (data['items'] as List)
          .map((e) => NotificationDto.fromJson(e))
          .toList(),
      totalCount: data['totalCount'],
      page: data['page'],
      pageSize: data['pageSize'],
      totalPages: data['totalPages'],
    );
  }

  /// Mark a notification as read
  Future<bool> markAsRead(String notificationId) async {
    final response = await _dio.put('/notifications/$notificationId/read');
    return response.data['isSuccess'] == true;
  }

  /// Mark all notifications as read
  Future<bool> markAllAsRead() async {
    final response = await _dio.put('/notifications/read-all');
    return response.data['isSuccess'] == true;
  }

  /// Get unread count
  Future<int> getUnreadCount() async {
    final response = await _dio.get('/notifications/unread-count');
    return response.data['data'] as int;
  }
}
```

#### SOS Service
```dart
class SOSService {
  final Dio _dio;

  SOSService(this._dio);

  /// Activate SOS emergency
  Future<SOSLogDto?> activateSOS({
    required double latitude,
    required double longitude,
    String? notes,
  }) async {
    try {
      final response = await _dio.post('/sos/activate', data: {
        'latitude': latitude,
        'longitude': longitude,
        if (notes != null) 'notes': notes,
      });
      if (response.data['isSuccess'] == true) {
        return SOSLogDto.fromJson(response.data['data']);
      }
      return null;
    } on DioException catch (e) {
      showError(e.response?.data['message'] ?? 'فشل تفعيل الطوارئ');
      return null;
    }
  }

  /// Dismiss SOS (accidental)
  Future<bool> dismissSOS(String sosId) async {
    final response = await _dio.post('/sos/$sosId/dismiss');
    return response.data['isSuccess'] == true;
  }

  /// Resolve SOS
  Future<bool> resolveSOS(
    String sosId, {
    String? resolution,
    required bool wasFalseAlarm,
  }) async {
    final response = await _dio.post('/sos/$sosId/resolve', data: {
      if (resolution != null) 'resolution': resolution,
      'wasFalseAlarm': wasFalseAlarm,
    });
    return response.data['isSuccess'] == true;
  }

  /// Get SOS history
  Future<PagedResult<SOSLogDto>> getHistory({
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get('/sos/history', queryParameters: {
      'page': page,
      'pageSize': pageSize,
    });
    final data = response.data['data'];
    return PagedResult(
      items: (data['items'] as List)
          .map((e) => SOSLogDto.fromJson(e))
          .toList(),
      totalCount: data['totalCount'],
      page: data['page'],
      pageSize: data['pageSize'],
      totalPages: data['totalPages'],
    );
  }
}
```

#### Chat Service
```dart
class ChatService {
  final Dio _dio;

  ChatService(this._dio);

  /// Get conversations
  Future<PagedResult<ConversationDto>> getConversations({
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get('/chat/conversations', queryParameters: {
      'page': page,
      'pageSize': pageSize,
    });
    final data = response.data['data'];
    return PagedResult(
      items: (data['items'] as List)
          .map((e) => ConversationDto.fromJson(e))
          .toList(),
      totalCount: data['totalCount'],
      page: data['page'],
      pageSize: data['pageSize'],
      totalPages: data['totalPages'],
    );
  }

  /// Create a new conversation
  Future<ConversationDto?> createConversation({
    required int chatType,
    String? subject,
    required String initialMessage,
  }) async {
    final response = await _dio.post('/chat/conversations', data: {
      'chatType': chatType,
      if (subject != null) 'subject': subject,
      'initialMessage': initialMessage,
    });
    if (response.data['isSuccess'] == true) {
      return ConversationDto.fromJson(response.data['data']);
    }
    return null;
  }

  /// Get messages in a conversation
  Future<PagedResult<ChatMessageDto>> getMessages(
    String conversationId, {
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get(
      '/chat/conversations/$conversationId/messages',
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    final data = response.data['data'];
    return PagedResult(
      items: (data['items'] as List)
          .map((e) => ChatMessageDto.fromJson(e))
          .toList(),
      totalCount: data['totalCount'],
      page: data['page'],
      pageSize: data['pageSize'],
      totalPages: data['totalPages'],
    );
  }

  /// Send a message
  Future<ChatMessageDto?> sendMessage(
    String conversationId, {
    required String content,
    String? attachmentUrl,
  }) async {
    final response = await _dio.post(
      '/chat/conversations/$conversationId/messages',
      data: {
        'content': content,
        if (attachmentUrl != null) 'attachmentUrl': attachmentUrl,
      },
    );
    if (response.data['isSuccess'] == true) {
      return ChatMessageDto.fromJson(response.data['data']);
    }
    return null;
  }

  /// Close a conversation
  Future<bool> closeConversation(String conversationId) async {
    final response = await _dio.put(
      '/chat/conversations/$conversationId/close',
    );
    return response.data['isSuccess'] == true;
  }

  /// Mark message as read
  Future<bool> markMessageRead(String messageId) async {
    final response = await _dio.put('/chat/messages/$messageId/read');
    return response.data['isSuccess'] == true;
  }

  /// Get unread message count
  Future<int> getUnreadCount() async {
    final response = await _dio.get('/chat/unread-count');
    return response.data['data'] as int;
  }
}
```

#### Message Template Service
```dart
class MessageTemplateService {
  final Dio _dio;

  MessageTemplateService(this._dio);

  /// Get all templates (system + custom)
  Future<List<MessageTemplateDto>> getTemplates() async {
    final response = await _dio.get('/message-templates');
    return (response.data['data'] as List)
        .map((e) => MessageTemplateDto.fromJson(e))
        .toList();
  }

  /// Create a custom template
  Future<MessageTemplateDto?> create({
    required String messageText,
    required int category,
  }) async {
    final response = await _dio.post('/message-templates', data: {
      'messageText': messageText,
      'category': category,
    });
    if (response.data['isSuccess'] == true) {
      return MessageTemplateDto.fromJson(response.data['data']);
    }
    return null;
  }

  /// Update a custom template
  Future<MessageTemplateDto?> update(
    String templateId, {
    String? messageText,
    int? category,
    int? sortOrder,
  }) async {
    final response = await _dio.put('/message-templates/$templateId', data: {
      if (messageText != null) 'messageText': messageText,
      if (category != null) 'category': category,
      if (sortOrder != null) 'sortOrder': sortOrder,
    });
    if (response.data['isSuccess'] == true) {
      return MessageTemplateDto.fromJson(response.data['data']);
    }
    return null;
  }

  /// Delete a custom template
  Future<bool> delete(String templateId) async {
    final response = await _dio.delete('/message-templates/$templateId');
    return response.data['isSuccess'] == true;
  }

  /// Record template usage
  Future<bool> recordUsage(String templateId) async {
    final response = await _dio.post('/message-templates/$templateId/use');
    return response.data['isSuccess'] == true;
  }
}
```

---

### 13.2 SignalR Integration

#### Dependencies (pubspec.yaml)
```yaml
dependencies:
  signalr_netcore: ^1.3.7
  # or
  signalr_core: ^1.1.2
```

#### Chat Hub Connection
```dart
import 'package:signalr_netcore/signalr_client.dart';

class ChatHubService {
  late HubConnection _hubConnection;
  final String _token;

  ChatHubService(this._token);

  /// Connect to the Chat hub
  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(
          'https://sekka.runasp.net/hubs/chat',
          options: HttpConnectionOptions(
            accessTokenFactory: () async => _token,
            transport: HttpTransportType.webSockets,
          ),
        )
        .withAutomaticReconnect()
        .build();

    // Listen for incoming messages
    _hubConnection.on('ReceiveMessage', (arguments) {
      final message = arguments?[0] as Map<String, dynamic>;
      final senderId = message['senderId'];
      final content = message['content'];
      final sentAt = message['sentAt'];
      // Update UI with new message
      onMessageReceived(senderId, content, sentAt);
    });

    // Listen for typing indicators
    _hubConnection.on('UserTyping', (arguments) {
      final userId = arguments?[0] as String;
      onUserTyping(userId);
    });

    _hubConnection.on('UserStoppedTyping', (arguments) {
      final userId = arguments?[0] as String;
      onUserStoppedTyping(userId);
    });

    // Listen for user join/leave
    _hubConnection.on('UserJoined', (arguments) {
      final userId = arguments?[0] as String;
      // Show "user joined" indicator
    });

    _hubConnection.on('UserLeft', (arguments) {
      final userId = arguments?[0] as String;
      // Show "user left" indicator
    });

    // Listen for conversation closed
    _hubConnection.on('ConversationClosed', (arguments) {
      final conversationId = arguments?[0] as String;
      onConversationClosed(conversationId);
    });

    await _hubConnection.start();
  }

  /// Join a conversation to receive messages
  Future<void> joinConversation(String conversationId) async {
    await _hubConnection.invoke('JoinConversation', args: [conversationId]);
  }

  /// Leave a conversation
  Future<void> leaveConversation(String conversationId) async {
    await _hubConnection.invoke('LeaveConversation', args: [conversationId]);
  }

  /// Send a message via SignalR (real-time)
  Future<void> sendMessage(String conversationId, String content) async {
    await _hubConnection.invoke('SendMessage', args: [conversationId, content]);
  }

  /// Notify typing started
  Future<void> startTyping(String conversationId) async {
    await _hubConnection.invoke('StartTyping', args: [conversationId]);
  }

  /// Notify typing stopped
  Future<void> stopTyping(String conversationId) async {
    await _hubConnection.invoke('StopTyping', args: [conversationId]);
  }

  /// Disconnect
  Future<void> disconnect() async {
    await _hubConnection.stop();
  }

  // Callbacks — override or use streams
  void onMessageReceived(String senderId, String content, String sentAt) {}
  void onUserTyping(String userId) {}
  void onUserStoppedTyping(String userId) {}
  void onConversationClosed(String conversationId) {}
}
```

#### Notification Hub Connection
```dart
class NotificationHubService {
  late HubConnection _hubConnection;
  final String _token;

  NotificationHubService(this._token);

  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(
          'https://sekka.runasp.net/hubs/notifications',
          options: HttpConnectionOptions(
            accessTokenFactory: () async => _token,
            transport: HttpTransportType.webSockets,
          ),
        )
        .withAutomaticReconnect()
        .build();

    // Auto-joins driver group on connection (server-side)

    // Listen for new notifications
    _hubConnection.on('NewNotification', (arguments) {
      final notification = arguments?[0] as Map<String, dynamic>;
      // Show local notification / update badge count
      onNewNotification(notification);
    });

    // Listen for read confirmations
    _hubConnection.on('NotificationRead', (arguments) {
      final notificationId = arguments?[0] as String;
      onNotificationRead(notificationId);
    });

    _hubConnection.on('AllNotificationsRead', (arguments) {
      onAllNotificationsRead();
    });

    // Listen for broadcasts
    _hubConnection.on('BroadcastMessage', (arguments) {
      final message = arguments?[0];
      onBroadcast(message);
    });

    // Listen for financial events
    _hubConnection.on('SettlementApproved', (arguments) {
      final settlement = arguments?[0];
      onSettlementApproved(settlement);
    });

    _hubConnection.on('PaymentApproved', (arguments) {
      final payment = arguments?[0];
      onPaymentApproved(payment);
    });

    await _hubConnection.start();
  }

  /// Mark notification as read via hub
  Future<void> markAsRead(String notificationId) async {
    await _hubConnection.invoke('MarkAsRead', args: [notificationId]);
  }

  /// Mark all as read via hub
  Future<void> markAllAsRead() async {
    await _hubConnection.invoke('MarkAllAsRead');
  }

  Future<void> disconnect() async {
    await _hubConnection.stop();
  }

  // Callbacks
  void onNewNotification(Map<String, dynamic> notification) {}
  void onNotificationRead(String notificationId) {}
  void onAllNotificationsRead() {}
  void onBroadcast(dynamic message) {}
  void onSettlementApproved(dynamic settlement) {}
  void onPaymentApproved(dynamic payment) {}
}
```

#### Cash Alert Hub Connection
```dart
class CashAlertHubService {
  late HubConnection _hubConnection;
  final String _token;

  CashAlertHubService(this._token);

  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(
          'https://sekka.runasp.net/hubs/cash-alerts',
          options: HttpConnectionOptions(
            accessTokenFactory: () async => _token,
            transport: HttpTransportType.webSockets,
          ),
        )
        .withAutomaticReconnect()
        .build();

    // Auto-joins driver group on connection (server-side)

    _hubConnection.on('CashThresholdExceeded', (arguments) {
      final cashStatus = arguments?[0];
      onCashThresholdExceeded(cashStatus);
    });

    _hubConnection.on('SettlementReminder', (arguments) {
      final reminder = arguments?[0];
      onSettlementReminder(reminder);
    });

    _hubConnection.on('DailySettlementSummary', (arguments) {
      final summary = arguments?[0];
      onDailySettlementSummary(summary);
    });

    _hubConnection.on('DepositConfirmed', (arguments) {
      final deposit = arguments?[0];
      onDepositConfirmed(deposit);
    });

    _hubConnection.on('AlertAcknowledged', (arguments) {
      final alertId = arguments?[0] as String;
      onAlertAcknowledged(alertId);
    });

    await _hubConnection.start();
  }

  /// Acknowledge a cash alert
  Future<void> acknowledgeAlert(String alertId) async {
    await _hubConnection.invoke('AcknowledgeAlert', args: [alertId]);
  }

  Future<void> disconnect() async {
    await _hubConnection.stop();
  }

  // Callbacks
  void onCashThresholdExceeded(dynamic cashStatus) {}
  void onSettlementReminder(dynamic reminder) {}
  void onDailySettlementSummary(dynamic summary) {}
  void onDepositConfirmed(dynamic deposit) {}
  void onAlertAcknowledged(String alertId) {}
}
```

#### Complete Hub Manager (Connect All Hubs)
```dart
/// Manages all SignalR hub connections for the app
class HubManager {
  late ChatHubService chatHub;
  late NotificationHubService notificationHub;
  late CashAlertHubService cashAlertHub;

  /// Initialize and connect all hubs after login
  Future<void> connectAll(String token) async {
    chatHub = ChatHubService(token);
    notificationHub = NotificationHubService(token);
    cashAlertHub = CashAlertHubService(token);

    await Future.wait([
      chatHub.connect(),
      notificationHub.connect(),
      cashAlertHub.connect(),
    ]);
  }

  /// Disconnect all hubs on logout
  Future<void> disconnectAll() async {
    await Future.wait([
      chatHub.disconnect(),
      notificationHub.disconnect(),
      cashAlertHub.disconnect(),
    ]);
  }
}

// Usage in app:
// After login:
//   final hubManager = HubManager();
//   await hubManager.connectAll(authToken);
//
// On logout:
//   await hubManager.disconnectAll();
```
