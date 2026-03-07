# Sekka.API — Master Implementation Reference
# Version: 4.0 | Date: 2026-03-07 | Status: Phase 1 Complete
# Source: clean-architecture-docs.md (~11,183 lines)

---

## 1. PROJECT IDENTITY

- **Name**: Sekka (سكّة) — Smart Delivery Management Platform
- **Backend**: .NET 8.0 Web API (Clean Architecture)
- **Mobile Client**: Flutter (Dart) — Bloc/Riverpod, Isar/Hive offline-first
- **Database**: SQL Server (Remote — databaseasp.net, EF Core Code-First)
- **Cache**: Redis (Upstash — TLS, `IDistributedCache` via `StackExchangeRedisCache`)
- **Auth**: JWT + ASP.NET Identity (Phone + Password login, OTP for verification/reset)
- **SMS Provider**: smseg (Smart Egypt SMS — HTTP API)
- **Market**: Egypt-only (Egyptian mobile numbers: 010/011/012/015)

### Quick Numbers (Target)
- 6 layers | 69 Controllers (43 driver + 25 admin + 1 extension)
- 579+ endpoints | 83+ services | 82 tables
- 452+ DTOs | 82 enums | 4 SignalR hubs | 11 background services
- 5 middleware | 2 health checks | 59+ indexes | 27 constraints

### Current Implementation Status
- **Phase 1 (Auth & Identity)**: COMPLETE
- Implemented: 12 controllers, 13 services, 7 entities, 15 DB tables (migrated)
- 11 enums, 9 validators, 80+ DTOs, 4 middleware
- Centralized message system (ErrorMessages + SuccessMessages)
- Role seeding: Driver, Admin, Support

---

## 2. ARCHITECTURE STRUCTURE

### Layer Dependency Graph
```
Core → Persistence → Infrastructure → Application → API
                                                    ↗
                               AdminControlDashboard
```

### Layer Details (Current Implementation)

**Sekka.Core** (no dependencies)
```
Common/
  ├── Result.cs, ApiResponse.cs, PagedResult.cs, EgyptianPhoneHelper.cs
  └── Messages/
      ├── ErrorMessages.cs      ← All error strings (centralized)
      └── SuccessMessages.cs    ← All success strings (centralized)
DTOs/
  ├── Auth/         ← LoginDto, RegisterDto, ForgotPasswordDto, ResetPasswordDto,
  │                   ChangePasswordDto, SendOtpDto, VerifyOtpDto, CompleteRegistrationDto,
  │                   RefreshTokenDto, RegisterDeviceDto, AuthResponseDto, SetPasswordDto
  ├── Profile/      ← DriverProfileDto, UpdateProfileDto, ProfileCompletionDto, DriverStatsDto,
  │                   BadgeDto, ActivityLogDto, SubscriptionDto, GamificationDtos,
  │                   EmergencyContactDto, ExpenseDto, VehicleInfoDto
  ├── Settings/     ← DriverPreferencesDto, SettingsRequestDtos, NotificationChannelDtos
  ├── Account/      ← AccountManagementDtos (DeleteAccount, ConfirmDeletion, ActiveSession)
  ├── Privacy/      ← PrivacyDtos (Consent, DeletionRequest, DataExport)
  ├── Admin/        ← AdminDriverDto, RoleDto, AdminSubscriptionDtos
  ├── Badge/        ← DigitalBadgeDto
  ├── Demo/         ← DemoSessionDto
  ├── HealthScore/  ← AccountHealthDto
  └── Common/       ← PaginationDto
Enums/              ← VehicleType, PhoneType, ThemeMode, MapApp, DevicePlatform,
                      DeletionRequestStatus, NotificationType, NotificationPriority,
                      ShiftStatus, SubscriptionStatus, OrderStatus, ChallengeType
Interfaces/
  ├── Services/     ← IAuthService, ISmsService, IProfileService, IDriverPreferencesService,
  │                   IAccountManagementService, IDataPrivacyService, IDemoService,
  │                   IEmailService, IBadgeService, IHealthScoreService,
  │                   IAdminDriversService, IAdminRolesService, IAdminSubscriptionsService
  └── Persistence/  ← IGenericRepository<TEntity,TKey>, IUnitOfWork
Specifications/     ← ISpecification<T>, BaseSpecification<T>
Validators/
  ├── Common/       ← EgyptianPhoneValidatorExtensions (MustBeEgyptianMobile)
  ├── Auth/         ← RegisterDtoValidator, LoginDtoValidator, ResetPasswordDtoValidator,
  │                   SendOtpDtoValidator, VerifyOtpDtoValidator,
  │                   CompleteRegistrationDtoValidator, RefreshTokenDtoValidator
  └── Emergency/    ← CreateEmergencyContactDtoValidator
Mapping/            ← MappingProfile.cs (AutoMapper)
```

**Sekka.Persistence** (→ Core)
```
Entities/
  ├── Base/         ← BaseEntity<TKey>, AuditableEntity<TKey>, SoftDeletableEntity<TKey>
  ├── Driver.cs                        ← extends IdentityUser<Guid>
  ├── DriverPreferences.cs             ← AuditableEntity<Guid>
  ├── NotificationChannelPreference.cs ← BaseEntity<Guid>
  ├── AccountDeletionRequest.cs        ← AuditableEntity<Guid>
  ├── ActiveSession.cs                 ← BaseEntity<Guid>
  ├── UserConsent.cs                   ← AuditableEntity<Guid>
  └── DataDeletionRequest.cs           ← AuditableEntity<Guid>
Configurations/     ← DriverConfiguration, DriverPreferencesConfiguration,
                      NotificationChannelPreferenceConfiguration,
                      AccountDeletionRequestConfiguration, ActiveSessionConfiguration,
                      UserConsentConfiguration, DataDeletionRequestConfiguration
Interceptors/       ← AuditInterceptor (SaveChangesInterceptor)
Migrations/         ← Phase1_AuthIdentity (15 tables: Identity + custom entities)
SekkaDbContext.cs   ← Global soft-delete query filter, Identity config
DbInitializer.cs    ← Role seeding (Driver, Admin, Support)
```

**Sekka.Infrastructure** (→ Persistence)
```
Repositories/
  ├── GenericRepository.cs
  └── SpecificationEvaluator.cs
UnitOfWork.cs
```

**Sekka.Application** (→ Core + Infrastructure)
```
Services/
  ├── Base/BaseService.cs              ← Generic CRUD base
  ├── AuthService.cs                   ← Phone+Password auth, JWT, OTP verification
  ├── SmsService.cs                    ← smseg integration, OTP send/verify via Redis
  ├── ProfileService.cs                ← Driver profile, completion, stats, badges
  ├── DriverPreferencesService.cs      ← Settings, notifications, quiet hours
  ├── AccountManagementService.cs      ← Account deletion, sessions
  ├── DataPrivacyService.cs            ← GDPR: consents, data export/deletion
  ├── DemoService.cs                   ← Demo mode
  ├── EmailService.cs                  ← Email (stub)
  ├── BadgeService.cs                  ← Digital badge, QR verification
  └── HealthScoreService.cs            ← Account health scoring
```

**Sekka.AdminControlDashboard** (→ Application + Persistence)
```
Services/
  ├── AdminDriversService.cs           ← List, activate/deactivate, performance
  ├── AdminRolesService.cs             ← CRUD roles, assign/revoke
  └── AdminSubscriptionsService.cs     ← Subscriptions, plans management
```

**Sekka.API** (→ All)
```
Controllers/
  ├── Base/BaseCrudController.cs       ← Generic CRUD + ToActionResult
  ├── Driver/
  │   ├── AuthController.cs            ← 15 endpoints (register/login/password/sessions/deletion)
  │   ├── ProfileController.cs         ← 20 endpoints (profile/stats/badges/expenses/contacts)
  │   ├── SettingsController.cs        ← 10 endpoints (preferences/notifications/location)
  │   ├── DataPrivacyController.cs     ← 5 endpoints (consents/export/delete)
  │   ├── BadgeController.cs           ← 2 endpoints (get/verify)
  │   ├── HealthScoreController.cs     ← 2 endpoints (score/tips)
  │   ├── DemoController.cs            ← 4 endpoints (start/data/end/convert)
  │   └── LookupsController.cs         ← Lookup endpoints
  └── Admin/
      ├── AdminDriversController.cs    ← 6 endpoints
      ├── AdminRolesController.cs      ← 7 endpoints
      └── AdminSubscriptionsController.cs ← 13 endpoints
Middleware/
  ├── GlobalExceptionHandler.cs        ← Catches unhandled exceptions → ErrorMessages.UnexpectedError
  ├── RequestLoggingMiddleware.cs      ← Request/Response logging
  ├── LocaleNormalizationMiddleware.cs ← Arabic/Hindi digits → ASCII
  └── MaintenanceMiddleware.cs         ← Maintenance mode toggle
Program.cs                             ← Full DI + middleware pipeline
appsettings.json                       ← Production config template
appsettings.Development.json           ← Dev config (remote DB + Upstash Redis)
```

---

## 3. CORE PATTERNS

### 3.1 Entity Hierarchy
```
BaseEntity<TKey>           → Id, CreatedAt
  ↓ AuditableEntity<TKey>  → + UpdatedAt, CreatedBy, ModifiedBy
    ↓ SoftDeletableEntity<TKey> → + IsDeleted, DeletedAt, DeletedBy
```

### 3.2 Result Pattern
```csharp
Result<T> {
    bool IsSuccess; T? Value; Error? Error;
    static Success(T), Failure(Error), NotFound(msg), BadRequest(msg), Conflict(msg), Unauthorized(msg)
}
record Error(string Code, string Message);
// Codes: "NOT_FOUND", "BAD_REQUEST", "CONFLICT", "UNAUTHORIZED"
```

### 3.3 ApiResponse Pattern
```csharp
ApiResponse<T> { bool IsSuccess, T? Data, string? Message, List<string>? Errors }
static Success(T data, string? message = null), Fail(string message), Fail(List<string> errors)
```
- All success responses include `SuccessMessages.*` message
- All error responses include `ErrorMessages.*` message

### 3.4 Centralized Messages
```csharp
// ErrorMessages — ALL error strings in one place
ErrorMessages.InvalidPhoneNumber, .PhoneAlreadyRegistered, .PasswordMismatch,
.InvalidCredentials, .AccountSuspended, .AccountNotFound, .InvalidToken,
.OtpResendLimitReached, .OtpAttemptsExceeded, .OtpExpiredOrNotFound, .OtpInvalid,
.SmsSendFailed, .DriverNotFound, .ItemNotFound, .SettingsNotFound,
.NoPendingDeletionRequest, .InvalidConfirmationCode, .SessionNotFound,
.UnexpectedError, .DefaultDriverName, .SmsOtpTemplate(code),
.Required(field), .MaxLength(field, max), .MinLength(field, min), .InvalidFormat(field),
.InvalidEgyptianPhone, .OtpMustBe4Digits,
.AccountCreationFailed(details), .SetPasswordFailed(details), .ChangePasswordFailed(details),
.FeatureUnderDevelopment(feature), .SubscriptionsUnderDev, .PlansUnderDev, .DemoConvertUnderDev,
.RoleAlreadyExists, .RoleNotFound, .UserNotFound,
.SubscriptionNotFound, .NoCurrentSubscription, .PlanNotFound, .NoDeletionRequest

// SuccessMessages — ALL success strings in one place
SuccessMessages.OtpSent, .Registered, .LoggedIn, .LoggedOut,
.PasswordChanged, .PasswordReset, .TokenRefreshed, .DeviceRegistered,
.ProfileUpdated, .ImageUploaded, .SettingsUpdated,
.DeletionRequested, .DeletionConfirmed, .SessionTerminated, .AllSessionsTerminated,
.ConsentUpdated, .DataExportRequested, .DataDeletionRequested,
.RoleCreated, .RoleUpdated, .RoleDeleted, .RoleAssigned, .RoleRemoved,
.DriverActivated, .DriverDeactivated,
.StepName, .StepVehicleType, .StepLicenseImage, .StepProfilePhoto, .StepEmail, .StepDefaultRegion
```

### 3.5 Specification Pattern
```csharp
ISpecification<T> {
    Criteria, Includes, IncludeStrings, OrderBy, OrderByDescending,
    Take, Skip, AsNoTracking
}
BaseSpecification<T> : ISpecification<T> {
    SetCriteria(), AddInclude(), SetOrderBy(), SetOrderByDescending(), ApplyPaging()
    AsNoTracking = true (default)
}
```

### 3.6 Generic Repository + Unit of Work
```csharp
IGenericRepository<TEntity, TKey> {
    GetByIdAsync, ListAsync(spec), CountAsync(spec),
    AddAsync, Update, Delete
}
IUnitOfWork {
    GetRepository<TEntity, TKey>(), SaveChangesAsync(), BeginTransactionAsync()
}
// BeginTransactionAsync returns IAsyncDisposable (not IDbContextTransaction) — keeps Core EF-free
```

### 3.7 Generic BaseService
```csharp
BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : AuditableEntity<Guid>
{
    GetPagedAsync(spec, pagination), GetByIdAsync(id),
    CreateAsync(dto), UpdateAsync(id, dto), SoftDeleteAsync(id)
}
```

### 3.8 EgyptianPhoneHelper (static)
```
Normalize(phone) → +201XXXXXXXXX (13 chars, canonical format)
IsValid(phone), IsMobile(phone) → bool
GetPhoneType(phone) → PhoneType enum
ToInternationalFormat(phone) → 201XXXXXXXXX (for SMS API, no +)
ToDisplayFormat(phone) → +20 101 234 5678
GetCarrierName(phone) → "Vodafone"|"Etisalat"|"Orange"|"WE"|"Unknown"
```
Networks: 010=Vodafone, 011=Etisalat, 012=Orange, 015=WE

### 3.9 FluentValidation
- Extension: `MustBeEgyptianMobile()` — uses EgyptianPhoneHelper.IsMobile() + ErrorMessages.InvalidEgyptianPhone
- All validator messages use `ErrorMessages.Required()`, `.MaxLength()`, `.MinLength()`, `.InvalidFormat()`, `.PasswordMismatch`, `.OtpMustBe4Digits`
- Registration: `AddValidatorsFromAssemblyContaining<SendOtpDtoValidator>()`

---

## 4. AUTH FLOW (Phase 1 — Implemented)

### Phone + Password Authentication
```
┌─ Registration Flow ──────────────────────────────────────────┐
│ 1. GET  /auth/vehicle-types          → List vehicle types    │
│ 2. POST /auth/send-verification      → Send OTP to phone     │
│    → Redis: Sekka_OTP:{phone} (5 min TTL)                    │
│    → SMS via smseg provider                                  │
│ 3. POST /auth/register               → Verify OTP + Create   │
│    → phone, otpCode, password, confirmPassword, name,        │
│      vehicleType, email                                      │
│    → Creates Driver + assigns "Driver" role                  │
│    → Returns JWT + RefreshToken + DriverProfile              │
└──────────────────────────────────────────────────────────────┘

┌─ Login Flow ─────────────────────────────────────────────────┐
│ POST /auth/login                                             │
│   → phone + password                                         │
│   → Validates via Identity UserManager                       │
│   → Returns JWT + RefreshToken + DriverProfile               │
└──────────────────────────────────────────────────────────────┘

┌─ Password Recovery ──────────────────────────────────────────┐
│ 1. POST /auth/forgot-password        → Send OTP              │
│ 2. POST /auth/reset-password         → Verify OTP + Set pass │
│    → phone, otpCode, newPassword, confirmPassword            │
└──────────────────────────────────────────────────────────────┘

┌─ Authenticated Operations ───────────────────────────────────┐
│ POST /auth/change-password     → currentPassword + newPass   │
│ POST /auth/refresh-token       → Rotate JWT + RefreshToken   │
│ POST /auth/logout              → Log out current device      │
│ POST /auth/register-device     → Register device for push    │
│ GET  /auth/sessions            → List active sessions        │
│ DEL  /auth/sessions/{id}       → Terminate specific session  │
│ POST /auth/logout-all          → Terminate all sessions      │
│ DEL  /auth/account             → Request account deletion    │
│ POST /auth/account/confirm-deletion → Confirm with code      │
└──────────────────────────────────────────────────────────────┘
```

### JWT Token Structure
- Claims: NameIdentifier (driver.Id), Name (driver.Name), MobilePhone, Jti
- Signing: HMAC-SHA256
- Expiry: 1440 min (24h) access, 30 days refresh
- SignalR: Token via query string `?access_token=` for `/hubs/*`

### OTP System
- 4 digits, 5 min expiry
- Max 5 verification attempts/hour per number
- Max 3 resend attempts/hour per number
- Stored in Redis (Upstash) via `IDistributedCache`
- Dev mode: `UseFakeOtpInDev: true` → code = "1234"
- Production: Real SMS via smseg HTTP API

### SMS Provider (smseg — Smart Egypt SMS)
```
POST https://smssmartegypt.com/sms/api/?username=...&password=...&sendername=...&mobiles=...&message=...
Response: [{"type": "success", ...}]
Integrated via HttpClient (AddHttpClient<ISmsService, SmsService>)
```

---

## 5. DATABASE (Current State)

### Phase 1 Migration: 15 Tables
```
Identity Tables (9):
  AspNetUsers (Driver), AspNetRoles, AspNetUserRoles,
  AspNetUserClaims, AspNetRoleClaims, AspNetUserLogins,
  AspNetUserTokens, AspNetRoleClaims

Custom Tables (6):
  DriverPreferences, NotificationChannelPreferences,
  AccountDeletionRequests, ActiveSessions,
  UserConsents, DataDeletionRequests
```

### Driver Entity (extends IdentityUser<Guid>)
```csharp
Name, VehicleType, ProfileImageUrl, LicenseImageUrl,
IsOnline, IsActive, CashOnHand, CashAlertThreshold,
TotalPoints, Level, SpeedCompleteMode,
DefaultRegionId, CreatedAt, UpdatedAt
```

### Connection
- Remote SQL Server: `db43715.public.databaseasp.net`
- Encrypt=True, TrustServerCertificate=True, MultipleActiveResultSets=True

### Seeded Data
- Roles: Driver, Admin, Support (via `DbInitializer.SeedAsync`)

---

## 6. INFRASTRUCTURE CONFIG

### NuGet Packages (Current)

**Core**: AutoMapper 13+, FluentValidation 11.9+
**Persistence**: EFCore 8.0.*, EFCore.SqlServer, Identity.EntityFrameworkCore
**Infrastructure**: (relies on Persistence reference)
**Application**: Microsoft.Extensions.Configuration.Binder, Caching.Abstractions, Identity.EntityFrameworkCore 8.0.*, System.IdentityModel.Tokens.Jwt
**API**: Swashbuckle, JwtBearer, Asp.Versioning.Mvc, Asp.Versioning.Mvc.ApiExplorer, FluentValidation.DependencyInjectionExtensions, Microsoft.Extensions.Caching.StackExchangeRedis, Microsoft.EntityFrameworkCore.Design

### Redis Keys (Active)
| Pattern | Purpose | TTL |
|---------|---------|-----|
| Sekka_OTP:{phone} | OTP code | 5 min |
| Sekka_OTP_ATTEMPTS:{phone} | Failed verification attempts | 60 min |
| Sekka_OTP_RESEND:{phone} | Resend count per number | 60 min |

### Redis Connection (Upstash)
```
witty-mammal-49129.upstash.io:6379
ssl=true, sslProtocols=tls12, connectTimeout=10000, syncTimeout=10000
```

### Rate Limiting
| Policy | Type | Limit | Window |
|--------|------|-------|--------|
| OtpLimiter | Sliding Window | 5 requests | 60 min (6 segments) |
| ApiLimiter | Fixed Window | 100 requests | 1 min |

### appsettings Structure (Current)
```json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "JwtSettings": { "SecretKey", "Issuer": "Sekka.API", "Audience": "Sekka.Mobile",
                   "AccessTokenExpiryMinutes": 1440, "RefreshTokenExpiryDays": 30 },
  "OtpSettings": { "Length": 4, "ExpiryMinutes": 5, "MaxAttemptsPerNumber": 5,
                   "MaxResendPerNumber": 3, "UseFakeOtpInDev": false, "FakeOtpCode": "1234" },
  "SmsProvider": { "BaseUrl", "Username", "Password", "SenderId" },
  "Redis": { "ConnectionString": "...", "InstanceName": "Sekka_" },
  "FileStorage": { "BasePath", "BaseUrl", "MaxFileSizeBytes": 5242880, "AllowedExtensions" },
  "AppSettings": { "MaxOrdersPerDay": 100, "CashAlertThresholdEGP": 2000 },
  "CorsPolicy": { "AllowedOrigins": ["https://admin.sekka.app", "https://sekka.app"] },
  "ApiVersioning": { "DefaultVersion": "1.0" }
}
```

---

## 7. DI REGISTRATION (Program.cs)

```
 1. DbContext + AuditInterceptor
 2. ASP.NET Identity (Driver, IdentityRole<Guid>)
 3. JWT Authentication (JwtBearer + SignalR query string support)
 4. Redis Cache (StackExchangeRedisCache — Upstash)
 5. SignalR (standalone — Redis backplane ready but commented)
 6. AutoMapper (cfg => { }, MappingProfile.Assembly)
 7. GenericRepository<,> + UnitOfWork (Scoped)
 8. Application Services (individual — 10 services)
 9. Admin Services (individual — 3 services)
10. FluentValidation (AddValidatorsFromAssemblyContaining)
11. Rate Limiting (OtpLimiter + ApiLimiter)
12. Health Checks (basic — SqlServer/Redis ready but commented)
13. API Versioning (URL segment + X-Api-Version header)
14. Response Compression (Brotli + Gzip)
15. CORS (restricted origins + AllowCredentials)
16. Swagger + Controllers
```

### Middleware Pipeline (order matters!)
```
 1. GlobalExceptionHandler      ← Catches all unhandled → 500 + ErrorMessages.UnexpectedError
 2. RequestLoggingMiddleware    ← Logs request/response
 3. LocaleNormalizationMiddleware ← Arabic/Hindi → ASCII digits
 4. MaintenanceMiddleware       ← Returns 503 if maintenance flag on
 5. ResponseCompression         ← Brotli (priority) + Gzip
 6. Security Headers (inline)   ← X-Content-Type-Options, X-Frame-Options, CSP, etc.
 7. Swagger (dev only)          ← DefaultModelsExpandDepth(-1) (schemas hidden)
 8. HTTPS Redirection
 9. Static Files
10. CORS
11. Authentication (JWT)
12. Authorization
13. Rate Limiter
14. MapControllers
15. Health Checks (/health)
16. SignalR Hubs (commented — ready for implementation)
```

### DbInitializer (runs on startup)
```csharp
// Seeds roles: Driver, Admin, Support
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
string[] roles = ["Driver", "Admin", "Support"];
```

---

## 8. IMPLEMENTATION RULES

1. **Entity hierarchy**: All entities inherit from BaseEntity/Auditable/SoftDeletable
2. **Result<T> everywhere**: No exceptions for business logic; only Result pattern
3. **Specification for queries**: No inline lambda in repositories; use BaseSpecification
4. **Individual DI**: Each service registered individually; NO ServiceManager
5. **Phone normalization**: All phones through EgyptianPhoneHelper.Normalize() before storage
6. **UTC timestamps**: All DateTime properties use DateTime.UtcNow
7. **Centralized messages**: ALL Arabic strings in ErrorMessages/SuccessMessages — zero hardcoded
8. **Soft delete for financial data**: Orders, Customers, Partners, etc.
9. **AutoMapper v16+**: Register with `AddAutoMapper(cfg => { }, assembly)` syntax
10. **IUnitOfWork.BeginTransactionAsync**: Returns IAsyncDisposable (keeps Core EF-free)
11. **Arabic/Hindi digit normalization**: LocaleNormalizationMiddleware runs before all controllers

---

## 9. SECURITY RULES

- JWT with HMAC-SHA256, token rotation (access 24h + refresh 30 days)
- OTP: 4 digits, 5 min expiry, max 5 attempts/hour, max 3 resends/hour
- Rate limiting: OTP = 5/hour (sliding window), API = 100/min (fixed window)
- Security headers: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy, CSP, Permissions-Policy
- CORS: restricted origins (admin.sekka.app, sekka.app) + AllowCredentials for SignalR
- Phone normalization: ALL phones stored as +201XXXXXXXXX
- Soft delete for financial/historical data (never hard delete)
- AuditInterceptor: auto-fills CreatedBy, ModifiedBy on SaveChanges
- Global soft-delete query filter in SekkaDbContext

---

## 10. KNOWN ISSUES RESOLVED

| Issue | Status | Resolution |
|-------|--------|------------|
| ServiceManager anti-pattern | RESOLVED | Individual DI per service |
| HealthChecks variable scope | RESOLVED | Uses builder.Configuration directly |
| Missing soft-delete query filter | RESOLVED | Added in SekkaDbContext |
| AuditInterceptor registration | RESOLVED | Added via DbContext AddInterceptors |
| Redis TLS connection | RESOLVED | sslProtocols=tls12, connectTimeout/syncTimeout=10000 |
| SMS response format (Array) | RESOLVED | Checks JsonValueKind.Array, accesses root[0] |
| Identity package version conflict | RESOLVED | Both projects use 8.0.* |
| DefaultModelsExpandDepth placement | RESOLVED | Moved to UseSwaggerUI callback |
| Role seeding | RESOLVED | DbInitializer seeds Driver/Admin/Support |

---

## 11. WHAT'S NEXT (Planned Phases)

### Phase 2: Orders & Customers
- Order entity + lifecycle (New → Delivered/Failed/Cancelled)
- Customer entity + CallerID
- Bulk import, duplicate detection
- Order-related controllers and services

### Phase 3: Finance & Settlements
- Wallet, Settlements, Expenses, PaymentRequests
- Distributed locks for financial operations
- Cash alert system

### Phase 4: Routes & Location
- Route optimization
- LocationHistory (BaseEntity<long>, keyset pagination)
- Real-time tracking

### Phase 5: Notifications & Communication
- Firebase push, Email, WhatsApp
- SignalR hubs (OrderTracking, Notification, CashAlert, Chat)
- Redis backplane for SignalR

### Phase 6: Advanced Features
- Subscriptions & plans
- Challenges & gamification
- SOS & safety
- Community features (road reports, savings circles)
- Customer Interest Engine

### Phase 7: Admin Dashboard
- Full admin controllers
- Analytics & statistics
- Audit logs

---

## 12. TARGET SCALE (Full Implementation)

### Services (83+)
- Auth & Identity: 2 (Auth, SMS)
- Account: 1 (AccountManagement)
- Orders: 9 (Order, BulkImport, DuplicateDetection, OrderWorth, Cancellation, AddressSwap, WaitingTimer, OrderSource, AutoAssignment)
- Routes: 1
- Customers: 5 (Customer, CallerId, SmartAddress, VoiceMemo, TrackingLink)
- Finance: 5 (Wallet, CashSafety, Settlement, PaymentRequest, Invoice, SurgePricing)
- Partners: 2 (PickupPoint, PartnerPortal)
- Notifications: 5 (Notification, Firebase, SignalR, Email, WhatsApp, NotificationDispatch)
- Analytics: 3 (Statistics, Analytics, Timeline)
- Driver Features: 6 (Preferences, ParkingSpot, BreakAdvisor, SOS, Shift, Profile)
- Gamification: 4 (Challenge, Referral, Badge, HealthScore)
- Communication: 1 (Chat)
- Integration: 4 (Webhook, Sync, Demo, Search)
- New Features: 7 (SavingsCircle, Ocr, ColleagueRadar, RoadReport, Truecaller, DataPrivacy, PriceCalculation)
- Interest Engine: 6
- Admin: 25

### SignalR Hubs (4 — planned)
1. **OrderTrackingHub** `/hubs/tracking`
2. **NotificationHub** `/hubs/notifications`
3. **CashAlertHub** `/hubs/cash-alerts`
4. **ChatHub** `/hubs/chat`

### Background Services (11 — planned)
| Service | Interval |
|---------|----------|
| StaleOrderCleanup | Every 5 min |
| CashAlert | Every 10 min |
| DailyStatistics | Daily 11 PM |
| RecurringOrder | Daily 6 AM |
| WebhookDispatch | Every 15 min |
| DemoCleanup | Every 30 min |
| RoadReportCleanup | Every 15 min |
| MaintenanceWindowChecker | Every 1 min |
| AccountDeletionCleanup | Daily 2 AM |
| InterestSignalProcessor | Every 15 min |
| SegmentRefresh | Daily 11 PM |

### 82 Tables (22 groups — planned)
1. Drivers & Identity | 2. Orders | 3. Order Lifecycle (5) | 4. Customers (3)
5. Routes | 6. Finance (4) | 7. Partners (3) | 8. Notifications (3)
9. Driver Features (5) | 10. Vehicles (2) | 11. Subscriptions (2)
12. Location (1) | 13. Analytics (2) | 14. Challenges & Gamification (3)
15. SOS & Safety (2) | 16. Integration (5) | 17. Community (2) | 18. Referrals (1)
19. Savings Circles (3) | 20. Settings (3) | 21. New Features (4)
22. Customer Interest Engine (8)
