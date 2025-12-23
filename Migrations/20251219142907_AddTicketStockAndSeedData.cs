using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookM.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketStockAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "QuantityAvailable",
                table: "TicketType",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "TicketType",
                columns: new[] { "TicketTypeId", "EventId", "Name", "Price", "QuantityAvailable" },
                values: new object[,]
                {
                    { 1, 1, "Standard", 450.00m, 1000 },
                    { 2, 1, "VIP", 800.00m, 100 },
                    { 3, 1, "Golden Circle", 1200.00m, 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TicketType",
                keyColumn: "TicketTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TicketType",
                keyColumn: "TicketTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TicketType",
                keyColumn: "TicketTypeId",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityAvailable",
                table: "TicketType",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
