using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Migrations
{
    /// <inheritdoc />
    public partial class EvitarPlacasIguales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NumberPlate",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_NumberPlate",
                table: "Vehicles",
                column: "NumberPlate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_NumberPlate",
                table: "Vehicles");

            migrationBuilder.AlterColumn<string>(
                name: "NumberPlate",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
