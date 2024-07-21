using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Collection",
                table: "accountMonthlyPlan",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "accountSalesVisitCollection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSalesVisitId = table.Column<int>(nullable: false),
                    Collection = table.Column<float>(nullable: false),
                    BrandId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountSalesVisitCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountSalesVisitCollection_accountSalesVisit_AccountSalesVisitId",
                        column: x => x.AccountSalesVisitId,
                        principalTable: "accountSalesVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_accountSalesVisitCollection_brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_BrandId",
                table: "accountMonthlyPlan",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisitCollection_AccountSalesVisitId",
                table: "accountSalesVisitCollection",
                column: "AccountSalesVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisitCollection_BrandId",
                table: "accountSalesVisitCollection",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMonthlyPlan_brand_BrandId",
                table: "accountMonthlyPlan",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMonthlyPlan_brand_BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropTable(
                name: "accountSalesVisitCollection");

            migrationBuilder.DropIndex(
                name: "IX_accountMonthlyPlan_BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "accountMonthlyPlan");

            migrationBuilder.DropColumn(
                name: "Collection",
                table: "accountMonthlyPlan");
        }
    }
}
