using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryExit_Space_SpaceId",
                table: "EntryExit");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryExit_Vehicle_VehicleId",
                table: "EntryExit");

            migrationBuilder.DropForeignKey(
                name: "FK_Space_Vehicle_VehicleId",
                table: "Space");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Users_UserId",
                table: "Vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Space",
                table: "Space");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryExit",
                table: "EntryExit");

            migrationBuilder.RenameTable(
                name: "Vehicle",
                newName: "Vehicles");

            migrationBuilder.RenameTable(
                name: "Space",
                newName: "Spaces");

            migrationBuilder.RenameTable(
                name: "EntryExit",
                newName: "EntryExits");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_UserId",
                table: "Vehicles",
                newName: "IX_Vehicles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Space_VehicleId",
                table: "Spaces",
                newName: "IX_Spaces_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryExit_VehicleId",
                table: "EntryExits",
                newName: "IX_EntryExits_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryExit_SpaceId",
                table: "EntryExits",
                newName: "IX_EntryExits_SpaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryExits",
                table: "EntryExits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryExits_Spaces_SpaceId",
                table: "EntryExits",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryExits_Vehicles_VehicleId",
                table: "EntryExits",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Spaces_Vehicles_VehicleId",
                table: "Spaces",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Users_UserId",
                table: "Vehicles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryExits_Spaces_SpaceId",
                table: "EntryExits");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryExits_Vehicles_VehicleId",
                table: "EntryExits");

            migrationBuilder.DropForeignKey(
                name: "FK_Spaces_Vehicles_VehicleId",
                table: "Spaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Users_UserId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spaces",
                table: "Spaces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryExits",
                table: "EntryExits");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicle");

            migrationBuilder.RenameTable(
                name: "Spaces",
                newName: "Space");

            migrationBuilder.RenameTable(
                name: "EntryExits",
                newName: "EntryExit");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicle",
                newName: "IX_Vehicle_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Spaces_VehicleId",
                table: "Space",
                newName: "IX_Space_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryExits_VehicleId",
                table: "EntryExit",
                newName: "IX_EntryExit_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryExits_SpaceId",
                table: "EntryExit",
                newName: "IX_EntryExit_SpaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Space",
                table: "Space",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryExit",
                table: "EntryExit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryExit_Space_SpaceId",
                table: "EntryExit",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryExit_Vehicle_VehicleId",
                table: "EntryExit",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Space_Vehicle_VehicleId",
                table: "Space",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Users_UserId",
                table: "Vehicle",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
