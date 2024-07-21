using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    extendidentityuserid = table.Column<string>(nullable: true),
                    AccountId = table.Column<int>(nullable: false),
                    VisitDateTime = table.Column<DateTime>(nullable: false),
                    SubmittingDateTime = table.Column<DateTime>(nullable: false),
                    VisitNotes = table.Column<string>(maxLength: 1000, nullable: true),
                    AdditionalNotes = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSupportiveVisit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisit_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisit_AspNetUsers_extendidentityuserid",
                        column: x => x.extendidentityuserid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisitBrand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSupportVisitId = table.Column<int>(nullable: false),
                    accountsupportivevisitId = table.Column<int>(nullable: true),
                    BrandId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisitPerson",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonName = table.Column<string>(maxLength: 50, nullable: false),
                    PersonPosition = table.Column<string>(maxLength: 30, nullable: true),
                    AccountSupportiveVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSupportiveVisitPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitPerson_AccountSupportiveVisit_AccountSupportiveVisitId",
                        column: x => x.AccountSupportiveVisitId,
                        principalTable: "AccountSupportiveVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisit_AccountId",
                table: "AccountSupportiveVisit",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisit_extendidentityuserid",
                table: "AccountSupportiveVisit",
                column: "extendidentityuserid");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitBrand_BrandId",
                table: "AccountSupportiveVisitBrand",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitBrand_accountsupportivevisitId",
                table: "AccountSupportiveVisitBrand",
                column: "accountsupportivevisitId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitPerson_AccountSupportiveVisitId",
                table: "AccountSupportiveVisitPerson",
                column: "AccountSupportiveVisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSupportiveVisitBrand");

            migrationBuilder.DropTable(
                name: "AccountSupportiveVisitPerson");

            migrationBuilder.DropTable(
                name: "AccountSupportiveVisit");
        }
    }
}
