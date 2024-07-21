using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlanCollection_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                column: "AccountMonthlyPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                column: "AccountMonthlyPlanId",
                principalTable: "accountMonthlyPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlanCollection_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.DropColumn(
                name: "AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection");
        }
    }
}
