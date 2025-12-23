using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookM.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketTypeId",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TicketTypeId",
                table: "Booking",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_TicketType_TicketTypeId",
                table: "Booking",
                column: "TicketTypeId",
                principalTable: "TicketType",
                principalColumn: "TicketTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_TicketType_TicketTypeId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TicketTypeId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "Booking");
        }
    }
}
