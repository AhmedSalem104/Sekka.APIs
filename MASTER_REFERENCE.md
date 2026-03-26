# Sekka.API — Master Implementation Reference
# Version: 5.0 | Date: 2026-03-26 | Status: Phase 3 Complete
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

### Current Implementation Status (Phases 0–3)

| Phase | Status | Tables | Controllers | Endpoints | Services | Enums | DTOs |
|-------|--------|--------|-------------|-----------|----------|-------|------|
| 0 — Foundation | COMPLETE | — | 1 (Base) | — | 1 (Base) | — | 2 |
| 1 — Auth & Identity | COMPLETE | 15 | 12 | 84 | 14 | 12 | 80+ |
| 2 — Orders & Delivery | COMPLETE | 12 | 9 | 72+ | 18 | 13 | 70+ |
| 3 — Customers & Partners | COMPLETE | 8 | 9 | 57 | 6 | 5 | 40+ |
| **TOTAL** | **3 Phases** | **35** | **31** | **213+** | **39** | **30** | **190+** |

- 3 Migrations applied (Phase1_AuthIdentity, Phase2_OrdersDelivery, Phase3_CustomersPartners)
- 2 Background services active (StaleOrderCleanup, CashAlert)
- 1 SignalR Hub active (OrderTrackingHub `/hubs/tracking`)
- Centralized message system: 50+ ErrorMessages, 43+ SuccessMessages
- Frontend docs: AUTH_API.md, ORDERS_API.md, CUSTOMERS_PARTNERS_API.md

---

## 2. ARCHITECTURE STRUCTURE

### Layer Dependency Graph
```
Core → Persistence → Infrastructure → Application → API
                                                    ↗
                               AdminControlDashboard
```

### Layer Details (Current — Phase 3)

**Sekka.Core** (no dependencies)
```
Common/
  ├── Result.cs, ApiResponse.cs, PagedResult.cs, EgyptianPhoneHelper.cs
  └── Messages/
      ├── ErrorMessages.cs      ← 50+ centralized error strings
      └── SuccessMessages.cs    ← 43+ centralized success strings
DTOs/
  ├── Auth/         ← 12 DTOs (Login, Register, OTP, JWT, etc.)
  ├── Profile/      ← 19 DTOs (DriverProfile, Stats, Badges, Expenses, etc.)
  ├── Settings/     ← 9 DTOs (Preferences, Notifications, QuietHours, etc.)
  ├── Account/      ← AccountManagementDtos
  ├── Privacy/      ← PrivacyDtos
  ├── Admin/        ← AdminDriverDto, RoleDto, AdminSubscriptionDtos,
  │                   AdminCustomerDtos, AdminPartnerDtos, BlacklistDtos
  ├── Badge/        ← DigitalBadgeDto
  ├── Demo/         ← DemoSessionDto
  ├── HealthScore/  ← AccountHealthDto
  ├── Order/        ← 14 files: OrderDtos, OrderActionDtos, OrderSubDtos,
  │                   BulkImportDtos, OrderWorthDtos, DisclaimerDtos, TrackingDtos,
  │                   AutoAssignmentDtos, TimeSlotDtos, AdminOrderDtos,
  │                   TimelineDtos, OcrDtos, VoiceMemoDtos, RecurringOrderDtos,
  │                   SmartAddressDtos, OrderSourceDtos
  ├── Route/        ← RouteDtos
  ├── Sync/         ← SyncDtos
  ├── Customer/     ← CustomerDtos, RatingDtos, BlockDtos, AddressDtos, CallerIdDtos
  ├── Partner/      ← PartnerDtos, PickupPointDtos, PartnerPortalDtos
  └── Common/       ← PaginationDto
Enums/ (30 total)
  Phase 1: VehicleType, PhoneType, ThemeMode, MapApp, DevicePlatform,
           DeletionRequestStatus, NotificationType, NotificationPriority,
           ShiftStatus, SubscriptionStatus, OrderStatus, ChallengeType
  Phase 2: PaymentMethod, OrderPriority, OrderSourceType, DeliveryFailReason,
           PhotoType, CancellationReason, TransferStatus, SourceTagType,
           SyncOperation, SyncStatus, DuplicateAction, AssignmentStrategy,
           TimelineEventType
  Phase 3: AddressType, ContactType, PartnerType, CommissionType, VerificationStatus
Interfaces/
  ├── Services/     ← 34 interfaces (13 Phase1 + 15 Phase2 + 6 Phase3)
  └── Persistence/  ← IGenericRepository<TEntity,TKey>, IUnitOfWork
Specifications/     ← ISpecification<T>, BaseSpecification<T>
Validators/
  ├── Common/       ← EgyptianPhoneValidatorExtensions
  ├── Auth/         ← 7 validators
  ├── Emergency/    ← 1 validator
  ├── Orders/       ← 10 validators
  └── Customers/    ← 6 validators
Mapping/            ← MappingProfile.cs (Core-level)
```

**Sekka.Persistence** (→ Core)
```
Entities/ (27 + 3 base = 30 entity classes)
  ├── Base/         ← BaseEntity<TKey>, AuditableEntity<TKey>, SoftDeletableEntity<TKey>
  Phase 1 (7):
  ├── Driver.cs, DriverPreferences.cs, NotificationChannelPreference.cs,
  │   AccountDeletionRequest.cs, ActiveSession.cs, UserConsent.cs, DataDeletionRequest.cs
  Phase 2 (12):
  ├── Order.cs, DeliveryAttempt.cs, OrderPhoto.cs, AddressSwapLog.cs,
  │   CancellationLog.cs, OrderSourceTag.cs, WaitingTimer.cs, OrderTransferLog.cs,
  │   VoiceMemo.cs, SyncQueue.cs, TrackingLink.cs, DeliveryTimeSlot.cs
  Phase 3 (8):
  └── Customer.cs, Address.cs, Rating.cs, CallerIdNote.cs, BlockedCustomer.cs,
      Partner.cs, PickupPoint.cs, CommunityBlacklist.cs

Configurations/ (27 files — one per entity)
Interceptors/       ← AuditInterceptor (SaveChangesInterceptor)
Migrations/
  ├── Phase1_AuthIdentity (15 tables)
  ├── Phase2_OrdersDelivery (12 tables)
  └── Phase3_CustomersPartners (8 tables)
SekkaDbContext.cs   ← 35 DbSets, global soft-delete filter
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
Services/ (26 service implementations)
  ├── Base/BaseService.cs
  Phase 1 (11): AuthService, SmsService, ProfileService, DriverPreferencesService,
                AccountManagementService, DataPrivacyService, DemoService,
                EmailService, BadgeService, HealthScoreService
  Phase 2 (15): OrderService, CancellationService, OrderTransferService,
                BulkImportService, DuplicateDetectionService, OrderWorthService,
                AddressSwapService, WaitingTimerService, RouteService,
                RecurringOrderService, SyncService, TimelineService,
                TrackingLinkService, AutoAssignmentService, TimeSlotService,
                SmartAddressService, VoiceMemoService, OrderSourceService
  Phase 3 (6): CustomerService, CallerIdService, AddressService,
               PartnerService, PickupPointService, PartnerPortalService
BackgroundServices/
  ├── StaleOrderCleanupService  ← Every 5 min (cancel pending > 30 min)
  └── CashAlertBackgroundService ← Every 10 min (check cash thresholds)
Mapping/
  └── ApplicationMappingProfile.cs ← All Entity↔DTO mappings
```

**Sekka.AdminControlDashboard** (→ Application + Persistence)
```
Services/
  ├── AdminDriversService.cs
  ├── AdminRolesService.cs
  └── AdminSubscriptionsService.cs
```

**Sekka.API** (→ All)
```
Controllers/ (31 total)
  ├── Base/BaseCrudController.cs
  ├── Driver/ (20 controllers)
  │   Phase 1: AuthController (15), ProfileController (20), SettingsController (10),
  │            DataPrivacyController (5), BadgeController (2), HealthScoreController (2),
  │            DemoController (4), LookupsController (1)
  │   Phase 2: OrderController (27), RouteController (5), RecurringOrderController (6),
  │            SyncController (4), TrackingController (1), TimelineController (3), OcrController (3)
  │   Phase 3: CustomerController (11), CallerIdController (5), AddressController (6),
  │            PartnerController (8), PickupPointController (4), PartnerPortalController (6)
  └── Admin/ (10 controllers)
      Phase 1: AdminDriversController (6), AdminRolesController (7),
               AdminSubscriptionsController (13)
      Phase 2: AdminOrdersController (10), AdminTimeSlotsController (6)
      Phase 3: AdminCustomersController (5), AdminPartnersController (8),
               AdminBlacklistController (4)
Hubs/
  └── OrderTrackingHub.cs ← /hubs/tracking (JoinOrderGroup, UpdateLocation, JoinAdmin)
Middleware/ (4)
  ├── GlobalExceptionHandler.cs
  ├── RequestLoggingMiddleware.cs
  ├── LocaleNormalizationMiddleware.cs
  └── MaintenanceMiddleware.cs
Program.cs ← Full DI (39 services + 2 background + hub)
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
Result<T> { bool IsSuccess; T? Value; Error? Error; }
static Success(T), Failure(Error), NotFound(msg), BadRequest(msg), Conflict(msg), Unauthorized(msg)
// Codes: "NOT_FOUND", "BAD_REQUEST", "CONFLICT", "UNAUTHORIZED"
```

### 3.3 ApiResponse Pattern
```csharp
ApiResponse<T> { bool IsSuccess, T? Data, string? Message, List<string>? Errors }
static Success(T data, string? message = null), Fail(string message), Fail(List<string> errors)
```

### 3.4 Specification Pattern
```csharp
BaseSpecification<T> : ISpecification<T> {
    SetCriteria(), AddInclude(), SetOrderBy(), SetOrderByDescending(), ApplyPaging()
}
// Used as inline internal classes in services (e.g., DriverOrdersPagedSpec, ActiveWaitingTimerSpec)
```

### 3.5 Generic Repository + Unit of Work
```csharp
IGenericRepository<TEntity, TKey> { GetByIdAsync, ListAsync, CountAsync, AddAsync, Update, Delete }
IUnitOfWork { GetRepository<,>(), SaveChangesAsync(), BeginTransactionAsync() → IAsyncDisposable }
```

---

## 4. DATABASE (35 Tables)

### Phase 1 — Auth & Identity (15 tables)
```
Identity (9): AspNetUsers(Driver), AspNetRoles, AspNetUserRoles, AspNetUserClaims,
              AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens
Custom (6): DriverPreferences, NotificationChannelPreferences, AccountDeletionRequests,
            ActiveSessions, UserConsents, DataDeletionRequests
```

### Phase 2 — Orders & Delivery (12 tables)
```
Orders, DeliveryAttempts, OrderPhotos, AddressSwapLogs, CancellationLogs,
OrderSourceTags, WaitingTimers, OrderTransferLogs, VoiceMemos,
SyncQueues, TrackingLinks, DeliveryTimeSlots
```

### Phase 3 — Customers & Partners (8 tables)
```
Customers (UNIQUE: DriverId+Phone), Addresses, Ratings (8 bool tags),
CallerIdNotes (UNIQUE: DriverId+PhoneNumber), BlockedCustomers (UNIQUE: DriverId+CustomerPhone),
Partners (with verification workflow), PickupPoints, CommunityBlacklist (PhoneNumber PK)
```

### Connection
- Remote SQL Server: `db43715.public.databaseasp.net`
- Redis: `infinite-glider-85179.upstash.io:6379` (Upstash, TLS)

---

## 5. DI REGISTRATION (Program.cs)

```
 1. DbContext + AuditInterceptor
 2. ASP.NET Identity (Driver, IdentityRole<Guid>)
 3. JWT Authentication (JwtBearer + SignalR query string)
 4. Redis Cache (StackExchangeRedisCache — Upstash)
 5. SignalR
 6. AutoMapper (Core + Application assemblies)
 7. GenericRepository<,> + UnitOfWork
 8. Phase 1 Services (11): Auth, SMS, Profile, Preferences, Account, Privacy, Demo, Email, Badge, HealthScore
 9. Admin Services (3): AdminDrivers, AdminRoles, AdminSubscriptions
10. Phase 2 Services (18): Order, Cancellation, Transfer, BulkImport, DuplicateDetection,
    OrderWorth, AddressSwap, WaitingTimer, Route, RecurringOrder, Sync, Timeline,
    TrackingLink, AutoAssignment, TimeSlot, SmartAddress, VoiceMemo, OrderSource
11. Phase 3 Services (6): Customer, CallerId, Address, Partner, PickupPoint, PartnerPortal
12. Background Services (2): StaleOrderCleanup, CashAlert
13. FluentValidation (assembly scan)
14. Rate Limiting (OtpLimiter + ApiLimiter)
15. Health Checks
16. API Versioning + Response Compression + CORS + Swagger
```

### Middleware Pipeline
```
1. GlobalExceptionHandler → 2. RequestLogging → 3. LocaleNormalization → 4. Maintenance
5. ResponseCompression → 6. Security Headers → 7. Swagger → 8. HTTPS → 9. Static Files
10. CORS → 11. Auth → 12. Authorization → 13. RateLimiter
14. MapControllers → 15. /health → 16. /hubs/tracking (OrderTrackingHub)
```

---

## 6. IMPLEMENTATION RULES

1. Entity hierarchy: All entities inherit from BaseEntity/Auditable/SoftDeletable
2. Result<T> everywhere: No exceptions for business logic
3. Specification for queries: Inline internal spec classes in services
4. Individual DI: Each service registered individually
5. Phone normalization: All phones through EgyptianPhoneHelper.Normalize()
6. UTC timestamps: All DateTime properties use DateTime.UtcNow
7. Centralized messages: ALL Arabic strings in ErrorMessages/SuccessMessages
8. Soft delete: For Order (financial data)
9. AutoMapper: Registered with both Core + Application assemblies
10. IUnitOfWork.BeginTransactionAsync: Returns IAsyncDisposable (Core EF-free)

---

## 7. WHAT'S NEXT (Remaining Phases)

### Phase 4: Finance & Settlements
- Wallet, Settlements, Expenses, PaymentRequests
- Distributed locks for financial operations
- Cash alert system enhancements

### Phase 5: Communication & Notifications
- Firebase push, Email, WhatsApp
- SignalR hubs (Notification, CashAlert, Chat)
- Redis backplane for SignalR

### Phase 6: Location & Vehicles
- Route optimization algorithms
- LocationHistory (keyset pagination)
- Real-time GPS tracking
- Vehicle management

### Phase 7: Intelligence & Analytics
- Customer Interest Engine
- Statistics & Analytics
- Challenges & gamification

### Phase 8: Admin & System
- Full admin dashboard
- Audit logs
- Webhook system

### Phase 9: Social & Extras
- SOS & safety
- Community features (road reports, savings circles)
- Colleague radar

### Phase 10: Infrastructure
- Subscriptions & plans
- OCR implementation
- Search system

---

## 8. FRONTEND API DOCS

| File | Module | Endpoints | Lines |
|------|--------|-----------|-------|
| `docs/AUTH_API.md` | Auth & Identity | 15 | ~1000 |
| `docs/ORDERS_API.md` | Orders & Delivery | 64+ | ~3700 |
| `docs/CUSTOMERS_PARTNERS_API.md` | Customers & Partners | 57 | ~3600 |
