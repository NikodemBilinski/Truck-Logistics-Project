using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrucksLogisticsServerAPI.Migrations
{
    /// <inheritdoc />
    public partial class JobRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientContactNumber",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AssignedUserId",
                table: "Jobs",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_AssignedUserId",
                table: "Jobs",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_AssignedUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_AssignedUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ClientContactNumber",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Jobs");
        }
    }
}
