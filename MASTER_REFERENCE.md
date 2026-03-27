# Sekka.API — Master Implementation Reference
# Version: 11.0 | Date: 2026-03-27 | Status: Phase 9 Complete
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

### Current Implementation Status (Phases 0–8)

| Phase | Status | Tables | Controllers | Endpoints | Services | Enums | DTOs |
|-------|--------|--------|-------------|-----------|----------|-------|------|
| 0 — Foundation | COMPLETE | — | 1 (Base) | — | 1 (Base) | — | 2 |
| 1 — Auth & Identity | COMPLETE | 15 | 12 | 84 | 14 | 12 | 80+ |
| 2 — Orders & Delivery | COMPLETE | 12 | 9 | 72+ | 18 | 13 | 70+ |
| 3 — Customers & Partners | COMPLETE | 8 | 9 | 57 | 6 | 5 | 40+ |
| 4 — Financial | COMPLETE | 10 | 13 | 82+ | 10 | 14 | 80+ |
| 5 — Communication | COMPLETE | 7 | 6 | 33 | 7 | 6 | 35+ |
| 6 — Location & Vehicles | COMPLETE | 6 | 4 | 26 | 4 | 3 | 25+ |
| 7 — Intelligence | COMPLETE | 9 | 4 | 37 | 6 | 9 | 50+ |
| 8 — Admin & System | COMPLETE | 9 | 5 | 40 | 3 | 4 | 40+ |
| 9 — Social & Extras | COMPLETE | 11 | 8 | 47 | 6 | 9 | 35+ |
| **TOTAL** | **9 Phases** | **87** | **71** | **478+** | **75** | **75** | **455+** |

- 9 Migrations applied (Phase1-9)
- 4 Background services active (StaleOrderCleanup, CashAlert, DailyStatistics, RoadReportCleanup)
- 3 Background services active (StaleOrderCleanup, CashAlert, DailyStatistics)
- 4 SignalR Hubs active (OrderTracking, Notification, CashAlert, Chat)
- Centralized message system: 79+ ErrorMessages, 91+ SuccessMessages
- Frontend docs: AUTH_API.md, ORDERS_API.md, CUSTOMERS_PARTNERS_API.md, FINANCIAL_API.md, COMMUNICATION_API.md, LOCATION_VEHICLES_API.md, INTELLIGENCE_API.md, ADMIN_SYSTEM_API.md, SOCIAL_EXTRAS_API.md

---

## 2. ARCHITECTURE STRUCTURE

### Layer Dependency Graph
```
Core → Persistence → Infrastructure → Application → API
                                                    ↗
                               AdminControlDashboard
```

### Layer Details (Current — Phase 4)

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
Enums/ (44 total)
  Phase 1: VehicleType, PhoneType, ThemeMode, MapApp, DevicePlatform,
           DeletionRequestStatus, NotificationType, NotificationPriority,
           ShiftStatus, SubscriptionStatus, OrderStatus, ChallengeType
  Phase 2: PaymentMethod, OrderPriority, OrderSourceType, DeliveryFailReason,
           PhotoType, CancellationReason, TransferStatus, SourceTagType,
           SyncOperation, SyncStatus, DuplicateAction, AssignmentStrategy,
           TimelineEventType
  Phase 3: AddressType, ContactType, PartnerType, CommissionType, VerificationStatus
  Phase 4: TransactionType, SettlementType, ExpenseType, PaymentPurpose,
           ManualPaymentMethod, PaymentRequestStatus, DisputeType, DisputeStatus,
           InvoiceStatus, RefundReason, RefundStatus, SurgeTrigger,
           WalletAdjustmentType, WalletFreezeReason
Interfaces/
  ├── Services/     ← 44 interfaces (13 Phase1 + 15 Phase2 + 6 Phase3 + 10 Phase4)
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
Entities/ (37 + 3 base = 40 entity classes)
  ├── Base/         ← BaseEntity<TKey>, AuditableEntity<TKey>, SoftDeletableEntity<TKey>
  Phase 1 (7):
  ├── Driver.cs, DriverPreferences.cs, NotificationChannelPreference.cs,
  │   AccountDeletionRequest.cs, ActiveSession.cs, UserConsent.cs, DataDeletionRequest.cs
  Phase 2 (12):
  ├── Order.cs, DeliveryAttempt.cs, OrderPhoto.cs, AddressSwapLog.cs,
  │   CancellationLog.cs, OrderSourceTag.cs, WaitingTimer.cs, OrderTransferLog.cs,
  │   VoiceMemo.cs, SyncQueue.cs, TrackingLink.cs, DeliveryTimeSlot.cs
  Phase 3 (8):
  ├── Customer.cs, Address.cs, Rating.cs, CallerIdNote.cs, BlockedCustomer.cs,
  │   Partner.cs, PickupPoint.cs, CommunityBlacklist.cs
  Phase 4 (10):
  └── WalletTransaction.cs, Settlement.cs, Expense.cs, PaymentRequest.cs,
      DailyStats.cs, OrderDispute.cs, Invoice.cs, InvoiceItem.cs,
      RefundRequest.cs, SurgePricingRule.cs

Configurations/ (37 files — one per entity)
Interceptors/       ← AuditInterceptor (SaveChangesInterceptor)
Migrations/
  ├── Phase1_AuthIdentity (15 tables)
  ├── Phase2_OrdersDelivery (12 tables)
  ├── Phase3_CustomersPartners (8 tables)
  └── Phase4_Financial (10 tables)
SekkaDbContext.cs   ← 45 DbSets, global soft-delete filter
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
Services/ (36 service implementations)
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
  Phase 4 (10): WalletService, CashSafetyService, SettlementService,
                StatisticsService, PaymentRequestService, AnalyticsService,
                InvoiceService, RefundService, DisputeService, SurgePricingService
BackgroundServices/
  ├── StaleOrderCleanupService    ← Every 5 min (cancel pending > 30 min)
  ├── CashAlertBackgroundService  ← Every 10 min (check cash thresholds)
  └── DailyStatisticsService      ← Daily 11 PM (aggregate daily stats)
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
Controllers/ (44 total)
  ├── Base/BaseCrudController.cs
  ├── Driver/ (26 controllers)
  │   Phase 1: AuthController (15), ProfileController (20), SettingsController (10),
  │            DataPrivacyController (5), BadgeController (2), HealthScoreController (2),
  │            DemoController (4), LookupsController (1)
  │   Phase 2: OrderController (27), RouteController (5), RecurringOrderController (6),
  │            SyncController (4), TrackingController (1), TimelineController (3), OcrController (3)
  │   Phase 3: CustomerController (11), CallerIdController (5), AddressController (6),
  │            PartnerController (8), PickupPointController (4), PartnerPortalController (6)
  │   Phase 4: WalletController (4), SettlementController (5), StatisticsController (5),
  │            PaymentRequestController (5), InvoiceController (4), AnalyticsController (6)
  └── Admin/ (17 controllers)
      Phase 1: AdminDriversController (6), AdminRolesController (7),
               AdminSubscriptionsController (13)
      Phase 2: AdminOrdersController (10), AdminTimeSlotsController (6)
      Phase 3: AdminCustomersController (5), AdminPartnersController (8),
               AdminBlacklistController (4)
      Phase 4: AdminSettlementsController (5), AdminStatisticsController (20),
               AdminPaymentController (5), AdminWalletController (10),
               AdminDisputesController (7), AdminInvoiceController (7),
               AdminRefundController (6)
Hubs/
  ├── OrderTrackingHub.cs    ← /hubs/tracking
  ├── NotificationHub.cs     ← /hubs/notifications
  └── CashAlertHub.cs        ← /hubs/cash-alerts
Middleware/ (4)
  ├── GlobalExceptionHandler.cs
  ├── RequestLoggingMiddleware.cs
  ├── LocaleNormalizationMiddleware.cs
  └── MaintenanceMiddleware.cs
Program.cs ← Full DI (49 services + 3 background + 3 hubs)
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

## 4. DATABASE (45 Tables)

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

### Phase 4 — Financial (10 tables)
```
WalletTransactions, Settlements, Expenses, PaymentRequests (UNIQUE: ReferenceCode),
DailyStats (UNIQUE: DriverId+Date), OrderDisputes, Invoices (UNIQUE: InvoiceNumber),
InvoiceItems, RefundRequests, SurgePricingRules
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
12. Phase 4 Services (10): Wallet, CashSafety, Settlement, Statistics, PaymentRequest,
    Analytics, Invoice, Refund, Dispute, SurgePricing
13. Background Services (3): StaleOrderCleanup, CashAlert, DailyStatistics
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
14. MapControllers → 15. /health → 16. /hubs/tracking, /hubs/notifications, /hubs/cash-alerts
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
| `docs/FINANCIAL_API.md` | Financial | 82+ | ~3500+ |
