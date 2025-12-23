using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookM.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFakeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "EventId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "EventId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "EventId",
                keyValue: 4);

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

            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "EventId",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "EventId", "Capacity", "CategoryId", "Description", "EventDate", "ImageUrl", "Location", "LType", "Title" },
                values: new object[,]
                {
                    { 1, 50000, 2, "The biggest music festival in Morocco returns.", new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.unsplash.com/photo-1459749411177-2a296581dca0?auto=format&fit=crop&w=600&q=80", "OLM Souissi, Rabat", "Outdoor", "Mawazine Festival - Opening Night" },
                    { 2, 45000, 3, "The most anticipated football match of the season.", new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=600&q=80", "Complexe Mohammed V, Casablanca", "Stadium", "Wydad vs Raja - The Derby" },
                    { 3, 200, 1, "Watch the Christopher Nolan masterpiece in IMAX.", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=600&q=80", "Megarama, Casablanca", "Indoor", "Inception - Special Screening" },
                    { 4, 1500, 4, "An evening of laughter with the best comedians.", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.unsplash.com/photo-1585699324551-f6c309eedeca?auto=format&fit=crop&w=600&q=80", "Palais El Badi, Marrakech", "Outdoor", "Marrakech du Rire" }
                });

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
    }
}
