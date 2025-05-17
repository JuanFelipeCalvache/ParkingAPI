using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Migrations
{
    /// <inheritdoc />
    public partial class CambioTablaEntryExit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeeToPaid",
                table: "EntryExits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeToPaid",
                table: "EntryExits");
        }
    }
}
