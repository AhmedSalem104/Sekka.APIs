using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ColleagueRadar_AddOrderAndPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverPhone",
                table: "FieldAssistanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "FieldAssistanceRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FieldAssistanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FieldAssistanceRequests_OrderId",
                table: "FieldAssistanceRequests",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldAssistanceRequests_Orders_OrderId",
                table: "FieldAssistanceRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldAssistanceRequests_Orders_OrderId",
                table: "FieldAssistanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_FieldAssistanceRequests_OrderId",
                table: "FieldAssistanceRequests");

            migrationBuilder.DropColumn(
                name: "DriverPhone",
                table: "FieldAssistanceRequests");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "FieldAssistanceRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FieldAssistanceRequests");
        }
    }
}
