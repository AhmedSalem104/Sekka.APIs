# Sekka API - Auth & Identity Documentation

> **Base URL**: `https://sekka.runasp.net/api/v1/auth`
>
> **Last Updated**: 2026-03-07

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Format](#2-response-format)
3. [Auth Flows](#3-auth-flows)
4. [Endpoints](#4-endpoints)
   - [Get Vehicle Types](#41-get-vehicle-types)
   - [Send Verification (OTP)](#42-send-verification-otp)
   - [Register](#43-register)
   - [Login](#44-login)
   - [Forgot Password](#45-forgot-password)
   - [Reset Password](#46-reset-password)
   - [Change Password](#47-change-password)
   - [Refresh Token](#48-refresh-token)
   - [Logout](#49-logout)
   - [Register Device](#410-register-device)
   - [Get Active Sessions](#411-get-active-sessions)
   - [Terminate Session](#412-terminate-session)
   - [Logout All Devices](#413-logout-all-devices)
   - [Request Account Deletion](#414-request-account-deletion)
   - [Confirm Account Deletion](#415-confirm-account-deletion)
5. [Enums](#5-enums)
6. [Validation Rules](#6-validation-rules)
7. [Error Messages Reference](#7-error-messages-reference)
8. [JWT Token Usage](#8-jwt-token-usage)
9. [Phone Number Format](#9-phone-number-format)
10. [Rate Limiting](#10-rate-limiting)

---

## 1. Overview

Sekka uses **phone number + password** authentication with **OTP verification** for registration and password reset.

| Feature | Detail |
|---------|--------|
| Auth Method | Phone + Password |
| Token Type | JWT (Bearer) |
| Token Expiry | 24 hours (1440 min) |
| Refresh Token Expiry | 30 days |
| OTP Length | 4 digits |
| OTP Expiry | 5 minutes |
| Password Min Length | 6 characters |
| Supported Phone Format | Egyptian mobile (010/011/012/015) |

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
| 201 | Created (registration) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid credentials/token) |
| 404 | Not Found |
| 409 | Conflict (phone already registered) |
| 429 | Too Many Requests (rate limit) |
| 500 | Server Error |

---

## 3. Auth Flows

### 3.1 Registration Flow

```
Step 1: GET  /auth/vehicle-types          (optional - get vehicle type options)
Step 2: POST /auth/send-verification      (send OTP to phone)
Step 3: (User receives SMS with 4-digit OTP code)
Step 4: POST /auth/register              (verify OTP + create account)
Step 5: (Store token & refreshToken from response)
```

### 3.2 Login Flow

```
Step 1: POST /auth/login                 (phone + password)
Step 2: (Store token & refreshToken from response)
```

### 3.3 Forgot Password Flow

```
Step 1: POST /auth/forgot-password       (send OTP to phone)
Step 2: (User receives SMS with 4-digit OTP code)
Step 3: POST /auth/reset-password        (verify OTP + set new password)
```

### 3.4 Token Refresh Flow

```
When token expires (401 response):
Step 1: POST /auth/refresh-token         (send expired token + refreshToken)
Step 2: (Store new token & refreshToken from response)
Step 3: (Retry the original request with new token)
```

### 3.5 Flow Diagram

```
                    ┌─────────────┐
                    │   App Start  │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │ Has Token?  │
                    └──┬───────┬──┘
                       │       │
                      Yes      No
                       │       │
              ┌────────▼──┐  ┌─▼──────────────┐
              │  Validate  │  │  Login Screen   │
              │   Token    │  │                 │
              └──┬─────┬──┘  │  ┌───────────┐  │
                 │     │     │  │   Login    │  │
              Valid  Expired │  └─────┬─────┘  │
                 │     │     │        │        │
        ┌────────▼┐ ┌──▼───┐ │  ┌─────▼─────┐  │
        │  Home   │ │Refresh│ │  │  Register  │  │
        │  Screen │ │ Token │ │  │   Flow     │  │
        └─────────┘ └──┬───┘ │  └────────────┘  │
                       │     └──────────────────┘
                    Success?
                    │     │
                   Yes    No
                    │     │
              ┌─────▼┐  ┌─▼──────────┐
              │ Home  │  │Login Screen│
              │Screen │  │(re-login)  │
              └───────┘  └────────────┘
```

---

## 4. Endpoints

### 4.1 Get Vehicle Types

Returns the list of available vehicle types for the registration form.

```
GET /auth/vehicle-types
```

**Auth Required**: No

**Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    { "id": 0, "name": "Motorcycle" },
    { "id": 1, "name": "Car" },
    { "id": 2, "name": "Van" },
    { "id": 3, "name": "Truck" },
    { "id": 4, "name": "Bicycle" }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.2 Send Verification (OTP)

Sends a 4-digit OTP code via SMS to the phone number. Used before registration.

```
POST /auth/send-verification
```

**Auth Required**: No
**Rate Limited**: 5 requests/hour per IP

**Request Body**:
```json
{
  "phoneNumber": "01012345678"
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إرسال كود التحقق بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `رقم موبايل مصري غير صالح` |
| 409 | `رقم الموبايل مسجل بالفعل` |
| 400 | `تم تجاوز الحد الأقصى لإعادة الإرسال. حاول بعد ساعة` |
| 429 | Rate limit exceeded |

---

### 4.3 Register

Creates a new driver account. Requires a valid OTP code from step 4.2.

```
POST /auth/register
```

**Auth Required**: No

**Request Body**:
```json
{
  "phoneNumber": "01012345678",
  "otpCode": "1234",
  "password": "mypassword123",
  "confirmPassword": "mypassword123",
  "name": "أحمد محمد",
  "vehicleType": 0,
  "email": "ahmed@example.com"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| phoneNumber | string | Yes | Valid Egyptian mobile (010/011/012/015) |
| otpCode | string | Yes | Exactly 4 digits |
| password | string | Yes | Minimum 6 characters |
| confirmPassword | string | Yes | Must match password |
| name | string | Yes | Max 100 characters |
| vehicleType | int | Yes | 0-4 (see [Enums](#5-enums)) |
| email | string | No | Valid email format |

**Success Response** `201 Created`:
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "a1b2c3d4e5f6...",
    "expiresAt": "2026-03-08T15:30:00Z",
    "isNewUser": true,
    "driver": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "أحمد محمد",
      "phone": "+201012345678",
      "email": "ahmed@example.com",
      "profileImageUrl": null,
      "licenseImageUrl": null,
      "vehicleType": 0,
      "isOnline": false,
      "cashOnHand": 0,
      "totalPoints": 0,
      "level": 0,
      "joinedAt": "2026-03-07T15:30:00Z",
      "referralCode": "3FA85F64"
    }
  },
  "message": "تم إنشاء الحساب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `كلمة المرور غير متطابقة` |
| 400 | `رقم موبايل مصري غير صالح` |
| 400 | `كود التحقق غير صحيح` |
| 400 | `كود التحقق منتهي الصلاحية أو غير موجود` |
| 409 | `رقم الموبايل مسجل بالفعل` |

---

### 4.4 Login

Authenticates a driver with phone number and password.

```
POST /auth/login
```

**Auth Required**: No

**Request Body**:
```json
{
  "phoneNumber": "01012345678",
  "password": "mypassword123"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| phoneNumber | string | Yes | Valid Egyptian mobile |
| password | string | Yes | Not empty |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "a1b2c3d4e5f6...",
    "expiresAt": "2026-03-08T15:30:00Z",
    "isNewUser": false,
    "driver": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "أحمد محمد",
      "phone": "+201012345678",
      "email": "ahmed@example.com",
      "profileImageUrl": null,
      "licenseImageUrl": null,
      "vehicleType": 0,
      "isOnline": false,
      "cashOnHand": 0,
      "totalPoints": 0,
      "level": 0,
      "joinedAt": "2026-03-07T15:30:00Z",
      "referralCode": "3FA85F64"
    }
  },
  "message": "تم تسجيل الدخول بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 401 | `رقم الموبايل أو كلمة المرور غير صحيحة` |
| 401 | `الحساب موقوف` |

---

### 4.5 Forgot Password

Sends an OTP code to the phone number for password reset.

```
POST /auth/forgot-password
```

**Auth Required**: No
**Rate Limited**: 5 requests/hour per IP

**Request Body**:
```json
{
  "phoneNumber": "01012345678"
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إرسال كود التحقق بنجاح",
  "errors": null
}
```

> **Note**: Always returns success even if the phone is not registered (security — prevents phone enumeration).

---

### 4.6 Reset Password

Resets the password using OTP code from forgot-password.

```
POST /auth/reset-password
```

**Auth Required**: No
**Rate Limited**: 5 requests/hour per IP

**Request Body**:
```json
{
  "phoneNumber": "01012345678",
  "otpCode": "1234",
  "newPassword": "newpassword123",
  "confirmPassword": "newpassword123"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| phoneNumber | string | Yes | Valid Egyptian mobile |
| otpCode | string | Yes | Exactly 4 digits |
| newPassword | string | Yes | Minimum 6 characters |
| confirmPassword | string | Yes | Must match newPassword |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إعادة تعيين كلمة المرور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `كلمة المرور غير متطابقة` |
| 400 | `كود التحقق غير صحيح` |
| 400 | `كود التحقق منتهي الصلاحية أو غير موجود` |
| 404 | `الحساب غير موجود` |

---

### 4.7 Change Password

Changes the password for an authenticated driver.

```
POST /auth/change-password
```

**Auth Required**: Yes (Bearer Token)

**Headers**:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Request Body**:
```json
{
  "currentPassword": "oldpassword123",
  "newPassword": "newpassword123",
  "confirmPassword": "newpassword123"
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| currentPassword | string | Yes | Not empty |
| newPassword | string | Yes | Minimum 6 characters |
| confirmPassword | string | Yes | Must match newPassword |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تغيير كلمة المرور بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 400 | `كلمة المرور غير متطابقة` |
| 400 | `فشل تغيير كلمة المرور: ...` |
| 404 | `السائق غير موجود` |

---

### 4.8 Refresh Token

Refreshes an expired JWT token using the refresh token.

```
POST /auth/refresh-token
```

**Auth Required**: No

**Request Body**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "a1b2c3d4e5f6..."
}
```

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| token | string | Yes | The expired JWT token |
| refreshToken | string | Yes | The refresh token from login/register |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...(NEW)",
    "refreshToken": "x1y2z3...(NEW)",
    "expiresAt": "2026-03-09T15:30:00Z",
    "isNewUser": false,
    "driver": { ... }
  },
  "message": "تم تجديد التوكن بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 401 | `توكن غير صالح` |
| 404 | `السائق غير موجود` |

> **Important**: After refreshing, you MUST store and use the new `token` AND `refreshToken`. The old ones are no longer valid.

---

### 4.9 Logout

Logs out the current device.

```
POST /auth/logout
```

**Auth Required**: Yes (Bearer Token)

**Headers**:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الخروج بنجاح",
  "errors": null
}
```

---

### 4.10 Register Device

Registers a device for push notifications (FCM).

```
POST /auth/register-device
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "token": "fcm_device_token_here",
  "platform": 0
}
```

| Field | Type | Required | Values |
|-------|------|----------|--------|
| token | string | Yes | FCM device token |
| platform | int | Yes | `0` = Android, `1` = iOS |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الجهاز بنجاح",
  "errors": null
}
```

---

### 4.11 Get Active Sessions

Returns all active login sessions for the driver.

```
GET /auth/sessions
```

**Auth Required**: Yes (Bearer Token)

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "deviceName": "Samsung Galaxy S24",
      "devicePlatform": 0,
      "ipAddress": "192.168.1.1",
      "lastActiveAt": "2026-03-07T15:30:00Z",
      "isCurrentSession": true,
      "createdAt": "2026-03-01T10:00:00Z"
    }
  ],
  "message": null,
  "errors": null
}
```

---

### 4.12 Terminate Session

Terminates a specific login session by ID.

```
DELETE /auth/sessions/{id}
```

**Auth Required**: Yes (Bearer Token)

**URL Params**: `id` (GUID) - Session ID

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم إنهاء الجلسة بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `الجلسة غير موجودة` |

---

### 4.13 Logout All Devices

Terminates all active sessions (logout from everywhere).

```
POST /auth/logout-all
```

**Auth Required**: Yes (Bearer Token)

**Request Body**: None

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تسجيل الخروج من جميع الأجهزة بنجاح",
  "errors": null
}
```

---

### 4.14 Request Account Deletion

Requests account deletion. Sends a confirmation code.

```
DELETE /auth/account
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "reason": "لم أعد أستخدم التطبيق"
}
```

| Field | Type | Required |
|-------|------|----------|
| reason | string | No |

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تقديم طلب حذف الحساب بنجاح",
  "errors": null
}
```

---

### 4.15 Confirm Account Deletion

Confirms account deletion using the confirmation code.

```
POST /auth/account/confirm-deletion
```

**Auth Required**: Yes (Bearer Token)

**Request Body**:
```json
{
  "confirmationCode": "123456"
}
```

**Success Response** `200 OK`:
```json
{
  "isSuccess": true,
  "data": true,
  "message": "تم تأكيد حذف الحساب بنجاح",
  "errors": null
}
```

**Error Responses**:
| Code | Message |
|------|---------|
| 404 | `لا يوجد طلب حذف معلّق` |
| 400 | `كود التأكيد غير صحيح` |

---

## 5. Enums

### VehicleType
| Value | Name | Arabic |
|-------|------|--------|
| 0 | Motorcycle | موتوسيكل |
| 1 | Car | سيارة |
| 2 | Van | فان |
| 3 | Truck | تراك |
| 4 | Bicycle | دراجة |

### DevicePlatform
| Value | Name |
|-------|------|
| 0 | Android |
| 1 | iOS |

---

## 6. Validation Rules

### Phone Number
- Must be a valid Egyptian mobile number
- Accepted formats: `01012345678`, `+201012345678`, `0020101234567`
- Must start with: `010`, `011`, `012`, or `015`
- Must be 11 digits (local format)
- The API normalizes all formats to `+201XXXXXXXXX` internally

### Password
- Minimum 6 characters
- `confirmPassword` must match `password` / `newPassword`

### OTP Code
- Exactly 4 digits
- Expires after 5 minutes
- Maximum 5 verification attempts per hour
- Maximum 3 resend requests per hour

### Name
- Required (not empty)
- Maximum 100 characters

### Email
- Optional
- Must be valid email format if provided

---

## 7. Error Messages Reference

### Auth Errors
| Message | When |
|---------|------|
| `رقم موبايل مصري غير صالح` | Invalid phone format |
| `رقم الموبايل مسجل بالفعل` | Phone already registered |
| `كلمة المرور غير متطابقة` | password != confirmPassword |
| `رقم الموبايل أو كلمة المرور غير صحيحة` | Wrong credentials |
| `الحساب موقوف` | Account suspended |
| `الحساب غير موجود` | Account not found |
| `توكن غير صالح` | Invalid/corrupted JWT |
| `السائق غير موجود` | Driver ID not found |

### OTP Errors
| Message | When |
|---------|------|
| `تم تجاوز الحد الأقصى لإعادة الإرسال. حاول بعد ساعة` | 3+ OTP resends |
| `تم تجاوز عدد المحاولات. حاول بعد ساعة` | 5+ wrong OTP attempts |
| `كود التحقق منتهي الصلاحية أو غير موجود` | OTP expired (>5 min) |
| `كود التحقق غير صحيح` | Wrong OTP code |
| `فشل إرسال الرسالة. حاول مرة أخرى` | SMS provider failed |

### Validation Errors
| Message | When |
|---------|------|
| `{field} مطلوب` | Required field is empty |
| `{field} يجب ألا يتجاوز {n} حرف` | Exceeds max length |
| `{field} يجب أن تكون {n} أحرف على الأقل` | Below min length |
| `كود التحقق يجب أن يكون 4 أرقام` | OTP not 4 digits |

---

## 8. JWT Token Usage

### Storing the Token
After login/register, store these values securely:
```
token        → JWT access token (use in API calls)
refreshToken → Refresh token (use to get new token)
expiresAt    → Token expiry time (check before API calls)
```

### Using the Token
Add the token to every authenticated request header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Token Refresh Strategy
```
Before each API call:
  1. Check if current time > expiresAt (or close to it)
  2. If expired → call POST /auth/refresh-token
  3. Store new token + refreshToken
  4. Proceed with the API call using new token

On 401 response:
  1. Try POST /auth/refresh-token
  2. If success → retry original request with new token
  3. If fail → redirect to login screen
```

### Token Claims (JWT Payload)
```json
{
  "nameid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",  // Driver ID
  "unique_name": "أحمد محمد",                          // Driver Name
  "phone": "+201012345678",                             // Phone
  "jti": "unique-token-id",                             // Token ID
  "exp": 1741456200,                                    // Expiry (Unix)
  "iss": "Sekka.API",                                   // Issuer
  "aud": "Sekka.Mobile"                                 // Audience
}
```

---

## 9. Phone Number Format

The API accepts multiple Egyptian phone formats and normalizes them automatically:

| Input | Stored As |
|-------|-----------|
| `01012345678` | `+201012345678` |
| `+201012345678` | `+201012345678` |
| `00201012345678` | `+201012345678` |
| `201012345678` | `+201012345678` |

### Valid Prefixes
| Prefix | Carrier |
|--------|---------|
| 010 | Vodafone |
| 011 | Etisalat |
| 012 | Orange |
| 015 | WE |

> **Tip**: You can send the phone in any format. The API handles normalization. But for display, use the format returned in the response (`+201XXXXXXXXX`).

---

## 10. Rate Limiting

| Endpoint | Limit | Window |
|----------|-------|--------|
| `POST /auth/send-verification` | 5 requests | per hour |
| `POST /auth/forgot-password` | 5 requests | per hour |
| `POST /auth/reset-password` | 5 requests | per hour |
| All other endpoints | 100 requests | per minute |

When rate limited, you receive:
```
HTTP 429 Too Many Requests
```

---

## Flutter Integration Example

### Dio Setup
```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://sekka.runasp.net/api/v1',
  headers: {'Content-Type': 'application/json'},
));

// Add token interceptor
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
      // Try refresh token
      final refreshed = await refreshToken();
      if (refreshed) {
        // Retry original request
        final retryResponse = await dio.fetch(error.requestOptions);
        handler.resolve(retryResponse);
      } else {
        // Go to login
        navigateToLogin();
        handler.next(error);
      }
    } else {
      handler.next(error);
    }
  },
));
```

### Login Example
```dart
Future<AuthResponse?> login(String phone, String password) async {
  try {
    final response = await dio.post('/auth/login', data: {
      'phoneNumber': phone,
      'password': password,
    });

    if (response.data['isSuccess'] == true) {
      final auth = AuthResponse.fromJson(response.data['data']);
      await saveToken(auth.token);
      await saveRefreshToken(auth.refreshToken);
      return auth;
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

### Registration Example
```dart
// Step 1: Send OTP
await dio.post('/auth/send-verification', data: {
  'phoneNumber': '01012345678',
});

// Step 2: Register (after user enters OTP)
final response = await dio.post('/auth/register', data: {
  'phoneNumber': '01012345678',
  'otpCode': '1234',
  'password': 'mypassword',
  'confirmPassword': 'mypassword',
  'name': 'أحمد محمد',
  'vehicleType': 0,
  'email': 'ahmed@example.com',  // optional
});
```
