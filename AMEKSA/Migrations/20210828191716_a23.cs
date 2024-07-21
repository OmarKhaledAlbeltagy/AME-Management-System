using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.AlterColumn<int>(
                name: "AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                column: "AccountMonthlyPlanId",
                principalTable: "accountMonthlyPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.AlterColumn<int>(
                name: "AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlanCollection_accountMonthlyPlan_AccountMonthlyPlanId",
                table: "accountMonthlyPlanCollection",
                column: "AccountMonthlyPlanId",
                principalTable: "accountMonthlyPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
