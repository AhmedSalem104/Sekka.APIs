using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sekka.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviousOrderStatus",
                table: "OrderTransferLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousOrderStatus",
                table: "OrderTransferLogs");
        }
    }
}
