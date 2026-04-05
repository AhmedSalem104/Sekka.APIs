using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sekka.API.Middleware;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Infrastructure;
using Sekka.Infrastructure.Repositories;
using Sekka.Persistence;
using Sekka.Persistence.Entities;
using Sekka.Persistence.Interceptors;
using Sekka.Application.Services;
using Sekka.AdminControlDashboard.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════
// 1. Database + Identity
// ══════════════════════════════════════════════════════════════
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<SekkaDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
});

builder.Services.AddIdentity<Driver, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<SekkaDbContext>()
    .AddDefaultTokenProviders();

// ══════════════════════════════════════════════════════════════
// 2. JWT Authentication
// ══════════════════════════════════════════════════════════════
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };

    // SignalR JWT: Token from Query String
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

// ══════════════════════════════════════════════════════════════
// 3. Redis (Cache + SignalR Backplane)
// — Uncomment when Redis is configured —
// ══════════════════════════════════════════════════════════════
// var redisConnection = builder.Configuration["Redis:ConnectionString"]!;
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = redisConnection;
//     options.InstanceName = builder.Configuration["Redis:InstanceName"];
// });
// builder.Services.AddSignalR().AddStackExchangeRedis(redisConnection);

builder.Services.AddSignalR();

// ══════════════════════════════════════════════════════════════
// 4. AutoMapper
// ══════════════════════════════════════════════════════════════
builder.Services.AddAutoMapper(cfg => { },
    typeof(Sekka.Core.Mapping.MappingProfile).Assembly,
    typeof(Sekka.Application.Mapping.ApplicationMappingProfile).Assembly);

// ══════════════════════════════════════════════════════════════
// 5. Repository + UnitOfWork
// ══════════════════════════════════════════════════════════════
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ══════════════════════════════════════════════════════════════
// 6. Application Services (Phase 1 — Auth & Identity)
// ══════════════════════════════════════════════════════════════
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Auth & Identity
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<ISmsService, SmsService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IDriverPreferencesService, DriverPreferencesService>();
builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IDataPrivacyService, DataPrivacyService>();
builder.Services.AddScoped<IDemoService, DemoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IHealthScoreService, HealthScoreService>();

// Admin Services
builder.Services.AddScoped<IAdminDriversService, AdminDriversService>();
builder.Services.AddScoped<IAdminRolesService, AdminRolesService>();
builder.Services.AddScoped<IAdminSubscriptionsService, AdminSubscriptionsService>();

// Phase 2 — Orders & Delivery Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICancellationService, CancellationService>();
builder.Services.AddScoped<IOrderTransferService, OrderTransferService>();
builder.Services.AddScoped<IBulkImportService, BulkImportService>();
builder.Services.AddScoped<IDuplicateDetectionService, DuplicateDetectionService>();
builder.Services.AddScoped<IOrderWorthService, OrderWorthService>();
builder.Services.AddScoped<IAddressSwapService, AddressSwapService>();
builder.Services.AddScoped<IWaitingTimerService, WaitingTimerService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IRecurringOrderService, RecurringOrderService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<ITimelineService, TimelineService>();
builder.Services.AddScoped<ITrackingLinkService, TrackingLinkService>();
builder.Services.AddScoped<IAutoAssignmentService, AutoAssignmentService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<ISmartAddressService, SmartAddressService>();
builder.Services.AddScoped<IVoiceMemoService, VoiceMemoService>();
builder.Services.AddScoped<IOrderSourceService, OrderSourceService>();

// Phase 3 — Customers & Partners Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICallerIdService, CallerIdService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IPickupPointService, PickupPointService>();
builder.Services.AddScoped<IPartnerPortalService, PartnerPortalService>();

// Phase 4 — Financial Services
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ICashSafetyService, CashSafetyService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IPaymentRequestService, PaymentRequestService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IDisputeService, DisputeService>();
builder.Services.AddScoped<ISurgePricingService, SurgePricingService>();

// Phase 5 — Communication & Voice Services
builder.Services.AddHttpClient<ISpeechToTextService, AzureSpeechService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<ISOSService, SOSService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageTemplateService, MessageTemplateService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<INotificationDispatchService, NotificationDispatchService>();

// Phase 6 — Location & Vehicles Services
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IBreakService, BreakService>();
builder.Services.AddScoped<IAdminVehicleService, AdminVehicleService>();

// Phase 7 — Intelligence Services
builder.Services.AddScoped<IInterestEngineService, InterestEngineService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<ICustomerProfilingService, CustomerProfilingService>();
builder.Services.AddScoped<ISegmentationService, SegmentationService>();
builder.Services.AddScoped<IBehaviorAnalysisService, BehaviorAnalysisService>();
builder.Services.AddScoped<ICampaignTargetingService, CampaignTargetingService>();

// Phase 8 — Admin & System Services
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddScoped<IAppConfigService, AppConfigService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Phase 9 — Social & Extras Services
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ISavingsCircleService, SavingsCircleService>();
builder.Services.AddScoped<IColleagueRadarService, ColleagueRadarService>();
builder.Services.AddScoped<IRoadReportService, RoadReportService>();

// ══════════════════════════════════════════════════════════════
// 7. Background Services
// ══════════════════════════════════════════════════════════════
builder.Services.AddHostedService<Sekka.Application.BackgroundServices.StaleOrderCleanupService>();
builder.Services.AddHostedService<Sekka.Application.BackgroundServices.CashAlertBackgroundService>();
builder.Services.AddHostedService<Sekka.Application.BackgroundServices.DailyStatisticsService>();
builder.Services.AddHostedService<Sekka.Application.BackgroundServices.RoadReportCleanupService>();
builder.Services.AddHostedService<Sekka.Application.BackgroundServices.HelpRequestExpiryService>();

// ══════════════════════════════════════════════════════════════
// 8. Rate Limiting
// ══════════════════════════════════════════════════════════════
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("OtpLimiter", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(60);
        opt.SegmentsPerWindow = 6;
    });
    options.AddFixedWindowLimiter("ApiLimiter", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

// ══════════════════════════════════════════════════════════════
// 9. Health Checks
// ══════════════════════════════════════════════════════════════
builder.Services.AddHealthChecks();
// .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "db", "critical" })
// .AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "critical" });

// ══════════════════════════════════════════════════════════════
// 10. API Versioning
// ══════════════════════════════════════════════════════════════
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ══════════════════════════════════════════════════════════════
// 11. Response Compression
// ══════════════════════════════════════════════════════════════
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);

// ══════════════════════════════════════════════════════════════
// 12. CORS
// ══════════════════════════════════════════════════════════════
var corsOrigins = builder.Configuration.GetSection("CorsPolicy:AllowedOrigins").Get<string[]>()
    ?? new[] { "https://admin.sekka.app", "https://sekka.app" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ══════════════════════════════════════════════════════════════
// 13. FluentValidation
// ══════════════════════════════════════════════════════════════
builder.Services.AddValidatorsFromAssemblyContaining<Sekka.Core.Validators.Auth.SendOtpDtoValidator>();

// ══════════════════════════════════════════════════════════════
// 14. Swagger + Controllers
// ══════════════════════════════════════════════════════════════
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sekka API",
        Version = "v1",
        Description = "Smart Delivery Management Platform"
    });

    // Include XML comments in Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token only (without Bearer prefix)"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ══════════════════════════════════════════════════════════════
// Build + Middleware Pipeline
// ══════════════════════════════════════════════════════════════
var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

// ══════════════ Middleware Pipeline (order matters!) ══════════════

// Phase 1: Custom Middleware
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<LocaleNormalizationMiddleware>();
app.UseMiddleware<MaintenanceMiddleware>();

// Phase 2: Compression + Security Headers
app.UseResponseCompression();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(self)");

    // Relaxed CSP for Swagger UI (inline scripts/styles); strict for all other paths
    var path = context.Request.Path;
    if (path.StartsWithSegments("/swagger"))
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; img-src 'self' data:");
    else
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");

    await next();
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
});

// Phase 3: ASP.NET Core Built-in Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve uploaded files from App_Data/uploads (persists across publish)
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "App_Data", "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");

// SignalR Hubs
app.MapHub<Sekka.API.Hubs.OrderTrackingHub>("/hubs/tracking");
app.MapHub<Sekka.API.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<Sekka.API.Hubs.CashAlertHub>("/hubs/cash-alerts");
app.MapHub<Sekka.API.Hubs.ChatHub>("/hubs/chat");

app.Run();
