using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlanCollection_brand_BrandId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "accountMonthlyPlanCollection",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlanCollection_brand_BrandId",
                table: "accountMonthlyPlanCollection",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlanCollection_brand_BrandId",
                table: "accountMonthlyPlanCollection");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "accountMonthlyPlanCollection",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlanCollection_brand_BrandId",
                table: "accountMonthlyPlanCollection",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
