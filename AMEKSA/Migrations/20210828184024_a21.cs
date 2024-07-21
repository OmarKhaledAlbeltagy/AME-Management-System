using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlan_brand_BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlan_BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "Collection",
                table: "accountMonthlyPlan");

            migrationBuilder.CreateTable(
                name: "accountMonthlyPlanCollection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Collection = table.Column<float>(nullable: true),
                    BrandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountMonthlyPlanCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountMonthlyPlanCollection_brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlanCollection_BrandId",
                table: "accountMonthlyPlanCollection",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accountMonthlyPlanCollection");

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Collection",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_BrandId",
                table: "accountMonthlyPlan",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlan_brand_BrandId",
                table: "accountMonthlyPlan",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
