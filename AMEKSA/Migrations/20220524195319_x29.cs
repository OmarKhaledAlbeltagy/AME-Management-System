using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x29 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountSupportiveVisitId",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_AccountSupportiveVisitId",
                table: "accountMonthlyPlan",
                column: "AccountSupportiveVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlan_AccountSupportiveVisit_AccountSupportiveVisitId",
                table: "accountMonthlyPlan",
                column: "AccountSupportiveVisitId",
                principalTable: "AccountSupportiveVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlan_AccountSupportiveVisit_AccountSupportiveVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlan_AccountSupportiveVisitId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "AccountSupportiveVisitId",
                table: "accountMonthlyPlan");
        }
    }
}
