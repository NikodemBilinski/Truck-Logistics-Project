using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrucksLogisticsServerAPI.Migrations
{
    /// <inheritdoc />
    public partial class truckchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Users_UsersID",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_UsersID",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "UsersID",
                table: "Trucks");

            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "Trucks",
                newName: "brand");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserTrucks",
                columns: table => new
                {
                    AssignedTrucksId = table.Column<int>(type: "int", nullable: false),
                    AssignedUsersID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTrucks", x => new { x.AssignedTrucksId, x.AssignedUsersID });
                    table.ForeignKey(
                        name: "FK_UserTrucks_Trucks_AssignedTrucksId",
                        column: x => x.AssignedTrucksId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrucks_Users_AssignedUsersID",
                        column: x => x.AssignedUsersID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTrucks_AssignedUsersID",
                table: "UserTrucks",
                column: "AssignedUsersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTrucks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Trucks");

            migrationBuilder.RenameColumn(
                name: "brand",
                table: "Trucks",
                newName: "Owner");

            migrationBuilder.AddColumn<int>(
                name: "UsersID",
                table: "Trucks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_UsersID",
                table: "Trucks",
                column: "UsersID");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Users_UsersID",
                table: "Trucks",
                column: "UsersID",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
