using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class _47 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "AccountSupportiveVisit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "AccountSupportiveVisit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "accountSalesVisit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "accountSalesVisit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "accountMedicalVisit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "accountMedicalVisit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisit_ManagerId1",
                table: "AccountSupportiveVisit",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisit_ManagerId1",
                table: "accountSalesVisit",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisit_ManagerId1",
                table: "accountMedicalVisit",
                column: "ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId1",
                table: "accountMedicalVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId1",
                table: "accountSalesVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId1",
                table: "AccountSupportiveVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId1",
                table: "accountMedicalVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropIndex(
                name: "IX_AccountSupportiveVisit_ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountSalesVisit_ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountMedicalVisit_ManagerId1",
                table: "accountMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "accountSalesVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "accountMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "accountMedicalVisit");
        }
    }
}
