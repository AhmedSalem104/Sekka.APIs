# Sekka.API — Master Implementation Reference
# Version: 3.3 | Date: 2026-03-03 | Status: Ready for Implementation
# Source: clean-architecture-docs.md (~11,183 lines)

---

## 1. PROJECT IDENTITY

- **Name**: Sekka (سكّة) — Smart Delivery Management Platform
- **Backend**: .NET 8.0 Web API (Clean Architecture)
- **Mobile Client**: Flutter (Dart) — Bloc/Riverpod, Isar/Hive offline-first
- **Database**: SQL Server (EF Core Code-First)
- **Cache/Realtime**: Redis (Upstash) + SignalR
- **Auth**: JWT + ASP.NET Identity (OTP-based phone login)
- **Market**: Egypt-only (Egyptian mobile numbers: 010/011/012/015)

### Quick Numbers
- 6 layers | 69 Controllers (43 driver + 25 admin + 1 extension)
- 579+ endpoints | 83+ services | 82 tables
- 452+ DTOs | 82 enums | 4 SignalR hubs | 11 background services
- 5 middleware | 2 health checks | 59+ indexes | 27 constraints

---

## 2. ARCHITECTURE STRUCTURE

### Layer Dependency Graph
```
Core → Persistence → Infrastructure → Application → API
                                                    ↗
                               AdminControlDashboard
```

### Layer Details

**Sekka.Core** (no dependencies)
- DTOs/, Enums/, Exceptions/, Interfaces/ (Services/ + Persistence/)
- Common/ (Result<T>, Error, PagedResult<T>, EgyptianPhoneHelper, ApiResponse<T>)
- Specifications/ (ISpecification<T>, BaseSpecification<T>)
- Validators/ (FluentValidation + EgyptianPhoneValidatorExtensions)
- Mapping/ (MappingProfile.cs — AutoMapper)

**Sekka.Persistence** (→ Core)
- Entities/Base/ (BaseEntity, AuditableEntity, SoftDeletableEntity)
- Entities/ (Driver, Order, Customer, etc. — 82 tables)
- Configurations/ (EF Core Fluent API configs)
- Interceptors/ (AuditInterceptor — SaveChangesInterceptor)
- Migrations/, Seeds/ (JSON seed files), SekkaDbContext.cs, DbInitializer.cs

**Sekka.Infrastructure** (→ Persistence)
- Repositories/GenericRepository.cs, SpecificationEvaluator.cs
- UnitOfWork.cs

**Sekka.Application** (→ Core + Infrastructure)
- Services/Base/BaseService.cs
- Services/ (45+ business services)
- BackgroundServices/ (11 hosted services)

**Sekka.AdminControlDashboard** (→ Application + Persistence)
- Services/ (25+ admin services, each independent — NO AdminServiceManager)

**Sekka.API** (→ All)
- Controllers/Base/BaseCrudController.cs
- Controllers/Driver/ (32 controllers), Controllers/Admin/ (12+ controllers)
- Hubs/ (4 SignalR hubs)
- Middleware/ (5 middleware classes)
- Extensions/ServiceExtensions.cs
- Program.cs, appsettings.json

---

## 3. CORE PATTERNS

### 3.1 Entity Hierarchy
```
BaseEntity<TKey>           → Id, CreatedAt
  ↓ AuditableEntity<TKey>  → + UpdatedAt, CreatedBy, ModifiedBy
    ↓ SoftDeletableEntity<TKey> → + IsDeleted, DeletedAt, DeletedBy
```

Entity Classification:
- **SoftDeletableEntity<Guid>**: Orders, Customers, Partners, Vehicles, Routes, Subscriptions, WebhookConfigs
- **AuditableEntity<Guid>**: Driver, Settlements, WalletTransactions, Notifications, most tables
- **BaseEntity<long>**: LocationHistory (bigint IDENTITY — write-heavy)
- **BaseEntity<Guid>**: RoadReportConfirmations, SavingsCirclePayments, WebhookLogs

### 3.2 Result Pattern
```csharp
Result<T> {
    bool IsSuccess; T? Value; Error? Error;
    static Success(T), Failure(Error), NotFound(msg), BadRequest(msg), Conflict(msg), Unauthorized(msg)
}
record Error(string Code, string Message);
// Codes: "NOT_FOUND", "BAD_REQUEST", "CONFLICT", "UNAUTHORIZED"
```

### 3.3 Specification Pattern
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

### 3.4 Generic Repository + Unit of Work
```csharp
IGenericRepository<TEntity, TKey> {
    GetByIdAsync, ListAsync(spec), CountAsync(spec),
    AddAsync, Update, Delete
}
IUnitOfWork {
    GetRepository<TEntity, TKey>(), SaveChangesAsync(), BeginTransactionAsync()
}
```

### 3.5 Generic BaseService
```csharp
BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : AuditableEntity<Guid>
{
    // Injected: IUnitOfWork, IMapper, ILogger
    GetPagedAsync(spec, pagination), GetByIdAsync(id),
    CreateAsync(dto), UpdateAsync(id, dto), SoftDeleteAsync(id)
}
```

### 3.6 Generic BaseCrudController
```csharp
[ApiController][ApiVersion("1.0")][Authorize]
BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto> : ControllerBase
{
    GetById(Guid id), Create(TCreateDto), Update(Guid id, TUpdateDto), Delete(Guid id)
    ToActionResult<T>(Result<T>, successCode=200) — maps error codes to HTTP status
}
```

### 3.7 ApiResponse<T>
```csharp
ApiResponse<T> { bool IsSuccess, T? Data, string? Message, List<string>? Errors }
static Success(T data), Fail(string message), Fail(List<string> errors)
```

### 3.8 PagedResult<T>
```csharp
PagedResult<T> { List<T> Items, int TotalCount, int Page, int PageSize, int TotalPages }
```

### 3.9 EgyptianPhoneHelper (static)
```
Normalize(phone) → +201XXXXXXXXX (13 chars, canonical format)
IsValid(phone), IsMobile(phone) → bool
GetPhoneType(phone) → PhoneType enum
ToInternationalFormat(phone) → 201XXXXXXXXX (for SMS API, no +)
ToDisplayFormat(phone) → +20 101 234 5678
GetCarrierName(phone) → "Vodafone"|"Etisalat"|"Orange"|"WE"|"Unknown"
```
Networks: 010=Vodafone, 011=Etisalat, 012=Orange, 015=WE

### 3.10 FluentValidation
- Extension: `MustBeEgyptianMobile()` — uses EgyptianPhoneHelper.IsMobile()
- 10+ validators: SendOtpDto, VerifyOtpDto, CreateOrderDto, CheckDuplicateDto, CreatePartnerDto, UpdatePartnerDto, CreateCallerIdNoteDto, CreateEmergencyContactDto, CreatePaymentRequestDto, OcrToOrderDto
- Registration: `AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>()`

---

## 4. DATABASE OVERVIEW

### 82 Tables organized in 22 groups:

1. **Drivers & Identity** (1): Drivers (extends IdentityUser<Guid>)
2. **Orders** (1): Orders (SoftDeletable, OrderNumber unique per driver)
3. **Order Lifecycle** (5): DeliveryAttempts, OrderPhotos, CancellationLogs, AddressSwapLogs, WaitingTimers
4. **Customers** (3): Customers, CallerIdNotes, BlockedCustomers
5. **Routes** (1): Routes (SoftDeletable)
6. **Finance** (4): WalletTransactions, Settlements, Expenses, PaymentRequests
7. **Partners** (3): Partners, PickupPoints, PartnerPortalSettings
8. **Notifications** (3): Notifications, DeviceTokens, NotificationChannelPreferences
9. **Driver Features** (5): DriverPreferences, ParkingSpots, BreakLogs, EmergencyContacts, QuickMessageTemplates
10. **Vehicles** (2): Vehicles, MaintenanceRecords
11. **Subscriptions** (2): SubscriptionPlans, Subscriptions (SoftDeletable)
12. **Location** (1): LocationHistory (BaseEntity<long>, keyset pagination)
13. **Analytics** (2): DailyStats, Ratings
14. **Challenges & Gamification** (3): Challenges, DriverAchievements, Badges
15. **SOS & Safety** (2): SOSLogs, FieldAssistanceRequests
16. **Integration** (5): WebhookConfigs, WebhookLogs, SyncQueue, OrderSourceTags, OrderTransferLogs
17. **Community** (2): RoadReports, RoadReportConfirmations
18. **Referrals** (1): Referrals
19. **Savings Circles** (3): SavingsCircles, SavingsCircleMembers, SavingsCirclePayments
20. **Settings** (3): Regions, AppConfigurations, CommunityBlacklist
21. **New Features** (4): VoiceMemos, Addresses, Shifts, AuditLogs
22. **Customer Interest Engine** (8): InterestSignals, InterestCategories, CustomerInterestScores, CustomerProfiles, Segments, SegmentRules, SegmentMembers, Recommendations

### Key Relationships
- Driver (1→N): Orders, Customers, Routes, WalletTransactions, Settlements
- Order (1→N): DeliveryAttempts, OrderPhotos, CancellationLogs, WaitingTimers
- Customer: per-driver (DriverId + Phone unique constraint)

---

## 5. API STRUCTURE

### Driver Controllers (43 total)
Route prefix: `api/v{version}/`

1. AuthController `[api/auth]` — OTP flow (send/verify/register/refresh/logout)
2. OrdersController `[api/orders]` — Full order lifecycle (CRUD + status + deliver/fail/cancel/transfer)
3. BulkImportController `[api/orders/bulk]` — Clipboard paste import
4. DuplicateController `[api/orders/duplicates]` — Check/resolve duplicates
5. OrderWorthController `[api/orders/worth]` — "Is it worth it?" calculator
6. CancellationController `[api/orders/cancellations]` — Cancel with loss tracking
7. AddressSwapController `[api/orders/address-swap]` — Swap delivery address
8. WaitingTimerController `[api/orders/waiting]` — Start/stop waiting timers
9. RouteController `[api/routes]` — Optimize, reorder, add/remove orders
10. CustomerController `[api/customers]` — CRUD + merge + smart-address
11. CallerIdController `[api/callerid]` — Lookup, notes, Truecaller
12. SmartAddressController `[api/smart-address]` — Frequent addresses, suggestions
13. WalletController `[api/wallet]` — Balance, transactions, transfer
14. SettlementController `[api/settlements]` — Create, approve, history
15. PaymentRequestController `[api/payments]` — Payment requests (VodafoneCash/InstaPay)
16. PartnerController `[api/partners]` — CRUD + stats + pickup-points
17. NotificationController `[api/notifications]` — List, read, preferences
18. StatisticsController `[api/statistics]` — Daily/weekly/monthly stats
19. AnalyticsController `[api/analytics]` — Deep analytics, trends, heatmaps
20. DriverPreferencesController `[api/preferences]` — Theme, map, language settings
21. ParkingSpotController `[api/parking]` — Save/share parking spots
22. BreakAdvisorController `[api/breaks]` — Suggest break spots
23. SOSController `[api/sos]` — Emergency alert
24. ShiftController `[api/shifts]` — Start/end/summary
25. ChallengeController `[api/challenges]` — Active challenges, progress
26. ReferralController `[api/referrals]` — Generate code, track
27. BadgeController `[api/badges]` — List badges, progress
28. HealthScoreController `[api/health-score]` — Driver health score
29. SavingsCircleController `[api/savings]` — Create/join circles
30. VoiceMemoController `[api/voice-memos]` — Record for orders
31. OcrController `[api/ocr]` — Photo → order parsing
32. RoadReportController `[api/road-reports]` — Report/confirm road issues
33. SearchController `[api/search]` — Global search
34. SyncController `[api/sync]` — Offline sync
35. DemoController `[api/demo]` — Demo mode
36. ProfileController `[api/profile]` — Full driver profile
37. ChatController `[api/chat]` — Conversations, messages
38. TrackingLinkController `[api/tracking]` — Customer tracking links
39. InvoiceController `[api/invoices]` — View invoices
40. DisputeController `[api/disputes]` — Create/track disputes
41. RefundController `[api/refunds]` — Request refunds
42. InterestEngineController `[api/interest]` — Customer interests, profiles, recommendations
43. ColleagueRadarController `[api/colleagues]` — Nearby drivers

### Admin Controllers (25 total)
Route prefix: `api/admin/`

1-12. AdminDrivers, AdminOrders, AdminSettlements, AdminPartners, AdminCustomers,
      AdminStatistics, AdminNotifications, AdminRegions, AdminBlacklist, AdminConfig,
      AdminPayment, AdminRoles
13-18. AdminAuditLogs, AdminSubscriptions, AdminWallet, AdminVehicles, AdminSavingsCircles, AdminSOS
19-22. AdminDisputes, AdminInvoice, AdminRefund, AdminTimeSlots
23-25. AdminSegments, AdminCampaigns, AdminInsights (Customer Interest Engine Admin)

---

## 6. REALTIME SYSTEM (SignalR)

### 4 Hubs with Redis Backplane

1. **OrderTrackingHub** `/hubs/tracking`
   - S→C: OrderStatusChanged, DriverLocationUpdated, OrderAssigned, OrderTransferred, NewOrderCreated
   - C→S: JoinOrderGroup, LeaveOrderGroup, UpdateLocation, JoinAdminDashboard
   - Groups: Order_{id}, Driver_{id}, AdminDashboard
   - Admin extras: DashboardStatsUpdated, AlertCreated, SOSActivated, DriverStatusChanged, SegmentMembershipChanged, InterestTrendAlert, CampaignStatusChanged

2. **NotificationHub** `/hubs/notifications`
   - S→C: NewNotification, NotificationRead, BroadcastMessage, SettlementApproved, PaymentApproved
   - C→S: MarkAsRead, MarkAllAsRead, JoinDriverGroup

3. **CashAlertHub** `/hubs/cash-alerts`
   - S→C: CashThresholdExceeded, SettlementReminder, DailySettlementSummary, DepositConfirmed
   - C→S: AcknowledgeAlert

4. **ChatHub** `/hubs/chat`
   - S→C: NewMessage, MessageRead, ConversationClosed, TypingIndicator
   - C→S: JoinConversation, LeaveConversation, SendMessage, StartTyping, StopTyping
   - Groups: Chat_{conversationId}, AdminChat

JWT via query string: `OnMessageReceived` extracts `access_token` for `/hubs/*` paths

---

## 7. INFRASTRUCTURE

### NuGet Packages by Layer

**Core**: AutoMapper 13+, FluentValidation 11.9+
**Persistence**: EFCore 8.0+, EFCore.SqlServer, EFCore.Tools, Identity.EntityFrameworkCore
**Infrastructure**: (none — relies on Persistence reference)
**Application**: StackExchange.Redis, FirebaseAdmin, AutoMapper.Extensions.DI, Http.Polly, Serilog.AspNetCore, Serilog.Sinks.File
**API**: Swashbuckle, JwtBearer, SignalR.StackExchangeRedis, RateLimiting, OutputCaching.StackExchangeRedis, FluentValidation.AspNetCore, Asp.Versioning.Mvc, HealthChecks.SqlServer, HealthChecks.Redis, ResponseCompression, EFCore.BulkExtensions

### Redis Keys
| Pattern | Purpose | TTL |
|---------|---------|-----|
| Sekka_OTP:{phone} | OTP code | 5 min |
| Sekka_OTP_ATTEMPTS:{phone} | Failed attempts | 60 min |
| Sekka_OTP_RESEND:{phone} | Resend count | 60 min |
| Sekka_REFRESH:{driverId} | Refresh token | 30 days |
| Sekka_BLACKLIST:{token} | JWT blacklist | 24 hours |
| Sekka_IDEMPOTENCY:{key} | Duplicate prevention | 24 hours |
| Sekka_INTEREST_PROFILE:{cid}:{did} | Customer profile cache | 30 min |
| Sekka_SIGNAL_LOCK:processing | Distributed lock | 20 min |

### Background Services (11)
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

---

## 8. SECURITY RULES

- JWT with token rotation (access + refresh via Redis)
- OTP: 4 digits, 5 min expiry, max 5 attempts/hour, 60s resend cooldown
- Rate limiting: OTP = 5/hour (sliding window), API = 100/min (fixed window)
- Distributed locks (Redlock) for financial operations
- Idempotency keys for payment/order creation
- Security headers: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy, CSP, Permissions-Policy
- CORS: restricted origins (admin.sekka.app, sekka.app) + AllowCredentials for SignalR
- Wallet freeze via Redis for fraud investigation
- Phone normalization: ALL phones stored as +201XXXXXXXXX in DB and Redis
- Soft delete for financial/historical data (never hard delete)
- AuditInterceptor: auto-fills CreatedBy, ModifiedBy on SaveChanges

---

## 9. PERFORMANCE RULES

- Specification pattern with AsNoTracking = true by default
- Keyset pagination for LocationHistory (write-heavy, bigint PK)
- Offset pagination (PagedResult<T>) for all other tables
- Bulk operations via EFCore.BulkExtensions
- Response compression: Brotli (priority) + Gzip
- Output cache with Redis backend (selective TTL per endpoint type)
- Redis caching for interest engine profiles/segments/recommendations
- Compiled regex in EgyptianPhoneHelper
- API versioning via URL segment + X-Api-Version header

---

## 10. BUSINESS FLOWS

### Order Lifecycle
```
New → Accepted → PickedUp → InTransit → Arrived → Delivered
                                                 → Failed (→ retry up to 3)
                                                 → Cancelled
                                                 → PartiallyDelivered
                          → Transferred (to another driver)
```

### Auth Flow
```
SendOtp(phone) → Redis stores OTP (5 min TTL)
VerifyOtp(phone, code) → if new user: return IsNewUser=true
CompleteRegistration(name, vehicle) → create Driver + JWT
Login → JWT access token + refresh token (Redis 30 days)
Refresh → rotate both tokens
Logout → blacklist token in Redis
```

### Financial Flow
- Orders collect cash → wallet balance increases
- Cash alert when balance > threshold (default 2000 EGP)
- Settlement: driver deposits cash at partner → admin confirms
- Distributed lock for concurrent financial operations

### Customer Interest Engine Flow
```
Signal Collection → Batch Scoring (every 15 min)
→ Time Decay (factor 0.95 per 7 days)
→ RFM Analysis (Recency/Frequency/Monetary)
→ Smart Segmentation (daily refresh at 11 PM)
→ Targeted Campaigns + Personalized Recommendations
```

---

## 11. PROJECT SCALE

### Services (83+)
- Auth & Identity: 2 (Auth, SMS)
- Account: 1 (AccountManagement)
- Orders: 9 (Order, BulkImport, DuplicateDetection, OrderWorth, Cancellation, AddressSwap, WaitingTimer, OrderSource, AutoAssignment)
- Disputes & Refunds: 2
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
- New Features: 7 (SavingsCircle, Ocr, ColleagueRadar, RoadReport, Truecaller, PriceCalculation, Localization, DataPrivacy)
- Interest Engine: 6 (InterestEngine, CustomerProfiling, Segmentation, Recommendation, BehaviorAnalysis, CampaignTargeting)
- Admin: 25 (see Admin Controllers list)

### Enums (82+)
OrderStatus, PaymentMethod, VehicleType, NotificationType, CancellationReason, TransferStatus,
DeliveryFailReason, PhotoType, OrderPriority, SubscriptionStatus, SOSStatus, CircleStatus,
ReferralStatus, SyncOperation, ThemeMode, MapApp, etc.

---

## 12. KNOWN ISSUES & IMPROVEMENTS

1. **ServiceManager anti-pattern**: Doc references both IServiceManager and individual DI. Decision: Use individual DI (ISP principle). Delete ServiceManager/AdminServiceManager.
2. **HealthChecks variable**: Program.cs uses `connectionString` variable not shown in scope — should use `builder.Configuration.GetConnectionString("DefaultConnection")`.
3. **Duplicate IsMobile/IsValid**: Both do the same thing in EgyptianPhoneHelper. Consider keeping only one.
4. **Missing ChatHub in Program.cs mapping**: Doc shows 3 hub mappings but ChatHub `/hubs/chat` is the 4th hub — must be added.
5. **AuditInterceptor registration**: Registered as Scoped but should be added to DbContext options via AddInterceptors.
6. **Output Cache setup**: Referenced in middleware pipeline but AddOutputCache() with Redis config not shown in Program.cs.
7. **No global query filter for soft delete**: SoftDeletableEntity should have `HasQueryFilter(e => !e.IsDeleted)` in DbContext.

---

## 13. IMPLEMENTATION RULES

1. **Entity hierarchy first**: All entities inherit from BaseEntity/Auditable/SoftDeletable
2. **Result<T> everywhere**: No exceptions for business logic; only Result pattern
3. **Specification for queries**: No inline lambda in repositories; use BaseSpecification
4. **Individual DI**: Each service registered individually; NO ServiceManager
5. **Phone normalization**: All phones through EgyptianPhoneHelper.Normalize() before storage
6. **UTC timestamps**: All DateTime properties use DateTime.UtcNow
7. **AutoMapper for DTO mapping**: Never manual mapping in services
8. **Soft delete for financial data**: Orders, Customers, Partners, etc.
9. **Distributed locks for money operations**: Settlements, wallet transfers, payments
10. **Idempotency keys for create operations**: Orders, payments
11. **Arabic/Hindi digit normalization**: LocaleNormalizationMiddleware runs before all controllers

### Middleware Pipeline Order (critical!)
```
1. GlobalExceptionHandler
2. RequestLoggingMiddleware (Serilog)
3. LocaleNormalizationMiddleware (Arabic/Hindi → ASCII digits)
4. MaintenanceMiddleware
5. ResponseCompression (Brotli/Gzip)
6. Security Headers (inline)
7. Swagger (dev only)
8. HTTPS Redirection
9. Static Files
10. CORS
11. Authentication (JWT)
12. Authorization
13. Rate Limiter
14. Output Cache
15. MapControllers
16. Health Checks (/health, /health/ready)
17. SignalR Hubs (/hubs/*)
```

### DI Registration Order
```
1. DbContext + Identity
2. JWT Authentication
3. Redis (Cache + SignalR Backplane)
4. AutoMapper
5. Serilog
6. FluentValidation
7. Repository + UnitOfWork
8. Application Services (individual)
9. Admin Services (individual)
10. Background Services
11. Rate Limiting
12. Health Checks
13. API Versioning
14. Response Compression
15. CORS
16. Swagger + Controllers
```

---

## 14. APPSETTINGS STRUCTURE

```json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "JwtSettings": { "SecretKey", "Issuer": "Sekka.API", "Audience": "Sekka.Mobile", "AccessTokenExpiryMinutes": 1440, "RefreshTokenExpiryDays": 30 },
  "OtpSettings": { "Length": 4, "ExpiryMinutes": 5, "MaxAttemptsPerNumber": 5, "MaxAttemptsWindowMinutes": 60, "ResendCooldownSeconds": 60, "MaxResendPerNumber": 3, "UseFakeOtpInDev": true, "FakeOtpCode": "1234" },
  "Redis": { "ConnectionString": "...", "InstanceName": "Sekka_" },
  "FileStorage": { "BasePath": "wwwroot/uploads", "BaseUrl": "https://api.sekka.app/uploads", "MaxFileSizeBytes": 5242880, "AllowedExtensions": [".jpg",".jpeg",".png",".webp"] },
  "SmsApi": { "PrimaryProvider": "Twilio", "FallbackProvider": "Vonage", ... },
  "EmailSettings": { "SmtpServer": "smtp.sendgrid.net", ... },
  "WhatsAppSettings": { "ApiUrl": "https://graph.facebook.com/v18.0", ... },
  "Firebase": { "ProjectId": "sekka-app", "CredentialPath": "firebase-credentials.json" },
  "AppSettings": { "MaxOrdersPerDay": 100, "CashAlertThresholdEGP": 2000, "StaleOrderMinutes": 30, "MaxDeliveryAttempts": 3, "DemoSessionExpiryHours": 24 },
  "InterestEngine": { "SignalWeights": {...}, "DecayFactor": 0.95, "DecayIntervalDays": 7, ... },
  "Serilog": { structured logging config },
  "CorsPolicy": { "AllowedOrigins": ["https://admin.sekka.app","https://sekka.app"], ... },
  "ApiVersioning": { "DefaultVersion": "1.0" },
  "HealthChecks": { "SqlServer": true, "Redis": true }
}
```

---

## 15. FOLDER TREE (Target Structure)

```
Sekka.API.sln
├── Sekka.Core/
│   ├── Common/           ← Result<T>, Error, PagedResult<T>, EgyptianPhoneHelper, ApiResponse<T>, PaginationDto
│   ├── DTOs/             ← 12+ subfolders (Auth/, Order/, Customer/, Route/, Wallet/, Settlement/, Partner/, Settings/, SOS/, Sync/, Analytics/, Admin/, Common/)
│   ├── Enums/            ← 82+ enums
│   ├── Exceptions/       ← BadRequest, NotFound, UnAuthorized, Conflict
│   ├── Interfaces/
│   │   ├── Services/     ← IOrderService, ICustomerService, etc.
│   │   └── Persistence/  ← IGenericRepository<,>, IUnitOfWork
│   ├── Specifications/   ← ISpecification<T>, BaseSpecification<T>, Orders/, Customers/
│   ├── Validators/       ← Common/, Auth/, Orders/, Partners/, CallerIds/, Emergency/, Payments/, Ocr/
│   └── Mapping/          ← MappingProfile.cs
├── Sekka.Persistence/
│   ├── Entities/
│   │   ├── Base/         ← BaseEntity, AuditableEntity, SoftDeletableEntity
│   │   └── (82 entity files)
│   ├── Configurations/   ← EF Fluent API configs
│   ├── Interceptors/     ← AuditInterceptor
│   ├── Migrations/
│   ├── Seeds/            ← regions.json, messageTemplates.json, vehicleTypes.json, breakSpots.json
│   ├── SekkaDbContext.cs
│   └── DbInitializer.cs
├── Sekka.Infrastructure/
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   └── SpecificationEvaluator.cs
│   └── UnitOfWork.cs
├── Sekka.Application/
│   ├── Services/
│   │   ├── Base/         ← BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
│   │   └── (45+ service files)
│   └── BackgroundServices/ ← 11 hosted services
├── Sekka.AdminControlDashboard/
│   └── Services/         ← 25+ admin service files
└── Sekka.API/
    ├── Controllers/
    │   ├── Base/         ← BaseCrudController
    │   ├── Driver/       ← 43 controllers
    │   └── Admin/        ← 25 controllers
    ├── Hubs/             ← OrderTrackingHub, NotificationHub, CashAlertHub, ChatHub
    ├── Middleware/        ← GlobalExceptionHandler, RequestLogging, LocaleNormalization, Maintenance, SecurityHeaders
    ├── Extensions/       ← ServiceExtensions.cs
    ├── Program.cs
    ├── appsettings.json
    └── appsettings.Development.json
```
