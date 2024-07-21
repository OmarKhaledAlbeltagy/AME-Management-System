using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactMedicalVisitId",
                table: "contactMonthlyPlan",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountMedicalVisitId",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountSalesVisitId",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactMonthlyPlan_ContactMedicalVisitId",
                table: "contactMonthlyPlan",
                column: "ContactMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_AccountMedicalVisitId",
                table: "accountMonthlyPlan",
                column: "AccountMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_AccountSalesVisitId",
                table: "accountMonthlyPlan",
                column: "AccountSalesVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlan_accountMedicalVisit_AccountMedicalVisitId",
                table: "accountMonthlyPlan",
                column: "AccountMedicalVisitId",
                principalTable: "accountMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlan_accountSalesVisit_AccountSalesVisitId",
                table: "accountMonthlyPlan",
                column: "AccountSalesVisitId",
                principalTable: "accountSalesVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMonthlyPlan_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMonthlyPlan",
                column: "ContactMedicalVisitId",
                principalTable: "contactMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlan_accountMedicalVisit_AccountMedicalVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlan_accountSalesVisit_AccountSalesVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMonthlyPlan_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMonthlyPlan");

            migrationBuilder.DropIndex(
                name: "IX_contactMonthlyPlan_ContactMedicalVisitId",
                table: "contactMonthlyPlan");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlan_AccountMedicalVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlan_AccountSalesVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "ContactMedicalVisitId",
                table: "contactMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "AccountMedicalVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "AccountSalesVisitId",
                table: "accountMonthlyPlan");
        }
    }
}
