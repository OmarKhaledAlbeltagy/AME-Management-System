using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accountMonthlyPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    ExtendIdentityUserId = table.Column<string>(nullable: true),
                    PlannedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountMonthlyPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountMonthlyPlan_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_accountMonthlyPlan_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contactMonthlyPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactId = table.Column<int>(nullable: false),
                    ExtendIdentityUserId = table.Column<string>(nullable: true),
                    PlannedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactMonthlyPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contactMonthlyPlan_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contactMonthlyPlan_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_AccountId",
                table: "accountMonthlyPlan",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMonthlyPlan_ExtendIdentityUserId",
                table: "accountMonthlyPlan",
                column: "ExtendIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMonthlyPlan_ContactId",
                table: "contactMonthlyPlan",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMonthlyPlan_ExtendIdentityUserId",
                table: "contactMonthlyPlan",
                column: "ExtendIdentityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accountMonthlyPlan");

            migrationBuilder.DropTable(
                name: "contactMonthlyPlan");
        }
    }
}
