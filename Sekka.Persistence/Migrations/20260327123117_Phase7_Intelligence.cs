using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase7_Intelligence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BehaviorPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatternType = table.Column<int>(type: "int", nullable: false),
                    PatternKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PatternValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PatternData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occurrences = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    FirstDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BehaviorPatterns_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BehaviorPatterns_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSegments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegmentType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Rules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    MinScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
                    LastRefreshedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSegments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestCategories_InterestCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSegmentMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExitedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSegmentMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSegmentMembers_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerSegmentMembers_CustomerSegments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "CustomerSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerSegmentMembers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CampaignType = table.Column<int>(type: "int", nullable: false),
                    SegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    TargetCount = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    OpenCount = table.Column<int>(type: "int", nullable: false),
                    ConversionCount = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_CustomerSegments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "CustomerSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SignalCount = table.Column<int>(type: "int", nullable: false),
                    LastSignalAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrendDirection = table.Column<int>(type: "int", nullable: false),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerInterests_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInterests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInterests_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterestSignals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignalType = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestSignals_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterestSignals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterestSignals_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterestSignals_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecommendationType = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinInterestScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MessageTemplateAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationRules_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CustomerRecommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecommendationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelevanceScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false),
                    IsActedUpon = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerRecommendations_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerRecommendations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerRecommendations_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CustomerRecommendations_RecommendationRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "RecommendationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BehaviorPatterns_CustomerId",
                table: "BehaviorPatterns",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BehaviorPatterns_CustomerId_DriverId_PatternType_PatternKey",
                table: "BehaviorPatterns",
                columns: new[] { "CustomerId", "DriverId", "PatternType", "PatternKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BehaviorPatterns_DriverId",
                table: "BehaviorPatterns",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CampaignType",
                table: "CampaignTargets",
                column: "CampaignType");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CategoryId",
                table: "CampaignTargets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_ScheduledAt",
                table: "CampaignTargets",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_SegmentId",
                table: "CampaignTargets",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_Status",
                table: "CampaignTargets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInterests_CategoryId",
                table: "CustomerInterests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInterests_CustomerId",
                table: "CustomerInterests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInterests_CustomerId_DriverId_CategoryId",
                table: "CustomerInterests",
                columns: new[] { "CustomerId", "DriverId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInterests_DriverId",
                table: "CustomerInterests",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_CategoryId",
                table: "CustomerRecommendations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_CustomerId",
                table: "CustomerRecommendations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_DriverId",
                table: "CustomerRecommendations",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_RecommendationType",
                table: "CustomerRecommendations",
                column: "RecommendationType");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_RuleId",
                table: "CustomerRecommendations",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_Status",
                table: "CustomerRecommendations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegmentMembers_CustomerId",
                table: "CustomerSegmentMembers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegmentMembers_DriverId",
                table: "CustomerSegmentMembers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegmentMembers_SegmentId",
                table: "CustomerSegmentMembers",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegmentMembers_SegmentId_CustomerId_DriverId",
                table: "CustomerSegmentMembers",
                columns: new[] { "SegmentId", "CustomerId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegments_Name",
                table: "CustomerSegments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSegments_SegmentType",
                table: "CustomerSegments",
                column: "SegmentType");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCategories_ParentCategoryId",
                table: "InterestCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCategories_SortOrder",
                table: "InterestCategories",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_CategoryId",
                table: "InterestSignals",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_CreatedAt",
                table: "InterestSignals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_CustomerId",
                table: "InterestSignals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_DriverId",
                table: "InterestSignals",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_IsProcessed",
                table: "InterestSignals",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_InterestSignals_OrderId",
                table: "InterestSignals",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationRules_CategoryId",
                table: "RecommendationRules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationRules_IsActive",
                table: "RecommendationRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationRules_RecommendationType",
                table: "RecommendationRules",
                column: "RecommendationType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BehaviorPatterns");

            migrationBuilder.DropTable(
                name: "CampaignTargets");

            migrationBuilder.DropTable(
                name: "CustomerInterests");

            migrationBuilder.DropTable(
                name: "CustomerRecommendations");

            migrationBuilder.DropTable(
                name: "CustomerSegmentMembers");

            migrationBuilder.DropTable(
                name: "InterestSignals");

            migrationBuilder.DropTable(
                name: "RecommendationRules");

            migrationBuilder.DropTable(
                name: "CustomerSegments");

            migrationBuilder.DropTable(
                name: "InterestCategories");
        }
    }
}
