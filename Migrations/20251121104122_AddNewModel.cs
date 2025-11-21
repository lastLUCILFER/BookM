using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookM.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_CategoryId",
                table: "Event",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Category_CategoryId",
                table: "Event",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Category_CategoryId",
                table: "Event");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Event_CategoryId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Event");
        }
    }
}
