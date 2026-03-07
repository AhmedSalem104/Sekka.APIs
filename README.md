<div align="center">

<img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
<img src="https://img.shields.io/badge/Architecture-Clean-00C853?style=for-the-badge&logo=buffer&logoColor=white" alt="Clean Architecture"/>
<img src="https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
<img src="https://img.shields.io/badge/Cache-Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis"/>
<img src="https://img.shields.io/badge/Realtime-SignalR-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="SignalR"/>
<img src="https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT"/>

<br/><br/>

# سكّة — Sekka.API

### Smart Delivery Management Platform

<p align="center">
  <strong>A comprehensive backend system for intelligent delivery operations in Egypt</strong>
</p>

<p align="center">
  <a href="#architecture">Architecture</a> •
  <a href="#tech-stack">Tech Stack</a> •
  <a href="#project-structure">Structure</a> •
  <a href="#getting-started">Getting Started</a> •
  <a href="#api-overview">API Overview</a> •
  <a href="#patterns">Patterns</a>
</p>

---

<table>
<tr>
<td align="center"><strong>579+</strong><br/>Endpoints</td>
<td align="center"><strong>83+</strong><br/>Services</td>
<td align="center"><strong>82</strong><br/>DB Tables</td>
<td align="center"><strong>6</strong><br/>Layers</td>
<td align="center"><strong>4</strong><br/>SignalR Hubs</td>
<td align="center"><strong>11</strong><br/>Background Services</td>
</tr>
</table>

</div>

---

## Architecture

Built on **Clean Architecture** principles with strict layer separation and SOLID compliance.

```
┌─────────────────────────────────────────────────────────┐
│                      Sekka.API                          │
│            Controllers • Hubs • Middleware               │
├──────────────────────┬──────────────────────────────────┤
│  AdminControlDashboard│         Sekka.Application        │
│    Admin Services     │    Business Logic • Services     │
├──────────────────────┴──────────────────────────────────┤
│                   Sekka.Infrastructure                   │
│              Repository • Unit of Work                   │
├─────────────────────────────────────────────────────────┤
│                    Sekka.Persistence                     │
│          Entities • DbContext • Configurations           │
├─────────────────────────────────────────────────────────┤
│                      Sekka.Core                          │
│       Interfaces • DTOs • Specifications • Validators    │
└─────────────────────────────────────────────────────────┘
```

### Dependency Flow

```
Core ← Persistence ← Infrastructure ← Application ← API
                                                     ↗
                                  AdminControlDashboard
```

> **Core** has zero dependencies. Each layer only depends on the layers below it.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | .NET 8.0 Web API |
| **Database** | SQL Server + EF Core (Code-First) |
| **Authentication** | JWT + ASP.NET Identity (OTP-based) |
| **Realtime** | SignalR + Redis Backplane |
| **Caching** | Redis (OTP, JWT Blacklist, Idempotency, Output Cache) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **API Versioning** | URL Segment + Header (`X-Api-Version`) |
| **Compression** | Brotli + Gzip |
| **Documentation** | Swagger / OpenAPI |
| **Mobile Client** | Flutter (Dart) |

---

## Project Structure

```
Sekka.API.sln
│
├── Sekka.Core/                          # Contracts & Abstractions
│   ├── Common/                          # Result<T>, ApiResponse<T>, PagedResult<T>
│   │                                    # EgyptianPhoneHelper
│   ├── DTOs/                            # 452+ Data Transfer Objects
│   ├── Enums/                           # 82+ Enumerations
│   ├── Interfaces/
│   │   ├── Services/                    # Service contracts
│   │   └── Persistence/                 # IGenericRepository, IUnitOfWork
│   ├── Specifications/                  # Query specifications
│   ├── Validators/                      # FluentValidation rules
│   └── Mapping/                         # AutoMapper profiles
│
├── Sekka.Persistence/                   # Data Layer
│   ├── Entities/
│   │   └── Base/                        # BaseEntity → AuditableEntity → SoftDeletableEntity
│   ├── Configurations/                  # EF Core Fluent API
│   ├── Interceptors/                    # AuditInterceptor
│   ├── Seeds/                           # JSON seed data
│   └── SekkaDbContext.cs
│
├── Sekka.Infrastructure/                # Data Access
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   └── SpecificationEvaluator.cs
│   └── UnitOfWork.cs
│
├── Sekka.Application/                   # Business Logic
│   ├── Services/
│   │   └── Base/                        # BaseService<TEntity, TDto, ...>
│   └── BackgroundServices/              # 11 Hosted Services
│
├── Sekka.AdminControlDashboard/         # Admin Operations
│   └── Services/                        # 25+ Admin Services
│
└── Sekka.API/                           # Presentation Layer
    ├── Controllers/
    │   ├── Base/                        # BaseCrudController<T>
    │   ├── Driver/                      # 43 Driver Controllers
    │   └── Admin/                       # 25 Admin Controllers
    ├── Hubs/                            # 4 SignalR Hubs
    ├── Middleware/                       # 5 Custom Middleware
    └── Program.cs                       # DI + Pipeline Configuration
```

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)
- [Redis](https://redis.io/download) (optional for development)

### Setup

```bash
# Clone the repository
git clone https://github.com/AhmedSalem104/Sekka.APIs.git
cd Sekka.APIs

# Restore packages
dotnet restore

# Update connection string in appsettings.Development.json

# Run migrations
dotnet ef database update -p Sekka.Persistence -s Sekka.API

# Run the application
dotnet run --project Sekka.API
```

The API will be available at `https://localhost:5001` with Swagger UI.

---

## API Overview

### Driver APIs (43 Controllers)

| Category | Endpoints | Description |
|----------|-----------|-------------|
| **Auth** | OTP Send/Verify, Register, Refresh, Logout | Phone-based OTP authentication |
| **Orders** | CRUD, Deliver, Cancel, Transfer, Bulk Import | Full order lifecycle management |
| **Routes** | Optimize, Reorder, Add/Remove | Smart route optimization |
| **Customers** | CRUD, Merge, Smart Address, Caller ID | Customer relationship management |
| **Finance** | Wallet, Settlements, Payments, Invoices | Financial operations with distributed locks |
| **Partners** | CRUD, Pickup Points, Portal | Partner management |
| **Analytics** | Stats, Trends, Heatmaps, Timeline | Deep analytics & reporting |
| **Gamification** | Challenges, Badges, Referrals, Health Score | Driver engagement system |
| **Communication** | Chat, Notifications, Voice Memos | Multi-channel communication |

### Admin APIs (25 Controllers)

Full dashboard management for drivers, orders, settlements, partners, customers, subscriptions, vehicles, SOS alerts, disputes, refunds, segments, campaigns, and more.

### Realtime (4 SignalR Hubs)

| Hub | Path | Purpose |
|-----|------|---------|
| **OrderTracking** | `/hubs/tracking` | Live order status & driver location |
| **Notification** | `/hubs/notifications` | Push notifications & broadcasts |
| **CashAlert** | `/hubs/cash-alerts` | Financial safety alerts |
| **Chat** | `/hubs/chat` | Direct messaging |

---

## Patterns

### Entity Hierarchy

```csharp
BaseEntity<TKey>              // Id, CreatedAt
  └─ AuditableEntity<TKey>    // + UpdatedAt, CreatedBy, ModifiedBy
      └─ SoftDeletableEntity<TKey>  // + IsDeleted, DeletedAt, DeletedBy
```

### Result Pattern

```csharp
// Services return Result<T> instead of throwing exceptions
public async Task<Result<OrderDto>> GetByIdAsync(Guid id)
{
    var order = await _repo.GetByIdAsync(id);
    if (order is null)
        return Result<OrderDto>.NotFound("Order not found");
    return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
}
```

### Specification Pattern

```csharp
// Reusable, testable query specifications
public class ActiveOrdersSpec : BaseSpecification<Order>
{
    public ActiveOrdersSpec(Guid driverId, int page, int pageSize)
    {
        SetCriteria(o => o.DriverId == driverId && o.Status != OrderStatus.Delivered);
        AddInclude(o => o.Customer!);
        SetOrderByDescending(o => o.CreatedAt);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}
```

### Generic Base Service & Controller

```csharp
// BaseService provides CRUD out of the box
public class OrderService : BaseService<Order, OrderDto, CreateOrderDto, UpdateOrderDto>

// BaseCrudController maps Result<T> to proper HTTP responses
public class OrdersController : BaseCrudController<Order, OrderDto, CreateOrderDto, UpdateOrderDto>
```

---

## Middleware Pipeline

```
Request
  │
  ├─ 1. GlobalExceptionHandler      — Catches unhandled exceptions
  ├─ 2. RequestLoggingMiddleware     — Structured logging (Serilog)
  ├─ 3. LocaleNormalizationMiddleware— Arabic/Hindi digits → ASCII
  ├─ 4. MaintenanceMiddleware        — Maintenance window check
  ├─ 5. ResponseCompression          — Brotli / Gzip
  ├─ 6. Security Headers             — CSP, X-Frame-Options, etc.
  ├─ 7. CORS                         — Restricted origins
  ├─ 8. Authentication               — JWT validation
  ├─ 9. Authorization                — Role-based access
  ├─ 10. Rate Limiter                — OTP: 5/hr, API: 100/min
  └─ 11. Controllers / Hubs          — Request handling
```

---

## Security

- **JWT** with token rotation (access + refresh tokens via Redis)
- **OTP** authentication: 4-digit, 5-min expiry, rate-limited
- **Distributed Locks** (Redlock) for financial operations
- **Idempotency Keys** for payment and order creation
- **Soft Delete** for financial/historical data integrity
- **Phone Normalization** — All phones stored as `+201XXXXXXXXX`
- **Security Headers** — CSP, X-Frame-Options, XSS Protection
- **CORS** — Restricted to known origins
- **Audit Trail** — Auto-tracked via `AuditInterceptor`

---

## Background Services

| Service | Schedule | Purpose |
|---------|----------|---------|
| StaleOrderCleanup | Every 5 min | Clean up unaccepted orders |
| CashAlert | Every 10 min | Monitor driver cash thresholds |
| DailyStatistics | Daily 11 PM | Aggregate daily stats |
| RecurringOrder | Daily 6 AM | Generate recurring orders |
| WebhookDispatch | Every 15 min | Dispatch pending webhooks |
| DemoCleanup | Every 30 min | Clean expired demo sessions |
| RoadReportCleanup | Every 15 min | Expire old road reports |
| MaintenanceChecker | Every 1 min | Check maintenance windows |
| AccountDeletion | Daily 2 AM | Process account deletions |
| InterestSignalProcessor | Every 15 min | Process customer interest signals |
| SegmentRefresh | Daily 11 PM | Recalculate customer segments |

---

## Customer Interest Engine

```
Signal Collection → Batch Scoring (15 min intervals)
       │
       ├─ Time Decay (factor: 0.95 per 7 days)
       ├─ RFM Analysis (Recency / Frequency / Monetary)
       ├─ Smart Segmentation (daily refresh)
       └─ Targeted Campaigns + Personalized Recommendations
```

---

<div align="center">

**Built with Clean Architecture, SOLID principles, and production-grade patterns.**

<br/>

<img src="https://img.shields.io/badge/Status-In%20Development-yellow?style=flat-square" alt="Status"/>
<img src="https://img.shields.io/badge/License-Proprietary-red?style=flat-square" alt="License"/>
<img src="https://img.shields.io/badge/Market-Egypt-green?style=flat-square" alt="Market"/>

</div>
