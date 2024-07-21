using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accountMedicalVisitChat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagerId = table.Column<string>(nullable: false),
                    RepId = table.Column<string>(nullable: false),
                    ManagerComment = table.Column<string>(maxLength: 500, nullable: false),
                    ManagerCommentDateTime = table.Column<DateTime>(nullable: false),
                    RepReply = table.Column<string>(maxLength: 500, nullable: true),
                    RepReplyDateTime = table.Column<DateTime>(nullable: false),
                    AccountMedicalVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountMedicalVisitChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountMedicalVisitChat_accountMedicalVisit_AccountMedicalVisitId",
                        column: x => x.AccountMedicalVisitId,
                        principalTable: "accountMedicalVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accountMedicalVisitChat_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accountMedicalVisitChat_AspNetUsers_RepId",
                        column: x => x.RepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accountSalesVisitChat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagerId = table.Column<string>(nullable: false),
                    RepId = table.Column<string>(nullable: false),
                    ManagerComment = table.Column<string>(maxLength: 500, nullable: false),
                    ManagerCommentDateTime = table.Column<DateTime>(nullable: false),
                    RepReply = table.Column<string>(maxLength: 500, nullable: true),
                    RepReplyDateTime = table.Column<DateTime>(nullable: false),
                    AccountSalesVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountSalesVisitChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountSalesVisitChat_accountSalesVisit_AccountSalesVisitId",
                        column: x => x.AccountSalesVisitId,
                        principalTable: "accountSalesVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accountSalesVisitChat_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accountSalesVisitChat_AspNetUsers_RepId",
                        column: x => x.RepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contactMedicalVisitChat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagerId = table.Column<string>(nullable: false),
                    RepId = table.Column<string>(nullable: false),
                    ManagerComment = table.Column<string>(maxLength: 500, nullable: false),
                    ManagerCommentDateTime = table.Column<DateTime>(nullable: false),
                    RepReply = table.Column<string>(maxLength: 500, nullable: true),
                    RepReplyDateTime = table.Column<DateTime>(nullable: false),
                    ContactMedicalVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactMedicalVisitChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contactMedicalVisitChat_contactMedicalVisit_ContactMedicalVisitId",
                        column: x => x.ContactMedicalVisitId,
                        principalTable: "contactMedicalVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contactMedicalVisitChat_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contactMedicalVisitChat_AspNetUsers_RepId",
                        column: x => x.RepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisitChat_AccountMedicalVisitId",
                table: "accountMedicalVisitChat",
                column: "AccountMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisitChat_ManagerId",
                table: "accountMedicalVisitChat",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisitChat_RepId",
                table: "accountMedicalVisitChat",
                column: "RepId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisitChat_AccountSalesVisitId",
                table: "accountSalesVisitChat",
                column: "AccountSalesVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisitChat_ManagerId",
                table: "accountSalesVisitChat",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisitChat_RepId",
                table: "accountSalesVisitChat",
                column: "RepId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisitChat_ContactMedicalVisitId",
                table: "contactMedicalVisitChat",
                column: "ContactMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisitChat_ManagerId",
                table: "contactMedicalVisitChat",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisitChat_RepId",
                table: "contactMedicalVisitChat",
                column: "RepId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accountMedicalVisitChat");

            migrationBuilder.DropTable(
                name: "accountSalesVisitChat");

            migrationBuilder.DropTable(
                name: "contactMedicalVisitChat");
        }
    }
}
