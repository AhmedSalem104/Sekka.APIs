using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_OrdersDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryTimeSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentBookings = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriceMultiplier = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTimeSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    PickupAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PickupLatitude = table.Column<double>(type: "float", nullable: true),
                    PickupLongitude = table.Column<double>(type: "float", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeliveryLatitude = table.Column<double>(type: "float", nullable: true),
                    DeliveryLongitude = table.Column<double>(type: "float", nullable: true),
                    DistanceKm = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    TimeWindowStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeWindowEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SequenceIndex = table.Column<int>(type: "int", nullable: true),
                    ExpectedChangeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ActualCollectedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ReturnReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PartialDeliveryNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    WorthScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SyncQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ConflictResolution = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncQueues_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressSwapLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OldLatitude = table.Column<double>(type: "float", nullable: true),
                    OldLongitude = table.Column<double>(type: "float", nullable: true),
                    NewAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NewLatitude = table.Column<double>(type: "float", nullable: true),
                    NewLongitude = table.Column<double>(type: "float", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DistanceDifferenceKm = table.Column<double>(type: "float", nullable: true),
                    TimeDifferenceMinutes = table.Column<int>(type: "int", nullable: true),
                    CostDifference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SwappedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressSwapLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressSwapLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CancellationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancellationReason = table.Column<int>(type: "int", nullable: false),
                    ReasonText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DistanceTravelledKm = table.Column<double>(type: "float", nullable: true),
                    TimeLostMinutes = table.Column<int>(type: "int", nullable: true),
                    FuelCostLost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DocumentationUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransferredToDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationLogs_AspNetUsers_TransferredToDriverId",
                        column: x => x.TransferredToDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CancellationLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    AutoMessageSent = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAttempts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhotoType = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    TakenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPhotos_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSourceTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSourceTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSourceTags_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTransferLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransferReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeepLinkToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransferredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTransferLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTransferLogs_AspNetUsers_FromDriverId",
                        column: x => x.FromDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTransferLogs_AspNetUsers_ToDriverId",
                        column: x => x.ToDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrderTransferLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackingLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackingLinks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceMemos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Transcription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceMemos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceMemos_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoiceMemos_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WaitingTimers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingTimers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingTimers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressSwapLogs_OrderId",
                table: "AddressSwapLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationLogs_OrderId",
                table: "CancellationLogs",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationLogs_TransferredToDriverId",
                table: "CancellationLogs",
                column: "TransferredToDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAttempts_OrderId_AttemptNumber",
                table: "DeliveryAttempts",
                columns: new[] { "OrderId", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTimeSlots_Date_StartTime_EndTime_RegionId",
                table: "DeliveryTimeSlots",
                columns: new[] { "Date", "StartTime", "EndTime", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPhotos_OrderId",
                table: "OrderPhotos",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverId",
                table: "Orders",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverId_CreatedAt",
                table: "Orders",
                columns: new[] { "DriverId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverId_Status",
                table: "Orders",
                columns: new[] { "DriverId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdempotencyKey",
                table: "Orders",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSourceTags_OrderId",
                table: "OrderSourceTags",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransferLogs_DeepLinkToken",
                table: "OrderTransferLogs",
                column: "DeepLinkToken",
                unique: true,
                filter: "[DeepLinkToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransferLogs_FromDriverId",
                table: "OrderTransferLogs",
                column: "FromDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransferLogs_OrderId",
                table: "OrderTransferLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransferLogs_ToDriverId",
                table: "OrderTransferLogs",
                column: "ToDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncQueues_DriverId_Status",
                table: "SyncQueues",
                columns: new[] { "DriverId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLinks_OrderId",
                table: "TrackingLinks",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackingLinks_TrackingCode",
                table: "TrackingLinks",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoiceMemos_DriverId",
                table: "VoiceMemos",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceMemos_OrderId",
                table: "VoiceMemos",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingTimers_OrderId",
                table: "WaitingTimers",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressSwapLogs");

            migrationBuilder.DropTable(
                name: "CancellationLogs");

            migrationBuilder.DropTable(
                name: "DeliveryAttempts");

            migrationBuilder.DropTable(
                name: "DeliveryTimeSlots");

            migrationBuilder.DropTable(
                name: "OrderPhotos");

            migrationBuilder.DropTable(
                name: "OrderSourceTags");

            migrationBuilder.DropTable(
                name: "OrderTransferLogs");

            migrationBuilder.DropTable(
                name: "SyncQueues");

            migrationBuilder.DropTable(
                name: "TrackingLinks");

            migrationBuilder.DropTable(
                name: "VoiceMemos");

            migrationBuilder.DropTable(
                name: "WaitingTimers");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
