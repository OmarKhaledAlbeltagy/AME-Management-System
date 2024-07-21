using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x39 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSupportiveVisitBrand");

            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisitproduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSupportVisitId = table.Column<int>(nullable: false),
                    accountsupportivevisitId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSupportiveVisitproduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitproduct_product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitproduct_AccountSupportiveVisit_accountsupportivevisitId",
                        column: x => x.accountsupportivevisitId,
                        principalTable: "AccountSupportiveVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitproduct_ProductId",
                table: "AccountSupportiveVisitproduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitproduct_accountsupportivevisitId",
                table: "AccountSupportiveVisitproduct",
                column: "accountsupportivevisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSupportiveVisitproduct");

            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisitBrand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSupportVisitId = table.Column<int>(nullable: false),
                    BrandId = table.Column<int>(nullable: false),
                    accountsupportivevisitId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSupportiveVisitBrand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitBrand_brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitBrand_AccountSupportiveVisit_accountsupportivevisitId",
                        column: x => x.accountsupportivevisitId,
                        principalTable: "AccountSupportiveVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitBrand_BrandId",
                table: "AccountSupportiveVisitBrand",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitBrand_accountsupportivevisitId",
                table: "AccountSupportiveVisitBrand",
                column: "accountsupportivevisitId");
        }
    }
}
