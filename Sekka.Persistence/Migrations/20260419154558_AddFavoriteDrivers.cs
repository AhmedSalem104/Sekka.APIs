using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteDrivers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteDrivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    LinkedDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAppUser = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteDrivers_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FavoriteDrivers_AspNetUsers_LinkedDriverId",
                        column: x => x.LinkedDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDrivers_DriverId_Phone",
                table: "FavoriteDrivers",
                columns: new[] { "DriverId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDrivers_LinkedDriverId",
                table: "FavoriteDrivers",
                column: "LinkedDriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteDrivers");
        }
    }
}
