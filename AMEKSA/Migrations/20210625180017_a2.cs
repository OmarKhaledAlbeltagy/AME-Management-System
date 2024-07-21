using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "requestDeleteAccountMedical",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountMedicalVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestDeleteAccountMedical", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestDeleteAccountMedical_accountMedicalVisit_AccountMedicalVisitId",
                        column: x => x.AccountMedicalVisitId,
                        principalTable: "accountMedicalVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "requestDeleteAccountSales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSalesVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestDeleteAccountSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestDeleteAccountSales_accountSalesVisit_AccountSalesVisitId",
                        column: x => x.AccountSalesVisitId,
                        principalTable: "accountSalesVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "requestDeleteContactMedical",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactMedicalVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestDeleteContactMedical", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestDeleteContactMedical_contactMedicalVisit_ContactMedicalVisitId",
                        column: x => x.ContactMedicalVisitId,
                        principalTable: "contactMedicalVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requestDeleteAccountMedical_AccountMedicalVisitId",
                table: "requestDeleteAccountMedical",
                column: "AccountMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_requestDeleteAccountSales_AccountSalesVisitId",
                table: "requestDeleteAccountSales",
                column: "AccountSalesVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_requestDeleteContactMedical_ContactMedicalVisitId",
                table: "requestDeleteContactMedical",
                column: "ContactMedicalVisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requestDeleteAccountMedical");

            migrationBuilder.DropTable(
                name: "requestDeleteAccountSales");

            migrationBuilder.DropTable(
                name: "requestDeleteContactMedical");
        }
    }
}
