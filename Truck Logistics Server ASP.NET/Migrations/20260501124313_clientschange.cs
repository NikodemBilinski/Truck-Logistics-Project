using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrucksLogisticsServerAPI.Migrations
{
    /// <inheritdoc />
    public partial class clientschange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Client_ClientID",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Jobs_JobID",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Client_ClientID",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                table: "Client");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "Client",
                newName: "Clients");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_JobID",
                table: "Invoices",
                newName: "IX_Invoices_JobID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_ClientID",
                table: "Invoices",
                newName: "IX_Invoices_ClientID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Clients_ClientID",
                table: "Invoices",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Jobs_JobID",
                table: "Invoices",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientID",
                table: "Jobs",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Clients_ClientID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Jobs_JobID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientID",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Client");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_JobID",
                table: "Invoice",
                newName: "IX_Invoice_JobID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_ClientID",
                table: "Invoice",
                newName: "IX_Invoice_ClientID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                table: "Client",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Client_ClientID",
                table: "Invoice",
                column: "ClientID",
                principalTable: "Client",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Jobs_JobID",
                table: "Invoice",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Client_ClientID",
                table: "Jobs",
                column: "ClientID",
                principalTable: "Client",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
