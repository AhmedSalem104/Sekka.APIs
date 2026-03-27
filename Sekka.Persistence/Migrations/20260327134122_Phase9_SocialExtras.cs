using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase9_SocialExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ChallengeType = table.Column<int>(type: "int", nullable: false),
                    TargetMetric = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    BadgeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BadgeIconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldAssistanceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestingDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssistingDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldAssistanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldAssistanceRequests_AspNetUsers_AssistingDriverId",
                        column: x => x.AssistingDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldAssistanceRequests_AspNetUsers_RequestingDriverId",
                        column: x => x.RequestingDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferrerDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferredDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferralCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferredPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RewardType = table.Column<int>(type: "int", nullable: false),
                    RewardGiven = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReferrerRewardGiven = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RewardedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referrals_AspNetUsers_ReferredDriverId",
                        column: x => x.ReferredDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrals_AspNetUsers_ReferrerDriverId",
                        column: x => x.ReferrerDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    ConfirmationsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadReports_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsCircles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MonthlyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxMembers = table.Column<int>(type: "int", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MinHealthScore = table.Column<int>(type: "int", nullable: false, defaultValue: 80),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsCircles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingsCircles_AspNetUsers_CreatorDriverId",
                        column: x => x.CreatorDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PriceMonthly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceAnnual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxOrdersPerDay = table.Column<int>(type: "int", nullable: true),
                    HistoryDays = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverAchievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PointsEarned = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverAchievements_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverAchievements_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadReportConfirmations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadReportConfirmations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadReportConfirmations_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadReportConfirmations_RoadReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "RoadReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavingsCircleMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsCircleMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingsCircleMembers_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavingsCircleMembers_SavingsCircles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "SavingsCircles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PaymentRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_PaymentRequests_PaymentRequestId",
                        column: x => x.PaymentRequestId,
                        principalTable: "PaymentRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsCirclePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsCirclePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingsCirclePayments_SavingsCircleMembers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "SavingsCircleMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavingsCirclePayments_SavingsCircles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "SavingsCircles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengeType",
                table: "Challenges",
                column: "ChallengeType");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_IsActive",
                table: "Challenges",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DriverAchievements_ChallengeId",
                table: "DriverAchievements",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverAchievements_DriverId_ChallengeId",
                table: "DriverAchievements",
                columns: new[] { "DriverId", "ChallengeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldAssistanceRequests_AssistingDriverId",
                table: "FieldAssistanceRequests",
                column: "AssistingDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldAssistanceRequests_RequestingDriverId",
                table: "FieldAssistanceRequests",
                column: "RequestingDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldAssistanceRequests_Status",
                table: "FieldAssistanceRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferralCode",
                table: "Referrals",
                column: "ReferralCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferredDriverId",
                table: "Referrals",
                column: "ReferredDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferrerDriverId",
                table: "Referrals",
                column: "ReferrerDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadReportConfirmations_DriverId",
                table: "RoadReportConfirmations",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadReportConfirmations_ReportId_DriverId",
                table: "RoadReportConfirmations",
                columns: new[] { "ReportId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadReports_DriverId",
                table: "RoadReports",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadReports_ExpiresAt",
                table: "RoadReports",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RoadReports_IsActive",
                table: "RoadReports",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RoadReports_Type",
                table: "RoadReports",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCircleMembers_CircleId_DriverId",
                table: "SavingsCircleMembers",
                columns: new[] { "CircleId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCircleMembers_DriverId",
                table: "SavingsCircleMembers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCirclePayments_CircleId_MemberId_RoundNumber",
                table: "SavingsCirclePayments",
                columns: new[] { "CircleId", "MemberId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCirclePayments_MemberId",
                table: "SavingsCirclePayments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCircles_CreatorDriverId",
                table: "SavingsCircles",
                column: "CreatorDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsCircles_Status",
                table: "SavingsCircles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_IsActive",
                table: "SubscriptionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_SortOrder",
                table: "SubscriptionPlans",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_DriverId",
                table: "Subscriptions",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_DriverId_Status",
                table: "Subscriptions",
                columns: new[] { "DriverId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PaymentRequestId",
                table: "Subscriptions",
                column: "PaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverAchievements");

            migrationBuilder.DropTable(
                name: "FieldAssistanceRequests");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "RoadReportConfirmations");

            migrationBuilder.DropTable(
                name: "SavingsCirclePayments");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "RoadReports");

            migrationBuilder.DropTable(
                name: "SavingsCircleMembers");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "SavingsCircles");
        }
    }
}
